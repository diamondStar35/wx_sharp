using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WxSharp;

public class ToggleButton : Control
{
    /// <summary>Wraps a ToggleButton wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal ToggleButton(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    public event EventHandler<CommandEventArgs> Toggled
    {
        add => AddHandler(WxEvents.ToggleButtonToggled, value);
        remove => RemoveHandler(WxEvents.ToggleButtonToggled, value);
    }
    public ToggleButton(Window parent, string label = "", int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(ToggleButton)
            ? NativeMethods.wxsharp_togglebutton_create(parent.Handle, id, label, Token)
            : NativeMethods.wxsharp_custom_togglebutton_create(parent.Handle, id, label, Token));
    public bool Value { get => NativeMethods.wxsharp_togglebutton_get(Handle); set => NativeMethods.wxsharp_togglebutton_set(Handle, value); }
}

public class Gauge : Control
{
    /// <summary>Wraps a Gauge wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal Gauge(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    public Gauge(Window parent, int range = 100, int value = 0, Orientation orientation = Orientation.Horizontal,
        int id = WindowId.Any) : base(parent, id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(range);
        Initialize(GetType() == typeof(Gauge)
            ? NativeMethods.wxsharp_gauge_create(parent.Handle, id, range, value, orientation == Orientation.Vertical, Token)
            : NativeMethods.wxsharp_custom_gauge_create(parent.Handle, id, range, value, orientation == Orientation.Vertical, Token));
    }
    public int Value { get => NativeMethods.wxsharp_gauge_get(Handle); set => NativeMethods.wxsharp_gauge_set(Handle, value); }
    public int Range { get => NativeMethods.wxsharp_gauge_get_range(Handle); set { ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value); NativeMethods.wxsharp_gauge_set_range(Handle, value); } }
    public void Pulse() => NativeMethods.wxsharp_gauge_pulse(Handle);
    public bool IsVertical => NativeMethods.wxsharp_gauge_is_vertical(Handle);
    [Obsolete("Phoenix exposes this legacy wxGauge property, but wxWidgets always returns zero and ignores writes.")]
    public int BezelFace { get => NativeMethods.wxsharp_gauge_get_bezel_face(Handle); set => NativeMethods.wxsharp_gauge_set_bezel_face(Handle, value); }
    [Obsolete("Phoenix exposes this legacy wxGauge property, but wxWidgets always returns zero and ignores writes.")]
    public int ShadowWidth { get => NativeMethods.wxsharp_gauge_get_shadow_width(Handle); set => NativeMethods.wxsharp_gauge_set_shadow_width(Handle, value); }
    [Obsolete("Phoenix exposes this legacy wxGauge method, but wxWidgets always returns zero.")]
    public int GetBezelFace() => NativeMethods.wxsharp_gauge_get_bezel_face(Handle);
    [Obsolete("Phoenix exposes this legacy wxGauge method, but wxWidgets ignores it.")]
    public void SetBezelFace(int width) => NativeMethods.wxsharp_gauge_set_bezel_face(Handle, width);
    [Obsolete("Phoenix exposes this legacy wxGauge method, but wxWidgets always returns zero.")]
    public int GetShadowWidth() => NativeMethods.wxsharp_gauge_get_shadow_width(Handle);
    [Obsolete("Phoenix exposes this legacy wxGauge method, but wxWidgets ignores it.")]
    public void SetShadowWidth(int width) => NativeMethods.wxsharp_gauge_set_shadow_width(Handle, width);
}

public class SpinCtrl : Control
{
    /// <summary>Wraps a SpinCtrl wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal SpinCtrl(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

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

    public SpinCtrl(Window parent, int value = 0, int minimum = 0, int maximum = 100, int id = WindowId.Any) : base(parent, id)
    {
        if (minimum > maximum) throw new ArgumentException("Minimum cannot exceed maximum.");
        Initialize(GetType() == typeof(SpinCtrl)
            ? NativeMethods.wxsharp_spinctrl_create(parent.Handle, id, minimum, maximum, value, Token)
            : NativeMethods.wxsharp_custom_spinctrl_create(parent.Handle, id, minimum, maximum, value, Token));
    }
    public int Value { get => NativeMethods.wxsharp_spinctrl_get(Handle); set => NativeMethods.wxsharp_spinctrl_set(Handle, value); }
    public int Minimum { get => GetMin(); set => SetMin(value); }
    public int Maximum { get => GetMax(); set => SetMax(value); }
    public int Increment { get => GetIncrement(); set => SetIncrement(value); }
    public int Base { get => GetBase(); set { if (!SetBase(value)) throw new ArgumentException("The numeric base is not supported.", nameof(value)); } }
    public int GetMin() => NativeMethods.wxsharp_spinctrl_get_min(Handle);
    public int GetMax() => NativeMethods.wxsharp_spinctrl_get_max(Handle);
    public (int Minimum, int Maximum) GetRange() => (GetMin(), GetMax());
    public void SetMin(int minimum) => SetRange(minimum, GetMax());
    public void SetMax(int maximum) => SetRange(GetMin(), maximum);
    public int GetIncrement() => NativeMethods.wxsharp_spinctrl_get_increment(Handle);
    public void SetIncrement(int increment) => NativeMethods.wxsharp_spinctrl_set_increment(Handle, increment);
    public int GetBase() => NativeMethods.wxsharp_spinctrl_get_base(Handle);
    public bool SetBase(int numberBase) => NativeMethods.wxsharp_spinctrl_set_base(Handle, numberBase);
    public unsafe string GetTextValue()
    {
        var length = NativeMethods.wxsharp_spinctrl_get_text_value(Handle, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_spinctrl_get_text_value(Handle, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
    public string TextValue { get => GetTextValue(); set => NativeMethods.wxsharp_spinctrl_set_text_value(Handle, value ?? string.Empty); }
    public void SetSelection(int from, int to) => NativeMethods.wxsharp_spinctrl_set_selection(Handle, from, to);
    public void SetRange(int minimum, int maximum)
    {
        if (minimum > maximum) throw new ArgumentException("Minimum cannot exceed maximum.");
        NativeMethods.wxsharp_spinctrl_set_range(Handle, minimum, maximum);
    }
}

public class ComboBox : Control, ITextEntry
{
    /// <summary>Wraps a ComboBox wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal ComboBox(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    public event EventHandler<CommandEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.ComboBoxSelected, value);
        remove => RemoveHandler(WxEvents.ComboBoxSelected, value);
    }
    public event EventHandler<CommandEventArgs> TextChanged
    {
        add => AddHandler(WxEvents.TextChanged, value);
        remove => RemoveHandler(WxEvents.TextChanged, value);
    }
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
    public void Add(string value) => NativeMethods.wxsharp_combobox_append(Handle, value);
    public void Insert(string value, int index) => NativeMethods.wxsharp_combobox_insert(Handle, value, index);
    public void RemoveAt(int index) => NativeMethods.wxsharp_combobox_delete(Handle, index);
    /// <summary>Empties the control. wxComboBox resolves the ambiguity between its list and its text by
    /// clearing both, and so does this.</summary>
    public void Clear() => NativeMethods.wxsharp_combobox_clear(Handle);
    public int Count => NativeMethods.wxsharp_combobox_count(Handle);

    /// <summary>Gets or replaces the text of the item at <paramref name="index"/>.</summary>
    public unsafe string this[int index]
    {
        get => ReadString((buffer, length) => NativeMethods.wxsharp_combobox_get_string(Handle, index, buffer, length));
        set => NativeMethods.wxsharp_combobox_set_string(Handle, index, value);
    }

    /// <summary>The index of the first item equal to <paramref name="text"/> (case-insensitive), or -1.</summary>
    public int IndexOf(string text) => NativeMethods.wxsharp_combobox_find_string(Handle, text);
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

    public SearchCtrl(Window parent, string value = "", int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(SearchCtrl)
            ? NativeMethods.wxsharp_searchctrl_create(parent.Handle, id, value, Token)
            : NativeMethods.wxsharp_custom_searchctrl_create(parent.Handle, id, value, Token));

    // Phoenix copies the wxTextEntry API onto SearchCtrl instead of pretending this is a wxTextCtrl.
    public string Value { get => TextEntryNative.GetValue(Handle); set => TextEntryNative.SetValue(Handle, value); }
    public void ChangeValue(string value) => TextEntryNative.ChangeValue(Handle, value);
    public void Write(string text) => TextEntryNative.Write(Handle, text);
    public void WriteText(string text) => Write(text);
    public void Append(string text) => TextEntryNative.Append(Handle, text);
    public void AppendText(string text) => Append(text);
    public string GetRange(int from, int to) => TextEntryNative.GetRange(Handle, from, to);
    public void Replace(int from, int to, string value) => TextEntryNative.Replace(Handle, from, to, value);
    public void Remove(int from, int to) => TextEntryNative.Remove(Handle, from, to);
    public void Clear() => TextEntryNative.Clear(Handle);
    public bool IsEmpty => TextEntryNative.IsEmpty(Handle);

    public void Copy() => TextEntryNative.Copy(Handle);
    public void Cut() => TextEntryNative.Cut(Handle);
    public void Paste() => TextEntryNative.Paste(Handle);
    public bool CanCopy => TextEntryNative.CanCopy(Handle);
    public bool CanCut => TextEntryNative.CanCut(Handle);
    public bool CanPaste => TextEntryNative.CanPaste(Handle);
    public void Undo() => TextEntryNative.Undo(Handle);
    public void Redo() => TextEntryNative.Redo(Handle);
    public bool CanUndo => TextEntryNative.CanUndo(Handle);
    public bool CanRedo => TextEntryNative.CanRedo(Handle);

    public int InsertionPoint
    {
        get => TextEntryNative.GetInsertionPoint(Handle);
        set => TextEntryNative.SetInsertionPoint(Handle, value);
    }
    public void MoveCaretToEnd() => TextEntryNative.MoveCaretToEnd(Handle);
    public void SetInsertionPointEnd() => MoveCaretToEnd();
    public int LastPosition => TextEntryNative.LastPosition(Handle);
    public (int From, int To) Selection
    {
        get => TextEntryNative.GetSelection(Handle);
        set => TextEntryNative.SetSelection(Handle, value.From, value.To);
    }
    public void SelectAll() => TextEntryNative.SelectAll(Handle);
    public void SelectNone() => TextEntryNative.SelectNone(Handle);
    public bool HasSelection => TextEntryNative.HasSelection(Handle);
    public string SelectedText => TextEntryNative.SelectedText(Handle);
    public string GetStringSelection() => SelectedText;
    public void RemoveSelection() => TextEntryNative.RemoveSelection(Handle);

    public bool Editable
    {
        get => TextEntryNative.IsEditable(Handle);
        set => TextEntryNative.SetEditable(Handle, value);
    }
    public bool IsEditable() => Editable;
    public void SetEditable(bool editable) => Editable = editable;
    public int MaxLength { set => TextEntryNative.SetMaxLength(Handle, value); }
    public void SetMaxLength(int length) => MaxLength = length;
    public void ForceUpper() => TextEntryNative.ForceUpper(Handle);
    public string Hint { get => TextEntryNative.GetHint(Handle); set => TextEntryNative.SetHint(Handle, value); }
    public string GetHint() => Hint;
    public bool SetHint(string hint)
        => NativeMethods.wxsharp_textentry_set_hint(Handle, hint ?? string.Empty);
    public (int Left, int Top) Margins => TextEntryNative.GetMargins(Handle);
    public (int Left, int Top) GetMargins() => Margins;
    public bool SetMargins(int left, int top = -1) => TextEntryNative.SetMargins(Handle, left, top);
    public bool AutoComplete(params string[] choices) => TextEntryNative.AutoComplete(Handle, choices);
    public bool AutoCompleteFileNames() => TextEntryNative.AutoCompleteFileNames(Handle);
    public bool AutoCompleteDirectories() => TextEntryNative.AutoCompleteDirectories(Handle);

    public void ShowCancelButton(bool show = true) => NativeMethods.wxsharp_searchctrl_show_cancel(Handle, show);
    public bool IsCancelButtonVisible() => NativeMethods.wxsharp_searchctrl_is_cancel_visible(Handle);
    public bool CancelButtonVisible { get => IsCancelButtonVisible(); set => ShowCancelButton(value); }

    public void ShowSearchButton(bool show = true) => NativeMethods.wxsharp_searchctrl_show_search(Handle, show);
    public bool IsSearchButtonVisible() => NativeMethods.wxsharp_searchctrl_is_search_visible(Handle);
    public bool SearchButtonVisible { get => IsSearchButtonVisible(); set => ShowSearchButton(value); }

    public unsafe string GetDescriptiveText()
    {
        var length = NativeMethods.wxsharp_searchctrl_get_descriptive_text(Handle, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_searchctrl_get_descriptive_text(Handle, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
    public void SetDescriptiveText(string text)
        => NativeMethods.wxsharp_searchctrl_set_descriptive_text(Handle, text ?? string.Empty);
    public string DescriptiveText { get => GetDescriptiveText(); set => SetDescriptiveText(value); }

    public Menu? GetMenu()
    {
        var handle = NativeMethods.wxsharp_searchctrl_get_menu(Handle);
        return handle == 0 ? null : Menu.Attach(handle);
    }
    public void SetMenu(Menu? menu)
        => NativeMethods.wxsharp_searchctrl_set_menu(Handle, menu?.TransferOwnership() ?? 0);
    public Menu? Menu { get => GetMenu(); set => SetMenu(value); }

    public void SetSearchBitmap(Bitmap bitmap)
        => NativeMethods.wxsharp_searchctrl_set_search_bitmap(Handle,
            bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)));
    public void SetSearchMenuBitmap(Bitmap bitmap)
        => NativeMethods.wxsharp_searchctrl_set_search_menu_bitmap(Handle,
            bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)));
    public void SetCancelBitmap(Bitmap bitmap)
        => NativeMethods.wxsharp_searchctrl_set_cancel_bitmap(Handle,
            bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)));
}

public class CheckListBox : ListBox
{
    public event EventHandler<CommandEventArgs> ItemChecked
    {
        add => AddHandler(WxEvents.CheckListBoxToggled, value);
        remove => RemoveHandler(WxEvents.CheckListBoxToggled, value);
    }
    public CheckListBox(Window parent, int id = WindowId.Any) : base(parent, id, deferInitialization: true)
        => Initialize(GetType() == typeof(CheckListBox)
            ? NativeMethods.wxsharp_checklistbox_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_checklistbox_create(parent.Handle, id, Token));
    public bool IsChecked(int index) => NativeMethods.wxsharp_checklistbox_is_checked(Handle, index);
    public void SetChecked(int index, bool value = true) => NativeMethods.wxsharp_checklistbox_check(Handle, index, value);
    public void Check(int index, bool check = true) => SetChecked(index, check);
    public void Toggle(int index) => SetChecked(index, !IsChecked(index));
    public int[] GetCheckedItems()
    {
        var items = new List<int>();
        for (var i = 0; i < Count; ++i) if (IsChecked(i)) items.Add(i);
        return items.ToArray();
    }
    public string[] GetCheckedStrings()
    {
        var items = GetCheckedItems();
        var values = new string[items.Length];
        for (var i = 0; i < items.Length; ++i) values[i] = this[items[i]];
        return values;
    }
    public void SetCheckedItems(IEnumerable<int> indexes)
    {
        ArgumentNullException.ThrowIfNull(indexes);
        var selected = new HashSet<int>(indexes);
        foreach (var index in selected)
            if ((uint)index >= (uint)Count) throw new ArgumentOutOfRangeException(nameof(indexes));
        for (var i = 0; i < Count; ++i) SetChecked(i, selected.Contains(i));
    }
    public void SetCheckedStrings(IEnumerable<string> strings)
    {
        ArgumentNullException.ThrowIfNull(strings);
        var selected = new HashSet<string>(strings, StringComparer.Ordinal);
        foreach (var value in selected)
            if (IndexOf(value) < 0) throw new ArgumentException($"String '{value}' was not found.", nameof(strings));
        for (var i = 0; i < Count; ++i) SetChecked(i, selected.Contains(this[i]));
    }
}

public class RadioBox : Control
{
    public event EventHandler<CommandEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.RadioBoxSelected, value);
        remove => RemoveHandler(WxEvents.RadioBoxSelected, value);
    }
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
    public int SelectedIndex { get => NativeMethods.wxsharp_radiobox_get_selection(Handle); set => NativeMethods.wxsharp_radiobox_set_selection(Handle, value); }
}

public class StaticBox : Control
{
    /// <summary>Wraps a StaticBox wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal StaticBox(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    public StaticBox(Window parent, string label = "", int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(StaticBox)
            ? NativeMethods.wxsharp_staticbox_create(parent.Handle, id, label, Token)
            : NativeMethods.wxsharp_custom_staticbox_create(parent.Handle, id, label, Token));
    public (int Top, int Other) GetBordersForSizer()
    {
        NativeMethods.wxsharp_staticbox_get_borders(Handle, out var top, out var other);
        return (top, other);
    }
}

public class StaticLine : Control
{
    public StaticLine(Window parent, Orientation orientation = Orientation.Horizontal, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(StaticLine)
            ? NativeMethods.wxsharp_staticline_create(parent.Handle, id, orientation == Orientation.Vertical, Token)
            : NativeMethods.wxsharp_custom_staticline_create(parent.Handle, id, orientation == Orientation.Vertical, Token));
    public static int DefaultSize => NativeMethods.wxsharp_staticline_default_size();
    public static int GetDefaultSize() => NativeMethods.wxsharp_staticline_default_size();
    public bool IsVertical => NativeMethods.wxsharp_staticline_is_vertical(Handle);
}

public class ActivityIndicator : Control
{
    public ActivityIndicator(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(ActivityIndicator)
            ? NativeMethods.wxsharp_activity_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_activity_create(parent.Handle, id, Token));
    public bool IsRunning => NativeMethods.wxsharp_activity_is_running(Handle);
    public void Start() => NativeMethods.wxsharp_activity_start(Handle);
    public void Stop() => NativeMethods.wxsharp_activity_stop(Handle);
}

public class SpinCtrlDouble : Control
{
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

    public SpinCtrlDouble(Window parent, double value = 0, double minimum = 0, double maximum = 100,
        double increment = 1, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(SpinCtrlDouble)
            ? NativeMethods.wxsharp_spinctrldouble_create(parent.Handle, id, minimum, maximum, value, increment, Token)
            : NativeMethods.wxsharp_custom_spinctrldouble_create(parent.Handle, id, minimum, maximum, value, increment, Token));
    public double Value { get => NativeMethods.wxsharp_spinctrldouble_get(Handle); set => NativeMethods.wxsharp_spinctrldouble_set(Handle, value); }
    public double Minimum { get => GetMin(); set => SetMin(value); }
    public double Maximum { get => GetMax(); set => SetMax(value); }
    public double Increment { get => GetIncrement(); set => SetIncrement(value); }
    public uint Digits { get => GetDigits(); set => SetDigits(value); }
    public double GetMin() => NativeMethods.wxsharp_spinctrldouble_get_min(Handle);
    public double GetMax() => NativeMethods.wxsharp_spinctrldouble_get_max(Handle);
    public (double Minimum, double Maximum) GetRange() => (GetMin(), GetMax());
    public void SetMin(double minimum) => SetRange(minimum, GetMax());
    public void SetMax(double maximum) => SetRange(GetMin(), maximum);
    public double GetIncrement() => NativeMethods.wxsharp_spinctrldouble_get_increment(Handle);
    public void SetIncrement(double increment) => NativeMethods.wxsharp_spinctrldouble_set_increment(Handle, increment);
    public uint GetDigits() => NativeMethods.wxsharp_spinctrldouble_get_digits(Handle);
    public void SetDigits(uint digits) => NativeMethods.wxsharp_spinctrldouble_set_digits(Handle, digits);
    public void SetRange(double minimum, double maximum)
        => NativeMethods.wxsharp_spinctrldouble_set_range(Handle, minimum, maximum);
    public unsafe string GetTextValue()
    {
        var length = NativeMethods.wxsharp_spinctrldouble_get_text_value(Handle, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_spinctrldouble_get_text_value(Handle, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
    public string TextValue { get => GetTextValue(); set => NativeMethods.wxsharp_spinctrldouble_set_text_value(Handle, value ?? string.Empty); }
}

public class ScrollBar : Control
{
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

    public event EventHandler<ScrollEventArgs> ScrolledLineUp
    {
        add => AddHandler(WxEvents.ScrollLineUp, value);
        remove => RemoveHandler(WxEvents.ScrollLineUp, value);
    }

    public event EventHandler<ScrollEventArgs> ScrolledLineDown
    {
        add => AddHandler(WxEvents.ScrollLineDown, value);
        remove => RemoveHandler(WxEvents.ScrollLineDown, value);
    }

    public event EventHandler<ScrollEventArgs> ScrolledPageUp
    {
        add => AddHandler(WxEvents.ScrollPageUp, value);
        remove => RemoveHandler(WxEvents.ScrollPageUp, value);
    }

    public event EventHandler<ScrollEventArgs> ScrolledPageDown
    {
        add => AddHandler(WxEvents.ScrollPageDown, value);
        remove => RemoveHandler(WxEvents.ScrollPageDown, value);
    }

    public event EventHandler<ScrollEventArgs> ScrolledToTop
    {
        add => AddHandler(WxEvents.ScrollToTop, value);
        remove => RemoveHandler(WxEvents.ScrollToTop, value);
    }

    public event EventHandler<ScrollEventArgs> ScrolledToBottom
    {
        add => AddHandler(WxEvents.ScrollToBottom, value);
        remove => RemoveHandler(WxEvents.ScrollToBottom, value);
    }

    public ScrollBar(Window parent, Orientation orientation = Orientation.Vertical, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(ScrollBar)
            ? NativeMethods.wxsharp_scrollbar_create(parent.Handle, id, orientation == Orientation.Vertical, Token)
            : NativeMethods.wxsharp_custom_scrollbar_create(parent.Handle, id, orientation == Orientation.Vertical, Token));
    public int ThumbPosition
    {
        get => NativeMethods.wxsharp_scrollbar_get_position(Handle);
        set => NativeMethods.wxsharp_scrollbar_set_position(Handle, value);
    }
    public int ThumbSize => NativeMethods.wxsharp_scrollbar_get_thumb_size(Handle);
    public int Range => NativeMethods.wxsharp_scrollbar_get_range(Handle);
    public int PageSize => NativeMethods.wxsharp_scrollbar_get_page_size(Handle);
    public bool IsVertical() => NativeMethods.wxsharp_scrollbar_is_vertical(Handle);
    public int GetThumbPosition() => ThumbPosition;
    public void SetThumbPosition(int position) => ThumbPosition = position;
    public int GetThumbSize() => ThumbSize;
    public int GetRange() => Range;
    public int GetPageSize() => PageSize;
    public void SetScrollbar(int position, int thumbSize, int range, int pageSize, bool refresh = true)
        => NativeMethods.wxsharp_scrollbar_set_ex(Handle, position, thumbSize, range, pageSize, refresh);
    public void SetScrollInfo(int position, int thumbSize, int range, int pageSize)
        => SetScrollbar(position, thumbSize, range, pageSize);
}

public class HyperlinkCtrl : Control
{
    public event EventHandler<HyperlinkEventArgs> Click
    {
        add => AddHandler(WxEvents.HyperlinkClicked, value);
        remove => RemoveHandler(WxEvents.HyperlinkClicked, value);
    }
    public HyperlinkCtrl(Window parent, string label, string url, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(HyperlinkCtrl)
            ? NativeMethods.wxsharp_hyperlink_create(parent.Handle, id, label, url, Token)
            : NativeMethods.wxsharp_custom_hyperlink_create(parent.Handle, id, label, url, Token));
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
    public string GetURL() => URL;
    public void SetURL(string url) => URL = url;
    public bool Visited
    {
        get => NativeMethods.wxsharp_hyperlink_get_visited(Handle);
        set => NativeMethods.wxsharp_hyperlink_set_visited(Handle, value);
    }
    public bool GetVisited() => Visited;
    public void SetVisited(bool visited = true) => Visited = visited;
    public Colour NormalColour
    {
        get => Colour.FromArgb(NativeMethods.wxsharp_hyperlink_get_normal_colour(Handle));
        set => NativeMethods.wxsharp_hyperlink_set_normal_colour(Handle, value.ToArgb());
    }
    public Colour GetNormalColour() => NormalColour;
    public void SetNormalColour(Colour colour) => NormalColour = colour;
    public Colour HoverColour
    {
        get => Colour.FromArgb(NativeMethods.wxsharp_hyperlink_get_hover_colour(Handle));
        set => NativeMethods.wxsharp_hyperlink_set_hover_colour(Handle, value.ToArgb());
    }
    public Colour GetHoverColour() => HoverColour;
    public void SetHoverColour(Colour colour) => HoverColour = colour;
    public Colour VisitedColour
    {
        get => Colour.FromArgb(NativeMethods.wxsharp_hyperlink_get_visited_colour(Handle));
        set => NativeMethods.wxsharp_hyperlink_set_visited_colour(Handle, value.ToArgb());
    }
    public Colour GetVisitedColour() => VisitedColour;
    public void SetVisitedColour(Colour colour) => VisitedColour = colour;
}

public abstract class DateTimePickerBase : Control
{
    private protected DateTimePickerBase(Window parent, int id) : base(parent, id) { }
    public DateTime Value
    {
        get { NativeMethods.wxsharp_datetime_get(Handle, out var y, out var m, out var d, out var h, out var min, out var s); return new DateTime(y, m, d, h, min, s, DateTimeKind.Local); }
        set => NativeMethods.wxsharp_datetime_set(Handle, value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
    }
    public DateTime GetValue() => Value;
    public void SetValue(DateTime value) => Value = value;
}

public class DatePickerCtrl : DateTimePickerBase
{
    public event EventHandler<DateEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.DateChanged, value);
        remove => RemoveHandler(WxEvents.DateChanged, value);
    }
    public DatePickerCtrl(Window parent, int id = WindowId.Any) : base(parent, id) => Initialize(GetType() == typeof(DatePickerCtrl)
            ? NativeMethods.wxsharp_datepicker_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_datepicker_create(parent.Handle, id, Token));
    public bool TryGetRange(out DateTime lower, out DateTime upper)
    {
        if (!NativeMethods.wxsharp_datepicker_get_range(Handle, out var y1, out var m1, out var d1, out var y2, out var m2, out var d2))
        {
            lower = default; upper = default; return false;
        }
        lower = new DateTime(y1, m1, d1); upper = new DateTime(y2, m2, d2); return true;
    }
    public (bool HasRange, DateTime Lower, DateTime Upper) GetRange()
    {
        var hasRange = TryGetRange(out var lower, out var upper);
        return (hasRange, lower, upper);
    }
    public void SetRange(DateTime lower, DateTime upper)
        => NativeMethods.wxsharp_datepicker_set_range(Handle, lower.Year, lower.Month, lower.Day, upper.Year, upper.Month, upper.Day);
    public string NullText { set { ArgumentNullException.ThrowIfNull(value); NativeMethods.wxsharp_datepicker_set_null_text(Handle, value); } }
    public void SetNullText(string text) { ArgumentNullException.ThrowIfNull(text); NativeMethods.wxsharp_datepicker_set_null_text(Handle, text); }
}

public class TimePickerCtrl : DateTimePickerBase
{
    public event EventHandler<DateEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.TimeChanged, value);
        remove => RemoveHandler(WxEvents.TimeChanged, value);
    }
    public TimePickerCtrl(Window parent, int id = WindowId.Any) : base(parent, id) => Initialize(GetType() == typeof(TimePickerCtrl)
            ? NativeMethods.wxsharp_timepicker_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_timepicker_create(parent.Handle, id, Token));
}
