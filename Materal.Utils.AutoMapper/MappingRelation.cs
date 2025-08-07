namespace Materal.Utils.AutoMapper
{
    /// <summary>
    /// 映射关系
    /// </summary>
    public abstract class MappingRelation
    {
        /// <summary>
        /// 源类型
        /// </summary>
        public abstract Type SourceType { get; }
        /// <summary>
        /// 目标类型
        /// </summary>
        public abstract Type TargetType { get; }
        /// <summary>
        /// 映射方法
        /// </summary>
        public Action<IMapper, object, object?> MapObj { get; protected set; } = (mapper, source, target) => { };
    }
    /// <summary>
    /// 映射关系
    /// </summary>
    public partial class MappingRelation<T1, T2> : MappingRelation
    {
        /// <inheritdoc/>
        public override Type SourceType { get; } = typeof(T1);

        /// <inheritdoc/>
        public override Type TargetType { get; } = typeof(T2);
        /// <summary>
        /// 映射方法
        /// </summary>
        public Action<IMapper, T1, T2?> Map { get; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="map"></param>
        public MappingRelation(Action<IMapper, T1, T2?> map)
        {
            Map = map;
            MapObj = (mapper, source, target) => Map(mapper, (T1)source, target is null ? default : (T2)target);
        }
    }
}
