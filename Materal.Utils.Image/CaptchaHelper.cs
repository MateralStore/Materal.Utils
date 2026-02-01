#if NET
using SkiaSharp;

namespace Materal.Utils.Image;

/// <summary>
/// 验证码帮助类
/// </summary>
public static class CaptchaHelper
{
    private static readonly CaptchaPresetOptions[] DefaultPresets;

    static CaptchaHelper()
    {
        DefaultPresets =
        [
            new CaptchaPresetOptions
            {
                Name = "经典白底黑字",
                BackgroundColor = SKColors.White,
                TextColor = SKColors.Black,
                InterferenceLineColor = SKColors.LightGray,
                NoisePointColor = SKColors.LightGray
            },
            new CaptchaPresetOptions
            {
                Name = "浅蓝背景",
                BackgroundColor = new SKColor(240, 248, 255),
                TextColor = SKColors.DarkBlue,
                InterferenceLineColor = SKColors.LightSteelBlue,
                NoisePointColor = SKColors.LightSteelBlue
            },
            new CaptchaPresetOptions
            {
                Name = "浅黄背景",
                BackgroundColor = new SKColor(255, 250, 240),
                TextColor = SKColors.DarkRed,
                InterferenceLineColor = SKColors.Gold,
                NoisePointColor = SKColors.Gold
            },
            new CaptchaPresetOptions
            {
                Name = "浅绿背景",
                BackgroundColor = new SKColor(240, 255, 240),
                TextColor = SKColors.DarkGreen,
                InterferenceLineColor = SKColors.LightGreen,
                NoisePointColor = SKColors.LightGreen
            },
            new CaptchaPresetOptions
            {
                Name = "浅粉背景",
                BackgroundColor = new SKColor(255, 240, 245),
                TextColor = SKColors.DarkMagenta,
                InterferenceLineColor = SKColors.Plum,
                NoisePointColor = SKColors.Plum
            },
            new CaptchaPresetOptions
            {
                Name = "渐变蓝紫",
                GradientBackgroundStartColor = new SKColor(240, 248, 255),
                GradientBackgroundEndColor = new SKColor(230, 220, 255),
                TextColor = SKColors.DarkSlateBlue,
                InterferenceLineColor = SKColors.MediumPurple,
                NoisePointColor = SKColors.MediumPurple
            },
            new CaptchaPresetOptions
            {
                Name = "渐变橙红",
                GradientBackgroundStartColor = new SKColor(255, 250, 240),
                GradientBackgroundEndColor = new SKColor(255, 230, 230),
                TextColor = SKColors.DarkRed,
                InterferenceLineColor = SKColors.Salmon,
                NoisePointColor = SKColors.Salmon
            },
            new CaptchaPresetOptions
            {
                Name = "渐变青绿",
                GradientBackgroundStartColor = new SKColor(240, 255, 240),
                GradientBackgroundEndColor = new SKColor(220, 255, 250),
                TextColor = SKColors.Teal,
                InterferenceLineColor = SKColors.MediumTurquoise,
                NoisePointColor = SKColors.MediumTurquoise
            }
        ];
    }

    /// <summary>
    /// 获取所有预设样式
    /// </summary>
    public static IReadOnlyList<CaptchaPresetOptions> Presets => DefaultPresets;

    /// <summary>
    /// 随机获取一个预设样式
    /// </summary>
    public static CaptchaPresetOptions GetRandomPreset()
    {
        Random random = new();
        return DefaultPresets[random.Next(DefaultPresets.Length)];
    }

    /// <summary>
    /// 绘制验证码图片
    /// </summary>
    /// <param name="captchaText">验证码文本</param>
    /// <param name="width">图片宽度</param>
    /// <param name="height">图片高度</param>
    /// <param name="options">绘制选项，传入 null 时随机选择预设样式</param>
    /// <returns></returns>
    public static SKImage Draw(string captchaText, int width, int height, CaptchaOptions? options = null)
    {
        options ??= GetRandomPreset().ToOptions();
        using SKBitmap bitmap = new(width, height);
        using SKCanvas canvas = new(bitmap);
        DrawBackground(canvas, width, height, options);
        if (options.EnableGridLines)
        {
            DrawGridLines(canvas, width, height, options);
        }
        if (options.EnableInterferenceLines)
        {
            DrawInterferenceLines(canvas, width, height, options);
        }
        if (options.EnableCurvedLines)
        {
            DrawCurvedLines(canvas, width, height, options);
        }
        if (options.EnableGradientLines)
        {
            DrawGradientLines(canvas, width, height, options);
        }
        DrawText(canvas, captchaText, width, height, options);
        if (options.EnableNoisePoints)
        {
            DrawNoisePoints(canvas, width, height, options);
        }
        if (options.EnableSpecialNoise)
        {
            DrawSpecialNoise(canvas, width, height, options);
        }
        SKImage image = SKImage.FromBitmap(bitmap);
        if (options.EnableWaveDistortion)
        {
            image = ApplyWaveDistortion(image, options);
        }
        return image;
    }
    /// <summary>
    /// 绘制验证码图片并保存
    /// </summary>
    /// <param name="captchaText">验证码文本</param>
    /// <param name="savePath">保存路径</param>
    /// <param name="width">图片宽度</param>
    /// <param name="height">图片高度</param>
    /// <param name="options">绘制选项</param>
    public static void Draw(string captchaText, string savePath, int width, int height, CaptchaOptions? options = null)
    {
        using SKImage image = Draw(captchaText, width, height, options);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        SKBitmap tempBitmap = SKBitmap.Decode(data.ToArray());
        tempBitmap.SaveAs(savePath);
    }
    /// <summary>
    /// 绘制验证码图片并转换为Stream
    /// </summary>
    /// <param name="captchaText">验证码文本</param>
    /// <param name="width">图片宽度</param>
    /// <param name="height">图片高度</param>
    /// <param name="options">绘制选项</param>
    /// <returns></returns>
    public static Stream DrawToStream(string captchaText, int width, int height, CaptchaOptions? options = null)
    {
        using SKImage image = Draw(captchaText, width, height, options);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        MemoryStream stream = new(data.ToArray());
        return stream;
    }
    /// <summary>
    /// 绘制验证码图片并转换为Base64
    /// </summary>
    /// <param name="captchaText">验证码文本</param>
    /// <param name="width">图片宽度</param>
    /// <param name="height">图片高度</param>
    /// <param name="options">绘制选项</param>
    /// <returns></returns>
    public static string DrawToBase64(string captchaText, int width, int height, CaptchaOptions? options = null)
    {
        using SKImage image = Draw(captchaText, width, height, options);
        return image.GetBase64Image();
    }
    #region 私有方法
    private static void DrawBackground(SKCanvas canvas, int width, int height, CaptchaOptions options)
    {
        if (options.GradientBackgroundStartColor.HasValue && options.GradientBackgroundEndColor.HasValue)
        {
            using SKPaint brush = new()
            {
                Style = SKPaintStyle.Fill
            };
            SKColor startColor = options.GradientBackgroundStartColor.Value;
            SKColor endColor = options.GradientBackgroundEndColor.Value;
            brush.Shader = SKShader.CreateLinearGradient(
                new SKPoint(0, 0),
                options.GradientDirection == 0 ? new SKPoint(width, 0) : new SKPoint(0, height),
                [startColor, endColor],
                null,
                SKShaderTileMode.Clamp
            );
            canvas.DrawRect(0, 0, width, height, brush);
        }
        else
        {
            using SKPaint brush = new()
            {
                Color = options.BackgroundColor,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRect(0, 0, width, height, brush);
        }
    }
    private static void DrawGridLines(SKCanvas canvas, int width, int height, CaptchaOptions options)
    {
        Random random = new();
        using SKPaint linePaint = new()
        {
            Color = options.GridLineColor,
            StrokeWidth = 1,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };
        int spacing = options.GridSpacing > 0 ? options.GridSpacing : 20;
        for (int x = 0; x <= width; x += spacing)
        {
            float alpha = (float)random.NextDouble() * 0.3f + 0.1f;
            linePaint.Color = new SKColor(options.GridLineColor.Red, options.GridLineColor.Green, options.GridLineColor.Blue, (byte)(alpha * 255));
            canvas.DrawLine(x, 0, x, height, linePaint);
        }
        for (int y = 0; y <= height; y += spacing)
        {
            float alpha = (float)random.NextDouble() * 0.3f + 0.1f;
            linePaint.Color = new SKColor(options.GridLineColor.Red, options.GridLineColor.Green, options.GridLineColor.Blue, (byte)(alpha * 255));
            canvas.DrawLine(0, y, width, y, linePaint);
        }
    }
    private static void DrawText(SKCanvas canvas, string captchaText, int width, int height, CaptchaOptions options)
    {
        if (string.IsNullOrEmpty(captchaText)) return;
        float fontSize = options.FontSize > 0 ? options.FontSize : height * 0.6f;
        using SKTypeface typeface = GetTypeface(options.FontFamily);
        using SKFont font = new(typeface, fontSize);
        Random random = new();
        font.MeasureText(captchaText, out SKRect textBounds);
        float totalWidth = textBounds.Width;
        float charSpacing = totalWidth / captchaText.Length;
        float startX = (width - totalWidth) / 2 - textBounds.Left;
        float centerY = height / 2f;
        float maxRotation = options.MaxCharRotation;
        float maxOffset = options.MaxCharOffset;
        for (int i = 0; i < captchaText.Length; i++)
        {
            char currentChar = captchaText[i];
            float charX = startX;
            for (int j = 0; j < i; j++)
            {
                font.MeasureText(captchaText[j].ToString(), out SKRect charBounds);
                charX += charBounds.Width;
            }
            charX += charSpacing / 2;
            float rotation = (float)(random.NextDouble() * 2 - 1) * maxRotation;
            float offsetX = (float)(random.NextDouble() * 2 - 1) * maxOffset;
            float offsetY = (float)(random.NextDouble() * 2 - 1) * maxOffset;
            SKColor charColor = options.TextColor;
            if (options.EnableCharColorVariation)
            {
                int variation = options.CharColorVariationRange;
                int r = Clamp(options.TextColor.Red + random.Next(-variation, variation + 1), 0, 255);
                int g = Clamp(options.TextColor.Green + random.Next(-variation, variation + 1), 0, 255);
                int b = Clamp(options.TextColor.Blue + random.Next(-variation, variation + 1), 0, 255);
                charColor = new SKColor((byte)r, (byte)g, (byte)b);
            }
            canvas.Save();
            canvas.Translate(charX + offsetX, centerY + offsetY);
            canvas.RotateDegrees(rotation);
            canvas.Translate(-(charX + offsetX), -(centerY + offsetY));
            if (options.EnableTextShadow)
            {
                using SKPaint shadowPaint = new()
                {
                    Color = options.ShadowColor,
                    IsAntialias = true
                };
                canvas.DrawText(currentChar.ToString(), charX + options.ShadowOffsetX, centerY + fontSize / 3f + options.ShadowOffsetY, SKTextAlign.Center, font, shadowPaint);
            }
            using SKPaint textPaint = new()
            {
                Color = charColor,
                IsAntialias = true
            };
            canvas.DrawText(currentChar.ToString(), charX, centerY + fontSize / 3f, SKTextAlign.Center, font, textPaint);
            canvas.Restore();
        }
    }
    private static SKTypeface GetTypeface(string? fontFamily)
    {
        if (!string.IsNullOrEmpty(fontFamily))
        {
            SKTypeface? typeface = SKTypeface.FromFamilyName(fontFamily);
            if (typeface != null) return typeface;
        }
        string[] chineseFonts = ["Microsoft YaHei", "SimSun", "SimHei", "PingFang SC", "Heiti SC", "WenQuanYi Micro Hei"];
        foreach (string font in chineseFonts)
        {
            SKTypeface? typeface = SKTypeface.FromFamilyName(font);
            if (typeface != null) return typeface;
        }
        return SKTypeface.FromFamilyName("Arial") ?? SKTypeface.Default;
    }
    private static void DrawInterferenceLines(SKCanvas canvas, int width, int height, CaptchaOptions options)
    {
        Random random = new();
        int lineCount = options.InterferenceLineCount > 0 ? options.InterferenceLineCount : 3;
        for (int i = 0; i < lineCount; i++)
        {
            using SKPaint linePaint = new()
            {
                Color = options.InterferenceLineColor,
                StrokeWidth = options.InterferenceLineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };
            int startX = random.Next(width);
            int startY = random.Next(height);
            int endX = random.Next(width);
            int endY = random.Next(height);
            canvas.DrawLine(startX, startY, endX, endY, linePaint);
        }
    }
    private static void DrawCurvedLines(SKCanvas canvas, int width, int height, CaptchaOptions options)
    {
        Random random = new();
        int lineCount = options.CurvedLineCount > 0 ? options.CurvedLineCount : 2;
        float amplitude = options.CurveAmplitude;
        float frequency = options.CurveFrequency;
        for (int i = 0; i < lineCount; i++)
        {
            SKColor color = new(
                (byte)random.Next(100, 200),
                (byte)random.Next(100, 200),
                (byte)random.Next(100, 200),
                (byte)random.Next(100, 150)
            );
            using SKPaint linePaint = new()
            {
                Color = color,
                StrokeWidth = (float)random.NextDouble() * 1.5f + 0.5f,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };
            int startY = random.Next(height);
            float phase = (float)random.NextDouble() * (float)Math.PI * 2;
            using SKPath path = new();
            path.MoveTo(0, startY);
            for (int x = 0; x <= width; x += 5)
            {
                float y = startY + (float)Math.Sin(x * frequency * 0.01f + phase) * amplitude;
                y = Math.Clamp(y, 0, height - 1);
                path.LineTo(x, y);
            }
            canvas.DrawPath(path, linePaint);
        }
    }
    private static void DrawGradientLines(SKCanvas canvas, int width, int height, CaptchaOptions options)
    {
        Random random = new();
        int lineCount = options.GradientLineCount > 0 ? options.GradientLineCount : 2;
        for (int i = 0; i < lineCount; i++)
        {
            int startX = random.Next(width);
            int startY = random.Next(height);
            int endX = random.Next(width);
            int endY = random.Next(height);
            SKColor startColor = new(
                (byte)random.Next(50, 200),
                (byte)random.Next(50, 200),
                (byte)random.Next(50, 200),
                (byte)random.Next(100, 200)
            );
            SKColor endColor = new(
                (byte)random.Next(50, 200),
                (byte)random.Next(50, 200),
                (byte)random.Next(50, 200),
                (byte)random.Next(100, 200)
            );
            using SKPaint linePaint = new()
            {
                StrokeWidth = (float)random.NextDouble() * 2f + 0.5f,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };
            linePaint.Shader = SKShader.CreateLinearGradient(
                new SKPoint(startX, startY),
                new SKPoint(endX, endY),
                [startColor, endColor],
                null,
                SKShaderTileMode.Clamp
            );
            canvas.DrawLine(startX, startY, endX, endY, linePaint);
        }
    }
    private static void DrawNoisePoints(SKCanvas canvas, int width, int height, CaptchaOptions options)
    {
        Random random = new();
        int pointCount = options.NoisePointCount > 0 ? options.NoisePointCount : 100;
        for (int i = 0; i < pointCount; i++)
        {
            SKColor color = new(
                options.NoisePointColor.Red,
                options.NoisePointColor.Green,
                options.NoisePointColor.Blue,
                (byte)random.Next(50, 200)
            );
            using SKPaint pointPaint = new()
            {
                Color = color,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            float x = random.Next(width);
            float y = random.Next(height);
            float radius = (float)random.NextDouble() * 1.5f + 0.5f;
            canvas.DrawCircle(x, y, radius, pointPaint);
        }
    }
    private static void DrawSpecialNoise(SKCanvas canvas, int width, int height, CaptchaOptions options)
    {
        Random random = new();
        int count = options.SpecialNoiseCount > 0 ? options.SpecialNoiseCount : 20;
        for (int i = 0; i < count; i++)
        {
            int noiseType = random.Next(4);
            float x = random.Next(width);
            float y = random.Next(height);
            SKColor color = new(
                (byte)random.Next(100, 200),
                (byte)random.Next(100, 200),
                (byte)random.Next(100, 200),
                (byte)random.Next(50, 150)
            );
            using SKPaint paint = new()
            {
                Color = color,
                IsAntialias = true
            };
            switch (noiseType)
            {
                case 0:
                    {
                        float rectWidth = (float)random.NextDouble() * 10 + 2;
                        float rectHeight = (float)random.NextDouble() * 10 + 2;
                        canvas.DrawRect(x, y, rectWidth, rectHeight, paint);
                    }
                    break;
                case 1:
                    {
                        float lineX2 = x + (float)random.NextDouble() * 20 - 10;
                        float lineY2 = y + (float)random.NextDouble() * 20 - 10;
                        paint.StrokeWidth = (float)random.NextDouble() * 1.5f + 0.5f;
                        paint.Style = SKPaintStyle.Stroke;
                        canvas.DrawLine(x, y, lineX2, lineY2, paint);
                    }
                    break;
                case 2:
                    {
                        float arcRadius = (float)random.NextDouble() * 15 + 5;
                        float startAngle = (float)random.NextDouble() * 360;
                        float sweepAngle = (float)random.NextDouble() * 180 + 30;
                        using SKPath arcPath = new();
                        arcPath.AddArc(new SKRect(x - arcRadius, y - arcRadius, x + arcRadius, y + arcRadius), startAngle, sweepAngle);
                        paint.Style = SKPaintStyle.Stroke;
                        canvas.DrawPath(arcPath, paint);
                    }
                    break;
                case 3:
                    {
                        float crossSize = (float)random.NextDouble() * 8 + 2;
                        paint.StrokeWidth = (float)random.NextDouble() * 1f + 0.5f;
                        paint.Style = SKPaintStyle.Stroke;
                        canvas.DrawLine(x - crossSize, y - crossSize, x + crossSize, y + crossSize, paint);
                        canvas.DrawLine(x + crossSize, y - crossSize, x - crossSize, y + crossSize, paint);
                    }
                    break;
            }
        }
    }
    private static SKImage ApplyWaveDistortion(SKImage sourceImage, CaptchaOptions options)
    {
        int width = sourceImage.Width;
        int height = sourceImage.Height;
        using SKBitmap sourceBitmap = SKBitmap.FromImage(sourceImage);
        using SKBitmap resultBitmap = new(width, height);
        Random random = new();
        float ampX = options.WaveAmplitudeX;
        float ampY = options.WaveAmplitudeY;
        float freqX = options.WaveFrequencyX;
        float freqY = options.WaveFrequencyY;
        float phaseX = (float)random.NextDouble() * (float)Math.PI * 2;
        float phaseY = (float)random.NextDouble() * (float)Math.PI * 2;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int offsetX = (int)(Math.Sin(y * freqX * 0.01f + phaseX) * ampX);
                int offsetY = (int)(Math.Cos(x * freqY * 0.01f + phaseY) * ampY);
                int srcX = Math.Clamp(x + offsetX, 0, width - 1);
                int srcY = Math.Clamp(y + offsetY, 0, height - 1);
                resultBitmap.SetPixel(x, y, sourceBitmap.GetPixel(srcX, srcY));
            }
        }
        return SKImage.FromBitmap(resultBitmap);
    }
    private static int Clamp(int value, int min, int max) => Math.Max(min, Math.Min(max, value));
    #endregion
}
#endif