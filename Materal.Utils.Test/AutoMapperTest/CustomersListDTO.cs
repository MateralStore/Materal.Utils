namespace Materal.Utils.Test.AutoMapperTest;

/// <summary>
/// 客户列表数据传输模型
/// </summary>
public partial class CustomersListDTO
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public Guid ID { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 性别
    /// </summary>
    public Sex Sex { get; set; }

    /// <summary>
    /// 性别文本
    /// </summary>
    public string SexText => Sex.GetDescription();
    /// <summary>
    /// 电话
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }
}
/// <summary>
/// 性别
/// </summary>
public enum Sex : byte
{
    /// <summary>
    /// 女性
    /// </summary>
    Woman = 0,
    /// <summary>
    /// 男性
    /// </summary>
    Man = 1
}