using Materal.Utils.Crypto;
using System.Security.Cryptography;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// AES 加密解密工具测试类
/// 测试 AES-CBC 和 AES-GCM 两种模式的加密解密功能
/// </summary>
[TestClass]
public partial class AesStreamCryptoTest
{
    private readonly Encoding _encoding = Encoding.UTF8;

    #region 私有方法
    /// <summary>
    /// 生成测试用的随机数据
    /// </summary>
    private static byte[] GenerateRandomData(int size)
    {
        byte[] data = new byte[size];
        RandomNumberGenerator.Fill(data);
        return data;
    }

    /// <summary>
    /// 比较两个流的内容是否相同
    /// </summary>
    private static bool StreamsEqual(Stream stream1, Stream stream2)
    {
        if (stream1.Length != stream2.Length)
            return false;

        stream1.Position = 0;
        stream2.Position = 0;

        byte[] buffer1 = new byte[4096];
        byte[] buffer2 = new byte[4096];
        int bytesRead1, bytesRead2;

        while ((bytesRead1 = stream1.Read(buffer1, 0, buffer1.Length)) > 0)
        {
            bytesRead2 = stream2.Read(buffer2, 0, buffer2.Length);
            if (bytesRead1 != bytesRead2)
                return false;

            for (int i = 0; i < bytesRead1; i++)
            {
                if (buffer1[i] != buffer2[i])
                    return false;
            }
        }

        return true;
    }
    #endregion

    #region Aes-CBC 流加解密测试
    /// <summary>
    /// 测试Aes-CBC流加密解密（指定密钥和IV）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStream_Test()
    {
        // 准备测试数据
        byte[] originalData = GenerateRandomData(1024 * 100); // 100KB随机数据
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456")); // 16字节
        string iv = Convert.ToBase64String(_encoding.GetBytes("abcdefghijklmnop")); // 16字节

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 加密
        long encryptedBytes = AesCrypto.AesCBCEncryptStream(inputStream, encryptedStream, key, iv);
        Assert.AreEqual(originalData.Length, encryptedBytes, "加密字节数应与原始数据相同");

        // 解密
        encryptedStream.Position = 0;
        long decryptedBytes = AesCrypto.AesCBCDecryptStream(encryptedStream, decryptedStream, key, iv);
        Assert.AreEqual(encryptedBytes, decryptedBytes, "解密字节数应与加密字节数相同");

        // 验证解密结果与原文相同
        CollectionAssert.AreEqual(originalData, decryptedStream.ToArray(), "解密结果应与原文相同");
    }

    /// <summary>
    /// 测试Aes-CBC流加密解密（使用随机IV）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStreamWithRandomIV_Test()
    {
        // 准备测试数据
        byte[] originalData = _encoding.GetBytes("测试使用随机IV的流式加密解密功能！@#￥%……&*");
        string key = Convert.ToBase64String(_encoding.GetBytes("123456789012345678901234")); // 24字节

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 加密（使用随机IV）
        var (cryptoStream, iv) = AesCrypto.CreateAesCBCEncryptStream(encryptedStream, key);
        inputStream.CopyTo(cryptoStream);
        cryptoStream.FlushFinalBlock();
        cryptoStream.Close();

        // 解密（自动提取IV）
        encryptedStream.Position = 0;
        using AesCrypto.AesCBCDecryptStreamWrapper decryptStream = AesCrypto.CreateAesCBCDecryptStream(encryptedStream, key);
        decryptStream.CopyTo(decryptedStream);

        // 验证解密结果与原文相同
        CollectionAssert.AreEqual(originalData, decryptedStream.ToArray(), "解密结果应与原文相同");
    }

    /// <summary>
    /// 测试Aes-CBC流加密解密（大文件）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStreamLargeFile_Test()
    {
        // 准备测试数据（10MB）
        byte[] originalData = GenerateRandomData(1024 * 1024 * 10);
        string key = Convert.ToBase64String(_encoding.GetBytes("12345678901234567890123456789012")); // 32字节
        string iv = Convert.ToBase64String(_encoding.GetBytes("abcdefghijklmnop")); // 16字节

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 使用较小的缓冲区测试流式处理
        const int bufferSize = 4096;

        // 加密
        long encryptedBytes = AesCrypto.AesCBCEncryptStream(inputStream, encryptedStream, key, iv, bufferSize);
        Assert.AreEqual(originalData.Length, encryptedBytes);

        // 解密
        encryptedStream.Position = 0;
        long decryptedBytes = AesCrypto.AesCBCDecryptStream(encryptedStream, decryptedStream, key, iv, bufferSize);
        Assert.AreEqual(encryptedBytes, decryptedBytes);

        // 验证解密结果
        CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
    }

    /// <summary>
    /// 测试Aes-CBC流加密解密（空流）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStreamEmpty_Test()
    {
        byte[] originalData = [];
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        string iv = Convert.ToBase64String(_encoding.GetBytes("abcdefghijklmnop"));

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 加密空流
        long encryptedBytes = AesCrypto.AesCBCEncryptStream(inputStream, encryptedStream, key, iv);
        Assert.AreEqual(0, encryptedBytes);

        // 解密空流
        encryptedStream.Position = 0;
        long decryptedBytes = AesCrypto.AesCBCDecryptStream(encryptedStream, decryptedStream, key, iv);
        Assert.AreEqual(0, decryptedBytes);

        // 验证结果
        CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
    }

    /// <summary>
    /// 测试Aes-CBC流加密（null参数异常）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptStreamNullParameters_Test()
    {
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        string iv = Convert.ToBase64String(_encoding.GetBytes("abcdefghijklmnop"));
        byte[] keyBytes = _encoding.GetBytes("1234567890123456");
        byte[] ivBytes = _encoding.GetBytes("abcdefghijklmnop");

        using MemoryStream stream = new();

        // 测试null输入流
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            AesCrypto.AesCBCEncryptStream(null!, stream, key, iv));

        // 测试null输出流
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            AesCrypto.AesCBCEncryptStream(stream, null!, key, iv));

        // 测试null密钥
        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesCBCEncryptStream(stream, stream, null!, iv));

        // 测试null IV
        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesCBCEncryptStream(stream, stream, key, null!));

        // 测试字节数组版本
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            AesCrypto.AesCBCEncryptStream(null!, stream, keyBytes, ivBytes));

        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesCBCEncryptStream(stream, stream, null!, ivBytes));
    }

    /// <summary>
    /// 测试Aes-CBC流加密解密（无效密钥长度）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStreamInvalidKeyLength_Test()
    {
        byte[] data = GenerateRandomData(100);
        string iv = Convert.ToBase64String(_encoding.GetBytes("abcdefghijklmnop"));

        // 测试无效的密钥长度
        string invalidKey1 = Convert.ToBase64String(_encoding.GetBytes("12345678")); // 8字节
        string invalidKey2 = Convert.ToBase64String(_encoding.GetBytes("12345678901234567")); // 17字节

        using MemoryStream inputStream = new(data);
        using MemoryStream outputStream = new();

        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesCBCEncryptStream(inputStream, outputStream, invalidKey1, iv));

        inputStream.Position = 0;
        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesCBCEncryptStream(inputStream, outputStream, invalidKey2, iv));
    }

    /// <summary>
    /// 测试Aes-CBC流加密解密（无效IV长度）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStreamInvalidIVLength_Test()
    {
        byte[] data = GenerateRandomData(100);
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));

        // 测试无效的IV长度
        string invalidIV1 = Convert.ToBase64String(_encoding.GetBytes("12345678")); // 8字节
        string invalidIV2 = Convert.ToBase64String(_encoding.GetBytes("12345678901234567")); // 17字节

        using MemoryStream inputStream = new(data);
        using MemoryStream outputStream = new();

        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesCBCEncryptStream(inputStream, outputStream, key, invalidIV1));

        inputStream.Position = 0;
        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesCBCEncryptStream(inputStream, outputStream, key, invalidIV2));
    }

    /// <summary>
    /// 测试Aes-CBC流加密解密（错误的密钥或IV）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStreamWrongKeyOrIV_Test()
    {
        byte[] originalData = GenerateRandomData(1000);
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        string iv = Convert.ToBase64String(_encoding.GetBytes("abcdefghijklmnop"));
        string wrongKey = Convert.ToBase64String(_encoding.GetBytes("6543210987654321"));
        string wrongIV = Convert.ToBase64String(_encoding.GetBytes("ponmlkjihgfedcba"));

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 正确加密
        AesCrypto.AesCBCEncryptStream(inputStream, encryptedStream, key, iv);

        // 使用错误的密钥解密应该抛出异常
        encryptedStream.Position = 0;
        Assert.ThrowsExactly<CryptographicException>(() =>
            AesCrypto.AesCBCDecryptStream(encryptedStream, decryptedStream, wrongKey, iv));

        // 使用错误的IV解密 - 注意：可能不会抛出异常，但解密结果应该与原文不同
        decryptedStream.SetLength(0);
        encryptedStream.Position = 0;
        try
        {
            AesCrypto.AesCBCDecryptStream(encryptedStream, decryptedStream, key, wrongIV);
            // 如果没有抛出异常，检查解密结果是否与原文不同
            byte[] decryptedData = decryptedStream.ToArray();
            CollectionAssert.AreNotEqual(originalData, decryptedData, "使用错误的IV解密，结果应该与原文不同");
        }
        catch (CryptographicException)
        {
            // 如果抛出异常，这也是预期的行为
        }
    }

    /// <summary>
    /// 测试Aes-CBC流加密解密（流不可读/写异常）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStreamStreamNotReadableOrWritable_Test()
    {
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        string iv = Convert.ToBase64String(_encoding.GetBytes("abcdefghijklmnop"));

        // 创建不可读的流
        using MemoryStream notReadableStream = new();
        notReadableStream.Close();

        // 创建不可写的流
        using MemoryStream notWritableStream = new();
        notWritableStream.Close();

        using MemoryStream readableStream = new();
        using MemoryStream writableStream = new();

        // 测试不可读的输入流
        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesCBCEncryptStream(notReadableStream, writableStream, key, iv));

        // 测试不可写的输出流
        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesCBCEncryptStream(readableStream, notWritableStream, key, iv));
    }

    /// <summary>
    /// 测试Aes-CBC流加密解密（不同缓冲区大小）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStreamDifferentBufferSizes_Test()
    {
        byte[] originalData = GenerateRandomData(1024 * 10); // 10KB
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        string iv = Convert.ToBase64String(_encoding.GetBytes("abcdefghijklmnop"));

        int[] bufferSizes = [1, 16, 64, 256, 1024, 8192, 16384];

        foreach (int bufferSize in bufferSizes)
        {
            using MemoryStream inputStream = new(originalData);
            using MemoryStream encryptedStream = new();
            using MemoryStream decryptedStream = new();

            // 加密
            long encryptedBytes = AesCrypto.AesCBCEncryptStream(inputStream, encryptedStream, key, iv, bufferSize);

            // 解密
            encryptedStream.Position = 0;
            long decryptedBytes = AesCrypto.AesCBCDecryptStream(encryptedStream, decryptedStream, key, iv, bufferSize);

            // 验证
            Assert.AreEqual(originalData.Length, encryptedBytes, $"缓冲区大小{bufferSize}时加密字节数不匹配");
            Assert.AreEqual(encryptedBytes, decryptedBytes, $"缓冲区大小{bufferSize}时解密字节数不匹配");
            CollectionAssert.AreEqual(originalData, decryptedStream.ToArray(), $"缓冲区大小{bufferSize}时解密结果不匹配");
        }
    }

    /// <summary>
    /// 测试Aes-CBC流加密解密（不同密钥长度）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStreamDifferentKeyLengths_Test()
    {
        byte[] originalData = GenerateRandomData(1024);
        string iv = Convert.ToBase64String(_encoding.GetBytes("abcdefghijklmnop"));

        // 测试16字节密钥（AES-128）
        string key128 = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        using (MemoryStream inputStream = new(originalData))
        using (MemoryStream encryptedStream = new())
        using (MemoryStream decryptedStream = new())
        {
            AesCrypto.AesCBCEncryptStream(inputStream, encryptedStream, key128, iv);
            encryptedStream.Position = 0;
            AesCrypto.AesCBCDecryptStream(encryptedStream, decryptedStream, key128, iv);
            CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
        }

        // 测试24字节密钥（AES-192）
        string key192 = Convert.ToBase64String(_encoding.GetBytes("123456789012345678901234"));
        using (MemoryStream inputStream = new(originalData))
        using (MemoryStream encryptedStream = new())
        using (MemoryStream decryptedStream = new())
        {
            AesCrypto.AesCBCEncryptStream(inputStream, encryptedStream, key192, iv);
            encryptedStream.Position = 0;
            AesCrypto.AesCBCDecryptStream(encryptedStream, decryptedStream, key192, iv);
            CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
        }

        // 测试32字节密钥（AES-256）
        string key256 = Convert.ToBase64String(_encoding.GetBytes("12345678901234567890123456789012"));
        using (MemoryStream inputStream = new(originalData))
        using (MemoryStream encryptedStream = new())
        using (MemoryStream decryptedStream = new())
        {
            AesCrypto.AesCBCEncryptStream(inputStream, encryptedStream, key256, iv);
            encryptedStream.Position = 0;
            AesCrypto.AesCBCDecryptStream(encryptedStream, decryptedStream, key256, iv);
            CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
        }
    }
    #endregion

#if NET
    #region Aes-GCM 流加解密测试
    /// <summary>
    /// 测试Aes-GCM流加密解密
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStream_Test()
    {
        // 准备测试数据
        byte[] originalData = GenerateRandomData(1024 * 50); // 50KB随机数据
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456")); // 16字节

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 加密
        long encryptedBytes = AesCrypto.AesGCMEncryptToStream(inputStream, encryptedStream, key);
        Assert.AreEqual(originalData.Length, encryptedBytes, "加密字节数应与原始数据相同");

        // 解密
        encryptedStream.Position = 0;
        long decryptedBytes = AesCrypto.AesGCMDecryptFromStream(encryptedStream, decryptedStream, key);
        Assert.AreEqual(originalData.Length, decryptedBytes, "解密字节数应与原始数据相同");

        // 验证解密结果与原文相同
        CollectionAssert.AreEqual(originalData, decryptedStream.ToArray(), "解密结果应与原文相同");
    }

    /// <summary>
    /// 测试Aes-GCM流加密解密（使用包装器）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStreamWithWrapper_Test()
    {
        // 准备测试数据
        byte[] originalData = _encoding.GetBytes("测试GCM流式包装器的加密解密功能！@#￥%……&*");
        string key = Convert.ToBase64String(_encoding.GetBytes("123456789012345678901234")); // 24字节

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 加密（使用包装器）
        var (encryptWrapper, nonce) = AesCrypto.CreateAesGCMEncryptStream(encryptedStream, key);
        inputStream.CopyTo(encryptWrapper);
        encryptWrapper.Close();

        // 解密（使用包装器）
        encryptedStream.Position = 0;
        using AesCrypto.AesGCMDecryptStreamWrapper decryptWrapper = AesCrypto.CreateAesGCMDecryptStream(encryptedStream, key);
        decryptWrapper.CopyTo(decryptedStream);

        // 验证解密结果与原文相同
        CollectionAssert.AreEqual(originalData, decryptedStream.ToArray(), "解密结果应与原文相同");
    }

    /// <summary>
    /// 测试Aes-GCM流加密解密（大文件）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStreamLargeFile_Test()
    {
        // 准备测试数据（5MB）
        byte[] originalData = GenerateRandomData(1024 * 1024 * 5);
        string key = Convert.ToBase64String(_encoding.GetBytes("12345678901234567890123456789012")); // 32字节

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 使用较小的缓冲区测试
        const int bufferSize = 4096;

        // 加密
        long encryptedBytes = AesCrypto.AesGCMEncryptToStream(inputStream, encryptedStream, key, bufferSize);
        Assert.AreEqual(originalData.Length, encryptedBytes);

        // 解密
        encryptedStream.Position = 0;
        long decryptedBytes = AesCrypto.AesGCMDecryptFromStream(encryptedStream, decryptedStream, key, bufferSize);
        Assert.AreEqual(originalData.Length, decryptedBytes);

        // 验证解密结果
        CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
    }

    /// <summary>
    /// 测试Aes-GCM流加密解密（空流）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStreamEmpty_Test()
    {
        byte[] originalData = [];
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 加密空流
        long encryptedBytes = AesCrypto.AesGCMEncryptToStream(inputStream, encryptedStream, key);
        Assert.AreEqual(0, encryptedBytes);

        // 解密空流
        encryptedStream.Position = 0;
        long decryptedBytes = AesCrypto.AesGCMDecryptFromStream(encryptedStream, decryptedStream, key);
        Assert.AreEqual(0, decryptedBytes);

        // 验证结果
        CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
    }

    /// <summary>
    /// 测试Aes-GCM流加密（null参数异常）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptStreamNullParameters_Test()
    {
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        byte[] keyBytes = _encoding.GetBytes("1234567890123456");

        using MemoryStream stream = new();

        // 测试null输入流
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            AesCrypto.AesGCMEncryptToStream(null!, stream, key));

        // 测试null输出流
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            AesCrypto.AesGCMEncryptToStream(stream, null!, key));

        // 测试null密钥
        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesGCMEncryptToStream(stream, stream, (string)null!));

        // 测试字节数组版本
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            AesCrypto.AesGCMEncryptToStream(null!, stream, keyBytes));

        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesGCMEncryptToStream(stream, stream, (byte[])null!));
    }

    /// <summary>
    /// 测试Aes-GCM流加密解密（无效密钥长度）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStreamInvalidKeyLength_Test()
    {
        byte[] data = GenerateRandomData(100);

        // 测试无效的密钥长度
        string invalidKey1 = Convert.ToBase64String(_encoding.GetBytes("12345678")); // 8字节
        string invalidKey2 = Convert.ToBase64String(_encoding.GetBytes("12345678901234567")); // 17字节

        using MemoryStream inputStream = new(data);
        using MemoryStream outputStream = new();

        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesGCMEncryptToStream(inputStream, outputStream, invalidKey1));

        inputStream.Position = 0;
        Assert.ThrowsExactly<ArgumentException>(() =>
            AesCrypto.AesGCMEncryptToStream(inputStream, outputStream, invalidKey2));
    }

    /// <summary>
    /// 测试Aes-GCM流加密解密（错误的密钥）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStreamWrongKey_Test()
    {
        byte[] originalData = GenerateRandomData(1000);
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        string wrongKey = Convert.ToBase64String(_encoding.GetBytes("6543210987654321"));

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 正确加密
        AesCrypto.AesGCMEncryptToStream(inputStream, encryptedStream, key);

        // 使用错误的密钥解密应该抛出异常
        encryptedStream.Position = 0;
        Assert.ThrowsExactly<AuthenticationTagMismatchException>(() =>
            AesCrypto.AesGCMDecryptFromStream(encryptedStream, decryptedStream, wrongKey));
    }

    /// <summary>
    /// 测试Aes-GCM流加密解密（数据篡改）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStreamTamperedData_Test()
    {
        byte[] originalData = GenerateRandomData(1000);
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));

        using MemoryStream inputStream = new(originalData);
        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 正确加密
        AesCrypto.AesGCMEncryptToStream(inputStream, encryptedStream, key);

        // 篡改加密数据
        byte[] encryptedBytes = encryptedStream.ToArray();
        encryptedBytes[^1] ^= 0x01; // 翻转最后一个字节的一个位

        using MemoryStream tamperedStream = new(encryptedBytes);

        // 解密被篡改的数据应该抛出异常
        Assert.ThrowsExactly<AuthenticationTagMismatchException>(() =>
            AesCrypto.AesGCMDecryptFromStream(tamperedStream, decryptedStream, key));
    }

    /// <summary>
    /// 测试Aes-GCM流加密解密（不同密钥长度）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStreamDifferentKeyLengths_Test()
    {
        byte[] originalData = GenerateRandomData(1024);

        // 测试16字节密钥（AES-128）
        string key128 = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        using (MemoryStream inputStream = new(originalData))
        using (MemoryStream encryptedStream = new())
        using (MemoryStream decryptedStream = new())
        {
            AesCrypto.AesGCMEncryptToStream(inputStream, encryptedStream, key128);
            encryptedStream.Position = 0;
            AesCrypto.AesGCMDecryptFromStream(encryptedStream, decryptedStream, key128);
            CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
        }

        // 测试24字节密钥（AES-192）
        string key192 = Convert.ToBase64String(_encoding.GetBytes("123456789012345678901234"));
        using (MemoryStream inputStream = new(originalData))
        using (MemoryStream encryptedStream = new())
        using (MemoryStream decryptedStream = new())
        {
            AesCrypto.AesGCMEncryptToStream(inputStream, encryptedStream, key192);
            encryptedStream.Position = 0;
            AesCrypto.AesGCMDecryptFromStream(encryptedStream, decryptedStream, key192);
            CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
        }

        // 测试32字节密钥（AES-256）
        string key256 = Convert.ToBase64String(_encoding.GetBytes("12345678901234567890123456789012"));
        using (MemoryStream inputStream = new(originalData))
        using (MemoryStream encryptedStream = new())
        using (MemoryStream decryptedStream = new())
        {
            AesCrypto.AesGCMEncryptToStream(inputStream, encryptedStream, key256);
            encryptedStream.Position = 0;
            AesCrypto.AesGCMDecryptFromStream(encryptedStream, decryptedStream, key256);
            CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
        }
    }

    /// <summary>
    /// 测试Aes-GCM流加密解密（相同的密钥但不同的nonce）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStreamSameKeyDifferentNonce_Test()
    {
        byte[] originalData = GenerateRandomData(1000);
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));

        using MemoryStream inputStream1 = new(originalData);
        using MemoryStream inputStream2 = new(originalData);
        using MemoryStream encryptedStream1 = new();
        using MemoryStream encryptedStream2 = new();
        using MemoryStream decryptedStream1 = new();
        using MemoryStream decryptedStream2 = new();

        // 使用相同密钥加密两次，应该产生不同的结果（因为nonce不同）
        AesCrypto.AesGCMEncryptToStream(inputStream1, encryptedStream1, key);
        AesCrypto.AesGCMEncryptToStream(inputStream2, encryptedStream2, key);

        // 加密结果应该不同
        byte[] encryptedBytes1 = encryptedStream1.ToArray();
        byte[] encryptedBytes2 = encryptedStream2.ToArray();
        CollectionAssert.AreNotEqual(encryptedBytes1, encryptedBytes2, "相同密钥不同nonce的加密结果应不同");

        // 但解密结果应该相同
        encryptedStream1.Position = 0;
        encryptedStream2.Position = 0;
        AesCrypto.AesGCMDecryptFromStream(encryptedStream1, decryptedStream1, key);
        AesCrypto.AesGCMDecryptFromStream(encryptedStream2, decryptedStream2, key);

        CollectionAssert.AreEqual(originalData, decryptedStream1.ToArray());
        CollectionAssert.AreEqual(originalData, decryptedStream2.ToArray());
    }

    /// <summary>
    /// 测试Aes-GCM流包装器（多次写入）
    /// </summary>
    [TestMethod]
    public void AesGCMStreamWrapperMultipleWrites_Test()
    {
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        string originalText = "测试多次写入的GCM流包装器";
        byte[] originalData = _encoding.GetBytes(originalText);

        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 加密（多次写入）
        var (encryptWrapper, nonce) = AesCrypto.CreateAesGCMEncryptStream(encryptedStream, key);

        // 分多次写入数据
        encryptWrapper.Write(originalData, 0, originalData.Length / 2);
        encryptWrapper.Write(originalData, originalData.Length / 2, originalData.Length - originalData.Length / 2);
        encryptWrapper.Close();

        // 解密
        encryptedStream.Position = 0;
        using AesCrypto.AesGCMDecryptStreamWrapper decryptWrapper = AesCrypto.CreateAesGCMDecryptStream(encryptedStream, key);
        decryptWrapper.CopyTo(decryptedStream);

        // 验证结果
        CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
    }

    /// <summary>
    /// 测试Aes-GCM流包装器（异步读写）
    /// </summary>
    [TestMethod]
    public async Task AesGCMStreamWrapperAsyncReadWrite_Test()
    {
        string key = Convert.ToBase64String(_encoding.GetBytes("1234567890123456"));
        byte[] originalData = GenerateRandomData(1024 * 10); // 10KB

        using MemoryStream encryptedStream = new();
        using MemoryStream decryptedStream = new();

        // 异步加密
        var (encryptWrapper, nonce) = AesCrypto.CreateAesGCMEncryptStream(encryptedStream, key);
        await encryptWrapper.WriteAsync(originalData, TestContext.CancellationToken);
        await encryptWrapper.FlushAsync(TestContext.CancellationToken);
        encryptWrapper.Close();

        // 异步解密
        encryptedStream.Position = 0;
        using AesCrypto.AesGCMDecryptStreamWrapper decryptWrapper = AesCrypto.CreateAesGCMDecryptStream(encryptedStream, key);
        await decryptWrapper.CopyToAsync(decryptedStream, TestContext.CancellationToken);

        // 验证结果
        CollectionAssert.AreEqual(originalData, decryptedStream.ToArray());
    }

    public TestContext TestContext { get; set; }
    #endregion
#endif
}