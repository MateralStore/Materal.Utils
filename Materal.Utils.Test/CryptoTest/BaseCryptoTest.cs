using Materal.Utils.Crypto;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// BaseCrypto 测试类
/// </summary>
[TestClass]
public class BaseCryptoTest
{
    private const string TestString = "Hello, World! 你好世界！";
    private const string EmptyString = "";

    /// <summary>
    /// 测试二进制编码
    /// </summary>
    [TestMethod]
    public void EncodeBinary_Test()
    {
        // Act
        string result = BaseCrypto.EncodeBinary(TestString);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(string.Empty, result);
        // 二进制字符串应该只包含 0 和 1
        Assert.IsTrue(result.All(c => c == '0' || c == '1'));
    }

    /// <summary>
    /// 测试二进制解码
    /// </summary>
    [TestMethod]
    public void DecodeBinary_Test()
    {
        // Arrange
        string binaryEncoded = BaseCrypto.EncodeBinary(TestString);

        // Act
        string result = BaseCrypto.DecodeBinary(binaryEncoded);

        // Assert
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试十进制编码
    /// </summary>
    [TestMethod]
    public void EncodeDecimalism_Test()
    {
        // Act
        string result = BaseCrypto.EncodeDecimalism(TestString);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(string.Empty, result);
        // 十进制字符串应该只包含数字 0-9
        Assert.IsTrue(result.All(char.IsDigit));
    }

    /// <summary>
    /// 测试十进制解码
    /// </summary>
    [TestMethod]
    public void DecodeDecimalism_Test()
    {
        // Arrange
        string decimalEncoded = BaseCrypto.EncodeDecimalism(TestString);

        // Act
        string result = BaseCrypto.DecodeDecimalism(decimalEncoded);

        // Assert
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试十六进制编码
    /// </summary>
    [TestMethod]
    public void EncodeHex_Test()
    {
        // Act
        string result = BaseCrypto.EncodeHex(TestString);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(string.Empty, result);
        // 十六进制字符串应该只包含 0-9 和 A-F
        Assert.IsTrue(result.All(c => char.IsDigit(c) || (c >= 'A' && c <= 'F')));
    }

    /// <summary>
    /// 测试十六进制解码
    /// </summary>
    [TestMethod]
    public void DecodeHex_Test()
    {
        // Arrange
        string hexEncoded = BaseCrypto.EncodeHex(TestString);

        // Act
        string result = BaseCrypto.DecodeHex(hexEncoded);

        // Assert
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试自定义字符集编码
    /// </summary>
    [TestMethod]
    public void Encode_CustomDict_Test()
    {
        // Arrange
        string customDict = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Act
        string result = BaseCrypto.Encode(TestString, customDict);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(string.Empty, result);
        // 结果应该只包含自定义字符集中的字符
        Assert.IsTrue(result.All(customDict.Contains));
    }

    /// <summary>
    /// 测试自定义字符集解码
    /// </summary>
    [TestMethod]
    public void Decode_CustomDict_Test()
    {
        // Arrange
        string customDict = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string encoded = BaseCrypto.Encode(TestString, customDict);

        // Act
        string result = BaseCrypto.Decode(encoded, customDict);

        // Assert
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试编码解码往返一致性
    /// </summary>
    [TestMethod]
    public void EncodeDecode_RoundTrip_Binary_Test()
    {
        // Arrange
        string originalText = "这是一个测试字符串，包含中文和 English! 123!@#";

        // Act
        string encoded = BaseCrypto.EncodeBinary(originalText);
        string decoded = BaseCrypto.DecodeBinary(encoded);

        // Assert
        Assert.AreEqual(originalText, decoded);
    }

    /// <summary>
    /// 测试编码解码往返一致性
    /// </summary>
    [TestMethod]
    public void EncodeDecode_RoundTrip_Decimalism_Test()
    {
        // Arrange
        string originalText = "这是一个测试字符串，包含中文和 English! 123!@#";

        // Act
        string encoded = BaseCrypto.EncodeDecimalism(originalText);
        string decoded = BaseCrypto.DecodeDecimalism(encoded);

        // Assert
        Assert.AreEqual(originalText, decoded);
    }

    /// <summary>
    /// 测试编码解码往返一致性
    /// </summary>
    [TestMethod]
    public void EncodeDecode_RoundTrip_Hex_Test()
    {
        // Arrange
        string originalText = "这是一个测试字符串，包含中文和 English! 123!@#";

        // Act
        string encoded = BaseCrypto.EncodeHex(originalText);
        string decoded = BaseCrypto.DecodeHex(encoded);

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
        Assert.AreEqual(string.Empty, BaseCrypto.EncodeBinary(EmptyString));
        Assert.AreEqual(string.Empty, BaseCrypto.DecodeBinary(EmptyString));
        Assert.AreEqual(string.Empty, BaseCrypto.EncodeDecimalism(EmptyString));
        Assert.AreEqual(string.Empty, BaseCrypto.DecodeDecimalism(EmptyString));
        Assert.AreEqual(string.Empty, BaseCrypto.EncodeHex(EmptyString));
        Assert.AreEqual(string.Empty, BaseCrypto.DecodeHex(EmptyString));
    }

    /// <summary>
    /// 测试无效字符解码
    /// </summary>
    [TestMethod]
    public void Decode_InvalidCharacter_Test()
    {
        // Arrange
        string invalidBinary = "01012";
        string invalidDecimal = "0123456789a";
        string invalidHex = "0123456789ABCDEFZ";

        // Act & Assert
        Assert.ThrowsExactly<FormatException>(() => BaseCrypto.DecodeBinary(invalidBinary));
        Assert.ThrowsExactly<FormatException>(() => BaseCrypto.DecodeDecimalism(invalidDecimal));
        Assert.ThrowsExactly<FormatException>(() => BaseCrypto.DecodeHex(invalidHex));
    }

    /// <summary>
    /// 测试空字符集
    /// </summary>
    [TestMethod]
    public void EmptyDict_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Encode(TestString, ""));
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Encode(TestString, []));
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Decode("test", ""));
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Decode("test", []));
    }

    /// <summary>
    /// 测试 null 参数
    /// </summary>
    [TestMethod]
    public void NullParameter_Test()
    {
        // Act & Assert - 编码时 null 返回空字符串
        Assert.AreEqual(string.Empty, BaseCrypto.EncodeBinary(null!));
        Assert.AreEqual(string.Empty, BaseCrypto.EncodeDecimalism(null!));
        Assert.AreEqual(string.Empty, BaseCrypto.EncodeHex(null!));
        Assert.AreEqual(string.Empty, BaseCrypto.Encode(null!, "abcd"));
        Assert.AreEqual(string.Empty, BaseCrypto.Encode(null!, 'a', 'b', 'c', 'd'));

        // 解码时 null 返回空字符串
        Assert.AreEqual(string.Empty, BaseCrypto.DecodeBinary(null!));
        Assert.AreEqual(string.Empty, BaseCrypto.DecodeDecimalism(null!));
        Assert.AreEqual(string.Empty, BaseCrypto.DecodeHex(null!));
        Assert.AreEqual(string.Empty, BaseCrypto.Decode(null!, "abcd"));
        Assert.AreEqual(string.Empty, BaseCrypto.Decode(null!, 'a', 'b', 'c', 'd'));
    }

    /// <summary>
    /// 测试特殊字符编码解码
    /// </summary>
    [TestMethod]
    public void SpecialCharacters_Test()
    {
        // Arrange
        string specialText = "\n\r\t\u0000\u00FF测试";

        // Act & Assert
        string binaryEncoded = BaseCrypto.EncodeBinary(specialText);
        string binaryDecoded = BaseCrypto.DecodeBinary(binaryEncoded);
        Assert.AreEqual(specialText, binaryDecoded);

        string decimalEncoded = BaseCrypto.EncodeDecimalism(specialText);
        string decimalDecoded = BaseCrypto.DecodeDecimalism(decimalEncoded);
        Assert.AreEqual(specialText, decimalDecoded);

        string hexEncoded = BaseCrypto.EncodeHex(specialText);
        string hexDecoded = BaseCrypto.DecodeHex(hexEncoded);
        Assert.AreEqual(specialText, hexDecoded);
    }

    /// <summary>
    /// 测试自定义字符集编码解码
    /// </summary>
    [TestMethod]
    public void CustomDict_RoundTrip_Test()
    {
        // Arrange
        string originalText = "这是一个测试字符串，包含中文和 English! 123!@#";
        string customDict = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";

        // Act
        string encoded = BaseCrypto.Encode(originalText, customDict);
        string decoded = BaseCrypto.Decode(encoded, customDict);

        // Assert
        Assert.AreEqual(originalText, decoded);
        // 验证所有字符都在自定义字符集中
        Assert.IsTrue(encoded.All(customDict.Contains));
    }

    /// <summary>
    /// 测试使用中文作为自定义字符集
    /// </summary>
    [TestMethod]
    public void ChineseDict_RoundTrip_Test()
    {
        // Arrange
        string originalText = "Hello世界! 123测试";
        string chineseDict = "富强民主文明和谐自由平等公正法治爱国敬业诚信友善";

        // Act
        string encoded = BaseCrypto.Encode(originalText, chineseDict);
        string decoded = BaseCrypto.Decode(encoded, chineseDict);

        // Assert
        Assert.AreEqual(originalText, decoded);
        // 验证所有字符都在中文字符集中
        Assert.IsTrue(encoded.All(chineseDict.Contains));

        // 验证编码结果不包含原始文本中的字符（证明确实进行了编码转换）
        Assert.IsFalse(encoded.Any(c => originalText.Contains(c) && char.IsLetterOrDigit(c)));
    }

    /// <summary>
    /// 测试单个字符编码
    /// </summary>
    [TestMethod]
    public void SingleCharacter_Test()
    {
        // Arrange
        string singleChar = "A";

        // Act & Assert
        string binaryEncoded = BaseCrypto.EncodeBinary(singleChar);
        Assert.AreEqual(8, binaryEncoded.Length); // 单个字符的二进制应该是8位

        string decoded = BaseCrypto.DecodeBinary(binaryEncoded);
        Assert.AreEqual(singleChar, decoded);
    }

    /// <summary>
    /// 测试使用包含重复字符的字符集编码应该抛出异常
    /// </summary>
    [TestMethod]
    public void Encode_WithDuplicateDict_ShouldThrowException_Test()
    {
        // Arrange
        string plainText = "Hello";
        string duplicateDict = "0123456789ABCDEFABCDEF"; // 包含重复的 A-F

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Encode(plainText, duplicateDict));
    }

    /// <summary>
    /// 测试使用包含重复字符的字符集解码应该抛出异常
    /// </summary>
    [TestMethod]
    public void Decode_WithDuplicateDict_ShouldThrowException_Test()
    {
        // Arrange
        string duplicateDict = "0123456789ABCDEFABCDEF"; // 包含重复的 A-F

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Decode("test", duplicateDict));
    }

    /// <summary>
    /// 测试字符集映射的行为
    /// </summary>
    [TestMethod]
    public void DictMapping_Behavior_Test()
    {
        // Arrange
        string duplicateDict = "ABCABC"; // A、B、C 各重复两次

        // Act & Assert
        // 编码应该抛出异常
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Encode("Test", duplicateDict));

        // 解码也应该抛出异常
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Decode("test", duplicateDict));
    }

    /// <summary>
    /// 测试极端情况 - 所有字符都相同
    /// </summary>
    [TestMethod]
    public void AllSameChar_Dict_ShouldThrowException_Test()
    {
        // Arrange
        string sameCharDict = "AAAAAA"; // 所有字符都是 A

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Encode("Test", sameCharDict));
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Decode("test", sameCharDict));
    }

    /// <summary>
    /// 测试极端情况 - 字符集只有一个
    /// </summary>
    [TestMethod]
    public void SingleCharDict_ShouldThrowException_Test()
    {
        // Arrange
        string sameCharDict = "A";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => BaseCrypto.Encode("Test", sameCharDict));
        Assert.ThrowsExactly<DivideByZeroException>(() => BaseCrypto.Decode("test", sameCharDict));
    }

    /// <summary>
    /// 测试社会主义核心价值观便捷方法
    /// </summary>
    [TestMethod]
    public void CoreSocialistValues_ConvenienceMethods_Test()
    {
        // Arrange
        string originalText = "Hello世界! 123测试";

        // Act
        string encoded = BaseCrypto.EncodeCoreSocialistValues(originalText);
        string decoded = BaseCrypto.DecodeCoreSocialistValues(encoded);

        // Assert
        Assert.AreEqual(originalText, decoded);
        // 验证所有字符都在社会主义核心价值观字符集中
        Assert.IsTrue(encoded.All("富强民主文明和谐自由平等公正法治爱国敬业诚信友善".Contains));

        // 验证编码结果不包含原始文本中的字符（证明确实进行了编码转换）
        Assert.IsFalse(encoded.Any(c => originalText.Contains(c) && char.IsLetterOrDigit(c)));
    }
}
