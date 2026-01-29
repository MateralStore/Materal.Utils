namespace Materal.Utils.Models.Attributes;

/// <summary>
/// 开头包含过滤特性
/// 用于在动态查询中生成字符串开头匹配（StartsWith）条件的表达式树
/// </summary>
/// <remarks>
/// <para><b>功能说明：</b></para>
/// <para>
/// 该特性继承自 <see cref="FilterAttribute"/>，用于标记过滤模型（<see cref="FilterModel"/>）的属性。
/// 当调用 <see cref="FilterModel.GetSearchExpression{T}"/> 方法时，会自动为标记了此特性的属性生成字符串开头匹配查询条件。
/// </para>
/// <para><b>工作原理：</b></para>
/// <para>
/// 生成类似 <c>m => m.TargetProperty.StartsWith(filterValue)</c> 的表达式树，用于判断目标字符串是否以指定值开头。
/// </para>
/// <para><b>典型使用场景：</b></para>
/// <list type="bullet">
/// <item><description>前缀搜索：查找以特定前缀开头的编号、代码或名称</description></item>
/// <item><description>分类查询：根据分类代码前缀进行过滤</description></item>
/// <item><description>路径匹配：查找特定路径前缀的文件或目录</description></item>
/// <item><description>电话号码区号过滤：根据区号前缀查询电话号码</description></item>
/// </list>
/// <para><b>使用示例：</b></para>
/// <code>
/// public class ProductFilterModel : FilterModel
/// {
///     // 按产品编号前缀过滤
///     [StartContains]
///     public string? Code { get; set; }
///     
///     // 按电话区号过滤
///     [StartContains(targetPropertyName: "PhoneNumber")]
///     public string? AreaCode { get; set; }
/// }
/// 
/// var filter = new ProductFilterModel { Code = "PRD" };
/// // 生成表达式：m => m.Code.StartsWith("PRD")
/// var expression = filter.GetSearchExpression&lt;Product&gt;();
/// </code>
/// <para><b>注意事项：</b></para>
/// <list type="bullet">
/// <item><description>字符串比较是区分大小写的</description></item>
/// <item><description>如果过滤属性值为 null 或空字符串，该条件会被忽略</description></item>
/// <item><description>仅适用于字符串类型的属性</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class StartContainsAttribute(string? targetPropertyName = null) : FilterAttribute(targetPropertyName)
{
    /// <inheritdoc/>
    public override Expression? GetSearchExpression<T>(ParameterExpression parameterExpression, PropertyInfo propertyInfo, T value, PropertyInfo targetPropertyInfo)
    {
        if (value is null || value.IsNullOrEmptyString()) return null;
        Expression? result = GetCallExpression(parameterExpression, propertyInfo, value, "StartsWith", targetPropertyInfo);
        return result;
    }
}
