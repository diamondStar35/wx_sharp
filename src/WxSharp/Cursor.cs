using System;

namespace WxSharp;

/// <summary>One of the platform's own cursors, following <c>wxStockCursor</c>.</summary>
public enum StockCursor
{
    /// <summary>No cursor; the window shows no pointer at all.</summary>
    None = 0,
    /// <summary>The standard arrow pointer.</summary>
    Arrow = 1,
    /// <summary>An arrow pointing to the upper right.</summary>
    RightArrow = 2,
    /// <summary>A bullseye of concentric circles.</summary>
    Bullseye = 3,
    /// <summary>A text-character cursor.</summary>
    Character = 4,
    /// <summary>A crosshair, for precise pointing.</summary>
    Cross = 5,
    /// <summary>An open hand, marking a draggable or clickable spot.</summary>
    Hand = 6,
    /// <summary>The I-beam shown over editable text.</summary>
    IBeam = 7,
    /// <summary>A left mouse-button cursor; not available on every platform.</summary>
    LeftButton = 8,
    /// <summary>A magnifying glass.</summary>
    Magnifier = 9,
    /// <summary>A middle mouse-button cursor; not available on every platform.</summary>
    MiddleButton = 10,
    /// <summary>The "no entry" forbidden symbol.</summary>
    NoEntry = 11,
    /// <summary>A paintbrush.</summary>
    PaintBrush = 12,
    /// <summary>A pencil.</summary>
    Pencil = 13,
    /// <summary>A hand pointing left.</summary>
    PointLeft = 14,
    /// <summary>A hand pointing right.</summary>
    PointRight = 15,
    /// <summary>An arrow with a question mark, for context help.</summary>
    QuestionArrow = 16,
    /// <summary>A right mouse-button cursor; not available on every platform.</summary>
    RightButton = 17,
    /// <summary>A diagonal resize arrow running north-east to south-west.</summary>
    SizeNeSw = 18,
    /// <summary>A vertical resize arrow (north-south).</summary>
    SizeNs = 19,
    /// <summary>A diagonal resize arrow running north-west to south-east.</summary>
    SizeNwSe = 20,
    /// <summary>A horizontal resize arrow (west-east).</summary>
    SizeWe = 21,
    /// <summary>A general sizing cursor.</summary>
    Sizing = 22,
    /// <summary>A spray-can.</summary>
    SprayCan = 23,
    /// <summary>An hourglass shown while the application is busy and unresponsive.</summary>
    Wait = 24,
    /// <summary>A watch, an alternative "busy" cursor.</summary>
    Watch = 25,
    /// <summary>An invisible cursor.</summary>
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

    /// <summary>Releases the native cursor.</summary>
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
    /// <summary>No or unknown format.</summary>
    Invalid = 0,
    /// <summary>Windows bitmap (<c>.bmp</c>).</summary>
    Bmp = 1,
    /// <summary>Windows icon (<c>.ico</c>).</summary>
    Ico = 3,
    /// <summary>Windows cursor (<c>.cur</c>), carrying its own hotspot.</summary>
    Cur = 5,
    /// <summary>PNG image.</summary>
    Png = 15,
}
