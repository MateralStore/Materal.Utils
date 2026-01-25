
using System.Collections.Concurrent;

namespace Materal.Utils.AutoMapper;

/// <summary>
/// 配置管理器
/// </summary>
internal static class ProfileManager
{
#if NET9_0_OR_GREATER
    private static readonly Lock _lockObject = new();
#else
    private static readonly object _lockObject = new();
#endif
    /// <summary>
    /// 映射关系集合
    /// </summary>
    public static ConcurrentBag<MappingRelation> MappingRelations { get; } = [];
    /// <summary>
    /// 配置对象
    /// </summary>
    public static AutoMapperConfig Config { get; private set; } = new();
    /// <summary>
    /// 初始化配置管理器
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    public static void Init(IServiceProvider? serviceProvider = null)
    {
        lock (_lockObject)
        {
            Type[] types = [.. Config.ProfileTypes];
            foreach (Type profileType in types)
            {
                Profile profile = serviceProvider is null
                    ? profileType.Instantiation<Profile>()
                    : profileType.Instantiation<Profile>(serviceProvider);
                foreach (MappingRelation profileRelation in profile.MappingRelations)
                {
                    if (MappingRelations.Any(m => m.SourceType == profileRelation.SourceType && m.TargetType == profileRelation.TargetType))
                    {
                        continue;
                    }
                    MappingRelations.Add(profileRelation);
                }
            }
        }
    }
    /// <summary>
    /// 重置配置管理器状态(仅供测试使用)
    /// </summary>
    public static void Reset()
    {
        lock (_lockObject)
        {
            MappingRelations.Clear();
            Config = new();
        }
    }
}
