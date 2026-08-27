using System;

namespace WxSharp;

/// <summary>A push button.</summary>
public class Button : Control
{
    public event EventHandler<CommandEventArgs> Click
    {
        add => AddHandler(WxEvents.ButtonClicked, value);
        remove => RemoveHandler(WxEvents.ButtonClicked, value);
    }

    public Button(Window parent, int id = WindowId.Any, string label = "", Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_button_create(parent.Handle, id, label, Token));
        ApplyInitialGeometry(position, size);
    }

    /// <summary>Makes this the default button, so pressing Enter activates it (e.g. a dialog's OK).</summary>
    public void SetDefault() => NativeMethods.wxsharp_button_set_default(Handle);

    // The button's text is Window.Label: wxWindow::SetLabel is virtual and wxButton overrides it, so the
    // inherited property already reaches the right implementation.
}
