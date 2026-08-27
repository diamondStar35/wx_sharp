using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WxSharp;

public class ToggleButton : Control
{
    public event EventHandler<CommandEventArgs> Toggled
    {
        add => AddHandler(WxEvents.ToggleButtonToggled, value);
        remove => RemoveHandler(WxEvents.ToggleButtonToggled, value);
    }
    public ToggleButton(Window parent, string label = "", int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_togglebutton_create(parent.Handle, id, label, Token));
    public bool Value { get => NativeMethods.wxsharp_togglebutton_get(Handle); set => NativeMethods.wxsharp_togglebutton_set(Handle, value); }
}

public class Gauge : Control
{
    public Gauge(Window parent, int range = 100, int value = 0, Orientation orientation = Orientation.Horizontal,
        int id = WindowId.Any) : base(parent, id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(range);
        Initialize(NativeMethods.wxsharp_gauge_create(parent.Handle, id, range, value, orientation == Orientation.Vertical, Token));
    }
    public int Value { get => NativeMethods.wxsharp_gauge_get(Handle); set => NativeMethods.wxsharp_gauge_set(Handle, value); }
    public int Range { get => NativeMethods.wxsharp_gauge_get_range(Handle); set { ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value); NativeMethods.wxsharp_gauge_set_range(Handle, value); } }
    public void Pulse() => NativeMethods.wxsharp_gauge_pulse(Handle);
}

public class SpinCtrl : Control
{
    public event EventHandler<SpinEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.SpinChanged, value);
        remove => RemoveHandler(WxEvents.SpinChanged, value);
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
        Initialize(NativeMethods.wxsharp_spinctrl_create(parent.Handle, id, minimum, maximum, value, Token));
    }
    public int Value { get => NativeMethods.wxsharp_spinctrl_get(Handle); set => NativeMethods.wxsharp_spinctrl_set(Handle, value); }
    public void SetRange(int minimum, int maximum)
    {
        if (minimum > maximum) throw new ArgumentException("Minimum cannot exceed maximum.");
        NativeMethods.wxsharp_spinctrl_set_range(Handle, minimum, maximum);
    }
}

public class ComboBox : Control, ITextEntry
{
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
        => Initialize(NativeMethods.wxsharp_combobox_create(parent.Handle, id, value, readOnly, Token));

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

/// <summary>A text field with a search affordance, following <c>wxSearchCtrl</c>. It derives from
/// <see cref="TextCtrl"/> because wxWidgets does, so the whole editing surface comes with it.</summary>
public class SearchCtrl : TextCtrl
{
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
        => Initialize(NativeMethods.wxsharp_searchctrl_create(parent.Handle, id, value, Token));

    /// <summary>Whether the cancel button is shown.</summary>
    public bool ShowCancelButton { set => NativeMethods.wxsharp_searchctrl_show_cancel(Handle, value); }

    /// <summary>Whether the search button is shown.</summary>
    public bool ShowSearchButton { set => NativeMethods.wxsharp_searchctrl_show_search(Handle, value); }
}

public class CheckListBox : Control
{
    public event EventHandler<CommandEventArgs> ItemChecked
    {
        add => AddHandler(WxEvents.CheckListBoxToggled, value);
        remove => RemoveHandler(WxEvents.CheckListBoxToggled, value);
    }
    public event EventHandler<CommandEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.ListBoxSelected, value);
        remove => RemoveHandler(WxEvents.ListBoxSelected, value);
    }
    public CheckListBox(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_checklistbox_create(parent.Handle, id, Token));
    public void Add(string value) => NativeMethods.wxsharp_checklistbox_append(Handle, value);
    public int Count => NativeMethods.wxsharp_checklistbox_count(Handle);
    public bool IsChecked(int index) => NativeMethods.wxsharp_checklistbox_is_checked(Handle, index);
    public void SetChecked(int index, bool value = true) => NativeMethods.wxsharp_checklistbox_check(Handle, index, value);
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
                Initialize(NativeMethods.wxsharp_radiobox_create(parent.Handle, id, label, values, strings.Length, columns, Token));
        }
        finally { foreach (var value in strings) if (value != 0) Marshal.FreeCoTaskMem(value); }
    }
    public int SelectedIndex { get => NativeMethods.wxsharp_radiobox_get_selection(Handle); set => NativeMethods.wxsharp_radiobox_set_selection(Handle, value); }
}

public class StaticBox : Control
{
    public StaticBox(Window parent, string label = "", int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_staticbox_create(parent.Handle, id, label, Token));
}

public class StaticLine : Control
{
    public StaticLine(Window parent, Orientation orientation = Orientation.Horizontal, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_staticline_create(parent.Handle, id, orientation == Orientation.Vertical, Token));
}

public class ActivityIndicator : Control
{
    public ActivityIndicator(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_activity_create(parent.Handle, id, Token));
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
    public SpinCtrlDouble(Window parent, double value = 0, double minimum = 0, double maximum = 100,
        double increment = 1, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_spinctrldouble_create(parent.Handle, id, minimum, maximum, value, increment, Token));
    public double Value { get => NativeMethods.wxsharp_spinctrldouble_get(Handle); set => NativeMethods.wxsharp_spinctrldouble_set(Handle, value); }
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
        => Initialize(NativeMethods.wxsharp_scrollbar_create(parent.Handle, id, orientation == Orientation.Vertical, Token));
    public int ThumbPosition => NativeMethods.wxsharp_scrollbar_get_position(Handle);
    public void SetScrollInfo(int position, int thumbSize, int range, int pageSize)
        => NativeMethods.wxsharp_scrollbar_set(Handle, position, thumbSize, range, pageSize);
}

public class HyperlinkCtrl : Control
{
    public event EventHandler<HyperlinkEventArgs> Click
    {
        add => AddHandler(WxEvents.HyperlinkClicked, value);
        remove => RemoveHandler(WxEvents.HyperlinkClicked, value);
    }
    public HyperlinkCtrl(Window parent, string label, string url, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_hyperlink_create(parent.Handle, id, label, url, Token));
    public unsafe string Url
    {
        get
        {
            var length = NativeMethods.wxsharp_hyperlink_get_url(Handle, null, 0); if (length <= 0) return string.Empty;
            var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_hyperlink_get_url(Handle, buffer, bytes.Length);
            return Utf8String.Decode(bytes, length);
        }
        set => NativeMethods.wxsharp_hyperlink_set_url(Handle, value);
    }
}

public abstract class DateTimePickerBase : Control
{
    private protected DateTimePickerBase(Window parent, int id) : base(parent, id) { }
    public DateTime Value
    {
        get { NativeMethods.wxsharp_datetime_get(Handle, out var y, out var m, out var d, out var h, out var min, out var s); return new DateTime(y, m, d, h, min, s, DateTimeKind.Local); }
        set => NativeMethods.wxsharp_datetime_set(Handle, value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
    }
}

public class DatePickerCtrl : DateTimePickerBase
{
    public event EventHandler<DateEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.DateChanged, value);
        remove => RemoveHandler(WxEvents.DateChanged, value);
    }
    public DatePickerCtrl(Window parent, int id = WindowId.Any) : base(parent, id) => Initialize(NativeMethods.wxsharp_datepicker_create(parent.Handle, id, Token));
}

public class TimePickerCtrl : DateTimePickerBase
{
    public event EventHandler<DateEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.TimeChanged, value);
        remove => RemoveHandler(WxEvents.TimeChanged, value);
    }
    public TimePickerCtrl(Window parent, int id = WindowId.Any) : base(parent, id) => Initialize(NativeMethods.wxsharp_timepicker_create(parent.Handle, id, Token));
}
