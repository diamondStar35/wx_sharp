using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WxSharp;

/// <summary>A kind of content the clipboard can hold, following the standard <c>wxDataFormat</c> values.</summary>
public enum ClipboardFormat
{
    Text = 0,
    FileNames = 1,
    Bitmap = 2,
}

/// <summary>The system clipboard, following <c>wxClipboard</c>.</summary>
///
/// <remarks>
/// As in Phoenix, call <see cref="Open"/> before reading or writing and <see cref="Close"/> afterwards.
/// Keep the clipboard open through <c>Set* → Flush → Close</c> when copied data must be rendered immediately.
///
/// <para><see cref="Flush"/> has the same explicit lifetime semantics as Phoenix: successful data remains
/// available after the application exits only when the caller flushes it.</para>
/// </remarks>
public static class Clipboard
{
    /// <summary>Opens the clipboard and keeps it open until <see cref="Close"/>. Returns false when another
    /// application has it.</summary>
    public static bool Open()
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_clipboard_open();
    }

    public static void Close()
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_clipboard_close();
    }

    public static bool IsOpen
    {
        get { _ = App.RequireCurrent(); return NativeMethods.wxsharp_clipboard_is_opened(); }
    }

    /// <summary>Renders the contents and hands them to the system, so they outlive this application and no
    /// other application has to call back into it to read them. See the note on <see cref="Clipboard"/> for
    /// why this matters beyond the contents merely surviving.</summary>
    public static bool Flush()
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_clipboard_flush();
    }

    /// <summary>Empties the clipboard.</summary>
    public static void Clear()
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_clipboard_clear();
    }

    /// <summary>Whether the clipboard currently holds this kind of content.</summary>
    public static bool IsSupported(ClipboardFormat format)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_clipboard_is_supported((int)format);
    }

    /// <summary>Starts Phoenix's asynchronous format-availability query. Bind
    /// <see cref="WxEvents.ClipboardChanged"/> on <paramref name="sink"/> for completion.</summary>
    public static bool IsSupportedAsync(Window sink)
    {
        ArgumentNullException.ThrowIfNull(sink);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_clipboard_is_supported_async(sink.Handle);
    }

    /// <summary>On X11, switches between the clipboard proper and the primary selection. No effect
    /// elsewhere.</summary>
    public static void UsePrimarySelection(bool primary = false)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_clipboard_use_primary_selection(primary);
    }

    // ---- Text -----------------------------------------------------------------------------------------

    /// <summary>Puts text on the clipboard. False when the write failed, which usually means another
    /// application held the clipboard for a moment; retrying is reasonable, giving up quietly is
    /// reasonable, and crashing is not.</summary>
    public static bool SetText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_clipboard_set_text(text);
    }

    /// <summary>The clipboard's text, or an empty string when it holds none.</summary>
    public static unsafe string GetText()
    {
        _ = App.RequireCurrent();
        var length = NativeMethods.wxsharp_clipboard_get_text(null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_clipboard_get_text(p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    // ---- File lists -----------------------------------------------------------------------------------

    /// <summary>Puts a list of paths on the clipboard, so a file manager can paste them.</summary>
    public static unsafe bool SetFiles(params string[] paths)
    {
        ArgumentNullException.ThrowIfNull(paths);
        _ = App.RequireCurrent();

        var handles = new GCHandle[paths.Length];
        var pointers = new nint[paths.Length == 0 ? 1 : paths.Length];
        try
        {
            for (var i = 0; i < paths.Length; ++i)
            {
                ArgumentNullException.ThrowIfNull(paths[i]);
                var utf8 = Encoding.UTF8.GetBytes(paths[i] + "\0");
                handles[i] = GCHandle.Alloc(utf8, GCHandleType.Pinned);
                pointers[i] = handles[i].AddrOfPinnedObject();
            }
            fixed (nint* p = pointers)
                return NativeMethods.wxsharp_clipboard_set_files((byte**)p, paths.Length);
        }
        finally
        {
            foreach (var handle in handles)
                if (handle.IsAllocated) handle.Free();
        }
    }

    /// <summary>The paths on the clipboard, or an empty array when it holds none.</summary>
    public static unsafe string[] GetFiles()
    {
        _ = App.RequireCurrent();
        var count = NativeMethods.wxsharp_clipboard_read_files();
        if (count <= 0) return Array.Empty<string>();

        var paths = new string[count];
        for (var i = 0; i < count; ++i)
        {
            var length = NativeMethods.wxsharp_clipboard_get_file(i, null, 0);
            if (length <= 0) { paths[i] = string.Empty; continue; }
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_clipboard_get_file(i, p, buffer.Length);
            paths[i] = Utf8String.Decode(buffer, length);
        }
        return paths;
    }

    // ---- Bitmaps --------------------------------------------------------------------------------------

    public static bool SetBitmap(Bitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_clipboard_set_bitmap(bitmap.Handle);
    }

    /// <summary>The image on the clipboard, or null when it holds none. The caller owns what comes back.</summary>
    public static Bitmap? GetBitmap()
    {
        _ = App.RequireCurrent();
        var handle = NativeMethods.wxsharp_clipboard_get_bitmap();
        return handle == 0 ? null : Bitmap.Attach(handle);
    }
}
