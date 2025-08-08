using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Materal.Utils.AutoMapper.Extensions;

/// <summary>
/// ServiceCollection扩展
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// 添加所有工具
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<AutoMapperConfig>? configAction = null)
    {
        services.TryAddSingleton<IMapper, Mapper>();
        configAction?.Invoke(ProfileManager.Config);
        return services;
    }
    /// <summary>
    /// 使用AutoMapper
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceProvider UseAutoMapper(this IServiceProvider services)
    {
        ProfileManager.Init(services);
        return services;
    }
}
