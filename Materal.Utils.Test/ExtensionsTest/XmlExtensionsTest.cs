using System.Xml;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// XmlExtensions 测试类
/// 测试XML扩展方法的功能
/// </summary>
[TestClass]
public class XmlExtensionsTest
{
    #region ToXml Tests

    /// <summary>
    /// 测试简单对象转换为XML
    /// </summary>
    [TestMethod]
    public void ToXml_WithSimpleObject_ReturnsXmlString_Test()
    {
        // Arrange
        TestClass obj = new() { Id = 1, Name = "Test" };

        // Act
        string result = obj.ToXml();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("<Id>1</Id>", result);
        Assert.Contains("<Name>Test</Name>", result);
    }

    /// <summary>
    /// 测试字典转换为XML
    /// </summary>
    [TestMethod]
    public void ToXml_WithDictionary_ReturnsXmlString_Test()
    {
        // Arrange
        IDictionary<string, object> dictionary = new Dictionary<string, object>
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" }
        };

        // Act
        string result = dictionary.ToXml();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("<Root>", result);
        Assert.Contains("<Key1>Value1</Key1>", result);
        Assert.Contains("<Key2>Value2</Key2>", result);
    }

    /// <summary>
    /// 测试嵌套字典转换为XML
    /// </summary>
    [TestMethod]
    public void ToXml_WithNestedDictionary_ReturnsXmlString_Test()
    {
        // Arrange
        IDictionary<string, object> nestedDict = new Dictionary<string, object>
        {
            { "NestedKey", "NestedValue" }
        };
        IDictionary<string, object> dictionary = new Dictionary<string, object>
        {
            { "Parent", nestedDict }
        };

        // Act
        string result = dictionary.ToXml();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("<Parent>", result);
        Assert.Contains("<NestedKey>NestedValue</NestedKey>", result);
    }

    /// <summary>
    /// 测试空字典转换为XML
    /// </summary>
    [TestMethod]
    public void ToXml_WithEmptyDictionary_ReturnsRootElement_Test()
    {
        // Arrange
        IDictionary<string, object> dictionary = new Dictionary<string, object>();

        // Act
        string result = dictionary.ToXml();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("<Root", result);
    }

    /// <summary>
    /// 测试包含数字值的字典转换为XML
    /// </summary>
    [TestMethod]
    public void ToXml_WithNumericValues_ReturnsXmlString_Test()
    {
        // Arrange
        IDictionary<string, object> dictionary = new Dictionary<string, object>
        {
            { "IntValue", 123 },
            { "DoubleValue", 45.67 }
        };

        // Act
        string result = dictionary.ToXml();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("<IntValue>123</IntValue>", result);
        Assert.Contains("<DoubleValue>", result);
    }

    /// <summary>
    /// 测试生成的XML是有效的XML文档
    /// </summary>
    [TestMethod]
    public void ToXml_GeneratesValidXml_Test()
    {
        // Arrange
        IDictionary<string, object> dictionary = new Dictionary<string, object>
        {
            { "Key1", "Value1" }
        };

        // Act
        string result = dictionary.ToXml();
        XmlDocument xmlDoc = new();

        // Assert - 如果XML无效会抛出异常
        xmlDoc.LoadXml(result);
        Assert.IsNotNull(xmlDoc.DocumentElement);
    }

    /// <summary>
    /// 测试对象转换为XML时处理特殊字符
    /// </summary>
    [TestMethod]
    public void ToXml_WithSpecialCharacters_HandlesCorrectly_Test()
    {
        // Arrange
        TestClass obj = new() { Id = 1, Name = "Test<>&\"'" };

        // Act
        string result = obj.ToXml();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThan(0, result.Length);
    }

    /// <summary>
    /// 测试复杂对象转换为XML
    /// </summary>
    [TestMethod]
    public void ToXml_WithComplexObject_ReturnsXmlString_Test()
    {
        // Arrange
        ComplexClass obj = new()
        {
            Id = 1,
            Name = "Test",
            Items = [1, 2, 3]
        };

        // Act
        string result = obj.ToXml();

        // Assert
        Assert.IsNotNull(result);
        Assert.Contains("<Id>1</Id>", result);
        Assert.Contains("<Name>Test</Name>", result);
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// 测试用的简单类
    /// </summary>
    public class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 测试用的复杂类
    /// </summary>
    public class ComplexClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<int> Items { get; set; } = [];
    }

    #endregion
}
