namespace WxSharp;

/// <summary>An RGBA colour, packed as 0xAARRGGBB at the native boundary.</summary>
public readonly record struct Colour(byte R, byte G, byte B, byte A = 255)
{
    /// <summary>The colour packed as 0xAARRGGBB.</summary>
    public uint ToArgb() => ((uint)A << 24) | ((uint)R << 16) | ((uint)G << 8) | B;

    /// <summary>Unpacks a colour from 0xAARRGGBB.</summary>
    public static Colour FromArgb(uint value)
        => new((byte)(value >> 16), (byte)(value >> 8), (byte)value, (byte)(value >> 24));
    public static Colour Rgb(byte r, byte g, byte b) => new(r, g, b);
    public static Colour Black => new(0, 0, 0);
    public static Colour White => new(255, 255, 255);
    public static Colour Red => new(255, 0, 0);
    public static Colour Green => new(0, 128, 0);
    public static Colour Blue => new(0, 0, 255);
    public static Colour Yellow => new(255, 255, 0);
    public static Colour Transparent => new(0, 0, 0, 0);
    public override string ToString() => $"#{A:X2}{R:X2}{G:X2}{B:X2}";

    /// <summary>Reads a colour written the way wxWidgets writes them: one of the standard colour names
    /// ("cornflower blue"), <c>#RRGGBB</c>, or CSS <c>rgb(...)</c> and <c>rgba(...)</c> notation. False when
    /// the text names no colour.</summary>
    public static bool TryParse(string text, out Colour colour)
    {
        colour = default;
        if (string.IsNullOrWhiteSpace(text)) return false;
        _ = App.RequireCurrent();
        if (!NativeMethods.wxsharp_colour_parse(text, out var argb)) return false;
        colour = FromArgb(argb);
        return true;
    }

    /// <summary>Reads a colour, throwing when the text names none.</summary>
    public static Colour Parse(string text)
        => TryParse(text, out var colour) ? colour : throw new FormatException($"'{text}' is not a colour.");

    /// <summary>Always true. <c>wxColour</c> has an uninitialised state to guard against; a
    /// <see cref="Colour"/> is a value type whose every value names a colour.</summary>
    public bool IsOk => true;

    /// <summary>Whether the colour is fully opaque.</summary>
    public bool IsOpaque => A == 255;

    /// <summary>Whether the colour is fully transparent.</summary>
    public bool IsTransparent => A == 0;

    /// <summary>Whether the colour is either fully opaque or fully transparent, and so needs no
    /// blending.</summary>
    public bool IsSolid => A == 255 || A == 0;

    /// <summary>Whether the colour is partly transparent and has to be blended with what is behind it.</summary>
    public bool IsTranslucent => !IsSolid;

    /// <summary>How bright the colour is, from 0 for black to 1 for white. Worth checking before pairing
    /// two colours: text and background too close in luminance are hard to read.</summary>
    public double Luminance
    {
        get { _ = App.RequireCurrent(); return NativeMethods.wxsharp_colour_luminance(ToArgb()); }
    }

    /// <summary>A lighter or darker version of this colour. 100 leaves it unchanged, below 100 darkens,
    /// above 100 lightens.</summary>
    public Colour ChangeLightness(int lightness)
    {
        _ = App.RequireCurrent();
        return FromArgb(NativeMethods.wxsharp_colour_change_lightness(ToArgb(), lightness));
    }

    /// <summary>The washed-out version of this colour used to draw a disabled control.</summary>
    public Colour MakeDisabled(byte brightness = 255)
    {
        _ = App.RequireCurrent();
        return FromArgb(NativeMethods.wxsharp_colour_make_disabled(ToArgb(), brightness));
    }

    /// <summary>The greyscale version of this colour.</summary>
    public Colour MakeGrey()
    {
        _ = App.RequireCurrent();
        return FromArgb(NativeMethods.wxsharp_colour_make_grey(ToArgb()));
    }

    /// <summary>Black or white, whichever <paramref name="on"/> selects.</summary>
    public Colour MakeMono(bool on)
    {
        _ = App.RequireCurrent();
        return FromArgb(NativeMethods.wxsharp_colour_make_mono(ToArgb(), on));
    }

    /// <summary>Blends one channel value over another. <paramref name="alpha"/> runs from 0 for all
    /// background to 1 for all foreground.</summary>
    public static byte AlphaBlend(byte foreground, byte background, double alpha)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_colour_alpha_blend(foreground, background, alpha);
    }

    /// <summary>The standard name for this colour where it has one, and CSS <c>rgb(...)</c> notation where
    /// it does not.</summary>
    public unsafe string ToName()
    {
        _ = App.RequireCurrent();
        var argb = ToArgb();
        var length = NativeMethods.wxsharp_colour_name(argb, null, 0);
        if (length <= 0) return ToString();
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_colour_name(argb, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
}
