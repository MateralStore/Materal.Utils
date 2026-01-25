namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// FileInfoExtensions 测试类
/// 测试文件信息扩展方法的功能
/// </summary>
[TestClass]
public class FileInfoExtensionsTest
{
    private string _testBasePath = null!;

    [TestInitialize]
    public void Setup()
    {
        _testBasePath = Path.Combine(Path.GetTempPath(), $"FileExtensionsTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testBasePath);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_testBasePath))
        {
            try
            {
                Directory.Delete(_testBasePath, true);
            }
            catch
            {
            }
        }
    }

    #region IsImageFile Tests

    /// <summary>
    /// 测试JPEG图片文件识别
    /// </summary>
    [TestMethod]
    public void IsImageFile_WithJpegFile_ReturnsTrue_Test()
    {
        // Arrange
        string filePath = Path.Combine(_testBasePath, "test.jpg");
        byte[] jpegHeader = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10];
        File.WriteAllBytes(filePath, jpegHeader);
        FileInfo fileInfo = new(filePath);

        // Act
        bool result = fileInfo.IsImageFile(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(imageType);
        Assert.AreEqual("JPG|JPEG", imageType);
    }

    /// <summary>
    /// 测试PNG图片文件识别
    /// </summary>
    [TestMethod]
    public void IsImageFile_WithPngFile_ReturnsTrue_Test()
    {
        // Arrange
        string filePath = Path.Combine(_testBasePath, "test.png");
        byte[] pngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        File.WriteAllBytes(filePath, pngHeader);
        FileInfo fileInfo = new(filePath);

        // Act
        bool result = fileInfo.IsImageFile(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("PNG", imageType);
    }

    /// <summary>
    /// 测试GIF图片文件识别（GIF89a）
    /// </summary>
    [TestMethod]
    public void IsImageFile_WithGif89aFile_ReturnsTrue_Test()
    {
        // Arrange
        string filePath = Path.Combine(_testBasePath, "test.gif");
        byte[] gifHeader = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61];
        File.WriteAllBytes(filePath, gifHeader);
        FileInfo fileInfo = new(filePath);

        // Act
        bool result = fileInfo.IsImageFile();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试GIF图片文件识别（GIF87a变体）
    /// </summary>
    [TestMethod]
    public void IsImageFile_WithGif87aFile_ReturnsTrue_Test()
    {
        // Arrange
        string filePath = Path.Combine(_testBasePath, "test87a.gif");
        byte[] gifHeader = [0x47, 0x49, 0x46, 0x38, 0x37, 0x61];
        File.WriteAllBytes(filePath, gifHeader);
        FileInfo fileInfo = new(filePath);

        // Act
        bool result = fileInfo.IsImageFile(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("GIF", imageType);
    }

    /// <summary>
    /// 测试非图片文件返回false
    /// </summary>
    [TestMethod]
    public void IsImageFile_WithTextFile_ReturnsFalse_Test()
    {
        // Arrange
        string filePath = Path.Combine(_testBasePath, "test.txt");
        File.WriteAllText(filePath, "This is a text file");
        FileInfo fileInfo = new(filePath);

        // Act
        bool result = fileInfo.IsImageFile(out string? imageType);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(imageType);
    }

    /// <summary>
    /// 测试不存在的文件抛出异常
    /// </summary>
    [TestMethod]
    public void IsImageFile_WithNonExistentFile_ThrowsFileNotFoundException_Test()
    {
        // Arrange
        string filePath = Path.Combine(_testBasePath, "nonexistent.jpg");
        FileInfo fileInfo = new(filePath);

        // Act & Assert
        Assert.ThrowsExactly<FileNotFoundException>(() => fileInfo.IsImageFile());
    }

    /// <summary>
    /// 测试空文件返回false
    /// </summary>
    [TestMethod]
    public void IsImageFile_WithEmptyFile_ReturnsFalse_Test()
    {
        // Arrange
        string filePath = Path.Combine(_testBasePath, "empty.jpg");
        File.WriteAllBytes(filePath, []);
        FileInfo fileInfo = new(filePath);

        // Act
        bool result = fileInfo.IsImageFile();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试BMP图片文件识别
    /// </summary>
    [TestMethod]
    public void IsImageFile_WithBmpFile_ReturnsTrue_Test()
    {
        // Arrange
        string filePath = Path.Combine(_testBasePath, "test.bmp");
        byte[] bmpHeader = [0x42, 0x4D, 0x00, 0x00];
        File.WriteAllBytes(filePath, bmpHeader);
        FileInfo fileInfo = new(filePath);

        // Act
        bool result = fileInfo.IsImageFile(out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("BMP", imageType);
    }

    /// <summary>
    /// 测试IsImageFile重载方法（不返回imageType）
    /// </summary>
    [TestMethod]
    public void IsImageFile_WithoutImageTypeParameter_WorksCorrectly_Test()
    {
        // Arrange
        string filePath = Path.Combine(_testBasePath, "test.png");
        byte[] pngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        File.WriteAllBytes(filePath, pngHeader);
        FileInfo fileInfo = new(filePath);

        // Act
        bool result = fileInfo.IsImageFile();

        // Assert
        Assert.IsTrue(result);
    }

    #endregion
}
