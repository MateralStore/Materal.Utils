using Materal.Utils.Crypto;
using System.Text;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// MD5Crypto 测试类
/// </summary>
[TestClass]
public class MD5CryptoTest
{
    private const string TestString = "Hello, World! 你好世界！";
    private readonly string _expectedHash32Upper = "C0A4754AAA8ADC03CE0ACEF5DEE5F04D";
    private readonly string _expectedHash32Lower = "c0a4754aaa8adc03ce0acef5dee5f04d";
    private readonly string _expectedHash16Upper = "AA8ADC03CE0ACEF5";
    private readonly string _expectedHash16Lower = "aa8adc03ce0acef5";

    /// <summary>
    /// 测试32位MD5哈希（大写）
    /// </summary>
    [TestMethod]
    public void Hash32_String_Upper_Test()
    {
        // Act
        string result = MD5Crypto.Hash32(TestString);

        // Assert
        Assert.AreEqual(_expectedHash32Upper, result);
        Assert.AreEqual(32, result.Length);
    }

    /// <summary>
    /// 测试32位MD5哈希（小写）
    /// </summary>
    [TestMethod]
    public void Hash32_String_Lower_Test()
    {
        // Act
        string result = MD5Crypto.Hash32(TestString, true);

        // Assert
        Assert.AreEqual(_expectedHash32Lower, result);
        Assert.AreEqual(32, result.Length);
    }

    /// <summary>
    /// 测试16位MD5哈希（大写）
    /// </summary>
    [TestMethod]
    public void Hash16_String_Upper_Test()
    {
        // Act
        string result = MD5Crypto.Hash16(TestString);

        // Assert
        Assert.AreEqual(_expectedHash16Upper, result);
        Assert.AreEqual(16, result.Length);
    }

    /// <summary>
    /// 测试16位MD5哈希（小写）
    /// </summary>
    [TestMethod]
    public void Hash16_String_Lower_Test()
    {
        // Act
        string result = MD5Crypto.Hash16(TestString, true);

        // Assert
        Assert.AreEqual(_expectedHash16Lower, result);
        Assert.AreEqual(16, result.Length);
    }

    /// <summary>
    /// 测试字节数组32位MD5哈希
    /// </summary>
    [TestMethod]
    public void Hash32_Bytes_Test()
    {
        // Arrange
        byte[] data = Encoding.UTF8.GetBytes(TestString);

        // Act
        string result = MD5Crypto.Hash32(data);

        // Assert
        Assert.AreEqual(_expectedHash32Upper, result);
    }

    /// <summary>
    /// 测试字节数组16位MD5哈希
    /// </summary>
    [TestMethod]
    public void Hash16_Bytes_Test()
    {
        // Arrange
        byte[] data = Encoding.UTF8.GetBytes(TestString);

        // Act
        string result = MD5Crypto.Hash16(data);

        // Assert
        Assert.AreEqual(_expectedHash16Upper, result);
    }

    /// <summary>
    /// 测试流32位MD5哈希
    /// </summary>
    [TestMethod]
    public void Hash32_Stream_Test()
    {
        // Arrange
        byte[] data = Encoding.UTF8.GetBytes(TestString);
        using MemoryStream stream = new(data);

        // Act
        string result = MD5Crypto.Hash32(stream);

        // Assert
        Assert.AreEqual(_expectedHash32Upper, result);
    }

    /// <summary>
    /// 测试流16位MD5哈希
    /// </summary>
    [TestMethod]
    public void Hash16_Stream_Test()
    {
        // Arrange
        byte[] data = Encoding.UTF8.GetBytes(TestString);
        using MemoryStream stream = new(data);

        // Act
        string result = MD5Crypto.Hash16(stream);

        // Assert
        Assert.AreEqual(_expectedHash16Upper, result);
    }

    /// <summary>
    /// 测试空字符串哈希
    /// </summary>
    [TestMethod]
    public void EmptyString_Test()
    {
        // Act & Assert
        Assert.AreEqual("D41D8CD98F00B204E9800998ECF8427E", MD5Crypto.Hash32(""));
        Assert.AreEqual("8F00B204E9800998", MD5Crypto.Hash16(""));
    }

    /// <summary>
    /// 测试空字节数组哈希
    /// </summary>
    [TestMethod]
    public void EmptyBytes_Test()
    {
        // Arrange
        byte[] emptyData = [];

        // Act & Assert
        Assert.AreEqual("D41D8CD98F00B204E9800998ECF8427E", MD5Crypto.Hash32(emptyData));
        Assert.AreEqual("8F00B204E9800998", MD5Crypto.Hash16(emptyData));
    }

    /// <summary>
    /// 测试空流哈希
    /// </summary>
    [TestMethod]
    public void EmptyStream_Test()
    {
        // Arrange
        using MemoryStream emptyStream = new();

        // Act & Assert
        Assert.AreEqual("D41D8CD98F00B204E9800998ECF8427E", MD5Crypto.Hash32(emptyStream));
        Assert.AreEqual("8F00B204E9800998", MD5Crypto.Hash16(emptyStream));
    }

    /// <summary>
    /// 测试一致性 - 相同输入应产生相同哈希
    /// </summary>
    [TestMethod]
    public void Consistency_Test()
    {
        // Arrange
        string testString = "Consistency Test";

        // Act
        string hash1 = MD5Crypto.Hash32(testString);
        string hash2 = MD5Crypto.Hash32(testString);

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    /// <summary>
    /// 测试不同输入产生不同哈希
    /// </summary>
    [TestMethod]
    public void DifferentInput_DifferentHash_Test()
    {
        // Arrange
        string string1 = "Test String 1";
        string string2 = "Test String 2";

        // Act
        string hash1 = MD5Crypto.Hash32(string1);
        string hash2 = MD5Crypto.Hash32(string2);

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    /// <summary>
    /// 测试大小写敏感性
    /// </summary>
    [TestMethod]
    public void CaseSensitivity_Test()
    {
        // Arrange
        string lowerCase = "test";
        string upperCase = "Test";

        // Act
        string hash1 = MD5Crypto.Hash32(lowerCase);
        string hash2 = MD5Crypto.Hash32(upperCase);

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    /// <summary>
    /// 测试null参数
    /// </summary>
    [TestMethod]
    public void NullParameter_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => MD5Crypto.Hash32((string)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => MD5Crypto.Hash16((string)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => MD5Crypto.Hash32((byte[])null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => MD5Crypto.Hash16((byte[])null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => MD5Crypto.Hash32((Stream)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => MD5Crypto.Hash16((Stream)null!));
    }

    /// <summary>
    /// 测试不可读的流
    /// </summary>
    [TestMethod]
    public void NonReadableStream_Test()
    {
        // Arrange
        using MemoryStream stream = new();
        using Stream nonReadableStream = new NonReadableStream(stream);

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => MD5Crypto.Hash32(nonReadableStream));
        Assert.ThrowsExactly<ArgumentException>(() => MD5Crypto.Hash16(nonReadableStream));
    }

    /// <summary>
    /// 测试大文件哈希
    /// </summary>
    [TestMethod]
    public void LargeFile_Hash_Test()
    {
        // Arrange
        byte[] largeData = new byte[10 * 1024 * 1024]; // 10MB
        new Random().NextBytes(largeData);
        using MemoryStream stream = new(largeData);

        // Act
        string hash = MD5Crypto.Hash32(stream);

        // Assert
        Assert.IsNotNull(hash);
        Assert.AreEqual(32, hash.Length);
    }

    /// <summary>
    /// 不可读流的包装器（用于测试）
    /// </summary>
    private class NonReadableStream(Stream innerStream) : Stream
    {
        public override bool CanRead => false;
        public override bool CanSeek => innerStream.CanSeek;
        public override bool CanWrite => innerStream.CanWrite;
        public override long Length => innerStream.Length;
        public override long Position { get => innerStream.Position; set => innerStream.Position = value; }

        public override void Flush() => innerStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => innerStream.Seek(offset, origin);
        public override void SetLength(long value) => innerStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => innerStream.Write(buffer, offset, count);
    }

    /// <summary>
    /// 测试文件路径32位MD5哈希
    /// </summary>
    [TestMethod]
    public void Hash32_FilePath_Test()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, TestString);

            // Act
            string result = MD5Crypto.Hash32FromFile(tempFile);

            // Assert
            Assert.AreEqual(_expectedHash32Upper, result);
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
    /// 测试文件路径16位MD5哈希
    /// </summary>
    [TestMethod]
    public void Hash16_FilePath_Test()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, TestString);

            // Act
            string result = MD5Crypto.Hash16FromFile(tempFile);

            // Assert
            Assert.AreEqual(_expectedHash16Upper, result);
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
    /// 测试FileInfo 32位MD5哈希
    /// </summary>
    [TestMethod]
    public void Hash32_FileInfo_Test()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, TestString);
            FileInfo fileInfo = new(tempFile);

            // Act
            string result = MD5Crypto.Hash32FromFile(fileInfo);

            // Assert
            Assert.AreEqual(_expectedHash32Upper, result);
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
    /// 测试FileInfo 16位MD5哈希
    /// </summary>
    [TestMethod]
    public void Hash16_FileInfo_Test()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, TestString);
            FileInfo fileInfo = new(tempFile);

            // Act
            string result = MD5Crypto.Hash16FromFile(fileInfo);

            // Assert
            Assert.AreEqual(_expectedHash16Upper, result);
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
    /// 测试空文件哈希
    /// </summary>
    [TestMethod]
    public void EmptyFile_Hash_Test()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            // Act
            string hash32 = MD5Crypto.Hash32FromFile(tempFile);
            string hash16 = MD5Crypto.Hash16FromFile(tempFile);

            // Assert
            Assert.AreEqual("D41D8CD98F00B204E9800998ECF8427E", hash32);
            Assert.AreEqual("8F00B204E9800998", hash16);
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
    /// 测试文件哈希与字符串哈希的一致性
    /// </summary>
    [TestMethod]
    public void File_String_Consistency_Test()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, TestString);

            // Act
            string fileHash = MD5Crypto.Hash32FromFile(tempFile);
            string stringHash = MD5Crypto.Hash32(TestString);

            // Assert
            Assert.AreEqual(stringHash, fileHash);
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
    /// 测试文件哈希与流哈希的一致性
    /// </summary>
    [TestMethod]
    public void File_Stream_Consistency_Test()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, TestString);
            using FileStream stream = File.OpenRead(tempFile);

            // Act
            string fileHash = MD5Crypto.Hash32FromFile(tempFile);
            string streamHash = MD5Crypto.Hash32(stream);

            // Assert
            Assert.AreEqual(streamHash, fileHash);
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
    /// 测试不存在的文件路径
    /// </summary>
    [TestMethod]
    public void NonExistentFile_Test()
    {
        // Arrange
        string nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");

        // Act & Assert
        Assert.ThrowsExactly<FileNotFoundException>(() => MD5Crypto.Hash32FromFile(nonExistentFile));
        Assert.ThrowsExactly<FileNotFoundException>(() => MD5Crypto.Hash16FromFile(nonExistentFile));
    }

    /// <summary>
    /// 测试null文件路径
    /// </summary>
    [TestMethod]
    public void NullFilePath_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => MD5Crypto.Hash32FromFile((string)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => MD5Crypto.Hash16FromFile((string)null!));
    }

    /// <summary>
    /// 测试空文件路径
    /// </summary>
    [TestMethod]
    public void EmptyFilePath_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => MD5Crypto.Hash32FromFile(""));
        Assert.ThrowsExactly<ArgumentException>(() => MD5Crypto.Hash16FromFile(""));
        Assert.ThrowsExactly<ArgumentException>(() => MD5Crypto.Hash32FromFile("   "));
        Assert.ThrowsExactly<ArgumentException>(() => MD5Crypto.Hash16FromFile("   "));
    }

    /// <summary>
    /// 测试null FileInfo
    /// </summary>
    [TestMethod]
    public void NullFileInfo_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => MD5Crypto.Hash32FromFile((FileInfo)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => MD5Crypto.Hash16FromFile((FileInfo)null!));
    }

    /// <summary>
    /// 测试不存在的FileInfo
    /// </summary>
    [TestMethod]
    public void NonExistentFileInfo_Test()
    {
        // Arrange
        FileInfo nonExistentFile = new(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp"));

        // Act & Assert
        Assert.ThrowsExactly<FileNotFoundException>(() => MD5Crypto.Hash32FromFile(nonExistentFile));
        Assert.ThrowsExactly<FileNotFoundException>(() => MD5Crypto.Hash16FromFile(nonExistentFile));
    }

    /// <summary>
    /// 测试大文件哈希（文件版本）
    /// </summary>
    [TestMethod]
    public void LargeFile_Hash_File_Test()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            // 创建 5MB 的测试文件
            byte[] data = new byte[5 * 1024 * 1024];
            new Random(42).NextBytes(data);
            File.WriteAllBytes(tempFile, data);

            // Act
            string hash = MD5Crypto.Hash32FromFile(tempFile);

            // Assert
            Assert.IsNotNull(hash);
            Assert.AreEqual(32, hash.Length);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}
