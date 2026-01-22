using System.Security.Cryptography;
using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// Aes加解密字符串操作扩展
/// </summary>
public static partial class AesCrypto
{
    #region Aes-CBC 字符串加解密
    /// <summary>
    /// Aes-CBC加密字符串（PKCS7填充）
    /// </summary>
    /// <param name="content">要加密的字符串</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="iv">Base64编码的初始化向量（16字节）</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
    /// <returns>Base64编码的加密数据</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 安全警告：CBC模式下，相同的密钥和IV不应重复使用，这会降低安全性。
    /// 推荐每次加密都使用新的随机IV，或使用自动生成IV的重载方法。
    /// </remarks>
    public static string CBCEncrypt(string content, string key, string iv, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(content)) throw new ArgumentException("内容不能为空", nameof(content));
        byte[] contentBytes = encoding.GetBytes(content);
        byte[] encryptedBytes = CBCEncrypt(contentBytes, key, iv);
        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Aes-CBC解密字符串（PKCS7填充）
    /// </summary>
    /// <param name="content">Base64编码的加密数据</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="iv">Base64编码的初始化向量（16字节）</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
    /// <returns>解密后的原始字符串</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    public static string CBCDecrypt(string content, string key, string iv, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(content)) throw new ArgumentException("内容不能为空", nameof(content));
        byte[] contentBytes = Convert.FromBase64String(content);
        byte[] decryptedBytes = CBCDecrypt(contentBytes, key, iv);
        return encoding.GetString(decryptedBytes);
    }

    /// <summary>
    /// Aes-CBC加密字符串（自动生成密钥和IV）
    /// </summary>
    /// <param name="content">要加密的字符串</param>
    /// <param name="key">输出的Base64编码密钥</param>
    /// <param name="iv">输出的Base64编码IV</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
    /// <returns>Base64编码的加密数据</returns>
    /// <remarks>
    /// 此方法会生成新的随机密钥和IV，适用于需要生成密钥的场景。
    /// 请妥善保存输出的密钥和IV，解密时需要使用相同的密钥和IV。
    /// </remarks>
    public static string CBCEncrypt(string content, out string key, out string iv, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(content)) throw new ArgumentException("内容不能为空", nameof(content));
        byte[] contentBytes = encoding.GetBytes(content);
        byte[] encryptedBytes = CBCEncrypt(contentBytes, out key, out iv);
        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Aes-CBC加密字符串（使用随机IV）
    /// </summary>
    /// <param name="content">要加密的字符串</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
    /// <returns>Base64编码的加密数据（IV前置）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 返回格式：Base64(IV + 加密数据)
    /// IV（16字节）会被前置到加密数据前，解密时自动提取。
    /// 这种方式避免了单独管理IV的麻烦，推荐在大多数场景中使用。
    /// </remarks>
    public static string CBCEncrypt(string content, string key, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(content)) throw new ArgumentException("内容不能为空", nameof(content));
        byte[] contentBytes = encoding.GetBytes(content);
        byte[] encryptedBytes = CBCEncrypt(contentBytes, key);
        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Aes-CBC解密字符串（自动提取IV）
    /// </summary>
    /// <param name="content">Base64编码的加密数据（IV前置格式）</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
    /// <returns>解密后的原始字符串</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 输入格式：Base64(IV + 加密数据)
    /// 会自动从前16字节提取IV进行解密。
    /// 与 AesCBCEncrypt(content, key) 方法配对使用。
    /// </remarks>
    public static string CBCDecrypt(string content, string key, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(content)) throw new ArgumentException("内容不能为空", nameof(content));
        byte[] contentBytes = Convert.FromBase64String(content);
        byte[] decryptedBytes = CBCDecrypt(contentBytes, key);
        return encoding.GetString(decryptedBytes);
    }
    #endregion

#if NET
    #region Aes-GCM 字符串加解密
    /// <summary>
    /// Aes-GCM加密字符串（推荐用于高安全性场景）
    /// </summary>
    /// <param name="content">要加密的字符串</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
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
    public static string GCMEncrypt(string content, string key, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(content)) throw new ArgumentException("内容不能为空", nameof(content));
        byte[] contentBytes = encoding.GetBytes(content);
        byte[] encryptedBytes = GCMEncrypt(contentBytes, key);
        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Aes-GCM加密字符串（自动生成密钥和nonce）
    /// </summary>
    /// <param name="content">要加密的字符串</param>
    /// <param name="key">输出的Base64编码密钥</param>
    /// <param name="nonce">输出的Base64编码nonce</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
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
    public static string GCMEncrypt(string content, out string key, out string nonce, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(content)) throw new ArgumentException("内容不能为空", nameof(content));
        byte[] contentBytes = encoding.GetBytes(content);
        byte[] encryptedBytes = GCMEncrypt(contentBytes, out key, out nonce);
        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Aes-GCM解密字符串
    /// </summary>
    /// <param name="content">Base64编码的加密数据（nonce + tag + ciphertext格式）</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
    /// <returns>解密后的原始字符串</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败或认证失败时抛出</exception>
    /// <exception cref="PlatformNotSupportedException">在.NET Standard 2.0中不支持</exception>
    /// <remarks>
    /// 输入格式：Base64(nonce + tag + ciphertext)
    /// 解密时会自动验证认证标签，如果数据被篡改会抛出异常。
    /// 与 AesGCMEncryptString 方法配对使用。
    /// </remarks>
    public static string GCMDecrypt(string content, string key, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(content)) throw new ArgumentException("内容不能为空", nameof(content));
        byte[] contentBytes = Convert.FromBase64String(content);
        byte[] decryptedBytes = GCMDecrypt(contentBytes, key);
        return encoding.GetString(decryptedBytes);
    }

    /// <summary>
    /// Aes-GCM解密字符串（使用单独的nonce）
    /// </summary>
    /// <param name="content">Base64编码的加密数据（tag + ciphertext格式）</param>
    /// <param name="key">Base64编码的密钥（16/24/32字节）</param>
    /// <param name="nonce">Base64编码的nonce（12字节）</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
    /// <returns>解密后的原始字符串</returns>
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
    public static string GCMDecrypt(string content, string key, string nonce, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(content)) throw new ArgumentException("内容不能为空", nameof(content));
        byte[] contentBytes = Convert.FromBase64String(content);
        byte[] decryptedBytes = GCMDecrypt(contentBytes, key, nonce);
        return encoding.GetString(decryptedBytes);
    }
    #endregion
#endif
}
