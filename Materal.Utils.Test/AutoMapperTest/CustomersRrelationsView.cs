namespace Materal.Utils.Test.AutoMapperTest;

/// <summary>
/// 客户关系视图
/// </summary>
public class CustomersRrelationsView
{
    public Guid ID { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid CustomersID { get; set; }
    /// <summary>
    /// 客户姓名
    /// </summary>
    public string CustomersName { get; set; } = string.Empty;
    /// <summary>
    /// 客户性别
    /// </summary>
    public Sex CustomersSex { get; set; }
    /// <summary>
    /// 客户电话
    /// </summary>
    public string CustomersPhoneNumber { get; set; } = string.Empty;
    /// <summary>
    /// 客户地址
    /// </summary>
    public string? CustomersAddress { get; set; }
    /// <summary>
    /// 责任人ID
    /// </summary>
    public Guid UserID { get; set; }
    /// <summary>
    /// 责任人姓名
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// 责任人性别
    /// </summary>
    public Sex UserSex { get; set; } = Sex.Woman;
    /// <summary>
    /// 责任人账号
    /// </summary>
    public string UserAccount { get; set; } = string.Empty;
    /// <summary>
    /// 责任人角色
    /// </summary>
    public Role UserRole { get; set; } = Role.Admin;
    /// <summary>
    /// 离职标识
    /// </summary>
    public bool UserIsDimission { get; set; } = false;
}
/// <summary>
/// 角色
/// </summary>
public enum Role : byte
{
    /// <summary>
    /// 管理员
    /// </summary>
    Admin = 0,
    /// <summary>
    /// 销售员
    /// </summary>
    Sales = 1,
    /// <summary>
    /// 采购员
    /// </summary>
    Buyer = 2,
}