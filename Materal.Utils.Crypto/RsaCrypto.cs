using System.Security.Cryptography;
using System.Text;

namespace Materal.Utils.Crypto;

/// <summary>
/// RSA非对称加密解密工具类
/// </summary>
public static partial class RsaCrypto
{
    #region 密钥生成
    /// <summary>
    /// 生成RSA密钥对
    /// </summary>
    /// <param name="keySize">密钥长度，默认2048位</param>
    /// <returns>包含公钥和私钥的元组</returns>
    /// <exception cref="ArgumentException">当密钥长度无效时抛出</exception>
    public static (string publicKey, string privateKey) GenerateKeyPair(int keySize = 2048)
    {
        // 验证密钥长度必须是有效的RSA密钥长度
        int[] validKeySizes = [512, 1024, 2048, 4096, 8192, 16384];
        if (!validKeySizes.Contains(keySize))
            throw new ArgumentException("密钥长度必须为512、1024、2048、4096、8192或16384位", nameof(keySize));

        using RSACryptoServiceProvider rsa = new(keySize);

        string publicKey = rsa.ToXmlString(false);
        string privateKey = rsa.ToXmlString(true);

        return (publicKey, privateKey);
    }

    /// <summary>
    /// 从PEM格式字符串生成RSA密钥对
    /// </summary>
    /// <param name="publicKeyPem">PEM格式的公钥</param>
    /// <param name="privateKeyPem">PEM格式的私钥</param>
    /// <returns>包含公钥和私钥的元组</returns>
    public static (string publicKey, string privateKey) GenerateKeyPairFromPem(string publicKeyPem, string privateKeyPem)
    {
        if (string.IsNullOrEmpty(publicKeyPem))
            throw new ArgumentException("公钥不能为空", nameof(publicKeyPem));
        if (string.IsNullOrEmpty(privateKeyPem))
            throw new ArgumentException("私钥不能为空", nameof(privateKeyPem));

        return (publicKeyPem, privateKeyPem);
    }
    #endregion

    #region 字节数组加密解密
    /// <summary>
    /// RSA加密字节数组
    /// </summary>
    /// <param name="data">要加密的字节数组</param>
    /// <param name="publicKey">XML格式的公钥</param>
    /// <param name="padding">填充模式，默认为PKCS1</param>
    /// <returns>加密后的字节数组</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    /// <exception cref="InvalidOperationException">当数据过长时抛出</exception>
    public static byte[] Encrypt(byte[] data, string publicKey, RSAEncryptionPadding? padding = null)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("数据不能为空", nameof(data));
        if (string.IsNullOrEmpty(publicKey))
            throw new ArgumentException("公钥不能为空", nameof(publicKey));

        padding ??= RSAEncryptionPadding.Pkcs1;

        using RSACryptoServiceProvider rsa = new();
        rsa.FromXmlString(publicKey);

        // 计算最大加密长度
        int maxLength = GetMaxEncryptLength(rsa, padding);
        if (data.Length > maxLength)
            throw new InvalidOperationException($"数据长度({data.Length})超过最大加密长度({maxLength})");

        return rsa.Encrypt(data, padding);
    }

    /// <summary>
    /// RSA解密字节数组
    /// </summary>
    /// <param name="encryptedData">要解密的字节数组</param>
    /// <param name="privateKey">XML格式的私钥</param>
    /// <param name="padding">填充模式，默认为PKCS1</param>
    /// <returns>解密后的字节数组</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    public static byte[] Decrypt(byte[] encryptedData, string privateKey, RSAEncryptionPadding? padding = null)
    {
        if (encryptedData == null || encryptedData.Length == 0)
            throw new ArgumentException("加密数据不能为空", nameof(encryptedData));
        if (string.IsNullOrEmpty(privateKey))
            throw new ArgumentException("私钥不能为空", nameof(privateKey));

        padding ??= RSAEncryptionPadding.Pkcs1;

        using RSACryptoServiceProvider rsa = new();
        rsa.FromXmlString(privateKey);

        return rsa.Decrypt(encryptedData, padding);
    }

    /// <summary>
    /// RSA分块加密大数据
    /// </summary>
    /// <param name="data">要加密的字节数组</param>
    /// <param name="publicKey">XML格式的公钥</param>
    /// <param name="padding">填充模式，默认为PKCS1</param>
    /// <returns>加密后的字节数组</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    public static byte[] EncryptLargeData(byte[] data, string publicKey, RSAEncryptionPadding? padding = null)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("数据不能为空", nameof(data));
        if (string.IsNullOrEmpty(publicKey))
            throw new ArgumentException("公钥不能为空", nameof(publicKey));

        padding ??= RSAEncryptionPadding.Pkcs1;

        using RSACryptoServiceProvider rsa = new();
        rsa.FromXmlString(publicKey);

        int maxLength = GetMaxEncryptLength(rsa, padding);
        List<byte> result = [];

        for (int i = 0; i < data.Length; i += maxLength)
        {
            int chunkSize = Math.Min(maxLength, data.Length - i);
            byte[] chunk = new byte[chunkSize];
            Array.Copy(data, i, chunk, 0, chunkSize);

            byte[] encryptedChunk = rsa.Encrypt(chunk, padding);
            result.AddRange(encryptedChunk);
        }

        return [.. result];
    }

    /// <summary>
    /// RSA分块解密大数据
    /// </summary>
    /// <param name="encryptedData">要解密的字节数组</param>
    /// <param name="privateKey">XML格式的私钥</param>
    /// <param name="padding">填充模式，默认为PKCS1</param>
    /// <returns>解密后的字节数组</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    public static byte[] DecryptLargeData(byte[] encryptedData, string privateKey, RSAEncryptionPadding? padding = null)
    {
        if (encryptedData == null || encryptedData.Length == 0)
            throw new ArgumentException("加密数据不能为空", nameof(encryptedData));
        if (string.IsNullOrEmpty(privateKey))
            throw new ArgumentException("私钥不能为空", nameof(privateKey));

        padding ??= RSAEncryptionPadding.Pkcs1;

        using RSACryptoServiceProvider rsa = new();
        rsa.FromXmlString(privateKey);

        int encryptedChunkSize = rsa.KeySize / 8;
        List<byte> result = [];

        for (int i = 0; i < encryptedData.Length; i += encryptedChunkSize)
        {
            int chunkSize = Math.Min(encryptedChunkSize, encryptedData.Length - i);
            byte[] chunk = new byte[chunkSize];
            Array.Copy(encryptedData, i, chunk, 0, chunkSize);

            byte[] decryptedChunk = rsa.Decrypt(chunk, padding);
            result.AddRange(decryptedChunk);
        }

        return [.. result];
    }
    #endregion

    #region 字符串加密解密
    /// <summary>
    /// RSA加密字符串
    /// </summary>
    /// <param name="plainText">要加密的明文</param>
    /// <param name="publicKey">XML格式的公钥</param>
    /// <param name="padding">填充模式，默认为PKCS1</param>
    /// <returns>Base64编码的加密字符串</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    public static string Encrypt(string plainText, string publicKey, RSAEncryptionPadding? padding = null)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("明文不能为空", nameof(plainText));

        byte[] data = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedData = Encrypt(data, publicKey, padding);
        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// RSA解密字符串
    /// </summary>
    /// <param name="cipherText">Base64编码的加密字符串</param>
    /// <param name="privateKey">XML格式的私钥</param>
    /// <param name="padding">填充模式，默认为PKCS1</param>
    /// <returns>解密后的明文</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    public static string Decrypt(string cipherText, string privateKey, RSAEncryptionPadding? padding = null)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentException("密文不能为空", nameof(cipherText));
        if (string.IsNullOrEmpty(privateKey))
            throw new ArgumentException("私钥不能为空", nameof(privateKey));

        try
        {
            byte[] encryptedData = Convert.FromBase64String(cipherText);
            byte[] decryptedData = Decrypt(encryptedData, privateKey, padding);
            return Encoding.UTF8.GetString(decryptedData);
        }
        catch (FormatException)
        {
            throw new FormatException("密文格式无效，必须为有效的Base64字符串");
        }
    }

    /// <summary>
    /// RSA分块加密长字符串
    /// </summary>
    /// <param name="plainText">要加密的明文</param>
    /// <param name="publicKey">XML格式的公钥</param>
    /// <param name="padding">填充模式，默认为PKCS1</param>
    /// <returns>Base64编码的加密字符串</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">加密失败时抛出</exception>
    public static string EncryptLargeText(string plainText, string publicKey, RSAEncryptionPadding? padding = null)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("明文不能为空", nameof(plainText));

        byte[] data = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedData = EncryptLargeData(data, publicKey, padding);
        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// RSA分块解密长字符串
    /// </summary>
    /// <param name="cipherText">Base64编码的加密字符串</param>
    /// <param name="privateKey">XML格式的私钥</param>
    /// <param name="padding">填充模式，默认为PKCS1</param>
    /// <returns>解密后的明文</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">解密失败时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    public static string DecryptLargeText(string cipherText, string privateKey, RSAEncryptionPadding? padding = null)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentException("密文不能为空", nameof(cipherText));
        if (string.IsNullOrEmpty(privateKey))
            throw new ArgumentException("私钥不能为空", nameof(privateKey));

        try
        {
            byte[] encryptedData = Convert.FromBase64String(cipherText);
            byte[] decryptedData = DecryptLargeData(encryptedData, privateKey, padding);
            return Encoding.UTF8.GetString(decryptedData);
        }
        catch (FormatException)
        {
            throw new FormatException("密文格式无效，必须为有效的Base64字符串");
        }
    }
    #endregion

    #region 数字签名
    /// <summary>
    /// 使用私钥对数据进行数字签名
    /// </summary>
    /// <param name="data">要签名的字节数组</param>
    /// <param name="privateKey">XML格式的私钥</param>
    /// <param name="hashAlgorithm">哈希算法，默认为SHA256</param>
    /// <returns>数字签名字节数组</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">签名失败时抛出</exception>
    public static byte[] SignData(byte[] data, string privateKey, HashAlgorithmName hashAlgorithm = default)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("数据不能为空", nameof(data));
        if (string.IsNullOrEmpty(privateKey))
            throw new ArgumentException("私钥不能为空", nameof(privateKey));

        hashAlgorithm = hashAlgorithm == default ? HashAlgorithmName.SHA256 : hashAlgorithm;

        using RSACryptoServiceProvider rsa = new();
        rsa.FromXmlString(privateKey);

        return rsa.SignData(data, hashAlgorithm, RSASignaturePadding.Pkcs1);
    }

    /// <summary>
    /// 使用公钥验证数字签名
    /// </summary>
    /// <param name="data">原始数据字节数组</param>
    /// <param name="signature">数字签名字节数组</param>
    /// <param name="publicKey">XML格式的公钥</param>
    /// <param name="hashAlgorithm">哈希算法，默认为SHA256</param>
    /// <returns>验证是否成功</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    public static bool VerifyData(byte[] data, byte[] signature, string publicKey, HashAlgorithmName hashAlgorithm = default)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("数据不能为空", nameof(data));
        if (signature == null || signature.Length == 0)
            throw new ArgumentException("签名不能为空", nameof(signature));
        if (string.IsNullOrEmpty(publicKey))
            throw new ArgumentException("公钥不能为空", nameof(publicKey));

        hashAlgorithm = hashAlgorithm == default ? HashAlgorithmName.SHA256 : hashAlgorithm;

        using RSACryptoServiceProvider rsa = new();
        rsa.FromXmlString(publicKey);

        return rsa.VerifyData(data, signature, hashAlgorithm, RSASignaturePadding.Pkcs1);
    }

    /// <summary>
    /// 使用私钥对字符串进行数字签名
    /// </summary>
    /// <param name="plainText">要签名的明文</param>
    /// <param name="privateKey">XML格式的私钥</param>
    /// <param name="hashAlgorithm">哈希算法，默认为SHA256</param>
    /// <returns>Base64编码的数字签名</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="CryptographicException">签名失败时抛出</exception>
    public static string SignText(string plainText, string privateKey, HashAlgorithmName hashAlgorithm = default)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("明文不能为空", nameof(plainText));

        byte[] data = Encoding.UTF8.GetBytes(plainText);
        byte[] signature = SignData(data, privateKey, hashAlgorithm);
        return Convert.ToBase64String(signature);
    }

    /// <summary>
    /// 使用公钥验证字符串的数字签名
    /// </summary>
    /// <param name="plainText">原始明文</param>
    /// <param name="signature">Base64编码的数字签名</param>
    /// <param name="publicKey">XML格式的公钥</param>
    /// <param name="hashAlgorithm">哈希算法，默认为SHA256</param>
    /// <returns>验证是否成功</returns>
    /// <exception cref="ArgumentException">当参数为空时抛出</exception>
    /// <exception cref="FormatException">当Base64编码无效时抛出</exception>
    public static bool VerifyText(string plainText, string signature, string publicKey, HashAlgorithmName hashAlgorithm = default)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("明文不能为空", nameof(plainText));
        if (string.IsNullOrEmpty(signature))
            throw new ArgumentException("签名不能为空", nameof(signature));

        byte[] data = Encoding.UTF8.GetBytes(plainText);
        byte[] signatureBytes = Convert.FromBase64String(signature);
        return VerifyData(data, signatureBytes, publicKey, hashAlgorithm);
    }
    #endregion

    #region 辅助方法
    /// <summary>
    /// 获取RSA最大加密长度
    /// </summary>
    /// <param name="rsa">RSA实例</param>
    /// <param name="padding">填充模式</param>
    /// <returns>最大加密长度</returns>
    private static int GetMaxEncryptLength(RSACryptoServiceProvider rsa, RSAEncryptionPadding padding)
    {
        int keySize = rsa.KeySize;

        // 根据填充模式计算最大加密长度
        if (padding == RSAEncryptionPadding.Pkcs1)
        {
            return (keySize / 8) - 11; // PKCS1填充需要11字节的开销
        }
        else if (padding == RSAEncryptionPadding.OaepSHA256 ||
                 padding == RSAEncryptionPadding.OaepSHA384 ||
                 padding == RSAEncryptionPadding.OaepSHA512)
        {
            return (keySize / 8) - 42; // OAEP填充需要42字节的开销（SHA256）
        }
        else
        {
            return (keySize / 8) - 11; // 默认使用PKCS1的计算方式
        }
    }
    #endregion
}
