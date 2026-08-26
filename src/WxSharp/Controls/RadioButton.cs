using System;

namespace WxSharp;

/// <summary>A radio button. Pass <c>groupStart: true</c> on the first button of a group; the buttons that
/// follow it (until the next group start) are mutually exclusive.</summary>
public class RadioButton : Control
{
    public event EventHandler<CommandEventArgs> Selected
    {
        add => AddHandler(WxEvents.RadioButtonSelected, value);
        remove => RemoveHandler(WxEvents.RadioButtonSelected, value);
    }

    public RadioButton(Window parent, int id = WindowId.Any, string label = "", bool groupStart = false,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_radio_create(parent.Handle, id, label, groupStart, Token));
        ApplyInitialGeometry(position, size);
    }

    public bool Value
    {
        get => NativeMethods.wxsharp_radio_get(Handle);
        set => NativeMethods.wxsharp_radio_set(Handle, value);
    }
}
