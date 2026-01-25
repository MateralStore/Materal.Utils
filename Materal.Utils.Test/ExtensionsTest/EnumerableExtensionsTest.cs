namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// EnumerableExtensions 测试类
/// 测试Enumerable扩展方法的功能
/// </summary>
[TestClass]
public class EnumerableExtensionsTest
{
    #region DistinctByHashSet Tests

    /// <summary>
    /// 测试使用默认比较器去重
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_WithDefaultComparer_RemovesDuplicates_Test()
    {
        // Arrange
        IEnumerable<int> sources = [1, 2, 3, 2, 4, 1, 5];

        // Act
        IEnumerable<int> result = sources.DistinctByHashSet();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Count());
        CollectionAssert.AreEquivalent(new List<int> { 1, 2, 3, 4, 5 }, result.ToList());
    }

    /// <summary>
    /// 测试使用自定义比较器去重
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_WithCustomComparer_RemovesDuplicates_Test()
    {
        // Arrange
        IEnumerable<string> sources = ["apple", "APPLE", "banana", "BANANA"];
        IEqualityComparer<string> comparer = StringComparer.OrdinalIgnoreCase;

        // Act
        IEnumerable<string> result = sources.DistinctByHashSet(comparer);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    /// <summary>
    /// 测试空集合去重
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_WithEmptyCollection_ReturnsEmpty_Test()
    {
        // Arrange
        IEnumerable<int> sources = [];

        // Act
        IEnumerable<int> result = sources.DistinctByHashSet();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    /// <summary>
    /// 测试null集合抛出异常
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_WithNullSource_ThrowsArgumentNullException_Test()
    {
        // Arrange
        IEnumerable<int> sources = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => sources.DistinctByHashSet().ToList());
    }

    /// <summary>
    /// 测试没有重复元素的集合
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_WithNoDuplicates_ReturnsAllElements_Test()
    {
        // Arrange
        IEnumerable<int> sources = [1, 2, 3, 4, 5];

        // Act
        IEnumerable<int> result = sources.DistinctByHashSet();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Count());
        CollectionAssert.AreEqual(sources.ToList(), result.ToList());
    }

    #endregion

    #region DistinctByHashSet with KeySelector Tests

    /// <summary>
    /// 测试使用键选择器去重
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_WithKeySelector_RemovesDuplicates_Test()
    {
        // Arrange
        List<Person> sources =
        [
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 2, Name = "Bob" },
            new Person { Id = 1, Name = "Alice2" },
            new Person { Id = 3, Name = "Charlie" }
        ];

        // Act
        IEnumerable<Person> result = sources.DistinctByHashSet(p => p.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count());
    }

    /// <summary>
    /// 测试使用键选择器和自定义比较器去重
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_WithKeySelectorAndComparer_RemovesDuplicates_Test()
    {
        // Arrange
        List<Person> sources =
        [
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 2, Name = "alice" },
            new Person { Id = 3, Name = "Bob" }
        ];
        IEqualityComparer<string> comparer = StringComparer.OrdinalIgnoreCase;

        // Act
        IEnumerable<Person> result = sources.DistinctByHashSet(p => p.Name, comparer);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    /// <summary>
    /// 测试null键选择器抛出异常
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_WithNullKeySelector_ThrowsArgumentNullException_Test()
    {
        // Arrange
        List<Person> sources = [new Person { Id = 1, Name = "Alice" }];
        Func<Person, int> keySelector = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => sources.DistinctByHashSet(keySelector).ToList());
    }

    /// <summary>
    /// 测试使用复杂键选择器
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_WithComplexKeySelector_RemovesDuplicates_Test()
    {
        // Arrange
        List<Person> sources =
        [
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 2, Name = "Bob" },
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 2, Name = "Bob" }
        ];

        // Act
        IEnumerable<Person> result = sources.DistinctByHashSet(p => $"{p.Id}_{p.Name}");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    /// <summary>
    /// 测试延迟执行特性
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_IsLazyEvaluated_Test()
    {
        // Arrange
        List<int> sources = [1, 2, 3, 2, 1];
        int evaluationCount = 0;
        IEnumerable<int> query = sources.Select(x =>
        {
            evaluationCount++;
            return x;
        }).DistinctByHashSet();

        // Act - 未枚举时不应执行
        Assert.AreEqual(0, evaluationCount);

        // Act - 枚举时才执行
        _ = query.ToList();

        // Assert
        Assert.AreEqual(5, evaluationCount);
    }

    /// <summary>
    /// 测试使用键选择器和默认比较器去重
    /// </summary>
    [TestMethod]
    public void DistinctByHashSet_WithKeySelectorAndDefaultComparer_RemovesDuplicates_Test()
    {
        // Arrange
        List<Person> sources =
        [
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 2, Name = "Bob" },
            new Person { Id = 1, Name = "Alice2" }
        ];

        // Act
        IEnumerable<Person> result = sources.DistinctByHashSet(p => p.Id, null);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// 测试用的Person类
    /// </summary>
    private class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    #endregion
}
