using Materal.Utils.Crypto;
using System.Security.Cryptography;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// AES文件加解密测试
/// </summary>
[TestClass]
[DoNotParallelize]
public class AesFileCryptoTest
{
    private readonly string _testBaseDirectory = Path.Combine(Path.GetTempPath(), "AesFileCryptoTest");
    private readonly string _testContent = "这是一个测试文件内容，用于验证AES文件加解密功能。\nThis is a test file content for verifying AES file encryption and decryption.\n包含中文和English混合内容。";
    private string _testDirectory = string.Empty;

    [TestInitialize]
    public void Init()
    {
        // 为每个测试创建独立的目录
        _testDirectory = Path.Combine(_testBaseDirectory, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testDirectory);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // 清理测试目录
        if (!Directory.Exists(_testDirectory)) return;
        try
        {
            // 先设置所有文件为正常属性
            foreach (string file in Directory.GetFiles(_testDirectory, "*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
            // 删除目录
            Directory.Delete(_testDirectory, true);
        }
        catch
        {
            // 忽略清理错误
        }
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        // 清理基础测试目录
        string testBaseDirectory = Path.Combine(Path.GetTempPath(), "AesFileCryptoTest");
        if (!Directory.Exists(testBaseDirectory)) return;
        try
        {
            Directory.Delete(testBaseDirectory, true);
        }
        catch
        {
            // 忽略清理错误
        }
    }

    #region Aes-CBC 文件加解密测试

    /// <summary>
    /// Aes-CBC文件加解密测试 - 使用FileInfo
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesCBCFileEncryptDecrypt_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备测试文件
        string inputFile = Path.Combine(_testDirectory, "input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "decrypted.txt");

        File.WriteAllText(inputFile, _testContent, Encoding.UTF8);

        // 生成密钥和IV
        (string key, string iv) = AesCrypto.GenerateAesCBCStringKey();

        // 加密文件
        FileInfo inputFileInfo = new(inputFile);
        FileInfo encryptedFileInfo = new(encryptedFile);
        AesCrypto.AesCBCEncryptFile(inputFileInfo, encryptedFileInfo, key, iv);

        Assert.IsTrue(File.Exists(encryptedFile));
        Assert.AreNotEqual(File.ReadAllBytes(inputFile), File.ReadAllBytes(encryptedFile));

        // 解密文件
        FileInfo decryptedFileInfo = new(decryptedFile);
        AesCrypto.AesCBCDecryptFile(encryptedFileInfo, decryptedFileInfo, key, iv);

        // 验证解密结果
        Assert.IsTrue(File.Exists(decryptedFile));
        string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
        Assert.AreEqual(_testContent, decryptedContent);
    }

    /// <summary>
    /// Aes-CBC文件加解密测试 - 使用FilePath
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesCBCFileEncryptDecryptByPath_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备测试文件
        string inputFile = Path.Combine(_testDirectory, "input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "decrypted.txt");

        File.WriteAllText(inputFile, _testContent, Encoding.UTF8);

        // 生成密钥和IV
        (string key, string iv) = AesCrypto.GenerateAesCBCStringKey();

        // 加密文件
        AesCrypto.AesCBCEncryptFileByPath(inputFile, encryptedFile, key, iv);

        Assert.IsTrue(File.Exists(encryptedFile));
        Assert.AreNotEqual(File.ReadAllBytes(inputFile), File.ReadAllBytes(encryptedFile));

        // 解密文件
        AesCrypto.AesCBCDecryptFileByPath(encryptedFile, decryptedFile, key, iv);

        // 验证解密结果
        Assert.IsTrue(File.Exists(decryptedFile));
        string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
        Assert.AreEqual(_testContent, decryptedContent);
    }

    /// <summary>
    /// Aes-CBC文件加解密测试 - 自动生成密钥和IV
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesCBCFileEncryptDecryptWithGeneratedKey_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备测试文件
        string inputFile = Path.Combine(_testDirectory, "input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "decrypted.txt");

        File.WriteAllText(inputFile, _testContent, Encoding.UTF8);

        // 加密文件（自动生成密钥和IV）
        FileInfo inputFileInfo = new(inputFile);
        FileInfo encryptedFileInfo = new(encryptedFile);
        AesCrypto.AesCBCEncryptFile(inputFileInfo, encryptedFileInfo, out string key, out string iv);

        Assert.IsNotNull(key);
        Assert.IsNotNull(iv);
        Assert.IsTrue(File.Exists(encryptedFile));

        // 解密文件
        FileInfo decryptedFileInfo = new(decryptedFile);
        AesCrypto.AesCBCDecryptFile(encryptedFileInfo, decryptedFileInfo, key, iv);

        // 验证解密结果
        string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
        Assert.AreEqual(_testContent, decryptedContent);
    }

    /// <summary>
    /// Aes-CBC文件加解密测试 - 使用随机IV
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesCBCFileEncryptDecryptWithRandomIV_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备测试文件
        string inputFile = Path.Combine(_testDirectory, "input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "decrypted.txt");

        File.WriteAllText(inputFile, _testContent, Encoding.UTF8);

        // 生成密钥
        string key = AesCrypto.GenerateAesGCMStringKey();

        // 加密文件（使用随机IV）
        FileInfo inputFileInfo = new(inputFile);
        FileInfo encryptedFileInfo = new(encryptedFile);
        AesCrypto.AesCBCEncryptFile(inputFileInfo, encryptedFileInfo, key);

        Assert.IsTrue(File.Exists(encryptedFile));

        // 验证IV已前置到文件中
        byte[] iv;
        using (FileStream encryptedStream = new(encryptedFile, FileMode.Open))
        {
            iv = new byte[16];
            int bytesRead = encryptedStream.Read(iv, 0, 16);
            Assert.AreEqual(16, bytesRead);
        }

        // 解密文件（自动提取IV）
        FileInfo decryptedFileInfo = new(decryptedFile);
        AesCrypto.AesCBCDecryptFile(encryptedFileInfo, decryptedFileInfo, key);

        // 验证解密结果
        string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
        Assert.AreEqual(_testContent, decryptedContent);
    }

    /// <summary>
    /// Aes-CBC大文件加解密测试
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesCBCLargeFileEncryptDecrypt_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备大文件（10MB）
        string inputFile = Path.Combine(_testDirectory, "large_input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "large_encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "large_decrypted.txt");

        // 生成大文件内容
        StringBuilder sb = new();
        for (int i = 0; i < 100000; i++)
        {
            sb.AppendLine($"这是第{i}行测试内容，包含随机数：{Random.Shared.Next(1, 1000000)}");
        }
        string largeContent = sb.ToString();
        File.WriteAllText(inputFile, largeContent, Encoding.UTF8);

        // 生成密钥和IV
        (string key, string iv) = AesCrypto.GenerateAesCBCStringKey();

        // 加密文件
        AesCrypto.AesCBCEncryptFileByPath(inputFile, encryptedFile, key, iv);

        // 解密文件
        AesCrypto.AesCBCDecryptFileByPath(encryptedFile, decryptedFile, key, iv);

        // 验证解密结果
        string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
        Assert.AreEqual(largeContent, decryptedContent);

        // 验证文件大小
        Assert.AreEqual(new FileInfo(inputFile).Length, new FileInfo(decryptedFile).Length);
    }

    /// <summary>
    /// Aes-CBC二进制文件加解密测试
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesCBCBinaryFileEncryptDecrypt_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备二进制文件
        string inputFile = Path.Combine(_testDirectory, "input.bin");
        string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "decrypted.bin");

        // 生成随机二进制数据
        byte[] originalData = new byte[1024 * 10]; // 10KB
        Random.Shared.NextBytes(originalData);
        File.WriteAllBytes(inputFile, originalData);

        // 生成密钥和IV
        (string key, string iv) = AesCrypto.GenerateAesCBCStringKey();

        // 加密文件
        AesCrypto.AesCBCEncryptFileByPath(inputFile, encryptedFile, key, iv);

        // 解密文件
        AesCrypto.AesCBCDecryptFileByPath(encryptedFile, decryptedFile, key, iv);

        // 验证解密结果
        byte[] decryptedData = File.ReadAllBytes(decryptedFile);
        CollectionAssert.AreEqual(originalData, decryptedData);
    }

    #endregion

#if NET
    #region Aes-GCM 文件加解密测试

    /// <summary>
    /// Aes-GCM文件加解密测试 - 使用FileInfo
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesGCMFileEncryptDecrypt_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备测试文件
        string inputFile = Path.Combine(_testDirectory, "input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "decrypted.txt");

        File.WriteAllText(inputFile, _testContent, Encoding.UTF8);

        // 生成密钥
        string key = AesCrypto.GenerateAesGCMStringKey();

        // 加密文件
        FileInfo inputFileInfo = new(inputFile);
        FileInfo encryptedFileInfo = new(encryptedFile);
        AesCrypto.AesGCMEncryptFile(inputFileInfo, encryptedFileInfo, key);

        Assert.IsTrue(File.Exists(encryptedFile));
        Assert.AreNotEqual(File.ReadAllBytes(inputFile), File.ReadAllBytes(encryptedFile));

        // 解密文件
        FileInfo decryptedFileInfo = new(decryptedFile);
        AesCrypto.AesGCMDecryptFile(encryptedFileInfo, decryptedFileInfo, key);

        // 验证解密结果
        Assert.IsTrue(File.Exists(decryptedFile));
        string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
        Assert.AreEqual(_testContent, decryptedContent);
    }

    /// <summary>
    /// Aes-GCM文件加解密测试 - 使用FilePath
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesGCMFileEncryptDecryptByPath_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备测试文件
        string inputFile = Path.Combine(_testDirectory, "input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "decrypted.txt");

        File.WriteAllText(inputFile, _testContent, Encoding.UTF8);

        // 生成密钥
        string key = AesCrypto.GenerateAesGCMStringKey();

        // 加密文件
        AesCrypto.AesGCMEncryptFileByPath(inputFile, encryptedFile, key);

        Assert.IsTrue(File.Exists(encryptedFile));

        // 解密文件
        AesCrypto.AesGCMDecryptFileByPath(encryptedFile, decryptedFile, key);

        // 验证解密结果
        string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
        Assert.AreEqual(_testContent, decryptedContent);
    }

    /// <summary>
    /// Aes-GCM文件加解密测试 - 自动生成密钥和nonce
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesGCMFileEncryptDecryptWithGeneratedKey_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备测试文件
        string inputFile = Path.Combine(_testDirectory, "input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "decrypted.txt");

        File.WriteAllText(inputFile, _testContent, Encoding.UTF8);

        // 加密文件（自动生成密钥和nonce）
        FileInfo inputFileInfo = new(inputFile);
        FileInfo encryptedFileInfo = new(encryptedFile);
        AesCrypto.AesGCMEncryptFile(inputFileInfo, encryptedFileInfo, out string key, out string nonce);

        Assert.IsNotNull(key);
        Assert.IsNotNull(nonce);
        Assert.IsTrue(File.Exists(encryptedFile));

        // 解密文件（使用单独的nonce）
        FileInfo decryptedFileInfo = new(decryptedFile);
        AesCrypto.AesGCMDecryptFile(encryptedFileInfo, decryptedFileInfo, key, nonce);

        // 验证解密结果
        string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
        Assert.AreEqual(_testContent, decryptedContent);
    }

    /// <summary>
    /// Aes-GCM文件加解密测试 - 使用单独的nonce
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesGCMFileEncryptDecryptWithSeparateNonce_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备测试文件
        string inputFile = Path.Combine(_testDirectory, "input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "decrypted.txt");

        File.WriteAllText(inputFile, _testContent, Encoding.UTF8);

        // 生成nonce
        byte[] nonceBytes = new byte[12];
        RandomNumberGenerator.Fill(nonceBytes);

        // 加密文件（使用单独的nonce）
        AesCrypto.AesGCMEncryptFileByPath(inputFile, encryptedFile, out string generatedKey, out string generatedNonce);

        // 解密文件（使用生成的nonce）
        AesCrypto.AesGCMDecryptFileByPath(encryptedFile, decryptedFile, generatedKey, generatedNonce);

        // 验证解密结果
        string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
        Assert.AreEqual(_testContent, decryptedContent);
    }

    /// <summary>
    /// Aes-GCM大文件加解密测试
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesGCMLargeFileEncryptDecrypt_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 添加短暂延迟确保目录创建完成
        Thread.Sleep(10);

        // 准备大文件（5MB）
        string inputFile = Path.Combine(_testDirectory, "large_input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "large_encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "large_decrypted.txt");

        // 生成大文件内容
        StringBuilder sb = new();
        for (int i = 0; i < 50000; i++)
        {
            sb.AppendLine($"这是第{i}行测试内容，包含随机数：{Random.Shared.Next(1, 1000000)}");
        }
        string largeContent = sb.ToString();
        File.WriteAllText(inputFile, largeContent, Encoding.UTF8);

        // 确保文件完全写入
        using (FileStream fs = new(inputFile, FileMode.Open, FileAccess.ReadWrite))
        {
            fs.Flush();
        }

        // 强制垃圾回收以确保文件句柄释放
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // 生成密钥
        string key = AesCrypto.GenerateAesGCMStringKey();

        // 加密文件
        AesCrypto.AesGCMEncryptFileByPath(inputFile, encryptedFile, key);

        // 解密文件
        AesCrypto.AesGCMDecryptFileByPath(encryptedFile, decryptedFile, key);

        // 验证解密结果
        string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
        Assert.AreEqual(largeContent, decryptedContent);

        // 验证文件大小
        Assert.AreEqual(new FileInfo(inputFile).Length, new FileInfo(decryptedFile).Length);
    }

    /// <summary>
    /// Aes-GCM文件完整性验证测试
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void AesGCMFileTamperedData_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // 准备测试文件
        string inputFile = Path.Combine(_testDirectory, "input.txt");
        string encryptedFile = Path.Combine(_testDirectory, "encrypted.bin");
        string decryptedFile = Path.Combine(_testDirectory, "decrypted.txt");

        File.WriteAllText(inputFile, _testContent, Encoding.UTF8);

        // 生成密钥
        string key = AesCrypto.GenerateAesGCMStringKey();

        // 加密文件
        AesCrypto.AesGCMEncryptFileByPath(inputFile, encryptedFile, key);

        // 篡改加密文件（修改中间的一个字节）
        byte[] encryptedData = File.ReadAllBytes(encryptedFile);
        encryptedData[50] ^= 0x01; // 翻转一个位
        File.WriteAllBytes(encryptedFile, encryptedData);

        // 尝试解密（应该抛出异常）
        Assert.ThrowsExactly<AuthenticationTagMismatchException>(() =>
            AesCrypto.AesGCMDecryptFileByPath(encryptedFile, decryptedFile, key));
    }

    #endregion
#endif

    #region 异常测试

    /// <summary>
    /// 空文件路径测试
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void EmptyFilePath_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesCBCEncryptFileByPath("", "output.bin", "key", "iv"));
    }

    /// <summary>
    /// 文件不存在测试
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void FileNotExists_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        string inputFile = Path.Combine(_testDirectory, "not_exist.txt");
        string outputFile = Path.Combine(_testDirectory, "output.bin");

        Assert.ThrowsExactly<FileNotFoundException>(() =>
            AesCrypto.AesCBCEncryptFileByPath(inputFile, outputFile, "key", "iv"));
    }

    /// <summary>
    /// 无效密钥测试
    /// </summary>
    [TestMethod]
    [DoNotParallelize]
    public void InvalidKey_Test()
    {
        // 确保测试目录存在
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        string inputFile = Path.Combine(_testDirectory, "input.txt");
        string outputFile = Path.Combine(_testDirectory, "output.bin");

        File.WriteAllText(inputFile, "test");

        Assert.ThrowsExactly<FormatException>(() =>
            AesCrypto.AesCBCEncryptFileByPath(inputFile, outputFile, "invalid_key", "invalid_iv"));
    }
    #endregion
}
