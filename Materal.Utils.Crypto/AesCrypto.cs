using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

/// <summary>
/// Aes加解密核心
/// </summary>
public static partial class AesCrypto
{
    #region Aes-CBC 加密（非认证加密）
    /// <summary>
    /// 生成Aes-CBC密钥和IV
    /// </summary>
    /// <returns>Base64编码的(密钥, IV)元组</returns>
    /// <remarks>
    /// 密钥长度：256位（32字节）
    /// IV长度：128位（16字节）
    /// 注意：每次调用都会生成新的随机密钥和IV
    /// </remarks>
    public static (byte[] key, byte[] iv) GenerateAesCBCKey()
    {
        using Aes aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();
        return (aes.Key, aes.IV);
    }

    /// <summary>
    /// 生成Aes-CBC密钥和IV
    /// </summary>
    /// <returns>Base64编码的(密钥, IV)元组</returns>
    /// <remarks>
    /// 密钥长度：256位（32字节）
    /// IV长度：128位（16字节）
    /// 注意：每次调用都会生成新的随机密钥和IV
    /// </remarks>
    public static (string key, string iv) GenerateAesCBCStringKey()
    {
        (byte[] keyBytes, byte[] ivBytes) = GenerateAesCBCKey();
        string key = Convert.ToBase64String(keyBytes);
        string iv = Convert.ToBase64String(ivBytes);
        return (key, iv);
    }

    /// <summary>
    /// Aes-CBC加密（PKCS7填充）
    /// </summary>
    /// <param name="content">要加密的内容（UTF-8编码）</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="iv">Base64编码的初始化向量（16字节）</param>
    /// <returns>Base64编码的加密数据</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 安全警告：CBC模式下，相同的密钥和IV不应重复使用，这会降低安全性。
    /// 推荐每次加密都使用新的随机IV，或使用自动生成IV的重载方法。
    /// </remarks>
    public static byte[] AesCBCEncrypt(byte[] content, string key, string iv)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        return AesCBCEncrypt(content, keyBytes, ivBytes);
    }

    /// <summary>
    /// Aes-CBC加密（PKCS7填充）
    /// </summary>
    /// <param name="content">要加密的内容（UTF-8编码）</param>
    /// <param name="keyBytes">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="ivBytes">Base64编码的初始化向量（16字节）</param>
    /// <returns>Base64编码的加密数据</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 安全警告：CBC模式下，相同的密钥和IV不应重复使用，这会降低安全性。
    /// 推荐每次加密都使用新的随机IV，或使用自动生成IV的重载方法。
    /// </remarks>
    public static byte[] AesCBCEncrypt(byte[] content, byte[] keyBytes, byte[] ivBytes)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (ivBytes is null || ivBytes.Length == 0) throw new ArgumentException("初始化向量不能为空", nameof(ivBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）", nameof(keyBytes));
        if (ivBytes.Length != 16) throw new ArgumentException("IV长度必须为16字节", nameof(ivBytes));
        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = ivBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using ICryptoTransform encryptor = aes.CreateEncryptor();
        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
        cs.Write(content, 0, content.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    /// <summary>
    /// Aes-CBC解密（PKCS7填充）
    /// </summary>
    /// <param name="content">要解密的Base64编码加密数据</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="iv">Base64编码的初始化向量（16字节）</param>
    /// <returns>解密后的原始内容（UTF-8编码）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    public static byte[] AesCBCDecrypt(byte[] content, string key, string iv)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        return AesCBCDecrypt(content, keyBytes, ivBytes);
    }

    /// <summary>
    /// Aes-CBC解密（PKCS7填充）
    /// </summary>
    /// <param name="content">要解密的Base64编码加密数据</param>
    /// <param name="keyBytes">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="ivBytes">Base64编码的初始化向量（16字节）</param>
    /// <returns>解密后的原始内容（UTF-8编码）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    public static byte[] AesCBCDecrypt(byte[] content, byte[] keyBytes, byte[] ivBytes)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (ivBytes is null || ivBytes.Length == 0) throw new ArgumentException("初始化向量不能为空", nameof(ivBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）", nameof(keyBytes));
        if (ivBytes.Length != 16) throw new ArgumentException("IV长度必须为16字节", nameof(ivBytes));
        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = ivBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        using MemoryStream ms = new(content);
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using MemoryStream decryptedMs = new();
        cs.CopyTo(decryptedMs);
        return decryptedMs.ToArray();
    }

    /// <summary>
    /// Aes-CBC加密（自动生成密钥和IV）
    /// </summary>
    /// <param name="content">要加密的内容（UTF-8编码）</param>
    /// <param name="key">输出的Base64编码密钥</param>
    /// <param name="iv">输出的Base64编码IV</param>
    /// <returns>Base64编码的加密数据</returns>
    /// <remarks>
    /// 此方法会生成新的随机密钥和IV，适用于需要生成密钥的场景。
    /// 请妥善保存输出的密钥和IV，解密时需要使用相同的密钥和IV。
    /// </remarks>
    public static byte[] AesCBCEncrypt(byte[] content, out string key, out string iv)
    {
        (key, iv) = GenerateAesCBCStringKey();
        return AesCBCEncrypt(content, key, iv);
    }

    /// <summary>
    /// Aes-CBC加密（使用随机IV）
    /// </summary>
    /// <param name="content">要加密的内容（UTF-8编码）</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>Base64编码的加密数据（IV前置）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 返回格式：Base64(IV + 加密数据)
    /// IV（16字节）会被前置到加密数据前，解密时自动提取。
    /// 这种方式避免了单独管理IV的麻烦，推荐在大多数场景中使用。
    /// </remarks>
    public static byte[] AesCBCEncrypt(byte[] content, string key)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        return AesCBCEncrypt(content, keyBytes);
    }

    /// <summary>
    /// Aes-CBC加密（使用随机IV）
    /// </summary>
    /// <param name="content">要加密的内容（UTF-8编码）</param>
    /// <param name="keyBytes">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>Base64编码的加密数据（IV前置）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 返回格式：Base64(IV + 加密数据)
    /// IV（16字节）会被前置到加密数据前，解密时自动提取。
    /// 这种方式避免了单独管理IV的麻烦，推荐在大多数场景中使用。
    /// </remarks>
    public static byte[] AesCBCEncrypt(byte[] content, byte[] keyBytes)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）", nameof(keyBytes));
        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.GenerateIV();
        using ICryptoTransform encryptor = aes.CreateEncryptor();
        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
        cs.Write(content, 0, content.Length);
        cs.FlushFinalBlock();
        byte[] encryptedBytes = ms.ToArray();
        byte[] resultBytes = new byte[aes.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, resultBytes, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, resultBytes, aes.IV.Length, encryptedBytes.Length);
        return resultBytes;
    }

    /// <summary>
    /// Aes-CBC解密（自动提取IV）
    /// </summary>
    /// <param name="content">Base64编码的加密数据（IV前置格式）</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>解密后的原始内容（UTF-8编码）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 输入格式：Base64(IV + 加密数据)
    /// 会自动从前16字节提取IV进行解密。
    /// 与 AesCBCEncrypt(content, key) 方法配对使用。
    /// </remarks>
    public static byte[] AesCBCDecrypt(byte[] content, string key)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        return AesCBCDecrypt(content, keyBytes);
    }

    /// <summary>
    /// Aes-CBC解密（自动提取IV）
    /// </summary>
    /// <param name="content">Base64编码的加密数据（IV前置格式）</param>
    /// <param name="keyBytes">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>解密后的原始内容（UTF-8编码）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 输入格式：Base64(IV + 加密数据)
    /// 会自动从前16字节提取IV进行解密。
    /// 与 AesCBCEncrypt(content, key) 方法配对使用。
    /// </remarks>
    public static byte[] AesCBCDecrypt(byte[] content, byte[] keyBytes)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）", nameof(keyBytes));
        using Aes aes = Aes.Create();
        int ivLength = aes.BlockSize / 8;
        if (content.Length < ivLength) throw new ArgumentException("数据长度不足，无法提取IV");
        byte[] ivBytes = new byte[ivLength];
        byte[] encryptedBytes = new byte[content.Length - ivLength];
        Buffer.BlockCopy(content, 0, ivBytes, 0, ivLength);
        Buffer.BlockCopy(content, ivLength, encryptedBytes, 0, encryptedBytes.Length);
        aes.Key = keyBytes;
        aes.IV = ivBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        using MemoryStream ms = new(encryptedBytes);
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using MemoryStream decryptedMs = new();
        cs.CopyTo(decryptedMs);
        return decryptedMs.ToArray();
    }
    #endregion

    #region Aes-GCM 认证加密（AEAD）
    // Aes-GCM 认证加密
    // GCM模式提供认证加密（AEAD），能同时保证机密性和完整性。
    // 相比CBC模式更安全，性能更好，推荐在新项目中使用。
    // 注意：仅在 .NET Core 3.0+ 和 .NET 5+ 中支持。
#if NET
    /// <summary>
    /// GCM认证标签大小（128位）
    /// </summary>
    public const int AesGcmTagSize = 16;

    /// <summary>
    /// GCM推荐nonce大小（96位）
    /// </summary>
    public const int AesGcmNonceSize = 12;

    /// <summary>
    /// 生成Aes-GCM密钥
    /// </summary>
    /// <param name="keySize">密钥大小（128、192或256位）</param>
    /// <returns>Base64编码的密钥</returns>
    /// <exception cref="ArgumentException">密钥大小无效时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 推荐使用256位密钥以获得更高的安全性。
    /// 生成的密钥是随机的，请妥善保存。
    /// </remarks>
    public static byte[] GenerateAesGCMKey(int keySize = 256)
    {
        if (keySize != 128 && keySize != 192 && keySize != 256) throw new ArgumentException("密钥大小必须是128、192或256位");
        using Aes aes = Aes.Create();
        aes.KeySize = keySize;
        aes.GenerateKey();
        return aes.Key;
    }

    /// <summary>
    /// 生成Aes-GCM密钥
    /// </summary>
    /// <param name="keySize">密钥大小（128、192或256位）</param>
    /// <returns>Base64编码的密钥</returns>
    /// <exception cref="ArgumentException">密钥大小无效时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 推荐使用256位密钥以获得更高的安全性。
    /// 生成的密钥是随机的，请妥善保存。
    /// </remarks>
    public static string GenerateAesGCMStringKey(int keySize = 256)
    {
        byte[] keyBytes = GenerateAesGCMKey(keySize);
        string key = Convert.ToBase64String(keyBytes);
        return key;
    }

    /// <summary>
    /// Aes-GCM加密（推荐用于高安全性场景）
    /// </summary>
    /// <param name="content">要加密的内容（UTF-8编码）</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>Base64编码的加密数据（nonce + tag + ciphertext）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 返回格式：Base64(nonce + tag + ciphertext)
    /// - nonce: 12字节（随机数，每次加密必须唯一）
    /// - tag: 16字节（认证标签，验证数据完整性）
    /// - ciphertext: 加密后的数据
    /// 安全提示：使用相同密钥和nonce加密多次会严重危及安全。
    /// </remarks>
    public static byte[] AesGCMEncrypt(byte[] content, string key)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        return AesGCMEncrypt(content, keyBytes);
    }

    /// <summary>
    /// Aes-GCM加密（自动生成密钥和nonce）
    /// </summary>
    /// <param name="content">要加密的内容（UTF-8编码）</param>
    /// <param name="key">输出的Base64编码密钥</param>
    /// <param name="nonce">输出的Base64编码nonce</param>
    /// <returns>Base64编码的加密数据（tag + ciphertext）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 此方法会生成新的随机密钥和nonce，适用于需要生成密钥的场景。
    /// 返回格式：Base64(tag + ciphertext)
    /// - tag: 16字节（认证标签，验证数据完整性）
    /// - ciphertext: 加密后的数据
    /// nonce通过out参数单独返回，请妥善保存输出的密钥和nonce，解密时需要使用相同的密钥和nonce。
    /// 安全提示：使用相同密钥和nonce加密多次会严重危及安全。
    /// </remarks>
    public static byte[] AesGCMEncrypt(byte[] content, out string key, out string nonce)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        key = GenerateAesGCMStringKey();
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] nonceBytes = new byte[AesGcmNonceSize];
        RandomNumberGenerator.Fill(nonceBytes);
        nonce = Convert.ToBase64String(nonceBytes);
        (byte[] tag, byte[] ciphertext) = AesGCMEncryptCore(content, keyBytes, nonceBytes);
        byte[] result = new byte[tag.Length + ciphertext.Length];
        Buffer.BlockCopy(tag, 0, result, 0, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, result, tag.Length, ciphertext.Length);
        return result;
    }

    /// <summary>
    /// Aes-GCM加密（推荐用于高安全性场景）
    /// </summary>
    /// <param name="content">要加密的内容（UTF-8编码）</param>
    /// <param name="keyBytes">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>Base64编码的加密数据（nonce + tag + ciphertext）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 返回格式：Base64(nonce + tag + ciphertext)
    /// - nonce: 12字节（随机数，每次加密必须唯一）
    /// - tag: 16字节（认证标签，验证数据完整性）
    /// - ciphertext: 加密后的数据
    /// 安全提示：使用相同密钥和nonce加密多次会严重危及安全。
    /// </remarks>
    public static byte[] AesGCMEncrypt(byte[] content, byte[] keyBytes)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）");
        byte[] nonce = new byte[AesGcmNonceSize];
        RandomNumberGenerator.Fill(nonce);
        (byte[] tag, byte[] ciphertext) = AesGCMEncryptCore(content, keyBytes, nonce);
        byte[] result = new byte[nonce.Length + tag.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, result, nonce.Length + tag.Length, ciphertext.Length);
        return result;
    }

    /// <summary>
    /// Aes-GCM加密核心实现
    /// </summary>
    /// <param name="content">要加密的内容</param>
    /// <param name="keyBytes">密钥字节数组</param>
    /// <param name="nonce">随机数（12字节）</param>
    /// <returns>加密后的(tag, ciphertext)元组</returns>
    private static (byte[] tag, byte[] ciphertext) AesGCMEncryptCore(byte[] content, byte[] keyBytes, byte[] nonce)
    {
        byte[] ciphertext = new byte[content.Length];
        byte[] tag = new byte[AesGcmTagSize];
        using AesGcm aesGcm = new(keyBytes, AesGcmTagSize);
        aesGcm.Encrypt(nonce, content, ciphertext, tag);
        return (tag, ciphertext);
    }

    /// <summary>
    /// Aes-GCM解密
    /// </summary>
    /// <param name="content">Base64编码的加密数据（nonce + tag + ciphertext格式）</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>解密后的原始内容（UTF-8编码）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：Base64(nonce + tag + ciphertext)
    /// 解密时会自动验证认证标签，如果数据被篡改会抛出异常。
    /// 与 AesGCMEncrypt 方法配对使用。
    /// </remarks>
    public static byte[] AesGCMDecrypt(byte[] content, string key)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        return AesGCMDecrypt(content, keyBytes);
    }

    /// <summary>
    /// Aes-GCM解密
    /// </summary>
    /// <param name="content">Base64编码的加密数据（nonce + tag + ciphertext格式）</param>
    /// <param name="keyBytes">Base64编码的密钥（16/24/32字节）</param>
    /// <returns>解密后的原始内容（UTF-8编码）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：Base64(nonce + tag + ciphertext)
    /// 解密时会自动验证认证标签，如果数据被篡改会抛出异常。
    /// 与 AesGCMEncrypt 方法配对使用。
    /// </remarks>
    public static byte[] AesGCMDecrypt(byte[] content, byte[] keyBytes)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）");
        if (content.Length < AesGcmNonceSize + AesGcmTagSize) throw new ArgumentException("数据长度不足，无法提取nonce和tag");
        byte[] nonce = new byte[AesGcmNonceSize];
        byte[] tagAndCiphertext = new byte[content.Length - AesGcmNonceSize];
        Buffer.BlockCopy(content, 0, nonce, 0, AesGcmNonceSize);
        Buffer.BlockCopy(content, AesGcmNonceSize, tagAndCiphertext, 0, tagAndCiphertext.Length);
        return AesGCMDecrypt(tagAndCiphertext, keyBytes, nonce);
    }

    /// <summary>
    /// Aes-GCM解密（使用单独的nonce）
    /// </summary>
    /// <param name="content">Base64编码的加密数据（tag + ciphertext格式）</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="nonce">Base64编码的nonce（12字节）</param>
    /// <returns>解密后的原始内容（UTF-8编码）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：Base64(tag + ciphertext)
    /// - tag: 16字节（认证标签）
    /// - ciphertext: 加密后的数据
    /// nonce作为单独参数传入，解密时会自动验证认证标签，如果数据被篡改会抛出异常。
    /// 与 AesGCMEncrypt(content, out key, out nonce) 方法配对使用。
    /// </remarks>
    public static byte[] AesGCMDecrypt(byte[] content, string key, string nonce)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] nonceBytes = Convert.FromBase64String(nonce);
        return AesGCMDecrypt(content, keyBytes, nonceBytes);
    }

    /// <summary>
    /// Aes-GCM解密（使用单独的nonce）
    /// </summary>
    /// <param name="content">Base64编码的加密数据（tag + ciphertext格式）</param>
    /// <param name="keyBytes">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="nonceBytes">nonce字节数组（12字节）</param>
    /// <returns>解密后的原始内容（UTF-8编码）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：Base64(tag + ciphertext)
    /// - tag: 16字节（认证标签）
    /// - ciphertext: 加密后的数据
    /// nonce作为单独参数传入，解密时会自动验证认证标签，如果数据被篡改会抛出异常。
    /// 与 AesGCMEncrypt(content, out key, out nonce) 方法配对使用。
    /// </remarks>
    public static byte[] AesGCMDecrypt(byte[] content, byte[] keyBytes, byte[] nonceBytes)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (nonceBytes is null || nonceBytes.Length == 0) throw new ArgumentException("nonce不能为空", nameof(nonceBytes));
        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32) throw new ArgumentException("密钥长度无效，必须是16、24或32字节（对应Aes-128、Aes-192、Aes-256）");
        if (nonceBytes.Length != AesGcmNonceSize) throw new ArgumentException($"nonce长度必须为{AesGcmNonceSize}字节", nameof(nonceBytes));
        if (content.Length < AesGcmTagSize) throw new ArgumentException("数据长度不足，无法提取tag");
        byte[] tag = new byte[AesGcmTagSize];
        byte[] ciphertext = new byte[content.Length - AesGcmTagSize];
        Buffer.BlockCopy(content, 0, tag, 0, AesGcmTagSize);
        Buffer.BlockCopy(content, AesGcmTagSize, ciphertext, 0, ciphertext.Length);
        byte[] plaintext = new byte[ciphertext.Length];
        using AesGcm aesGcm = new(keyBytes, AesGcmTagSize);
        aesGcm.Decrypt(nonceBytes, ciphertext, tag, plaintext);
        return plaintext;
    }
#endif
    #endregion
}
