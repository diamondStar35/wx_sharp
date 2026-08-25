namespace WxSharp;

/// <summary>The system clipboard (text).</summary>
public static class Clipboard
{
    public static void SetText(string text) => NativeMethods.wxsharp_clipboard_set_text(text);

    public static unsafe string GetText()
    {
        var length = NativeMethods.wxsharp_clipboard_get_text(null, 0);
        if (length <= 0)
            return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_clipboard_get_text(p, length + 1);
        return Utf8String.Decode(buffer, length);
    }
}
