namespace Materal.Utils.Test.AutoMapperTest;

/// <summary>
/// 测试映射关系实现
/// </summary>
public class TestMappingRelation : MappingRelation<TestSourceModel, TestTargetModel>
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public TestMappingRelation() : base((mapper, source, target) =>
    {
        target.Id = source.Id;
        target.Name = source.Name;
    }, false)
    {
    }
}

/// <summary>
/// 测试反向映射关系
/// </summary>
public class TestReverseMappingRelation : MappingRelation<TestTargetModel, TestSourceModel>
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public TestReverseMappingRelation() : base((mapper, source, target) =>
    {
        target.Id = source.Id;
        target.Name = source.Name;
    }, false)
    {
    }
}

/// <summary>
/// 测试使用默认映射的映射关系
/// </summary>
public class TestUseDefaultMapperRelation : MappingRelation<TestSourceModel, TestTargetModel>
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public TestUseDefaultMapperRelation() : base((mapper, source, target) =>
    {
        // 额外的映射逻辑
        target.Name = $"{target.Name} (mapped)";
    }, true)
    {
    }
}

/// <summary>
/// MappingRelation 测试类
/// 测试映射关系类的基本功能
/// </summary>
[TestClass]
public class MappingRelationTest
{
    /// <summary>
    /// 测试构造方法创建映射关系实例
    /// </summary>
    [TestMethod]
    public void Constructor_WhenCalled_CreatesInstanceWithoutThrowing_Test()
    {
        // Act & Assert
        TestMappingRelation relation = new();
        Assert.IsNotNull(relation);
    }

    /// <summary>
    /// 测试 SourceType 属性返回正确的源类型
    /// </summary>
    [TestMethod]
    public void SourceType_WhenCalled_ReturnsCorrectSourceType_Test()
    {
        // Arrange
        TestMappingRelation relation = new();

        // Act
        Type sourceType = relation.SourceType;

        // Assert
        Assert.AreEqual(typeof(TestSourceModel), sourceType);
    }

    /// <summary>
    /// 测试 TargetType 属性返回正确的目标类型
    /// </summary>
    [TestMethod]
    public void TargetType_WhenCalled_ReturnsCorrectTargetType_Test()
    {
        // Arrange
        TestMappingRelation relation = new();

        // Act
        Type targetType = relation.TargetType;

        // Assert
        Assert.AreEqual(typeof(TestTargetModel), targetType);
    }

    /// <summary>
    /// 测试 UseDefaultMapper 属性默认值为 false
    /// </summary>
    [TestMethod]
    public void UseDefaultMapper_WhenNotSpecified_ReturnsFalse_Test()
    {
        // Arrange
        TestMappingRelation relation = new();

        // Act
        bool useDefaultMapper = relation.UseDefaultMapper;

        // Assert
        Assert.IsFalse(useDefaultMapper);
    }

    /// <summary>
    /// 测试 UseDefaultMapper 属性可以设置为 true
    /// </summary>
    [TestMethod]
    public void UseDefaultMapper_WhenSpecifiedAsTrue_ReturnsTrue_Test()
    {
        // Arrange
        TestUseDefaultMapperRelation relation = new();

        // Act
        bool useDefaultMapper = relation.UseDefaultMapper;

        // Assert
        Assert.IsTrue(useDefaultMapper);
    }

    /// <summary>
    /// 测试 Map 属性执行自定义映射逻辑
    /// </summary>
    [TestMethod]
    public void Map_WhenCalled_ExecutesCustomMappingLogic_Test()
    {
        // Arrange
        TestMappingRelation relation = new();
        TestSourceModel source = new() { Id = 1, Name = "Test" };
        TestTargetModel target = new() { Id = 0, Name = string.Empty };
        IMapper mapper = new Mapper();

        // Act
        relation.Map(mapper, source, target);

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
    }

    /// <summary>
    /// 测试 MapObj 属性可以正确调用 Map 方法
    /// </summary>
    [TestMethod]
    public void MapObj_WhenCalled_CallsMapMethod_Test()
    {
        // Arrange
        TestMappingRelation relation = new();
        TestSourceModel source = new() { Id = 2, Name = "Test2" };
        TestTargetModel target = new() { Id = 0, Name = string.Empty };
        IMapper mapper = new Mapper();

        // Act
        relation.MapObj(mapper, source, target);

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
    }

    /// <summary>
    /// 测试反向映射关系
    /// </summary>
    [TestMethod]
    public void ReverseMappingRelation_WhenCreated_MapsFromTargetToSource_Test()
    {
        // Arrange
        TestReverseMappingRelation relation = new();
        TestTargetModel source = new() { Id = 3, Name = "Test3" };
        TestSourceModel target = new() { Id = 0, Name = string.Empty };
        IMapper mapper = new Mapper();

        // Act
        relation.Map(mapper, source, target);

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
    }

    /// <summary>
    /// 测试映射关系继承自抽象类
    /// </summary>
    [TestMethod]
    public void MappingRelation_InheritsFromAbstractClass_Test()
    {
        // Arrange
        TestMappingRelation relation = new();

        // Act & Assert
        Assert.IsInstanceOfType<MappingRelation>(relation);
    }

    /// <summary>
    /// 测试使用默认映射时也会执行自定义映射逻辑
    /// </summary>
    [TestMethod]
    public void UseDefaultMapperTrue_WhenCalled_ExecutesCustomLogicAfterDefault_Test()
    {
        // Arrange
        TestUseDefaultMapperRelation relation = new();
        TestSourceModel source = new() { Id = 4, Name = "Test4" };
        TestTargetModel target = new() { Id = 0, Name = string.Empty };
        IMapper mapper = new Mapper();

        // Act
        relation.Map(mapper, source, target);

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual($"{source.Name} (mapped)", target.Name);
    }
}
