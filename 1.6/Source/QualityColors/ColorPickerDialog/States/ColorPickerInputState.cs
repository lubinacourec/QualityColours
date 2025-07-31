using QualityColors;

public class ColorPickerInputState
{
    public readonly InputFieldState<int> RedChannelInput = new();
    public readonly InputFieldState<int> GreenChannelInput = new();
    public readonly InputFieldState<int> BlueChannelInput = new();
    public readonly InputFieldState<int> AlphaChannelInput = new();

    public readonly InputFieldState<double> HueDegreesInput = new();
    public readonly InputFieldState<double> SaturationPercentInput = new();
    public readonly InputFieldState<double> LightnessPercentInput = new();

    public readonly InputFieldState<string> HexColorInput = new();

    public ColorPickerInputState()
    {
        RedChannelInput.InputSanitizer = ParserUtils.IntFilter;
        GreenChannelInput.InputSanitizer = ParserUtils.IntFilter;
        BlueChannelInput.InputSanitizer = ParserUtils.IntFilter;
        AlphaChannelInput.InputSanitizer = ParserUtils.IntFilter;

        HueDegreesInput.InputSanitizer = ParserUtils.DoubleFilter;
        SaturationPercentInput.InputSanitizer = ParserUtils.DoubleFilter;
        LightnessPercentInput.InputSanitizer = ParserUtils.DoubleFilter;

        HexColorInput.InputSanitizer = ParserUtils.HexFilter;
    }
}
