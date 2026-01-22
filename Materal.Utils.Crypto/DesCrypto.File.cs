using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

/// <summary>
/// Des加解密文件操作扩展
/// </summary>
public static partial class DesCrypto
{
    #region Des-CBC 文件加解密
    /// <summary>
    /// Des-CBC加密文件（PKCS7填充）
    /// </summary>
    /// <param name="inputFileInfo">要加密的文件信息</param>
    /// <param name="outputFileInfo">加密后的输出文件信息</param>
    /// <param name="key">Base64编码的密钥（8字节）</param>
    /// <param name="iv">Base64编码的初始化向量（8字节）</param>
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
    /// Des-CBC加密文件（PKCS7填充）
    /// </summary>
    /// <param name="inputFilePath">要加密的文件路径</param>
    /// <param name="outputFilePath">加密后的输出文件路径</param>
    /// <param name="key">Base64编码的密钥（8字节）</param>
    /// <param name="iv">Base64编码的初始化向量（8字节）</param>
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
    /// Des-CBC解密文件（PKCS7填充）
    /// </summary>
    /// <param name="inputFileInfo">要解密的文件信息</param>
    /// <param name="outputFileInfo">解密后的输出文件信息</param>
    /// <param name="key">Base64编码的密钥（8字节）</param>
    /// <param name="iv">Base64编码的初始化向量（8字节）</param>
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
    /// Des-CBC解密文件（PKCS7填充）
    /// </summary>
    /// <param name="inputFilePath">要解密的文件路径</param>
    /// <param name="outputFilePath">解密后的输出文件路径</param>
    /// <param name="key">Base64编码的密钥（8字节）</param>
    /// <param name="iv">Base64编码的初始化向量（8字节）</param>
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
    /// Des-CBC加密文件并生成密钥和IV
    /// </summary>
    /// <param name="inputFileInfo">要加密的文件信息</param>
    /// <param name="outputFileInfo">加密后的输出文件信息</param>
    /// <returns>Base64编码的(密钥, IV)元组</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 每次调用都会生成新的随机密钥和IV，确保安全性
    /// </remarks>
    public static (string key, string iv) CBCEncryptFileWithGeneratedKey(FileInfo inputFileInfo, FileInfo outputFileInfo)
    {
        if (inputFileInfo is null) throw new ArgumentNullException(nameof(inputFileInfo));
        if (outputFileInfo is null) throw new ArgumentNullException(nameof(outputFileInfo));
        if (!inputFileInfo.Exists) throw new FileNotFoundException($"输入文件不存在: {inputFileInfo.FullName}");

        (string key, string iv) = GenerateCBCStringKey();
        CBCEncryptFile(inputFileInfo, outputFileInfo, key, iv);
        return (key, iv);
    }

    /// <summary>
    /// Des-CBC加密文件并生成密钥和IV
    /// </summary>
    /// <param name="inputFilePath">要加密的文件路径</param>
    /// <param name="outputFilePath">加密后的输出文件路径</param>
    /// <returns>Base64编码的(密钥, IV)元组</returns>
    /// <exception cref="ArgumentException">当参数为空或无效时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="FileNotFoundException">输入文件不存在时抛出</exception>
    /// <remarks>
    /// 每次调用都会生成新的随机密钥和IV，确保安全性
    /// </remarks>
    public static (string key, string iv) CBCEncryptFileWithGeneratedKey(string inputFilePath, string outputFilePath)
    {
        if (string.IsNullOrEmpty(inputFilePath)) throw new ArgumentException("输入文件路径不能为空", nameof(inputFilePath));
        if (string.IsNullOrEmpty(outputFilePath)) throw new ArgumentException("输出文件路径不能为空", nameof(outputFilePath));
        
        FileInfo inputFileInfo = new(inputFilePath);
        FileInfo outputFileInfo = new(outputFilePath);
        return CBCEncryptFileWithGeneratedKey(inputFileInfo, outputFileInfo);
    }
    #endregion
}
