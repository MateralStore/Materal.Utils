namespace Materal.Utils;

/// <summary>
/// Json扩展
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Json字符串转换对象
    /// </summary>
    /// <typeparam name="T">接口类型</typeparam>
    /// <param name="jsonStr"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    /// <exception cref="ExtensionException"></exception>
    public static T JsonToInterface<T>(this string jsonStr, string typeName)
    {
        Type triggerDataType = TypeHelper.GetTypeByTypeName<T>(typeName) ?? throw new ExtensionException("转换失败");
        return (T)jsonStr.JsonToObject(triggerDataType);
    }
}
