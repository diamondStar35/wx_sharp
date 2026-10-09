using System;

namespace WxSharp;

/// <summary>wxWidgets' global name/value option store, following <c>wxSystemOptions</c>. It holds strings
/// keyed by name that wxWidgets itself reads at various points to alter its behaviour, and that an
/// application can also read back.</summary>
///
/// <remarks>
/// Unlike most of WxSharp, these calls do not need a running <see cref="App"/>: the store is global and
/// several of its keys are read while the application is still starting up. The most useful of those on
/// Windows is <c>msw.dark-mode</c> - set it to 1 to turn on dark mode when the system is using it, or 2 to
/// force dark mode on, and set it <b>before</b> creating the <see cref="App"/>, because wxWidgets reads it
/// as the application initializes. It has the same effect as <see cref="App.EnableDarkMode"/> but is chosen
/// up front rather than called afterwards.
/// </remarks>
public static class SystemOptions
{
    /// <summary>Sets an integer-valued option, following <c>wxSystemOptions.SetOption</c>.</summary>
    public static void SetOption(string name, int value)
    {
        ArgumentNullException.ThrowIfNull(name);
        NativeMethods.wxsharp_systemoptions_set_option_int(name, value);
    }

    /// <summary>Sets a string-valued option, following <c>wxSystemOptions.SetOption</c>.</summary>
    public static void SetOption(string name, string value)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);
        NativeMethods.wxsharp_systemoptions_set_option_string(name, value);
    }

    /// <summary>The integer value of an option, or 0 when it has not been set, following
    /// <c>wxSystemOptions.GetOptionInt</c>.</summary>
    public static int GetOptionInt(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return NativeMethods.wxsharp_systemoptions_get_option_int(name);
    }

    /// <summary>Whether an option has been set, following <c>wxSystemOptions.HasOption</c>.</summary>
    public static bool HasOption(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return NativeMethods.wxsharp_systemoptions_has_option(name);
    }
}
