using System.Data;
using System.Text.RegularExpressions;

namespace Materal.Utils.Extensions;

/// <summary>
/// Object扩展
/// </summary>
public static partial class ObjectExtensions
{
    /// <summary>
    /// 对象是否为空或者空字符串
    /// </summary>
    /// <param name="inputObj">输入对象</param>
    /// <returns>是否为空或空字符串</returns>
    public static bool IsNullOrEmptyString(this object inputObj) => inputObj switch
    {
        null => true,
        string inputStr => string.IsNullOrEmpty(inputStr),
        _ => false,
    };

    /// <summary>
    /// 对象是否为空或者空或者空格字符串
    /// </summary>
    /// <param name="inputObj">输入对象</param>
    /// <returns>是否为空或空白字符串</returns>
    public static bool IsNullOrWhiteSpaceString(this object inputObj) => inputObj switch
    {
        null => true,
        string inputStr => string.IsNullOrWhiteSpace(inputStr),
        _ => false,
    };
    /// <summary>
    /// 对象是否相等
    /// </summary>
    /// <param name="aModel">对象A</param>
    /// <param name="bModel">对象B</param>
    /// <param name="maps">属性映射规则</param>
    /// <returns>是否相等</returns>
    public static bool Equals(this object aModel, object bModel, Dictionary<string, Func<object?, bool>> maps)
    {
        Type aType = aModel.GetType();
        Type bType = bModel.GetType();
        foreach (PropertyInfo aProperty in aType.GetProperties())
        {
            object? aValue = aProperty.GetValue(aModel);
            if (maps.TryGetValue(aProperty.Name, out Func<object?, bool>? value))
            {
                bool mapResult = value.Invoke(aValue);
                if (!mapResult) return false;
            }
            else
            {
                PropertyInfo? bProperty = bType.GetProperty(aProperty.Name);
                if (bProperty is null || aProperty.PropertyType != bProperty.PropertyType) return false;
                object? bValue = bProperty.GetValue(bModel);
                if (aValue is null && bValue is null) continue;
                if (aValue is null || bValue is null) return false;
                if (!aValue.Equals(bValue)) return false;
            }
        }
        return true;
    }

#if NET
    /// <summary>
    /// 用于匹配数组索引的模版表达式（如[0], [1], [100]等）
    /// </summary>
    /// <returns>匹配数组索引的正则表达式</returns>
    /// <remarks>
    /// 在.NET环境中使用源生成器生成正则表达式，性能更优
    /// </remarks>
    [GeneratedRegex(@"\[\d+\]")]
    private static partial Regex ExpressionRegex();
#endif

    /// <summary>
    /// 获得值
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="obj">对象</param>
    /// <param name="name">属性名称或路径</param>
    /// <returns>属性值，未找到或类型不匹配时返回null</returns>
    /// <example>
    /// <code>
    /// var user = new User { Name = "张三", Address = new Address { City = "北京" } };
    /// 
    /// // 获取简单属性
    /// string name = user.GetObjectValue&lt;string&gt;("Name");  // 返回 "张三"
    /// 
    /// // 获取嵌套属性
    /// string city = user.GetObjectValue&lt;string&gt;("Address.City");  // 返回 "北京"
    /// 
    /// // 获取数组元素
    /// var list = new List&lt;string&gt; { "A", "B", "C" };
    /// string firstItem = list.GetObjectValue&lt;string&gt;("[0]");  // 返回 "A"
    /// 
    /// // 获取字典值
    /// var dict = new Dictionary&lt;string, object&gt; { { "Key1", "Value1" } };
    /// object value = dict.GetObjectValue("Key1");  // 返回 "Value1"
    /// </code>
    /// </example>
    public static T? GetObjectValue<T>(this object obj, string name)
    {
        object? resultObj = GetObjectValue(obj, name);
        if (resultObj is null || resultObj is not T result) return default;
        return result;
    }

    /// <summary>
    /// 获得值
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="obj">对象</param>
    /// <param name="names">属性名称数组</param>
    /// <returns>属性值，未找到或类型不匹配时返回null</returns>
    /// <example>
    /// <code>
    /// var obj = new { Level1 = new { Level2 = new { Level3 = "Value" } } };
    /// string value = obj.GetObjectValue&lt;string&gt;("Level1", "Level2", "Level3");  // 返回 "Value"
    /// 
    /// // 等价于：
    /// string value2 = obj.GetObjectValue&lt;string&gt;("Level1.Level2.Level3");
    /// </code>
    /// </example>
    public static T? GetObjectValue<T>(this object obj, params string[] names)
    {
        object? resultObj = GetObjectValue(obj, names);
        if (resultObj is null || resultObj is not T result) return default;
        return result;
    }

    /// <summary>
    /// 获得值
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="name">属性名称或路径</param>
    /// <returns>属性值，未找到时返回null</returns>
    /// <example>
    /// <code>
    /// var user = new User { Name = "张三", Age = 25 };
    /// var name = user.GetObjectValue("Name");  // 返回 "张三"
    /// var age = user.GetObjectValue&lt;int&gt;("Age");  // 返回 25
    /// </code>
    /// </example>
    public static object? GetObjectValue(this object obj, string name)
    {
        string[] trueNames = name.Split('.');
        if (trueNames.Length == 1)
        {
#if NET
            Regex regex = ExpressionRegex();
#else
            Regex regex = new(@"\[\d+\]");
#endif
            string trueName = name;
            MatchCollection matchCollection = regex.Matches(trueName);
            if (matchCollection.Count > 0)
            {
                List<string> tempNames = [];
                foreach (object? matchItem in matchCollection)
                {
                    if (matchItem is not Match match) continue;
                    tempNames.Add(match.Value[1..^1]);
                    trueName = trueName.Replace(match.Value, string.Empty);
                }
                if (!string.IsNullOrWhiteSpace(trueName))
                {
                    tempNames.Insert(0, trueName);
                }
                return obj.GetObjectValue(tempNames);
            }
            if (obj is IDictionary<string, object> dicObj) return dicObj.GetObjectDictionaryValue(trueName);
            if (obj is IDictionary dic) return dic.GetObjectValue(trueName);
            if (obj is IList list) return list.GetObjectValue(trueName);
            if (obj is ICollection collection) return collection.GetObjectValue(trueName);
            if (obj is DataTable dt) return dt.GetObjectValue(trueName);
            if (obj is DataRow dr) return dr.GetObjectValue(trueName);
            PropertyInfo? propertyInfo = obj.GetType().GetRuntimeProperty(trueName);
            if (propertyInfo is not null && propertyInfo.CanRead)
            {
                return propertyInfo.GetValue(obj);
            }
            FieldInfo? fieldInfo = obj.GetType().GetRuntimeField(trueName);
            if (fieldInfo is not null)
            {
                return fieldInfo.GetValue(obj);
            }
            return null;
        }
        else
        {
            return obj.GetObjectValue(trueNames);
        }
    }

    /// <summary>
    /// 获得值
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="names">属性名称集合</param>
    /// <returns>属性值</returns>
    public static object? GetObjectValue(this object obj, ICollection<string> names)
    {
        object? currentObj = obj;
        foreach (string name in names)
        {
            currentObj = currentObj?.GetObjectValue(name);
            if (currentObj is null) break;
        }
        return currentObj;
    }

    /// <summary>
    /// 获得值
    /// </summary>
    /// <param name="dt">数据表</param>
    /// <param name="name">行索引</param>
    /// <returns>数据行</returns>
    private static DataRow? GetObjectValue(this DataTable dt, string name)
        => int.TryParse(name, out int targetIndex) && targetIndex >= 0 && targetIndex < dt.Rows.Count ? dt.Rows[targetIndex] : null;

    /// <summary>
    /// 获得值
    /// </summary>
    /// <param name="dr">数据行</param>
    /// <param name="name">列名或列索引</param>
    /// <returns>单元格值</returns>
    private static object? GetObjectValue(this DataRow dr, string name)
    {
        if (dr.Table.Columns.Contains(name))
        {
            return dr[name];
        }
        else if (int.TryParse(name, out int targetIndex) && targetIndex >= 0 && targetIndex < dr.ItemArray?.Length)
        {
            return dr[targetIndex];
        }
        return null;
    }

    /// <summary>
    /// 获得值
    /// </summary>
    /// <param name="list">列表</param>
    /// <param name="name">索引</param>
    /// <returns>元素</returns>
    private static object? GetObjectValue(this IList list, string name)
    {
        if (!int.TryParse(name, out int targetIndex)) return null;
        return targetIndex >= 0 && targetIndex < list.Count ? list[targetIndex] : null;
    }

    /// <summary>
    /// 获得值
    /// </summary>
    /// <param name="collection">集合</param>
    /// <param name="name">索引</param>
    /// <returns>元素</returns>
    private static object? GetObjectValue(this ICollection collection, string name)
    {
        if (!int.TryParse(name, out int targetIndex)) return null;
        if (targetIndex >= 0 && targetIndex < collection.Count)
        {
            int index = 0;
            foreach (object? item in collection)
            {
                if (index++ != targetIndex) continue;
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 获得值
    /// </summary>
    /// <param name="dic">字典</param>
    /// <param name="name">键</param>
    /// <returns>值</returns>
    private static object? GetObjectValue(this IDictionary dic, string name)
    {
        foreach (object? item in dic.Keys)
        {
            if (item is not null && item.Equals(name)) return dic[item];
        }
        return null;
    }

    /// <summary>
    /// 获得值
    /// </summary>
    /// <param name="dic">字典</param>
    /// <param name="name">键</param>
    /// <returns>值</returns>
    private static object? GetObjectDictionaryValue(this IDictionary<string, object> dic, string name) => dic.TryGetValue(name, out object? value) ? value : null;
}
