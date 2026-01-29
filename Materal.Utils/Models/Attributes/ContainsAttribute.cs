namespace Materal.Utils.Models.Attributes;

/// <summary>
/// 包含过滤特性
/// 用于在动态查询中生成字符串包含（Contains）条件的表达式树
/// </summary>
/// <remarks>
/// <para><b>功能说明：</b></para>
/// <para>
/// 该特性继承自 <see cref="FilterAttribute"/>，用于标记过滤模型（<see cref="FilterModel"/>）的属性。
/// 当调用 <see cref="FilterModel.GetSearchExpression{T}"/> 方法时，会自动为标记了此特性的属性生成字符串包含查询条件。
/// </para>
/// <para><b>工作原理：</b></para>
/// <list type="number">
/// <item>
/// <description>在 <see cref="FilterModel.GetSearchExpression{T}"/> 中，通过反射扫描过滤模型的所有属性</description>
/// </item>
/// <item>
/// <description>对于标记了 <see cref="ContainsAttribute"/> 的属性，调用其 <see cref="GetSearchExpression{T}"/> 方法</description>
/// </item>
/// <item>
/// <description>生成类似 <c>m => m.TargetProperty.Contains(filterValue)</c> 的表达式树</description>
/// </item>
/// <item>
/// <description>如果属性值为 null 或空字符串，则不生成查询条件</description>
/// </item>
/// <item>
/// <description>多个条件会通过 <c>Expression.AndAlso</c> 组合成 AND 关系</description>
/// </item>
/// </list>
/// <para><b>构造函数参数：</b></para>
/// <list type="bullet">
/// <item>
/// <description><c>targetPropertyName</c>：目标实体的属性名称。如果为 null，则使用过滤模型中标记该特性的属性名称</description>
/// </item>
/// </list>
/// <para><b>典型使用场景：</b></para>
/// <list type="bullet">
/// <item><description>模糊搜索：根据用户输入的关键字在名称、描述等字段中进行模糊匹配</description></item>
/// <item><description>多字段搜索：在多个字符串字段中同时进行包含查询</description></item>
/// <item><description>动态查询构建：配合 EF Core 或 LINQ to Objects 进行动态过滤</description></item>
/// </list>
/// <para><b>支持的数据类型：</b></para>
/// <list type="bullet">
/// <item><description>string：标准字符串类型</description></item>
/// <item><description>string?：可空字符串类型</description></item>
/// <item><description>集合类型：如果目标属性是集合，会使用 <see cref="Enumerable.Contains{TSource}(IEnumerable{TSource}, TSource)"/> 方法</description></item>
/// </list>
/// <para><b>使用示例：</b></para>
/// <code>
/// // 定义过滤模型
/// public class UserFilterModel : FilterModel
/// {
///     // 按用户名模糊搜索
///     [Contains]
///     public string? Name { get; set; }
///     
///     // 按邮箱模糊搜索
///     [Contains]
///     public string? Email { get; set; }
///     
///     // 指定目标属性名称（过滤模型属性名与实体属性名不同时）
///     [Contains(targetPropertyName: "Description")]
///     public string? Desc { get; set; }
/// }
/// 
/// // 使用过滤模型
/// var filter = new UserFilterModel 
/// { 
///     Name = "张三",
///     Email = "example.com"
/// };
/// 
/// // 生成表达式：m => m.Name.Contains("张三") &amp;&amp; m.Email.Contains("example.com")
/// Expression&lt;Func&lt;User, bool&gt;&gt; expression = filter.GetSearchExpression&lt;User&gt;();
/// 
/// // 应用到查询
/// var users = dbContext.Users.Where(expression).ToList();
/// </code>
/// <para><b>注意事项：</b></para>
/// <list type="bullet">
/// <item><description>字符串比较是区分大小写的，如需不区分大小写，请在数据库层面配置或使用其他特性</description></item>
/// <item><description>对于可空类型属性，会自动添加 HasValue 检查，避免空引用异常</description></item>
/// <item><description>如果过滤属性值为 null 或空字符串，该条件会被忽略，不会影响查询结果</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class ContainsAttribute(string? targetPropertyName = null) : FilterAttribute(targetPropertyName)
{
    /// <inheritdoc/>
    public override Expression? GetSearchExpression<T>(ParameterExpression parameterExpression, PropertyInfo propertyInfo, T value, PropertyInfo targetPropertyInfo)
    {
        if (value is null || value.IsNullOrEmptyString()) return null;
        Expression? result = GetCallExpression(parameterExpression, propertyInfo, value, "Contains", targetPropertyInfo);
        return result;
    }
}
