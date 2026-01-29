namespace Materal.Utils.Models.Attributes;

/// <summary>
/// 大于过滤特性
/// 用于在动态查询中生成大于比较（&gt;）条件的表达式树
/// </summary>
/// <remarks>
/// <para><b>功能说明：</b></para>
/// <para>
/// 该特性继承自 <see cref="FilterAttribute"/>，用于标记过滤模型（<see cref="FilterModel"/>）的属性。
/// 当调用 <see cref="FilterModel.GetSearchExpression{T}"/> 方法时，会自动为标记了此特性的属性生成大于比较查询条件。
/// </para>
/// <para><b>工作原理：</b></para>
/// <para>
/// 生成类似 <c>m => m.TargetProperty &gt; filterValue</c> 的表达式树，用于范围查询的下限（不包含边界值）。
/// </para>
/// <para><b>典型使用场景：</b></para>
/// <list type="bullet">
/// <item><description>数值范围查询：查找大于某个数值的记录（如价格、数量）</description></item>
/// <item><description>时间范围查询：查找某个时间点之后的记录（不包含该时间点）</description></item>
/// <item><description>年龄过滤：查找年龄大于某个值的用户</description></item>
/// <item><description>评分筛选：查找评分高于某个阈值的项目</description></item>
/// </list>
/// <para><b>支持的数据类型：</b></para>
/// <list type="bullet">
/// <item><description>数值类型：int、long、decimal、double、float 等</description></item>
/// <item><description>日期时间类型：DateTime、DateTimeOffset 等</description></item>
/// <item><description>可空类型：int?、DateTime? 等</description></item>
/// <item><description>其他实现了比较接口的类型</description></item>
/// </list>
/// <para><b>使用示例：</b></para>
/// <code>
/// public class ProductFilterModel : FilterModel
/// {
///     // 查找价格大于指定值的产品
///     [GreaterThan]
///     public decimal? MinPrice { get; set; }
///     
///     // 查找创建时间晚于指定时间的记录
///     [GreaterThan(targetPropertyName: "CreateTime")]
///     public DateTime? StartTime { get; set; }
/// }
/// 
/// var filter = new ProductFilterModel { MinPrice = 100 };
/// // 生成表达式：m => m.MinPrice &gt; 100
/// var expression = filter.GetSearchExpression&lt;Product&gt;();
/// </code>
/// <para><b>注意事项：</b></para>
/// <list type="bullet">
/// <item><description>不包含边界值，如需包含请使用 <see cref="GreaterThanOrEqualAttribute"/></description></item>
/// <item><description>对于可空类型，会自动处理 HasValue 检查</description></item>
/// <item><description>如果过滤属性值为 null 或空字符串，该条件会被忽略</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class GreaterThanAttribute(string? targetPropertyName = null) : FilterAttribute(targetPropertyName)
{
    /// <inheritdoc/>
    public override Expression? GetSearchExpression<T>(ParameterExpression parameterExpression, PropertyInfo propertyInfo, T value, PropertyInfo targetPropertyInfo)
    {
        return GetSearchExpression(parameterExpression, propertyInfo, value, targetPropertyInfo, Expression.GreaterThan);
    }
}
