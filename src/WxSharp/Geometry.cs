namespace WxSharp;

/// <summary>A width/height pair in device pixels.</summary>
public readonly record struct Size(int Width, int Height)
{
    /// <summary>Formats the size as <c>WxH</c>, e.g. <c>640x480</c>.</summary>
    public override string ToString() => $"{Width}x{Height}";
}

/// <summary>An x/y position in device pixels, relative to the parent's client area.</summary>
public readonly record struct Point(int X, int Y)
{
    /// <summary>Formats the point as <c>(X, Y)</c>.</summary>
    public override string ToString() => $"({X}, {Y})";
}

/// <summary>A rectangle in screen or client coordinates, as documented by the consuming API.</summary>
public readonly record struct Rect(int X, int Y, int Width, int Height)
{
    /// <summary>The top-left corner as a <see cref="Point"/>.</summary>
    public Point Position => new(X, Y);
    /// <summary>The width and height as a <see cref="Size"/>.</summary>
    public Size Size => new(Width, Height);
}
