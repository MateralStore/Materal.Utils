using Materal.Utils.Network.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Materal.Utils.Network;

/// <summary>
/// 依赖注入扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加Network工具
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMateralNetworkUtils(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpHelper, HttpHelper>();
        return services;
    }
}
