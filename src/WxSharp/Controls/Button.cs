using System;

namespace WxSharp;

/// <summary>A push button.</summary>
public class Button : Control
{
    public event EventHandler<CommandEventArgs> Click
    {
        add => AddHandler(WxEvents.ButtonClicked, value);
        remove => RemoveHandler(WxEvents.ButtonClicked, value);
    }

    public Button(Window parent, int id = WindowId.Any, string label = "", Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_button_create(parent.Handle, id, label, Token));
        ApplyInitialGeometry(position, size);
    }

    /// <summary>Makes this the default button, so pressing Enter activates it (e.g. a dialog's OK).</summary>
    public void SetDefault() => NativeMethods.wxsharp_button_set_default(Handle);

    public unsafe string Label
    {
        get
        {
            var length = NativeMethods.wxsharp_button_get_label(Handle, null, 0);
            if (length <= 0)
                return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer)
                _ = NativeMethods.wxsharp_button_get_label(Handle, p, length + 1);
            return Utf8String.Decode(buffer, length);
        }
        set => NativeMethods.wxsharp_button_set_label(Handle, value);
    }
}
