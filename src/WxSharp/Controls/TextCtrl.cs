using System;

namespace WxSharp;

/// <summary>A native wxTextCtrl. Single-line by default.</summary>
public class TextCtrl : Control
{
    public event EventHandler<CommandEventArgs>? TextChanged;
    public event EventHandler<CommandEventArgs>? EnterPressed;

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

    internal override uint Dispatch(in NativeEvent e)
    {
        return e.Kind switch
        {
            EventKind.Text => RaiseCommand(new CommandEventArgs(this, e.Id), TextChanged),
            EventKind.TextEnter => RaiseCommand(new CommandEventArgs(this, e.Id), EnterPressed),
            _ => base.Dispatch(e),
        };
    }
}
