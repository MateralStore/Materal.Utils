namespace Materal.Utils.Models;

/// <summary>
/// 返回模型
/// </summary>
public class ResultModel
{
    /// <summary>
    /// 返回消息
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// 对象类型
    /// </summary>
    public ResultType ResultType { get; set; } = ResultType.Success;
    /// <summary>
    /// 构造函数
    /// </summary>
    public ResultModel() { }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="resultType">返回类型</param>
    /// <param name="message">返回消息</param>
    public ResultModel(ResultType resultType, string? message = null)
    {
        ResultType = resultType;
        Message = message;
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="exception">异常消息</param>
    public ResultModel(Exception exception)
    {
        ResultType = ResultType.Fail;
        Message = exception.GetErrorMessage();
    }
    /// <summary>
    /// 获得一个成功返回对象
    /// </summary>
    /// <param name="message">返回消息</param>
    /// <returns>成功返回对象</returns>
    public static ResultModel Success(string? message = null) => new(ResultType.Success, message);
    /// <summary>
    /// 获得一个失败返回对象
    /// </summary>
    /// <param name="message">返回消息</param>
    /// <returns>失败返回对象</returns>
    public static ResultModel Fail(string? message = null) => new(ResultType.Fail, message);
    /// <summary>
    /// 获得一个警告返回对象
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ResultModel Waring(string? message = null) => new(ResultType.Waring, message);
}
/// <summary>
/// 返回模型
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResultModel<T> : ResultModel
{
    /// <summary>
    /// 返回数据
    /// </summary>
    public T? Data { get; set; }
    /// <summary>
    /// 构造函数
    /// </summary>
    public ResultModel() : base() { }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="resultType">返回类型</param>
    /// <param name="data">返回数据对象</param>
    /// <param name="message">返回消息</param>
    public ResultModel(ResultType resultType, T? data, string? message = null) : base(resultType, message) => Data = data;
    /// <summary>
    /// 获得一个成功返回对象
    /// </summary>
    /// <param name="data">返回数据对象</param>
    /// <param name="message">返回消息</param>
    /// <returns>成功返回对象</returns>
    public static ResultModel<T> Success(T data, string? message = null) => new(ResultType.Success, data, message);
    /// <summary>
    /// 获得一个成功返回对象
    /// </summary>
    /// <param name="message">返回消息</param>
    /// <returns>成功返回对象</returns>
    public new static ResultModel<T> Success(string? message = null) => new(ResultType.Success, default, message);
    /// <summary>
    /// 获得一个失败返回对象
    /// </summary>
    /// <param name="data">返回数据对象</param>
    /// <param name="message">返回消息</param>
    /// <returns>失败返回对象</returns>
    public static ResultModel<T> Fail(T data, string? message = null) => new(ResultType.Fail, data, message);
    /// <summary>
    /// 获得一个失败返回对象
    /// </summary>
    /// <param name="message">返回消息</param>
    /// <returns>失败返回对象</returns>
    public new static ResultModel<T> Fail(string? message = null) => new(ResultType.Fail, default, message);
    /// <summary>
    /// 获得一个警告返回对象
    /// </summary>
    /// <param name="data">返回数据对象</param>
    /// <param name="message">返回消息</param>
    /// <returns>警告返回对象</returns>
    public static ResultModel<T> Waring(T data, string? message = null) => new(ResultType.Waring, data, message);
    /// <summary>
    /// 获得一个警告返回对象
    /// </summary>
    /// <param name="message">返回消息</param>
    /// <returns>警告返回对象</returns>
    public new static ResultModel<T> Waring(string? message = null) => new(ResultType.Waring, default, message);
}
