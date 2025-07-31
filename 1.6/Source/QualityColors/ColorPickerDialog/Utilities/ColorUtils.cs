using System;
using System.Globalization;

public static class ColorUtils
{
    public static double Clamp01(double value) =>
        value < 0 ? 0 : value > 1 ? 1 : value;

    public static double ClampHue(double value) =>
        (value % ColorPickerConstants.HueMaxDegrees + ColorPickerConstants.HueMaxDegrees) % ColorPickerConstants.HueMaxDegrees;

    public static double ClampPercent(double value) =>
        value < 0 ? 0 : value > 100 ? 100 : value;

    public static int To255(double value) =>
        (int)(Clamp01(value) * 255.0 + 0.5);

    public static double From255(int value255) =>
        Clamp01(value255 / 255.0);

    public static (double r, double g, double b) HSLToRGB(double hueDegrees, double saturationPercent, double lightnessPercent)
    {
        double h = ClampHue(hueDegrees) / ColorPickerConstants.HueMaxDegrees;
        double s = Clamp01(saturationPercent / ColorPickerConstants.PercentageMax);
        double l = Clamp01(lightnessPercent / ColorPickerConstants.PercentageMax);

        if (s == 0)
            return (l, l, l);

        double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
        double p = 2 * l - q;

        double r = HueToRGB(p, q, h + 1.0 / 3.0);
        double g = HueToRGB(p, q, h);
        double b = HueToRGB(p, q, h - 1.0 / 3.0);

        return (Clamp01(r), Clamp01(g), Clamp01(b));
    }

    private static double HueToRGB(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;

        if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;

        return p;
    }

    public static void RGBToHSL(double r, double g, double b, out double h, out double s, out double l)
    {
        double max = Math.Max(Math.Max(r, g), b);
        double min = Math.Min(Math.Min(r, g), b);
        l = (max + min) / 2.0;

        if (max == min)
        {
            h = 0;
            s = 0;
        }
        else
        {
            double d = max - min;
            s = l < 0.5 ? d / (max + min) : d / (2.0 - max - min);

            if (max == r)
                h = (g - b) / d + (g < b ? 6 : 0);
            else if (max == g)
                h = (b - r) / d + 2;
            else
                h = (r - g) / d + 4;

            h /= 6.0;
        }

        h *= ColorPickerConstants.HueMaxDegrees;
        s *= ColorPickerConstants.PercentageMax;
        l *= ColorPickerConstants.PercentageMax;
    }

    public static double RGBToHueDegrees(double r, double g, double b)
    {
        RGBToHSL(r, g, b, out var h, out _, out _);
        return h;
    }

    public static double RGBToSaturationPercent(double r, double g, double b)
    {
        RGBToHSL(r, g, b, out _, out var s, out _);
        return s;
    }

    public static double RGBToLightnessPercent(double r, double g, double b)
    {
        RGBToHSL(r, g, b, out _, out _, out var l);
        return l;
    }

    public static string ToHexRGB(double r, double g, double b) => $"{To255(r):X2}{To255(g):X2}{To255(b):X2}";

    public static string ToHexRGBA(double r, double g, double b, double a) => $"{To255(r):X2}{To255(g):X2}{To255(b):X2}{To255(a):X2}";

    public static void HexToColor(string hex, out double r, out double g, out double b, out double a)
    {
        r = g = b = a = 1.0;
        hex = hex.TrimStart('#');

        int red = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        int green = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        int blue = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        int alpha = hex.Length == 8
            ? int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber)
            : 255;

        r = From255(red);
        g = From255(green);
        b = From255(blue);
        a = From255(alpha);
    }
}
