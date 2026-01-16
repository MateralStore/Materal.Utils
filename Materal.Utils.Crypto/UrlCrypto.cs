using System.Text;
using System.Web;

namespace Materal.Utils.Crypto;

/// <summary>
/// URL 编码解码工具类
/// </summary>
public static partial class UrlCrypto
{
    /// <summary>
    /// 对字符串进行 URL 编码
    /// </summary>
    /// <param name="plainText">要编码的字符串</param>
    /// <param name="encoding">字符编码，默认为 UTF-8</param>
    /// <returns>URL 编码后的字符串</returns>
    public static string Encode(string plainText, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return HttpUtility.UrlEncode(plainText, encoding);
    }

    /// <summary>
    /// 对字符串进行 URL 解码
    /// </summary>
    /// <param name="encodedText">URL 编码的字符串</param>
    /// <param name="encoding">字符编码，默认为 UTF-8</param>
    /// <returns>解码后的原始字符串</returns>
    public static string Decode(string encodedText, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return HttpUtility.UrlDecode(encodedText, encoding);
    }

    /// <summary>
    /// 对字符串进行 URL 路径编码（不编码斜杠和问号）
    /// </summary>
    /// <param name="plainText">要编码的字符串</param>
    /// <returns>URL 路径编码后的字符串</returns>
    public static string PathEncode(string plainText) => HttpUtility.UrlPathEncode(plainText);

    /// <summary>
    /// 对字符串进行 URL 编码（仅编码特殊字符，保留空格）
    /// </summary>
    /// <param name="plainText">要编码的字符串</param>
    /// <param name="encoding">字符编码，默认为 UTF-8</param>
    /// <returns>URL 编码后的字符串</returns>
    public static string EncodeNoSpace(string plainText, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        string encoded = HttpUtility.UrlEncode(plainText, encoding);
        return encoded.Replace("+", "%20");
    }

    /// <summary>
    /// 对 URL 参数进行编码（键值对形式）
    /// </summary>
    /// <param name="parameters">参数字典</param>
    /// <param name="encoding">字符编码，默认为 UTF-8</param>
    /// <returns>编码后的 URL 参数字符串</returns>
    public static string EncodeParameters(Dictionary<string, string> parameters, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var encodedPairs = new List<string>();

        foreach (var kvp in parameters)
        {
            string encodedKey = HttpUtility.UrlEncode(kvp.Key, encoding);
            string encodedValue = HttpUtility.UrlEncode(kvp.Value, encoding);
            encodedPairs.Add($"{encodedKey}={encodedValue}");
        }

        return string.Join("&", encodedPairs);
    }

    /// <summary>
    /// 解析 URL 参数字符串为字典
    /// </summary>
    /// <param name="queryString">URL 参数字符串（如：key1=value1&amp;key2=value2）</param>
    /// <param name="encoding">字符编码，默认为 UTF-8</param>
    /// <returns>参数字典</returns>
    public static Dictionary<string, string> DecodeParameters(string queryString, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var parameters = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(queryString))
            return parameters;

#if NETSTANDARD2_0
        if (queryString.StartsWith("?"))
#else
        if (queryString.StartsWith('?'))
#endif
        {
            queryString = queryString[1..];
        }

        string[] pairs = queryString.Split('&');

        foreach (string pair in pairs)
        {
            if (string.IsNullOrEmpty(pair)) continue;
#if NETSTANDARD2_0
            string[] keyValue = pair.Split('=');
#else
            string[] keyValue = pair.Split('=', 2);
#endif
            string key = HttpUtility.UrlDecode(keyValue[0], encoding);
            string value = keyValue.Length > 1 ? HttpUtility.UrlDecode(keyValue[1], encoding) : string.Empty;

            parameters[key] = value;
        }

        return parameters;
    }

    /// <summary>
    /// 对 Base64 字符串进行 URL 安全编码（替换 + 和 / 为 - 和 _）
    /// </summary>
    /// <param name="base64Text">Base64 字符串</param>
    /// <returns>URL 安全的 Base64 字符串</returns>
    public static string Base64UrlEncode(string base64Text)
    {
        return base64Text.Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    /// <summary>
    /// 解码 URL 安全的 Base64 字符串
    /// </summary>
    /// <param name="base64UrlText">URL 安全的 Base64 字符串</param>
    /// <returns>标准 Base64 字符串</returns>
    public static string Base64UrlDecode(string base64UrlText)
    {
        string base64Text = base64UrlText.Replace("-", "+").Replace("_", "/");

        // 补齐填充字符
        switch (base64Text.Length % 4)
        {
            case 2:
                base64Text += "==";
                break;
            case 3:
                base64Text += "=";
                break;
        }

        return base64Text;
    }
}
