using System.Text.Json;

namespace Materal.Utils.Test.AutoMapperTest;

[TestClass]
[DoNotParallelize]
public class BugTest
{
    private Mapper _mapper = null!;

    [TestInitialize]
    public void Setup()
    {
        _mapper = new Mapper();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _mapper = null!;
        ProfileManager.Reset();
    }
    [TestMethod]
    public void Bug1Test()
    {
        ProfileManager.Config.AddMap(typeof(BugProfile));
        ProfileManager.Init();
        List<CustomersRrelationsView> source = LoadBugData();
        List<CustomersListDTO> result = _mapper.Map<List<CustomersListDTO>>(source);
        Assert.AreEqual(source.Count, result.Count);
        Assert.AreEqual(source[0].CustomersID, result[0].ID);
        Assert.AreEqual(source[0].CustomersName, result[0].Name);
        Assert.AreEqual(source[0].CustomersPhoneNumber, result[0].PhoneNumber);
        Assert.AreEqual(source[0].CustomersSex, result[0].Sex);
        Assert.AreEqual(source[0].CustomersAddress, result[0].Address);
    }

    private static List<CustomersRrelationsView> LoadBugData()
    {
        string jsonPath = FindBugDataPath();
        string json = File.ReadAllText(jsonPath, Encoding.UTF8);
        return JsonSerializer.Deserialize<List<CustomersRrelationsView>>(json) ?? [];
    }

    private static string FindBugDataPath()
    {
        foreach (string startPath in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            DirectoryInfo? directory = new(startPath);
            while (directory is not null)
            {
                string path = Path.Combine(directory.FullName, "Materal.Utils", "Materal.Utils.Test", "AutoMapperTest", "BugData.json");
                if (File.Exists(path)) return path;
                path = Path.Combine(directory.FullName, "AutoMapperTest", "BugData.json");
                if (File.Exists(path)) return path;
                directory = directory.Parent;
            }
        }
        throw new FileNotFoundException("未找到BugData.json");
    }

    private sealed class BugProfile : Profile
    {
        public BugProfile()
        {
            CreateMap<CustomersRrelationsView, CustomersListDTO>((mapper, m, n) =>
            {
                n ??= new();
                n.CreateTime = m.CreateTime;
                n.ID = m.CustomersID;
                n.Name = m.CustomersName;
                n.PhoneNumber = m.CustomersPhoneNumber;
                n.Sex = m.CustomersSex;
                n.Address = m.CustomersAddress;
            });
        }
    }
}
