namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// AES 加密解密工具测试类
/// 测试 AES-CBC 和 AES-GCM 两种模式的加密解密功能
/// </summary>
[TestClass]
public partial class AesCryptoTest
{
    /// <summary>
    /// 测试内容
    /// </summary>
    private const string TestContent = "这是一个测试内容，用于验证AES加解密功能！Hello World! 123456789";
    /// <summary>
    /// 测试内容Byte数组
    /// </summary>
    private readonly byte[] _testBytes = Encoding.UTF8.GetBytes(TestContent);
}