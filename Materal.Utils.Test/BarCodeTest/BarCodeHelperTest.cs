using SkiaSharp;
using ZXing;
using ZXing.SkiaSharp;

namespace Materal.Utils.Test.BarCodeTest;

/// <summary>
/// 条码帮助类测试
/// </summary>
[TestClass]
public sealed class BarCodeHelperTest
{
    /// <summary>
    /// 测试读取二维码 - 成功场景
    /// </summary>
    [TestMethod]
    public void ReadBarCode_WithValidQRCode_ReturnsContent_Test()
    {
        // Arrange
        string content = "https://example.com";
        using SKBitmap qrCode = QRCodeHelper.CreateQRCode(content, 300);

        // Act
        string result = BarCodeHelper.ReadBarCode(qrCode, out BarcodeFormat format);

        // Assert
        Assert.AreEqual(content, result);
        Assert.AreEqual(BarcodeFormat.QR_CODE, format);
    }

    /// <summary>
    /// 测试读取条形码 - 成功场景
    /// </summary>
    [TestMethod]
    public void ReadBarCode_WithValidBarCode_ReturnsContent_Test()
    {
        // Arrange
        string content = "123456789";
        BarcodeWriter writer = new()
        {
            Format = BarcodeFormat.CODE_128,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = 300,
                Height = 100
            }
        };
        using SKBitmap barCode = writer.Write(content);

        // Act
        string result = BarCodeHelper.ReadBarCode(barCode, out BarcodeFormat format);

        // Assert
        Assert.AreEqual(content, result);
        Assert.AreEqual(BarcodeFormat.CODE_128, format);
    }

    /// <summary>
    /// 测试读取空白条码 - 抛出异常
    /// </summary>
    [TestMethod]
    public void ReadBarCode_WithWhiteBitmap_ThrowsUtilException_Test()
    {
        // Arrange
        using SKBitmap whiteBitmap = new(100, 100);
        using SKCanvas canvas = new(whiteBitmap);
        canvas.Clear(SKColors.White);

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => BarCodeHelper.ReadBarCode(whiteBitmap, out _));
    }

    /// <summary>
    /// 测试读取带特殊字符的二维码
    /// </summary>
    [TestMethod]
    public void ReadBarCode_WithSpecialCharacters_ReturnsCorrectContent_Test()
    {
        // Arrange
        string content = "HelloWorld123!@#$%";
        using SKBitmap qrCode = QRCodeHelper.CreateQRCode(content, 300);

        // Act
        string result = BarCodeHelper.ReadBarCode(qrCode, out _);

        // Assert
        Assert.AreEqual(content, result);
    }
}
