namespace Materal.Utils.Extensions;

/// <summary>
/// 字符串转换扩展类
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// 将字符串解析为枚举值
    /// </summary>
    /// <typeparam name="T">目标枚举类型</typeparam>
    /// <param name="value">要解析的字符串</param>
    /// <param name="ignoreCase">是否忽略大小写，默认为true</param>
    /// <returns>解析后的枚举值</returns>
    /// <exception cref="ArgumentException">当value不是有效的枚举值时抛出</exception>
    public static T ConvertToEnum<T>(this string value, bool ignoreCase = true) where T : Enum => (T)Enum.Parse(typeof(T), value, ignoreCase);
    /// <summary>
    /// 将字符串首字母转换为小写
    /// </summary>
    /// <param name="inputString">输入字符串</param>
    /// <returns>首字母小写的字符串</returns>
    public static string ToLowerFirstLetter(this string inputString)
    {
        if (string.IsNullOrWhiteSpace(inputString)) return inputString;
        return inputString[0].ToString().ToLower() + inputString[1..];
    }
    /// <summary>
    /// 将字符串首字母转换为大写
    /// </summary>
    /// <param name="inputString">输入字符串</param>
    /// <returns>首字母大写的字符串</returns>
    public static string ToUpperFirstLetter(this string inputString)
    {
        if (string.IsNullOrWhiteSpace(inputString)) return inputString;
        return inputString[0].ToString().ToUpper() + inputString[1..];
    }
}
