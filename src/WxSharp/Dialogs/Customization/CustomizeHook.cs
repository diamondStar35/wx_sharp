namespace WxSharp;

/// <summary>Creates and updates native dialog controls, following wxFileDialogCustomizeHook.</summary>
/// <remarks>Read accepted values in TransferDataFromCustomControls. Controls belong to one showing
/// and cannot be used after it returns. The dialog retains its hook until it is replaced.</remarks>
public abstract class FileDialogCustomizeHook
{
    internal DialogCustomization? Attachment;
    /// <summary>Adds controls before the native dialog is shown.</summary>
    public abstract void AddCustomControls(FileDialogCustomize customizer);
    /// <summary>Updates controls after the current folder, selection, or file filter changes.</summary>
    public virtual void UpdateCustomControls() { }
    /// <summary>Copies accepted values while controls still exist. Not called on cancellation.</summary>
    public virtual void TransferDataFromCustomControls() { }
}
