using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

/// <summary>
/// RSA+AES混合加密解密工具类
/// </summary>
/// <remarks>
/// 混合加密结合了RSA非对称加密和AES对称加密的优势：
/// - RSA用于加密AES密钥（小数据量，解决密钥分发问题）
/// - AES用于加密实际数据（大数据量，高性能）
/// - 每次加密使用新的随机AES密钥，增强安全性
/// 
/// 平台支持：
/// - .NET Standard: 使用AES-CBC模式
/// - .NET: 使用AES-GCM模式（推荐，提供认证加密）
/// 
/// 加密流程：
/// 1. 生成随机AES密钥和IV/nonce
/// 2. 用AES加密数据
/// 3. 用RSA公钥加密AES密钥
/// 4. 返回：加密的AES密钥 + IV/nonce + 加密的数据
/// 
/// 解密流程：
/// 1. 用RSA私钥解密AES密钥
/// 2. 用AES密钥和IV/nonce解密数据
/// </remarks>
public static partial class HybridCrypto
{
    #region 常量定义
    /// <summary>
    /// AES密钥长度（字节）
    /// </summary>
    private const int AesKeySize = 32; // 256位

#if NETSTANDARD
    /// <summary>
    /// CBC IV长度（字节）
    /// </summary>
    private const int IvSize = 16; // 128位
#else
    /// <summary>
    /// GCM nonce长度（字节）
    /// </summary>
    private const int GcmNonceSize = 12; // 96位

    /// <summary>
    /// GCM认证标签长度（字节）
    /// </summary>
    private const int GcmTagSize = 16; // 128位
#endif
    #endregion

    #region 核心加密解密方法
    /// <summary>
    /// 混合加密字节数组
    /// </summary>
    /// <param name="data">要加密的字节数组</param>
    /// <param name="rsaPublicKey">RSA公钥（支持XML和PEM格式）</param>
    /// <returns>加密后的字节数组</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <remarks>
    /// 输出格式：
    /// [4字节] 加密的AES密钥长度
    /// [N字节] 加密的AES密钥（RSA加密）
    /// [16字节] IV (.NET Standard) 或 [12字节] nonce (.NET)
    /// [16字节] 认证标签（仅.NET）
    /// [M字节] 加密数据
    /// </remarks>
    public static byte[] Encrypt(byte[] data, string rsaPublicKey)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("数据不能为空", nameof(data));
        if (string.IsNullOrEmpty(rsaPublicKey))
            throw new ArgumentException("RSA公钥不能为空", nameof(rsaPublicKey));

        // 1. 生成随机AES密钥和IV/nonce
        byte[] aesKey = new byte[AesKeySize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(aesKey);
        }
#if NETSTANDARD
        byte[] iv = new byte[IvSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(iv);
        }
        // 2. 用AES-CBC加密数据
        byte[] encryptedData = AesCbcEncrypt(data, aesKey, iv);
#else
        byte[] nonce = new byte[GcmNonceSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(nonce);
        }
        // 2. 用AES-GCM加密数据
        byte[] encryptedData = AesGcmEncrypt(data, aesKey, nonce);
#endif

        // 3. 用RSA公钥加密AES密钥
        byte[] encryptedAesKey = RsaCrypto.Encrypt(aesKey, rsaPublicKey);

        // 4. 组合输出数据
#if NETSTANDARD
        return CombineEncryptedData(encryptedAesKey, iv, encryptedData);
#else
        return CombineEncryptedData(encryptedAesKey, nonce, encryptedData);
#endif
    }

    /// <summary>
    /// 混合解密字节数组
    /// </summary>
    /// <param name="encryptedData">加密的字节数组</param>
    /// <param name="rsaPrivateKey">RSA私钥（支持XML和PEM格式）</param>
    /// <returns>解密后的原始字节数组</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当数据格式无效时抛出</exception>
    public static byte[] Decrypt(byte[] encryptedData, string rsaPrivateKey)
    {
        if (encryptedData == null || encryptedData.Length == 0)
            throw new ArgumentException("加密数据不能为空", nameof(encryptedData));
        if (string.IsNullOrEmpty(rsaPrivateKey))
            throw new ArgumentException("RSA私钥不能为空", nameof(rsaPrivateKey));

        // 1. 解析加密数据
#if NETSTANDARD
        var (encryptedAesKey, iv, ciphertext) = ParseEncryptedData(encryptedData);
#else
        var (encryptedAesKey, nonce, ciphertext, tag) = ParseEncryptedData(encryptedData);
#endif

        // 2. 用RSA私钥解密AES密钥
        byte[] aesKey = RsaCrypto.Decrypt(encryptedAesKey, rsaPrivateKey);

        // 3. 用AES解密数据
#if NETSTANDARD
        return AesCbcDecrypt(ciphertext, aesKey, iv);
#else
        // 重新组合认证标签和密文用于解密
        byte[] encryptedDataWithTag = CombineBytes(tag, ciphertext);
        return AesGcmDecrypt(encryptedDataWithTag, aesKey, nonce);
#endif
    }
    #endregion

    #region AES加密辅助方法
#if NETSTANDARD
    /// <summary>
    /// AES-CBC加密
    /// </summary>
    private static byte[] AesCbcEncrypt(byte[] data, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using ICryptoTransform encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(data, 0, data.Length);
    }

    /// <summary>
    /// AES-CBC解密
    /// </summary>
    private static byte[] AesCbcDecrypt(byte[] encryptedData, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using ICryptoTransform decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
    }
#else
    /// <summary>
    /// AES-GCM加密
    /// </summary>
    private static byte[] AesGcmEncrypt(byte[] data, byte[] key, byte[] nonce)
    {
        byte[] tag = new byte[GcmTagSize];
        byte[] ciphertext = new byte[data.Length];

        using AesGcm aesGcm = new(key, GcmTagSize);
        aesGcm.Encrypt(nonce, data, ciphertext, tag);

        // 返回格式：认证标签 + 加密数据
        return CombineBytes(tag, ciphertext);
    }

    /// <summary>
    /// AES-GCM解密
    /// </summary>
    private static byte[] AesGcmDecrypt(byte[] encryptedData, byte[] key, byte[] nonce)
    {
        if (encryptedData.Length < GcmTagSize)
            throw new CryptographicException("加密数据长度无效");

        byte[] tag = new byte[GcmTagSize];
        Array.Copy(encryptedData, 0, tag, 0, GcmTagSize);
        byte[] ciphertext = new byte[encryptedData.Length - GcmTagSize];
        Array.Copy(encryptedData, GcmTagSize, ciphertext, 0, ciphertext.Length);

        byte[] plaintext = new byte[ciphertext.Length];
        using AesGcm aesGcm = new(key, GcmTagSize);
        
        try
        {
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
        }
        catch (CryptographicException)
        {
            throw new CryptographicException("认证标签验证失败或数据已损坏");
        }

        return plaintext;
    }
#endif
    #endregion

    #region 数据组合和解析方法
    /// <summary>
    /// 组合加密数据
    /// </summary>
#if NETSTANDARD
    private static byte[] CombineEncryptedData(byte[] encryptedAesKey, byte[] iv, byte[] encryptedData)
#else
    private static byte[] CombineEncryptedData(byte[] encryptedAesKey, byte[] nonce, byte[] encryptedData)
#endif
    {
        // 加密的AES密钥长度（4字节）
        byte[] keyLengthBytes = BitConverter.GetBytes(encryptedAesKey.Length);

#if NETSTANDARD
        // 组合：密钥长度 + 加密的AES密钥 + IV + 加密数据
        return CombineBytes(keyLengthBytes, encryptedAesKey, iv, encryptedData);
#else
        // 组合：密钥长度 + 加密的AES密钥 + nonce + 加密数据（包含认证标签）
        return CombineBytes(keyLengthBytes, encryptedAesKey, nonce, encryptedData);
#endif
    }

    /// <summary>
    /// 解析加密数据
    /// </summary>
#if NETSTANDARD
    private static (byte[] encryptedAesKey, byte[] iv, byte[] ciphertext) ParseEncryptedData(byte[] encryptedData)
    {
        if (encryptedData.Length < 4 + IvSize)
            throw new FormatException("加密数据格式无效");

        int offset = 0;

        // 读取加密的AES密钥长度
        byte[] keyLengthBytes = new byte[4];
        Array.Copy(encryptedData, offset, keyLengthBytes, 0, 4);
        int keyLength = BitConverter.ToInt32(keyLengthBytes, 0);
        offset += 4;

        if (keyLength <= 0 || keyLength > encryptedData.Length - offset)
            throw new FormatException("加密的AES密钥长度无效");

        // 读取加密的AES密钥
        byte[] encryptedAesKey = new byte[keyLength];
        Array.Copy(encryptedData, offset, encryptedAesKey, 0, keyLength);
        offset += keyLength;

        // 读取IV
        byte[] iv = new byte[IvSize];
        Array.Copy(encryptedData, offset, iv, 0, IvSize);
        offset += IvSize;

        // 读取密文
        byte[] ciphertext = new byte[encryptedData.Length - offset];
        Array.Copy(encryptedData, offset, ciphertext, 0, ciphertext.Length);

        return (encryptedAesKey, iv, ciphertext);
    }
#else
    private static (byte[] encryptedAesKey, byte[] nonce, byte[] ciphertext, byte[] tag) ParseEncryptedData(byte[] encryptedData)
    {
        if (encryptedData.Length < 4 + GcmNonceSize + GcmTagSize)
            throw new FormatException("加密数据格式无效");

        int offset = 0;

        // 读取加密的AES密钥长度
        byte[] keyLengthBytes = new byte[4];
        Array.Copy(encryptedData, offset, keyLengthBytes, 0, 4);
        int keyLength = BitConverter.ToInt32(keyLengthBytes, 0);
        offset += 4;

        if (keyLength <= 0 || keyLength > encryptedData.Length - offset)
            throw new FormatException("加密的AES密钥长度无效");

        // 读取加密的AES密钥
        byte[] encryptedAesKey = new byte[keyLength];
        Array.Copy(encryptedData, offset, encryptedAesKey, 0, keyLength);
        offset += keyLength;

        // 读取nonce
        byte[] nonce = new byte[GcmNonceSize];
        Array.Copy(encryptedData, offset, nonce, 0, GcmNonceSize);
        offset += GcmNonceSize;

        // 读取认证标签
        byte[] tag = new byte[GcmTagSize];
        Array.Copy(encryptedData, offset, tag, 0, GcmTagSize);
        offset += GcmTagSize;

        // 读取密文
        byte[] ciphertext = new byte[encryptedData.Length - offset];
        Array.Copy(encryptedData, offset, ciphertext, 0, ciphertext.Length);

        return (encryptedAesKey, nonce, ciphertext, tag);
    }
#endif

    /// <summary>
    /// 合并多个字节数组
    /// </summary>
    private static byte[] CombineBytes(params byte[][] arrays)
    {
        int totalLength = arrays.Sum(arr => arr.Length);
        byte[] result = new byte[totalLength];
        int offset = 0;

        foreach (byte[] array in arrays)
        {
            Array.Copy(array, 0, result, offset, array.Length);
            offset += array.Length;
        }

        return result;
    }
    #endregion
}
