using Materal.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Materal.Utils.Email;

/// <summary>
/// 依赖注入扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加Consul工具
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMateralEmailUtils(this IServiceCollection services)
    {
        services.AddMateralUtils();
        services.TryAddSingleton<IEmailService, EmailService>();
        return services;
    }
}
