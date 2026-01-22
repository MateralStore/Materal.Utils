using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// Base64 编码解码工具类
/// </summary>
public static partial class Base64Crypto
{
    /// <summary>
    /// 将字符串进行 Base64 编码
    /// </summary>
    /// <param name="plainText">要编码的明文字符串</param>
    /// <param name="encoding">字符编码，默认为 UTF-8</param>
    /// <returns>Base64 编码后的字符串</returns>
    public static string Encode(string plainText, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        byte[] input = encoding.GetBytes(plainText);
        return Convert.ToBase64String(input);
    }

    /// <summary>
    /// 将 Base64 编码的字符串解码为原始字符串
    /// </summary>
    /// <param name="base64Text">Base64 编码的字符串</param>
    /// <param name="encoding">字符编码，默认为 UTF-8</param>
    /// <returns>解码后的原始字符串</returns>
    public static string Decode(string base64Text, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        byte[] input = Convert.FromBase64String(base64Text);
        return encoding.GetString(input);
    }
}
