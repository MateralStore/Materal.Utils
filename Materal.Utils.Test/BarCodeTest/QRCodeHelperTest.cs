using SkiaSharp;
using ZXing.Common;

namespace Materal.Utils.Test.BarCodeTest;

/// <summary>
/// 二维码帮助类测试
/// </summary>
[TestClass]
public sealed class QRCodeHelperTest
{
    /// <summary>
    /// 测试创建二维码 - 默认尺寸
    /// </summary>
    [TestMethod]
    public void CreateQRCode_WithDefaultSize_ReturnsBitmap_Test()
    {
        // Arrange
        string content = "https://example.com";

        // Act
        using SKBitmap result = QRCodeHelper.CreateQRCode(content);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(300, result.Width);
        Assert.AreEqual(300, result.Height);
    }

    /// <summary>
    /// 测试创建二维码 - 自定义尺寸
    /// </summary>
    [TestMethod]
    public void CreateQRCode_WithCustomSize_ReturnsBitmap_Test()
    {
        // Arrange
        string content = "https://example.com";
        int size = 500;

        // Act
        using SKBitmap result = QRCodeHelper.CreateQRCode(content, size);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(size, result.Width);
        Assert.AreEqual(size, result.Height);
    }

    /// <summary>
    /// 测试创建二维码 - 自定义宽高
    /// </summary>
    [TestMethod]
    public void CreateQRCode_WithCustomWidthAndHeight_ReturnsBitmap_Test()
    {
        // Arrange
        string content = "https://example.com";
        int width = 400;
        int height = 300;

        // Act
        using SKBitmap result = QRCodeHelper.CreateQRCode(content, height, width);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(width, result.Width);
        Assert.AreEqual(height, result.Height);
    }

    /// <summary>
    /// 测试创建二维码 - 使用EncodingOptions
    /// </summary>
    [TestMethod]
    public void CreateQRCode_WithEncodingOptions_ReturnsBitmap_Test()
    {
        // Arrange
        string content = "https://example.com";
        EncodingOptions options = new()
        {
            Width = 600,
            Height = 600,
            Margin = 2
        };

        // Act
        using SKBitmap result = QRCodeHelper.CreateQRCode(content, options);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(600, result.Width);
        Assert.AreEqual(600, result.Height);
    }

    /// <summary>
    /// 测试创建二维码 - 中文内容
    /// </summary>
    [TestMethod]
    public void CreateQRCode_WithChineseContent_ReturnsBitmap_Test()
    {
        // Arrange
        string content = "这是一个中文测试";

        // Act
        using SKBitmap result = QRCodeHelper.CreateQRCode(content);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(300, result.Width);
        Assert.AreEqual(300, result.Height);
    }

    /// <summary>
    /// 测试读取二维码 - 成功场景
    /// </summary>
    [TestMethod]
    public void ReadQRCode_WithValidQRCode_ReturnsContent_Test()
    {
        // Arrange
        string content = "https://example.com";
        using SKBitmap qrCode = QRCodeHelper.CreateQRCode(content, 300);

        // Act
        string result = QRCodeHelper.ReadQRCode(qrCode);

        // Assert
        Assert.AreEqual(content, result);
    }

    /// <summary>
    /// 测试读取二维码 - 空图片抛出异常
    /// </summary>
    [TestMethod]
    public void ReadQRCode_WithEmptyBitmap_ThrowsUtilException_Test()
    {
        // Arrange - 创建白色位图（无条码）
        using SKBitmap emptyBitmap = new(100, 100);
        using SKCanvas canvas = new(emptyBitmap);
        canvas.Clear(SKColors.White);

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => QRCodeHelper.ReadQRCode(emptyBitmap));
    }

    /// <summary>
    /// 测试添加Logo - 成功场景
    /// </summary>
    [TestMethod]
    public void AddLogo_WithValidQRCodeAndLogo_ReturnsBitmapWithLogo_Test()
    {
        // Arrange
        string content = "https://example.com";
        using SKBitmap qrCode = QRCodeHelper.CreateQRCode(content, 300);
        using SKBitmap logo = new(50, 50);
        using (SKCanvas canvas = new(logo))
        {
            canvas.Clear(SKColors.Red);
        }
        float logoSize = 50f;

        // Act
        using SKBitmap result = QRCodeHelper.AddLogo(qrCode, logo, logoSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(300, result.Width);
        Assert.AreEqual(300, result.Height);
        // 检查中心区域是否为红色(logo颜色)
        SKColor centerPixel = result.GetPixel(150, 150);
        Assert.AreEqual(SKColors.Red, centerPixel);
    }

    /// <summary>
    /// 测试添加Logo - 不同尺寸
    /// </summary>
    [DataRow(30f)]
    [DataRow(50f)]
    [DataRow(80f)]
    [TestMethod]
    public void AddLogo_WithDifferentLogoSizes_ReturnsBitmap_Test(float logoSize)
    {
        // Arrange
        string content = "https://example.com";
        using SKBitmap qrCode = QRCodeHelper.CreateQRCode(content, 300);
        using SKBitmap logo = new((int)logoSize, (int)logoSize);
        using (SKCanvas canvas = new(logo))
        {
            canvas.Clear(SKColors.Blue);
        }

        // Act
        using SKBitmap result = QRCodeHelper.AddLogo(qrCode, logo, logoSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(300, result.Width);
        Assert.AreEqual(300, result.Height);
    }

    /// <summary>
    /// 测试更改二维码图片 - 自定义绘制
    /// </summary>
    [TestMethod]
    public void ChangeQRCodeImage_WithCustomDrawAction_ReturnsModifiedQRCode_Test()
    {
        // Arrange
        string content = "https://example.com";
        using SKBitmap qrCode = QRCodeHelper.CreateQRCode(content, 300);

        // Act
        using SKBitmap result = QRCodeHelper.ChangeQRCodeImage(qrCode, (canvas, paint, point, size) =>
        {
            paint.Color = new SKColor(255, 0, 0, 128); // 半透明红色
            canvas.DrawCircle(point.X, point.Y, (float)Math.Min(size.Width, size.Height) / 2, paint);
        });

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(300, result.Width);
        Assert.AreEqual(300, result.Height);
    }

    /// <summary>
    /// 测试更改二维码图片 - 带自定义背景色
    /// </summary>
    [TestMethod]
    public void ChangeQRCodeImage_WithCustomBackground_ReturnsQRCodeWithBackground_Test()
    {
        // Arrange
        string content = "https://example.com";
        using SKBitmap qrCode = QRCodeHelper.CreateQRCode(content, 300);
        SKColor customBackground = new(200, 200, 255); // 浅蓝色背景

        // Act
        using SKBitmap result = QRCodeHelper.ChangeQRCodeImage(qrCode,
            (canvas, paint, point, size) => paint.Color = SKColors.Black,
            background: customBackground);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(300, result.Width);
        Assert.AreEqual(300, result.Height);
        // 检查背景色
        SKColor cornerPixel = result.GetPixel(0, 0);
        Assert.AreEqual(customBackground, cornerPixel);
    }

    /// <summary>
    /// 测试更改二维码图片 - 使用标记绘制回调
    /// </summary>
    [TestMethod]
    public void ChangeQRCodeImage_WithMarkPaintSet_ReturnsQRCodeWithCustomMarks_Test()
    {
        // Arrange
        string content = "https://example.com";
        using SKBitmap qrCode = QRCodeHelper.CreateQRCode(content, 300);
        bool markPaintCalled = false;

        // Act
        using SKBitmap result = QRCodeHelper.ChangeQRCodeImage(qrCode,
            (canvas, paint, point, size) => paint.Color = SKColors.Black,
            (paint, point) =>
            {
                markPaintCalled = true;
                paint.Color = new SKColor(0, 0, 255); // 蓝色标记
            });

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(markPaintCalled);
    }

    /// <summary>
    /// 测试读取和创建二维码的往返测试
    /// </summary>
    [TestMethod]
    public void CreateAndReadQRCode_RoundTrip_ReturnsOriginalContent_Test()
    {
        // Arrange
        string originalContent = "https://github.com/MateralCMX/Materal";

        // Act - 创建
        using SKBitmap qrCode = QRCodeHelper.CreateQRCode(originalContent, 400);

        // Act - 读取
        string readContent = QRCodeHelper.ReadQRCode(qrCode);

        // Assert
        Assert.AreEqual(originalContent, readContent);
    }
}
