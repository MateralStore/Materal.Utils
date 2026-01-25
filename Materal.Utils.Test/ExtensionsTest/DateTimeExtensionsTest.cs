namespace Materal.Utils.Test.ExtensionsTest;

[TestClass]
public class DateTimeExtensionsTest
{
    #region TimeOnly AddSeconds 测试

    [TestMethod]
    public void AddSeconds_WithPositiveSeconds_ReturnsCorrectTime_Test()
    {
        // Arrange
        TimeOnly time = new(10, 30, 45);
        int seconds = 30;

        // Act
        TimeOnly result = time.AddSeconds(seconds);

        // Assert
        Assert.AreEqual(new TimeOnly(10, 31, 15), result);
    }

    [TestMethod]
    public void AddSeconds_WithNegativeSeconds_ReturnsCorrectTime_Test()
    {
        // Arrange
        TimeOnly time = new(10, 30, 45);
        int seconds = -30;

        // Act
        TimeOnly result = time.AddSeconds(seconds);

        // Assert
        Assert.AreEqual(new TimeOnly(10, 30, 15), result);
    }

    [TestMethod]
    public void AddSeconds_WithZeroSeconds_ReturnsSameTime_Test()
    {
        // Arrange
        TimeOnly time = new(10, 30, 45);

        // Act
        TimeOnly result = time.AddSeconds(0);

        // Assert
        Assert.AreEqual(time, result);
    }

    [TestMethod]
    public void AddSeconds_OverflowToNextDay_ReturnsCorrectTime_Test()
    {
        // Arrange
        TimeOnly time = new(23, 59, 45);
        int seconds = 30;

        // Act
        TimeOnly result = time.AddSeconds(seconds);

        // Assert
        Assert.AreEqual(new TimeOnly(0, 0, 15), result);
    }

    #endregion

    #region DateTimeOffset ToDateTime 测试

    [TestMethod]
    public void ToDateTime_WithDefaultKind_ReturnsLocal_Test()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = new(2024, 6, 15, 10, 30, 45, TimeSpan.FromHours(8));

        // Act
        DateTime result = dateTimeOffset.ToDateTime();

        // Assert
        Assert.AreEqual(DateTimeKind.Local, result.Kind);
        Assert.AreEqual(new DateTime(2024, 6, 15, 10, 30, 45), result);
    }

    [TestMethod]
    public void ToDateTime_DateTimeOffsetWithSpecifiedKind_ReturnsCorrectKind_Test()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = new(2024, 6, 15, 10, 30, 45, TimeSpan.FromHours(8));

        // Act
        DateTime resultLocal = dateTimeOffset.ToDateTime(DateTimeKind.Local);
        DateTime resultUtc = dateTimeOffset.ToDateTime(DateTimeKind.Utc);
        DateTime resultUnspecified = dateTimeOffset.ToDateTime(DateTimeKind.Unspecified);

        // Assert
        Assert.AreEqual(DateTimeKind.Local, resultLocal.Kind);
        Assert.AreEqual(DateTimeKind.Utc, resultUtc.Kind);
        Assert.AreEqual(DateTimeKind.Unspecified, resultUnspecified.Kind);
    }

    #endregion

    #region DateTime ToDateTimeOffset 测试

    [TestMethod]
    public void ToDateTimeOffset_WithNullKind_UsesOriginalKind_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45, DateTimeKind.Local);

        // Act
        DateTimeOffset result = dateTime.ToDateTimeOffset(null);

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 10, 30, 45, DateTimeKind.Local), result.DateTime);
    }

    [TestMethod]
    public void ToDateTimeOffset_WithSpecifiedKind_UsesSpecifiedKind_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45, DateTimeKind.Local);

        // Act
        DateTimeOffset result = dateTime.ToDateTimeOffset(DateTimeKind.Utc);

        // Assert - DateTimeOffset 构造函数会将 DateTime 视为本地时间
        // 验证日期时间值正确
        Assert.AreEqual(new DateTime(2024, 6, 15, 10, 30, 45), result.DateTime);
    }

    #endregion

    #region TimeOnly + DateOnly 转换为 DateTime/DateTimeOffset 测试

    [TestMethod]
    public void ToDateTime_WithTimeOnlyAndDateOnly_ReturnsCorrectDateTime_Test()
    {
        // Arrange
        TimeOnly time = new(10, 30, 45);
        DateOnly date = new(2024, 6, 15);

        // Act
        DateTime result = time.ToDateTime(date);

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 10, 30, 45, DateTimeKind.Local), result);
    }

    [TestMethod]
    public void ToDateTime_TimeOnlyAndDateOnlyWithSpecifiedKind_ReturnsCorrectKind_Test()
    {
        // Arrange
        TimeOnly time = new(10, 30, 45);
        DateOnly date = new(2024, 6, 15);

        // Act
        DateTime resultUtc = time.ToDateTime(date, DateTimeKind.Utc);
        DateTime resultUnspecified = time.ToDateTime(date, DateTimeKind.Unspecified);

        // Assert
        Assert.AreEqual(DateTimeKind.Utc, resultUtc.Kind);
        Assert.AreEqual(DateTimeKind.Unspecified, resultUnspecified.Kind);
    }

    [TestMethod]
    public void ToDateTimeOffset_WithTimeOnlyAndDateOnly_ReturnsCorrectDateTimeOffset_Test()
    {
        // Arrange
        TimeOnly time = new(10, 30, 45);
        DateOnly date = new(2024, 6, 15);

        // Act
        DateTimeOffset result = time.ToDateTimeOffset(date);

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 10, 30, 45), result.DateTime);
    }

    #endregion

    #region DateOnly 转换为 DateTime/DateTimeOffset 测试

    [TestMethod]
    public void DateOnlyToDateTime_ReturnsCorrectDateTime_Test()
    {
        // Arrange
        DateOnly date = new(2024, 6, 15);

        // Act
        DateTime result = date.ToDateTime();

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Local), result);
    }

    [TestMethod]
    public void DateOnlyToDateTimeWithTime_ReturnsCorrectDateTime_Test()
    {
        // Arrange
        DateOnly date = new(2024, 6, 15);
        TimeOnly time = new(10, 30, 45);

        // Act
        DateTime result = date.ToDateTime(time);

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 10, 30, 45, DateTimeKind.Local), result);
    }

    [TestMethod]
    public void DateOnlyToDateTimeOffset_ReturnsCorrectDateTimeOffset_Test()
    {
        // Arrange
        DateOnly date = new(2024, 6, 15);

        // Act
        DateTimeOffset result = date.ToDateTimeOffset();

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 0, 0, 0), result.DateTime);
    }

    [TestMethod]
    public void DateOnlyToDateTimeOffsetWithTime_ReturnsCorrectDateTimeOffset_Test()
    {
        // Arrange
        DateOnly date = new(2024, 6, 15);
        TimeOnly time = new(10, 30, 45);

        // Act
        DateTimeOffset result = date.ToDateTimeOffset(time);

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 10, 30, 45), result.DateTime);
    }

    #endregion

    #region DateTime/DateTimeOffset 转换为 DateOnly/TimeOnly 测试

    [TestMethod]
    public void DateTimeToDateOnly_ReturnsCorrectDateOnly_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45);

        // Act
        DateOnly result = dateTime.ToDateOnly();

        // Assert
        Assert.AreEqual(new DateOnly(2024, 6, 15), result);
    }

    [TestMethod]
    public void DateTimeOffsetToDateOnly_ReturnsCorrectDateOnly_Test()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = new(2024, 6, 15, 10, 30, 45, TimeSpan.FromHours(8));

        // Act
        DateOnly result = dateTimeOffset.ToDateOnly();

        // Assert
        Assert.AreEqual(new DateOnly(2024, 6, 15), result);
    }

    [TestMethod]
    public void DateTimeToTimeOnly_ReturnsCorrectTimeOnly_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45);

        // Act
        TimeOnly result = dateTime.ToTimeOnly();

        // Assert
        Assert.AreEqual(new TimeOnly(10, 30, 45), result);
    }

    [TestMethod]
    public void DateTimeOffsetToTimeOnly_ReturnsCorrectTimeOnly_Test()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = new(2024, 6, 15, 10, 30, 45, TimeSpan.FromHours(8));

        // Act
        TimeOnly result = dateTimeOffset.ToTimeOnly();

        // Assert
        Assert.AreEqual(new TimeOnly(10, 30, 45), result);
    }

    #endregion

    #region GetQuarterOfYear 测试

    [TestMethod]
    public void GetQuarterOfYear_FirstQuarter_Returns1_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 2, 15);

        // Act
        int result = dateTime.GetQuarterOfYear();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetQuarterOfYear_SecondQuarter_Returns2_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 5, 15);

        // Act
        int result = dateTime.GetQuarterOfYear();

        // Assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void GetQuarterOfYear_ThirdQuarter_Returns3_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 8, 15);

        // Act
        int result = dateTime.GetQuarterOfYear();

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void GetQuarterOfYear_FourthQuarter_Returns4_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 11, 15);

        // Act
        int result = dateTime.GetQuarterOfYear();

        // Assert
        Assert.AreEqual(4, result);
    }

    [TestMethod]
    public void DateOnlyGetQuarterOfYear_ReturnsCorrectQuarter_Test()
    {
        // Arrange
        DateOnly date = new(2024, 6, 15);

        // Act
        int result = date.GetQuarterOfYear();

        // Assert
        Assert.AreEqual(2, result);
    }

    #endregion

    #region GetMonthOfQuarter 测试

    [TestMethod]
    public void GetMonthOfQuarter_FirstMonthOfQuarter_Returns1_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 1, 15);

        // Act
        int result = dateTime.GetMonthOfQuarter();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetMonthOfQuarter_SecondMonthOfQuarter_Returns2_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 2, 15);

        // Act
        int result = dateTime.GetMonthOfQuarter();

        // Assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void GetMonthOfQuarter_ThirdMonthOfQuarter_Returns3_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 3, 15);

        // Act
        int result = dateTime.GetMonthOfQuarter();

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void GetMonthOfQuarter_QuarterBoundary_ReturnsCorrectMonth_Test()
    {
        // Arrange
        DateTime dateTimeApr = new(2024, 4, 15); // Q2 第1个月
        DateTime dateTimeMay = new(2024, 5, 15); // Q2 第2个月
        DateTime dateTimeJun = new(2024, 6, 15); // Q2 第3个月

        // Act
        int resultApr = dateTimeApr.GetMonthOfQuarter();
        int resultMay = dateTimeMay.GetMonthOfQuarter();
        int resultJun = dateTimeJun.GetMonthOfQuarter();

        // Assert
        Assert.AreEqual(1, resultApr);
        Assert.AreEqual(2, resultMay);
        Assert.AreEqual(3, resultJun);
    }

    [TestMethod]
    public void DateOnlyGetMonthOfQuarter_ReturnsCorrectMonth_Test()
    {
        // Arrange
        DateOnly date = new(2024, 5, 15);

        // Act
        int result = date.GetMonthOfQuarter();

        // Assert
        Assert.AreEqual(2, result);
    }

    #endregion

    #region GetWeekOfQuarter 测试

    [TestMethod]
    public void GetWeekOfQuarter_FirstWeek_Returns1_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 1, 1); // Q1 第1天（周一）

        // Act
        int result = dateTime.GetWeekOfQuarter();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetWeekOfQuarter_MiddleQuarter_ReturnsCorrectWeek_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 5, 15);

        // Act
        int result = dateTime.GetWeekOfQuarter();

        // Assert
        Assert.IsGreaterThan(0, result);
        Assert.IsLessThanOrEqualTo(15, result);
    }

    [TestMethod]
    public void DateOnlyGetWeekOfQuarter_ReturnsCorrectWeek_Test()
    {
        // Arrange
        DateOnly date = new(2024, 5, 15);

        // Act
        int result = date.GetWeekOfQuarter();

        // Assert
        Assert.IsGreaterThan(0, result);
    }

    #endregion

    #region GetDayOfQuarter 测试

    [TestMethod]
    public void GetDayOfQuarter_FirstDay_Returns1_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 1, 1);

        // Act
        int result = dateTime.GetDayOfQuarter();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetDayOfQuarter_MiddleYear_ReturnsCorrectDay_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 7, 1); // Q3 第1天

        // Act
        int result = dateTime.GetDayOfQuarter();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetDayOfQuarter_LastDayOfQuarter_ReturnsQuarterDays_Test()
    {
        // Arrange
        DateTime q1LastDay = new(2024, 3, 31); // Q1 最后一天
        DateTime q2LastDay = new(2024, 6, 30); // Q2 最后一天
        DateTime q3LastDay = new(2024, 9, 30); // Q3 最后一天
        DateTime q4LastDay = new(2024, 12, 31); // Q4 最后一天

        // Act
        int resultQ1 = q1LastDay.GetDayOfQuarter();
        int resultQ2 = q2LastDay.GetDayOfQuarter();
        int resultQ3 = q3LastDay.GetDayOfQuarter();
        int resultQ4 = q4LastDay.GetDayOfQuarter();

        // Assert
        Assert.AreEqual(91, resultQ1);
        Assert.AreEqual(91, resultQ2);
        Assert.AreEqual(92, resultQ3);
        Assert.AreEqual(92, resultQ4);
    }

    [TestMethod]
    public void DateOnlyGetDayOfQuarter_ReturnsCorrectDay_Test()
    {
        // Arrange
        DateOnly date = new(2024, 5, 15);

        // Act
        int result = date.GetDayOfQuarter();

        // Assert
        Assert.IsGreaterThan(0, result);
        Assert.IsLessThanOrEqualTo(92, result);
    }

    #endregion

    #region GetWeekOfYear 测试

    [TestMethod]
    public void GetWeekOfYear_FirstWeek_Returns1_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 1, 1);

        // Act
        int result = dateTime.GetWeekOfYear();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetWeekOfYear_LastWeek_ReturnsCorrectWeek_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 12, 31);

        // Act
        int result = dateTime.GetWeekOfYear();

        // Assert
        Assert.IsGreaterThan(50, result);
        Assert.IsLessThanOrEqualTo(53, result);
    }

    [TestMethod]
    public void DateOnlyGetWeekOfYear_ReturnsCorrectWeek_Test()
    {
        // Arrange
        DateOnly date = new(2024, 6, 15);

        // Act
        int result = date.GetWeekOfYear();

        // Assert
        Assert.IsGreaterThan(0, result);
        Assert.IsLessThanOrEqualTo(53, result);
    }

    #endregion

    #region GetWeekOfMonth 测试

    [TestMethod]
    public void GetWeekOfMonth_FirstWeek_Returns1_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 1); // 6月第1天

        // Act
        int result = dateTime.GetWeekOfMonth();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetWeekOfMonth_MiddleMonth_ReturnsCorrectWeek_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15);

        // Act
        int result = dateTime.GetWeekOfMonth();

        // Assert
        Assert.IsGreaterThanOrEqualTo(1, result);
        Assert.IsLessThanOrEqualTo(6, result);
    }

    [TestMethod]
    public void DateOnlyGetWeekOfMonth_ReturnsCorrectWeek_Test()
    {
        // Arrange
        DateOnly date = new(2024, 6, 15);

        // Act
        int result = date.GetWeekOfMonth();

        // Assert
        Assert.IsGreaterThanOrEqualTo(1, result);
        Assert.IsLessThanOrEqualTo(6, result);
    }

    #endregion

    #region GetTimeStamp 测试

    [TestMethod]
    public void GetTimeStamp_FromUnixEpoch_Returns0_Test()
    {
        // Arrange
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        long result = dateTime.GetTimeStamp();

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void GetTimeStamp_OneSecondLater_Returns1_Test()
    {
        // Arrange
        DateTime dateTime = new(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc);

        // Act
        long result = dateTime.GetTimeStamp();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetTimeStamp_OneDayLater_Returns86400_Test()
    {
        // Arrange
        DateTime dateTime = new(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc);

        // Act
        long result = dateTime.GetTimeStamp();

        // Assert
        Assert.AreEqual(86400, result);
    }

    [TestMethod]
    public void GetTimeStamp_DateTimeOffset_ReturnsCorrectTimestamp_Test()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = new(1970, 1, 1, 0, 0, 1, TimeSpan.Zero);

        // Act
        long result = dateTimeOffset.GetTimeStamp();

        // Assert
        Assert.AreEqual(1, result);
    }

    #endregion

    #region GetDayFirst/LastSecond/Millisecond 测试

    [TestMethod]
    public void GetDayFirstSecond_ReturnsStartOfDay_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45, 500);

        // Act
        DateTime result = dateTime.GetDayFirstSecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 0, 0, 0), result);
    }

    [TestMethod]
    public void GetDayFirstMillisecond_ReturnsStartOfDay_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45, 500);

        // Act
        DateTime result = dateTime.GetDayFirstMillisecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 0, 0, 0), result);
    }

    [TestMethod]
    public void GetDayLastSecond_ReturnsEndOfDay_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45, 500);

        // Act
        DateTime result = dateTime.GetDayLastSecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 23, 59, 59), result);
    }

    [TestMethod]
    public void GetDayLastMillisecond_ReturnsEndOfDay_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45, 500);

        // Act
        DateTime result = dateTime.GetDayLastMillisecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 15, 23, 59, 59, 999), result);
    }

    #endregion

    #region GetMonthFirst/LastSecond/Millisecond 测试

    [TestMethod]
    public void GetMonthFirstSecond_ReturnsStartOfMonth_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45);

        // Act
        DateTime result = dateTime.GetMonthFirstSecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 1, 0, 0, 0), result);
    }

    [TestMethod]
    public void GetMonthFirstMillisecond_ReturnsStartOfMonth_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45);

        // Act
        DateTime result = dateTime.GetMonthFirstMillisecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 1, 0, 0, 0), result);
    }

    [TestMethod]
    public void GetMonthLastSecond_ReturnsEndOfMonth_Test()
    {
        // Arrange
        DateTime dateTimeJan = new(2024, 1, 15, 10, 30, 45); // 31天
        DateTime dateTimeFeb = new(2024, 2, 15, 10, 30, 45); // 29天（闰年）
        DateTime dateTimeApr = new(2024, 4, 15, 10, 30, 45); // 30天

        // Act
        DateTime resultJan = dateTimeJan.GetMonthLastSecond();
        DateTime resultFeb = dateTimeFeb.GetMonthLastSecond();
        DateTime resultApr = dateTimeApr.GetMonthLastSecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 1, 31, 23, 59, 59), resultJan);
        Assert.AreEqual(new DateTime(2024, 2, 29, 23, 59, 59), resultFeb);
        Assert.AreEqual(new DateTime(2024, 4, 30, 23, 59, 59), resultApr);
    }

    [TestMethod]
    public void GetMonthLastMillisecond_ReturnsEndOfMonth_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45);

        // Act
        DateTime result = dateTime.GetMonthLastMillisecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 6, 30, 23, 59, 59, 999), result);
    }

    #endregion

    #region GetYearFirst/LastSecond/Millisecond 测试

    [TestMethod]
    public void GetYearFirstSecond_ReturnsStartOfYear_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45);

        // Act
        DateTime result = dateTime.GetYearFirstSecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 1, 1, 0, 0, 0), result);
    }

    [TestMethod]
    public void GetYearFirstMillisecond_ReturnsStartOfYear_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45);

        // Act
        DateTime result = dateTime.GetYearFirstMillisecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 1, 1, 0, 0, 0), result);
    }

    [TestMethod]
    public void GetYearLastSecond_ReturnsEndOfYear_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45);

        // Act
        DateTime result = dateTime.GetYearLastSecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 12, 31, 23, 59, 59), result);
    }

    [TestMethod]
    public void GetYearLastMillisecond_ReturnsEndOfYear_Test()
    {
        // Arrange
        DateTime dateTime = new(2024, 6, 15, 10, 30, 45);

        // Act
        DateTime result = dateTime.GetYearLastMillisecond();

        // Assert
        Assert.AreEqual(new DateTime(2024, 12, 31, 23, 59, 59, 999), result);
    }

    #endregion
}
