using Materal.Utils.Helpers;

namespace Materal.Utils.Test.HelpersTest;

/// <summary>
/// FileHelper 测试类
/// 测试文件类型判断功能
/// </summary>
[TestClass]
public class FileHelperTest
{
    private string _testDirectory = null!;

    [TestInitialize]
    public void Setup()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"FileHelperTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    #region IsImageFile(string, out string?) 测试

    [TestMethod]
    public void IsImageFile_WithJpgFile_ReturnsTrueAndJpgType_Test()
    {
        // Arrange
        string jpgPath = Path.Combine(_testDirectory, "test.jpg");
        byte[] jpgSignature = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10];
        File.WriteAllBytes(jpgPath, jpgSignature);

        // Act
        bool result = FileHelper.IsImageFile(jpgPath, out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(imageType);
        Assert.Contains("JPG", imageType);
    }

    [TestMethod]
    public void IsImageFile_WithPngFile_ReturnsTrueAndPngType_Test()
    {
        // Arrange
        string pngPath = Path.Combine(_testDirectory, "test.png");
        byte[] pngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        File.WriteAllBytes(pngPath, pngSignature);

        // Act
        bool result = FileHelper.IsImageFile(pngPath, out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(imageType);
        Assert.AreEqual("PNG", imageType);
    }

    [TestMethod]
    public void IsImageFile_WithGifFile_ReturnsTrueAndGifType_Test()
    {
        // Arrange
        string gifPath = Path.Combine(_testDirectory, "test.gif");
        byte[] gifSignature = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61];
        File.WriteAllBytes(gifPath, gifSignature);

        // Act
        bool result = FileHelper.IsImageFile(gifPath, out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(imageType);
        Assert.AreEqual("GIF", imageType);
    }

    [TestMethod]
    public void IsImageFile_WithBmpFile_ReturnsTrueAndBmpType_Test()
    {
        // Arrange
        string bmpPath = Path.Combine(_testDirectory, "test.bmp");
        byte[] bmpSignature = [0x42, 0x4D, 0x00, 0x00];
        File.WriteAllBytes(bmpPath, bmpSignature);

        // Act
        bool result = FileHelper.IsImageFile(bmpPath, out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(imageType);
        Assert.AreEqual("BMP", imageType);
    }

    [TestMethod]
    public void IsImageFile_WithTiffFile_ReturnsTrueAndTiffType_Test()
    {
        // Arrange
        string tiffPath = Path.Combine(_testDirectory, "test.tiff");
        byte[] tiffSignature = [0x49, 0x49, 0x2A, 0x00];
        File.WriteAllBytes(tiffPath, tiffSignature);

        // Act
        bool result = FileHelper.IsImageFile(tiffPath, out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(imageType);
        Assert.AreEqual("TIFF", imageType);
    }

    [TestMethod]
    public void IsImageFile_WithIcoFile_ReturnsTrueAndIcoType_Test()
    {
        // Arrange
        string icoPath = Path.Combine(_testDirectory, "test.ico");
        byte[] icoSignature = [0x00, 0x00, 0x01, 0x00];
        File.WriteAllBytes(icoPath, icoSignature);

        // Act
        bool result = FileHelper.IsImageFile(icoPath, out string? imageType);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(imageType);
        Assert.AreEqual("ICO", imageType);
    }

    [TestMethod]
    public void IsImageFile_WithNonImageFile_ReturnsFalseAndNullType_Test()
    {
        // Arrange
        string textPath = Path.Combine(_testDirectory, "test.txt");
        File.WriteAllText(textPath, "This is not an image file");

        // Act
        bool result = FileHelper.IsImageFile(textPath, out string? imageType);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(imageType);
    }

    [TestMethod]
    public void IsImageFile_WithEmptyFile_ReturnsFalseAndNullType_Test()
    {
        // Arrange
        string emptyPath = Path.Combine(_testDirectory, "empty.dat");
        File.WriteAllBytes(emptyPath, []);

        // Act
        bool result = FileHelper.IsImageFile(emptyPath, out string? imageType);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(imageType);
    }

    [TestMethod]
    public void IsImageFile_WithNonExistentFile_ThrowsFileNotFoundException_Test()
    {
        // Arrange
        string nonExistentPath = Path.Combine(_testDirectory, "nonexistent.jpg");

        // Act & Assert
        Assert.ThrowsExactly<FileNotFoundException>(() => FileHelper.IsImageFile(nonExistentPath, out string? _));
    }

    #endregion

    #region IsImageFile(string) 测试

    [TestMethod]
    public void IsImageFile_WithJpgFile_ReturnsTrue_Test()
    {
        // Arrange
        string jpgPath = Path.Combine(_testDirectory, "test2.jpg");
        byte[] jpgSignature = [0xFF, 0xD8, 0xFF, 0xE1, 0x00, 0x10];
        File.WriteAllBytes(jpgPath, jpgSignature);

        // Act
        bool result = FileHelper.IsImageFile(jpgPath);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsImageFile_WithPngFile_ReturnsTrue_Test()
    {
        // Arrange
        string pngPath = Path.Combine(_testDirectory, "test2.png");
        byte[] pngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        File.WriteAllBytes(pngPath, pngSignature);

        // Act
        bool result = FileHelper.IsImageFile(pngPath);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsImageFile_WithNonImageFile_ReturnsFalse_Test()
    {
        // Arrange
        string textPath = Path.Combine(_testDirectory, "test2.txt");
        File.WriteAllText(textPath, "Not an image");

        // Act
        bool result = FileHelper.IsImageFile(textPath);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsImageFile_WithEmptyFile_ReturnsFalse_Test()
    {
        // Arrange
        string emptyPath = Path.Combine(_testDirectory, "empty2.dat");
        File.WriteAllBytes(emptyPath, []);

        // Act
        bool result = FileHelper.IsImageFile(emptyPath);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion
}
