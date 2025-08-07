namespace Materal.Utils.AutoMapper
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public abstract class Profile
    {
        internal List<MappingRelation> MappingRelations { get; } = [];
        /// <summary>
        /// 创建映射
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="map"></param>
        /// <param name="reverseMap"></param>
        protected void CreateMap<T1, T2>(Action<IMapper, T1, T2> map, Action<IMapper, T2, T1>? reverseMap = null)
        {
            MappingRelations.Add(new MappingRelation<T1, T2>(map));
            if (reverseMap is not null)
            {
                MappingRelations.Add(new MappingRelation<T2, T1>(reverseMap));
            }
        }
    }
}
