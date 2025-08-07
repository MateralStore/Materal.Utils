using Materal.Abstractions;

namespace Materal.Utils.AutoMapper
{
    /// <summary>
    /// AutoMapper异常
    /// </summary>
    public class MateralAutoMapperException : MateralException
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public MateralAutoMapperException()
        {
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message"></param>
        public MateralAutoMapperException(string message) : base(message)
        {
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MateralAutoMapperException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
