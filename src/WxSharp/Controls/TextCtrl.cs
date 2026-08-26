using System;

namespace WxSharp;

/// <summary>A native wxTextCtrl. Single-line by default.</summary>
public class TextCtrl : Control
{
    public event EventHandler<CommandEventArgs> TextChanged
    {
        add => AddHandler(WxEvents.TextChanged, value);
        remove => RemoveHandler(WxEvents.TextChanged, value);
    }
    /// <summary>Enter was pressed. Requires TextCtrlStyle.ProcessEnter on a multi-line control; single-line controls process Enter already.</summary>
    public event EventHandler<CommandEventArgs> EnterPressed
    {
        add => AddHandler(WxEvents.TextEntered, value);
        remove => RemoveHandler(WxEvents.TextEntered, value);
    }

    /// <summary>The user tried to type past the length limit.</summary>
    public event EventHandler<CommandEventArgs> MaxLengthReached
    {
        add => AddHandler(WxEvents.TextMaxLengthReached, value);
        remove => RemoveHandler(WxEvents.TextMaxLengthReached, value);
    }

    /// <summary>A URL in the text was clicked. Requires TextCtrlStyle.AutoUrl and a rich control.</summary>
    public event EventHandler<TextUrlEventArgs> UrlClicked
    {
        add => AddHandler(WxEvents.TextUrlClicked, value);
        remove => RemoveHandler(WxEvents.TextUrlClicked, value);
    }

    public TextCtrl(Window parent, int id = WindowId.Any, string value = "", TextCtrlStyle style = TextCtrlStyle.None,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_textbox_create(parent.Handle, id, value, (int)style, Token));
        ApplyInitialGeometry(position, size);
    }

    public unsafe string Value
    {
        get
        {
            var length = NativeMethods.wxsharp_textbox_get_value(Handle, null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_textbox_get_value(Handle, p, length + 1);
            return Utf8String.Decode(buffer, length);
        }
        set => NativeMethods.wxsharp_textbox_set_value(Handle, value);
    }

    public void Append(string text) => NativeMethods.wxsharp_textbox_append(Handle, text);
    public void Write(string text) => NativeMethods.wxsharp_textbox_write(Handle, text);
    public void Clear() => NativeMethods.wxsharp_textbox_clear(Handle);
    public void SelectAll() => NativeMethods.wxsharp_textbox_select_all(Handle);
    public int Length => NativeMethods.wxsharp_textbox_length(Handle);
    public int InsertionPoint
    {
        get => NativeMethods.wxsharp_textbox_get_insertion_point(Handle);
        set => NativeMethods.wxsharp_textbox_set_insertion_point(Handle, value);
    }
    public void MoveCaretToEnd() => NativeMethods.wxsharp_textbox_set_insertion_point_end(Handle);
    public (int From, int To) Selection
    {
        get { NativeMethods.wxsharp_textbox_get_selection(Handle, out var from, out var to); return (from, to); }
        set => NativeMethods.wxsharp_textbox_set_selection(Handle, value.From, value.To);
    }
    public unsafe string SelectedText
    {
        get
        {
            var length = NativeMethods.wxsharp_textbox_get_selected_text(Handle, null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_textbox_get_selected_text(Handle, p, length + 1);
            return Utf8String.Decode(buffer, length);
        }
    }
    public bool Editable { set => NativeMethods.wxsharp_textbox_set_editable(Handle, value); }

    /// <summary>How many lines the control holds. Always 1 for a single-line control.</summary>
    public int LineCount => NativeMethods.wxsharp_textbox_line_count(Handle);

    /// <summary>The length of one line, or -1 when there is no such line.</summary>
    public int GetLineLength(int line) => NativeMethods.wxsharp_textbox_line_length(Handle, line);

    /// <summary>The text of one line, without its line ending. Empty when there is no such line.</summary>
    public unsafe string GetLineText(int line)
    {
        var length = NativeMethods.wxsharp_textbox_get_line_text(Handle, line, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_textbox_get_line_text(Handle, line, p, length + 1);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>Scrolls so that <paramref name="position"/> is visible, without moving the caret - for
    /// following appended output while leaving the insertion point where the user left it.</summary>
    public void ShowPosition(int position) => NativeMethods.wxsharp_textbox_show_position(Handle, position);

    /// <summary>Scrolls to the end without moving the caret.</summary>
    public void ScrollToEnd() => ShowPosition(Length);
}
