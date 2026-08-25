using System;

namespace WxSharp;

/// <summary>A radio button. Pass <c>groupStart: true</c> on the first button of a group; the buttons that
/// follow it (until the next group start) are mutually exclusive.</summary>
public class RadioButton : Control
{
    public event Action? Selected;

    public RadioButton(Container parent, string label, bool groupStart = false)
    {
        Init(parent, NativeMethods.wxsharp_radio_create(parent.Panel, label, groupStart, Id));
        AccessibleName = label; // wx would otherwise announce "radiobutton"; read the label instead
    }

    public bool Value
    {
        get => NativeMethods.wxsharp_radio_get(Handle);
        set => NativeMethods.wxsharp_radio_set(Handle, value);
    }

    private protected override void OnEvent(EventKind evt)
    {
        if (evt == EventKind.Select)
            Selected?.Invoke();
    }
}
