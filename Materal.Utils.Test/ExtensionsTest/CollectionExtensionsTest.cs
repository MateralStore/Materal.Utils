using System.Collections.ObjectModel;

namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// CollectionExtensions 测试类
/// 测试集合扩展方法的功能
/// </summary>
[TestClass]
public class CollectionExtensionsTest
{
    #region ToObservableCollection Tests

    /// <summary>
    /// 测试将列表转换为ObservableCollection
    /// </summary>
    [TestMethod]
    public void ToObservableCollection_WithValidList_ReturnsObservableCollection_Test()
    {
        // Arrange
        List<int> list = [1, 2, 3, 4, 5];

        // Act
        ObservableCollection<int> result = list.ToObservableCollection();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(list.Count, result);
        CollectionAssert.AreEqual(list, result.ToList());
    }

    /// <summary>
    /// 测试将空列表转换为ObservableCollection
    /// </summary>
    [TestMethod]
    public void ToObservableCollection_WithEmptyList_ReturnsEmptyObservableCollection_Test()
    {
        // Arrange
        List<string> list = [];

        // Act
        ObservableCollection<string> result = list.ToObservableCollection();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 测试将数组转换为ObservableCollection
    /// </summary>
    [TestMethod]
    public void ToObservableCollection_WithArray_ReturnsObservableCollection_Test()
    {
        // Arrange
        string[] array = ["a", "b", "c"];

        // Act
        ObservableCollection<string> result = array.ToObservableCollection();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(array.Length, result);
        CollectionAssert.AreEqual(array, result.ToArray());
    }

    /// <summary>
    /// 测试将IEnumerable转换为ObservableCollection
    /// </summary>
    [TestMethod]
    public void ToObservableCollection_WithIEnumerable_ReturnsObservableCollection_Test()
    {
        // Arrange
        IEnumerable<int> enumerable = Enumerable.Range(1, 10);

        // Act
        ObservableCollection<int> result = enumerable.ToObservableCollection();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(10, result);
        CollectionAssert.AreEqual(enumerable.ToList(), result.ToList());
    }

    /// <summary>
    /// 测试null列表抛出异常
    /// </summary>
    [TestMethod]
    public void ToObservableCollection_WithNullList_ThrowsArgumentNullException_Test()
    {
        // Arrange
        List<int> list = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => list.ToObservableCollection());
    }

    /// <summary>
    /// 测试转换后的集合是独立的副本
    /// </summary>
    [TestMethod]
    public void ToObservableCollection_CreatesIndependentCopy_Test()
    {
        // Arrange
        List<int> list = [1, 2, 3];

        // Act
        ObservableCollection<int> result = list.ToObservableCollection();
        list.Add(4);

        // Assert
        Assert.HasCount(4, list);
        Assert.HasCount(3, result);
    }

    #endregion

    #region GetAddArrayAndRemoveArray Tests

    /// <summary>
    /// 测试获取新增和删除的元素
    /// </summary>
    [TestMethod]
    public void GetAddArrayAndRemoveArray_WithDifferentCollections_ReturnsCorrectArrays_Test()
    {
        // Arrange
        ICollection<int> sourceArray = [1, 2, 3, 4, 5];
        ICollection<int> oldArray = [3, 4, 5, 6, 7];

        // Act
        var (addArray, removeArray) = sourceArray.GetAddArrayAndRemoveArray(oldArray);

        // Assert
        Assert.IsNotNull(addArray);
        Assert.IsNotNull(removeArray);
        Assert.HasCount(2, addArray);
        Assert.HasCount(2, removeArray);
        CollectionAssert.AreEquivalent(new List<int> { 1, 2 }, addArray.ToList());
        CollectionAssert.AreEquivalent(new List<int> { 6, 7 }, removeArray.ToList());
    }

    /// <summary>
    /// 测试相同集合返回空的新增和删除数组
    /// </summary>
    [TestMethod]
    public void GetAddArrayAndRemoveArray_WithSameCollections_ReturnsEmptyArrays_Test()
    {
        // Arrange
        ICollection<string> sourceArray = ["a", "b", "c"];
        ICollection<string> oldArray = ["a", "b", "c"];

        // Act
        var (addArray, removeArray) = sourceArray.GetAddArrayAndRemoveArray(oldArray);

        // Assert
        Assert.IsNotNull(addArray);
        Assert.IsNotNull(removeArray);
        Assert.IsEmpty(addArray);
        Assert.IsEmpty(removeArray);
    }

    /// <summary>
    /// 测试新集合为空时返回所有旧元素为删除数组
    /// </summary>
    [TestMethod]
    public void GetAddArrayAndRemoveArray_WithEmptySourceArray_ReturnsAllOldAsRemove_Test()
    {
        // Arrange
        ICollection<int> sourceArray = [];
        ICollection<int> oldArray = [1, 2, 3];

        // Act
        var (addArray, removeArray) = sourceArray.GetAddArrayAndRemoveArray(oldArray);

        // Assert
        Assert.IsNotNull(addArray);
        Assert.IsNotNull(removeArray);
        Assert.IsEmpty(addArray);
        Assert.HasCount(3, removeArray);
        CollectionAssert.AreEquivalent(new List<int> { 1, 2, 3 }, removeArray.ToList());
    }

    /// <summary>
    /// 测试旧集合为空时返回所有新元素为新增数组
    /// </summary>
    [TestMethod]
    public void GetAddArrayAndRemoveArray_WithEmptyOldArray_ReturnsAllSourceAsAdd_Test()
    {
        // Arrange
        ICollection<int> sourceArray = [1, 2, 3];
        ICollection<int> oldArray = [];

        // Act
        var (addArray, removeArray) = sourceArray.GetAddArrayAndRemoveArray(oldArray);

        // Assert
        Assert.IsNotNull(addArray);
        Assert.IsNotNull(removeArray);
        Assert.HasCount(3, addArray);
        Assert.IsEmpty(removeArray);
        CollectionAssert.AreEquivalent(new List<int> { 1, 2, 3 }, addArray.ToList());
    }

    /// <summary>
    /// 测试两个空集合返回空的新增和删除数组
    /// </summary>
    [TestMethod]
    public void GetAddArrayAndRemoveArray_WithBothEmptyCollections_ReturnsEmptyArrays_Test()
    {
        // Arrange
        ICollection<int> sourceArray = [];
        ICollection<int> oldArray = [];

        // Act
        var (addArray, removeArray) = sourceArray.GetAddArrayAndRemoveArray(oldArray);

        // Assert
        Assert.IsNotNull(addArray);
        Assert.IsNotNull(removeArray);
        Assert.IsEmpty(addArray);
        Assert.IsEmpty(removeArray);
    }

    /// <summary>
    /// 测试null源集合抛出异常
    /// </summary>
    [TestMethod]
    public void GetAddArrayAndRemoveArray_WithNullSourceArray_ThrowsArgumentNullException_Test()
    {
        // Arrange
        ICollection<int> sourceArray = null!;
        ICollection<int> oldArray = [1, 2, 3];

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => sourceArray.GetAddArrayAndRemoveArray(oldArray));
    }

    /// <summary>
    /// 测试null旧集合抛出异常
    /// </summary>
    [TestMethod]
    public void GetAddArrayAndRemoveArray_WithNullOldArray_ThrowsArgumentNullException_Test()
    {
        // Arrange
        ICollection<int> sourceArray = [1, 2, 3];
        ICollection<int> oldArray = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => sourceArray.GetAddArrayAndRemoveArray(oldArray));
    }

    /// <summary>
    /// 测试处理重复元素的集合
    /// </summary>
    [TestMethod]
    public void GetAddArrayAndRemoveArray_WithDuplicateElements_HandlesCorrectly_Test()
    {
        // Arrange
        ICollection<int> sourceArray = [1, 1, 2, 2, 3];
        ICollection<int> oldArray = [2, 2, 3, 3, 4];

        // Act
        var (addArray, removeArray) = sourceArray.GetAddArrayAndRemoveArray(oldArray);

        // Assert
        Assert.IsNotNull(addArray);
        Assert.IsNotNull(removeArray);
        Assert.Contains(1, addArray);
        Assert.Contains(4, removeArray);
    }

    #endregion

    #region Distinct Tests

    /// <summary>
    /// 测试使用自定义比较器去重
    /// </summary>
    [TestMethod]
    public void Distinct_WithCustomComparer_RemovesDuplicates_Test()
    {
        // Arrange
        List<Person> people =
        [
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 2, Name = "Bob" },
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 3, Name = "Charlie" }
        ];
        static bool comparer(Person x, Person y) => x.Id == y.Id;

        // Act
        IEnumerable<Person> result = people.Distinct(comparer);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count());
    }

    /// <summary>
    /// 测试空集合去重返回空集合
    /// </summary>
    [TestMethod]
    public void Distinct_WithEmptyCollection_ReturnsEmptyCollection_Test()
    {
        // Arrange
        List<Person> people = [];
        static bool comparer(Person x, Person y) => x.Id == y.Id;

        // Act
        IEnumerable<Person> result = people.Distinct(comparer);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    /// <summary>
    /// 测试没有重复元素的集合去重
    /// </summary>
    [TestMethod]
    public void Distinct_WithNoDuplicates_ReturnsAllElements_Test()
    {
        // Arrange
        List<Person> people =
        [
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 2, Name = "Bob" },
            new Person { Id = 3, Name = "Charlie" }
        ];
        static bool comparer(Person x, Person y) => x.Id == y.Id;

        // Act
        IEnumerable<Person> result = people.Distinct(comparer);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count());
    }

    /// <summary>
    /// 测试所有元素都重复的集合去重
    /// </summary>
    [TestMethod]
    public void Distinct_WithAllDuplicates_ReturnsOneElement_Test()
    {
        // Arrange
        List<Person> people =
        [
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 1, Name = "Alice" }
        ];
        static bool comparer(Person x, Person y) => x.Id == y.Id;

        // Act
        IEnumerable<Person> result = people.Distinct(comparer);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
    }

    /// <summary>
    /// 测试null源集合抛出异常
    /// </summary>
    [TestMethod]
    public void Distinct_WithNullSource_ThrowsArgumentNullException_Test()
    {
        // Arrange
        IEnumerable<Person> people = null!;
        static bool comparer(Person x, Person y) => x.Id == y.Id;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => people.Distinct(comparer).ToList());
    }

    /// <summary>
    /// 测试null比较器抛出异常
    /// </summary>
    [TestMethod]
    public void Distinct_WithNullComparer_ThrowsArgumentNullException_Test()
    {
        // Arrange
        List<Person> people = [new Person { Id = 1, Name = "Alice" }];
        Func<Person, Person, bool> comparer = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => people.Distinct(comparer).ToList());
    }

    /// <summary>
    /// 测试使用不同属性的比较器
    /// </summary>
    [TestMethod]
    public void Distinct_WithNameComparer_RemovesDuplicatesByName_Test()
    {
        // Arrange
        List<Person> people =
        [
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 2, Name = "Bob" },
            new Person { Id = 3, Name = "Alice" }
        ];
        static bool comparer(Person x, Person y) => x.Name == y.Name;

        // Act
        IEnumerable<Person> result = people.Distinct(comparer);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    /// <summary>
    /// 测试比较器处理复杂逻辑
    /// </summary>
    [TestMethod]
    public void Distinct_WithComplexComparer_WorksCorrectly_Test()
    {
        // Arrange
        List<Person> people =
        [
            new Person { Id = 1, Name = "Alice" },
            new Person { Id = 2, Name = "Bob" },
            new Person { Id = 3, Name = "alice" },
            new Person { Id = 4, Name = "BOB" }
        ];
        static bool comparer(Person x, Person y) =>
            string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);

        // Act
        IEnumerable<Person> result = people.Distinct(comparer);

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

        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object? obj)
        {
            if (obj is Person person)
            {
                return Id == person.Id;
            }
            return false;
        }
    }

    #endregion
}
