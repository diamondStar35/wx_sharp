using System;

namespace WxSharp;

/// <summary>A monitor attached to the machine, following <c>wxDisplay</c>.</summary>
///
/// <remarks>
/// This is what a window has to consult before restoring a position it saved last time: the screen that
/// position was on may not be attached now, and a window placed on a screen that is gone is invisible with
/// no way for the user to retrieve it. <see cref="GetFromPoint"/> answers that question directly.
///
/// A display is identified by index, and indices shift when monitors are attached or removed, so read one
/// when you need it rather than holding it.
/// </remarks>
public readonly unsafe struct Display
{
    /// <summary>The display's index, which is only stable while the monitors are.</summary>
    public uint Index { get; }

    public Display(uint index)
    {
        _ = App.RequireCurrent();
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
        Index = index;
    }

    /// <summary>How many monitors are attached.</summary>
    public static uint Count
    {
        get { _ = App.RequireCurrent(); return NativeMethods.wxsharp_display_count(); }
    }

    /// <summary>The primary display - the one the desktop treats as the main screen.</summary>
    public static Display Primary
    {
        get
        {
            for (var i = 0u; i < Count; i++)
                if (new Display(i).IsPrimary) return new Display(i);
            return new Display(0);
        }
    }

    /// <summary>The display containing a point in screen coordinates, or null when the point is off every
    /// screen - which is exactly what a saved window position needs checking for.</summary>
    public static Display? GetFromPoint(Point point)
    {
        _ = App.RequireCurrent();
        var index = NativeMethods.wxsharp_display_from_point(point.X, point.Y);
        return index < 0 ? null : new Display((uint)index);
    }

    /// <summary>The display a window is mostly on, or null when it is not on any.</summary>
    public static Display? GetFromWindow(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        var index = NativeMethods.wxsharp_display_from_window(window.Handle);
        return index < 0 ? null : new Display((uint)index);
    }

    /// <summary>The whole screen, in desktop coordinates.</summary>
    public Rect Geometry
    {
        get
        {
            NativeMethods.wxsharp_display_geometry(Index, out var x, out var y, out var w, out var h);
            return new Rect(x, y, w, h);
        }
    }

    /// <summary>The area a maximised window gets - the geometry less the taskbar and any other reserved
    /// edge. This, not <see cref="Geometry"/>, is what a window should be sized against.</summary>
    public Rect ClientArea
    {
        get
        {
            NativeMethods.wxsharp_display_client_area(Index, out var x, out var y, out var w, out var h);
            return new Rect(x, y, w, h);
        }
    }

    /// <summary>Whether this is the primary display.</summary>
    public bool IsPrimary => NativeMethods.wxsharp_display_is_primary(Index);

    /// <summary>The platform's name for this display, which may be empty.</summary>
    public unsafe string Name
    {
        get
        {
            var length = NativeMethods.wxsharp_display_name(Index, null, 0);
            if (length <= 0) return string.Empty;
            var bytes = new byte[length + 1];
            fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_display_name(Index, buffer, bytes.Length);
            return Utf8String.Decode(bytes, length);
        }
    }

    /// <summary>How much this display scales content - 2.0 on a screen at twice the nominal density.</summary>
    public double ScaleFactor => NativeMethods.wxsharp_display_scale_factor(Index);

    /// <summary>The display's resolution in pixels per inch.</summary>
    public Size Ppi
    {
        get { NativeMethods.wxsharp_display_ppi(Index, out var x, out var y); return new Size(x, y); }
    }

    /// <summary>Every attached display, in index order.</summary>
    public static Display[] GetAll()
    {
        var count = Count;
        var displays = new Display[count];
        for (var i = 0u; i < count; i++) displays[i] = new Display(i);
        return displays;
    }
}
