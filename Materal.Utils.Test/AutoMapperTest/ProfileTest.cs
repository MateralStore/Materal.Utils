namespace Materal.Utils.Test.AutoMapperTest;

/// <summary>
/// Profile 测试类
/// 测试配置文件类的基本功能
/// </summary>
[TestClass]
public class ProfileTest
{
    /// <summary>
    /// 测试构造方法创建配置文件实例
    /// </summary>
    [TestMethod]
    public void Constructor_WhenCalled_CreatesInstanceWithoutThrowing_Test()
    {
        // Act & Assert
        TestProfile profile = new();
        Assert.IsNotNull(profile);
    }

    /// <summary>
    /// 测试带有反向映射的 Profile 构造方法
    /// </summary>
    [TestMethod]
    public void Constructor_WithReverseMap_CreatesInstanceWithoutThrowing_Test()
    {
        // Act & Assert
        TestReverseProfile profile = new();
        Assert.IsNotNull(profile);
    }

    /// <summary>
    /// 测试多映射 Profile 构造方法
    /// </summary>
    [TestMethod]
    public void Constructor_WithMultipleMaps_CreatesInstanceWithoutThrowing_Test()
    {
        // Act & Assert
        TestMultipleMapsProfile profile = new();
        Assert.IsNotNull(profile);
    }

    /// <summary>
    /// 测试使用默认映射的 Profile 构造方法
    /// </summary>
    [TestMethod]
    public void Constructor_WithUseDefaultMapper_CreatesInstanceWithoutThrowing_Test()
    {
        // Act & Assert
        TestUseDefaultMapperProfile profile = new();
        Assert.IsNotNull(profile);
    }

    /// <summary>
    /// 测试 Profile 继承自抽象类
    /// </summary>
    [TestMethod]
    public void Profile_InheritsFromAbstractClass_Test()
    {
        // Arrange
        TestProfile profile = new();

        // Act & Assert
        Assert.IsInstanceOfType<Profile>(profile);
    }

    /// <summary>
    /// 测试 Profile 可以通过 Mapper 使用
    /// </summary>
    [TestMethod]
    public void Profile_CanBeUsedWithMapper_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 1, Name = "Test" };
        Mapper mapper = new();

        // Act
        TestTargetModel result = mapper.Map<TestTargetModel>(source);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(source.Id, result.Id);
        Assert.AreEqual(source.Name, result.Name);
    }

    /// <summary>
    /// 测试创建多个 Profile 实例
    /// </summary>
    [TestMethod]
    public void MultipleProfiles_CanBeCreatedWithoutThrowing_Test()
    {
        // Act & Assert
        TestProfile profile1 = new();
        TestProfile profile2 = new();
        Assert.AreNotSame(profile1, profile2);
    }
}
