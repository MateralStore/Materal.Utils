using Materal.Utils.Crypto;
using Materal.Extensions;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// DisplacementCrypto 测试类
/// </summary>
[TestClass]
public class DisplacementCryptoTest
{
    private const string TestString = "HELLO WORLD";
    private const string TestStringWithLowerCase = "Hello World";
    private const string TestStringWithNumbers = "HELLO123";
    private const string TestStringWithSpaces = "HELLO WORLD TEST";
    private const string TestStringWithSpecialChars = "HELLO!@#";

    /// <summary>
    /// 测试基本加密功能
    /// </summary>
    [TestMethod]
    public void Encrypt_Basic_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt(TestString);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(TestString, result);
        Assert.AreEqual("KHOOR ZRUOG", result);
    }

    /// <summary>
    /// 测试自定义位移量加密
    /// </summary>
    [TestMethod]
    public void Encrypt_CustomKey_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt(TestString, 5);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("MJQQT BTWQI", result);
    }

    /// <summary>
    /// 测试负位移量加密
    /// </summary>
    [TestMethod]
    public void Encrypt_NegativeKey_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt(TestString, -2);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("FCJJM UMPJB", result);
    }

    /// <summary>
    /// 测试大位移量加密（超过字母表长度）
    /// </summary>
    [TestMethod]
    public void Encrypt_LargeKey_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt(TestString, 29); // 29 % 26 = 3

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("KHOOR ZRUOG", result); // 应该与位移3相同
    }

    /// <summary>
    /// 测试小写字母加密（转换为大写）
    /// </summary>
    [TestMethod]
    public void Encrypt_LowerCase_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt(TestStringWithLowerCase);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("KHOOR ZRUOG", result);
    }

    /// <summary>
    /// 测试包含数字的字符串加密
    /// </summary>
    [TestMethod]
    public void Encrypt_WithNumbers_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt(TestStringWithNumbers);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("KHOOR123", result);
    }

    /// <summary>
    /// 测试包含空格的字符串加密
    /// </summary>
    [TestMethod]
    public void Encrypt_WithSpaces_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt(TestStringWithSpaces);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("KHOOR ZRUOG WHVW", result);
    }

    /// <summary>
    /// 测试包含特殊字符的字符串加密
    /// </summary>
    [TestMethod]
    public void Encrypt_WithSpecialChars_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt(TestStringWithSpecialChars);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("KHOOR!@#", result);
    }

    /// <summary>
    /// 测试基本解密功能
    /// </summary>
    [TestMethod]
    public void Decrypt_Basic_Test()
    {
        // Arrange
        string encrypted = "KHOOR ZRUOG";

        // Act
        string result = DisplacementCrypto.Decrypt(encrypted);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试自定义位移量解密
    /// </summary>
    [TestMethod]
    public void Decrypt_CustomKey_Test()
    {
        // Arrange
        string encrypted = "MJQQT BTWQI";

        // Act
        string result = DisplacementCrypto.Decrypt(encrypted, 5);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试加密解密往返一致性
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_RoundTrip_Test()
    {
        // Arrange
        string originalText = "THIS IS A TEST STRING WITH 123 NUMBERS!@#";

        // Act
        string encrypted = DisplacementCrypto.Encrypt(originalText);
        string decrypted = DisplacementCrypto.Decrypt(encrypted);

        // Assert
        Assert.AreEqual(originalText, decrypted);
    }

    /// <summary>
    /// 测试不同位移量的往返一致性
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_RoundTrip_CustomKey_Test()
    {
        // Arrange
        string originalText = "CUSTOM KEY TEST";
        int key = 7;

        // Act
        string encrypted = DisplacementCrypto.Encrypt(originalText, key);
        string decrypted = DisplacementCrypto.Decrypt(encrypted, key);

        // Assert
        Assert.AreEqual(originalText, decrypted);
    }

    /// <summary>
    /// 测试负位移量的往返一致性
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_RoundTrip_NegativeKey_Test()
    {
        // Arrange
        string originalText = "NEGATIVE KEY TEST";
        int key = -4;

        // Act
        string encrypted = DisplacementCrypto.Encrypt(originalText, key);
        string decrypted = DisplacementCrypto.Decrypt(encrypted, key);

        // Assert
        Assert.AreEqual(originalText, decrypted);
    }

    /// <summary>
    /// 测试边界情况 - Z字母的位移
    /// </summary>
    [TestMethod]
    public void Encrypt_Boundary_Z_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt("Z", 3);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("C", result);
    }

    /// <summary>
    /// 测试边界情况 - A字母的负位移
    /// </summary>
    [TestMethod]
    public void Encrypt_Boundary_A_Negative_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt("A", -3);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("X", result);
    }

    /// <summary>
    /// 测试零位移量
    /// </summary>
    [TestMethod]
    public void Encrypt_ZeroKey_Test()
    {
        // Act
        string result = DisplacementCrypto.Encrypt(TestString, 0);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TestString, result);
    }

    /// <summary>
    /// 测试空字符串参数
    /// </summary>
    [TestMethod]
    public void EmptyString_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => DisplacementCrypto.Encrypt(""));
        Assert.ThrowsExactly<ArgumentException>(() => DisplacementCrypto.Decrypt(""));
    }

    /// <summary>
    /// 测试null参数
    /// </summary>
    [TestMethod]
    public void NullParameter_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => DisplacementCrypto.Encrypt(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => DisplacementCrypto.Decrypt(null!));
    }

    /// <summary>
    /// 测试不支持的字符
    /// </summary>
    [TestMethod]
    public void UnsupportedCharacter_Test()
    {
        // Arrange
        string stringWithUnsupportedChars = "HELLO中文";

        // Act & Assert
        Assert.ThrowsExactly<ExtensionException>(() => DisplacementCrypto.Encrypt(stringWithUnsupportedChars));
    }

    /// <summary>
    /// 测试完整字母表加密
    /// </summary>
    [TestMethod]
    public void Encrypt_FullAlphabet_Test()
    {
        // Arrange
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Act
        string result = DisplacementCrypto.Encrypt(alphabet, 1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("BCDEFGHIJKLMNOPQRSTUVWXYZA", result);
    }

    /// <summary>
    /// 测试完整字母表解密
    /// </summary>
    [TestMethod]
    public void Decrypt_FullAlphabet_Test()
    {
        // Arrange
        string encryptedAlphabet = "BCDEFGHIJKLMNOPQRSTUVWXYZA";

        // Act
        string result = DisplacementCrypto.Decrypt(encryptedAlphabet, 1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZ", result);
    }
}
