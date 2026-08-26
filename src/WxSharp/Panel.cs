namespace WxSharp;

/// <summary>An explicit wxPanel child container.</summary>
public class Panel : Window
{
    public Panel(Window parent, int id = WindowId.Any, Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_panel_create(parent.Handle, id, Token));
        ApplyInitialGeometry(position, size);
    }
}
