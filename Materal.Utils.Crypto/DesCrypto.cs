using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

/// <summary>
/// Des加解密流操作扩展
/// </summary>
public static partial class DesCrypto
{
    #region Des-CBC 流加解密
    /// <summary>
    /// Des-CBC加密流（PKCS7填充）
    /// </summary>
    /// <param name="inputStream">要加密的输入流</param>
    /// <param name="outputStream">加密后的输出流</param>
    /// <param name="key">Base64编码的密钥（8字节）</param>
    /// <param name="iv">Base64编码的初始化向量（8字节）</param>
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
    /// Des-CBC加密流（PKCS7填充）
    /// </summary>
    /// <param name="inputStream">要加密的输入流</param>
    /// <param name="outputStream">加密后的输出流</param>
    /// <param name="keyBytes">密钥字节数组（8字节）</param>
    /// <param name="ivBytes">初始化向量字节数组（8字节）</param>
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
        if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (ivBytes is null || ivBytes.Length == 0) throw new ArgumentException("初始化向量不能为空", nameof(ivBytes));
        if (keyBytes.Length != 8) throw new ArgumentException("密钥长度必须为8字节", nameof(keyBytes));
        if (ivBytes.Length != 8) throw new ArgumentException("IV长度必须为8字节", nameof(ivBytes));
        if (bufferSize <= 0) throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        using DES des = DES.Create();
        des.Key = keyBytes;
        des.IV = ivBytes;
        des.Padding = PaddingMode.PKCS7;
        des.Mode = CipherMode.CBC;

        using ICryptoTransform encryptor = des.CreateEncryptor();
        using CryptoStream cryptoStream = new(outputStream, encryptor, CryptoStreamMode.Write);
        
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
    /// Des-CBC解密流（PKCS7填充）
    /// </summary>
    /// <param name="inputStream">要解密的输入流</param>
    /// <param name="outputStream">解密后的输出流</param>
    /// <param name="key">Base64编码的密钥（8字节）</param>
    /// <param name="iv">Base64编码的初始化向量（8字节）</param>
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
    /// Des-CBC解密流（PKCS7填充）
    /// </summary>
    /// <param name="inputStream">要解密的输入流</param>
    /// <param name="outputStream">解密后的输出流</param>
    /// <param name="keyBytes">密钥字节数组（8字节）</param>
    /// <param name="ivBytes">初始化向量字节数组（8字节）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>解密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    public static long CBCDecrypt(Stream inputStream, Stream outputStream, byte[] keyBytes, byte[] ivBytes, int bufferSize = 8192)
    {
        if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (keyBytes is null || keyBytes.Length == 0) throw new ArgumentException("密钥不能为空", nameof(keyBytes));
        if (ivBytes is null || ivBytes.Length == 0) throw new ArgumentException("初始化向量不能为空", nameof(ivBytes));
        if (keyBytes.Length != 8) throw new ArgumentException("密钥长度必须为8字节", nameof(keyBytes));
        if (ivBytes.Length != 8) throw new ArgumentException("IV长度必须为8字节", nameof(ivBytes));
        if (bufferSize <= 0) throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        using DES des = DES.Create();
        des.Key = keyBytes;
        des.IV = ivBytes;
        des.Padding = PaddingMode.PKCS7;
        des.Mode = CipherMode.CBC;

        using ICryptoTransform decryptor = des.CreateDecryptor();
        using CryptoStream cryptoStream = new(inputStream, decryptor, CryptoStreamMode.Read);
        
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
    #endregion
}
