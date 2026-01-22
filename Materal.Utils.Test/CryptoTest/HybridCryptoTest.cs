using Materal.Utils.Crypto;
using System.Security.Cryptography;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// HybridCrypto混合加密测试类
/// </summary>
[TestClass]
public class HybridCryptoTest
{
    private string _publicKey = string.Empty;
    private string _privateKey = string.Empty;
    private string _publicKeyPem = string.Empty;
    private string _privateKeyPem = string.Empty;
    private readonly string _testText = "这是一个测试文本，用于HybridCrypto混合加密解密测试。Hello HybridCrypto! 这是一个包含中文和English的混合文本，用于测试不同编码下的加密解密功能。";
    private readonly byte[] _testBytes = Encoding.UTF8.GetBytes("这是一个测试字节数组，用于HybridCrypto混合加密解密测试。包含各种特殊字符：!@#$%^&*()_+-=[]{}|;':\",./<>?");

    [TestInitialize]
    public void TestInitialize()
    {
        // 生成测试用的密钥对
        (_publicKey, _privateKey) = HybridCrypto.GenerateKeyPair(2048);
        (_publicKeyPem, _privateKeyPem) = HybridCrypto.GenerateKeyPairPem(2048);
    }

    #region 密钥生成测试
    /// <summary>
    /// 测试RSA密钥对生成
    /// </summary>
    [TestMethod]
    public void GenerateKeyPair_ShouldReturnValidKeys()
    {
        // Act
        var (publicKey, privateKey) = HybridCrypto.GenerateKeyPair();

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
        // Arrange & Act & Assert
        var (publicKey1024, privateKey1024) = HybridCrypto.GenerateKeyPair(1024);
        Assert.IsNotNull(publicKey1024);
        Assert.IsNotNull(privateKey1024);

        var (publicKey2048, privateKey2048) = HybridCrypto.GenerateKeyPair(2048);
        Assert.IsNotNull(publicKey2048);
        Assert.IsNotNull(privateKey2048);

        var (publicKey4096, privateKey4096) = HybridCrypto.GenerateKeyPair(4096);
        Assert.IsNotNull(publicKey4096);
        Assert.IsNotNull(privateKey4096);

        // 验证密钥长度递增
        Assert.IsGreaterThan(privateKey1024.Length, privateKey2048.Length);
        Assert.IsGreaterThan(privateKey2048.Length, privateKey4096.Length);
    }

    /// <summary>
    /// 测试PEM格式密钥生成
    /// </summary>
    [TestMethod]
    public void GenerateKeyPairPem_ShouldReturnValidPemKeys()
    {
        // Act
        var (publicKeyPem, privateKeyPem) = HybridCrypto.GenerateKeyPairPem();

        // Assert
        Assert.IsNotNull(publicKeyPem);
        Assert.IsNotNull(privateKeyPem);
        Assert.StartsWith("-----BEGIN PUBLIC KEY-----", publicKeyPem);
        Assert.Contains("-----END PUBLIC KEY-----", publicKeyPem);
        Assert.Contains("-----BEGIN", privateKeyPem);
        Assert.Contains("-----END", privateKeyPem);
    }

    /// <summary>
    /// 测试无效密钥长度生成
    /// </summary>
    [TestMethod]
    public void GenerateKeyPair_WithInvalidKeySize_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => HybridCrypto.GenerateKeyPair(1000));
    }
    #endregion

    #region 字节数组加密解密测试
    /// <summary>
    /// 测试字节数组加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_WithValidBytes_ShouldReturnOriginalData()
    {
        // Act
        byte[] encryptedData = HybridCrypto.Encrypt(_testBytes, _publicKey);
        byte[] decryptedData = HybridCrypto.Decrypt(encryptedData, _privateKey);

        // Assert
        Assert.IsNotNull(encryptedData);
        Assert.IsNotNull(decryptedData);
        Assert.IsNotEmpty(encryptedData);
        Assert.HasCount(_testBytes.Length, decryptedData);
        CollectionAssert.AreEqual(_testBytes, decryptedData);
    }

    /// <summary>
    /// 测试空字节数组加密
    /// </summary>
    [TestMethod]
    public void Encrypt_WithEmptyBytes_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => HybridCrypto.Encrypt([], _publicKey));
    }

    /// <summary>
    /// 测试null字节数组加密
    /// </summary>
    [TestMethod]
    public void Encrypt_WithNullBytes_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => HybridCrypto.Encrypt(null!, _publicKey));
    }

    /// <summary>
    /// 测试使用无效公钥加密
    /// </summary>
    [TestMethod]
    public void Encrypt_WithInvalidPublicKey_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => HybridCrypto.Encrypt(_testBytes, "invalid_key"));
    }

    /// <summary>
    /// 测试使用空公钥加密
    /// </summary>
    [TestMethod]
    public void Encrypt_WithEmptyPublicKey_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => HybridCrypto.Encrypt(_testBytes, string.Empty));
    }

    /// <summary>
    /// 测试解密空数据
    /// </summary>
    [TestMethod]
    public void Decrypt_WithEmptyData_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => HybridCrypto.Decrypt([], _privateKey));
    }

    /// <summary>
    /// 测试解密null数据
    /// </summary>
    [TestMethod]
    public void Decrypt_WithNullData_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => HybridCrypto.Decrypt(null!, _privateKey));
    }

    /// <summary>
    /// 测试使用无效私钥解密
    /// </summary>
    [TestMethod]
    public void Decrypt_WithInvalidPrivateKey_ShouldThrowException()
    {
        // Arrange
        byte[] encryptedData = HybridCrypto.Encrypt(_testBytes, _publicKey);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => HybridCrypto.Decrypt(encryptedData, "invalid_key"));
    }
    #endregion

    #region 字符串加密解密测试
    /// <summary>
    /// 测试字符串加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_WithValidString_ShouldReturnOriginalText()
    {
        // Act
        string cipherText = HybridCrypto.Encrypt(_testText, _publicKey);
        string decryptedText = HybridCrypto.Decrypt(cipherText, _privateKey);

        // Assert
        Assert.IsNotNull(cipherText);
        Assert.IsNotNull(decryptedText);
        Assert.IsGreaterThan(0, cipherText.Length);
        Assert.AreEqual(_testText, decryptedText);
    }

    /// <summary>
    /// 测试空字符串加密
    /// </summary>
    [TestMethod]
    public void Encrypt_WithEmptyString_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => HybridCrypto.Encrypt(string.Empty, _publicKey));
    }

    /// <summary>
    /// 测试null字符串加密
    /// </summary>
    [TestMethod]
    public void Encrypt_WithNullString_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => HybridCrypto.Encrypt(null!, _publicKey));
    }

    /// <summary>
    /// 测试解密Base64格式错误的字符串
    /// </summary>
    [TestMethod]
    public void Decrypt_WithInvalidBase64String_ShouldThrowException()
    {
        // Act & Assert
        Assert.ThrowsExactly<FormatException>(() => HybridCrypto.Decrypt("invalid_base64_string", _privateKey));
    }

    /// <summary>
    /// 测试不同编码的字符串加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_WithDifferentEncodings_ShouldWorkCorrectly()
    {
        // Arrange
        string testText = "English123!@#"; // 使用ASCII兼容的文本
        Encoding[] encodings = [Encoding.UTF8, Encoding.Unicode, Encoding.ASCII];

        foreach (Encoding encoding in encodings)
        {
            // Act
            string cipherText = HybridCrypto.Encrypt(testText, _publicKey, encoding);
            string decryptedText = HybridCrypto.Decrypt(cipherText, _privateKey, encoding);

            // Assert
            Assert.AreEqual(testText, decryptedText, $"Failed for encoding: {encoding.EncodingName}");
        }
    }
    #endregion

    #region PEM密钥测试
    /// <summary>
    /// 测试使用PEM格式密钥加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_WithPemKeys_ShouldWorkCorrectly()
    {
        // Act
        byte[] encryptedData = HybridCrypto.Encrypt(_testBytes, _publicKeyPem);
        byte[] decryptedData = HybridCrypto.Decrypt(encryptedData, _privateKeyPem);

        // Assert
        CollectionAssert.AreEqual(_testBytes, decryptedData);
    }

    /// <summary>
    /// 测试使用PEM格式密钥加密解密字符串
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_WithPemKeysString_ShouldWorkCorrectly()
    {
        // Act
        string cipherText = HybridCrypto.Encrypt(_testText, _publicKeyPem);
        string decryptedText = HybridCrypto.Decrypt(cipherText, _privateKeyPem);

        // Assert
        Assert.AreEqual(_testText, decryptedText);
    }
    #endregion

    #region 文件加密解密测试
    /// <summary>
    /// 测试文件加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecryptFile_WithValidFiles_ShouldWorkCorrectly()
    {
        // Arrange
        string tempDir = Path.GetTempPath();
        string inputFile = Path.Combine(tempDir, $"hybrid_test_{Guid.NewGuid()}.txt");
        string encryptedFile = Path.Combine(tempDir, $"hybrid_test_{Guid.NewGuid()}.dat");
        string decryptedFile = Path.Combine(tempDir, $"hybrid_test_{Guid.NewGuid()}.txt");

        try
        {
            // 写入测试内容
            File.WriteAllText(inputFile, _testText);

            // Act
            HybridCrypto.EncryptFile(inputFile, encryptedFile, _publicKey);
            HybridCrypto.DecryptFile(encryptedFile, decryptedFile, _privateKey);

            // Assert
            Assert.IsTrue(File.Exists(encryptedFile));
            Assert.IsTrue(File.Exists(decryptedFile));

            string originalContent = File.ReadAllText(inputFile);
            string decryptedContent = File.ReadAllText(decryptedFile);
            Assert.AreEqual(originalContent, decryptedContent);

            // 验证加密文件大小
            long originalSize = new FileInfo(inputFile).Length;
            long encryptedSize = new FileInfo(encryptedFile).Length;
            Assert.IsGreaterThan(originalSize, encryptedSize);
        }
        finally
        {
            // Cleanup - 使用重试机制删除文件
            DeleteFileWithRetry(inputFile);
            DeleteFileWithRetry(encryptedFile);
            DeleteFileWithRetry(decryptedFile);
        }
    }

    /// <summary>
    /// 使用重试机制删除文件
    /// </summary>
    private static void DeleteFileWithRetry(string filePath)
    {
        if (File.Exists(filePath))
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    File.Delete(filePath);
                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }
    }

    /// <summary>
    /// 测试使用FileInfo对象加密解密文件
    /// </summary>
    [TestMethod]
    public void EncryptDecryptFile_WithFileInfo_ShouldWorkCorrectly()
    {
        // Arrange
        string tempDir = Path.GetTempPath();
        string inputFile = Path.Combine(tempDir, "hybrid_test_fileinfo.txt");
        string encryptedFile = Path.Combine(tempDir, "hybrid_test_fileinfo_encrypted.dat");
        string decryptedFile = Path.Combine(tempDir, "hybrid_test_fileinfo_decrypted.txt");

        try
        {
            // Write test content
            File.WriteAllText(inputFile, _testText);
            FileInfo inputFileInfo = new(inputFile);
            FileInfo encryptedFileInfo = new(encryptedFile);
            FileInfo decryptedFileInfo = new(decryptedFile);

            // Act
            HybridCrypto.EncryptFile(inputFileInfo, encryptedFileInfo, _publicKey);
            HybridCrypto.DecryptFile(encryptedFileInfo, decryptedFileInfo, _privateKey);

            // Assert
            Assert.IsTrue(encryptedFileInfo.Exists);
            Assert.IsTrue(decryptedFileInfo.Exists);

            string originalContent = File.ReadAllText(inputFile);
            string decryptedContent = File.ReadAllText(decryptedFile);
            Assert.AreEqual(originalContent, decryptedContent);
        }
        finally
        {
            // Cleanup - 强制释放文件句柄
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (File.Exists(inputFile)) File.Delete(inputFile);
            if (File.Exists(encryptedFile)) File.Delete(encryptedFile);
            if (File.Exists(decryptedFile)) File.Delete(decryptedFile);
        }
    }

    /// <summary>
    /// 测试加密不存在的文件
    /// </summary>
    [TestMethod]
    public void EncryptFile_WithNonExistentFile_ShouldThrowException()
    {
        // Arrange
        string tempDir = Path.GetTempPath();
        string nonExistentFile = Path.Combine(tempDir, "non_existent_file.txt");
        string encryptedFile = Path.Combine(tempDir, "encrypted.dat");

        try
        {
            // Act & Assert
            Assert.ThrowsExactly<FileNotFoundException>(() => HybridCrypto.EncryptFile(nonExistentFile, encryptedFile, _publicKey));
        }
        finally
        {
            // Cleanup
            if (File.Exists(encryptedFile)) File.Delete(encryptedFile);
        }
    }
    #endregion

    #region 流加密解密测试
    /// <summary>
    /// 测试流加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecryptStream_WithValidStreams_ShouldWorkCorrectly()
    {
        // Arrange
        using MemoryStream inputStream = new(_testBytes);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // Act
        long encryptedLength = HybridCrypto.Encrypt(inputStream, encryptedStream, _publicKey);

        encryptedStream.Position = 0;
        long decryptedLength = HybridCrypto.Decrypt(encryptedStream, decryptedStream, _privateKey);

        // Assert
        Assert.AreEqual(_testBytes.Length, encryptedLength);
        Assert.AreEqual(_testBytes.Length, decryptedLength);

        decryptedStream.Position = 0;
        byte[] decryptedBytes = decryptedStream.ToArray();
        CollectionAssert.AreEqual(_testBytes, decryptedBytes);
    }

    /// <summary>
    /// 测试大文件流加密解密
    /// </summary>
    [TestMethod]
    public void EncryptDecryptStream_WithLargeData_ShouldWorkCorrectly()
    {
        // Arrange
        byte[] largeData = new byte[1024 * 1024]; // 1MB
        new Random().NextBytes(largeData);

        using MemoryStream inputStream = new(largeData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // Act
        HybridCrypto.Encrypt(inputStream, encryptedStream, _publicKey);

        encryptedStream.Position = 0;
        HybridCrypto.Decrypt(encryptedStream, decryptedStream, _privateKey);

        // Assert
        decryptedStream.Position = 0;
        byte[] decryptedBytes = decryptedStream.ToArray();
        CollectionAssert.AreEqual(largeData, decryptedBytes);
    }
    #endregion

    #region 工具方法测试
    /// <summary>
    /// 测试密钥格式检测
    /// </summary>
    [TestMethod]
    public void DetectKeyFormat_WithValidKeys_ShouldReturnCorrectFormat()
    {
        // Act & Assert
        Assert.AreEqual(KeyFormat.Xml, HybridCrypto.DetectKeyFormat(_publicKey));
        Assert.AreEqual(KeyFormat.Xml, HybridCrypto.DetectKeyFormat(_privateKey));
        Assert.AreEqual(KeyFormat.PemPublic, HybridCrypto.DetectKeyFormat(_publicKeyPem));
        Assert.AreEqual(KeyFormat.PemPrivate, HybridCrypto.DetectKeyFormat(_privateKeyPem));
        Assert.AreEqual(KeyFormat.Unknown, HybridCrypto.DetectKeyFormat("invalid_key"));
        Assert.AreEqual(KeyFormat.Unknown, HybridCrypto.DetectKeyFormat(string.Empty));
    }

    /// <summary>
    /// 测试获取加密数据信息
    /// </summary>
    [TestMethod]
    public void GetEncryptedDataInfo_WithValidData_ShouldReturnCorrectInfo()
    {
        // Arrange
        byte[] encryptedData = HybridCrypto.Encrypt(_testBytes, _publicKey);

        // Act
        var (keyLength, totalLength) = HybridCrypto.GetEncryptedDataInfo(encryptedData);

        // Assert
        Assert.IsGreaterThan(0, keyLength);
        Assert.AreEqual(encryptedData.Length, totalLength);
    }

    /// <summary>
    /// 测试验证加密数据格式
    /// </summary>
    [TestMethod]
    public void ValidateEncryptedDataFormat_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        byte[] encryptedData = HybridCrypto.Encrypt(_testBytes, _publicKey);

        // Act
        bool isValid = HybridCrypto.ValidateEncryptedDataFormat(encryptedData);

        // Assert
        Assert.IsTrue(isValid);
    }

    /// <summary>
    /// 测试验证无效数据格式
    /// </summary>
    [TestMethod]
    public void ValidateEncryptedDataFormat_WithInvalidData_ShouldReturnFalse()
    {
        // Arrange
        byte[] invalidData = [1, 2, 3, 4, 5];

        // Act
        bool isValid = HybridCrypto.ValidateEncryptedDataFormat(invalidData);

        // Assert
        Assert.IsFalse(isValid);
    }

    /// <summary>
    /// 测试估算加密大小
    /// </summary>
    [TestMethod]
    public void EstimateEncryptedSize_WithValidInput_ShouldReturnReasonableSize()
    {
        // Arrange
        long originalSize = 1024; // 1KB

        // Act
        long estimatedSize = HybridCrypto.EstimateEncryptedSize(originalSize);

        // Assert
        Assert.IsGreaterThan(originalSize, estimatedSize);
        Assert.IsLessThan(originalSize * 2, estimatedSize); // 应该不会超过原始大小的2倍
    }

    /// <summary>
    /// 测试获取当前AES模式
    /// </summary>
    [TestMethod]
    public void GetCurrentAesMode_ShouldReturnValidMode()
    {
        // Act
        string mode = HybridCrypto.GetCurrentAesMode();

        // Assert
        Assert.IsNotNull(mode);
        Assert.Contains("AES", mode);
    }

    /// <summary>
    /// 测试获取数据格式描述
    /// </summary>
    [TestMethod]
    public void GetDataFormatDescription_ShouldReturnValidDescription()
    {
        // Act
        string description = HybridCrypto.GetDataFormatDescription();

        // Assert
        Assert.IsNotNull(description);
        Assert.Contains("加密数据格式", description);
        Assert.IsGreaterThan(0, description.Length);
    }

    /// <summary>
    /// 测试性能比较
    /// </summary>
    [TestMethod]
    public void ComparePerformanceWithRsa_WithValidInput_ShouldReturnComparison()
    {
        // Arrange
        long dataSize = 10240; // 10KB

        // Act
        string comparison = HybridCrypto.ComparePerformanceWithRsa(dataSize);

        // Assert
        Assert.IsNotNull(comparison);
        Assert.Contains("数据大小", comparison);
        Assert.Contains("RSA", comparison);
        Assert.Contains("混合加密", comparison);
    }
    #endregion

    #region 边界条件测试
    /// <summary>
    /// 测试最小数据加密
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_WithMinimalData_ShouldWorkCorrectly()
    {
        // Arrange
        byte[] singleByte = [42];

        // Act
        byte[] encryptedData = HybridCrypto.Encrypt(singleByte, _publicKey);
        byte[] decryptedData = HybridCrypto.Decrypt(encryptedData, _privateKey);

        // Assert
        CollectionAssert.AreEqual(singleByte, decryptedData);
    }

    /// <summary>
    /// 测试单字符字符串加密
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_WithSingleCharacter_ShouldWorkCorrectly()
    {
        // Arrange
        string singleChar = "A";

        // Act
        string cipherText = HybridCrypto.Encrypt(singleChar, _publicKey);
        string decryptedText = HybridCrypto.Decrypt(cipherText, _privateKey);

        // Assert
        Assert.AreEqual(singleChar, decryptedText);
    }

    /// <summary>
    /// 测试长字符串加密
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_WithLongString_ShouldWorkCorrectly()
    {
        // Arrange
        StringBuilder longTextBuilder = new();
        for (int i = 0; i < 1000; i++)
        {
            longTextBuilder.AppendLine($"这是第{i}行测试文本，包含一些特殊字符：!@#$%^&*()");
        }
        string longText = longTextBuilder.ToString();

        // Act
        string cipherText = HybridCrypto.Encrypt(longText, _publicKey);
        string decryptedText = HybridCrypto.Decrypt(cipherText, _privateKey);

        // Assert
        Assert.AreEqual(longText, decryptedText);
    }

    /// <summary>
    /// 测试Unicode字符加密
    /// </summary>
    [TestMethod]
    public void EncryptDecrypt_WithUnicodeCharacters_ShouldWorkCorrectly()
    {
        // Arrange
        string unicodeText = "🔒🔐🔑 测试加密 🌟💎🎯 العربية русский 中文 日本語 한국어";

        // Act
        string cipherText = HybridCrypto.Encrypt(unicodeText, _publicKey);
        string decryptedText = HybridCrypto.Decrypt(cipherText, _privateKey);

        // Assert
        Assert.AreEqual(unicodeText, decryptedText);
    }
    #endregion

    #region 错误处理测试
    /// <summary>
    /// 测试使用错误密钥解密
    /// </summary>
    [TestMethod]
    public void Decrypt_WithWrongPrivateKey_ShouldThrowException()
    {
        // Arrange
        var (wrongPublicKey, wrongPrivateKey) = HybridCrypto.GenerateKeyPair();
        byte[] encryptedData = HybridCrypto.Encrypt(_testBytes, _publicKey);

        // Act & Assert
        Assert.ThrowsExactly<CryptographicException>(() => HybridCrypto.Decrypt(encryptedData, wrongPrivateKey));
    }

    /// <summary>
    /// 测试解密被篡改的数据
    /// </summary>
    [TestMethod]
    public void Decrypt_WithTamperedData_ShouldThrowException()
    {
        // Arrange
        byte[] encryptedData = HybridCrypto.Encrypt(_testBytes, _publicKey);

        // 篡改数据
        if (encryptedData.Length > 10)
        {
            encryptedData[^1] ^= 0xFF; // 翻转最后一个字节
        }

        // Act & Assert
        Assert.ThrowsExactly<CryptographicException>(() => HybridCrypto.Decrypt(encryptedData, _privateKey));
    }

    /// <summary>
    /// 测试使用公钥解密
    /// </summary>
    [TestMethod]
    public void Decrypt_WithPublicKey_ShouldThrowException()
    {
        // Arrange
        byte[] encryptedData = HybridCrypto.Encrypt(_testBytes, _publicKey);

        // Act & Assert
        Assert.ThrowsExactly<CryptographicException>(() => HybridCrypto.Decrypt(encryptedData, _publicKey));
    }
    #endregion

    #region 条件编译测试
    /// <summary>
    /// 测试条件编译功能 - 验证当前平台使用正确的AES模式
    /// </summary>
    [TestMethod]
    public void ConditionalCompilation_ShouldUseCorrectAesMode()
    {
        // Act
        string currentMode = HybridCrypto.GetCurrentAesMode();

        // Assert - 根据运行时环境验证
        string description = HybridCrypto.GetDataFormatDescription();

        Assert.IsNotNull(currentMode);
        Assert.IsNotNull(description);

#if NETSTANDARD
        Assert.IsTrue(currentMode.Contains("CBC"));
        Assert.IsTrue(description.Contains("CBC"));
#else
        Assert.Contains("GCM", currentMode);
        Assert.Contains("GCM", description);
#endif
    }

    /// <summary>
    /// 测试不同平台下的加密数据格式兼容性
    /// </summary>
    [TestMethod]
    public void EncryptedDataFormat_ShouldBeConsistentOnCurrentPlatform()
    {
        // Arrange
        byte[] testData = Encoding.UTF8.GetBytes("测试数据格式一致性");

        // Act
        byte[] encryptedData1 = HybridCrypto.Encrypt(testData, _publicKey);
        byte[] encryptedData2 = HybridCrypto.Encrypt(testData, _publicKey);

        // Assert - 相同数据在不同时间加密应该产生不同的密文（因为随机IV/nonce）
        Assert.AreNotEqual(encryptedData1, encryptedData2);

        // 但解密后应该得到相同结果
        byte[] decryptedData1 = HybridCrypto.Decrypt(encryptedData1, _privateKey);
        byte[] decryptedData2 = HybridCrypto.Decrypt(encryptedData2, _privateKey);
        CollectionAssert.AreEqual(testData, decryptedData1);
        CollectionAssert.AreEqual(testData, decryptedData2);
    }
    #endregion
}
