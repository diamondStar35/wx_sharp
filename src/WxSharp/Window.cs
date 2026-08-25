using System;

namespace WxSharp;

/// <summary>A native top-level window with a vertical content area; controls created against it stack in
/// order. Show it after laying out, and pump <see cref="Wx.Pump"/> each tick to keep it live.</summary>
public sealed class Window : Container
{
    /// <summary>Raised when the window's close box is pressed (the window is then destroyed).</summary>
    public event Action? Closed;
    /// <summary>Raised when the window becomes visible.</summary>
    public event Action? Shown;
    /// <summary>Raised when the window gains focus.</summary>
    public event Action? Activated;
    /// <summary>Raised when the window loses focus.</summary>
    public event Action? Deactivated;
    /// <summary>Raised when the window is resized.</summary>
    public event Action? Resized;
    /// <summary>Raised when the window is moved.</summary>
    public event Action? Moved;
    /// <summary>Raised when the window is maximized.</summary>
    public event Action? Maximized;

    /// <summary>Creates a window. Pass <paramref name="panel"/> false for a bare frame with no content
    /// wxPanel - a screen reader announces that panel as a grouping, so windows that just host an occasional
    /// control (or none) should omit it.</summary>
    public Window(string title, int width = 480, int height = 360, bool panel = true)
    {
        var handle = NativeMethods.wxsharp_window_create(title, width, height, Id, panel);
        AttachContainer(handle, NativeMethods.wxsharp_window_panel(handle));
    }

    public string Title { set => NativeMethods.wxsharp_window_set_title(Handle, value); }

    /// <summary>Re-flows the content after controls have been added.</summary>
    public void Layout() => NativeMethods.wxsharp_window_layout(Handle);

    public void Center() => NativeMethods.wxsharp_window_center(Handle);

    /// <summary>Toggles borderless full-screen - hides the title bar, borders and any menu bar. Useful for
    /// the game's playfield and anywhere you want no window chrome.</summary>
    public void SetFullScreen(bool fullScreen) => NativeMethods.wxsharp_window_set_fullscreen(Handle, fullScreen);

    /// <summary>The size of the window's client area (inside any chrome), in pixels.</summary>
    public Size ClientSize
    {
        get { NativeMethods.wxsharp_control_get_client_size(Handle, out var w, out var h); return new Size(w, h); }
    }

    /// <summary>The OS-level window handle (HWND on Windows), for native interop.</summary>
    public nint NativeHandle => NativeMethods.wxsharp_window_native_handle(Handle);

    public void Show(bool show = true) => NativeMethods.wxsharp_window_show(Handle, show);

    public void Close() => NativeMethods.wxsharp_window_close(Handle);

    public void Destroy()
    {
        NativeMethods.wxsharp_window_destroy(Handle);
        Cleanup();
    }

    internal override void OnNativeEvent(EventKind evt)
    {
        switch (evt)
        {
            case EventKind.Shown: Shown?.Invoke(); break;
            case EventKind.Activate: Activated?.Invoke(); break;
            case EventKind.Deactivate: Deactivated?.Invoke(); break;
            case EventKind.Resize: Resized?.Invoke(); break;
            case EventKind.Move: Moved?.Invoke(); break;
            case EventKind.Maximize: Maximized?.Invoke(); break;
            case EventKind.Close:
                Closed?.Invoke();
                Cleanup(); // the native frame is destroyed on close; release the registry ids too
                break;
        }
    }
}
