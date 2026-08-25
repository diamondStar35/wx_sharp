namespace WxSharp;

/// <summary>Explicit layout for callers that want more than a container's default vertical stack. A sizer
/// arranges items in one direction; each item is added with a <paramref name="proportion"/> (0 = fixed size,
/// &gt;0 = share of the leftover space), optional expand/centre, and a border. Sizers nest, and a container
/// adopts one through <see cref="Container.SetSizer"/>.</summary>
public abstract class Sizer
{
    internal nint Handle { get; }

    private protected Sizer(nint handle) => Handle = handle;

    /// <summary>Adds a control. <paramref name="expand"/> stretches it across the sizer's cross axis;
    /// <paramref name="center"/> centres it there; <paramref name="border"/> is the margin (in pixels) on all
    /// sides.</summary>
    public void Add(Control control, int proportion = 0, bool expand = false, bool center = false, int border = 0)
        => NativeMethods.wxsharp_sizer_add_control(Handle, control.Handle, proportion, expand, center, border);

    /// <summary>Nests another sizer inside this one (a row of controls within a column, …).</summary>
    public void Add(Sizer child, int proportion = 0, bool expand = false, int border = 0)
        => NativeMethods.wxsharp_sizer_add_sizer(Handle, child.Handle, proportion, expand, border);

    /// <summary>Adds a fixed-size gap of <paramref name="size"/> pixels along the sizer's main axis.</summary>
    public void AddSpacer(int size) => NativeMethods.wxsharp_sizer_add_spacer(Handle, size);

    /// <summary>Adds a stretchable gap that grows to push items apart (e.g. a spacer before a right-aligned
    /// button row).</summary>
    public void AddStretchSpacer(int proportion = 1) => NativeMethods.wxsharp_sizer_add_stretch_spacer(Handle, proportion);
}
