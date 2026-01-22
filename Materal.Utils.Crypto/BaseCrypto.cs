using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// 进制编码/解码工具类
/// 支持文本在不同进制之间的转换
/// </summary>
public static partial class BaseCrypto
{
    /// <summary>
    /// 二进制字符集
    /// </summary>
    private const string _binary = "01";

    /// <summary>
    /// 十进制字符集
    /// </summary>
    private const string _decimalism = "0123456789";

    /// <summary>
    /// 十六进制字符集（大写）
    /// </summary>
    private const string _hex = "0123456789ABCDEF";

    /// <summary>
    /// 社会主义核心价值观字符集
    /// </summary>
    private const string _coreSocialistValues = "富强民主文明和谐自由平等公正法治爱国敬业诚信友善";

    /// <summary>
    /// 将文本编码为二进制字符串
    /// </summary>
    /// <param name="plainText">要编码的明文</param>
    /// <returns>二进制编码字符串</returns>
    public static string EncodeBinary(string plainText) => Encode(plainText, _binary);

    /// <summary>
    /// 将二进制字符串解码为文本
    /// </summary>
    /// <param name="cipherText">二进制编码字符串</param>
    /// <returns>解码后的明文</returns>
    public static string DecodeBinary(string cipherText) => Decode(cipherText, _binary);

    /// <summary>
    /// 将文本编码为十进制字符串
    /// </summary>
    /// <param name="plainText">要编码的明文</param>
    /// <returns>十进制编码字符串</returns>
    public static string EncodeDecimalism(string plainText) => Encode(plainText, _decimalism);

    /// <summary>
    /// 将十进制字符串解码为文本
    /// </summary>
    /// <param name="cipherText">十进制编码字符串</param>
    /// <returns>解码后的明文</returns>
    public static string DecodeDecimalism(string cipherText) => Decode(cipherText, _decimalism);

    /// <summary>
    /// 将文本编码为十六进制字符串
    /// </summary>
    /// <param name="plainText">要编码的明文</param>
    /// <returns>十六进制编码字符串</returns>
    public static string EncodeHex(string plainText) => Encode(plainText, _hex);

    /// <summary>
    /// 将十六进制字符串解码为文本
    /// </summary>
    /// <param name="cipherText">十六进制编码字符串</param>
    /// <returns>解码后的明文</returns>
    public static string DecodeHex(string cipherText) => Decode(cipherText, _hex);

    /// <summary>
    /// 将文本编码为社会主义核心价值观字符串
    /// </summary>
    /// <param name="plainText">要编码的明文</param>
    /// <returns>社会主义核心价值观编码字符串</returns>
    public static string EncodeCoreSocialistValues(string plainText) => Encode(plainText, _coreSocialistValues);

    /// <summary>
    /// 将社会主义核心价值观字符串解码为文本
    /// </summary>
    /// <param name="cipherText">社会主义核心价值观编码字符串</param>
    /// <returns>解码后的明文</returns>
    public static string DecodeCoreSocialistValues(string cipherText) => Decode(cipherText, _coreSocialistValues);

    /// <summary>
    /// 使用字符串字典将文本编码为指定进制
    /// </summary>
    /// <param name="plainText">要编码的明文</param>
    /// <param name="dict">进制字符集</param>
    /// <returns>编码后的字符串</returns>
    public static string Encode(string plainText, string dict) => Encode(plainText, dict.ToCharArray());

    /// <summary>
    /// 使用字符串字典将指定进制字符串解码为文本
    /// </summary>
    /// <param name="cipherText">编码字符串</param>
    /// <param name="dict">进制字符集</param>
    /// <returns>解码后的明文</returns>
    public static string Decode(string cipherText, string dict) => Decode(cipherText, dict.ToCharArray());

    /// <summary>
    /// 使用字符数组字典将文本编码为指定进制
    /// </summary>
    /// <param name="plainText">要编码的明文</param>
    /// <param name="dict">进制字符集</param>
    /// <returns>编码后的字符串</returns>
    /// <exception cref="ArgumentException">当字符集为空或明文为null时抛出</exception>
    /// <exception cref="ArgumentException">当字符集包含重复字符时抛出</exception>
    public static string Encode(string plainText, params char[] dict)
    {
        if (dict == null || dict.Length == 0) throw new ArgumentException("字符集不能为空", nameof(dict));

        // 检查字符集是否有重复字符
        HashSet<char> uniqueChars = [.. dict];
        if (uniqueChars.Count != dict.Length)
        {
            throw new ArgumentException("字符集不能包含重复字符", nameof(dict));
        }

        if (string.IsNullOrEmpty(plainText)) return string.Empty;

        // 将文本转换为字节数组（使用UTF-8编码）
        byte[] bytes = Encoding.UTF8.GetBytes(plainText);
        StringBuilder result = new();

        foreach (byte b in bytes)
        {
            // 将字节转换为指定进制的字符串
            string encoded = ConvertToBase(b, dict);
            result.Append(encoded);
        }

        return result.ToString();
    }

    /// <summary>
    /// 使用字符数组字典将指定进制字符串解码为文本
    /// </summary>
    /// <param name="cipherText">编码字符串</param>
    /// <param name="dict">进制字符集</param>
    /// <returns>解码后的明文</returns>
    /// <exception cref="ArgumentException">当字符集为空或密文为null时抛出</exception>
    /// <exception cref="ArgumentException">当字符集包含重复字符时抛出</exception>
    /// <exception cref="FormatException">当密文包含无效字符时抛出</exception>
    public static string Decode(string cipherText, params char[] dict)
    {
        if (dict == null || dict.Length == 0) throw new ArgumentException("字符集不能为空", nameof(dict));

        // 检查字符集是否有重复字符
        HashSet<char> uniqueChars = [.. dict];
        if (uniqueChars.Count != dict.Length)
        {
            throw new ArgumentException("字符集不能包含重复字符", nameof(dict));
        }

        if (string.IsNullOrEmpty(cipherText)) return string.Empty;

        // 创建字符到值的映射
        Dictionary<char, int> charToValue = [];
        for (int i = 0; i < dict.Length; i++)
        {
            charToValue[dict[i]] = i;
        }

        int baseValue = dict.Length;
        // 计算每个字节编码后的固定长度
        int charsPerByte = GetCharsPerByte(baseValue);

        // 验证密文长度是否为固定长度的倍数
        if (cipherText.Length % charsPerByte != 0)
        {
            throw new FormatException($"密文长度无效,应为 {charsPerByte} 的倍数");
        }

        List<byte> bytes = [];
        // 按固定长度分组解码
        for (int i = 0; i < cipherText.Length; i += charsPerByte)
        {
            string chunk = cipherText.Substring(i, charsPerByte);
            int value = 0;

            // 将字符串转换为数值
            foreach (char c in chunk)
            {
                if (!charToValue.TryGetValue(c, out int digitValue))
                    throw new FormatException($"无效字符: {c}");
                value = value * baseValue + digitValue;
            }

            // 验证值是否在字节范围内
            if (value > 255)
                throw new FormatException($"解码值超出字节范围: {value}");

            bytes.Add((byte)value);
        }

        // 将字节数组转换为文本
        return Encoding.UTF8.GetString([.. bytes]);
    }

    /// <summary>
    /// 将字节转换为指定进制的字符串
    /// </summary>
    /// <param name="value">字节值</param>
    /// <param name="dict">进制字符集</param>
    /// <returns>进制字符串</returns>
    private static string ConvertToBase(byte value, char[] dict)
    {
        if (dict == null || dict.Length < 2)
        {
            throw new ArgumentException("字典长度必须至少为2", nameof(dict));
        }
        int baseValue = dict.Length;
        StringBuilder result = new();

        // 计算需要的固定长度
        int length = GetCharsPerByte(baseValue);

        // 转换为指定进制
        int tempValue = value;
        do
        {
            int remainder = tempValue % baseValue;
            result.Insert(0, dict[remainder]);
            tempValue /= baseValue;
        } while (tempValue > 0);

        // 左侧填充到固定长度
        while (result.Length < length)
        {
            result.Insert(0, dict[0]);
        }

        return result.ToString();
    }

    /// <summary>
    /// 计算每个字节在指定进制下需要的字符数
    /// </summary>
    /// <param name="baseValue">进制基数</param>
    /// <returns>字符数</returns>
    private static int GetCharsPerByte(int baseValue)
    {
        // 计算表示255需要多少位
        // 例如: 十进制需要3位(255), 十六进制需要2位(FF), 二进制需要8位
        return (int)Math.Ceiling(Math.Log(256, baseValue));
    }
}