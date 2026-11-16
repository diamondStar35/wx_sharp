using System;
using System.Collections.Generic;

namespace WxSharp;

/// <summary>A dialog-owned event handler, not a Window. Valid only during its native showing.</summary>
public abstract class FileDialogCustomControl : EvtHandler
{
    internal readonly DialogCustomization Session;
    internal readonly int Id;
    private protected FileDialogCustomControl(DialogCustomization session, int id) { Session = session; Id = id; }
    internal override App OwnerApp => Session.App;
    private protected override bool IsDead => !Session.IsActive || !Session.OwnerAlive
        || !Session.Controls.TryGetValue(Id, out var control) || !ReferenceEquals(control, this);
    private protected override void Verify()
    {
        OwnerApp.VerifyAccess();
        ObjectDisposedException.ThrowIf(IsDead, this);
    }
    private protected override bool BindNative(int eventId) => eventId == EventKind;
    private protected override void UnbindNative(int eventId) { }
    internal virtual int EventKind => 0;
    /// <summary>Shows or hides this control.</summary>
    public void Show(bool show = true) => SetNumber(3, show ? 1 : 0);
    /// <summary>Hides this control.</summary>
    public void Hide() => Show(false);
    /// <summary>Enables or disables this control.</summary>
    public void Enable(bool enable = true) => SetNumber(4, enable ? 1 : 0);
    /// <summary>Disables this control.</summary>
    public void Disable() => Enable(false);
    private protected unsafe int GetNumber(int property)
    {
        Verify();
        var value = 0;
        DialogCustomization.Check(NativeMethods.wxsharp_item_dialog_number(Session.Context, Id, property, false, &value));
        return value;
    }
    private protected unsafe void SetNumber(int property, int value)
    {
        Verify();
        DialogCustomization.Check(NativeMethods.wxsharp_item_dialog_number(Session.Context, Id, property, true, &value));
    }
    private protected void SetText(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Verify();
        DialogCustomization.Check(NativeMethods.wxsharp_item_dialog_set_text(Session.Context, Id, value));
    }
    internal void Invalidate() => ClearSubscriptions();
}

/// <summary>A native file dialog button.</summary>
public sealed class FileDialogButton : FileDialogCustomControl
{
    internal FileDialogButton(DialogCustomization session, int id) : base(session, id) { }
    internal override int EventKind => WxEvents.ButtonClicked.EventId;
    /// <summary>The user clicked the button.</summary>
    public event EventHandler<CommandEventArgs> Click { add => AddHandler(WxEvents.ButtonClicked, value); remove => RemoveHandler(WxEvents.ButtonClicked, value); }
}
/// <summary>A native file dialog checkbox.</summary>
public sealed class FileDialogCheckBox : FileDialogCustomControl
{
    internal FileDialogCheckBox(DialogCustomization session, int id) : base(session, id) { }
    internal override int EventKind => WxEvents.CheckBoxToggled.EventId;
    /// <summary>Whether the checkbox is checked. Setting this does not raise Toggled.</summary>
    public bool Checked { get => GetNumber(2) != 0; set => SetNumber(2, value ? 1 : 0); }
    /// <summary>The user changed the checkbox.</summary>
    public event EventHandler<CommandEventArgs> Toggled { add => AddHandler(WxEvents.CheckBoxToggled, value); remove => RemoveHandler(WxEvents.CheckBoxToggled, value); }
}
/// <summary>A radio button in a native file dialog.</summary>
public sealed class FileDialogRadioButton : FileDialogCustomControl
{
    internal FileDialogRadioButton(DialogCustomization session, int id) : base(session, id) { }
    internal override int EventKind => WxEvents.RadioButtonSelected.EventId;
    /// <summary>Whether selected. Set true to select; clear the group by selecting another button.</summary>
    public bool Value { get => GetNumber(2) != 0; set => SetNumber(2, value ? 1 : 0); }
    /// <summary>The user selected this radio button.</summary>
    public event EventHandler<CommandEventArgs> Selected { add => AddHandler(WxEvents.RadioButtonSelected, value); remove => RemoveHandler(WxEvents.RadioButtonSelected, value); }
}
/// <summary>A native read-only choice. Dynamic replacement is a Windows extension to Phoenix.</summary>
public sealed class FileDialogChoice : FileDialogCustomControl
{
    internal FileDialogChoice(DialogCustomization session, int id) : base(session, id) { }
    internal override int EventKind => WxEvents.ChoiceSelected.EventId;
    /// <summary>The selected index, or -1 if none is selected.</summary>
    public int SelectedIndex
    {
        get => GetNumber(1);
        set
        {
            if (value == -1) { Set(_items); return; }
            if (value < 0 || value >= _items.Length) throw new ArgumentOutOfRangeException(nameof(value));
            SetNumber(1, value);
        }
    }
    private string[] _items = Array.Empty<string>();
    /// <summary>Replaces all choices and clears the selection, without raising SelectionChanged.</summary>
    public unsafe void Set(IEnumerable<string> choices)
    {
        ArgumentNullException.ThrowIfNull(choices);
        Verify();
        var items = new List<string>(choices).ToArray();
        ItemsInterop.Invoke(Session.Context, items, Id, &SetItems);
        _items = items;
    }
    private static unsafe void SetItems(nint context, byte** items, int count, int id)
        => DialogCustomization.Check(NativeMethods.wxsharp_item_dialog_items(context, id, items, count));
    /// <summary>The user selected an item. Programmatic changes do not raise this event.</summary>
    public event EventHandler<CommandEventArgs> SelectionChanged { add => AddHandler(WxEvents.ChoiceSelected, value); remove => RemoveHandler(WxEvents.ChoiceSelected, value); }
}
/// <summary>An edit field in a native file dialog.</summary>
public sealed class FileDialogTextCtrl : FileDialogCustomControl
{
    internal FileDialogTextCtrl(DialogCustomization session, int id) : base(session, id) { }
    /// <summary>The current text. Read accepted text in TransferDataFromCustomControls.</summary>
    public string Value { get { Verify(); return Session.Read(2, Id); } set => SetText(value); }
}
/// <summary>Explanatory text in a native file dialog.</summary>
public sealed class FileDialogStaticText : FileDialogCustomControl
{
    internal FileDialogStaticText(DialogCustomization session, int id) : base(session, id) { }
    /// <summary>Replaces the displayed text.</summary>
    public void SetLabelText(string text) => SetText(text);
}
