using System;

namespace WxSharp;

/// <summary>A text entry. Single-line by default; pass <see cref="TextBoxStyle"/> flags for a masked, read-only
/// or multi-line field.</summary>
public class TextBox : Control
{
    /// <summary>Raised whenever the text changes (typing, paste, programmatic set).</summary>
    public event Action? TextChanged;

    /// <summary>Raised when Enter is pressed in a single-line box (a masked or plain field, not a
    /// <c>fill</c>/multi-line one, which keeps Enter for newlines) — handy to submit a prompt.</summary>
    public event Action? EnterPressed;

    /// <summary>Creates a text box. <paramref name="fill"/> true makes it cover the parent window (outside
    /// any sizer, no relayout) - for an inline prompt that owns the frame.</summary>
    public TextBox(Container parent, string value = "", TextBoxStyle style = TextBoxStyle.None, bool fill = false)
        => Init(parent, NativeMethods.wxsharp_textbox_create(parent.Panel, value, fill, (int)style, Id));

    public unsafe string Text
    {
        get
        {
            var length = NativeMethods.wxsharp_textbox_get_value(Handle, null, 0);
            if (length <= 0)
                return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer)
                _ = NativeMethods.wxsharp_textbox_get_value(Handle, p, length + 1);
            return Utf8String.Decode(buffer, length);
        }
        set => NativeMethods.wxsharp_textbox_set_value(Handle, value);
    }

    /// <summary>Appends text to the end (handy for a log/output field).</summary>
    public void Append(string text) => NativeMethods.wxsharp_textbox_append(Handle, text);

    /// <summary>Inserts text at the caret, replacing the current selection, and moves the caret past it.</summary>
    public void Write(string text) => NativeMethods.wxsharp_textbox_write(Handle, text);

    public void Clear() => NativeMethods.wxsharp_textbox_clear(Handle);

    public void SelectAll() => NativeMethods.wxsharp_textbox_select_all(Handle);

    /// <summary>The number of characters in the box (the position just past the last one).</summary>
    public int Length => NativeMethods.wxsharp_textbox_length(Handle);

    /// <summary>The caret position (a character offset).</summary>
    public int InsertionPoint
    {
        get => NativeMethods.wxsharp_textbox_get_insertion_point(Handle);
        set => NativeMethods.wxsharp_textbox_set_insertion_point(Handle, value);
    }

    /// <summary>Moves the caret to the end of the text.</summary>
    public void MoveCaretToEnd() => NativeMethods.wxsharp_textbox_set_insertion_point_end(Handle);

    /// <summary>The current selection as a (from, to) character range; from == to means just the caret.</summary>
    public (int From, int To) Selection
    {
        get { NativeMethods.wxsharp_textbox_get_selection(Handle, out var from, out var to); return (from, to); }
        set => NativeMethods.wxsharp_textbox_set_selection(Handle, value.From, value.To);
    }

    /// <summary>The currently selected text (empty when nothing is selected).</summary>
    public unsafe string SelectedText
    {
        get
        {
            var length = NativeMethods.wxsharp_textbox_get_selected_text(Handle, null, 0);
            if (length <= 0)
                return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer)
                _ = NativeMethods.wxsharp_textbox_get_selected_text(Handle, p, length + 1);
            return Utf8String.Decode(buffer, length);
        }
    }

    /// <summary>Whether the user can edit the text (set false for a read-only field).</summary>
    public bool Editable { set => NativeMethods.wxsharp_textbox_set_editable(Handle, value); }

    private protected override void OnEvent(EventKind evt)
    {
        switch (evt)
        {
            case EventKind.Text: TextChanged?.Invoke(); break;
            case EventKind.TextEnter: EnterPressed?.Invoke(); break;
        }
    }
}
