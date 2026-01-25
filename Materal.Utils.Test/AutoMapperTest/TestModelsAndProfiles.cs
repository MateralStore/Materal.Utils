namespace Materal.Utils.Test.AutoMapperTest;

#region Shared Test Models

/// <summary>
/// 测试源模型
/// </summary>
public class TestSourceModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// 测试目标模型
/// </summary>
public class TestTargetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// 测试反向源模型
/// </summary>
public class TestReverseSourceModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// 测试反向目标模型
/// </summary>
public class TestReverseTargetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// 带所有属性类型的源模型
/// </summary>
public class FullPropertiesSourceModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? NullableInt { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public NestedSourceModel? Nested { get; set; }
}

/// <summary>
/// 带所有属性类型的目标模型
/// </summary>
public class FullPropertiesTargetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? NullableInt { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public NestedTargetModel? Nested { get; set; }
}

/// <summary>
/// 嵌套源模型
/// </summary>
public class NestedSourceModel
{
    public int Value { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// 嵌套目标模型
/// </summary>
public class NestedTargetModel
{
    public int Value { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// 列表源模型
/// </summary>
public class ListSourceModel
{
    public List<string> Strings { get; set; } = [];
    public List<int> Ints { get; set; } = [];
}

/// <summary>
/// 列表目标模型
/// </summary>
public class ListTargetModel
{
    public List<string> Strings { get; set; } = [];
    public List<int> Ints { get; set; } = [];
}

/// <summary>
/// 嵌套列表源模型
/// </summary>
public class NestedListSourceModel
{
    public List<NestedSourceModel> Items { get; set; } = [];
}

/// <summary>
/// 嵌套列表目标模型
/// </summary>
public class NestedListTargetModel
{
    public List<NestedTargetModel> Items { get; set; } = [];
}

/// <summary>
/// 只读属性源模型
/// </summary>
public class ReadOnlySourceModel
{
    public int Id { get; set; }
    public string Name => $"Name_{Id}";
}

/// <summary>
/// 只读属性目标模型
/// </summary>
public class ReadOnlyTargetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// 带构造函数的模型
/// </summary>
public class ConstructorModel
{
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty;

    public ConstructorModel()
    {
    }

    public ConstructorModel(int value, string name)
    {
        Value = value;
        Name = name;
    }
}

#endregion

#region Shared Test Profiles

/// <summary>
/// 测试配置文件
/// </summary>
public class TestProfile : Profile
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public TestProfile()
    {
        CreateMap<TestSourceModel, TestTargetModel>((mapper, source, target) =>
        {
            target.Id = source.Id;
            target.Name = source.Name;
        });
    }
}

/// <summary>
/// 测试反向映射配置文件
/// </summary>
public class TestReverseProfile : Profile
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public TestReverseProfile()
    {
        CreateMap<TestSourceModel, TestTargetModel>((mapper, source, target) =>
        {
            target.Id = source.Id;
            target.Name = source.Name;
        }, (mapper, target, source) =>
        {
            source.Id = target.Id;
            source.Name = target.Name;
        });
    }
}

/// <summary>
/// 测试使用默认映射的配置文件
/// </summary>
public class TestUseDefaultMapperProfile : Profile
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public TestUseDefaultMapperProfile()
    {
        CreateMap<TestSourceModel, TestTargetModel>((mapper, source, target) =>
        {
            target.Name = $"{target.Name} (customized)";
        }, useDefaultMapper: true);
    }
}

/// <summary>
/// 测试多映射配置文件
/// </summary>
public class TestMultipleMapsProfile : Profile
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public TestMultipleMapsProfile()
    {
        CreateMap<TestSourceModel, TestTargetModel>((mapper, source, target) =>
        {
            target.Id = source.Id;
            target.Name = source.Name;
        });

        CreateMap<TestReverseSourceModel, TestReverseTargetModel>((mapper, source, target) =>
        {
            target.Id = source.Id;
            target.Name = source.Name;
        });
    }
}

/// <summary>
/// 测试配置 A
/// </summary>
public class TestProfileA : Profile
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public TestProfileA()
    {
        CreateMap<TestSourceModel, TestTargetModel>((mapper, source, target) =>
        {
            target.Id = source.Id;
            target.Name = source.Name;
        });
    }
}

/// <summary>
/// 测试配置 B
/// </summary>
public class TestProfileB : Profile
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public TestProfileB()
    {
        CreateMap<TestReverseSourceModel, TestReverseTargetModel>((mapper, source, target) =>
        {
            target.Id = source.Id;
            target.Name = source.Name;
        });
    }
}

/// <summary>
/// 非配置类
/// </summary>
public class NotAProfile
{
}

/// <summary>
/// 测试用配置文件
/// </summary>
public class TestMapperProfile : Profile
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public TestMapperProfile()
    {
        CreateMap<TestSourceModel, TestTargetModel>((mapper, source, target) =>
        {
            target.Id = source.Id;
            target.Name = source.Name;
        });

        CreateMap<NestedSourceModel, NestedTargetModel>((mapper, source, target) =>
        {
            target.Value = source.Value;
            target.Description = source.Description;
        });
    }
}

#endregion
