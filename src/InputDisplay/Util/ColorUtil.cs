using System.Globalization;

namespace InputDisplay.Util;

public static class ColorUtil
{
    public static string ToHex(this Color color) =>
        $"{color.R:X2}{color.G:X2}{color.B:X2}";


    public static Color FromHex(string value)
    {
        var colorCode = value.ToUpperInvariant().TrimStart('#');
        try
        {
            return new(
                byte.Parse(colorCode[..2], NumberStyles.HexNumber),
                byte.Parse(colorCode.Substring(2, 2), NumberStyles.HexNumber),
                byte.Parse(colorCode.Substring(4, 2), NumberStyles.HexNumber),
                (byte)255
            );
        }
        catch
        {
            return Color.Pink;
        }
    }
}
