namespace Materal.Utils.AutoMapper
{
    /// <summary>
    /// 自定映射配置
    /// </summary>
    public class AutoMapperConfig
    {
        /// <summary>
        /// 配置类型列表
        /// </summary>
        internal List<Type> ProfileTypes { get; } = [];
        /// <summary>
        /// 添加映射
        /// </summary>
        /// <param name="assemblies"></param>
        public void AddMaps(params Assembly[] assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                AddMaps(assembly);
            }
        }
        /// <summary>
        /// 添加映射
        /// </summary>
        /// <param name="assembly"></param>
        public void AddMaps(Assembly assembly)
        {
            Type[] ypes = [.. assembly.GetTypesByFilter(m => m.IsAssignableTo<Profile>())];
            AddMaps(ypes);
        }
        /// <summary>
        /// 添加映射
        /// </summary>
        /// <param name="types"></param>
        public void AddMaps(params Type[] types)
        {
            foreach (Type type in types)
            {
                AddMap(type);
            }
        }
        /// <summary>
        /// 添加映射
        /// </summary>
        /// <param name="type"></param>
        public void AddMap(Type type)
        {
            if (!type.IsAssignableTo<Profile>()) throw new MateralAutoMapperException("配置类型错误");
            ProfileTypes.Add(type);
        }
    }
}
