using Microsoft.Extensions.Configuration;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// ConfigurationExtensions 测试类
/// 测试配置对象扩展方法的功能
/// </summary>
[TestClass]
public class ConfigurationExtensionsTest
{
    #region Get<T> Tests

    /// <summary>
    /// 测试获取字符串类型的配置值
    /// </summary>
    [TestMethod]
    public void Get_WithStringType_ReturnsStringValue_Test()
    {
        // Arrange
        Dictionary<string, string?> configData = new()
        {
            { "TestKey", "TestValue" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        string? result = configuration.GetConfigItem<string>("TestKey");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestValue", result);
    }

    /// <summary>
    /// 测试获取整数类型的配置值
    /// </summary>
    [TestMethod]
    public void Get_WithIntType_ReturnsIntValue_Test()
    {
        // Arrange
        Dictionary<string, string?> configData = new()
        {
            { "TestKey", "123" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        int? result = configuration.GetConfigItem<int>("TestKey");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(123, result);
    }

    /// <summary>
    /// 测试获取布尔类型的配置值
    /// </summary>
    [TestMethod]
    public void Get_WithBoolType_ReturnsBoolValue_Test()
    {
        // Arrange
        Dictionary<string, string?> configData = new()
        {
            { "TestKey", "true" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        bool? result = configuration.GetConfigItem<bool>("TestKey");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Value);
    }

    /// <summary>
    /// 测试获取不存在的配置键返回默认值
    /// </summary>
    [TestMethod]
    public void Get_WithNonExistentKey_ReturnsDefault_Test()
    {
        // Arrange
        Dictionary<string, string?> configData = [];
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        string? result = configuration.GetConfigItem<string>("NonExistentKey");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试获取复杂对象类型的配置值
    /// </summary>
    [TestMethod]
    public void Get_WithComplexType_ReturnsObject_Test()
    {
        // Arrange
        Dictionary<string, string?> configData = new()
        {
            { "TestSection:Name", "TestName" },
            { "TestSection:Value", "100" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        TestConfig? result = configuration.GetConfigItem<TestConfig>("TestSection");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestName", result.Name);
        Assert.AreEqual(100, result.Value);
    }

    /// <summary>
    /// 测试null配置对象抛出异常
    /// </summary>
    [TestMethod]
    public void Get_WithNullConfiguration_ThrowsArgumentNullException_Test()
    {
        // Arrange
        IConfiguration configuration = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => configuration.GetConfigItem<string>("TestKey"));
    }

    /// <summary>
    /// 测试null键抛出异常
    /// </summary>
    [TestMethod]
    public void Get_WithNullKey_ThrowsArgumentNullException_Test()
    {
        // Arrange
        Dictionary<string, string?> configData = [];
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => configuration.GetConfigItem<string>(null!));
    }

    /// <summary>
    /// 测试获取空字符串配置值
    /// </summary>
    [TestMethod]
    public void Get_WithEmptyStringValue_ReturnsEmptyOrNull_Test()
    {
        // Arrange
        Dictionary<string, string?> configData = new()
        {
            { "TestKey", "" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        string? result = configuration.GetConfigItem<string>("TestKey");

        // Assert - 空字符串可能被处理为null或空字符串
        Assert.IsTrue(string.IsNullOrEmpty(result));
    }

    /// <summary>
    /// 测试获取数组类型的配置值
    /// </summary>
    [TestMethod]
    public void Get_WithArrayType_ReturnsArray_Test()
    {
        // Arrange
        Dictionary<string, string?> configData = new()
        {
            { "TestArray:0", "Value1" },
            { "TestArray:1", "Value2" },
            { "TestArray:2", "Value3" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        string[]? result = configuration.GetConfigItem<string[]>("TestArray");

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(3, result);
        Assert.AreEqual("Value1", result[0]);
        Assert.AreEqual("Value2", result[1]);
        Assert.AreEqual("Value3", result[2]);
    }

    /// <summary>
    /// 测试获取嵌套配置对象
    /// </summary>
    [TestMethod]
    public void Get_WithNestedConfig_ReturnsObject_Test()
    {
        // Arrange
        Dictionary<string, string?> configData = new()
        {
            { "TestSection:Nested:Id", "1" },
            { "TestSection:Nested:Name", "NestedName" },
            { "TestSection:Value", "100" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        TestConfig? result = configuration.GetConfigItem<TestConfig>("TestSection");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(100, result.Value);
    }

    /// <summary>
    /// 测试获取包含特殊字符的配置值
    /// </summary>
    [TestMethod]
    public void Get_WithSpecialCharacters_ReturnsValue_Test()
    {
        // Arrange
        Dictionary<string, string?> configData = new()
        {
            { "TestKey", "Test<>\"'&Value" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        // Act
        string? result = configuration.GetConfigItem<string>("TestKey");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test<>\"'&Value", result);
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// 测试用的配置类
    /// </summary>
    private class TestConfig
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    #endregion
}
