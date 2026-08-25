namespace WxSharp;

/// <summary>A width/height pair in device pixels.</summary>
public readonly record struct Size(int Width, int Height)
{
    public override string ToString() => $"{Width}x{Height}";
}

/// <summary>An x/y position in device pixels, relative to the parent's client area.</summary>
public readonly record struct Point(int X, int Y)
{
    public override string ToString() => $"({X}, {Y})";
}
