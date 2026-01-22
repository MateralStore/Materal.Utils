using Materal.Utils.Crypto;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// DES 加密解密工具测试类
/// 测试 DES-CBC 模式的加密解密功能
/// </summary>
[TestClass]
public partial class DesCryptoTest
{
    /// <summary>
    /// 测试内容
    /// </summary>
    private const string TestContent = "这是一个DES测试内容，用于验证DES加解密功能！Hello World! 123456789";
    /// <summary>
    /// 测试内容Byte数组
    /// </summary>
    private readonly byte[] _testBytes = Encoding.UTF8.GetBytes(TestContent);

    #region 密钥生成测试
    /// <summary>
    /// 测试 DES-CBC 密钥生成功能
    /// 验证生成的密钥和 IV 长度正确
    /// </summary>
    [TestMethod]
    public void GenerateDesCBCKey_ShouldReturnValidKeyAndIV()
    {
        // Act
        (byte[] key, byte[] iv) = DesCrypto.GenerateCBCKey();

        // Assert
        Assert.IsNotNull(key);
        Assert.IsNotNull(iv);
        Assert.HasCount(8, key); // 64位
        Assert.HasCount(8, iv);  // 64位
        CollectionAssert.AllItemsAreNotNull(key);
        CollectionAssert.AllItemsAreNotNull(iv);
    }

    /// <summary>
    /// 测试 DES-CBC 字符串密钥生成功能
    /// 验证生成的 Base64 编码密钥和 IV 可以正确解码且长度正确
    /// </summary>
    [TestMethod]
    public void GenerateDesCBCStringKey_ShouldReturnValidBase64KeyAndIV()
    {
        // Act
        (string key, string iv) = DesCrypto.GenerateCBCStringKey();

        // Assert
        Assert.IsNotNull(key);
        Assert.IsNotNull(iv);
        Assert.IsGreaterThan(0, key.Length);
        Assert.IsGreaterThan(0, iv.Length);

        // 验证Base64格式
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        Assert.HasCount(8, keyBytes);
        Assert.HasCount(8, ivBytes);
    }
    #endregion

    #region CBC加密解密测试（使用字符串密钥）
    /// <summary>
    /// 测试使用字符串密钥的 DES-CBC 加密解密功能
    /// 验证使用 Base64 编码的密钥和 IV 可以正确加密并解密回原始内容
    /// </summary>
    [TestMethod]
    public void DesCBCEncryptDecrypt_WithStringKey_ShouldReturnOriginalContent()
    {
        // Arrange
        (string key, string iv) = DesCrypto.GenerateCBCStringKey();

        // Act
        byte[] encrypted = DesCrypto.CBCEncrypt(_testBytes, key, iv);
        byte[] decrypted = DesCrypto.CBCDecrypt(encrypted, key, iv);

        // Assert
        CollectionAssert.AreEqual(_testBytes, decrypted);
        Assert.AreEqual(TestContent, Encoding.UTF8.GetString(decrypted));
    }

    /// <summary>
    /// 测试使用字节数组密钥的 DES-CBC 加密解密功能
    /// 验证使用原始字节数组密钥和 IV 可以正确加密并解密回原始内容
    /// </summary>
    [TestMethod]
    public void DesCBCEncryptDecrypt_WithByteArrayKey_ShouldReturnOriginalContent()
    {
        // Arrange
        (byte[] keyBytes, byte[] ivBytes) = DesCrypto.GenerateCBCKey();

        // Act
        byte[] encrypted = DesCrypto.CBCEncrypt(_testBytes, keyBytes, ivBytes);
        byte[] decrypted = DesCrypto.CBCDecrypt(encrypted, keyBytes, ivBytes);

        // Assert
        CollectionAssert.AreEqual(_testBytes, decrypted);
        Assert.AreEqual(TestContent, Encoding.UTF8.GetString(decrypted));
    }

    /// <summary>
    /// 测试使用自动生成密钥的 DES-CBC 加密解密功能
    /// 验证自动生成的密钥和 IV 可以正确加密并解密回原始内容
    /// </summary>
    [TestMethod]
    public void DesCBCEncryptDecrypt_WithGeneratedKey_ShouldReturnOriginalContent()
    {
        // Act
        (byte[] encryptedData, byte[] key, byte[] iv) = DesCrypto.CBCEncryptWithGeneratedKey(_testBytes);
        byte[] decrypted = DesCrypto.CBCDecrypt(encryptedData, key, iv);

        // Assert
        CollectionAssert.AreEqual(_testBytes, decrypted);
        Assert.AreEqual(TestContent, Encoding.UTF8.GetString(decrypted));
    }

    /// <summary>
    /// 测试使用自动生成密钥的字符串 DES-CBC 加密解密功能
    /// </summary>
    [TestMethod]
    public void DesCBCEncryptDecrypt_WithGeneratedStringKey_ShouldReturnOriginalContent()
    {
        // Act
        (string encryptedData, string key, string iv) = DesCrypto.CBCEncryptWithGeneratedKey(TestContent);
        string decrypted = DesCrypto.CBCDecrypt(encryptedData, key, iv);

        // Assert
        Assert.AreEqual(TestContent, decrypted);
    }
    #endregion

    #region 字符串加密解密测试
    /// <summary>
    /// 测试 DES-CBC 字符串加密解密功能
    /// </summary>
    [TestMethod]
    public void DesCBCStringEncryptDecrypt_ShouldReturnOriginalString()
    {
        // Arrange
        (string key, string iv) = DesCrypto.GenerateCBCStringKey();

        // Act
        string encrypted = DesCrypto.CBCEncrypt(TestContent, key, iv);
        string decrypted = DesCrypto.CBCDecrypt(encrypted, key, iv);

        // Assert
        Assert.AreEqual(TestContent, decrypted);
    }

    /// <summary>
    /// 测试 DES-CBC 字符串加密解密功能（使用UTF8编码）
    /// </summary>
    [TestMethod]
    public void DesCBCStringEncryptDecrypt_WithUTF8Encoding_ShouldReturnOriginalString()
    {
        // Arrange
        (string key, string iv) = DesCrypto.GenerateCBCStringKey();
        string testString = "测试中文字符串！🎉 Hello World! 123456789";

        // Act
        string encrypted = DesCrypto.CBCEncrypt(testString, key, iv, Encoding.UTF8);
        string decrypted = DesCrypto.CBCDecrypt(encrypted, key, iv, Encoding.UTF8);

        // Assert
        Assert.AreEqual(testString, decrypted);
    }
    #endregion

    #region 流加密解密测试
    /// <summary>
    /// 测试 DES-CBC 流加密解密功能
    /// </summary>
    [TestMethod]
    public void DesCBCStreamEncryptDecrypt_ShouldReturnOriginalContent()
    {
        // Arrange
        (string key, string iv) = DesCrypto.GenerateCBCStringKey();

        using MemoryStream inputStream = new(_testBytes);
        using MemoryStream outputStream = new();
        using MemoryStream decryptedStream = new();

        // Act
        long encryptedBytes = DesCrypto.CBCEncrypt(inputStream, outputStream, key, iv);
        byte[] encryptedData = outputStream.ToArray();

        using MemoryStream encryptedStream = new(encryptedData);
        long decryptedBytes = DesCrypto.CBCDecrypt(encryptedStream, decryptedStream, key, iv);
        byte[] decryptedData = decryptedStream.ToArray();

        // Assert
        Assert.AreEqual(_testBytes.Length, encryptedBytes);
        Assert.AreEqual(_testBytes.Length, decryptedBytes);
        CollectionAssert.AreEqual(_testBytes, decryptedData);
    }

    /// <summary>
    /// 测试 DES-CBC 流加密解密功能（使用字节数组密钥）
    /// </summary>
    [TestMethod]
    public void DesCBCStreamEncryptDecrypt_WithByteArrayKey_ShouldReturnOriginalContent()
    {
        // Arrange
        (byte[] keyBytes, byte[] ivBytes) = DesCrypto.GenerateCBCKey();

        using MemoryStream inputStream = new(_testBytes);
        using MemoryStream outputStream = new();
        using MemoryStream decryptedStream = new();

        // Act
        long encryptedBytes = DesCrypto.CBCEncrypt(inputStream, outputStream, keyBytes, ivBytes);
        byte[] encryptedData = outputStream.ToArray();

        using MemoryStream encryptedStream = new(encryptedData);
        long decryptedBytes = DesCrypto.CBCDecrypt(encryptedStream, decryptedStream, keyBytes, ivBytes);
        byte[] decryptedData = decryptedStream.ToArray();

        // Assert
        Assert.AreEqual(_testBytes.Length, encryptedBytes);
        Assert.AreEqual(_testBytes.Length, decryptedBytes);
        CollectionAssert.AreEqual(_testBytes, decryptedData);
    }
    #endregion

    #region 文件加密解密测试
    /// <summary>
    /// 测试 DES-CBC 文件加密解密功能
    /// </summary>
    [TestMethod]
    public void DesCBCFileEncryptDecrypt_ShouldReturnOriginalContent()
    {
        // Arrange
        string testDirectory = Path.Combine(Path.GetTempPath(), $"DesCryptoFileTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);

        string originalFile = Path.Combine(testDirectory, "original.txt");
        string encryptedFile = Path.Combine(testDirectory, "encrypted.dat");
        string decryptedFile = Path.Combine(testDirectory, "decrypted.txt");

        try
        {
            // 创建测试文件
            File.WriteAllText(originalFile, TestContent, Encoding.UTF8);
            (string key, string iv) = DesCrypto.GenerateCBCStringKey();

            // Act
            DesCrypto.CBCEncryptFile(originalFile, encryptedFile, key, iv);
            DesCrypto.CBCDecryptFile(encryptedFile, decryptedFile, key, iv);

            // Assert
            string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
            Assert.AreEqual(TestContent, decryptedContent);
        }
        finally
        {
            // 确保文件句柄释放后再清理
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100); // 等待文件句柄释放

            if (Directory.Exists(testDirectory))
            {
                try
                {
                    Directory.Delete(testDirectory, true);
                }
                catch (IOException)
                {
                    // 如果无法删除，忽略错误（测试已通过）
                }
            }
        }
    }

    /// <summary>
    /// 测试 DES-CBC 文件加密解密功能（使用FileInfo）
    /// </summary>
    [TestMethod]
    public void DesCBCFileEncryptDecrypt_WithFileInfo_ShouldReturnOriginalContent()
    {
        // Arrange
        string testDirectory = Path.Combine(Path.GetTempPath(), $"DesCryptoFileInfoTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);

        FileInfo originalFile = new(Path.Combine(testDirectory, "original.txt"));
        FileInfo encryptedFile = new(Path.Combine(testDirectory, "encrypted.dat"));
        FileInfo decryptedFile = new(Path.Combine(testDirectory, "decrypted.txt"));

        try
        {
            // 创建测试文件
            File.WriteAllText(originalFile.FullName, TestContent, Encoding.UTF8);
            (string key, string iv) = DesCrypto.GenerateCBCStringKey();

            // Act
            DesCrypto.CBCEncryptFile(originalFile, encryptedFile, key, iv);
            DesCrypto.CBCDecryptFile(encryptedFile, decryptedFile, key, iv);

            // Assert
            string decryptedContent = File.ReadAllText(decryptedFile.FullName, Encoding.UTF8);
            Assert.AreEqual(TestContent, decryptedContent);
        }
        finally
        {
            // 确保文件句柄释放后再清理
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100); // 等待文件句柄释放

            if (Directory.Exists(testDirectory))
            {
                try
                {
                    Directory.Delete(testDirectory, true);
                }
                catch (IOException)
                {
                    // 如果无法删除，忽略错误（测试已通过）
                }
            }
        }
    }

    /// <summary>
    /// 测试使用自动生成密钥的 DES-CBC 文件加密解密功能
    /// </summary>
    [TestMethod]
    public void DesCBCFileEncryptDecrypt_WithGeneratedKey_ShouldReturnOriginalContent()
    {
        // Arrange
        string testDirectory = Path.Combine(Path.GetTempPath(), $"DesCryptoGeneratedKeyTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);

        string originalFile = Path.Combine(testDirectory, "original.txt");
        string encryptedFile = Path.Combine(testDirectory, "encrypted.dat");
        string decryptedFile = Path.Combine(testDirectory, "decrypted.txt");

        try
        {
            // 创建测试文件
            File.WriteAllText(originalFile, TestContent, Encoding.UTF8);

            // Act
            var (key, iv) = DesCrypto.CBCEncryptFileWithGeneratedKey(originalFile, encryptedFile);
            DesCrypto.CBCDecryptFile(encryptedFile, decryptedFile, key, iv);

            // Assert
            string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
            Assert.AreEqual(TestContent, decryptedContent);
        }
        finally
        {
            // 确保文件句柄释放后再清理
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100); // 等待文件句柄释放

            if (Directory.Exists(testDirectory))
            {
                try
                {
                    Directory.Delete(testDirectory, true);
                }
                catch (IOException)
                {
                    // 如果无法删除，忽略错误（测试已通过）
                }
            }
        }
    }
    #endregion

    #region 异常处理测试
    /// <summary>
    /// 测试空内容异常
    /// </summary>
    [TestMethod]
    public void DesCBCEncrypt_WithEmptyContent_ShouldThrowArgumentException()
    {
        // Arrange
        (string key, string iv) = DesCrypto.GenerateCBCStringKey();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => DesCrypto.CBCEncrypt("", key, iv));
    }

    /// <summary>
    /// 测试空密钥异常
    /// </summary>
    [TestMethod]
    public void DesCBCEncrypt_WithEmptyKey_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => DesCrypto.CBCEncrypt(TestContent, "", DesCrypto.GenerateCBCStringKey().iv));
    }

    /// <summary>
    /// 测试空IV异常
    /// </summary>
    [TestMethod]
    public void DesCBCEncrypt_WithEmptyIV_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => DesCrypto.CBCEncrypt(TestContent, DesCrypto.GenerateCBCStringKey().key, ""));
    }

    /// <summary>
    /// 测试无效密钥长度异常
    /// </summary>
    [TestMethod]
    public void DesCBCEncrypt_WithInvalidKeyLength_ShouldThrowArgumentException()
    {
        // Arrange
        string invalidKey = Convert.ToBase64String(new byte[16]); // 16字节，DES需要8字节
        string validIv = Convert.ToBase64String(new byte[8]);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => DesCrypto.CBCEncrypt(TestContent, invalidKey, validIv));
    }

    /// <summary>
    /// 测试无效IV长度异常
    /// </summary>
    [TestMethod]
    public void DesCBCEncrypt_WithInvalidIVLength_ShouldThrowArgumentException()
    {
        // Arrange
        string validKey = Convert.ToBase64String(new byte[8]);
        string invalidIv = Convert.ToBase64String(new byte[16]); // 16字节，DES需要8字节

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => DesCrypto.CBCEncrypt(TestContent, validKey, invalidIv));
    }

    /// <summary>
    /// 测试无效Base64格式异常
    /// </summary>
    [TestMethod]
    public void DesCBCEncrypt_WithInvalidBase64_ShouldThrowFormatException()
    {
        // Act & Assert
        Assert.ThrowsExactly<FormatException>(() => DesCrypto.CBCEncrypt(TestContent, "invalid_base64", "invalid_base64"));
    }
    #endregion

    #region 边界条件测试
    /// <summary>
    /// 测试单字符加密解密
    /// </summary>
    [TestMethod]
    public void DesCBCEncryptDecrypt_SingleCharacter_ShouldReturnOriginalContent()
    {
        // Arrange
        (string key, string iv) = DesCrypto.GenerateCBCStringKey();
        string singleChar = "A";

        // Act
        string encrypted = DesCrypto.CBCEncrypt(singleChar, key, iv);
        string decrypted = DesCrypto.CBCDecrypt(encrypted, key, iv);

        // Assert
        Assert.AreEqual(singleChar, decrypted);
    }

    /// <summary>
    /// 测试8字节倍数数据加密解密
    /// </summary>
    [TestMethod]
    public void DesCBCEncryptDecrypt_Exact8ByteMultiple_ShouldReturnOriginalContent()
    {
        // Arrange
        (string key, string iv) = DesCrypto.GenerateCBCStringKey();
        string exact8Bytes = "12345678"; // 正好8字节

        // Act
        string encrypted = DesCrypto.CBCEncrypt(exact8Bytes, key, iv);
        string decrypted = DesCrypto.CBCDecrypt(encrypted, key, iv);

        // Assert
        Assert.AreEqual(exact8Bytes, decrypted);
    }

    /// <summary>
    /// 测试大数据加密解密
    /// </summary>
    [TestMethod]
    public void DesCBCEncryptDecrypt_LargeData_ShouldReturnOriginalContent()
    {
        // Arrange
        (string key, string iv) = DesCrypto.GenerateCBCStringKey();
        string largeData = string.Join("", Enumerable.Repeat("这是大数据测试内容。", 1000));

        // Act
        string encrypted = DesCrypto.CBCEncrypt(largeData, key, iv);
        string decrypted = DesCrypto.CBCDecrypt(encrypted, key, iv);

        // Assert
        Assert.AreEqual(largeData, decrypted);
    }
    #endregion

    #region 性能测试
    /// <summary>
    /// 测试DES加密解密性能
    /// </summary>
    [TestMethod]
    public void DesCBCEncryptDecrypt_PerformanceTest_ShouldCompleteInReasonableTime()
    {
        // Arrange
        (string key, string iv) = DesCrypto.GenerateCBCStringKey();
        int iterations = 100;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            string encrypted = DesCrypto.CBCEncrypt(TestContent, key, iv);
            DesCrypto.CBCDecrypt(encrypted, key, iv);
        }

        stopwatch.Stop();

        // Assert
        Assert.IsLessThan(10000, stopwatch.ElapsedMilliseconds,
            $"DES加密解密性能测试超时：{stopwatch.ElapsedMilliseconds}ms，期望小于10000ms");

        Console.WriteLine($"DES {iterations}次加密解密耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"平均每次操作: {(double)stopwatch.ElapsedMilliseconds / iterations:F2}ms");
    }
    #endregion
}
