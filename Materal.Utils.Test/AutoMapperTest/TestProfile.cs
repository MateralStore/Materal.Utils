using Materal.Utils.AutoMapper;

namespace Materal.Utils.Test.AutoMapperTest
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<ModelA, ModelC>((mapper, m, n) =>
            {
                n ??= new();
                n.CreateTime = m.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
            }, (mapper, m, n) =>
            {
                n ??= new();
                n.CreateTime = DateTime.Parse(m.CreateTime);
            });
            CreateMap<SubModelA, SubModelC>((mapper, m, n) =>
            {
                n ??= new();
                n.Age = m.Age.ToString();
            }, (mapper, m, n) =>
            {
                n ??= new();
                n.Age = int.Parse(m.Age);
            });
        }
    }
}
