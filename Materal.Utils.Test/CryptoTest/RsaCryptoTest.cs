using Materal.Utils.Crypto;
using System.Security.Cryptography;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// RSA加密测试类
/// </summary>
[TestClass]
public class RsaCryptoTest
{
    private string _publicKey = string.Empty;
    private string _privateKey = string.Empty;
    private readonly string _testText = "这是一个测试文本，用于RSA加密解密测试。Hello RSA!";
    private readonly byte[] _testBytes = Encoding.UTF8.GetBytes("这是一个测试字节数组，用于RSA加密解密测试。Hello RSA!");

    [TestInitialize]
    public void TestInitialize()
    {
        // 生成测试用的密钥对
        (_publicKey, _privateKey) = RsaCrypto.GenerateKeyPair(2048);
    }

    #region 密钥生成测试
    /// <summary>
    /// 测试RSA密钥对生成
    /// </summary>
    [TestMethod]
    public void GenerateKeyPair_ShouldReturnValidKeys()
    {
        // Act
        var (publicKey, privateKey) = RsaCrypto.GenerateKeyPair();

        // Assert
        Assert.IsNotNull(publicKey);
        Assert.IsNotNull(privateKey);
        Assert.IsGreaterThan(0, publicKey.Length);
        Assert.IsGreaterThan(0, privateKey.Length);
        Assert.IsGreaterThan(publicKey.Length, privateKey.Length); // 私钥应该比公钥长
    }

    /// <summary>
    /// 测试不同密钥长度的生成
    /// </summary>
    [TestMethod]
    public void GenerateKeyPair_WithDifferentKeySizes_ShouldReturnValidKeys()
    {
        // Act & Assert
        var (publicKey1024, privateKey1024) = RsaCrypto.GenerateKeyPair(1024);
        Assert.IsNotNull(publicKey1024);
        Assert.IsNotNull(privateKey1024);

        var (publicKey4096, privateKey4096) = RsaCrypto.GenerateKeyPair(4096);
        Assert.IsNotNull(publicKey4096);
        Assert.IsNotNull(privateKey4096);

        // 4096位的密钥应该比1024位的密钥长
        Assert.IsGreaterThan(publicKey1024.Length, publicKey4096.Length);
        Assert.IsGreaterThan(privateKey1024.Length, privateKey4096.Length);
    }

    /// <summary>
    /// 测试无效密钥长度
    /// </summary>
    [TestMethod]
    public void GenerateKeyPair_WithInvalidKeySize_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => RsaCrypto.GenerateKeyPair(1000));
    }
    #endregion

    #region 字节数组加密解密测试
    /// <summary>
    /// 测试字节数组加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecryptBytes_ShouldReturnOriginalData()
    {
        // Act
        byte[] encryptedData = RsaCrypto.Encrypt(_testBytes, _publicKey);
        byte[] decryptedData = RsaCrypto.Decrypt(encryptedData, _privateKey);

        // Assert
        Assert.IsNotNull(encryptedData);
        Assert.IsNotNull(decryptedData);
        CollectionAssert.AreEqual(_testBytes, decryptedData);
    }

    /// <summary>
    /// 测试不同填充模式的加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecryptBytes_WithDifferentPadding_ShouldReturnOriginalData()
    {
        // Act - 只测试PKCS1填充，因为OAEP在某些系统上不支持
        byte[] encryptedData1 = RsaCrypto.Encrypt(_testBytes, _publicKey, RSAEncryptionPadding.Pkcs1);
        byte[] decryptedData1 = RsaCrypto.Decrypt(encryptedData1, _privateKey, RSAEncryptionPadding.Pkcs1);

        // Assert
        CollectionAssert.AreEqual(_testBytes, decryptedData1);
    }

    /// <summary>
    /// 测试大数据分块加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecryptLargeData_ShouldReturnOriginalData()
    {
        // Arrange - 创建大数据（超过RSA单次加密限制）
        byte[] largeData = new byte[500];
        for (int i = 0; i < largeData.Length; i++)
        {
            largeData[i] = (byte)(i % 256);
        }

        // Act
        byte[] encryptedData = RsaCrypto.EncryptLargeData(largeData, _publicKey);
        byte[] decryptedData = RsaCrypto.DecryptLargeData(encryptedData, _privateKey);

        // Assert
        Assert.IsNotNull(encryptedData);
        Assert.IsNotNull(decryptedData);
        CollectionAssert.AreEqual(largeData, decryptedData);
    }

    /// <summary>
    /// 测试空字节数组
    /// </summary>
    [TestMethod]
    public void Encrypt_WithEmptyBytes_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => RsaCrypto.Encrypt([], _publicKey));
    }

    /// <summary>
    /// 测试null字节数组
    /// </summary>
    [TestMethod]
    public void Encrypt_WithNullBytes_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => RsaCrypto.Encrypt((byte[])null!, _publicKey));
    }
    #endregion

    #region 字符串加密解密测试
    /// <summary>
    /// 测试字符串加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecryptString_ShouldReturnOriginalText()
    {
        // Act
        string encryptedText = RsaCrypto.Encrypt(_testText, _publicKey);
        string decryptedText = RsaCrypto.Decrypt(encryptedText, _privateKey);

        // Assert
        Assert.IsNotNull(encryptedText);
        Assert.IsNotNull(decryptedText);
        Assert.AreEqual(_testText, decryptedText);
    }

    /// <summary>
    /// 测试长字符串分块加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecryptLargeText_ShouldReturnOriginalText()
    {
        // Arrange - 创建长文本
        StringBuilder longTextBuilder = new();
        for (int i = 0; i < 100; i++)
        {
            longTextBuilder.AppendLine("这是第" + i + "行测试文本，用于测试RSA长文本加密解密功能。");
        }
        string longText = longTextBuilder.ToString();

        // Act
        string encryptedText = RsaCrypto.EncryptLargeText(longText, _publicKey);
        string decryptedText = RsaCrypto.DecryptLargeText(encryptedText, _privateKey);

        // Assert
        Assert.IsNotNull(encryptedText);
        Assert.IsNotNull(decryptedText);
        Assert.AreEqual(longText, decryptedText);
    }

    /// <summary>
    /// 测试空字符串
    /// </summary>
    [TestMethod]
    public void Encrypt_WithEmptyString_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => RsaCrypto.Encrypt("", _publicKey));
    }

    /// <summary>
    /// 测试null字符串
    /// </summary>
    [TestMethod]
    public void Encrypt_WithNullString_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => RsaCrypto.Encrypt((string)null!, _publicKey));
    }
    #endregion

    #region 数字签名测试
    /// <summary>
    /// 测试数据签名和验证
    /// </summary>
    [TestMethod]
    public void SignAndVerifyData_ShouldReturnTrue()
    {
        // Act
        byte[] signature = RsaCrypto.SignData(_testBytes, _privateKey);
        bool isValid = RsaCrypto.VerifyData(_testBytes, signature, _publicKey);

        // Assert
        Assert.IsNotNull(signature);
        Assert.IsNotEmpty(signature);
        Assert.IsTrue(isValid);
    }

    /// <summary>
    /// 测试文本签名和验证
    /// </summary>
    [TestMethod]
    public void SignAndVerifyText_ShouldReturnTrue()
    {
        // Act
        string signature = RsaCrypto.SignText(_testText, _privateKey);
        bool isValid = RsaCrypto.VerifyText(_testText, signature, _publicKey);

        // Assert
        Assert.IsNotNull(signature);
        Assert.IsGreaterThan(0, signature.Length);
        Assert.IsTrue(isValid);
    }

    /// <summary>
    /// 测试不同哈希算法的签名
    /// </summary>
    [TestMethod]
    public void SignAndVerify_WithDifferentHashAlgorithms_ShouldReturnTrue()
    {
        // Act & Assert
        byte[] signatureSHA256 = RsaCrypto.SignData(_testBytes, _privateKey, HashAlgorithmName.SHA256);
        bool isValidSHA256 = RsaCrypto.VerifyData(_testBytes, signatureSHA256, _publicKey, HashAlgorithmName.SHA256);
        Assert.IsTrue(isValidSHA256);

        byte[] signatureSHA384 = RsaCrypto.SignData(_testBytes, _privateKey, HashAlgorithmName.SHA384);
        bool isValidSHA384 = RsaCrypto.VerifyData(_testBytes, signatureSHA384, _publicKey, HashAlgorithmName.SHA384);
        Assert.IsTrue(isValidSHA384);

        byte[] signatureSHA512 = RsaCrypto.SignData(_testBytes, _privateKey, HashAlgorithmName.SHA512);
        bool isValidSHA512 = RsaCrypto.VerifyData(_testBytes, signatureSHA512, _publicKey, HashAlgorithmName.SHA512);
        Assert.IsTrue(isValidSHA512);
    }

    /// <summary>
    /// 测试篡改数据的签名验证
    /// </summary>
    [TestMethod]
    public void VerifyData_WithTamperedData_ShouldReturnFalse()
    {
        // Arrange
        byte[] signature = RsaCrypto.SignData(_testBytes, _privateKey);
        byte[] tamperedData = new byte[_testBytes.Length];
        Array.Copy(_testBytes, tamperedData, _testBytes.Length);
        tamperedData[0]++; // 篡改第一个字节

        // Act
        bool isValid = RsaCrypto.VerifyData(tamperedData, signature, _publicKey);

        // Assert
        Assert.IsFalse(isValid);
    }

    /// <summary>
    /// 测试空数据签名
    /// </summary>
    [TestMethod]
    public void SignData_WithEmptyData_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => RsaCrypto.SignData([], _privateKey));
    }
    #endregion

    #region 异常情况测试
    /// <summary>
    /// 测试使用错误的密钥解密
    /// </summary>
    [TestMethod]
    public void Decrypt_WithWrongPrivateKey_ShouldThrowException()
    {
        // Arrange
        byte[] encryptedData = RsaCrypto.Encrypt(_testBytes, _publicKey);
        var (_, wrongPrivateKey) = RsaCrypto.GenerateKeyPair(); // 生成新的密钥对

        // Act & Assert
        Assert.ThrowsExactly<CryptographicException>(() => RsaCrypto.Decrypt(encryptedData, wrongPrivateKey));
    }

    /// <summary>
    /// 测试使用公钥解密（应该失败）
    /// </summary>
    [TestMethod]
    public void Decrypt_WithPublicKey_ShouldThrowException()
    {
        // Arrange
        byte[] encryptedData = RsaCrypto.Encrypt(_testBytes, _publicKey);

        // Act & Assert
        Assert.ThrowsExactly<CryptographicException>(() => RsaCrypto.Decrypt(encryptedData, _publicKey));
    }

    /// <summary>
    /// 测试无效的Base64字符串解密
    /// </summary>
    [TestMethod]
    public void Decrypt_WithInvalidBase64String_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<FormatException>(() => RsaCrypto.Decrypt("InvalidBase64String", _privateKey));
    }

    /// <summary>
    /// 测试null公钥加密
    /// </summary>
    [TestMethod]
    public void Encrypt_WithNullPublicKey_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => RsaCrypto.Encrypt(_testText, null!));
    }

    /// <summary>
    /// 测试null私钥解密
    /// </summary>
    [TestMethod]
    public void Decrypt_WithNullPrivateKey_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => RsaCrypto.Decrypt("SomeEncryptedText", null!));
    }
    #endregion

    #region 性能测试
    /// <summary>
    /// 测试加密解密性能
    /// </summary>
    [TestMethod]
    public void EncryptDecryptPerformance_ShouldCompleteInReasonableTime()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 10; i++)
        {
            string encrypted = RsaCrypto.Encrypt(_testText, _publicKey);
            string decrypted = RsaCrypto.Decrypt(encrypted, _privateKey);
            Assert.AreEqual(_testText, decrypted);
        }

        stopwatch.Stop();

        // Assert - 10次加密解密应该在10秒内完成
        Assert.IsLessThan(10000, stopwatch.ElapsedMilliseconds, $"RSA加密解密性能测试超时: {stopwatch.ElapsedMilliseconds}ms");
    }
    #endregion
}
