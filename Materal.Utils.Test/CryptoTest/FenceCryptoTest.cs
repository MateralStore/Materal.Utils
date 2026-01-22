using Materal.Utils.Crypto;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// FenceCrypto 测试类
/// </summary>
[TestClass]
public class FenceCryptoTest
{
    private const string TestString = "HELLO WORLD";
    private const string TestStringEven = "ABCDEFGH";
    private const string TestStringOdd = "ABCDEFG";
    private const string TestStringWithSpaces = "HELLO WORLD TEST";
    private const string TestStringWithNumbers = "HELLO123WORLD";
    private const string TestStringEmpty = "";
    private const string TestStringSingle = "A";

    #region 基础功能测试

    /// <summary>
    /// 测试2栏加密基本功能
    /// </summary>
    [TestMethod]
    public void Encrypt_TwoRails_Basic_Test()
    {
        // Act
        string result = FenceCrypto.Encrypt(TestString);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(TestString, result);
        Assert.AreEqual("HLOWRDEL OL", result);
    }

    /// <summary>
    /// 测试2栏解密基本功能
    /// </summary>
    [TestMethod]
    public void Decode_TwoRails_Basic_Test()
    {
        // Arrange
        string encoded = "HLOWRDEL OL";

        // Act
        string result = FenceCrypto.Decode(encoded);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试加密解密往返一致性
    /// </summary>
    [TestMethod]
    public void Encrypt_Decode_RoundTrip_Test()
    {
        // Act
        string encoded = FenceCrypto.Encrypt(TestString);
        string decoded = FenceCrypto.Decode(encoded);

        // Assert
        Assert.AreEqual(TestString, decoded);
    }

    #endregion

    #region 多栏加密测试

    /// <summary>
    /// 测试3栏加密
    /// </summary>
    [TestMethod]
    public void Encrypt_ThreeRails_Test()
    {
        // Act
        string result = FenceCrypto.Encrypt(TestString, 3);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(TestString, result);
        Assert.AreEqual("HOREL OLLWD", result);
    }

    /// <summary>
    /// 测试3栏解密
    /// </summary>
    [TestMethod]
    public void Decode_ThreeRails_Test()
    {
        // Arrange
        string encoded = "HOREL OLLWD";

        // Act
        string result = FenceCrypto.Decode(encoded, 3);

        // Assert
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试4栏加密
    /// </summary>
    [TestMethod]
    public void Encrypt_FourRails_Test()
    {
        // Act
        string result = FenceCrypto.Encrypt(TestString, 4);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("HWE OLORDLL", result);
    }

    /// <summary>
    /// 测试4栏解密
    /// </summary>
    [TestMethod]
    public void Decode_FourRails_Test()
    {
        // Arrange
        string encoded = "HWE OLORDLL";

        // Act
        string result = FenceCrypto.Decode(encoded, 4);

        // Assert
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试最大栏数加密（字符串长度）
    /// </summary>
    [TestMethod]
    public void Encrypt_MaxRails_Test()
    {
        // Act
        string result = FenceCrypto.Encrypt(TestStringEven, 8);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("ABCDEFGH", result); // 最大栏数时应该保持不变
    }

    #endregion

    #region 边界情况测试

    /// <summary>
    /// 测试偶数长度字符串
    /// </summary>
    [TestMethod]
    public void Encrypt_EvenLength_Test()
    {
        // Act
        string result = FenceCrypto.Encrypt(TestStringEven);

        // Assert
        Assert.AreEqual("ACEGBDFH", result);
    }

    /// <summary>
    /// 测试奇数长度字符串
    /// </summary>
    [TestMethod]
    public void Encrypt_OddLength_Test()
    {
        // Act
        string result = FenceCrypto.Encrypt(TestStringOdd);

        // Assert
        Assert.AreEqual("ACEGBDF", result);
    }

    /// <summary>
    /// 测试包含数字的字符串
    /// </summary>
    [TestMethod]
    public void Encrypt_WithNumbers_Test()
    {
        // Act
        string result = FenceCrypto.Encrypt(TestStringWithNumbers);

        // Assert
        Assert.AreEqual("HLO2WRDEL13OL", result);
    }

    /// <summary>
    /// 测试包含多个空格的字符串
    /// </summary>
    [TestMethod]
    public void Encrypt_WithMultipleSpaces_Test()
    {
        // Act
        string result = FenceCrypto.Encrypt(TestStringWithSpaces);

        // Assert
        Assert.AreEqual("HLOWRDTSEL OL ET", result);
    }

    #endregion

    #region 异常情况测试

    /// <summary>
    /// 测试null输入异常
    /// </summary>
    [TestMethod]
    public void Encrypt_NullInput_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => FenceCrypto.Encrypt(null!));
    }

    /// <summary>
    /// 测试空字符串异常
    /// </summary>
    [TestMethod]
    public void Encrypt_EmptyInput_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => FenceCrypto.Encrypt(TestStringEmpty));
    }

    /// <summary>
    /// 测试栏数为1的异常
    /// </summary>
    [TestMethod]
    public void Encrypt_RailsEqualsOne_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => FenceCrypto.Encrypt(TestString, 1));
    }

    /// <summary>
    /// 测试栏数为0的异常
    /// </summary>
    [TestMethod]
    public void Encrypt_RailsEqualsZero_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => FenceCrypto.Encrypt(TestString, 0));
    }

    /// <summary>
    /// 测试栏数大于字符串长度的异常
    /// </summary>
    [TestMethod]
    public void Encrypt_RailsGreaterThanLength_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => FenceCrypto.Encrypt(TestString, 20));
    }

    /// <summary>
    /// 测试单字符字符串
    /// </summary>
    [TestMethod]
    public void Encrypt_SingleChar_Test()
    {
        // Act & Assert - 单字符字符串无法使用栅栏加密，因为栏数必须大于1
        Assert.ThrowsExactly<ArgumentException>(() => FenceCrypto.Encrypt(TestStringSingle, 1));
    }

    #endregion

    #region 解密异常测试

    /// <summary>
    /// 测试解密null输入异常
    /// </summary>
    [TestMethod]
    public void Decode_NullInput_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => FenceCrypto.Decode(null!));
    }

    /// <summary>
    /// 测试解密空字符串异常
    /// </summary>
    [TestMethod]
    public void Decode_EmptyInput_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => FenceCrypto.Decode(TestStringEmpty));
    }

    /// <summary>
    /// 测试解密栏数为1的异常
    /// </summary>
    [TestMethod]
    public void Decode_RailsEqualsOne_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => FenceCrypto.Decode(TestString, 1));
    }

    #endregion

    #region 性能测试

    /// <summary>
    /// 测试长字符串性能
    /// </summary>
    [TestMethod]
    public void Encrypt_LongString_Performance_Test()
    {
        // Arrange
        string longString = new('A', 1000);

        // Act
        var startTime = DateTime.Now;
        string result = FenceCrypto.Encrypt(longString);
        var endTime = DateTime.Now;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsLessThan(100, (endTime - startTime).TotalMilliseconds); // 应该在100ms内完成
    }

    /// <summary>
    /// 测试长字符串多栏加密性能
    /// </summary>
    [TestMethod]
    public void Encrypt_LongString_MultipleRails_Performance_Test()
    {
        // Arrange
        string longString = new('A', 1000);

        // Act
        var startTime = DateTime.Now;
        string result = FenceCrypto.Encrypt(longString, 5);
        var endTime = DateTime.Now;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsLessThan(100, (endTime - startTime).TotalMilliseconds); // 应该在100ms内完成
    }

    #endregion

    #region 特殊情况测试

    /// <summary>
    /// 测试全是空格的字符串
    /// </summary>
    [TestMethod]
    public void Encrypt_AllSpaces_Test()
    {
        // Arrange
        string spacesString = "     ";

        // Act
        string result = FenceCrypto.Encrypt(spacesString);

        // Assert
        Assert.AreEqual("     ", result); // 全空格字符串加密后保持不变
    }

    /// <summary>
    /// 测试全是相同字符的字符串
    /// </summary>
    [TestMethod]
    public void Encrypt_AllSameChars_Test()
    {
        // Arrange
        string sameCharsString = "AAAAAA";

        // Act
        string result = FenceCrypto.Encrypt(sameCharsString);

        // Assert
        Assert.AreEqual("AAAAAA", result); // 全相同字符字符串加密后保持不变
    }

    /// <summary>
    /// 测试中文字符串
    /// </summary>
    [TestMethod]
    public void Encrypt_ChineseChars_Test()
    {
        // Arrange
        string chineseString = "你好世界";

        // Act
        string result = FenceCrypto.Encrypt(chineseString);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(chineseString, result);

        // 测试往返
        string decoded = FenceCrypto.Decode(result);
        Assert.AreEqual(chineseString, decoded);
    }

    #endregion
}
