using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

/// <summary>
/// Des加解密核心
/// </summary>
public static partial class DesCrypto
{
    #region Des-CBC 加密（非认证加密）
    /// <summary>
    /// 生成Des-CBC密钥和IV
    /// </summary>
    /// <returns>Base64编码的(密钥, IV)元组</returns>
    /// <remarks>
    /// 密钥长度：64位（8字节）
    /// IV长度：64位（8字节）
    /// 注意：每次调用都会生成新的随机密钥和IV
    /// </remarks>
    public static (byte[] key, byte[] iv) GenerateCBCKey()
    {
        using DES des = DES.Create();
        des.GenerateKey();
        des.GenerateIV();
        return (des.Key, des.IV);
    }

    /// <summary>
    /// 生成Des-CBC密钥和IV
    /// </summary>
    /// <returns>Base64编码的(密钥, IV)元组</returns>
    /// <remarks>
    /// 密钥长度：64位（8字节）
    /// IV长度：64位（8字节）
    /// 注意：每次调用都会生成新的随机密钥和IV
    /// </remarks>
    public static (string key, string iv) GenerateCBCStringKey()
    {
        (byte[] keyBytes, byte[] ivBytes) = GenerateCBCKey();
        string key = Convert.ToBase64String(keyBytes);
        string iv = Convert.ToBase64String(ivBytes);
        return (key, iv);
    }

    /// <summary>
    /// Des-CBC加密（PKCS7填充）
    /// </summary>
    /// <param name="content">要加密的内容（UTF-8编码）</param>
    /// <param name="key">Base64编码的密钥（8字节）</param>
    /// <param name="iv">Base64编码的初始化向量（8字节）</param>
    /// <returns>Base64编码的加密数据</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 安全警告：CBC模式下，相同的密钥和IV不应重复使用，这会降低安全性。
    /// 推荐每次加密都使用新的随机IV，或使用自动生成IV的重载方法。
    /// </remarks>
    public static byte[] CBCEncrypt(byte[] content, string key, string iv)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        return CBCEncrypt(content, keyBytes, ivBytes);
    }

    /// <summary>
    /// Des-CBC加密（PKCS7填充）
    /// </summary>
    /// <param name="content">要加密的内容（UTF-8编码）</param>
    /// <param name="keyBytes">密钥字节数组（8字节）</param>
    /// <param name="ivBytes">初始化向量字节数组（8字节）</param>
    /// <returns>Base64编码的加密数据</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    /// <remarks>
    /// 安全警告：CBC模式下，相同的密钥和IV不应重复使用，这会降低安全性。
    /// 推荐每次加密都使用新的随机IV，或使用自动生成IV的重载方法。
    /// </remarks>
    public static byte[] CBCEncrypt(byte[] content, byte[] keyBytes, byte[] ivBytes)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (ivBytes is null || ivBytes.Length == 0) throw new ArgumentException("初始化向量不能为空", nameof(ivBytes));
        if (keyBytes.Length != 8) throw new ArgumentException("密钥长度必须为8字节", nameof(keyBytes));
        if (ivBytes.Length != 8) throw new ArgumentException("IV长度必须为8字节", nameof(ivBytes));

        using MemoryStream inputStream = new(content);
        using MemoryStream outputStream = new();
        CBCEncrypt(inputStream, outputStream, keyBytes, ivBytes);
        return outputStream.ToArray();
    }

    /// <summary>
    /// Des-CBC解密（PKCS7填充）
    /// </summary>
    /// <param name="content">要解密的Base64编码加密数据</param>
    /// <param name="key">Base64编码的密钥（8字节）</param>
    /// <param name="iv">Base64编码的初始化向量（8字节）</param>
    /// <returns>解密后的原始内容（UTF-8编码）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    public static byte[] CBCDecrypt(byte[] content, string key, string iv)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        return CBCDecrypt(content, keyBytes, ivBytes);
    }

    /// <summary>
    /// Des-CBC解密（PKCS7填充）
    /// </summary>
    /// <param name="content">要解密的Base64编码加密数据</param>
    /// <param name="keyBytes">密钥字节数组（8字节）</param>
    /// <param name="ivBytes">初始化向量字节数组（8字节）</param>
    /// <returns>解密后的原始内容（UTF-8编码）</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    public static byte[] CBCDecrypt(byte[] content, byte[] keyBytes, byte[] ivBytes)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (ivBytes is null || ivBytes.Length == 0) throw new ArgumentException("初始化向量不能为空", nameof(ivBytes));
        if (keyBytes.Length != 8) throw new ArgumentException("密钥长度必须为8字节", nameof(keyBytes));
        if (ivBytes.Length != 8) throw new ArgumentException("IV长度必须为8字节", nameof(ivBytes));

        using MemoryStream inputStream = new(content);
        using MemoryStream outputStream = new();
        CBCDecrypt(inputStream, outputStream, keyBytes, ivBytes);
        return outputStream.ToArray();
    }

    /// <summary>
    /// Des-CBC加密并生成密钥和IV
    /// </summary>
    /// <param name="content">要加密的内容</param>
    /// <returns>Base64编码的(加密数据, 密钥, IV)元组</returns>
    /// <remarks>
    /// 每次调用都会生成新的随机密钥和IV，确保安全性
    /// </remarks>
    public static (byte[] encryptedData, byte[] key, byte[] iv) CBCEncryptWithGeneratedKey(byte[] content)
    {
        if (content is null || content.Length == 0) throw new ArgumentException("内容不能为空", nameof(content));
        
        (byte[] keyBytes, byte[] ivBytes) = GenerateCBCKey();
        byte[] encryptedData = CBCEncrypt(content, keyBytes, ivBytes);
        return (encryptedData, keyBytes, ivBytes);
    }

    /// <summary>
    /// Des-CBC加密并生成密钥和IV
    /// </summary>
    /// <param name="content">要加密的内容</param>
    /// <returns>Base64编码的(加密数据, 密钥, IV)元组</returns>
    /// <remarks>
    /// 每次调用都会生成新的随机密钥和IV，确保安全性
    /// </remarks>
    public static (string encryptedData, string key, string iv) CBCEncryptWithGeneratedKey(string content)
    {
        if (string.IsNullOrEmpty(content)) throw new ArgumentException("内容不能为空", nameof(content));
        
        (byte[] encryptedData, byte[] keyBytes, byte[] ivBytes) = CBCEncryptWithGeneratedKey(System.Text.Encoding.UTF8.GetBytes(content));
        string encryptedDataBase64 = Convert.ToBase64String(encryptedData);
        string keyBase64 = Convert.ToBase64String(keyBytes);
        string ivBase64 = Convert.ToBase64String(ivBytes);
        return (encryptedDataBase64, keyBase64, ivBase64);
    }
    #endregion
}
