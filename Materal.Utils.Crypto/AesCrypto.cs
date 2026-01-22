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
    public static long CBCEncrypt(Stream inputStream, Stream outputStream, string key, string iv, int bufferSize = 8192)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        if (iv is null) throw new ArgumentException("初始化向量不能为空", nameof(iv));
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        return CBCEncrypt(inputStream, outputStream, keyBytes, ivBytes, bufferSize);
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
    public static long CBCEncrypt(Stream inputStream, Stream outputStream, byte[] keyBytes, byte[] ivBytes, int bufferSize = 8192)
    {
        ValidateStreamParameters(inputStream, outputStream);
        ValidateKeyBytes(keyBytes);
        ValidateIVBytes(ivBytes);
        ValidateBufferSize(bufferSize);

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
    public static long CBCDecrypt(Stream inputStream, Stream outputStream, string key, string iv, int bufferSize = 8192)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        if (iv is null) throw new ArgumentException("初始化向量不能为空", nameof(iv));
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        return CBCDecrypt(inputStream, outputStream, keyBytes, ivBytes, bufferSize);
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
    public static long CBCDecrypt(Stream inputStream, Stream outputStream, byte[] keyBytes, byte[] ivBytes, int bufferSize = 8192)
    {
        ValidateStreamParameters(inputStream, outputStream);
        ValidateKeyBytes(keyBytes);
        ValidateIVBytes(ivBytes);
        ValidateBufferSize(bufferSize);

        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = ivBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using ICryptoTransform decryptor = aes.CreateDecryptor();
#if NET
        using CryptoStream cryptoStream = new(inputStream, decryptor, CryptoStreamMode.Read, leaveOpen: true);
#else
        using CryptoStream cryptoStream = new(inputStream, decryptor, CryptoStreamMode.Read);
#endif
        byte[] buffer = new byte[bufferSize];
        long totalBytes = 0;
        int bytesRead;

        while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            outputStream.Write(buffer, 0, bytesRead);
            totalBytes += bytesRead;
        }

        return totalBytes;
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
    public static (CryptoStream cryptoStream, byte[] iv) CreateCBCEncryptStream(Stream outputStream, string key)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return CreateCBCEncryptStream(outputStream, keyBytes);
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
    public static (CryptoStream cryptoStream, byte[] iv) CreateCBCEncryptStream(Stream outputStream, byte[] keyBytes)
    {
        ValidateStreamParameters(null, outputStream, false, true);
        ValidateKeyBytes(keyBytes);

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
    /// <returns>解密流包装器</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 输入格式：IV + 加密数据
    /// 会自动从前16字节提取IV进行解密。
    /// 与 CreateAesCBCEncryptStream 方法配对使用。
    /// 注意：返回的流必须正确释放以确保资源清理。
    /// </remarks>
    public static CBCDecryptStreamWrapper CreateCBCDecryptStream(Stream inputStream, string key)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return CreateCBCDecryptStream(inputStream, keyBytes);
    }

    /// <summary>
    /// 创建Aes-CBC解密流（自动提取IV）
    /// </summary>
    /// <param name="inputStream">输入流（IV前置格式）</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <returns>解密流包装器</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <remarks>
    /// 输入格式：IV + 加密数据
    /// 会自动从前16字节提取IV进行解密。
    /// 与 CreateAesCBCEncryptStream 方法配对使用。
    /// 注意：返回的流必须正确释放以确保资源清理。
    /// </remarks>
    public static CBCDecryptStreamWrapper CreateCBCDecryptStream(Stream inputStream, byte[] keyBytes)
    {
        ValidateStreamParameters(inputStream, null, true, false);
        ValidateKeyBytes(keyBytes);

        // 读取IV
        byte[] iv = new byte[16];
        int bytesRead = inputStream.Read(iv, 0, iv.Length);
        if (bytesRead != 16) throw new ArgumentException("无法读取完整的IV", nameof(inputStream));

        return new CBCDecryptStreamWrapper(inputStream, keyBytes, iv);
    }
    #endregion
    #region Aes-GCM 认证加密（AEAD）
#if NET
    /// <summary>
    /// Aes-GCM加密到流（推荐用于高安全性场景）
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
    public static long GCMEncrypt(Stream inputStream, Stream outputStream, string key, int bufferSize = 8192)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return GCMEncrypt(inputStream, outputStream, keyBytes, bufferSize, null);
    }

    /// <summary>
    /// Aes-GCM加密到流（推荐用于高安全性场景）
    /// </summary>
    /// <param name="inputStream">要加密的输入流</param>
    /// <param name="outputStream">加密后的输出流</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="progressCallback">进度回调函数，参数为已处理字节数和总字节数</param>
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
    public static long GCMEncrypt(Stream inputStream, Stream outputStream, string key, Action<long, long>? progressCallback, int bufferSize = 8192)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return GCMEncrypt(inputStream, outputStream, keyBytes, bufferSize, progressCallback);
    }

    /// <summary>
    /// Aes-GCM加密到流（推荐用于高安全性场景）
    /// </summary>
    /// <param name="inputStream">要加密的输入流</param>
    /// <param name="outputStream">加密后的输出流</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <param name="progressCallback">进度回调函数，参数为已处理字节数和总字节数</param>
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
    public static long GCMEncrypt(Stream inputStream, Stream outputStream, byte[] keyBytes, int bufferSize = 8192, Action<long, long>? progressCallback = null)
    {
        ValidateStreamParameters(inputStream, outputStream);
        ValidateKeyBytes(keyBytes);
        ValidateBufferSize(bufferSize);

        // 读取整个流到内存，支持进度回调
        using MemoryStream ms = new();
        byte[] buffer = new byte[bufferSize];
        int bytesRead;
        long totalBytesRead = 0;
        long totalLength = inputStream.CanSeek ? inputStream.Length : -1;

        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, bytesRead);
            totalBytesRead += bytesRead;
            progressCallback?.Invoke(totalBytesRead, totalLength);
        }

        byte[] contentBytes = ms.ToArray();

        // 处理空内容
        if (contentBytes.Length == 0)
        {
            return 0;
        }

        // 使用GCM加密
        byte[] encryptedBytes = GCMEncrypt(contentBytes, keyBytes);

        // 写入输出流
        outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);

        return contentBytes.Length;
    }

    /// <summary>
    /// Aes-GCM从流解密（推荐用于高安全性场景）
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
    public static long GCMDecrypt(Stream inputStream, Stream outputStream, string key, int bufferSize = 8192)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return GCMDecrypt(inputStream, outputStream, keyBytes, bufferSize, null);
    }

    /// <summary>
    /// Aes-GCM从流解密（推荐用于高安全性场景）
    /// </summary>
    /// <param name="inputStream">要解密的输入流</param>
    /// <param name="outputStream">解密后的输出流</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="progressCallback">进度回调函数，参数为已处理字节数和总字节数</param>
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
    public static long GCMDecrypt(Stream inputStream, Stream outputStream, string key, Action<long, long>? progressCallback, int bufferSize = 8192)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return GCMDecrypt(inputStream, outputStream, keyBytes, bufferSize, progressCallback);
    }

    /// <summary>
    /// Aes-GCM从流解密（推荐用于高安全性场景）
    /// </summary>
    /// <param name="inputStream">要解密的输入流</param>
    /// <param name="outputStream">解密后的输出流</param>
    /// <param name="keyBytes">密钥字节数组（16/24/32字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <param name="progressCallback">进度回调函数，参数为已处理字节数和总字节数</param>
    /// <returns>解密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：nonce + tag + ciphertext
    /// 注意：由于GCM不支持真正的流式解密，此方法会将整个流读入内存。
    /// 对于大文件，建议使用分块处理或CBC模式。
    /// </remarks>
    public static long GCMDecrypt(Stream inputStream, Stream outputStream, byte[] keyBytes, int bufferSize = 8192, Action<long, long>? progressCallback = null)
    {
        ValidateStreamParameters(inputStream, outputStream);
        ValidateKeyBytes(keyBytes);
        ValidateBufferSize(bufferSize);

        // 读取整个流到内存，支持进度回调
        using MemoryStream ms = new();
        byte[] buffer = new byte[bufferSize];
        int bytesRead;
        long totalBytesRead = 0;
        long totalLength = inputStream.CanSeek ? inputStream.Length : -1;

        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, bytesRead);
            totalBytesRead += bytesRead;
            progressCallback?.Invoke(totalBytesRead, totalLength);
        }

        byte[] contentBytes = ms.ToArray();

        // 处理空内容
        if (contentBytes.Length == 0)
        {
            return 0;
        }

        // 使用GCM解密
        byte[] decryptedBytes = GCMDecrypt(contentBytes, keyBytes);

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
    public static (GCMEncryptStreamWrapper streamWrapper, byte[] nonce) CreateGCMEncryptStream(Stream outputStream, string key)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return CreateGCMEncryptStream(outputStream, keyBytes);
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
    public static (GCMEncryptStreamWrapper streamWrapper, byte[] nonce) CreateGCMEncryptStream(Stream outputStream, byte[] keyBytes)
    {
        ValidateStreamParameters(null, outputStream, false, true);
        ValidateKeyBytes(keyBytes);

        // 生成nonce
        byte[] nonce = new byte[GCMNonceSize];
        RandomNumberGenerator.Fill(nonce);

        // 创建包装器
        GCMEncryptStreamWrapper wrapper = new(outputStream, keyBytes, nonce);

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
    public static GCMDecryptStreamWrapper CreateGCMDecryptStream(Stream inputStream, string key)
    {
        if (key is null) throw new ArgumentException("密钥不能为空", nameof(key));
        byte[] keyBytes = Convert.FromBase64String(key);
        return CreateGCMDecryptStream(inputStream, keyBytes);
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
    public static GCMDecryptStreamWrapper CreateGCMDecryptStream(Stream inputStream, byte[] keyBytes)
    {
        ValidateStreamParameters(inputStream, null, true, false);
        ValidateKeyBytes(keyBytes);

        // 创建包装器
        GCMDecryptStreamWrapper wrapper = new(inputStream, keyBytes);

        return wrapper;
    }
#endif
    #endregion
    #region 公共验证辅助方法
    /// <summary>
    /// 验证流参数
    /// </summary>
    private static void ValidateStreamParameters(Stream? inputStream, Stream? outputStream, bool validateInput = true, bool validateOutput = true)
    {
        if (validateInput)
        {
            if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
            if (!inputStream.CanRead) throw new ArgumentException("输入流必须可读", nameof(inputStream));
        }
        if (validateOutput)
        {
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
            if (!outputStream.CanWrite) throw new ArgumentException("输出流必须可写", nameof(outputStream));
        }
    }

    /// <summary>
    /// 验证密钥字节数组
    /// </summary>
    private static void ValidateKeyBytes(byte[]? keyBytes)
    {
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32)
            throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）", nameof(keyBytes));
    }

    /// <summary>
    /// 验证IV字节数组
    /// </summary>
    private static void ValidateIVBytes(byte[]? ivBytes)
    {
        if (ivBytes is null || ivBytes.Length == 0) throw new ArgumentException("初始化向量不能为空", nameof(ivBytes));
        if (ivBytes.Length != 16) throw new ArgumentException("IV长度必须为16字节", nameof(ivBytes));
    }

    /// <summary>
    /// 验证缓冲区大小
    /// </summary>
    private static void ValidateBufferSize(int bufferSize)
    {
        if (bufferSize <= 0) throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));
    }
    #endregion
}

