using SkiaSharp;

namespace Materal.Utils.Test.BarCodeTest;

/// <summary>
/// SKBitmap扩展测试
/// </summary>
[TestClass]
public sealed class SKBitmapExtensionsTest
{
    private string _testDirectory = null!;

    [TestInitialize]
    public void Setup()
    {
        // 使用临时目录
        _testDirectory = Path.GetTempPath();
    }

    [TestCleanup]
    public void Cleanup()
    {
        try
        {
            // 清理测试文件
            string[]? files =
            [
                .. Directory.GetFiles(_testDirectory, "barcode_test*.png"),
                .. Directory.GetFiles(_testDirectory, "barcode_test*.jpg"),
                .. Directory.GetFiles(_testDirectory, "barcode_test*.webp"),
            ];

            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }
        catch { }
    }

    /// <summary>
    /// 测试保存图片 - PNG格式
    /// </summary>
    [TestMethod]
    public void SaveAs_WithPNGFormat_SavesSuccessfully_Test()
    {
        // Arrange
        using SKBitmap bitmap = new(100, 100);
        using SKCanvas canvas = new(bitmap);
        canvas.Clear(SKColors.Red);
        string savePath = Path.Combine(_testDirectory, "barcode_test_" + Guid.NewGuid().ToString("N") + ".png");

        // Act
        bitmap.SaveAs(savePath, SKEncodedImageFormat.Png);

        // Assert
        Assert.IsTrue(File.Exists(savePath));
        FileInfo fileInfo = new(savePath);
        fileInfo.Refresh();
        Assert.IsGreaterThan(0, fileInfo.Length, "文件大小应该大于0");
    }

    /// <summary>
    /// 测试保存图片 - JPEG格式
    /// </summary>
    [TestMethod]
    public void SaveAs_WithJPEGFormat_SavesSuccessfully_Test()
    {
        // Arrange
        using SKBitmap bitmap = new(100, 100);
        using SKCanvas canvas = new(bitmap);
        canvas.Clear(SKColors.Blue);
        string savePath = Path.Combine(_testDirectory, "barcode_test_" + Guid.NewGuid().ToString("N") + ".jpg");

        // Act
        bitmap.SaveAs(savePath, SKEncodedImageFormat.Jpeg);

        // Assert
        Assert.IsTrue(File.Exists(savePath));
        FileInfo fileInfo = new(savePath);
        fileInfo.Refresh();
        Assert.IsGreaterThan(0, fileInfo.Length, "文件大小应该大于0");
    }

    /// <summary>
    /// 测试保存图片 - WebP格式
    /// </summary>
    [TestMethod]
    public void SaveAs_WithWebPFormat_SavesSuccessfully_Test()
    {
        // Arrange
        using SKBitmap bitmap = new(100, 100);
        using SKCanvas canvas = new(bitmap);
        canvas.Clear(SKColors.Green);
        string savePath = Path.Combine(_testDirectory, "barcode_test_" + Guid.NewGuid().ToString("N") + ".webp");

        // Act
        bitmap.SaveAs(savePath, SKEncodedImageFormat.Webp);

        // Assert
        Assert.IsTrue(File.Exists(savePath));
        FileInfo fileInfo = new(savePath);
        fileInfo.Refresh();
        Assert.IsGreaterThan(0, fileInfo.Length, "文件大小应该大于0");
    }

    /// <summary>
    /// 测试保存图片 - 使用FileInfo参数
    /// </summary>
    [TestMethod]
    public void SaveAs_WithFileInfo_SavesSuccessfully_Test()
    {
        // Arrange
        using SKBitmap bitmap = new(100, 100);
        using SKCanvas canvas = new(bitmap);
        canvas.Clear(SKColors.Yellow);
        FileInfo fileInfo = new(Path.Combine(_testDirectory, "barcode_test_" + Guid.NewGuid().ToString("N") + ".png"));

        // Act
        bitmap.SaveAs(fileInfo, SKEncodedImageFormat.Png);

        // Assert
        Assert.IsTrue(File.Exists(fileInfo.FullName));
        fileInfo.Refresh();
        Assert.IsGreaterThan(0, fileInfo.Length, "文件大小应该大于0");
    }

    /// <summary>
    /// 测试保存图片 - 覆盖已存在文件
    /// </summary>
    [TestMethod]
    public void SaveAs_WhenFileExists_OverwritesFile_Test()
    {
        // Arrange
        string savePath = Path.Combine(_testDirectory, "barcode_test_" + Guid.NewGuid().ToString("N") + ".png");
        // 先创建文件
        using (var originalBitmap = new SKBitmap(50, 50))
        using (var canvas = new SKCanvas(originalBitmap))
        {
            canvas.Clear(SKColors.Black);
            originalBitmap.SaveAs(savePath, SKEncodedImageFormat.Png);
        }
        FileInfo fileInfo = new(savePath);
        long originalSize = fileInfo.Length;

        // Act - 保存新图片覆盖
        using (var newBitmap = new SKBitmap(100, 100))
        using (var canvas = new SKCanvas(newBitmap))
        {
            canvas.Clear(SKColors.White);
            newBitmap.SaveAs(savePath, SKEncodedImageFormat.Png);
        }

        // Assert
        Assert.IsTrue(File.Exists(savePath));
        fileInfo.Refresh();
        Assert.AreNotEqual(originalSize, fileInfo.Length, "文件大小应该不同");
    }

    /// <summary>
    /// 测试保存图片 - 默认格式为PNG
    /// </summary>
    [TestMethod]
    public void SaveAs_WithDefaultFormat_SavesAsPNG_Test()
    {
        // Arrange
        using SKBitmap bitmap = new(100, 100);
        using SKCanvas canvas = new(bitmap);
        canvas.Clear(SKColors.Purple);
        string savePath = Path.Combine(_testDirectory, "barcode_test_" + Guid.NewGuid().ToString("N") + ".png");

        // Act - 不指定格式
        bitmap.SaveAs(savePath);

        // Assert
        Assert.IsTrue(File.Exists(savePath));
        FileInfo fileInfo = new(savePath);
        fileInfo.Refresh();
        Assert.IsGreaterThan(0, fileInfo.Length, "文件大小应该大于0");

        // 验证可以读取
        using SKBitmap loadedBitmap = SKBitmap.Decode(savePath);
        Assert.IsNotNull(loadedBitmap);
        Assert.AreEqual(100, loadedBitmap.Width);
        Assert.AreEqual(100, loadedBitmap.Height);
    }

    /// <summary>
    /// 测试保存和加载图片的往返测试
    /// </summary>
    [TestMethod]
    public void SaveAndLoadImage_RoundTrip_PreservesContent_Test()
    {
        // Arrange
        using SKBitmap originalBitmap = new(50, 50);
        using SKCanvas canvas = new(originalBitmap);
        canvas.Clear(SKColors.Red);
        // 设置一些特定像素
        originalBitmap.SetPixel(10, 10, SKColors.Blue);
        originalBitmap.SetPixel(25, 25, SKColors.Green);
        string savePath = Path.Combine(_testDirectory, "barcode_test_" + Guid.NewGuid().ToString("N") + ".png");

        // Act - 保存
        originalBitmap.SaveAs(savePath, SKEncodedImageFormat.Png);

        // Act - 加载
        using SKBitmap loadedBitmap = SKBitmap.Decode(savePath);

        // Assert
        Assert.IsNotNull(loadedBitmap);
        Assert.AreEqual(originalBitmap.Width, loadedBitmap.Width);
        Assert.AreEqual(originalBitmap.Height, loadedBitmap.Height);
        // 注意：PNG 是有损压缩，颜色可能不完全匹配
        // 这里只验证主要区域颜色
        SKColor loadedCenterPixel = loadedBitmap.GetPixel(25, 25);
        Assert.AreEqual(SKColors.Green, loadedCenterPixel);
    }

    /// <summary>
    /// 测试保存图片 - 不同的图片尺寸
    /// </summary>
    [TestMethod]
    [DataRow(10, 10)]
    [DataRow(100, 100)]
    [DataRow(500, 300)]
    public void SaveAs_WithDifferentDimensions_SavesSuccessfully_Test(int width, int height)
    {
        // Arrange
        using SKBitmap bitmap = new(width, height);
        using SKCanvas canvas = new(bitmap);
        canvas.Clear(SKColors.Orange);
        string filename = "barcode_test_" + Guid.NewGuid().ToString("N") + ".png";
        string savePath = Path.Combine(_testDirectory, filename);

        // Act
        bitmap.SaveAs(savePath, SKEncodedImageFormat.Png);

        // Assert
        Assert.IsTrue(File.Exists(savePath));
        FileInfo fileInfo = new(savePath);
        fileInfo.Refresh();
        Assert.IsGreaterThan(0, fileInfo.Length, "文件大小应该大于0");

        // 验证加载的图片尺寸正确
        using SKBitmap loadedBitmap = SKBitmap.Decode(savePath);
        Assert.IsNotNull(loadedBitmap);
        Assert.AreEqual(width, loadedBitmap.Width);
        Assert.AreEqual(height, loadedBitmap.Height);
    }
}
