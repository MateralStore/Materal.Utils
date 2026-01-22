using Materal.Utils.Crypto;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// SHA256加密测试
/// </summary>
[TestClass]
public class SHA256CryptoTest
{
    /// <summary>
    /// 测试字符串哈希
    /// </summary>
    [TestMethod]
    public void Hash_String_Test()
    {
        // Arrange
        string input = "Hello World";
        string expected = "A591A6D40BF420404A011733CFB7B190D62C65BF0BCDA32B57B277D9AD9F146E";

        // Act
        string result = SHA256Crypto.Hash(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// 测试字符串哈希（小写）
    /// </summary>
    [TestMethod]
    public void Hash_String_Lower_Test()
    {
        // Arrange
        string input = "Hello World";
        string expected = "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e";

        // Act
        string result = SHA256Crypto.Hash(input, true);

        // Assert
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// 测试字节数组哈希
    /// </summary>
    [TestMethod]
    public void Hash_Bytes_Test()
    {
        // Arrange
        byte[] input = Encoding.UTF8.GetBytes("Hello World");
        string expected = "A591A6D40BF420404A011733CFB7B190D62C65BF0BCDA32B57B277D9AD9F146E";

        // Act
        string result = SHA256Crypto.Hash(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// 测试空字符串哈希
    /// </summary>
    [TestMethod]
    public void Hash_EmptyString_Test()
    {
        // Arrange
        string input = "";
        string expected = "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855";

        // Act
        string result = SHA256Crypto.Hash(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// 测试null参数异常
    /// </summary>
    [TestMethod]
    public void Hash_NullString_ThrowArgumentNullException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => SHA256Crypto.Hash((string)null!));
    }

    /// <summary>
    /// 测试null字节数组异常
    /// </summary>
    [TestMethod]
    public void Hash_NullBytes_ThrowArgumentNullException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => SHA256Crypto.Hash((byte[])null!));
    }

    /// <summary>
    /// 测试null流异常
    /// </summary>
    [TestMethod]
    public void Hash_NullStream_ThrowArgumentNullException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => SHA256Crypto.Hash((Stream)null!));
    }

    /// <summary>
    /// 测试不可读流异常
    /// </summary>
    [TestMethod]
    public void Hash_UnreadableStream_ThrowArgumentException_Test()
    {
        // Arrange
        using MemoryStream stream = new();
        stream.Close(); // 关闭流使其不可读

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => SHA256Crypto.Hash(stream));
    }

    /// <summary>
    /// 测试流哈希
    /// </summary>
    [TestMethod]
    public void Hash_Stream_Test()
    {
        // Arrange
        byte[] input = Encoding.UTF8.GetBytes("Hello World");
        string expected = "A591A6D40BF420404A011733CFB7B190D62C65BF0BCDA32B57B277D9AD9F146E";
        using MemoryStream stream = new(input);

        // Act
        string result = SHA256Crypto.Hash(stream);

        // Assert
        Assert.AreEqual(expected, result);
    }

    #region 文件哈希测试

    /// <summary>
    /// 测试文件路径哈希
    /// </summary>
    [TestMethod]
    public void HashFromFile_FilePath_Test()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Hello World");
            string expected = "A591A6D40BF420404A011733CFB7B190D62C65BF0BCDA32B57B277D9AD9F146E";

            // Act
            string result = SHA256Crypto.HashFromFile(tempFile);

            // Assert
            Assert.AreEqual(expected, result);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    /// 测试FileInfo哈希
    /// </summary>
    [TestMethod]
    public void HashFromFile_FileInfo_Test()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Hello World");
            FileInfo fileInfo = new(tempFile);
            string expected = "A591A6D40BF420404A011733CFB7B190D62C65BF0BCDA32B57B277D9AD9F146E";

            // Act
            string result = SHA256Crypto.HashFromFile(fileInfo);

            // Assert
            Assert.AreEqual(expected, result);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    /// 测试不存在文件异常
    /// </summary>
    [TestMethod]
    public void HashFromFile_NonExistentFile_ThrowFileNotFoundException_Test()
    {
        // Arrange
        string nonExistentFile = "C:\\NonExistentFile.txt";

        // Act & Assert
        Assert.ThrowsExactly<FileNotFoundException>(() => SHA256Crypto.HashFromFile(nonExistentFile));
    }

    /// <summary>
    /// 测试null文件路径异常
    /// </summary>
    [TestMethod]
    public void HashFromFile_NullFilePath_ThrowArgumentNullException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => SHA256Crypto.HashFromFile((string)null!));
    }

    /// <summary>
    /// 测试null FileInfo异常
    /// </summary>
    [TestMethod]
    public void HashFromFile_NullFileInfo_ThrowArgumentNullException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => SHA256Crypto.HashFromFile((FileInfo)null!));
    }

    #endregion
}
