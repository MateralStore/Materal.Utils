using System.Xml.Serialization;

namespace Materal.Utils.Helpers;

/// <summary>
/// 对象克隆Helper类
/// </summary>
/// <remarks>
/// <para>性能对比（从快到慢）：</para>
/// <para>1. CloneByJson - 最快，使用JSON序列化，推荐用于大多数场景</para>
/// <para>2. CloneByReflex - 慢于JSON，但不需要序列化特性，支持深度克隆</para>
/// <para>3. CloneByXml - 慢于Reflex，需要[Serializable]特性</para>
/// <para>4. CloneBySerializable - 最慢，仅限NETSTANDARD框架</para>
/// <para>建议：优先使用CloneByJson，性能最佳且支持度广</para>
/// </remarks>
public static class CloneHelper
{
    /// <summary>
    /// 通过JSON序列化克隆对象
    /// 将对象序列化为JSON字符串，然后再反序列化为新对象
    /// </summary>
    /// <typeparam name="T">要克隆的对象类型，必须是非空类型</typeparam>
    /// <param name="inputObj">要克隆的输入对象</param>
    /// <returns>返回克隆后的新对象</returns>
    /// <remarks>
    /// 适用于支持JSON序列化的对象
    /// </remarks>
    public static T CloneByJson<T>(T inputObj)
        where T : notnull
    {
        string jsonStr = inputObj.ToJson();
        return jsonStr.JsonToObject<T>();
    }
    /// <summary>
    /// 通过XML序列化克隆对象
    /// 将对象序列化为XML格式，然后再反序列化为新对象
    /// </summary>
    /// <typeparam name="T">要克隆的对象类型，必须是非空类型</typeparam>
    /// <param name="inputObj">要克隆的输入对象</param>
    /// <returns>返回克隆后的新对象，如果克隆失败则返回默认值</returns>
    /// <remarks>
    /// 该方法使用XmlSerializer进行序列化和反序列化
    /// 适用于支持XML序列化的对象
    /// </remarks>
    public static T? CloneByXml<T>(T inputObj)
        where T : notnull
    {
        object? result;
        using (MemoryStream ms = new())
        {
            XmlSerializer xml = new(typeof(T));
            xml.Serialize(ms, inputObj);
            ms.Seek(0, SeekOrigin.Begin);
            result = xml.Deserialize(ms);
            ms.Close();
        }
        return result is null ? default : (T)result;
    }
    /// <summary>
    /// 通过反射克隆对象
    /// 使用反射获取对象的所有属性，并逐个复制属性值到新对象
    /// </summary>
    /// <typeparam name="T">要克隆的对象类型，必须是非空类型</typeparam>
    /// <param name="inputObj">要克隆的输入对象</param>
    /// <returns>返回克隆后的新对象，如果克隆失败则返回默认值</returns>
    /// <remarks>
    /// 该方法使用反射机制获取和设置属性值
    /// 对于引用类型属性，会递归调用CloneByReflex方法进行深度克隆
    /// </remarks>
    public static T? CloneByReflex<T>(T inputObj)
        where T : notnull
    {
        Type tType = inputObj.GetType();
        T resM = tType.Instantiation<T>();
        PropertyInfo[] propertyInfos = tType.GetProperties();
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            object? value = propertyInfo.GetValue(inputObj);
            if (value is null) continue;

            // 对于值类型和字符串类型，直接复制
            if (value is ValueType || value is string)
            {
                propertyInfo.SetValue(resM, value);
            }
            // 对于实现了 IEnumerable 的集合类型（但不包括字符串），使用 JSON 方式克隆
            else if (value is IEnumerable && value is not string)
            {
                // 使用 JSON 序列化，并指定正确的目标类型
                string jsonStr = value.ToJson();
                object? clonedValue = jsonStr.JsonToObject(propertyInfo.PropertyType);
                if (clonedValue is not null)
                {
                    propertyInfo.SetValue(resM, clonedValue);
                }
            }
            // 对于其他引用类型，递归调用 CloneByReflex
            else
            {
                try
                {
                    // 尝试使用反射递归克隆
                    MethodInfo? cloneMethod = typeof(CloneHelper).GetMethod(nameof(CloneByReflex), [])?.MakeGenericMethod(value.GetType());
                    if (cloneMethod is not null)
                    {
                        object? clonedValue = cloneMethod.Invoke(null, [value]);
                        if (clonedValue is not null)
                        {
                            propertyInfo.SetValue(resM, clonedValue);
                        }
                    }
                    else
                    {
                        // 如果无法获取方法，使用 JSON 方式克隆
                        string jsonStr = value.ToJson();
                        object? clonedValue = jsonStr.JsonToObject(propertyInfo.PropertyType);
                        if (clonedValue is not null)
                        {
                            propertyInfo.SetValue(resM, clonedValue);
                        }
                    }
                }
                catch
                {
                    // 如果反射克隆失败，使用 JSON 方式克隆
                    string jsonStr = value.ToJson();
                    object? clonedValue = jsonStr.JsonToObject(propertyInfo.PropertyType);
                    if (clonedValue is not null)
                    {
                        propertyInfo.SetValue(resM, clonedValue);
                    }
                }
            }
        }
        return resM;
    }
#if NETSTANDARD
    /// <summary>
    /// 通过二进制序列化克隆对象
    /// 使用BinaryFormatter将对象序列化为二进制流，然后再反序列化为新对象
    /// </summary>
    /// <typeparam name="T">要克隆的对象类型，必须是非空类型</typeparam>
    /// <param name="inputObj">要克隆的输入对象</param>
    /// <returns>返回克隆后的新对象，如果克隆失败则返回默认值</returns>
    /// <exception cref="UtilException">当对象未标记为可序列化时抛出</exception>
    /// <remarks>
    /// 该方法要求对象必须标记[Serializable]特性
    /// 使用BinaryFormatter进行二进制序列化和反序列化
    /// </remarks>
    public static T? CloneBySerializable<T>(T inputObj)
        where T : notnull
    {
        if (!inputObj.GetType().HasCustomAttribute<SerializableAttribute>()) throw new UtilException("未标识为可序列化");
        MemoryStream stream = new();
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new();
        formatter.Serialize(stream, inputObj);
        stream.Position = 0;
        object obj = formatter.Deserialize(stream);
        if (obj is T result) return result;
        return default;
    }
    /// <summary>
    /// 克隆对象
    /// 根据对象是否标记为可序列化自动选择克隆方式
    /// </summary>
    /// <typeparam name="T">要克隆的对象类型，必须是非空类型</typeparam>
    /// <param name="inputObj">要克隆的输入对象</param>
    /// <returns>返回克隆后的新对象，如果克隆失败则返回默认值</returns>
    /// <remarks>
    /// 该方法会检查对象是否标记了[Serializable]特性：
    /// - 如果标记了，则使用CloneBySerializable方法进行克隆
    /// - 如果未标记，则使用CloneByJson方法进行克隆
    /// </remarks>
    public static T? Clone<T>(T inputObj)
        where T : notnull
    {
        SerializableAttribute? serializableAttribute = inputObj.GetType().GetCustomAttribute<SerializableAttribute>();
        return serializableAttribute is not null ? CloneBySerializable(inputObj) : CloneByJson(inputObj);
    }
#else
    /// <summary>
    /// 克隆对象
    /// 使用JSON序列化方式进行克隆
    /// </summary>
    /// <typeparam name="T">要克隆的对象类型，必须是非空类型</typeparam>
    /// <param name="inputObj">要克隆的输入对象</param>
    /// <returns>返回克隆后的新对象，如果克隆失败则返回默认值</returns>
    /// <remarks>
    /// 在非NETSTANDARD环境下，直接使用JSON序列化方式进行克隆
    /// </remarks>
    public static T? Clone<T>(T inputObj)
        where T : notnull => CloneByJson(inputObj);
#endif
    /// <summary>
    /// 属性复制source->target
    /// </summary>
    /// <typeparam name="T">复制的模型类型</typeparam>
    /// <param name="source">复制源头对象</param>
    /// <param name="target">复制目标对象</param>
    /// <param name="isCopy">属性过滤函数，返回true表示复制，false表示不复制</param>
    /// <remarks>
    /// <para>该方法使用IL Emit动态生成委托进行属性复制</para>
    /// <para>首次调用时会编译IL代码并缓存委托，后续调用使用缓存的委托，性能优异</para>
    /// <para>委托缓存在ConcurrentDictionary中，支持多线程并发访问</para>
    /// </remarks>
    public static void CopyProperties<T>(object source, T target, Func<string, bool>? isCopy) => CopyPropertiesHelper.CopyProperties<T>(source, target, isCopy);

    /// <summary>
    /// 属性复制并返回新对象
    /// </summary>
    /// <typeparam name="T">复制的模型类型</typeparam>
    /// <param name="source">复制源头对象</param>
    /// <param name="isCopy">属性过滤函数，返回true表示复制，false表示不复制</param>
    /// <returns>复制的新对象</returns>
    public static T CopyProperties<T>(object source, Func<string, bool>? isCopy) => CopyPropertiesHelper.CopyProperties<T>(source, isCopy);

    /// <summary>
    /// 属性复制source->target（排除指定属性）
    /// </summary>
    /// <typeparam name="T">复制的模型类型</typeparam>
    /// <param name="source">复制源头对象</param>
    /// <param name="target">复制目标对象</param>
    /// <param name="notCopyPropertyNames">不复制的属性名称数组</param>
    public static void CopyProperties<T>(object source, T target, params string[] notCopyPropertyNames) => CopyPropertiesHelper.CopyProperties(source, target, notCopyPropertyNames);

    /// <summary>
    /// 属性复制并返回新对象（排除指定属性）
    /// </summary>
    /// <typeparam name="T">复制的模型类型</typeparam>
    /// <param name="source">复制源头对象</param>
    /// <param name="notCopyPropertyNames">不复制的属性名称数组</param>
    /// <returns>复制的新对象</returns>
    public static T CopyProperties<T>(object source, params string[] notCopyPropertyNames) => CopyPropertiesHelper.CopyProperties<T>(source, notCopyPropertyNames);
}
