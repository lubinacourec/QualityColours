using System;

namespace QualityColors
{
    public class InputFieldState<T>
    {
        // The temporary user-edited input buffer (only active while focused)
        public string UserInputBuffer { get; private set; } = string.Empty;

        // Whether the input field is currently focused by the user
        public bool IsUserFocused { get; private set; }

        // Optional filter function to sanitize input (e.g., strip non-numeric chars)
        public Func<string, string>? InputSanitizer { get; set; }

        // Called when the input field gains focus. Initializes the buffer with the current displayed value.
        public void Focus(string currentDisplayedValue)
        {
            IsUserFocused = true;
            UserInputBuffer = currentDisplayedValue;
        }

        // Called when the input field loses focus. Clears the user-edited buffer.
        public void Unfocus()
        {
            IsUserFocused = false;
            UserInputBuffer = string.Empty;
        }

        // Updates the buffer with user input, applying the optional sanitizer if set.
        public void UpdateBuffer(string rawInput)
        {
            UserInputBuffer = InputSanitizer != null
                ? InputSanitizer(rawInput)
                : rawInput;
        }

        // Returns the string to display in the input box, depending on focus state.
        public string GetDisplayValue(string fallbackValue)
        {
            return IsUserFocused ? UserInputBuffer : fallbackValue;
        }
    }
}
