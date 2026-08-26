using System;

namespace WxSharp;

/// <summary>A native modal or modeless wxDialog.</summary>
public class Dialog : Window
{
    public event EventHandler<CloseEventArgs>? Closing;

    public Dialog(Window? parent = null, int id = WindowId.Any, string title = "",
        Point? position = null, Size? size = null) : base(parent, id)
    {
        var p = position ?? new Point(-1, -1);
        var s = size ?? new Size(-1, -1);
        Initialize(NativeMethods.wxsharp_dialog_create(parent?.Handle ?? 0, id, title, p.X, p.Y, s.Width, s.Height, Token));
    }

    public unsafe string Title
    {
        get { var n = NativeMethods.wxsharp_dialog_get_title(Handle, null, 0); if (n <= 0) return string.Empty; var b = new byte[n + 1]; fixed (byte* p = b) _ = NativeMethods.wxsharp_dialog_get_title(Handle, p, n + 1); return Utf8String.Decode(b, n); }
        set { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_dialog_set_title(Handle, value); }
    }
    public void SetEscapeId(DialogResult result) { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_dialog_set_escape_id(Handle, (int)result); }
    public void SetAffirmativeId(DialogResult result) { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_dialog_set_affirmative_id(Handle, (int)result); }
    public DialogResult ShowModal() { OwnerApp.VerifyAccess(); return (DialogResult)NativeMethods.wxsharp_dialog_show_modal(Handle); }
    public void EndModal(DialogResult result) { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_dialog_end_modal(Handle, (int)result); }

    internal override uint Dispatch(in NativeEvent e)
    {
        if (e.Kind != EventKind.Close) return base.Dispatch(e);
        var close = new CloseEventArgs(this, e.Id, e.CanVeto != 0); Closing?.Invoke(this, close);
        return (close.Handled ? 1u : 0u) | (close.Cancel ? 2u : 0u);
    }
}
