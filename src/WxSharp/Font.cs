namespace WxSharp;

/// <summary>A generic font family; the native side resolves it to a concrete typeface when no
/// <see cref="Font.Face"/> is given.</summary>
public enum FontFamily
{
    Default = 0,
    Serif = 1,
    Script = 2,
    SansSerif = 3,
    Modern = 4,   // a fixed-pitch font suited to code/tabular text
    Teletype = 5,
}

public enum FontWeight
{
    Light = 1,
    Normal = 0,
    Bold = 2,
}

public enum FontStyle
{
    Normal = 0,
    Italic = 1,
    Slant = 2,
}

/// <summary>Describes a font to apply to a control via <see cref="Control.SetFont"/>. A <see cref="PointSize"/>
/// of 0 keeps the system default size; an empty <see cref="Face"/> lets the <see cref="Family"/> pick the
/// typeface.</summary>
public sealed class Font
{
    public int PointSize { get; }
    public FontFamily Family { get; }
    public FontWeight Weight { get; }
    public FontStyle Style { get; }
    public bool Underline { get; }
    public string? Face { get; }

    public Font(int pointSize = 0, FontFamily family = FontFamily.Default, FontWeight weight = FontWeight.Normal,
        FontStyle style = FontStyle.Normal, bool underline = false, string? face = null)
    {
        PointSize = pointSize;
        Family = family;
        Weight = weight;
        Style = style;
        Underline = underline;
        Face = face;
    }

    /// <summary>A bold copy of this font.</summary>
    public Font Bold() => new(PointSize, Family, FontWeight.Bold, Style, Underline, Face);

    /// <summary>An italic copy of this font.</summary>
    public Font Italic() => new(PointSize, Family, Weight, FontStyle.Italic, Underline, Face);

    /// <summary>A copy of this font at a different point size.</summary>
    public Font WithSize(int pointSize) => new(pointSize, Family, Weight, Style, Underline, Face);
}
