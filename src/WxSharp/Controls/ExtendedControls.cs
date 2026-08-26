using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WxSharp;

public class ToggleButton : Control
{
    public event EventHandler<CommandEventArgs>? Toggled;
    public ToggleButton(Window parent, string label = "", int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_togglebutton_create(parent.Handle, id, label, Token));
    public bool Value { get => NativeMethods.wxsharp_togglebutton_get(Handle); set => NativeMethods.wxsharp_togglebutton_set(Handle, value); }
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Toggle
        ? RaiseCommand(new CommandEventArgs(this, e.Id), Toggled) : base.Dispatch(e);
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
    public event EventHandler<CommandEventArgs>? ValueChanged;
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
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Slider
        ? RaiseCommand(new CommandEventArgs(this, e.Id), ValueChanged) : base.Dispatch(e);
}

public class ComboBox : Control
{
    public event EventHandler<CommandEventArgs>? SelectionChanged;
    public event EventHandler<CommandEventArgs>? TextChanged;
    public ComboBox(Window parent, string value = "", bool readOnly = false, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_combobox_create(parent.Handle, id, value, readOnly, Token));
    public unsafe string Value
    {
        get => ReadString((buffer, length) => NativeMethods.wxsharp_combobox_get_value(Handle, buffer, length));
        set => NativeMethods.wxsharp_combobox_set_value(Handle, value);
    }
    public void Add(string value) => NativeMethods.wxsharp_combobox_append(Handle, value);
    public void Clear() => NativeMethods.wxsharp_combobox_clear(Handle);
    public int Count => NativeMethods.wxsharp_combobox_count(Handle);
    public int SelectedIndex { get => NativeMethods.wxsharp_combobox_get_selection(Handle); set => NativeMethods.wxsharp_combobox_set_selection(Handle, value); }
    internal override uint Dispatch(in NativeEvent e) => e.Kind switch
    {
        EventKind.Select => RaiseCommand(new CommandEventArgs(this, e.Id), SelectionChanged),
        EventKind.Text => RaiseCommand(new CommandEventArgs(this, e.Id), TextChanged),
        _ => base.Dispatch(e),
    };
    private unsafe delegate int StringReader(byte* buffer, int length);
    private static unsafe string ReadString(StringReader reader)
    {
        var length = reader(null, 0); if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = reader(buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }
}

public class SearchCtrl : Control
{
    public event EventHandler<CommandEventArgs>? TextChanged;
    public event EventHandler<CommandEventArgs>? Search;
    public SearchCtrl(Window parent, string value = "", int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_searchctrl_create(parent.Handle, id, value, Token));
    public unsafe string Value
    {
        get
        {
            var length = NativeMethods.wxsharp_searchctrl_get_value(Handle, null, 0); if (length <= 0) return string.Empty;
            var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_searchctrl_get_value(Handle, buffer, bytes.Length);
            return Utf8String.Decode(bytes, length);
        }
        set => NativeMethods.wxsharp_searchctrl_set_value(Handle, value);
    }
    public bool ShowCancelButton { set => NativeMethods.wxsharp_searchctrl_show_cancel(Handle, value); }
    public bool ShowSearchButton { set => NativeMethods.wxsharp_searchctrl_show_search(Handle, value); }
    internal override uint Dispatch(in NativeEvent e) => e.Kind switch
    {
        EventKind.Text => RaiseCommand(new CommandEventArgs(this, e.Id), TextChanged),
        EventKind.TextEnter => RaiseCommand(new CommandEventArgs(this, e.Id), Search),
        _ => base.Dispatch(e),
    };
}

public class CheckListBox : Control
{
    public event EventHandler<CommandEventArgs>? ItemChecked;
    public event EventHandler<CommandEventArgs>? SelectionChanged;
    public CheckListBox(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_checklistbox_create(parent.Handle, id, Token));
    public void Add(string value) => NativeMethods.wxsharp_checklistbox_append(Handle, value);
    public int Count => NativeMethods.wxsharp_checklistbox_count(Handle);
    public bool IsChecked(int index) => NativeMethods.wxsharp_checklistbox_is_checked(Handle, index);
    public void SetChecked(int index, bool value = true) => NativeMethods.wxsharp_checklistbox_check(Handle, index, value);
    internal override uint Dispatch(in NativeEvent e) => e.Kind switch
    {
        EventKind.Toggle => RaiseCommand(new CommandEventArgs(this, e.Id), ItemChecked),
        EventKind.Select => RaiseCommand(new CommandEventArgs(this, e.Id), SelectionChanged),
        _ => base.Dispatch(e),
    };
}

public class RadioBox : Control
{
    public event EventHandler<CommandEventArgs>? SelectionChanged;
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
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Select
        ? RaiseCommand(new CommandEventArgs(this, e.Id), SelectionChanged) : base.Dispatch(e);
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
    public event EventHandler<CommandEventArgs>? ValueChanged;
    public SpinCtrlDouble(Window parent, double value = 0, double minimum = 0, double maximum = 100,
        double increment = 1, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_spinctrldouble_create(parent.Handle, id, minimum, maximum, value, increment, Token));
    public double Value { get => NativeMethods.wxsharp_spinctrldouble_get(Handle); set => NativeMethods.wxsharp_spinctrldouble_set(Handle, value); }
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Slider
        ? RaiseCommand(new CommandEventArgs(this, e.Id), ValueChanged) : base.Dispatch(e);
}

public class ScrollBar : Control
{
    public event EventHandler<CommandEventArgs>? ValueChanged;
    public ScrollBar(Window parent, Orientation orientation = Orientation.Vertical, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_scrollbar_create(parent.Handle, id, orientation == Orientation.Vertical, Token));
    public int ThumbPosition => NativeMethods.wxsharp_scrollbar_get_position(Handle);
    public void SetScrollInfo(int position, int thumbSize, int range, int pageSize)
        => NativeMethods.wxsharp_scrollbar_set(Handle, position, thumbSize, range, pageSize);
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Slider
        ? RaiseCommand(new CommandEventArgs(this, e.Id), ValueChanged) : base.Dispatch(e);
}

public class HyperlinkCtrl : Control
{
    public event EventHandler<CommandEventArgs>? Click;
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
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Click
        ? RaiseCommand(new CommandEventArgs(this, e.Id), Click) : base.Dispatch(e);
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
    public event EventHandler<CommandEventArgs>? ValueChanged;
    public DatePickerCtrl(Window parent, int id = WindowId.Any) : base(parent, id) => Initialize(NativeMethods.wxsharp_datepicker_create(parent.Handle, id, Token));
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Select ? RaiseCommand(new CommandEventArgs(this, e.Id), ValueChanged) : base.Dispatch(e);
}

public class TimePickerCtrl : DateTimePickerBase
{
    public event EventHandler<CommandEventArgs>? ValueChanged;
    public TimePickerCtrl(Window parent, int id = WindowId.Any) : base(parent, id) => Initialize(NativeMethods.wxsharp_timepicker_create(parent.Handle, id, Token));
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Select ? RaiseCommand(new CommandEventArgs(this, e.Id), ValueChanged) : base.Dispatch(e);
}
