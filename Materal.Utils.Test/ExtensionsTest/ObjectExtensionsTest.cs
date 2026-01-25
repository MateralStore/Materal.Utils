using System.Collections;
using System.Data;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// ObjectExtensions 测试类
/// 测试 Object 扩展方法的功能
/// </summary>
[TestClass]
public class ObjectExtensionsTest
{
    #region 测试模型类

    /// <summary>
    /// 简单测试模型
    /// </summary>
    public class SimpleModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    /// <summary>
    /// 嵌套测试模型
    /// </summary>
    public class NestedModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Address? Address { get; set; }
        public List<string>? Tags { get; set; }
    }

    /// <summary>
    /// 地址模型
    /// </summary>
    public class Address
    {
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public int ZipCode { get; set; }
    }

    /// <summary>
    /// 带字段的测试模型
    /// </summary>
    public class ModelWithFields
    {
        public string PublicField = "FieldValue";
        public int Id { get; set; }
    }

    #endregion

    #region IsNullOrEmptyString Tests

    /// <summary>
    /// 测试null对象返回true
    /// </summary>
    [TestMethod]
    public void IsNullOrEmptyString_WithNullObject_ReturnsTrue_Test()
    {
        // Arrange
        object? obj = null;

        // Act
        bool result = obj!.IsNullOrEmptyString();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试空字符串返回true
    /// </summary>
    [TestMethod]
    public void IsNullOrEmptyString_WithEmptyString_ReturnsTrue_Test()
    {
        // Arrange
        object obj = string.Empty;

        // Act
        bool result = obj.IsNullOrEmptyString();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试非空字符串返回false
    /// </summary>
    [TestMethod]
    public void IsNullOrEmptyString_WithNonEmptyString_ReturnsFalse_Test()
    {
        // Arrange
        object obj = "test";

        // Act
        bool result = obj.IsNullOrEmptyString();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试空格字符串返回false
    /// </summary>
    [TestMethod]
    public void IsNullOrEmptyString_WithWhiteSpaceString_ReturnsFalse_Test()
    {
        // Arrange
        object obj = "   ";

        // Act
        bool result = obj.IsNullOrEmptyString();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试非字符串对象返回false
    /// </summary>
    [TestMethod]
    public void IsNullOrEmptyString_WithNonStringObject_ReturnsFalse_Test()
    {
        // Arrange
        object obj = 123;

        // Act
        bool result = obj.IsNullOrEmptyString();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsNullOrWhiteSpaceString Tests

    /// <summary>
    /// 测试null对象返回true
    /// </summary>
    [TestMethod]
    public void IsNullOrWhiteSpaceString_WithNullObject_ReturnsTrue_Test()
    {
        // Arrange
        object? obj = null;

        // Act
        bool result = obj!.IsNullOrWhiteSpaceString();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试空字符串返回true
    /// </summary>
    [TestMethod]
    public void IsNullOrWhiteSpaceString_WithEmptyString_ReturnsTrue_Test()
    {
        // Arrange
        object obj = string.Empty;

        // Act
        bool result = obj.IsNullOrWhiteSpaceString();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试空格字符串返回true
    /// </summary>
    [TestMethod]
    public void IsNullOrWhiteSpaceString_WithWhiteSpaceString_ReturnsTrue_Test()
    {
        // Arrange
        object obj = "   ";

        // Act
        bool result = obj.IsNullOrWhiteSpaceString();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试制表符和换行符返回true
    /// </summary>
    [TestMethod]
    public void IsNullOrWhiteSpaceString_WithTabAndNewLine_ReturnsTrue_Test()
    {
        // Arrange
        object obj = "\t\n\r";

        // Act
        bool result = obj.IsNullOrWhiteSpaceString();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试非空字符串返回false
    /// </summary>
    [TestMethod]
    public void IsNullOrWhiteSpaceString_WithNonEmptyString_ReturnsFalse_Test()
    {
        // Arrange
        object obj = "test";

        // Act
        bool result = obj.IsNullOrWhiteSpaceString();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试非字符串对象返回false
    /// </summary>
    [TestMethod]
    public void IsNullOrWhiteSpaceString_WithNonStringObject_ReturnsFalse_Test()
    {
        // Arrange
        object obj = 123;

        // Act
        bool result = obj.IsNullOrWhiteSpaceString();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region Equals Tests

    /// <summary>
    /// 测试相同属性值的对象返回true
    /// </summary>
    [TestMethod]
    public void Equals_WithSamePropertyValues_ReturnsTrue_Test()
    {
        // Arrange
        string name = "Test";
        var a = new { Id = 1, Name = name, Price = 100m };
        var b = new { Id = 1, Name = name, Price = 100m };
        Dictionary<string, Func<object?, bool>> maps = [];

        // Act
        bool result = a.Equals(b, maps);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试不同属性值的对象返回false
    /// </summary>
    [TestMethod]
    public void Equals_WithDifferentPropertyValues_ReturnsFalse_Test()
    {
        // Arrange
        string name = "Test";
        var a = new { Id = 1, Name = name, Price = 100m };
        var b = new { Id = 2, Name = name, Price = 100m };
        Dictionary<string, Func<object?, bool>> maps = [];

        // Act
        bool result = a.Equals(b, maps);

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试使用自定义映射规则
    /// </summary>
    [TestMethod]
    public void Equals_WithCustomMaps_UsesCustomLogic_Test()
    {
        // Arrange
        string name = "Test";
        var a = new { Id = 1, Name = name, Price = 100m };
        var b = new { Id = 2, Name = name, Price = 100m };
        Dictionary<string, Func<object?, bool>> maps = new()
        {
            { "Id", _ => true }
        };

        // Act
        bool result = a.Equals(b, maps);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试自定义映射返回false时整体返回false
    /// </summary>
    [TestMethod]
    public void Equals_WithCustomMapReturnsFalse_ReturnsFalse_Test()
    {
        // Arrange
        string name = "Test";
        var a = new { Id = 1, Name = name, Price = 100m };
        var b = new { Id = 1, Name = name, Price = 100m };
        Dictionary<string, Func<object?, bool>> maps = new()
        {
            { "Name", value => value?.ToString() == "Different" }
        };

        // Act
        bool result = a.Equals(b, maps);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region GetObjectValue<T> Tests

    /// <summary>
    /// 测试获取简单属性
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithSimpleProperty_ReturnsValue_Test()
    {
        // Arrange
        SimpleModel model = new() { Id = 1, Name = "Test", Price = 99.99m };

        // Act
        string? name = model.GetObjectValue<string>("Name");
        int? id = model.GetObjectValue<int>("Id");

        // Assert
        Assert.AreEqual("Test", name);
        Assert.AreEqual(1, id);
    }

    /// <summary>
    /// 测试获取嵌套属性
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithNestedProperty_ReturnsValue_Test()
    {
        // Arrange
        NestedModel model = new()
        {
            Id = 1,
            Name = "Test",
            Address = new Address { City = "北京", Street = "长安街", ZipCode = 100000 }
        };

        // Act
        string? city = model.GetObjectValue<string>("Address.City");
        int? zipCode = model.GetObjectValue<int>("Address.ZipCode");

        // Assert
        Assert.AreEqual("北京", city);
        Assert.AreEqual(100000, zipCode);
    }

    /// <summary>
    /// 测试获取列表元素
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithListIndex_ReturnsElement_Test()
    {
        // Arrange
        List<string> list = ["A", "B", "C"];

        // Act
        string? first = list.GetObjectValue<string>("0");
        string? second = list.GetObjectValue<string>("1");

        // Assert
        Assert.AreEqual("A", first);
        Assert.AreEqual("B", second);
    }

    /// <summary>
    /// 测试使用数组索引语法
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithArrayIndexSyntax_ReturnsElement_Test()
    {
        // Arrange
        NestedModel model = new()
        {
            Id = 1,
            Name = "Test",
            Tags = ["Tag1", "Tag2", "Tag3"]
        };

        // Act
        string? firstTag = model.GetObjectValue<string>("Tags[0]");
        string? secondTag = model.GetObjectValue<string>("Tags[1]");

        // Assert
        Assert.AreEqual("Tag1", firstTag);
        Assert.AreEqual("Tag2", secondTag);
    }

    /// <summary>
    /// 测试获取字典值
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithDictionary_ReturnsValue_Test()
    {
        // Arrange
        Dictionary<string, object> dict = new()
        {
            { "Key1", "Value1" },
            { "Key2", 123 }
        };

        // Act
        string? value1 = dict.GetObjectValue<string>("Key1");
        int? value2 = dict.GetObjectValue<int>("Key2");

        // Assert
        Assert.AreEqual("Value1", value1);
        Assert.AreEqual(123, value2);
    }

    /// <summary>
    /// 测试获取不存在的属性返回null
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithNonExistentProperty_ReturnsNull_Test()
    {
        // Arrange
        SimpleModel model = new() { Id = 1, Name = "Test", Price = 99.99m };

        // Act
        string? result = model.GetObjectValue<string>("NonExistent");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试类型不匹配返回默认值
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithTypeMismatch_ReturnsDefault_Test()
    {
        // Arrange
        SimpleModel model = new() { Id = 1, Name = "Test", Price = 99.99m };

        // Act
        int result = model.GetObjectValue<int>("Name");

        // Assert
        Assert.AreEqual(0, result);
    }

    /// <summary>
    /// 测试获取字段值
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithPublicField_ReturnsValue_Test()
    {
        // Arrange
        ModelWithFields model = new() { Id = 1, PublicField = "TestField" };

        // Act
        string? fieldValue = model.GetObjectValue<string>("PublicField");

        // Assert
        Assert.AreEqual("TestField", fieldValue);
    }

    /// <summary>
    /// 测试使用参数数组获取嵌套值
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithParamsArray_ReturnsNestedValue_Test()
    {
        // Arrange
        NestedModel model = new()
        {
            Id = 1,
            Name = "Test",
            Address = new Address { City = "北京", Street = "长安街", ZipCode = 100000 }
        };

        // Act
        string? city = model.GetObjectValue<string>("Address", "City");
        string? street = model.GetObjectValue<string>("Address", "Street");

        // Assert
        Assert.AreEqual("北京", city);
        Assert.AreEqual("长安街", street);
    }

    /// <summary>
    /// 测试从DataTable获取DataRow
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithDataTable_ReturnsDataRow_Test()
    {
        // Arrange
        DataTable dt = new();
        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Rows.Add(1, "Row1");
        dt.Rows.Add(2, "Row2");

        // Act
        DataRow? row = dt.GetObjectValue<DataRow>("0");

        // Assert
        Assert.IsNotNull(row);
        Assert.AreEqual(1, row["Id"]);
        Assert.AreEqual("Row1", row["Name"]);
    }

    /// <summary>
    /// 测试从DataRow获取列值
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithDataRow_ReturnsColumnValue_Test()
    {
        // Arrange
        DataTable dt = new();
        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        DataRow dr = dt.NewRow();
        dr["Id"] = 1;
        dr["Name"] = "Test";
        dt.Rows.Add(dr);

        // Act
        int? id = dr.GetObjectValue<int>("Id");
        string? name = dr.GetObjectValue<string>("Name");

        // Assert
        Assert.AreEqual(1, id);
        Assert.AreEqual("Test", name);
    }

    /// <summary>
    /// 测试从DataRow使用列索引获取值
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithDataRowColumnIndex_ReturnsValue_Test()
    {
        // Arrange
        DataTable dt = new();
        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        DataRow dr = dt.NewRow();
        dr["Id"] = 1;
        dr["Name"] = "Test";
        dt.Rows.Add(dr);

        // Act
        int? id = dr.GetObjectValue<int>("0");
        string? name = dr.GetObjectValue<string>("1");

        // Assert
        Assert.AreEqual(1, id);
        Assert.AreEqual("Test", name);
    }

    /// <summary>
    /// 测试从ICollection获取元素
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithICollection_ReturnsElement_Test()
    {
        // Arrange
        List<string> collection = ["A", "B", "C"];

        // Act
        string? first = collection.GetObjectValue<string>("0");

        // Assert
        Assert.AreEqual("A", first);
    }

    /// <summary>
    /// 测试从非泛型字典获取值
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithNonGenericDictionary_ReturnsValue_Test()
    {
        // Arrange
        Hashtable hashtable = new()
        {
            { "Key1", "Value1" },
            { "Key2", 123 }
        };

        // Act
        string? value1 = hashtable.GetObjectValue<string>("Key1");
        int? value2 = hashtable.GetObjectValue<int>("Key2");

        // Assert
        Assert.AreEqual("Value1", value1);
        Assert.AreEqual(123, value2);
    }

    /// <summary>
    /// 测试复杂路径访问
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithComplexPath_ReturnsValue_Test()
    {
        // Arrange
        var complexObj = new
        {
            Level1 = new
            {
                Level2 = new
                {
                    Level3 = "DeepValue"
                }
            }
        };

        // Act
        string? value = complexObj.GetObjectValue<string>("Level1.Level2.Level3");

        // Assert
        Assert.AreEqual("DeepValue", value);
    }

    /// <summary>
    /// 测试中间路径为null时返回null
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithNullInPath_ReturnsNull_Test()
    {
        // Arrange
        NestedModel model = new()
        {
            Id = 1,
            Name = "Test",
            Address = null
        };

        // Act
        string? city = model.GetObjectValue<string>("Address.City");

        // Assert
        Assert.IsNull(city);
    }

    /// <summary>
    /// 测试数组索引超出范围返回null
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithIndexOutOfRange_ReturnsNull_Test()
    {
        // Arrange
        List<string> list = ["A", "B", "C"];

        // Act
        string? result = list.GetObjectValue<string>("10");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试负数索引返回null
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithNegativeIndex_ReturnsNull_Test()
    {
        // Arrange
        List<string> list = ["A", "B", "C"];

        // Act
        string? result = list.GetObjectValue<string>("-1");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试DataTable索引超出范围返回null
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithDataTableIndexOutOfRange_ReturnsNull_Test()
    {
        // Arrange
        DataTable dt = new();
        dt.Columns.Add("Id", typeof(int));
        dt.Rows.Add(1);

        // Act
        DataRow? row = dt.GetObjectValue<DataRow>("10");

        // Assert
        Assert.IsNull(row);
    }

    /// <summary>
    /// 测试DataRow列名不存在且索引无效返回null
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithDataRowInvalidColumnAndIndex_ReturnsNull_Test()
    {
        // Arrange
        DataTable dt = new();
        dt.Columns.Add("Id", typeof(int));
        DataRow dr = dt.NewRow();
        dr["Id"] = 1;
        dt.Rows.Add(dr);

        // Act
        object? result = dr.GetObjectValue<object>("NonExistent");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试多个数组索引
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithMultipleArrayIndexes_ReturnsValue_Test()
    {
        // Arrange
        var obj = new
        {
            Matrix = new List<List<int>>
            {
                new() { 1, 2, 3 },
                new() { 4, 5, 6 },
                new() { 7, 8, 9 }
            }
        };

        // Act
        int? value = obj.GetObjectValue<int>("Matrix[1][2]");

        // Assert
        Assert.AreEqual(6, value);
    }

    /// <summary>
    /// 测试混合属性和数组索引
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithMixedPropertyAndArrayIndex_ReturnsValue_Test()
    {
        // Arrange
        var obj = new
        {
            Data = new
            {
                Items = new List<string> { "First", "Second", "Third" }
            }
        };

        // Act
        string? value = obj.GetObjectValue<string>("Data.Items[1]");

        // Assert
        Assert.AreEqual("Second", value);
    }

    /// <summary>
    /// 测试空集合名称使用数组索引
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithArrayIndexOnly_ReturnsValue_Test()
    {
        // Arrange
        List<string> list = ["A", "B", "C"];

        // Act
        string? value = list.GetObjectValue<string>("[1]");

        // Assert
        Assert.AreEqual("B", value);
    }

    #endregion

    #region GetObjectValue (non-generic) Tests

    /// <summary>
    /// 测试非泛型版本返回object
    /// </summary>
    [TestMethod]
    public void GetObjectValue_NonGeneric_ReturnsObject_Test()
    {
        // Arrange
        SimpleModel model = new() { Id = 1, Name = "Test", Price = 99.99m };

        // Act
        object? name = model.GetObjectValue("Name");
        object? id = model.GetObjectValue("Id");

        // Assert
        Assert.IsNotNull(name);
        Assert.IsNotNull(id);
        Assert.AreEqual("Test", name);
        Assert.AreEqual(1, id);
    }

    /// <summary>
    /// 测试非泛型版本获取嵌套属性
    /// </summary>
    [TestMethod]
    public void GetObjectValue_NonGenericWithNestedProperty_ReturnsObject_Test()
    {
        // Arrange
        NestedModel model = new()
        {
            Id = 1,
            Name = "Test",
            Address = new Address { City = "北京" }
        };

        // Act
        object? city = model.GetObjectValue("Address.City");

        // Assert
        Assert.IsNotNull(city);
        Assert.AreEqual("北京", city);
    }

    #endregion

    #region Edge Cases Tests

    /// <summary>
    /// 测试空字符串属性名返回null
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithEmptyPropertyName_ReturnsNull_Test()
    {
        // Arrange
        SimpleModel model = new() { Id = 1, Name = "Test", Price = 99.99m };

        // Act
        string? result = model.GetObjectValue<string>("");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试只读属性可以获取
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithReadOnlyProperty_ReturnsValue_Test()
    {
        // Arrange
        var obj = new { ReadOnlyProp = "ReadOnlyValue" };

        // Act
        string? value = obj.GetObjectValue<string>("ReadOnlyProp");

        // Assert
        Assert.AreEqual("ReadOnlyValue", value);
    }

    /// <summary>
    /// 测试ICollection为空时索引返回null
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithEmptyCollection_ReturnsNull_Test()
    {
        // Arrange
        List<string> collection = [];

        // Act
        string? result = collection.GetObjectValue<string>("0");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试非数字索引字符串返回null
    /// </summary>
    [TestMethod]
    public void GetObjectValue_WithNonNumericIndex_ReturnsNull_Test()
    {
        // Arrange
        List<string> list = ["A", "B", "C"];

        // Act
        string? result = list.GetObjectValue<string>("abc");

        // Assert
        Assert.IsNull(result);
    }

    #endregion
}
