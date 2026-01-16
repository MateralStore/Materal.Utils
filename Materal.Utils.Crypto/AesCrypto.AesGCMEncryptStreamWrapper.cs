using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

public static partial class AesCrypto
{
    #region Aes-GCM 认证加密（AEAD）
#if NET
    /// <summary>
    /// Aes-GCM加密流包装器
    /// </summary>
    /// <remarks>
    /// 注意：这不是真正的GCM流式加密，而是一个包装器实现。
    /// 真正的GCM加密需要在知道所有数据后才能计算认证标签。
    /// </remarks>
    public class AesGCMEncryptStreamWrapper(Stream outputStream, byte[] key, byte[] nonce) : Stream
    {
        private readonly MemoryStream _buffer = new();
        private bool _disposed;

        /// <inheritdoc/>
        public override bool CanRead => false;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => true;

        /// <inheritdoc/>
        public override long Length => _buffer.Length;

        /// <inheritdoc/>
        public override long Position
        {
            get => _buffer.Position;
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            _buffer.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _buffer.Write(buffer, offset, count);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // 获取所有数据
                byte[] data = _buffer.ToArray();
                
                // 使用GCM加密
                byte[] ciphertext = new byte[data.Length];
                byte[] tag = new byte[AesGcmTagSize];
                using AesGcm aesGcm = new(key, AesGcmTagSize);
                aesGcm.Encrypt(nonce, data, ciphertext, tag);
                
                // 写入nonce、tag和ciphertext
                outputStream.Write(nonce, 0, nonce.Length);
                outputStream.Write(tag, 0, tag.Length);
                outputStream.Write(ciphertext, 0, ciphertext.Length);
                
                _buffer.Dispose();
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
#endif
    #endregion
}

