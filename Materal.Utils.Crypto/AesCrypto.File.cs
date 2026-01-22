using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

/// <summary>
/// Aes加解密文件操作扩展
/// </summary>
public static partial class AesCrypto
{
    #region Aes-CBC 文件加解密
    /// <summary>
    /// Aes-CBC加密文件（PKCS7填充）
    /// </summary>
    /// <param name="inputFileInfo">要加密的文件信息</param>
    /// <param name="outputFileInfo">加密后的输出文件信息</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="iv">Base64编码的初始化向量（16字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 安全警告：CBC模式下，相同的密钥和IV不应重复使用，这会降低安全性。
    /// 推荐每次加密都使用新的随机IV，或使用自动生成IV的重载方法。
    /// </remarks>
    public static void CBCEncryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, string key, string iv)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");

        using FileStream inputStream = inputFileInfo.OpenRead();
        using FileStream outputStream = outputFileInfo.Create();
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        CBCEncrypt(inputStream, outputStream, keyBytes, ivBytes);
    }

    /// <summary>
    /// Aes-CBC加密文件（PKCS7填充）
    /// </summary>
    /// <param name="inputFilePath">要加密的文件路径</param>
    /// <param name="outputFilePath">加密后的输出文件路径</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="iv">Base64编码的初始化向量（16字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 安全警告：CBC模式下，相同的密钥和IV不应重复使用，这会降低安全性。
    /// 推荐每次加密都使用新的随机IV，或使用自动生成IV的重载方法。
    /// </remarks>
    public static void CBCEncryptFile(string inputFilePath, string outputFilePath, string key, string iv)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        CBCEncryptFile(inputFileInfo, outputFileInfo, key, iv);
    }

    /// <summary>
    /// Aes-CBC解密文件（PKCS7填充）
    /// </summary>
    /// <param name="inputFileInfo">要解密的文件信息</param>
    /// <param name="outputFileInfo">解密后的输出文件信息</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="iv">Base64编码的初始化向量（16字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    public static void CBCDecryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, string key, string iv)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");

        using FileStream inputStream = inputFileInfo.OpenRead();
        using FileStream outputStream = outputFileInfo.Create();
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        CBCDecrypt(inputStream, outputStream, keyBytes, ivBytes);
    }

    /// <summary>
    /// Aes-CBC解密文件（PKCS7填充）
    /// </summary>
    /// <param name="inputFilePath">要解密的文件路径</param>
    /// <param name="outputFilePath">解密后的输出文件路径</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="iv">Base64编码的初始化向量（16字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    public static void CBCDecryptFile(string inputFilePath, string outputFilePath, string key, string iv)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        CBCDecryptFile(inputFileInfo, outputFileInfo, key, iv);
    }

    /// <summary>
    /// Aes-CBC加密文件（自动生成密钥和IV）
    /// </summary>
    /// <param name="inputFileInfo">要加密的文件信息</param>
    /// <param name="outputFileInfo">加密后的输出文件信息</param>
    /// <param name="key">输出的Base64编码密钥</param>
    /// <param name="iv">输出的Base64编码IV</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 此方法会生成新的随机密钥和IV，适用于需要生成密钥的场景。
    /// 请妥善保存输出的密钥和IV，解密时需要使用相同的密钥和IV。
    /// </remarks>
    public static void CBCEncryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, out string key, out string iv)
    {
        (key, iv) = GenerateCBCStringKey();
        CBCEncryptFile(inputFileInfo, outputFileInfo, key, iv);
    }

    /// <summary>
    /// Aes-CBC加密文件（自动生成密钥和IV）
    /// </summary>
    /// <param name="inputFilePath">要加密的文件路径</param>
    /// <param name="outputFilePath">加密后的输出文件路径</param>
    /// <param name="key">输出的Base64编码密钥</param>
    /// <param name="iv">输出的Base64编码IV</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 此方法会生成新的随机密钥和IV，适用于需要生成密钥的场景。
    /// 请妥善保存输出的密钥和IV，解密时需要使用相同的密钥和IV。
    /// </remarks>
    public static void CBCEncryptFile(string inputFilePath, string outputFilePath, out string key, out string iv)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        CBCEncryptFile(inputFileInfo, outputFileInfo, out key, out iv);
    }

    /// <summary>
    /// Aes-CBC加密文件（使用随机IV）
    /// </summary>
    /// <param name="inputFileInfo">要加密的文件信息</param>
    /// <param name="outputFileInfo">加密后的输出文件信息</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 输出格式：IV + 加密数据
    /// IV（16字节）会被前置到加密数据前，解密时自动提取。
    /// 这种方式避免了单独管理IV的麻烦，推荐在大多数场景中使用。
    /// </remarks>
    public static void CBCEncryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, string key)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");

        using FileStream inputStream = inputFileInfo.OpenRead();
        using FileStream outputStream = outputFileInfo.Create();
        byte[] keyBytes = Convert.FromBase64String(key);

        // 生成随机IV并写入输出流
        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.GenerateIV();
        outputStream.Write(aes.IV, 0, aes.IV.Length);

        // 加密剩余数据
        using CryptoStream cryptoStream = new(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        inputStream.CopyTo(cryptoStream);
        cryptoStream.FlushFinalBlock();
    }

    /// <summary>
    /// Aes-CBC加密文件（使用随机IV）
    /// </summary>
    /// <param name="inputFilePath">要加密的文件路径</param>
    /// <param name="outputFilePath">加密后的输出文件路径</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 输出格式：IV + 加密数据
    /// IV（16字节）会被前置到加密数据前，解密时自动提取。
    /// 这种方式避免了单独管理IV的麻烦，推荐在大多数场景中使用。
    /// </remarks>
    public static void CBCEncryptFile(string inputFilePath, string outputFilePath, string key)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        CBCEncryptFile(inputFileInfo, outputFileInfo, key);
    }

    /// <summary>
    /// Aes-CBC解密文件（自动提取IV）
    /// </summary>
    /// <param name="inputFileInfo">要解密的文件信息（IV前置格式）</param>
    /// <param name="outputFileInfo">解密后的输出文件信息</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 输入格式：IV + 加密数据
    /// 会自动从前16字节提取IV进行解密。
    /// 与 AesCBCEncryptFile(inputFileInfo, outputFileInfo, key) 方法配对使用。
    /// </remarks>
    public static void CBCDecryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, string key)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");

        using FileStream inputStream = inputFileInfo.OpenRead();
        using FileStream outputStream = outputFileInfo.Create();
        byte[] keyBytes = Convert.FromBase64String(key);

        // 读取IV
        byte[] ivBytes = new byte[16];
        int bytesRead = inputStream.Read(ivBytes, 0, ivBytes.Length);
        if (bytesRead != 16) throw new ArgumentException("数据长度不足，无法提取IV");

        // 解密剩余数据
        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = ivBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using CryptoStream cryptoStream = new(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        cryptoStream.CopyTo(outputStream);
    }

    /// <summary>
    /// Aes-CBC解密文件（自动提取IV）
    /// </summary>
    /// <param name="inputFilePath">要解密的文件路径（IV前置格式）</param>
    /// <param name="outputFilePath">解密后的输出文件路径</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 输入格式：IV + 加密数据
    /// 会自动从前16字节提取IV进行解密。
    /// 与 AesCBCEncryptFile(inputFilePath, outputFilePath, key) 方法配对使用。
    /// </remarks>
    public static void CBCDecryptFile(string inputFilePath, string outputFilePath, string key)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        CBCDecryptFile(inputFileInfo, outputFileInfo, key);
    }
    #endregion

    #region Aes-GCM 文件加解密
#if NET
    /// <summary>
    /// Aes-GCM加密文件（推荐用于高安全性场景）
    /// </summary>
    /// <param name="inputFileInfo">要加密的文件信息</param>
    /// <param name="outputFileInfo">加密后的输出文件信息</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输出格式：nonce + tag + ciphertext
    /// - nonce: 12字节（随机数，每次加密必须唯一）
    /// - tag: 16字节（认证标签，验证数据完整性）
    /// - ciphertext: 加密后的数据
    /// 安全提示：使用相同密钥和nonce加密多次会严重危及安全。
    /// </remarks>
    public static void GCMEncryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, string key)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");

        using FileStream inputStream = inputFileInfo.OpenRead();
        using FileStream outputStream = outputFileInfo.Create();
        byte[] keyBytes = Convert.FromBase64String(key);

        // 生成随机nonce并写入输出流
        byte[] nonce = new byte[GCMNonceSize];
        RandomNumberGenerator.Fill(nonce);
        outputStream.Write(nonce, 0, nonce.Length);

        // 使用流式加密处理大文件
        const int bufferSize = 8192;
        byte[] buffer = new byte[bufferSize];
        byte[] tag = new byte[GCMTagSize];

        // 读取所有数据到内存（GCM需要完整数据来生成认证标签）
        using MemoryStream inputMemoryStream = new();
        inputStream.CopyTo(inputMemoryStream);
        byte[] plaintext = inputMemoryStream.ToArray();
        byte[] ciphertext = new byte[plaintext.Length];

        using AesGcm aesGcm = new(keyBytes, GCMTagSize);
        aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

        // 写入tag和密文
        outputStream.Write(tag, 0, tag.Length);
        outputStream.Write(ciphertext, 0, ciphertext.Length);
    }

    /// <summary>
    /// Aes-GCM加密文件（推荐用于高安全性场景）
    /// </summary>
    /// <param name="inputFilePath">要加密的文件路径</param>
    /// <param name="outputFilePath">加密后的输出文件路径</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输出格式：nonce + tag + ciphertext
    /// - nonce: 12字节（随机数，每次加密必须唯一）
    /// - tag: 16字节（认证标签，验证数据完整性）
    /// - ciphertext: 加密后的数据
    /// 安全提示：使用相同密钥和nonce加密多次会严重危及安全。
    /// </remarks>
    public static void GCMEncryptFile(string inputFilePath, string outputFilePath, string key)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        GCMEncryptFile(inputFileInfo, outputFileInfo, key);
    }

    /// <summary>
    /// Aes-GCM加密文件（自动生成密钥和nonce）
    /// </summary>
    /// <param name="inputFileInfo">要加密的文件信息</param>
    /// <param name="outputFileInfo">加密后的输出文件信息</param>
    /// <param name="key">输出的Base64编码密钥</param>
    /// <param name="nonce">输出的Base64编码nonce</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 此方法会生成新的随机密钥和nonce，适用于需要生成密钥的场景。
    /// 输出格式：tag + ciphertext
    /// - tag: 16字节（认证标签，验证数据完整性）
    /// - ciphertext: 加密后的数据
    /// nonce通过out参数单独返回，请妥善保存输出的密钥和nonce，解密时需要使用相同的密钥和nonce。
    /// 安全提示：使用相同密钥和nonce加密多次会严重危及安全。
    /// </remarks>
    public static void GCMEncryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, out string key, out string nonce)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");

        key = GenerateGCMStringKey();
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] nonceBytes = new byte[GCMNonceSize];
        RandomNumberGenerator.Fill(nonceBytes);
        nonce = Convert.ToBase64String(nonceBytes);

        using FileStream inputStream = inputFileInfo.OpenRead();
        using FileStream outputStream = outputFileInfo.Create();

        // 读取所有数据到内存（GCM需要完整数据来生成认证标签）
        using MemoryStream inputMemoryStream = new();
        inputStream.CopyTo(inputMemoryStream);
        byte[] plaintext = inputMemoryStream.ToArray();
        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[GCMTagSize];

        using AesGcm aesGcm = new(keyBytes, GCMTagSize);
        aesGcm.Encrypt(nonceBytes, plaintext, ciphertext, tag);

        // 写入tag和密文
        outputStream.Write(tag, 0, tag.Length);
        outputStream.Write(ciphertext, 0, ciphertext.Length);
    }

    /// <summary>
    /// Aes-GCM加密文件（自动生成密钥和nonce）
    /// </summary>
    /// <param name="inputFilePath">要加密的文件路径</param>
    /// <param name="outputFilePath">加密后的输出文件路径</param>
    /// <param name="key">输出的Base64编码密钥</param>
    /// <param name="nonce">输出的Base64编码nonce</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 此方法会生成新的随机密钥和nonce，适用于需要生成密钥的场景。
    /// 输出格式：tag + ciphertext
    /// - tag: 16字节（认证标签，验证数据完整性）
    /// - ciphertext: 加密后的数据
    /// nonce通过out参数单独返回，请妥善保存输出的密钥和nonce，解密时需要使用相同的密钥和nonce。
    /// 安全提示：使用相同密钥和nonce加密多次会严重危及安全。
    /// </remarks>
    public static void GCMEncryptFile(string inputFilePath, string outputFilePath, out string key, out string nonce)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        GCMEncryptFile(inputFileInfo, outputFileInfo, out key, out nonce);
    }

    /// <summary>
    /// Aes-GCM解密文件
    /// </summary>
    /// <param name="inputFileInfo">要解密的文件信息（nonce + tag + ciphertext格式）</param>
    /// <param name="outputFileInfo">解密后的输出文件信息</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：nonce + tag + ciphertext
    /// 解密时会自动验证认证标签，如果数据被篡改会抛出异常。
    /// 与 AesGCMEncryptFile 方法配对使用。
    /// </remarks>
    public static void GCMDecryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, string key)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");

        using FileStream inputStream = inputFileInfo.OpenRead();
        using FileStream outputStream = outputFileInfo.Create();
        byte[] keyBytes = Convert.FromBase64String(key);

        // 读取nonce
        byte[] nonce = new byte[GCMNonceSize];
        int bytesRead = inputStream.Read(nonce, 0, nonce.Length);
        if (bytesRead != GCMNonceSize) throw new ArgumentException("数据长度不足，无法提取nonce");

        // 读取tag和密文
        using MemoryStream inputMemoryStream = new();
        inputStream.CopyTo(inputMemoryStream);
        byte[] tagAndCiphertext = inputMemoryStream.ToArray();

        if (tagAndCiphertext.Length < GCMTagSize) throw new ArgumentException("数据长度不足，无法提取tag");

        byte[] tag = new byte[GCMTagSize];
        byte[] ciphertext = new byte[tagAndCiphertext.Length - GCMTagSize];
        Buffer.BlockCopy(tagAndCiphertext, 0, tag, 0, GCMTagSize);
        Buffer.BlockCopy(tagAndCiphertext, GCMTagSize, ciphertext, 0, ciphertext.Length);

        byte[] plaintext = new byte[ciphertext.Length];
        using AesGcm aesGcm = new(keyBytes, GCMTagSize);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

        outputStream.Write(plaintext, 0, plaintext.Length);
    }

    /// <summary>
    /// Aes-GCM解密文件
    /// </summary>
    /// <param name="inputFilePath">要解密的文件路径（nonce + tag + ciphertext格式）</param>
    /// <param name="outputFilePath">解密后的输出文件路径</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：nonce + tag + ciphertext
    /// 解密时会自动验证认证标签，如果数据被篡改会抛出异常。
    /// 与 AesGCMEncryptFile 方法配对使用。
    /// </remarks>
    public static void GCMDecryptFile(string inputFilePath, string outputFilePath, string key)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        GCMDecryptFile(inputFileInfo, outputFileInfo, key);
    }

    /// <summary>
    /// Aes-GCM解密文件（使用单独的nonce）
    /// </summary>
    /// <param name="inputFileInfo">要解密的文件信息（tag + ciphertext格式）</param>
    /// <param name="outputFileInfo">解密后的输出文件信息</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="nonce">Base64编码的nonce（12字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：tag + ciphertext
    /// - tag: 16字节（认证标签）
    /// - ciphertext: 加密后的数据
    /// nonce作为单独参数传入，解密时会自动验证认证标签，如果数据被篡改会抛出异常。
    /// 与 AesGCMEncryptFile(inputFileInfo, outputFileInfo, out key, out nonce) 方法配对使用。
    /// </remarks>
    public static void GCMDecryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, string key, string nonce)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");

        using FileStream inputStream = inputFileInfo.OpenRead();
        using FileStream outputStream = outputFileInfo.Create();
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] nonceBytes = Convert.FromBase64String(nonce);

        // 读取tag和密文
        using MemoryStream inputMemoryStream = new();
        inputStream.CopyTo(inputMemoryStream);
        byte[] tagAndCiphertext = inputMemoryStream.ToArray();

        if (tagAndCiphertext.Length < GCMTagSize) throw new ArgumentException("数据长度不足，无法提取tag");

        byte[] tag = new byte[GCMTagSize];
        byte[] ciphertext = new byte[tagAndCiphertext.Length - GCMTagSize];
        Buffer.BlockCopy(tagAndCiphertext, 0, tag, 0, GCMTagSize);
        Buffer.BlockCopy(tagAndCiphertext, GCMTagSize, ciphertext, 0, ciphertext.Length);

        byte[] plaintext = new byte[ciphertext.Length];
        using AesGcm aesGcm = new(keyBytes, GCMTagSize);
        aesGcm.Decrypt(nonceBytes, ciphertext, tag, plaintext);

        outputStream.Write(plaintext, 0, plaintext.Length);
    }

    /// <summary>
    /// Aes-GCM解密文件（使用单独的nonce）
    /// </summary>
    /// <param name="inputFilePath">要解密的文件路径（tag + ciphertext格式）</param>
    /// <param name="outputFilePath">解密后的输出文件路径</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="nonce">Base64编码的nonce（12字节）</param>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：tag + ciphertext
    /// - tag: 16字节（认证标签）
    /// - ciphertext: 加密后的数据
    /// nonce作为单独参数传入，解密时会自动验证认证标签，如果数据被篡改会抛出异常。
    /// 与 AesGCMEncryptFile(inputFilePath, outputFilePath, out key, out nonce) 方法配对使用。
    /// </remarks>
    public static void GCMDecryptFile(string inputFilePath, string outputFilePath, string key, string nonce)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        GCMDecryptFile(inputFileInfo, outputFileInfo, key, nonce);
    }
#endif
    #endregion
}
