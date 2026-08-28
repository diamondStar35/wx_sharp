using System;

namespace WxSharp;

/// <summary>When a scrolled window shows a scrollbar, following wxWidgets' <c>wxScrollbarVisibility</c>.
/// </summary>
public enum ScrollbarVisibility
{
    /// <summary>Never shown, and the axis cannot be scrolled with it.</summary>
    Never = 0,
    /// <summary>Shown only when the content does not fit, which is wxWidgets' default.</summary>
    Automatic = 1,
    /// <summary>Always shown, so the layout does not shift when the content grows.</summary>
    Always = 2,
}

public class ScrolledWindow : Window
{
    public ScrolledWindow(Window parent, int id = WindowId.Any, ScrolledStyle style = ScrolledStyle.Default)
        : base(parent, id)
        => Initialize(GetType() == typeof(ScrolledWindow)
            ? NativeMethods.wxsharp_scrolled_create(parent.Handle, id, (int)style, Token)
            : NativeMethods.wxsharp_custom_scrolled_create(parent.Handle, id, (int)style, Token));
    public Point ViewStart { get { NativeMethods.wxsharp_scrolled_get_view_start(Handle, out var x, out var y); return new Point(x, y); } }
    public void SetScrollRate(int xStep, int yStep) => NativeMethods.wxsharp_scrolled_set_rate(Handle, xStep, yStep);
    public void Scroll(int x, int y) => NativeMethods.wxsharp_scrolled_scroll(Handle, x, y);

    /// <summary>Sets the scroll step and the extent to scroll over in one call, following
    /// <c>wxScrolled.SetScrollbars</c>. The scrollable area is <paramref name="unitsX"/> by
    /// <paramref name="unitsY"/> steps, each of the given pixel size.</summary>
    public void SetScrollbars(int pixelsPerUnitX, int pixelsPerUnitY, int unitsX, int unitsY,
        int positionX = 0, int positionY = 0, bool noRefresh = false)
        => NativeMethods.wxsharp_scrolled_set_scrollbars(Handle, pixelsPerUnitX, pixelsPerUnitY, unitsX,
            unitsY, positionX, positionY, noRefresh);

    /// <summary>Whether the window scrolls physically on each axis, following
    /// <c>wxScrolled.EnableScrolling</c>. Turning an axis off leaves its scrollbar working but stops the
    /// window blitting, which is what a window whose children move themselves wants.</summary>
    public void EnableScrolling(bool x, bool y) => NativeMethods.wxsharp_scrolled_enable_scrolling(Handle, x, y);

    /// <summary>When each scrollbar is shown, following <c>wxScrolled.ShowScrollbars</c>.</summary>
    public void ShowScrollbars(ScrollbarVisibility horizontal, ScrollbarVisibility vertical)
        => NativeMethods.wxsharp_scrolled_show_scrollbars(Handle, (int)horizontal, (int)vertical);

    /// <summary>The pixel size of one scroll step on each axis. Follows
    /// <c>wxScrolled.GetScrollPixelsPerUnit</c>.</summary>
    public Size ScrollPixelsPerUnit
    {
        get { NativeMethods.wxsharp_scrolled_get_pixels_per_unit(Handle, out var x, out var y); return new Size(x, y); }
    }

    /// <summary>Scrolls a different window than this one, following <c>wxScrolled.SetTargetWindow</c>.
    /// Used when the scrolled window is a frame around a separate content panel.</summary>
    public void SetTargetWindow(Window target)
    {
        ArgumentNullException.ThrowIfNull(target);
        NativeMethods.wxsharp_scrolled_set_target_window(Handle, target.Handle);
    }

    /// <summary>How far Page Up and Page Down move on one axis, in scroll units. Follows
    /// <c>wxScrolled.SetScrollPageSize</c> / <c>GetScrollPageSize</c>.</summary>
    public int GetScrollPageSize(Orientation orientation)
        => NativeMethods.wxsharp_scrolled_get_scroll_page_size(Handle, OrientationFlag(orientation));

    /// <summary>See <see cref="GetScrollPageSize"/>.</summary>
    public void SetScrollPageSize(Orientation orientation, int size)
        => NativeMethods.wxsharp_scrolled_set_scroll_page_size(Handle, OrientationFlag(orientation), size);

    private static int OrientationFlag(Orientation orientation)
        => orientation == Orientation.Vertical ? 1 : 0;

    // ---- Overridable wxScrolled virtuals ----------------------------------------------------------------

    /// <summary>Whether the window should scroll itself to bring a newly focused child into view. A window
    /// that manages its own scrolling answers false so wxWidgets does not fight it - which is what stops a
    /// scrolled settings page jumping about as a screen reader user tabs through it. Follows
    /// <c>wxScrolled.ShouldScrollToChildOnFocus</c>.</summary>
    public virtual bool ShouldScrollToChildOnFocus(Window? child)
        => CallBaseWithWindow(VirtualMember.ShouldScrollToChildOnFocus, child).Result != 0;

    /// <summary>How much room the scrolled content may use, given the window's size. Follows
    /// <c>wxScrolled.GetSizeAvailableForScrollTarget</c>.</summary>
    public virtual Size GetSizeAvailableForScrollTarget(Size size)
    {
        var request = CallBase(VirtualMember.SizeAvailableForScrollTarget, size.Width, size.Height);
        return new Size(request.X, request.Y);
    }

    internal override unsafe bool TryAnswerVirtual(ref NativeVirtualRequest request)
    {
        switch ((VirtualMember)request.Which)
        {
            case VirtualMember.ShouldScrollToChildOnFocus:
                request.Result = ShouldScrollToChildOnFocus(App.Lookup((nint)request.Handle)) ? 1 : 0;
                return true;
            case VirtualMember.SizeAvailableForScrollTarget:
            {
                var available = GetSizeAvailableForScrollTarget(new Size(request.Args[0], request.Args[1]));
                request.X = available.Width;
                request.Y = available.Height;
                return true;
            }
            default:
                return base.TryAnswerVirtual(ref request);
        }
    }
}

public class SplitterWindow : Window
{
    /// <summary>The sash was dragged to a new position.</summary>
    public event EventHandler<SplitterEventArgs> SashPositionChanged
    {
        add => AddHandler(WxEvents.SashPositionChanged, value);
        remove => RemoveHandler(WxEvents.SashPositionChanged, value);
    }

    /// <summary>The sash was double-clicked, which normally unsplits. Veto to keep the split.</summary>
    public event EventHandler<SplitterEventArgs> SashDoubleClicked
    {
        add => AddHandler(WxEvents.SashDoubleClicked, value);
        remove => RemoveHandler(WxEvents.SashDoubleClicked, value);
    }

    /// <summary>The sash is being dragged. Veto to refuse the new position - the usual way to enforce a minimum pane size beyond what SetMinimumPaneSize covers.</summary>
    public event EventHandler<SplitterEventArgs> SashPositionChanging
    {
        add => AddHandler(WxEvents.SashPositionChanging, value);
        remove => RemoveHandler(WxEvents.SashPositionChanging, value);
    }

    /// <summary>The window was unsplit.</summary>
    public event EventHandler<SplitterEventArgs> PaneUnsplit
    {
        add => AddHandler(WxEvents.Unsplit, value);
        remove => RemoveHandler(WxEvents.Unsplit, value);
    }

    public Orientation Orientation { get; }
    public SplitterWindow(Window parent, Orientation orientation = Orientation.Vertical, int id = WindowId.Any) : base(parent, id)
    {
        Orientation = orientation;
        Initialize(GetType() == typeof(SplitterWindow)
            ? NativeMethods.wxsharp_splitter_create(parent.Handle, id, orientation == Orientation.Vertical, Token)
            : NativeMethods.wxsharp_custom_splitter_create(parent.Handle, id, orientation == Orientation.Vertical, Token));
    }
    public int SashPosition { get => NativeMethods.wxsharp_splitter_get_position(Handle); set => NativeMethods.wxsharp_splitter_set_position(Handle, value); }
    public bool Split(Window first, Window second, int position = 0)
    {
        ArgumentNullException.ThrowIfNull(first); ArgumentNullException.ThrowIfNull(second);
        return NativeMethods.wxsharp_splitter_split(Handle, first.Handle, second.Handle, position);
    }
    public bool Unsplit(Window? remove = null) => NativeMethods.wxsharp_splitter_unsplit(Handle, remove?.Handle ?? 0);
}

public class Notebook : Control
{
    /// <summary>The visible page changed.</summary>
    public event EventHandler<BookEventArgs> PageChanged
    {
        add => AddHandler(WxEvents.NotebookPageChanged, value);
        remove => RemoveHandler(WxEvents.NotebookPageChanged, value);
    }

    /// <summary>The page is about to change. Veto to keep the current page - for refusing to leave a page
    /// with unsaved edits, for instance.</summary>
    public event EventHandler<BookEventArgs> PageChanging
    {
        add => AddHandler(WxEvents.NotebookPageChanging, value);
        remove => RemoveHandler(WxEvents.NotebookPageChanging, value);
    }

    public Notebook(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(Notebook)
            ? NativeMethods.wxsharp_notebook_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_notebook_create(parent.Handle, id, Token));
    public int Count => NativeMethods.wxsharp_notebook_count(Handle);
    public int SelectedIndex
    {
        get => NativeMethods.wxsharp_notebook_get_selection(Handle);
        set => _ = NativeMethods.wxsharp_notebook_set_selection(Handle, value);
    }
    public bool AddPage(Window page, string text, bool select = false) => NativeMethods.wxsharp_notebook_add_page(Handle, page.Handle, text, select);
    public bool RemovePage(int index) => NativeMethods.wxsharp_notebook_delete_page(Handle, index);
    public unsafe string GetPageText(int index)
    {
        var length = NativeMethods.wxsharp_notebook_get_page_text(Handle, index, null, 0); if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_notebook_get_page_text(Handle, index, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }
    public bool SetPageText(int index, string text) => NativeMethods.wxsharp_notebook_set_page_text(Handle, index, text);
}

public class SimpleBook : Control
{
    public event EventHandler<BookEventArgs> PageChanged
    {
        add => AddHandler(WxEvents.BookPageChanged, value);
        remove => RemoveHandler(WxEvents.BookPageChanged, value);
    }

    public event EventHandler<BookEventArgs> PageChanging
    {
        add => AddHandler(WxEvents.BookPageChanging, value);
        remove => RemoveHandler(WxEvents.BookPageChanging, value);
    }

    public SimpleBook(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(SimpleBook)
            ? NativeMethods.wxsharp_simplebook_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_simplebook_create(parent.Handle, id, Token));
    public int Count => NativeMethods.wxsharp_notebook_count(Handle);
    public int SelectedIndex
    {
        get => NativeMethods.wxsharp_notebook_get_selection(Handle);
        set => _ = NativeMethods.wxsharp_notebook_set_selection(Handle, value);
    }
    public bool AddPage(Window page, string text = "", bool select = false) => NativeMethods.wxsharp_notebook_add_page(Handle, page.Handle, text, select);
    public bool RemovePage(int index) => NativeMethods.wxsharp_notebook_delete_page(Handle, index);
}
