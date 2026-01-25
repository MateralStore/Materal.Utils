using System.Reflection;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// MemberInfoExtensions 测试类
/// 测试MemberInfo扩展方法的功能
/// </summary>
[TestClass]
public class MemberInfoExtensionsTest
{
    #region GetValue Tests

    /// <summary>
    /// 测试从PropertyInfo获取值
    /// </summary>
    [TestMethod]
    public void GetValue_FromPropertyInfo_ReturnsPropertyValue_Test()
    {
        // Arrange
        TestClass obj = new() { TestProperty = "TestValue" };
        PropertyInfo propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty))!;

        // Act
        object? result = propertyInfo.GetValue(obj);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestValue", result);
    }

    /// <summary>
    /// 测试从FieldInfo获取值
    /// </summary>
    [TestMethod]
    public void GetValue_FromFieldInfo_ReturnsFieldValue_Test()
    {
        // Arrange
        TestClass obj = new();
        obj.SetTestField(42);
        FieldInfo fieldInfo = typeof(TestClass).GetField("_testField", BindingFlags.NonPublic | BindingFlags.Instance)!;

        // Act
        object? result = fieldInfo.GetValue(obj);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(42, result);
    }

    /// <summary>
    /// 测试从不支持的MemberInfo类型获取值抛出异常
    /// </summary>
    [TestMethod]
    public void GetValue_FromUnsupportedMemberInfo_ThrowsUtilException_Test()
    {
        // Arrange
        TestClass obj = new();
        MemberInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod))!;

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => methodInfo.GetValue(obj));
    }

    /// <summary>
    /// 测试获取null值的属性
    /// </summary>
    [TestMethod]
    public void GetValue_FromNullableProperty_ReturnsNull_Test()
    {
        // Arrange
        TestClass obj = new() { NullableProperty = null };
        PropertyInfo propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.NullableProperty))!;

        // Act
        object? result = propertyInfo.GetValue(obj);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试获取整数类型属性
    /// </summary>
    [TestMethod]
    public void GetValue_FromIntProperty_ReturnsIntValue_Test()
    {
        // Arrange
        TestClass obj = new() { IntProperty = 100 };
        PropertyInfo propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

        // Act
        object? result = propertyInfo.GetValue(obj);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(100, result);
    }

    #endregion

    #region HasCustomAttribute Tests

    /// <summary>
    /// 测试检查属性是否具有指定特性（存在的情况）
    /// </summary>
    [TestMethod]
    public void HasCustomAttribute_WhenAttributeExists_ReturnsTrue_Test()
    {
        // Arrange
        PropertyInfo propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.AttributedProperty))!;

        // Act
        bool result = propertyInfo.HasCustomAttribute<TestAttribute>();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试检查属性是否具有指定特性（不存在的情况）
    /// </summary>
    [TestMethod]
    public void HasCustomAttribute_WhenAttributeDoesNotExist_ReturnsFalse_Test()
    {
        // Arrange
        PropertyInfo propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty))!;

        // Act
        bool result = propertyInfo.HasCustomAttribute<TestAttribute>();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试检查字段是否具有指定特性
    /// </summary>
    [TestMethod]
    public void HasCustomAttribute_OnField_WorksCorrectly_Test()
    {
        // Arrange
        FieldInfo fieldInfo = typeof(TestClass).GetField(nameof(TestClass.AttributedField))!;

        // Act
        bool result = fieldInfo.HasCustomAttribute<TestAttribute>();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试检查方法是否具有指定特性
    /// </summary>
    [TestMethod]
    public void HasCustomAttribute_OnMethod_WorksCorrectly_Test()
    {
        // Arrange
        MethodInfo methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.AttributedMethod))!;

        // Act
        bool result = methodInfo.HasCustomAttribute<TestAttribute>();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试检查类是否具有指定特性
    /// </summary>
    [TestMethod]
    public void HasCustomAttribute_OnType_WorksCorrectly_Test()
    {
        // Arrange
        Type type = typeof(AttributedClass);

        // Act
        bool result = type.HasCustomAttribute<TestAttribute>();

        // Assert
        Assert.IsTrue(result);
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// 测试用的类
    /// </summary>
    private class TestClass
    {
        private int _testField;

        public string TestProperty { get; set; } = string.Empty;
        public string? NullableProperty { get; set; }
        public int IntProperty { get; set; }

        [TestAttribute]
        public string AttributedProperty { get; set; } = string.Empty;

        [TestAttribute]
        public string AttributedField = string.Empty;

        public void SetTestField(int value) => _testField = value;

        public void TestMethod() { }

        [TestAttribute]
        public void AttributedMethod() { }
    }

    /// <summary>
    /// 测试用的特性标记类
    /// </summary>
    [TestAttribute]
    private class AttributedClass
    {
    }

    /// <summary>
    /// 测试用的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    private class TestAttribute : Attribute
    {
    }

    #endregion
}
