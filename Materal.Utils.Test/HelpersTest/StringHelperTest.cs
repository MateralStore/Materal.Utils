using Materal.Utils.Helpers;

namespace Materal.Utils.Test.HelpersTest;

/// <summary>
/// StringHelper 测试类
/// 测试随机字符串生成功能
/// </summary>
[TestClass]
public class StringHelperTest
{
    #region GetRandomStringByGuid Tests

    /// <summary>
    /// 测试使用 GUID 生成随机字符串（固定长度）
    /// </summary>
    [TestMethod]
    public void GetRandomStringByGuid_WithLength_ReturnsStringOfLength_Test()
    {
        // Arrange
        int length = 32;

        // Act
        string result = StringHelper.GetRandomStringByGuid(length);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(length, result.Length);
    }

    /// <summary>
    /// 测试使用 GUID 生成随机字符串（可变长度）
    /// </summary>
    [TestMethod]
    public void GetRandomStringByGuid_WithVariableLength_ReturnsStringInRange_Test()
    {
        // Arrange
        int minLength = 10;
        int maxLength = 50;

        // Act
        string result = StringHelper.GetRandomStringByGuid(minLength, maxLength);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThanOrEqualTo(minLength, result.Length);
        Assert.IsLessThan(maxLength, result.Length);
    }

    /// <summary>
    /// 测试使用无效长度（<= 0）生成字符串时抛出异常
    /// </summary>
    [TestMethod]
    public void GetRandomStringByGuid_WithZeroLength_ThrowsUtilException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => StringHelper.GetRandomStringByGuid(0));
    }

    /// <summary>
    /// 测试使用负数长度生成字符串时抛出异常
    /// </summary>
    [TestMethod]
    public void GetRandomStringByGuid_WithNegativeLength_ThrowsUtilException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => StringHelper.GetRandomStringByGuid(-10));
    }

    /// <summary>
    /// 测试多次调用生成不同的随机字符串
    /// </summary>
    [TestMethod]
    public void GetRandomStringByGuid_CalledMultipleTimes_ReturnsDifferentStrings_Test()
    {
        // Act
        string result1 = StringHelper.GetRandomStringByGuid(32);
        string result2 = StringHelper.GetRandomStringByGuid(32);
        string result3 = StringHelper.GetRandomStringByGuid(32);

        // Assert
        Assert.AreNotEqual(result1, result2);
        Assert.AreNotEqual(result2, result3);
        Assert.AreNotEqual(result1, result3);
    }

    /// <summary>
    /// 测试生成非32倍数长度的字符串
    /// </summary>
    [TestMethod]
    public void GetRandomStringByGuid_WithNonMultipleOf32Length_ReturnsCorrectLength_Test()
    {
        // Arrange
        int length1 = 35;
        int length2 = 50;
        int length3 = 100;

        // Act
        string result1 = StringHelper.GetRandomStringByGuid(length1);
        string result2 = StringHelper.GetRandomStringByGuid(length2);
        string result3 = StringHelper.GetRandomStringByGuid(length3);

        // Assert
        Assert.AreEqual(length1, result1.Length);
        Assert.AreEqual(length2, result2.Length);
        Assert.AreEqual(length3, result3.Length);
    }

    /// <summary>
    /// 测试使用无效范围（minLength >= maxLength）生成字符串时抛出异常
    /// </summary>
    [TestMethod]
    public void GetRandomStringByGuid_WithInvalidRange_ThrowsUtilException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => StringHelper.GetRandomStringByGuid(50, 50));
        Assert.ThrowsExactly<UtilException>(() => StringHelper.GetRandomStringByGuid(50, 10));
    }

    #endregion

    #region GetRandomStringByDictionary Tests

    /// <summary>
    /// 测试使用字典生成随机字符串（固定长度）
    /// </summary>
    [TestMethod]
    public void GetRandomStringByDictionary_WithLength_ReturnsStringOfLength_Test()
    {
        // Arrange
        int length = 20;
        string dictionary = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Act
        string result = StringHelper.GetRandomStringByDictionary(length, dictionary);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(length, result.Length);

        // Verify all characters are from dictionary
        foreach (char c in result)
        {
            Assert.Contains(c, dictionary);
        }
    }

    /// <summary>
    /// 测试使用字典生成随机字符串（可变长度）
    /// </summary>
    [TestMethod]
    public void GetRandomStringByDictionary_WithVariableLength_ReturnsStringInRange_Test()
    {
        // Arrange
        int minLength = 5;
        int maxLength = 20;
        string dictionary = "ABC";

        // Act
        string result = StringHelper.GetRandomStringByDictionary(minLength, maxLength, dictionary);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThanOrEqualTo(minLength, result.Length);
        Assert.IsLessThan(maxLength, result.Length);

        // Verify all characters are from dictionary
        foreach (char c in result)
        {
            Assert.Contains(c, dictionary);
        }
    }

    /// <summary>
    /// 测试使用默认字典生成随机字符串
    /// </summary>
    [TestMethod]
    public void GetRandomStringByDictionary_WithDefaultDictionary_ReturnsAlphanumericString_Test()
    {
        // Act
        string result = StringHelper.GetRandomStringByDictionary(32);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(32, result.Length);

        // Verify alphanumeric only
        foreach (char c in result)
        {
            Assert.IsTrue(char.IsLetterOrDigit(c));
        }
    }

    /// <summary>
    /// 测试使用无效长度生成字符串时抛出异常
    /// </summary>
    [TestMethod]
    public void GetRandomStringByDictionary_WithZeroLength_ThrowsUtilException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => StringHelper.GetRandomStringByDictionary(0));
    }

    /// <summary>
    /// 测试使用无效范围生成字符串时抛出异常
    /// </summary>
    [TestMethod]
    public void GetRandomStringByDictionary_WithInvalidRange_ThrowsUtilException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => StringHelper.GetRandomStringByDictionary(50, 50, "ABC"));
        Assert.ThrowsExactly<UtilException>(() => StringHelper.GetRandomStringByDictionary(50, 10, "ABC"));
    }

    /// <summary>
    /// 测试使用空字典生成字符串时抛出异常
    /// </summary>
    [TestMethod]
    public void GetRandomStringByDictionary_WithEmptyDictionary_ThrowsException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<IndexOutOfRangeException>(() => StringHelper.GetRandomStringByDictionary(10, string.Empty));
    }

    /// <summary>
    /// 测试使用负数长度生成字符串时抛出异常
    /// </summary>
    [TestMethod]
    public void GetRandomStringByDictionary_WithNegativeLength_ThrowsUtilException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => StringHelper.GetRandomStringByDictionary(-10, "ABC"));
    }

    /// <summary>
    /// 测试多次调用字典方法生成不同的随机字符串
    /// </summary>
    [TestMethod]
    public void GetRandomStringByDictionary_CalledMultipleTimes_ReturnsDifferentStrings_Test()
    {
        // Arrange
        string dictionary = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Act
        string result1 = StringHelper.GetRandomStringByDictionary(20, dictionary);
        string result2 = StringHelper.GetRandomStringByDictionary(20, dictionary);
        string result3 = StringHelper.GetRandomStringByDictionary(20, dictionary);

        // Assert
        Assert.AreNotEqual(result1, result2);
        Assert.AreNotEqual(result2, result3);
        Assert.AreNotEqual(result1, result3);
    }

    /// <summary>
    /// 测试使用小字典生成长字符串
    /// </summary>
    [TestMethod]
    public void GetRandomStringByDictionary_WithSmallDictionary_ReturnsStringWithRepeatedChars_Test()
    {
        // Arrange
        string dictionary = "AB";
        int length = 20;

        // Act
        string result = StringHelper.GetRandomStringByDictionary(length, dictionary);

        // Assert
        Assert.AreEqual(length, result.Length);
        foreach (char c in result)
        {
            Assert.Contains(c, dictionary);
        }
    }

    #endregion

    #region GetRandomStringByTick Tests

    /// <summary>
    /// 测试使用 Tick 生成随机字符串
    /// </summary>
    [TestMethod]
    public void GetRandomStringByTick_WithLength_ReturnsStringOfLength_Test()
    {
        // Arrange
        int length = 16;

        // Act
        string result = StringHelper.GetRandomStringByTick(length);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(length, result.Length);
    }

    /// <summary>
    /// 测试使用无效长度生成字符串时抛出异常
    /// </summary>
    [TestMethod]
    public void GetRandomStringByTick_WithZeroLength_ThrowsUtilException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => StringHelper.GetRandomStringByTick(0));
    }

    /// <summary>
    /// 测试使用负数长度生成字符串时抛出异常
    /// </summary>
    [TestMethod]
    public void GetRandomStringByTick_WithNegativeLength_ThrowsUtilException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => StringHelper.GetRandomStringByTick(-10));
    }

    /// <summary>
    /// 测试多次调用 Tick 方法生成不同的随机字符串
    /// </summary>
    [TestMethod]
    public void GetRandomStringByTick_CalledMultipleTimes_ReturnsDifferentStrings_Test()
    {
        // Act
        string result1 = StringHelper.GetRandomStringByTick(16);
        Thread.Sleep(10);
        string result2 = StringHelper.GetRandomStringByTick(16);
        Thread.Sleep(10);
        string result3 = StringHelper.GetRandomStringByTick(16);

        // Assert
        Assert.AreNotEqual(result1, result2);
        Assert.AreNotEqual(result2, result3);
        Assert.AreNotEqual(result1, result3);
    }

    /// <summary>
    /// 测试 Tick 方法生成的字符串只包含字母和数字
    /// </summary>
    [TestMethod]
    public void GetRandomStringByTick_ReturnsAlphanumericString_Test()
    {
        // Act
        string result = StringHelper.GetRandomStringByTick(50);

        // Assert
        Assert.AreEqual(50, result.Length);
        foreach (char c in result)
        {
            Assert.IsTrue(char.IsLetterOrDigit(c));
        }
    }

    #endregion
}
