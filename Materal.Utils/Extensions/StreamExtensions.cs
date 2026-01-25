namespace Materal.Utils.Extensions;

/// <summary>
/// 流扩展
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// 是否是图片文件
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static bool IsImage(this Stream stream) => IsImage(stream, out string? _);
    /// <summary>
    /// 是否是图片文件
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="imageType"></param>
    /// <returns></returns>
    public static bool IsImage(this Stream stream, out string? imageType)
    {
        imageType = null;
        stream.Position = 0;
        try
        {
            using BinaryReader binaryReader = new(stream);
            Dictionary<string, List<byte[]>> signatures = [];
            signatures.Add("JPG|JPEG", [[0xFF, 0xD8, 0xFF, 0xE0], [0xFF, 0xD8, 0xFF, 0xE1], [0xFF, 0xD8, 0xFF, 0xE8]]);
            signatures.Add("PNG", [[0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]]);
            signatures.Add("GIF", [[0x47, 0x49, 0x46, 0x38, 0x37, 0x61], [0x47, 0x49, 0x46, 0x38, 0x39, 0x61]]);
            signatures.Add("BMP", [[0x42, 0x4D]]);
            signatures.Add("TIFF", [[0x49, 0x49, 0x2A, 0x00], [0x4D, 0x4D, 0x00, 0x2A]]);
            signatures.Add("ICO", [[0x00, 0x00, 0x01, 0x00], [0x00, 0x00, 0x02, 0x00]]);
            int maxLength = 0;
            foreach (KeyValuePair<string, List<byte[]>> item in signatures)
            {
                int temp = item.Value.Max(m => m.Length);
                if (temp > maxLength)
                {
                    maxLength = temp;
                }
            }
            if (maxLength > stream.Length)
            {
                maxLength = (int)stream.Length;
            }
            byte[] headerBytes = binaryReader.ReadBytes(maxLength);
            foreach (KeyValuePair<string, List<byte[]>> signature in signatures)
            {
                foreach (byte[] signatureItem in signature.Value)
                {
                    if (headerBytes.Length < signatureItem.Length) continue;
                    bool result = signatureItem.SequenceEqual(headerBytes.Take(signatureItem.Length));
                    if (result)
                    {
                        imageType = signature.Key;
                        return true;
                    }
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
