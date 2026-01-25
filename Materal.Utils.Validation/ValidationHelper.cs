using Materal.Utils.Extensions;
using Materal.Utils.Validation.Attributes;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Materal.Utils.Validation;

/// <summary>
/// 对象验证Helper类
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// 默认验证错误处理
    /// </summary>
    public static Dictionary<Type, Func<ValidationAttribute, string, object?, string>> DefaultValidationFailHandler { get; } = new()
    {
        [typeof(RequiredAttribute)] = (a, m, o) => GetValidationFailMessage<RequiredAttribute>(a, m, o, (ta, tm, to) => $"{tm}必填"),
        [typeof(MinAttribute)] = (a, m, o) => GetValidationFailMessage<MinAttribute>(a, m, o, (ta, tm, to) => $"{tm}必须大于{ta.MinValue}"),
        [typeof(MaxAttribute)] = (a, m, o) => GetValidationFailMessage<MaxAttribute>(a, m, o, (ta, tm, to) => $"{tm}必须小于{ta.MaxValue}"),
        [typeof(RangeAttribute)] = (a, m, o) => GetValidationFailMessage<RangeAttribute>(a, m, o, (ta, tm, to) => $"{tm}必须在{ta.Minimum}-{ta.Maximum}之间"),
        [typeof(MinLengthAttribute)] = (a, m, o) => GetValidationFailMessage<MinLengthAttribute>(a, m, o, (ta, tm, to) => $"{tm}长度必须大于{ta.Length}"),
        [typeof(MaxLengthAttribute)] = (a, m, o) => GetValidationFailMessage<MaxLengthAttribute>(a, m, o, (ta, tm, to) => $"{tm}长度必须小于{ta.Length}"),
        [typeof(StringLengthAttribute)] = (a, m, o) => GetValidationFailMessage<StringLengthAttribute>(a, m, o, (ta, tm, to) => $"{tm}长度必须在{ta.MinimumLength}-{ta.MaximumLength}之间"),
    };
    /// <summary>
    /// 验证模型
    /// </summary>
    /// <param name="model">要验证的模型</param>
    /// <param name="errorMessage">错误消息输出</param>
    /// <returns>验证是否成功</returns>
    /// <example>
    /// <code>
    /// var user = new User
    /// {
    ///     Name = "",
    ///     Age = 15
    /// };
    /// if (!ValidationHelper.Validate(user, out string errorMessage))
    /// {
    ///     Console.WriteLine($"验证失败：{errorMessage}");
    /// }
    /// </code>
    /// </example>
    public static bool Validate(object model, out string errorMessage)
    {
        try
        {
            Validate(model);
            errorMessage = string.Empty;
            return true;
        }
        catch (ValidationException ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }
    /// <summary>
    /// 验证模型
    /// </summary>
    /// <param name="model">要验证的模型</param>
    /// <exception cref="ValidationException">验证失败时抛出</exception>
    /// <example>
    /// <code>
    /// var user = new User { Name = "张三" };
    /// try
    /// {
    ///     ValidationHelper.Validate(user);
    /// }
    /// catch (ValidationException ex)
    /// {
    ///     Console.WriteLine($"验证失败：{ex.Message}");
    /// }
    /// </code>
    /// </example>
    public static void Validate(object model) => Validate(model, "");
    /// <summary>
    /// 验证模型
    /// </summary>
    /// <param name="model">要验证的模型</param>
    /// <param name="prefix">属性前缀</param>
    public static void Validate(object model, string prefix)
    {
        Type modelType = model.GetType();
        List<MemberInfo> memberInfos = [.. modelType.GetProperties(), .. modelType.GetFields()];
        foreach (MemberInfo memberInfo in memberInfos)
        {
            Validate(model, memberInfo, prefix);
        }
    }
    /// <summary>
    /// 验证成员
    /// </summary>
    /// <param name="model">模型对象</param>
    /// <param name="memberInfo">成员信息</param>
    /// <param name="errorMessage">错误消息输出</param>
    /// <returns>验证是否成功</returns>
    /// <example>
    /// <code>
    /// var user = new User { Name = "" };
    /// var nameProperty = user.GetType().GetProperty("Name");
    /// if (!ValidationHelper.Validate(user, nameProperty, out string errorMessage))
    /// {
    ///     Console.WriteLine($"验证失败：{errorMessage}");
    /// }
    /// </code>
    /// </example>
    public static bool Validate(object model, MemberInfo memberInfo, out string errorMessage)
    {
        try
        {
            Validate(model, memberInfo, string.Empty);
            errorMessage = string.Empty;
            return true;
        }
        catch (ValidationException ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }
    /// <summary>
    /// 验证成员
    /// </summary>
    /// <param name="model">模型对象</param>
    /// <param name="memberInfo">成员信息</param>
    /// <param name="prefix">属性前缀</param>
    /// <exception cref="ValidationException">验证失败时抛出</exception>
    /// <example>
    /// <code>
    /// var user = new User { Name = "", Address = new Address { City = "" } };
    /// try
    /// {
    ///     ValidationHelper.Validate(user, "Address");  // 验证Address属性及其子属性
    /// }
    /// catch (ValidationException ex)
    /// {
    ///     Console.WriteLine($"验证失败：{ex.Message}");
    /// }
    /// </code>
    /// </example>
    public static void Validate(object model, MemberInfo memberInfo, string prefix = "")
    {
        if (IsDefaultType(model)) return;
        object? memberValue;
        try
        {
            memberValue = memberInfo.GetValue(model);
            Validate(memberValue, memberInfo.GetCustomAttributes<ValidationAttribute>(), validationAttribute => NewException(validationAttribute, prefix, memberInfo, memberValue));
            if (memberValue is null || IsDefaultType(memberValue)) return;
            string nextPrefix = memberInfo.Name;
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                nextPrefix = $"{prefix}.{nextPrefix}";
            }
            if (memberValue is ICollection collection)
            {
                int index = 0;
                foreach (object? item in collection)
                {
                    if (memberValue is null || IsDefaultType(item)) continue;
                    Validate(item, $"{nextPrefix}[{index}]");
                }
            }
            else
            {
                Validate(memberValue, $"{nextPrefix}");
            }
        }
        catch (ValidationException)
        {
            throw;
        }
        catch
        {
        }
    }
    /// <summary>
    /// 验证值
    /// </summary>
    /// <param name="value">要验证的值</param>
    /// <param name="validations">验证特性集合</param>
    /// <param name="newException">异常创建函数</param>
    /// <param name="errorMessage">错误消息输出</param>
    /// <returns>验证是否成功</returns>
    public static bool Validate(object? value, IEnumerable<ValidationAttribute> validations, Func<ValidationAttribute, ValidationException>? newException, out string errorMessage)
    {
        try
        {
            Validate(value, validations, newException);
            errorMessage = string.Empty;
            return true;
        }
        catch (ValidationException ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }
    /// <summary>
    /// 验证值
    /// </summary>
    /// <param name="value">要验证的值</param>
    /// <param name="validations">验证特性集合</param>
    /// <param name="newException">异常创建函数</param>
    public static void Validate(object? value, IEnumerable<ValidationAttribute> validations, Func<ValidationAttribute, ValidationException>? newException = null)
    {
        newException ??= validationAttribute => new ValidationException($"[{validationAttribute.GetType().Name}]验证失败", validationAttribute, value);
        foreach (ValidationAttribute validationAttribute in validations)
        {
            if (!validationAttribute.IsValid(value))
            {
                throw newException(validationAttribute);
            }
            else if (validationAttribute is RequiredAttribute requiredAttribute)
            {
                if (value is null) throw newException(validationAttribute);
                switch (value)
                {
                    case Guid guid when guid == Guid.Empty:
                        throw newException(validationAttribute);
                    case DateTime dateTime when dateTime == DateTime.MinValue:
                        throw newException(validationAttribute);
                }
            }
        }
    }
    /// <summary>
    /// 验证值
    /// </summary>
    /// <param name="value">要验证的值</param>
    /// <param name="validations">验证特性数组</param>
    public static void Validate(object? value, params ValidationAttribute[] validations) => Validate(value, validations, null);
    /// <summary>
    /// 新异常
    /// </summary>
    /// <param name="validationAttribute">验证特性</param>
    /// <param name="prefix">前缀</param>
    /// <param name="memberInfo">成员信息</param>
    /// <param name="memberValue">成员值</param>
    /// <returns>验证异常</returns>
    /// <exception cref="ValidationException"></exception>
    private static ValidationException NewException(ValidationAttribute validationAttribute, string prefix, MemberInfo memberInfo, object? memberValue)
    {
        string errorMessage = GetValidationFailMesage(validationAttribute, prefix, memberInfo, memberValue);
        return new ValidationException(errorMessage, validationAttribute, memberValue);
    }
    /// <summary>
    /// 是默认类型
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>是否为默认类型</returns>
    private static bool IsDefaultType(object value) => value is int || value is uint || value is short || value is ushort || value is long || value is ulong ||
            value is float || value is double || value is decimal || value is string || value is DateTime || value is TimeSpan || value is Guid ||
            value is Enum;
    /// <summary>
    /// 获得验证失败消息
    /// </summary>
    /// <param name="validationAttribute">验证特性</param>
    /// <param name="prefix">前缀</param>
    /// <param name="memberInfo">成员信息</param>
    /// <param name="propertyValue">属性值</param>
    /// <returns>错误消息</returns>
    private static string GetValidationFailMesage(ValidationAttribute validationAttribute, string prefix, MemberInfo memberInfo, object? propertyValue)
    {
        string? message = validationAttribute.ErrorMessage;
        if (!string.IsNullOrWhiteSpace(message)) return message;
        Type type = validationAttribute.GetType();
        string memberName = memberInfo.Name;
        if (!string.IsNullOrWhiteSpace(prefix))
        {
            memberName = $"{prefix}.{memberName}";
        }
        if (DefaultValidationFailHandler.TryGetValue(type, out Func<ValidationAttribute, string, object?, string>? value))
        {
            message = value(validationAttribute, memberName, propertyValue);
        }
        else
        {
            message = GetDefaultValidationFailMessage(memberName);
        }
        return message;
    }
    /// <summary>
    /// 获得默认验证失败消息
    /// </summary>
    /// <param name="memberName">成员名称</param>
    /// <returns>默认错误消息</returns>
    private static string GetDefaultValidationFailMessage(string memberName) => $"{memberName}验证失败";
    /// <summary>
    /// 获得必填验证失败消息
    /// </summary>
    /// <typeparam name="T">验证特性类型</typeparam>
    /// <param name="validationAttribute">验证特性</param>
    /// <param name="memberName">成员名称</param>
    /// <param name="propertyValue">属性值</param>
    /// <param name="getErrorMessage">获取错误消息函数</param>
    /// <returns>错误消息</returns>
    private static string GetValidationFailMessage<T>(ValidationAttribute validationAttribute, string memberName, object? propertyValue, Func<T, string, object?, string> getErrorMessage)
        where T : ValidationAttribute
    {
        if (validationAttribute is not T tAttribute) return GetDefaultValidationFailMessage(memberName);
        return getErrorMessage(tAttribute, memberName, propertyValue);
    }
}
