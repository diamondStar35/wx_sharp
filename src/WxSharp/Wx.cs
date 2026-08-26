using System;

namespace WxSharp;

/// <summary>Global wxWidgets services. Application lifetime is owned by <see cref="App"/>.</summary>
public static class Wx
{
    public static bool SupportsCustomAccessibility => NativeMethods.wxsharp_custom_accessibility_available();
    public static MessageBoxStyle MessageBox(string message, string caption, MessageBoxStyle style = MessageBoxStyle.Ok)
    {
        var app = App.Current ?? throw new InvalidOperationException("Create an App before showing UI.");
        app.VerifyAccess();
        return (MessageBoxStyle)NativeMethods.wxsharp_message_box(message, caption, (int)style);
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
