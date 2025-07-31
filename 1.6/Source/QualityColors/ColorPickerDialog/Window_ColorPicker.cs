using System;
using UnityEngine;
using Verse;

namespace QualityColors
{
    public class Window_ColorPicker : Window
    {
        private readonly Color originalColor;
        private readonly Action<Color> onSaveColorCallback;

        private readonly ColorPickerColorState colorState;
        private readonly ColorPickerTextureManager textureManager;
        private readonly ColorPickerUserInteractionHandler userInteractionHandler;

        public Window_ColorPicker(Color initialColor, Action<Color> onSaveCallback)
        {
            originalColor = initialColor;
            onSaveColorCallback = onSaveCallback;

            colorState = new ColorPickerColorState(initialColor);
            textureManager = new ColorPickerTextureManager(colorState.GetHue(), colorState.GetSaturation());
            userInteractionHandler = new ColorPickerUserInteractionHandler();

            doCloseX = false;
            doCloseButton = false;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
        }

        public override Vector2 InitialSize => new Vector2(ColorPickerConstants.WindowWidth, ColorPickerConstants.WindowHeight);

        public override void DoWindowContents(Rect mainRect)
        {
            ColorPickerLayoutHelper layoutHelper = new ColorPickerLayoutHelper(mainRect);

            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl(null); // Unfocus input box when clicking outside
            }

            ColorPickerDrawer.DrawColorWheel(layoutHelper, colorState, textureManager);
            userInteractionHandler.HandleColorWheelInteraction(layoutHelper.ColorWheelRect, colorState);

            ColorPickerDrawer.DrawBrightnessSlider(layoutHelper, colorState, textureManager);
            userInteractionHandler.HandleBrightnessSliderInteraction(layoutHelper.BrightnessSliderRect, colorState);

            ColorPickerDrawer.DrawPalette(layoutHelper, colorState);
            ColorPickerDrawer.DrawColorPreviews(layoutHelper, originalColor, colorState.GetUnityColor);

            ColorPickerDrawer.DrawInputSection(layoutHelper, colorState, userInteractionHandler);
            ColorPickerDrawer.DrawActionButtonsSection(layoutHelper, colorState, onSaveColorCallback, () => Close());
        }

        public override void PreClose()
        {
            base.PreClose();
            textureManager.Dispose();
        }
    }
}
