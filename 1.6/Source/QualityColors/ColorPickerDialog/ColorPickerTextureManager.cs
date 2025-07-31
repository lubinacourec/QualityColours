using System;
using UnityEngine;

public class ColorPickerTextureManager : IDisposable
{
    private Texture2D _colorWheelTexture;
    private Texture2D _brightnessGradientTexture;
    private Texture2D _whiteTriangleTexture;

    private double _hueDegrees;
    private double _saturationPercent;

    public ColorPickerTextureManager(double initialHueDegrees, double initialSaturationPercent)
    {
        _hueDegrees = initialHueDegrees;
        _saturationPercent = initialSaturationPercent;
    }

    public Texture2D GetOrCreateColorWheelTexture()
    {
        return _colorWheelTexture ??= GenerateColorWheelTexture((int)ColorPickerConstants.ColorWheelSize);
    }

    public Texture2D GetOrCreateBrightnessGradientTexture()
    {
        return _brightnessGradientTexture ??= GenerateBrightnessGradientTexture();
    }

    public Texture2D GetOrCreateWhiteTriangleTexture()
    {
        return _whiteTriangleTexture ??= GenerateWhiteTriangleTexture();
    }

    public void UpdateHueAndSaturation(double newHueDegrees, double newSaturationPercent)
    {
        if (Math.Abs(newHueDegrees - _hueDegrees) > ColorPickerConstants.HSVTolerance ||
            Math.Abs(newSaturationPercent - _saturationPercent) > ColorPickerConstants.HSVTolerance)
        {
            _hueDegrees = newHueDegrees;
            _saturationPercent = newSaturationPercent;

            if (_brightnessGradientTexture != null)
            {
                UnityEngine.Object.Destroy(_brightnessGradientTexture);
                _brightnessGradientTexture = null;
            }
        }
    }

    private Texture2D GenerateColorWheelTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size) { wrapMode = TextureWrapMode.Clamp };
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                Vector2 delta = pos - center;
                float dist = delta.magnitude;

                if (dist <= radius)
                {
                    double angle = -Math.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                    double hue = (angle + ColorPickerConstants.HueMaxDegrees) % ColorPickerConstants.HueMaxDegrees;
                    double saturation = dist / radius;

                    Color color = Color.HSVToRGB(
                        (float)(hue / ColorPickerConstants.HueMaxDegrees),
                        (float)saturation,
                        1f
                    );

                    texture.SetPixel(x, y, color);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return texture;
    }

    private Texture2D GenerateBrightnessGradientTexture()
    {
        int width = (int)ColorPickerConstants.BrightnessSliderWidth;
        int height = (int)ColorPickerConstants.BrightnessSliderHeight;

        Texture2D texture = new Texture2D(width, height) { wrapMode = TextureWrapMode.Clamp };

        double hueUnit = _hueDegrees / ColorPickerConstants.HueMaxDegrees;
        double satUnit = _saturationPercent / ColorPickerConstants.PercentageMax;

        for (int y = 0; y < height; y++)
        {
            float value = y / (float)(height - 1);
            Color color = Color.HSVToRGB((float)hueUnit, (float)satUnit, value);

            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private Texture2D GenerateWhiteTriangleTexture()
    {
        int width = ColorPickerConstants.TriangleTextureWidth;
        int height = ColorPickerConstants.TriangleTextureHeight;

        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };

        Color[] pixels = new Color[width * height];
        Array.Fill(pixels, Color.clear);

        Color white = Color.white;
        int centerY = height / 2;

        for (int x = 0; x < width; x++)
        {
            float t = x / (float)(width - 1);
            int halfHeight = (int)(t * (height * 0.5f));

            for (int y = centerY - halfHeight; y <= centerY + halfHeight; y++)
            {
                if (y >= 0 && y < height)
                {
                    pixels[y * width + x] = white;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public void Dispose()
    {
        if (_colorWheelTexture != null) UnityEngine.Object.Destroy(_colorWheelTexture);
        if (_brightnessGradientTexture != null) UnityEngine.Object.Destroy(_brightnessGradientTexture);
        if (_whiteTriangleTexture != null) UnityEngine.Object.Destroy(_whiteTriangleTexture);
    }
}
