namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// IServiceCollectionExtensions 测试类
/// 测试IServiceCollection扩展方法的功能
/// </summary>
[TestClass]
public class IServiceCollectionExtensionsTest
{
    #region GetSingletonInstance Tests

    /// <summary>
    /// 测试获取已注册的单例实例
    /// </summary>
    [TestMethod]
    public void GetSingletonInstance_WithRegisteredSingleton_ReturnsInstance_Test()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        TestService instance = new() { Value = "Test" };
        services.AddSingleton(instance);

        // Act
        TestService? result = services.GetSingletonInstance<TestService>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreSame(instance, result);
        Assert.AreEqual("Test", result.Value);
    }

    /// <summary>
    /// 测试获取未注册的服务返回null
    /// </summary>
    [TestMethod]
    public void GetSingletonInstance_WithUnregisteredService_ReturnsNull_Test()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act
        TestService? result = services.GetSingletonInstance<TestService>();

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试获取通过工厂注册的单例返回null
    /// </summary>
    [TestMethod]
    public void GetSingletonInstance_WithFactoryRegistration_ReturnsNull_Test()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<TestService>(_ => new TestService { Value = "Factory" });

        // Act
        TestService? result = services.GetSingletonInstance<TestService>();

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试获取通过类型注册的单例返回null
    /// </summary>
    [TestMethod]
    public void GetSingletonInstance_WithTypeRegistration_ReturnsNull_Test()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<TestService>();

        // Act
        TestService? result = services.GetSingletonInstance<TestService>();

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试获取接口类型的单例实例
    /// </summary>
    [TestMethod]
    public void GetSingletonInstance_WithInterfaceType_ReturnsInstance_Test()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        ITestService instance = new TestService { Value = "Interface" };
        services.AddSingleton(instance);

        // Act
        ITestService? result = services.GetSingletonInstance<ITestService>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreSame(instance, result);
    }

    /// <summary>
    /// 测试空服务集合返回null
    /// </summary>
    [TestMethod]
    public void GetSingletonInstance_WithEmptyServiceCollection_ReturnsNull_Test()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act
        TestService? result = services.GetSingletonInstance<TestService>();

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试获取多个注册中的第一个单例实例
    /// </summary>
    [TestMethod]
    public void GetSingletonInstance_WithMultipleRegistrations_ReturnsFirstInstance_Test()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        TestService instance1 = new() { Value = "First" };
        TestService instance2 = new() { Value = "Second" };
        services.AddSingleton(instance1);
        services.AddSingleton(instance2);

        // Act
        TestService? result = services.GetSingletonInstance<TestService>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreSame(instance1, result);
        Assert.AreEqual("First", result.Value);
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// 测试用的接口
    /// </summary>
    public interface ITestService
    {
        string Value { get; set; }
    }

    /// <summary>
    /// 测试用的服务类
    /// </summary>
    public class TestService : ITestService
    {
        public string Value { get; set; } = string.Empty;
    }

    #endregion
}
