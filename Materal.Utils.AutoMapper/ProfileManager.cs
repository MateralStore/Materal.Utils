using Materal.Extensions.DependencyInjection;

namespace Materal.Utils.AutoMapper;

/// <summary>
/// 配置管理器
/// </summary>
internal static class ProfileManager
{
    public static List<MappingRelation> MappingRelations { get; } = [];
    public static AutoMapperConfig Config = new();
    public static void Init()
    {
        foreach (Type profileType in Config.ProfileTypes)
        {
            Profile profile = MateralServices.ServiceProvider is null
                ? profileType.Instantiation<Profile>()
                : profileType.Instantiation<Profile>(MateralServices.ServiceProvider);
            MappingRelations.AddRange(profile.MappingRelations);
        }
    }
}
