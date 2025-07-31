using RimWorld;
using System;
using UnityEngine;
using Verse;
using static QualityColors.ColorPickerWidgets;

namespace QualityColors
{
    // -------------------- Interaction Handling --------------------
    public class ColorPickerUserInteractionHandler
    {
        private bool _isDraggingColorWheel;

        public void HandleColorWheelInteraction(Rect colorWheelRect, ColorPickerColorState colorState)
        {
            Vector2 center = colorWheelRect.center;
            Vector2 mousePosition = Event.current.mousePosition;
            Vector2 delta = mousePosition - center;
            float radius = colorWheelRect.width / 2f;

            EventType currentEventType = Event.current.type;

            if (currentEventType == EventType.MouseDown && Event.current.button == 0)
            {
                if (delta.magnitude <= radius)
                {
                    _isDraggingColorWheel = true;
                    Event.current.Use();
                }
            }

            if (currentEventType == EventType.MouseUp && Event.current.button == 0)
            {
                _isDraggingColorWheel = false;
            }

            if (currentEventType == EventType.MouseDrag && _isDraggingColorWheel)
            {
                Event.current.Use();
            }

            if (_isDraggingColorWheel)
            {
                float hueDegrees = (Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg + 360f) % 360f;
                float saturationNormalized = Mathf.Min(delta.magnitude / radius, 1f);

                colorState.UpdateHue(hueDegrees);
                colorState.UpdateSaturation(saturationNormalized * 100f);
            }

            DrawColorWheelSelectorDot(colorWheelRect, colorState.HueDegrees, colorState.SaturationPercent);
        }

        public void HandleBrightnessSliderInteraction(Rect sliderRect, ColorPickerColorState colorState)
        {
            EventType evt = Event.current.type;
            if ((evt == EventType.MouseDown || evt == EventType.MouseDrag) && Event.current.button == 0)
            {
                Vector2 mousePos = Event.current.mousePosition;
                Rect extendedRect = new Rect(sliderRect.x, sliderRect.y, sliderRect.width + 16f, sliderRect.height);

                if (extendedRect.Contains(mousePos))
                {
                    float relativeY = Mathf.Clamp01((mousePos.y - sliderRect.y) / sliderRect.height);
                    float newBrightness = (1f - relativeY) * 100f;
                    colorState.UpdateBrightness(newBrightness);
                    Event.current.Use();
                }
            }
        }

        public void DrawBrightnessSlider(Rect sliderRect, ColorPickerTextureManager textureManager, ColorPickerColorState colorState)
        {
            Texture2D brightnessTexture = textureManager.GetOrCreateBrightnessGradientTexture();
            GUI.DrawTexture(sliderRect, brightnessTexture);

            HandleBrightnessSliderInteraction(sliderRect, colorState);

            float relativeY = Mathf.Clamp01(1f - colorState.BrightnessPercent / 100f);
            float triangleY = sliderRect.y + relativeY * sliderRect.height;

            float triangleWidth = 10f;
            float triangleHeight = 12f;
            float triangleX = sliderRect.xMax + 4f; // Add margin to right side

            Rect triangleRect = new Rect(triangleX, triangleY - triangleHeight / 2f, triangleWidth, triangleHeight);
            GUI.DrawTexture(triangleRect, textureManager.GetOrCreateWhiteTriangleTexture());
        }
    }

    // -------------------- UI Widgets --------------------
    public static class ColorPickerWidgets
    {
        public static class LabeledFloatInput
        {
            public static bool Draw(Rect rect, string label, ref string inputBuffer, Action<float> onValueChanged)
            {
                Rect labelRect = new Rect(rect.x, rect.y, ColorPickerUIConstants.LabelWidth, rect.height);
                Widgets.Label(labelRect, label);

                Rect textFieldRect = new Rect(rect.x + ColorPickerUIConstants.LabelWidth + 5f, rect.y,
                                              rect.width - ColorPickerUIConstants.LabelWidth - 5f, rect.height);

                string newBuffer = Widgets.TextField(textFieldRect, inputBuffer);
                if (newBuffer != inputBuffer)
                {
                    inputBuffer = newBuffer;
                    if (float.TryParse(inputBuffer, out float parsed))
                    {
                        onValueChanged(parsed);
                        return true;
                    }
                }
                return false;
            }
        }

        public static class HexColorInput
        {
            public static bool Draw(Rect rect, string label, ref string hexInput, Func<string, bool> tryParseAndUpdateColor)
            {
                Rect labelRect = new Rect(rect.x, rect.y, ColorPickerUIConstants.HexLabelWidth, rect.height);
                Widgets.Label(labelRect, label);

                Rect textFieldRect = new Rect(rect.x + ColorPickerUIConstants.HexLabelWidth + 5f, rect.y,
                                              ColorPickerUIConstants.HexFieldWidth, rect.height);

                string newInput = Widgets.TextField(textFieldRect, hexInput);
                if (newInput != hexInput)
                {
                    hexInput = newInput;
                    if (tryParseAndUpdateColor(hexInput))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static class ButtonsRow
        {
            public static void Draw(Rect rect, string leftText, Action onLeftClick, string rightText, Action onRightClick)
            {
                Rect leftRect = new Rect(rect.x, rect.y, rect.width / 2f - ColorPickerUIConstants.ButtonSpacing / 2f, rect.height);
                Rect rightRect = new Rect(rect.x + rect.width / 2f + ColorPickerUIConstants.ButtonSpacing / 2f, rect.y,
                                           rect.width / 2f - ColorPickerUIConstants.ButtonSpacing / 2f, rect.height);

                if (Widgets.ButtonText(leftRect, leftText))
                {
                    onLeftClick?.Invoke();
                }

                if (Widgets.ButtonText(rightRect, rightText))
                {
                    onRightClick?.Invoke();
                }
            }
        }

        public static void DrawColorWheelSelectorDot(Rect colorWheelRect, float hueDegrees, float saturationPercent)
        {
            float radius = colorWheelRect.width / 2f;
            float radHue = hueDegrees * Mathf.Deg2Rad;
            float saturationRatio = Mathf.Clamp01(saturationPercent / 100f);
            Vector2 offset = new Vector2(Mathf.Cos(radHue), Mathf.Sin(radHue)) * saturationRatio * radius;
            Vector2 selectorPosition = colorWheelRect.center + offset;

            Rect dotRect = new Rect(selectorPosition.x - 3f, selectorPosition.y - 3f, 6f, 6f);
            Widgets.DrawBoxSolid(dotRect, ColorPickerUIConstants.SelectorDotBorderColor);
            Widgets.DrawBox(dotRect, 1);
            Widgets.DrawBoxSolid(new Rect(dotRect.x + 1, dotRect.y + 1, dotRect.width - 2, dotRect.height - 2), ColorPickerUIConstants.SelectorDotFillColor);
        }
    }

    // -------------------- Main Window --------------------
    public class Window_ColorPicker_Old : Window
    {
        private readonly Color _originalColor;
        private readonly Action<Color> _onSaveCallback;

        private readonly ColorPickerColorState _colorState;
        private readonly ColorPickerTextureManager _textureManager;
        private readonly ColorPickerUserInteractionHandler _interactionHandler;

        public Window_ColorPicker_Old(Color initialColor, Action<Color> onSaveCallback)
        {
            _originalColor = initialColor;
            _onSaveCallback = onSaveCallback;

            _colorState = new ColorPickerColorState(initialColor);
            _textureManager = new ColorPickerTextureManager(_colorState.HueDegrees, _colorState.SaturationPercent);
            _interactionHandler = new ColorPickerUserInteractionHandler();

            doCloseX = false;
            doCloseButton = false;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
        }

        public override Vector2 InitialSize => new Vector2(ColorPickerUIConstants.WindowWidth, ColorPickerUIConstants.WindowHeight);

        public override void DoWindowContents(Rect mainRect)
        {
            float totalWidth = mainRect.width;
            float totalHeight = mainRect.height;

            float wheelSize = ColorPickerUIConstants.ColorWheelSize;
            float brightnessSliderWidth = ColorPickerUIConstants.BrightnessSliderWidth;
            float brightnessSliderHeight = ColorPickerUIConstants.BrightnessSliderHeight;
            float padding = ColorPickerUIConstants.Padding;

            // --- Layout ---
            float topSectionHeight = wheelSize + 100f; // add extra height for labels and previews
            Rect topSectionRect = new Rect(mainRect.x, mainRect.y, totalWidth, topSectionHeight);
            Rect bottomSectionRect = new Rect(mainRect.x, mainRect.y + topSectionHeight + 10f, totalWidth, totalHeight - topSectionHeight - 10f);

            Rect colorWheelRect = new Rect(topSectionRect.x, topSectionRect.y + 20f, wheelSize, wheelSize);
            Rect brightnessSliderRect = new Rect(colorWheelRect.xMax + padding, colorWheelRect.y, brightnessSliderWidth, brightnessSliderHeight);

            float paletteWidth = totalWidth - brightnessSliderRect.xMax - padding;
            // Move palette down to avoid clipping, align Y with color wheel bottom + some padding
            Rect paletteRect = new Rect(brightnessSliderRect.xMax + padding, colorWheelRect.y, paletteWidth, wheelSize + 40f);

            DrawColorWheel(colorWheelRect);
            DrawBrightnessSlider(brightnessSliderRect);
            DrawColorPalette(paletteRect);

            DrawColorPreviewsWithLabels(colorWheelRect);

            DrawBottomControls(bottomSectionRect);
        }

        private void DrawColorPreviewsWithLabels(Rect colorWheelRect)
        {
            float padding = ColorPickerUIConstants.Padding;
            float previewSize = ColorPickerUIConstants.PreviewBoxSize;
            float labelHeight = ColorPickerUIConstants.PreviewLabelHeight;
            float labelWidth = ColorPickerUIConstants.PreviewLabelWidth;

            // Position previews below color wheel, spaced horizontally
            float startX = colorWheelRect.x;
            float startY = colorWheelRect.yMax + 10f;

            // Current Color Label
            Rect currentLabelRect = new Rect(startX, startY, labelWidth, labelHeight);
            GUI.color = _originalColor; // label text in original color
            Widgets.Label(currentLabelRect, "Current Color");

            // Current Color Box below label
            Rect currentBoxRect = new Rect(startX, currentLabelRect.yMax + 2f, previewSize, previewSize);
            GUI.color = Color.white;
            Widgets.DrawBoxSolid(currentBoxRect, _originalColor);
            Widgets.DrawBox(currentBoxRect, 2);

            // New Color Label (with padding between labels, not boxes)
            float newLabelX = currentBoxRect.xMax + (padding * 4); // increased padding to avoid clipping
            Rect newLabelRect = new Rect(newLabelX, startY, labelWidth, labelHeight);
            GUI.color = _colorState.CurrentColor; // label text in new color
            Widgets.Label(newLabelRect, "New Color");

            // New Color Box below label
            Rect newBoxRect = new Rect(newLabelX, newLabelRect.yMax + 2f, previewSize, previewSize);
            GUI.color = Color.white;
            Widgets.DrawBoxSolid(newBoxRect, _colorState.CurrentColor);
            Widgets.DrawBox(newBoxRect, 2);
        }

        private void DrawColorWheel(Rect rect)
        {
            _textureManager.UpdateHueAndSaturation(_colorState.HueDegrees, _colorState.SaturationPercent);

            Texture2D wheelTex = _textureManager.GetOrCreateColorWheelTexture();
            GUI.DrawTexture(rect, wheelTex);

            _interactionHandler.HandleColorWheelInteraction(rect, _colorState);
        }

        private void DrawBrightnessSlider(Rect rect)
        {
            _interactionHandler.DrawBrightnessSlider(rect, _textureManager, _colorState);
        }

        private void DrawColorPalette(Rect area)
        {
            var colors = DefDatabase<ColorDef>.AllDefsListForReading;
            if (colors == null || colors.Count == 0)
                return;

            float boxSize = ColorPickerUIConstants.PaletteBoxSize;
            float padding = ColorPickerUIConstants.PaletteCellSpacing;
            int cols = Mathf.FloorToInt(area.width / (boxSize + padding));
            if (cols < 1) cols = 1;

            int rows = Mathf.CeilToInt((float)colors.Count / cols);

            for (int i = 0; i < colors.Count; i++)
            {
                var colorDef = colors[i];
                int row = i / cols;
                int col = i % cols;

                Rect cellRect = new Rect(
                    area.x + col * (boxSize + padding),
                    area.y + row * (boxSize + padding),
                    boxSize,
                    boxSize);

                Widgets.DrawBoxSolid(cellRect, colorDef.color);
                if (Widgets.ButtonInvisible(cellRect))
                {
                    _colorState.SetColor(colorDef.color); // Your method to set current color & update UI buffers
                }

                string label = !colorDef.LabelCap.NullOrEmpty() ? colorDef.LabelCap.ToString() : colorDef.defName;
                TooltipHandler.TipRegion(cellRect, label);
            }
        }

        private void DrawBottomControls(Rect bottomRect)
        {
            const float margin = 40f;
            const float rowHeight = 32f;
            const float spacing = 10f;
            const float labelPadding = 6f;
            const float buttonWidth = 120f;
            const float buttonHeight = 38f;

            float listingHeight = bottomRect.height - buttonHeight - spacing - margin;
            Rect listingRect = new Rect(bottomRect.x, bottomRect.y, bottomRect.width, listingHeight);

            var listing = new Listing_Standard();
            listing.Begin(listingRect);

            void DrawRow(Action rowContent)
            {
                rowContent();
                listing.Gap(spacing * 1.5f); // Increased bottom margin
            }

            // Row 1: Hex + Alpha
            DrawRow(() =>
            {
                float totalWidth = bottomRect.width - 3 * margin;
                float hexWidth = totalWidth * 0.6f;
                float alphaWidth = totalWidth - hexWidth;

                Rect hexRect = listing.GetRect(rowHeight);
                hexRect.width = hexWidth;
                hexRect.x = bottomRect.xMin + margin;

                Rect alphaRect = hexRect;
                alphaRect.x += hexWidth + margin;
                alphaRect.width = alphaWidth;

                ColorPickerWidgets.HexColorInput.Draw(VerticallyCentered(hexRect), "Hex", ref _colorState.HexColorString, _colorState.TryParseHexColorString);
                ColorPickerWidgets.LabeledFloatInput.Draw(VerticallyCentered(alphaRect), "A", ref _colorState.AlphaInputBuffer, _colorState.UpdateAlpha);
            });

            // Row 2: RGB
            DrawRow(() =>
            {
                float sectionWidth = (bottomRect.width - 4 * margin) / 3f;

                Rect rRect = listing.GetRect(rowHeight);
                rRect.width = sectionWidth;
                rRect.x = bottomRect.xMin + margin;

                Rect gRect = rRect;
                gRect.x += sectionWidth + margin;

                Rect bRect = gRect;
                bRect.x += sectionWidth + margin;

                ColorPickerWidgets.LabeledFloatInput.Draw(VerticallyCentered(rRect), "R", ref _colorState.RedInputBuffer, _colorState.UpdateRed);
                ColorPickerWidgets.LabeledFloatInput.Draw(VerticallyCentered(gRect), "G", ref _colorState.GreenInputBuffer, _colorState.UpdateGreen);
                ColorPickerWidgets.LabeledFloatInput.Draw(VerticallyCentered(bRect), "B", ref _colorState.BlueInputBuffer, _colorState.UpdateBlue);
            });

            // Row 3: HSL
            DrawRow(() =>
            {
                float sectionWidth = (bottomRect.width - 4 * margin) / 3f;

                Rect hRect = listing.GetRect(rowHeight);
                hRect.width = sectionWidth;
                hRect.x = bottomRect.xMin + margin;

                Rect sRect = hRect;
                sRect.x += sectionWidth + margin;

                Rect lRect = sRect;
                lRect.x += sectionWidth + margin;

                ColorPickerWidgets.LabeledFloatInput.Draw(VerticallyCentered(hRect), "H", ref _colorState.HueInputBuffer, _colorState.UpdateHue);
                ColorPickerWidgets.LabeledFloatInput.Draw(VerticallyCentered(sRect), "S", ref _colorState.SaturationInputBuffer, _colorState.UpdateSaturation);
                ColorPickerWidgets.LabeledFloatInput.Draw(VerticallyCentered(lRect), "L", ref _colorState.BrightnessInputBuffer, _colorState.UpdateBrightness);
            });

            listing.End();

            // Buttons
            float buttonY = bottomRect.yMax - buttonHeight - spacing;

            Rect cancelButton = new Rect(bottomRect.xMin + margin, buttonY, buttonWidth, buttonHeight);
            Rect saveButton = new Rect(bottomRect.xMax - buttonWidth - margin, buttonY, buttonWidth, buttonHeight);

            if (Widgets.ButtonText(cancelButton, "Cancel"))
                Close();

            if (Widgets.ButtonText(saveButton, "Save"))
            {
                _onSaveCallback?.Invoke(_colorState.CurrentColor);
                Close();
            }

            // Helper function to center controls vertically inside their row
            Rect VerticallyCentered(Rect r)
            {
                r.y += (rowHeight - Text.LineHeight) / 2f - 1f;
                return r;
            }
        }
    }
}
