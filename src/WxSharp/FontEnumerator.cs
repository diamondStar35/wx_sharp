using System;

namespace WxSharp;

/// <summary>Lists the fonts the platform has, following <c>wxFontEnumerator</c>.</summary>
///
/// <remarks>
/// wxWidgets shapes this as a class to derive from, with <c>OnFacename</c> and <c>OnFontEncoding</c>
/// callbacks that a subclass overrides to collect results. It also ships statics that do the collecting for
/// you, and those are what an application actually wants - filling a font picker, or checking a face before
/// using it. Only the statics are wrapped, which is the form wxPython recommends too.
///
/// This is the one way to know that a face exists before asking for it: assigning an unavailable
/// <see cref="Font.FaceName"/> leaves the font unchanged, and only <see cref="Font.TrySetFaceName"/>
/// reports it afterwards.
/// </remarks>
public static class FontEnumerator
{
    /// <summary>Every typeface installed, optionally restricted to one encoding or to fixed-width faces.
    /// Follows <c>wxFontEnumerator.GetFacenames</c>.</summary>
    public static string[] GetFacenames(FontEncoding encoding = FontEncoding.System, bool fixedWidthOnly = false)
    {
        _ = App.RequireCurrent();
        return Collect(NativeMethods.wxsharp_font_enumerate_facenames((int)encoding, fixedWidthOnly));
    }

    /// <summary>The encodings available, for every face or for one of them. Follows
    /// <c>wxFontEnumerator.GetEncodings</c>.</summary>
    public static string[] GetEncodings(string facename = "")
    {
        ArgumentNullException.ThrowIfNull(facename);
        _ = App.RequireCurrent();
        return Collect(NativeMethods.wxsharp_font_enumerate_encodings(facename));
    }

    /// <summary>Whether a typeface of this name is installed.</summary>
    public static bool IsValidFacename(string facename)
    {
        ArgumentNullException.ThrowIfNull(facename);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_font_is_valid_facename(facename);
    }

    /// <summary>Forgets what was enumerated, so the next call asks the system again. Needed only after
    /// fonts have been installed or removed while the application is running.</summary>
    public static void InvalidateCache()
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_font_invalidate_enumeration_cache();
    }

    // The native side holds the result set until the next call rather than guessing at a buffer size, the
    // same way a multiple file selection is read back.
    private static unsafe string[] Collect(int count)
    {
        if (count <= 0) return [];
        var names = new string[count];
        for (var i = 0; i < count; i++)
        {
            var length = NativeMethods.wxsharp_font_enumerated_name(i, null, 0);
            if (length <= 0) { names[i] = string.Empty; continue; }
            var bytes = new byte[length + 1];
            fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_font_enumerated_name(i, buffer, bytes.Length);
            names[i] = Utf8String.Decode(bytes, length);
        }
        return names;
    }
}
