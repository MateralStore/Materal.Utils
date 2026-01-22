using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// HybridCrypto工具方法扩展
/// </summary>
public static partial class HybridCrypto
{
    #region 工具方法
    /// <summary>
    /// 生成RSA密钥对用于混合加密
    /// </summary>
    /// <param name="keySize">密钥长度，默认2048位</param>
    /// <returns>包含公钥和私钥的元组</returns>
    /// <exception cref="ArgumentException">当密钥长度无效时抛出</exception>
    /// <remarks>
    /// 推荐使用2048位或更长的密钥以确保安全性
    /// </remarks>
    public static (string publicKey, string privateKey) GenerateKeyPair(int keySize = 2048)
    {
        return RsaCrypto.GenerateKeyPair(keySize);
    }

    /// <summary>
    /// 生成PEM格式的RSA密钥对用于混合加密
    /// </summary>
    /// <param name="keySize">密钥长度，默认2048位</param>
    /// <returns>包含PEM格式公钥和私钥的元组</returns>
    /// <exception cref="ArgumentException">当密钥长度无效时抛出</exception>
    /// <remarks>
    /// PEM格式更适合在不同系统间传输和存储
    /// </remarks>
    public static (string publicKeyPem, string privateKeyPem) GenerateKeyPairPem(int keySize = 2048)
    {
        return RsaCrypto.GenerateKeyPairPem(keySize);
    }

    /// <summary>
    /// 检测RSA密钥格式
    /// </summary>
    /// <param name="key">密钥字符串</param>
    /// <returns>密钥格式类型</returns>
    public static KeyFormat DetectKeyFormat(string key)
    {
        return RsaCrypto.DetectKeyFormat(key);
    }

    /// <summary>
    /// 获取混合加密数据的元信息
    /// </summary>
    /// <param name="encryptedData">加密的数据</param>
    /// <returns>包含元信息的元组</returns>
    /// <exception cref="ArgumentException">当数据为空时抛出</exception>
    /// <exception cref="FormatException">当数据格式无效时抛出</exception>
    public static (int encryptedKeyLength, int totalLength) GetEncryptedDataInfo(byte[] encryptedData)
    {
        if (encryptedData == null || encryptedData.Length == 0)
            throw new ArgumentException("加密数据不能为空", nameof(encryptedData));

#if NETSTANDARD
        if (encryptedData.Length < 4 + IvSize)
            throw new FormatException("加密数据格式无效");
#else
        if (encryptedData.Length < 4 + GcmNonceSize + GcmTagSize)
            throw new FormatException("加密数据格式无效");
#endif

        // 读取加密的AES密钥长度
        byte[] keyLengthBytes = new byte[4];
        Array.Copy(encryptedData, 0, keyLengthBytes, 0, 4);
        int keyLength = BitConverter.ToInt32(keyLengthBytes, 0);

        return (keyLength, encryptedData.Length);
    }

    /// <summary>
    /// 验证混合加密数据格式是否有效
    /// </summary>
    /// <param name="encryptedData">加密的数据</param>
    /// <returns>格式是否有效</returns>
    public static bool ValidateEncryptedDataFormat(byte[] encryptedData)
    {
        try
        {
            if (encryptedData == null || encryptedData.Length == 0)
                return false;

            // 读取加密的AES密钥长度
            if (encryptedData.Length < 4)
                return false;

            byte[] keyLengthBytes = new byte[4];
            Array.Copy(encryptedData, 0, keyLengthBytes, 0, 4);
            int keyLength = BitConverter.ToInt32(keyLengthBytes, 0);

#if NETSTANDARD
            if (encryptedData.Length < 4 + IvSize || keyLength <= 0)
                return false;

            // 验证各部分长度
            return keyLength > 0 &&
                   encryptedData.Length >= 4 + keyLength + IvSize;
#else
            if (encryptedData.Length < 4 + GcmNonceSize + GcmTagSize || keyLength <= 0)
                return false;

            // 验证各部分长度
            return keyLength > 0 && 
                   encryptedData.Length >= 4 + keyLength + GcmNonceSize + GcmTagSize;
#endif
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 估算加密后数据的大小
    /// </summary>
    /// <param name="originalDataSize">原始数据大小（字节）</param>
    /// <param name="rsaKeySize">RSA密钥长度（位），默认2048</param>
    /// <returns>估算的加密后数据大小</returns>
    /// <remarks>
    /// 估算公式：
    /// .NET Standard: 4字节(密钥长度) + RSA加密的AES密钥 + 16字节(IV) + 原始数据大小
    /// .NET: 4字节(密钥长度) + RSA加密的AES密钥 + 12字节(nonce) + 16字节(标签) + 原始数据大小
    /// </remarks>
    public static long EstimateEncryptedSize(long originalDataSize, int rsaKeySize = 2048)
    {
        // RSA加密的AES密钥大小 = RSA密钥长度 / 8
        int encryptedAesKeySize = rsaKeySize / 8;

#if NETSTANDARD
        // 总大小 = 密钥长度(4) + 加密的AES密钥 + IV(16) + 原始数据
        return 4 + encryptedAesKeySize + IvSize + originalDataSize;
#else
        // 总大小 = 密钥长度(4) + 加密的AES密钥 + nonce(12) + 标签(16) + 原始数据
        return 4 + encryptedAesKeySize + GcmNonceSize + GcmTagSize + originalDataSize;
#endif
    }

    /// <summary>
    /// 比较混合加密与纯RSA加密的性能优势
    /// </summary>
    /// <param name="dataSize">数据大小（字节）</param>
    /// <param name="rsaKeySize">RSA密钥长度（位），默认2048</param>
    /// <returns>性能比较信息</returns>
    /// <remarks>
    /// 这是一个理论估算，实际性能取决于具体实现和硬件
    /// </remarks>
    public static string ComparePerformanceWithRsa(long dataSize, int rsaKeySize = 2048)
    {
        // RSA最大加密长度（使用PKCS1填充）
        int rsaMaxChunkSize = (rsaKeySize / 8) - 11;

        // RSA需要分块的数量
        long rsaChunks = (dataSize + rsaMaxChunkSize - 1) / rsaMaxChunkSize;

        // RSA加密后的大小
        long rsaEncryptedSize = rsaChunks * (rsaKeySize / 8);

        // 混合加密的大小
        long hybridEncryptedSize = EstimateEncryptedSize(dataSize, rsaKeySize);

        StringBuilder sb = new();
        sb.AppendLine($"数据大小: {dataSize:N0} 字节");
        sb.AppendLine($"RSA密钥长度: {rsaKeySize} 位");
        sb.AppendLine($"平台模式: {(Environment.Version.Major >= 5 ? ".NET (AES-GCM)" : ".NET Standard (AES-CBC)")}");
        sb.AppendLine();
        sb.AppendLine("纯RSA加密:");
        sb.AppendLine($"  - 分块数量: {rsaChunks:N0}");
        sb.AppendLine($"  - 加密后大小: {rsaEncryptedSize:N0} 字节");
        sb.AppendLine($"  - 大小增长率: {(double)rsaEncryptedSize / dataSize * 100:F1}%");
        sb.AppendLine();
        sb.AppendLine("混合加密 (RSA+AES):");
        sb.AppendLine($"  - 加密后大小: {hybridEncryptedSize:N0} 字节");
        sb.AppendLine($"  - 大小增长率: {(double)hybridEncryptedSize / dataSize * 100:F1}%");
        sb.AppendLine($"  - 节省空间: {rsaEncryptedSize - hybridEncryptedSize:N0} 字节 ({(double)(rsaEncryptedSize - hybridEncryptedSize) / rsaEncryptedSize * 100:F1}%)");
        sb.AppendLine();
        sb.AppendLine("性能优势:");
        sb.AppendLine("  - 混合加密只需要1次RSA操作（加密AES密钥）");
        sb.AppendLine($"  - 纯RSA需要 {rsaChunks:N0} 次RSA操作");
        sb.AppendLine($"  - RSA操作减少: {rsaChunks - 1:N0} 次 ({(double)(rsaChunks - 1) / rsaChunks * 100:F1}%)");
#if NETSTANDARD
        sb.AppendLine("  - 使用AES-CBC模式（兼容性更好）");
#else
        sb.AppendLine("  - 使用AES-GCM模式（安全性更高，支持认证加密）");
#endif

        return sb.ToString();
    }

    /// <summary>
    /// 获取当前平台使用的AES模式
    /// </summary>
    /// <returns>AES模式描述</returns>
    public static string GetCurrentAesMode()
    {
#if NETSTANDARD
        return "AES-CBC (PKCS7填充)";
#else
        return "AES-GCM (认证加密)";
#endif
    }

    /// <summary>
    /// 获取加密数据格式的详细说明
    /// </summary>
    /// <returns>格式说明字符串</returns>
    public static string GetDataFormatDescription()
    {
        StringBuilder sb = new();
        sb.AppendLine("混合加密数据格式:");
        sb.AppendLine("[4字节] 加密的AES密钥长度");
        sb.AppendLine("[N字节] 加密的AES密钥（RSA加密）");

#if NETSTANDARD
        sb.AppendLine("[16字节] IV（初始化向量）");
        sb.AppendLine("[M字节] 加密数据（AES-CBC模式）");
        sb.AppendLine();
        sb.AppendLine("注意：.NET Standard版本使用AES-CBC模式，不包含认证标签");
#else
        sb.AppendLine("[12字节] nonce（随机数）");
        sb.AppendLine("[16字节] 认证标签（GCM tag）");
        sb.AppendLine("[M字节] 加密数据（AES-GCM模式）");
        sb.AppendLine();
        sb.AppendLine("注意：.NET版本使用AES-GCM模式，提供认证加密功能");
#endif

        return sb.ToString();
    }
    #endregion
}
