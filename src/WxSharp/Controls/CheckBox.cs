using System;

namespace WxSharp;

/// <summary>A check box.</summary>
public class CheckBox : Control
{
    public event Action? Toggled;

    public CheckBox(Container parent, string label, CheckBoxStyle style = CheckBoxStyle.TwoState)
    {
        Init(parent, NativeMethods.wxsharp_checkbox_create(parent.Panel, label, (int)style, Id));
        AccessibleName = label; // wx would otherwise announce "check"; make screen readers read the label
    }

    public bool Checked
    {
        get => NativeMethods.wxsharp_checkbox_get(Handle);
        set => NativeMethods.wxsharp_checkbox_set(Handle, value);
    }

    private protected override void OnEvent(EventKind evt)
    {
        if (evt == EventKind.Toggle)
            Toggled?.Invoke();
    }
}
