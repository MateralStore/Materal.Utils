using Materal.Utils.Helpers;

namespace Materal.Utils.Test.HelpersTest;

/// <summary>
/// ConvertHelper 测试类
/// 测试类型转换功能
/// </summary>
[TestClass]
public class ConvertHelperTest
{
    #region ConvertTo<bool?> Tests

    /// <summary>
    /// 测试将字符串转换为布尔值
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithStringTrue_ReturnsTrue_Test()
    {
        // Arrange
        string input = "true";

        // Act
        bool? result = ConvertHelper.ConvertTo<bool?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Value);
    }

    /// <summary>
    /// 测试将字符串转换为布尔值（false）
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithStringFalse_ReturnsFalse_Test()
    {
        // Arrange
        string input = "false";

        // Act
        bool? result = ConvertHelper.ConvertTo<bool?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Value);
    }

    /// <summary>
    /// 测试将数字转换为布尔值
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithNonZeroNumber_ReturnsTrue_Test()
    {
        // Arrange
        int input = 1;

        // Act
        bool? result = ConvertHelper.ConvertTo<bool?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Value);
    }

    /// <summary>
    /// 测试将零转换为布尔值
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithZeroNumber_ReturnsFalse_Test()
    {
        // Arrange
        int input = 0;

        // Act
        bool? result = ConvertHelper.ConvertTo<bool?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Value);
    }

    #endregion

    #region ConvertTo<int?> Tests

    /// <summary>
    /// 测试将字符串转换为整数
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithStringNumber_ReturnsInt_Test()
    {
        // Arrange
        string input = "123";

        // Act
        int? result = ConvertHelper.ConvertTo<int?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(123, result.Value);
    }

    /// <summary>
    /// 测试将浮点数转换为整数
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithDouble_ReturnsInt_Test()
    {
        // Arrange
        double input = 123.7;

        // Act
        int? result = ConvertHelper.ConvertTo<int?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(124, result.Value); // Should round
    }

    /// <summary>
    /// 测试将布尔值转换为整数
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithTrue_ReturnsOne_Test()
    {
        // Arrange
        bool input = true;

        // Act
        int? result = ConvertHelper.ConvertTo<int?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Value);
    }

    /// <summary>
    /// 测试将布尔值（false）转换为整数
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithFalse_ReturnsZero_Test()
    {
        // Arrange
        bool input = false;

        // Act
        int? result = ConvertHelper.ConvertTo<int?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Value);
    }

    #endregion

    #region ConvertTo<long?> Tests

    /// <summary>
    /// 测试将字符串转换为长整数
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithStringNumber_ReturnsLong_Test()
    {
        // Arrange
        string input = "9223372036854775807";

        // Act
        long? result = ConvertHelper.ConvertTo<long?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(9223372036854775807L, result.Value);
    }

    #endregion

    #region ConvertTo<double?> Tests

    /// <summary>
    /// 测试将字符串转换为双精度浮点数
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithStringNumber_ReturnsDouble_Test()
    {
        // Arrange
        string input = "123.456";

        // Act
        double? result = ConvertHelper.ConvertTo<double?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(123.456, result.Value, 0.001);
    }

    #endregion

    #region ConvertTo<decimal?> Tests

    /// <summary>
    /// 测试将字符串转换为小数
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithStringNumber_ReturnsDecimal_Test()
    {
        // Arrange
        string input = "123.45";

        // Act
        decimal? result = ConvertHelper.ConvertTo<decimal?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(123.45m, result.Value);
    }

    #endregion

    #region ConvertTo<Guid?> Tests

    /// <summary>
    /// 测试将字符串转换为 GUID
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithValidGuidString_ReturnsGuid_Test()
    {
        // Arrange
        string input = "d1d3f4a5-6b7c-8d9e-0f1a-2b3c4d5e6f7a";

        // Act
        Guid? result = ConvertHelper.ConvertTo<Guid?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(new Guid("d1d3f4a5-6b7c-8d9e-0f1a-2b3c4d5e6f7a"), result.Value);
    }

    /// <summary>
    /// 测试将无效 GUID 字符串转换为 GUID
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithInvalidGuidString_ReturnsNull_Test()
    {
        // Arrange
        string input = "invalid-guid-string";

        // Act
        Guid? result = ConvertHelper.ConvertTo<Guid?>(input);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试将 null 字符串转换为可空 GUID
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithNullString_ReturnsNull_Test()
    {
        // Arrange
        string? input = null;

        // Act
        Guid? result = ConvertHelper.ConvertTo<Guid?>(input!);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region ConvertTo<string?> Tests

    /// <summary>
    /// 测试将数字转换为字符串
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithInt_ReturnsString_Test()
    {
        // Arrange
        int input = 123;

        // Act
        string? result = ConvertHelper.ConvertTo<string?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("123", result);
    }

    /// <summary>
    /// 测试将 GUID 转换为字符串
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithGuid_ReturnsString_Test()
    {
        // Arrange
        Guid input = new("d1d3f4a5-6b7c-8d9e-0f1a-2b3c4d5e6f7a");

        // Act
        string? result = ConvertHelper.ConvertTo<string?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("d1d3f4a5-6b7c-8d9e-0f1a-2b3c4d5e6f7a", result);
    }

    /// <summary>
    /// 测试将 null 转换为字符串
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithNull_ReturnsNull_Test()
    {
        // Arrange
        object? input = null;

        // Act
        string? result = ConvertHelper.ConvertTo<string?>(input!);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region ConvertTo<DateTime?> Tests

    /// <summary>
    /// 测试将字符串转换为 DateTime
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithDateTimeString_ReturnsDateTime_Test()
    {
        // Arrange
        string input = "2021-01-01 12:00:00";

        // Act
        DateTime? result = ConvertHelper.ConvertTo<DateTime?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2021, result.Value.Year);
        Assert.AreEqual(1, result.Value.Month);
        Assert.AreEqual(1, result.Value.Day);
    }

    /// <summary>
    /// 测试将时间戳字符串转换为 DateTime
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithTimestampString_ReturnsDateTime_Test()
    {
        // Arrange
        string input = "1609459200";

        // Act
        DateTime? result = ConvertHelper.ConvertTo<DateTime?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2021, result.Value.Year);
        Assert.AreEqual(1, result.Value.Month);
    }

    #endregion

    #region CanConvertTo Tests

    /// <summary>
    /// 测试检查是否可转换为指定类型
    /// </summary>
    [TestMethod]
    public void CanConvertTo_WithSupportedType_ReturnsTrue_Test()
    {
        // Act
        bool canConvertToInt = ConvertHelper.CanConvertTo(typeof(int));
        bool canConvertToString = ConvertHelper.CanConvertTo(typeof(string));
        bool canConvertToDateTime = ConvertHelper.CanConvertTo(typeof(DateTime));

        // Assert
        Assert.IsTrue(canConvertToInt);
        Assert.IsTrue(canConvertToString);
        Assert.IsTrue(canConvertToDateTime);
    }

    #endregion

    #region Null and Empty String Handling

    /// <summary>
    /// 测试将 null 转换为值类型时抛出异常
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithNullToValueType_ThrowsArgumentNullException_Test()
    {
        // Arrange
        object? input = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => ConvertHelper.ConvertTo<int>(input!));
    }

    /// <summary>
    /// 测试将空白字符串转换为字符串时返回 null
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithWhitespaceStringToType_ReturnsNull_Test()
    {
        // Arrange
        string input = "   ";

        // Act
        int? result = ConvertHelper.ConvertTo<int?>(input);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试将空白字符串转换为字符串类型时返回字符串本身
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithWhitespaceStringToString_ReturnsString_Test()
    {
        // Arrange
        string input = "   ";

        // Act
        string? result = ConvertHelper.ConvertTo<string?>(input);

        // Assert
        Assert.AreEqual(input, result);
    }

    #endregion

    #region Same Type Conversion

    /// <summary>
    /// 测试将对象转换为其自身类型
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithSameType_ReturnsSameObject_Test()
    {
        // Arrange
        int input = 123;

        // Act
        int? result = ConvertHelper.ConvertTo<int?>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(input, result.Value);
    }

    /// <summary>
    /// 测试将对象转换为其实现的接口类型
    /// </summary>
    [TestMethod]
    public void ConvertTo_WithAssignableType_ReturnsSameObject_Test()
    {
        // Arrange
        string input = "test";

        // Act
        object? result = ConvertHelper.ConvertTo<object>(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreSame(input, result);
    }

    #endregion
}
