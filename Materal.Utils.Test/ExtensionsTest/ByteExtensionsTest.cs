namespace Materal.Utils.Test.ExtensionsTest;

/// <summary>
/// ByteExtensions 测试类
/// 测试字节数组扩展方法的功能
/// </summary>
[TestClass]
public class ByteExtensionsTest
{
    #region GetBinaryString Tests

    /// <summary>
    /// 测试将字节数组转换为二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryString_WithSingleByte_ReturnsCorrectBinaryString_Test()
    {
        // Arrange
        byte[] buffer = [255];

        // Act
        string result = buffer.GetBinaryString();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("11111111", result);
    }

    /// <summary>
    /// 测试将多个字节转换为二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryString_WithMultipleBytes_ReturnsCorrectBinaryString_Test()
    {
        // Arrange
        byte[] buffer = [1, 2, 3];

        // Act
        string result = buffer.GetBinaryString();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("000000010000001000000011", result);
    }

    /// <summary>
    /// 测试零字节转换为二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryString_WithZeroByte_ReturnsZeroBinaryString_Test()
    {
        // Arrange
        byte[] buffer = [0];

        // Act
        string result = buffer.GetBinaryString();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("00000000", result);
    }

    /// <summary>
    /// 测试空字节数组转换为二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryString_WithEmptyArray_ReturnsEmptyString_Test()
    {
        // Arrange
        byte[] buffer = [];

        // Act
        string result = buffer.GetBinaryString();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// 测试null字节数组抛出异常
    /// </summary>
    [TestMethod]
    public void GetBinaryString_WithNullBuffer_ThrowsArgumentNullException_Test()
    {
        // Arrange
        byte[] buffer = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => buffer.GetBinaryString());
    }

    /// <summary>
    /// 测试二进制字符串长度是字节数的8倍
    /// </summary>
    [TestMethod]
    public void GetBinaryString_ResultLengthIsEightTimesBufferLength_Test()
    {
        // Arrange
        byte[] buffer = [10, 20, 30, 40, 50];

        // Act
        string result = buffer.GetBinaryString();

        // Assert
        Assert.AreEqual(buffer.Length * 8, result.Length);
    }

    #endregion

    #region GetBinaryStringByBitIndex Tests

    /// <summary>
    /// 测试获取指定位范围的二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryStringByBitIndex_WithValidRange_ReturnsCorrectSubstring_Test()
    {
        // Arrange
        byte[] buffer = [255]; // 11111111
        int startBitIndex = 0;
        int endBitIndex = 3;

        // Act
        string result = buffer.GetBinaryStringByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("1111", result);
    }

    /// <summary>
    /// 测试获取中间位范围的二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryStringByBitIndex_WithMiddleRange_ReturnsCorrectSubstring_Test()
    {
        // Arrange
        byte[] buffer = [170]; // 10101010
        int startBitIndex = 2;
        int endBitIndex = 5;

        // Act
        string result = buffer.GetBinaryStringByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("1010", result);
    }

    /// <summary>
    /// 测试获取单个位的二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryStringByBitIndex_WithSingleBit_ReturnsSingleCharacter_Test()
    {
        // Arrange
        byte[] buffer = [128]; // 10000000
        int startBitIndex = 0;
        int endBitIndex = 0;

        // Act
        string result = buffer.GetBinaryStringByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("1", result);
    }

    /// <summary>
    /// 测试跨多个字节获取二进制字符串
    /// </summary>
    [TestMethod]
    public void GetBinaryStringByBitIndex_AcrossMultipleBytes_ReturnsCorrectSubstring_Test()
    {
        // Arrange
        byte[] buffer = [255, 0]; // 1111111100000000
        int startBitIndex = 6;
        int endBitIndex = 9;

        // Act
        string result = buffer.GetBinaryStringByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("1100", result);
    }

    /// <summary>
    /// 测试null字节数组抛出异常
    /// </summary>
    [TestMethod]
    public void GetBinaryStringByBitIndex_WithNullBuffer_ThrowsArgumentNullException_Test()
    {
        // Arrange
        byte[] buffer = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => buffer.GetBinaryStringByBitIndex(0, 7));
    }

    /// <summary>
    /// 测试负数起始索引抛出异常
    /// </summary>
    [TestMethod]
    public void GetBinaryStringByBitIndex_WithNegativeStartIndex_ThrowsArgumentOutOfRangeException_Test()
    {
        // Arrange
        byte[] buffer = [255];

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => buffer.GetBinaryStringByBitIndex(-1, 7));
    }

    /// <summary>
    /// 测试负数结束索引抛出异常
    /// </summary>
    [TestMethod]
    public void GetBinaryStringByBitIndex_WithNegativeEndIndex_ThrowsArgumentOutOfRangeException_Test()
    {
        // Arrange
        byte[] buffer = [255];

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => buffer.GetBinaryStringByBitIndex(0, -1));
    }

    /// <summary>
    /// 测试起始索引大于结束索引抛出异常
    /// </summary>
    [TestMethod]
    public void GetBinaryStringByBitIndex_WithStartIndexGreaterThanEndIndex_ThrowsArgumentOutOfRangeException_Test()
    {
        // Arrange
        byte[] buffer = [255];

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => buffer.GetBinaryStringByBitIndex(5, 2));
    }

    /// <summary>
    /// 测试结束索引超出范围抛出异常
    /// </summary>
    [TestMethod]
    public void GetBinaryStringByBitIndex_WithEndIndexOutOfRange_ThrowsArgumentOutOfRangeException_Test()
    {
        // Arrange
        byte[] buffer = [255];

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => buffer.GetBinaryStringByBitIndex(0, 8));
    }

    #endregion

    #region GetValueByBitIndex Tests

    /// <summary>
    /// 测试获取指定位范围的字节数组值
    /// </summary>
    [TestMethod]
    public void GetValueByBitIndex_WithValidRange_ReturnsCorrectByteArray_Test()
    {
        // Arrange
        byte[] buffer = [255]; // 11111111
        int startBitIndex = 0;
        int endBitIndex = 3; // 1111 = 15

        // Act
        byte[] result = buffer.GetValueByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result);
        int intValue = BitConverter.ToInt32(result, 0);
        Assert.AreEqual(15, intValue);
    }

    /// <summary>
    /// 测试获取零值的字节数组
    /// </summary>
    [TestMethod]
    public void GetValueByBitIndex_WithZeroValue_ReturnsZeroByteArray_Test()
    {
        // Arrange
        byte[] buffer = [0]; // 00000000
        int startBitIndex = 0;
        int endBitIndex = 7;

        // Act
        byte[] result = buffer.GetValueByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.IsNotNull(result);
        int intValue = BitConverter.ToInt32(result, 0);
        Assert.AreEqual(0, intValue);
    }

    /// <summary>
    /// 测试null字节数组抛出异常
    /// </summary>
    [TestMethod]
    public void GetValueByBitIndex_WithNullBuffer_ThrowsArgumentNullException_Test()
    {
        // Arrange
        byte[] buffer = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => buffer.GetValueByBitIndex(0, 7));
    }

    /// <summary>
    /// 测试无效索引抛出异常
    /// </summary>
    [TestMethod]
    public void GetValueByBitIndex_WithInvalidIndex_ThrowsArgumentOutOfRangeException_Test()
    {
        // Arrange
        byte[] buffer = [255];

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => buffer.GetValueByBitIndex(-1, 7));
    }

    #endregion

    #region GetIntValueByBitIndex Tests

    /// <summary>
    /// 测试获取指定位范围的整数值
    /// </summary>
    [TestMethod]
    public void GetIntValueByBitIndex_WithValidRange_ReturnsCorrectIntValue_Test()
    {
        // Arrange
        byte[] buffer = [255]; // 11111111
        int startBitIndex = 0;
        int endBitIndex = 3; // 1111 = 15

        // Act
        int result = buffer.GetIntValueByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.AreEqual(15, result);
    }

    /// <summary>
    /// 测试获取全字节的整数值
    /// </summary>
    [TestMethod]
    public void GetIntValueByBitIndex_WithFullByte_ReturnsCorrectIntValue_Test()
    {
        // Arrange
        byte[] buffer = [170]; // 10101010
        int startBitIndex = 0;
        int endBitIndex = 7;

        // Act
        int result = buffer.GetIntValueByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.AreEqual(170, result);
    }

    /// <summary>
    /// 测试获取单个位的整数值
    /// </summary>
    [TestMethod]
    public void GetIntValueByBitIndex_WithSingleBit_ReturnsZeroOrOne_Test()
    {
        // Arrange
        byte[] buffer = [128]; // 10000000
        int startBitIndex = 0;
        int endBitIndex = 0;

        // Act
        int result = buffer.GetIntValueByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.AreEqual(1, result);
    }

    /// <summary>
    /// 测试获取零值
    /// </summary>
    [TestMethod]
    public void GetIntValueByBitIndex_WithZeroBits_ReturnsZero_Test()
    {
        // Arrange
        byte[] buffer = [0]; // 00000000
        int startBitIndex = 0;
        int endBitIndex = 7;

        // Act
        int result = buffer.GetIntValueByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.AreEqual(0, result);
    }

    /// <summary>
    /// 测试跨多个字节获取整数值
    /// </summary>
    [TestMethod]
    public void GetIntValueByBitIndex_AcrossMultipleBytes_ReturnsCorrectIntValue_Test()
    {
        // Arrange
        byte[] buffer = [255, 255]; // 1111111111111111
        int startBitIndex = 4;
        int endBitIndex = 11; // 11111111 = 255

        // Act
        int result = buffer.GetIntValueByBitIndex(startBitIndex, endBitIndex);

        // Assert
        Assert.AreEqual(255, result);
    }

    /// <summary>
    /// 测试null字节数组抛出异常
    /// </summary>
    [TestMethod]
    public void GetIntValueByBitIndex_WithNullBuffer_ThrowsArgumentNullException_Test()
    {
        // Arrange
        byte[] buffer = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => buffer.GetIntValueByBitIndex(0, 7));
    }

    /// <summary>
    /// 测试无效索引抛出异常
    /// </summary>
    [TestMethod]
    public void GetIntValueByBitIndex_WithInvalidIndex_ThrowsArgumentOutOfRangeException_Test()
    {
        // Arrange
        byte[] buffer = [255];

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => buffer.GetIntValueByBitIndex(0, 10));
    }

    /// <summary>
    /// 测试起始索引大于结束索引抛出异常
    /// </summary>
    [TestMethod]
    public void GetIntValueByBitIndex_WithStartIndexGreaterThanEndIndex_ThrowsArgumentOutOfRangeException_Test()
    {
        // Arrange
        byte[] buffer = [255];

        // Act & Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => buffer.GetIntValueByBitIndex(7, 0));
    }

    #endregion
}
