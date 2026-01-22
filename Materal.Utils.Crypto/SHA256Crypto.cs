using System.Security.Cryptography;
using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// SHA256 哈希计算工具类
/// </summary>
/// <remarks>
/// SHA256 是 SHA-2 家族的一员，目前被认为是安全的哈希算法。
/// 适用于密码存储、数字签名、文件校验等安全敏感场景。
/// 输出长度为 256 位（32 字节）。
/// </remarks>
public static partial class SHA256Crypto
{
    /// <summary>
    /// 计算64位SHA256哈希值
    /// </summary>
    /// <param name="inputStr">要计算哈希的字符串</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>64位SHA256哈希值</returns>
    /// <exception cref="ArgumentNullException">当输入字符串为null时抛出</exception>
    public static string Hash(string inputStr, bool isLower = false)
    {
#if NETSTANDARD
        if (inputStr is null) throw new ArgumentNullException(nameof(inputStr));
        byte[] data = Encoding.UTF8.GetBytes(inputStr);
        using SHA256 sha256 = SHA256.Create();
        byte[] output = sha256.ComputeHash(data);
#else
        ArgumentNullException.ThrowIfNull(inputStr);
        byte[] data = Encoding.UTF8.GetBytes(inputStr);
        byte[] output = SHA256.HashData(data);
#endif
        return FormatHash(output, isLower);
    }

    /// <summary>
    /// 计算64位SHA256哈希值（字节数组版本）
    /// </summary>
    /// <param name="data">要计算哈希的字节数组</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>64位SHA256哈希值</returns>
    /// <exception cref="ArgumentNullException">当输入字节数组为null时抛出</exception>
    public static string Hash(byte[] data, bool isLower = false)
    {
#if NETSTANDARD
        if (data is null) throw new ArgumentNullException(nameof(data));
        using SHA256 sha256 = SHA256.Create();
        byte[] output = sha256.ComputeHash(data);
#else
        ArgumentNullException.ThrowIfNull(data);
        byte[] output = SHA256.HashData(data);
#endif
        return FormatHash(output, isLower);
    }

    /// <summary>
    /// 计算64位SHA256哈希值（流版本）
    /// </summary>
    /// <param name="stream">要计算哈希的流</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>64位SHA256哈希值</returns>
    /// <exception cref="ArgumentNullException">当输入流为null时抛出</exception>
    /// <exception cref="ArgumentException">当流不可读时抛出</exception>
    public static string Hash(Stream stream, bool isLower = false)
    {
#if NETSTANDARD
        if (stream is null) throw new ArgumentNullException(nameof(stream));
        if (!stream.CanRead) throw new ArgumentException("流必须可读", nameof(stream));
        using SHA256 sha256 = SHA256.Create();
        byte[] output = sha256.ComputeHash(stream);
#else
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead) throw new ArgumentException("流必须可读", nameof(stream));
        byte[] output = SHA256.HashData(stream);
#endif
        return FormatHash(output, isLower);
    }

    /// <summary>
    /// 格式化哈希字节数组为字符串
    /// </summary>
    /// <param name="hash">哈希字节数组</param>
    /// <param name="isLower">是否返回小写格式</param>
    /// <returns>格式化的哈希字符串</returns>
    private static string FormatHash(byte[] hash, bool isLower)
    {
#if NET9_0_OR_GREATER
        string outputStr = Convert.ToHexString(hash);
#else
        string outputStr = BitConverter.ToString(hash).Replace("-", "");
#endif
        outputStr = isLower ? outputStr.ToLower() : outputStr.ToUpper();
        return outputStr;
    }
}
