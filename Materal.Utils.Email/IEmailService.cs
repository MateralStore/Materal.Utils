using System.Net.Mail;

namespace Materal.Utils.Email;

/// <summary>
/// 邮件服务接口
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="to">收件人邮箱</param>
    /// <param name="subject">邮件主题</param>
    /// <param name="body">邮件内容</param>
    /// <param name="isHtml">是否HTML格式</param>
    /// <param name="attachments">附件列表</param>
    /// <returns></returns>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, IEnumerable<Attachment>? attachments = null);

    /// <summary>
    /// 发送邮件给多个收件人
    /// </summary>
    /// <param name="tos">收件人邮箱列表</param>
    /// <param name="subject">邮件主题</param>
    /// <param name="body">邮件内容</param>
    /// <param name="isHtml">是否HTML格式</param>
    /// <param name="attachments">附件列表</param>
    /// <returns></returns>
    Task SendEmailAsync(IEnumerable<string> tos, string subject, string body, bool isHtml = false, IEnumerable<Attachment>? attachments = null);
}
