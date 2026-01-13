using Materal.Utils.Email;
using System.Net.Mail;

namespace Materal.Utils.Test;

[TestClass]
public sealed class EMailTest
{
    private EmailService? _emailService;
    private EmailConfig _config = null!;

    [TestInitialize]
    public void Setup()
    {
        _config = new EmailConfig
        {
            SmtpHost = "smtp.qq.com",
            SmtpPort = 587,
            FromAddress = "342860484@qq.com", // 请替换为实际的邮箱
            AuthorizationCode = "leqlylmhjokubgjg", // 请替换为实际的授权码
            EnableSsl = true,
            DisplayName = "测试邮件系统"
        };
        _emailService = new EmailService(_config);
    }

    /// <summary>
    /// 测试发送邮件
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    [DataRow("1452940959@qq.com")]
    [DataRow("spzhanggeng@gmail.com")]
    [DataRow("cloomcmx1554@hotmail.com")]
    public async Task TestSendEmailAsync(string toAddress)
    {
        // 测试配置
        if (!_config.IsValid())
        {
            Assert.Inconclusive("邮件配置不完整，请设置环境变量或修改测试代码中的配置");
            return;
        }
        string subject = $"测试邮件 - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        string body = $@"
<h2>这是一封测试邮件</h2>
<p>此邮件用于测试邮件发送功能。</p>
<p><strong>发送时间：</strong>{DateTime.Now}</p>
<p><strong>SMTP服务器：</strong>{_config.SmtpHost}:{_config.SmtpPort}</p>
<p><strong>发送方：</strong>{_config.FromAddress}</p>
<p><strong>SSL加密：</strong>{(_config.EnableSsl ? "是" : "否")}</p>
<hr>
<p style=""color: #666; font-size: 12px;"">此邮件由 MSTest 测试框架自动发送</p>
";

        try
        {
            await _emailService!.SendEmailAsync(toAddress, subject, body, isHtml: true);
            Console.WriteLine("邮件发送成功！");
        }
        catch (InvalidOperationException ex)
        {
            Assert.Fail($"邮件发送失败：{ex.Message}");
            Console.WriteLine($"邮件发送失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 测试发送带附件的邮件
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSendEmailWithAttachmentAsync()
    {
        // 测试配置
        if (!_config.IsValid())
        {
            Assert.Inconclusive("邮件配置不完整，请设置环境变量或修改测试代码中的配置");
            return;
        }

        string toAddress = "recipient@example.com"; // 请替换为实际的收件人邮箱
        string subject = $"测试邮件（带附件） - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        string body = $@"
<h2>这是一封带附件的测试邮件</h2>
<p>请查收附件中的测试文件。</p>
<p><strong>发送时间：</strong>{DateTime.Now}</p>
<hr>
<p style=""color: #666; font-size: 12px;"">此邮件由 MSTest 测试框架自动发送</p>
";

        // 创建临时附件
        List<string> tempFiles = [];
        List<Attachment> attachments = [];

        try
        {
            // 创建文本文件附件
            string textFile = Path.GetTempFileName();
            await File.WriteAllTextAsync(textFile, $"测试文本附件\n创建时间：{DateTime.Now}\n测试内容：Hello, World!\n", TestContext.CancellationToken);
            tempFiles.Add(textFile);
            attachments.Add(new Attachment(new FileStream(textFile, FileMode.Open), "测试附件.txt", "text/plain"));

            // 创建CSV文件附件
            string csvFile = Path.GetTempFileName();
            string csvContent = "姓名,年龄,城市\n张三,25,北京\n李四,30,上海\n王五,28,广州\n";
            await File.WriteAllTextAsync(csvFile, csvContent, Encoding.UTF8, TestContext.CancellationToken);
            tempFiles.Add(csvFile);
            attachments.Add(new Attachment(new FileStream(csvFile, FileMode.Open), "测试数据.csv", "text/csv"));

            // 发送邮件
            await _emailService!.SendEmailAsync(toAddress, subject, body, isHtml: true, attachments: attachments);
            Console.WriteLine("带附件的邮件发送成功！");
        }
        finally
        {
            // 清理资源
            foreach (var attachment in attachments)
            {
                attachment.Dispose();
            }

            // 删除临时文件
            foreach (string tempFile in tempFiles)
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }

    /// <summary>
    /// 测试发送多收件人邮件
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    [DataRow("1452940959@qq.com", "spzhanggeng@gmail.com", "cloomcmx1554@hotmail.com")]
    public async Task TestSendMultipleRecipientsEmailAsync(string address1, string address2, string address3)
    {
        // 测试配置
        if (!_config.IsValid())
        {
            Assert.Inconclusive("邮件配置不完整，请设置环境变量或修改测试代码中的配置");
            return;
        }

        string[] recipients = [address1, address2, address3];
        string subject = $"群发测试邮件 - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        string body = $@"
<h2>这是一封群发测试邮件</h2>
<p>此邮件发送给多个收件人。</p>
<p><strong>发送时间：</strong>{DateTime.Now}</p>
<p><strong>收件人数量：</strong>{recipients.Length}</p>
<ul>
{string.Join("\n", recipients.Select(r => $"<li>{r}</li>"))}
</ul>
<hr>
<p style=""color: #666; font-size: 12px;"">此邮件由 MSTest 测试框架自动发送</p>
";

        try
        {
            await _emailService!.SendEmailAsync(recipients, subject, body, isHtml: true);
            Console.WriteLine($"群发邮件发送成功！收件人数量：{recipients.Length}");
        }
        catch (InvalidOperationException ex)
        {
            Assert.Fail($"群发邮件发送失败：{ex.Message}");
            Console.WriteLine($"群发邮件发送失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 测试邮件配置验证
    /// </summary>
    [TestMethod]
    public void TestEmailConfigValidation()
    {
        // 测试有效配置
        var validConfig = new EmailConfig
        {
            SmtpHost = "smtp.qq.com",
            SmtpPort = 587,
            FromAddress = "test@qq.com",
            AuthorizationCode = "test123456",
            EnableSsl = true
        };
        Assert.IsTrue(validConfig.IsValid(), "有效配置应该通过验证");

        // 测试无效配置 - 缺少发送方邮箱
        var invalidConfig1 = new EmailConfig
        {
            SmtpHost = "smtp.qq.com",
            SmtpPort = 587,
            FromAddress = "",
            AuthorizationCode = "test123456",
            EnableSsl = true
        };
        Assert.IsFalse(invalidConfig1.IsValid(), "缺少发送方邮箱的配置应该验证失败");

        // 测试无效配置 - 缺少授权码
        var invalidConfig2 = new EmailConfig
        {
            SmtpHost = "smtp.qq.com",
            SmtpPort = 587,
            FromAddress = "test@qq.com",
            AuthorizationCode = "",
            EnableSsl = true
        };
        Assert.IsFalse(invalidConfig2.IsValid(), "缺少授权码的配置应该验证失败");
    }

    public TestContext TestContext { get; set; }
}
