namespace WxSharp;

/// <summary>A static text label.</summary>
public class Label : Control
{
    public Label(Container parent, string text, Alignment alignment = Alignment.Left)
        => Init(parent, NativeMethods.wxsharp_label_create(parent.Panel, text, (int)alignment));

    public unsafe string Text
    {
        get
        {
            var length = NativeMethods.wxsharp_label_get_text(Handle, null, 0);
            if (length <= 0)
                return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer)
                _ = NativeMethods.wxsharp_label_get_text(Handle, p, length + 1);
            return Utf8String.Decode(buffer, length);
        }
        set => NativeMethods.wxsharp_label_set_text(Handle, value);
    }
}
