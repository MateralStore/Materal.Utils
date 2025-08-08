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
            MappingRelations.AddRange(profile.MappingRelations);
        }
    }
}
