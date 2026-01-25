using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Materal.Utils.Caching;

/// <summary>
/// 依赖注入扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加缓存工具
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMateralCachingUtils(this IServiceCollection services)
    {
        services.TryAddSingleton<ICacheHelper, MemoryCacheHelper>();
        return services;
    }
}
