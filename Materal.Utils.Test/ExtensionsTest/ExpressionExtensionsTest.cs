using System.Linq.Expressions;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// ExpressionExtensions 测试类
/// 测试表达式扩展方法的功能
/// </summary>
[TestClass]
public class ExpressionExtensionsTest
{
    #region And Tests

    /// <summary>
    /// 测试使用And组合两个表达式
    /// </summary>
    [TestMethod]
    public void And_WithTwoExpressions_ReturnsCombinedExpression_Test()
    {
        // Arrange
        Expression<Func<int, bool>> first = x => x > 5;
        Expression<Func<int, bool>> second = x => x < 10;

        // Act
        Expression<Func<int, bool>> result = first.And(second);
        Func<int, bool> compiled = result.Compile();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(compiled(7));
        Assert.IsFalse(compiled(3));
        Assert.IsFalse(compiled(12));
    }

    /// <summary>
    /// 测试And组合后的表达式边界条件
    /// </summary>
    [TestMethod]
    public void And_WithBoundaryConditions_WorksCorrectly_Test()
    {
        // Arrange
        Expression<Func<int, bool>> first = x => x >= 5;
        Expression<Func<int, bool>> second = x => x <= 10;

        // Act
        Expression<Func<int, bool>> result = first.And(second);
        Func<int, bool> compiled = result.Compile();

        // Assert
        Assert.IsTrue(compiled(5));
        Assert.IsTrue(compiled(10));
        Assert.IsFalse(compiled(4));
        Assert.IsFalse(compiled(11));
    }

    /// <summary>
    /// 测试And组合字符串表达式
    /// </summary>
    [TestMethod]
    public void And_WithStringExpressions_WorksCorrectly_Test()
    {
        // Arrange
        Expression<Func<string, bool>> first = s => s.Length > 3;
        Expression<Func<string, bool>> second = s => s.StartsWith('T');

        // Act
        Expression<Func<string, bool>> result = first.And(second);
        Func<string, bool> compiled = result.Compile();

        // Assert
        Assert.IsTrue(compiled("Test"));
        Assert.IsFalse(compiled("T"));
        Assert.IsFalse(compiled("Hello"));
    }

    /// <summary>
    /// 测试And组合复杂对象表达式
    /// </summary>
    [TestMethod]
    public void And_WithComplexObjectExpressions_WorksCorrectly_Test()
    {
        // Arrange
        Expression<Func<Person, bool>> first = p => p.Age > 18;
        Expression<Func<Person, bool>> second = p => p.Name.StartsWith('A');

        // Act
        Expression<Func<Person, bool>> result = first.And(second);
        Func<Person, bool> compiled = result.Compile();

        // Assert
        Assert.IsTrue(compiled(new Person { Age = 25, Name = "Alice" }));
        Assert.IsFalse(compiled(new Person { Age = 15, Name = "Alice" }));
        Assert.IsFalse(compiled(new Person { Age = 25, Name = "Bob" }));
    }

    #endregion

    #region Or Tests

    /// <summary>
    /// 测试使用Or组合两个表达式
    /// </summary>
    [TestMethod]
    public void Or_WithTwoExpressions_ReturnsCombinedExpression_Test()
    {
        // Arrange
        Expression<Func<int, bool>> first = x => x < 5;
        Expression<Func<int, bool>> second = x => x > 10;

        // Act
        Expression<Func<int, bool>> result = first.Or(second);
        Func<int, bool> compiled = result.Compile();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(compiled(3));
        Assert.IsTrue(compiled(12));
        Assert.IsFalse(compiled(7));
    }

    /// <summary>
    /// 测试Or组合后的表达式边界条件
    /// </summary>
    [TestMethod]
    public void Or_WithBoundaryConditions_WorksCorrectly_Test()
    {
        // Arrange
        Expression<Func<int, bool>> first = x => x <= 5;
        Expression<Func<int, bool>> second = x => x >= 10;

        // Act
        Expression<Func<int, bool>> result = first.Or(second);
        Func<int, bool> compiled = result.Compile();

        // Assert
        Assert.IsTrue(compiled(5));
        Assert.IsTrue(compiled(10));
        Assert.IsFalse(compiled(7));
    }

    /// <summary>
    /// 测试Or组合字符串表达式
    /// </summary>
    [TestMethod]
    public void Or_WithStringExpressions_WorksCorrectly_Test()
    {
        // Arrange
        Expression<Func<string, bool>> first = s => s.Length < 3;
        Expression<Func<string, bool>> second = s => s.EndsWith('t');

        // Act
        Expression<Func<string, bool>> result = first.Or(second);
        Func<string, bool> compiled = result.Compile();

        // Assert
        Assert.IsTrue(compiled("Hi"));
        Assert.IsTrue(compiled("Test"));
        Assert.IsFalse(compiled("Hello"));
    }

    /// <summary>
    /// 测试Or组合复杂对象表达式
    /// </summary>
    [TestMethod]
    public void Or_WithComplexObjectExpressions_WorksCorrectly_Test()
    {
        // Arrange
        Expression<Func<Person, bool>> first = p => p.Age < 18;
        Expression<Func<Person, bool>> second = p => p.Name == "Admin";

        // Act
        Expression<Func<Person, bool>> result = first.Or(second);
        Func<Person, bool> compiled = result.Compile();

        // Assert
        Assert.IsTrue(compiled(new Person { Age = 15, Name = "Alice" }));
        Assert.IsTrue(compiled(new Person { Age = 25, Name = "Admin" }));
        Assert.IsFalse(compiled(new Person { Age = 25, Name = "Bob" }));
    }

    #endregion

    #region Compose Tests

    /// <summary>
    /// 测试使用自定义合并函数组合表达式
    /// </summary>
    [TestMethod]
    public void Compose_WithCustomMergeFunction_WorksCorrectly_Test()
    {
        // Arrange
        Expression<Func<int, bool>> first = x => x > 5;
        Expression<Func<int, bool>> second = x => x < 10;

        // Act
        Expression<Func<int, bool>> result = first.Compose(second, Expression.AndAlso);
        Func<int, bool> compiled = result.Compile();

        // Assert
        Assert.IsTrue(compiled(7));
        Assert.IsFalse(compiled(3));
    }

    /// <summary>
    /// 测试链式组合多个表达式
    /// </summary>
    [TestMethod]
    public void Compose_ChainedExpressions_WorksCorrectly_Test()
    {
        // Arrange
        Expression<Func<int, bool>> first = x => x > 0;
        Expression<Func<int, bool>> second = x => x < 100;
        Expression<Func<int, bool>> third = x => x % 2 == 0;

        // Act
        Expression<Func<int, bool>> result = first.And(second).And(third);
        Func<int, bool> compiled = result.Compile();

        // Assert
        Assert.IsTrue(compiled(50));
        Assert.IsFalse(compiled(51));
        Assert.IsFalse(compiled(-2));
        Assert.IsFalse(compiled(102));
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// 测试用的Person类
    /// </summary>
    private class Person
    {
        public int Age { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    #endregion
}
