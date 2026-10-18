namespace WxSharp;

/// <summary>The standard icon displayed by a <see cref="NotificationMessage"/>.</summary>
public enum NotificationIcon
{
    /// <summary>An informational notification.</summary>
    Information = 0x00000800,
    /// <summary>A warning notification.</summary>
    Warning = 0x00000100,
    /// <summary>An error notification.</summary>
    Error = 0x00000200,
}

/// <summary>A non-intrusive desktop notification backed by <c>wxNotificationMessage</c>.</summary>
public sealed class NotificationMessage
{
    /// <summary>Lets the platform choose how long the notification remains visible.</summary>
    public const int TimeoutAuto = -1;

    /// <summary>Requests a notification that remains visible until the user dismisses it.</summary>
    public const int TimeoutNever = 0;

    /// <summary>Enables Windows toast notifications. Returns false when the platform or registration does
    /// not support them; callers should not call <see cref="Show"/> then because Windows would use a tray
    /// icon balloon instead.</summary>
    public static bool UseToasts(string shortcutPath, string appId)
    {
        ArgumentNullException.ThrowIfNull(shortcutPath);
        ArgumentNullException.ThrowIfNull(appId);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_notification_use_toasts(shortcutPath, appId);
    }

    /// <summary>Creates a notification. Call <see cref="Show"/> to display it.</summary>
    public NotificationMessage(
        string title,
        string message = "",
        Window? parent = null,
        NotificationIcon icon = NotificationIcon.Information)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(message);
        Title = title;
        Message = message;
        Parent = parent;
        Icon = icon;
    }

    /// <summary>The short heading shown by the operating system.</summary>
    public string Title { get; }

    /// <summary>The body of the notification.</summary>
    public string Message { get; }

    /// <summary>The window the notification is associated with, or null for the application default.</summary>
    public Window? Parent { get; }

    /// <summary>The standard icon shown with the notification.</summary>
    public NotificationIcon Icon { get; }

    /// <summary>Displays the notification and reports whether the platform accepted it.</summary>
    public bool Show(int timeout = TimeoutAuto)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_notification_show(
            Parent?.Handle ?? 0, Title, Message, (int)Icon, timeout);
    }
}
