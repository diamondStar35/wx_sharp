namespace WxSharp;

/// <summary>One entry in a <see cref="Sizer"/> — a window, a nested sizer, or a spacer — together with the
/// proportion, flags and border the sizer was told about it. Returned by every method that adds to a sizer,
/// following <c>wxSizerItem</c>.</summary>
///
/// <remarks>
/// The item belongs to its sizer; removing it from the sizer destroys it, and this wrapper is stale
/// afterwards. Hold one only as long as the item is in the layout.
/// </remarks>
public sealed class SizerItem
{
    private readonly nint _handle;

    internal SizerItem(nint handle) => _handle = handle;
    internal nint Handle => _handle;

    /// <summary>Wraps a native item, or returns null for a null handle.</summary>
    internal static SizerItem? From(nint handle) => handle == 0 ? null : new SizerItem(handle);

    /// <summary>How much of the sizer's spare space along its direction this item takes, relative to the
    /// other items. 0 means the item keeps its own size.</summary>
    public int Proportion
    {
        get => NativeMethods.wxsharp_sizeritem_get_proportion(_handle);
        set => NativeMethods.wxsharp_sizeritem_set_proportion(_handle, value);
    }

    /// <summary>The alignment, border-edge and growth flags this item was added with.</summary>
    ///
    /// <remarks>
    /// <see cref="SizerFlags.AlignLeft"/> and <see cref="SizerFlags.AlignTop"/> are zero in wxWidgets, so
    /// they cannot be read back; an item added with either reports no alignment on that axis, which means
    /// the same thing.
    /// </remarks>
    public SizerFlags Flags
    {
        get => (SizerFlags)NativeMethods.wxsharp_sizeritem_get_flags(_handle);
        set => NativeMethods.wxsharp_sizeritem_set_flags(_handle, (int)value);
    }

    /// <summary>The border width, in pixels, applied to whichever edges the flags name.</summary>
    public int Border
    {
        get => NativeMethods.wxsharp_sizeritem_get_border(_handle);
        set => NativeMethods.wxsharp_sizeritem_set_border(_handle, value);
    }

    /// <summary>An identifier of the item's own, which every item can carry including a spacer. It is
    /// separate from any window ID and starts unset, so it is only useful once assigned - and it is what
    /// <see cref="Sizer.GetItemById"/> searches. To find the item holding a particular window, use
    /// <see cref="Sizer.GetItem(Window, bool)"/> instead.</summary>
    public int Id
    {
        get => NativeMethods.wxsharp_sizeritem_get_id(_handle);
        set => NativeMethods.wxsharp_sizeritem_set_id(_handle, value);
    }

    public bool IsWindow => NativeMethods.wxsharp_sizeritem_is_window(_handle);
    public bool IsSizer => NativeMethods.wxsharp_sizeritem_is_sizer(_handle);
    public bool IsSpacer => NativeMethods.wxsharp_sizeritem_is_spacer(_handle);

    /// <summary>Whether the item takes part in the layout. Hiding an item removes it from the layout unless
    /// it was added with <see cref="SizerFlags.ReserveSpaceEvenIfHidden"/>.</summary>
    public bool Shown
    {
        get => NativeMethods.wxsharp_sizeritem_is_shown(_handle);
        set => NativeMethods.wxsharp_sizeritem_show(_handle, value);
    }

    /// <summary>The smallest size the sizer will give this item.</summary>
    public Size MinSize
    {
        get
        {
            NativeMethods.wxsharp_sizeritem_get_min_size(_handle, out var w, out var h);
            return new Size(w, h);
        }
        set => NativeMethods.wxsharp_sizeritem_set_min_size(_handle, value.Width, value.Height);
    }

    /// <summary>The size the sizer last gave this item.</summary>
    public Size Size
    {
        get
        {
            NativeMethods.wxsharp_sizeritem_get_size(_handle, out var w, out var h);
            return new Size(w, h);
        }
    }

    /// <summary>Where the sizer last placed this item.</summary>
    public Point Position
    {
        get
        {
            NativeMethods.wxsharp_sizeritem_get_position(_handle, out var x, out var y);
            return new Point(x, y);
        }
    }
}
