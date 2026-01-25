using System.Reflection;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// AssemblyExtensions 测试类
/// 测试程序集扩展方法的功能
/// </summary>
[TestClass]
public class AssemblyExtensionsTest
{
    private Assembly _testAssembly = null!;

    [TestInitialize]
    public void Setup()
    {
        _testAssembly = typeof(AssemblyExtensionsTest).Assembly;
    }

    #region GetTypeByFilter Tests

    /// <summary>
    /// 测试使用过滤器获取第一个匹配的类型
    /// </summary>
    [TestMethod]
    public void GetTypeByFilter_WithMatchingFilter_ReturnsFirstMatchingType_Test()
    {
        // Arrange
        static bool filter(Type t) => t.Name == nameof(AssemblyExtensionsTest);

        // Act
        Type? result = _testAssembly.GetTypeByFilter(filter);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(AssemblyExtensionsTest), result.Name);
    }

    /// <summary>
    /// 测试使用不匹配的过滤器返回null
    /// </summary>
    [TestMethod]
    public void GetTypeByFilter_WithNonMatchingFilter_ReturnsNull_Test()
    {
        // Arrange
        static bool filter(Type t) => t.Name == "NonExistentType";

        // Act
        Type? result = _testAssembly.GetTypeByFilter(filter);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试过滤器抛出异常时返回null
    /// </summary>
    [TestMethod]
    public void GetTypeByFilter_WhenFilterThrowsException_ReturnsNull_Test()
    {
        // Arrange
        static bool filter(Type t) => throw new InvalidOperationException("Test exception");

        // Act
        Type? result = _testAssembly.GetTypeByFilter(filter);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试过滤公共类
    /// </summary>
    [TestMethod]
    public void GetTypeByFilter_WithPublicClassFilter_ReturnsPublicClass_Test()
    {
        // Arrange
        static bool filter(Type t) => t.IsPublic && t.IsClass;

        // Act
        Type? result = _testAssembly.GetTypeByFilter(filter);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsPublic);
        Assert.IsTrue(result.IsClass);
    }

    #endregion

    #region GetTypesByFilter Tests

    /// <summary>
    /// 测试使用过滤器获取所有匹配的类型
    /// </summary>
    [TestMethod]
    public void GetTypesByFilter_WithMatchingFilter_ReturnsMatchingTypes_Test()
    {
        // Arrange
        static bool filter(Type t) => t.IsClass && t.IsPublic;

        // Act
        IEnumerable<Type> result = _testAssembly.GetTypesByFilter(filter);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        Assert.IsTrue(result.All(t => t.IsClass && t.IsPublic));
    }

    /// <summary>
    /// 测试使用不匹配的过滤器返回空集合
    /// </summary>
    [TestMethod]
    public void GetTypesByFilter_WithNonMatchingFilter_ReturnsEmptyCollection_Test()
    {
        // Arrange
        static bool filter(Type t) => t.Name == "NonExistentType";

        // Act
        IEnumerable<Type> result = _testAssembly.GetTypesByFilter(filter);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Any());
    }

    /// <summary>
    /// 测试过滤器抛出异常时返回空集合
    /// </summary>
    [TestMethod]
    public void GetTypesByFilter_WhenFilterThrowsException_ReturnsEmptyCollection_Test()
    {
        // Arrange
        static bool filter(Type t)
        {
            throw new InvalidOperationException("Test exception");
        }

        // Act
        IEnumerable<Type> result = _testAssembly.GetTypesByFilter(filter);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Any());
    }

    /// <summary>
    /// 测试过滤所有测试类
    /// </summary>
    [TestMethod]
    public void GetTypesByFilter_WithTestClassFilter_ReturnsTestClasses_Test()
    {
        // Arrange
        static bool filter(Type t) => t.GetCustomAttribute<TestClassAttribute>() is not null;

        // Act
        IEnumerable<Type> result = _testAssembly.GetTypesByFilter(filter);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        Assert.IsTrue(result.All(t => t.GetCustomAttribute<TestClassAttribute>() is not null));
    }

    #endregion

    #region GetTypes<T> Tests

    /// <summary>
    /// 测试获取实现指定接口的所有公共具体类
    /// </summary>
    [TestMethod]
    public void GetTypes_WithInterface_ReturnsImplementingClasses_Test()
    {
        // Arrange
        Assembly utilsAssembly = typeof(Materal.Utils.Extensions.AssemblyExtensions).Assembly;

        // Act
        IEnumerable<Type> result = utilsAssembly.GetTypes<IEnumerable<string>>();

        // Assert
        Assert.IsNotNull(result);
    }

    /// <summary>
    /// 测试获取继承指定基类的所有公共具体类
    /// </summary>
    [TestMethod]
    public void GetTypes_WithBaseClass_ReturnsDerivedClasses_Test()
    {
        // Arrange
        Assembly utilsAssembly = typeof(Materal.Utils.Extensions.AssemblyExtensions).Assembly;

        // Act
        IEnumerable<Type> result = utilsAssembly.GetTypes<Exception>();

        // Assert
        Assert.IsNotNull(result);
    }

    /// <summary>
    /// 测试获取类型时排除抽象类
    /// </summary>
    [TestMethod]
    public void GetTypes_ExcludesAbstractClasses_Test()
    {
        // Arrange
        Assembly utilsAssembly = typeof(Materal.Utils.Extensions.AssemblyExtensions).Assembly;

        // Act
        IEnumerable<Type> result = utilsAssembly.GetTypes<object>();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.All(t => !t.IsAbstract));
    }

    /// <summary>
    /// 测试获取类型时只返回公共类
    /// </summary>
    [TestMethod]
    public void GetTypes_ReturnsOnlyPublicClasses_Test()
    {
        // Arrange
        Assembly utilsAssembly = typeof(Materal.Utils.Extensions.AssemblyExtensions).Assembly;

        // Act
        IEnumerable<Type> result = utilsAssembly.GetTypes<object>();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.All(t => t.IsPublic && t.IsClass));
    }

    #endregion

    #region GetDirectoryPath Tests

    /// <summary>
    /// 测试获取程序集所在文件夹路径
    /// </summary>
    [TestMethod]
    public void GetDirectoryPath_WithValidAssembly_ReturnsDirectoryPath_Test()
    {
        // Act
        string result = _testAssembly.GetDirectoryPath();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        Assert.IsTrue(Directory.Exists(result));
    }

    /// <summary>
    /// 测试获取的路径是有效的目录路径
    /// </summary>
    [TestMethod]
    public void GetDirectoryPath_ReturnsValidDirectoryPath_Test()
    {
        // Act
        string result = _testAssembly.GetDirectoryPath();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(Path.IsPathRooted(result));
        Assert.DoesNotEndWith(Path.DirectorySeparatorChar.ToString(), result);
    }

    /// <summary>
    /// 测试获取的路径包含程序集文件
    /// </summary>
    [TestMethod]
    public void GetDirectoryPath_DirectoryContainsAssemblyFile_Test()
    {
        // Act
        string directoryPath = _testAssembly.GetDirectoryPath();
        string assemblyFileName = Path.GetFileName(_testAssembly.Location);
        string expectedFilePath = Path.Combine(directoryPath, assemblyFileName);

        // Assert
        Assert.IsTrue(File.Exists(expectedFilePath));
    }

    #endregion

    #region HasCustomAttribute<T> Tests

    /// <summary>
    /// 测试检查程序集是否包含指定特性（存在的情况）
    /// </summary>
    [TestMethod]
    public void HasCustomAttribute_WhenAttributeExists_ReturnsTrue_Test()
    {
        // Arrange
        Assembly utilsAssembly = typeof(Materal.Utils.Extensions.AssemblyExtensions).Assembly;

        // Act
        bool result = utilsAssembly.HasCustomAttribute<AssemblyCompanyAttribute>();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试检查程序集是否包含指定特性（不存在的情况）
    /// </summary>
    [TestMethod]
    public void HasCustomAttribute_WhenAttributeDoesNotExist_ReturnsFalse_Test()
    {
        // Arrange
        Assembly utilsAssembly = typeof(Materal.Utils.Extensions.AssemblyExtensions).Assembly;

        // Act
        bool result = utilsAssembly.HasCustomAttribute<ObsoleteAttribute>();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试检查自定义特性
    /// </summary>
    [TestMethod]
    public void HasCustomAttribute_WithCustomAttribute_WorksCorrectly_Test()
    {
        // Arrange
        Assembly utilsAssembly = typeof(Materal.Utils.Extensions.AssemblyExtensions).Assembly;

        // Act
        bool hasTitle = utilsAssembly.HasCustomAttribute<AssemblyTitleAttribute>();
        bool hasProduct = utilsAssembly.HasCustomAttribute<AssemblyProductAttribute>();

        // Assert
        Assert.IsTrue(hasTitle || hasProduct);
    }

    #endregion
}
