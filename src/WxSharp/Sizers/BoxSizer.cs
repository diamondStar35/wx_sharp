namespace WxSharp;

public sealed class BoxSizer : Sizer
{
    /// <summary>Creates a box sizer. The default orientation is horizontal, as it is in wxWidgets and
    /// Phoenix.</summary>
    public BoxSizer(Orientation orientation = Orientation.Horizontal)
        : base(Create(orientation)) { }

    private static nint Create(Orientation orientation)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_boxsizer_create(orientation == Orientation.Horizontal);
    }
}
