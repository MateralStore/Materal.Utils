using System.Security.Cryptography;
using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// MD5 哈希计算工具类
/// </summary>
/// <remarks>
/// 警告：MD5 算法已不再安全，容易被碰撞攻击。
/// 请勿将 MD5 用于密码存储、数字签名等安全敏感场景。
/// 推荐使用 SHA-256 或更强的哈希算法。
/// </remarks>
public static partial class MD5Crypto
{
    /// <summary>
    /// 计算32位MD5哈希值
    /// </summary>
    /// <param name="inputStr">要计算哈希的字符串</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>32位MD5哈希值</returns>
    /// <exception cref="ArgumentNullException">当输入字符串为null时抛出</exception>
    public static string Hash32(string inputStr, bool isLower = false)
    {
#if NETSTANDARD
        if (inputStr is null) throw new ArgumentNullException(nameof(inputStr));
        byte[] data = Encoding.UTF8.GetBytes(inputStr);
        using MD5 md5 = MD5.Create();
        byte[] output = md5.ComputeHash(data);
#else
        ArgumentNullException.ThrowIfNull(inputStr);
        byte[] data = Encoding.UTF8.GetBytes(inputStr);
        byte[] output = MD5.HashData(data);
#endif
        return FormatHash(output, isLower);
    }

    /// <summary>
    /// 计算16位MD5哈希值
    /// </summary>
    /// <param name="inputStr">要计算哈希的字符串</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>16位MD5哈希值（32位哈希的中间8-24位）</returns>
    /// <exception cref="ArgumentNullException">当输入字符串为null时抛出</exception>
    /// <remarks>
    /// 16位MD5实际上是32位MD5的中间部分（第9到24个字符），
    /// 这种做法虽然常见，但安全性低于完整的32位MD5。
    /// </remarks>
    public static string Hash16(string inputStr, bool isLower = false) => Hash32(inputStr, isLower)[8..24];

    /// <summary>
    /// 计算32位MD5哈希值（字节数组版本）
    /// </summary>
    /// <param name="data">要计算哈希的字节数组</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>32位MD5哈希值</returns>
    /// <exception cref="ArgumentNullException">当输入字节数组为null时抛出</exception>
    public static string Hash32(byte[] data, bool isLower = false)
    {
#if NETSTANDARD
        if (data is null) throw new ArgumentNullException(nameof(data));
        using MD5 md5 = MD5.Create();
        byte[] output = md5.ComputeHash(data);
#else
        ArgumentNullException.ThrowIfNull(data);
        byte[] output = MD5.HashData(data);
#endif
        return FormatHash(output, isLower);
    }

    /// <summary>
    /// 计算16位MD5哈希值（字节数组版本）
    /// </summary>
    /// <param name="data">要计算哈希的字节数组</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>16位MD5哈希值（32位哈希的中间8-24位）</returns>
    /// <exception cref="ArgumentNullException">当输入字节数组为null时抛出</exception>
    public static string Hash16(byte[] data, bool isLower = false) => Hash32(data, isLower)[8..24];

    /// <summary>
    /// 计算32位MD5哈希值（流版本）
    /// </summary>
    /// <param name="stream">要计算哈希的流</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>32位MD5哈希值</returns>
    /// <exception cref="ArgumentNullException">当输入流为null时抛出</exception>
    /// <exception cref="ArgumentException">当流不可读时抛出</exception>
    public static string Hash32(Stream stream, bool isLower = false)
    {
#if NETSTANDARD
        if (stream is null) throw new ArgumentNullException(nameof(stream));
        if (!stream.CanRead) throw new ArgumentException("流必须可读", nameof(stream));
        using MD5 md5 = MD5.Create();
        byte[] output = md5.ComputeHash(stream);
#else
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead) throw new ArgumentException("流必须可读", nameof(stream));
        byte[] output = MD5.HashData(stream);
#endif
        return FormatHash(output, isLower);
    }

    /// <summary>
    /// 计算16位MD5哈希值（流版本）
    /// </summary>
    /// <param name="stream">要计算哈希的流</param>
    /// <param name="isLower">是否返回小写格式，默认返回大写</param>
    /// <returns>16位MD5哈希值（32位哈希的中间8-24位）</returns>
    /// <exception cref="ArgumentNullException">当输入流为null时抛出</exception>
    /// <exception cref="ArgumentException">当流不可读时抛出</exception>
    public static string Hash16(Stream stream, bool isLower = false) => Hash32(stream, isLower)[8..24];

    /// <summary>
    /// 格式化哈希字节数组为字符串
    /// </summary>
    /// <param name="hash">哈希字节数组</param>
    /// <param name="isLower">是否返回小写格式</param>
    /// <returns>格式化的哈希字符串</returns>
    private static string FormatHash(byte[] hash, bool isLower)
    {
#if NET9_0_OR_GREATER
        string outputStr = Convert.ToHexString(hash);
#else
        string outputStr = BitConverter.ToString(hash).Replace("-", "");
#endif
        outputStr = isLower ? outputStr.ToLower() : outputStr.ToUpper();
        return outputStr;
    }
}
