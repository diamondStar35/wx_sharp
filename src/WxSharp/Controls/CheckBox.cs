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
        Initialize(GetType() == typeof(CheckBox)
            ? NativeMethods.wxsharp_checkbox_create(parent.Handle, id, label, (int)style, Token)
            : NativeMethods.wxsharp_custom_checkbox_create(parent.Handle, id, label, (int)style, Token));
        ApplyInitialGeometry(position, size);
    }

    /// <summary>The check state as a boolean. On a three-state box the indeterminate state reads as false;
    /// use <see cref="State"/> there instead.</summary>
    public bool Checked
    {
        get => NativeMethods.wxsharp_checkbox_get(Handle);
        set => NativeMethods.wxsharp_checkbox_set(Handle, value);
    }

    /// <summary>The full state, including the indeterminate one. Setting
    /// <see cref="CheckBoxState.Undetermined"/> on a box that was not created with
    /// <see cref="CheckBoxStyle.ThreeState"/> does nothing.</summary>
    public CheckBoxState State
    {
        get => (CheckBoxState)NativeMethods.wxsharp_checkbox_get_3state(Handle);
        set => NativeMethods.wxsharp_checkbox_set_3state(Handle, (int)value);
    }

    /// <summary>Whether this box was created with <see cref="CheckBoxStyle.ThreeState"/>.</summary>
    public bool IsThreeState => NativeMethods.wxsharp_checkbox_is_3state(Handle);

    /// <summary>Whether the user can reach the indeterminate state, as opposed to only code.</summary>
    public bool IsThirdStateAllowedForUser => NativeMethods.wxsharp_checkbox_is_3rd_state_allowed_for_user(Handle);
    public CheckBoxState Get3StateValue() => State;
    public void Set3StateValue(CheckBoxState state) => State = state;
    public bool Is3State() => IsThreeState;
    public bool Is3rdStateAllowedForUser() => IsThirdStateAllowedForUser;
    public void SetTransparentPartColour(Colour colour)
        => NativeMethods.wxsharp_checkbox_set_transparent_part_colour(Handle, colour.ToArgb());
}
