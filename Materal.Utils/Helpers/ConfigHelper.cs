using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace Materal.Utils.Helpers;

/// <summary>
/// 配置帮助类
/// </summary>
public class ConfigHelper
{
    private readonly IConfigurationRoot _configurationRoot;
    private readonly ConcurrentDictionary<string, object?> _pendingChanges = new();
    private readonly string _configFilePath = string.Empty;
    private readonly ILogger<ConfigHelper>? _logger;
#if NET9_0_OR_GREATER
    private readonly Lock _saveLock = new();
#else
    private readonly object _saveLock = new();
#endif

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="configurationRoot">配置根对象</param>
    /// <param name="logger">日志记录器（可选）</param>
    public ConfigHelper(IConfigurationRoot configurationRoot, ILogger<ConfigHelper>? logger = null)
    {
        _configurationRoot = configurationRoot ?? throw new ArgumentNullException(nameof(configurationRoot));
        _logger = logger;
        _configFilePath = FindJsonConfigFile();
        if (string.IsNullOrEmpty(_configFilePath))
        {
            _logger?.LogWarning("未找到可写入的 JSON 配置文件");
        }
    }

    /// <summary>
    /// 设置配置值（字符串类型）
    /// </summary>
    /// <param name="key">配置键，支持嵌套键（使用 : 分隔）</param>
    /// <param name="value">配置值</param>
    /// <returns>是否设置成功</returns>
    public bool SetValue(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger?.LogWarning("配置键不能为空");
            return false;
        }

        try
        {
            _pendingChanges[key] = value;
            _logger?.LogDebug("配置项 {Key} 已标记为待保存", key);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "设置配置值失败: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 设置配置值（泛型类型）
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    /// <param name="key">配置键，支持嵌套键（使用 : 分隔）</param>
    /// <param name="value">配置值</param>
    /// <returns>是否设置成功</returns>
    public bool SetValue<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger?.LogWarning("配置键不能为空");
            return false;
        }

        try
        {
            _pendingChanges[key] = value;
            _logger?.LogDebug("配置项 {Key} 已标记为待保存", key);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "设置配置值失败: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 保存所有待写入的配置更改到文件
    /// </summary>
    /// <returns>是否保存成功</returns>
    /// <exception cref="InvalidOperationException">当未找到配置文件时抛出</exception>
    /// <exception cref="IOException">当文件操作失败时抛出</exception>
    public bool SaveChanges()
    {
        if (_pendingChanges.IsEmpty)
        {
            _logger?.LogDebug("没有待保存的配置更改");
            return true;
        }

        if (string.IsNullOrEmpty(_configFilePath))
        {
            var exception = new InvalidOperationException("未找到可写入的 JSON 配置文件");
            _logger?.LogError(exception, "保存配置失败");
            throw exception;
        }

        lock (_saveLock)
        {
            try
            {
                _logger?.LogInformation("开始保存配置更改，共 {Count} 项", _pendingChanges.Count);

                // 读取现有配置文件
                string jsonContent = File.ReadAllText(_configFilePath);
                JsonNode? rootNode = JsonNode.Parse(jsonContent);
                if (rootNode is null)
                {
                    var exception = new InvalidOperationException("配置文件内容无效");
                    _logger?.LogError(exception, "解析配置文件失败: {FilePath}", _configFilePath);
                    throw exception;
                }

                // 应用所有挂起的更改
                foreach (var kvp in _pendingChanges)
                {
                    SetJsonValue(rootNode, kvp.Key, kvp.Value);
                    _logger?.LogDebug("应用配置更改: {Key}", kvp.Key);
                }

                // 写入临时文件（原子性操作）
                string tempFilePath = $"{_configFilePath}.tmp";
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new Utf8JsonWriter(fileStream, new JsonWriterOptions { Indented = true }))
                {
                    rootNode.WriteTo(writer);
                }

                // 替换原文件
                File.Copy(tempFilePath, _configFilePath, overwrite: true);
                File.Delete(tempFilePath);

                // 更新内存中的配置
                foreach (var kvp in _pendingChanges)
                {
                    _configurationRoot[kvp.Key] = ConvertToString(kvp.Value);
                }

                // 清空挂起的更改
                _pendingChanges.Clear();

                _logger?.LogInformation("配置更改保存成功: {FilePath}", _configFilePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "保存配置更改失败: {FilePath}", _configFilePath);
                throw new IOException($"保存配置文件失败: {_configFilePath}", ex);
            }
        }
    }

    /// <summary>
    /// 查找 JSON 配置文件路径（优先查找主配置文件）
    /// </summary>
    /// <returns>配置文件的完整路径，如果未找到则返回空字符串</returns>
    private string FindJsonConfigFile()
    {
        string? primaryConfigPath = null;

        foreach (var provider in _configurationRoot.Providers)
        {
            if (provider is not JsonConfigurationProvider jsonProvider) continue;
            FileConfigurationSource source = jsonProvider.Source;
            if (source == null || string.IsNullOrEmpty(source.Path)) continue;

            // 获取完整路径（处理相对路径）
            string fullPath = source.Path!;
            if (!Path.IsPathRooted(fullPath) && !string.IsNullOrEmpty(source.FileProvider?.GetFileInfo(source.Path!)?.PhysicalPath))
            {
                fullPath = source.FileProvider!.GetFileInfo(source.Path!).PhysicalPath!;
            }

            // 如果路径不是绝对路径且没有物理路径，尝试使用基础路径
            if (!Path.IsPathRooted(fullPath))
            {
                var basePath = AppContext.BaseDirectory;
                fullPath = Path.Combine(basePath, fullPath);
            }

            // 检查文件是否存在
            if (!File.Exists(fullPath)) continue;

            // 优先选择 appsettings.json（不带环境后缀的主配置文件）
            if (source.Path!.EndsWith("appsettings.json", StringComparison.OrdinalIgnoreCase))
            {
                _logger?.LogDebug("找到主配置文件: {Path}", fullPath);
                return fullPath;
            }

            // 记录第一个找到的 JSON 配置文件作为备选
            primaryConfigPath ??= fullPath;
        }

        if (!string.IsNullOrEmpty(primaryConfigPath))
        {
            _logger?.LogDebug("使用备选配置文件: {Path}", primaryConfigPath);
        }

        return primaryConfigPath ?? string.Empty;
    }

    /// <summary>
    /// 设置 JSON 嵌套值
    /// </summary>
    /// <param name="root">JSON 根节点</param>
    /// <param name="key">配置键（支持 : 分隔的嵌套路径）</param>
    /// <param name="value">配置值</param>
    private static void SetJsonValue(JsonNode root, string key, object? value)
    {
        var keys = key.Split(':');
        JsonNode? current = root;

        // 遍历到倒数第二个键
        for (int i = 0; i < keys.Length - 1; i++)
        {
            string keyPart = keys[i];
            if (current?[keyPart] is JsonNode child)
            {
                current = child;
            }
            else
            {
                // 路径不存在，创建中间对象
                if (current != null)
                {
                    current[keyPart] = new JsonObject();
                    current = current[keyPart];
                }
            }
        }

        // 设置最后一个键的值
        string lastKey = keys[^1];
        if (current is null) return;

        if (value == null)
        {
            current[lastKey] = null;
        }
        else
        {
            // 根据值的类型进行适当的转换
            current[lastKey] = ConvertToJsonValue(value);
        }
    }

    /// <summary>
    /// 将对象转换为 JsonNode
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>JsonNode 对象</returns>
    private static JsonNode? ConvertToJsonValue(object? value)
    {
        if (value == null) return null;

        return value switch
        {
            string str => JsonValue.Create(str),
            int intVal => JsonValue.Create(intVal),
            long longVal => JsonValue.Create(longVal),
            double doubleVal => JsonValue.Create(doubleVal),
            float floatVal => JsonValue.Create(floatVal),
            bool boolVal => JsonValue.Create(boolVal),
            decimal decimalVal => JsonValue.Create(decimalVal),
            DateTime dateTime => JsonValue.Create(dateTime),
            _ => JsonNode.Parse(JsonSerializer.Serialize(value))
        };
    }

    /// <summary>
    /// 将对象转换为字符串（用于更新 IConfigurationRoot）
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>字符串表示</returns>
    private static string? ConvertToString(object? value)
    {
        if (value == null) return null;

        return value switch
        {
            string str => str,
            _ => JsonSerializer.Serialize(value)
        };
    }
}
