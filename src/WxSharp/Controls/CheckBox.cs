using System;

namespace WxSharp;

/// <summary>A check box.</summary>
public class CheckBox : Control
{
    public event EventHandler<CommandEventArgs>? Toggled;

    public CheckBox(Window parent, int id = WindowId.Any, string label = "", CheckBoxStyle style = CheckBoxStyle.TwoState,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_checkbox_create(parent.Handle, id, label, (int)style, Token));
        AccessibleName = label; // wx would otherwise announce "check"; make screen readers read the label
        ApplyInitialGeometry(position, size);
    }

    public bool Checked
    {
        get => NativeMethods.wxsharp_checkbox_get(Handle);
        set => NativeMethods.wxsharp_checkbox_set(Handle, value);
    }

    internal override uint Dispatch(in NativeEvent e)
    {
        if (e.Kind != EventKind.Toggle) return base.Dispatch(e);
        return RaiseCommand(new CommandEventArgs(this, e.Id), Toggled);
    }
}
