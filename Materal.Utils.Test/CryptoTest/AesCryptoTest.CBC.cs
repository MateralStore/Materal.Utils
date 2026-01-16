using Materal.Utils.Crypto;

namespace Materal.Utils.Test.CryptoTest;

public partial class AesCryptoTest
{
    #region 密钥生成测试
    /// <summary>
    /// 测试 AES-CBC 密钥生成功能
    /// 验证生成的密钥和 IV 长度正确
    /// </summary>
    [TestMethod]
    public void GenerateAesCBCKey_ShouldReturnValidKeyAndIV()
    {
        // Act
        (byte[] key, byte[] iv) = AesCrypto.GenerateAesCBCKey();

        // Assert
        Assert.IsNotNull(key);
        Assert.IsNotNull(iv);
        Assert.HasCount(32, key); // 256位
        Assert.HasCount(16, iv);  // 128位
        CollectionAssert.AllItemsAreNotNull(key);
        CollectionAssert.AllItemsAreNotNull(iv);
    }

    /// <summary>
    /// 测试 AES-CBC 字符串密钥生成功能
    /// 验证生成的 Base64 编码密钥和 IV 可以正确解码且长度正确
    /// </summary>
    [TestMethod]
    public void GenerateAesCBCStringKey_ShouldReturnValidBase64KeyAndIV()
    {
        // Act
        (string key, string iv) = AesCrypto.GenerateAesCBCStringKey();

        // Assert
        Assert.IsNotNull(key);
        Assert.IsNotNull(iv);
        Assert.IsGreaterThan(0, key.Length);
        Assert.IsGreaterThan(0, iv.Length);

        // 验证Base64格式
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        Assert.HasCount(32, keyBytes);
        Assert.HasCount(16, ivBytes);
    }
    #endregion

    #region CBC加密解密测试（使用字符串密钥）
    /// <summary>
    /// 测试使用字符串密钥的 AES-CBC 加密解密功能
    /// 验证使用 Base64 编码的密钥和 IV 可以正确加密并解密回原始内容
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecrypt_WithStringKey_ShouldReturnOriginalContent()
    {
        // Arrange
        (string key, string iv) = AesCrypto.GenerateAesCBCStringKey();

        // Act
        byte[] encrypted = AesCrypto.AesCBCEncrypt(_testBytes, key, iv);
        byte[] decrypted = AesCrypto.AesCBCDecrypt(encrypted, key, iv);

        // Assert
        CollectionAssert.AreEqual(_testBytes, decrypted);
        Assert.AreEqual(TestContent, Encoding.UTF8.GetString(decrypted));
    }

    /// <summary>
    /// 测试使用字节数组密钥的 AES-CBC 加密解密功能
    /// 验证使用原始字节数组密钥和 IV 可以正确加密并解密回原始内容
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecrypt_WithByteArrayKey_ShouldReturnOriginalContent()
    {
        // Arrange
        (byte[]? keyBytes, byte[]? ivBytes) = AesCrypto.GenerateAesCBCKey();

        // Act
        byte[] encrypted = AesCrypto.AesCBCEncrypt(_testBytes, keyBytes, ivBytes);
        byte[] decrypted = AesCrypto.AesCBCDecrypt(encrypted, keyBytes, ivBytes);

        // Assert
        CollectionAssert.AreEqual(_testBytes, decrypted);
        Assert.AreEqual(TestContent, Encoding.UTF8.GetString(decrypted));
    }
    #endregion

    #region CBC自动生成密钥测试
    /// <summary>
    /// 测试 AES-CBC 自动生成密钥和 IV 的加密功能
    /// 验证自动生成的密钥和 IV 可以正确加密并解密回原始内容
    /// </summary>
    [TestMethod]
    public void AesCBCEncrypt_WithOutKeyAndIV_ShouldGenerateValidKeyAndIV()
    {
        // Act
        byte[] encrypted = AesCrypto.AesCBCEncrypt(_testBytes, out string key, out string iv);
        byte[] decrypted = AesCrypto.AesCBCDecrypt(encrypted, key, iv);

        // Assert
        Assert.IsNotNull(key);
        Assert.IsNotNull(iv);
        CollectionAssert.AreEqual(_testBytes, decrypted);
    }
    #endregion

    #region CBC随机IV测试（IV前置）
    /// <summary>
    /// 测试 AES-CBC 随机 IV 加密功能
    /// 验证使用相同密钥但随机 IV 的多次加密结果不同，但都能正确解密
    /// </summary>
    [TestMethod]
    public void AesCBCEncrypt_WithRandomIV_ShouldReturnValidEncryptedData()
    {
        // Arrange
        (string? key, string _) = AesCrypto.GenerateAesCBCStringKey();

        // Act
        byte[] encrypted1 = AesCrypto.AesCBCEncrypt(_testBytes, key);
        byte[] encrypted2 = AesCrypto.AesCBCEncrypt(_testBytes, key);
        byte[] decrypted1 = AesCrypto.AesCBCDecrypt(encrypted1, key);
        byte[] decrypted2 = AesCrypto.AesCBCDecrypt(encrypted2, key);

        // Assert
        CollectionAssert.AreNotEqual(encrypted1, encrypted2); // 每次加密结果应该不同（随机IV）
        CollectionAssert.AreEqual(_testBytes, decrypted1);
        CollectionAssert.AreEqual(_testBytes, decrypted2);
    }

    /// <summary>
    /// 测试使用字节数组密钥的 AES-CBC 随机 IV 加密功能
    /// 验证加密结果包含 IV 且能正确解密
    /// </summary>
    [TestMethod]
    public void AesCBCEncrypt_WithRandomIVAndByteArrayKey_ShouldReturnValidEncryptedData()
    {
        // Arrange
        (byte[]? keyBytes, byte[] _) = AesCrypto.GenerateAesCBCKey();

        // Act
        byte[] encrypted = AesCrypto.AesCBCEncrypt(_testBytes, keyBytes);
        byte[] decrypted = AesCrypto.AesCBCDecrypt(encrypted, keyBytes);

        // Assert
        Assert.IsGreaterThan(_testBytes.Length + 16, encrypted.Length); // 应该包含IV
        CollectionAssert.AreEqual(_testBytes, decrypted);
    }
    #endregion

    #region CBC异常测试
    /// <summary>
    /// 测试空内容加密时抛出 ArgumentException
    /// </summary>
    [TestMethod]
    public void AesCBCEncrypt_WithEmptyContent_ShouldThrowArgumentException()
    {
        // Arrange
        (string key, string iv) = AesCrypto.GenerateAesCBCStringKey();
        byte[] emptyContent = [];

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesCBCEncrypt(emptyContent, key, iv));
    }

    /// <summary>
    /// 测试 null 内容加密时抛出 ArgumentException
    /// </summary>
    [TestMethod]
    public void AesCBCEncrypt_WithNullContent_ShouldThrowArgumentException()
    {
        // Arrange
        (string key, string iv) = AesCrypto.GenerateAesCBCStringKey();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesCBCEncrypt(null!, key, iv));
    }

    /// <summary>
    /// 测试无效密钥长度时抛出 ArgumentException
    /// </summary>
    [TestMethod]
    public void AesCBCEncrypt_WithInvalidKeyLength_ShouldThrowArgumentException()
    {
        // Arrange
        (string key, string iv) = AesCrypto.GenerateAesCBCStringKey();
        string invalidKey = Convert.ToBase64String(new byte[10]); // 10字节无效长度

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesCBCEncrypt(_testBytes, invalidKey, iv));
    }

    /// <summary>
    /// 测试无效 IV 长度时抛出 ArgumentException
    /// </summary>
    [TestMethod]
    public void AesCBCEncrypt_WithInvalidIVLength_ShouldThrowArgumentException()
    {
        // Arrange
        (string? key, string _) = AesCrypto.GenerateAesCBCStringKey();
        string invalidIV = Convert.ToBase64String(new byte[10]); // 10字节无效长度

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesCBCEncrypt(_testBytes, key, invalidIV));
    }

    /// <summary>
    /// 测试当使用无效的Base64密钥时，AesCBCEncrypt方法是否抛出FormatException异常
    /// </summary>
    [TestMethod]
    public void AesCBCEncrypt_WithInvalidBase64Key_ShouldThrowFormatException()
    {
        // Arrange
        (string key, string iv) = AesCrypto.GenerateAesCBCStringKey();
        string invalidKey = "InvalidBase64!@#";

        // Act & Assert
        Assert.ThrowsExactly<FormatException>(() => AesCrypto.AesCBCEncrypt(_testBytes, invalidKey, iv));
    }
    #endregion
}