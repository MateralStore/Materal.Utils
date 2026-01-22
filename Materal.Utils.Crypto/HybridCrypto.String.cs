using System.Security.Cryptography;
using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// HybridCrypto字符串操作扩展
/// </summary>
public static partial class HybridCrypto
{
    #region 字符串加密解密
    /// <summary>
    /// 混合加密字符串
    /// </summary>
    /// <param name="plainText">要加密的明文</param>
    /// <param name="rsaPublicKey">RSA公钥（支持XML和PEM格式）</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
    /// <returns>Base64编码的加密字符串</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <remarks>
    /// 输出格式为Base64编码的混合加密数据
    /// </remarks>
    public static string Encrypt(string plainText, string rsaPublicKey, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("明文不能为空", nameof(plainText));
        if (string.IsNullOrEmpty(rsaPublicKey))
            throw new ArgumentException("RSA公钥不能为空", nameof(rsaPublicKey));

        byte[] data = encoding.GetBytes(plainText);
        byte[] encryptedData = Encrypt(data, rsaPublicKey);
        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// 混合解密字符串
    /// </summary>
    /// <param name="cipherText">Base64编码的加密字符串</param>
    /// <param name="rsaPrivateKey">RSA私钥（支持XML和PEM格式）</param>
    /// <param name="encoding">编码格式，默认为UTF8</param>
    /// <returns>解密后的原始字符串</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    public static string Decrypt(string cipherText, string rsaPrivateKey, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentException("密文不能为空", nameof(cipherText));
        if (string.IsNullOrEmpty(rsaPrivateKey))
            throw new ArgumentException("RSA私钥不能为空", nameof(rsaPrivateKey));

        try
        {
            byte[] encryptedData = Convert.FromBase64String(cipherText);
            byte[] decryptedData = Decrypt(encryptedData, rsaPrivateKey);
            return encoding.GetString(decryptedData);
        }
        catch (FormatException)
        {
            throw new FormatException("密文格式无效，必须为有效的Base64字符串");
        }
    }
    #endregion
}
