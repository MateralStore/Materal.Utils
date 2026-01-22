using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

/// <summary>
/// SHA256 哈希计算工具类（文件扩展）
/// </summary>
public static partial class SHA256Crypto
{
    /// <summary>
    /// 计算64位SHA256哈希值（文件路径版本）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>64位SHA256哈希值</returns>
    /// <exception cref="ArgumentNullException">当文件路径为null时抛出</exception>
    /// <exception cref="ArgumentException">当文件路径为空或文件不存在时抛出</exception>
    /// <exception cref="FileNotFoundException">当文件不存在时抛出</exception>
    /// <exception cref="UnauthorizedAccessException">当没有权限访问文件时抛出</exception>
    public static string HashFromFile(string filePath, bool isLower = false)
    {
#if NETSTANDARD
        if (filePath is null) throw new ArgumentNullException(nameof(filePath));
#else
        ArgumentNullException.ThrowIfNull(filePath);
#endif
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("文件路径不能为空", nameof(filePath));
        if (!File.Exists(filePath)) throw new FileNotFoundException("文件不存在", filePath);
        using FileStream stream = File.OpenRead(filePath);
        return Hash(stream, isLower);
    }

    /// <summary>
    /// 计算64位SHA256哈希值（FileInfo版本）
    /// </summary>
    /// <param name="fileInfo">文件信息</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>64位SHA256哈希值</returns>
    /// <exception cref="ArgumentNullException">当FileInfo为null时抛出</exception>
    /// <exception cref="FileNotFoundException">当文件不存在时抛出</exception>
    /// <exception cref="UnauthorizedAccessException">当没有权限访问文件时抛出</exception>
    public static string HashFromFile(FileInfo fileInfo, bool isLower = false)
    {
#if NETSTANDARD
        if (fileInfo is null) throw new ArgumentNullException(nameof(fileInfo));
#else
        ArgumentNullException.ThrowIfNull(fileInfo);
#endif
        if (!fileInfo.Exists) throw new FileNotFoundException("文件不存在", fileInfo.FullName);
        using FileStream stream = fileInfo.OpenRead();
        return Hash(stream, isLower);
    }
}
