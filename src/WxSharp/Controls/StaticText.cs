namespace WxSharp;

/// <summary>A native wxStaticText label.</summary>
public class StaticText : Control
{
    public StaticText(Window parent, int id = WindowId.Any, string label = "", Alignment alignment = Alignment.Left,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_label_create(parent.Handle, id, label, (int)alignment, Token));
        ApplyInitialGeometry(position, size);
    }

    // The text is Window.Label: wxWindow::SetLabel is virtual and wxStaticText overrides it, so the
    // inherited property already reaches the right implementation.
}
