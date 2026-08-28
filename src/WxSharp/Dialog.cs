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
        Point? position = null, Size? size = null, DialogStyle style = DialogStyle.Default)
        : this(parent, id, title, position, size, style, deferNativeCreation: false)
    {
    }

    /// <summary>For a dialog wxWidgets builds itself - the file, folder, text, number, colour and font
    /// pickers. Each of those is a real <c>wxDialog</c> with its own constructor, so the derived class makes
    /// the native window and calls <see cref="Window.Initialize"/> rather than this doing it.</summary>
    private protected Dialog(Window? parent, int id, string title, Point? position, Size? size,
        DialogStyle style, bool deferNativeCreation) : base(parent, id)
    {
        if (deferNativeCreation) return;
        var p = position ?? new Point(-1, -1);
        var s = size ?? new Size(-1, -1);
        Initialize(GetType() == typeof(Dialog)
            ? NativeMethods.wxsharp_dialog_create(parent?.Handle ?? 0, id, title, p.X, p.Y, s.Width, s.Height,
                (int)style, Token)
            : NativeMethods.wxsharp_custom_dialog_create(parent?.Handle ?? 0, id, title, p.X, p.Y, s.Width,
                s.Height, (int)style, Token));
    }

    /// <summary>Reads a string out of a native dialog with the usual two-call protocol.</summary>
    private protected unsafe string ReadDialogString(ReadDialogText read)
    {
        Verify();
        var length = read(Handle, null, 0);
        if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1];
        fixed (byte* buffer = bytes) _ = read(Handle, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }

    private protected unsafe delegate int ReadDialogText(nint dialog, byte* buffer, int bufferLength);

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

    // ---- Overridable wxDialog virtuals ------------------------------------------------------------------

    /// <summary>Whether this dialog still existing should keep the application alive. Follows
    /// <c>wxTopLevelWindow.ShouldPreventAppExit</c>.</summary>
    public virtual bool ShouldPreventAppExit() => BaseBool(VirtualMember.ShouldPreventAppExit);

    /// <summary>The window a dialog's content and standard button row are added to. A dialog that wraps its
    /// contents in a panel returns that panel, so wxWidgets puts things in the right place. Follows
    /// <c>wxDialog.GetContentWindow</c>.</summary>
    public virtual Window? GetContentWindow()
    {
        var request = CallBase(VirtualMember.GetContentWindow);
        return App.Lookup((nint)request.Handle);
    }

    internal override bool TryAnswerVirtual(ref NativeVirtualRequest request)
    {
        switch ((VirtualMember)request.Which)
        {
            case VirtualMember.ShouldPreventAppExit:
                request.Result = ShouldPreventAppExit() ? 1 : 0;
                return true;
            case VirtualMember.GetContentWindow:
                request.Handle = GetContentWindow()?.NativeHandleForLookup ?? 0;
                return true;
            default:
                return base.TryAnswerVirtual(ref request);
        }
    }
}
