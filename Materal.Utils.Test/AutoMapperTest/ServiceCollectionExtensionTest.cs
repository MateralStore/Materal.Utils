using Materal.Utils.AutoMapper.Extensions;

namespace Materal.Utils.Test.AutoMapperTest;

/// <summary>
/// ServiceCollectionExtension 测试类
/// 测试依赖注入扩展方法
/// </summary>
[TestClass]
public class ServiceCollectionExtensionTest
{
    private IServiceCollection _services = null!;
    private IServiceProvider? _serviceProvider;

    [TestInitialize]
    public void Setup()
    {
        _services = new ServiceCollection();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _services = null!;
        _serviceProvider = null;
        ProfileManager.Reset();            
    }

    #region AddAutoMapper Tests

    /// <summary>
    /// 测试 AddAutoMapper 方法添加 IMapper 单例服务
    /// </summary>
    [TestMethod]
    public void AddAutoMapper_WhenCalled_AddsIMapperAsSingleton_Test()
    {
        // Act
        IServiceCollection result = _services.AddAutoMapper();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreSame(_services, result);

        ServiceDescriptor? descriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IMapper));
        Assert.IsNotNull(descriptor);
        Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.AreEqual(typeof(Mapper), descriptor.ImplementationType);
    }

    /// <summary>
    /// 测试 AddAutoMapper 方法使用默认配置
    /// </summary>
    [TestMethod]
    public void AddAutoMapper_WithoutConfigAction_UsesDefaultConfig_Test()
    {
        // Act
        _services.AddAutoMapper();

        // Assert
        ServiceDescriptor? descriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IMapper));
        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(Mapper), descriptor.ImplementationType);
    }

    /// <summary>
    /// 测试 AddAutoMapper 方法使用自定义配置
    /// </summary>
    [TestMethod]
    public void AddAutoMapper_WithConfigAction_UsesCustomConfig_Test()
    {
        // Arrange
        Type testProfileType = typeof(TestMapperProfile);

        // Act
        _services.AddAutoMapper(config =>
        {
            config.AddMaps(testProfileType);
        });

        // Assert
        ServiceDescriptor? descriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IMapper));
        Assert.IsNotNull(descriptor);
    }

    /// <summary>
    /// 测试 AddAutoMapper 方法可以多次调用而不重复添加
    /// </summary>
    [TestMethod]
    public void AddAutoMapper_CalledMultipleTimes_AddsMapperOnce_Test()
    {
        // Act
        _services.AddAutoMapper();
        _services.AddAutoMapper();

        // Assert
        int count = _services.Count(d => d.ServiceType == typeof(IMapper));
        Assert.AreEqual(1, count);
    }

    /// <summary>
    /// 测试 AddAutoMapper 方法添加的服务可以被解析
    /// </summary>
    [TestMethod]
    public void AddAutoMapper_ServiceCanBeResolved_Test()
    {
        // Arrange
        _services.AddAutoMapper();

        // Act
        _serviceProvider = _services.BuildServiceProvider();
        IMapper? mapper = _serviceProvider.GetService<IMapper>();

        // Assert
        Assert.IsNotNull(mapper);
        Assert.IsInstanceOfType<Mapper>(mapper);
        Assert.IsNotNull(mapper.ServiceProvider);
    }

    /// <summary>
    /// 测试 AddAutoMapper 方法解析的服务是单例
    /// </summary>
    [TestMethod]
    public void AddAutoMapper_ResolvedServiceIsSingleton_Test()
    {
        // Arrange
        _services.AddAutoMapper();
        _serviceProvider = _services.BuildServiceProvider();

        // Act
        IMapper mapper1 = _serviceProvider.GetRequiredService<IMapper>();
        IMapper mapper2 = _serviceProvider.GetRequiredService<IMapper>();

        // Assert
        Assert.AreSame(mapper1, mapper2);
    }

    /// <summary>
    /// 测试 AddAutoMapper 方法配置 null 时正常工作
    /// </summary>
    [TestMethod]
    public void AddAutoMapper_WithNullConfigAction_WorksWithoutError_Test()
    {
        // Act & Assert
        IServiceCollection result = _services.AddAutoMapper(null!);
        Assert.AreSame(_services, result);
    }

    #endregion

    #region UseAutoMapper Tests

    /// <summary>
    /// 测试 UseAutoMapper 方法初始化
    /// </summary>
    [TestMethod]
    public void UseAutoMapper_WhenCalled_ReturnsServiceProvider_Test()
    {
        // Arrange
        _services.AddAutoMapper(config =>
        {
            config.AddMaps(typeof(TestMapperProfile));
        });
        _serviceProvider = _services.BuildServiceProvider();

        // Act
        IServiceProvider result = _serviceProvider.UseAutoMapper();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreSame(_serviceProvider, result);
    }

    /// <summary>
    /// 测试 UseAutoMapper 后可以正常映射
    /// </summary>
    [TestMethod]
    public void UseAutoMapper_AfterInitialization_CanMapObjects_Test()
    {
        // Arrange
        _services.AddAutoMapper(config =>
        {
            config.AddMaps(typeof(TestMapperProfile));
        });
        _serviceProvider = _services.BuildServiceProvider();
        _serviceProvider.UseAutoMapper();
        IMapper mapper = _serviceProvider.GetRequiredService<IMapper>();
        TestSourceModel source = new() { Id = 1, Name = "Test" };

        // Act
        TestTargetModel result = mapper.Map<TestTargetModel>(source);

        // Assert
        Assert.AreEqual(source.Id, result.Id);
        Assert.AreEqual(source.Name, result.Name);
    }

    /// <summary>
    /// 测试 UseAutoMapper 返回相同的 ServiceProvider
    /// </summary>
    [TestMethod]
    public void UseAutoMapper_ReturnsSameServiceProvider_Test()
    {
        // Arrange
        _services.AddAutoMapper();
        _serviceProvider = _services.BuildServiceProvider();
        IServiceProvider original = _serviceProvider;

        // Act
        IServiceProvider result = _serviceProvider.UseAutoMapper();

        // Assert
        Assert.AreSame(original, result);
    }

    #endregion

    #region Integration Tests

    /// <summary>
    /// 测试完整的 DI 集成流程
    /// </summary>
    [TestMethod]
    public void Integration_AddAutoMapperAndUseAutoMapper_WorksEndToEnd_Test()
    {
        // Arrange
        _services.AddAutoMapper(config =>
        {
            config.AddMaps(typeof(TestMapperProfile));
        });
        _serviceProvider = _services.BuildServiceProvider();
        _serviceProvider.UseAutoMapper();

        // Act
        IMapper mapper = _serviceProvider.GetRequiredService<IMapper>();
        TestSourceModel source = new() { Id = 100, Name = "Test" };
        TestTargetModel result = mapper.Map<TestTargetModel>(source);

        // Assert
        Assert.IsNotNull(mapper);
        Assert.IsNotNull(result);
        Assert.AreEqual(source.Id, result.Id);
        Assert.AreEqual(source.Name, result.Name);
    }

    /// <summary>
    /// 测试在服务中使用 IMapper
    /// </summary>
    [TestMethod]
    public void Integration_ServiceUsingAutoMapper_CanMapObjects_Test()
    {
        // Arrange
        _services.AddAutoMapper();
        _services.AddScoped<TestService>();
        _serviceProvider = _services.BuildServiceProvider();
        _serviceProvider.UseAutoMapper();

        // Act
        using IServiceScope scope = _serviceProvider.CreateScope();
        TestService service = scope.ServiceProvider.GetRequiredService<TestService>();
        TestTargetModel result = service.MapTest(new TestSourceModel { Id = 1, Name = "Test" });

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Test", result.Name);
    }

    /// <summary>
    /// 测试在不同作用域中使用 Mapper
    /// </summary>
    [TestMethod]
    public void Integration_MapperInDifferentScopes_IsSingleton_Test()
    {
        // Arrange
        _services.AddAutoMapper();
        _serviceProvider = _services.BuildServiceProvider();
        _serviceProvider.UseAutoMapper();

        // Act
        using IServiceScope scope1 = _serviceProvider.CreateScope();
        using IServiceScope scope2 = _serviceProvider.CreateScope();
        IMapper mapper1 = scope1.ServiceProvider.GetRequiredService<IMapper>();
        IMapper mapper2 = scope2.ServiceProvider.GetRequiredService<IMapper>();

        // Assert
        Assert.AreSame(mapper1, mapper2); // 单例，所有作用域共享同一个实例
    }

    #endregion

    #region Edge Cases

    /// <summary>
    /// 测试不调用 UseAutoMapper 也能映射（默认映射）
    /// </summary>
    [TestMethod]
    public void WithoutUseAutoMapper_CanStillMapWithDefaultBehavior_Test()
    {
        // Arrange
        _services.AddAutoMapper();
        _serviceProvider = _services.BuildServiceProvider();
        IMapper mapper = _serviceProvider.GetRequiredService<IMapper>();
        TestSourceModel source = new() { Id = 1, Name = "Test" };

        // Act
        TestTargetModel result = mapper.Map<TestTargetModel>(source);

        // Assert
        Assert.AreEqual(source.Id, result.Id);
        Assert.AreEqual(source.Name, result.Name);
    }

    #endregion
}

#region Test Service

/// <summary>
/// 测试服务类
/// </summary>
/// <remarks>
/// 构造方法
/// </remarks>
public class TestService(IMapper mapper)
{

    /// <summary>
    /// 映射测试方法
    /// </summary>
    public TestTargetModel MapTest(TestSourceModel source)
    {
        return mapper.Map<TestTargetModel>(source);
    }
}

#endregion
