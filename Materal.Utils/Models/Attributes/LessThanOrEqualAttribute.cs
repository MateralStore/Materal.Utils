namespace Materal.Utils.Models.Attributes;

/// <summary>
/// 小于等于过滤特性
/// 用于在动态查询中生成小于等于比较（&lt;=）条件的表达式树
/// </summary>
/// <remarks>
/// <para><b>功能说明：</b></para>
/// <para>
/// 该特性继承自 <see cref="FilterAttribute"/>，用于标记过滤模型（<see cref="FilterModel"/>）的属性。
/// 当调用 <see cref="FilterModel.GetSearchExpression{T}"/> 方法时，会自动为标记了此特性的属性生成小于等于比较查询条件。
/// </para>
/// <para><b>工作原理：</b></para>
/// <para>
/// 生成类似 <c>m => m.TargetProperty &lt;= filterValue</c> 的表达式树，用于范围查询的上限（包含边界值）。
/// </para>
/// <para><b>典型使用场景：</b></para>
/// <list type="bullet">
/// <item><description>数值范围查询：查找小于等于某个数值的记录（如最高价格、最大库存）</description></item>
/// <item><description>时间范围查询：查找到某个时间点为止的记录（包含该时间点）</description></item>
/// <item><description>最大值过滤：设置各种指标的最大阈值</description></item>
/// <item><description>截止日期查询：查找指定日期及之前的数据</description></item>
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
/// public class OrderFilterModel : FilterModel
/// {
///     // 查找订单金额小于等于指定值的订单
///     [LessThanOrEqual]
///     public decimal? MaxAmount { get; set; }
///     
///     // 查找指定日期及之前创建的订单
///     [LessThanOrEqual(targetPropertyName: "CreateTime")]
///     public DateTime? EndDate { get; set; }
/// }
/// 
/// var filter = new OrderFilterModel { EndDate = new DateTime(2024, 12, 31) };
/// // 生成表达式：m => m.CreateTime &lt;= new DateTime(2024, 12, 31)
/// var expression = filter.GetSearchExpression&lt;Order&gt;();
/// </code>
/// <para><b>注意事项：</b></para>
/// <list type="bullet">
/// <item><description>包含边界值，与 <see cref="LessThanAttribute"/> 的区别在于包含等于的情况</description></item>
/// <item><description>对于可空类型，会自动处理 HasValue 检查</description></item>
/// <item><description>如果过滤属性值为 null 或空字符串，该条件会被忽略</description></item>
/// <item><description>常与 <see cref="GreaterThanOrEqualAttribute"/> 配合使用实现闭区间范围查询</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class LessThanOrEqualAttribute(string? targetPropertyName = null) : FilterAttribute(targetPropertyName)
{
    /// <inheritdoc/>
    public override Expression? GetSearchExpression<T>(ParameterExpression parameterExpression, PropertyInfo propertyInfo, T value, PropertyInfo targetPropertyInfo)
    {
        return GetSearchExpression(parameterExpression, propertyInfo, value, targetPropertyInfo, Expression.LessThanOrEqual);
    }
}
