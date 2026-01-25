namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// StringExtensions.Convert 测试类
/// 测试字符串转换扩展方法的功能
/// </summary>
[TestClass]
public class StringExtensionsConvertTest
{
    #region 测试枚举

    /// <summary>
    /// 测试枚举
    /// </summary>
    public enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }

    /// <summary>
    /// 带标志的枚举
    /// </summary>
    [Flags]
    public enum TestFlagsEnum
    {
        None = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag3 = 4
    }

    #endregion

    #region ConvertToEnum Tests

    /// <summary>
    /// 测试转换为枚举值
    /// </summary>
    [TestMethod]
    public void ConvertToEnum_WithValidValue_ReturnsEnum_Test()
    {
        // Arrange
        string value = "Value1";

        // Act
        TestEnum result = value.ConvertToEnum<TestEnum>();

        // Assert
        Assert.AreEqual(TestEnum.Value1, result);
    }

    /// <summary>
    /// 测试忽略大小写转换
    /// </summary>
    [TestMethod]
    public void ConvertToEnum_WithIgnoreCase_ReturnsEnum_Test()
    {
        // Arrange
        string value = "value2";

        // Act
        TestEnum result = value.ConvertToEnum<TestEnum>();

        // Assert
        Assert.AreEqual(TestEnum.Value2, result);
    }

    /// <summary>
    /// 测试不忽略大小写转换失败
    /// </summary>
    [TestMethod]
    public void ConvertToEnum_WithCaseSensitive_ThrowsException_Test()
    {
        // Arrange
        string value = "value3";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => value.ConvertToEnum<TestEnum>(false));
    }

    /// <summary>
    /// 测试无效值抛出异常
    /// </summary>
    [TestMethod]
    public void ConvertToEnum_WithInvalidValue_ThrowsException_Test()
    {
        // Arrange
        string value = "InvalidValue";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => value.ConvertToEnum<TestEnum>());
    }

    /// <summary>
    /// 测试数字字符串转换
    /// </summary>
    [TestMethod]
    public void ConvertToEnum_WithNumericValue_ReturnsEnum_Test()
    {
        // Arrange
        string value = "1";

        // Act
        TestEnum result = value.ConvertToEnum<TestEnum>();

        // Assert
        Assert.AreEqual(TestEnum.Value2, result);
    }

    /// <summary>
    /// 测试标志枚举转换
    /// </summary>
    [TestMethod]
    public void ConvertToEnum_WithFlagsEnum_ReturnsEnum_Test()
    {
        // Arrange
        string value = "Flag1";

        // Act
        TestFlagsEnum result = value.ConvertToEnum<TestFlagsEnum>();

        // Assert
        Assert.AreEqual(TestFlagsEnum.Flag1, result);
    }

    #endregion

    #region ToLowerFirstLetter Tests

    /// <summary>
    /// 测试首字母转小写
    /// </summary>
    [TestMethod]
    public void ToLowerFirstLetter_WithUpperCase_ReturnsLowerFirstLetter_Test()
    {
        // Arrange
        string input = "Hello";

        // Act
        string result = input.ToLowerFirstLetter();

        // Assert
        Assert.AreEqual("hello", result);
    }

    /// <summary>
    /// 测试已经是小写的字符串
    /// </summary>
    [TestMethod]
    public void ToLowerFirstLetter_WithLowerCase_ReturnsUnchanged_Test()
    {
        // Arrange
        string input = "hello";

        // Act
        string result = input.ToLowerFirstLetter();

        // Assert
        Assert.AreEqual("hello", result);
    }

    /// <summary>
    /// 测试单个字符
    /// </summary>
    [TestMethod]
    public void ToLowerFirstLetter_WithSingleChar_ReturnsLowerCase_Test()
    {
        // Arrange
        string input = "A";

        // Act
        string result = input.ToLowerFirstLetter();

        // Assert
        Assert.AreEqual("a", result);
    }

    /// <summary>
    /// 测试空字符串返回原值
    /// </summary>
    [TestMethod]
    public void ToLowerFirstLetter_WithEmptyString_ReturnsEmpty_Test()
    {
        // Arrange
        string input = string.Empty;

        // Act
        string result = input.ToLowerFirstLetter();

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// 测试空白字符串返回原值
    /// </summary>
    [TestMethod]
    public void ToLowerFirstLetter_WithWhiteSpace_ReturnsWhiteSpace_Test()
    {
        // Arrange
        string input = "   ";

        // Act
        string result = input.ToLowerFirstLetter();

        // Assert
        Assert.AreEqual("   ", result);
    }

    /// <summary>
    /// 测试多个单词
    /// </summary>
    [TestMethod]
    public void ToLowerFirstLetter_WithMultipleWords_OnlyFirstLetterLower_Test()
    {
        // Arrange
        string input = "HelloWorld";

        // Act
        string result = input.ToLowerFirstLetter();

        // Assert
        Assert.AreEqual("helloWorld", result);
    }

    /// <summary>
    /// 测试数字开头
    /// </summary>
    [TestMethod]
    public void ToLowerFirstLetter_WithNumberFirst_ReturnsUnchanged_Test()
    {
        // Arrange
        string input = "123Test";

        // Act
        string result = input.ToLowerFirstLetter();

        // Assert
        Assert.AreEqual("123Test", result);
    }

    #endregion

    #region ToUpperFirstLetter Tests

    /// <summary>
    /// 测试首字母转大写
    /// </summary>
    [TestMethod]
    public void ToUpperFirstLetter_WithLowerCase_ReturnsUpperFirstLetter_Test()
    {
        // Arrange
        string input = "hello";

        // Act
        string result = input.ToUpperFirstLetter();

        // Assert
        Assert.AreEqual("Hello", result);
    }

    /// <summary>
    /// 测试已经是大写的字符串
    /// </summary>
    [TestMethod]
    public void ToUpperFirstLetter_WithUpperCase_ReturnsUnchanged_Test()
    {
        // Arrange
        string input = "Hello";

        // Act
        string result = input.ToUpperFirstLetter();

        // Assert
        Assert.AreEqual("Hello", result);
    }

    /// <summary>
    /// 测试单个字符
    /// </summary>
    [TestMethod]
    public void ToUpperFirstLetter_WithSingleChar_ReturnsUpperCase_Test()
    {
        // Arrange
        string input = "a";

        // Act
        string result = input.ToUpperFirstLetter();

        // Assert
        Assert.AreEqual("A", result);
    }

    /// <summary>
    /// 测试空字符串返回原值
    /// </summary>
    [TestMethod]
    public void ToUpperFirstLetter_WithEmptyString_ReturnsEmpty_Test()
    {
        // Arrange
        string input = string.Empty;

        // Act
        string result = input.ToUpperFirstLetter();

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// 测试空白字符串返回原值
    /// </summary>
    [TestMethod]
    public void ToUpperFirstLetter_WithWhiteSpace_ReturnsWhiteSpace_Test()
    {
        // Arrange
        string input = "   ";

        // Act
        string result = input.ToUpperFirstLetter();

        // Assert
        Assert.AreEqual("   ", result);
    }

    /// <summary>
    /// 测试多个单词
    /// </summary>
    [TestMethod]
    public void ToUpperFirstLetter_WithMultipleWords_OnlyFirstLetterUpper_Test()
    {
        // Arrange
        string input = "helloWorld";

        // Act
        string result = input.ToUpperFirstLetter();

        // Assert
        Assert.AreEqual("HelloWorld", result);
    }

    /// <summary>
    /// 测试数字开头
    /// </summary>
    [TestMethod]
    public void ToUpperFirstLetter_WithNumberFirst_ReturnsUnchanged_Test()
    {
        // Arrange
        string input = "123test";

        // Act
        string result = input.ToUpperFirstLetter();

        // Assert
        Assert.AreEqual("123test", result);
    }

    /// <summary>
    /// 测试中文字符
    /// </summary>
    [TestMethod]
    public void ToUpperFirstLetter_WithChinese_ReturnsUnchanged_Test()
    {
        // Arrange
        string input = "你好世界";

        // Act
        string result = input.ToUpperFirstLetter();

        // Assert
        Assert.AreEqual("你好世界", result);
    }

    #endregion

    #region 边界条件测试

    /// <summary>
    /// 测试null字符串转换枚举抛出异常
    /// </summary>
    [TestMethod]
    public void ConvertToEnum_WithNull_ThrowsException_Test()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => value!.ConvertToEnum<TestEnum>());
    }

    /// <summary>
    /// 测试空字符串转换枚举抛出异常
    /// </summary>
    [TestMethod]
    public void ConvertToEnum_WithEmptyString_ThrowsException_Test()
    {
        // Arrange
        string value = string.Empty;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => value.ConvertToEnum<TestEnum>());
    }

    /// <summary>
    /// 测试组合标志枚举
    /// </summary>
    [TestMethod]
    public void ConvertToEnum_WithCombinedFlags_ReturnsEnum_Test()
    {
        // Arrange
        string value = "Flag1, Flag2";

        // Act
        TestFlagsEnum result = value.ConvertToEnum<TestFlagsEnum>();

        // Assert
        Assert.AreEqual(TestFlagsEnum.Flag1 | TestFlagsEnum.Flag2, result);
    }

    #endregion
}
