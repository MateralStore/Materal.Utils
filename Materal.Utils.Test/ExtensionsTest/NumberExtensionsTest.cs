namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// NumberExtensions 测试类
/// 测试数字扩展方法的功能
/// </summary>
[TestClass]
public class NumberExtensionsTest
{
    #region ConvertToSimplifiedChinese Tests

    /// <summary>
    /// 测试零转换为简体中文
    /// </summary>
    [TestMethod]
    public void ConvertToSimplifiedChinese_WithZero_ReturnsZero_Test()
    {
        // Arrange
        int number = 0;

        // Act
        string result = number.ConvertToSimplifiedChinese();

        // Assert
        Assert.AreEqual("零", result);
    }

    /// <summary>
    /// 测试个位数转换为简体中文
    /// </summary>
    [TestMethod]
    public void ConvertToSimplifiedChinese_WithSingleDigit_ReturnsCorrectChinese_Test()
    {
        // Arrange & Act & Assert
        Assert.AreEqual("一", 1.ConvertToSimplifiedChinese());
        Assert.AreEqual("五", 5.ConvertToSimplifiedChinese());
        Assert.AreEqual("九", 9.ConvertToSimplifiedChinese());
    }

    /// <summary>
    /// 测试十位数转换为简体中文
    /// </summary>
    [TestMethod]
    public void ConvertToSimplifiedChinese_WithTens_ReturnsCorrectChinese_Test()
    {
        // Arrange & Act & Assert
        Assert.AreEqual("一十", 10.ConvertToSimplifiedChinese());
        Assert.AreEqual("二十", 20.ConvertToSimplifiedChinese());
        Assert.AreEqual("九十九", 99.ConvertToSimplifiedChinese());
    }

    /// <summary>
    /// 测试百位数转换为简体中文
    /// </summary>
    [TestMethod]
    public void ConvertToSimplifiedChinese_WithHundreds_ReturnsCorrectChinese_Test()
    {
        // Arrange & Act & Assert
        Assert.AreEqual("一百", 100.ConvertToSimplifiedChinese());
        Assert.AreEqual("一百零一", 101.ConvertToSimplifiedChinese());
        Assert.AreEqual("一百一十", 110.ConvertToSimplifiedChinese());
    }

    /// <summary>
    /// 测试千位数转换为简体中文
    /// </summary>
    [TestMethod]
    public void ConvertToSimplifiedChinese_WithThousands_ReturnsCorrectChinese_Test()
    {
        // Arrange & Act & Assert
        Assert.AreEqual("一千", 1000.ConvertToSimplifiedChinese());
        Assert.AreEqual("一千零一", 1001.ConvertToSimplifiedChinese());
    }

    /// <summary>
    /// 测试万位数转换为简体中文
    /// </summary>
    [TestMethod]
    public void ConvertToSimplifiedChinese_WithTenThousands_ReturnsCorrectChinese_Test()
    {
        // Arrange
        int number = 10000;

        // Act
        string result = number.ConvertToSimplifiedChinese();

        // Assert
        Assert.Contains("万", result);
    }

    /// <summary>
    /// 测试负数转换为简体中文
    /// </summary>
    [TestMethod]
    public void ConvertToSimplifiedChinese_WithNegativeNumber_ReturnsNegativeChinese_Test()
    {
        // Arrange
        int number = -123;

        // Act
        string result = number.ConvertToSimplifiedChinese();

        // Assert
        Assert.StartsWith("负", result);
    }

    #endregion

    #region ConvertToCapitalChinese Tests

    /// <summary>
    /// 测试零转换为大写中文
    /// </summary>
    [TestMethod]
    public void ConvertToCapitalChinese_WithZero_ReturnsZero_Test()
    {
        // Arrange
        int number = 0;

        // Act
        string result = number.ConvertToCapitalChinese();

        // Assert
        Assert.AreEqual("零", result);
    }

    /// <summary>
    /// 测试个位数转换为大写中文
    /// </summary>
    [TestMethod]
    public void ConvertToCapitalChinese_WithSingleDigit_ReturnsCorrectChinese_Test()
    {
        // Arrange & Act & Assert
        Assert.AreEqual("壹", 1.ConvertToCapitalChinese());
        Assert.AreEqual("伍", 5.ConvertToCapitalChinese());
        Assert.AreEqual("玖", 9.ConvertToCapitalChinese());
    }

    /// <summary>
    /// 测试十位数转换为大写中文
    /// </summary>
    [TestMethod]
    public void ConvertToCapitalChinese_WithTens_ReturnsCorrectChinese_Test()
    {
        // Arrange & Act & Assert
        Assert.AreEqual("壹拾", 10.ConvertToCapitalChinese());
        Assert.AreEqual("贰拾", 20.ConvertToCapitalChinese());
    }

    /// <summary>
    /// 测试百位数转换为大写中文
    /// </summary>
    [TestMethod]
    public void ConvertToCapitalChinese_WithHundreds_ReturnsCorrectChinese_Test()
    {
        // Arrange & Act & Assert
        Assert.AreEqual("壹佰", 100.ConvertToCapitalChinese());
        Assert.Contains("佰", 101.ConvertToCapitalChinese());
    }

    /// <summary>
    /// 测试千位数转换为大写中文
    /// </summary>
    [TestMethod]
    public void ConvertToCapitalChinese_WithThousands_ReturnsCorrectChinese_Test()
    {
        // Arrange
        int number = 1000;

        // Act
        string result = number.ConvertToCapitalChinese();

        // Assert
        Assert.Contains("仟", result);
    }

    /// <summary>
    /// 测试负数转换为大写中文
    /// </summary>
    [TestMethod]
    public void ConvertToCapitalChinese_WithNegativeNumber_ReturnsNegativeChinese_Test()
    {
        // Arrange
        int number = -456;

        // Act
        string result = number.ConvertToCapitalChinese();

        // Assert
        Assert.StartsWith("负", result);
    }

    #endregion

    #region GetBinaryString Tests

    /// <summary>
    /// 测试零转换为二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryString_WithZero_ReturnsZero_Test()
    {
        // Arrange
        int number = 0;

        // Act
        string result = number.GetBinaryString();

        // Assert
        Assert.AreEqual("0", result);
    }

    /// <summary>
    /// 测试正整数转换为二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryString_WithPositiveInteger_ReturnsBinaryString_Test()
    {
        // Arrange & Act & Assert
        Assert.AreEqual("1", 1.GetBinaryString());
        Assert.AreEqual("10", 2.GetBinaryString());
        Assert.AreEqual("11", 3.GetBinaryString());
        Assert.AreEqual("100", 4.GetBinaryString());
        Assert.AreEqual("1000", 8.GetBinaryString());
    }

    /// <summary>
    /// 测试较大数字转换为二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryString_WithLargeNumber_ReturnsBinaryString_Test()
    {
        // Arrange
        int number = 255;

        // Act
        string result = number.GetBinaryString();

        // Assert
        Assert.AreEqual("11111111", result);
    }

    /// <summary>
    /// 测试负数转换为二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryString_WithNegativeNumber_ReturnsBinaryString_Test()
    {
        // Arrange
        int number = -1;

        // Act
        string result = number.GetBinaryString();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThan(0, result.Length);
    }

    /// <summary>
    /// 测试2的幂次转换为二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryString_WithPowerOfTwo_ReturnsBinaryString_Test()
    {
        // Arrange & Act & Assert
        Assert.AreEqual("10000", 16.GetBinaryString());
        Assert.AreEqual("100000", 32.GetBinaryString());
        Assert.AreEqual("1000000", 64.GetBinaryString());
        Assert.AreEqual("10000000", 128.GetBinaryString());
    }

    #endregion
}
