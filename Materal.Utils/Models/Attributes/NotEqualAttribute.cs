namespace Materal.Utils.Models.Attributes;

/// <summary>
/// 不等于过滤特性
/// 用于在动态查询中生成不相等比较（!=）条件的表达式树
/// </summary>
/// <remarks>
/// <para><b>功能说明：</b></para>
/// <para>
/// 该特性继承自 <see cref="FilterAttribute"/>，用于标记过滤模型（<see cref="FilterModel"/>）的属性。
/// 当调用 <see cref="FilterModel.GetSearchExpression{T}"/> 方法时，会自动为标记了此特性的属性生成不相等比较查询条件。
/// </para>
/// <para><b>工作原理：</b></para>
/// <para>
/// 生成类似 <c>m => m.TargetProperty != filterValue</c> 的表达式树，用于排除特定值的查询。
/// </para>
/// <para><b>典型使用场景：</b></para>
/// <list type="bullet">
/// <item><description>排除查询：排除特定状态、类型的数据</description></item>
/// <item><description>过滤已删除数据：排除已标记为删除的记录</description></item>
/// <item><description>排除特定用户：排除系统用户、测试用户等</description></item>
/// <item><description>黑名单过滤：排除黑名单中的值</description></item>
/// </list>
/// <para><b>使用示例：</b></para>
/// <code>
/// public class OrderFilterModel : FilterModel
/// {
///     // 排除特定状态的订单
///     [NotEqual]
///     public OrderStatus? ExcludeStatus { get; set; }
///     
///     // 排除特定用户的订单
///     [NotEqual(targetPropertyName: "UserID")]
///     public Guid? ExcludeUserID { get; set; }
/// }
/// 
/// var filter = new OrderFilterModel { ExcludeStatus = OrderStatus.Cancelled };
/// // 生成表达式：m => m.ExcludeStatus != OrderStatus.Cancelled
/// var expression = filter.GetSearchExpression&lt;Order&gt;();
/// </code>
/// <para><b>注意事项：</b></para>
/// <list type="bullet">
/// <item><description>对于可空类型，会自动处理 HasValue 检查</description></item>
/// <item><description>如果过滤属性值为 null 或空字符串，该条件会被忽略</description></item>
/// <item><description>与 <see cref="EqualAttribute"/> 相反，用于排除而非包含</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class NotEqualAttribute(string? targetPropertyName = null) : FilterAttribute(targetPropertyName)
{
    /// <inheritdoc/>
    public override Expression? GetSearchExpression<T>(ParameterExpression parameterExpression, PropertyInfo propertyInfo, T value, PropertyInfo targetPropertyInfo)
    {
        return GetSearchExpression(parameterExpression, propertyInfo, value, targetPropertyInfo, Expression.NotEqual);
    }
}
