using MongoDB.Driver;
using System.Collections.Concurrent;

namespace Materal.Utils.MongoDB
{
    /// <summary>
    /// MongoDB连接管理器（单例模式）
    /// </summary>
    public class MongoClientManager
    {
        private static readonly Lazy<MongoClientManager> _instance = new(() => new MongoClientManager());
        /// <summary>
        /// 单例实例
        /// </summary>
        public static MongoClientManager Instance => _instance.Value;
        /// <summary>
        /// MongoClient实例缓存
        /// </summary>
        private readonly ConcurrentDictionary<string, MongoClient> _clients = new();
        private MongoClientManager() { }
        /// <summary>
        /// 获取MongoClient实例
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>MongoClient实例</returns>
        public MongoClient GetClient(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new MongoUtilException("连接字符串为空");

            return _clients.GetOrAdd(connectionString, connStr =>
            {
                try
                {
                    MongoClientSettings settings = MongoClientSettings.FromConnectionString(connStr);
                    //settings.MaxConnectionPoolSize = 100;
                    //settings.MinConnectionPoolSize = 10;
                    settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(10);
                    return new MongoClient(settings);
                }
                catch (Exception ex)
                {
                    throw new MongoUtilException($"连接数据库失败", ex);
                }
            });
        }
    }
}