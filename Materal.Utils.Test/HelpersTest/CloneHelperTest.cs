using Materal.Utils.Helpers;

namespace Materal.Utils.Test.HelpersTest;

/// <summary>
/// CloneHelper 测试类
/// 测试对象克隆功能，包括 JSON、XML、反射和二进制序列化方式
/// </summary>
[TestClass]
public class CloneHelperTest
{
    /// <summary>
    /// 测试对象
    /// </summary>
    private class TestObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<string>? Tags { get; set; }
        public NestedObject? Nested { get; set; }
    }

    /// <summary>
    /// 嵌套测试对象
    /// </summary>
    private class NestedObject
    {
        public string? Value { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// 可序列化测试对象
    /// </summary>
    [Serializable]
    public class SerializableObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    #region CloneByJson 测试

    /// <summary>
    /// 测试使用 JSON 序列化克隆简单对象
    /// </summary>
    [TestMethod]
    public void CloneByJson_WithSimpleObject_ReturnsNewObject_Test()
    {
        // Arrange
        var original = new TestObject { Id = 1, Name = "Test" };

        // Act
        var cloned = CloneHelper.CloneByJson(original);

        // Assert
        Assert.IsNotNull(cloned);
        Assert.AreEqual(original.Id, cloned.Id);
        Assert.AreEqual(original.Name, cloned.Name);
        Assert.AreNotSame(original, cloned);
    }

    /// <summary>
    /// 测试使用 JSON 序列化克隆包含集合的对象
    /// </summary>
    [TestMethod]
    public void CloneByJson_WithCollection_ReturnsNewCollection_Test()
    {
        // Arrange
        var original = new TestObject
        {
            Id = 1,
            Tags = ["Tag1", "Tag2", "Tag3"]
        };

        // Act
        var cloned = CloneHelper.CloneByJson(original);

        // Assert
        Assert.IsNotNull(cloned);
        Assert.IsNotNull(cloned.Tags);
        Assert.HasCount(3, cloned.Tags);
        CollectionAssert.AreEqual(original.Tags, cloned.Tags);
        Assert.AreNotSame(original.Tags, cloned.Tags);
    }

    /// <summary>
    /// 测试使用 JSON 序列化克隆包含嵌套对象的对象
    /// </summary>
    [TestMethod]
    public void CloneByJson_WithNestedObject_ReturnsDeepClone_Test()
    {
        // Arrange
        var original = new TestObject
        {
            Id = 1,
            Nested = new NestedObject { Value = "Nested", Count = 5 }
        };

        // Act
        var cloned = CloneHelper.CloneByJson(original);

        // Assert
        Assert.IsNotNull(cloned);
        Assert.IsNotNull(cloned.Nested);
        Assert.AreEqual(original.Nested.Value, cloned.Nested.Value);
        Assert.AreEqual(original.Nested.Count, cloned.Nested.Count);
        Assert.AreNotSame(original.Nested, cloned.Nested);
    }

    #endregion

    #region CloneByXml 测试

    /// <summary>
    /// 测试使用 XML 序列化克隆简单对象
    /// </summary>
    [TestMethod]
    public void CloneByXml_WithSimpleObject_ReturnsNewObject_Test()
    {
        // Arrange
        var original = new SerializableObject { Id = 1, Name = "Test" };

        // Act
        var cloned = CloneHelper.CloneByXml(original);

        // Assert
        Assert.IsNotNull(cloned);
        Assert.AreEqual(original.Id, cloned.Id);
        Assert.AreEqual(original.Name, cloned.Name);
        Assert.AreNotSame(original, cloned);
    }

    #endregion

    #region CloneByReflex 测试

    /// <summary>
    /// 测试使用反射克隆简单对象
    /// </summary>
    [TestMethod]
    public void CloneByReflex_WithSimpleObject_ReturnsNewObject_Test()
    {
        // Arrange
        var original = new TestObject { Id = 1, Name = "Test" };

        // Act
        var cloned = CloneHelper.CloneByReflex(original);

        // Assert
        Assert.IsNotNull(cloned);
        Assert.AreEqual(original.Id, cloned.Id);
        Assert.AreEqual(original.Name, cloned.Name);
        Assert.AreNotSame(original, cloned);
    }

    /// <summary>
    /// 测试使用反射克隆包含嵌套对象的对象
    /// 验证反射克隆支持深度克隆
    /// </summary>
    [TestMethod]
    public void CloneByReflex_WithNestedObject_ReturnsDeepClone_Test()
    {
        // Arrange
        var original = new TestObject
        {
            Id = 1,
            Nested = new NestedObject { Value = "Nested", Count = 5 }
        };

        // Act
        var cloned = CloneHelper.CloneByReflex(original);

        // Assert
        Assert.IsNotNull(cloned);
        Assert.IsNotNull(cloned.Nested);
        Assert.AreEqual(original.Nested.Value, cloned.Nested.Value);
        Assert.AreEqual(original.Nested.Count, cloned.Nested.Count);
        Assert.AreNotSame(original.Nested, cloned.Nested);
    }

    #endregion

#if NETSTANDARD
    #region CloneBySerializable 测试

    /// <summary>
    /// 测试使用二进制序列化克隆可序列化对象
    /// </summary>
    [TestMethod]
    public void CloneBySerializable_WithSerializableObject_ReturnsNewObject_Test()
    {
        // Arrange
        var original = new SerializableObject { Id = 1, Name = "Test" };

        // Act
        var cloned = CloneHelper.CloneBySerializable(original);

        // Assert
        Assert.IsNotNull(cloned);
        Assert.AreEqual(original.Id, cloned.Id);
        Assert.AreEqual(original.Name, cloned.Name);
        Assert.AreNotSame(original, cloned);
    }

    /// <summary>
    /// 测试使用二进制序列化克隆不可序列化对象时抛出异常
    /// </summary>
    [TestMethod]
    public void CloneBySerializable_WithNonSerializableObject_ThrowsUtilException_Test()
    {
        // Arrange
        var original = new TestObject { Id = 1, Name = "Test" };

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => CloneHelper.CloneBySerializable(original));
    }

    #endregion
#endif

    #region Clone 测试

    /// <summary>
    /// 测试自动选择克隆方式克隆不可序列化对象
    /// 非NETSTANDARD环境下应使用JSON方式
    /// </summary>
    [TestMethod]
    public void Clone_WithNonSerializableObject_ReturnsNewObject_Test()
    {
        // Arrange
        var original = new TestObject { Id = 1, Name = "Test" };

        // Act
        var cloned = CloneHelper.Clone(original);

        // Assert
        Assert.IsNotNull(cloned);
        Assert.AreEqual(original.Id, cloned.Id);
        Assert.AreEqual(original.Name, cloned.Name);
        Assert.AreNotSame(original, cloned);
    }

#if NETSTANDARD
    /// <summary>
    /// 测试自动选择克隆方式克隆可序列化对象
    /// NETSTANDARD环境下应使用二进制方式
    /// </summary>
    [TestMethod]
    public void Clone_WithSerializableObject_ReturnsNewObject_Test()
    {
        // Arrange
        var original = new SerializableObject { Id = 1, Name = "Test" };

        // Act
        var cloned = CloneHelper.Clone(original);

        // Assert
        Assert.IsNotNull(cloned);
        Assert.AreEqual(original.Id, cloned.Id);
        Assert.AreEqual(original.Name, cloned.Name);
        Assert.AreNotSame(original, cloned);
    }
#endif

    #endregion

    #region CopyProperties 测试

    /// <summary>
    /// 测试属性复制到现有对象
    /// </summary>
    [TestMethod]
    public void CopyProperties_ToExistingObject_CopiesAllProperties_Test()
    {
        // Arrange
        var source = new TestObject { Id = 1, Name = "Source", Tags = ["A", "B"] };
        var target = new TestObject { Id = 0, Name = "Target", Tags = [] };

        // Act
        CloneHelper.CopyProperties(source, target);

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
        CollectionAssert.AreEqual(source.Tags, target.Tags);
    }

    /// <summary>
    /// 测试使用过滤函数复制属性
    /// </summary>
    [TestMethod]
    public void CopyProperties_WithFilterFunction_CopiesFilteredProperties_Test()
    {
        // Arrange
        var source = new TestObject { Id = 1, Name = "Source", Tags = ["A"] };
        var target = new TestObject { Id = 0, Name = "Target", Tags = [] };

        // Act
        CloneHelper.CopyProperties(source, target, propName => propName != "Name");

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreNotEqual(source.Name, target.Name);
        Assert.AreEqual("Target", target.Name);
    }

    /// <summary>
    /// 测试复制属性并返回新对象
    /// </summary>
    [TestMethod]
    public void CopyProperties_ReturnNewObject_ReturnsNewObject_Test()
    {
        // Arrange
        var source = new TestObject { Id = 1, Name = "Source", Tags = ["A"] };

        // Act
        var target = CloneHelper.CopyProperties<TestObject>(source);

        // Assert
        Assert.IsNotNull(target);
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
        Assert.AreNotSame(source, target);
    }

    /// <summary>
    /// 测试排除指定属性名称复制属性
    /// </summary>
    [TestMethod]
    public void CopyProperties_WithExcludePropertyNames_ExcludesProperties_Test()
    {
        // Arrange
        var source = new TestObject { Id = 1, Name = "Source", Tags = ["A"] };
        var target = new TestObject { Id = 0, Name = "Target", Tags = [] };

        // Act
        CloneHelper.CopyProperties(source, target, "Name");

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreNotEqual(source.Name, target.Name);
        Assert.AreEqual("Target", target.Name);
    }

    /// <summary>
    /// 测试排除指定属性名称复制属性并返回新对象
    /// </summary>
    [TestMethod]
    public void CopyProperties_WithExcludePropertyNames_ReturnNewObject_Test()
    {
        // Arrange
        var source = new TestObject { Id = 1, Name = "Source", Tags = ["A"] };

        // Act
        var target = CloneHelper.CopyProperties<TestObject>(source, "Name");

        // Assert
        Assert.IsNotNull(target);
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreNotEqual(source.Name, target.Name);
        Assert.IsNull(target.Name);
    }

    /// <summary>
    /// 测试使用空排除数组复制所有属性
    /// </summary>
    [TestMethod]
    public void CopyProperties_WithEmptyExcludeArray_CopiesAllProperties_Test()
    {
        // Arrange
        var source = new TestObject { Id = 1, Name = "Source", Tags = ["A"] };
        var target = new TestObject { Id = 0, Name = "Target", Tags = [] };

        // Act
        CloneHelper.CopyProperties(source, target, []);

        // Assert
        Assert.AreEqual(source.Id, target.Id);
        Assert.AreEqual(source.Name, target.Name);
        CollectionAssert.AreEqual(source.Tags, target.Tags);
    }

    #endregion
}
