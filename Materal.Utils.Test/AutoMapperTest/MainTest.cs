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
    /// <summary>
    /// 无配置文件测试
    /// </summary>
    [TestMethod]
    public void NotProfileTest()
    {
        IMapper mapper = ServiceProvider.GetRequiredService<IMapper>();
        ModelA modelA = new()
        {
            Name = "测试",
            Sub = new()
            {
                Age = 18
            }
        };
        ModelB modelB = mapper.Map<ModelB>(modelA);
        Assert.AreEqual(modelA.Name, modelB.Name);
        Assert.AreEqual(modelA.Sub, modelB.Sub);
        Assert.AreEqual(modelA.Sub.Age, modelB.Sub.Age);
        Assert.AreEqual(modelA.CreateTime, modelB.CreateTime);
        List<ModelA> modelAs =
        [
            modelA,
            new ModelA
            {
                Name = "测试1",
                Sub = new()
                {
                    Age = 19
                }
            }
        ];
        List<ModelB> modelBs = mapper.Map<List<ModelB>>(modelAs);
        Assert.AreEqual(modelAs.Count, modelBs.Count);
        for (int i = 0; i < modelAs.Count; i++)
        {
            Assert.AreEqual(modelAs[i].Name, modelBs[i].Name);
            Assert.AreEqual(modelAs[i].Sub, modelBs[i].Sub);
            Assert.AreEqual(modelAs[i].Sub.Age, modelBs[i].Sub.Age);
            Assert.AreEqual(modelAs[i].CreateTime, modelBs[i].CreateTime);
        }
    }
    /// <summary>
    /// 有配置文件测试
    /// </summary>
    [TestMethod]
    public void ProfileTest()
    {
        IMapper mapper = ServiceProvider.GetRequiredService<IMapper>();
        ModelA modelA = new()
        {
            Name = "测试",
            Sub = new()
            {
                Age = 18
            }
        };
        ModelC modelC = mapper.Map<ModelC>(modelA);
        Assert.AreEqual(modelA.Name, modelC.Name);
        Assert.AreEqual(modelA.CreateTime.ToString("yyyy/MM/dd HH:mm:ss"), modelC.CreateTime);
        List<ModelA> modelAs =
        [
            modelA,
            new ModelA
            {
                Name = "测试1",
                Sub = new()
                {
                    Age = 19
                }
            }
        ];
        List<ModelC> modelCs = mapper.Map<List<ModelC>>(modelAs);
        Assert.AreEqual(modelAs.Count, modelCs.Count);
        for (int i = 0; i < modelAs.Count; i++)
        {
            Assert.AreEqual(modelAs[i].Name, modelCs[i].Name);
            Assert.AreEqual(modelAs[i].Sub.Age, int.Parse(modelCs[i].Sub.Age));
            Assert.AreEqual(modelAs[i].CreateTime.ToString("yyyy/MM/dd HH:mm:ss"), modelCs[i].CreateTime);
        }
    }
    /// <summary>
    /// 无配置文件对象测试
    /// </summary>
    [TestMethod]
    public void NotProfileObjTest()
    {
        IMapper mapper = ServiceProvider.GetRequiredService<IMapper>();
        ModelA modelA = new()
        {
            Name = "测试",
            Sub = new()
            {
                Age = 18
            }
        };
        ModelB modelB = new();
        mapper.Map(modelA, modelB);
        Assert.AreEqual(modelA.Name, modelB.Name);
        Assert.AreEqual(modelA.Sub, modelB.Sub);
        Assert.AreEqual(modelA.Sub.Age, modelB.Sub.Age);
        Assert.AreEqual(modelA.CreateTime, modelB.CreateTime);
        List<ModelA> modelAs =
        [
            modelA,
            new ModelA
            {
                Name = "测试1",
                Sub = new()
                {
                    Age = 19
                }
            }
        ];
        List<ModelB> modelBs = [];
        mapper.Map(modelAs, modelBs);
        Assert.AreEqual(modelAs.Count, modelBs.Count);
        for (int i = 0; i < modelAs.Count; i++)
        {
            Assert.AreEqual(modelAs[i].Name, modelBs[i].Name);
            Assert.AreEqual(modelAs[i].Sub, modelBs[i].Sub);
            Assert.AreEqual(modelAs[i].Sub.Age, modelBs[i].Sub.Age);
            Assert.AreEqual(modelAs[i].CreateTime, modelBs[i].CreateTime);
        }
    }
    /// <summary>
    /// 有配置文件测试
    /// </summary>
    [TestMethod]
    public void ProfileObjTest()
    {
        IMapper mapper = ServiceProvider.GetRequiredService<IMapper>();
        ModelA modelA = new()
        {
            Name = "测试",
            Sub = new()
            {
                Age = 18
            }
        };
        ModelC modelC = new();
        mapper.Map(modelA, modelC);
        Assert.AreEqual(modelA.Name, modelC.Name);
        Assert.AreEqual(modelA.CreateTime.ToString("yyyy/MM/dd HH:mm:ss"), modelC.CreateTime);
        List<ModelA> modelAs =
        [
            modelA,
            new ModelA
            {
                Name = "测试1",
                Sub = new()
                {
                    Age = 19
                }
            }
        ];
        List<ModelC> modelCs = [];
        mapper.Map(modelAs, modelCs);
        Assert.AreEqual(modelAs.Count, modelCs.Count);
        for (int i = 0; i < modelAs.Count; i++)
        {
            Assert.AreEqual(modelAs[i].Name, modelCs[i].Name);
            Assert.AreEqual(modelAs[i].Sub.Age, int.Parse(modelCs[i].Sub.Age));
            Assert.AreEqual(modelAs[i].CreateTime.ToString("yyyy/MM/dd HH:mm:ss"), modelCs[i].CreateTime);
        }
    }
}