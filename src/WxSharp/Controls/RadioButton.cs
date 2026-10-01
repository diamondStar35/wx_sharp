using System;

namespace WxSharp;

/// <summary>A radio button. Pass <c>groupStart: true</c> on the first button of a group; the buttons that
/// follow it (until the next group start) are mutually exclusive.</summary>
public class RadioButton : Control
{
    /// <summary>Wraps a RadioButton wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal RadioButton(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    /// <summary>The button was selected by the user, following <c>wxEVT_RADIOBUTTON</c>. Fires on the button
    /// that gains the selection, not on the one that loses it.</summary>
    public event EventHandler<CommandEventArgs> Selected
    {
        add => AddHandler(WxEvents.RadioButtonSelected, value);
        remove => RemoveHandler(WxEvents.RadioButtonSelected, value);
    }

    /// <summary>Creates a radio button labelled <paramref name="label"/>. Pass <paramref name="groupStart"/>
    /// true to begin a new mutually-exclusive group (<c>wxRB_GROUP</c>); otherwise the button joins the group
    /// of the preceding sibling.</summary>
    public RadioButton(Window parent, int id = WindowId.Any, string label = "", bool groupStart = false,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(GetType() == typeof(RadioButton)
            ? NativeMethods.wxsharp_radio_create(parent.Handle, id, label, groupStart, Token)
            : NativeMethods.wxsharp_custom_radio_create(parent.Handle, id, label, groupStart, Token));
        ApplyInitialGeometry(position, size);
    }

    /// <summary>Whether this button is the selected one in its group. Setting it true selects this button and
    /// clears the rest of the group.</summary>
    public bool Value
    {
        get => NativeMethods.wxsharp_radio_get(Handle);
        set => NativeMethods.wxsharp_radio_set(Handle, value);
    }
    /// <summary>The first button in this button's group, following <c>wxRadioButton::GetFirstInGroup</c>.</summary>
    public RadioButton? GetFirstInGroup() => App.Lookup(NativeMethods.wxsharp_radio_get_first(Handle)) as RadioButton;
    /// <summary>The last button in this button's group, following <c>wxRadioButton::GetLastInGroup</c>.</summary>
    public RadioButton? GetLastInGroup() => App.Lookup(NativeMethods.wxsharp_radio_get_last(Handle)) as RadioButton;
    /// <summary>The previous button in the group, or null if this is the first, following
    /// <c>wxRadioButton::GetPreviousInGroup</c>.</summary>
    public RadioButton? GetPreviousInGroup() => App.Lookup(NativeMethods.wxsharp_radio_get_previous(Handle)) as RadioButton;
    /// <summary>The next button in the group, or null if this is the last, following
    /// <c>wxRadioButton::GetNextInGroup</c>.</summary>
    public RadioButton? GetNextInGroup() => App.Lookup(NativeMethods.wxsharp_radio_get_next(Handle)) as RadioButton;
}
