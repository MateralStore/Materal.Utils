namespace Materal.Utils.Crypto;

/// <summary>
/// 密钥格式枚举
/// </summary>
public enum KeyFormat
{
    /// <summary>
    /// 未知格式
    /// </summary>
    Unknown,
    
    /// <summary>
    /// XML格式
    /// </summary>
    Xml,
    
    /// <summary>
    /// PEM格式公钥
    /// </summary>
    PemPublic,
    
    /// <summary>
    /// PEM格式私钥
    /// </summary>
    PemPrivate
}
