using System;

namespace WxSharp;

/// <summary>A check box.</summary>
public class CheckBox : Control
{
    public event EventHandler<CommandEventArgs> Toggled
    {
        add => AddHandler(WxEvents.CheckBoxToggled, value);
        remove => RemoveHandler(WxEvents.CheckBoxToggled, value);
    }

    public CheckBox(Window parent, int id = WindowId.Any, string label = "", CheckBoxStyle style = CheckBoxStyle.TwoState,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_checkbox_create(parent.Handle, id, label, (int)style, Token));
        ApplyInitialGeometry(position, size);
    }

    public bool Checked
    {
        get => NativeMethods.wxsharp_checkbox_get(Handle);
        set => NativeMethods.wxsharp_checkbox_set(Handle, value);
    }
}
