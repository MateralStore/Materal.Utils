namespace Materal.Utils.Extensions;

/// <summary>
/// 文件信息扩展
/// </summary>
public static class FileInfoExtensions
{
    /// <summary>
    /// 判断文件是否为图片文件
    /// </summary>
    /// <param name="fileInfo">文件信息对象</param>
    /// <param name="imageType">图片类型</param>
    /// <returns>是图片文件返回true，否则返回false</returns>
    /// <exception cref="FileNotFoundException">文件不存在时抛出异常</exception>
    public static bool IsImageFile(this FileInfo fileInfo, out string? imageType)
    {
        using FileStream fileStream = OpenFile(fileInfo);
        return fileStream.IsImage(out imageType);
    }

    /// <summary>
    /// 判断文件是否为图片文件
    /// </summary>
    /// <param name="fileInfo">文件信息对象</param>
    /// <returns>是图片文件返回true，否则返回false</returns>
    /// <exception cref="FileNotFoundException">文件不存在时抛出异常</exception>
    public static bool IsImageFile(this FileInfo fileInfo)
    {
        using FileStream fileStream = OpenFile(fileInfo);
        return fileStream.IsImage();
    }

    /// <summary>
    /// 打开文件并返回文件流
    /// </summary>
    /// <param name="fileInfo">文件信息对象</param>
    /// <returns>文件流</returns>
    /// <exception cref="FileNotFoundException">文件不存在时抛出异常</exception>
    private static FileStream OpenFile(FileInfo fileInfo)
    {
        if (!fileInfo.Exists) throw new FileNotFoundException("文件不存在", fileInfo.FullName);
        return fileInfo.OpenRead();
    }
}
