using System;

namespace WxSharp;

/// <summary>A solid fill colour for drawing, following <c>wxBrush</c>.</summary>
public readonly record struct Brush(Colour Colour);
/// <summary>A line colour and width for strokes and outlines, following <c>wxPen</c>.</summary>
public readonly record struct Pen(Colour Colour, int Width = 1);

/// <summary>A platform-independent, in-memory image, following <c>wxImage</c>. Loaded from a file and
/// usually turned into a <see cref="Bitmap"/> before it is drawn.</summary>
public sealed class Image : IDisposable
{
    private nint _handle;
    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Image));
    /// <summary>Loads an image from a file, detecting the format from its contents. Throws when the file
    /// cannot be loaded.</summary>
    public Image(string path)
    {
        App.RequireCurrent(); ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _handle = NativeMethods.wxsharp_image_load(path);
        if (_handle == 0) throw new ArgumentException("The image could not be loaded.", nameof(path));
    }
    /// <summary>The width in pixels.</summary>
    public int Width => NativeMethods.wxsharp_image_width(Handle);
    /// <summary>The height in pixels.</summary>
    public int Height => NativeMethods.wxsharp_image_height(Handle);
    /// <summary>The size in pixels.</summary>
    public Size Size => new(Width, Height);
    /// <summary>Writes the image to a file, choosing the format from the extension. False on failure.</summary>
    public bool Save(string path) => NativeMethods.wxsharp_image_save(Handle, path);
    /// <summary>Releases the native image.</summary>
    public void Dispose() { if (_handle != 0) NativeMethods.wxsharp_image_destroy(_handle); _handle = 0; }
}

/// <summary>A device-dependent image ready to be drawn to screen, following <c>wxBitmap</c>.</summary>
public sealed class Bitmap : IDisposable
{
    private nint _handle;
    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Bitmap));

    /// <summary>Wraps a bitmap wxWidgets handed us, such as one read from the clipboard. The caller owns
    /// it from here.</summary>
    internal static Bitmap Attach(nint handle) => new(handle);

    private Bitmap(nint handle) => _handle = handle;

    /// <summary>Loads a bitmap from a file. Throws when the file cannot be loaded.</summary>
    public Bitmap(string path)
    {
        App.RequireCurrent(); ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _handle = NativeMethods.wxsharp_bitmap_load(path);
        if (_handle == 0) throw new ArgumentException("The bitmap could not be loaded.", nameof(path));
    }
    /// <summary>Converts an <see cref="Image"/> into a bitmap for drawing.</summary>
    public Bitmap(Image image)
    {
        ArgumentNullException.ThrowIfNull(image); _handle = NativeMethods.wxsharp_bitmap_from_image(image.Handle);
        if (_handle == 0) throw new InvalidOperationException("The bitmap could not be created.");
    }
    /// <summary>The width in pixels.</summary>
    public int Width => NativeMethods.wxsharp_bitmap_width(Handle);
    /// <summary>The height in pixels.</summary>
    public int Height => NativeMethods.wxsharp_bitmap_height(Handle);
    /// <summary>The size in pixels.</summary>
    public Size Size => new(Width, Height);
    /// <summary>Releases the native bitmap.</summary>
    public void Dispose() { if (_handle != 0) NativeMethods.wxsharp_bitmap_destroy(_handle); _handle = 0; }
}

/// <summary>A small image for a window title bar or taskbar, following <c>wxIcon</c>.</summary>
public sealed class Icon : IDisposable
{
    private nint _handle;
    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Icon));
    /// <summary>Loads an icon from a file. Throws when the file cannot be loaded.</summary>
    public Icon(string path)
    {
        App.RequireCurrent(); _handle = NativeMethods.wxsharp_icon_load(path);
        if (_handle == 0) throw new ArgumentException("The icon could not be loaded.", nameof(path));
    }
    internal static Icon Attach(nint handle) => new(handle);
    private Icon(nint handle) => _handle = handle;
    /// <summary>Releases the native icon.</summary>
    public void Dispose() { if (_handle != 0) NativeMethods.wxsharp_icon_destroy(_handle); _handle = 0; }
}
