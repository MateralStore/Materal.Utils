namespace Materal.Utils.Helpers;

/// <summary>
/// 日期时间帮助类
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// 获得时间戳
    /// 1970年1月1日 0点0分0秒以来的秒数
    /// </summary>
    /// <returns>时间戳</returns>
    public static long GetTimeStamp(DateTimeKind dateTimeKind = DateTimeKind.Utc)
    {
        DateTime dateTime = dateTimeKind switch
        {
            DateTimeKind.Utc => DateTime.UtcNow,
            DateTimeKind.Local => DateTime.Now,
            _ => throw new InvalidEnumArgumentException($"不支持{dateTimeKind}")
        };
        return dateTime.GetTimeStamp();
    }
    /// <summary>
    /// 时间戳转换为时间
    /// 1970年1月1日 0点0分0秒以来的秒数
    /// </summary>
    /// <param name="timeStamp">时间戳（秒数）</param>
    /// <param name="dateTimeKind">DateTimeKind</param>
    /// <returns>转换后的DateTime对象</returns>
    public static DateTime TimeStampToDateTime(long timeStamp, DateTimeKind dateTimeKind = DateTimeKind.Utc)
    {
        DateTime dtStart = new(1970, 1, 1, 0, 0, 0, 0, dateTimeKind);
        TimeSpan target = TimeSpan.FromSeconds(timeStamp);
        DateTime result = dtStart.Add(target);
        return result;
    }
    /// <summary>
    /// 时间戳转换为时间
    /// 1970年1月1日 0点0分0秒以来的秒数
    /// </summary>
    /// <param name="timeStamp">时间戳（秒数）</param>
    /// <param name="dateTimeKind">DateTimeKind</param>
    /// <returns>转换后的DateTimeOffset对象</returns>
    public static DateTimeOffset TimeStampToDateTimeOffset(long timeStamp, DateTimeKind dateTimeKind = DateTimeKind.Utc)
        => TimeStampToDateTime(timeStamp, dateTimeKind).ToDateTimeOffset();
    /// <summary>
    /// 转换为毫秒
    /// </summary>
    /// <param name="timeValue">时间值</param>
    /// <param name="dateTimeType">时间单位类型</param>
    /// <returns>转换后的毫秒数</returns>
    public static double ToMilliseconds(double timeValue, DateTimeUnit dateTimeType)
    {
        return dateTimeType switch
        {
            DateTimeUnit.YearUnit => timeValue * 365.25 * 24 * 60 * 60 * 1000, // 考虑闰年
            DateTimeUnit.MonthUnit => timeValue * 30.44 * 24 * 60 * 60 * 1000, // 平均每月天数
            DateTimeUnit.DayUnit => timeValue * 24 * 60 * 60 * 1000,
            DateTimeUnit.HourUnit => timeValue * 60 * 60 * 1000,
            DateTimeUnit.MinuteUnit => timeValue * 60 * 1000,
            DateTimeUnit.SecondUnit => timeValue * 1000,
            DateTimeUnit.MillisecondUnit => timeValue,
            _ => throw new InvalidEnumArgumentException($"不支持的时间单位类型: {dateTimeType}")
        };
    }
    /// <summary>
    /// 转换为秒
    /// </summary>
    /// <param name="timeValue">时间值</param>
    /// <param name="dateTimeType">时间单位类型</param>
    /// <returns>转换后的秒数</returns>
    public static double ToSeconds(double timeValue, DateTimeUnit dateTimeType)
    {
        return dateTimeType switch
        {
            DateTimeUnit.YearUnit => timeValue * 365.25 * 24 * 60 * 60, // 考虑闰年
            DateTimeUnit.MonthUnit => timeValue * 30.44 * 24 * 60 * 60, // 平均每月天数
            DateTimeUnit.DayUnit => timeValue * 24 * 60 * 60,
            DateTimeUnit.HourUnit => timeValue * 60 * 60,
            DateTimeUnit.MinuteUnit => timeValue * 60,
            DateTimeUnit.SecondUnit => timeValue,
            DateTimeUnit.MillisecondUnit => timeValue / 1000,
            _ => throw new InvalidEnumArgumentException($"不支持的时间单位类型: {dateTimeType}")
        };
    }
    /// <summary>
    /// 转换为分钟
    /// </summary>
    /// <param name="timeValue">时间值</param>
    /// <param name="dateTimeType">时间单位类型</param>
    /// <returns>转换后的分钟数</returns>
    public static double ToMinutes(double timeValue, DateTimeUnit dateTimeType)
    {
        return dateTimeType switch
        {
            DateTimeUnit.YearUnit => timeValue * 365.25 * 24 * 60, // 考虑闰年
            DateTimeUnit.MonthUnit => timeValue * 30.44 * 24 * 60, // 平均每月天数
            DateTimeUnit.DayUnit => timeValue * 24 * 60,
            DateTimeUnit.HourUnit => timeValue * 60,
            DateTimeUnit.MinuteUnit => timeValue,
            DateTimeUnit.SecondUnit => timeValue / 60,
            DateTimeUnit.MillisecondUnit => timeValue / 60 / 1000,
            _ => throw new InvalidEnumArgumentException($"不支持的时间单位类型: {dateTimeType}")
        };
    }
    /// <summary>
    /// 转换为小时
    /// </summary>
    /// <param name="timeValue">时间值</param>
    /// <param name="dateTimeType">时间单位类型</param>
    /// <returns>转换后的小时数</returns>
    public static double ToHours(double timeValue, DateTimeUnit dateTimeType)
    {
        return dateTimeType switch
        {
            DateTimeUnit.YearUnit => timeValue * 365.25 * 24, // 考虑闰年
            DateTimeUnit.MonthUnit => timeValue * 30.44 * 24, // 平均每月天数
            DateTimeUnit.DayUnit => timeValue * 24,
            DateTimeUnit.HourUnit => timeValue,
            DateTimeUnit.MinuteUnit => timeValue / 60,
            DateTimeUnit.SecondUnit => timeValue / 60 / 60,
            DateTimeUnit.MillisecondUnit => timeValue / 60 / 60 / 1000,
            _ => throw new InvalidEnumArgumentException($"不支持的时间单位类型: {dateTimeType}")
        };
    }
    /// <summary>
    /// 转换为天
    /// </summary>
    /// <param name="timeValue">时间值</param>
    /// <param name="dateTimeType">时间单位类型</param>
    /// <returns>转换后的天数</returns>
    public static double ToDay(double timeValue, DateTimeUnit dateTimeType)
    {
        return dateTimeType switch
        {
            DateTimeUnit.YearUnit => timeValue * 365.25, // 考虑闰年
            DateTimeUnit.MonthUnit => timeValue * 30.44, // 平均每月天数
            DateTimeUnit.DayUnit => timeValue,
            DateTimeUnit.HourUnit => timeValue / 24,
            DateTimeUnit.MinuteUnit => timeValue / 24 / 60,
            DateTimeUnit.SecondUnit => timeValue / 24 / 60 / 60,
            DateTimeUnit.MillisecondUnit => timeValue / 24 / 60 / 60 / 1000,
            _ => throw new InvalidEnumArgumentException($"不支持的时间单位类型: {dateTimeType}")
        };
    }
    /// <summary>
    /// 转换为月
    /// </summary>
    /// <param name="timeValue">时间值</param>
    /// <param name="dateTimeType">时间单位类型</param>
    /// <returns>转换后的月数</returns>
    public static double ToMonth(double timeValue, DateTimeUnit dateTimeType)
    {
        return dateTimeType switch
        {
            DateTimeUnit.YearUnit => timeValue * 12,
            DateTimeUnit.MonthUnit => timeValue,
            DateTimeUnit.DayUnit => timeValue / 30.44, // 平均每月天数
            DateTimeUnit.HourUnit => timeValue / 30.44 / 24,
            DateTimeUnit.MinuteUnit => timeValue / 30.44 / 24 / 60,
            DateTimeUnit.SecondUnit => timeValue / 30.44 / 24 / 60 / 60,
            DateTimeUnit.MillisecondUnit => timeValue / 30.44 / 24 / 60 / 60 / 1000,
            _ => throw new InvalidEnumArgumentException($"不支持的时间单位类型: {dateTimeType}")
        };
    }
    /// <summary>
    /// 转换为年
    /// </summary>
    /// <param name="timeValue">时间值</param>
    /// <param name="dateTimeType">时间单位类型</param>
    /// <returns>转换后的年数</returns>
    public static double ToYear(double timeValue, DateTimeUnit dateTimeType)
    {
        return dateTimeType switch
        {
            DateTimeUnit.YearUnit => timeValue,
            DateTimeUnit.MonthUnit => timeValue / 12,
            DateTimeUnit.DayUnit => timeValue / 365.25, // 考虑闰年
            DateTimeUnit.HourUnit => timeValue / 365.25 / 24,
            DateTimeUnit.MinuteUnit => timeValue / 365.25 / 24 / 60,
            DateTimeUnit.SecondUnit => timeValue / 365.25 / 24 / 60 / 60,
            DateTimeUnit.MillisecondUnit => timeValue / 365.25 / 24 / 60 / 60 / 1000,
            _ => throw new InvalidEnumArgumentException($"不支持的时间单位类型: {dateTimeType}")
        };
    }
}
