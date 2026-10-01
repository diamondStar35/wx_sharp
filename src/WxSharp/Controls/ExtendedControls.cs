using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WxSharp;

/// <summary>A button that stays pressed or released, following <c>wxToggleButton</c>.</summary>
public class ToggleButton : Control
{
    /// <summary>Wraps a ToggleButton wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal ToggleButton(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    /// <summary>Raised when the button is toggled on or off.</summary>
    public event EventHandler<CommandEventArgs> Toggled
    {
        add => AddHandler(WxEvents.ToggleButtonToggled, value);
        remove => RemoveHandler(WxEvents.ToggleButtonToggled, value);
    }
    /// <summary>Creates a <c>wxToggleButton</c> child of <paramref name="parent"/> with the given label.</summary>
    public ToggleButton(Window parent, string label = "", int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(ToggleButton)
            ? NativeMethods.wxsharp_togglebutton_create(parent.Handle, id, label, Token)
            : NativeMethods.wxsharp_custom_togglebutton_create(parent.Handle, id, label, Token));
    /// <summary>Whether the button is currently pressed in.</summary>
    public bool Value { get => NativeMethods.wxsharp_togglebutton_get(Handle); set => NativeMethods.wxsharp_togglebutton_set(Handle, value); }
}

/// <summary>A progress bar, following <c>wxGauge</c>. Set <see cref="Value"/> for determinate progress or
/// call <see cref="Pulse"/> for indeterminate.</summary>
public class Gauge : Control
{
    /// <summary>Wraps a Gauge wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal Gauge(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    /// <summary>Creates a <c>wxGauge</c> child of <paramref name="parent"/> with the given range,
    /// starting value and orientation.</summary>
    public Gauge(Window parent, int range = 100, int value = 0, Orientation orientation = Orientation.Horizontal,
        int id = WindowId.Any) : base(parent, id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(range);
        Initialize(GetType() == typeof(Gauge)
            ? NativeMethods.wxsharp_gauge_create(parent.Handle, id, range, value, orientation == Orientation.Vertical, Token)
            : NativeMethods.wxsharp_custom_gauge_create(parent.Handle, id, range, value, orientation == Orientation.Vertical, Token));
    }
    /// <summary>The current progress, from 0 to <see cref="Range"/>.</summary>
    public int Value { get => NativeMethods.wxsharp_gauge_get(Handle); set => NativeMethods.wxsharp_gauge_set(Handle, value); }
    /// <summary>The value that represents full progress.</summary>
    public int Range { get => NativeMethods.wxsharp_gauge_get_range(Handle); set { ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value); NativeMethods.wxsharp_gauge_set_range(Handle, value); } }
    /// <summary>Moves the bar back and forth to show that work is going on without saying how far along it
    /// is. Follows <c>wxGauge.Pulse</c>.</summary>
    /// <remarks>
    /// This puts the gauge into indeterminate mode, and <see cref="Value"/> puts it back - on MSW by turning
    /// the PBS_MARQUEE window style on and off. Switching between the two restarts the bar, so a caller that
    /// pulses whenever it has nothing new to report and sets a value when it does will make the bar appear to
    /// reset on every alternation. Pick one for the life of a given operation.
    /// </remarks>
    public void Pulse() => NativeMethods.wxsharp_gauge_pulse(Handle);
    /// <summary>Whether the gauge is drawn vertically.</summary>
    public bool IsVertical => NativeMethods.wxsharp_gauge_is_vertical(Handle);
    /// <summary>Legacy bezel-face width. wxWidgets always returns zero and ignores writes.</summary>
    [Obsolete("Phoenix exposes this legacy wxGauge property, but wxWidgets always returns zero and ignores writes.")]
    public int BezelFace { get => NativeMethods.wxsharp_gauge_get_bezel_face(Handle); set => NativeMethods.wxsharp_gauge_set_bezel_face(Handle, value); }
    /// <summary>Legacy shadow width. wxWidgets always returns zero and ignores writes.</summary>
    [Obsolete("Phoenix exposes this legacy wxGauge property, but wxWidgets always returns zero and ignores writes.")]
    public int ShadowWidth { get => NativeMethods.wxsharp_gauge_get_shadow_width(Handle); set => NativeMethods.wxsharp_gauge_set_shadow_width(Handle, value); }
    /// <summary>Legacy bezel-face getter; always returns zero.</summary>
    [Obsolete("Phoenix exposes this legacy wxGauge method, but wxWidgets always returns zero.")]
    public int GetBezelFace() => NativeMethods.wxsharp_gauge_get_bezel_face(Handle);
    /// <summary>Legacy bezel-face setter; ignored by wxWidgets.</summary>
    [Obsolete("Phoenix exposes this legacy wxGauge method, but wxWidgets ignores it.")]
    public void SetBezelFace(int width) => NativeMethods.wxsharp_gauge_set_bezel_face(Handle, width);
    /// <summary>Legacy shadow-width getter; always returns zero.</summary>
    [Obsolete("Phoenix exposes this legacy wxGauge method, but wxWidgets always returns zero.")]
    public int GetShadowWidth() => NativeMethods.wxsharp_gauge_get_shadow_width(Handle);
    /// <summary>Legacy shadow-width setter; ignored by wxWidgets.</summary>
    [Obsolete("Phoenix exposes this legacy wxGauge method, but wxWidgets ignores it.")]
    public void SetShadowWidth(int width) => NativeMethods.wxsharp_gauge_set_shadow_width(Handle, width);
}

/// <summary>An integer entry with up/down arrows, following <c>wxSpinCtrl</c>.</summary>
public class SpinCtrl : Control
{
    /// <summary>Wraps a SpinCtrl wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal SpinCtrl(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    /// <summary>Raised when the value changes, by the arrows or by committing typed text
    /// (<c>wxEVT_SPINCTRL</c>).</summary>
    public event EventHandler<SpinEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.SpinChanged, value);
        remove => RemoveHandler(WxEvents.SpinChanged, value);
    }

    /// <summary>The text in the entry changed, whether typed or set in code. wxWidgets raises
    /// <c>wxEVT_SPINCTRL</c> only for the arrows and for a value committed by the control, so a value being
    /// typed is seen here and nowhere else - watch both when a range has to be re-checked on every
    /// keystroke.</summary>
    public event EventHandler<CommandEventArgs> TextChanged
    {
        add => AddHandler(WxEvents.TextChanged, value);
        remove => RemoveHandler(WxEvents.TextChanged, value);
    }

    /// <summary>Enter was pressed in the entry, following <c>wxEVT_TEXT_ENTER</c>. Only raised when the
    /// control was created with <see cref="TextCtrlStyle.ProcessEnter"/>.</summary>
    public event EventHandler<CommandEventArgs> TextEntered
    {
        add => AddHandler(WxEvents.TextEntered, value);
        remove => RemoveHandler(WxEvents.TextEntered, value);
    }

    /// <summary>The up arrow was pressed, separately from the value it produced. Veto to refuse the step.</summary>
    public event EventHandler<SpinEventArgs> SpinUp
    {
        add => AddHandler(WxEvents.SpinUp, value);
        remove => RemoveHandler(WxEvents.SpinUp, value);
    }

    /// <summary>The down arrow was pressed.</summary>
    public event EventHandler<SpinEventArgs> SpinDown
    {
        add => AddHandler(WxEvents.SpinDown, value);
        remove => RemoveHandler(WxEvents.SpinDown, value);
    }

    /// <summary>Creates a <c>wxSpinCtrl</c> child of <paramref name="parent"/> with the given value and
    /// <paramref name="minimum"/>..<paramref name="maximum"/> range.</summary>
    public SpinCtrl(Window parent, int value = 0, int minimum = 0, int maximum = 100, int id = WindowId.Any) : base(parent, id)
    {
        if (minimum > maximum) throw new ArgumentException("Minimum cannot exceed maximum.");
        Initialize(GetType() == typeof(SpinCtrl)
            ? NativeMethods.wxsharp_spinctrl_create(parent.Handle, id, minimum, maximum, value, Token)
            : NativeMethods.wxsharp_custom_spinctrl_create(parent.Handle, id, minimum, maximum, value, Token));
    }
    /// <summary>The current value, clamped to the range.</summary>
    public int Value { get => NativeMethods.wxsharp_spinctrl_get(Handle); set => NativeMethods.wxsharp_spinctrl_set(Handle, value); }
    /// <summary>The lowest value the control allows.</summary>
    public int Minimum { get => GetMin(); set => SetMin(value); }
    /// <summary>The highest value the control allows.</summary>
    public int Maximum { get => GetMax(); set => SetMax(value); }
    /// <summary>How much each arrow press changes the value.</summary>
    public int Increment { get => GetIncrement(); set => SetIncrement(value); }
    /// <summary>The numeric base for display: 10 (decimal) or 16 (hexadecimal).</summary>
    public int Base { get => GetBase(); set { if (!SetBase(value)) throw new ArgumentException("The numeric base is not supported.", nameof(value)); } }
    /// <summary>Returns the lowest allowed value.</summary>
    public int GetMin() => NativeMethods.wxsharp_spinctrl_get_min(Handle);
    /// <summary>Returns the highest allowed value.</summary>
    public int GetMax() => NativeMethods.wxsharp_spinctrl_get_max(Handle);
    /// <summary>Returns the allowed range as a tuple.</summary>
    public (int Minimum, int Maximum) GetRange() => (GetMin(), GetMax());
    /// <summary>Sets the lowest allowed value, keeping the current maximum.</summary>
    public void SetMin(int minimum) => SetRange(minimum, GetMax());
    /// <summary>Sets the highest allowed value, keeping the current minimum.</summary>
    public void SetMax(int maximum) => SetRange(GetMin(), maximum);
    /// <summary>Returns the per-step increment.</summary>
    public int GetIncrement() => NativeMethods.wxsharp_spinctrl_get_increment(Handle);
    /// <summary>Sets the per-step increment.</summary>
    public void SetIncrement(int increment) => NativeMethods.wxsharp_spinctrl_set_increment(Handle, increment);
    /// <summary>Returns the display base (10 or 16).</summary>
    public int GetBase() => NativeMethods.wxsharp_spinctrl_get_base(Handle);
    /// <summary>Sets the display base (10 or 16). Returns false if unsupported.</summary>
    public bool SetBase(int numberBase) => NativeMethods.wxsharp_spinctrl_set_base(Handle, numberBase);
    /// <summary>Returns the entry's text as shown, including any base formatting.</summary>
    public unsafe string GetTextValue()
    {
        var length = NativeMethods.wxsharp_spinctrl_get_text_value(Handle, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_spinctrl_get_text_value(Handle, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
    /// <summary>The entry's text as shown. Setting it replaces the displayed text.</summary>
    public string TextValue { get => GetTextValue(); set => NativeMethods.wxsharp_spinctrl_set_text_value(Handle, value ?? string.Empty); }
    /// <summary>Selects the text between <paramref name="from"/> and <paramref name="to"/> in the entry.</summary>
    public void SetSelection(int from, int to) => NativeMethods.wxsharp_spinctrl_set_selection(Handle, from, to);
    /// <summary>Sets the allowed value range.</summary>
    public void SetRange(int minimum, int maximum)
    {
        if (minimum > maximum) throw new ArgumentException("Minimum cannot exceed maximum.");
        NativeMethods.wxsharp_spinctrl_set_range(Handle, minimum, maximum);
    }
}

/// <summary>A drop-down list combined with a text field, following <c>wxComboBox</c>.</summary>
public class ComboBox : Control, ITextEntry
{
    /// <summary>Wraps a ComboBox wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal ComboBox(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    /// <summary>Raised when the selected list item changes (<c>wxEVT_COMBOBOX</c>).</summary>
    public event EventHandler<CommandEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.ComboBoxSelected, value);
        remove => RemoveHandler(WxEvents.ComboBoxSelected, value);
    }
    /// <summary>Raised when the text in the field changes (<c>wxEVT_TEXT</c>).</summary>
    public event EventHandler<CommandEventArgs> TextChanged
    {
        add => AddHandler(WxEvents.TextChanged, value);
        remove => RemoveHandler(WxEvents.TextChanged, value);
    }
    /// <summary>Creates a <c>wxComboBox</c> child of <paramref name="parent"/>. Pass
    /// <paramref name="readOnly"/> for a drop-down the user picks from but cannot type into.</summary>
    public ComboBox(Window parent, string value = "", bool readOnly = false, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(ComboBox)
            ? NativeMethods.wxsharp_combobox_create(parent.Handle, id, value, readOnly, Token)
            : NativeMethods.wxsharp_custom_combobox_create(parent.Handle, id, value, readOnly, Token));

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
    /// <summary>The selected <em>item</em>, not the selected text. wxComboBox inherits
    /// <c>GetStringSelection</c> from both its bases and resolves it to the list's, so this reports what is
    /// chosen rather than what is highlighted in the field. For the highlighted text, read
    /// <see cref="Selection"/> and pass it to <see cref="GetRange"/>.</summary>
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
    /// <summary>Appends an item to the drop-down list.</summary>
    public void Add(string value) => NativeMethods.wxsharp_combobox_append(Handle, value);
    /// <summary>Inserts an item at <paramref name="index"/> in the drop-down list.</summary>
    public void Insert(string value, int index) => NativeMethods.wxsharp_combobox_insert(Handle, value, index);
    /// <summary>Removes the list item at <paramref name="index"/>.</summary>
    public void RemoveAt(int index) => NativeMethods.wxsharp_combobox_delete(Handle, index);
    /// <summary>Empties the control. wxComboBox resolves the ambiguity between its list and its text by
    /// clearing both, and so does this.</summary>
    public void Clear() => NativeMethods.wxsharp_combobox_clear(Handle);
    /// <summary>The number of items in the drop-down list.</summary>
    public int Count => NativeMethods.wxsharp_combobox_count(Handle);

    /// <summary>Gets or replaces the text of the item at <paramref name="index"/>.</summary>
    public unsafe string this[int index]
    {
        get => ReadString((buffer, length) => NativeMethods.wxsharp_combobox_get_string(Handle, index, buffer, length));
        set => NativeMethods.wxsharp_combobox_set_string(Handle, index, value);
    }

    /// <summary>The index of the first item equal to <paramref name="text"/> (case-insensitive), or -1.</summary>
    public int IndexOf(string text) => NativeMethods.wxsharp_combobox_find_string(Handle, text);
    /// <summary>The index of the selected list item, or -1 when none is selected.</summary>
    public int SelectedIndex { get => NativeMethods.wxsharp_combobox_get_selection(Handle); set => NativeMethods.wxsharp_combobox_set_selection(Handle, value); }
    private unsafe delegate int StringReader(byte* buffer, int length);
    private static unsafe string ReadString(StringReader reader)
    {
        var length = reader(null, 0); if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = reader(buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }
}

/// <summary>A text field with a search affordance, following Phoenix's platform-neutral
/// <c>wxSearchCtrl</c> surface.</summary>
/// <remarks>On Windows this native control is a composite <c>wxControl</c> implementing
/// <c>wxTextEntry</c>; it is not a <c>wxTextCtrl</c>. Phoenix deliberately exposes the same common base and
/// transplants the text-entry methods, which avoids invalid wxTextCtrl casts on Windows.</remarks>
public class SearchCtrl : Control, ITextEntry
{
    /// <summary>Raised when the text in the field changes (<c>wxEVT_TEXT</c>).</summary>
    public event EventHandler<CommandEventArgs> TextChanged
    {
        add => AddHandler(WxEvents.TextChanged, value);
        remove => RemoveHandler(WxEvents.TextChanged, value);
    }

    /// <summary>The search button was pressed, or Enter was hit in the field.</summary>
    public event EventHandler<CommandEventArgs> Search
    {
        add => AddHandler(WxEvents.Search, value);
        remove => RemoveHandler(WxEvents.Search, value);
    }

    /// <summary>The cancel button was pressed.</summary>
    public event EventHandler<CommandEventArgs> SearchCancelled
    {
        add => AddHandler(WxEvents.SearchCancelled, value);
        remove => RemoveHandler(WxEvents.SearchCancelled, value);
    }

    /// <summary>Creates a <c>wxSearchCtrl</c> child of <paramref name="parent"/>.</summary>
    public SearchCtrl(Window parent, string value = "", int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(SearchCtrl)
            ? NativeMethods.wxsharp_searchctrl_create(parent.Handle, id, value, Token)
            : NativeMethods.wxsharp_custom_searchctrl_create(parent.Handle, id, value, Token));

    // Phoenix copies the wxTextEntry API onto SearchCtrl instead of pretending this is a wxTextCtrl.
    /// <summary>The text in the field. Setting it raises <see cref="TextChanged"/>.</summary>
    public string Value { get => TextEntryNative.GetValue(Handle); set => TextEntryNative.SetValue(Handle, value); }
    /// <summary>Sets the text without raising <see cref="TextChanged"/>.</summary>
    public void ChangeValue(string value) => TextEntryNative.ChangeValue(Handle, value);
    /// <summary>Inserts text at the caret.</summary>
    public void Write(string text) => TextEntryNative.Write(Handle, text);
    /// <summary>Inserts text at the caret (alias of <see cref="Write"/>).</summary>
    public void WriteText(string text) => Write(text);
    /// <summary>Appends text to the end of the field.</summary>
    public void Append(string text) => TextEntryNative.Append(Handle, text);
    /// <summary>Appends text to the end of the field (alias of <see cref="Append"/>).</summary>
    public void AppendText(string text) => Append(text);
    /// <summary>Returns the text between two character positions.</summary>
    public string GetRange(int from, int to) => TextEntryNative.GetRange(Handle, from, to);
    /// <summary>Replaces the text in a character range.</summary>
    public void Replace(int from, int to, string value) => TextEntryNative.Replace(Handle, from, to, value);
    /// <summary>Removes the text in a character range.</summary>
    public void Remove(int from, int to) => TextEntryNative.Remove(Handle, from, to);
    /// <summary>Clears the field.</summary>
    public void Clear() => TextEntryNative.Clear(Handle);
    /// <summary>Whether the field is empty.</summary>
    public bool IsEmpty => TextEntryNative.IsEmpty(Handle);

    /// <summary>Copies the selection to the clipboard.</summary>
    public void Copy() => TextEntryNative.Copy(Handle);
    /// <summary>Cuts the selection to the clipboard.</summary>
    public void Cut() => TextEntryNative.Cut(Handle);
    /// <summary>Pastes the clipboard at the caret.</summary>
    public void Paste() => TextEntryNative.Paste(Handle);
    /// <summary>Whether there is a selection to copy.</summary>
    public bool CanCopy => TextEntryNative.CanCopy(Handle);
    /// <summary>Whether there is a selection to cut.</summary>
    public bool CanCut => TextEntryNative.CanCut(Handle);
    /// <summary>Whether the clipboard holds text that can be pasted.</summary>
    public bool CanPaste => TextEntryNative.CanPaste(Handle);
    /// <summary>Undoes the last edit.</summary>
    public void Undo() => TextEntryNative.Undo(Handle);
    /// <summary>Redoes the last undone edit.</summary>
    public void Redo() => TextEntryNative.Redo(Handle);
    /// <summary>Whether there is an edit to undo.</summary>
    public bool CanUndo => TextEntryNative.CanUndo(Handle);
    /// <summary>Whether there is an edit to redo.</summary>
    public bool CanRedo => TextEntryNative.CanRedo(Handle);

    /// <summary>The caret position as a character offset.</summary>
    public int InsertionPoint
    {
        get => TextEntryNative.GetInsertionPoint(Handle);
        set => TextEntryNative.SetInsertionPoint(Handle, value);
    }
    /// <summary>Moves the caret to the end of the text.</summary>
    public void MoveCaretToEnd() => TextEntryNative.MoveCaretToEnd(Handle);
    /// <summary>Moves the caret to the end of the text (alias of <see cref="MoveCaretToEnd"/>).</summary>
    public void SetInsertionPointEnd() => MoveCaretToEnd();
    /// <summary>The character offset just past the last character.</summary>
    public int LastPosition => TextEntryNative.LastPosition(Handle);
    /// <summary>The selected character range as (from, to).</summary>
    public (int From, int To) Selection
    {
        get => TextEntryNative.GetSelection(Handle);
        set => TextEntryNative.SetSelection(Handle, value.From, value.To);
    }
    /// <summary>Selects all text.</summary>
    public void SelectAll() => TextEntryNative.SelectAll(Handle);
    /// <summary>Clears the selection.</summary>
    public void SelectNone() => TextEntryNative.SelectNone(Handle);
    /// <summary>Whether any text is selected.</summary>
    public bool HasSelection => TextEntryNative.HasSelection(Handle);
    /// <summary>The selected text.</summary>
    public string SelectedText => TextEntryNative.SelectedText(Handle);
    /// <summary>The selected text (alias of <see cref="SelectedText"/>).</summary>
    public string GetStringSelection() => SelectedText;
    /// <summary>Deletes the selected text.</summary>
    public void RemoveSelection() => TextEntryNative.RemoveSelection(Handle);

    /// <summary>Whether the field can be edited by the user.</summary>
    public bool Editable
    {
        get => TextEntryNative.IsEditable(Handle);
        set => TextEntryNative.SetEditable(Handle, value);
    }
    /// <summary>Whether the field is editable (alias of <see cref="Editable"/>).</summary>
    public bool IsEditable() => Editable;
    /// <summary>Sets whether the field is editable.</summary>
    public void SetEditable(bool editable) => Editable = editable;
    /// <summary>The maximum number of characters the user may type (write-only).</summary>
    public int MaxLength { set => TextEntryNative.SetMaxLength(Handle, value); }
    /// <summary>Sets the maximum number of characters the user may type.</summary>
    public void SetMaxLength(int length) => MaxLength = length;
    /// <summary>Forces typed text to upper case.</summary>
    public void ForceUpper() => TextEntryNative.ForceUpper(Handle);
    /// <summary>Grey placeholder text shown while the field is empty.</summary>
    public string Hint { get => TextEntryNative.GetHint(Handle); set => TextEntryNative.SetHint(Handle, value); }
    /// <summary>Returns the placeholder hint.</summary>
    public string GetHint() => Hint;
    /// <summary>Sets the placeholder hint. Returns false if unsupported.</summary>
    public bool SetHint(string hint)
        => NativeMethods.wxsharp_textentry_set_hint(Handle, hint ?? string.Empty);
    /// <summary>The field's inner margins as (left, top) in pixels.</summary>
    public (int Left, int Top) Margins => TextEntryNative.GetMargins(Handle);
    /// <summary>Returns the inner margins.</summary>
    public (int Left, int Top) GetMargins() => Margins;
    /// <summary>Sets the inner margins. Returns false if unsupported.</summary>
    public bool SetMargins(int left, int top = -1) => TextEntryNative.SetMargins(Handle, left, top);
    /// <summary>Enables autocomplete from a fixed list of choices.</summary>
    public bool AutoComplete(params string[] choices) => TextEntryNative.AutoComplete(Handle, choices);
    /// <summary>Enables autocomplete from file names.</summary>
    public bool AutoCompleteFileNames() => TextEntryNative.AutoCompleteFileNames(Handle);
    /// <summary>Enables autocomplete from directory names.</summary>
    public bool AutoCompleteDirectories() => TextEntryNative.AutoCompleteDirectories(Handle);

    /// <summary>Shows or hides the cancel (clear) button.</summary>
    public void ShowCancelButton(bool show = true) => NativeMethods.wxsharp_searchctrl_show_cancel(Handle, show);
    /// <summary>Whether the cancel button is shown.</summary>
    public bool IsCancelButtonVisible() => NativeMethods.wxsharp_searchctrl_is_cancel_visible(Handle);
    /// <summary>Whether the cancel (clear) button is shown.</summary>
    public bool CancelButtonVisible { get => IsCancelButtonVisible(); set => ShowCancelButton(value); }

    /// <summary>Shows or hides the search button.</summary>
    public void ShowSearchButton(bool show = true) => NativeMethods.wxsharp_searchctrl_show_search(Handle, show);
    /// <summary>Whether the search button is shown.</summary>
    public bool IsSearchButtonVisible() => NativeMethods.wxsharp_searchctrl_is_search_visible(Handle);
    /// <summary>Whether the search button is shown.</summary>
    public bool SearchButtonVisible { get => IsSearchButtonVisible(); set => ShowSearchButton(value); }

    /// <summary>Returns the grey prompt text shown while the field is empty.</summary>
    public unsafe string GetDescriptiveText()
    {
        var length = NativeMethods.wxsharp_searchctrl_get_descriptive_text(Handle, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_searchctrl_get_descriptive_text(Handle, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
    /// <summary>Sets the grey prompt text shown while the field is empty.</summary>
    public void SetDescriptiveText(string text)
        => NativeMethods.wxsharp_searchctrl_set_descriptive_text(Handle, text ?? string.Empty);
    /// <summary>The grey prompt text shown while the field is empty.</summary>
    public string DescriptiveText { get => GetDescriptiveText(); set => SetDescriptiveText(value); }

    /// <summary>Returns the drop-down menu attached to the search button, or null.</summary>
    public Menu? GetMenu()
    {
        var handle = NativeMethods.wxsharp_searchctrl_get_menu(Handle);
        return handle == 0 ? null : Menu.Attach(handle);
    }
    /// <summary>Attaches (or clears) the drop-down menu on the search button. The control takes ownership.</summary>
    public void SetMenu(Menu? menu)
        => NativeMethods.wxsharp_searchctrl_set_menu(Handle, menu?.TransferOwnership() ?? 0);
    /// <summary>The drop-down menu attached to the search button.</summary>
    public Menu? Menu { get => GetMenu(); set => SetMenu(value); }

    /// <summary>Sets a custom bitmap for the search button.</summary>
    public void SetSearchBitmap(Bitmap bitmap)
        => NativeMethods.wxsharp_searchctrl_set_search_bitmap(Handle,
            bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)));
    /// <summary>Sets a custom bitmap for the search button when it has a drop-down menu.</summary>
    public void SetSearchMenuBitmap(Bitmap bitmap)
        => NativeMethods.wxsharp_searchctrl_set_search_menu_bitmap(Handle,
            bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)));
    /// <summary>Sets a custom bitmap for the cancel button.</summary>
    public void SetCancelBitmap(Bitmap bitmap)
        => NativeMethods.wxsharp_searchctrl_set_cancel_bitmap(Handle,
            bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)));
}

/// <summary>A list box with a checkbox beside each item, following <c>wxCheckListBox</c>.</summary>
public class CheckListBox : ListBox
{
    /// <summary>Raised when an item is checked or unchecked (<c>wxEVT_CHECKLISTBOX</c>).</summary>
    public event EventHandler<CommandEventArgs> ItemChecked
    {
        add => AddHandler(WxEvents.CheckListBoxToggled, value);
        remove => RemoveHandler(WxEvents.CheckListBoxToggled, value);
    }
    /// <summary>Creates a <c>wxCheckListBox</c> child of <paramref name="parent"/>.</summary>
    public CheckListBox(Window parent, int id = WindowId.Any) : base(parent, id, deferInitialization: true)
        => Initialize(GetType() == typeof(CheckListBox)
            ? NativeMethods.wxsharp_checklistbox_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_checklistbox_create(parent.Handle, id, Token));
    /// <summary>Whether the item at <paramref name="index"/> is checked.</summary>
    public bool IsChecked(int index) => NativeMethods.wxsharp_checklistbox_is_checked(Handle, index);
    /// <summary>Checks or unchecks the item at <paramref name="index"/>.</summary>
    public void SetChecked(int index, bool value = true) => NativeMethods.wxsharp_checklistbox_check(Handle, index, value);
    /// <summary>Checks or unchecks the item at <paramref name="index"/> (alias of <see cref="SetChecked"/>).</summary>
    public void Check(int index, bool check = true) => SetChecked(index, check);
    /// <summary>Flips the checked state of the item at <paramref name="index"/>.</summary>
    public void Toggle(int index) => SetChecked(index, !IsChecked(index));
    /// <summary>The indices of all checked items.</summary>
    public int[] GetCheckedItems()
    {
        var items = new List<int>();
        for (var i = 0; i < Count; ++i) if (IsChecked(i)) items.Add(i);
        return items.ToArray();
    }
    /// <summary>The labels of all checked items.</summary>
    public string[] GetCheckedStrings()
    {
        var items = GetCheckedItems();
        var values = new string[items.Length];
        for (var i = 0; i < items.Length; ++i) values[i] = this[items[i]];
        return values;
    }
    /// <summary>Checks exactly the items at the given indices and unchecks the rest.</summary>
    public void SetCheckedItems(IEnumerable<int> indexes)
    {
        ArgumentNullException.ThrowIfNull(indexes);
        var selected = new HashSet<int>(indexes);
        foreach (var index in selected)
            if ((uint)index >= (uint)Count) throw new ArgumentOutOfRangeException(nameof(indexes));
        for (var i = 0; i < Count; ++i) SetChecked(i, selected.Contains(i));
    }
    /// <summary>Checks exactly the items with the given labels and unchecks the rest.</summary>
    public void SetCheckedStrings(IEnumerable<string> strings)
    {
        ArgumentNullException.ThrowIfNull(strings);
        var selected = new HashSet<string>(strings, StringComparer.Ordinal);
        foreach (var value in selected)
            if (IndexOf(value) < 0) throw new ArgumentException($"String '{value}' was not found.", nameof(strings));
        for (var i = 0; i < Count; ++i) SetChecked(i, selected.Contains(this[i]));
    }
}

/// <summary>A labelled group of mutually exclusive radio buttons, following <c>wxRadioBox</c>.</summary>
public class RadioBox : Control
{
    /// <summary>Raised when the selected button changes (<c>wxEVT_RADIOBOX</c>).</summary>
    public event EventHandler<CommandEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.RadioBoxSelected, value);
        remove => RemoveHandler(WxEvents.RadioBoxSelected, value);
    }
    /// <summary>Creates a <c>wxRadioBox</c> with the given <paramref name="label"/>, one button per entry in
    /// <paramref name="choices"/>, laid out in <paramref name="columns"/> columns.</summary>
    public unsafe RadioBox(Window parent, string label, IReadOnlyList<string> choices, int columns = 1,
        int id = WindowId.Any) : base(parent, id)
    {
        ArgumentNullException.ThrowIfNull(choices);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columns);
        var strings = new nint[choices.Count];
        try
        {
            for (var i = 0; i < strings.Length; ++i) strings[i] = Marshal.StringToCoTaskMemUTF8(choices[i]);
            fixed (nint* values = strings)
                Initialize(GetType() == typeof(RadioBox)
            ? NativeMethods.wxsharp_radiobox_create(parent.Handle, id, label, values, strings.Length, columns, Token)
            : NativeMethods.wxsharp_custom_radiobox_create(parent.Handle, id, label, values, strings.Length, columns, Token));
        }
        finally { foreach (var value in strings) if (value != 0) Marshal.FreeCoTaskMem(value); }
    }
    /// <summary>The index of the selected button.</summary>
    public int SelectedIndex { get => NativeMethods.wxsharp_radiobox_get_selection(Handle); set => NativeMethods.wxsharp_radiobox_set_selection(Handle, value); }
}

/// <summary>A labelled frame drawn around a group of controls, following <c>wxStaticBox</c>.</summary>
public class StaticBox : Control
{
    /// <summary>Wraps a StaticBox wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal StaticBox(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    /// <summary>Creates a <c>wxStaticBox</c> child of <paramref name="parent"/> with the given label.</summary>
    public StaticBox(Window parent, string label = "", int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(StaticBox)
            ? NativeMethods.wxsharp_staticbox_create(parent.Handle, id, label, Token)
            : NativeMethods.wxsharp_custom_staticbox_create(parent.Handle, id, label, Token));
    /// <summary>The border thickness a sizer should leave inside the box, as (top, other edges) in
    /// pixels.</summary>
    public (int Top, int Other) GetBordersForSizer()
    {
        NativeMethods.wxsharp_staticbox_get_borders(Handle, out var top, out var other);
        return (top, other);
    }
}

/// <summary>A horizontal or vertical divider line, following <c>wxStaticLine</c>.</summary>
public class StaticLine : Control
{
    /// <summary>Creates a <c>wxStaticLine</c> child of <paramref name="parent"/> in the given orientation.</summary>
    public StaticLine(Window parent, Orientation orientation = Orientation.Horizontal, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(StaticLine)
            ? NativeMethods.wxsharp_staticline_create(parent.Handle, id, orientation == Orientation.Vertical, Token)
            : NativeMethods.wxsharp_custom_staticline_create(parent.Handle, id, orientation == Orientation.Vertical, Token));
    /// <summary>The line's default thickness in pixels.</summary>
    public static int DefaultSize => NativeMethods.wxsharp_staticline_default_size();
    /// <summary>Returns the line's default thickness in pixels.</summary>
    public static int GetDefaultSize() => NativeMethods.wxsharp_staticline_default_size();
    /// <summary>Whether the line is drawn vertically.</summary>
    public bool IsVertical => NativeMethods.wxsharp_staticline_is_vertical(Handle);
}

/// <summary>A spinning busy indicator, following <c>wxActivityIndicator</c>.</summary>
public class ActivityIndicator : Control
{
    /// <summary>Creates a <c>wxActivityIndicator</c> child of <paramref name="parent"/>. Start it with
    /// <see cref="Start"/>.</summary>
    public ActivityIndicator(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(ActivityIndicator)
            ? NativeMethods.wxsharp_activity_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_activity_create(parent.Handle, id, Token));
    /// <summary>Whether the indicator is currently animating.</summary>
    public bool IsRunning => NativeMethods.wxsharp_activity_is_running(Handle);
    /// <summary>Starts the animation.</summary>
    public void Start() => NativeMethods.wxsharp_activity_start(Handle);
    /// <summary>Stops the animation.</summary>
    public void Stop() => NativeMethods.wxsharp_activity_stop(Handle);
}

/// <summary>A floating-point entry with up/down arrows, following <c>wxSpinCtrlDouble</c>.</summary>
public class SpinCtrlDouble : Control
{
    /// <summary>Raised when the value changes, by the arrows or by committing typed text
    /// (<c>wxEVT_SPINCTRLDOUBLE</c>).</summary>
    public event EventHandler<SpinEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.SpinDoubleChanged, value);
        remove => RemoveHandler(WxEvents.SpinDoubleChanged, value);
    }

    /// <summary>The text in the entry changed, whether typed or set in code. wxWidgets raises
    /// <c>wxEVT_SPINCTRL</c> only for the arrows and for a value committed by the control, so a value being
    /// typed is seen here and nowhere else - watch both when a range has to be re-checked on every
    /// keystroke.</summary>
    public event EventHandler<CommandEventArgs> TextChanged
    {
        add => AddHandler(WxEvents.TextChanged, value);
        remove => RemoveHandler(WxEvents.TextChanged, value);
    }

    /// <summary>Enter was pressed in the entry, following <c>wxEVT_TEXT_ENTER</c>. Only raised when the
    /// control was created with <see cref="TextCtrlStyle.ProcessEnter"/>.</summary>
    public event EventHandler<CommandEventArgs> TextEntered
    {
        add => AddHandler(WxEvents.TextEntered, value);
        remove => RemoveHandler(WxEvents.TextEntered, value);
    }

    /// <summary>Creates a <c>wxSpinCtrlDouble</c> child of <paramref name="parent"/> with the given value,
    /// range and per-step <paramref name="increment"/>.</summary>
    public SpinCtrlDouble(Window parent, double value = 0, double minimum = 0, double maximum = 100,
        double increment = 1, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(SpinCtrlDouble)
            ? NativeMethods.wxsharp_spinctrldouble_create(parent.Handle, id, minimum, maximum, value, increment, Token)
            : NativeMethods.wxsharp_custom_spinctrldouble_create(parent.Handle, id, minimum, maximum, value, increment, Token));
    /// <summary>The current value, clamped to the range.</summary>
    public double Value { get => NativeMethods.wxsharp_spinctrldouble_get(Handle); set => NativeMethods.wxsharp_spinctrldouble_set(Handle, value); }
    /// <summary>The lowest value the control allows.</summary>
    public double Minimum { get => GetMin(); set => SetMin(value); }
    /// <summary>The highest value the control allows.</summary>
    public double Maximum { get => GetMax(); set => SetMax(value); }
    /// <summary>How much each arrow press changes the value.</summary>
    public double Increment { get => GetIncrement(); set => SetIncrement(value); }
    /// <summary>How many digits are shown after the decimal point.</summary>
    public uint Digits { get => GetDigits(); set => SetDigits(value); }
    /// <summary>Returns the lowest allowed value.</summary>
    public double GetMin() => NativeMethods.wxsharp_spinctrldouble_get_min(Handle);
    /// <summary>Returns the highest allowed value.</summary>
    public double GetMax() => NativeMethods.wxsharp_spinctrldouble_get_max(Handle);
    /// <summary>Returns the allowed range as a tuple.</summary>
    public (double Minimum, double Maximum) GetRange() => (GetMin(), GetMax());
    /// <summary>Sets the lowest allowed value, keeping the current maximum.</summary>
    public void SetMin(double minimum) => SetRange(minimum, GetMax());
    /// <summary>Sets the highest allowed value, keeping the current minimum.</summary>
    public void SetMax(double maximum) => SetRange(GetMin(), maximum);
    /// <summary>Returns the per-step increment.</summary>
    public double GetIncrement() => NativeMethods.wxsharp_spinctrldouble_get_increment(Handle);
    /// <summary>Sets the per-step increment.</summary>
    public void SetIncrement(double increment) => NativeMethods.wxsharp_spinctrldouble_set_increment(Handle, increment);
    /// <summary>Returns the number of digits shown after the decimal point.</summary>
    public uint GetDigits() => NativeMethods.wxsharp_spinctrldouble_get_digits(Handle);
    /// <summary>Sets the number of digits shown after the decimal point.</summary>
    public void SetDigits(uint digits) => NativeMethods.wxsharp_spinctrldouble_set_digits(Handle, digits);
    /// <summary>Sets the allowed value range.</summary>
    public void SetRange(double minimum, double maximum)
        => NativeMethods.wxsharp_spinctrldouble_set_range(Handle, minimum, maximum);
    /// <summary>Returns the entry's text as shown.</summary>
    public unsafe string GetTextValue()
    {
        var length = NativeMethods.wxsharp_spinctrldouble_get_text_value(Handle, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_spinctrldouble_get_text_value(Handle, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
    /// <summary>The entry's text as shown. Setting it replaces the displayed text.</summary>
    public string TextValue { get => GetTextValue(); set => NativeMethods.wxsharp_spinctrldouble_set_text_value(Handle, value ?? string.Empty); }
}

/// <summary>A standalone scrollbar control, following <c>wxScrollBar</c>.</summary>
public class ScrollBar : Control
{
    /// <summary>Raised continuously while the thumb is dragged (<c>wxEVT_SCROLL_THUMBTRACK</c>).</summary>
    public event EventHandler<ScrollEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.ScrollThumbTrack, value);
        remove => RemoveHandler(WxEvents.ScrollThumbTrack, value);
    }
    /// <summary>Dragging finished. The moment to act on an expensive change, rather than on every ValueChanged.</summary>
    public event EventHandler<ScrollEventArgs> ThumbReleased
    {
        add => AddHandler(WxEvents.ScrollThumbReleased, value);
        remove => RemoveHandler(WxEvents.ScrollThumbReleased, value);
    }

    /// <summary>Raised when the line-up/left arrow is used (<c>wxEVT_SCROLL_LINEUP</c>).</summary>
    public event EventHandler<ScrollEventArgs> ScrolledLineUp
    {
        add => AddHandler(WxEvents.ScrollLineUp, value);
        remove => RemoveHandler(WxEvents.ScrollLineUp, value);
    }

    /// <summary>Raised when the line-down/right arrow is used (<c>wxEVT_SCROLL_LINEDOWN</c>).</summary>
    public event EventHandler<ScrollEventArgs> ScrolledLineDown
    {
        add => AddHandler(WxEvents.ScrollLineDown, value);
        remove => RemoveHandler(WxEvents.ScrollLineDown, value);
    }

    /// <summary>Raised when the page-up/left region is used (<c>wxEVT_SCROLL_PAGEUP</c>).</summary>
    public event EventHandler<ScrollEventArgs> ScrolledPageUp
    {
        add => AddHandler(WxEvents.ScrollPageUp, value);
        remove => RemoveHandler(WxEvents.ScrollPageUp, value);
    }

    /// <summary>Raised when the page-down/right region is used (<c>wxEVT_SCROLL_PAGEDOWN</c>).</summary>
    public event EventHandler<ScrollEventArgs> ScrolledPageDown
    {
        add => AddHandler(WxEvents.ScrollPageDown, value);
        remove => RemoveHandler(WxEvents.ScrollPageDown, value);
    }

    /// <summary>Raised when scrolled to the top/left (<c>wxEVT_SCROLL_TOP</c>).</summary>
    public event EventHandler<ScrollEventArgs> ScrolledToTop
    {
        add => AddHandler(WxEvents.ScrollToTop, value);
        remove => RemoveHandler(WxEvents.ScrollToTop, value);
    }

    /// <summary>Raised when scrolled to the bottom/right (<c>wxEVT_SCROLL_BOTTOM</c>).</summary>
    public event EventHandler<ScrollEventArgs> ScrolledToBottom
    {
        add => AddHandler(WxEvents.ScrollToBottom, value);
        remove => RemoveHandler(WxEvents.ScrollToBottom, value);
    }

    /// <summary>Creates a <c>wxScrollBar</c> child of <paramref name="parent"/> in the given orientation.
    /// Configure its extent with <see cref="SetScrollbar"/>.</summary>
    public ScrollBar(Window parent, Orientation orientation = Orientation.Vertical, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(ScrollBar)
            ? NativeMethods.wxsharp_scrollbar_create(parent.Handle, id, orientation == Orientation.Vertical, Token)
            : NativeMethods.wxsharp_custom_scrollbar_create(parent.Handle, id, orientation == Orientation.Vertical, Token));
    /// <summary>The thumb's position within the range.</summary>
    public int ThumbPosition
    {
        get => NativeMethods.wxsharp_scrollbar_get_position(Handle);
        set => NativeMethods.wxsharp_scrollbar_set_position(Handle, value);
    }
    /// <summary>The size of the thumb, in scroll units.</summary>
    public int ThumbSize => NativeMethods.wxsharp_scrollbar_get_thumb_size(Handle);
    /// <summary>The total range the scrollbar covers.</summary>
    public int Range => NativeMethods.wxsharp_scrollbar_get_range(Handle);
    /// <summary>How far a page scroll moves, in scroll units.</summary>
    public int PageSize => NativeMethods.wxsharp_scrollbar_get_page_size(Handle);
    /// <summary>Whether the scrollbar is vertical.</summary>
    public bool IsVertical() => NativeMethods.wxsharp_scrollbar_is_vertical(Handle);
    /// <summary>Returns the thumb position.</summary>
    public int GetThumbPosition() => ThumbPosition;
    /// <summary>Sets the thumb position.</summary>
    public void SetThumbPosition(int position) => ThumbPosition = position;
    /// <summary>Returns the thumb size.</summary>
    public int GetThumbSize() => ThumbSize;
    /// <summary>Returns the total range.</summary>
    public int GetRange() => Range;
    /// <summary>Returns the page size.</summary>
    public int GetPageSize() => PageSize;
    /// <summary>Sets all scrollbar metrics at once, following <c>wxScrollBar.SetScrollbar</c>.</summary>
    public void SetScrollbar(int position, int thumbSize, int range, int pageSize, bool refresh = true)
        => NativeMethods.wxsharp_scrollbar_set_ex(Handle, position, thumbSize, range, pageSize, refresh);
    /// <summary>Sets all scrollbar metrics at once (alias of <see cref="SetScrollbar"/>).</summary>
    public void SetScrollInfo(int position, int thumbSize, int range, int pageSize)
        => SetScrollbar(position, thumbSize, range, pageSize);
}

/// <summary>A clickable link that opens a URL, following <c>wxHyperlinkCtrl</c>.</summary>
public class HyperlinkCtrl : Control
{
    /// <summary>Raised when the link is clicked (<c>wxEVT_HYPERLINK</c>). Skip it to let the default
    /// browser-launch happen.</summary>
    public event EventHandler<HyperlinkEventArgs> Click
    {
        add => AddHandler(WxEvents.HyperlinkClicked, value);
        remove => RemoveHandler(WxEvents.HyperlinkClicked, value);
    }
    /// <summary>Creates a <c>wxHyperlinkCtrl</c> child of <paramref name="parent"/> showing
    /// <paramref name="label"/> and linking to <paramref name="url"/>.</summary>
    public HyperlinkCtrl(Window parent, string label, string url, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(HyperlinkCtrl)
            ? NativeMethods.wxsharp_hyperlink_create(parent.Handle, id, label, url, Token)
            : NativeMethods.wxsharp_custom_hyperlink_create(parent.Handle, id, label, url, Token));
    /// <summary>The URL the link points to.</summary>
    public unsafe string URL
    {
        get
        {
            var length = NativeMethods.wxsharp_hyperlink_get_url(Handle, null, 0); if (length <= 0) return string.Empty;
            var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_hyperlink_get_url(Handle, buffer, bytes.Length);
            return Utf8String.Decode(bytes, length);
        }
        set => NativeMethods.wxsharp_hyperlink_set_url(Handle, value);
    }
    /// <summary>Returns the link URL.</summary>
    public string GetURL() => URL;
    /// <summary>Sets the link URL.</summary>
    public void SetURL(string url) => URL = url;
    /// <summary>Whether the link is marked as already visited.</summary>
    public bool Visited
    {
        get => NativeMethods.wxsharp_hyperlink_get_visited(Handle);
        set => NativeMethods.wxsharp_hyperlink_set_visited(Handle, value);
    }
    /// <summary>Returns whether the link is marked visited.</summary>
    public bool GetVisited() => Visited;
    /// <summary>Sets whether the link is marked visited.</summary>
    public void SetVisited(bool visited = true) => Visited = visited;
    /// <summary>The colour of the link before it is visited.</summary>
    public Colour NormalColour
    {
        get => Colour.FromArgb(NativeMethods.wxsharp_hyperlink_get_normal_colour(Handle));
        set => NativeMethods.wxsharp_hyperlink_set_normal_colour(Handle, value.ToArgb());
    }
    /// <summary>Returns the unvisited link colour.</summary>
    public Colour GetNormalColour() => NormalColour;
    /// <summary>Sets the unvisited link colour.</summary>
    public void SetNormalColour(Colour colour) => NormalColour = colour;
    /// <summary>The colour of the link while the pointer is over it.</summary>
    public Colour HoverColour
    {
        get => Colour.FromArgb(NativeMethods.wxsharp_hyperlink_get_hover_colour(Handle));
        set => NativeMethods.wxsharp_hyperlink_set_hover_colour(Handle, value.ToArgb());
    }
    /// <summary>Returns the hover link colour.</summary>
    public Colour GetHoverColour() => HoverColour;
    /// <summary>Sets the hover link colour.</summary>
    public void SetHoverColour(Colour colour) => HoverColour = colour;
    /// <summary>The colour of the link after it has been visited.</summary>
    public Colour VisitedColour
    {
        get => Colour.FromArgb(NativeMethods.wxsharp_hyperlink_get_visited_colour(Handle));
        set => NativeMethods.wxsharp_hyperlink_set_visited_colour(Handle, value.ToArgb());
    }
    /// <summary>Returns the visited link colour.</summary>
    public Colour GetVisitedColour() => VisitedColour;
    /// <summary>Sets the visited link colour.</summary>
    public void SetVisitedColour(Colour colour) => VisitedColour = colour;
}

/// <summary>Shared base for the date and time picker controls, following <c>wxDateTimePickerCtrl</c>.</summary>
public abstract class DateTimePickerBase : Control
{
    private protected DateTimePickerBase(Window parent, int id) : base(parent, id) { }
    /// <summary>The picked date and time. Only the date part matters for a date picker, the time part for a
    /// time picker.</summary>
    public DateTime Value
    {
        get { NativeMethods.wxsharp_datetime_get(Handle, out var y, out var m, out var d, out var h, out var min, out var s); return new DateTime(y, m, d, h, min, s, DateTimeKind.Local); }
        set => NativeMethods.wxsharp_datetime_set(Handle, value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
    }
    /// <summary>Returns the picked value.</summary>
    public DateTime GetValue() => Value;
    /// <summary>Sets the picked value.</summary>
    public void SetValue(DateTime value) => Value = value;
}

/// <summary>A control for picking a calendar date, following <c>wxDatePickerCtrl</c>.</summary>
public class DatePickerCtrl : DateTimePickerBase
{
    /// <summary>Raised when the selected date changes (<c>wxEVT_DATE_CHANGED</c>).</summary>
    public event EventHandler<DateEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.DateChanged, value);
        remove => RemoveHandler(WxEvents.DateChanged, value);
    }
    /// <summary>Creates a <c>wxDatePickerCtrl</c> child of <paramref name="parent"/>.</summary>
    public DatePickerCtrl(Window parent, int id = WindowId.Any) : base(parent, id) => Initialize(GetType() == typeof(DatePickerCtrl)
            ? NativeMethods.wxsharp_datepicker_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_datepicker_create(parent.Handle, id, Token));
    /// <summary>Gets the allowed date range. Returns false when no range is set.</summary>
    public bool TryGetRange(out DateTime lower, out DateTime upper)
    {
        if (!NativeMethods.wxsharp_datepicker_get_range(Handle, out var y1, out var m1, out var d1, out var y2, out var m2, out var d2))
        {
            lower = default; upper = default; return false;
        }
        lower = new DateTime(y1, m1, d1); upper = new DateTime(y2, m2, d2); return true;
    }
    /// <summary>Returns the allowed date range as (has-range, lower, upper).</summary>
    public (bool HasRange, DateTime Lower, DateTime Upper) GetRange()
    {
        var hasRange = TryGetRange(out var lower, out var upper);
        return (hasRange, lower, upper);
    }
    /// <summary>Restricts the pickable dates to <paramref name="lower"/>..<paramref name="upper"/>.</summary>
    public void SetRange(DateTime lower, DateTime upper)
        => NativeMethods.wxsharp_datepicker_set_range(Handle, lower.Year, lower.Month, lower.Day, upper.Year, upper.Month, upper.Day);
    /// <summary>The text shown when no date is set (write-only).</summary>
    public string NullText { set { ArgumentNullException.ThrowIfNull(value); NativeMethods.wxsharp_datepicker_set_null_text(Handle, value); } }
    /// <summary>Sets the text shown when no date is set.</summary>
    public void SetNullText(string text) { ArgumentNullException.ThrowIfNull(text); NativeMethods.wxsharp_datepicker_set_null_text(Handle, text); }
}

/// <summary>A control for picking a time of day, following <c>wxTimePickerCtrl</c>.</summary>
public class TimePickerCtrl : DateTimePickerBase
{
    /// <summary>Raised when the selected time changes (<c>wxEVT_TIME_CHANGED</c>).</summary>
    public event EventHandler<DateEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.TimeChanged, value);
        remove => RemoveHandler(WxEvents.TimeChanged, value);
    }
    /// <summary>Creates a <c>wxTimePickerCtrl</c> child of <paramref name="parent"/>.</summary>
    public TimePickerCtrl(Window parent, int id = WindowId.Any) : base(parent, id) => Initialize(GetType() == typeof(TimePickerCtrl)
            ? NativeMethods.wxsharp_timepicker_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_timepicker_create(parent.Handle, id, Token));
}
