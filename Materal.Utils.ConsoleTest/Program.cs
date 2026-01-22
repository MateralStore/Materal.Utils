using Materal.Utils.Crypto;

namespace Materal.Utils.ConsoleTest;

internal class Program
{
    static async Task Main(string[] args)
    {
        ConsoleQueue.WriteLine("测试结束，按任意键退出", ConsoleColor.Yellow);
        await ConsoleQueue.ShutdownAsync();
        Console.ReadKey();
    }
}
