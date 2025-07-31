
using UnityEngine;

public static class ColorPickerConstants
{
    // Window
    public const float WindowWidth = 720f;
    public const float WindowHeight = 650f;

    // Color wheel
    public const float ColorWheelSize = 200f;
    public const float ColorWheelTopOffset = 20f;
    public const float SelectorDotSize = 6f;
    public const int SelectorDotBorderThickness = 1;
    public const float CrosshairLineLength = 10f;
    public const float CrosshairLineThickness = 2f;
    public const float CrosshairGapSize = 8f; // size of the hollow center

    // Brightness slider
    public const float BrightnessSliderHeight = 200f;
    public const float BrightnessSliderWidth = 20f;
    public const float BrightnessSliderExtraWidth = 16f;
    public const float TriangleWidth = 12f;
    public const float TriangleHeight = 10f;
    public const float TriangleSliderGap = 4f;

    // Layout
    public const float Padding = 20f;
    public const float PaddingLeftAndRight = 2f * Padding;
    public const float RowSpacing = 10f;
    public const float VerticalRowSpacing = 12f;
    public const float SectionSpacing = 10f;
    public const float TopSectionHeight = ColorWheelSize + 100f;
    public const float PaletteHeightOffset = 40f;

    // Palette
    public const float PaletteBoxSize = 22f;
    public const float PaletteCellSpacing = 4f;

    // Input fields
    public const float InputRowHeight = 30f;
    public const float InputRowSpacingMultiplier = 1.5f;
    public const float LabelWidth = 20f;
    public const float LargeLabelWidth = 40f;
    public const float LargeFieldWidth = 120f;
    public const float LabelToBoxVerticalSpacing = 2f;
    public const float LabelToFieldHorizontalSpacing = 5f;

    // Hex Alpha layout
    public const float HexFieldRatio = 0.6f;
    public const float HexAlphaExtraPaddingMultiplier = 3f;

    // Multi-column layout
    public const float ThreeColumnPaddingMultiplier = 4f;

    // Preview boxes
    public const float PreviewBoxSize = 28f;
    public const float PreviewLabelWidth = 160f;
    public const float PreviewLabelHeight = 24f;
    public const float PreviewBoxSpacing = 10f;
    public const float LabelSpacing = 80f;
    public const int PreviewBoxBorderThickness = 2;

    // Buttons
    public const float ButtonHeight = 32f;

    // Color math
    public const float HueMaxDegrees = 360f;
    public const float PercentageMax = 100f;
    public const float HSVTolerance = 0.01f;
    public const float HSVHueToUnit = 1f / HueMaxDegrees;
    public const float HSVSaturationToUnit = 1f / PercentageMax;
    public const float HSVBrightnessToUnit = 1f / PercentageMax;

    // Triangle selector texture
    public const int TriangleTextureWidth = 12;
    public const int TriangleTextureHeight = 10;

    // Visuals
    public static readonly Color SelectorDotBorderColor = Color.black;
    public static readonly Color SelectorDotFillColor = Color.white;

    // Labels
    public const string LabelCancel = "Cancel";
    public const string LabelSave = "Save";
    public const string LabelCurrentColor = "Current Color";
    public const string LabelNewColor = "New Color";

    public const string LabelHex = "Hex";
    public const string LabelAlpha = "Alpha";
    public const string LabelRed = "R";
    public const string LabelGreen = "G";
    public const string LabelBlue = "B";
    public const string LabelHue = "H";
    public const string LabelSaturation = "S";
    public const string LabelBrightness = "L";
    public const string InputPrefix = "Input_";
}
