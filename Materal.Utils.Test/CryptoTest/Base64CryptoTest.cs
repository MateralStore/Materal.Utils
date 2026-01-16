using Materal.Utils.Crypto;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// Base64Crypto 测试类
/// </summary>
[TestClass]
public class Base64CryptoTest
{
    private const string TestString = "Hello, World! 你好世界！";
    private readonly string _expectedBase64 = "SGVsbG8sIFdvcmxkISDkvaDlpb3kuJbnlYzvvIE=";

    static Base64CryptoTest()
    {
        // 注册编码提供程序以支持GBK等编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// 测试字符串编码
    /// </summary>
    [TestMethod]
    public void Encode_String_Test()
    {
        // Act
        string result = Base64Crypto.Encode(TestString);

        // Assert
        Assert.AreEqual(_expectedBase64, result);
    }

    /// <summary>
    /// 测试字符串编码（指定编码）
    /// </summary>
    [TestMethod]
    public void Encode_String_WithEncoding_Test()
    {
        // Arrange
        string gbkTestString = "你好世界";
        Encoding gbkEncoding = Encoding.GetEncoding("GBK");

        // Act
        string result = Base64Crypto.Encode(gbkTestString, gbkEncoding);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(string.Empty, result);
    }

    /// <summary>
    /// 测试字符串解码
    /// </summary>
    [TestMethod]
    public void Decode_String_Test()
    {
        // Act
        string result = Base64Crypto.Decode(_expectedBase64);

        // Assert
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试字符串解码（指定编码）
    /// </summary>
    [TestMethod]
    public void Decode_String_WithEncoding_Test()
    {
        // Arrange
        string gbkTestString = "你好世界";
        Encoding gbkEncoding = Encoding.GetEncoding("GBK");
        string gbkBase64 = Base64Crypto.Encode(gbkTestString, gbkEncoding);

        // Act
        string result = Base64Crypto.Decode(gbkBase64, gbkEncoding);

        // Assert
        Assert.AreEqual(gbkTestString, result);
    }

    /// <summary>
    /// 测试编码解码往返一致性
    /// </summary>
    [TestMethod]
    public void EncodeDecode_RoundTrip_Test()
    {
        // Arrange
        string originalText = "这是一个测试字符串，包含中文和 English! 123!@#";

        // Act
        string encoded = Base64Crypto.Encode(originalText);
        string decoded = Base64Crypto.Decode(encoded);

        // Assert
        Assert.AreEqual(originalText, decoded);
    }

    /// <summary>
    /// 测试空字符串
    /// </summary>
    [TestMethod]
    public void EmptyString_Test()
    {
        // Act & Assert
        Assert.AreEqual(string.Empty, Base64Crypto.Encode(""));
        Assert.AreEqual(string.Empty, Base64Crypto.Decode(""));
    }

    /// <summary>
    /// 测试无效 Base64 字符串解码
    /// </summary>
    [TestMethod]
    public void Decode_InvalidBase64_Test()
    {
        // Arrange
        string invalidBase64 = "Invalid Base64 String!@#$";

        // Act & Assert
        Assert.ThrowsExactly<FormatException>(() => Base64Crypto.Decode(invalidBase64));
    }

    /// <summary>
    /// 测试 null 参数
    /// </summary>
    [TestMethod]
    public void NullParameter_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => Base64Crypto.Encode(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => Base64Crypto.Decode(null!));
    }
}
