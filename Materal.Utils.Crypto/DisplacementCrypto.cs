using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// 位移加密工具类，实现凯撒密码算法
/// 支持大写字母的位移加密，小写字母会转换为大写处理
/// </summary>
public static partial class DisplacementCrypto
{
    private static readonly Dictionary<char, int> UpperCaseMap = [];
    private static readonly char[] UpperCaseAlphabet = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];

    static DisplacementCrypto()
    {
        // 初始化字符映射表以提高性能
        for (int i = 0; i < UpperCaseAlphabet.Length; i++)
        {
            UpperCaseMap[UpperCaseAlphabet[i]] = i;
        }
    }

    /// <summary>
    /// 使用位移算法加密字符串
    /// </summary>
    /// <param name="inputStr">要加密的字符串</param>
    /// <param name="key">位移量，默认为3</param>
    /// <returns>加密后的字符串</returns>
    /// <exception cref="ArgumentNullException">输入字符串为null时抛出</exception>
    /// <exception cref="ArgumentException">输入字符串为空时抛出</exception>
    /// <exception cref="UtilException">字符串包含不支持的字符时抛出</exception>
    public static string Encrypt(string inputStr, int key = 3)
    {
        // 参数验证
#if NET
        ArgumentNullException.ThrowIfNull(inputStr);
#else
        if (string.IsNullOrEmpty(inputStr)) throw new ArgumentNullException("输入字符串不能为空", nameof(inputStr));
#endif

        if (inputStr.Length == 0) throw new ArgumentException("输入字符串不能为空", nameof(inputStr));


        StringBuilder outputStr = new(inputStr.Length);

        foreach (char c in inputStr)
        {
            if (char.IsWhiteSpace(c))
            {
                outputStr.Append(c);
                continue;
            }

            if (char.IsUpper(c))
            {
                outputStr.Append(DisplaceChar(c, key, UpperCaseAlphabet, UpperCaseMap));
            }
            else if (char.IsLower(c))
            {
                // 转换为大写处理
                char upperChar = char.ToUpper(c);
                char displacedUpper = DisplaceChar(upperChar, key, UpperCaseAlphabet, UpperCaseMap);
                outputStr.Append(displacedUpper);
            }
            else if (char.IsDigit(c))
            {
                outputStr.Append(c);
            }
            else if (!char.IsLetterOrDigit(c))
            {
                outputStr.Append(c);
            }
            else
            {
                throw new UtilException($"不支持的字符: '{c}'");
            }
        }

        return outputStr.ToString();
    }

    /// <summary>
    /// 使用位移算法解密字符串
    /// </summary>
    /// <param name="inputStr">要解密的字符串</param>
    /// <param name="key">位移量，默认为3</param>
    /// <returns>解密后的字符串</returns>
    /// <exception cref="ArgumentNullException">输入字符串为null时抛出</exception>
    /// <exception cref="ArgumentException">输入字符串为空时抛出</exception>
    /// <exception cref="UtilException">字符串包含不支持的字符时抛出</exception>
    public static string Decrypt(string inputStr, int key = 3) => Encrypt(inputStr, -key);

    /// <summary>
    /// 对单个字符进行位移处理
    /// </summary>
    /// <param name="c">要处理的字符</param>
    /// <param name="key">位移量</param>
    /// <param name="alphabet">字符集</param>
    /// <param name="charMap">字符映射表</param>
    /// <returns>位移后的字符</returns>
    private static char DisplaceChar(char c, int key, char[] alphabet, Dictionary<char, int> charMap)
    {
        if (!charMap.TryGetValue(c, out int index)) throw new UtilException($"不支持的字符: '{c}'");

        int displacedIndex = (index + key) % alphabet.Length;
        if (displacedIndex < 0)
        {
            displacedIndex += alphabet.Length;
        }
        return alphabet[displacedIndex];
    }
}
