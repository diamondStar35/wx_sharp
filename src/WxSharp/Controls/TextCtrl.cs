using System;

namespace WxSharp;

/// <summary>A native wxTextCtrl. Single-line by default.</summary>
public class TextCtrl : Control, ITextEntry
{
    /// <summary>Wraps a TextCtrl wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal TextCtrl(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

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
        Initialize(GetType() == typeof(TextCtrl)
            ? NativeMethods.wxsharp_textbox_create(parent.Handle, id, value, (int)style, Token)
            : NativeMethods.wxsharp_custom_textbox_create(parent.Handle, id, value, (int)style, Token));
        ApplyInitialGeometry(position, size);
    }

    /// <summary>For a subclass that creates its own native control, as wxSearchCtrl does — it derives from
    /// wxTextCtrl in wxWidgets, and so inherits the whole editing surface.</summary>
    private protected TextCtrl(Window parent, int id) : base(parent, id) { }

    // ---- ITextEntry: the editing surface wxTextCtrl, wxComboBox and wxSearchCtrl share ----------------

    /// <inheritdoc/>
    public string Value
    {
        get => TextEntryNative.GetValue(Handle);
        set => TextEntryNative.SetValue(Handle, value);
    }

    /// <inheritdoc/>
    public void ChangeValue(string value) => TextEntryNative.ChangeValue(Handle, value);
    /// <inheritdoc/>
    public void Write(string text) => TextEntryNative.Write(Handle, text);
    /// <inheritdoc/>
    public void Append(string text) => TextEntryNative.Append(Handle, text);
    /// <inheritdoc/>
    public string GetRange(int from, int to) => TextEntryNative.GetRange(Handle, from, to);
    /// <inheritdoc/>
    public void Replace(int from, int to, string value) => TextEntryNative.Replace(Handle, from, to, value);
    /// <inheritdoc/>
    public void Remove(int from, int to) => TextEntryNative.Remove(Handle, from, to);
    /// <inheritdoc/>
    public void Clear() => TextEntryNative.Clear(Handle);
    /// <inheritdoc/>
    public bool IsEmpty => TextEntryNative.IsEmpty(Handle);

    /// <inheritdoc/>
    public void Copy() => TextEntryNative.Copy(Handle);
    /// <inheritdoc/>
    public void Cut() => TextEntryNative.Cut(Handle);
    /// <inheritdoc/>
    public void Paste() => TextEntryNative.Paste(Handle);
    /// <inheritdoc/>
    public bool CanCopy => TextEntryNative.CanCopy(Handle);
    /// <inheritdoc/>
    public bool CanCut => TextEntryNative.CanCut(Handle);
    /// <inheritdoc/>
    public bool CanPaste => TextEntryNative.CanPaste(Handle);
    /// <inheritdoc/>
    public void Undo() => TextEntryNative.Undo(Handle);
    /// <inheritdoc/>
    public void Redo() => TextEntryNative.Redo(Handle);
    /// <inheritdoc/>
    public bool CanUndo => TextEntryNative.CanUndo(Handle);
    /// <inheritdoc/>
    public bool CanRedo => TextEntryNative.CanRedo(Handle);

    /// <inheritdoc/>
    public int InsertionPoint
    {
        get => TextEntryNative.GetInsertionPoint(Handle);
        set => TextEntryNative.SetInsertionPoint(Handle, value);
    }

    /// <inheritdoc/>
    public void MoveCaretToEnd() => TextEntryNative.MoveCaretToEnd(Handle);
    /// <inheritdoc/>
    public int LastPosition => TextEntryNative.LastPosition(Handle);

    /// <inheritdoc/>
    public (int From, int To) Selection
    {
        get => TextEntryNative.GetSelection(Handle);
        set => TextEntryNative.SetSelection(Handle, value.From, value.To);
    }

    /// <inheritdoc/>
    public void SelectAll() => TextEntryNative.SelectAll(Handle);
    /// <inheritdoc/>
    public void SelectNone() => TextEntryNative.SelectNone(Handle);
    /// <inheritdoc/>
    public bool HasSelection => TextEntryNative.HasSelection(Handle);
    /// <inheritdoc/>
    public string SelectedText => TextEntryNative.SelectedText(Handle);
    /// <inheritdoc/>
    public void RemoveSelection() => TextEntryNative.RemoveSelection(Handle);

    /// <inheritdoc/>
    public bool Editable
    {
        get => TextEntryNative.IsEditable(Handle);
        set => TextEntryNative.SetEditable(Handle, value);
    }

    /// <inheritdoc/>
    public int MaxLength { set => TextEntryNative.SetMaxLength(Handle, value); }
    /// <inheritdoc/>
    public void ForceUpper() => TextEntryNative.ForceUpper(Handle);

    /// <inheritdoc/>
    public string Hint
    {
        get => TextEntryNative.GetHint(Handle);
        set => TextEntryNative.SetHint(Handle, value);
    }

    /// <inheritdoc/>
    public (int Left, int Top) Margins => TextEntryNative.GetMargins(Handle);
    /// <inheritdoc/>
    public bool SetMargins(int left, int top = -1) => TextEntryNative.SetMargins(Handle, left, top);

    /// <inheritdoc/>
    public bool AutoComplete(params string[] choices) => TextEntryNative.AutoComplete(Handle, choices);
    /// <inheritdoc/>
    public bool AutoCompleteFileNames() => TextEntryNative.AutoCompleteFileNames(Handle);
    /// <inheritdoc/>
    public bool AutoCompleteDirectories() => TextEntryNative.AutoCompleteDirectories(Handle);


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
    public void ScrollToEnd() => ShowPosition(LastPosition);

    /// <summary>Whether the control was created with <see cref="TextCtrlStyle.MultiLine"/>. Several members
    /// here only mean anything on a multi-line control.</summary>
    public bool IsMultiLine => NativeMethods.wxsharp_textbox_is_multiline(Handle);

    // ---- The modified flag ------------------------------------------------------------------------------

    /// <summary>Whether the text has been edited since the control was created or since the last
    /// <see cref="DiscardEdits"/>. This is what a "save your changes?" prompt should be asking.</summary>
    public bool IsModified
    {
        get => NativeMethods.wxsharp_textbox_is_modified(Handle);
        set => NativeMethods.wxsharp_textbox_set_modified(Handle, value);
    }

    /// <summary>Marks the text as edited without changing it.</summary>
    public void MarkDirty() => NativeMethods.wxsharp_textbox_mark_dirty(Handle);

    /// <summary>Clears the modified flag, as after saving.</summary>
    public void DiscardEdits() => NativeMethods.wxsharp_textbox_discard_edits(Handle);

    // ---- Positions and coordinates ----------------------------------------------------------------------

    /// <summary>The column and line a character position falls on. False when the position is out of
    /// range, in which case the column and line are meaningless.</summary>
    public bool PositionToXY(int position, out int column, out int line)
        => NativeMethods.wxsharp_textbox_position_to_xy(Handle, position, out column, out line);

    /// <summary>The character position at a column and line, or -1 when there is no such place.</summary>
    public int XYToPosition(int column, int line) => NativeMethods.wxsharp_textbox_xy_to_position(Handle, column, line);

    /// <summary>Which character a point in client coordinates lands on. Some platforms do not implement
    /// this and answer <see cref="TextCtrlHitTest.Unknown"/>.</summary>
    public TextCtrlHitTest HitTest(Point point, out int position)
        => (TextCtrlHitTest)NativeMethods.wxsharp_textbox_hit_test(Handle, point.X, point.Y, out position);

    // ---- Files ------------------------------------------------------------------------------------------

    /// <summary>Replaces the contents with a file's, and clears the modified flag.</summary>
    public bool LoadFile(string path) => NativeMethods.wxsharp_textbox_load_file(Handle, path ?? string.Empty);

    /// <summary>Writes the contents to a file, and clears the modified flag. An empty path saves back over
    /// the file the control was last loaded from.</summary>
    public bool SaveFile(string path = "") => NativeMethods.wxsharp_textbox_save_file(Handle, path ?? string.Empty);

    // ---- Styling ----------------------------------------------------------------------------------------
    // Styling needs the control to have been created with TextCtrlStyle.Rich or Rich2; without it the
    // platform has nowhere to record per-character attributes and these report failure.

    /// <summary>Applies a style to a range of characters.</summary>
    public unsafe bool SetStyle(int start, int end, TextAttr style)
    {
        ArgumentNullException.ThrowIfNull(style);
        var native = style.ToNative();
        return NativeMethods.wxsharp_textbox_set_style(Handle, start, end, &native);
    }

    /// <summary>The style in force at a character position, or null when the control cannot report one.</summary>
    public unsafe TextAttr? GetStyle(int position)
    {
        NativeTextAttr native;
        return NativeMethods.wxsharp_textbox_get_style(Handle, position, &native)
            ? TextAttr.FromNative(native)
            : null;
    }

    /// <summary>The style newly written text takes, so appending in a colour needs no second call.</summary>
    public unsafe TextAttr DefaultStyle
    {
        get
        {
            NativeTextAttr native;
            NativeMethods.wxsharp_textbox_get_default_style(Handle, &native);
            return TextAttr.FromNative(native);
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            var native = value.ToNative();
            NativeMethods.wxsharp_textbox_set_default_style(Handle, &native);
        }
    }
}

/// <summary>Where a point landed relative to the text, following <c>wxTextCtrlHitTestResult</c>.</summary>
public enum TextCtrlHitTest
{
    /// <summary>The platform does not implement hit testing for this control.</summary>
    Unknown = -2,
    /// <summary>Before the text, either to its left or above it.</summary>
    Before = -1,
    /// <summary>Directly on a character.</summary>
    OnText = 0,
    /// <summary>Below the last line.</summary>
    Below = 1,
    /// <summary>Past the end of the line.</summary>
    Beyond = 2,
}
