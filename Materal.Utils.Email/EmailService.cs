using System.Net;
using System.Net.Mail;
using System.Text;

namespace Materal.Utils.Email;

/// <summary>
/// 邮件发送服务
/// </summary>
public partial class EmailService(EmailConfig config) : IEmailService
{
    private readonly EmailConfig _config = config;

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="to">收件人邮箱</param>
    /// <param name="subject">邮件主题</param>
    /// <param name="body">邮件内容</param>
    /// <param name="isHtml">是否HTML格式</param>
    /// <param name="attachments">附件列表</param>
    /// <returns></returns>
    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, IEnumerable<Attachment>? attachments = null)
    {
        if (!_config.IsValid()) throw new InvalidOperationException("邮件配置不完整，请检查配置信息");

        MailMessage mailMessage = new()
        {
            From = new MailAddress(_config.FromAddress, _config.DisplayName, Encoding.UTF8),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml,
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8
        };
        mailMessage.To.Add(to);

        // 添加附件
        if (attachments != null)
        {
            foreach (Attachment attachment in attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }
        }

        await SendEmailAsync(mailMessage);
    }

    /// <summary>
    /// 发送邮件给多个收件人
    /// </summary>
    /// <param name="tos">收件人邮箱列表</param>
    /// <param name="subject">邮件主题</param>
    /// <param name="body">邮件内容</param>
    /// <param name="isHtml">是否HTML格式</param>
    /// <param name="attachments">附件列表</param>
    /// <returns></returns>
    public async Task SendEmailAsync(IEnumerable<string> tos, string subject, string body, bool isHtml = false, IEnumerable<Attachment>? attachments = null)
    {
        if (!_config.IsValid()) throw new InvalidOperationException("邮件配置不完整，请检查配置信息");

        MailMessage mailMessage = new()
        {
            From = new MailAddress(_config.FromAddress, _config.DisplayName, Encoding.UTF8),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml,
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8
        };

        foreach (string to in tos)
        {
            mailMessage.To.Add(to);
        }

        // 添加附件
        if (attachments != null)
        {
            foreach (var attachment in attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }
        }

        await SendEmailAsync(mailMessage);
    }

    /// <summary>
    /// 发送邮件（内部方法）
    /// </summary>
    /// <param name="mailMessage">邮件消息</param>
    /// <returns></returns>
    private async Task SendEmailAsync(MailMessage mailMessage)
    {
        using SmtpClient smtpClient = new(_config.SmtpHost, _config.SmtpPort)
        {
            EnableSsl = _config.EnableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_config.FromAddress, _config.AuthorizationCode),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 30000 // 30秒超时
        };

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (SmtpException ex)
        {
            // 记录详细的错误信息
            string errorMessage = $"SMTP错误: {ex.StatusCode} - {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\n内部异常: {ex.InnerException.Message}";
            }

            throw new InvalidOperationException(errorMessage, ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"发送邮件时发生错误: {ex.Message}", ex);
        }
    }
}
