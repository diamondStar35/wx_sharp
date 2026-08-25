namespace WxSharp;

/// <summary>A sizer that lays its items out in a single row (<paramref name="horizontal"/> true) or column
/// (the default). The most common layout; nest a horizontal box inside a vertical one for rows-in-columns.</summary>
public sealed class BoxSizer : Sizer
{
    public BoxSizer(bool horizontal = false)
        : base(NativeMethods.wxsharp_boxsizer_create(horizontal))
    {
    }
}
