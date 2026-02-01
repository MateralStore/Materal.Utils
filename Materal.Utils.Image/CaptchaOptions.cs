using SkiaSharp;

namespace Materal.Utils.Image;

/// <summary>
/// 验证码绘制选项
/// </summary>
public class CaptchaOptions
{
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
    /// 渐变背景方向（0=水平, 1=垂直）
    /// </summary>
    public int GradientDirection { get; set; } = 1;
    /// <summary>
    /// 文字颜色
    /// </summary>
    public SKColor TextColor { get; set; } = SKColors.Black;
    /// <summary>
    /// 是否启用字符颜色差异
    /// </summary>
    public bool EnableCharColorVariation { get; set; } = true;
    /// <summary>
    /// 字符颜色变化范围（RGB偏移）
    /// </summary>
    public int CharColorVariationRange { get; set; } = 50;
    /// <summary>
    /// 是否启用文字阴影
    /// </summary>
    public bool EnableTextShadow { get; set; } = true;
    /// <summary>
    /// 阴影颜色
    /// </summary>
    public SKColor ShadowColor { get; set; } = new SKColor(128, 128, 128, 128);
    /// <summary>
    /// 阴影偏移X
    /// </summary>
    public float ShadowOffsetX { get; set; } = 2;
    /// <summary>
    /// 阴影偏移Y
    /// </summary>
    public float ShadowOffsetY { get; set; } = 2;
    /// <summary>
    /// 字体家族
    /// </summary>
    public string? FontFamily { get; set; }
    /// <summary>
    /// 字体大小
    /// </summary>
    public float FontSize { get; set; }
    /// <summary>
    /// 字符最大旋转角度（正负值）
    /// </summary>
    public float MaxCharRotation { get; set; } = 25;
    /// <summary>
    /// 字符最大偏移量（上下左右）
    /// </summary>
    public float MaxCharOffset { get; set; } = 5;
    /// <summary>
    /// 是否绘制干扰线
    /// </summary>
    public bool EnableInterferenceLines { get; set; } = true;
    /// <summary>
    /// 干扰线颜色
    /// </summary>
    public SKColor InterferenceLineColor { get; set; } = SKColors.LightGray;
    /// <summary>
    /// 干扰线宽度
    /// </summary>
    public float InterferenceLineWidth { get; set; } = 1;
    /// <summary>
    /// 干扰线数量
    /// </summary>
    public int InterferenceLineCount { get; set; } = 3;
    /// <summary>
    /// 是否启用曲线干扰线
    /// </summary>
    public bool EnableCurvedLines { get; set; } = true;
    /// <summary>
    /// 曲线干扰线数量
    /// </summary>
    public int CurvedLineCount { get; set; } = 2;
    /// <summary>
    /// 曲线幅度
    /// </summary>
    public float CurveAmplitude { get; set; } = 10;
    /// <summary>
    /// 曲线频率
    /// </summary>
    public float CurveFrequency { get; set; } = 2;
    /// <summary>
    /// 是否启用渐变干扰线
    /// </summary>
    public bool EnableGradientLines { get; set; } = true;
    /// <summary>
    /// 渐变干扰线数量
    /// </summary>
    public int GradientLineCount { get; set; } = 2;
    /// <summary>
    /// 是否绘制网格线
    /// </summary>
    public bool EnableGridLines { get; set; }
    /// <summary>
    /// 网格线颜色
    /// </summary>
    public SKColor GridLineColor { get; set; } = new SKColor(200, 200, 200, 50);
    /// <summary>
    /// 网格线间距
    /// </summary>
    public int GridSpacing { get; set; } = 20;
    /// <summary>
    /// 是否绘制噪点
    /// </summary>
    public bool EnableNoisePoints { get; set; } = true;
    /// <summary>
    /// 噪点颜色
    /// </summary>
    public SKColor NoisePointColor { get; set; } = SKColors.LightGray;
    /// <summary>
    /// 噪点数量
    /// </summary>
    public int NoisePointCount { get; set; } = 100;
    /// <summary>
    /// 是否启用特殊噪点（矩形、线条等）
    /// </summary>
    public bool EnableSpecialNoise { get; set; } = true;
    /// <summary>
    /// 特殊噪点数量
    /// </summary>
    public int SpecialNoiseCount { get; set; } = 20;
    /// <summary>
    /// 是否启用图片波浪扭曲
    /// </summary>
    public bool EnableWaveDistortion { get; set; } = true;
    /// <summary>
    /// 波浪幅度X
    /// </summary>
    public float WaveAmplitudeX { get; set; } = 3;
    /// <summary>
    /// 波浪幅度Y
    /// </summary>
    public float WaveAmplitudeY { get; set; } = 5;
    /// <summary>
    /// 波浪频率X
    /// </summary>
    public float WaveFrequencyX { get; set; } = 10;
    /// <summary>
    /// 波浪频率Y
    /// </summary>
    public float WaveFrequencyY { get; set; } = 5;
}
