using System;

namespace WxSharp;

/// <summary>Global wxWidgets services. Application lifetime is owned by <see cref="App"/>.</summary>
public static partial class Wx
{
    public static bool SupportsCustomAccessibility => NativeMethods.wxsharp_custom_accessibility_available();
    /// <summary>Shows a native message box and returns the button pressed. Pass the window it belongs to as
    /// <paramref name="parent"/>: that is what makes it modal to the right window and tells assistive
    /// technology which window it came from.</summary>
    public static MessageBoxStyle MessageBox(string message, string caption,
        MessageBoxStyle style = MessageBoxStyle.Ok, Window? parent = null)
    {
        var app = App.Current ?? throw new InvalidOperationException("Create an App before showing UI.");
        app.VerifyAccess();
        return (MessageBoxStyle)NativeMethods.wxsharp_message_box(parent?.Handle ?? 0, message, caption, (int)style);
    }
    public static void CallAfter(Action action) => App.Queue(action);

    public static bool Yield(bool onlyIfNeeded = false)
    {
        var app = App.Current ?? throw new InvalidOperationException("Create an App before yielding.");
        app.VerifyAccess();
        return NativeMethods.wxsharp_yield(onlyIfNeeded);
    }
    public static IDisposable BusyCursor()
    {
        _ = App.RequireCurrent(); NativeMethods.wxsharp_begin_busy_cursor(); return new BusyCursorScope();
    }
    private sealed class BusyCursorScope : IDisposable
    {
        private bool _disposed;
        public void Dispose() { if (_disposed) return; _disposed = true; NativeMethods.wxsharp_end_busy_cursor(); }
    }
}
