namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// JsonExtensions 测试类
/// 测试JSON扩展方法的功能
/// </summary>
[TestClass]
public class JsonExtensionsTest
{
    #region ToJson Tests

    /// <summary>
    /// 测试简单对象转换为JSON
    /// </summary>
    [TestMethod]
    public void ToJson_WithSimpleObject_ReturnsJsonString_Test()
    {
        // Arrange
        TestClass obj = new() { Id = 1, Name = "Test" };

        // Act
        string result = obj.ToJson();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("Id", result);
        Assert.Contains("Name", result);
    }

    /// <summary>
    /// 测试空对象转换为JSON
    /// </summary>
    [TestMethod]
    public void ToJson_WithEmptyObject_ReturnsJsonString_Test()
    {
        // Arrange
        TestClass obj = new();

        // Act
        string result = obj.ToJson();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("{", result);
        Assert.Contains("}", result);
    }

    /// <summary>
    /// 测试集合转换为JSON
    /// </summary>
    [TestMethod]
    public void ToJson_WithCollection_ReturnsJsonArray_Test()
    {
        // Arrange
        List<int> list = [1, 2, 3, 4, 5];

        // Act
        string result = list.ToJson();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("[", result);
        Assert.Contains("]", result);
        Assert.Contains("1", result);
        Assert.Contains("5", result);
    }

    /// <summary>
    /// 测试字典转换为JSON
    /// </summary>
    [TestMethod]
    public void ToJson_WithDictionary_ReturnsJsonObject_Test()
    {
        // Arrange
        Dictionary<string, object> dict = new()
        {
            { "key1", "value1" },
            { "key2", 123 }
        };

        // Act
        string result = dict.ToJson();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("key1", result);
        Assert.Contains("value1", result);
        Assert.Contains("key2", result);
        Assert.Contains("123", result);
    }

    #endregion

    #region JsonToObject Tests

    /// <summary>
    /// 测试JSON字符串转换为对象
    /// </summary>
    [TestMethod]
    public void JsonToObject_WithValidJson_ReturnsObject_Test()
    {
        // Arrange
        string json = "{\"Id\":1,\"Name\":\"Test\"}";

        // Act
        TestClass result = json.JsonToObject<TestClass>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Test", result.Name);
    }

    /// <summary>
    /// 测试JSON字符串转换为对象（指定类型）
    /// </summary>
    [TestMethod]
    public void JsonToObject_WithTypeParameter_ReturnsObject_Test()
    {
        // Arrange
        string json = "{\"Id\":1,\"Name\":\"Test\"}";
        Type type = typeof(TestClass);

        // Act
        object result = json.JsonToObject(type);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TestClass>(result);
        TestClass testObj = (TestClass)result;
        Assert.AreEqual(1, testObj.Id);
        Assert.AreEqual("Test", testObj.Name);
    }

    /// <summary>
    /// 测试无效JSON字符串抛出异常
    /// </summary>
    [TestMethod]
    public void JsonToObject_WithInvalidJson_ThrowsUtilException_Test()
    {
        // Arrange
        string json = "invalid json";

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => json.JsonToObject<TestClass>());
    }

    /// <summary>
    /// 测试不带类型参数的JSON转换为对象
    /// </summary>
    [TestMethod]
    public void JsonToObject_WithoutTypeParameter_ReturnsObject_Test()
    {
        // Arrange
        string json = "{\"Id\":1,\"Name\":\"Test\"}";

        // Act
        object result = json.JsonToObject();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<object>(result);
    }

    /// <summary>
    /// 测试JSON数组转换为列表
    /// </summary>
    [TestMethod]
    public void JsonToObject_WithJsonArray_ReturnsList_Test()
    {
        // Arrange
        string json = "[1,2,3,4,5]";

        // Act
        List<int> result = json.JsonToObject<List<int>>();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(5, result);
        Assert.AreEqual(1, result[0]);
        Assert.AreEqual(5, result[4]);
    }

    /// <summary>
    /// 测试大小写不敏感的JSON反序列化
    /// </summary>
    [TestMethod]
    public void JsonToObject_WithDifferentCase_WorksCorrectly_Test()
    {
        // Arrange
        string json = "{\"id\":1,\"name\":\"Test\"}";

        // Act
        TestClass result = json.JsonToObject<TestClass>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Test", result.Name);
    }

    #endregion

    #region ToJsonWithInferredTypes Tests

    /// <summary>
    /// 测试对象转换为带类型推断的JSON
    /// </summary>
    [TestMethod]
    public void ToJsonWithInferredTypes_WithObject_ReturnsJsonString_Test()
    {
        // Arrange
        TestClass obj = new() { Id = 1, Name = "Test" };

        // Act
        string result = obj.ToJsonWithInferredTypes();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThan(0, result.Length);
    }

    /// <summary>
    /// 测试包含不同类型的对象转换
    /// </summary>
    [TestMethod]
    public void ToJsonWithInferredTypes_WithMixedTypes_ReturnsJsonString_Test()
    {
        // Arrange
        object obj = new { IntValue = 123, StringValue = "test", BoolValue = true };

        // Act
        string result = obj.ToJsonWithInferredTypes();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("123", result);
        Assert.Contains("test", result);
        Assert.Contains("true", result);
    }

    #endregion

    #region JsonToObjectWithInferredTypes Tests

    /// <summary>
    /// 测试带类型推断的JSON转换为对象
    /// </summary>
    [TestMethod]
    public void JsonToObjectWithInferredTypes_WithValidJson_ReturnsObject_Test()
    {
        // Arrange
        TestClass original = new() { Id = 1, Name = "Test" };
        string json = original.ToJsonWithInferredTypes();

        // Act
        TestClass result = json.JsonToObjectWithInferredTypes<TestClass>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(original.Id, result.Id);
        Assert.AreEqual(original.Name, result.Name);
    }

    /// <summary>
    /// 测试带类型推断的JSON转换（指定类型）
    /// </summary>
    [TestMethod]
    public void JsonToObjectWithInferredTypes_WithTypeParameter_ReturnsObject_Test()
    {
        // Arrange
        TestClass original = new() { Id = 1, Name = "Test" };
        string json = original.ToJsonWithInferredTypes();
        Type type = typeof(TestClass);

        // Act
        object result = json.JsonToObjectWithInferredTypes(type);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TestClass>(result);
    }

    /// <summary>
    /// 测试不带类型参数的带类型推断JSON转换
    /// </summary>
    [TestMethod]
    public void JsonToObjectWithInferredTypes_WithoutTypeParameter_ReturnsObject_Test()
    {
        // Arrange
        TestClass original = new() { Id = 1, Name = "Test" };
        string json = original.ToJsonWithInferredTypes();

        // Act
        object result = json.JsonToObjectWithInferredTypes();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<TestClass>(result);
    }

    #endregion

    #region JsonToInterface Tests

    /// <summary>
    /// 测试JSON转换为接口实现
    /// </summary>
    [TestMethod]
    public void JsonToInterface_WithValidTypeAndJson_ReturnsImplementation_Test()
    {
        // Arrange
        string json = "{\"Id\":1,\"Name\":\"Test\"}";
        string typeName = typeof(TestClass).AssemblyQualifiedName!;

        // Act
        ITestInterface result = json.JsonToInterface<ITestInterface>(typeName);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// 测试用的接口
    /// </summary>
    public interface ITestInterface
    {
        int Id { get; set; }
    }

    /// <summary>
    /// 测试用的类
    /// </summary>
    public class TestClass : ITestInterface
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    #endregion
}
