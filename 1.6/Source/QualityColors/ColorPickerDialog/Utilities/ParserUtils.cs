using System;
using System.Globalization;
using System.Linq;

namespace QualityColors
{
    public static class ParserUtils
    {
        public static bool IsValidHex(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                return false;

            string normalizedHex = hexString.TrimStart('#');
            return (normalizedHex.Length == 6 || normalizedHex.Length == 8)
                   && normalizedHex.All(IsHexDigit);
        }

        public static string HexFilter(string rawInput) =>
            string.Concat(rawInput.Where(IsHexDigit));

        public static string IntFilter(string rawInput) =>
            string.Concat(rawInput.Where(char.IsDigit));

        public static string DoubleFilter(string rawInput)
        {
            bool decimalPointAlreadySeen = false;
            return string.Concat(rawInput.Where(character =>
            {
                if (char.IsDigit(character)) return true;
                if (character == '.' && !decimalPointAlreadySeen)
                {
                    decimalPointAlreadySeen = true;
                    return true;
                }
                return false;
            }));
        }

        public static bool IsHexDigit(char character) =>
            (character >= '0' && character <= '9') ||
            (character >= 'a' && character <= 'f') ||
            (character >= 'A' && character <= 'F');

        public static (bool isValid, string normalizedHex) TryParseHex(string hexInput)
        {
            string normalizedHex = hexInput.TrimStart('#');
            return (IsValidHex(normalizedHex), normalizedHex);
        }

        public static (bool isValid, int parsedValue) TryParseInt(string intInput) =>
            (int.TryParse(intInput, out int parsedValue), parsedValue);

        public static (bool isValid, double parsedValue) TryParseDouble(string doubleInput) =>
            (double.TryParse(doubleInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue), parsedValue);
    }
}
