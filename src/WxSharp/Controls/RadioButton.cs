using System;

namespace WxSharp;

/// <summary>A radio button. Pass <c>groupStart: true</c> on the first button of a group; the buttons that
/// follow it (until the next group start) are mutually exclusive.</summary>
public class RadioButton : Control
{
    /// <summary>Wraps a RadioButton wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal RadioButton(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    public event EventHandler<CommandEventArgs> Selected
    {
        add => AddHandler(WxEvents.RadioButtonSelected, value);
        remove => RemoveHandler(WxEvents.RadioButtonSelected, value);
    }

    public RadioButton(Window parent, int id = WindowId.Any, string label = "", bool groupStart = false,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(GetType() == typeof(RadioButton)
            ? NativeMethods.wxsharp_radio_create(parent.Handle, id, label, groupStart, Token)
            : NativeMethods.wxsharp_custom_radio_create(parent.Handle, id, label, groupStart, Token));
        ApplyInitialGeometry(position, size);
    }

    public bool Value
    {
        get => NativeMethods.wxsharp_radio_get(Handle);
        set => NativeMethods.wxsharp_radio_set(Handle, value);
    }
    public RadioButton? GetFirstInGroup() => App.Lookup(NativeMethods.wxsharp_radio_get_first(Handle)) as RadioButton;
    public RadioButton? GetLastInGroup() => App.Lookup(NativeMethods.wxsharp_radio_get_last(Handle)) as RadioButton;
    public RadioButton? GetPreviousInGroup() => App.Lookup(NativeMethods.wxsharp_radio_get_previous(Handle)) as RadioButton;
    public RadioButton? GetNextInGroup() => App.Lookup(NativeMethods.wxsharp_radio_get_next(Handle)) as RadioButton;
}
