using System.Collections;

namespace Materal.Utils.AutoMapper;

/// <summary>
/// 映射器
/// </summary>
public partial class Mapper : IMapper
{
    /// <inheritdoc/>
    public IServiceProvider? ServiceProvider { get; }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="serviceProvider"></param>
    public Mapper(IServiceProvider? serviceProvider = null)
    {
        ServiceProvider = serviceProvider;
        MethodInfo[] methodInfos = typeof(IMapper).GetMethods();
        _mapperGenericMethod = methodInfos.First(m => m.Name == nameof(Map) && m.IsGenericMethod);
        _mapperMethod = methodInfos.First(m => m.Name == nameof(Map) && !m.IsGenericMethod);
    }
    /// <inheritdoc/>
    public T Map<T>(object source)
    {
        Type targetType = typeof(T);
        T target = ServiceProvider is null ? targetType.Instantiation<T>() : targetType.Instantiation<T>(ServiceProvider);
        Map(source, target!);
        return target;
    }
    /// <inheritdoc/>
    public void Map(object source, object target)
    {
        Type sourceType = source.GetType();
        Type targetType = target.GetType();
        if (sourceType.IsAssignableTo<IList>() && targetType.IsAssignableTo<IList>())
        {
            Type trueTargetType = targetType.GetGenericArguments().FirstOrDefault() ?? throw new MateralAutoMapperException("获取列表类型失败");
            MapList((IList)source, (IList)target);
            return;
        }
        MappingRelation? mappingRelation = ProfileManager.MappingRelations.FirstOrDefault(m => m.SourceType == sourceType && m.TargetType == targetType);
        MapObject(source, target, mappingRelation);
    }
    private void MapList(IList source, IList target)
    {
        Type sourceType = source.GetType();
        Type targetType = target.GetType();
        Type trueSourceType = sourceType.GetGenericArguments().FirstOrDefault() ?? throw new MateralAutoMapperException("获取列表类型失败");
        Type trueTargetType = targetType.GetGenericArguments().FirstOrDefault() ?? throw new MateralAutoMapperException("获取列表类型失败");
        MappingRelation? mappingRelation = ProfileManager.MappingRelations.FirstOrDefault(m => m.SourceType == trueSourceType && m.TargetType == trueTargetType);
        foreach (object? item in source)
        {
            if (item is null)
            {
                target.Add(default!);
            }
            else
            {
                object tItem = ServiceProvider is null ? trueTargetType.Instantiation() : trueTargetType.Instantiation(ServiceProvider);
                MapObject(item, tItem, mappingRelation);
                target.Add(tItem);
            }
        }
    }
    /// <summary>
    /// 映射Object
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="mappingRelation"></param>
    /// <exception cref="MateralAutoMapperException"></exception>
    private void MapObject(object source, object target, MappingRelation? mappingRelation = null)
    {
        try
        {
            if (mappingRelation is null)
            {
                DefaultMap(source, target);
                return;
            }
            if (mappingRelation.UseDefaultMapper)
            {
                DefaultMap(source, target);
            }
            mappingRelation.MapObj(this, source, target);
        }
        catch (Exception ex)
        {
            throw new MateralAutoMapperException("映射结果转换失败", ex);
        }
    }
}
