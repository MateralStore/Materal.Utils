using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// 栅栏加密工具类
/// 提供字符串的栅栏加密和解密功能
/// </summary>
/// <remarks>
/// 栅栏加密是一种置换加密算法，通过将字符串按行排列并按列读取来实现加密。
/// 时间复杂度：O(n)，其中n为字符串长度
/// </remarks>
/// <example>
/// <code>
/// // 基础2栏加密
/// string encoded = FenceCrypto.ToFenceEncode("HELLO WORLD");
/// string decoded = FenceCrypto.FenceDecode(encoded);
/// 
/// // 多栏加密
/// string encoded3 = FenceCrypto.ToFenceEncode("HELLO WORLD", 3);
/// string decoded3 = FenceCrypto.FenceDecode(encoded3, 3);
/// </code>
/// </example>
public static partial class FenceCrypto
{
    /// <summary>
    /// 默认栅栏数量
    /// </summary>
    public const int DefaultRails = 2;
    /// <summary>
    /// 栅栏加密（使用默认2栏）
    /// </summary>
    /// <param name="inputStr">输入字符串</param>
    /// <returns>加密后字符串</returns>
    /// <exception cref="ArgumentNullException">当输入字符串为null时抛出</exception>
    /// <exception cref="ArgumentException">当输入字符串为空时抛出</exception>
    public static string Encrypt(string inputStr) => Encrypt(inputStr, DefaultRails);

    /// <summary>
    /// 栅栏加密（指定栏数）
    /// </summary>
    /// <param name="inputStr">输入字符串</param>
    /// <param name="rails">栅栏栏数，必须大于1且小于等于字符串长度</param>
    /// <returns>加密后字符串</returns>
    /// <exception cref="ArgumentNullException">当输入字符串为null时抛出</exception>
    /// <exception cref="ArgumentException">当输入字符串为空或栏数无效时抛出</exception>
    public static string Encrypt(string inputStr, int rails)
    {
        ValidateInput(inputStr, rails);
        
        if (rails == 2)
        {
            return EncodeWithTwoRails(inputStr);
        }
        
        return EncodeWithMultipleRails(inputStr, rails);
    }
    /// <summary>
    /// 栅栏解密（使用默认2栏）
    /// </summary>
    /// <param name="inputStr">输入字符串</param>
    /// <returns>解密后字符串</returns>
    /// <exception cref="ArgumentNullException">当输入字符串为null时抛出</exception>
    /// <exception cref="ArgumentException">当输入字符串为空时抛出</exception>
    public static string Decode(string inputStr) => Decode(inputStr, DefaultRails);

    /// <summary>
    /// 栅栏解密（指定栏数）
    /// </summary>
    /// <param name="inputStr">输入字符串</param>
    /// <param name="rails">栅栏栏数，必须大于1且小于等于字符串长度</param>
    /// <returns>解密后字符串</returns>
    /// <exception cref="ArgumentNullException">当输入字符串为null时抛出</exception>
    /// <exception cref="ArgumentException">当输入字符串为空或栏数无效时抛出</exception>
    public static string Decode(string inputStr, int rails)
    {
        ValidateInput(inputStr, rails);
            
        if (rails == 2)
        {
            return DecodeWithTwoRails(inputStr);
        }
        
        return DecodeWithMultipleRails(inputStr, rails);
    }
    
    #region 私有辅助方法
    
    /// <summary>
    /// 验证输入参数
    /// </summary>
    private static void ValidateInput(string inputStr, int rails)
    {
        if (inputStr == null)
            throw new ArgumentNullException(nameof(inputStr), "输入字符串不能为null");
        if (string.IsNullOrEmpty(inputStr))
            throw new ArgumentException("输入字符串不能为空", nameof(inputStr));
        if (rails <= 1)
            throw new ArgumentException("栅栏栏数必须大于1", nameof(rails));
        if (rails > inputStr.Length)
            throw new ArgumentException("栅栏栏数不能大于字符串长度", nameof(rails));
    }
    
    /// <summary>
    /// 计算下一个栅栏位置
    /// </summary>
    private static (int nextRail, bool nextGoingDown) GetNextRailPosition(int currentRail, int rails, bool goingDown)
    {
        if (currentRail == 0)
            goingDown = true;
        else if (currentRail == rails - 1)
            goingDown = false;
            
        return (currentRail + (goingDown ? 1 : -1), goingDown);
    }
    
    /// <summary>
    /// 使用2栏进行加密（优化版本）
    /// </summary>
    private static string EncodeWithTwoRails(string inputStr)
    {
        int count = inputStr.Length;
        StringBuilder evenChars = new(count / 2 + 1);
        StringBuilder oddChars = new(count / 2);
        
        for (int i = 0; i < count; i++)
        {
            if (i % 2 == 0)
            {
                evenChars.Append(inputStr[i]);
            }
            else
            {
                oddChars.Append(inputStr[i]);
            }
        }
        
        return evenChars.ToString() + oddChars.ToString();
    }
    
    /// <summary>
    /// 使用2栏进行解密（优化版本）
    /// </summary>
    private static string DecodeWithTwoRails(string inputStr)
    {
        int count = inputStr.Length;
        StringBuilder result = new(count);
        
        string evenPart, oddPart;
        if (count % 2 == 0)
        {
            evenPart = inputStr[..(count / 2)];
            oddPart = inputStr[(count / 2)..];
        }
        else
        {
            evenPart = inputStr[..((count / 2) + 1)];
            oddPart = inputStr[((count / 2) + 1)..];
        }
        
        int evenIndex = 0, oddIndex = 0;
        for (int i = 0; i < count; i++)
        {
            if (i % 2 == 0)
            {
                result.Append(evenPart[evenIndex++]);
            }
            else
            {
                result.Append(oddPart[oddIndex++]);
            }
        }
        
        return result.ToString();
    }
    
    /// <summary>
    /// 使用多栏进行加密
    /// </summary>
    private static string EncodeWithMultipleRails(string inputStr, int rails)
    {
        StringBuilder[] fence = new StringBuilder[rails];
        for (int i = 0; i < rails; i++)
        {
            fence[i] = new StringBuilder();
        }
        
        int currentRail = 0;
        bool goingDown = true;
        
        foreach (char c in inputStr)
        {
            fence[currentRail].Append(c);
            (currentRail, goingDown) = GetNextRailPosition(currentRail, rails, goingDown);
        }

        StringBuilder result = new(inputStr.Length);
        foreach (StringBuilder rail in fence)
        {
            result.Append(rail);
        }
        
        return result.ToString();
    }
    
    /// <summary>
    /// 使用多栏进行解密
    /// </summary>
    private static string DecodeWithMultipleRails(string inputStr, int rails)
    {
        int length = inputStr.Length;
        int[] railLengths = new int[rails];
        string[] fence = new string[rails];
        
        // 计算每栏的字符数量
        int currentRail = 0;
        bool goingDown = true;
        for (int i = 0; i < length; i++)
        {
            railLengths[currentRail]++;
            (currentRail, goingDown) = GetNextRailPosition(currentRail, rails, goingDown);
        }
        
        // 分割字符串到各栏
        int startIndex = 0;
        for (int i = 0; i < rails; i++)
        {
            fence[i] = inputStr.Substring(startIndex, railLengths[i]);
            startIndex += railLengths[i];
        }

        // 重新构建原始字符串
        StringBuilder result = new(length);
        int[] railIndices = new int[rails];
        currentRail = 0;
        goingDown = true;
        
        for (int i = 0; i < length; i++)
        {
            result.Append(fence[currentRail][railIndices[currentRail]++]);
            (currentRail, goingDown) = GetNextRailPosition(currentRail, rails, goingDown);
        }
        
        return result.ToString();
    }
    #endregion
}
