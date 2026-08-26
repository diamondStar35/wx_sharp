using System;

namespace WxSharp;

/// <summary>A native modal or modeless wxDialog.</summary>
public class Dialog : Window
{
    public event EventHandler<CloseEventArgs> Closing
    {
        add => AddHandler(WxEvents.Closing, value);
        remove => RemoveHandler(WxEvents.Closing, value);
    }

    /// <summary>A menu item or accelerator was chosen. Dialogs get accelerators of their own through
    /// <see cref="Window.SetAcceleratorTable"/>, so this fires for those too.</summary>
    public event EventHandler<CommandEventArgs> MenuCommand
    {
        add => AddHandler(WxEvents.MenuCommand, value);
        remove => RemoveHandler(WxEvents.MenuCommand, value);
    }

    public Dialog(Window? parent = null, int id = WindowId.Any, string title = "",
        Point? position = null, Size? size = null, DialogStyle style = DialogStyle.Default) : base(parent, id)
    {
        var p = position ?? new Point(-1, -1);
        var s = size ?? new Size(-1, -1);
        Initialize(NativeMethods.wxsharp_dialog_create(parent?.Handle ?? 0, id, title, p.X, p.Y, s.Width, s.Height,
            (int)style, Token));
    }

    /// <summary>Builds the platform's standard button row. wxWidgets decides the order, the spacing and which
    /// button is the default, which is also the order a screen reader reads them in - so prefer this over
    /// laying out OK and Cancel by hand. Add the result to the dialog's sizer. Returns null when the platform
    /// declines to build one.</summary>
    public Sizer? CreateButtonSizer(ButtonSizerFlags buttons)
    {
        OwnerApp.VerifyAccess();
        var handle = NativeMethods.wxsharp_dialog_create_button_sizer(Handle, (int)buttons);
        return handle == 0 ? null : Sizer.Attach(handle);
    }

    public unsafe string Title
    {
        get { var n = NativeMethods.wxsharp_dialog_get_title(Handle, null, 0); if (n <= 0) return string.Empty; var b = new byte[n + 1]; fixed (byte* p = b) _ = NativeMethods.wxsharp_dialog_get_title(Handle, p, n + 1); return Utf8String.Decode(b, n); }
        set { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_dialog_set_title(Handle, value); }
    }
    /// <summary>The command ID Escape ends the dialog with. Pass <see cref="StandardId.None"/> to make
    /// Escape do nothing.</summary>
    public void SetEscapeId(int id) { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_dialog_set_escape_id(Handle, id); }

    /// <summary>The command ID that counts as the dialog being accepted - <see cref="StandardId.Ok"/> by
    /// default.</summary>
    public void SetAffirmativeId(int id) { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_dialog_set_affirmative_id(Handle, id); }

    /// <summary>Shows the dialog and returns the command ID it ended with - whatever was passed to
    /// <see cref="EndModal"/>, or the ID of the button pressed. Compare against <see cref="StandardId"/>.</summary>
    public int ShowModal() { OwnerApp.VerifyAccess(); return NativeMethods.wxsharp_dialog_show_modal(Handle); }

    /// <summary>Closes a modal dialog, and makes <see cref="ShowModal"/> return <paramref name="result"/>.</summary>
    public void EndModal(int result) { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_dialog_end_modal(Handle, result); }
}
