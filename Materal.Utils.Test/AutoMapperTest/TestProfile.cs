using Materal.Utils.AutoMapper;

namespace Materal.Utils.Test.AutoMapperTest
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<ModelA, ModelC>((mapper, m, n) =>
            {
                m.CopyProperties(n);
                mapper.Map(m.Sub, n.Sub);
                n.CreateTime = m.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
            }, (mapper, m, n) =>
            {
                m.CopyProperties(n);
                mapper.Map(m.Sub, n.Sub);
                n.CreateTime = DateTime.Parse(m.CreateTime);
            });
            CreateMap<SubModelA, SubModelC>((mapper, m, n) =>
            {
                n.Age = m.Age.ToString();
            }, (mapper, m, n) =>
            {
                n.Age = int.Parse(m.Age);
            });
        }
    }
}
