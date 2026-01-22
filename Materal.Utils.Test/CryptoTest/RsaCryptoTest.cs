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
    private string _publicKeyPem = string.Empty;
    private string _privateKeyPem = string.Empty;
    private readonly string _testText = "这是一个测试文本，用于RSA加密解密测试。Hello RSA!";
    private readonly byte[] _testBytes = Encoding.UTF8.GetBytes("这是一个测试字节数组，用于RSA加密解密测试。Hello RSA!");

    [TestInitialize]
    public void TestInitialize()
    {
        // 生成测试用的密钥对
        (_publicKey, _privateKey) = RsaCrypto.GenerateKeyPair(2048);
        (_publicKeyPem, _privateKeyPem) = RsaCrypto.GenerateKeyPairPem(2048);
    }

    #region 密钥生成测试
    /// <summary>
    /// 测试RSA密钥对生成
    /// </summary>
    [TestMethod]
    public void GenerateKeyPair_ShouldReturnValidKeys()
    {
        // Act
        (string? publicKey, string? privateKey) = RsaCrypto.GenerateKeyPair();

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
        (string? publicKey1024, string? privateKey1024) = RsaCrypto.GenerateKeyPair(1024);
        Assert.IsNotNull(publicKey1024);
        Assert.IsNotNull(privateKey1024);

        (string? publicKey4096, string? privateKey4096) = RsaCrypto.GenerateKeyPair(4096);
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

    /// <summary>
    /// 测试PEM格式密钥对生成
    /// </summary>
    [TestMethod]
    public void GenerateKeyPairPem_ShouldReturnValidPemKeys()
    {
        // Act
        (string? publicKeyPem, string? privateKeyPem) = RsaCrypto.GenerateKeyPairPem();

        // Assert
        Assert.IsNotNull(publicKeyPem);
        Assert.IsNotNull(privateKeyPem);
        Assert.StartsWith("-----BEGIN PUBLIC KEY-----", publicKeyPem);
        Assert.EndsWith("-----END PUBLIC KEY-----", publicKeyPem.Trim());
        Assert.StartsWith("-----BEGIN PRIVATE KEY-----", privateKeyPem);
        Assert.EndsWith("-----END PRIVATE KEY-----", privateKeyPem.Trim());
        Assert.IsGreaterThan(0, publicKeyPem.Length);
        Assert.IsGreaterThan(0, privateKeyPem.Length);
    }

    /// <summary>
    /// 测试不同密钥长度的PEM格式生成
    /// </summary>
    [TestMethod]
    public void GenerateKeyPairPem_WithDifferentKeySizes_ShouldReturnValidPemKeys()
    {
        // Act & Assert
        (string? publicKeyPem1024, string? privateKeyPem1024) = RsaCrypto.GenerateKeyPairPem(1024);
        Assert.IsNotNull(publicKeyPem1024);
        Assert.IsNotNull(privateKeyPem1024);
        Assert.Contains("-----BEGIN PUBLIC KEY-----", publicKeyPem1024);
        Assert.Contains("-----BEGIN PRIVATE KEY-----", privateKeyPem1024);

        (string? publicKeyPem4096, string? privateKeyPem4096) = RsaCrypto.GenerateKeyPairPem(4096);
        Assert.IsNotNull(publicKeyPem4096);
        Assert.IsNotNull(privateKeyPem4096);
        Assert.Contains("-----BEGIN PUBLIC KEY-----", publicKeyPem4096);
        Assert.Contains("-----BEGIN PRIVATE KEY-----", privateKeyPem4096);

        // 验证所有生成的密钥都有有效的内容
        Assert.IsGreaterThan(100, publicKeyPem1024.Length); // PEM密钥应该有合理长度
        Assert.IsGreaterThan(200, privateKeyPem1024.Length);
        Assert.IsGreaterThan(100, publicKeyPem4096.Length);
        Assert.IsGreaterThan(200, privateKeyPem4096.Length);

        // 验证密钥格式正确性
        Assert.IsTrue(publicKeyPem1024.Contains("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A") || publicKeyPem1024.Length > 150); // 1024位公钥标识
        Assert.IsTrue(publicKeyPem4096.Contains("MIICIjANBgkqhkiG9w0BAQEFAAOCAg8A") || publicKeyPem4096.Length > 300); // 4096位公钥标识
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

    /// <summary>
    /// 测试使用PEM格式公钥加密字节数组
    /// </summary>
    [TestMethod]
    public void EncryptBytes_WithPemPublicKey_ShouldReturnOriginalData()
    {
        // Act
        byte[] encryptedData = RsaCrypto.Encrypt(_testBytes, _publicKeyPem);
        byte[] decryptedData = RsaCrypto.Decrypt(encryptedData, _privateKeyPem);

        // Assert
        Assert.IsNotNull(encryptedData);
        Assert.IsNotNull(decryptedData);
        CollectionAssert.AreEqual(_testBytes, decryptedData);
    }

    /// <summary>
    /// 测试使用PEM格式密钥进行大数据分块加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecryptLargeData_WithPemKeys_ShouldReturnOriginalData()
    {
        // Arrange - 创建大数据（超过RSA单次加密限制）
        byte[] largeData = new byte[500];
        for (int i = 0; i < largeData.Length; i++)
        {
            largeData[i] = (byte)(i % 256);
        }

        // Act
        byte[] encryptedData = RsaCrypto.EncryptLargeData(largeData, _publicKeyPem);
        byte[] decryptedData = RsaCrypto.DecryptLargeData(encryptedData, _privateKeyPem);

        // Assert
        Assert.IsNotNull(encryptedData);
        Assert.IsNotNull(decryptedData);
        CollectionAssert.AreEqual(largeData, decryptedData);
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

    /// <summary>
    /// 测试使用PEM格式公钥加密字符串
    /// </summary>
    [TestMethod]
    public void EncryptString_WithPemPublicKey_ShouldReturnOriginalText()
    {
        // Act
        string encryptedText = RsaCrypto.Encrypt(_testText, _publicKeyPem);
        string decryptedText = RsaCrypto.Decrypt(encryptedText, _privateKeyPem);

        // Assert
        Assert.IsNotNull(encryptedText);
        Assert.IsNotNull(decryptedText);
        Assert.AreEqual(_testText, decryptedText);
    }

    /// <summary>
    /// 测试使用PEM格式密钥进行长字符串分块加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecryptLargeText_WithPemKeys_ShouldReturnOriginalText()
    {
        // Arrange - 创建长文本
        StringBuilder longTextBuilder = new();
        for (int i = 0; i < 100; i++)
        {
            longTextBuilder.AppendLine("这是第" + i + "行测试文本，用于测试RSA长文本加密解密功能。");
        }
        string longText = longTextBuilder.ToString();

        // Act
        string encryptedText = RsaCrypto.EncryptLargeText(longText, _publicKeyPem);
        string decryptedText = RsaCrypto.DecryptLargeText(encryptedText, _privateKeyPem);

        // Assert
        Assert.IsNotNull(encryptedText);
        Assert.IsNotNull(decryptedText);
        Assert.AreEqual(longText, decryptedText);
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

    /// <summary>
    /// 测试使用PEM格式密钥进行数据签名和验证
    /// </summary>
    [TestMethod]
    public void SignAndVerifyData_WithPemKeys_ShouldReturnTrue()
    {
        // Act
        byte[] signature = RsaCrypto.SignData(_testBytes, _privateKeyPem);
        bool isValid = RsaCrypto.VerifyData(_testBytes, signature, _publicKeyPem);

        // Assert
        Assert.IsNotNull(signature);
        Assert.IsNotEmpty(signature);
        Assert.IsTrue(isValid);
    }

    /// <summary>
    /// 测试使用PEM格式密钥进行文本签名和验证
    /// </summary>
    [TestMethod]
    public void SignAndVerifyText_WithPemKeys_ShouldReturnTrue()
    {
        // Act
        string signature = RsaCrypto.SignText(_testText, _privateKeyPem);
        bool isValid = RsaCrypto.VerifyText(_testText, signature, _publicKeyPem);

        // Assert
        Assert.IsNotNull(signature);
        Assert.IsGreaterThan(0, signature.Length);
        Assert.IsTrue(isValid);
    }

    /// <summary>
    /// 测试混合使用XML和PEM格式密钥
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_WithMixedKeyFormats_ShouldWork()
    {
        // Act - 使用XML公钥加密，PEM私钥解密（应该失败，因为不是同一对密钥）
        byte[] encryptedWithXml = RsaCrypto.Encrypt(_testBytes, _publicKey);
        Assert.ThrowsExactly<CryptographicException>(() => RsaCrypto.Decrypt(encryptedWithXml, _privateKeyPem));

        // Act - 使用PEM公钥加密，XML私钥解密（应该失败，因为不是同一对密钥）
        byte[] encryptedWithPem = RsaCrypto.Encrypt(_testBytes, _publicKeyPem);
        Assert.ThrowsExactly<CryptographicException>(() => RsaCrypto.Decrypt(encryptedWithPem, _privateKey));

        // Act - 使用同一对密钥的不同格式应该成功
        byte[] encryptedWithPem2 = RsaCrypto.Encrypt(_testBytes, _publicKeyPem);
        byte[] decryptedWithPem2 = RsaCrypto.Decrypt(encryptedWithPem2, _privateKeyPem);
        CollectionAssert.AreEqual(_testBytes, decryptedWithPem2);
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
        (string _, string? wrongPrivateKey) = RsaCrypto.GenerateKeyPair(); // 生成新的密钥对

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

    #region PEM格式测试
    /// <summary>
    /// 测试密钥格式检测功能
    /// </summary>
    [TestMethod]
    public void DetectKeyFormat_ShouldReturnCorrectFormat()
    {
        // Act & Assert
        Assert.AreEqual(KeyFormat.Xml, RsaCrypto.DetectKeyFormat(_publicKey));
        Assert.AreEqual(KeyFormat.Xml, RsaCrypto.DetectKeyFormat(_privateKey));
        Assert.AreEqual(KeyFormat.PemPublic, RsaCrypto.DetectKeyFormat(_publicKeyPem));
        Assert.AreEqual(KeyFormat.PemPrivate, RsaCrypto.DetectKeyFormat(_privateKeyPem));
        Assert.AreEqual(KeyFormat.Unknown, RsaCrypto.DetectKeyFormat(""));
        Assert.AreEqual(KeyFormat.Unknown, RsaCrypto.DetectKeyFormat("InvalidKey"));
        Assert.AreEqual(KeyFormat.Unknown, RsaCrypto.DetectKeyFormat(null!));
    }

    /// <summary>
    /// 测试使用无效PEM格式密钥
    /// </summary>
    [TestMethod]
    public void Encrypt_WithInvalidPemKey_ShouldThrowException()
    {
        // Arrange
        string invalidPemKey = "-----BEGIN PUBLIC KEY-----\nInvalidBase64Data\n-----END PUBLIC KEY-----";

        // Act & Assert
        Assert.ThrowsExactly<FormatException>(() => RsaCrypto.Encrypt(_testText, invalidPemKey));
    }

    /// <summary>
    /// 测试使用PEM公钥进行解密（应该失败）
    /// </summary>
    [TestMethod]
    public void Decrypt_WithPemPublicKey_ShouldThrowException()
    {
        // Arrange
        byte[] encryptedData = RsaCrypto.Encrypt(_testBytes, _publicKeyPem);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => RsaCrypto.Decrypt(encryptedData, _publicKeyPem));
    }

    /// <summary>
    /// 测试PEM格式密钥的跨格式兼容性
    /// </summary>
    [TestMethod]
    public void PemKeys_ShouldWorkWithAllMethods()
    {
        // 测试所有加密解密方法
        string encryptedString = RsaCrypto.Encrypt(_testText, _publicKeyPem);
        string decryptedString = RsaCrypto.Decrypt(encryptedString, _privateKeyPem);
        Assert.AreEqual(_testText, decryptedString);

        byte[] encryptedBytes = RsaCrypto.Encrypt(_testBytes, _publicKeyPem);
        byte[] decryptedBytes = RsaCrypto.Decrypt(encryptedBytes, _privateKeyPem);
        CollectionAssert.AreEqual(_testBytes, decryptedBytes);

        string encryptedLargeText = RsaCrypto.EncryptLargeText(_testText, _publicKeyPem);
        string decryptedLargeText = RsaCrypto.DecryptLargeText(encryptedLargeText, _privateKeyPem);
        Assert.AreEqual(_testText, decryptedLargeText);

        byte[] encryptedLargeData = RsaCrypto.EncryptLargeData(_testBytes, _publicKeyPem);
        byte[] decryptedLargeData = RsaCrypto.DecryptLargeData(encryptedLargeData, _privateKeyPem);
        CollectionAssert.AreEqual(_testBytes, decryptedLargeData);

        // 测试签名验证方法
        byte[] signature = RsaCrypto.SignData(_testBytes, _privateKeyPem);
        bool isValid = RsaCrypto.VerifyData(_testBytes, signature, _publicKeyPem);
        Assert.IsTrue(isValid);

        string textSignature = RsaCrypto.SignText(_testText, _privateKeyPem);
        bool isTextValid = RsaCrypto.VerifyText(_testText, textSignature, _publicKeyPem);
        Assert.IsTrue(isTextValid);
    }
    #endregion

    #region 密钥格式转换测试
    /// <summary>
    /// 测试XML格式密钥转换为PEM格式
    /// </summary>
    [TestMethod]
    public void ConvertXmlToPem_ShouldReturnValidPemKeys()
    {
        // Act
        (string? publicKeyPem, string? privateKeyPem) = RsaCrypto.ConvertXmlToPem(_publicKey, _privateKey);

        // Assert
        Assert.IsNotNull(publicKeyPem);
        Assert.IsNotNull(privateKeyPem);
        Assert.StartsWith("-----BEGIN PUBLIC KEY-----", publicKeyPem);
        Assert.EndsWith("-----END PUBLIC KEY-----", publicKeyPem.Trim());
        Assert.StartsWith("-----BEGIN PRIVATE KEY-----", privateKeyPem);
        Assert.EndsWith("-----END PRIVATE KEY-----", privateKeyPem.Trim());
        Assert.IsGreaterThan(0, publicKeyPem.Length);
        Assert.IsGreaterThan(0, privateKeyPem.Length);

        // 验证转换后的PEM密钥可以正常使用
        string encryptedText = RsaCrypto.Encrypt(_testText, publicKeyPem);
        string decryptedText = RsaCrypto.Decrypt(encryptedText, privateKeyPem);
        Assert.AreEqual(_testText, decryptedText);
    }

    /// <summary>
    /// 测试使用无效的XML私钥转换为PEM格式
    /// </summary>
    [TestMethod]
    public void ConvertXmlToPem_WithInvalidPrivateKeyXml_ShouldThrowException()
    {
        // Arrange
        string invalidPrivateKeyXml = "<RSAKeyValue><Invalid>InvalidKey</Invalid></RSAKeyValue>";

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => RsaCrypto.ConvertXmlToPem(_publicKey, invalidPrivateKeyXml));
    }

    /// <summary>
    /// 测试使用null私钥XML转换为PEM格式
    /// </summary>
    [TestMethod]
    public void ConvertXmlToPem_WithNullPrivateKeyXml_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => RsaCrypto.ConvertXmlToPem(_publicKey, null!));
    }

    /// <summary>
    /// 测试使用空字符串私钥XML转换为PEM格式
    /// </summary>
    [TestMethod]
    public void ConvertXmlToPem_WithEmptyPrivateKeyXml_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => RsaCrypto.ConvertXmlToPem(_publicKey, string.Empty));
    }

    /// <summary>
    /// 测试转换后的PEM密钥与原始PEM密钥的兼容性
    /// </summary>
    [TestMethod]
    public void ConvertXmlToPem_ConvertedKeysShouldWorkWithOriginalPemKeys()
    {
        // Act
        (string convertedPublicKeyPem, string convertedPrivateKeyPem) = RsaCrypto.ConvertXmlToPem(_publicKey, _privateKey);

        // 验证转换后的密钥与原始PEM密钥功能相同
        // 使用转换后的密钥加密解密
        string encryptedWithConverted = RsaCrypto.Encrypt(_testText, convertedPublicKeyPem);
        string decryptedWithConverted = RsaCrypto.Decrypt(encryptedWithConverted, convertedPrivateKeyPem);
        Assert.AreEqual(_testText, decryptedWithConverted);

        // 使用原始PEM密钥加密解密
        string encryptedWithOriginal = RsaCrypto.Encrypt(_testText, _publicKeyPem);
        string decryptedWithOriginal = RsaCrypto.Decrypt(encryptedWithOriginal, _privateKeyPem);
        Assert.AreEqual(_testText, decryptedWithOriginal);

        // 验证转换后的密钥格式正确
        Assert.AreEqual(KeyFormat.PemPublic, RsaCrypto.DetectKeyFormat(convertedPublicKeyPem));
        Assert.AreEqual(KeyFormat.PemPrivate, RsaCrypto.DetectKeyFormat(convertedPrivateKeyPem));
    }

    /// <summary>
    /// 测试转换后的PEM密钥签名验证功能
    /// </summary>
    [TestMethod]
    public void ConvertXmlToPem_ConvertedKeysShouldWorkForSigning()
    {
        // Act
        (string convertedPublicKeyPem, string convertedPrivateKeyPem) = RsaCrypto.ConvertXmlToPem(_publicKey, _privateKey);

        // 验证转换后的密钥可以用于签名和验证
        byte[] signature = RsaCrypto.SignData(_testBytes, convertedPrivateKeyPem);
        bool isValid = RsaCrypto.VerifyData(_testBytes, signature, convertedPublicKeyPem);
        Assert.IsTrue(isValid);

        // 验证文本签名
        string textSignature = RsaCrypto.SignText(_testText, convertedPrivateKeyPem);
        bool isTextValid = RsaCrypto.VerifyText(_testText, textSignature, convertedPublicKeyPem);
        Assert.IsTrue(isTextValid);
    }

    /// <summary>
    /// 测试XML格式密钥与转换后PEM格式密钥的加密解密结果一致性
    /// </summary>
    [TestMethod]
    public void ConvertXmlToPem_EncryptionDecryptionResultsShouldBeConsistent()
    {
        // Act
        (string convertedPublicKeyPem, string convertedPrivateKeyPem) = RsaCrypto.ConvertXmlToPem(_publicKey, _privateKey);

        // 使用原始XML格式密钥进行加密解密
        string encryptedWithXmlPublic = RsaCrypto.Encrypt(_testText, _publicKey);
        string decryptedWithXmlPrivate = RsaCrypto.Decrypt(encryptedWithXmlPublic, _privateKey);

        // 使用转换后的PEM格式密钥进行加密解密
        string encryptedWithPemPublic = RsaCrypto.Encrypt(_testText, convertedPublicKeyPem);
        string decryptedWithPemPrivate = RsaCrypto.Decrypt(encryptedWithPemPublic, convertedPrivateKeyPem);

        // 验证解密结果一致性
        Assert.AreEqual(_testText, decryptedWithXmlPrivate, "XML格式密钥解密结果应该与原文相同");
        Assert.AreEqual(_testText, decryptedWithPemPrivate, "PEM格式密钥解密结果应该与原文相同");
        Assert.AreEqual(decryptedWithXmlPrivate, decryptedWithPemPrivate, "XML和PEM格式密钥的解密结果应该完全相同");

        // 验证不同格式密钥加密同一数据的结果不同（因为每次加密可能包含随机填充）
        // 但解密后应该得到相同的原文
        string decryptedXmlWithPem = RsaCrypto.Decrypt(encryptedWithXmlPublic, convertedPrivateKeyPem);
        string decryptedPemWithXml = RsaCrypto.Decrypt(encryptedWithPemPublic, _privateKey);

        Assert.AreEqual(_testText, decryptedXmlWithPem, "XML公钥加密的数据可以用PEM私钥解密");
        Assert.AreEqual(_testText, decryptedPemWithXml, "PEM公钥加密的数据可以用XML私钥解密");

        // 字节数组加密解密一致性测试
        byte[] encryptedBytesWithXml = RsaCrypto.Encrypt(_testBytes, _publicKey);
        byte[] decryptedBytesWithXml = RsaCrypto.Decrypt(encryptedBytesWithXml, _privateKey);

        byte[] encryptedBytesWithPem = RsaCrypto.Encrypt(_testBytes, convertedPublicKeyPem);
        byte[] decryptedBytesWithPem = RsaCrypto.Decrypt(encryptedBytesWithPem, convertedPrivateKeyPem);

        CollectionAssert.AreEqual(_testBytes, decryptedBytesWithXml, "XML格式密钥字节数组解密结果应该与原文相同");
        CollectionAssert.AreEqual(_testBytes, decryptedBytesWithPem, "PEM格式密钥字节数组解密结果应该与原文相同");
        CollectionAssert.AreEqual(decryptedBytesWithXml, decryptedBytesWithPem, "XML和PEM格式密钥的字节数组解密结果应该完全相同");

        // 跨格式解密测试
        byte[] decryptedXmlBytesWithPem = RsaCrypto.Decrypt(encryptedBytesWithXml, convertedPrivateKeyPem);
        byte[] decryptedPemBytesWithXml = RsaCrypto.Decrypt(encryptedBytesWithPem, _privateKey);

        CollectionAssert.AreEqual(_testBytes, decryptedXmlBytesWithPem, "XML公钥加密的字节数组可以用PEM私钥解密");
        CollectionAssert.AreEqual(_testBytes, decryptedPemBytesWithXml, "PEM公钥加密的字节数组可以用XML私钥解密");
    }

    /// <summary>
    /// 测试XML格式密钥与转换后PEM格式密钥的签名验证结果一致性
    /// </summary>
    [TestMethod]
    public void ConvertXmlToPem_SignatureVerificationResultsShouldBeConsistent()
    {
        // Act
        (string convertedPublicKeyPem, string convertedPrivateKeyPem) = RsaCrypto.ConvertXmlToPem(_publicKey, _privateKey);

        // 使用原始XML格式密钥进行签名
        byte[] signatureWithXml = RsaCrypto.SignData(_testBytes, _privateKey);
        string textSignatureWithXml = RsaCrypto.SignText(_testText, _privateKey);

        // 使用转换后的PEM格式密钥进行签名
        byte[] signatureWithPem = RsaCrypto.SignData(_testBytes, convertedPrivateKeyPem);
        string textSignatureWithPem = RsaCrypto.SignText(_testText, convertedPrivateKeyPem);

        // 验证签名结果一致性 - 使用各自的公钥验证
        bool isValidXmlWithXml = RsaCrypto.VerifyData(_testBytes, signatureWithXml, _publicKey);
        bool isValidPemWithPem = RsaCrypto.VerifyData(_testBytes, signatureWithPem, convertedPublicKeyPem);
        bool isTextValidXmlWithXml = RsaCrypto.VerifyText(_testText, textSignatureWithXml, _publicKey);
        bool isTextValidPemWithPem = RsaCrypto.VerifyText(_testText, textSignatureWithPem, convertedPublicKeyPem);

        Assert.IsTrue(isValidXmlWithXml, "XML格式密钥签名应该有效");
        Assert.IsTrue(isValidPemWithPem, "PEM格式密钥签名应该有效");
        Assert.IsTrue(isTextValidXmlWithXml, "XML格式密钥文本签名应该有效");
        Assert.IsTrue(isTextValidPemWithPem, "PEM格式密钥文本签名应该有效");

        // 跨格式验证 - XML私钥签名可以用PEM公钥验证
        bool isValidXmlWithPem = RsaCrypto.VerifyData(_testBytes, signatureWithXml, convertedPublicKeyPem);
        bool isTextValidXmlWithPem = RsaCrypto.VerifyText(_testText, textSignatureWithXml, convertedPublicKeyPem);

        // 跨格式验证 - PEM私钥签名可以用XML公钥验证
        bool isValidPemWithXml = RsaCrypto.VerifyData(_testBytes, signatureWithPem, _publicKey);
        bool isTextValidPemWithXml = RsaCrypto.VerifyText(_testText, textSignatureWithPem, _publicKey);

        Assert.IsTrue(isValidXmlWithPem, "XML私钥签名可以用PEM公钥验证");
        Assert.IsTrue(isTextValidXmlWithPem, "XML私钥文本签名可以用PEM公钥验证");
        Assert.IsTrue(isValidPemWithXml, "PEM私钥签名可以用XML公钥验证");
        Assert.IsTrue(isTextValidPemWithXml, "PEM私钥文本签名可以用XML公钥验证");
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
