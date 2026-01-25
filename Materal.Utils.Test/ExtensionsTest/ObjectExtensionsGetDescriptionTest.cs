using System.Reflection;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// ObjectExtensions.GetDescription 测试类
/// 测试 Object 扩展方法中的 GetDescription 相关功能
/// </summary>
[TestClass]
public class ObjectExtensionsGetDescriptionTest
{
    #region 测试模型类

    /// <summary>
    /// 带描述特性的枚举
    /// </summary>
    public enum TestEnum
    {
        [Description("值1描述")]
        Value1,
        [Description("值2描述")]
        Value2,
        Value3
    }

    /// <summary>
    /// 带类级别描述特性的类
    /// </summary>
    [Description("这是一个测试类")]
    public class ClassWithDescription
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 带Description属性的类
    /// </summary>
    public class ClassWithDescriptionProperty
    {
        public int Id { get; set; }
        public string Description { get; set; } = "默认描述";
    }

    /// <summary>
    /// 带Description字段的类
    /// </summary>
    public class ClassWithDescriptionField
    {
        public int Id { get; set; }
        public string Description = "字段描述";
    }

    /// <summary>
    /// 无描述的普通类
    /// </summary>
    public class ClassWithoutDescription
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 带成员描述特性的类
    /// </summary>
    public class ClassWithMemberDescription
    {
        [Description("ID属性描述")]
        public int Id { get; set; }

        [Description("名称字段描述")]
        public string NameField = "测试";

        public string NormalProperty { get; set; } = string.Empty;
    }

    /// <summary>
    /// Description属性为null的类
    /// </summary>
    public class ClassWithNullDescription
    {
        public int Id { get; set; }
        public string? Description { get; set; } = null;
    }

    /// <summary>
    /// Description为非字符串类型的类
    /// </summary>
    public class ClassWithNonStringDescription
    {
        public int Id { get; set; }
        public int Description { get; set; } = 123;
    }

    #endregion

    #region GetDescription(object) Tests

    /// <summary>
    /// 测试枚举值返回描述特性
    /// </summary>
    [TestMethod]
    public void GetDescription_WithEnumHavingDescription_ReturnsDescription_Test()
    {
        // Arrange
        TestEnum enumValue = TestEnum.Value1;

        // Act
        string result = enumValue.GetDescription();

        // Assert
        Assert.AreEqual("值1描述", result);
    }

    /// <summary>
    /// 测试无描述特性的枚举值返回枚举名称
    /// </summary>
    [TestMethod]
    public void GetDescription_WithEnumWithoutDescription_ReturnsEnumName_Test()
    {
        // Arrange
        TestEnum enumValue = TestEnum.Value3;

        // Act
        string result = enumValue.GetDescription();

        // Assert
        Assert.AreEqual("Value3", result);
    }

    /// <summary>
    /// 测试带类级别描述特性的对象返回描述
    /// </summary>
    [TestMethod]
    public void GetDescription_WithClassHavingDescription_ReturnsDescription_Test()
    {
        // Arrange
        ClassWithDescription obj = new() { Id = 1, Name = "Test" };

        // Act
        string result = obj.GetDescription();

        // Assert
        Assert.AreEqual("这是一个测试类", result);
    }

    /// <summary>
    /// 测试带Description属性的对象返回属性值
    /// </summary>
    [TestMethod]
    public void GetDescription_WithDescriptionProperty_ReturnsPropertyValue_Test()
    {
        // Arrange
        ClassWithDescriptionProperty obj = new() { Id = 1, Description = "自定义描述" };

        // Act
        string result = obj.GetDescription();

        // Assert
        Assert.AreEqual("自定义描述", result);
    }

    /// <summary>
    /// 测试带Description字段的对象返回字段值
    /// </summary>
    [TestMethod]
    public void GetDescription_WithDescriptionField_ReturnsFieldValue_Test()
    {
        // Arrange
        ClassWithDescriptionField obj = new() { Id = 1, Description = "字段描述值" };

        // Act
        string result = obj.GetDescription();

        // Assert
        Assert.AreEqual("字段描述值", result);
    }

    /// <summary>
    /// 测试无描述的对象抛出异常
    /// </summary>
    [TestMethod]
    public void GetDescription_WithoutDescription_ThrowsException_Test()
    {
        // Arrange
        ClassWithoutDescription obj = new() { Id = 1, Name = "Test" };

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => obj.GetDescription());
    }

    /// <summary>
    /// 测试Description属性为null时抛出异常
    /// </summary>
    [TestMethod]
    public void GetDescription_WithNullDescriptionProperty_ThrowsException_Test()
    {
        // Arrange
        ClassWithNullDescription obj = new() { Id = 1, Description = null };

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => obj.GetDescription());
    }

    /// <summary>
    /// 测试Description为非字符串类型时抛出异常
    /// </summary>
    [TestMethod]
    public void GetDescription_WithNonStringDescription_ThrowsException_Test()
    {
        // Arrange
        ClassWithNonStringDescription obj = new() { Id = 1, Description = 123 };

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => obj.GetDescription());
    }

    #endregion

    #region GetDescriptionOrNull(object) Tests

    /// <summary>
    /// 测试枚举值返回描述
    /// </summary>
    [TestMethod]
    public void GetDescriptionOrNull_WithEnumHavingDescription_ReturnsDescription_Test()
    {
        // Arrange
        TestEnum enumValue = TestEnum.Value2;

        // Act
        string? result = enumValue.GetDescriptionOrNull();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("值2描述", result);
    }

    /// <summary>
    /// 测试无描述的对象返回null
    /// </summary>
    [TestMethod]
    public void GetDescriptionOrNull_WithoutDescription_ReturnsNull_Test()
    {
        // Arrange
        ClassWithoutDescription obj = new() { Id = 1, Name = "Test" };

        // Act
        string? result = obj.GetDescriptionOrNull();

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试带描述特性的对象返回描述
    /// </summary>
    [TestMethod]
    public void GetDescriptionOrNull_WithClassHavingDescription_ReturnsDescription_Test()
    {
        // Arrange
        ClassWithDescription obj = new() { Id = 1, Name = "Test" };

        // Act
        string? result = obj.GetDescriptionOrNull();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("这是一个测试类", result);
    }

    /// <summary>
    /// 测试Description属性为null时返回null
    /// </summary>
    [TestMethod]
    public void GetDescriptionOrNull_WithNullDescriptionProperty_ReturnsNull_Test()
    {
        // Arrange
        ClassWithNullDescription obj = new() { Id = 1, Description = null };

        // Act
        string? result = obj.GetDescriptionOrNull();

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region GetDescription(object, MemberInfo) Tests

    /// <summary>
    /// 测试获取带描述特性的属性描述
    /// </summary>
    [TestMethod]
    public void GetDescription_WithMemberInfoHavingDescription_ReturnsDescription_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { Id = 1 };
        MemberInfo memberInfo = typeof(ClassWithMemberDescription).GetProperty("Id")!;

        // Act
        string result = obj.GetDescription(memberInfo);

        // Assert
        Assert.AreEqual("ID属性描述", result);
    }

    /// <summary>
    /// 测试获取带描述特性的字段描述
    /// </summary>
    [TestMethod]
    public void GetDescription_WithFieldMemberInfoHavingDescription_ReturnsDescription_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { NameField = "测试" };
        MemberInfo memberInfo = typeof(ClassWithMemberDescription).GetField("NameField")!;

        // Act
        string result = obj.GetDescription(memberInfo);

        // Assert
        Assert.AreEqual("名称字段描述", result);
    }

    /// <summary>
    /// 测试枚举成员返回枚举名称
    /// </summary>
    [TestMethod]
    public void GetDescription_WithEnumMemberInfo_ReturnsEnumName_Test()
    {
        // Arrange
        TestEnum enumValue = TestEnum.Value3;
        Type enumType = typeof(TestEnum);
        MemberInfo memberInfo = enumType.GetField("Value3")!;

        // Act
        string result = enumValue.GetDescription(memberInfo);

        // Assert
        Assert.AreEqual("Value3", result);
    }

    /// <summary>
    /// 测试Description成员名称返回字符串值
    /// </summary>
    [TestMethod]
    public void GetDescription_WithDescriptionMemberName_ReturnsStringValue_Test()
    {
        // Arrange
        ClassWithDescriptionProperty obj = new() { Id = 1, Description = "成员描述" };
        MemberInfo memberInfo = typeof(ClassWithDescriptionProperty).GetProperty("Description")!;

        // Act
        string result = obj.GetDescription(memberInfo);

        // Assert
        Assert.AreEqual("成员描述", result);
    }

    /// <summary>
    /// 测试无描述特性的成员抛出异常
    /// </summary>
    [TestMethod]
    public void GetDescription_WithMemberInfoWithoutDescription_ThrowsException_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { NormalProperty = "Test" };
        MemberInfo memberInfo = typeof(ClassWithMemberDescription).GetProperty("NormalProperty")!;

        // Act & Assert
        UtilException exception = Assert.ThrowsExactly<UtilException>(() => obj.GetDescription(memberInfo));
        Assert.Contains("DescriptionAttribute", exception.Message);
    }

    #endregion

    #region GetDescriptionOrNull(object, MemberInfo) Tests

    /// <summary>
    /// 测试获取带描述特性的成员返回描述
    /// </summary>
    [TestMethod]
    public void GetDescriptionOrNull_WithMemberInfoHavingDescription_ReturnsDescription_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { Id = 1 };
        MemberInfo memberInfo = typeof(ClassWithMemberDescription).GetProperty("Id")!;

        // Act
        string? result = obj.GetDescriptionOrNull(memberInfo);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("ID属性描述", result);
    }

    /// <summary>
    /// 测试无描述特性的成员返回null
    /// </summary>
    [TestMethod]
    public void GetDescriptionOrNull_WithMemberInfoWithoutDescription_ReturnsNull_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { NormalProperty = "Test" };
        MemberInfo memberInfo = typeof(ClassWithMemberDescription).GetProperty("NormalProperty")!;

        // Act
        string? result = obj.GetDescriptionOrNull(memberInfo);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region GetDescription(object, string) Tests

    /// <summary>
    /// 测试通过属性名称获取描述
    /// </summary>
    [TestMethod]
    public void GetDescription_WithPropertyName_ReturnsDescription_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { Id = 1 };

        // Act
        string result = obj.GetDescription("Id");

        // Assert
        Assert.AreEqual("ID属性描述", result);
    }

    /// <summary>
    /// 测试通过字段名称获取描述
    /// </summary>
    [TestMethod]
    public void GetDescription_WithFieldName_ReturnsDescription_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { NameField = "测试" };

        // Act
        string result = obj.GetDescription("NameField");

        // Assert
        Assert.AreEqual("名称字段描述", result);
    }

    /// <summary>
    /// 测试通过Description成员名称获取值
    /// </summary>
    [TestMethod]
    public void GetDescription_WithDescriptionMemberNameString_ReturnsValue_Test()
    {
        // Arrange
        ClassWithDescriptionProperty obj = new() { Id = 1, Description = "描述值" };

        // Act
        string result = obj.GetDescription("Description");

        // Assert
        Assert.AreEqual("描述值", result);
    }

    /// <summary>
    /// 测试不存在的成员名称抛出异常
    /// </summary>
    [TestMethod]
    public void GetDescription_WithNonExistentMemberName_ThrowsException_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { Id = 1 };

        // Act & Assert
        UtilException exception = Assert.ThrowsExactly<UtilException>(() => obj.GetDescription("NonExistent"));
        Assert.Contains("NonExistent", exception.Message);
    }

    /// <summary>
    /// 测试枚举通过成员名称获取描述
    /// </summary>
    [TestMethod]
    public void GetDescription_WithEnumMemberName_ReturnsDescription_Test()
    {
        // Arrange
        TestEnum enumValue = TestEnum.Value1;

        // Act
        string result = enumValue.GetDescription("Value1");

        // Assert
        Assert.AreEqual("值1描述", result);
    }

    #endregion

    #region GetDescriptionOrNull(object, string) Tests

    /// <summary>
    /// 测试通过属性名称获取描述
    /// </summary>
    [TestMethod]
    public void GetDescriptionOrNull_WithPropertyName_ReturnsDescription_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { Id = 1 };

        // Act
        string? result = obj.GetDescriptionOrNull("Id");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("ID属性描述", result);
    }

    /// <summary>
    /// 测试不存在的成员名称返回null
    /// </summary>
    [TestMethod]
    public void GetDescriptionOrNull_WithNonExistentMemberName_ReturnsNull_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { Id = 1 };

        // Act
        string? result = obj.GetDescriptionOrNull("NonExistent");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 测试无描述特性的成员返回null
    /// </summary>
    [TestMethod]
    public void GetDescriptionOrNull_WithMemberNameWithoutDescription_ReturnsNull_Test()
    {
        // Arrange
        ClassWithMemberDescription obj = new() { NormalProperty = "Test" };

        // Act
        string? result = obj.GetDescriptionOrNull("NormalProperty");

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region 边界条件测试

    /// <summary>
    /// 测试Description字段为空字符串时返回空字符串
    /// </summary>
    [TestMethod]
    public void GetDescription_WithEmptyDescriptionField_ReturnsEmptyString_Test()
    {
        // Arrange
        ClassWithDescriptionField obj = new() { Id = 1, Description = string.Empty };

        // Act
        string result = obj.GetDescription();

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// 测试枚举的多个值
    /// </summary>
    [TestMethod]
    [DataRow(TestEnum.Value1, "值1描述")]
    [DataRow(TestEnum.Value2, "值2描述")]
    [DataRow(TestEnum.Value3, "Value3")]
    public void GetDescription_WithVariousEnumValues_ReturnsExpectedDescription_Test(TestEnum enumValue, string expected)
    {
        // Act
        string result = enumValue.GetDescription();

        // Assert
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// 测试类级别描述特性优先级高于Description属性
    /// </summary>
    [TestMethod]
    public void GetDescription_WithBothClassAttributeAndProperty_PrefersAttribute_Test()
    {
        // Arrange
        ClassWithDescriptionAndAttribute obj = new() { Description = "属性描述" };

        // Act
        string result = obj.GetDescription();

        // Assert
        Assert.AreEqual("类特性描述", result);
    }

    /// <summary>
    /// 同时具有类特性和Description属性的类
    /// </summary>
    [Description("类特性描述")]
    public class ClassWithDescriptionAndAttribute
    {
        public string Description { get; set; } = "属性描述";
    }

    #endregion
}
