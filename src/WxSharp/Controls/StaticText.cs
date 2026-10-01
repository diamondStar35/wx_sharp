namespace WxSharp;

/// <summary>A native wxStaticText label.</summary>
public class StaticText : Control
{
    /// <summary>Wraps a StaticText wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal StaticText(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    /// <summary>Creates a label showing <paramref name="label"/>, aligned per <paramref name="alignment"/>.
    /// The text is read and written through the inherited <see cref="Window.Label"/>.</summary>
    public StaticText(Window parent, int id = WindowId.Any, string label = "", Alignment alignment = Alignment.Left,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(GetType() == typeof(StaticText)
            ? NativeMethods.wxsharp_label_create(parent.Handle, id, label, (int)alignment, Token)
            : NativeMethods.wxsharp_custom_label_create(parent.Handle, id, label, (int)alignment, Token));
        ApplyInitialGeometry(position, size);
    }

    // The text is Window.Label: wxWindow::SetLabel is virtual and wxStaticText overrides it, so the
    // inherited property already reaches the right implementation.
    /// <summary>Inserts line breaks so the text wraps within <paramref name="width"/> pixels, following
    /// <c>wxStaticText::Wrap</c>. Pass -1 to undo a previous wrap.</summary>
    public void Wrap(int width) => NativeMethods.wxsharp_label_wrap(Handle, width);
    /// <summary>Whether the label is currently showing an ellipsis because the text did not fit, following
    /// <c>wxStaticText::IsEllipsized</c>. Only meaningful when created with an ellipsize style.</summary>
    public bool IsEllipsized() => NativeMethods.wxsharp_label_is_ellipsized(Handle);
}
