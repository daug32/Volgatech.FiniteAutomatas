using System.Drawing;

namespace FiniteAutomatas.Visualizations.Implementation;

public static class ColorExtensions
{
    public static Color Avg( this Color f, Color s )
    {
        return Color.FromArgb(
            alpha: ( f.A + s.A ) / 2,
            red: ( f.R + s.R ) / 2,
            green: ( f.G + s.G ) / 2,
            blue: ( f.B + s.B ) / 2 );
    }

    public static string ToHex( this Color color ) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
}