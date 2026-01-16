
#if NET
using Materal.Utils.Crypto;

namespace Materal.Utils.Test.CryptoTest;

public partial class AesCryptoTest
{
    #region GCM密钥生成测试
    /// <summary>
    /// 测试生成AES-GCM密钥
    /// </summary>
    [TestMethod]
    public void GenerateAesGCMKey_ShouldReturnValidKey()
    {
        // Act
        byte[] key = AesCrypto.GenerateAesGCMKey();

        // Assert
        Assert.IsNotNull(key);
        Assert.HasCount(32, key); // 默认256位
        CollectionAssert.AllItemsAreNotNull(key);
    }

    /// <summary>
    /// 测试生成不同大小的AES-GCM密钥
    /// 验证生成的密钥长度是否符合预期（128位=16字节，192位=24字节，256位=32字节）
    /// </summary>
    [TestMethod]
    public void GenerateAesGCMKey_WithDifferentSizes_ShouldReturnValidKeys()
    {
        // Act & Assert
        byte[] key128 = AesCrypto.GenerateAesGCMKey(128);
        byte[] key192 = AesCrypto.GenerateAesGCMKey(192);
        byte[] key256 = AesCrypto.GenerateAesGCMKey(256);

        Assert.HasCount(16, key128);
        Assert.HasCount(24, key192);
        Assert.HasCount(32, key256);
    }

    /// <summary>
    /// 测试使用无效密钥大小生成AES-GCM密钥时抛出ArgumentException
    /// 验证只支持128、192、256位密钥大小
    /// </summary>
    [TestMethod]
    public void GenerateAesGCMKey_WithInvalidSize_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.GenerateAesGCMKey(512));
    }

    /// <summary>
    /// 测试生成AES-GCM字符串密钥
    /// 验证返回的密钥是有效的Base64格式且长度为32字节（256位）
    /// </summary>
    [TestMethod]
    public void GenerateAesGCMStringKey_ShouldReturnValidBase64Key()
    {
        // Act
        string key = AesCrypto.GenerateAesGCMStringKey();

        // Assert
        Assert.IsNotNull(key);
        Assert.IsGreaterThan(0, key.Length);

        // 验证Base64格式
        byte[] keyBytes = Convert.FromBase64String(key);
        Assert.HasCount(32, keyBytes);
    }
    #endregion

    #region GCM加密解密测试
    /// <summary>
    /// 测试使用字符串密钥的AES-GCM加密解密功能
    /// 验证使用Base64编码的密钥可以正确加密并解密回原始内容
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecrypt_WithStringKey_ShouldReturnOriginalContent()
    {
        // Arrange
        string key = AesCrypto.GenerateAesGCMStringKey();

        // Act
        byte[] encrypted = AesCrypto.AesGCMEncrypt(_testBytes, key);
        byte[] decrypted = AesCrypto.AesGCMDecrypt(encrypted, key);

        // Assert
        CollectionAssert.AreEqual(_testBytes, decrypted);
        Assert.AreEqual(TestContent, Encoding.UTF8.GetString(decrypted));
    }

    /// <summary>
    /// 测试使用字节数组密钥的AES-GCM加密解密功能
    /// 验证使用原始字节数组密钥可以正确加密并解密回原始内容
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecrypt_WithByteArrayKey_ShouldReturnOriginalContent()
    {
        // Arrange
        byte[] keyBytes = AesCrypto.GenerateAesGCMKey();

        // Act
        byte[] encrypted = AesCrypto.AesGCMEncrypt(_testBytes, keyBytes);
        byte[] decrypted = AesCrypto.AesGCMDecrypt(encrypted, keyBytes);

        // Assert
        CollectionAssert.AreEqual(_testBytes, decrypted);
        Assert.AreEqual(TestContent, Encoding.UTF8.GetString(decrypted));
    }

    /// <summary>
    /// 测试AES-GCM自动生成密钥和Nonce的加密功能
    /// 验证自动生成的密钥和Nonce可以正确加密并解密回原始内容
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecrypt_WithOutKeyAndNonce_ShouldReturnOriginalContent()
    {
        // Act
        byte[] encrypted = AesCrypto.AesGCMEncrypt(_testBytes, out string key, out string nonce);
        byte[] decrypted = AesCrypto.AesGCMDecrypt(encrypted, key, nonce);

        // Assert
        Assert.IsNotNull(key);
        Assert.IsNotNull(nonce);
        CollectionAssert.AreEqual(_testBytes, decrypted);
    }

    /// <summary>
    /// 测试使用相同密钥但不同Nonce进行AES-GCM加密的结果
    /// 验证即使使用相同的密钥和明文，不同的Nonce也会产生完全不同的密文
    /// 这是AES-GCM模式的重要安全特性，确保相同数据的多次加密产生不同的密文
    /// </summary>
    [TestMethod]
    public void AesGCMEncrypt_WithSameKeyButDifferentNonce_ShouldReturnDifferentResults()
    {
        // Act
        byte[] encrypted1 = AesCrypto.AesGCMEncrypt(_testBytes, out _, out _);
        byte[] encrypted2 = AesCrypto.AesGCMEncrypt(_testBytes, out _, out _);

        // Assert
        CollectionAssert.AreNotEqual(encrypted1, encrypted2);
    }
    #endregion

    #region GCM异常测试
    /// <summary>
    /// 测试使用空内容进行AES-GCM加密时抛出ArgumentException
    /// 验证加密方法不接受空的内容，确保输入数据的有效性
    /// </summary>
    [TestMethod]
    public void AesGCMEncrypt_WithEmptyContent_ShouldThrowArgumentException()
    {
        // Arrange
        string key = AesCrypto.GenerateAesGCMStringKey();
        byte[] emptyContent = [];

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesGCMEncrypt(emptyContent, key));
    }

    /// <summary>
    /// 测试无效密钥长度加密时抛出 ArgumentException
    /// </summary>
    [TestMethod]
    public void AesGCMEncrypt_WithInvalidKeyLength_ShouldThrowArgumentException()
    {
        // Arrange
        string invalidKey = Convert.ToBase64String(new byte[10]);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesGCMEncrypt(_testBytes, invalidKey));
    }

    /// <summary>
    /// 测试数据被篡改时解密抛出AuthenticationTagMismatchException
    /// 验证AES-GCM的认证功能能够检测数据篡改
    /// </summary>
    [TestMethod]
    public void AesGCMDecrypt_WithTamperedData_ShouldThrowAuthenticationTagMismatchException()
    {
        // Arrange
        string key = AesCrypto.GenerateAesGCMStringKey();
        byte[] encrypted = AesCrypto.AesGCMEncrypt(_testBytes, key);

        // 篡改数据
        encrypted[0] ^= (byte)1;

        // Act & Assert
        Assert.ThrowsExactly<System.Security.Cryptography.AuthenticationTagMismatchException>(() => AesCrypto.AesGCMDecrypt(encrypted, key));
    }

    /// <summary>
    /// 测试无效Nonce长度解密时抛出 ArgumentException
    /// </summary>
    [TestMethod]
    public void AesGCMDecrypt_WithInvalidNonceLength_ShouldThrowArgumentException()
    {
        // Arrange
        string key = AesCrypto.GenerateAesGCMStringKey();
        string invalidNonce = Convert.ToBase64String(new byte[10]);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesGCMDecrypt(_testBytes, key, invalidNonce));
    }
    #endregion
}
#endif