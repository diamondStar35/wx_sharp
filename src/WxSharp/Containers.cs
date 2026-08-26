using System;

namespace WxSharp;

public class ScrolledWindow : Window
{
    public ScrolledWindow(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_scrolled_create(parent.Handle, id, Token));
    public Point ViewStart { get { NativeMethods.wxsharp_scrolled_get_view_start(Handle, out var x, out var y); return new Point(x, y); } }
    public void SetScrollRate(int xStep, int yStep) => NativeMethods.wxsharp_scrolled_set_rate(Handle, xStep, yStep);
    public void Scroll(int x, int y) => NativeMethods.wxsharp_scrolled_scroll(Handle, x, y);
}

public class SplitterWindow : Window
{
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
    public event EventHandler<SelectionEventArgs>? SelectionChanged;
    public Notebook(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_notebook_create(parent.Handle, id, Token));
    public int Count => NativeMethods.wxsharp_notebook_count(Handle);
    public int SelectedIndex
    {
        get => NativeMethods.wxsharp_notebook_get_selection(Handle);
        set { var previousSelection = NativeMethods.wxsharp_notebook_set_selection(Handle, value); _ = previousSelection; }
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
    internal override uint Dispatch(in NativeEvent e)
    {
        if (e.Kind != EventKind.Select) return base.Dispatch(e);
        return Raise(new SelectionEventArgs(this, e.Id, e.Y, e.X), SelectionChanged);
    }
}

public class SimpleBook : Control
{
    public event EventHandler<SelectionEventArgs>? SelectionChanged;
    public SimpleBook(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_simplebook_create(parent.Handle, id, Token));
    public int Count => NativeMethods.wxsharp_notebook_count(Handle);
    public int SelectedIndex
    {
        get => NativeMethods.wxsharp_notebook_get_selection(Handle);
        set { var previous = NativeMethods.wxsharp_notebook_set_selection(Handle, value); _ = previous; }
    }
    public bool AddPage(Window page, string text = "", bool select = false) => NativeMethods.wxsharp_notebook_add_page(Handle, page.Handle, text, select);
    public bool RemovePage(int index) => NativeMethods.wxsharp_notebook_delete_page(Handle, index);
    internal override uint Dispatch(in NativeEvent e)
    {
        if (e.Kind != EventKind.Select) return base.Dispatch(e);
        return Raise(new SelectionEventArgs(this, e.Id, e.Y, e.X), SelectionChanged);
    }
}
