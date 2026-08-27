using System;
using System.Collections.Generic;

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

    // ---- Window state -----------------------------------------------------------------------------------

    /// <summary>Minimises the frame to the taskbar, or restores it from there. Passing false is how an
    /// application brings itself back after a second copy is started and hands its work over.</summary>
    public void Iconize(bool iconize = true)
    {
        OwnerApp.VerifyAccess();
        NativeMethods.wxsharp_frame_iconize(Handle, iconize);
    }

    /// <summary>Whether the frame is minimised.</summary>
    public bool IsIconized => NativeMethods.wxsharp_frame_is_iconized(Handle);

    /// <summary>Maximises the frame, or restores it to its previous size.</summary>
    public void Maximize(bool maximize = true)
    {
        OwnerApp.VerifyAccess();
        NativeMethods.wxsharp_frame_maximize(Handle, maximize);
    }

    /// <summary>Whether the frame is maximised.</summary>
    public bool IsMaximized => NativeMethods.wxsharp_frame_is_maximized(Handle);

    /// <summary>Whether the platform always shows this frame maximised, as some do.</summary>
    public bool IsAlwaysMaximized => NativeMethods.wxsharp_frame_is_always_maximized(Handle);

    /// <summary>Returns the frame to its normal size from minimised or maximised.</summary>
    public void Restore()
    {
        OwnerApp.VerifyAccess();
        NativeMethods.wxsharp_frame_restore(Handle);
    }

    /// <summary>Whether this frame or one of its children has the keyboard focus.</summary>
    public bool IsActive => NativeMethods.wxsharp_frame_is_active(Handle);

    /// <summary>Shows or leaves full screen. <paramref name="style"/> chooses which chrome is hidden.</summary>
    public bool ShowFullScreen(bool show, FullScreenStyle style = FullScreenStyle.All)
    {
        OwnerApp.VerifyAccess();
        return NativeMethods.wxsharp_frame_show_full_screen(Handle, show, (int)style);
    }

    /// <summary>Whether the frame is currently full screen.</summary>
    public bool IsFullScreen => NativeMethods.wxsharp_frame_is_full_screen(Handle);

    /// <summary>Allows or forbids the platform full-screen affordance, where there is one. False where the
    /// platform has none, which is the case on Windows.</summary>
    public bool EnableFullScreenView(bool enable = true, FullScreenStyle style = FullScreenStyle.All)
    {
        OwnerApp.VerifyAccess();
        return NativeMethods.wxsharp_frame_enable_full_screen_view(Handle, enable, (int)style);
    }

    /// <summary>Shows the frame without taking focus from whatever the user is doing.</summary>
    public void ShowWithoutActivating()
    {
        OwnerApp.VerifyAccess();
        NativeMethods.wxsharp_frame_show_without_activating(Handle);
    }

    /// <summary>Flashes the taskbar button to say something happened here, without stealing focus. The
    /// courteous way to interrupt: a screen reader user who is reading elsewhere keeps their place, and the
    /// window is still marked as wanting attention when they come back.</summary>
    public void RequestUserAttention(UserAttention attention = UserAttention.Info)
    {
        OwnerApp.VerifyAccess();
        NativeMethods.wxsharp_frame_request_user_attention(Handle, (int)attention);
    }

    /// <summary>Enables or disables the title bar close button. False where the platform does not allow
    /// it.</summary>
    public bool EnableCloseButton(bool enable = true)
        => NativeMethods.wxsharp_frame_enable_close_button(Handle, enable);

    /// <summary>Enables or disables the title bar maximise button.</summary>
    public bool EnableMaximizeButton(bool enable = true)
        => NativeMethods.wxsharp_frame_enable_maximize_button(Handle, enable);

    /// <summary>Enables or disables the title bar minimise button.</summary>
    public bool EnableMinimizeButton(bool enable = true)
        => NativeMethods.wxsharp_frame_enable_minimize_button(Handle, enable);

    /// <summary>Centres the frame on the screen rather than on its parent.</summary>
    public void CentreOnScreen(Orientation? direction = null)
    {
        OwnerApp.VerifyAccess();
        // wxBOTH is wxHORIZONTAL | wxVERTICAL; no direction means both.
        var flags = direction switch
        {
            Orientation.Horizontal => 0x0004,
            Orientation.Vertical => 0x0008,
            _ => 0x0004 | 0x0008,
        };
        NativeMethods.wxsharp_frame_centre_on_screen(Handle, flags);
    }

    /// <summary>Whether the platform is asked to keep this window out of screen captures. Not every
    /// platform can, and <see cref="ContentProtection"/> reports what it actually did.</summary>
    public ContentProtection ContentProtection
    {
        get => (ContentProtection)NativeMethods.wxsharp_frame_get_content_protection(Handle);
        set => NativeMethods.wxsharp_frame_set_content_protection(Handle, (int)value);
    }

    /// <summary>Ties the frame to a file, which macOS shows in the title bar. Does nothing
    /// elsewhere.</summary>
    public void SetRepresentedFilename(string path)
        => NativeMethods.wxsharp_frame_set_represented_filename(Handle, path ?? string.Empty);

    /// <summary>How modal this frame is with respect to the rest of the application.</summary>
    public WindowModality Modality
    {
        set { OwnerApp.VerifyAccess(); NativeMethods.wxsharp_frame_set_window_modality(Handle, (int)value); }
    }

    /// <summary>The size a frame gets when none is given.</summary>
    public static Size DefaultSize
    {
        get
        {
            _ = App.RequireCurrent();
            NativeMethods.wxsharp_frame_get_default_size(out var width, out var height);
            return new Size(width, height);
        }
    }

    /// <summary>The button Enter activates, where one has been set.</summary>
    public Window? DefaultItem
    {
        get => App.Lookup(NativeMethods.wxsharp_frame_get_default_item(Handle));
        set => NativeMethods.wxsharp_frame_set_default_item(Handle, value?.Handle ?? 0);
    }

    // ---- Icons ------------------------------------------------------------------------------------------

    /// <summary>The frame icon, or null when it has none. The caller owns what comes back.</summary>
    public Icon? GetIcon()
    {
        var handle = NativeMethods.wxsharp_frame_get_icon(Handle);
        return handle == 0 ? null : Icon.Attach(handle);
    }

    /// <summary>Gives the frame several sizes of the same icon so the platform can pick the one it wants -
    /// the title bar, the taskbar and Alt-Tab all use different sizes, and letting the platform choose
    /// avoids a blurry scaled one.</summary>
    public unsafe void SetIcons(params Icon[] icons)
    {
        ArgumentNullException.ThrowIfNull(icons);
        OwnerApp.VerifyAccess();
        var handles = new nint[icons.Length == 0 ? 1 : icons.Length];
        for (var i = 0; i < icons.Length; ++i) handles[i] = icons[i]?.Handle ?? 0;
        fixed (nint* p = handles) NativeMethods.wxsharp_frame_set_icons(Handle, p, icons.Length);
    }

    /// <summary>Every icon the frame holds. The caller owns what comes back.</summary>
    public Icon[] GetIcons()
    {
        var count = NativeMethods.wxsharp_frame_get_icons(Handle);
        if (count <= 0) return Array.Empty<Icon>();
        var icons = new List<Icon>(count);
        for (var i = 0; i < count; ++i)
        {
            var handle = NativeMethods.wxsharp_frame_get_icon_at(i);
            if (handle != 0) icons.Add(Icon.Attach(handle));
        }
        return icons.ToArray();
    }

    // ---- The frame-owned bars ---------------------------------------------------------------------------

    /// <summary>The menu bar installed with <see cref="SetMenuBar"/>, or null when there is none.</summary>
    public MenuBar? GetMenuBar()
    {
        var handle = NativeMethods.wxsharp_frame_get_menubar(Handle);
        return handle == 0 ? null : MenuBar.Attach(handle);
    }

    /// <summary>Finds a menu item anywhere in the menu bar by its command ID, or null.</summary>
    public MenuItem? FindItemInMenuBar(int id)
    {
        var handle = NativeMethods.wxsharp_frame_find_item_in_menubar(Handle, id);
        return handle == 0 ? null : new MenuItem(handle);
    }

    /// <summary>Creates the frame status bar and returns it. The frame owns and positions it.</summary>
    public StatusBar CreateStatusBar(int fields = 1, StatusBarStyle style = StatusBarStyle.Default,
        int id = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fields);
        OwnerApp.VerifyAccess();
        return new StatusBar(this, 0, fields, (int)style, id);
    }

    /// <summary>The frame status bar, or null when it has none.</summary>
    public StatusBar? StatusBar
    {
        get
        {
            var handle = NativeMethods.wxsharp_frame_get_statusbar(Handle);
            if (handle == 0) return null;
            if (_statusBar is not null) return _statusBar;
            return new StatusBar(this, handle, 1, 0, WindowId.Any);
        }
        set
        {
            OwnerApp.VerifyAccess();
            NativeMethods.wxsharp_frame_set_statusbar(Handle, value?.Handle ?? 0);
            _statusBar = value;
        }
    }

    /// <summary>Sets the text of one status bar field. Screen readers do not announce a status bar change
    /// on their own, so anything the user must not miss belongs somewhere they are told about as
    /// well.</summary>
    public void SetStatusText(string text, int field = 0)
        => NativeMethods.wxsharp_frame_set_status_text(Handle, text ?? string.Empty, field);

    /// <summary>Sets a field's text, remembering what was there so <see cref="PopStatusText"/> can put it
    /// back. This is how a temporary message, such as a menu item help string, is shown without losing what
    /// the field said before.</summary>
    public void PushStatusText(string text, int field = 0)
        => NativeMethods.wxsharp_frame_push_status_text(Handle, text ?? string.Empty, field);

    /// <summary>Restores what a field said before the matching <see cref="PushStatusText"/>.</summary>
    public void PopStatusText(int field = 0) => NativeMethods.wxsharp_frame_pop_status_text(Handle, field);

    /// <summary>Sets each field's width. A negative width is a growable share of the leftover space, so
    /// <c>[-1, 100]</c> gives a fixed 100-pixel second field and the rest to the first.</summary>
    public unsafe void SetStatusWidths(params int[] widths)
    {
        ArgumentNullException.ThrowIfNull(widths);
        if (widths.Length == 0) return;
        OwnerApp.VerifyAccess();
        fixed (int* p = widths) NativeMethods.wxsharp_frame_set_status_widths(Handle, p, widths.Length);
    }

    /// <summary>Which field shows menu and toolbar help. -1 turns that off.</summary>
    public int StatusBarPane
    {
        get => NativeMethods.wxsharp_frame_get_status_bar_pane(Handle);
        set => NativeMethods.wxsharp_frame_set_status_bar_pane(Handle, value);
    }

    /// <summary>Creates the frame toolbar and returns it. The frame owns and positions it.</summary>
    public ToolBar CreateToolBar(ToolBarStyle style = ToolBarStyle.Default, int id = WindowId.Any)
    {
        OwnerApp.VerifyAccess();
        return new ToolBar(this, 0, (int)style, id);
    }

    /// <summary>The frame toolbar, or null when it has none.</summary>
    public ToolBar? ToolBar
    {
        get
        {
            var handle = NativeMethods.wxsharp_frame_get_toolbar(Handle);
            if (handle == 0) return null;
            if (_toolBar is not null) return _toolBar;
            return new ToolBar(this, handle, 0, WindowId.Any);
        }
        set
        {
            OwnerApp.VerifyAccess();
            NativeMethods.wxsharp_frame_set_toolbar(Handle, value?.Handle ?? 0);
            _toolBar = value;
        }
    }

    private StatusBar? _statusBar;
    private ToolBar? _toolBar;

    internal void AdoptStatusBar(StatusBar bar) => _statusBar = bar;
    internal void AdoptToolBar(ToolBar bar) => _toolBar = bar;

    /// <summary>Whether frames created from now on use the platform status bar rather than one wxWidgets
    /// draws itself. The native one is the accessible choice - a screen reader knows what it is, and a
    /// drawn one is just pixels. Windows only; ignored elsewhere.</summary>
    public static bool UseNativeStatusBar
    {
        get { _ = App.RequireCurrent(); return NativeMethods.wxsharp_frame_uses_native_statusbar(); }
        set { _ = App.RequireCurrent(); NativeMethods.wxsharp_frame_use_native_statusbar(value); }
    }

    // ---- Geometry persistence ---------------------------------------------------------------------------

    /// <summary>The frame position, size and state as a string to write to a settings file, or null when
    /// the platform could not report it. The contents are wxWidgets' own and vary by platform, so treat it
    /// as opaque and hand it back to <see cref="RestoreToGeometry"/> unchanged.</summary>
    public unsafe string? SaveGeometry()
    {
        var length = NativeMethods.wxsharp_frame_save_geometry(Handle, null, 0);
        if (length < 0) return null;
        if (length == 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_frame_save_geometry(Handle, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>Puts the frame back where <see cref="SaveGeometry"/> found it. Values the string does not
    /// carry are simply left alone, so a string saved by an older version still works.</summary>
    public bool RestoreToGeometry(string geometry)
    {
        OwnerApp.VerifyAccess();
        return NativeMethods.wxsharp_frame_restore_to_geometry(Handle, geometry ?? string.Empty);
    }
}

/// <summary>Which chrome <see cref="Frame.ShowFullScreen"/> hides, following the <c>wxFULLSCREEN_*</c>
/// flags.</summary>
[Flags]
public enum FullScreenStyle
{
    None = 0,
    NoMenuBar = 0x0001,
    NoToolBar = 0x0002,
    NoStatusBar = 0x0004,
    NoBorder = 0x0008,
    NoCaption = 0x0010,
    All = NoMenuBar | NoToolBar | NoStatusBar | NoBorder | NoCaption,
}

/// <summary>How insistently <see cref="Frame.RequestUserAttention"/> asks, following the
/// <c>wxUSER_ATTENTION_*</c> flags.</summary>
public enum UserAttention
{
    /// <summary>A brief hint.</summary>
    Info = 1,

    /// <summary>Keeps signalling until the user looks.</summary>
    Error = 2,
}

/// <summary>Whether the platform is asked to keep a window out of screen captures, following
/// <c>wxContentProtection</c>.</summary>
public enum ContentProtection
{
    None = 0,
    Enabled = 1,
}

/// <summary>How modal a window is, following <c>wxWindowMode</c>.</summary>
public enum WindowModality
{
    /// <summary>Not modal.</summary>
    Normal = 0,

    /// <summary>Modal with respect to its parent only.</summary>
    WindowModal = 1,

    /// <summary>Modal with respect to the whole application.</summary>
    AppModal = 2,
}
