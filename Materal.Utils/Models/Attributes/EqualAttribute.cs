namespace Materal.Utils.Models.Attributes;

/// <summary>
/// 等于过滤特性
/// 用于在动态查询中生成相等比较（==）条件的表达式树
/// </summary>
/// <remarks>
/// <para><b>功能说明：</b></para>
/// <para>
/// 该特性继承自 <see cref="FilterAttribute"/>，用于标记过滤模型（<see cref="FilterModel"/>）的属性。
/// 当调用 <see cref="FilterModel.GetSearchExpression{T}"/> 方法时，会自动为标记了此特性的属性生成相等比较查询条件。
/// </para>
/// <para><b>工作原理：</b></para>
/// <para>
/// 生成类似 <c>m => m.TargetProperty == filterValue</c> 的表达式树，用于精确匹配查询。
/// </para>
/// <para><b>典型使用场景：</b></para>
/// <list type="bullet">
/// <item><description>精确查询：根据 ID、编号等唯一标识进行精确查找</description></item>
/// <item><description>状态过滤：根据状态值、类型等枚举值进行过滤</description></item>
/// <item><description>布尔值过滤：根据是否启用、是否删除等布尔标志进行筛选</description></item>
/// <item><description>分类过滤：根据分类 ID、部门 ID 等进行精确匹配</description></item>
/// </list>
/// <para><b>支持的数据类型：</b></para>
/// <list type="bullet">
/// <item><description>值类型：int、long、decimal、DateTime、Guid 等</description></item>
/// <item><description>可空值类型：int?、DateTime? 等</description></item>
/// <item><description>引用类型：string、自定义类等</description></item>
/// <item><description>枚举类型：各种枚举值</description></item>
/// </list>
/// <para><b>使用示例：</b></para>
/// <code>
/// public class UserFilterModel : FilterModel
/// {
///     // 按用户 ID 精确查询
///     [Equal]
///     public Guid? ID { get; set; }
///     
///     // 按状态过滤
///     [Equal]
///     public UserStatus? Status { get; set; }
///     
///     // 按是否启用过滤
///     [Equal(targetPropertyName: "IsEnabled")]
///     public bool? Enabled { get; set; }
/// }
/// 
/// var filter = new UserFilterModel { Status = UserStatus.Active };
/// // 生成表达式：m => m.Status == UserStatus.Active
/// var expression = filter.GetSearchExpression&lt;User&gt;();
/// </code>
/// <para><b>注意事项：</b></para>
/// <list type="bullet">
/// <item><description>对于引用类型，使用引用相等性比较</description></item>
/// <item><description>对于可空类型，会自动处理 HasValue 检查</description></item>
/// <item><description>如果过滤属性值为 null 或空字符串，该条件会被忽略</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class EqualAttribute(string? targetPropertyName = null) : FilterAttribute(targetPropertyName)
{
    /// <inheritdoc/>
    public override Expression? GetSearchExpression<T>(ParameterExpression parameterExpression, PropertyInfo propertyInfo, T value, PropertyInfo targetPropertyInfo)
    {
        return GetSearchExpression(parameterExpression, propertyInfo, value, targetPropertyInfo, Expression.Equal);
    }
}
