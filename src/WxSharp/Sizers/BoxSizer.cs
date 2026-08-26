namespace WxSharp;

public sealed class BoxSizer : Sizer
{
    public BoxSizer(Orientation orientation = Orientation.Vertical)
        : base(Create(orientation)) { }

    private static nint Create(Orientation orientation)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_boxsizer_create(orientation == Orientation.Horizontal);
    }
}
