using System.Data;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// TypeExtensions 测试类
/// 测试 Type 扩展方法的功能
/// </summary>
[TestClass]
public class TypeExtensionsTest
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
    /// 简单测试类
    /// </summary>
    public class SimpleClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 带参数构造函数的类
    /// </summary>
    public class ClassWithConstructor(int id, string name)
    {
        public int Id { get; } = id;
        public string Name { get; } = name;
    }

    /// <summary>
    /// 带多个构造函数的类
    /// </summary>
    public class ClassWithMultipleConstructors
    {
        public int Id { get; }
        public string Name { get; }
        public decimal Price { get; }

        public ClassWithMultipleConstructors()
        {
            Id = 0;
            Name = string.Empty;
            Price = 0m;
        }

        public ClassWithMultipleConstructors(int id)
        {
            Id = id;
            Name = string.Empty;
            Price = 0m;
        }

        public ClassWithMultipleConstructors(int id, string name)
        {
            Id = id;
            Name = name;
            Price = 0m;
        }

        public ClassWithMultipleConstructors(int id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }

    /// <summary>
    /// 带依赖注入的类
    /// </summary>
    public class ClassWithDependency(TypeExtensionsTest.ITestService service)
    {
        public ITestService Service { get; } = service;
    }

    /// <summary>
    /// 测试服务接口
    /// </summary>
    public interface ITestService
    {
        string GetName();
    }

    /// <summary>
    /// 测试服务实现
    /// </summary>
    public class TestService : ITestService
    {
        public string GetName() => "TestService";
    }

    /// <summary>
    /// 基类
    /// </summary>
    public class BaseClass
    {
        public int BaseId { get; set; }
    }

    /// <summary>
    /// 派生类
    /// </summary>
    public class DerivedClass : BaseClass
    {
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 多层继承的类
    /// </summary>
    public class MultiLevelDerivedClass : DerivedClass
    {
        public decimal Price { get; set; }
    }

    /// <summary>
    /// 接口1
    /// </summary>
    public interface IInterface1
    {
        void Method1();
    }

    /// <summary>
    /// 接口2
    /// </summary>
    public interface IInterface2
    {
        void Method2();
    }

    /// <summary>
    /// 接口3继承接口1
    /// </summary>
    public interface IInterface3 : IInterface1
    {
        void Method3();
    }

    /// <summary>
    /// 实现多个接口的类
    /// </summary>
    public class ClassWithInterfaces : IInterface2, IInterface3
    {
        public void Method1() { }
        public void Method2() { }
        public void Method3() { }
    }

    /// <summary>
    /// 带特性的类
    /// </summary>
    [Obsolete("This is obsolete")]
    public class ClassWithAttribute
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// 结构体
    /// </summary>
    public struct TestStruct
    {
        public int Value { get; set; }
    }

    /// <summary>
    /// 带属性的测试类
    /// </summary>
    public class ClassWithProperties
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    #endregion

    #region GetEnumByDescription Tests

    /// <summary>
    /// 测试根据描述获取枚举值
    /// </summary>
    [TestMethod]
    public void GetEnumByDescription_WithValidDescription_ReturnsEnum_Test()
    {
        // Arrange
        Type enumType = typeof(TestEnum);

        // Act
        object result = enumType.GetEnumByDescription("值1描述");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TestEnum.Value1, result);
    }

    /// <summary>
    /// 测试根据描述获取另一个枚举值
    /// </summary>
    [TestMethod]
    public void GetEnumByDescription_WithAnotherValidDescription_ReturnsEnum_Test()
    {
        // Arrange
        Type enumType = typeof(TestEnum);

        // Act
        object result = enumType.GetEnumByDescription("值2描述");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TestEnum.Value2, result);
    }

    /// <summary>
    /// 测试根据枚举名称获取枚举值
    /// </summary>
    [TestMethod]
    public void GetEnumByDescription_WithEnumName_ReturnsEnum_Test()
    {
        // Arrange
        Type enumType = typeof(TestEnum);

        // Act
        object result = enumType.GetEnumByDescription("Value3");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TestEnum.Value3, result);
    }

    /// <summary>
    /// 测试非枚举类型抛出异常
    /// </summary>
    [TestMethod]
    public void GetEnumByDescription_WithNonEnumType_ThrowsException_Test()
    {
        // Arrange
        Type nonEnumType = typeof(string);

        // Act & Assert
        UtilException exception = Assert.ThrowsExactly<UtilException>(() => nonEnumType.GetEnumByDescription("test"));
        Assert.Contains("枚举类型", exception.Message);
    }

    /// <summary>
    /// 测试不存在的描述抛出异常
    /// </summary>
    [TestMethod]
    public void GetEnumByDescription_WithNonExistentDescription_ThrowsException_Test()
    {
        // Arrange
        Type enumType = typeof(TestEnum);

        // Act & Assert
        UtilException exception = Assert.ThrowsExactly<UtilException>(() => enumType.GetEnumByDescription("不存在的描述"));
        Assert.Contains("未找到", exception.Message);
    }

    #endregion

    #region InstantiationOrDefault Tests

    /// <summary>
    /// 测试无参构造函数实例化
    /// </summary>
    [TestMethod]
    public void InstantiationOrDefault_WithDefaultConstructor_ReturnsInstance_Test()
    {
        // Arrange
        Type type = typeof(SimpleClass);

        // Act
        object? result = type.InstantiationOrDefault();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<SimpleClass>(result);
    }

    /// <summary>
    /// 测试带参数构造函数实例化
    /// </summary>
    [TestMethod]
    public void InstantiationOrDefault_WithParameters_ReturnsInstance_Test()
    {
        // Arrange
        Type type = typeof(ClassWithConstructor);
        object[] args = [1, "Test"];

        // Act
        object? result = type.InstantiationOrDefault(args);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<ClassWithConstructor>(result);
        ClassWithConstructor instance = (ClassWithConstructor)result;
        Assert.AreEqual(1, instance.Id);
        Assert.AreEqual("Test", instance.Name);
    }

    /// <summary>
    /// 测试多个构造函数选择正确的构造函数
    /// </summary>
    [TestMethod]
    public void InstantiationOrDefault_WithMultipleConstructors_SelectsCorrectOne_Test()
    {
        // Arrange
        Type type = typeof(ClassWithMultipleConstructors);
        object[] args = [1, "Test"];

        // Act
        object? result = type.InstantiationOrDefault(args);

        // Assert
        Assert.IsNotNull(result);
        ClassWithMultipleConstructors instance = (ClassWithMultipleConstructors)result;
        Assert.AreEqual(1, instance.Id);
        Assert.AreEqual("Test", instance.Name);
    }

    /// <summary>
    /// 测试无法实例化的类型返回null
    /// </summary>
    [TestMethod]
    public void InstantiationOrDefault_WithInvalidType_ReturnsNull_Test()
    {
        // Arrange
        Type type = typeof(ITestService);

        // Act
        object? result = type.InstantiationOrDefault();

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region InstantiationOrDefault with ServiceProvider Tests

    /// <summary>
    /// 测试使用ServiceProvider实例化
    /// </summary>
    [TestMethod]
    public void InstantiationOrDefault_WithServiceProvider_ReturnsInstance_Test()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddSingleton<ITestService, TestService>();
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        Type type = typeof(ClassWithDependency);

        // Act
        object? result = type.InstantiationOrDefault(serviceProvider);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<ClassWithDependency>(result);
        ClassWithDependency instance = (ClassWithDependency)result;
        Assert.IsNotNull(instance.Service);
    }

    /// <summary>
    /// 测试使用ServiceProvider和参数实例化
    /// </summary>
    [TestMethod]
    public void InstantiationOrDefault_WithServiceProviderAndArgs_ReturnsInstance_Test()
    {
        // Arrange
        ServiceCollection services = new();
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        Type type = typeof(ClassWithConstructor);
        object[] args = [1, "Test"];

        // Act
        object? result = type.InstantiationOrDefault(serviceProvider, args);

        // Assert
        Assert.IsNotNull(result);
        ClassWithConstructor instance = (ClassWithConstructor)result;
        Assert.AreEqual(1, instance.Id);
        Assert.AreEqual("Test", instance.Name);
    }

    #endregion

    #region Instantiation Tests

    /// <summary>
    /// 测试实例化成功
    /// </summary>
    [TestMethod]
    public void Instantiation_WithValidType_ReturnsInstance_Test()
    {
        // Arrange
        Type type = typeof(SimpleClass);

        // Act
        object result = type.Instantiation();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<SimpleClass>(result);
    }

    /// <summary>
    /// 测试实例化失败抛出异常
    /// </summary>
    [TestMethod]
    public void Instantiation_WithInvalidType_ThrowsException_Test()
    {
        // Arrange
        Type type = typeof(ITestService);

        // Act & Assert
        UtilException exception = Assert.ThrowsExactly<UtilException>(() => type.Instantiation());
        Assert.Contains(exception.Message, "实例化失败");
    }

    #endregion

    #region Instantiation with ServiceProvider Tests

    /// <summary>
    /// 测试使用ServiceProvider实例化成功
    /// </summary>
    [TestMethod]
    public void Instantiation_WithServiceProvider_ReturnsInstance_Test()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddSingleton<ITestService, TestService>();
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        Type type = typeof(ClassWithDependency);

        // Act
        object result = type.Instantiation(serviceProvider);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<ClassWithDependency>(result);
    }

    #endregion

    #region InstantiationOrDefault<T> Tests

    /// <summary>
    /// 测试泛型实例化返回正确类型
    /// </summary>
    [TestMethod]
    public void InstantiationOrDefault_Generic_ReturnsTypedInstance_Test()
    {
        // Arrange
        Type type = typeof(SimpleClass);

        // Act
        SimpleClass? result = type.InstantiationOrDefault<SimpleClass>();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<SimpleClass>(result);
    }

    /// <summary>
    /// 测试泛型实例化类型不匹配返回默认值
    /// </summary>
    [TestMethod]
    public void InstantiationOrDefault_Generic_WithTypeMismatch_ReturnsDefault_Test()
    {
        // Arrange
        Type type = typeof(SimpleClass);

        // Act
        ClassWithConstructor? result = type.InstantiationOrDefault<ClassWithConstructor>();

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region Instantiation<T> Tests

    /// <summary>
    /// 测试泛型实例化成功
    /// </summary>
    [TestMethod]
    public void Instantiation_Generic_ReturnsTypedInstance_Test()
    {
        // Arrange
        Type type = typeof(SimpleClass);

        // Act
        SimpleClass result = type.Instantiation<SimpleClass>();

        // Assert
        Assert.IsNotNull(result);
    }

    /// <summary>
    /// 测试泛型实例化失败抛出异常
    /// </summary>
    [TestMethod]
    public void Instantiation_Generic_WithInvalidType_ThrowsException_Test()
    {
        // Arrange
        Type type = typeof(ITestService);

        // Act & Assert
        Assert.ThrowsExactly<UtilException>(() => type.Instantiation<ITestService>());
    }

    #endregion

    #region IsAssignableFrom Tests

    /// <summary>
    /// 测试派生类可分配给基类
    /// </summary>
    [TestMethod]
    public void IsAssignableFrom_WithDerivedClass_ReturnsTrue_Test()
    {
        // Arrange
        Type type = typeof(DerivedClass);

        // Act
        bool result = type.IsAssignableFrom<BaseClass>();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试基类不可分配给派生类
    /// </summary>
    [TestMethod]
    public void IsAssignableFrom_WithBaseClass_ReturnsFalse_Test()
    {
        // Arrange
        Type type = typeof(BaseClass);

        // Act
        bool result = type.IsAssignableFrom<DerivedClass>();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试相同类型可分配
    /// </summary>
    [TestMethod]
    public void IsAssignableFrom_WithSameType_ReturnsTrue_Test()
    {
        // Arrange
        Type type = typeof(SimpleClass);

        // Act
        bool result = type.IsAssignableFrom<SimpleClass>();

        // Assert
        Assert.IsTrue(result);
    }

    #endregion

    #region IsAssignableTo Tests

    /// <summary>
    /// 测试派生类可分配到基类
    /// </summary>
    [TestMethod]
    public void IsAssignableTo_WithDerivedClass_ReturnsTrue_Test()
    {
        // Arrange
        Type type = typeof(DerivedClass);

        // Act
        bool result = type.IsAssignableTo<BaseClass>();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试基类不可分配到派生类
    /// </summary>
    [TestMethod]
    public void IsAssignableTo_WithBaseClass_ReturnsFalse_Test()
    {
        // Arrange
        Type type = typeof(BaseClass);

        // Act
        bool result = type.IsAssignableTo<DerivedClass>();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试实现类可分配到接口
    /// </summary>
    [TestMethod]
    public void IsAssignableTo_WithInterface_ReturnsTrue_Test()
    {
        // Arrange
        Type type = typeof(TestService);

        // Act
        bool result = type.IsAssignableTo<ITestService>();

        // Assert
        Assert.IsTrue(result);
    }

    #endregion

    #region GetAllBaseType Tests

    /// <summary>
    /// 测试获取所有基类
    /// </summary>
    [TestMethod]
    public void GetAllBaseType_WithDerivedClass_ReturnsAllBaseTypes_Test()
    {
        // Arrange
        Type type = typeof(DerivedClass);

        // Act
        List<Type> result = type.GetAllBaseType();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThanOrEqualTo(2, result.Count);
        CollectionAssert.Contains(result, typeof(DerivedClass));
        CollectionAssert.Contains(result, typeof(BaseClass));
        CollectionAssert.Contains(result, typeof(object));
    }

    /// <summary>
    /// 测试多层继承获取所有基类
    /// </summary>
    [TestMethod]
    public void GetAllBaseType_WithMultiLevelDerived_ReturnsAllBaseTypes_Test()
    {
        // Arrange
        Type type = typeof(MultiLevelDerivedClass);

        // Act
        List<Type> result = type.GetAllBaseType();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThanOrEqualTo(3, result.Count);
        CollectionAssert.Contains(result, typeof(MultiLevelDerivedClass));
        CollectionAssert.Contains(result, typeof(DerivedClass));
        CollectionAssert.Contains(result, typeof(BaseClass));
        CollectionAssert.Contains(result, typeof(object));
    }

    /// <summary>
    /// 测试基类只返回自身和object
    /// </summary>
    [TestMethod]
    public void GetAllBaseType_WithBaseClass_ReturnsOnlyObjectAndSelf_Test()
    {
        // Arrange
        Type type = typeof(SimpleClass);

        // Act
        List<Type> result = type.GetAllBaseType();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);
        CollectionAssert.Contains(result, typeof(SimpleClass));
        CollectionAssert.Contains(result, typeof(object));
    }

    #endregion

    #region GetAllInterfaces Tests

    /// <summary>
    /// 测试获取所有接口
    /// </summary>
    [TestMethod]
    public void GetAllInterfaces_WithMultipleInterfaces_ReturnsAllInterfaces_Test()
    {
        // Arrange
        Type type = typeof(ClassWithInterfaces);

        // Act
        List<Type> result = type.GetAllInterfaces();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsGreaterThanOrEqualTo(3, result.Count);
        CollectionAssert.Contains(result, typeof(IInterface1));
        CollectionAssert.Contains(result, typeof(IInterface2));
        CollectionAssert.Contains(result, typeof(IInterface3));
    }

    /// <summary>
    /// 测试无接口的类返回空列表
    /// </summary>
    [TestMethod]
    public void GetAllInterfaces_WithNoInterfaces_ReturnsEmptyList_Test()
    {
        // Arrange
        Type type = typeof(SimpleClass);

        // Act
        List<Type> result = type.GetAllInterfaces();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 测试单个接口
    /// </summary>
    [TestMethod]
    public void GetAllInterfaces_WithSingleInterface_ReturnsInterface_Test()
    {
        // Arrange
        Type type = typeof(TestService);

        // Act
        List<Type> result = type.GetAllInterfaces();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(1, result);
        CollectionAssert.Contains(result, typeof(ITestService));
    }

    #endregion

    #region ToDataTable Tests

    /// <summary>
    /// 测试类型转换为DataTable
    /// </summary>
    [TestMethod]
    public void ToDataTable_WithClass_ReturnsDataTable_Test()
    {
        // Arrange
        Type type = typeof(ClassWithProperties);

        // Act
        DataTable result = type.ToDataTable();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("ClassWithProperties", result.TableName);
        Assert.HasCount(5, result.Columns);
#pragma warning disable MSTEST0037 // 使用正确的“断言”方法
        Assert.IsTrue(result.Columns.Contains("Id"));
        Assert.IsTrue(result.Columns.Contains("Name"));
        Assert.IsTrue(result.Columns.Contains("Price"));
        Assert.IsTrue(result.Columns.Contains("CreatedDate"));
        Assert.IsTrue(result.Columns.Contains("IsActive"));
#pragma warning restore MSTEST0037 // 使用正确的“断言”方法
    }

    /// <summary>
    /// 测试列类型正确
    /// </summary>
    [TestMethod]
    public void ToDataTable_ColumnTypes_AreCorrect_Test()
    {
        // Arrange
        Type type = typeof(ClassWithProperties);

        // Act
        DataTable result = type.ToDataTable();

        // Assert
        Assert.AreEqual(typeof(int), result.Columns["Id"]!.DataType);
        Assert.AreEqual(typeof(string), result.Columns["Name"]!.DataType);
        Assert.AreEqual(typeof(decimal), result.Columns["Price"]!.DataType);
        Assert.AreEqual(typeof(DateTime), result.Columns["CreatedDate"]!.DataType);
        Assert.AreEqual(typeof(bool), result.Columns["IsActive"]!.DataType);
    }

    /// <summary>
    /// 测试无属性的类返回空DataTable
    /// </summary>
    [TestMethod]
    public void ToDataTable_WithNoProperties_ReturnsEmptyDataTable_Test()
    {
        // Arrange
        Type type = typeof(object);

        // Act
        DataTable result = type.ToDataTable();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Columns);
    }

    #endregion

    #region GetEnumCount Tests

    /// <summary>
    /// 测试获取枚举总数
    /// </summary>
    [TestMethod]
    public void GetEnumCount_WithEnum_ReturnsCount_Test()
    {
        // Arrange
        Type type = typeof(TestEnum);

        // Act
        int result = type.GetEnumCount();

        // Assert
        Assert.AreEqual(3, result);
    }

    /// <summary>
    /// 测试非枚举类型抛出异常
    /// </summary>
    [TestMethod]
    public void GetEnumCount_WithNonEnum_ThrowsException_Test()
    {
        // Arrange
        Type type = typeof(string);

        // Act & Assert
        UtilException exception = Assert.ThrowsExactly<UtilException>(() => type.GetEnumCount());
        Assert.Contains("枚举类型", exception.Message);
    }

    #endregion

    #region HasCustomAttribute Tests

    /// <summary>
    /// 测试有特性返回true
    /// </summary>
    [TestMethod]
    public void HasCustomAttribute_WithAttribute_ReturnsTrue_Test()
    {
        // Arrange
#pragma warning disable CS0618 // 类型或成员已过时
        Type type = typeof(ClassWithAttribute);
#pragma warning restore CS0618 // 类型或成员已过时

        // Act
        bool result = type.HasCustomAttribute<ObsoleteAttribute>();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试无特性返回false
    /// </summary>
    [TestMethod]
    public void HasCustomAttribute_WithoutAttribute_ReturnsFalse_Test()
    {
        // Arrange
        Type type = typeof(SimpleClass);

        // Act
        bool result = type.HasCustomAttribute<ObsoleteAttribute>();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsNullableType Tests

    /// <summary>
    /// 测试可空值类型返回true
    /// </summary>
    [TestMethod]
    public void IsNullableType_WithNullableValueType_ReturnsTrue_Test()
    {
        // Arrange
        Type type = typeof(int?);

        // Act
        bool result = type.IsNullableType();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试非可空值类型返回false
    /// </summary>
    [TestMethod]
    public void IsNullableType_WithNonNullableValueType_ReturnsFalse_Test()
    {
        // Arrange
        Type type = typeof(int);

        // Act
        bool result = type.IsNullableType();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试引用类型返回false
    /// </summary>
    [TestMethod]
    public void IsNullableType_WithReferenceType_ReturnsFalse_Test()
    {
        // Arrange
        Type type = typeof(string);

        // Act
        bool result = type.IsNullableType();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试可空DateTime返回true
    /// </summary>
    [TestMethod]
    public void IsNullableType_WithNullableDateTime_ReturnsTrue_Test()
    {
        // Arrange
        Type type = typeof(DateTime?);

        // Act
        bool result = type.IsNullableType();

        // Assert
        Assert.IsTrue(result);
    }

    #endregion

    #region IsNullableType with GenericType Tests

    /// <summary>
    /// 测试可空类型匹配泛型参数
    /// </summary>
    [TestMethod]
    public void IsNullableType_WithMatchingGenericType_ReturnsTrue_Test()
    {
        // Arrange
        Type type = typeof(int?);
        Type genericType = typeof(int);

        // Act
        bool result = type.IsNullableType(genericType);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试可空类型不匹配泛型参数
    /// </summary>
    [TestMethod]
    public void IsNullableType_WithNonMatchingGenericType_ReturnsFalse_Test()
    {
        // Arrange
        Type type = typeof(int?);
        Type genericType = typeof(string);

        // Act
        bool result = type.IsNullableType(genericType);

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试非可空类型返回false
    /// </summary>
    [TestMethod]
    public void IsNullableType_WithNonNullableType_ReturnsFalse_Test()
    {
        // Arrange
        Type type = typeof(int);
        Type genericType = typeof(int);

        // Act
        bool result = type.IsNullableType(genericType);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsNullableType<T> Tests

    /// <summary>
    /// 测试泛型版本可空类型匹配
    /// </summary>
    [TestMethod]
    public void IsNullableType_Generic_WithMatchingType_ReturnsTrue_Test()
    {
        // Arrange
        Type type = typeof(int?);

        // Act
        bool result = type.IsNullableType<int>();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试泛型版本可空类型不匹配
    /// </summary>
    [TestMethod]
    public void IsNullableType_Generic_WithNonMatchingType_ReturnsFalse_Test()
    {
        // Arrange
        Type type = typeof(int?);

        // Act
        bool result = type.IsNullableType<string>();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region IsStruct Tests

    /// <summary>
    /// 测试结构体返回true
    /// </summary>
    [TestMethod]
    public void IsStruct_WithStruct_ReturnsTrue_Test()
    {
        // Arrange
        Type type = typeof(TestStruct);

        // Act
        bool result = type.IsStruct();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试int是结构体
    /// </summary>
    [TestMethod]
    public void IsStruct_WithInt_ReturnsTrue_Test()
    {
        // Arrange
        Type type = typeof(int);

        // Act
        bool result = type.IsStruct();

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 测试枚举不是结构体
    /// </summary>
    [TestMethod]
    public void IsStruct_WithEnum_ReturnsFalse_Test()
    {
        // Arrange
        Type type = typeof(TestEnum);

        // Act
        bool result = type.IsStruct();

        // Assert
        Assert.IsFalse(result);
    }

    /// <summary>
    /// 测试类不是结构体
    /// </summary>
    [TestMethod]
    public void IsStruct_WithClass_ReturnsFalse_Test()
    {
        // Arrange
        Type type = typeof(SimpleClass);

        // Act
        bool result = type.IsStruct();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion
}
