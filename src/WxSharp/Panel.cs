namespace WxSharp;

/// <summary>An explicit wxPanel child container.</summary>
public class Panel : Window
{
    /// <summary>Wraps a Panel wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal Panel(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    public Panel(Window parent, int id = WindowId.Any, Point? position = null, Size? size = null,
        PanelStyle style = PanelStyle.Default) : base(parent, id)
    {
        Initialize(GetType() == typeof(Panel)
            ? NativeMethods.wxsharp_panel_create(parent.Handle, id, (int)style, Token)
            : NativeMethods.wxsharp_custom_panel_create(parent.Handle, id, (int)style, Token));
        ApplyInitialGeometry(position, size);
    }
}
