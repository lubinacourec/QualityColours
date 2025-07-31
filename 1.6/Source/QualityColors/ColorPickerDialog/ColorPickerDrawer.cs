using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace QualityColors
{
    public static class ColorPickerDrawer
    {
        public static void DrawColorWheel(ColorPickerLayoutHelper layoutHelper, ColorPickerColorState colorState, ColorPickerTextureManager textureManager)
        {
            textureManager.UpdateHueAndSaturation(colorState.GetHue(), colorState.GetSaturation());
            GUI.DrawTexture(layoutHelper.ColorWheelRect, textureManager.GetOrCreateColorWheelTexture());
            DrawColorWheelSelectorCrosshair(layoutHelper, colorState.GetHue(), colorState.GetSaturation());
        }

        public static void DrawBrightnessSlider(ColorPickerLayoutHelper layoutHelper, ColorPickerColorState colorState, ColorPickerTextureManager textureManager)
        {
            GUI.DrawTexture(layoutHelper.BrightnessSliderRect, textureManager.GetOrCreateBrightnessGradientTexture());
            GUI.DrawTexture(layoutHelper.BrightnessTriangleRect(colorState.GetLightness()), textureManager.GetOrCreateWhiteTriangleTexture());
        }

        public static void DrawColorWheelSelectorCrosshair(ColorPickerLayoutHelper layoutHelper, double hueDegrees, double saturationPercent)
        {
            var crosshairRects = layoutHelper.GetSelectorCrosshairRects(hueDegrees, saturationPercent);
            Widgets.DrawBoxSolid(crosshairRects.Top, ColorPickerConstants.SelectorDotBorderColor);
            Widgets.DrawBoxSolid(crosshairRects.Bottom, ColorPickerConstants.SelectorDotBorderColor);
            Widgets.DrawBoxSolid(crosshairRects.Left, ColorPickerConstants.SelectorDotBorderColor);
            Widgets.DrawBoxSolid(crosshairRects.Right, ColorPickerConstants.SelectorDotBorderColor);
        }

        public static void DrawPalette(ColorPickerLayoutHelper layoutHelper, ColorPickerColorState colorState)
        {
            var allColorDefs = DefDatabase<ColorDef>.AllDefsListForReading;
            if (allColorDefs.NullOrEmpty()) return;

            for (int index = 0; index < allColorDefs.Count; index++)
            {
                var colorCellRect = layoutHelper.PaletteCellRect(index, allColorDefs.Count);
                var colorDef = allColorDefs[index];

                Widgets.DrawBoxSolid(colorCellRect, colorDef.color);
                if (Widgets.ButtonInvisible(colorCellRect))
                    colorState.SetColorNormalized(colorDef.color);

                TooltipHandler.TipRegion(colorCellRect, colorDef.LabelCap.NullOrEmpty() ? colorDef.defName : colorDef.LabelCap.ToString());
            }
        }

        public static void DrawColorPreviews(ColorPickerLayoutHelper layoutHelper, Color originalColor, Color selectedColor)
        {
            var (originalLabelRect, originalColorRect, newLabelRect, newColorRect) = layoutHelper.ColorPreviewsRect();

            GUI.color = originalColor;
            Widgets.Label(originalLabelRect, ColorPickerConstants.LabelCurrentColor);
            GUI.color = Color.white;
            Widgets.DrawBoxSolid(originalColorRect, originalColor);
            Widgets.DrawBox(originalColorRect, ColorPickerConstants.PreviewBoxBorderThickness);

            GUI.color = selectedColor;
            Widgets.Label(newLabelRect, ColorPickerConstants.LabelNewColor);
            GUI.color = Color.white;
            Widgets.DrawBoxSolid(newColorRect, selectedColor);
            Widgets.DrawBox(newColorRect, ColorPickerConstants.PreviewBoxBorderThickness);
        }

        public static bool DrawLabeledInput<T>(
            ColorPickerLayoutHelper layoutHelper,
            Rect fullRect,
            string label,
            InputFieldState<T> inputFieldState,
            Func<string, (bool isValid, T value)> tryParseFunc,
            Action<T> onValidInputAction,
            Func<T> getCurrentValueFunc,
            ColorPickerUserInteractionHandler interactionHandler)
        {
            var (labelRect, textFieldRect) = layoutHelper.LabeledTextInputRects(fullRect);
            Widgets.Label(labelRect, label);

            string controlName = "Input_" + label;
            GUI.SetNextControlName(controlName);

            string currentTextValue = getCurrentValueFunc().ToString();
            string displayText = inputFieldState.GetDisplayValue(currentTextValue);
            string updatedBuffer = Widgets.TextField(textFieldRect, displayText);

            interactionHandler.HandleInputFieldInteraction(inputFieldState, controlName);
            return interactionHandler.HandleTextFieldChange(inputFieldState, updatedBuffer, tryParseFunc, onValidInputAction);
        }

        public static void DrawInputSection(ColorPickerLayoutHelper layoutHelper, ColorPickerColorState colorState, ColorPickerUserInteractionHandler interactionHandler)
        {
            var listing = new Listing_Standard();
            listing.Begin(layoutHelper.InputSectionRect);

            listing.GapLine();
            listing.Gap(ColorPickerConstants.RowSpacing * ColorPickerConstants.InputRowSpacingMultiplier);
            DrawHexAlphaRow(layoutHelper, colorState, layoutHelper.InputSectionRect, listing, interactionHandler);

            listing.Gap(ColorPickerConstants.RowSpacing * ColorPickerConstants.InputRowSpacingMultiplier);
            DrawRGBRow(layoutHelper, colorState, layoutHelper.InputSectionRect, listing, interactionHandler);

            listing.Gap(ColorPickerConstants.RowSpacing * ColorPickerConstants.InputRowSpacingMultiplier);
            DrawHSLRow(layoutHelper, colorState, layoutHelper.InputSectionRect, listing, interactionHandler);

            listing.End();
        }

        private static void DrawHexAlphaRow(ColorPickerLayoutHelper layoutHelper, ColorPickerColorState colorState, Rect fullRect, Listing_Standard listing, ColorPickerUserInteractionHandler interactionHandler)
        {
            var (hexInputRect, alphaInputRect) = layoutHelper.HexAlphaRects(fullRect, listing.GetRect(ColorPickerConstants.InputRowHeight));

            DrawLabeledInput(
                layoutHelper,
                hexInputRect,
                ColorPickerConstants.LabelHex,
                colorState.Input.HexColorInput,
                ParserUtils.TryParseHex,
                colorState.SetFromHex,
                () => colorState.GetHexRGB(),
                interactionHandler);

            DrawLabeledInput(
                layoutHelper,
                alphaInputRect,
                ColorPickerConstants.LabelAlpha,
                colorState.Input.AlphaChannelInput,
                ParserUtils.TryParseInt,
                colorState.SetAlpha255,
                () => colorState.Alpha255,
                interactionHandler
            );
        }

        public static void DrawRGBRow(ColorPickerLayoutHelper layoutHelper, ColorPickerColorState colorState, Rect fullRect, Listing_Standard listing, ColorPickerUserInteractionHandler interactionHandler)
        {
            var (redInputRect, greenInputRect, blueInputRect) = layoutHelper.ThreeColumnRowRects(fullRect, listing.GetRect(ColorPickerConstants.InputRowHeight));

            DrawLabeledInput(layoutHelper, redInputRect, ColorPickerConstants.LabelRed,
                colorState.Input.RedChannelInput, ParserUtils.TryParseInt,
                colorState.SetRed255, () => colorState.Red255, interactionHandler);

            DrawLabeledInput(layoutHelper, greenInputRect, ColorPickerConstants.LabelGreen,
                colorState.Input.GreenChannelInput, ParserUtils.TryParseInt,
                colorState.SetGreen255, () => colorState.Green255, interactionHandler);

            DrawLabeledInput(layoutHelper, blueInputRect, ColorPickerConstants.LabelBlue,
                colorState.Input.BlueChannelInput, ParserUtils.TryParseInt,
                colorState.SetBlue255, () => colorState.Blue255, interactionHandler);
        }

        private static void DrawHSLRow(ColorPickerLayoutHelper layoutHelper, ColorPickerColorState colorState, Rect fullRect, Listing_Standard listing, ColorPickerUserInteractionHandler interactionHandler)
        {
            var (hueInputRect, saturationInputRect, lightnessInputRect) = layoutHelper.ThreeColumnRowRects(fullRect, listing.GetRect(ColorPickerConstants.InputRowHeight));

            DrawLabeledInput(layoutHelper, hueInputRect, ColorPickerConstants.LabelHue,
                colorState.Input.HueDegreesInput, ParserUtils.TryParseDouble,
                colorState.SetHue, () => Math.Round(colorState.GetHue(), 2), interactionHandler);

            DrawLabeledInput(layoutHelper, saturationInputRect, ColorPickerConstants.LabelSaturation,
                colorState.Input.SaturationPercentInput, ParserUtils.TryParseDouble,
                colorState.SetSaturation, () => Math.Round(colorState.GetSaturation(), 2), interactionHandler);

            DrawLabeledInput(layoutHelper, lightnessInputRect, ColorPickerConstants.LabelBrightness,
                colorState.Input.LightnessPercentInput, ParserUtils.TryParseDouble,
                colorState.SetLightness, () => Math.Round(colorState.GetLightness(), 2), interactionHandler);
        }

        public static void DrawActionButtonsSection(ColorPickerLayoutHelper layoutHelper, ColorPickerColorState colorState, Action<Color> onSaveCallback, Action closeAction)
        {
            var (cancelButtonRect, saveButtonRect) = layoutHelper.SaveCancelButtonRects(layoutHelper.ButtonSectionRect);

            if (Widgets.ButtonText(cancelButtonRect, ColorPickerConstants.LabelCancel))
                closeAction?.Invoke();

            if (Widgets.ButtonText(saveButtonRect, ColorPickerConstants.LabelSave))
            {
                onSaveCallback?.Invoke(colorState.GetUnityColor);
                closeAction?.Invoke();
            }
        }
    }
}
