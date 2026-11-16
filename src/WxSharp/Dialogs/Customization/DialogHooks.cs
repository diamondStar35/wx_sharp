using System;

namespace WxSharp;

public sealed unsafe partial class FileDialog
{
    private DialogCustomization? _customization;
    /// <summary>Attaches a Phoenix-style customization hook. Returns false on unsupported platforms.
    /// Pass null to detach. The Windows Common Item Dialog remains native.</summary>
    public bool SetCustomizeHook(FileDialogCustomizeHook? hook)
    {
        Verify();
        if (!OperatingSystem.IsWindows()) return false;
        hook?.Attachment?.ReleaseDeadOwner();
        if (_customization is not null && ReferenceEquals(_customization, hook?.Attachment)) return true;
        if (hook?.Attachment is not null) throw new InvalidOperationException("This hook is already attached to a dialog.");
        _customization?.Detach();
        _customization = hook is null ? null : new DialogCustomization(this, hook);
        return true;
    }
    /// <summary>The current native selection while the dialog is showing.</summary>
    public string CurrentlySelectedFilename { get { Verify(); return _customization?.CurrentPath ?? string.Empty; } }
    /// <summary>The native file filter index, counting from zero.</summary>
    public int CurrentlySelectedFilterIndex { get { Verify(); return _customization?.IsActive == true ? NativeMethods.wxsharp_item_dialog_filter(_customization.Context) : FilterIndex; } }
    /// <inheritdoc/>
    public override int ShowModal() { Verify(); return _customization is null ? base.ShowModal() : _customization.Show(false); }
}

public sealed unsafe partial class DirDialog
{
    private DialogCustomization? _customization;
    /// <summary>Windows extension: attaches the same customization hook as FileDialog. Returns false
    /// on other platforms. Pass null to detach.</summary>
    public bool SetCustomizeHook(FileDialogCustomizeHook? hook)
    {
        Verify();
        if (!OperatingSystem.IsWindows()) return false;
        hook?.Attachment?.ReleaseDeadOwner();
        if (_customization is not null && ReferenceEquals(_customization, hook?.Attachment)) return true;
        if (hook?.Attachment is not null) throw new InvalidOperationException("This hook is already attached to a dialog.");
        _customization?.Detach();
        _customization = hook is null ? null : new DialogCustomization(this, hook);
        return true;
    }
    /// <summary>The folder selected for acceptance, or the current folder if none is selected.
    /// Available while a customized native picker is showing.</summary>
    public string CurrentlySelectedPath { get { Verify(); return _customization?.CurrentPath ?? string.Empty; } }
    /// <inheritdoc/>
    public override int ShowModal() { Verify(); return _customization is null ? base.ShowModal() : _customization.Show(true); }
}
