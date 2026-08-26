namespace WxSharp;

/// <summary>A native wxStaticText label.</summary>
public class StaticText : Control
{
    public StaticText(Window parent, int id = WindowId.Any, string label = "", Alignment alignment = Alignment.Left,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_label_create(parent.Handle, id, label, (int)alignment, Token));
        ApplyInitialGeometry(position, size);
    }

    public unsafe string Label
    {
        get
        {
            var length = NativeMethods.wxsharp_label_get_text(Handle, null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_label_get_text(Handle, p, length + 1);
            return Utf8String.Decode(buffer, length);
        }
        set => NativeMethods.wxsharp_label_set_text(Handle, value);
    }
}
