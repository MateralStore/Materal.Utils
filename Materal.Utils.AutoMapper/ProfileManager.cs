namespace Materal.Utils.AutoMapper
{
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
                Profile profile = profileType.Instantiation<Profile>();
                MappingRelations.AddRange(profile.MappingRelations);
            }
        }
    }
}
