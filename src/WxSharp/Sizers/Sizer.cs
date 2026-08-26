using System;

namespace WxSharp;

public enum Orientation { Horizontal, Vertical }

/// <summary>How a sizer treats one item: whether it grows, how it is aligned in the space it is given, and
/// which of its edges get the border.</summary>
///
/// <remarks>
/// Alignment only applies across a sizer's direction, never along it - the item's position along the
/// direction is what the sizer is deciding. So a vertical <see cref="BoxSizer"/> takes
/// <see cref="SizerFlags.AlignLeft"/>, <see cref="SizerFlags.AlignRight"/> and
/// <see cref="SizerFlags.AlignCenterHorizontal"/>, and a horizontal one takes the top, bottom and
/// <see cref="SizerFlags.AlignCenterVertical"/> counterparts. wxWidgets asserts at run time when the wrong
/// axis is used, rather than quietly ignoring it.
/// </remarks>
[Flags]
public enum SizerFlags
{
    None = 0,

    /// <summary>Grow to fill the space across the sizer's direction.</summary>
    Expand = 1,

    /// <summary>Centre in both directions.</summary>
    AlignCenter = 2,

    BorderLeft = 4,
    BorderTop = 8,
    BorderRight = 16,
    BorderBottom = 32,

    AlignLeft = 64,
    AlignRight = 128,
    AlignTop = 256,
    AlignBottom = 512,

    /// <summary>Centre vertically - what a label beside a taller control usually wants.</summary>
    AlignCenterVertical = 1024,
    AlignCenterHorizontal = 2048,

    /// <summary>Keep the item's aspect ratio while it grows.</summary>
    Shaped = 4096,

    /// <summary>Never shrink the item below its initial best size.</summary>
    FixedMinSize = 8192,

    /// <summary>Keep the item's space reserved while it is hidden, so the layout does not jump.</summary>
    ReserveSpaceEvenIfHidden = 16384,

    /// <summary>A border on all four edges.</summary>
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
    /// <summary>Wraps a sizer wxWidgets built for us, such as a dialog's standard button row.</summary>
    internal static Sizer Attach(nint handle) => new AttachedSizer(handle);

    private sealed class AttachedSizer : Sizer
    {
        internal AttachedSizer(nint handle) : base(handle) { }
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
