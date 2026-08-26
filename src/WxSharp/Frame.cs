using System;

namespace WxSharp;

/// <summary>A native top-level wxFrame.</summary>
public class Frame : Window
{
    public event EventHandler<CloseEventArgs>? Closing;
    public event EventHandler<WxEventArgs>? Shown;
    public event EventHandler<ActivateEventArgs>? Activated;
    public event EventHandler<ActivateEventArgs>? Deactivated;
    public event EventHandler<SizeEventArgs>? Resized;
    public event EventHandler<MoveEventArgs>? Moved;
    public event EventHandler<WxEventArgs>? Maximized;
    public event EventHandler<CommandEventArgs>? MenuCommand;

    public Frame(Window? parent = null, int id = WindowId.Any, string title = "",
        Point? position = null, Size? size = null) : base(parent, id)
    {
        var p = position ?? new Point(-1, -1);
        var s = size ?? new Size(-1, -1);
        Initialize(NativeMethods.wxsharp_window_create(parent?.Handle ?? 0, id, title, p.X, p.Y, s.Width, s.Height, Token));
        if (App.Current!.TopWindow is null) App.Current.TopWindow = this;
    }

    public unsafe string Title
    {
        get { var n = NativeMethods.wxsharp_window_get_title(Handle, null, 0); if (n <= 0) return string.Empty; var b = new byte[n + 1]; fixed (byte* p = b) _ = NativeMethods.wxsharp_window_get_title(Handle, p, n + 1); return Utf8String.Decode(b, n); }
        set { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_window_set_title(Handle, value); }
    }
    public nint NativeHandle { get { OwnerApp.VerifyAccess(); return NativeMethods.wxsharp_window_native_handle(Handle); } }
    public void Center() { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_window_center(Handle); }
    public void SetFullScreen(bool fullScreen) { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_window_set_fullscreen(Handle, fullScreen); }
    public void Close() { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_window_close(Handle); }
    public void SetMenuBar(MenuBar menuBar)
    {
        ArgumentNullException.ThrowIfNull(menuBar); OwnerApp.VerifyAccess();
        NativeMethods.wxsharp_frame_set_menubar(Handle, menuBar.TransferOwnership());
    }
    public void SetIcon(Icon icon) => NativeMethods.wxsharp_frame_set_icon(Handle,
        icon?.Handle ?? throw new ArgumentNullException(nameof(icon)));
    public unsafe void SetAccelerators(params Accelerator[] accelerators)
    {
        ArgumentNullException.ThrowIfNull(accelerators);
        var native = new NativeAccelerator[accelerators.Length];
        for (var i = 0; i < native.Length; ++i)
            native[i] = new NativeAccelerator
            {
                Modifiers = (int)accelerators[i].Modifiers,
                KeyCode = accelerators[i].KeyCode,
                CommandId = accelerators[i].CommandId
            };
        fixed (NativeAccelerator* entries = native) NativeMethods.wxsharp_frame_set_accelerators(Handle, entries, native.Length);
    }

    internal override uint Dispatch(in NativeEvent e)
    {
        switch (e.Kind)
        {
            case EventKind.Close:
                var close = new CloseEventArgs(this, e.Id, e.CanVeto != 0); Closing?.Invoke(this, close);
                return (close.Handled ? 1u : 0u) | (close.Cancel ? 2u : 0u);
            case EventKind.Shown: return Raise(new WxEventArgs(this, e.Id), Shown);
            case EventKind.Activate: return Raise(new ActivateEventArgs(this, e), Activated);
            case EventKind.Deactivate: return Raise(new ActivateEventArgs(this, e), Deactivated);
            case EventKind.Resize: return Raise(new SizeEventArgs(this, e), Resized);
            case EventKind.Move: return Raise(new MoveEventArgs(this, e), Moved);
            case EventKind.Maximize: return Raise(new WxEventArgs(this, e.Id), Maximized);
            case EventKind.Menu: return RaiseCommand(new CommandEventArgs(this, e.Id), MenuCommand);
            default: return base.Dispatch(e);
        }
    }
}
