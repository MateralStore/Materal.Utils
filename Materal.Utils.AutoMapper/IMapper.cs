namespace Materal.Utils.AutoMapper;

/// <summary>
/// 映射器
/// </summary>
public interface IMapper
{
    /// <summary>
    /// 服务容器
    /// </summary>
    public IServiceProvider? ServiceProvider { get; }
    /// <summary>
    /// 映射
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public T Map<T>(object source);
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public void Map(object source, object target);
}
