namespace WxSharp;

/// <summary>An RGBA colour, packed as 0xAARRGGBB at the native boundary.</summary>
public readonly record struct Colour(byte R, byte G, byte B, byte A = 255)
{
    internal uint ToArgb() => ((uint)A << 24) | ((uint)R << 16) | ((uint)G << 8) | B;
    internal static Colour FromArgb(uint value) => new((byte)(value >> 16), (byte)(value >> 8), (byte)value, (byte)(value >> 24));
    public static Colour Rgb(byte r, byte g, byte b) => new(r, g, b);
    public static Colour Black => new(0, 0, 0);
    public static Colour White => new(255, 255, 255);
    public static Colour Red => new(255, 0, 0);
    public static Colour Green => new(0, 128, 0);
    public static Colour Blue => new(0, 0, 255);
    public static Colour Yellow => new(255, 255, 0);
    public static Colour Transparent => new(0, 0, 0, 0);
    public override string ToString() => $"#{A:X2}{R:X2}{G:X2}{B:X2}";
}
