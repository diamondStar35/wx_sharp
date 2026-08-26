using System;

namespace WxSharp;

public static class TextEntryDialog
{
    public static string? Show(string message, string caption = "Input", string value = "",
        bool password = false, Window? parent = null)
    {
        _ = App.RequireCurrent(); return ShowCore(parent, message, caption, value, password);
    }
    private static unsafe string? ShowCore(Window? parent, string message, string caption, string value, bool password)
    {
        var buffer = new byte[8192]; bool accepted;
        fixed (byte* bytes = buffer)
            accepted = NativeMethods.wxsharp_text_entry_dialog(parent?.Handle ?? 0, message, caption, value, password, bytes, buffer.Length);
        return accepted ? Utf8String.DecodeNullTerminated(buffer) : null;
    }
}

public static class NumberEntryDialog
{
    public static long? Show(string message, string prompt, string caption = "Input", long value = 0,
        long minimum = 0, long maximum = 100, Window? parent = null)
    {
        _ = App.RequireCurrent();
        if (minimum > maximum) throw new ArgumentException("Minimum cannot exceed maximum.");
        return NativeMethods.wxsharp_number_entry_dialog(parent?.Handle ?? 0, message, prompt, caption,
            value, minimum, maximum, out var result) ? result : null;
    }
}

public static class ColourDialog
{
    public static Colour? Show(Colour initial, Window? parent = null)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_colour_dialog(parent?.Handle ?? 0, initial.ToArgb(), out var result)
            ? Colour.FromArgb(result) : null;
    }
}
