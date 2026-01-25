using Materal.Utils.Helpers;

namespace Materal.Utils.Test.HelpersTest;

/// <summary>
/// TypeHelper 测试类
/// 测试类型查找和实例化功能
/// </summary>
[TestClass]
public class TypeHelperTest
{
    /// <summary>
    /// 测试对象
    /// </summary>
    private class TestClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    /// <summary>
    /// 抽象测试类
    /// </summary>
    private abstract class AbstractTestClass
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// 测试接口
    /// </summary>
    private interface ITestInterface { }

    /// <summary>
    /// 测试接口实现
    /// </summary>
    private class TestImplementation : ITestInterface
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// 带参数的测试类
    /// </summary>
    private class TestClassWithConstructor(int id, string? name)
    {
        public int Id { get; } = id;
        public string? Name { get; } = name;
    }

    /// <summary>
    /// 测试派生类
    /// </summary>
    private class DerivedTestClass : TestClass
    {
        public string? ExtraProperty { get; set; }
    }

    #region GetTypeByFilter Tests

    /// <summary>
    /// 测试通过过滤器获取类型
    /// </summary>
    [TestMethod]
    public void GetTypeByFilter_WithMatchingFilter_ReturnsFirstMatch_Test()
    {
        // Arrange
        static bool filter(Type m) => m.Name == "TestClass" && m.IsClass && !m.IsAbstract;

        // Act
        Type? result = TypeHelper.GetTypeByFilter(filter);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestClass", result.Name);
    }

    /// <summary>
    /// 测试通过过滤器获取类型 - 无匹配时返回 null
    /// </summary>
    [TestMethod]
    public void GetTypeByFilter_WithNoMatchingFilter_ReturnsNull_Test()
    {
        // Arrange
        static bool filter(Type m) => m.Name == "NonExistentClass";

        // Act
        Type? result = TypeHelper.GetTypeByFilter(filter);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试通过过滤器和构造函数参数类型获取类型
    /// </summary>
    [TestMethod]
    public void GetTypeByFilter_WithArgTypes_ReturnsTypeWithMatchingConstructor_Test()
    {
        // Arrange
        Type[] argTypes = [typeof(int), typeof(string)];
        static bool filter(Type m) => m.Name == "TestClassWithConstructor";

        // Act
        Type? result = TypeHelper.GetTypeByFilter(filter, argTypes);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestClassWithConstructor", result.Name);
    }

    /// <summary>
    /// 测试通过过滤器和构造函数参数类型获取类型 - 无匹配构造函数时返回 null
    /// </summary>
    [TestMethod]
    public void GetTypeByFilter_WithArgTypesNoMatchingConstructor_ReturnsNull_Test()
    {
        // Arrange
        Type[] argTypes = [typeof(int), typeof(string), typeof(bool)];
        static bool filter(Type m) => m.Name == "TestClassWithConstructor";

        // Act
        Type? result = TypeHelper.GetTypeByFilter(filter, argTypes);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region GetObjectByTypeFullName Tests

    /// <summary>
    /// 测试通过完整类型名称获取对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeFullName_WithValidTypeName_ReturnsObject_Test()
    {
        // Arrange
        string typeName = "Materal.Utils.Test.HelpersTest.TypeHelperTest+TestClass";

        // Act
        object? result = TypeHelper.GetObjectByTypeFullName(typeName);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TestClass>(result);
    }

    /// <summary>
    /// 测试通过无效类型名称获取对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeFullName_WithInvalidTypeName_ReturnsNull_Test()
    {
        // Arrange
        string typeName = "Invalid.Namespace.NonExistentClass";

        // Act
        object? result = TypeHelper.GetObjectByTypeFullName(typeName);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region GetObjectByTypeName Tests

    /// <summary>
    /// 测试通过简单类型名称获取对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeName_WithValidTypeName_ReturnsObject_Test()
    {
        // Arrange
        string typeName = "TestClass";

        // Act
        object? result = TypeHelper.GetObjectByTypeName(typeName);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TestClass>(result);
    }

    /// <summary>
    /// 测试通过简单类型名称和参数获取对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeName_WithValidTypeNameAndArgs_ReturnsObject_Test()
    {
        // Arrange
        string typeName = "TestClassWithConstructor";
        object[] args = [5, "TestName"];

        // Act
        object? result = TypeHelper.GetObjectByTypeName(typeName, args);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TestClassWithConstructor>(result);
        var typedResult = (TestClassWithConstructor)result;
        Assert.AreEqual(5, typedResult.Id);
        Assert.AreEqual("TestName", typedResult.Name);
    }

    #endregion

    #region GetObjectByTypeName<T> Tests

    /// <summary>
    /// 测试通过简单类型名称获取强类型对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeNameGeneric_WithValidTypeName_ReturnsTypedObject_Test()
    {
        // Arrange
        string typeName = "TestImplementation";

        // Act
        ITestInterface? result = TypeHelper.GetObjectByTypeName<ITestInterface>(typeName);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TestImplementation>(result);
    }

    /// <summary>
    /// 测试通过简单类型名称和参数获取强类型对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeNameGeneric_WithArgs_ReturnsTypedObject_Test()
    {
        // Arrange
        string typeName = "TestClassWithConstructor";
        object[] args = [5, "TestName"];

        // Act
        TestClassWithConstructor? result = TypeHelper.GetObjectByTypeName<TestClassWithConstructor>(typeName, args);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Id);
        Assert.AreEqual("TestName", result.Name);
    }

    #endregion

    #region GetObjectByTypeFullName<T> Tests

    /// <summary>
    /// 测试通过完整类型名称获取强类型对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeFullNameGeneric_WithValidTypeName_ReturnsTypedObject_Test()
    {
        // Arrange
        string typeName = "Materal.Utils.Test.HelpersTest.TypeHelperTest+TestClass";

        // Act
        TestClass? result = TypeHelper.GetObjectByTypeFullName<TestClass>(typeName);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TestClass>(result);
    }

    /// <summary>
    /// 测试通过无效类型名称获取强类型对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeFullNameGeneric_WithInvalidTypeName_ReturnsNull_Test()
    {
        // Arrange
        string typeName = "Invalid.Namespace.NonExistentClass";

        // Act
        TestClass? result = TypeHelper.GetObjectByTypeFullName<TestClass>(typeName);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region GetObjectByTypeFullName with Arguments Tests

    /// <summary>
    /// 测试通过完整类型名称和参数获取对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeFullName_WithValidTypeNameAndArgs_ReturnsObject_Test()
    {
        // Arrange
        string typeName = "Materal.Utils.Test.HelpersTest.TypeHelperTest+TestClassWithConstructor";
        object[] args = [5, "TestName"];

        // Act
        object? result = TypeHelper.GetObjectByTypeFullName(typeName, args);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TestClassWithConstructor>(result);

        // Verify constructor was called
        var typedResult = (TestClassWithConstructor)result;
        Assert.AreEqual(5, typedResult.Id);
        Assert.AreEqual("TestName", typedResult.Name);
    }

    #endregion

    #region GetObjectByTypeFullName<T> with Arguments Tests

    /// <summary>
    /// 测试通过完整类型名称、目标和参数获取对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeFullName_WithValidTypeNameAndTargetType_ReturnsObject_Test()
    {
        // Arrange
        string typeName = "Materal.Utils.Test.HelpersTest.TypeHelperTest+TestImplementation";

        // Act
        ITestInterface? result = TypeHelper.GetObjectByTypeFullName<ITestInterface>(typeName);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TestImplementation>(result);
    }

    /// <summary>
    /// 测试通过完整类型名称、目标和参数对象获取对象
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeFullNameGeneric_WithArgs_ReturnsTypedObject_Test()
    {
        // Arrange
        string typeName = "Materal.Utils.Test.HelpersTest.TypeHelperTest+TestClassWithConstructor";
        object[] args = [10, "Test"];

        // Act
        TestClassWithConstructor? result = TypeHelper.GetObjectByTypeFullName<TestClassWithConstructor>(typeName, args);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(10, result.Id);
        Assert.AreEqual("Test", result.Name);
    }

    #endregion

    #region GetTypeByTypeFullName Tests

    /// <summary>
    /// 测试通过完整类型名称获取类型
    /// </summary>
    [TestMethod]
    public void GetTypeByTypeFullName_WithValidTypeName_ReturnsType_Test()
    {
        // Arrange
        string typeName = "Materal.Utils.Test.HelpersTest.TypeHelperTest+TestClass";

        // Act
        Type? result = TypeHelper.GetTypeByTypeFullName(typeName);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Materal.Utils.Test.HelpersTest.TypeHelperTest+TestClass", result.FullName);
    }

    /// <summary>
    /// 测试通过完整类型名称获取不存在的类型
    /// </summary>
    [TestMethod]
    public void GetTypeByTypeFullName_WithInvalidTypeName_ReturnsNull_Test()
    {
        // Arrange
        string typeName = "Invalid.Namespace.NonExistentClass";

        // Act
        Type? result = TypeHelper.GetTypeByTypeFullName(typeName);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试通过完整类型名称和目标类型获取类型
    /// </summary>
    [TestMethod]
    public void GetTypeByTypeFullName_WithTargetType_ReturnsMatchingType_Test()
    {
        // Arrange
        string typeName = "Materal.Utils.Test.HelpersTest.TypeHelperTest+TestImplementation";
        Type targetType = typeof(ITestInterface);

        // Act
        Type? result = TypeHelper.GetTypeByTypeFullName(typeName, targetType);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(typeof(TestImplementation), result);
        Assert.IsTrue(targetType.IsAssignableFrom(result));
    }

    /// <summary>
    /// 测试通过完整类型名称和父类型获取类型
    /// </summary>
    [TestMethod]
    public void GetTypeByTypeFullName_WithParentType_ReturnsChildType_Test()
    {
        // Arrange
        string typeName = "Materal.Utils.Test.HelpersTest.TypeHelperTest+DerivedTestClass";
        Type parentType = typeof(TestClass);

        // Act
        Type? result = TypeHelper.GetTypeByTypeFullName(typeName, parentType);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(parentType.IsAssignableFrom(result));
    }

    #endregion

    #region GetTypeByTypeName Tests

    /// <summary>
    /// 测试通过简单类型名称获取类型
    /// </summary>
    [TestMethod]
    public void GetTypeByTypeName_WithValidTypeName_ReturnsType_Test()
    {
        // Arrange
        string typeName = "TestClass";

        // Act
        Type? result = TypeHelper.GetTypeByTypeName(typeName);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestClass", result.Name);
    }

    #endregion

    #region Abstract and Interface Filtering

    /// <summary>
    /// 测试抽象类不会被GetTypeByTypeFullName返回
    /// </summary>
    [TestMethod]
    public void GetTypeByTypeFullName_WithAbstractClass_ReturnsNull_Test()
    {
        // Arrange
        string typeName = "Materal.Utils.Test.HelpersTest.TypeHelperTest+AbstractTestClass";

        // Act
        Type? result = TypeHelper.GetTypeByTypeFullName(typeName);

        // Assert
        // Should return null because it's abstract
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试接口不会被GetTypeByTypeFullName返回
    /// </summary>
    [TestMethod]
    public void GetTypeByTypeFullName_WithInterface_ReturnsNull_Test()
    {
        // Arrange
        string typeName = "Materal.Utils.Test.HelpersTest.TypeHelperTest+ITestInterface";

        // Act
        Type? result = TypeHelper.GetTypeByTypeFullName(typeName);

        // Assert
        // Should return null because it's an interface
        Assert.IsNull(result);
    }

    #endregion

    #region Empty and Null Arguments

    /// <summary>
    /// 测试使用空类型名称时返回null
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeFullName_WithEmptyTypeName_ReturnsNull_Test()
    {
        // Act
        object? result = TypeHelper.GetObjectByTypeFullName("");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试使用空白类型名称时返回null
    /// </summary>
    [TestMethod]
    public void GetObjectByTypeFullName_WithWhitespaceTypeName_ReturnsNull_Test()
    {
        // Act
        object? result = TypeHelper.GetObjectByTypeFullName("   ");

        // Assert
        Assert.IsNull(result);
    }

    #endregion
}
