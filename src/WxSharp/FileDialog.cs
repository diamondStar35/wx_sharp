using System;

namespace WxSharp;

/// <summary>The native open and save dialogs.</summary>
///
/// <remarks>
/// The wildcard is a wxWidgets filter string: description and pattern separated by <c>|</c>, repeated for
/// each filter — <c>"Audio (*.mp3;*.flac)|*.mp3;*.flac|All files (*.*)|*.*"</c>. An empty wildcard means all
/// files.
/// </remarks>
public static class FileDialog
{
    /// <summary>Shows an open dialog and returns the chosen path, or null if it was cancelled.</summary>
    public static string? Open(string title, string wildcard = "", Window? parent = null,
        string? directory = null, string? fileName = null,
        FileDialogStyle style = FileDialogStyle.DefaultOpen)
    {
        var paths = Show(title, wildcard, parent, directory, fileName, style & ~FileDialogStyle.Multiple);
        return paths.Length == 0 ? null : paths[0];
    }

    /// <summary>Shows an open dialog that allows more than one file, and returns the chosen paths. Empty if
    /// it was cancelled.</summary>
    public static string[] OpenMultiple(string title, string wildcard = "", Window? parent = null,
        string? directory = null, FileDialogStyle style = FileDialogStyle.DefaultOpen)
        => Show(title, wildcard, parent, directory, null, style | FileDialogStyle.Multiple);

    /// <summary>Shows a save dialog and returns the chosen path, or null if it was cancelled.</summary>
    public static string? Save(string title, string wildcard = "", Window? parent = null,
        string? directory = null, string? fileName = null,
        FileDialogStyle style = FileDialogStyle.DefaultSave)
    {
        var paths = Show(title, wildcard, parent, directory, fileName,
            (style & ~FileDialogStyle.Multiple & ~FileDialogStyle.Open) | FileDialogStyle.Save);
        return paths.Length == 0 ? null : paths[0];
    }

    /// <summary>Shows a folder picker; returns the chosen folder, or null if it was cancelled.</summary>
    public static unsafe string? Folder(string title, string? initialDir = null, Window? parent = null)
    {
        _ = App.RequireCurrent();
        var buffer = new byte[8192];
        bool ok;
        fixed (byte* p = buffer)
            ok = NativeMethods.wxsharp_dir_dialog(parent?.Handle ?? 0, title, initialDir ?? string.Empty, p, buffer.Length);
        return ok ? Utf8String.DecodeNullTerminated(buffer) : null;
    }

    /// <summary>Shows the dialog and returns every path chosen. The full control, for callers that want a
    /// style combination the shorthands above do not cover.</summary>
    public static unsafe string[] Show(string title, string wildcard, Window? parent,
        string? directory, string? fileName, FileDialogStyle style)
    {
        _ = App.RequireCurrent();
        var count = NativeMethods.wxsharp_file_dialog(parent?.Handle ?? 0, title, wildcard ?? string.Empty,
            directory ?? string.Empty, fileName ?? string.Empty, (int)style);
        if (count <= 0) return Array.Empty<string>();

        // The native side holds the results until the next call, so each path can be read back at its own
        // length instead of everything being squeezed into one caller-sized buffer.
        var paths = new string[count];
        for (var i = 0; i < count; ++i)
        {
            var length = NativeMethods.wxsharp_file_dialog_result(i, null, 0);
            if (length <= 0) { paths[i] = string.Empty; continue; }
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_file_dialog_result(i, p, buffer.Length);
            paths[i] = Utf8String.Decode(buffer, length);
        }
        return paths;
    }
}
