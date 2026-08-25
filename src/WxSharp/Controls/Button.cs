using System;

namespace WxSharp;

/// <summary>A push button.</summary>
public class Button : Control
{
    public event Action? Click;

    public Button(Container parent, string label)
        => Init(parent, NativeMethods.wxsharp_button_create(parent.Panel, label, Id));

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

    private protected override void OnEvent(EventKind evt)
    {
        if (evt == EventKind.Click)
            Click?.Invoke();
    }
}
