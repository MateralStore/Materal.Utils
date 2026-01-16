using System.Text;
using Materal.Utils.Crypto;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// URL 编码解码测试
/// </summary>
[TestClass]
public class UrlCryptoTest
{
    /// <summary>
    /// 测试 URL 编码
    /// </summary>
    [TestMethod]
    public void Encode_String_Test()
    {
        // Arrange
        string plainText = "Hello World! 测试";
        
        // Act
        string encoded = UrlCrypto.Encode(plainText);
        
        // Assert
        Assert.IsNotNull(encoded);
        Assert.AreNotEqual(plainText, encoded);
        Assert.Contains("%", encoded);
    }

    /// <summary>
    /// 测试 URL 编码解码
    /// </summary>
    [TestMethod]
    public void Encode_Decode_String_Test()
    {
        // Arrange
        string plainText = "Hello World! 测试@#$%";
        
        // Act
        string encoded = UrlCrypto.Encode(plainText);
        string decoded = UrlCrypto.Decode(encoded);
        
        // Assert
        Assert.AreEqual(plainText, decoded);
    }

    /// <summary>
    /// 测试 URL 编码（指定编码）
    /// </summary>
    [TestMethod]
    public void Encode_String_WithEncoding_Test()
    {
        // Arrange
        string plainText = "测试中文";
        Encoding encoding = Encoding.UTF8;
        
        // Act
        string encoded = UrlCrypto.Encode(plainText, encoding);
        string decoded = UrlCrypto.Decode(encoded, encoding);
        
        // Assert
        Assert.AreEqual(plainText, decoded);
    }

    /// <summary>
    /// 测试 URL 路径编码
    /// </summary>
    [TestMethod]
    public void PathEncode_String_Test()
    {
        // Arrange
        string path = "/api/test/测试文件.txt";
        
        // Act
        string encoded = UrlCrypto.PathEncode(path);
        
        // Assert
        Assert.IsNotNull(encoded);
        Assert.StartsWith("/", encoded);
        Assert.Contains("%", encoded);
    }

    /// <summary>
    /// 测试 URL 编码（保留空格）
    /// </summary>
    [TestMethod]
    public void EncodeNoSpace_String_Test()
    {
        // Arrange
        string plainText = "Hello World Test";
        
        // Act
        string encoded = UrlCrypto.EncodeNoSpace(plainText);
        
        // Assert
        Assert.IsNotNull(encoded);
        Assert.Contains("%20", encoded);
        Assert.DoesNotContain("+", encoded);
    }

    /// <summary>
    /// 测试 URL 参数编码
    /// </summary>
    [TestMethod]
    public void EncodeParameters_Dictionary_Test()
    {
        // Arrange
        var parameters = new Dictionary<string, string>
        {
            ["name"] = "张三",
            ["age"] = "25",
            ["city"] = "北京"
        };
        
        // Act
        string encoded = UrlCrypto.EncodeParameters(parameters);
        
        // Assert
        Assert.IsNotNull(encoded);
        Assert.Contains("=", encoded);
        Assert.Contains("&", encoded);
        Assert.Contains("%", encoded);
    }

    /// <summary>
    /// 测试 URL 参数解码
    /// </summary>
    [TestMethod]
    public void DecodeParameters_String_Test()
    {
        // Arrange
        string queryString = "name=%E5%BC%A0%E4%B8%89&age=25&city=%E5%8C%97%E4%BA%AC";

        // Act
        Dictionary<string, string> parameters = UrlCrypto.DecodeParameters(queryString);
        
        // Assert
        Assert.HasCount(3, parameters);
        Assert.AreEqual("张三", parameters["name"]);
        Assert.AreEqual("25", parameters["age"]);
        Assert.AreEqual("北京", parameters["city"]);
    }

    /// <summary>
    /// 测试 URL 参数解码（带问号）
    /// </summary>
    [TestMethod]
    public void DecodeParameters_WithQuestionMark_Test()
    {
        // Arrange
        string queryString = "?name=test&value=123";

        // Act
        Dictionary<string, string> parameters = UrlCrypto.DecodeParameters(queryString);
        
        // Assert
        Assert.HasCount(2, parameters);
        Assert.AreEqual("test", parameters["name"]);
        Assert.AreEqual("123", parameters["value"]);
    }

    /// <summary>
    /// 测试 Base64 URL 安全编码
    /// </summary>
    [TestMethod]
    public void Base64UrlEncode_String_Test()
    {
        // Arrange
        string base64Text = "SGVsbG8gV29ybGQ+VGVzdA==";
        
        // Act
        string urlSafe = UrlCrypto.Base64UrlEncode(base64Text);
        
        // Assert
        Assert.DoesNotContain("+", urlSafe);
        Assert.DoesNotContain("/", urlSafe);
        Assert.DoesNotEndWith("=", urlSafe);
        Assert.Contains("-", urlSafe);
    }

    /// <summary>
    /// 测试 Base64 URL 安全解码
    /// </summary>
    [TestMethod]
    public void Base64UrlDecode_String_Test()
    {
        // Arrange
        string urlSafeBase64 = "SGVsbG8gV29ybGQ-VGVzdA";
        
        // Act
        string base64Text = UrlCrypto.Base64UrlDecode(urlSafeBase64);
        
        // Assert
        Assert.Contains("+", base64Text);
        Assert.EndsWith("=", base64Text);
    }

    /// <summary>
    /// 测试 Base64 URL 安全编码解码循环
    /// </summary>
    [TestMethod]
    public void Base64UrlEncode_Decode_Cycle_Test()
    {
        // Arrange
        string originalBase64 = "SGVsbG8gV29ybGQ+VGVzdA==";
        
        // Act
        string urlEncoded = UrlCrypto.Base64UrlEncode(originalBase64);
        string decoded = UrlCrypto.Base64UrlDecode(urlEncoded);
        
        // Assert
        Assert.AreEqual(originalBase64, decoded);
    }

    /// <summary>
    /// 测试空字符串处理
    /// </summary>
    [TestMethod]
    public void EmptyString_Test()
    {
        // Arrange
        string empty = string.Empty;
        
        // Act & Assert
        Assert.AreEqual(empty, UrlCrypto.Encode(empty));
        Assert.AreEqual(empty, UrlCrypto.Decode(empty));

        Dictionary<string, string> emptyParams = UrlCrypto.DecodeParameters(empty);
        Assert.IsEmpty(emptyParams);
    }

    /// <summary>
    /// 测试 null 值处理
    /// </summary>
    [TestMethod]
    public void NullString_Test()
    {
        // Act
        UrlCrypto.Encode(null!);
    }
}
