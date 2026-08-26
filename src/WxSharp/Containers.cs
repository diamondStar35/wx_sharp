using System;

namespace WxSharp;

public class ScrolledWindow : Window
{
    public ScrolledWindow(Window parent, int id = WindowId.Any, ScrolledStyle style = ScrolledStyle.Default)
        : base(parent, id)
        => Initialize(NativeMethods.wxsharp_scrolled_create(parent.Handle, id, (int)style, Token));
    public Point ViewStart { get { NativeMethods.wxsharp_scrolled_get_view_start(Handle, out var x, out var y); return new Point(x, y); } }
    public void SetScrollRate(int xStep, int yStep) => NativeMethods.wxsharp_scrolled_set_rate(Handle, xStep, yStep);
    public void Scroll(int x, int y) => NativeMethods.wxsharp_scrolled_scroll(Handle, x, y);
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
        Initialize(NativeMethods.wxsharp_splitter_create(parent.Handle, id, orientation == Orientation.Vertical, Token));
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
        => Initialize(NativeMethods.wxsharp_notebook_create(parent.Handle, id, Token));
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
        => Initialize(NativeMethods.wxsharp_simplebook_create(parent.Handle, id, Token));
    public int Count => NativeMethods.wxsharp_notebook_count(Handle);
    public int SelectedIndex
    {
        get => NativeMethods.wxsharp_notebook_get_selection(Handle);
        set => _ = NativeMethods.wxsharp_notebook_set_selection(Handle, value);
    }
    public bool AddPage(Window page, string text = "", bool select = false) => NativeMethods.wxsharp_notebook_add_page(Handle, page.Handle, text, select);
    public bool RemovePage(int index) => NativeMethods.wxsharp_notebook_delete_page(Handle, index);
}
