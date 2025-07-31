using Verse;
using UnityEngine;

namespace QualityColors
{
    public class ColorPickerColorState
    {
        private double redNormalized;
        private double greenNormalized;
        private double blueNormalized;
        private double alphaNormalized;

        private double hueInDegrees;
        private double saturationPercent;
        private double lightnessPercent;

        public Color OriginalColor { get; }
        public ColorPickerInputState Input { get; } = new();

        public ColorPickerColorState(double red, double green, double blue, double alpha = 1.0)
        {
            OriginalColor = new Color((float)red, (float)green, (float)blue, (float)alpha);
            SetColorNormalized(red, green, blue, alpha);
        }

        public ColorPickerColorState(Color unityColor)
        {
            OriginalColor = unityColor;
            SetColorNormalized(unityColor.r, unityColor.g, unityColor.b, unityColor.a);
        }

        public double Red => redNormalized;
        public double Green => greenNormalized;
        public double Blue => blueNormalized;
        public double Alpha => alphaNormalized;

        public double GetHue() => hueInDegrees;
        public double GetSaturation() => saturationPercent;
        public double GetLightness() => lightnessPercent;

        public void SetColorNormalized(Color unityColor) =>
            SetColorNormalized(unityColor.r, unityColor.g, unityColor.b, unityColor.a);

        public void SetColorNormalized(double red, double green, double blue, double alpha = 1.0)
        {
            redNormalized = ColorUtils.Clamp01(red);
            greenNormalized = ColorUtils.Clamp01(green);
            blueNormalized = ColorUtils.Clamp01(blue);
            alphaNormalized = ColorUtils.Clamp01(alpha);
            UpdateHSLFromRGB();
        }

        public void SetColor255(int red255, int green255, int blue255, int alpha255 = 255)
        {
            SetColorNormalized(
                ColorUtils.From255(red255),
                ColorUtils.From255(green255),
                ColorUtils.From255(blue255),
                ColorUtils.From255(alpha255));
        }

        public void SetRed(double red)
        {
            redNormalized = ColorUtils.Clamp01(red);
            UpdateHSLFromRGB();
        }

        public void SetGreen(double green)
        {
            greenNormalized = ColorUtils.Clamp01(green);
            UpdateHSLFromRGB();
        }

        public void SetBlue(double blue)
        {
            blueNormalized = ColorUtils.Clamp01(blue);
            UpdateHSLFromRGB();
        }

        public void SetAlpha(double alpha)
        {
            alphaNormalized = ColorUtils.Clamp01(alpha);
        }

        public void SetRed255(int red255) => SetRed(ColorUtils.From255(red255));
        public void SetGreen255(int green255) => SetGreen(ColorUtils.From255(green255));
        public void SetBlue255(int blue255) => SetBlue(ColorUtils.From255(blue255));
        public void SetAlpha255(int alpha255) => SetAlpha(ColorUtils.From255(alpha255));

        public void SetHue(double hueDegrees)
        {
            hueInDegrees = ColorUtils.ClampHue(hueDegrees);
            (redNormalized, greenNormalized, blueNormalized) = ColorUtils.HSLToRGB(hueInDegrees, saturationPercent, lightnessPercent);
        }

        public void SetSaturation(double saturation)
        {
            saturationPercent = ColorUtils.ClampPercent(saturation);
            (redNormalized, greenNormalized, blueNormalized) = ColorUtils.HSLToRGB(hueInDegrees, saturationPercent, lightnessPercent);
        }

        public void SetLightness(double lightness)
        {
            lightnessPercent = ColorUtils.ClampPercent(lightness);
            (redNormalized, greenNormalized, blueNormalized) = ColorUtils.HSLToRGB(hueInDegrees, saturationPercent, lightnessPercent);
        }

        public void SetFromHex(string hexString)
        {
            if (!ParserUtils.IsValidHex(hexString))
            {
                Log.Warning($"[QualityColors] Invalid hex color string: \"{hexString}\". This error should be reported.");
                return;
            }

            ColorUtils.HexToColor(hexString, out var red, out var green, out var blue, out var alpha);
            SetColorNormalized(red, green, blue, alpha);
        }

        private void UpdateHSLFromRGB()
        {
            ColorUtils.RGBToHSL(redNormalized, 
                greenNormalized, 
                blueNormalized, 
                out hueInDegrees, 
                out saturationPercent, 
                out lightnessPercent);
        }

        public Color GetUnityColor => new Color(
            (float)redNormalized,
            (float)greenNormalized,
            (float)blueNormalized,
            (float)alphaNormalized
        );

        public int Red255 => ColorUtils.To255(redNormalized);
        public int Green255 => ColorUtils.To255(greenNormalized);
        public int Blue255 => ColorUtils.To255(blueNormalized);
        public int Alpha255 => ColorUtils.To255(alphaNormalized);

        public string GetHexRGB() => ColorUtils.ToHexRGB(redNormalized, greenNormalized, blueNormalized);
        public string GetHexRGBA() => ColorUtils.ToHexRGBA(redNormalized, greenNormalized, blueNormalized, alphaNormalized);
    }
}
