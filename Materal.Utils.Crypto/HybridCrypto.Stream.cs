using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

/// <summary>
/// HybridCrypto流操作扩展
/// </summary>
public static partial class HybridCrypto
{
    #region 流加密解密
    /// <summary>
    /// 混合加密流
    /// </summary>
    /// <param name="inputStream">要加密的输入流</param>
    /// <param name="outputStream">加密后的输出流</param>
    /// <param name="rsaPublicKey">RSA公钥（支持XML和PEM格式）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>加密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <remarks>
    /// 注意：由于混合加密的特性，此方法会将整个流读入内存进行处理。
    /// 对于超大文件，建议分块处理或使用其他方案。
    /// </remarks>
    public static long Encrypt(Stream inputStream, Stream outputStream, string rsaPublicKey, int bufferSize = 8192)
    {
        if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (!inputStream.CanRead) throw new ArgumentException("输入流必须可读", nameof(inputStream));
        if (!outputStream.CanWrite) throw new ArgumentException("输出流必须可写", nameof(outputStream));
        if (string.IsNullOrEmpty(rsaPublicKey)) throw new ArgumentException("RSA公钥不能为空", nameof(rsaPublicKey));
        if (bufferSize <= 0) throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        // 读取整个流到内存
        using MemoryStream ms = new();
        byte[] buffer = new byte[bufferSize];
        int bytesRead;
        long totalBytes = 0;

        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, bytesRead);
            totalBytes += bytesRead;
        }

        byte[] data = ms.ToArray();

        // 使用混合加密
        byte[] encryptedData = Encrypt(data, rsaPublicKey);

        // 写入输出流
        outputStream.Write(encryptedData, 0, encryptedData.Length);

        return totalBytes;
    }

    /// <summary>
    /// 混合解密流
    /// </summary>
    /// <param name="inputStream">要解密的输入流</param>
    /// <param name="outputStream">解密后的输出流</param>
    /// <param name="rsaPrivateKey">RSA私钥（支持XML和PEM格式）</param>
    /// <param name="bufferSize">缓冲区大小，默认为8192字节</param>
    /// <returns>解密的字节总数</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <remarks>
    /// 注意：由于混合加密的特性，此方法会将整个流读入内存进行处理。
    /// 对于超大文件，建议分块处理或使用其他方案。
    /// </remarks>
    public static long Decrypt(Stream inputStream, Stream outputStream, string rsaPrivateKey, int bufferSize = 8192)
    {
        if (inputStream is null) throw new ArgumentNullException(nameof(inputStream));
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (!inputStream.CanRead) throw new ArgumentException("输入流必须可读", nameof(inputStream));
        if (!outputStream.CanWrite) throw new ArgumentException("输出流必须可写", nameof(outputStream));
        if (string.IsNullOrEmpty(rsaPrivateKey)) throw new ArgumentException("RSA私钥不能为空", nameof(rsaPrivateKey));
        if (bufferSize <= 0) throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        // 读取整个流到内存
        using MemoryStream ms = new();
        byte[] buffer = new byte[bufferSize];
        int bytesRead;
        long totalBytes = 0;

        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, bytesRead);
            totalBytes += bytesRead;
        }

        byte[] encryptedData = ms.ToArray();

        // 使用混合解密
        byte[] decryptedData = Decrypt(encryptedData, rsaPrivateKey);

        // 写入输出流
        outputStream.Write(decryptedData, 0, decryptedData.Length);

        return decryptedData.Length;
    }
    #endregion
}
