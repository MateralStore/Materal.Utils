using SkiaSharp;

namespace Materal.Utils.Image;

/// <summary>
/// 验证码预设样式选项
/// </summary>
public class CaptchaPresetOptions
{
    /// <summary>
    /// 预设名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 背景颜色
    /// </summary>
    public SKColor BackgroundColor { get; set; } = SKColors.White;
    /// <summary>
    /// 渐变背景起始颜色
    /// </summary>
    public SKColor? GradientBackgroundStartColor { get; set; }
    /// <summary>
    /// 渐变背景结束颜色
    /// </summary>
    public SKColor? GradientBackgroundEndColor { get; set; }
    /// <summary>
    /// 文字颜色
    /// </summary>
    public SKColor TextColor { get; set; } = SKColors.Black;
    /// <summary>
    /// 干扰线颜色
    /// </summary>
    public SKColor InterferenceLineColor { get; set; } = SKColors.LightGray;
    /// <summary>
    /// 噪点颜色
    /// </summary>
    public SKColor NoisePointColor { get; set; } = SKColors.LightGray;
    /// <summary>
    /// 转换为 CaptchaOptions
    /// </summary>
    public CaptchaOptions ToOptions()
    {
        return new CaptchaOptions
        {
            BackgroundColor = BackgroundColor,
            GradientBackgroundStartColor = GradientBackgroundStartColor,
            GradientBackgroundEndColor = GradientBackgroundEndColor,
            TextColor = TextColor,
            InterferenceLineColor = InterferenceLineColor,
            NoisePointColor = NoisePointColor
        };
    }
}
