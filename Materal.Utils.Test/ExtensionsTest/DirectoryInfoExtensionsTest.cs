namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// DirectoryInfoExtensions 测试类
/// 测试文件夹信息扩展方法的功能
/// </summary>
[TestClass]
public class DirectoryInfoExtensionsTest
{
    private string _testBasePath = null!;
    private string _sourceDir = null!;
    private string _targetDir = null!;

    [TestInitialize]
    public void Setup()
    {
        _testBasePath = Path.Combine(Path.GetTempPath(), $"DirectoryExtensionsTest_{Guid.NewGuid()}");
        _sourceDir = Path.Combine(_testBasePath, "Source");
        _targetDir = Path.Combine(_testBasePath, "Target");
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_targetDir);
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

    #region CopyTo Tests

    /// <summary>
    /// 测试复制目录到目标路径
    /// </summary>
    [TestMethod]
    public void CopyTo_WithValidDirectory_CopiesSuccessfully_Test()
    {
        // Arrange
        string testFile = Path.Combine(_sourceDir, "test.txt");
        File.WriteAllText(testFile, "test content");
        DirectoryInfo sourceInfo = new(_sourceDir);
        string newTargetDir = Path.Combine(_testBasePath, "NewTarget");

        // Act
        sourceInfo.CopyTo(newTargetDir);

        // Assert
        Assert.IsTrue(Directory.Exists(newTargetDir));
        Assert.IsTrue(File.Exists(Path.Combine(newTargetDir, "test.txt")));
    }

    /// <summary>
    /// 测试复制包含子目录的目录
    /// </summary>
    [TestMethod]
    public void CopyTo_WithSubdirectories_CopiesRecursively_Test()
    {
        // Arrange
        string subDir = Path.Combine(_sourceDir, "SubDir");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "file.txt"), "content");
        DirectoryInfo sourceInfo = new(_sourceDir);
        string newTargetDir = Path.Combine(_testBasePath, "RecursiveTarget");

        // Act
        sourceInfo.CopyTo(newTargetDir);

        // Assert
        Assert.IsTrue(Directory.Exists(Path.Combine(newTargetDir, "SubDir")));
        Assert.IsTrue(File.Exists(Path.Combine(newTargetDir, "SubDir", "file.txt")));
    }

    /// <summary>
    /// 测试复制到已存在的目录（覆盖）
    /// </summary>
    [TestMethod]
    public void CopyTo_WithOverwriteTrue_OverwritesExistingFiles_Test()
    {
        // Arrange
        string sourceFile = Path.Combine(_sourceDir, "test.txt");
        string targetFile = Path.Combine(_targetDir, "test.txt");
        File.WriteAllText(sourceFile, "new content");
        File.WriteAllText(targetFile, "old content");
        DirectoryInfo sourceInfo = new(_sourceDir);
        DirectoryInfo targetInfo = new(_targetDir);

        // Act
        sourceInfo.CopyTo(targetInfo, overwrite: true);

        // Assert
        string content = File.ReadAllText(targetFile);
        Assert.AreEqual("new content", content);
    }

    /// <summary>
    /// 测试复制不存在的目录抛出异常
    /// </summary>
    [TestMethod]
    public void CopyTo_WithNonExistentSource_ThrowsDirectoryNotFoundException_Test()
    {
        // Arrange
        DirectoryInfo sourceInfo = new(Path.Combine(_testBasePath, "NonExistent"));
        string targetPath = Path.Combine(_testBasePath, "Target2");

        // Act & Assert
        Assert.ThrowsExactly<DirectoryNotFoundException>(() => sourceInfo.CopyTo(targetPath));
    }

    /// <summary>
    /// 测试复制到子目录抛出异常
    /// </summary>
    [TestMethod]
    public void CopyTo_ToSubdirectory_ThrowsIOException_Test()
    {
        // Arrange
        DirectoryInfo sourceInfo = new(_sourceDir);
        string subDirPath = Path.Combine(_sourceDir, "SubDir");

        // Act & Assert
        Assert.ThrowsExactly<IOException>(() => sourceInfo.CopyTo(subDirPath));
    }

    /// <summary>
    /// 测试null参数抛出异常
    /// </summary>
    [TestMethod]
    public void CopyTo_WithNullSource_ThrowsArgumentNullException_Test()
    {
        // Arrange
        DirectoryInfo sourceInfo = null!;
        string targetPath = _targetDir;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => sourceInfo.CopyTo(targetPath));
    }

    #endregion

    #region TryClear Tests

    /// <summary>
    /// 测试清空目录
    /// </summary>
    [TestMethod]
    public void TryClear_WithValidDirectory_ClearsSuccessfully_Test()
    {
        // Arrange
        File.WriteAllText(Path.Combine(_sourceDir, "file1.txt"), "content");
        File.WriteAllText(Path.Combine(_sourceDir, "file2.txt"), "content");
        DirectoryInfo dirInfo = new(_sourceDir);

        // Act
        bool result = dirInfo.TryClear();

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(Directory.Exists(_sourceDir));
        Assert.IsEmpty(Directory.GetFiles(_sourceDir));
    }

    /// <summary>
    /// 测试清空包含子目录的目录
    /// </summary>
    [TestMethod]
    public void TryClear_WithSubdirectories_RemovesAll_Test()
    {
        // Arrange
        string subDir = Path.Combine(_sourceDir, "SubDir");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "file.txt"), "content");
        DirectoryInfo dirInfo = new(_sourceDir);

        // Act
        bool result = dirInfo.TryClear();

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(Directory.Exists(_sourceDir));
        Assert.IsEmpty(Directory.GetDirectories(_sourceDir));
    }

    /// <summary>
    /// 测试清空不存在的目录抛出异常
    /// </summary>
    [TestMethod]
    public void TryClear_WithNonExistentDirectory_ThrowsDirectoryNotFoundException_Test()
    {
        // Arrange
        DirectoryInfo dirInfo = new(Path.Combine(_testBasePath, "NonExistent"));

        // Act & Assert
        Assert.ThrowsExactly<DirectoryNotFoundException>(() => dirInfo.TryClear());
    }

    /// <summary>
    /// 测试清空null目录抛出异常
    /// </summary>
    [TestMethod]
    public void TryClear_WithNullDirectory_ThrowsArgumentNullException_Test()
    {
        // Arrange
        DirectoryInfo dirInfo = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => dirInfo.TryClear());
    }

    /// <summary>
    /// 测试清空目录并获取异常列表
    /// </summary>
    [TestMethod]
    public void TryClear_WithExceptionsList_ReturnsExceptions_Test()
    {
        // Arrange
        File.WriteAllText(Path.Combine(_sourceDir, "file.txt"), "content");
        DirectoryInfo dirInfo = new(_sourceDir);

        // Act
        bool result = dirInfo.TryClear(out List<Exception> exceptions);

        // Assert
        Assert.IsTrue(result);
        Assert.IsEmpty(exceptions);
    }

    /// <summary>
    /// 测试清空空目录
    /// </summary>
    [TestMethod]
    public void TryClear_WithEmptyDirectory_ReturnsTrue_Test()
    {
        // Arrange
        DirectoryInfo dirInfo = new(_sourceDir);

        // Act
        bool result = dirInfo.TryClear();

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(Directory.Exists(_sourceDir));
    }

    #endregion
}
