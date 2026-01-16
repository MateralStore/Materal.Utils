using Materal.Utils.Crypto;
using System.Security.Cryptography;

namespace Materal.Utils.Test.CryptoTest;

/// <summary>
/// AES 字符串加密解密工具测试类
/// 测试 AES-CBC 和 AES-GCM 两种模式的字符串加密解密功能
/// </summary>
[TestClass]
public partial class AesStringCryptoTest
{
    static AesStringCryptoTest()
    {
        // 注册编码提供程序以支持GBK等编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    #region Aes-CBC 字符串加解密测试
    /// <summary>
    /// 测试Aes-CBC字符串加密解密（指定密钥和IV）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptString_Test()
    {
        // 准备测试数据
        string originalText = "这是一个需要加密的测试字符串123！@#";
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456")); // 16字节
        string iv = Convert.ToBase64String(Encoding.UTF8.GetBytes("abcdefghijklmnop")); // 16字节

        // 加密
        string encrypted = AesCrypto.AesCBCEncryptString(originalText, key, iv);

        // 验证加密结果不为空且不等于原文
        Assert.IsNotNull(encrypted, "加密结果不应为空");
        Assert.AreNotEqual(originalText, encrypted, "加密结果应不等于原文");

        // 解密
        string decrypted = AesCrypto.AesCBCDecryptString(encrypted, key, iv);

        // 验证解密结果与原文相同
        Assert.AreEqual(originalText, decrypted, "解密结果应与原文相同");
    }

    /// <summary>
    /// 测试Aes-CBC字符串加密解密（自动生成密钥和IV）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStringWithGeneratedKey_Test()
    {
        // 准备测试数据
        string originalText = "测试自动生成密钥的情况";

        // 加密（自动生成密钥和IV）
        string encrypted = AesCrypto.AesCBCEncryptString(originalText, out string key, out string iv);

        // 验证生成的密钥和IV不为空
        Assert.IsNotNull(key, "生成的密钥不应为空");
        Assert.IsNotNull(iv, "生成的IV不应为空");
        Assert.IsNotNull(encrypted, "加密结果不应为空");

        // 解密
        string decrypted = AesCrypto.AesCBCDecryptString(encrypted, key, iv);

        // 验证解密结果与原文相同
        Assert.AreEqual(originalText, decrypted, "解密结果应与原文相同");
    }

    /// <summary>
    /// 测试Aes-CBC字符串加密解密（使用随机IV）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStringWithRandomIV_Test()
    {
        // 准备测试数据
        string originalText = "测试使用随机IV的加密解密";
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("123456789012345678901234")); // 24字节

        // 加密（使用随机IV）
        string encrypted = AesCrypto.AesCBCEncryptString(originalText, key);

        // 验证加密结果
        Assert.IsNotNull(encrypted, "加密结果不应为空");
        Assert.AreNotEqual(originalText, encrypted, "加密结果应不等于原文");

        // 解密（自动提取IV）
        string decrypted = AesCrypto.AesCBCDecryptString(encrypted, key);

        // 验证解密结果与原文相同
        Assert.AreEqual(originalText, decrypted, "解密结果应与原文相同");
    }

    /// <summary>
    /// 测试Aes-CBC字符串加解密（不同编码格式）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStringWithEncoding_Test()
    {
        // 准备测试数据（包含中文）
        string originalText = "测试中文编码：你好世界！";
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456")); // 16字节
        string iv = Convert.ToBase64String(Encoding.UTF8.GetBytes("abcdefghijklmnop")); // 16字节

        // 使用GBK编码加密解密
        string encryptedGBK = AesCrypto.AesCBCEncryptString(originalText, key, iv, Encoding.GetEncoding("GBK"));
        string decryptedGBK = AesCrypto.AesCBCDecryptString(encryptedGBK, key, iv, Encoding.GetEncoding("GBK"));
        Assert.AreEqual(originalText, decryptedGBK, "GBK解密结果应与原文相同");

        // 使用UTF-8编码加密解密
        string encryptedUTF8 = AesCrypto.AesCBCEncryptString(originalText, key, iv, Encoding.UTF8);
        string decryptedUTF8 = AesCrypto.AesCBCDecryptString(encryptedUTF8, key, iv, Encoding.UTF8);
        Assert.AreEqual(originalText, decryptedUTF8);

        // 验证不同编码的加密结果不同
        Assert.AreNotEqual(encryptedGBK, encryptedUTF8, "不同编码的加密结果应不同");
    }

    /// <summary>
    /// 测试Aes-CBC字符串加密（空内容异常）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptStringEmptyContent_Test()
    {
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));
        string iv = Convert.ToBase64String(Encoding.UTF8.GetBytes("abcdefghijklmnop"));

        // 测试空字符串
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesCBCEncryptString("", key, iv), "空字符串应抛出异常");

        // 测试null
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesCBCEncryptString(null!, key, iv), "null值应抛出异常");
    }

    /// <summary>
    /// 测试Aes-CBC字符串解密（空内容异常）
    /// </summary>
    [TestMethod]
    public void AesCBCDecryptStringEmptyContent_Test()
    {
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));
        string iv = Convert.ToBase64String(Encoding.UTF8.GetBytes("abcdefghijklmnop"));

        // 测试空字符串
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesCBCDecryptString("", key, iv), "空字符串应抛出异常");

        // 测试null
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesCBCDecryptString(null!, key, iv), "null值应抛出异常");
    }

    /// <summary>
    /// 测试Aes-CBC字符串解密（无效Base64格式）
    /// </summary>
    [TestMethod]
    public void AesCBCDecryptStringInvalidBase64_Test()
    {
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));
        string iv = Convert.ToBase64String(Encoding.UTF8.GetBytes("abcdefghijklmnop"));

        // 测试无效的Base64字符串
        Assert.ThrowsExactly<FormatException>(() => AesCrypto.AesCBCDecryptString("InvalidBase64!!!", key, iv), "无效Base64应抛出异常");
    }

    /// <summary>
    /// 测试Aes-CBC字符串解密（错误的密钥或IV）
    /// </summary>
    [TestMethod]
    public void AesCBCDecryptStringWrongKeyOrIV_Test()
    {
        string originalText = "测试数据";
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));
        string iv = Convert.ToBase64String(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
        string wrongKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("6543210987654321"));
        string wrongIV = Convert.ToBase64String(Encoding.UTF8.GetBytes("ponmlkjihgfedcba"));

        // 正确加密
        string encrypted = AesCrypto.AesCBCEncryptString(originalText, key, iv);

        // 使用错误的密钥解密应该抛出异常
        Assert.ThrowsExactly<CryptographicException>(() => AesCrypto.AesCBCDecryptString(encrypted, wrongKey, iv), "使用错误密钥应抛出异常");

        // 使用错误的IV解密应该抛出异常
        Assert.ThrowsExactly<CryptographicException>(() => AesCrypto.AesCBCDecryptString(encrypted, key, wrongIV), "使用错误IV应抛出异常");
    }

    /// <summary>
    /// 测试Aes-CBC字符串加密解密（不同长度的密钥）
    /// </summary>
    [TestMethod]
    public void AesCBCEncryptDecryptStringDifferentKeyLengths_Test()
    {
        string originalText = "测试不同密钥长度";
        string iv = Convert.ToBase64String(Encoding.UTF8.GetBytes("abcdefghijklmnop"));

        // 测试16字节密钥（AES-128）
        string key128 = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));
        string encrypted128 = AesCrypto.AesCBCEncryptString(originalText, key128, iv);
        string decrypted128 = AesCrypto.AesCBCDecryptString(encrypted128, key128, iv);
        Assert.AreEqual(originalText, decrypted128);

        // 测试24字节密钥（AES-192）
        string key192 = Convert.ToBase64String(Encoding.UTF8.GetBytes("123456789012345678901234"));
        string encrypted192 = AesCrypto.AesCBCEncryptString(originalText, key192, iv);
        string decrypted192 = AesCrypto.AesCBCDecryptString(encrypted192, key192, iv);
        Assert.AreEqual(originalText, decrypted192);

        // 测试32字节密钥（AES-256）
        string key256 = Convert.ToBase64String(Encoding.UTF8.GetBytes("12345678901234567890123456789012"));
        string encrypted256 = AesCrypto.AesCBCEncryptString(originalText, key256, iv);
        string decrypted256 = AesCrypto.AesCBCDecryptString(encrypted256, key256, iv);
        Assert.AreEqual(originalText, decrypted256);

        // 验证不同密钥长度产生不同的加密结果
        Assert.AreNotEqual(encrypted128, encrypted192);
        Assert.AreNotEqual(encrypted192, encrypted256);
    }
    #endregion

#if NET
    #region Aes-GCM 字符串加解密测试
    /// <summary>
    /// 测试Aes-GCM字符串加密解密
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptString_Test()
    {
        // 准备测试数据
        string originalText = "这是一个需要GCM加密的测试字符串123！@#";
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456")); // 16字节

        // 加密
        string encrypted = AesCrypto.AesGCMEncryptString(originalText, key);

        // 验证加密结果
        Assert.IsNotNull(encrypted, "加密结果不应为空");
        Assert.AreNotEqual(originalText, encrypted, "加密结果应不等于原文");

        // 解密
        string decrypted = AesCrypto.AesGCMDecryptString(encrypted, key);

        // 验证解密结果与原文相同
        Assert.AreEqual(originalText, decrypted, "解密结果应与原文相同");
    }

    /// <summary>
    /// 测试Aes-GCM字符串加密解密（自动生成密钥和nonce）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStringWithGeneratedKey_Test()
    {
        // 准备测试数据
        string originalText = "测试GCM自动生成密钥的情况";

        // 加密（自动生成密钥和nonce）
        string encrypted = AesCrypto.AesGCMEncryptString(originalText, out string key, out string nonce);

        // 验证生成的密钥和nonce不为空
        Assert.IsNotNull(key, "生成的密钥不应为空");
        Assert.IsNotNull(nonce, "生成的nonce不应为空");
        Assert.IsNotNull(encrypted, "加密结果不应为空");

        // 解密（使用单独的nonce）
        string decrypted = AesCrypto.AesGCMDecryptString(encrypted, key, nonce);

        // 验证解密结果与原文相同
        Assert.AreEqual(originalText, decrypted, "解密结果应与原文相同");
    }

    /// <summary>
    /// 测试Aes-GCM字符串加密解密（不同编码格式）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStringWithEncoding_Test()
    {
        // 准备测试数据
        string originalText = "测试GCM中文编码：你好世界！";
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456")); // 16字节

        // 使用UTF-8编码加密解密
        string encryptedUTF8 = AesCrypto.AesGCMEncryptString(originalText, key, Encoding.UTF8);
        string decryptedUTF8 = AesCrypto.AesGCMDecryptString(encryptedUTF8, key, Encoding.UTF8);
        Assert.AreEqual(originalText, decryptedUTF8);

        // 使用Unicode编码加密解密
        string encryptedUnicode = AesCrypto.AesGCMEncryptString(originalText, key, Encoding.Unicode);
        string decryptedUnicode = AesCrypto.AesGCMDecryptString(encryptedUnicode, key, Encoding.Unicode);
        Assert.AreEqual(originalText, decryptedUnicode);

        // 验证不同编码的加密结果不同
        Assert.AreNotEqual(encryptedUTF8, encryptedUnicode);
    }

    /// <summary>
    /// 测试Aes-GCM字符串加密（空内容异常）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptStringEmptyContent_Test()
    {
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));

        // 测试空字符串
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesGCMEncryptString("", key), "空字符串应抛出异常");

        // 测试null
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesGCMEncryptString(null!, key), "null值应抛出异常");
    }

    /// <summary>
    /// 测试Aes-GCM字符串解密（空内容异常）
    /// </summary>
    [TestMethod]
    public void AesGCMDecryptStringEmptyContent_Test()
    {
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));

        // 测试空字符串
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesGCMDecryptString("", key), "空字符串应抛出异常");

        // 测试null
        Assert.ThrowsExactly<ArgumentException>(() => AesCrypto.AesGCMDecryptString(null!, key), "null值应抛出异常");
    }

    /// <summary>
    /// 测试Aes-GCM字符串解密（无效Base64格式）
    /// </summary>
    [TestMethod]
    public void AesGCMDecryptStringInvalidBase64_Test()
    {
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));

        // 测试无效的Base64字符串
        Assert.ThrowsExactly<FormatException>(() => AesCrypto.AesGCMDecryptString("InvalidBase64!!!", key), "无效Base64应抛出异常");
    }

    /// <summary>
    /// 测试Aes-GCM字符串解密（错误的密钥）
    /// </summary>
    [TestMethod]
    public void AesGCMDecryptStringWrongKey_Test()
    {
        string originalText = "测试GCM数据";
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));
        string wrongKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("6543210987654321"));

        // 正确加密
        string encrypted = AesCrypto.AesGCMEncryptString(originalText, key);

        // 使用错误的密钥解密应该抛出异常
        Assert.ThrowsExactly<AuthenticationTagMismatchException>(() => AesCrypto.AesGCMDecryptString(encrypted, wrongKey));
    }

    /// <summary>
    /// 测试Aes-GCM字符串解密（数据篡改）
    /// </summary>
    [TestMethod]
    public void AesGCMDecryptStringTamperedData_Test()
    {
        string originalText = "测试数据完整性";
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));

        // 正确加密
        string encrypted = AesCrypto.AesGCMEncryptString(originalText, key);

        // 篡改加密数据（修改Base64字符串的一个字符）
        byte[] encryptedBytes = Convert.FromBase64String(encrypted);
        encryptedBytes[^1] ^= 0x01; // 翻转最后一个字节的一个位
        string tamperedEncrypted = Convert.ToBase64String(encryptedBytes);

        // 解密被篡改的数据应该抛出异常
        Assert.ThrowsExactly<AuthenticationTagMismatchException>(() => AesCrypto.AesGCMDecryptString(tamperedEncrypted, key));
    }

    /// <summary>
    /// 测试Aes-GCM字符串加密解密（不同长度的密钥）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStringDifferentKeyLengths_Test()
    {
        string originalText = "测试GCM不同密钥长度";

        // 测试16字节密钥（AES-128）
        string key128 = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));
        string encrypted128 = AesCrypto.AesGCMEncryptString(originalText, key128);
        string decrypted128 = AesCrypto.AesGCMDecryptString(encrypted128, key128);
        Assert.AreEqual(originalText, decrypted128);

        // 测试24字节密钥（AES-192）
        string key192 = Convert.ToBase64String(Encoding.UTF8.GetBytes("123456789012345678901234"));
        string encrypted192 = AesCrypto.AesGCMEncryptString(originalText, key192);
        string decrypted192 = AesCrypto.AesGCMDecryptString(encrypted192, key192);
        Assert.AreEqual(originalText, decrypted192);

        // 测试32字节密钥（AES-256）
        string key256 = Convert.ToBase64String(Encoding.UTF8.GetBytes("12345678901234567890123456789012"));
        string encrypted256 = AesCrypto.AesGCMEncryptString(originalText, key256);
        string decrypted256 = AesCrypto.AesGCMDecryptString(encrypted256, key256);
        Assert.AreEqual(originalText, decrypted256);

        // 验证不同密钥长度产生不同的加密结果
        Assert.AreNotEqual(encrypted128, encrypted192);
        Assert.AreNotEqual(encrypted192, encrypted256);
    }

    /// <summary>
    /// 测试Aes-GCM字符串加密解密（相同的密钥但不同的nonce）
    /// </summary>
    [TestMethod]
    public void AesGCMEncryptDecryptStringSameKeyDifferentNonce_Test()
    {
        string originalText = "测试相同密钥不同nonce";
        string key = Convert.ToBase64String(Encoding.UTF8.GetBytes("1234567890123456"));

        // 使用相同密钥加密两次，应该产生不同的结果（因为nonce不同）
        string encrypted1 = AesCrypto.AesGCMEncryptString(originalText, key);
        string encrypted2 = AesCrypto.AesGCMEncryptString(originalText, key);

        // 加密结果应该不同
        Assert.AreNotEqual(encrypted1, encrypted2);

        // 但解密结果应该相同
        string decrypted1 = AesCrypto.AesGCMDecryptString(encrypted1, key);
        string decrypted2 = AesCrypto.AesGCMDecryptString(encrypted2, key);
        Assert.AreEqual(originalText, decrypted1);
        Assert.AreEqual(originalText, decrypted2);
    }
    #endregion
#endif
}