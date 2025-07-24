namespace Materal.Utils.Model
{
    /// <summary>
    /// 返回对象类型
    /// </summary>
    public enum ResultTypeEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success = 0,
        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Fail = 1,
        /// <summary>
        /// 警告
        /// </summary>
        [Description("警告")]
        Waring = 2
    }
}
