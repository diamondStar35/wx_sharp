namespace WxSharp;

/// <summary>An explicit wxPanel child container.</summary>
public class Panel : Window
{
    /// <summary>Wraps a Panel wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal Panel(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    /// <summary>Creates a <c>wxPanel</c> child of <paramref name="parent"/>. A panel is the ordinary
    /// container for other controls and handles tab traversal between them.</summary>
    /// <param name="parent">The window to place the panel in.</param>
    /// <param name="id">The window identifier, or <see cref="WindowId.Any"/> to assign one.</param>
    /// <param name="position">The panel's position in the parent, or null for the default.</param>
    /// <param name="size">The panel's size, or null for the default.</param>
    /// <param name="style">Panel style flags.</param>
    public Panel(Window parent, int id = WindowId.Any, Point? position = null, Size? size = null,
        PanelStyle style = PanelStyle.Default) : base(parent, id)
    {
        Initialize(GetType() == typeof(Panel)
            ? NativeMethods.wxsharp_panel_create(parent.Handle, id, (int)style, Token)
            : NativeMethods.wxsharp_custom_panel_create(parent.Handle, id, (int)style, Token));
        ApplyInitialGeometry(position, size);
    }
}
