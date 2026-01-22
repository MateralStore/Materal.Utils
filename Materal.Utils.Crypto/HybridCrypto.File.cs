using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

/// <summary>
/// HybridCrypto文件操作扩展
/// </summary>
public static partial class HybridCrypto
{
    #region 文件加密解密
    /// <summary>
    /// 混合加密文件
    /// </summary>
    /// <param name="inputFileInfo">要加密的文件信息</param>
    /// <param name="outputFileInfo">加密后的输出文件信息</param>
    /// <param name="rsaPublicKey">RSA公钥（支持XML和PEM格式）</param>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 注意：由于混合加密的特性，此方法会将整个文件读入内存进行处理。
    /// 对于超大文件，建议分块处理或使用其他方案。
    /// </remarks>
    public static void EncryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, string rsaPublicKey)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");
        if (string.IsNullOrEmpty(rsaPublicKey)) throw new ArgumentException("RSA公钥不能为空", nameof(rsaPublicKey));

        // 确保输出目录存在
        string? outputDir = Path.GetDirectoryName(outputFileInfo.FullName);
        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        using FileStream inputStream = inputFileInfo.OpenRead();
        using FileStream outputStream = outputFileInfo.Create();
        Encrypt(inputStream, outputStream, rsaPublicKey);
    }

    /// <summary>
    /// 混合加密文件
    /// </summary>
    /// <param name="inputFilePath">要加密的文件路径</param>
    /// <param name="outputFilePath">加密后的输出文件路径</param>
    /// <param name="rsaPublicKey">RSA公钥（支持XML和PEM格式）</param>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 注意：由于混合加密的特性，此方法会将整个文件读入内存进行处理。
    /// 对于超大文件，建议分块处理或使用其他方案。
    /// </remarks>
    public static void EncryptFile(string inputFilePath, string outputFilePath, string rsaPublicKey)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        EncryptFile(inputFileInfo, outputFileInfo, rsaPublicKey);
    }

    /// <summary>
    /// 混合解密文件
    /// </summary>
    /// <param name="inputFileInfo">要解密的文件信息</param>
    /// <param name="outputFileInfo">解密后的输出文件信息</param>
    /// <param name="rsaPrivateKey">RSA私钥（支持XML和PEM格式）</param>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 注意：由于混合加密的特性，此方法会将整个文件读入内存进行处理。
    /// 对于超大文件，建议分块处理或使用其他方案。
    /// </remarks>
    public static void DecryptFile(FileInfo inputFileInfo, FileInfo outputFileInfo, string rsaPrivateKey)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");
        if (string.IsNullOrEmpty(rsaPrivateKey)) throw new ArgumentException("RSA私钥不能为空", nameof(rsaPrivateKey));

        // 确保输出目录存在
        string? outputDir = Path.GetDirectoryName(outputFileInfo.FullName);
        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        using FileStream inputStream = inputFileInfo.OpenRead();
        using FileStream outputStream = outputFileInfo.Create();
        Decrypt(inputStream, outputStream, rsaPrivateKey);
    }

    /// <summary>
    /// 混合解密文件
    /// </summary>
    /// <param name="inputFilePath">要解密的文件路径</param>
    /// <param name="outputFilePath">解密后的输出文件路径</param>
    /// <param name="rsaPrivateKey">RSA私钥（支持XML和PEM格式）</param>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 注意：由于混合加密的特性，此方法会将整个文件读入内存进行处理。
    /// 对于超大文件，建议分块处理或使用其他方案。
    /// </remarks>
    public static void DecryptFile(string inputFilePath, string outputFilePath, string rsaPrivateKey)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));

        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        DecryptFile(inputFileInfo, outputFileInfo, rsaPrivateKey);
    }
    #endregion
}
