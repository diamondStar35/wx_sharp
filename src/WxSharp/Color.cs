namespace WxSharp;

/// <summary>An RGBA colour. Crosses to the native side as a packed 0xAARRGGBB value.</summary>
public readonly record struct Color(byte R, byte G, byte B, byte A = 255)
{
    /// <summary>Packs the colour into a 0xAARRGGBB integer for the native ABI.</summary>
    internal uint ToArgb() => ((uint)A << 24) | ((uint)R << 16) | ((uint)G << 8) | B;

    /// <summary>Unpacks a 0xAARRGGBB integer from the native ABI.</summary>
    internal static Color FromArgb(uint v)
        => new((byte)(v >> 16), (byte)(v >> 8), (byte)v, (byte)(v >> 24));

    /// <summary>Creates an opaque colour from red/green/blue components.</summary>
    public static Color Rgb(byte r, byte g, byte b) => new(r, g, b);

    public static Color Black => new(0, 0, 0);
    public static Color White => new(255, 255, 255);
    public static Color Red => new(255, 0, 0);
    public static Color Green => new(0, 128, 0);
    public static Color Blue => new(0, 0, 255);
    public static Color Yellow => new(255, 255, 0);
    public static Color Transparent => new(0, 0, 0, 0);

    public override string ToString() => $"#{A:X2}{R:X2}{G:X2}{B:X2}";
}
