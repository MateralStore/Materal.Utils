namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// StreamExtensions 测试类
/// 测试流扩展方法的功能
/// </summary>
[TestClass]
public class StreamExtensionsTest
{
    #region IsImage Tests

    /// <summary>
    /// 测试JPEG图片识别
    /// </summary>
    [TestMethod]
    public void IsImage_WithJpegImage_ReturnsTrue_Test()
    {
        // Arrange
        byte[] jpegHeader = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10];
        using MemoryStream stream = new(jpegHeader);

        // Act
        bool result = stream.IsImage(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(imageType);
        Assert.Contains("JPG", imageType);
    }

    /// <summary>
    /// 测试PNG图片识别
    /// </summary>
    [TestMethod]
    public void IsImage_WithPngImage_ReturnsTrue_Test()
    {
        // Arrange
        byte[] pngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        using MemoryStream stream = new(pngHeader);

        // Act
        bool result = stream.IsImage(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("PNG", imageType);
    }

    /// <summary>
    /// 测试GIF图片识别
    /// </summary>
    [TestMethod]
    public void IsImage_WithGifImage_ReturnsTrue_Test()
    {
        // Arrange
        byte[] gifHeader = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61];
        using MemoryStream stream = new(gifHeader);

        // Act
        bool result = stream.IsImage(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("GIF", imageType);
    }

    /// <summary>
    /// 测试BMP图片识别
    /// </summary>
    [TestMethod]
    public void IsImage_WithBmpImage_ReturnsTrue_Test()
    {
        // Arrange
        byte[] bmpHeader = [0x42, 0x4D, 0x00, 0x00];
        using MemoryStream stream = new(bmpHeader);

        // Act
        bool result = stream.IsImage(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("BMP", imageType);
    }

    /// <summary>
    /// 测试TIFF图片识别
    /// </summary>
    [TestMethod]
    public void IsImage_WithTiffImage_ReturnsTrue_Test()
    {
        // Arrange
        byte[] tiffHeader = [0x49, 0x49, 0x2A, 0x00];
        using MemoryStream stream = new(tiffHeader);

        // Act
        bool result = stream.IsImage(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("TIFF", imageType);
    }

    /// <summary>
    /// 测试ICO图片识别
    /// </summary>
    [TestMethod]
    public void IsImage_WithIcoImage_ReturnsTrue_Test()
    {
        // Arrange
        byte[] icoHeader = [0x00, 0x00, 0x01, 0x00];
        using MemoryStream stream = new(icoHeader);

        // Act
        bool result = stream.IsImage(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("ICO", imageType);
    }

    /// <summary>
    /// 测试非图片文件返回false
    /// </summary>
    [TestMethod]
    public void IsImage_WithNonImageData_ReturnsFalse_Test()
    {
        // Arrange
        byte[] nonImageData = [0x00, 0x01, 0x02, 0x03, 0x04, 0x05];
        using MemoryStream stream = new(nonImageData);

        // Act
        bool result = stream.IsImage(out string? imageType);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(imageType);
    }

    /// <summary>
    /// 测试空流返回false
    /// </summary>
    [TestMethod]
    public void IsImage_WithEmptyStream_ReturnsFalse_Test()
    {
        // Arrange
        using MemoryStream stream = new([]);

        // Act
        bool result = stream.IsImage(out string? imageType);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(imageType);
    }

    /// <summary>
    /// 测试IsImage重载方法（不返回imageType）
    /// </summary>
    [TestMethod]
    public void IsImage_WithoutImageTypeParameter_ReturnsTrue_Test()
    {
        // Arrange
        byte[] jpegHeader = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10];
        using MemoryStream stream = new(jpegHeader);

        // Act
        bool result = stream.IsImage();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试流位置在检测前被重置为0
    /// </summary>
    [TestMethod]
    public void IsImage_ResetsStreamPositionBeforeCheck_Test()
    {
        // Arrange
        byte[] pngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        MemoryStream stream = new(pngHeader)
        {
            Position = 5
        };

        // Act
        bool result = stream.IsImage();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试JPEG的其他变体头部
    /// </summary>
    [TestMethod]
    public void IsImage_WithJpegVariant_ReturnsTrue_Test()
    {
        // Arrange
        byte[] jpegHeader = [0xFF, 0xD8, 0xFF, 0xE1, 0x00, 0x10];
        using MemoryStream stream = new(jpegHeader);

        // Act
        bool result = stream.IsImage(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.Contains("JPG", imageType!);
    }

    /// <summary>
    /// 测试文本文件返回false
    /// </summary>
    [TestMethod]
    public void IsImage_WithTextData_ReturnsFalse_Test()
    {
        // Arrange
        byte[] textData = Encoding.UTF8.GetBytes("This is a text file");
        using MemoryStream stream = new(textData);

        // Act
        bool result = stream.IsImage();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion
}
