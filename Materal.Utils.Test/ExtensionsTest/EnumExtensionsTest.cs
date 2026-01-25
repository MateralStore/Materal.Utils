namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// EnumExtensions 测试类
/// 测试枚举扩展方法的功能
/// </summary>
[TestClass]
public class EnumExtensionsTest
{
    #region GetAllEnum Tests

    /// <summary>
    /// 测试从枚举实例获取所有枚举值
    /// </summary>
    [TestMethod]
    public void GetAllEnum_FromEnumInstance_ReturnsAllValues_Test()
    {
        // Arrange
        TestEnum testEnum = TestEnum.Value1;

        // Act
        List<Enum> result = testEnum.GetAllEnum();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(3, result);
        CollectionAssert.Contains(result, TestEnum.Value1);
        CollectionAssert.Contains(result, TestEnum.Value2);
        CollectionAssert.Contains(result, TestEnum.Value3);
    }

    /// <summary>
    /// 测试从类型获取所有枚举值
    /// </summary>
    [TestMethod]
    public void GetAllEnum_FromType_ReturnsAllValues_Test()
    {
        // Arrange
        Type enumType = typeof(TestEnum);

        // Act
        List<Enum> result = enumType.GetAllEnum();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(3, result);
    }

    /// <summary>
    /// 测试从非枚举类型获取枚举值抛出异常
    /// </summary>
    [TestMethod]
    public void GetAllEnum_FromNonEnumType_ThrowsUtilException_Test()
    {
        // Arrange
        Type nonEnumType = typeof(string);

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => nonEnumType.GetAllEnum());
    }

    /// <summary>
    /// 测试null类型抛出异常
    /// </summary>
    [TestMethod]
    public void GetAllEnum_WithNullType_ThrowsArgumentNullException_Test()
    {
        // Arrange
        Type type = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => type.GetAllEnum());
    }

    /// <summary>
    /// 测试获取标志枚举的所有值
    /// </summary>
    [TestMethod]
    public void GetAllEnum_WithFlagsEnum_ReturnsAllValues_Test()
    {
        // Arrange
        Type flagsEnumType = typeof(TestFlagsEnum);

        // Act
        List<Enum> result = flagsEnumType.GetAllEnum();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(4, result);
    }

    #endregion

    #region GetEnumCount Tests

    /// <summary>
    /// 测试获取枚举值总数
    /// </summary>
    [TestMethod]
    public void GetEnumCount_ReturnsCorrectCount_Test()
    {
        // Arrange
        TestEnum testEnum = TestEnum.Value1;

        // Act
        int result = testEnum.GetEnumCount();

        // Assert
        Assert.AreEqual(3, result);
    }

    /// <summary>
    /// 测试获取空枚举的值
    /// </summary>
    [TestMethod]
    public void GetAllEnum_WithEmptyEnum_ReturnsEmptyList_Test()
    {
        // Arrange
        Type emptyEnumType = typeof(EmptyEnum);

        // Act
        List<Enum> allEnums = emptyEnumType.GetAllEnum();

        // Assert
        Assert.IsEmpty(allEnums);
    }

    /// <summary>
    /// 测试获取空枚举的计数
    /// </summary>
    [TestMethod]
    public void GetEnumCount_WithEmptyEnum_ReturnsZero_Test()
    {
        // Arrange
        Type emptyEnumType = typeof(EmptyEnum);

        // Act
        int count = emptyEnumType.GetAllEnum().Count;

        // Assert
        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 测试获取标志枚举的计数
    /// </summary>
    [TestMethod]
    public void GetEnumCount_WithFlagsEnum_ReturnsCorrectCount_Test()
    {
        // Arrange
        TestFlagsEnum flagsEnum = TestFlagsEnum.None;

        // Act
        int result = flagsEnum.GetEnumCount();

        // Assert
        Assert.AreEqual(4, result);
    }

    #endregion

    #region Helper Enums

    /// <summary>
    /// 测试用的枚举
    /// </summary>
    private enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }

    /// <summary>
    /// 测试用的标志枚举
    /// </summary>
    [Flags]
    private enum TestFlagsEnum
    {
        None = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag3 = 4
    }

    /// <summary>
    /// 测试用的空枚举
    /// </summary>
    private enum EmptyEnum
    {
    }

    #endregion
}
