using System;

namespace WxSharp;

/// <summary>A radio button. Pass <c>groupStart: true</c> on the first button of a group; the buttons that
/// follow it (until the next group start) are mutually exclusive.</summary>
public class RadioButton : Control
{
    public event EventHandler<CommandEventArgs>? Selected;

    public RadioButton(Window parent, int id = WindowId.Any, string label = "", bool groupStart = false,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_radio_create(parent.Handle, id, label, groupStart, Token));
        AccessibleName = label; // wx would otherwise announce "radiobutton"; read the label instead
        ApplyInitialGeometry(position, size);
    }

    public bool Value
    {
        get => NativeMethods.wxsharp_radio_get(Handle);
        set => NativeMethods.wxsharp_radio_set(Handle, value);
    }

    internal override uint Dispatch(in NativeEvent e)
    {
        if (e.Kind != EventKind.Select) return base.Dispatch(e);
        return RaiseCommand(new CommandEventArgs(this, e.Id), Selected);
    }
}
