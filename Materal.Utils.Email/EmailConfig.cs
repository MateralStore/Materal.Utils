namespace Materal.Utils.Email;

/// <summary>
/// 邮件发送配置
/// </summary>
public sealed class EmailConfig
{
    /// <summary>
    /// SMTP服务器地址
    /// </summary>
    public string SmtpHost { get; set; } = "smtp.qq.com";

    /// <summary>
    /// SMTP端口
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// 发送方邮箱
    /// </summary>
    public string FromAddress { get; set; } = "";

    /// <summary>
    /// 授权码（不是密码）
    /// </summary>
    public string AuthorizationCode { get; set; } = "";

    /// <summary>
    /// 是否启用SSL
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// 发送者显示名称
    /// </summary>
    public string DisplayName { get; set; } = "测试邮件系统";

    /// <summary>
    /// 验证配置是否完整
    /// </summary>
    /// <returns></returns>
    public bool IsValid() => !string.IsNullOrEmpty(FromAddress) && !string.IsNullOrEmpty(AuthorizationCode) && !string.IsNullOrEmpty(SmtpHost);
}
