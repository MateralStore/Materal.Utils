using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

/// <summary>
/// Aes加解密流操作扩展
/// </summary>
public static partial class AesCrypto
{
    #region Aes-CBC 流加解密
    /// <summary>
    /// Aes-CBC加密流（PKCS7填充）
    /// </summary>
    /// <param name="inputStream">要加密的输入流</param>
    /// <param name="outputStream">加密后的输出流</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="iv">Base64编码的初始化向量（16字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>加密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 安全警告：CBC模式下，相同的密钥和IV不应重复使用，这会降低安全性。
    /// 推荐每次加密都使用新的随机IV，或使用自动生成IV的重载方法。
    /// </remarks>
    public static long AesCBCEncryptStream(Stream inputStream, Stream outputStream, string key, string iv, int bufferSize = 8192)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        if (iv is null) throw new ArgumentException("初始化向量不能为空", nameof(iv));
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        return AesCBCEncryptStream(inputStream, outputStream, keyBytes, ivBytes, bufferSize);
    }

    /// <summary>
    /// Aes-CBC加密流（PKCS7填充）
    /// </summary>
    /// <param name="inputStream">要加密的输入流</param>
    /// <param name="outputStream">加密后的输出流</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <param name="ivBytes">初始化向量字节数组（16字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>加密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <remarks>
    /// 安全警告：CBC模式下，相同的密钥和IV不应重复使用，这会降低安全性。
    /// 推荐每次加密都使用新的随机IV，或使用自动生成IV的重载方法。
    /// </remarks>
    public static long AesCBCEncryptStream(Stream inputStream, Stream outputStream, byte[] keyBytes, byte[] ivBytes, int bufferSize = 8192)
    {
        if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (!inputStream.CanRead) throw new ArgumentException("输入流必须可读", nameof(inputStream));
        if (!outputStream.CanWrite) throw new ArgumentException("输出流必须可写", nameof(outputStream));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (ivBytes is null || ivBytes.Length == 0) throw new ArgumentException("初始化向量不能为空", nameof(ivBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）", nameof(keyBytes));
        if (ivBytes.Length != 16) throw new ArgumentException("IV长度必须为16字节", nameof(ivBytes));
        if (bufferSize <= 0) throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = ivBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using ICryptoTransform encryptor = aes.CreateEncryptor();
#if NET
        using CryptoStream cryptoStream = new(outputStream, encryptor, CryptoStreamMode.Write, leaveOpen: true);
#else
        using CryptoStream cryptoStream = new(outputStream, encryptor, CryptoStreamMode.Write);
#endif
        byte[] buffer = new byte[bufferSize];
        long totalBytes = 0;
        int bytesRead;

        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            cryptoStream.Write(buffer, 0, bytesRead);
            totalBytes += bytesRead;
        }

        cryptoStream.FlushFinalBlock();
        return totalBytes;
    }

    /// <summary>
    /// Aes-CBC解密流（PKCS7填充）
    /// </summary>
    /// <param name="inputStream">要解密的输入流</param>
    /// <param name="outputStream">解密后的输出流</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="iv">Base64编码的初始化向量（16字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>解密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    public static long AesCBCDecryptStream(Stream inputStream, Stream outputStream, string key, string iv, int bufferSize = 8192)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        if (iv is null) throw new ArgumentException("初始化向量不能为空", nameof(iv));
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        return AesCBCDecryptStream(inputStream, outputStream, keyBytes, ivBytes, bufferSize);
    }

    /// <summary>
    /// Aes-CBC解密流（PKCS7填充）
    /// </summary>
    /// <param name="inputStream">要解密的输入流</param>
    /// <param name="outputStream">解密后的输出流</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <param name="ivBytes">初始化向量字节数组（16字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>解密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    public static long AesCBCDecryptStream(Stream inputStream, Stream outputStream, byte[] keyBytes, byte[] ivBytes, int bufferSize = 8192)
    {
        if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (!inputStream.CanRead) throw new ArgumentException("输入流必须可读", nameof(inputStream));
        if (!outputStream.CanWrite) throw new ArgumentException("输出流必须可写", nameof(outputStream));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (ivBytes is null || ivBytes.Length == 0) throw new ArgumentException("初始化向量不能为空", nameof(ivBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）", nameof(keyBytes));
        if (ivBytes.Length != 16) throw new ArgumentException("IV长度必须为16字节", nameof(ivBytes));
        if (bufferSize <= 0) throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = ivBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        using MemoryStream decryptedStream = new();
#if NET
        using CryptoStream cryptoStream = new(decryptedStream, decryptor, CryptoStreamMode.Write, leaveOpen: true);
#else
        using CryptoStream cryptoStream = new(decryptedStream, decryptor, CryptoStreamMode.Write);
#endif
        byte[] buffer = new byte[bufferSize];
        long totalBytes = 0;
        int bytesRead;

        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            cryptoStream.Write(buffer, 0, bytesRead);
            totalBytes += bytesRead;
        }
        cryptoStream.FlushFinalBlock();

        // 获取解密后的数据并移除PKCS7填充
        byte[] decryptedBytes = decryptedStream.ToArray();
        if (decryptedBytes.Length > 0)
        {
            // PKCS7填充：最后一个字节的值表示填充的字节数
            int paddingLength = decryptedBytes[decryptedBytes.Length - 1];
            if (paddingLength > 0 && paddingLength <= 16 && paddingLength <= decryptedBytes.Length) // AES块大小为16字节，且填充长度不能超过数据长度
            {
                // 验证填充是否正确
                bool validPadding = true;
                for (int i = decryptedBytes.Length - paddingLength; i < decryptedBytes.Length; i++)
                {
                    if (decryptedBytes[i] != paddingLength)
                    {
                        validPadding = false;
                        break;
                    }
                }
                
                if (validPadding)
                {
                    // 移除填充字节
                    int dataLength = decryptedBytes.Length - paddingLength;
                    outputStream.Write(decryptedBytes, 0, dataLength);
                    return dataLength;
                }
            }
        }
        
        // 如果没有填充或填充无效，写入全部数据
        outputStream.Write(decryptedBytes, 0, decryptedBytes.Length);
        return decryptedBytes.Length;
    }

    /// <summary>
    /// 创建Aes-CBC加密流（使用随机IV）
    /// </summary>
    /// <param name="outputStream">输出流</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>加密流和IV的元组</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// IV会自动写入到输出流的开始位置。
    /// 这种方式避免了单独管理IV的麻烦，推荐在大多数场景中使用。
    /// </remarks>
    public static (CryptoStream cryptoStream, byte[] iv) CreateAesCBCEncryptStream(Stream outputStream, string key)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return CreateAesCBCEncryptStream(outputStream, keyBytes);
    }

    /// <summary>
    /// 创建Aes-CBC加密流（使用随机IV）
    /// </summary>
    /// <param name="outputStream">输出流</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <returns>加密流和IV的元组</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <remarks>
    /// IV会自动写入到输出流的开始位置。
    /// 这种方式避免了单独管理IV的麻烦，推荐在大多数场景中使用。
    /// </remarks>
    public static (CryptoStream cryptoStream, byte[] iv) CreateAesCBCEncryptStream(Stream outputStream, byte[] keyBytes)
    {
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (!outputStream.CanWrite) throw new ArgumentException("输出流必须可写", nameof(outputStream));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）", nameof(keyBytes));

        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.GenerateIV();
        byte[] iv = aes.IV;

        // 将IV写入输出流
        outputStream.Write(iv, 0, iv.Length);

        // 创建加密器
        ICryptoTransform encryptor = aes.CreateEncryptor();
#if NET
        CryptoStream cryptoStream = new(outputStream, encryptor, CryptoStreamMode.Write, leaveOpen: true);
#else
        CryptoStream cryptoStream = new(outputStream, encryptor, CryptoStreamMode.Write);
#endif

        return (cryptoStream, iv);
    }

    /// <summary>
    /// 创建Aes-CBC解密流（自动提取IV）
    /// </summary>
    /// <param name="inputStream">输入流（IV前置格式）</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>解密流</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 输入格式：IV + 加密数据
    /// 会自动从前16字节提取IV进行解密。
    /// 与 CreateAesCBCEncryptStream 方法配对使用。
    /// </remarks>
    public static CryptoStream CreateAesCBCDecryptStream(Stream inputStream, string key)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return CreateAesCBCDecryptStream(inputStream, keyBytes);
    }

    /// <summary>
    /// 创建Aes-CBC解密流（自动提取IV）
    /// </summary>
    /// <param name="inputStream">输入流（IV前置格式）</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <returns>解密流</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <remarks>
    /// 输入格式：IV + 加密数据
    /// 会自动从前16字节提取IV进行解密。
    /// 与 CreateAesCBCEncryptStream 方法配对使用。
    /// </remarks>
    public static CryptoStream CreateAesCBCDecryptStream(Stream inputStream, byte[] keyBytes)
    {
        if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
        if (!inputStream.CanRead) throw new ArgumentException("输入流必须可读", nameof(inputStream));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）", nameof(keyBytes));

        // 读取IV
        byte[] iv = new byte[16];
        int bytesRead = inputStream.Read(iv, 0, iv.Length);
        if (bytesRead != 16) throw new ArgumentException("无法读取完整的IV", nameof(inputStream));

        // 创建解密器
        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        ICryptoTransform decryptor = aes.CreateDecryptor();
        
        // 创建一个包装流，跳过IV部分
        Stream decryptedStream = new SkipIVStream(inputStream, iv.Length);
        
#if NET
        return new CryptoStream(decryptedStream, decryptor, CryptoStreamMode.Read, leaveOpen: true);
#else
        return new CryptoStream(decryptedStream, decryptor, CryptoStreamMode.Read);
#endif
    }

    /// <summary>
    /// 跳过IV的包装流
    /// </summary>
    private class SkipIVStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly long _skipBytes;
        private long _position;

        public SkipIVStream(Stream innerStream, long skipBytes)
        {
            _innerStream = innerStream;
            _skipBytes = skipBytes;
            _position = 0;
        }

        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _innerStream.Length - _skipBytes;
        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _innerStream.Read(buffer, offset, count);
            _position += bytesRead;
            return bytesRead;
        }

        public override void Flush() => _innerStream.Flush();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    #endregion
    #region Aes-GCM 认证加密（AEAD）
#if NET
    /// <summary>
    /// Aes-GCM加密流（推荐用于高安全性场景）
    /// </summary>
    /// <param name="inputStream">要加密的输入流</param>
    /// <param name="outputStream">加密后的输出流</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>加密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 返回格式：nonce + tag + ciphertext
    /// - nonce: 12字节（随机数，每次加密必须唯一）
    /// - tag: 16字节（认证标签，验证数据完整性）
    /// - ciphertext: 加密后的数据
    /// 注意：由于GCM不支持真正的流式加密，此方法会将整个流读入内存。
    /// 对于大文件，建议使用分块处理或CBC模式。
    /// 安全提示：使用相同密钥和nonce加密多次会严重危及安全。
    /// </remarks>
    public static long AesGCMEncryptStream(Stream inputStream, Stream outputStream, string key, int bufferSize = 8192)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return AesGCMEncryptStream(inputStream, outputStream, keyBytes, bufferSize);
    }

    /// <summary>
    /// Aes-GCM加密流（推荐用于高安全性场景）
    /// </summary>
    /// <param name="inputStream">要加密的输入流</param>
    /// <param name="outputStream">加密后的输出流</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>加密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 返回格式：nonce + tag + ciphertext
    /// - nonce: 12字节（随机数，每次加密必须唯一）
    /// - tag: 16字节（认证标签，验证数据完整性）
    /// - ciphertext: 加密后的数据
    /// 注意：由于GCM不支持真正的流式加密，此方法会将整个流读入内存。
    /// 对于大文件，建议使用分块处理或CBC模式。
    /// 安全提示：使用相同密钥和nonce加密多次会严重危及安全。
    /// </remarks>
    public static long AesGCMEncryptStream(Stream inputStream, Stream outputStream, byte[] keyBytes, int bufferSize = 8192)
    {
        if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (!inputStream.CanRead) throw new ArgumentException("输入流必须可读", nameof(inputStream));
        if (!outputStream.CanWrite) throw new ArgumentException("输出流必须可写", nameof(outputStream));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）");
        if (bufferSize <= 0) throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        // 读取整个流到内存
        using MemoryStream ms = new();
        inputStream.CopyTo(ms, bufferSize);
        byte[] contentBytes = ms.ToArray();

        // 处理空内容
        if (contentBytes.Length == 0)
        {
            return 0;
        }

        // 使用GCM加密
        byte[] encryptedBytes = AesGCMEncrypt(contentBytes, keyBytes);

        // 写入输出流
        outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);

        return contentBytes.Length;
    }

    /// <summary>
    /// Aes-GCM解密流
    /// </summary>
    /// <param name="inputStream">要解密的输入流</param>
    /// <param name="outputStream">解密后的输出流</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>解密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：nonce + tag + ciphertext
    /// 注意：由于GCM不支持真正的流式解密，此方法会将整个流读入内存。
    /// 对于大文件，建议使用分块处理或CBC模式。
    /// </remarks>
    public static long AesGCMDecryptStream(Stream inputStream, Stream outputStream, string key, int bufferSize = 8192)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return AesGCMDecryptStream(inputStream, outputStream, keyBytes, bufferSize);
    }

    /// <summary>
    /// Aes-GCM解密流
    /// </summary>
    /// <param name="inputStream">要解密的输入流</param>
    /// <param name="outputStream">解密后的输出流</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>解密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：nonce + tag + ciphertext
    /// 注意：由于GCM不支持真正的流式解密，此方法会将整个流读入内存。
    /// 对于大文件，建议使用分块处理或CBC模式。
    /// </remarks>
    public static long AesGCMDecryptStream(Stream inputStream, Stream outputStream, byte[] keyBytes, int bufferSize = 8192)
    {
        if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (!inputStream.CanRead) throw new ArgumentException("输入流必须可读", nameof(inputStream));
        if (!outputStream.CanWrite) throw new ArgumentException("输出流必须可写", nameof(outputStream));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）");
        if (bufferSize <= 0) throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        // 读取整个流到内存
        using MemoryStream ms = new();
        inputStream.CopyTo(ms, bufferSize);
        byte[] contentBytes = ms.ToArray();

        // 处理空内容
        if (contentBytes.Length == 0)
        {
            return 0;
        }

        // 使用GCM解密
        byte[] decryptedBytes = AesGCMDecrypt(contentBytes, keyBytes);

        // 写入输出流
        outputStream.Write(decryptedBytes, 0, decryptedBytes.Length);

        return decryptedBytes.Length;
    }

    /// <summary>
    /// 创建Aes-GCM加密流（使用随机nonce）
    /// </summary>
    /// <param name="outputStream">输出流</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>加密流和nonce的元组</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 注意：此方法返回的流实际上不是真正的GCM流式加密，
    /// 而是一个包装器，会在关闭时计算并写入认证标签。
    /// 对于真正的流式处理，请考虑使用CBC模式。
    /// </remarks>
    public static (AesGCMEncryptStreamWrapper streamWrapper, byte[] nonce) CreateAesGCMEncryptStream(Stream outputStream, string key)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return CreateAesGCMEncryptStream(outputStream, keyBytes);
    }

    /// <summary>
    /// 创建Aes-GCM加密流（使用随机nonce）
    /// </summary>
    /// <param name="outputStream">输出流</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <returns>加密流和nonce的元组</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 注意：此方法返回的流实际上不是真正的GCM流式加密，
    /// 而是一个包装器，会在关闭时计算并写入认证标签。
    /// 对于真正的流式处理，请考虑使用CBC模式。
    /// </remarks>
    public static (AesGCMEncryptStreamWrapper streamWrapper, byte[] nonce) CreateAesGCMEncryptStream(Stream outputStream, byte[] keyBytes)
    {
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (!outputStream.CanWrite) throw new ArgumentException("输出流必须可写", nameof(outputStream));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）");

        // 生成nonce
        byte[] nonce = new byte[AesGcmNonceSize];
        RandomNumberGenerator.Fill(nonce);

        // 创建包装器
        AesGCMEncryptStreamWrapper wrapper = new(outputStream, keyBytes, nonce);

        return (wrapper, nonce);
    }

    /// <summary>
    /// 创建Aes-GCM解密流
    /// </summary>
    /// <param name="inputStream">输入流</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>解密流</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：nonce + tag + ciphertext
    /// 注意：此方法返回的流实际上不是真正的GCM流式解密，
    /// 而是一个包装器，会在读取时验证认证标签。
    /// </remarks>
    public static AesGCMDecryptStreamWrapper CreateAesGCMDecryptStream(Stream inputStream, string key)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return CreateAesGCMDecryptStream(inputStream, keyBytes);
    }

    /// <summary>
    /// 创建Aes-GCM解密流
    /// </summary>
    /// <param name="inputStream">输入流</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <returns>解密流</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：nonce + tag + ciphertext
    /// 注意：此方法返回的流实际上不是真正的GCM流式解密，
    /// 而是一个包装器，会在读取时验证认证标签。
    /// </remarks>
    public static AesGCMDecryptStreamWrapper CreateAesGCMDecryptStream(Stream inputStream, byte[] keyBytes)
    {
        if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
        if (!inputStream.CanRead) throw new ArgumentException("输入流必须可读", nameof(inputStream));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）");

        // 创建包装器
        AesGCMDecryptStreamWrapper wrapper = new(inputStream, keyBytes);

        return wrapper;
    }
#endif
    #endregion
}

