using QualityColors;
using System;
using UnityEngine;

public class ColorPickerUserInteractionHandler
{
    private bool isDraggingColorWheel;
    private const int LeftMouseButton = 0;

    public void HandleColorWheelInteraction(Rect colorWheelRect, ColorPickerColorState colorState)
    {
        if (Event.current.button != LeftMouseButton)
            return;

        // Prevent interaction if any text field has keyboard focus
        if (GUI.GetNameOfFocusedControl()?.StartsWith(ColorPickerConstants.InputPrefix) == true)
            return;

        Vector2 mousePosition = Event.current.mousePosition;
        Vector2 offsetFromCenter = mousePosition - colorWheelRect.center;
        double colorWheelRadius = colorWheelRect.width / 2.0;
        double distanceFromCenter = offsetFromCenter.magnitude;

        switch (Event.current.type)
        {
            case EventType.MouseDown when distanceFromCenter <= colorWheelRadius:
                isDraggingColorWheel = true;
                Event.current.Use(); // claim the event
                break;

            case EventType.MouseUp:
                isDraggingColorWheel = false;
                break;

            case EventType.MouseDrag when isDraggingColorWheel:
                Event.current.Use(); // claim the drag
                break;
        }

        if (isDraggingColorWheel)
        {
            double hueDegrees = (Math.Atan2(offsetFromCenter.y, offsetFromCenter.x) * Mathf.Rad2Deg
                                 + ColorPickerConstants.HueMaxDegrees) % ColorPickerConstants.HueMaxDegrees;

            double saturationPercent = Math.Min(distanceFromCenter / colorWheelRadius, 1.0) * ColorPickerConstants.PercentageMax;

            colorState.SetHue(hueDegrees);
            colorState.SetSaturation(saturationPercent);
        }
    }

    public void HandleBrightnessSliderInteraction(Rect sliderRect, ColorPickerColorState colorState)
    {
        if (!(Event.current.type is EventType.MouseDown or EventType.MouseDrag) || Event.current.button != LeftMouseButton)
            return;

        Vector2 mousePosition = Event.current.mousePosition;

        Rect expandedSliderRect = new Rect(
            sliderRect.x,
            sliderRect.y,
            sliderRect.width + ColorPickerConstants.BrightnessSliderExtraWidth,
            sliderRect.height
        );

        if (!expandedSliderRect.Contains(mousePosition))
            return;

        double relativeVerticalPosition = (mousePosition.y - sliderRect.y) / sliderRect.height;
        double clampedPosition = Math.Clamp(relativeVerticalPosition, 0.0, 1.0);
        double lightnessPercent = (1.0 - clampedPosition) * ColorPickerConstants.PercentageMax;

        colorState.SetLightness(lightnessPercent);
        Event.current.Use();
    }

    public void HandleInputFieldInteraction<T>(InputFieldState<T> inputFieldState, string expectedControlName)
    {
        string focusedControlName = GUI.GetNameOfFocusedControl();
        bool isInputFieldNowFocused = focusedControlName == expectedControlName;

        if (!inputFieldState.IsUserFocused && isInputFieldNowFocused)
        {
            inputFieldState.Focus(inputFieldState.GetDisplayValue(string.Empty));
        }
        else if (inputFieldState.IsUserFocused && !isInputFieldNowFocused)
        {
            inputFieldState.Unfocus();
        }
    }

    public bool HandleTextFieldChange<T>(
        InputFieldState<T> inputFieldState,
        string updatedBuffer,
        Func<string, (bool isValid, T value)> tryParseCallback,
        Action<T> applyValidValue)
    {
        if (!inputFieldState.IsUserFocused || updatedBuffer == inputFieldState.UserInputBuffer)
            return false;

        inputFieldState.UpdateBuffer(updatedBuffer);

        var (isValid, parsedValue) = tryParseCallback(updatedBuffer);
        if (isValid)
        {
            applyValidValue(parsedValue);
            return true;
        }

        return false;
    }
}
