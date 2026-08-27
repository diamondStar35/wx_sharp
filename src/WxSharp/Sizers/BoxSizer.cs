namespace WxSharp;

public sealed class BoxSizer : Sizer
{
    /// <summary>Creates a box sizer. The default orientation is horizontal, as it is in wxWidgets and
    /// Phoenix.</summary>
    public BoxSizer(Orientation orientation = Orientation.Horizontal)
        : base(Create(orientation)) { }

    /// <summary>Which way the sizer lays its items out. Changing it re-lays out on the next
    /// <see cref="Sizer.Layout"/>.</summary>
    public Orientation Orientation
    {
        get => NativeMethods.wxsharp_boxsizer_get_orientation(Handle) == 1 ? Orientation.Vertical : Orientation.Horizontal;
        set => NativeMethods.wxsharp_boxsizer_set_orientation(Handle, value == Orientation.Vertical);
    }

    /// <summary>Whether the sizer lays its items out top to bottom.</summary>
    public bool IsVertical => Orientation == Orientation.Vertical;

    private static nint Create(Orientation orientation)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_boxsizer_create(orientation == Orientation.Horizontal);
    }
}
