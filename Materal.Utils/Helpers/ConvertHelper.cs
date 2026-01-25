namespace Materal.Utils.Helpers;

/// <summary>
/// 类型转换Helper类
/// </summary>
public static class ConvertHelper
{
    /// <summary>
    /// 构造方法
    /// </summary>
    static ConvertHelper()
    {
        AddDefaultConvertDictionary();
    }

    /// <summary>
    /// 可转换类型字典
    /// </summary>
    private static readonly Dictionary<Type, Func<object, object?>> _convertDictionary = [];

    /// <summary>
    /// 添加默认转换字典
    /// </summary>
    private static void AddDefaultConvertDictionary()
    {
        _convertDictionary.Add(typeof(bool), WrapValueConvert(Convert.ToBoolean));
        _convertDictionary.Add(typeof(bool?), WrapValueConvert(Convert.ToBoolean));
        _convertDictionary.Add(typeof(int), WrapValueConvert(Convert.ToInt32));
        _convertDictionary.Add(typeof(int?), WrapValueConvert(Convert.ToInt32));
        _convertDictionary.Add(typeof(long), WrapValueConvert(Convert.ToInt64));
        _convertDictionary.Add(typeof(long?), WrapValueConvert(Convert.ToInt64));
        _convertDictionary.Add(typeof(short), WrapValueConvert(Convert.ToInt16));
        _convertDictionary.Add(typeof(short?), WrapValueConvert(Convert.ToInt16));
        _convertDictionary.Add(typeof(double), WrapValueConvert(Convert.ToDouble));
        _convertDictionary.Add(typeof(double?), WrapValueConvert(Convert.ToDouble));
        _convertDictionary.Add(typeof(float), WrapValueConvert(Convert.ToSingle));
        _convertDictionary.Add(typeof(float?), WrapValueConvert(Convert.ToSingle));
        _convertDictionary.Add(typeof(Guid), m =>
        {
            string? inputString = m.ToString();
            if (string.IsNullOrWhiteSpace(inputString) || !inputString.IsGuid())
            {
                return Guid.Empty;
            }
            return Guid.Parse(inputString);
        });
        _convertDictionary.Add(typeof(Guid?), m =>
        {
            string? inputString = m.ToString();
            if (string.IsNullOrWhiteSpace(inputString) || !inputString.IsGuid())
            {
                return null;
            }
            return Guid.Parse(inputString);
        });
        _convertDictionary.Add(typeof(string), Convert.ToString);
        _convertDictionary.Add(typeof(decimal), WrapValueConvert(Convert.ToDecimal));
        _convertDictionary.Add(typeof(decimal?), WrapValueConvert(Convert.ToDecimal));
        _convertDictionary.Add(typeof(DateTime), WrapValueConvert(ConvertToDateTime));
        _convertDictionary.Add(typeof(DateTime?), WrapValueConvert(ConvertToDateTime));
    }

    /// <summary>
    /// 添加转换字典
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="func">转换函数</param>
    public static void AddConvertDictionary<T>(Func<object, T> func) => _convertDictionary.Add(typeof(T), WrapValueConvert(func));

    /// <summary>
    /// 转换为 DateTime
    /// </summary>
    /// <param name="obj">要转换的对象</param>
    /// <returns>转换后的 DateTime</returns>
    private static DateTime ConvertToDateTime(object obj)
    {
        string? inputString = obj.ToString();
        if (string.IsNullOrWhiteSpace(inputString))
        {
            throw new FormatException("无法将空字符串转换为 DateTime");
        }
        if (long.TryParse(inputString, out long timestamp))
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        }
        return Convert.ToDateTime(obj);
    }

    /// <summary>
    /// 写入值转换类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="input">输入函数</param>
    /// <returns>转换后的函数</returns>
    private static Func<object, object?> WrapValueConvert<T>(Func<object, T?> input) => i => input(i);

    /// <summary>
    /// 判断是否提供到特定类型的转换
    /// </summary>
    /// <param name="targetType">目标类型</param>
    /// <returns>是否可转换</returns>
    public static bool CanConvertTo(Type targetType) => _convertDictionary.ContainsKey(targetType);

    /// <summary>
    /// 转换到特定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="obj">要转换的对象</param>
    /// <returns>转换后的对象</returns>
    public static T? ConvertTo<T>(object obj) => (T?)ConvertTo(obj, typeof(T));

    /// <summary>
    /// 转换到特定类型
    /// </summary>
    /// <param name="obj">要转换的对象</param>
    /// <param name="targetType">目标类型</param>
    /// <returns>转换后的对象</returns>
    public static object? ConvertTo(object obj, Type targetType)
    {
        if (obj is null)
        {
            if (!targetType.IsValueType) return null;
            if (Nullable.GetUnderlyingType(targetType) != null) return null;
            throw new ArgumentNullException(nameof(obj), "不能将null转换为" + targetType.Name);
        }
        if (obj.IsNullOrWhiteSpaceString()) return obj is string stringObj && targetType == typeof(string) ? stringObj : null;
        if (obj.GetType() == targetType || targetType.IsInstanceOfType(obj)) return obj;
        if (_convertDictionary.TryGetValue(targetType, out Func<object, object?>? value)) return value(obj);
        try
        {
            return Convert.ChangeType(obj, targetType);
        }
        catch
        {
            throw new UtilException("未实现到" + targetType.Name + "的转换");
        }
    }
}
