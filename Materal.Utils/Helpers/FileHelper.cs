namespace Materal.Utils.Helpers;

/// <summary>
/// 文件帮助类
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// 判断是否是图片文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="imageTyp">图片类型</param>
    /// <returns>是图片文件返回true，否则返回false</returns>
    public static bool IsImageFile(string filePath, out string? imageTyp) => new FileInfo(filePath).IsImageFile(out imageTyp);

    /// <summary>
    /// 判断是否是图片文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是图片文件返回true，否则返回false</returns>
    public static bool IsImageFile(string filePath) => new FileInfo(filePath).IsImageFile();
}
