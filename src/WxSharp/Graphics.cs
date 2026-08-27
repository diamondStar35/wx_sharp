using System;

namespace WxSharp;

public readonly record struct Brush(Colour Colour);
public readonly record struct Pen(Colour Colour, int Width = 1);

public sealed class Image : IDisposable
{
    private nint _handle;
    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Image));
    public Image(string path)
    {
        App.RequireCurrent(); ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _handle = NativeMethods.wxsharp_image_load(path);
        if (_handle == 0) throw new ArgumentException("The image could not be loaded.", nameof(path));
    }
    public int Width => NativeMethods.wxsharp_image_width(Handle);
    public int Height => NativeMethods.wxsharp_image_height(Handle);
    public Size Size => new(Width, Height);
    public bool Save(string path) => NativeMethods.wxsharp_image_save(Handle, path);
    public void Dispose() { if (_handle != 0) NativeMethods.wxsharp_image_destroy(_handle); _handle = 0; }
}

public sealed class Bitmap : IDisposable
{
    private nint _handle;
    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Bitmap));

    /// <summary>Wraps a bitmap wxWidgets handed us, such as one read from the clipboard. The caller owns
    /// it from here.</summary>
    internal static Bitmap Attach(nint handle) => new(handle);

    private Bitmap(nint handle) => _handle = handle;

    public Bitmap(string path)
    {
        App.RequireCurrent(); ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _handle = NativeMethods.wxsharp_bitmap_load(path);
        if (_handle == 0) throw new ArgumentException("The bitmap could not be loaded.", nameof(path));
    }
    public Bitmap(Image image)
    {
        ArgumentNullException.ThrowIfNull(image); _handle = NativeMethods.wxsharp_bitmap_from_image(image.Handle);
        if (_handle == 0) throw new InvalidOperationException("The bitmap could not be created.");
    }
    public int Width => NativeMethods.wxsharp_bitmap_width(Handle);
    public int Height => NativeMethods.wxsharp_bitmap_height(Handle);
    public Size Size => new(Width, Height);
    public void Dispose() { if (_handle != 0) NativeMethods.wxsharp_bitmap_destroy(_handle); _handle = 0; }
}

public sealed class Icon : IDisposable
{
    private nint _handle;
    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Icon));
    public Icon(string path)
    {
        App.RequireCurrent(); _handle = NativeMethods.wxsharp_icon_load(path);
        if (_handle == 0) throw new ArgumentException("The icon could not be loaded.", nameof(path));
    }
    internal static Icon Attach(nint handle) => new(handle);
    private Icon(nint handle) => _handle = handle;
    public void Dispose() { if (_handle != 0) NativeMethods.wxsharp_icon_destroy(_handle); _handle = 0; }
}
