using System;

namespace WxSharp;

/// <summary>One of the platform's own cursors, following <c>wxStockCursor</c>.</summary>
public enum StockCursor
{
    None = 0,
    Arrow = 1,
    RightArrow = 2,
    Bullseye = 3,
    Character = 4,
    Cross = 5,
    Hand = 6,
    IBeam = 7,
    LeftButton = 8,
    Magnifier = 9,
    MiddleButton = 10,
    NoEntry = 11,
    PaintBrush = 12,
    Pencil = 13,
    PointLeft = 14,
    PointRight = 15,
    QuestionArrow = 16,
    RightButton = 17,
    SizeNeSw = 18,
    SizeNs = 19,
    SizeNwSe = 20,
    SizeWe = 21,
    Sizing = 22,
    SprayCan = 23,
    Wait = 24,
    Watch = 25,
    Blank = 26,
    /// <summary>The hourglass shown while the application is busy but still responding.</summary>
    ArrowWait = 27,
}

/// <summary>A mouse cursor, following <c>wxCursor</c>.</summary>
///
/// <remarks>
/// The cursor is a real hint about what a control will do - a resize handle, a link, a place text can be
/// typed - so setting the right one is worth doing. It is only a hint, though: it says nothing to a screen
/// reader and nothing at all to a keyboard user, so it should never be the only way something is signalled.
/// </remarks>
public sealed class Cursor : IDisposable
{
    private nint _handle;

    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Cursor));

    private Cursor(nint handle) => _handle = handle;

    internal static Cursor Attach(nint handle) => new(handle);

    /// <summary>One of the platform's own cursors.</summary>
    public Cursor(StockCursor cursor)
    {
        _ = App.RequireCurrent();
        _handle = NativeMethods.wxsharp_cursor_create_stock((int)cursor);
        if (_handle == 0) throw new ArgumentException($"The platform has no {cursor} cursor.", nameof(cursor));
    }

    /// <summary>Loads a cursor from a file. The hotspot is the pixel the pointer actually points with,
    /// which matters for anything but an arrow.</summary>
    public static Cursor? FromFile(string path, BitmapType type = BitmapType.Cur, int hotspotX = 0, int hotspotY = 0)
    {
        ArgumentNullException.ThrowIfNull(path);
        _ = App.RequireCurrent();
        var handle = NativeMethods.wxsharp_cursor_create_from_file(path, (int)type, hotspotX, hotspotY);
        return handle == 0 ? null : new Cursor(handle);
    }

    /// <summary>Whether the cursor loaded successfully.</summary>
    public bool IsOk => _handle != 0 && NativeMethods.wxsharp_cursor_is_ok(_handle);

    /// <summary>Sets the cursor for the whole application, over every window, until it is set back. This is
    /// what a busy application shows; prefer <see cref="Wx.BusyCursor"/>, which puts it back for you.</summary>
    public static void SetGlobal(Cursor? cursor)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_cursor_set_global(cursor?.Handle ?? 0);
    }

    public void Dispose()
    {
        if (_handle != 0) NativeMethods.wxsharp_cursor_destroy(_handle);
        _handle = 0;
    }
}

/// <summary>An image file format, following <c>wxBitmapType</c>. Only the values a cursor or icon is
/// normally loaded from are named; wxWidgets defines more.</summary>
public enum BitmapType
{
    Invalid = 0,
    Bmp = 1,
    Ico = 3,
    Cur = 5,
    Png = 15,
}
