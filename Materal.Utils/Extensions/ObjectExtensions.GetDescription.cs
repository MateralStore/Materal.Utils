namespace Materal.Utils.Extensions;

/// <summary>
/// Object扩展
/// </summary>
public static partial class ObjectExtensions
{
    /// <summary>
    /// 描述字段名称
    /// </summary>
    private const string DescriptionMemberName = "Description";

    /// <summary>
    /// 获得描述
    /// </summary>
    /// <param name="inputObj">对象</param>
    /// <returns>描述</returns>
    public static string GetDescription(this object inputObj)
    {
        if (inputObj is Enum @enum) return GetDescription(inputObj, @enum.ToString());
        Type objType = inputObj.GetType();
        DescriptionAttribute? attribute = objType.GetCustomAttribute<DescriptionAttribute>();
        if (attribute is not null) return attribute.Description;
        const string descriptionName = DescriptionMemberName;
        return GetDescription(inputObj, descriptionName);
    }

    /// <summary>
    /// 获得描述（或返回null）
    /// </summary>
    /// <param name="inputObj">对象</param>
    /// <returns>描述，未找到则返回null</returns>
    public static string? GetDescriptionOrNull(this object inputObj)
    {
        try
        {
            return GetDescription(inputObj);
        }
        catch (UtilException)
        {
            return null;
        }
    }

    /// <summary>
    /// 获得描述
    /// </summary>
    /// <param name="inputObj">对象</param>
    /// <param name="memberInfo">成员信息</param>
    /// <returns>描述</returns>
    public static string GetDescription(this object inputObj, MemberInfo memberInfo)
    {
        DescriptionAttribute? attribute = memberInfo.GetCustomAttribute<DescriptionAttribute>();
        if (attribute is not null) return attribute.Description;
        if (inputObj is Enum @enum) return @enum.ToString();
        if (memberInfo.Name == DescriptionMemberName)
        {
            object? value = memberInfo.GetValue(inputObj);
            if (value is not null && value is string descriptionValue)
            {
                return descriptionValue;
            }
        }
        throw new UtilException($"未找到特性{nameof(DescriptionAttribute)}");
    }

    /// <summary>
    /// 获得描述（或返回null）
    /// </summary>
    /// <param name="inputObj">对象</param>
    /// <param name="memberInfo">成员信息</param>
    /// <returns>描述，未找到则返回null</returns>
    public static string? GetDescriptionOrNull(this object inputObj, MemberInfo memberInfo)
    {
        try
        {
            return GetDescription(inputObj, memberInfo);
        }
        catch (UtilException)
        {
            return null;
        }
    }

    /// <summary>
    /// 获得描述
    /// </summary>
    /// <param name="inputObj">对象</param>
    /// <param name="memberName">成员名称</param>
    /// <returns>描述</returns>
    public static string GetDescription(this object inputObj, string memberName)
    {
        Type objType = inputObj.GetType();
        MemberInfo? memberInfo = objType.GetRuntimeField(memberName);
        memberInfo ??= objType.GetRuntimeProperty(memberName);
        if (memberInfo is null) throw new UtilException($"未找到字段或属性{memberName}");
        return GetDescription(inputObj, memberInfo);
    }

    /// <summary>
    /// 获得描述（或返回null）
    /// </summary>
    /// <param name="inputObj">对象</param>
    /// <param name="memberName">成员名称</param>
    /// <returns>描述，未找到则返回null</returns>
    public static string? GetDescriptionOrNull(this object inputObj, string memberName)
    {
        try
        {
            return GetDescription(inputObj, memberName);
        }
        catch (UtilException)
        {
            return null;
        }
    }
}
