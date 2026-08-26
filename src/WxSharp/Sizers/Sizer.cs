using System;

namespace WxSharp;

public enum Orientation { Horizontal, Vertical }

[Flags]
public enum SizerFlags
{
    None = 0, Expand = 1, AlignCenter = 2,
    BorderLeft = 4, BorderTop = 8, BorderRight = 16, BorderBottom = 32,
    All = BorderLeft | BorderTop | BorderRight | BorderBottom,
}

public abstract class Sizer
{
    private readonly App _owner;
    private readonly nint _handle;
    internal nint Handle
    {
        get
        {
            var current = App.RequireCurrent();
            ObjectDisposedException.ThrowIf(current != _owner, this);
            return _handle;
        }
    }
    private protected Sizer(nint handle)
    {
        _owner = App.RequireCurrent();
        _handle = handle != 0 ? handle : throw new InvalidOperationException("wxWidgets failed to create the sizer.");
    }
    public void Add(Window window, int proportion = 0, SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(window); ValidateItem(proportion, border);
        NativeMethods.wxsharp_sizer_add_control(Handle, window.Handle, proportion, (int)flags, border);
    }
    public void Add(Sizer child, int proportion = 0, SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(child); ValidateItem(proportion, border);
        NativeMethods.wxsharp_sizer_add_sizer(Handle, child.Handle, proportion, (int)flags, border);
    }
    public void AddSpacer(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(size); NativeMethods.wxsharp_sizer_add_spacer(Handle, size);
    }
    public void AddStretchSpacer(int proportion = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(proportion);
        NativeMethods.wxsharp_sizer_add_stretch_spacer(Handle, proportion);
    }
    private static void ValidateItem(int proportion, int border)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(proportion);
        ArgumentOutOfRangeException.ThrowIfNegative(border);
    }
}
