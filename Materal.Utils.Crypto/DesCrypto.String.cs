using System.Security.Cryptography;
using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// Des加解密字符串操作扩展
/// </summary>
public static partial class DesCrypto
{
    #region Des-CBC 字符串加解密
    /// <summary>
    /// Des-CBC加密字符串（PKCS7填充）
    /// </summary>
    /// <param name="content">要加密的字符串</param>
    /// <param name="key">Base64编码的密钥（8字节）</param>
    /// <param name="iv">Base64编码的初始化向量（8字节）</param>
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
    /// Des-CBC解密字符串（PKCS7填充）
    /// </summary>
    /// <param name="content">Base64编码的加密数据</param>
    /// <param name="key">Base64编码的密钥（8字节）</param>
    /// <param name="iv">Base64编码的初始化向量（8字节）</param>
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
    #endregion
}
