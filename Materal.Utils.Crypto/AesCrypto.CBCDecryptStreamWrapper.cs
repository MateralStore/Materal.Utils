using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

public static partial class AesCrypto
{
    /// <summary>
    /// Aes-CBC解密流包装器，管理Aes和ICryptoTransform的生命周期
    /// </summary>
    /// <remarks>
    /// 此包装器确保在流关闭时正确释放加密相关资源，避免资源泄漏。
    /// 使用CBC模式和PKCS7填充进行解密。
    /// </remarks>
    public class CBCDecryptStreamWrapper : Stream
    {
        private readonly Aes _aes;
        private readonly ICryptoTransform _decryptor;
        private readonly CryptoStream _cryptoStream;
        private bool _disposed;

        /// <summary>
        /// 初始化Aes-CBC解密流包装器
        /// </summary>
        /// <param name="inputStream">要解密的输入流</param>
        /// <param name="keyBytes">AES密钥字节数组（16、24或32字节）</param>
        /// <param name="iv">初始化向量（16字节）</param>
        public CBCDecryptStreamWrapper(Stream inputStream, byte[] keyBytes, byte[] iv)
        {
            _aes = Aes.Create();
            _aes.Key = keyBytes;
            _aes.IV = iv;
            _aes.Mode = CipherMode.CBC;
            _aes.Padding = PaddingMode.PKCS7;
            _decryptor = _aes.CreateDecryptor();
#if NET
            _cryptoStream = new CryptoStream(inputStream, _decryptor, CryptoStreamMode.Read, leaveOpen: true);
#else
            _cryptoStream = new CryptoStream(inputStream, _decryptor, CryptoStreamMode.Read);
#endif
        }

        /// <summary>
        /// 获取一个值，该值指示当前流是否支持读取
        /// </summary>
        public override bool CanRead => _cryptoStream.CanRead;

        /// <summary>
        /// 获取一个值，该值指示当前流是否支持查找
        /// </summary>
        public override bool CanSeek => _cryptoStream.CanSeek;

        /// <summary>
        /// 获取一个值，该值指示当前流是否支持写入
        /// </summary>
        public override bool CanWrite => _cryptoStream.CanWrite;

        /// <summary>
        /// 获取流的长度（以字节为单位）
        /// </summary>
        public override long Length => _cryptoStream.Length;

        /// <summary>
        /// 获取或设置当前流中的位置
        /// </summary>
        public override long Position
        {
            get => _cryptoStream.Position;
            set => _cryptoStream.Position = value;
        }

        /// <summary>
        /// 清除此流的所有缓冲区，并使所有缓冲数据写入基础设备
        /// </summary>
        public override void Flush() => _cryptoStream.Flush();

        /// <summary>
        /// 从当前流读取字节序列，并将流中的位置提升读取的字节数
        /// </summary>
        /// <param name="buffer">字节数组。此方法返回时，该缓冲区包含指定的字节数组</param>
        /// <param name="offset">buffer中的从零开始的字节偏移量，从此处开始存储从当前流中读取的数据</param>
        /// <param name="count">要从当前流中最多读取的字节数</param>
        /// <returns>读入缓冲区中的总字节数</returns>
        public override int Read(byte[] buffer, int offset, int count) => _cryptoStream.Read(buffer, offset, count);

        /// <summary>
        /// 设置当前流中的位置
        /// </summary>
        /// <param name="offset">相对于origin参数的字节偏移量</param>
        /// <param name="origin">指示用于获取新位置的参考点</param>
        /// <returns>当前流中的新位置</returns>
        public override long Seek(long offset, SeekOrigin origin) => _cryptoStream.Seek(offset, origin);

        /// <summary>
        /// 设置当前流的长度
        /// </summary>
        /// <param name="value">所需的当前流的长度（以字节为单位）</param>
        public override void SetLength(long value) => _cryptoStream.SetLength(value);

        /// <summary>
        /// 向当前流中写入字节序列，并将此流中的当前位置提升写入的字节数
        /// </summary>
        /// <param name="buffer">字节数组。此方法将count个字节从buffer复制到当前流</param>
        /// <param name="offset">buffer中的从零开始的字节偏移量，从此处开始将字节复制到当前流</param>
        /// <param name="count">要写入当前流的字节数</param>
        public override void Write(byte[] buffer, int offset, int count) => _cryptoStream.Write(buffer, offset, count);

        /// <summary>
        /// 释放由Stream使用的非托管资源，并可选择释放托管资源
        /// </summary>
        /// <param name="disposing">如果为true，则释放托管资源和非托管资源；如果为false，则仅释放非托管资源</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _cryptoStream?.Dispose();
                _decryptor?.Dispose();
                _aes?.Dispose();
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}

