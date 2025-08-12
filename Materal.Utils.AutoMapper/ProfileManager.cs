namespace Materal.Utils.AutoMapper;

/// <summary>
/// 配置管理器
/// </summary>
internal static class ProfileManager
{
    public static List<MappingRelation> MappingRelations { get; } = [];
    public static AutoMapperConfig Config = new();
    public static void Init(IServiceProvider? serviceProvider = null)
    {
        foreach (Type profileType in Config.ProfileTypes)
        {
            Profile profile = serviceProvider is null
                ? profileType.Instantiation<Profile>()
                : profileType.Instantiation<Profile>(serviceProvider);
            if (MappingRelations.Any(m => profile.MappingRelations.Any(pm => pm.SourceType == m.SourceType && pm.TargetType == m.TargetType)))
            {
                throw new MateralAutoMapperException("MappingRelations映射关系重复");
            }
            MappingRelations.AddRange(profile.MappingRelations);
        }
    }
}
