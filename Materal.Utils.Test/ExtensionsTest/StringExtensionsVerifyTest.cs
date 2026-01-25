using System.Text.RegularExpressions;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// StringExtensions.Verify 测试类
/// 测试字符串验证扩展方法的功能
/// </summary>
[TestClass]
public class StringExtensionsVerifyTest
{
    #region VerifyRegex Tests

    /// <summary>
    /// 测试正则验证匹配
    /// </summary>
    [TestMethod]
    public void VerifyRegex_WithMatchingPattern_ReturnsTrue_Test()
    {
        // Arrange
        string input = "123";
        string pattern = @"\d+";

        // Act
        bool result = input.VerifyRegex(pattern);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试正则验证不匹配
    /// </summary>
    [TestMethod]
    public void VerifyRegex_WithNonMatchingPattern_ReturnsFalse_Test()
    {
        // Arrange
        string input = "abc";
        string pattern = @"\d+";

        // Act
        bool result = input.VerifyRegex(pattern);

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试空字符串返回false
    /// </summary>
    [TestMethod]
    public void VerifyRegex_WithEmptyString_ReturnsFalse_Test()
    {
        // Arrange
        string input = string.Empty;
        string pattern = @"\d+";

        // Act
        bool result = input.VerifyRegex(pattern);

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试空正则返回false
    /// </summary>
    [TestMethod]
    public void VerifyRegex_WithEmptyPattern_ReturnsFalse_Test()
    {
        // Arrange
        string input = "123";
        string pattern = string.Empty;

        // Act
        bool result = input.VerifyRegex(pattern);

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试完全匹配模式
    /// </summary>
    [TestMethod]
    public void VerifyRegex_WithPerfectMatch_ReturnsTrue_Test()
    {
        // Arrange
        string input = "123";
        string pattern = @"\d+";

        // Act
        bool result = input.VerifyRegex(pattern, true);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试部分匹配在完全匹配模式下返回false
    /// </summary>
    [TestMethod]
    public void VerifyRegex_WithPartialMatchInPerfectMode_ReturnsFalse_Test()
    {
        // Arrange
        string input = "123abc";
        string pattern = @"\d+";

        // Act
        bool result = input.VerifyRegex(pattern, true);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region GetVerifyRegex Tests

    /// <summary>
    /// 测试获取所有匹配
    /// </summary>
    [TestMethod]
    public void GetVerifyRegex_WithMultipleMatches_ReturnsAllMatches_Test()
    {
        // Arrange
        string input = "123 456 789";
        string pattern = @"\d+";

        // Act
        MatchCollection? result = input.GetVerifyRegex(pattern);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(3, result);
    }

    /// <summary>
    /// 测试无匹配返回空集合
    /// </summary>
    [TestMethod]
    public void GetVerifyRegex_WithNoMatches_ReturnsEmptyCollection_Test()
    {
        // Arrange
        string input = "abc";
        string pattern = @"\d+";

        // Act
        MatchCollection? result = input.GetVerifyRegex(pattern);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 测试空字符串返回null
    /// </summary>
    [TestMethod]
    public void GetVerifyRegex_WithEmptyString_ReturnsNull_Test()
    {
        // Arrange
        string input = string.Empty;
        string pattern = @"\d+";

        // Act
        MatchCollection? result = input.GetVerifyRegex(pattern);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region GetPerfectRegStr Tests

    /// <summary>
    /// 测试获取完全匹配正则
    /// </summary>
    [TestMethod]
    public void GetPerfectRegStr_WithNormalPattern_AddsAnchors_Test()
    {
        // Arrange
        string pattern = @"\d+";

        // Act
        string result = StringExtensions.GetPerfectRegStr(pattern);

        // Assert
        Assert.AreEqual(@"^\d+$", result);
    }

    /// <summary>
    /// 测试已有开始锚点
    /// </summary>
    [TestMethod]
    public void GetPerfectRegStr_WithStartAnchor_AddsEndAnchor_Test()
    {
        // Arrange
        string pattern = @"^\d+";

        // Act
        string result = StringExtensions.GetPerfectRegStr(pattern);

        // Assert
        Assert.AreEqual(@"^\d+$", result);
    }

    /// <summary>
    /// 测试已有结束锚点
    /// </summary>
    [TestMethod]
    public void GetPerfectRegStr_WithEndAnchor_AddsStartAnchor_Test()
    {
        // Arrange
        string pattern = @"\d+$";

        // Act
        string result = StringExtensions.GetPerfectRegStr(pattern);

        // Assert
        Assert.AreEqual(@"^\d+$", result);
    }

    /// <summary>
    /// 测试已有两个锚点
    /// </summary>
    [TestMethod]
    public void GetPerfectRegStr_WithBothAnchors_ReturnsUnchanged_Test()
    {
        // Arrange
        string pattern = @"^\d+$";

        // Act
        string result = StringExtensions.GetPerfectRegStr(pattern);

        // Assert
        Assert.AreEqual(@"^\d+$", result);
    }

    /// <summary>
    /// 测试空字符串
    /// </summary>
    [TestMethod]
    public void GetPerfectRegStr_WithEmptyString_ReturnsEmpty_Test()
    {
        // Arrange
        string pattern = string.Empty;

        // Act
        string result = StringExtensions.GetPerfectRegStr(pattern);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    #endregion

    #region GetNoPerfectRegStr Tests

    /// <summary>
    /// 测试移除锚点
    /// </summary>
    [TestMethod]
    public void GetNoPerfectRegStr_WithAnchors_RemovesAnchors_Test()
    {
        // Arrange
        string pattern = @"^\d+$";

        // Act
        string result = StringExtensions.GetNoPerfectRegStr(pattern);

        // Assert
        Assert.AreEqual(@"\d+", result);
    }

    /// <summary>
    /// 测试只有开始锚点
    /// </summary>
    [TestMethod]
    public void GetNoPerfectRegStr_WithStartAnchor_RemovesStartAnchor_Test()
    {
        // Arrange
        string pattern = @"^\d+";

        // Act
        string result = StringExtensions.GetNoPerfectRegStr(pattern);

        // Assert
        Assert.AreEqual(@"\d+", result);
    }

    /// <summary>
    /// 测试只有结束锚点
    /// </summary>
    [TestMethod]
    public void GetNoPerfectRegStr_WithEndAnchor_RemovesEndAnchor_Test()
    {
        // Arrange
        string pattern = @"\d+$";

        // Act
        string result = StringExtensions.GetNoPerfectRegStr(pattern);

        // Assert
        Assert.AreEqual(@"\d+", result);
    }

    /// <summary>
    /// 测试无锚点
    /// </summary>
    [TestMethod]
    public void GetNoPerfectRegStr_WithoutAnchors_ReturnsUnchanged_Test()
    {
        // Arrange
        string pattern = @"\d+";

        // Act
        string result = StringExtensions.GetNoPerfectRegStr(pattern);

        // Assert
        Assert.AreEqual(@"\d+", result);
    }

    #endregion

    #region IsJson Tests

    /// <summary>
    /// 测试有效JSON对象
    /// </summary>
    [TestMethod]
    public void IsJson_WithValidJsonObject_ReturnsTrue_Test()
    {
        // Arrange
        string input = "{\"name\":\"test\"}";

        // Act
        bool result = input.IsJson();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试有效JSON数组
    /// </summary>
    [TestMethod]
    public void IsJson_WithValidJsonArray_ReturnsTrue_Test()
    {
        // Arrange
        string input = "[1,2,3]";

        // Act
        bool result = input.IsJson();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效JSON
    /// </summary>
    [TestMethod]
    public void IsJson_WithInvalidJson_ReturnsFalse_Test()
    {
        // Arrange
        string input = "{name:test}";

        // Act
        bool result = input.IsJson();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试普通字符串
    /// </summary>
    [TestMethod]
    public void IsJson_WithPlainString_ReturnsFalse_Test()
    {
        // Arrange
        string input = "hello";

        // Act
        bool result = input.IsJson();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsObjectJson Tests

    /// <summary>
    /// 测试JSON对象
    /// </summary>
    [TestMethod]
    public void IsObjectJson_WithValidObject_ReturnsTrue_Test()
    {
        // Arrange
        string input = "{\"name\":\"test\"}";

        // Act
        bool result = input.IsObjectJson();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试JSON数组
    /// </summary>
    [TestMethod]
    public void IsObjectJson_WithArray_ReturnsFalse_Test()
    {
        // Arrange
        string input = "[1,2,3]";

        // Act
        bool result = input.IsObjectJson();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsArrayJson Tests

    /// <summary>
    /// 测试JSON数组
    /// </summary>
    [TestMethod]
    public void IsArrayJson_WithValidArray_ReturnsTrue_Test()
    {
        // Arrange
        string input = "[1,2,3]";

        // Act
        bool result = input.IsArrayJson();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试JSON对象
    /// </summary>
    [TestMethod]
    public void IsArrayJson_WithObject_ReturnsFalse_Test()
    {
        // Arrange
        string input = "{\"name\":\"test\"}";

        // Act
        bool result = input.IsArrayJson();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsXml Tests

    /// <summary>
    /// 测试有效XML
    /// </summary>
    [TestMethod]
    public void IsXml_WithValidXml_ReturnsTrue_Test()
    {
        // Arrange
        string input = "<root><item>test</item></root>";

        // Act
        bool result = input.IsXml();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效XML
    /// </summary>
    [TestMethod]
    public void IsXml_WithInvalidXml_ReturnsFalse_Test()
    {
        // Arrange
        string input = "<root><item>test</root>";

        // Act
        bool result = input.IsXml();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试普通字符串
    /// </summary>
    [TestMethod]
    public void IsXml_WithPlainString_ReturnsFalse_Test()
    {
        // Arrange
        string input = "hello";

        // Act
        bool result = input.IsXml();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsHexColor Tests

    /// <summary>
    /// 测试有效十六进制颜色（小写）
    /// </summary>
    [TestMethod]
    public void IsHexColor_WithValidColorLowerCase_ReturnsTrue_Test()
    {
        // Arrange
        string input = "#ff5733";

        // Act
        bool result = input.IsHexColor();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试有效十六进制颜色（大写）
    /// </summary>
    [TestMethod]
    public void IsHexColor_WithValidColorUpperCase_ReturnsTrue_Test()
    {
        // Arrange
        string input = "#FF5733";

        // Act
        bool result = input.IsHexColor();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试有效十六进制颜色（混合大小写）
    /// </summary>
    [TestMethod]
    public void IsHexColor_WithValidColorMixedCase_ReturnsTrue_Test()
    {
        // Arrange
        string input = "#Ff5733";

        // Act
        bool result = input.IsHexColor();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试短格式颜色（小写）
    /// </summary>
    [TestMethod]
    public void IsHexColor_WithShortFormatLowerCase_ReturnsTrue_Test()
    {
        // Arrange
        string input = "#f53";

        // Act
        bool result = input.IsHexColor();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试短格式颜色（大写）
    /// </summary>
    [TestMethod]
    public void IsHexColor_WithShortFormatUpperCase_ReturnsTrue_Test()
    {
        // Arrange
        string input = "#F53";

        // Act
        bool result = input.IsHexColor();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效颜色
    /// </summary>
    [TestMethod]
    public void IsHexColor_WithInvalidColor_ReturnsFalse_Test()
    {
        // Arrange
        string input = "#GG5733";

        // Act
        bool result = input.IsHexColor();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsIPv4 Tests

    /// <summary>
    /// 测试有效IPv4地址
    /// </summary>
    [TestMethod]
    public void IsIPv4_WithValidIP_ReturnsTrue_Test()
    {
        // Arrange
        string input = "192.168.1.1";

        // Act
        bool result = input.IsIPv4();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效IPv4地址
    /// </summary>
    [TestMethod]
    public void IsIPv4_WithInvalidIP_ReturnsFalse_Test()
    {
        // Arrange
        string input = "256.168.1.1";

        // Act
        bool result = input.IsIPv4();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试带端口号返回false
    /// </summary>
    [TestMethod]
    public void IsIPv4_WithPort_ReturnsFalse_Test()
    {
        // Arrange
        string input = "192.168.1.1:8080";

        // Act
        bool result = input.IsIPv4();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsIPv4AndPort Tests

    /// <summary>
    /// 测试有效IPv4地址和端口
    /// </summary>
    [TestMethod]
    public void IsIPv4AndPort_WithValidIPAndPort_ReturnsTrue_Test()
    {
        // Arrange
        string input = "192.168.1.1:8080";

        // Act
        bool result = input.IsIPv4AndPort();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无端口号返回false
    /// </summary>
    [TestMethod]
    public void IsIPv4AndPort_WithoutPort_ReturnsFalse_Test()
    {
        // Arrange
        string input = "192.168.1.1";

        // Act
        bool result = input.IsIPv4AndPort();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsEMail Tests

    /// <summary>
    /// 测试有效邮箱
    /// </summary>
    [TestMethod]
    public void IsEMail_WithValidEmail_ReturnsTrue_Test()
    {
        // Arrange
        string input = "test@example.com";

        // Act
        bool result = input.IsEMail();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效邮箱
    /// </summary>
    [TestMethod]
    public void IsEMail_WithInvalidEmail_ReturnsFalse_Test()
    {
        // Arrange
        string input = "test@";

        // Act
        bool result = input.IsEMail();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsNumber Tests

    /// <summary>
    /// 测试整数
    /// </summary>
    [TestMethod]
    public void IsNumber_WithInteger_ReturnsTrue_Test()
    {
        // Arrange
        string input = "123";

        // Act
        bool result = input.IsNumber();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试小数
    /// </summary>
    [TestMethod]
    public void IsNumber_WithDecimal_ReturnsTrue_Test()
    {
        // Arrange
        string input = "123.45";

        // Act
        bool result = input.IsNumber();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试负数
    /// </summary>
    [TestMethod]
    public void IsNumber_WithNegative_ReturnsTrue_Test()
    {
        // Arrange
        string input = "-123";

        // Act
        bool result = input.IsNumber();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试非数字
    /// </summary>
    [TestMethod]
    public void IsNumber_WithNonNumber_ReturnsFalse_Test()
    {
        // Arrange
        string input = "abc";

        // Act
        bool result = input.IsNumber();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsNumberPositive Tests

    /// <summary>
    /// 测试正数
    /// </summary>
    [TestMethod]
    public void IsNumberPositive_WithPositive_ReturnsTrue_Test()
    {
        // Arrange
        string input = "123";

        // Act
        bool result = input.IsNumberPositive();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试负数
    /// </summary>
    [TestMethod]
    public void IsNumberPositive_WithNegative_ReturnsFalse_Test()
    {
        // Arrange
        string input = "-123";

        // Act
        bool result = input.IsNumberPositive();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsNumberNegative Tests

    /// <summary>
    /// 测试负数
    /// </summary>
    [TestMethod]
    public void IsNumberNegative_WithNegative_ReturnsTrue_Test()
    {
        // Arrange
        string input = "-123";

        // Act
        bool result = input.IsNumberNegative();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试正数
    /// </summary>
    [TestMethod]
    public void IsNumberNegative_WithPositive_ReturnsFalse_Test()
    {
        // Arrange
        string input = "123";

        // Act
        bool result = input.IsNumberNegative();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsInteger Tests

    /// <summary>
    /// 测试整数
    /// </summary>
    [TestMethod]
    public void IsInteger_WithInteger_ReturnsTrue_Test()
    {
        // Arrange
        string input = "123";

        // Act
        bool result = input.IsInteger();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试小数
    /// </summary>
    [TestMethod]
    public void IsInteger_WithDecimal_ReturnsFalse_Test()
    {
        // Arrange
        string input = "123.45";

        // Act
        bool result = input.IsInteger();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsUrl Tests

    /// <summary>
    /// 测试有效URL
    /// </summary>
    [TestMethod]
    public void IsUrl_WithValidUrl_ReturnsTrue_Test()
    {
        // Arrange
        string input = "https://www.example.com";

        // Act
        bool result = input.IsUrl();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试HTTP URL
    /// </summary>
    [TestMethod]
    public void IsUrl_WithHttpUrl_ReturnsTrue_Test()
    {
        // Arrange
        string input = "http://example.com";

        // Act
        bool result = input.IsUrl();

        // Assert
        Assert.IsTrue(result);
    }

    #endregion

    #region IsPhoneNumber Tests

    /// <summary>
    /// 测试有效手机号
    /// </summary>
    [TestMethod]
    public void IsPhoneNumber_WithValidPhone_ReturnsTrue_Test()
    {
        // Arrange
        string input = "13800138000";

        // Act
        bool result = input.IsPhoneNumber();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效手机号
    /// </summary>
    [TestMethod]
    public void IsPhoneNumber_WithInvalidPhone_ReturnsFalse_Test()
    {
        // Arrange
        string input = "12345678901";

        // Act
        bool result = input.IsPhoneNumber();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsDate Tests

    /// <summary>
    /// 测试有效日期
    /// </summary>
    [TestMethod]
    public void IsDate_WithValidDate_ReturnsTrue_Test()
    {
        // Arrange
        string input = "2024-01-26";

        // Act
        bool result = input.IsDate();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效日期
    /// </summary>
    [TestMethod]
    public void IsDate_WithInvalidDate_ReturnsFalse_Test()
    {
        // Arrange
        string input = "2024-13-45";

        // Act
        bool result = input.IsDate();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试空字符串
    /// </summary>
    [TestMethod]
    public void IsDate_WithEmptyString_ReturnsFalse_Test()
    {
        // Arrange
        string input = string.Empty;

        // Act
        bool result = input.IsDate();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsTime Tests

    /// <summary>
    /// 测试有效时间
    /// </summary>
    [TestMethod]
    public void IsTime_WithValidTime_ReturnsTrue_Test()
    {
        // Arrange
        string input = "12:30:45";

        // Act
        bool result = input.IsTime();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效时间
    /// </summary>
    [TestMethod]
    public void IsTime_WithInvalidTime_ReturnsFalse_Test()
    {
        // Arrange
        string input = "25:70:80";

        // Act
        bool result = input.IsTime();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsDateTime Tests

    /// <summary>
    /// 测试有效日期时间
    /// </summary>
    [TestMethod]
    public void IsDateTime_WithValidDateTime_ReturnsTrue_Test()
    {
        // Arrange
        string input = "2024-01-26 12:30:45";

        // Act
        bool result = input.IsDateTime();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试T分隔符
    /// </summary>
    [TestMethod]
    public void IsDateTime_WithTSeparator_ReturnsTrue_Test()
    {
        // Arrange
        string input = "2024-01-26T12:30:45";

        // Act
        bool result = input.IsDateTime();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无分隔符
    /// </summary>
    [TestMethod]
    public void IsDateTime_WithoutSeparator_ReturnsFalse_Test()
    {
        // Arrange
        string input = "20240126123045";

        // Act
        bool result = input.IsDateTime();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsLetter Tests

    /// <summary>
    /// 测试字母
    /// </summary>
    [TestMethod]
    public void IsLetter_WithLetters_ReturnsTrue_Test()
    {
        // Arrange
        string input = "abc";

        // Act
        bool result = input.IsLetter();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试包含数字
    /// </summary>
    [TestMethod]
    public void IsLetter_WithNumbers_ReturnsFalse_Test()
    {
        // Arrange
        string input = "abc123";

        // Act
        bool result = input.IsLetter();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsLetterOrNumber Tests

    /// <summary>
    /// 测试字母和数字
    /// </summary>
    [TestMethod]
    public void IsLetterOrNumber_WithLettersAndNumbers_ReturnsTrue_Test()
    {
        // Arrange
        string input = "abc123";

        // Act
        bool result = input.IsLetterOrNumber();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试包含特殊字符
    /// </summary>
    [TestMethod]
    public void IsLetterOrNumber_WithSpecialChars_ReturnsFalse_Test()
    {
        // Arrange
        string input = "abc@123";

        // Act
        bool result = input.IsLetterOrNumber();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsChinese Tests

    /// <summary>
    /// 测试中文
    /// </summary>
    [TestMethod]
    public void IsChinese_WithChinese_ReturnsTrue_Test()
    {
        // Arrange
        string input = "中国";

        // Act
        bool result = input.IsChinese();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试英文
    /// </summary>
    [TestMethod]
    public void IsChinese_WithEnglish_ReturnsFalse_Test()
    {
        // Arrange
        string input = "China";

        // Act
        bool result = input.IsChinese();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsGuid Tests

    /// <summary>
    /// 测试有效GUID
    /// </summary>
    [TestMethod]
    public void IsGuid_WithValidGuid_ReturnsTrue_Test()
    {
        // Arrange
        string input = "550e8400-e29b-41d4-a716-446655440000";

        // Act
        bool result = input.IsGuid();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效GUID
    /// </summary>
    [TestMethod]
    public void IsGuid_WithInvalidGuid_ReturnsFalse_Test()
    {
        // Arrange
        string input = "not-a-guid";

        // Act
        bool result = input.IsGuid();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsIDCardForChina Tests

    /// <summary>
    /// 测试18位身份证
    /// </summary>
    [TestMethod]
    public void IsIDCardForChina_With18Digits_ReturnsTrue_Test()
    {
        // Arrange
        string input = "110101199001011234";

        // Act
        bool result = input.IsIDCardForChina();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试15位身份证
    /// </summary>
    [TestMethod]
    public void IsIDCardForChina_With15Digits_ReturnsTrue_Test()
    {
        // Arrange
        string input = "110101900101123";

        // Act
        bool result = input.IsIDCardForChina();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效长度
    /// </summary>
    [TestMethod]
    public void IsIDCardForChina_WithInvalidLength_ReturnsFalse_Test()
    {
        // Arrange
        string input = "1234567890";

        // Act
        bool result = input.IsIDCardForChina();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试空字符串
    /// </summary>
    [TestMethod]
    public void IsIDCardForChina_WithEmptyString_ReturnsFalse_Test()
    {
        // Arrange
        string input = string.Empty;

        // Act
        bool result = input.IsIDCardForChina();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsDiskPath Tests

    /// <summary>
    /// 测试有效磁盘路径
    /// </summary>
    [TestMethod]
    public void IsDiskPath_WithValidPath_ReturnsTrue_Test()
    {
        // Arrange
        string input = "C:";

        // Act
        bool result = input.IsDiskPath();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试带反斜杠的路径
    /// </summary>
    [TestMethod]
    public void IsDiskPath_WithBackslash_ReturnsTrue_Test()
    {
        // Arrange
        string input = "C:\\";

        // Act
        bool result = input.IsDiskPath();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无效磁盘路径
    /// </summary>
    [TestMethod]
    public void IsDiskPath_WithInvalidPath_ReturnsFalse_Test()
    {
        // Arrange
        string input = "CC:";

        // Act
        bool result = input.IsDiskPath();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsAbsolutePath Tests

    /// <summary>
    /// 测试有效绝对路径
    /// </summary>
    [TestMethod]
    public void IsAbsolutePath_WithValidPath_ReturnsTrue_Test()
    {
        // Arrange
        string input = "C:\\Windows\\System32";

        // Act
        bool result = input.IsAbsolutePath();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试相对路径
    /// </summary>
    [TestMethod]
    public void IsAbsolutePath_WithRelativePath_ReturnsFalse_Test()
    {
        // Arrange
        string input = "..\\test";

        // Act
        bool result = input.IsAbsolutePath();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion
}
