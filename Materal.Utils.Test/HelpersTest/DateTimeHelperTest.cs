using Materal.Utils.Helpers;

namespace Materal.Utils.Test.HelpersTest;

/// <summary>
/// DateTimeHelper 测试类
/// 测试日期时间转换功能
/// </summary>
[TestClass]
public class DateTimeHelperTest
{
    #region GetTimeStamp Tests

    /// <summary>
    /// 测试获取 UTC 时间戳
    /// </summary>
    [TestMethod]
    public void GetTimeStamp_WithUtcKind_ReturnsUtcTimestamp_Test()
    {
        // Act
        long timestamp = DateTimeHelper.GetTimeStamp(DateTimeKind.Utc);

        // Assert
        Assert.IsGreaterThan(0, timestamp);

        // Verify by converting back
        DateTime converted = DateTimeHelper.TimeStampToDateTime(timestamp, DateTimeKind.Utc);
        DateTime utcNow = DateTime.UtcNow;
        TimeSpan difference = utcNow - converted;

        // Should be within 1 second
        Assert.IsLessThan(1, difference.TotalSeconds);
    }

    /// <summary>
    /// 测试获取 Local 时间戳
    /// </summary>
    [TestMethod]
    public void GetTimeStamp_WithLocalKind_ReturnsLocalTimestamp_Test()
    {
        // Act
        long timestamp = DateTimeHelper.GetTimeStamp(DateTimeKind.Local);

        // Assert
        Assert.IsGreaterThan(0, timestamp);

        // Verify by converting back
        DateTime converted = DateTimeHelper.TimeStampToDateTime(timestamp, DateTimeKind.Local);
        DateTime localNow = DateTime.Now;
        TimeSpan difference = localNow - converted;

        // Should be within 1 second
        Assert.IsLessThan(1, difference.TotalSeconds);
    }

    #endregion

    #region TimeStampToDateTime Tests

    /// <summary>
    /// 测试将时间戳转换为 UTC DateTime
    /// </summary>
    [TestMethod]
    public void TimeStampToDateTime_WithUtcKind_ReturnsUtcDateTime_Test()
    {
        // Arrange
        long timestamp = 1609459200; // 2021-01-01 00:00:00 UTC

        // Act
        DateTime result = DateTimeHelper.TimeStampToDateTime(timestamp, DateTimeKind.Utc);

        // Assert
        Assert.AreEqual(2021, result.Year);
        Assert.AreEqual(1, result.Month);
        Assert.AreEqual(1, result.Day);
        Assert.AreEqual(DateTimeKind.Utc, result.Kind);
    }

    /// <summary>
    /// 测试将时间戳转换为 Local DateTime
    /// </summary>
    [TestMethod]
    public void TimeStampToDateTime_WithLocalKind_ReturnsLocalDateTime_Test()
    {
        // Arrange
        long timestamp = 1609459200; // 2021-01-01 00:00:00 UTC

        // Act
        DateTime result = DateTimeHelper.TimeStampToDateTime(timestamp, DateTimeKind.Local);

        // Assert
        Assert.AreEqual(DateTimeKind.Local, result.Kind);
    }

    #endregion

    #region TimeStampToDateTimeOffset Tests

    /// <summary>
    /// 测试将时间戳转换为 DateTimeOffset
    /// </summary>
    [TestMethod]
    public void TimeStampToDateTimeOffset_WithUtcKind_ReturnsDateTimeOffset_Test()
    {
        // Arrange
        long timestamp = 1609459200; // 2021-01-01 00:00:00 UTC

        // Act
        DateTimeOffset result = DateTimeHelper.TimeStampToDateTimeOffset(timestamp, DateTimeKind.Utc);

        // Assert
        Assert.AreEqual(2021, result.Year);
        Assert.AreEqual(1, result.Month);
        Assert.AreEqual(1, result.Day);
    }

    #endregion

    #region ToMilliseconds Tests

    /// <summary>
    /// 测试将年转换为毫秒
    /// </summary>
    [TestMethod]
    public void ToMilliseconds_WithYearUnit_ReturnsCorrectMilliseconds_Test()
    {
        // Arrange
        double years = 1;

        // Act
        double milliseconds = DateTimeHelper.ToMilliseconds(years, DateTimeUnit.YearUnit);

        // Assert
        // 1 year ≈ 365.25 * 24 * 60 * 60 * 1000 milliseconds
        double expected = years * 365.25 * 24 * 60 * 60 * 1000;
        Assert.AreEqual(expected, milliseconds, 0.01);
    }

    /// <summary>
    /// 测试将天转换为毫秒
    /// </summary>
    [TestMethod]
    public void ToMilliseconds_WithDayUnit_ReturnsCorrectMilliseconds_Test()
    {
        // Arrange
        double days = 1;

        // Act
        double milliseconds = DateTimeHelper.ToMilliseconds(days, DateTimeUnit.DayUnit);

        // Assert
        // 1 day = 24 * 60 * 60 * 1000 = 86400000 milliseconds
        double expected = days * 24 * 60 * 60 * 1000;
        Assert.AreEqual(expected, milliseconds, 0.01);
    }

    /// <summary>
    /// 测试将小时转换为毫秒
    /// </summary>
    [TestMethod]
    public void ToMilliseconds_WithHourUnit_ReturnsCorrectMilliseconds_Test()
    {
        // Arrange
        double hours = 1;

        // Act
        double milliseconds = DateTimeHelper.ToMilliseconds(hours, DateTimeUnit.HourUnit);

        // Assert
        // 1 hour = 60 * 60 * 1000 = 3600000 milliseconds
        double expected = hours * 60 * 60 * 1000;
        Assert.AreEqual(expected, milliseconds, 0.01);
    }

    /// <summary>
    /// 测试将秒转换为毫秒
    /// </summary>
    [TestMethod]
    public void ToMilliseconds_WithSecondUnit_ReturnsCorrectMilliseconds_Test()
    {
        // Arrange
        double seconds = 60;

        // Act
        double milliseconds = DateTimeHelper.ToMilliseconds(seconds, DateTimeUnit.SecondUnit);

        // Assert
        // 60 seconds = 60000 milliseconds
        double expected = seconds * 1000;
        Assert.AreEqual(expected, milliseconds, 0.01);
    }

    /// <summary>
    /// 测试将毫秒转换为毫秒
    /// </summary>
    [TestMethod]
    public void ToMilliseconds_WithMillisecondUnit_ReturnsSameValue_Test()
    {
        // Arrange
        double milliseconds = 1000;

        // Act
        double result = DateTimeHelper.ToMilliseconds(milliseconds, DateTimeUnit.MillisecondUnit);

        // Assert
        Assert.AreEqual(milliseconds, result, 0.01);
    }

    #endregion

    #region ToSeconds Tests

    /// <summary>
    /// 测试将天转换为秒
    /// </summary>
    [TestMethod]
    public void ToSeconds_WithDayUnit_ReturnsCorrectSeconds_Test()
    {
        // Arrange
        double days = 1;

        // Act
        double seconds = DateTimeHelper.ToSeconds(days, DateTimeUnit.DayUnit);

        // Assert
        // 1 day = 24 * 60 * 60 = 86400 seconds
        double expected = days * 24 * 60 * 60;
        Assert.AreEqual(expected, seconds, 0.01);
    }

    /// <summary>
    /// 测试将毫秒转换为秒
    /// </summary>
    [TestMethod]
    public void ToSeconds_WithMillisecondUnit_ReturnsCorrectSeconds_Test()
    {
        // Arrange
        double milliseconds = 60000;

        // Act
        double seconds = DateTimeHelper.ToSeconds(milliseconds, DateTimeUnit.MillisecondUnit);

        // Assert
        // 60000 milliseconds = 60 seconds
        double expected = milliseconds / 1000;
        Assert.AreEqual(expected, seconds, 0.01);
    }

    #endregion

    #region ToMinutes Tests

    /// <summary>
    /// 测试将小时转换为分钟
    /// </summary>
    [TestMethod]
    public void ToMinutes_WithHourUnit_ReturnsCorrectMinutes_Test()
    {
        // Arrange
        double hours = 2;

        // Act
        double minutes = DateTimeHelper.ToMinutes(hours, DateTimeUnit.HourUnit);

        // Assert
        // 2 hours = 120 minutes
        double expected = hours * 60;
        Assert.AreEqual(expected, minutes, 0.01);
    }

    #endregion

    #region ToHours Tests

    /// <summary>
    /// 测试将天转换为小时
    /// </summary>
    [TestMethod]
    public void ToHours_WithDayUnit_ReturnsCorrectHours_Test()
    {
        // Arrange
        double days = 1;

        // Act
        double hours = DateTimeHelper.ToHours(days, DateTimeUnit.DayUnit);

        // Assert
        // 1 day = 24 hours
        double expected = days * 24;
        Assert.AreEqual(expected, hours, 0.01);
    }

    #endregion

    #region ToDay Tests

    /// <summary>
    /// 测试将年转换为天
    /// </summary>
    [TestMethod]
    public void ToDay_WithYearUnit_ReturnsCorrectDays_Test()
    {
        // Arrange
        double years = 1;

        // Act
        double days = DateTimeHelper.ToDay(years, DateTimeUnit.YearUnit);

        // Assert
        // 1 year ≈ 365.25 days
        double expected = years * 365.25;
        Assert.AreEqual(expected, days, 0.01);
    }

    #endregion

    #region ToMonth Tests

    /// <summary>
    /// 测试将年转换为月
    /// </summary>
    [TestMethod]
    public void ToMonth_WithYearUnit_ReturnsCorrectMonths_Test()
    {
        // Arrange
        double years = 1;

        // Act
        double months = DateTimeHelper.ToMonth(years, DateTimeUnit.YearUnit);

        // Assert
        // 1 year = 12 months
        double expected = years * 12;
        Assert.AreEqual(expected, months, 0.01);
    }

    /// <summary>
    /// 测试将月转换为月
    /// </summary>
    [TestMethod]
    public void ToMonth_WithMonthUnit_ReturnsSameValue_Test()
    {
        // Arrange
        double months = 12;

        // Act
        double result = DateTimeHelper.ToMonth(months, DateTimeUnit.MonthUnit);

        // Assert
        Assert.AreEqual(months, result, 0.01);
    }

    #endregion

    #region ToYear Tests

    /// <summary>
    /// 测试将月转换为年
    /// </summary>
    [TestMethod]
    public void ToYear_WithMonthUnit_ReturnsCorrectYears_Test()
    {
        // Arrange
        double months = 24;

        // Act
        double years = DateTimeHelper.ToYear(months, DateTimeUnit.MonthUnit);

        // Assert
        // 24 months = 2 years
        double expected = months / 12;
        Assert.AreEqual(expected, years, 0.01);
    }

    #endregion
}
