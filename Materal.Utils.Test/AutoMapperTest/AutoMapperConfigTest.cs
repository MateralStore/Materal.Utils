namespace Materal.Utils.Test.AutoMapperTest;

/// <summary>
/// AutoMapperConfig 测试类
/// 测试 AutoMapper 配置类的基本功能
/// </summary>
[TestClass]
public class AutoMapperConfigTest
{
    private AutoMapperConfig _config = null!;

    [TestInitialize]
    public void Setup()
    {
        _config = new();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _config = null!;
        ProfileManager.Reset();
    }

    /// <summary>
    /// 测试构造方法创建配置实例
    /// </summary>
    [TestMethod]
    public void Constructor_WhenCalled_CreatesInstanceWithoutThrowing_Test()
    {
        // Act & Assert
        AutoMapperConfig config = new();
        Assert.IsNotNull(config);
    }

    /// <summary>
    /// 测试 AddMap 方法添加有效的 Profile 类型
    /// </summary>
    [TestMethod]
    public void AddMap_WithValidProfile_DoesNotThrow_Test()
    {
        // Act & Assert
        _config.AddMap(typeof(TestProfile));
    }

    /// <summary>
    /// 测试 AddMaps 方法添加多个 Profile 类型
    /// </summary>
    [TestMethod]
    public void AddMaps_WithMultipleProfiles_DoesNotThrow_Test()
    {
        // Act & Assert
        _config.AddMaps(typeof(TestProfile), typeof(TestMultipleMapsProfile));
    }

    /// <summary>
    /// 测试 AddMaps 方法使用参数数组添加 Profile 类型
    /// </summary>
    [TestMethod]
    public void AddMaps_WithArray_DoesNotThrow_Test()
    {
        // Arrange
        Type[] profiles = [typeof(TestProfile), typeof(TestReverseProfile)];

        // Act & Assert
        _config.AddMaps(profiles);
    }

    /// <summary>
    /// 测试 AddMap 方法添加非 Profile 类型时抛出异常
    /// </summary>
    [TestMethod]
    public void AddMap_WithNonProfileType_ThrowsMateralAutoMapperException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<MateralAutoMapperException>(() => _config.AddMap(typeof(NotAProfile)));
    }

    /// <summary>
    /// 测试 AddMap 方法添加 string 类型时抛出异常
    /// </summary>
    [TestMethod]
    public void AddMap_WithStringType_ThrowsMateralAutoMapperException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<MateralAutoMapperException>(() => _config.AddMap(typeof(string)));
    }

    /// <summary>
    /// 测试 AddMap 方法添加 int 类型时抛出异常
    /// </summary>
    [TestMethod]
    public void AddMap_WithIntType_ThrowsMateralAutoMapperException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<MateralAutoMapperException>(() => _config.AddMap(typeof(int)));
    }

    /// <summary>
    /// 测试 AddMap 方法添加 List 类型时抛出异常
    /// </summary>
    [TestMethod]
    public void AddMap_WithListType_ThrowsMateralAutoMapperException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<MateralAutoMapperException>(() => _config.AddMap(typeof(List<string>)));
    }
}
