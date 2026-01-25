namespace Materal.Utils.Test.AutoMapperTest;

/// <summary>
/// MateralAutoMapperException 测试类
/// 测试 AutoMapper 框架的异常处理
/// </summary>
[TestClass]
public class MateralAutoMapperExceptionTest
{
    /// <summary>
    /// 测试无参构造方法创建异常实例
    /// </summary>
    [TestMethod]
    public void Constructor_WhenCalled_CreatesInstanceWithoutThrowing_Test()
    {
        // Act & Assert
        MateralAutoMapperException exception = new();
        Assert.IsNotNull(exception);
    }

    /// <summary>
    /// 测试使用消息创建异常实例
    /// </summary>
    [TestMethod]
    public void Constructor_WithMessage_CreatesInstanceWithMessage_Test()
    {
        // Arrange
        string expectedMessage = "测试错误消息";

        // Act
        MateralAutoMapperException exception = new(expectedMessage);

        // Assert
        Assert.IsNotNull(exception);
        Assert.AreEqual(expectedMessage, exception.Message);
    }

    /// <summary>
    /// 测试使用消息和内部异常创建异常实例
    /// </summary>
    [TestMethod]
    public void Constructor_WithMessageAndInnerException_CreatesInstanceWithBoth_Test()
    {
        // Arrange
        string expectedMessage = "测试错误消息";
        Exception innerException = new InvalidOperationException("内部异常");

        // Act
        MateralAutoMapperException exception = new(expectedMessage, innerException);

        // Assert
        Assert.IsNotNull(exception);
        Assert.AreEqual(expectedMessage, exception.Message);
        Assert.AreSame(innerException, exception.InnerException);
    }

    /// <summary>
    /// 测试异常继承自 MateralException
    /// </summary>
    [TestMethod]
    public void MateralAutoMapperException_InheritsFromMateralException_Test()
    {
        // Arrange
        MateralAutoMapperException exception = new("Test");

        // Act & Assert
        Assert.IsInstanceOfType<MateralException>(exception);
    }

    /// <summary>
    /// 测试异常可以正常抛出和捕获
    /// </summary>
    [TestMethod]
    public void MateralAutoMapperException_WhenThrown_CanBeCaught_Test()
    {
        // Act & Assert
        Assert.ThrowsExactly<MateralAutoMapperException>(() => throw new MateralAutoMapperException());
        Assert.ThrowsExactly<MateralAutoMapperException>(() => throw new MateralAutoMapperException("测试消息"));
    }
}
