using System;
using UnityEngine;

namespace QualityColors
{
    public class ColorPickerLayoutHelper
    {
        private readonly Rect _mainRect;

        public ColorPickerLayoutHelper(Rect mainRect)
        {
            _mainRect = mainRect;
        }

        public Rect TopSectionRect => new Rect(
            _mainRect.x,
            _mainRect.y,
            _mainRect.width,
            ColorPickerConstants.TopSectionHeight);

        public Rect BottomSectionRect => new Rect(
            _mainRect.x,
            _mainRect.y + ColorPickerConstants.TopSectionHeight + ColorPickerConstants.SectionSpacing,
            _mainRect.width,
            _mainRect.height - ColorPickerConstants.TopSectionHeight - ColorPickerConstants.SectionSpacing);

        public Rect ColorWheelRect => new Rect(
            _mainRect.x,
            _mainRect.y + ColorPickerConstants.ColorWheelTopOffset,
            ColorPickerConstants.ColorWheelSize,
            ColorPickerConstants.ColorWheelSize);

        public Rect BrightnessSliderRect => new Rect(
            ColorWheelRect.xMax + ColorPickerConstants.Padding,
            ColorWheelRect.y,
            ColorPickerConstants.BrightnessSliderWidth,
            ColorPickerConstants.BrightnessSliderHeight);

        public Rect PaletteRect => new Rect(
            BrightnessSliderRect.xMax + ColorPickerConstants.Padding,
            ColorWheelRect.y,
            _mainRect.width - BrightnessSliderRect.xMax - ColorPickerConstants.PaddingLeftAndRight,
            ColorPickerConstants.ColorWheelSize + ColorPickerConstants.PaletteHeightOffset);

        public Rect BrightnessTriangleRect(double brightnessPercent) => new Rect(
            BrightnessSliderRect.xMax + ColorPickerConstants.TriangleSliderGap,
            (float)(BrightnessSliderRect.y + (1.0 - Math.Clamp(brightnessPercent / ColorPickerConstants.PercentageMax, 0.0, 1.0)) * BrightnessSliderRect.height - ColorPickerConstants.TriangleHeight / 2.0),
            ColorPickerConstants.TriangleWidth,
            ColorPickerConstants.TriangleHeight
        );

        public Rect SelectorDotRect(double hueDegrees, double saturationPercent)
        {
            double radius = ColorWheelRect.width / 2.0;
            double angle = hueDegrees * Math.PI / 180.0;
            double sat = Math.Clamp(saturationPercent / ColorPickerConstants.PercentageMax, 0.0, 1.0);

            float x = (float)(ColorWheelRect.center.x + Math.Cos(angle) * sat * radius);
            float y = (float)(ColorWheelRect.center.y + Math.Sin(angle) * sat * radius);

            return new Rect(
                x - ColorPickerConstants.SelectorDotSize / 2f,
                y - ColorPickerConstants.SelectorDotSize / 2f,
                ColorPickerConstants.SelectorDotSize,
                ColorPickerConstants.SelectorDotSize
            );
        }

        public (Rect labelA, Rect boxA, Rect labelB, Rect boxB) ColorPreviewsRect()
        {
            var labelA = new Rect(_mainRect.x, ColorWheelRect.yMax + ColorPickerConstants.PreviewBoxSpacing,
                ColorPickerConstants.PreviewLabelWidth, ColorPickerConstants.PreviewLabelHeight);
            var boxA = new Rect(labelA.x, labelA.yMax + ColorPickerConstants.LabelToBoxVerticalSpacing,
                ColorPickerConstants.PreviewBoxSize, ColorPickerConstants.PreviewBoxSize);

            var labelB = new Rect(boxA.xMax + ColorPickerConstants.LabelSpacing, labelA.y,
                ColorPickerConstants.PreviewLabelWidth, ColorPickerConstants.PreviewLabelHeight);
            var boxB = new Rect(labelB.x, labelB.yMax + ColorPickerConstants.LabelToBoxVerticalSpacing,
                ColorPickerConstants.PreviewBoxSize, ColorPickerConstants.PreviewBoxSize);

            return (labelA, boxA, labelB, boxB);
        }

        public Rect PaletteCellRect(int index, int total)
        {
            int cols = Mathf.Max(1, Mathf.FloorToInt(PaletteRect.width / (ColorPickerConstants.PaletteBoxSize + ColorPickerConstants.PaletteCellSpacing)));
            return new Rect(
                PaletteRect.x + (index % cols) * (ColorPickerConstants.PaletteBoxSize + ColorPickerConstants.PaletteCellSpacing),
                PaletteRect.y + (index / cols) * (ColorPickerConstants.PaletteBoxSize + ColorPickerConstants.PaletteCellSpacing),
                ColorPickerConstants.PaletteBoxSize,
                ColorPickerConstants.PaletteBoxSize);
        }

        public (Rect labelRect, Rect textFieldRect) LabeledTextInputRects(Rect rect) => (
            new Rect(rect.x, rect.y, ColorPickerConstants.LargeLabelWidth, rect.height),
            new Rect(rect.x + ColorPickerConstants.LargeLabelWidth + ColorPickerConstants.LabelToFieldHorizontalSpacing, rect.y,
                     rect.width - ColorPickerConstants.LargeLabelWidth - ColorPickerConstants.LabelToFieldHorizontalSpacing, rect.height));

        public (Rect hexRect, Rect alphaRect) HexAlphaRects(Rect fullRect, Rect rowRect)
        {
            float spacing = ColorPickerConstants.Padding;
            float inputWidth = (rowRect.width - spacing) / 2f;

            return (
                new Rect(rowRect.x, rowRect.y, inputWidth, rowRect.height),
                new Rect(rowRect.x + inputWidth + spacing, rowRect.y, inputWidth, rowRect.height));
        }

        public (Rect a, Rect b, Rect c) ThreeColumnRowRects(Rect fullRect, Rect rowRect)
        {
            float colWidth = (fullRect.width - ColorPickerConstants.ThreeColumnPaddingMultiplier * ColorPickerConstants.Padding) / 3f;
            return (
                new Rect(rowRect.xMin + ColorPickerConstants.Padding, rowRect.y, colWidth, rowRect.height),
                new Rect(rowRect.xMin + ColorPickerConstants.Padding * 2 + colWidth, rowRect.y, colWidth, rowRect.height),
                new Rect(rowRect.xMin + ColorPickerConstants.Padding * 3 + colWidth * 2, rowRect.y, colWidth, rowRect.height));
        }

        public (Rect cancelButton, Rect saveButton) SaveCancelButtonRects(Rect fullRect)
        {
            float y = fullRect.yMax - ColorPickerConstants.ButtonHeight - ColorPickerConstants.RowSpacing;
            return (
                new Rect(fullRect.xMin + ColorPickerConstants.Padding, y, ColorPickerConstants.LargeFieldWidth, ColorPickerConstants.ButtonHeight),
                new Rect(fullRect.xMax - ColorPickerConstants.LargeFieldWidth - ColorPickerConstants.Padding, y, ColorPickerConstants.LargeFieldWidth, ColorPickerConstants.ButtonHeight));
        }

        public Rect InputSectionRect => new Rect(
            BottomSectionRect.x,
            BottomSectionRect.y,
            BottomSectionRect.width,
            BottomSectionRect.height - ColorPickerConstants.ButtonHeight - ColorPickerConstants.RowSpacing - ColorPickerConstants.Padding);

        public Rect ButtonSectionRect => new Rect(
            BottomSectionRect.x,
            InputSectionRect.yMax + ColorPickerConstants.RowSpacing,
            BottomSectionRect.width,
            ColorPickerConstants.ButtonHeight);

        public Rect SelectorDotFillRect(double hueDegrees, double saturationPercent)
        {
            Rect outer = SelectorDotRect(hueDegrees, saturationPercent);
            return new Rect(
                (float)(outer.x + 1.0),
                (float)(outer.y + 1.0),
                (float)(outer.width - 2.0),
                (float)(outer.height - 2.0)
            );
        }

        public SelectorCrosshairRects GetSelectorCrosshairRects(double hueDegrees, double saturationPercent)
        {
            double radius = ColorWheelRect.width / 2.0;
            double angle = hueDegrees * Math.PI / 180.0;
            double sat = Math.Clamp(saturationPercent / ColorPickerConstants.PercentageMax, 0.0, 1.0);

            double offsetX = Math.Cos(angle) * sat * radius;
            double offsetY = Math.Sin(angle) * sat * radius;

            double centerX = ColorWheelRect.center.x + offsetX;
            double centerY = ColorWheelRect.center.y + offsetY;

            double halfGap = ColorPickerConstants.CrosshairGapSize / 2.0;
            double lineLength = ColorPickerConstants.CrosshairLineLength;
            double thickness = ColorPickerConstants.CrosshairLineThickness;

            return new SelectorCrosshairRects(
                new Rect((float)(centerX - thickness / 2.0), (float)(centerY - halfGap - lineLength), (float)thickness, (float)lineLength), // Top
                new Rect((float)(centerX - thickness / 2.0), (float)(centerY + halfGap), (float)thickness, (float)lineLength),             // Bottom
                new Rect((float)(centerX - halfGap - lineLength), (float)(centerY - thickness / 2.0), (float)lineLength, (float)thickness), // Left
                new Rect((float)(centerX + halfGap), (float)(centerY - thickness / 2.0), (float)lineLength, (float)thickness)              // Right
            );
        }

        public readonly struct SelectorCrosshairRects
        {
            public readonly Rect Top;
            public readonly Rect Bottom;
            public readonly Rect Left;
            public readonly Rect Right;

            public SelectorCrosshairRects(Rect top, Rect bottom, Rect left, Rect right)
            {
                Top = top;
                Bottom = bottom;
                Left = left;
                Right = right;
            }
        }
    }
}
