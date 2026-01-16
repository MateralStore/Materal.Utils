using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

public static partial class AesCrypto
{
    #region Aes-GCM 认证加密（AEAD）
#if NET

    /// <summary>
    /// Aes-GCM解密流包装器
    /// </summary>
    /// <remarks>
    /// 注意：这不是真正的GCM流式解密，而是一个包装器实现。
    /// 真正的GCM解密需要读取所有数据后才能验证认证标签。
    /// </remarks>
    public class AesGCMDecryptStreamWrapper(Stream inputStream, byte[] key) : Stream
    {
        private byte[] _nonce = null!;
        private byte[] _tag = null!;
        private byte[] _ciphertext = null!;
        private MemoryStream _decryptedStream = null!;
        private bool _initialized;
        private bool _disposed;

        private void Initialize()
        {
            if (_initialized) return;

            // 读取nonce
            _nonce = new byte[AesGcmNonceSize];
            int bytesRead = inputStream.Read(_nonce, 0, _nonce.Length);
            if (bytesRead != _nonce.Length) throw new ArgumentException("无法读取完整的nonce");

            // 读取tag
            _tag = new byte[AesGcmTagSize];
            bytesRead = inputStream.Read(_tag, 0, _tag.Length);
            if (bytesRead != _tag.Length) throw new ArgumentException("无法读取完整的认证标签");

            // 读取剩余的ciphertext
            using MemoryStream tempStream = new();
            inputStream.CopyTo(tempStream);
            _ciphertext = tempStream.ToArray();

            // 解密
            byte[] plaintext = new byte[_ciphertext.Length];
            using AesGcm aesGcm = new(key, AesGcmTagSize);
            aesGcm.Decrypt(_nonce, _ciphertext, _tag, plaintext);

            _decryptedStream = new MemoryStream(plaintext);
            _initialized = true;
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => true;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                Initialize();
                return _decryptedStream.Length;
            }
        }

        /// <inheritdoc/>
        public override long Position
        {
            get
            {
                Initialize();
                return _decryptedStream.Position;
            }
            set
            {
                Initialize();
                _decryptedStream.Position = value;
            }
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            Initialize();
            _decryptedStream.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            Initialize();
            return _decryptedStream.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            Initialize();
            return _decryptedStream.Seek(offset, origin);
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _decryptedStream?.Dispose();
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
#endif
    #endregion
}

