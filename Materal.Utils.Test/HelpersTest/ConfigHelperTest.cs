using Materal.Utils.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Materal.Utils.Test.HelpersTest;

/// <summary>
/// ConfigHelper 测试类
/// 测试配置文件读写功能
/// </summary>
[TestClass]
public class ConfigHelperTest
{
    private string _testConfigDirectory = string.Empty;
    private string _testConfigFilePath = string.Empty;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// 测试初始化
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        // 创建临时测试目录
        _testConfigDirectory = Path.Combine(Path.GetTempPath(), $"ConfigHelperTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testConfigDirectory);
        _testConfigFilePath = Path.Combine(_testConfigDirectory, "appsettings.json");

        // 创建测试配置文件
        var testConfig = new
        {
            AppName = "TestApp",
            Version = "1.0.0",
            MaxConnections = 100,
            EnableLogging = true,
            Database = new
            {
                ConnectionString = "Server=localhost;Database=test;",
                Timeout = 30
            },
            Logging = new
            {
                LogLevel = new
                {
                    Default = "Information",
                    System = "Warning"
                }
            }
        };

        File.WriteAllText(_testConfigFilePath, JsonSerializer.Serialize(testConfig, _jsonOptions));
    }
    /// <summary>
    /// 测试清理
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        // 删除测试目录
        if (Directory.Exists(_testConfigDirectory))
        {
            Directory.Delete(_testConfigDirectory, true);
        }
    }

    #region 构造函数测试

    /// <summary>
    /// 测试构造函数 - 正常情况
    /// </summary>
    [TestMethod]
    public void Constructor_WithValidConfiguration_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();

        // Act
        var helper = new ConfigHelper(configuration);

        // Assert
        Assert.IsNotNull(helper);
    }

    /// <summary>
    /// 测试构造函数 - 配置为空时抛出异常
    /// </summary>
    [TestMethod]
    public void Constructor_WithNullConfiguration_ThrowsException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new ConfigHelper(null!));
    }

    /// <summary>
    /// 测试构造函数 - 带日志记录器
    /// </summary>
    [TestMethod]
    public void Constructor_WithLogger_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var logger = new TestLogger<ConfigHelper>();

        // Act
        var helper = new ConfigHelper(configuration, logger);

        // Assert
        Assert.IsNotNull(helper);
    }

    #endregion

    #region SetValue 字符串测试

    /// <summary>
    /// 测试设置字符串配置值 - 简单键
    /// </summary>
    [TestMethod]
    public void SetValue_String_SimpleKey_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);

        // Act
        bool result = helper.SetValue("AppName", "NewTestApp");

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试设置字符串配置值 - 嵌套键
    /// </summary>
    [TestMethod]
    public void SetValue_String_NestedKey_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);

        // Act
        bool result = helper.SetValue("Database:ConnectionString", "Server=newserver;Database=newdb;");

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试设置字符串配置值 - 空键
    /// </summary>
    [TestMethod]
    public void SetValue_String_EmptyKey_ReturnsFalse_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);

        // Act
        bool result = helper.SetValue("", "value");

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试设置字符串配置值 - null 值
    /// </summary>
    [TestMethod]
    public void SetValue_String_NullValue_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);

        // Act
        bool result = helper.SetValue("AppName", null);

        // Assert
        Assert.IsTrue(result);
    }

    #endregion

    #region SetValue 泛型测试

    /// <summary>
    /// 测试设置整数配置值
    /// </summary>
    [TestMethod]
    public void SetValue_Generic_Int_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);

        // Act
        bool result = helper.SetValue<int>("MaxConnections", 200);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试设置布尔配置值
    /// </summary>
    [TestMethod]
    public void SetValue_Generic_Bool_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);

        // Act
        bool result = helper.SetValue<bool>("EnableLogging", false);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试设置 double 配置值
    /// </summary>
    [TestMethod]
    public void SetValue_Generic_Double_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);

        // Act
        bool result = helper.SetValue<double>("Database:Timeout", 60.5);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试设置 DateTime 配置值
    /// </summary>
    [TestMethod]
    public void SetValue_Generic_DateTime_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);
        var now = DateTime.Now;

        // Act
        bool result = helper.SetValue<DateTime>("LastUpdate", now);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试设置泛型配置值 - 空键
    /// </summary>
    [TestMethod]
    public void SetValue_Generic_EmptyKey_ReturnsFalse_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);

        // Act
        bool result = helper.SetValue<int>("", 100);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region SaveChanges 测试

    /// <summary>
    /// 测试保存配置更改 - 单个值
    /// </summary>
    [TestMethod]
    public void SaveChanges_SingleValue_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);
        helper.SetValue("AppName", "UpdatedApp");

        // Act
        bool result = helper.SaveChanges();

        // Assert
        Assert.IsTrue(result);

        // 验证文件内容
        string fileContent = File.ReadAllText(_testConfigFilePath);
        Assert.Contains("UpdatedApp", fileContent);
    }

    /// <summary>
    /// 测试保存配置更改 - 多个值
    /// </summary>
    [TestMethod]
    public void SaveChanges_MultipleValues_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);
        helper.SetValue("AppName", "UpdatedApp");
        helper.SetValue<int>("MaxConnections", 500);
        helper.SetValue<bool>("EnableLogging", false);

        // Act
        bool result = helper.SaveChanges();

        // Assert
        Assert.IsTrue(result);

        // 验证文件内容
        string fileContent = File.ReadAllText(_testConfigFilePath);
        Assert.Contains("UpdatedApp", fileContent);
        Assert.Contains("500", fileContent);
        Assert.Contains("false", fileContent);
    }

    /// <summary>
    /// 测试保存配置更改 - 嵌套值
    /// </summary>
    [TestMethod]
    public void SaveChanges_NestedValue_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);
        helper.SetValue("Database:ConnectionString", "Server=updated;");
        helper.SetValue("Logging:LogLevel:Default", "Debug");

        // Act
        bool result = helper.SaveChanges();

        // Assert
        Assert.IsTrue(result);

        // 验证文件内容
        string fileContent = File.ReadAllText(_testConfigFilePath);
        Assert.Contains("Server=updated;", fileContent);
        Assert.Contains("Debug", fileContent);
    }

    /// <summary>
    /// 测试保存配置更改 - 创建新的嵌套路径
    /// </summary>
    [TestMethod]
    public void SaveChanges_NewNestedPath_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);
        helper.SetValue("NewSection:NewKey", "NewValue");

        // Act
        bool result = helper.SaveChanges();

        // Assert
        Assert.IsTrue(result);

        // 验证文件内容
        string fileContent = File.ReadAllText(_testConfigFilePath);
        Assert.Contains("NewSection", fileContent);
        Assert.Contains("NewKey", fileContent);
        Assert.Contains("NewValue", fileContent);
    }

    /// <summary>
    /// 测试保存配置更改 - 没有待保存的更改
    /// </summary>
    [TestMethod]
    public void SaveChanges_NoChanges_ReturnsTrue_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);

        // Act
        bool result = helper.SaveChanges();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试保存配置更改 - 多次保存
    /// </summary>
    [TestMethod]
    public void SaveChanges_MultipleSaves_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);

        // 第一次保存
        helper.SetValue("AppName", "FirstUpdate");
        bool result1 = helper.SaveChanges();

        // 第二次保存
        helper.SetValue("Version", "2.0.0");
        bool result2 = helper.SaveChanges();

        // Assert
        Assert.IsTrue(result1);
        Assert.IsTrue(result2);

        // 验证文件内容
        string fileContent = File.ReadAllText(_testConfigFilePath);
        Assert.Contains("FirstUpdate", fileContent);
        Assert.Contains("2.0.0", fileContent);
    }

    /// <summary>
    /// 测试保存配置更改 - 未找到配置文件时抛出异常
    /// </summary>
    [TestMethod]
    public void SaveChanges_NoConfigFile_ThrowsException_Test()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        var configuration = configBuilder.Build();
        var helper = new ConfigHelper(configuration);
        helper.SetValue("Key", "Value");

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => helper.SaveChanges());
    }

    #endregion

    #region 线程安全测试

    /// <summary>
    /// 测试并发设置配置值
    /// </summary>
    [TestMethod]
    public void SetValue_ConcurrentAccess_Success_Test()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var helper = new ConfigHelper(configuration);
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            int index = i;
            tasks.Add(Task.Run(() =>
            {
                helper.SetValue($"Key{index}", $"Value{index}");
            }, TestContext.CancellationToken));
        }

        Task.WaitAll([.. tasks], TestContext.CancellationToken);
        bool result = helper.SaveChanges();

        // Assert
        Assert.IsTrue(result);

        // 验证文件内容
        string fileContent = File.ReadAllText(_testConfigFilePath);
        for (int i = 0; i < 10; i++)
        {
            Assert.Contains($"Key{i}", fileContent);
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 构建配置对象
    /// </summary>
    private IConfigurationRoot BuildConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(_testConfigDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

        return configBuilder.Build();
    }

    #endregion

    #region 测试辅助类

    /// <summary>
    /// 测试用的日志记录器
    /// </summary>
    private class TestLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // 测试用，不需要实际记录日志
        }
    }

    public TestContext TestContext { get; set; }

    #endregion
}
