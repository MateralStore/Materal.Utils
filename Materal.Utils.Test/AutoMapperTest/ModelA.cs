namespace Materal.Utils.Test.AutoMapperTest;

public class ModelA
{
    public string Name { get; set; } = string.Empty;
    public SubModelA Sub { get; set; } = new();
    public DateTime CreateTime { get; set; } = DateTime.Now;
}
public class ModelB
{
    public string Name { get; set; } = string.Empty;
    public SubModelA Sub { get; set; } = new();
    public DateTime CreateTime { get; set; } = DateTime.Now;
}
public class ModelC
{
    public string Name { get; set; } = string.Empty;
    public SubModelC Sub { get; set; } = new();
    public string CreateTime { get; set; } = string.Empty;
}
public class SubModelA
{
    public int Age { get; set; }
}
public class SubModelC
{
    public string Age { get; set; }
}