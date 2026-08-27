using System;

namespace WxSharp;

/// <summary>How <see cref="Wx.LaunchDefaultBrowser"/> opens a URL, following the <c>wxBROWSER_*</c>
/// flags.</summary>
[Flags]
public enum BrowserFlags
{
    None = 0,

    /// <summary>Open in a new window rather than a new tab, where the browser distinguishes them.</summary>
    NewWindow = 0x01,

    /// <summary>Do not show a busy cursor while the browser starts.</summary>
    NoBusyCursor = 0x02,
}

/// <summary>How <see cref="Wx.Execute"/> runs a command, following the <c>wxEXEC_*</c> flags.</summary>
[Flags]
public enum ExecuteFlags
{
    /// <summary>Start the process and return at once.</summary>
    Async = 0,

    /// <summary>Wait for the process to finish. The event loop keeps running unless
    /// <see cref="NoEvents"/> is also set, so the interface stays responsive.</summary>
    Sync = 1,

    /// <summary>Show the console window a console program would otherwise get hidden.</summary>
    ShowConsole = 2,

    /// <summary>Make the child a process group leader. Unix only.</summary>
    MakeGroupLeader = 4,

    /// <summary>Leave the application's windows enabled while waiting. Only meaningful with
    /// <see cref="Sync"/>.</summary>
    NoDisable = 8,

    /// <summary>Do not dispatch events while waiting. Only meaningful with <see cref="Sync"/>.</summary>
    NoEvents = 16,

    /// <summary>Hide the console window a console program would otherwise get.</summary>
    HideConsole = 32,

    /// <summary>Wait, and block the interface completely while waiting.</summary>
    Block = Sync | NoEvents,
}

/// <summary>Which mouse buttons are held down, as <see cref="Wx.GetMouseState"/> reports them.</summary>
[Flags]
public enum MouseButtons
{
    None = 0,
    Left = 1,
    Middle = 2,
    Right = 4,
    Aux1 = 8,
    Aux2 = 16,
}

/// <summary>Where the pointer is and what is held down, following <c>wxMouseState</c>.</summary>
public readonly record struct MouseState(Point Position, MouseButtons Buttons, KeyModifiers Modifiers);

/// <summary>Which operating system wxWidgets thinks it is running on, following the <c>wxOS_*</c>
/// values.</summary>
public enum OperatingSystemId
{
    Unknown = 0,
    MacOsX = 1 << 2,
    Windows9x = 1 << 6,
    WindowsNt = 1 << 7,
    WindowsMicro = 1 << 8,
    Linux = 1 << 11,
    Freebsd = 1 << 12,
    Openbsd = 1 << 13,
    Netbsd = 1 << 14,
    Solaris = 1 << 15,
    Aix = 1 << 16,
    Hpux = 1 << 17,
}

public static partial class Wx
{
    // ---- Handing a file or a URL to whatever the user has chosen for it ---------------------------------

    /// <summary>Opens a URL in the user's browser. False when the platform could not.</summary>
    ///
    /// <remarks>
    /// This asks the desktop what is registered for the scheme rather than looking for a browser by name,
    /// so it honours whatever the user has set as their default — including a browser chosen because it
    /// works with their screen reader.
    /// </remarks>
    public static bool LaunchDefaultBrowser(string url, BrowserFlags flags = BrowserFlags.None)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_launch_default_browser(url, (int)flags);
    }

    /// <summary>Opens a file or folder in whatever the user has associated with it — a document in their
    /// editor, a folder in their file manager. False when the platform could not.</summary>
    public static bool LaunchDefaultApplication(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_launch_default_application(path, 0);
    }

    /// <summary>Runs a command. Returns the process ID when started asynchronously and the exit code when
    /// waited for; 0 and -1 respectively mean it could not be started. That split is wxWidgets' own
    /// convention, kept rather than smoothed over.</summary>
    public static long Execute(string command, ExecuteFlags flags = ExecuteFlags.Async)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_execute(command, (int)flags);
    }

    /// <summary>Runs a command through the system shell, waiting for it to finish. An empty command opens
    /// an interactive shell where the platform has one.</summary>
    public static bool Shell(string command = "")
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_shell(command ?? string.Empty) == 0;
    }

    // ---- Feedback and input ------------------------------------------------------------------------------

    /// <summary>Plays the system alert sound. Prefer this to a bundled sound file: the user may have
    /// changed or silenced it deliberately, and assistive technology may be listening for it.</summary>
    public static void Bell()
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_bell();
    }

    /// <summary>Whether a key is down right now, without waiting for an event. Use it to read a modifier
    /// during an operation that is already under way; ordinary key handling belongs in a key event.</summary>
    public static bool GetKeyState(int keyCode)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_get_key_state(keyCode);
    }

    /// <summary>Where the pointer is, in screen coordinates.</summary>
    public static Point GetMousePosition()
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_get_mouse_position(out var x, out var y);
        return new Point(x, y);
    }

    /// <summary>Where the pointer is and what buttons and modifiers are held.</summary>
    public static MouseState GetMouseState()
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_get_mouse_state(out var x, out var y, out var buttons, out var modifiers);
        return new MouseState(new Point(x, y), (MouseButtons)buttons, (KeyModifiers)modifiers);
    }

    // ---- Who is running this, and where ------------------------------------------------------------------

    /// <summary>The account name the process is running as.</summary>
    public static unsafe string UserId => Read(NativeMethods.wxsharp_get_user_id);

    /// <summary>The user's display name, where the platform has one.</summary>
    public static unsafe string UserName => Read(NativeMethods.wxsharp_get_user_name);

    /// <summary>The machine's short network name.</summary>
    public static unsafe string HostName => Read(NativeMethods.wxsharp_get_host_name);

    /// <summary>The machine's fully qualified name.</summary>
    public static unsafe string FullHostName => Read(NativeMethods.wxsharp_get_full_host_name);

    /// <summary>The user's email address, where the platform can say.</summary>
    public static unsafe string EmailAddress => Read(NativeMethods.wxsharp_get_email_address);

    /// <summary>The user's home directory.</summary>
    public static unsafe string HomeDirectory => Read(NativeMethods.wxsharp_get_home_dir);

    // ---- What the machine is ------------------------------------------------------------------------------

    /// <summary>A human-readable description of the operating system.</summary>
    public static unsafe string OsDescription => Read(NativeMethods.wxsharp_get_os_description);

    /// <summary>Which operating system this is, and its version.</summary>
    public static (OperatingSystemId Id, int Major, int Minor, int Micro) GetOsVersion()
    {
        _ = App.RequireCurrent();
        var id = NativeMethods.wxsharp_get_os_version(out var major, out var minor, out var micro);
        return ((OperatingSystemId)id, major, minor, micro);
    }

    /// <summary>Whether the operating system is at least this version.</summary>
    public static bool CheckOsVersion(int major, int minor = 0, int micro = 0)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_check_os_version(major, minor, micro);
    }

    /// <summary>Whether the operating system is 64-bit, which is not the same as this process being
    /// 64-bit.</summary>
    public static bool IsPlatform64Bit
    {
        get { _ = App.RequireCurrent(); return NativeMethods.wxsharp_is_platform_64bit(); }
    }

    /// <summary>Whether this machine is little-endian.</summary>
    public static bool IsPlatformLittleEndian
    {
        get { _ = App.RequireCurrent(); return NativeMethods.wxsharp_is_platform_little_endian(); }
    }

    /// <summary>The architecture this process was built for.</summary>
    public static unsafe string CpuArchitectureName => Read(NativeMethods.wxsharp_get_cpu_architecture_name);

    /// <summary>The machine's own architecture, which differs from
    /// <see cref="CpuArchitectureName"/> when the process is being emulated.</summary>
    public static unsafe string NativeCpuArchitectureName => Read(NativeMethods.wxsharp_get_native_cpu_architecture_name);

    /// <summary>The wxWidgets version this wrapper is running against. Worth putting in a bug report or an
    /// About box.</summary>
    public static unsafe string LibraryVersion => Read(NativeMethods.wxsharp_get_library_version);

    /// <summary>This process's ID.</summary>
    public static uint ProcessId
    {
        get { _ = App.RequireCurrent(); return NativeMethods.wxsharp_get_process_id(); }
    }

    /// <summary>Free memory in bytes, or -1 where the platform will not say.</summary>
    public static long FreeMemory
    {
        get { _ = App.RequireCurrent(); return NativeMethods.wxsharp_get_free_memory(); }
    }

    /// <summary>The total and free space on the volume holding a path. False when the platform could not
    /// say, in which case both values are zero.</summary>
    public static bool GetDiskSpace(string path, out long total, out long free)
    {
        _ = App.RequireCurrent();
        total = 0;
        free = 0;
        return NativeMethods.wxsharp_get_disk_space(path ?? string.Empty, out total, out free);
    }

    // ---- The environment ----------------------------------------------------------------------------------

    /// <summary>An environment variable, or null when it is not set. Null and an empty string are
    /// different answers — though on Windows only the first is reachable through
    /// <see cref="SetEnv"/>, which deletes a variable rather than storing an empty one.</summary>
    public static unsafe string? GetEnv(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _ = App.RequireCurrent();
        var length = NativeMethods.wxsharp_get_env(name, null, 0);
        if (length < 0) return null;
        if (length == 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_get_env(name, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>Sets an environment variable for this process and anything it starts. On Windows an empty
    /// value deletes the variable, which is what the platform does rather than anything added here.</summary>
    public static bool SetEnv(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_set_env(name, value ?? string.Empty);
    }

    /// <summary>Removes an environment variable.</summary>
    public static bool UnsetEnv(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_unset_env(name);
    }

    // ---- Sleeping -----------------------------------------------------------------------------------------

    /// <summary>Blocks the calling thread. On the UI thread this freezes the interface outright, including
    /// anything a screen reader is trying to read from it — a timer or a background task is almost always
    /// the right answer instead. Wrapped for completeness, not as a recommendation.</summary>
    public static void Sleep(int seconds)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_sleep(seconds);
    }

    /// <inheritdoc cref="Sleep"/>
    public static void MilliSleep(ulong milliseconds)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_milli_sleep(milliseconds);
    }

    /// <inheritdoc cref="Sleep"/>
    public static void MicroSleep(ulong microseconds)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_micro_sleep(microseconds);
    }

    // ---- Finding and disabling windows ---------------------------------------------------------------------

    /// <summary>Finds a window by its name, searching a parent's children or every window. Null when
    /// nothing matches, and also when the window belongs to wxWidgets rather than to a wrapper.</summary>
    public static Window? FindWindowByName(string name, Window? parent = null)
    {
        _ = App.RequireCurrent();
        return App.Lookup(NativeMethods.wxsharp_find_window_by_name(name ?? string.Empty, parent?.Handle ?? 0));
    }

    /// <summary>Finds a window by its label.</summary>
    public static Window? FindWindowByLabel(string label, Window? parent = null)
    {
        _ = App.RequireCurrent();
        return App.Lookup(NativeMethods.wxsharp_find_window_by_label(label ?? string.Empty, parent?.Handle ?? 0));
    }

    /// <summary>The window under a point in screen coordinates.</summary>
    public static Window? FindWindowAtPoint(Point point)
    {
        _ = App.RequireCurrent();
        return App.Lookup(NativeMethods.wxsharp_find_window_at_point(point.X, point.Y));
    }

    /// <summary>The application's active top-level window, or null when none is.</summary>
    public static Window? GetActiveWindow()
    {
        _ = App.RequireCurrent();
        return App.Lookup(NativeMethods.wxsharp_get_active_window());
    }

    /// <summary>Enables or disables every top-level window at once.</summary>
    public static void EnableTopLevelWindows(bool enable = true)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_enable_top_level_windows(enable);
    }

    /// <summary>Disables every window except one until the returned object is disposed — the usual way to
    /// keep input out during a long operation. Leave a window enabled if it carries a cancel button;
    /// disabling everything with no way out is what makes an application feel hung.</summary>
    public static IDisposable DisableWindows(Window? except = null)
    {
        _ = App.RequireCurrent();
        return new WindowDisablerScope(NativeMethods.wxsharp_window_disabler_begin(except?.Handle ?? 0));
    }

    private sealed class WindowDisablerScope(nint handle) : IDisposable
    {
        private nint _handle = handle;

        public void Dispose()
        {
            if (_handle == 0) return;
            NativeMethods.wxsharp_window_disabler_end(_handle);
            _handle = 0;
        }
    }

    /// <summary>Strips the mnemonic markers and accelerator from a menu label, turning <c>"E&amp;xit\tCtrl+Q"</c>
    /// into <c>"Exit"</c>. Use it anywhere a menu label is shown or spoken outside a menu, where the
    /// ampersand would otherwise be read aloud.</summary>
    public static unsafe string StripMenuCodes(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        _ = App.RequireCurrent();
        var length = NativeMethods.wxsharp_strip_menu_codes(text, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_strip_menu_codes(text, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    private static unsafe string Read(ReadString read)
    {
        _ = App.RequireCurrent();
        var length = read(null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = read(p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    private unsafe delegate int ReadString(byte* buffer, int bufferLength);
}
