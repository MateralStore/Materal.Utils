using Materal.Utils.Enums;
using Materal.Utils.Helpers;

namespace Materal.Utils.Test.HelpersTest;

/// <summary>
/// EnumHelper 测试类
/// 测试枚举转换功能
/// </summary>
[TestClass]
public class EnumHelperTest
{
    #region ConvertToEnumByDescription Tests

    /// <summary>
    /// 测试根据描述转换为枚举值
    /// </summary>
    [TestMethod]
    public void ConvertToEnumByDescription_WithValidDescription_ReturnsEnumValue_Test()
    {
        // Act
        ResultType result = EnumHelper.ConvertToEnumByDescription<ResultType>("成功");

        // Assert
        Assert.AreEqual(ResultType.Success, result);
    }

    /// <summary>
    /// 测试根据描述转换为枚举值（失败）
    /// </summary>
    [TestMethod]
    public void ConvertToEnumByDescription_WithFailDescription_ReturnsEnumValue_Test()
    {
        // Act
        ResultType result = EnumHelper.ConvertToEnumByDescription<ResultType>("失败");

        // Assert
        Assert.AreEqual(ResultType.Fail, result);
    }

    /// <summary>
    /// 测试根据描述转换为枚举值（警告）
    /// </summary>
    [TestMethod]
    public void ConvertToEnumByDescription_WithWaringDescription_ReturnsEnumValue_Test()
    {
        // Act
        ResultType result = EnumHelper.ConvertToEnumByDescription<ResultType>("警告");

        // Assert
        Assert.AreEqual(ResultType.Warning, result);
    }

    /// <summary>
    /// 测试使用无效描述转换时抛出异常
    /// </summary>
    [TestMethod]
    public void ConvertToEnumByDescription_WithInvalidDescription_ThrowsUtilException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => EnumHelper.ConvertToEnumByDescription<ResultType>("无效描述"));
    }

    /// <summary>
    /// 测试使用 null 描述转换时抛出异常
    /// </summary>
    [TestMethod]
    public void ConvertToEnumByDescription_WithNullDescription_ThrowsArgumentNullException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => EnumHelper.ConvertToEnumByDescription<ResultType>(null!));
    }

    /// <summary>
    /// 测试使用空字符串描述转换时抛出异常
    /// </summary>
    [TestMethod]
    public void ConvertToEnumByDescription_WithEmptyDescription_ThrowsUtilException_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => EnumHelper.ConvertToEnumByDescription<ResultType>(""));
    }

    #endregion
}
