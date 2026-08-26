using System;

namespace WxSharp;

/// <summary>Native open/save file dialogs. The wildcard is a wx filter string, e.g.
/// <c>"Sounds (*.wav)|*.wav|All files (*.*)|*.*"</c>. Returns the chosen path, or null if cancelled.</summary>
public static class FileDialog
{
    public static string? Open(string title, string wildcard = "", Window? parent = null)
        => Show(title, wildcard, save: false, parent);

    public static string? Save(string title, string wildcard = "", Window? parent = null)
        => Show(title, wildcard, save: true, parent);

    /// <summary>Shows a folder picker; returns the chosen folder, or null if cancelled.</summary>
    public static unsafe string? Folder(string title, string? initialDir = null, Window? parent = null)
    {
        _ = App.RequireCurrent();
        var buffer = new byte[8192];
        bool ok;
        fixed (byte* p = buffer)
            ok = NativeMethods.wxsharp_dir_dialog(parent?.Handle ?? 0, title, initialDir ?? string.Empty, p, buffer.Length);
        return ok ? Utf8String.DecodeNullTerminated(buffer) : null;
    }

    private static unsafe string? Show(string title, string wildcard, bool save, Window? parent)
    {
        _ = App.RequireCurrent();
        var buffer = new byte[8192];
        bool ok;
        fixed (byte* p = buffer)
            ok = NativeMethods.wxsharp_file_dialog(parent?.Handle ?? 0, title, wildcard, save, p, buffer.Length);
        return ok ? Utf8String.DecodeNullTerminated(buffer) : null;
    }
}
