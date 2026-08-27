using System;

namespace WxSharp;

/// <summary>The editing surface <see cref="TextCtrl"/>, <see cref="ComboBox"/> and <see cref="SearchCtrl"/>
/// share, following <c>wxTextEntry</c>.</summary>
///
/// <remarks>
/// wxWidgets gives these controls a common editing interface through multiple inheritance, which C# cannot
/// express — so it appears here as an interface each control implements by forwarding to the same native
/// calls. The behaviour is identical whichever control it is reached through.
/// </remarks>
public interface ITextEntry
{
    /// <summary>The whole contents. Setting it raises a text-changed event; use <see cref="ChangeValue"/>
    /// to set it without one.</summary>
    string Value { get; set; }

    /// <summary>Sets the contents <em>without</em> raising a text-changed event. This is how a field is
    /// refreshed from the model without the change handler firing straight back at it.</summary>
    void ChangeValue(string value);

    /// <summary>Inserts text at the caret, replacing any selection, and moves the caret past it.</summary>
    void Write(string text);

    /// <summary>Adds text at the end and leaves the caret there.</summary>
    void Append(string text);

    /// <summary>The text between two character positions.</summary>
    string GetRange(int from, int to);

    /// <summary>Replaces a range with other text.</summary>
    void Replace(int from, int to, string value);

    /// <summary>Deletes a range.</summary>
    void Remove(int from, int to);

    void Clear();
    bool IsEmpty { get; }

    // ---- Clipboard and undo ---------------------------------------------------------------------------

    void Copy();
    void Cut();
    void Paste();
    bool CanCopy { get; }
    bool CanCut { get; }
    bool CanPaste { get; }
    void Undo();
    void Redo();
    bool CanUndo { get; }
    bool CanRedo { get; }

    // ---- Caret and selection --------------------------------------------------------------------------

    /// <summary>The caret's character position.</summary>
    int InsertionPoint { get; set; }

    /// <summary>Moves the caret past the last character.</summary>
    void MoveCaretToEnd();

    /// <summary>The position just past the last character, which is also the length.</summary>
    int LastPosition { get; }

    /// <summary>The selected range as [from, to). Equal values mean an empty selection.</summary>
    (int From, int To) Selection { get; set; }

    void SelectAll();

    /// <summary>Collapses the selection, leaving the caret where it was.</summary>
    void SelectNone();

    bool HasSelection { get; }
    string SelectedText { get; }

    /// <summary>Deletes the selection.</summary>
    void RemoveSelection();

    // ---- Constraints and presentation ------------------------------------------------------------------

    bool Editable { get; set; }

    /// <summary>The most characters the user may type. 0 lifts the limit. Typing past it raises
    /// <see cref="WxEvents.TextMaxLengthReached"/> rather than silently truncating.</summary>
    int MaxLength { set; }

    /// <summary>Makes everything typed from now on uppercase.</summary>
    void ForceUpper();

    /// <summary>Placeholder text shown while the field is empty. Returns false where the platform has no
    /// such thing.</summary>
    string Hint { get; set; }

    /// <summary>The inner margins, or (-1, -1) where the platform does not report them.</summary>
    (int Left, int Top) Margins { get; }

    /// <summary>Sets the inner margins. Returns false where the platform refuses.</summary>
    bool SetMargins(int left, int top = -1);

    // ---- Completion ------------------------------------------------------------------------------------

    /// <summary>Completes from a fixed list as the user types.</summary>
    bool AutoComplete(params string[] choices);

    /// <summary>Completes file names.</summary>
    bool AutoCompleteFileNames();

    /// <summary>Completes directory names.</summary>
    bool AutoCompleteDirectories();
}

/// <summary>Forwards the <see cref="ITextEntry"/> surface to wxWidgets. Each control implements the
/// interface with one-line calls into here, so the behaviour cannot drift between them.</summary>
internal static unsafe class TextEntryNative
{
    internal static string GetValue(nint h) => Read(h, NativeMethods.wxsharp_textentry_get_value);
    internal static void SetValue(nint h, string v) => NativeMethods.wxsharp_textentry_set_value(h, v ?? string.Empty);
    internal static void ChangeValue(nint h, string v) => NativeMethods.wxsharp_textentry_change_value(h, v ?? string.Empty);
    internal static void Write(nint h, string t) => NativeMethods.wxsharp_textentry_write_text(h, t ?? string.Empty);
    internal static void Append(nint h, string t) => NativeMethods.wxsharp_textentry_append_text(h, t ?? string.Empty);

    internal static string GetRange(nint h, int from, int to)
    {
        var length = NativeMethods.wxsharp_textentry_get_range(h, from, to, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_textentry_get_range(h, from, to, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    internal static void Replace(nint h, int from, int to, string v) => NativeMethods.wxsharp_textentry_replace(h, from, to, v ?? string.Empty);
    internal static void Remove(nint h, int from, int to) => NativeMethods.wxsharp_textentry_remove(h, from, to);
    internal static void Clear(nint h) => NativeMethods.wxsharp_textentry_clear(h);
    internal static bool IsEmpty(nint h) => NativeMethods.wxsharp_textentry_is_empty(h);

    internal static void Copy(nint h) => NativeMethods.wxsharp_textentry_copy(h);
    internal static void Cut(nint h) => NativeMethods.wxsharp_textentry_cut(h);
    internal static void Paste(nint h) => NativeMethods.wxsharp_textentry_paste(h);
    internal static bool CanCopy(nint h) => NativeMethods.wxsharp_textentry_can_copy(h);
    internal static bool CanCut(nint h) => NativeMethods.wxsharp_textentry_can_cut(h);
    internal static bool CanPaste(nint h) => NativeMethods.wxsharp_textentry_can_paste(h);
    internal static void Undo(nint h) => NativeMethods.wxsharp_textentry_undo(h);
    internal static void Redo(nint h) => NativeMethods.wxsharp_textentry_redo(h);
    internal static bool CanUndo(nint h) => NativeMethods.wxsharp_textentry_can_undo(h);
    internal static bool CanRedo(nint h) => NativeMethods.wxsharp_textentry_can_redo(h);

    internal static int GetInsertionPoint(nint h) => NativeMethods.wxsharp_textentry_get_insertion_point(h);
    internal static void SetInsertionPoint(nint h, int p) => NativeMethods.wxsharp_textentry_set_insertion_point(h, p);
    internal static void MoveCaretToEnd(nint h) => NativeMethods.wxsharp_textentry_set_insertion_point_end(h);
    internal static int LastPosition(nint h) => NativeMethods.wxsharp_textentry_get_last_position(h);

    internal static (int From, int To) GetSelection(nint h)
    {
        NativeMethods.wxsharp_textentry_get_selection(h, out var from, out var to);
        return (from, to);
    }

    internal static void SetSelection(nint h, int from, int to) => NativeMethods.wxsharp_textentry_set_selection(h, from, to);
    internal static void SelectAll(nint h) => NativeMethods.wxsharp_textentry_select_all(h);
    internal static void SelectNone(nint h) => NativeMethods.wxsharp_textentry_select_none(h);
    internal static bool HasSelection(nint h) => NativeMethods.wxsharp_textentry_has_selection(h);
    internal static string SelectedText(nint h) => Read(h, NativeMethods.wxsharp_textentry_get_selected_text);
    internal static void RemoveSelection(nint h) => NativeMethods.wxsharp_textentry_remove_selection(h);

    internal static bool IsEditable(nint h) => NativeMethods.wxsharp_textentry_is_editable(h);
    internal static void SetEditable(nint h, bool e) => NativeMethods.wxsharp_textentry_set_editable(h, e);
    internal static void SetMaxLength(nint h, int l) => NativeMethods.wxsharp_textentry_set_max_length(h, l);
    internal static void ForceUpper(nint h) => NativeMethods.wxsharp_textentry_force_upper(h);
    internal static string GetHint(nint h) => Read(h, NativeMethods.wxsharp_textentry_get_hint);
    internal static void SetHint(nint h, string v) => NativeMethods.wxsharp_textentry_set_hint(h, v ?? string.Empty);

    internal static (int Left, int Top) GetMargins(nint h)
    {
        NativeMethods.wxsharp_textentry_get_margins(h, out var left, out var top);
        return (left, top);
    }

    internal static bool SetMargins(nint h, int left, int top) => NativeMethods.wxsharp_textentry_set_margins(h, left, top);

    internal static bool AutoComplete(nint h, string[] choices)
    {
        ArgumentNullException.ThrowIfNull(choices);
        var utf8 = new byte[choices.Length][];
        var pointers = stackalloc nint[choices.Length == 0 ? 1 : choices.Length];
        for (var i = 0; i < choices.Length; ++i)
            utf8[i] = System.Text.Encoding.UTF8.GetBytes((choices[i] ?? string.Empty) + "\0");

        return WithPinned(h, utf8, choices.Length);
    }

    private static bool WithPinned(nint h, byte[][] utf8, int count)
    {
        // Each string is pinned for the duration of the call; wxWidgets copies them into a wxArrayString.
        var handles = new System.Runtime.InteropServices.GCHandle[count];
        var pointers = new nint[count == 0 ? 1 : count];
        try
        {
            for (var i = 0; i < count; ++i)
            {
                handles[i] = System.Runtime.InteropServices.GCHandle.Alloc(
                    utf8[i], System.Runtime.InteropServices.GCHandleType.Pinned);
                pointers[i] = handles[i].AddrOfPinnedObject();
            }
            fixed (nint* p = pointers)
                return NativeMethods.wxsharp_textentry_auto_complete(h, (byte**)p, count);
        }
        finally
        {
            for (var i = 0; i < count; ++i)
                if (handles[i].IsAllocated) handles[i].Free();
        }
    }

    internal static bool AutoCompleteFileNames(nint h) => NativeMethods.wxsharp_textentry_auto_complete_files(h);
    internal static bool AutoCompleteDirectories(nint h) => NativeMethods.wxsharp_textentry_auto_complete_directories(h);

    private delegate int Reader(nint handle, byte* buffer, int length);

    private static string Read(nint handle, Reader read)
    {
        var length = read(handle, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = read(handle, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
}
