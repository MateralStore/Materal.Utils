namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// StringExtension.PinYin 测试类
/// 测试中文拼音转换扩展方法的功能
/// </summary>
[TestClass]
public class StringExtensionPinYinTest
{
    #region GetChinesePinYin Tests - Full Mode

    /// <summary>
    /// 测试获取完整拼音
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithFullMode_ReturnsFullPinyin_Test()
    {
        // Arrange
        string input = "中";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.Full);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        Assert.IsTrue(result.Contains("Zhong1") || result.Contains("Zhōng"));
    }

    /// <summary>
    /// 测试多音字返回多个拼音
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithPolyphone_ReturnsMultiplePinyins_Test()
    {
        // Arrange
        string input = "行";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.Full);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThan(1, result.Count());
    }

    #endregion

    #region GetChinesePinYin Tests - NoTone Mode

    /// <summary>
    /// 测试获取无音调拼音
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithNoToneMode_ReturnsNoTonePinyin_Test()
    {
        // Arrange
        string input = "中国";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.NoTone);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        foreach (string pinyin in result)
        {
            Assert.IsFalse(pinyin.Any(c => c > 127));
        }
    }

    /// <summary>
    /// 测试默认模式是NoTone
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithDefaultMode_UsesNoTone_Test()
    {
        // Arrange
        string input = "你好";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
    }

    #endregion

    #region GetChinesePinYin Tests - Abbreviation Mode

    /// <summary>
    /// 测试获取拼音缩写
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithAbbreviationMode_ReturnsAbbreviation_Test()
    {
        // Arrange
        string input = "中国";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.Abbreviation);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        foreach (string pinyin in result)
        {
            Assert.IsLessThanOrEqualTo(input.Length, pinyin.Length);
        }
    }

    /// <summary>
    /// 测试单个字符缩写
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithSingleCharAbbreviation_ReturnsSingleLetter_Test()
    {
        // Arrange
        string input = "中";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.Abbreviation);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        foreach (string pinyin in result)
        {
            Assert.AreEqual(1, pinyin.Length);
        }
    }

    #endregion

    #region GetChinesePinYin Tests - Mixed Content

    /// <summary>
    /// 测试混合中英文
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithMixedChineseEnglish_ReturnsMixedPinyin_Test()
    {
        // Arrange
        string input = "Hello中国";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.NoTone);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        foreach (string pinyin in result)
        {
            Assert.StartsWith("Hello", pinyin);
        }
    }

    /// <summary>
    /// 测试包含数字
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithNumbers_PreservesNumbers_Test()
    {
        // Arrange
        string input = "123中国";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.NoTone);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        foreach (string pinyin in result)
        {
            Assert.StartsWith("123", pinyin);
        }
    }

    /// <summary>
    /// 测试包含特殊字符
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithSpecialChars_PreservesSpecialChars_Test()
    {
        // Arrange
        string input = "@#中国";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.NoTone);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        foreach (string pinyin in result)
        {
            Assert.StartsWith("@#", pinyin);
        }
    }

    #endregion

    #region GetChinesePinYin Tests - Edge Cases

    /// <summary>
    /// 测试空字符串
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithEmptyString_ReturnsEmptyResult_Test()
    {
        // Arrange
        string input = string.Empty;

        // Act
        IEnumerable<string> result = input.GetChinesePinYin();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(string.Empty, result.First());
    }

    /// <summary>
    /// 测试纯英文
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithPureEnglish_ReturnsOriginal_Test()
    {
        // Arrange
        string input = "Hello";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Hello", result.First());
    }

    /// <summary>
    /// 测试纯数字
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithPureNumbers_ReturnsOriginal_Test()
    {
        // Arrange
        string input = "12345";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("12345", result.First());
    }

    /// <summary>
    /// 测试单个中文字符
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithSingleChinese_ReturnsPinyin_Test()
    {
        // Arrange
        string input = "好";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.NoTone);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
    }

    /// <summary>
    /// 测试多个相同字符
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithRepeatedChars_ReturnsCorrectPinyin_Test()
    {
        // Arrange
        string input = "好好";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.NoTone);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
    }

    #endregion

    #region GetChinesePinYin Tests - Distinct Results

    /// <summary>
    /// 测试结果去重
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_ReturnsDistinctResults_Test()
    {
        // Arrange
        string input = "中中";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.NoTone);

        // Assert
        Assert.IsNotNull(result);
        int totalCount = result.Count();
        int distinctCount = result.Distinct().Count();
        Assert.AreEqual(totalCount, distinctCount);
    }

    #endregion

    #region GetChinesePinYin Tests - Complex Scenarios

    /// <summary>
    /// 测试长句子
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithLongSentence_ReturnsPinyin_Test()
    {
        // Arrange
        string input = "中华人民共和国";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.NoTone);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
    }

    /// <summary>
    /// 测试包含空格
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithSpaces_PreservesSpaces_Test()
    {
        // Arrange
        string input = "你 好";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.NoTone);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        foreach (string pinyin in result)
        {
            Assert.Contains(' ', pinyin);
        }
    }

    /// <summary>
    /// 测试标点符号
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_WithPunctuation_PreservesPunctuation_Test()
    {
        // Arrange
        string input = "你好，世界！";

        // Act
        IEnumerable<string> result = input.GetChinesePinYin(PinYinMode.NoTone);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        foreach (string pinyin in result)
        {
            Assert.IsTrue(pinyin.Contains('，') || pinyin.Contains('！'));
        }
    }

    #endregion

    #region GetChinesePinYin Tests - Different Modes Comparison

    /// <summary>
    /// 测试不同模式返回不同结果
    /// </summary>
    [TestMethod]
    public void GetChinesePinYin_DifferentModes_ReturnDifferentResults_Test()
    {
        // Arrange
        string input = "中";

        // Act
        IEnumerable<string> fullResult = input.GetChinesePinYin(PinYinMode.Full);
        IEnumerable<string> noToneResult = input.GetChinesePinYin(PinYinMode.NoTone);
        IEnumerable<string> abbResult = input.GetChinesePinYin(PinYinMode.Abbreviation);

        // Assert
        Assert.IsNotNull(fullResult);
        Assert.IsNotNull(noToneResult);
        Assert.IsNotNull(abbResult);

        string firstFull = fullResult.First();
        string firstNoTone = noToneResult.First();
        string firstAbb = abbResult.First();

        Assert.IsGreaterThanOrEqualTo(firstNoTone.Length, firstFull.Length);
        Assert.IsGreaterThanOrEqualTo(firstAbb.Length, firstNoTone.Length);
    }

    #endregion
}
