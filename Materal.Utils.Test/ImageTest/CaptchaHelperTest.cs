using Materal.Utils.Image;
using SkiaSharp;

namespace Materal.Utils.Test.ImageTest;

/// <summary>
/// 验证码帮助类测试
/// </summary>
[TestClass]
public sealed class CaptchaHelperTest
{
    private static readonly string TestOutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Captchas");

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        if (Directory.Exists(TestOutputPath))
        {
            Directory.Delete(TestOutputPath, true);
        }
        Directory.CreateDirectory(TestOutputPath);
    }

    /// <summary>
    /// 测试绘制验证码图片 - 默认选项
    /// </summary>
    [TestMethod]
    public void Draw_WithDefaultOptions_ReturnsImage_Test()
    {
        // Arrange
        string captchaText = "ABC123";
        int width = 200;
        int height = 60;

        // Act
        using SKImage result = CaptchaHelper.Draw(captchaText, width, height);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(width, result.Width);
        Assert.AreEqual(height, result.Height);
    }

    /// <summary>
    /// 测试绘制验证码图片 - 自定义选项
    /// </summary>
    [TestMethod]
    public void Draw_WithCustomOptions_ReturnsImage_Test()
    {
        // Arrange
        string captchaText = "Test";
        int width = 250;
        int height = 80;
        var options = new CaptchaOptions
        {
            FontFamily = "Arial",
            FontSize = 45,
            EnableInterferenceLines = true,
            InterferenceLineCount = 3,
            EnableNoisePoints = true,
            NoisePointCount = 50
        };

        // Act
        using SKImage result = CaptchaHelper.Draw(captchaText, width, height, options);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(width, result.Width);
        Assert.AreEqual(height, result.Height);
    }

    /// <summary>
    /// 测试绘制验证码图片 - 禁用干扰线
    /// </summary>
    [TestMethod]
    public void Draw_WithoutInterferenceLines_ReturnsImage_Test()
    {
        // Arrange
        string captchaText = "NoLines";
        var options = new CaptchaOptions
        {
            EnableInterferenceLines = false,
            EnableNoisePoints = true,
            NoisePointCount = 100
        };

        // Act
        using SKImage result = CaptchaHelper.Draw(captchaText, 200, 60, options);

        // Assert
        Assert.IsNotNull(result);
    }

    /// <summary>
    /// 测试绘制验证码图片 - 禁用噪点
    /// </summary>
    [TestMethod]
    public void Draw_WithoutNoisePoints_ReturnsImage_Test()
    {
        // Arrange
        string captchaText = "NoNoise";
        var options = new CaptchaOptions
        {
            EnableInterferenceLines = true,
            EnableNoisePoints = false
        };

        // Act
        using SKImage result = CaptchaHelper.Draw(captchaText, 200, 60, options);

        // Assert
        Assert.IsNotNull(result);
    }

    /// <summary>
    /// 测试绘制验证码图片 - 使用随机预设
    /// </summary>
    [TestMethod]
    public void Draw_WithRandomPreset_ReturnsImage_Test()
    {
        // Arrange
        string captchaText = "Preset";
        var preset = CaptchaHelper.GetRandomPreset();

        // Act
        using SKImage result = CaptchaHelper.Draw(captchaText, 200, 60, preset.ToOptions());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.Width);
        Assert.AreEqual(60, result.Height);
    }

    /// <summary>
    /// 测试获取所有预设
    /// </summary>
    [TestMethod]
    public void GetPresets_ReturnsAllPresets_Test()
    {
        // Act
        IReadOnlyList<CaptchaPresetOptions> presets = CaptchaHelper.Presets;

        // Assert
        Assert.IsNotNull(presets);
        Assert.IsNotEmpty(presets);
    }

    /// <summary>
    /// 测试绘制验证码图片 - 空文本
    /// </summary>
    [TestMethod]
    public void Draw_WithEmptyText_ReturnsImage_Test()
    {
        // Arrange
        string captchaText = "";

        // Act
        using SKImage result = CaptchaHelper.Draw(captchaText, 200, 60);

        // Assert
        Assert.IsNotNull(result);
    }

    /// <summary>
    /// 测试绘制验证码图片并转换为Stream
    /// </summary>
    [TestMethod]
    public void DrawToStream_ReturnsStream_Test()
    {
        // Arrange
        string captchaText = "Stream";

        // Act
        using Stream result = CaptchaHelper.DrawToStream(captchaText, 200, 60);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThan(0, result.Length);
    }

    /// <summary>
    /// 测试绘制验证码图片并转换为Base64
    /// </summary>
    [TestMethod]
    public void DrawToBase64_ReturnsBase64String_Test()
    {
        // Arrange
        string captchaText = "Base64";

        // Act
        string result = CaptchaHelper.DrawToBase64(captchaText, 200, 60);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThan(0, result.Length);
    }

    /// <summary>
    /// 测试绘制验证码图片并保存到文件
    /// </summary>
    [TestMethod]
    public void Draw_SavesToFile_Test()
    {
        // Arrange
        string captchaText = "SaveFile";
        string fileName = $"Captcha_{DateTime.Now:yyyyMMddHHmmssfff}.png";
        string filePath = Path.Combine(TestOutputPath, fileName);

        // Act
        CaptchaHelper.Draw(captchaText, filePath, 200, 60);

        // Assert
        Assert.IsTrue(File.Exists(filePath));
    }

    /// <summary>
    /// 测试生成多种样式的验证码 - 供肉眼判断
    /// </summary>
    [TestMethod]
    public void Draw_VariousStyles_ForVisualInspection_Test()
    {
        // Arrange
        string[] testTexts = ["1234", "ABCD", "AbCd1", "aBcD2", "数字123", "混合XyZ"];
        // Act & Assert - 不传 options，使用随机预设颜色
        int fileIndex = 0;
        foreach (var text in testTexts)
        {
            string fileName = $"CaptchaVisual_{fileIndex:D2}_{text}.png";
            string filePath = Path.Combine(TestOutputPath, fileName);
            CaptchaHelper.Draw(text, filePath, 200, 60, null); // null 表示使用随机预设
            fileIndex++;
        }

        // Assert - 验证文件已生成
        Assert.HasCount(testTexts.Length, Directory.GetFiles(TestOutputPath, "CaptchaVisual_*.png"));
    }

    /// <summary>
    /// 测试不同尺寸的验证码图片
    /// </summary>
    [TestMethod]
    public void Draw_DifferentSizes_ReturnsCorrectDimensions_Test()
    {
        // Arrange
        var sizes = new[] { (120, 50), (160, 60), (200, 70), (250, 90), (320, 120) };

        // Act & Assert
        foreach (var (width, height) in sizes)
        {
            using SKImage result = CaptchaHelper.Draw("Test", width, height);
            Assert.AreEqual(width, result.Width, $"Width should be {width}");
            Assert.AreEqual(height, result.Height, $"Height should be {height}");
        }
    }
}
