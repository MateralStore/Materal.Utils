using System.Runtime.InteropServices;

namespace Materal.Utils.Windows
{
    /// <summary>
    /// 进程管理器
    /// </summary>
    public class ProcessHelper
    {
        /// <summary>
        /// 输出数据
        /// </summary>
        public event DataReceivedEventHandler? OutputDataReceived;
        /// <summary>
        /// 错误数据
        /// </summary>
        public event DataReceivedEventHandler? ErrorDataReceived;
        /// <summary>
        /// 获得进程开始信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static ProcessStartInfo GetProcessStartInfo(string cmd, string arg)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = cmd,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                Verb = "RunAs",
                Arguments = arg
            };
            return processStartInfo;
        }
        /// <summary>
        /// 启动一个进程
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="arg"></param>
        public void ProcessStart(string cmd, string arg)
        {
            ProcessStartInfo processStartInfo = GetProcessStartInfo(cmd, arg);
            using Process process = new() { StartInfo = processStartInfo };
            if (OutputDataReceived is not null)
            {
                process.OutputDataReceived += OutputDataReceived;
            }
            if (ErrorDataReceived is not null)
            {
                process.ErrorDataReceived += ErrorDataReceived;
            }
            if (process.Start())
            {
                if (OutputDataReceived is not null)
                {
                    process.BeginOutputReadLine();
                }
                if (ErrorDataReceived is not null)
                {
                    process.BeginErrorReadLine();
                }
            }
            process.StandardInput.AutoFlush = true;
            process.WaitForExit();
            process.Close();
        }
        /// <summary>
        /// 在当前活动用户会话中启动进程
        /// </summary>
        /// <param name="cmd">可执行文件路径</param>
        /// <param name="arg">命令行参数</param>
        /// <returns>启动的进程</returns>
        public void ProcessStartAsCurrentUser(string cmd, string arg)
        {
            using Process? process = StartProcessAsCurrentUser(cmd, arg);
            if (process is null) return;
            process.WaitForExit();
            process.Close();
        }
        #region 以当前用户身份启动进程的相关方法
        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUserW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string? lpApplicationName,
            string? lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string? lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx", SetLastError = true)]
        private static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            uint dwDesiredAccess,
            IntPtr lpTokenAttributes,
            int impersonationLevel,
            int tokenType,
            ref IntPtr phNewToken);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool CreateEnvironmentBlock(
            out IntPtr lpEnvironment,
            IntPtr hToken,
            bool bInherit);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQueryUserToken(uint sessionId, out IntPtr phToken);

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        private const int TOKEN_DUPLICATE = 0x0002;
        private const int TOKEN_QUERY = 0x0008;
        private const int TOKEN_ASSIGN_PRIMARY = 0x0001;
        private const int TOKEN_ADJUST_DEFAULT = 0x0080;
        private const int TOKEN_ADJUST_SESSIONID = 0x0100;
        private const uint MAXIMUM_ALLOWED = 0x2000000;

        private const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;
        private const int NORMAL_PRIORITY_CLASS = 0x00000020;
        private const int CREATE_NEW_CONSOLE = 0x00000010;

        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        /// <summary>
        /// 在当前活动用户会话中启动进程
        /// </summary>
        /// <param name="appPath">应用程序路径</param>
        /// <param name="cmdLine">命令行参数</param>
        /// <param name="workDir">工作目录</param>
        /// <param name="visible">是否可见</param>
        /// <returns>启动的进程，如果失败则返回null</returns>
        private static Process? StartProcessAsCurrentUser(string appPath, string cmdLine = "", string? workDir = null, bool visible = true)
        {
            if (string.IsNullOrEmpty(workDir))
            {
                workDir = Path.GetDirectoryName(appPath);
            }
            // 获取当前活动会话ID
            uint sessionId = WTSGetActiveConsoleSessionId();

            // 获取会话令牌
            if (!WTSQueryUserToken(sessionId, out IntPtr hSessionToken))
            {
                int error = Marshal.GetLastWin32Error();
                Console.WriteLine($"WTSQueryUserToken failed with error: {error}");
                return null;
            }

            try
            {
                // 复制令牌
                IntPtr hPrimaryToken = IntPtr.Zero;
                IntPtr lpEnvironment = IntPtr.Zero;
                try
                {
                    uint tokenRights = TOKEN_DUPLICATE | TOKEN_QUERY | TOKEN_ASSIGN_PRIMARY | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID;
                    if (!DuplicateTokenEx(hSessionToken, tokenRights, IntPtr.Zero, 2, 1, ref hPrimaryToken))
                    {
                        int error = Marshal.GetLastWin32Error();
                        Console.WriteLine($"DuplicateTokenEx failed with error: {error}");
                        return null;
                    }
                    // 创建环境块
                    if (!CreateEnvironmentBlock(out lpEnvironment, hPrimaryToken, false))
                    {
                        int error = Marshal.GetLastWin32Error();
                        Console.WriteLine($"CreateEnvironmentBlock failed with error: {error}");
                        return null;
                    }
                    // 设置启动信息
                    STARTUPINFO startupInfo = new();
                    startupInfo.cb = Marshal.SizeOf(startupInfo);
                    startupInfo.lpDesktop = "winsta0\\default";

                    // 准备命令行
                    string fullCommand = $"\"{appPath}\" {cmdLine}";

                    // 创建进程
                    uint creationFlags = NORMAL_PRIORITY_CLASS | CREATE_UNICODE_ENVIRONMENT;
                    if (!visible)
                        creationFlags |= CREATE_NEW_CONSOLE;

                    PROCESS_INFORMATION processInfo = new();
                    bool result = CreateProcessAsUser(
                        hPrimaryToken,
                        null,
                        fullCommand,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        false,
                        creationFlags,
                        lpEnvironment,
                        workDir,
                        ref startupInfo,
                        out processInfo);

                    if (!result)
                    {
                        int error = Marshal.GetLastWin32Error();
                        Console.WriteLine($"CreateProcessAsUser failed with error: {error}");
                        return null;
                    }

                    // 创建.NET进程对象
                    Process process = Process.GetProcessById((int)processInfo.dwProcessId);
                    // 关闭句柄
                    CloseHandle(processInfo.hProcess);
                    CloseHandle(processInfo.hThread);

                    return process;
                }
                finally
                {
                    // 清理资源
                    if (lpEnvironment != IntPtr.Zero)
                    {
                        DestroyEnvironmentBlock(lpEnvironment);
                    }
                    if (hPrimaryToken != IntPtr.Zero)
                    {
                        CloseHandle(hPrimaryToken);
                    }
                }
            }
            finally
            {
                if (hSessionToken != IntPtr.Zero)
                {
                    CloseHandle(hSessionToken);
                }
            }
        }
        #endregion
    }
}
