using System.Security.Cryptography;

namespace Materal.Utils.Crypto;

public static partial class AesCrypto
{
    /// <summary>
    /// Aes-CBC解密流包装器，管理Aes和ICryptoTransform的生命周期
    /// </summary>
    public class AesCBCDecryptStreamWrapper : Stream
    {
        private readonly Aes _aes;
        private readonly ICryptoTransform _decryptor;
        private readonly CryptoStream _cryptoStream;
        private bool _disposed;

        public AesCBCDecryptStreamWrapper(Stream inputStream, byte[] keyBytes, byte[] iv)
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

        public override bool CanRead => _cryptoStream.CanRead;
        public override bool CanSeek => _cryptoStream.CanSeek;
        public override bool CanWrite => _cryptoStream.CanWrite;
        public override long Length => _cryptoStream.Length;
        public override long Position
        {
            get => _cryptoStream.Position;
            set => _cryptoStream.Position = value;
        }

        public override void Flush() => _cryptoStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _cryptoStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _cryptoStream.Seek(offset, origin);
        public override void SetLength(long value) => _cryptoStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _cryptoStream.Write(buffer, offset, count);

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

