using System;

namespace WxSharp;

/// <summary>A native top-level wxFrame.</summary>
public class Frame : Window
{
    public event EventHandler<CloseEventArgs> Closing
    {
        add => AddHandler(WxEvents.Closing, value);
        remove => RemoveHandler(WxEvents.Closing, value);
    }

    public event EventHandler<ShowEventArgs> Shown
    {
        add => AddHandler(WxEvents.Shown, value);
        remove => RemoveHandler(WxEvents.Shown, value);
    }

    /// <summary>Raised when the frame becomes, or stops being, the active window. Check
    /// <see cref="ActivateEventArgs.Active"/> for which.</summary>
    public event EventHandler<ActivateEventArgs> Activated
    {
        add => AddHandler(WxEvents.Activated, value);
        remove => RemoveHandler(WxEvents.Activated, value);
    }

    public event EventHandler<WxEventArgs> Maximized
    {
        add => AddHandler(WxEvents.Maximized, value);
        remove => RemoveHandler(WxEvents.Maximized, value);
    }

    public event EventHandler<ActivateEventArgs> Iconized
    {
        add => AddHandler(WxEvents.Iconized, value);
        remove => RemoveHandler(WxEvents.Iconized, value);
    }

    /// <summary>A menu item or accelerator was chosen. Filter by command ID with
    /// <see cref="Window.Bind{T}"/> when one handler should not see every command.</summary>
    public event EventHandler<CommandEventArgs> MenuCommand
    {
        add => AddHandler(WxEvents.MenuCommand, value);
        remove => RemoveHandler(WxEvents.MenuCommand, value);
    }

    /// <summary>A menu is about to open. The moment to rebuild anything dynamic in it - a recent-files
    /// list, say - because it happens before the user sees the menu.</summary>
    public event EventHandler<MenuEventArgs> MenuOpened
    {
        add => AddHandler(WxEvents.MenuOpened, value);
        remove => RemoveHandler(WxEvents.MenuOpened, value);
    }

    public event EventHandler<MenuEventArgs> MenuClosed
    {
        add => AddHandler(WxEvents.MenuClosed, value);
        remove => RemoveHandler(WxEvents.MenuClosed, value);
    }

    /// <summary>An item is highlighted as the user moves through a menu. wxWidgets already puts the item's
    /// help string in the status bar; handle this to do something else with it.</summary>
    public event EventHandler<MenuEventArgs> MenuHighlighted
    {
        add => AddHandler(WxEvents.MenuHighlighted, value);
        remove => RemoveHandler(WxEvents.MenuHighlighted, value);
    }

    public Frame(Window? parent = null, int id = WindowId.Any, string title = "",
        Point? position = null, Size? size = null, FrameStyle style = FrameStyle.Default) : base(parent, id)
    {
        var p = position ?? new Point(-1, -1);
        var s = size ?? new Size(-1, -1);
        Initialize(NativeMethods.wxsharp_window_create(parent?.Handle ?? 0, id, title, p.X, p.Y, s.Width, s.Height,
            (int)style, Token));
        if (App.Current!.TopWindow is null) App.Current.TopWindow = this;
    }

    public unsafe string Title
    {
        get { var n = NativeMethods.wxsharp_window_get_title(Handle, null, 0); if (n <= 0) return string.Empty; var b = new byte[n + 1]; fixed (byte* p = b) _ = NativeMethods.wxsharp_window_get_title(Handle, p, n + 1); return Utf8String.Decode(b, n); }
        set { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_window_set_title(Handle, value); }
    }
    public nint NativeHandle { get { OwnerApp.VerifyAccess(); return NativeMethods.wxsharp_window_native_handle(Handle); } }
    public void SetFullScreen(bool fullScreen) { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_window_set_fullscreen(Handle, fullScreen); }

    /// <summary>Installs a menu bar. The frame takes ownership of <paramref name="menuBar"/>.</summary>
    public void SetMenuBar(MenuBar menuBar)
    {
        ArgumentNullException.ThrowIfNull(menuBar); OwnerApp.VerifyAccess();
        NativeMethods.wxsharp_frame_set_menubar(Handle, menuBar.TransferOwnership());
    }

    public void SetIcon(Icon icon) => NativeMethods.wxsharp_frame_set_icon(Handle,
        icon?.Handle ?? throw new ArgumentNullException(nameof(icon)));

    /// <summary>Sends update-UI events to every item in the menu bar. wxWidgets already does this whenever
    /// a menu is about to open, so this is only for refreshing without waiting. Follows
    /// <c>wxFrame.DoMenuUpdates</c>.</summary>
    public void DoMenuUpdates()
    {
        OwnerApp.VerifyAccess();
        NativeMethods.wxsharp_frame_update_menus(Handle);
    }
}
