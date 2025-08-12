using Materal.Utils.AutoMapper;
using Materal.Utils.AutoMapper.Extensions;

namespace Materal.Utils.Test.AutoMapperTest;

/// <summary>
/// 主要测试
/// </summary>
[TestClass]
public partial class MainTest : MateralTestBase
{
    public override void AddServices(IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddMaps(GetType().Assembly);
        });
    }
    protected override IServiceProvider BuilderServiceProvider(IServiceCollection services)
    {
        IServiceProvider? serviceProvider = base.BuilderServiceProvider(services);
        serviceProvider.UseAutoMapper();
        return serviceProvider;
    }
    /// <summary>
    /// 测试默认映射单个对象
    /// </summary>
    [TestMethod]
    public void MapOneModelTest()
    {
        IMapper mapper = ServiceProvider.GetRequiredService<IMapper>();
        ModelA source = new()
        {
            Name = "测试",
            Sub = new()
            {
                Age = 18
            },
            Subs = [
                new(){ Age = 18 },
                new(){ Age = 19 },
                new(){ Age = 20 }
            ]
        };
        {
            ModelA modelA = mapper.Map<ModelA>(source);
            Assert.AreEqual(source.Name, modelA.Name);
            Assert.AreEqual(source.Sub.Age, modelA.Sub.Age);
            Assert.AreEqual(source.CreateTime.ToString("yyyy/MM/dd HH:mm:ss"), modelA.CreateTime.ToString("yyyy/MM/dd HH:mm:ss"));
            Assert.AreEqual(source.Subs.Count, modelA.Subs.Count);
            for (int i = 0; i < source.Subs.Count; i++)
            {
                Assert.AreEqual(source.Subs[i].Age, modelA.Subs[i].Age);
            }
        }
        {
            ModelB modelB = mapper.Map<ModelB>(source);
            Assert.AreEqual(source.Name, modelB.Name);
            Assert.AreEqual(source.Sub.Age, modelB.Sub.Age);
            Assert.AreEqual(source.CreateTime.ToString("yyyy/MM/dd HH:mm:ss"), modelB.CreateTime.ToString("yyyy/MM/dd HH:mm:ss"));
            Assert.AreEqual(source.Subs.Count, modelB.Subs.Count);
            for (int i = 0; i < source.Subs.Count; i++)
            {
                Assert.AreEqual(source.Subs[i].Age, modelB.Subs[i].Age);
            }
        }
        {
            ModelC modelC = mapper.Map<ModelC>(source);
            Assert.AreEqual(source.Name, modelC.Name);
            Assert.AreEqual(source.Sub.Age, int.Parse(modelC.Sub.Age));
            Assert.AreEqual(source.CreateTime.ToString("yyyy/MM/dd HH:mm:ss"), modelC.CreateTime);
            Assert.AreEqual(source.Subs.Count, modelC.Subs.Count);
            for (int i = 0; i < source.Subs.Count; i++)
            {
                Assert.AreEqual(source.Subs[i].Age, int.Parse(modelC.Subs[i].Age));
            }
        }
    }
}