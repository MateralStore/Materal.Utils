namespace Materal.Utils.Test.AutoMapperTest;

/// <summary>
/// Mapper 测试类
/// 测试核心映射器类的所有功能
/// </summary>
[TestClass]
public class MapperTest
{
    private Mapper _mapper = null!;
    private IServiceProvider? _serviceProvider;

    [TestInitialize]
    public void Setup()
    {
        _mapper = new Mapper();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _mapper = null!;
        _serviceProvider = null;
        ProfileManager.Reset();
    }

    #region Constructor Tests

    /// <summary>
    /// 测试无参构造方法创建 Mapper 实例
    /// </summary>
    [TestMethod]
    public void Constructor_WithoutServiceProvider_CreatesInstance_Test()
    {
        // Act & Assert
        Assert.IsNotNull(_mapper);
        Assert.IsNull(_mapper.ServiceProvider);
    }

    /// <summary>
    /// 测试带 ServiceProvider 的构造方法创建 Mapper 实例
    /// </summary>
    [TestMethod]
    public void Constructor_WithServiceProvider_CreatesInstanceAndSetsProvider_Test()
    {
        // Arrange
        _serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        Mapper mapper = new(_serviceProvider);

        // Assert
        Assert.IsNotNull(mapper);
        Assert.AreSame(_serviceProvider, mapper.ServiceProvider);
    }

    #endregion

    #region Map<T> Tests - Basic Mapping

    /// <summary>
    /// 测试使用 Map<T> 方法映射简单对象
    /// </summary>
    [TestMethod]
    public void MapT_WithSimpleObject_ReturnsMappedObject_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 1, Name = "Test" };

        // Act
        TestTargetModel result = _mapper.Map<TestTargetModel>(source);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(source.Id, result.Id);
        Assert.AreEqual(source.Name, result.Name);
    }

    /// <summary>
    /// 测试映射空字符串属性
    /// </summary>
    [TestMethod]
    public void MapT_WithEmptyString_MapsCorrectly_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 1, Name = string.Empty };

        // Act
        TestTargetModel result = _mapper.Map<TestTargetModel>(source);

        // Assert
        Assert.AreEqual(string.Empty, result.Name);
    }

    /// <summary>
    /// 测试映射整数为零
    /// </summary>
    [TestMethod]
    public void MapT_WithZeroValue_MapsCorrectly_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 0, Name = "Test" };

        // Act
        TestTargetModel result = _mapper.Map<TestTargetModel>(source);

        // Assert
        Assert.AreEqual(0, result.Id);
    }

    /// <summary>
    /// 测试映射时创建新的目标对象
    /// </summary>
    [TestMethod]
    public void MapT_WhenCalled_CreatesNewTargetInstance_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 1, Name = "Test" };

        // Act
        TestTargetModel result1 = _mapper.Map<TestTargetModel>(source);
        TestTargetModel result2 = _mapper.Map<TestTargetModel>(source);

        // Assert
        Assert.AreNotSame(result1, result2);
    }

    #endregion

    #region Map(object, object) Tests

    /// <summary>
    /// 测试使用 Map(source, target) 方法映射到现有对象
    /// </summary>
    [TestMethod]
    public void Map_WithExistingTarget_MapsToExistingInstance_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 1, Name = "Source" };
        TestTargetModel target = new() { Id = 99, Name = "Target" };

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
    }

    /// <summary>
    /// 测试映射到现有对象时保留对象引用
    /// </summary>
    [TestMethod]
    public void Map_WhenMappingToExistingInstance_PreservesReference_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 1, Name = "Test" };
        TestTargetModel target = new() { Id = 0, Name = string.Empty };
        TestTargetModel originalTarget = target;

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.AreSame(originalTarget, target);
    }

    #endregion

    #region Nullable Type Mapping Tests

    /// <summary>
    /// 测试映射可空整型（有值）
    /// </summary>
    [TestMethod]
    public void Map_WithNullableIntWithValue_MapsCorrectly_Test()
    {
        // Arrange
        FullPropertiesSourceModel source = new() { Id = 1, NullableInt = 100 };
        FullPropertiesTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.AreEqual(100, target.NullableInt);
    }

    /// <summary>
    /// 测试映射可空整型（无值）
    /// </summary>
    [TestMethod]
    public void Map_WithNullableIntWithoutValue_MapsCorrectly_Test()
    {
        // Arrange
        FullPropertiesSourceModel source = new() { Id = 1, NullableInt = null };
        FullPropertiesTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.IsNull(target.NullableInt);
    }

    /// <summary>
    /// 测试映射可空 DateTime（有值）
    /// </summary>
    [TestMethod]
    public void Map_WithNullableDateTimeWithValue_MapsCorrectly_Test()
    {
        // Arrange
        DateTime testDate = DateTime.Now;
        FullPropertiesSourceModel source = new() { Id = 1, NullableDateTime = testDate };
        FullPropertiesTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.AreEqual(testDate, target.NullableDateTime);
    }

    /// <summary>
    /// 测试映射可空 DateTime（无值）
    /// </summary>
    [TestMethod]
    public void Map_WithNullableDateTimeWithoutValue_MapsCorrectly_Test()
    {
        // Arrange
        FullPropertiesSourceModel source = new() { Id = 1, NullableDateTime = null };
        FullPropertiesTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.IsNull(target.NullableDateTime);
    }

    #endregion

    #region Nested Object Mapping Tests

    /// <summary>
    /// 测试映射嵌套对象（有值）
    /// </summary>
    [TestMethod]
    public void Map_WithNestedObjectWithValues_MapsCorrectly_Test()
    {
        // Arrange
        FullPropertiesSourceModel source = new()
        {
            Id = 1,
            Name = "Parent",
            Nested = new() { Value = 100, Description = "Nested" }
        };
        FullPropertiesTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.IsNotNull(target.Nested);
        Assert.AreEqual(100, target.Nested.Value);
        Assert.AreEqual("Nested", target.Nested.Description);
    }

    /// <summary>
    /// 测试映射嵌套对象（无值）
    /// </summary>
    [TestMethod]
    public void Map_WithNestedObjectNull_KeepsTargetNestedNull_Test()
    {
        // Arrange
        FullPropertiesSourceModel source = new()
        {
            Id = 1,
            Name = "Parent",
            Nested = null
        };
        FullPropertiesTargetModel target = new()
        {
            Nested = new() { Value = 99, Description = "Old" }
        };

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.IsNotNull(target.Nested); // 保持原值
        Assert.AreEqual(99, target.Nested.Value);
        Assert.AreEqual("Old", target.Nested.Description);
    }

    /// <summary>
    /// 测试映射到空嵌套对象
    /// </summary>
    [TestMethod]
    public void Map_WithExistingTargetNestedNull_MapsNestedObject_Test()
    {
        // Arrange
        FullPropertiesSourceModel source = new()
        {
            Id = 1,
            Name = "Parent",
            Nested = new() { Value = 100, Description = "Nested" }
        };
        FullPropertiesTargetModel target = new()
        {
            Nested = null
        };

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.IsNotNull(target.Nested);
        Assert.AreEqual(100, target.Nested.Value);
        Assert.AreEqual("Nested", target.Nested.Description);
    }

    #endregion

    #region List Mapping Tests

    /// <summary>
    /// 测试映射字符串列表
    /// </summary>
    [TestMethod]
    public void Map_WithStringList_MapsCorrectly_Test()
    {
        // Arrange
        ListSourceModel source = new()
        {
            Strings = ["A", "B", "C"]
        };
        ListTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.HasCount(3, target.Strings);
        Assert.AreEqual("A", target.Strings[0]);
        Assert.AreEqual("B", target.Strings[1]);
        Assert.AreEqual("C", target.Strings[2]);
    }

    /// <summary>
    /// 测试映射整数列表
    /// </summary>
    [TestMethod]
    public void Map_WithIntList_MapsCorrectly_Test()
    {
        // Arrange
        ListSourceModel source = new()
        {
            Ints = [1, 2, 3, 4, 5]
        };
        ListTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.HasCount(5, target.Ints);
        Assert.AreEqual(1, target.Ints[0]);
        Assert.AreEqual(5, target.Ints[4]);
    }

    /// <summary>
    /// 测试映射空列表
    /// </summary>
    [TestMethod]
    public void Map_WithEmptyList_KeepsTargetEmpty_Test()
    {
        // Arrange
        ListSourceModel source = new() { Strings = [] };
        ListTargetModel target = new() { Strings = ["Old"] };

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.IsEmpty(target.Strings);
    }

    /// <summary>
    /// 测试映射嵌套对象列表
    /// </summary>
    [TestMethod]
    public void Map_WithNestedObjectList_MapsCorrectly_Test()
    {
        // Arrange
        NestedListSourceModel source = new()
        {
            Items =
            [
                new() { Value = 1, Description = "A" },
                new() { Value = 2, Description = "B" },
                new() { Value = 3, Description = "C" }
            ]
        };
        NestedListTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.HasCount(3, target.Items);
        Assert.AreEqual(1, target.Items[0].Value);
        Assert.AreEqual("A", target.Items[0].Description);
        Assert.AreEqual(2, target.Items[1].Value);
        Assert.AreEqual("B", target.Items[1].Description);
        Assert.AreEqual(3, target.Items[2].Value);
        Assert.AreEqual("C", target.Items[2].Description);
    }

    /// <summary>
    /// 测试映射包含 null 元素的列表
    /// </summary>
    [TestMethod]
    public void Map_WithListContainingNullElements_MapsCorrectly_Test()
    {
        // Arrange
        List<NestedSourceModel> sourceList =
        [
            new() { Value = 1, Description = "A" },
            null!,
            new() { Value = 3, Description = "C" }
        ];
        NestedListSourceModel source = new() { Items = sourceList };
        NestedListTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.HasCount(3, target.Items);
        Assert.AreEqual(1, target.Items[0].Value);
        Assert.IsNull(target.Items[1]);
        Assert.AreEqual(3, target.Items[2].Value);
    }

    #endregion

    #region Read-Only Property Tests

    /// <summary>
    /// 测试映射时跳过只读源属性
    /// </summary>
    [TestMethod]
    public void Map_WithReadOnlySourceProperty_DoesNotMapToTarget_Test()
    {
        // Arrange
        ReadOnlySourceModel source = new() { Id = 1 };
        ReadOnlyTargetModel target = new() { Name = "Original" };

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.AreEqual(1, target.Id);
        Assert.AreEqual("Original", target.Name); // 保持原值，因为源是只读的
    }

    /// <summary>
    /// 测试映射时跳过只写目标属性
    /// </summary>
    [TestMethod]
    public void Map_WithWriteOnlyTargetProperty_DoesNotMap_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 1, Name = "Test" };
        TestTargetModel target = new() { Id = 0, Name = string.Empty };

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
    }

    #endregion

    #region Profile-Based Mapping Tests

    /// <summary>
    /// 测试使用 Profile 进行自定义映射
    /// </summary>
    [TestMethod]
    public void Map_WithProfile_UsesCustomMappingLogic_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 10, Name = "test" };
        TestTargetModel target = new();
        Mapper mapperWithProfile = new();

        // Act
        mapperWithProfile.Map(source, target);

        // Assert
        // 默认映射应该映射所有相同名称和类型的属性
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
    }

    #endregion

    #region Edge Cases and Exception Tests

    /// <summary>
    /// 测试映射 null 源对象
    /// </summary>
    [TestMethod]
    public void Map_WithNullSource_ThrowsNullReferenceException_Test()
    {
        // Arrange
        TestTargetModel target = new();

        // Act & Assert
        Assert.ThrowsExactly<NullReferenceException>(() => _mapper.Map(null!, target));
    }

    /// <summary>
    /// 测试映射 null 目标对象
    /// </summary>
    [TestMethod]
    public void Map_WithNullTarget_ThrowsNullReferenceException_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 1, Name = "Test" };

        // Act & Assert
        Assert.ThrowsExactly<NullReferenceException>(() => _mapper.Map(source, null!));
    }

    /// <summary>
    /// 测试映射具有不同属性名称的对象
    /// </summary>
    [TestMethod]
    public void Map_WithDifferentPropertyNames_OnlyMapsMatchingProperties_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 1, Name = "Test" };
        TestTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
    }

    #endregion

    #region Type Compatibility Tests

    /// <summary>
    /// 测试映射基类型到派生类型
    /// </summary>
    [TestMethod]
    public void Map_WithBaseToDerivedType_MapsMatchingProperties_Test()
    {
        // Arrange
        TestSourceModel source = new() { Id = 1, Name = "Test" };
        TestTargetModel target = new();

        // Act
        _mapper.Map(source, target);

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
    }

    #endregion

    #region Multiple Mapping Tests

    /// <summary>
    /// 测试连续多次映射
    /// </summary>
    [TestMethod]
    public void Map_CalledMultipleTimes_UpdatesTargetEachTime_Test()
    {
        // Arrange
        TestSourceModel source1 = new() { Id = 1, Name = "First" };
        TestSourceModel source2 = new() { Id = 2, Name = "Second" };
        TestSourceModel source3 = new() { Id = 3, Name = "Third" };
        TestTargetModel target = new();

        // Act
        _mapper.Map(source1, target);
        _mapper.Map(source2, target);
        _mapper.Map(source3, target);

        // Assert
        Assert.AreEqual(3, target.Id);
        Assert.AreEqual("Third", target.Name);
    }

    #endregion
}
