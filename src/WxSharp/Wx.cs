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

    /// <summary>Queues a command event on <paramref name="target"/>, as <c>wx.PostEvent</c> does. The event
    /// is dispatched from the event loop rather than inside this call, so it is safe to raise from a
    /// handler that is still running, and it travels up the parent chain like any other command event.
    /// </summary>
    /// <param name="target">The window the event is sent to.</param>
    /// <param name="eventType">Which command event to raise, e.g. <see cref="WxEvents.MenuCommand"/>.</param>
    /// <param name="id">The command ID handlers filter on.</param>
    /// <param name="value">The event's integer payload, read as <c>CommandEventArgs.Value</c>.</param>
    /// <param name="text">The event's string payload.</param>
    /// <exception cref="ArgumentException">The event type is not a command event. Only those can be
    /// synthesised: the other classes carry state wxWidgets fills in from a real occurrence, such as a key
    /// event's scan code or a mouse event's position.</exception>
    public static void PostEvent(Window target, EventType<CommandEventArgs> eventType, int id,
        int value = 0, string text = "")
        => Send(target, eventType, id, value, text, processNow: false);

    /// <summary>Sends a command event to <paramref name="target"/> and runs its handlers before returning,
    /// as <c>wxEvtHandler.ProcessEvent</c> does. Returns whether a handler took it - that is, whether one
    /// ran and did not skip. Prefer <see cref="PostEvent"/> from inside a handler.</summary>
    public static bool ProcessEvent(Window target, EventType<CommandEventArgs> eventType, int id,
        int value = 0, string text = "")
        => Send(target, eventType, id, value, text, processNow: true);

    private static bool Send(Window target, EventType<CommandEventArgs> eventType, int id, int value,
        string text, bool processNow)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(eventType);
        var app = App.Current ?? throw new InvalidOperationException("Create an App before raising events.");
        app.VerifyAccess();
        var result = NativeMethods.wxsharp_post_command_event(target.Handle, eventType.EventId, id, value,
            text, processNow);
        if (result < 0)
            throw new ArgumentException("Only a command event can be raised this way.", nameof(eventType));
        return result != 0;
    }

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
