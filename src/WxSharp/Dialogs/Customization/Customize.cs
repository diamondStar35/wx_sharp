using System;
using System.Collections.Generic;

namespace WxSharp;

/// <summary>Creates native controls during FileDialogCustomizeHook.AddCustomControls.</summary>
public sealed class FileDialogCustomize
{
    private readonly DialogCustomization _session;
    internal FileDialogCustomize(DialogCustomization session) => _session = session;
    /// <summary>Adds a command button.</summary>
    public FileDialogButton AddButton(string label) => Add(new FileDialogButton(_session, Create(1, label)));
    /// <summary>Adds a checkbox, initially unchecked.</summary>
    public FileDialogCheckBox AddCheckBox(string label) => Add(new FileDialogCheckBox(_session, Create(2, label)));
    /// <summary>Adds a radio button. Consecutive radio buttons form one exclusive group.</summary>
    public FileDialogRadioButton AddRadioButton(string label) => Add(new FileDialogRadioButton(_session, Create(3, label)));
    /// <summary>Adds a read-only choice. The optional label is a Windows extension for accessible naming.</summary>
    public FileDialogChoice AddChoice(IEnumerable<string> choices, string label = "")
    {
        ArgumentNullException.ThrowIfNull(choices);
        var control = Add(new FileDialogChoice(_session, Create(4, label)));
        control.Set(choices);
        return control;
    }
    /// <summary>Adds a labelled edit field. Native file dialog edit fields do not report text-change events.</summary>
    public FileDialogTextCtrl AddTextCtrl(string label = "") => Add(new FileDialogTextCtrl(_session, Create(5, label)));
    /// <summary>Adds explanatory text.</summary>
    public FileDialogStaticText AddStaticText(string label) => Add(new FileDialogStaticText(_session, Create(6, label)));
    private int Create(int kind, string label)
    {
        ArgumentNullException.ThrowIfNull(label);
        _session.VerifyBuilding();
        return DialogCustomization.Check(NativeMethods.wxsharp_item_dialog_add(_session.Context, kind, label));
    }
    private T Add<T>(T control) where T : FileDialogCustomControl
    {
        _session.Controls.Add(control.Id, control);
        return control;
    }
}
