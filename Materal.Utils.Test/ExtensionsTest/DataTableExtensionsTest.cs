using System.Data;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// DataTableExtensions 测试类
/// 测试 DataTable 扩展方法的功能
/// </summary>
[TestClass]
public class DataTableExtensionsTest
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
    /// 包含可空类型的测试模型
    /// </summary>
    public class NullableModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public DateTime? BirthDate { get; set; }
    }

    /// <summary>
    /// 复杂测试模型
    /// </summary>
    public class ComplexModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreateTime { get; set; }
        public decimal Amount { get; set; }
    }

    #endregion

    #region ToDataTable Tests

    /// <summary>
    /// 测试将空集合转换为DataTable
    /// </summary>
    [TestMethod]
    public void ToDataTable_WithEmptyCollection_ReturnsEmptyDataTable_Test()
    {
        // Arrange
        List<SimpleModel> emptyList = [];

        // Act
        DataTable result = emptyList.ToDataTable();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Rows);
        Assert.HasCount(3, result.Columns);
    }

    /// <summary>
    /// 测试将单个对象的集合转换为DataTable
    /// </summary>
    [TestMethod]
    public void ToDataTable_WithSingleItem_ReturnsDataTableWithOneRow_Test()
    {
        // Arrange
        List<SimpleModel> list = [new SimpleModel { Id = 1, Name = "Test", Price = 99.99m }];

        // Act
        DataTable result = list.ToDataTable();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(1, result.Rows);
        Assert.HasCount(3, result.Columns);
        Assert.AreEqual(1, result.Rows[0]["Id"]);
        Assert.AreEqual("Test", result.Rows[0]["Name"]);
        Assert.AreEqual(99.99m, result.Rows[0]["Price"]);
    }

    /// <summary>
    /// 测试将多个对象的集合转换为DataTable
    /// </summary>
    [TestMethod]
    public void ToDataTable_WithMultipleItems_ReturnsDataTableWithMultipleRows_Test()
    {
        // Arrange
        List<SimpleModel> list =
        [
            new SimpleModel { Id = 1, Name = "Item1", Price = 10.5m },
            new SimpleModel { Id = 2, Name = "Item2", Price = 20.5m },
            new SimpleModel { Id = 3, Name = "Item3", Price = 30.5m }
        ];

        // Act
        DataTable result = list.ToDataTable();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(3, result.Rows);
        Assert.HasCount(3, result.Columns);
        Assert.AreEqual(2, result.Rows[1]["Id"]);
        Assert.AreEqual("Item2", result.Rows[1]["Name"]);
    }

    /// <summary>
    /// 测试集合中包含null元素时跳过该元素
    /// </summary>
    [TestMethod]
    public void ToDataTable_WithNullItemsInCollection_SkipsNullItems_Test()
    {
        // Arrange
        List<SimpleModel?> list =
        [
            new SimpleModel { Id = 1, Name = "Item1", Price = 10.5m },
            null,
            new SimpleModel { Id = 2, Name = "Item2", Price = 20.5m }
        ];

        // Act
        DataTable result = list.ToDataTable();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result.Rows);
    }

    #endregion

    #region ToDataRow Tests

    /// <summary>
    /// 测试将对象转换为数据行（提供DataRow参数）
    /// </summary>
    [TestMethod]
    public void ToDataRow_WithValidObjectAndDataRow_ReturnsFilledDataRow_Test()
    {
        // Arrange
        SimpleModel model = new() { Id = 1, Name = "Test", Price = 99.99m };
        DataTable dt = typeof(SimpleModel).ToDataTable();
        DataRow dr = dt.NewRow();

        // Act
        DataRow result = model.ToDataRow(dr);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result["Id"]);
        Assert.AreEqual("Test", result["Name"]);
        Assert.AreEqual(99.99m, result["Price"]);
    }

    /// <summary>
    /// 测试对象为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void ToDataRow_WithNullObject_ThrowsArgumentNullException_Test()
    {
        // Arrange
        SimpleModel? model = null;
        DataTable dt = typeof(SimpleModel).ToDataTable();
        DataRow dr = dt.NewRow();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => model!.ToDataRow(dr));
    }

    /// <summary>
    /// 测试DataRow为null时抛出UtilException
    /// </summary>
    [TestMethod]
    public void ToDataRow_WithNullDataRow_ThrowsUtilException_Test()
    {
        // Arrange
        SimpleModel model = new() { Id = 1, Name = "Test", Price = 99.99m };
        DataRow? dr = null;

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => model.ToDataRow(dr!));
    }

    /// <summary>
    /// 测试将对象转换为数据行（不提供DataRow参数）
    /// </summary>
    [TestMethod]
    public void ToDataRow_WithValidObjectOnly_ReturnsNewDataRow_Test()
    {
        // Arrange
        SimpleModel model = new() { Id = 1, Name = "Test", Price = 99.99m };

        // Act
        DataRow result = model.ToDataRow();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result["Id"]);
        Assert.AreEqual("Test", result["Name"]);
        Assert.AreEqual(99.99m, result["Price"]);
    }

    /// <summary>
    /// 测试对象属性值为null时转换为DBNull
    /// </summary>
    [TestMethod]
    public void ToDataRow_WithNullPropertyValue_ConvertsToDBNull_Test()
    {
        // Arrange
        NullableModel model = new() { Id = 1, Name = null, Age = null };

        // Act
        DataRow result = model.ToDataRow();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result["Id"]);
        Assert.AreEqual(DBNull.Value, result["Name"]);
        Assert.AreEqual(DBNull.Value, result["Age"]);
    }

    #endregion

    #region SetValueByDataRow Tests

    /// <summary>
    /// 测试通过DataRow设置对象属性值
    /// </summary>
    [TestMethod]
    public void SetValueByDataRow_WithValidData_SetsObjectProperties_Test()
    {
        // Arrange
        SimpleModel model = new();
        DataTable dt = typeof(SimpleModel).ToDataTable();
        DataRow dr = dt.NewRow();
        dr["Id"] = 10;
        dr["Name"] = "TestName";
        dr["Price"] = 123.45m;

        // Act
        model.SetValueByDataRow(dr);

        // Assert
        Assert.AreEqual(10, model.Id);
        Assert.AreEqual("TestName", model.Name);
        Assert.AreEqual(123.45m, model.Price);
    }

    /// <summary>
    /// 测试对象为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void SetValueByDataRow_WithNullObject_ThrowsArgumentNullException_Test()
    {
        // Arrange
        SimpleModel? model = null;
        DataTable dt = typeof(SimpleModel).ToDataTable();
        DataRow dr = dt.NewRow();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => model!.SetValueByDataRow(dr));
    }

    /// <summary>
    /// 测试DataRow为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void SetValueByDataRow_WithNullDataRow_ThrowsArgumentNullException_Test()
    {
        // Arrange
        SimpleModel model = new();
        DataRow? dr = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => model.SetValueByDataRow(dr!));
    }

    /// <summary>
    /// 测试转换异常时收集到exceptions列表
    /// </summary>
    [TestMethod]
    public void SetValueByDataRow_WithConversionError_CollectsExceptions_Test()
    {
        // Arrange
        SimpleModel model = new();
        DataTable dt = new();
        dt.Columns.Add("Id", typeof(object));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Price", typeof(decimal));
        DataRow dr = dt.NewRow();
        dr["Id"] = "InvalidNumber";
        dr["Name"] = "TestName";
        dr["Price"] = 123.45m;
        dt.Rows.Add(dr);
        List<Exception> exceptions = [];

        // Act
        model.SetValueByDataRow(dr, exceptions);

        // Assert
        Assert.IsNotEmpty(exceptions);
    }

    #endregion

    #region GetValue Tests

    /// <summary>
    /// 测试从DataRow获取对象
    /// </summary>
    [TestMethod]
    public void GetValue_WithValidDataRow_ReturnsObject_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();
        DataRow dr = dt.NewRow();
        dr["Id"] = 5;
        dr["Name"] = "TestItem";
        dr["Price"] = 50.5m;

        // Act
        SimpleModel result = dr.GetValue<SimpleModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Id);
        Assert.AreEqual("TestItem", result.Name);
        Assert.AreEqual(50.5m, result.Price);
    }

    /// <summary>
    /// 测试DataRow为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void GetValue_WithNullDataRow_ThrowsArgumentNullException_Test()
    {
        // Arrange
        DataRow? dr = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => dr!.GetValue<SimpleModel>());
    }

    /// <summary>
    /// 测试转换异常时收集到exceptions列表
    /// </summary>
    [TestMethod]
    public void GetValue_WithConversionError_CollectsExceptions_Test()
    {
        // Arrange
        DataTable dt = new();
        dt.Columns.Add("Id", typeof(object));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Price", typeof(decimal));
        DataRow dr = dt.NewRow();
        dr["Id"] = "InvalidNumber";
        dr["Name"] = "TestItem";
        dr["Price"] = 50.5m;
        dt.Rows.Add(dr);
        List<Exception> exceptions = [];

        // Act
        SimpleModel result = dr.GetValue<SimpleModel>(exceptions);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(exceptions);
    }

    #endregion

    #region GetStringValue Tests

    /// <summary>
    /// 测试从DataRow获取字符串值
    /// </summary>
    [TestMethod]
    public void GetStringValue_WithValidIndex_ReturnsStringValue_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();
        DataRow dr = dt.NewRow();
        dr["Id"] = 1;
        dr["Name"] = "TestValue";
        dr["Price"] = 99.99m;

        // Act
        string? result = dr.GetStringValue(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestValue", result);
    }

    /// <summary>
    /// 测试索引超出范围时返回null
    /// </summary>
    [TestMethod]
    public void GetStringValue_WithIndexOutOfRange_ReturnsNull_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();
        DataRow dr = dt.NewRow();
        dr["Id"] = 1;
        dr["Name"] = "TestValue";
        dr["Price"] = 99.99m;

        // Act
        string? result = dr.GetStringValue(10);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试DataRow为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void GetStringValue_WithNullDataRow_ThrowsArgumentNullException_Test()
    {
        // Arrange
        DataRow? dr = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => dr!.GetStringValue(0));
    }

    /// <summary>
    /// 测试获取数字类型的字符串表示
    /// </summary>
    [TestMethod]
    public void GetStringValue_WithNumericValue_ReturnsStringRepresentation_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();
        DataRow dr = dt.NewRow();
        dr["Id"] = 123;
        dr["Name"] = "TestValue";
        dr["Price"] = 99.99m;

        // Act
        string? result = dr.GetStringValue(0);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("123", result);
    }

    #endregion

    #region ToList Tests

    /// <summary>
    /// 测试将DataTable转换为对象列表
    /// </summary>
    [TestMethod]
    public void ToList_WithValidDataTable_ReturnsObjectList_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();
        for (int i = 1; i <= 3; i++)
        {
            DataRow dr = dt.NewRow();
            dr["Id"] = i;
            dr["Name"] = $"Item{i}";
            dr["Price"] = i * 10.5m;
            dt.Rows.Add(dr);
        }

        // Act
        List<SimpleModel> result = dt.ToList<SimpleModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(3, result);
        Assert.AreEqual(1, result[0].Id);
        Assert.AreEqual("Item2", result[1].Name);
        Assert.AreEqual(31.5m, result[2].Price);
    }

    /// <summary>
    /// 测试空DataTable转换为空列表
    /// </summary>
    [TestMethod]
    public void ToList_WithEmptyDataTable_ReturnsEmptyList_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();

        // Act
        List<SimpleModel> result = dt.ToList<SimpleModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 测试DataTable为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void ToList_WithNullDataTable_ThrowsArgumentNullException_Test()
    {
        // Arrange
        DataTable? dt = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => dt!.ToList<SimpleModel>());
    }

    /// <summary>
    /// 测试转换异常时收集到exceptions列表
    /// </summary>
    [TestMethod]
    public void ToList_WithConversionError_CollectsExceptions_Test()
    {
        // Arrange
        DataTable dt = new();
        dt.Columns.Add("Id", typeof(object));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Price", typeof(decimal));
        DataRow dr = dt.NewRow();
        dr["Id"] = "InvalidNumber";
        dr["Name"] = "TestItem";
        dr["Price"] = 50.5m;
        dt.Rows.Add(dr);
        List<Exception> exceptions = [];

        // Act
        List<SimpleModel> result = dt.ToList<SimpleModel>(exceptions);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(exceptions);
    }

    #endregion

    #region ToArray Tests

    /// <summary>
    /// 测试将DataTable转换为对象数组
    /// </summary>
    [TestMethod]
    public void ToArray_WithValidDataTable_ReturnsObjectArray_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();
        for (int i = 1; i <= 3; i++)
        {
            DataRow dr = dt.NewRow();
            dr["Id"] = i;
            dr["Name"] = $"Item{i}";
            dr["Price"] = i * 10.5m;
            dt.Rows.Add(dr);
        }

        // Act
        SimpleModel?[] result = dt.ToArray<SimpleModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(3, result);
        Assert.IsNotNull(result[0]);
        Assert.AreEqual(1, result[0]!.Id);
        Assert.AreEqual("Item2", result[1]!.Name);
    }

    /// <summary>
    /// 测试空DataTable转换为空数组
    /// </summary>
    [TestMethod]
    public void ToArray_WithEmptyDataTable_ReturnsEmptyArray_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();

        // Act
        SimpleModel?[] result = dt.ToArray<SimpleModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 测试DataTable为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void ToArray_WithNullDataTable_ThrowsArgumentNullException_Test()
    {
        // Arrange
        DataTable? dt = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => dt!.ToArray<SimpleModel>());
    }

    #endregion

    #region DataSet ToList Tests

    /// <summary>
    /// 测试将DataSet转换为对象列表的列表
    /// </summary>
    [TestMethod]
    public void ToList_WithValidDataSet_ReturnsListOfLists_Test()
    {
        // Arrange
        DataSet ds = new();
        for (int tableIndex = 0; tableIndex < 2; tableIndex++)
        {
            DataTable dt = typeof(SimpleModel).ToDataTable();
            dt.TableName = $"Table{tableIndex}";
            for (int i = 1; i <= 2; i++)
            {
                DataRow dr = dt.NewRow();
                dr["Id"] = i;
                dr["Name"] = $"Table{tableIndex}_Item{i}";
                dr["Price"] = i * 10.5m;
                dt.Rows.Add(dr);
            }
            ds.Tables.Add(dt);
        }

        // Act
        List<List<SimpleModel>> result = ds.ToList<SimpleModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);
        Assert.HasCount(2, result[0]);
        Assert.AreEqual("Table0_Item1", result[0][0].Name);
        Assert.AreEqual("Table1_Item2", result[1][1].Name);
    }

    /// <summary>
    /// 测试空DataSet转换为空列表
    /// </summary>
    [TestMethod]
    public void ToList_WithEmptyDataSet_ReturnsEmptyList_Test()
    {
        // Arrange
        DataSet ds = new();

        // Act
        List<List<SimpleModel>> result = ds.ToList<SimpleModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 测试DataSet为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void ToList_WithNullDataSet_ThrowsArgumentNullException_Test()
    {
        // Arrange
        DataSet? ds = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => ds!.ToList<SimpleModel>());
    }

    #endregion

    #region DataSet ToArray Tests

    /// <summary>
    /// 测试将DataSet转换为二维对象数组
    /// </summary>
    [TestMethod]
    public void ToArray_WithValidDataSet_ReturnsTwoDimensionalArray_Test()
    {
        // Arrange
        DataSet ds = new();
        for (int tableIndex = 0; tableIndex < 2; tableIndex++)
        {
            DataTable dt = typeof(SimpleModel).ToDataTable();
            dt.TableName = $"Table{tableIndex}";
            for (int i = 1; i <= 3; i++)
            {
                DataRow dr = dt.NewRow();
                dr["Id"] = i;
                dr["Name"] = $"Table{tableIndex}_Item{i}";
                dr["Price"] = i * 10.5m;
                dt.Rows.Add(dr);
            }
            ds.Tables.Add(dt);
        }

        // Act
        SimpleModel?[,] result = ds.ToArray<SimpleModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.GetLength(0));
        Assert.AreEqual(3, result.GetLength(1));
        Assert.IsNotNull(result[0, 0]);
        Assert.AreEqual("Table0_Item1", result[0, 0]!.Name);
        Assert.AreEqual("Table1_Item2", result[1, 1]!.Name);
    }

    /// <summary>
    /// 测试DataSet包含不同行数的表时正确处理
    /// </summary>
    [TestMethod]
    public void ToArray_WithDifferentRowCounts_HandlesCorrectly_Test()
    {
        // Arrange
        DataSet ds = new();

        DataTable dt1 = typeof(SimpleModel).ToDataTable();
        dt1.TableName = "Table1";
        for (int i = 1; i <= 2; i++)
        {
            DataRow dr = dt1.NewRow();
            dr["Id"] = i;
            dr["Name"] = $"Item{i}";
            dr["Price"] = i * 10.5m;
            dt1.Rows.Add(dr);
        }
        ds.Tables.Add(dt1);

        DataTable dt2 = typeof(SimpleModel).ToDataTable();
        dt2.TableName = "Table2";
        for (int i = 1; i <= 4; i++)
        {
            DataRow dr = dt2.NewRow();
            dr["Id"] = i;
            dr["Name"] = $"Item{i}";
            dr["Price"] = i * 10.5m;
            dt2.Rows.Add(dr);
        }
        ds.Tables.Add(dt2);

        // Act
        SimpleModel?[,] result = ds.ToArray<SimpleModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.GetLength(0));
        Assert.AreEqual(4, result.GetLength(1));
        Assert.IsNotNull(result[0, 0]);
        Assert.IsNotNull(result[0, 1]);
        Assert.IsNull(result[0, 2]);
        Assert.IsNull(result[0, 3]);
    }

    /// <summary>
    /// 测试DataSet为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void ToArray_WithNullDataSet_ThrowsArgumentNullException_Test()
    {
        // Arrange
        DataSet? ds = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => ds!.ToArray<SimpleModel>());
    }

    #endregion

    #region ToDictionaries Tests

    /// <summary>
    /// 测试将DataTable转换为字典列表
    /// </summary>
    [TestMethod]
    public void ToDictionaries_WithValidDataTable_ReturnsDictionaryList_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();
        for (int i = 1; i <= 2; i++)
        {
            DataRow dr = dt.NewRow();
            dr["Id"] = i;
            dr["Name"] = $"Item{i}";
            dr["Price"] = i * 10.5m;
            dt.Rows.Add(dr);
        }

        // Act
        List<Dictionary<string, object?>> result = dt.ToDictionaries();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);
        Assert.HasCount(3, result[0]);
        Assert.AreEqual(1, result[0]["Id"]);
        Assert.AreEqual("Item1", result[0]["Name"]);
        Assert.AreEqual(10.5m, result[0]["Price"]);
    }

    /// <summary>
    /// 测试使用自定义转换函数
    /// </summary>
    [TestMethod]
    public void ToDictionaries_WithCustomFunc_AppliesCustomConversion_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();
        DataRow dr = dt.NewRow();
        dr["Id"] = 1;
        dr["Name"] = "Item1";
        dr["Price"] = 10.5m;
        dt.Rows.Add(dr);

        // Act
        List<Dictionary<string, object?>> result = dt.ToDictionaries(value =>
            value is string str ? str.ToUpper() : value);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(1, result);
        Assert.AreEqual("ITEM1", result[0]["Name"]);
        Assert.AreEqual(1, result[0]["Id"]);
    }

    /// <summary>
    /// 测试空DataTable转换为空字典列表
    /// </summary>
    [TestMethod]
    public void ToDictionaries_WithEmptyDataTable_ReturnsEmptyList_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();

        // Act
        List<Dictionary<string, object?>> result = dt.ToDictionaries();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 测试DataTable为null时抛出ArgumentNullException
    /// </summary>
    [TestMethod]
    public void ToDictionaries_WithNullDataTable_ThrowsArgumentNullException_Test()
    {
        // Arrange
        DataTable? dt = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => dt!.ToDictionaries());
    }

    /// <summary>
    /// 测试自定义函数返回null值
    /// </summary>
    [TestMethod]
    public void ToDictionaries_WithCustomFuncReturningNull_HandlesNullValues_Test()
    {
        // Arrange
        DataTable dt = typeof(SimpleModel).ToDataTable();
        DataRow dr = dt.NewRow();
        dr["Id"] = 1;
        dr["Name"] = "Item1";
        dr["Price"] = 10.5m;
        dt.Rows.Add(dr);

        // Act
        List<Dictionary<string, object?>> result = dt.ToDictionaries(value =>
            value is string ? null : value);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(1, result);
        Assert.IsNull(result[0]["Name"]);
        Assert.AreEqual(1, result[0]["Id"]);
    }

    #endregion

    #region 复杂场景测试

    /// <summary>
    /// 测试包含可空类型的模型转换
    /// </summary>
    [TestMethod]
    public void ToDataTable_WithNullableProperties_HandlesCorrectly_Test()
    {
        // Arrange
        List<NullableModel> list =
        [
            new NullableModel { Id = 1, Name = "Test1", Age = 25, BirthDate = DateTime.Now },
            new NullableModel { Id = 2, Name = null, Age = null, BirthDate = null }
        ];

        // Act
        DataTable result = list.ToDataTable();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result.Rows);
        Assert.AreEqual("Test1", result.Rows[0]["Name"]);
        Assert.AreEqual(DBNull.Value, result.Rows[1]["Name"]);
        Assert.AreEqual(DBNull.Value, result.Rows[1]["Age"]);
    }

    /// <summary>
    /// 测试完整的往返转换（对象->DataTable->对象）
    /// </summary>
    [TestMethod]
    public void RoundTrip_ObjectToDataTableToObject_PreservesData_Test()
    {
        // Arrange
        List<ComplexModel> originalList =
        [
            new ComplexModel
            {
                Id = 1,
                Name = "Test1",
                IsActive = true,
                CreateTime = new DateTime(2024, 1, 1),
                Amount = 100.50m
            },
            new ComplexModel
            {
                Id = 2,
                Name = "Test2",
                IsActive = false,
                CreateTime = new DateTime(2024, 2, 1),
                Amount = 200.75m
            }
        ];

        // Act
        DataTable dt = originalList.ToDataTable();
        List<ComplexModel> resultList = dt.ToList<ComplexModel>();

        // Assert
        Assert.HasCount(originalList.Count, resultList);
        for (int i = 0; i < originalList.Count; i++)
        {
            Assert.AreEqual(originalList[i].Id, resultList[i].Id);
            Assert.AreEqual(originalList[i].Name, resultList[i].Name);
            Assert.AreEqual(originalList[i].IsActive, resultList[i].IsActive);
            Assert.AreEqual(originalList[i].CreateTime, resultList[i].CreateTime);
            Assert.AreEqual(originalList[i].Amount, resultList[i].Amount);
        }
    }

    #endregion
}
