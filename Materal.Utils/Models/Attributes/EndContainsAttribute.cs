namespace Materal.Utils.Models.Attributes;

/// <summary>
/// 结尾包含过滤特性
/// 用于在动态查询中生成字符串结尾匹配（EndsWith）条件的表达式树
/// </summary>
/// <remarks>
/// <para><b>功能说明：</b></para>
/// <para>
/// 该特性继承自 <see cref="FilterAttribute"/>，用于标记过滤模型（<see cref="FilterModel"/>）的属性。
/// 当调用 <see cref="FilterModel.GetSearchExpression{T}"/> 方法时，会自动为标记了此特性的属性生成字符串结尾匹配查询条件。
/// </para>
/// <para><b>工作原理：</b></para>
/// <para>
/// 生成类似 <c>m => m.TargetProperty.EndsWith(filterValue)</c> 的表达式树，用于判断目标字符串是否以指定值结尾。
/// </para>
/// <para><b>典型使用场景：</b></para>
/// <list type="bullet">
/// <item><description>文件扩展名过滤：查找特定扩展名的文件（如 .jpg、.pdf）</description></item>
/// <item><description>域名后缀匹配：查找特定域名后缀的邮箱或网址</description></item>
/// <item><description>编号后缀查询：根据编号后缀进行分类查询</description></item>
/// </list>
/// <para><b>使用示例：</b></para>
/// <code>
/// public class FileFilterModel : FilterModel
/// {
///     // 按文件扩展名过滤
///     [EndContains]
///     public string? Extension { get; set; }
///     
///     // 按邮箱域名过滤
///     [EndContains(targetPropertyName: "Email")]
///     public string? Domain { get; set; }
/// }
/// 
/// var filter = new FileFilterModel { Extension = ".jpg" };
/// // 生成表达式：m => m.Extension.EndsWith(".jpg")
/// var expression = filter.GetSearchExpression&lt;FileInfo&gt;();
/// </code>
/// <para><b>注意事项：</b></para>
/// <list type="bullet">
/// <item><description>字符串比较是区分大小写的</description></item>
/// <item><description>如果过滤属性值为 null 或空字符串，该条件会被忽略</description></item>
/// <item><description>仅适用于字符串类型的属性</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class EndContainsAttribute(string? targetPropertyName = null) : FilterAttribute(targetPropertyName)
{
    /// <inheritdoc/>
    public override Expression? GetSearchExpression<T>(ParameterExpression parameterExpression, PropertyInfo propertyInfo, T value, PropertyInfo targetPropertyInfo)
    {
        if (value is null || value.IsNullOrEmptyString()) return null;
        Expression? result = GetCallExpression(parameterExpression, propertyInfo, value, "EndsWith", targetPropertyInfo);
        return result;
    }
}
