using System;

namespace WxSharp;

/// <summary>How a window's background is painted, following <c>wxBackgroundStyle</c>.</summary>
public enum BackgroundStyle
{
    /// <summary>The background is erased before painting. The default.</summary>
    Erase = 0,
    /// <summary>The system paints it — the usual choice for a native control.</summary>
    System = 1,
    /// <summary>Nothing paints it; the paint handler is responsible for every pixel. Required for
    /// flicker-free custom drawing.</summary>
    Paint = 2,
    /// <summary>What is behind the window shows through, where the platform supports it.</summary>
    Transparent = 3,
}

/// <summary>The relative size a control is drawn at, following <c>wxWindowVariant</c>.</summary>
public enum WindowVariant
{
    Normal = 0,
    Small = 1,
    Mini = 2,
    Large = 3,
}

/// <summary>What part of a window a point falls on, following <c>wxHitTest</c>.</summary>
public enum HitTestResult
{
    NoWhere = 0,
    HorizontalScrollBar = 1,
    VerticalScrollBar = 2,
    Corner = 3,
    Inside = 4,
}

public abstract partial class Window
{
    // ---- Repaint batching -----------------------------------------------------------------------------

    /// <summary>Stops the window repainting until <see cref="Thaw"/>. Wrap a bulk update in this and the
    /// window redraws once instead of once per change. Calls nest; every <see cref="Freeze"/> needs its
    /// <see cref="Thaw"/>.</summary>
    public void Freeze() { Verify(); NativeMethods.wxsharp_window_freeze(_handle); }

    /// <summary>Lets the window repaint again.</summary>
    public void Thaw() { Verify(); NativeMethods.wxsharp_window_thaw(_handle); }

    public bool IsFrozen { get { Verify(); return NativeMethods.wxsharp_window_is_frozen(_handle); } }

    /// <summary>Fills the window with its background colour.</summary>
    public void ClearBackground() { Verify(); NativeMethods.wxsharp_window_clear_background(_handle); }

    // ---- Geometry -------------------------------------------------------------------------------------

    /// <summary>The window's position and size in its parent's coordinates.</summary>
    public Rect Rect
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_rect(_handle, out var x, out var y, out var w, out var h);
            return new Rect(x, y, w, h);
        }
    }

    /// <summary>The area inside the window's borders, in the window's own coordinates.</summary>
    public Rect ClientRect
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_client_rect(_handle, out var x, out var y, out var w, out var h);
            return new Rect(x, y, w, h);
        }
    }

    /// <summary>The window's position and size in screen coordinates.</summary>
    public Rect ScreenRect
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_screen_rect(_handle, out var x, out var y, out var w, out var h);
            return new Rect(x, y, w, h);
        }
    }

    /// <summary>The window's top-left corner in screen coordinates.</summary>
    public Point ScreenPosition
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_screen_position(_handle, out var x, out var y);
            return new Point(x, y);
        }
    }

    /// <summary>Converts a point in this window's client area to screen coordinates.</summary>
    public Point ClientToScreen(Point point)
    {
        Verify();
        int x = point.X, y = point.Y;
        NativeMethods.wxsharp_window_client_to_screen(_handle, ref x, ref y);
        return new Point(x, y);
    }

    /// <summary>Converts a screen point to this window's client coordinates.</summary>
    public Point ScreenToClient(Point point)
    {
        Verify();
        int x = point.X, y = point.Y;
        NativeMethods.wxsharp_window_screen_to_client(_handle, ref x, ref y);
        return new Point(x, y);
    }

    /// <summary>The size of the scrollable area, which may be larger than the window itself.</summary>
    public Size VirtualSize
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_virtual_size(_handle, out var w, out var h);
            return new Size(w, h);
        }
        set { Verify(); NativeMethods.wxsharp_window_set_virtual_size(_handle, value.Width, value.Height); }
    }

    /// <summary>The virtual size the window's contents actually need.</summary>
    public Size BestVirtualSize
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_best_virtual_size(_handle, out var w, out var h);
            return new Size(w, h);
        }
    }

    /// <summary>The smallest size the window may be given. Setting it stops the layout shrinking it further.</summary>
    public Size MinSize
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_min_size(_handle, out var w, out var h);
            return new Size(w, h);
        }
        set { Verify(); NativeMethods.wxsharp_control_set_min_size(_handle, value.Width, value.Height); }
    }

    /// <summary>The largest size the window may be given.</summary>
    public Size MaxSize
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_max_size(_handle, out var w, out var h);
            return new Size(w, h);
        }
        set { Verify(); NativeMethods.wxsharp_control_set_max_size(_handle, value.Width, value.Height); }
    }

    /// <summary>The minimum client area, which is the same constraint expressed without the borders.</summary>
    public Size MinClientSize
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_min_client_size(_handle, out var w, out var h);
            return new Size(w, h);
        }
        set { Verify(); NativeMethods.wxsharp_window_set_min_client_size(_handle, value.Width, value.Height); }
    }

    public Size MaxClientSize
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_max_client_size(_handle, out var w, out var h);
            return new Size(w, h);
        }
        set { Verify(); NativeMethods.wxsharp_window_set_max_client_size(_handle, value.Width, value.Height); }
    }

    /// <summary>How much the window's decoration takes on each axis — the difference between its size and
    /// its client size.</summary>
    public Size BorderSize
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_border_size(_handle, out var w, out var h);
            return new Size(w, h);
        }
    }

    /// <summary>Resizes the window so its client area is exactly this size.</summary>
    public void SetClientSize(Size size)
    {
        Verify();
        NativeMethods.wxsharp_window_set_client_size(_handle, size.Width, size.Height);
    }

    /// <summary>Sizes the window's virtual area to its children, rather than the window itself.</summary>
    public void FitInside() { Verify(); NativeMethods.wxsharp_window_fit_inside(_handle); }

    /// <summary>Converts a point from dialog units to pixels, using the window's font.</summary>
    public Point ConvertDialogToPixels(Point point)
    {
        Verify();
        int x = point.X, y = point.Y;
        NativeMethods.wxsharp_window_convert_dialog_to_pixels(_handle, ref x, ref y);
        return new Point(x, y);
    }

    public Point ConvertPixelsToDialog(Point point)
    {
        Verify();
        int x = point.X, y = point.Y;
        NativeMethods.wxsharp_window_convert_pixels_to_dialog(_handle, ref x, ref y);
        return new Point(x, y);
    }

    // ---- Text metrics ---------------------------------------------------------------------------------

    /// <summary>Measures a string in this window's font.</summary>
    public (Size Size, int Descent, int ExternalLeading) GetTextExtent(string text)
    {
        Verify();
        NativeMethods.wxsharp_window_get_text_extent(_handle, text ?? string.Empty,
            out var w, out var h, out var descent, out var leading);
        return (new Size(w, h), descent, leading);
    }

    /// <summary>The height of one line in this window's font.</summary>
    public int CharHeight { get { Verify(); return NativeMethods.wxsharp_window_get_char_height(_handle); } }

    /// <summary>The average character width in this window's font.</summary>
    public int CharWidth { get { Verify(); return NativeMethods.wxsharp_window_get_char_width(_handle); } }

    // ---- DPI ------------------------------------------------------------------------------------------

    /// <summary>The resolution of the display this window is on.</summary>
    public Size Dpi
    {
        get
        {
            Verify();
            NativeMethods.wxsharp_window_get_dpi(_handle, out var x, out var y);
            return new Size(x, y);
        }
    }

    /// <summary>Scales a size from device-independent pixels to this display's pixels. Any hard-coded size
    /// should go through here, or it is wrong on a scaled display.</summary>
    public Size FromDip(Size size)
    {
        Verify();
        int w = size.Width, h = size.Height;
        NativeMethods.wxsharp_window_from_dip(_handle, ref w, ref h);
        return new Size(w, h);
    }

    /// <summary>Scales a size from this display's pixels back to device-independent ones.</summary>
    public Size ToDip(Size size)
    {
        Verify();
        int w = size.Width, h = size.Height;
        NativeMethods.wxsharp_window_to_dip(_handle, ref w, ref h);
        return new Size(w, h);
    }

    // ---- Z-order and lifetime -------------------------------------------------------------------------

    /// <summary>Brings the window in front of its siblings.</summary>
    public void Raise() { Verify(); NativeMethods.wxsharp_window_raise(_handle); }

    /// <summary>Puts the window behind its siblings.</summary>
    public void Lower() { Verify(); NativeMethods.wxsharp_window_lower(_handle); }

    /// <summary>Whether the window and every ancestor is shown, so it is really on screen.</summary>
    public bool IsShownOnScreen
    {
        get { Verify(); return NativeMethods.wxsharp_window_is_shown_on_screen(_handle); }
    }

    /// <summary>Asks the window to close, raising <see cref="WxEvents.Closing"/> first. Returns false when a
    /// handler vetoed it. Passing true refuses to take no for an answer.</summary>
    public bool Close(bool force = false)
    {
        Verify();
        return NativeMethods.wxsharp_window_close_any(_handle, force);
    }

    /// <summary>Centres the window on the screen, or on its parent.</summary>
    public void Center(bool onParent = false)
    {
        Verify();
        NativeMethods.wxsharp_window_center_any(_handle, onParent);
    }

    // ---- Keyboard navigation --------------------------------------------------------------------------

    /// <summary>Moves focus to the next control in the tab order, as pressing Tab would.</summary>
    public bool Navigate(bool forward = true, bool windowChange = false)
    {
        Verify();
        return NativeMethods.wxsharp_window_navigate(_handle, forward, windowChange);
    }

    /// <summary>Moves focus to the first control inside this window rather than past it.</summary>
    public bool NavigateIn(bool forward = true, bool windowChange = false)
    {
        Verify();
        return NativeMethods.wxsharp_window_navigate_in(_handle, forward, windowChange);
    }

    // ---- Scrolling ------------------------------------------------------------------------------------

    /// <summary>Configures one of the window's scrollbars.</summary>
    public void SetScrollbar(Orientation orientation, int position, int thumbSize, int range, bool refresh = true)
    {
        Verify();
        NativeMethods.wxsharp_window_set_scrollbar(_handle, orientation == Orientation.Vertical,
            position, thumbSize, range, refresh);
    }

    public void SetScrollPosition(Orientation orientation, int position, bool refresh = true)
    {
        Verify();
        NativeMethods.wxsharp_window_set_scroll_pos(_handle, orientation == Orientation.Vertical, position, refresh);
    }

    public int GetScrollPosition(Orientation orientation)
    {
        Verify();
        return NativeMethods.wxsharp_window_get_scroll_pos(_handle, orientation == Orientation.Vertical);
    }

    public int GetScrollRange(Orientation orientation)
    {
        Verify();
        return NativeMethods.wxsharp_window_get_scroll_range(_handle, orientation == Orientation.Vertical);
    }

    public int GetScrollThumb(Orientation orientation)
    {
        Verify();
        return NativeMethods.wxsharp_window_get_scroll_thumb(_handle, orientation == Orientation.Vertical);
    }

    public bool HasScrollbar(Orientation orientation)
    {
        Verify();
        return NativeMethods.wxsharp_window_has_scrollbar(_handle, orientation == Orientation.Vertical);
    }

    /// <summary>Scrolls by whole lines. A negative count scrolls back.</summary>
    public bool ScrollLines(int lines) { Verify(); return NativeMethods.wxsharp_window_scroll_lines(_handle, lines); }

    /// <summary>Scrolls by whole pages.</summary>
    public bool ScrollPages(int pages) { Verify(); return NativeMethods.wxsharp_window_scroll_pages(_handle, pages); }

    public bool LineUp() => ScrollLines(-1);
    public bool LineDown() => ScrollLines(1);
    public bool PageUp() => ScrollPages(-1);
    public bool PageDown() => ScrollPages(1);

    /// <summary>Scrolls the window's contents by a pixel offset.</summary>
    public void ScrollWindow(int dx, int dy) { Verify(); NativeMethods.wxsharp_window_scroll_window(_handle, dx, dy); }

    // ---- Styles and appearance ------------------------------------------------------------------------

    /// <summary>The raw wxWidgets style flags. The typed style enums are the readable way to set these at
    /// creation; this is for reading back or for a flag the wrapper does not name.</summary>
    public int WindowStyleFlags
    {
        get { Verify(); return NativeMethods.wxsharp_window_get_style_flags(_handle); }
        set { Verify(); NativeMethods.wxsharp_window_set_style_flags(_handle, value); }
    }

    /// <summary>Whether a raw wxWidgets style flag is set.</summary>
    public bool HasWindowStyleFlag(int flag)
    {
        Verify();
        return NativeMethods.wxsharp_window_has_style_flag(_handle, flag);
    }

    /// <summary>The window's label. What this means depends on the control — a button's text, a frame's
    /// title, a static box's caption.</summary>
    public unsafe string Label
    {
        get
        {
            Verify();
            var length = NativeMethods.wxsharp_window_get_label(_handle, null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_window_get_label(_handle, p, buffer.Length);
            return Utf8String.Decode(buffer, length);
        }
        set { Verify(); NativeMethods.wxsharp_window_set_label(_handle, value ?? string.Empty); }
    }

    /// <summary>Context help for this window.</summary>
    ///
    /// <remarks>
    /// wxWidgets stores help text in a help provider rather than on the window, and installs none by
    /// default - so with no provider this silently keeps nothing and reads back empty. That is wxWidgets'
    /// own behaviour; <c>wxHelpProvider</c> is not wrapped yet. Until it is, a tooltip
    /// (<see cref="Window.ToolTip"/>) is the reliable way to attach an explanation to a control.
    /// </remarks>
    public unsafe string HelpText
    {
        get
        {
            Verify();
            var length = NativeMethods.wxsharp_window_get_help_text(_handle, null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_window_get_help_text(_handle, p, buffer.Length);
            return Utf8String.Decode(buffer, length);
        }
        set { Verify(); NativeMethods.wxsharp_window_set_help_text(_handle, value ?? string.Empty); }
    }

    /// <summary>Whether the window draws through an off-screen buffer, which removes flicker from custom
    /// drawing at the cost of memory.</summary>
    public bool DoubleBuffered
    {
        get { Verify(); return NativeMethods.wxsharp_window_is_double_buffered(_handle); }
        set { Verify(); NativeMethods.wxsharp_window_set_double_buffered(_handle, value); }
    }

    /// <summary>How the background is painted. Custom drawing normally wants
    /// <see cref="BackgroundStyle.Paint"/>. Returns false when the platform refuses the style.</summary>
    public BackgroundStyle BackgroundStyle
    {
        get { Verify(); return (BackgroundStyle)NativeMethods.wxsharp_window_get_background_style(_handle); }
        set { Verify(); _ = NativeMethods.wxsharp_window_set_background_style(_handle, (int)value); }
    }

    /// <summary>The relative size this control is drawn at.</summary>
    public WindowVariant Variant
    {
        get { Verify(); return (WindowVariant)NativeMethods.wxsharp_window_get_variant(_handle); }
        set { Verify(); NativeMethods.wxsharp_window_set_variant(_handle, (int)value); }
    }

    /// <summary>Whether this platform can make the window translucent.</summary>
    public bool CanSetTransparent
    {
        get { Verify(); return NativeMethods.wxsharp_window_can_set_transparent(_handle); }
    }

    /// <summary>Makes the whole window translucent, from 0 (invisible) to 255 (opaque). Returns false where
    /// the platform does not support it.</summary>
    public bool SetTransparent(int alpha)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(alpha);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(alpha, 255);
        Verify();
        return NativeMethods.wxsharp_window_set_transparent(_handle, alpha);
    }

    // ---- Pointer --------------------------------------------------------------------------------------

    /// <summary>Moves the mouse pointer to a point in this window. Use sparingly: moving the pointer under
    /// the user is disorienting, and assistive technology does not expect it.</summary>
    public void WarpPointer(Point point)
    {
        Verify();
        NativeMethods.wxsharp_window_warp_pointer(_handle, point.X, point.Y);
    }

    /// <summary>What part of the window a client-area point falls on.</summary>
    public HitTestResult HitTest(Point point)
    {
        Verify();
        return (HitTestResult)NativeMethods.wxsharp_window_hit_test(_handle, point.X, point.Y);
    }

    /// <summary>Shows a menu and returns the command ID chosen, or <see cref="StandardId.None"/> when it was
    /// dismissed. The blocking counterpart of <see cref="PopupMenu"/>, for when the caller only wants the
    /// answer rather than a command event.</summary>
    public int GetPopupMenuSelectionFromUser(Menu menu, Point? position = null)
    {
        ArgumentNullException.ThrowIfNull(menu);
        Verify();
        var point = position ?? new Point(-1, -1);
        return NativeMethods.wxsharp_window_popup_menu_selection(_handle, menu.Handle, point.X, point.Y);
    }
}
