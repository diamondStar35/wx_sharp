using System;
using System.Collections.Generic;

namespace WxSharp;

/// <summary>Base for anything backed by a native widget that can receive events. It registers for an id the
/// shim reports back, and holds the opaque native handle.</summary>
public abstract class EventTarget
{
    internal int Id { get; }
    internal nint Handle { get; private set; }

    protected EventTarget() => Id = Wx.Register(this);

    private protected void Attach(nint handle) => Handle = handle;

    internal virtual void OnNativeEvent(EventKind evt) { }
}

/// <summary>Base for the leaf controls (buttons, fields, …): adds the operations, geometry, appearance,
/// accessibility, focus and keyboard events common to every widget. Not sealed - subclass it (or a concrete
/// control) to build custom controls: override <see cref="OnKeyDown"/>/<see cref="OnGotFocus"/> and the other
/// protected hooks to add behaviour. Built-in controls override the internal <see cref="OnEvent"/> for their
/// own command events.</summary>
public abstract class Control : EventTarget
{
    private Container? _parent;

    /// <summary>Raised when this control gains keyboard focus.</summary>
    public event Action? GotFocus;
    /// <summary>Raised when this control loses keyboard focus.</summary>
    public event Action? LostFocus;
    /// <summary>Raised when a key is pressed while this control has focus. Set
    /// <see cref="KeyEventArgs.Handled"/> to consume it (stopping the control's own handling).</summary>
    public event Action<KeyEventArgs>? KeyDown;
    /// <summary>Raised when a key is released while this control has focus.</summary>
    public event Action<KeyEventArgs>? KeyUp;
    /// <summary>Raised when the left mouse button is pressed on the control.</summary>
    public event Action? MouseDown;
    /// <summary>Raised when the left mouse button is released on the control.</summary>
    public event Action? MouseUp;
    /// <summary>Raised when the right mouse button is pressed on the control (a context click).</summary>
    public event Action? RightClick;
    /// <summary>Raised when the control is double-clicked with the left button.</summary>
    public event Action? DoubleClick;
    /// <summary>Raised when the pointer enters the control.</summary>
    public event Action? MouseEnter;
    /// <summary>Raised when the pointer leaves the control.</summary>
    public event Action? MouseLeave;
    /// <summary>Raised when the pointer moves over the control. The event carries no coordinates - read
    /// <see cref="MousePosition"/> to get the current pointer location (e.g. for hover hit-testing).</summary>
    public event Action? MouseMove;
    /// <summary>Raised when the mouse wheel is scrolled up/away over the control.</summary>
    public event Action? MouseWheelUp;
    /// <summary>Raised when the mouse wheel is scrolled down/toward over the control.</summary>
    public event Action? MouseWheelDown;

    // Attaches the native handle and registers the control with its parent for layout + cleanup.
    private protected void Init(Container parent, nint handle)
    {
        Attach(handle);
        _parent = parent;
        parent.Track(this);
    }

    /// <summary>Enables or disables the control.</summary>
    public bool Enabled { set => NativeMethods.wxsharp_control_enable(Handle, value); }

    /// <summary>Shows or hides the control.</summary>
    public bool Visible { set => NativeMethods.wxsharp_control_show(Handle, value); }

    /// <summary>Gives this control keyboard focus.</summary>
    public void Focus() => NativeMethods.wxsharp_control_focus(Handle);

    // ---- Geometry ----------------------------------------------------------------------------------------

    /// <summary>The control's outer size in pixels.</summary>
    public Size Size
    {
        get { NativeMethods.wxsharp_control_get_size(Handle, out var w, out var h); return new Size(w, h); }
        set => NativeMethods.wxsharp_control_set_size(Handle, value.Width, value.Height);
    }

    /// <summary>The size of the control's usable client area (inside any border).</summary>
    public Size ClientSize
    {
        get { NativeMethods.wxsharp_control_get_client_size(Handle, out var w, out var h); return new Size(w, h); }
    }

    /// <summary>The control's position relative to its parent's client area.</summary>
    public Point Position
    {
        get { NativeMethods.wxsharp_control_get_position(Handle, out var x, out var y); return new Point(x, y); }
        set => NativeMethods.wxsharp_control_set_position(Handle, value.X, value.Y);
    }

    /// <summary>A lower bound on the control's size (the sizer won't shrink it below this).</summary>
    public Size MinSize { set => NativeMethods.wxsharp_control_set_min_size(Handle, value.Width, value.Height); }

    /// <summary>An upper bound on the control's size.</summary>
    public Size MaxSize { set => NativeMethods.wxsharp_control_set_max_size(Handle, value.Width, value.Height); }

    /// <summary>The size the control would like to be, based on its content.</summary>
    public Size BestSize
    {
        get { NativeMethods.wxsharp_control_get_best_size(Handle, out var w, out var h); return new Size(w, h); }
    }

    /// <summary>Resizes the control to fit its content (its best size).</summary>
    public void Fit() => NativeMethods.wxsharp_control_fit(Handle);

    /// <summary>The mouse pointer's current position in this control's client coordinates. Pair with
    /// <see cref="MouseMove"/> to hit-test hovered content on a canvas.</summary>
    public Point MousePosition
    {
        get { NativeMethods.wxsharp_control_get_pointer_position(Handle, out var x, out var y); return new Point(x, y); }
    }

    // ---- Appearance --------------------------------------------------------------------------------------

    /// <summary>The control's background colour.</summary>
    public Color BackgroundColour
    {
        get => Color.FromArgb(NativeMethods.wxsharp_control_get_background_colour(Handle));
        set => NativeMethods.wxsharp_control_set_background_colour(Handle, value.ToArgb());
    }

    /// <summary>The control's foreground (text) colour.</summary>
    public Color ForegroundColour
    {
        get => Color.FromArgb(NativeMethods.wxsharp_control_get_foreground_colour(Handle));
        set => NativeMethods.wxsharp_control_set_foreground_colour(Handle, value.ToArgb());
    }

    /// <summary>The hover tooltip shown for this control.</summary>
    public string ToolTip { set => NativeMethods.wxsharp_control_set_tooltip(Handle, value); }

    /// <summary>The border drawn around the control.</summary>
    public Border Border { set => NativeMethods.wxsharp_control_set_border(Handle, (int)value); }

    /// <summary>Applies a font to the control.</summary>
    public void SetFont(Font font)
        => NativeMethods.wxsharp_control_set_font(Handle, font.PointSize, (int)font.Family, (int)font.Weight,
            (int)font.Style, font.Underline, font.Face ?? string.Empty);

    /// <summary>Forces a repaint of the control.</summary>
    public void Refresh(bool eraseBackground = true) => NativeMethods.wxsharp_control_refresh(Handle, eraseBackground);

    // ---- State -------------------------------------------------------------------------------------------

    /// <summary>Whether the control is currently enabled (accounts for a disabled ancestor).</summary>
    public bool IsEnabled => NativeMethods.wxsharp_control_is_enabled(Handle);

    /// <summary>Whether the control is currently shown (accounts for a hidden ancestor).</summary>
    public bool IsShown => NativeMethods.wxsharp_control_is_shown(Handle);

    /// <summary>Whether the control currently has keyboard focus.</summary>
    public bool HasFocus => NativeMethods.wxsharp_control_has_focus(Handle);

    /// <summary>The name assistive technology announces for this control. wxWidgets uses a control's window
    /// name as its accessible name, which for some controls (check boxes, radio buttons) defaults to a class
    /// string rather than the label — set this to the label so screen readers read the real text.</summary>
    public string AccessibleName { set => NativeMethods.wxsharp_control_set_name(Handle, value); }

    /// <summary>Overrides the role assistive technology reports for this control (e.g. to announce a
    /// custom-drawn surface as a slider). <see cref="AccessibleRole.Default"/> keeps wx's own role.</summary>
    public AccessibleRole AccessibleRole
    {
        set { EnsureCustomAccessibility(); NativeMethods.wxsharp_control_set_role(Handle, (int)value); }
    }

    /// <summary>A longer description a screen reader can read after the name (the control's purpose).</summary>
    public string AccessibleDescription
    {
        set { EnsureCustomAccessibility(); NativeMethods.wxsharp_control_set_description(Handle, value); }
    }

    /// <summary>Help text (a hint/tooltip) assistive technology can surface for this control.</summary>
    public string AccessibleHelp
    {
        set { EnsureCustomAccessibility(); NativeMethods.wxsharp_control_set_help(Handle, value); }
    }

    /// <summary>The value assistive technology reports for this control, for surfaces where wx has none to
    /// infer (e.g. a custom canvas exposing a reading).</summary>
    public string AccessibleValue
    {
        set { EnsureCustomAccessibility(); NativeMethods.wxsharp_control_set_accessible_value(Handle, value); }
    }

    /// <summary>The keyboard shortcut assistive technology reports, such as <c>Alt+K</c>.</summary>
    public string AccessibleKeyboardShortcut
    {
        set { EnsureCustomAccessibility(); NativeMethods.wxsharp_control_set_accessible_keyboard_shortcut(Handle, value); }
    }

    /// <summary>The localized name of the control's default action, such as <c>Press</c>.</summary>
    public string AccessibleDefaultAction
    {
        set { EnsureCustomAccessibility(); NativeMethods.wxsharp_control_set_accessible_default_action(Handle, value); }
    }

    /// <summary>Overrides the state flags reported by a Phoenix-compatible custom accessible object.</summary>
    public AccessibleState AccessibleState
    {
        set { EnsureCustomAccessibility(); NativeMethods.wxsharp_control_set_accessible_state(Handle, (uint)value); }
    }

    private static void EnsureCustomAccessibility()
    {
        if (!Wx.SupportsCustomAccessibility)
            throw new PlatformNotSupportedException(
                "Custom wxAccessible objects are not implemented by wxWidgets on this platform. " +
                "Standard controls still use the platform's native accessibility bridge.");
    }

    /// <summary>Destroys the native control and releases it. Use for create-on-demand controls (e.g. an
    /// inline edit box that should not exist until a prompt needs it).</summary>
    public void Destroy()
    {
        NativeMethods.wxsharp_control_destroy(Handle);
        _parent?.Untrack(this);
        Wx.Unregister(Id);
    }

    // Routes the shared focus events here, then hands the rest to the concrete control. Sealed so every
    // control gets uniform focus handling; derived types override OnEvent (built-in) or the protected hooks.
    internal sealed override void OnNativeEvent(EventKind evt)
    {
        switch (evt)
        {
            case EventKind.FocusGained: OnGotFocus(); break;
            case EventKind.FocusLost: OnLostFocus(); break;
            case EventKind.MouseDown: OnMouseDown(); break;
            case EventKind.MouseUp: OnMouseUp(); break;
            case EventKind.MouseRight: OnRightClick(); break;
            case EventKind.MouseDouble: OnDoubleClick(); break;
            case EventKind.MouseEnter: OnMouseEnter(); break;
            case EventKind.MouseLeave: OnMouseLeave(); break;
            case EventKind.MouseMove: OnMouseMove(); break;
            case EventKind.WheelUp: OnMouseWheelUp(); break;
            case EventKind.WheelDown: OnMouseWheelDown(); break;
            default: OnEvent(evt); break;
        }
    }

    /// <summary>Handles the control-specific command events; the base already dispatches focus, keys and mouse.
    /// Built-in controls override this to raise their own events.</summary>
    private protected virtual void OnEvent(EventKind evt) { }

    // Called by the dispatcher for a key down/up on this control; returns whether the key was consumed.
    internal bool RaiseKeyDown(KeyEventArgs args) { OnKeyDown(args); return args.Handled; }
    internal bool RaiseKeyUp(KeyEventArgs args) { OnKeyUp(args); return args.Handled; }

    /// <summary>Raises <see cref="GotFocus"/>. Override to react to focus in a custom control (call base to
    /// keep the event firing).</summary>
    protected virtual void OnGotFocus() => GotFocus?.Invoke();

    /// <summary>Raises <see cref="LostFocus"/>.</summary>
    protected virtual void OnLostFocus() => LostFocus?.Invoke();

    /// <summary>Raises <see cref="KeyDown"/>. Override to handle keys in a custom control (e.g. the managed
    /// slider); set <see cref="KeyEventArgs.Handled"/> to consume the key.</summary>
    protected virtual void OnKeyDown(KeyEventArgs e) => KeyDown?.Invoke(e);

    /// <summary>Raises <see cref="KeyUp"/>.</summary>
    protected virtual void OnKeyUp(KeyEventArgs e) => KeyUp?.Invoke(e);

    /// <summary>Raises <see cref="MouseDown"/>.</summary>
    protected virtual void OnMouseDown() => MouseDown?.Invoke();

    /// <summary>Raises <see cref="MouseUp"/>.</summary>
    protected virtual void OnMouseUp() => MouseUp?.Invoke();

    /// <summary>Raises <see cref="RightClick"/>.</summary>
    protected virtual void OnRightClick() => RightClick?.Invoke();

    /// <summary>Raises <see cref="DoubleClick"/>.</summary>
    protected virtual void OnDoubleClick() => DoubleClick?.Invoke();

    /// <summary>Raises <see cref="MouseEnter"/>.</summary>
    protected virtual void OnMouseEnter() => MouseEnter?.Invoke();

    /// <summary>Raises <see cref="MouseLeave"/>.</summary>
    protected virtual void OnMouseLeave() => MouseLeave?.Invoke();

    /// <summary>Raises <see cref="MouseMove"/>.</summary>
    protected virtual void OnMouseMove() => MouseMove?.Invoke();

    /// <summary>Raises <see cref="MouseWheelUp"/>.</summary>
    protected virtual void OnMouseWheelUp() => MouseWheelUp?.Invoke();

    /// <summary>Raises <see cref="MouseWheelDown"/>.</summary>
    protected virtual void OnMouseWheelDown() => MouseWheelDown?.Invoke();
}

/// <summary>Base for surfaces that hold controls (windows, dialogs, layout panels): exposes the content panel
/// controls are created against, a key hook, and ownership of its children for registry cleanup.</summary>
public abstract class Container : EventTarget
{
    private readonly List<EventTarget> _children = new();
    private bool _cleaned;

    internal nint Panel { get; private set; }

    /// <summary>Raised for every key pressed while this surface is active. Set <see cref="KeyEventArgs.Handled"/>
    /// to consume the key; leave it unset to let normal processing (tab nav, typing, Esc-to-close) proceed.</summary>
    public event Action<KeyEventArgs>? KeyDown;

    private protected void AttachContainer(nint handle, nint panel)
    {
        Attach(handle);
        Panel = panel;
    }

    /// <summary>Replaces this surface's default vertical stack with an explicit <see cref="Sizer"/>, for
    /// custom layouts. Add controls to the sizer, then set it here; the content re-flows to match.</summary>
    public void SetSizer(Sizer sizer) => NativeMethods.wxsharp_window_set_sizer(Panel, sizer.Handle);

    internal void Track(EventTarget child) => _children.Add(child);

    internal void Untrack(EventTarget child) => _children.Remove(child);

    internal bool RaiseKeyDown(KeyEventArgs args)
    {
        KeyDown?.Invoke(args);
        return args.Handled;
    }

    // Releases this surface's registry id, and its children's, recursively (sub-panels clean their own). The
    // native widgets are freed by wx when the top-level is destroyed; this just stops the managed map leaking.
    private protected void Cleanup()
    {
        if (_cleaned)
            return;
        _cleaned = true;
        foreach (var child in _children)
        {
            if (child is Container container)
                container.Cleanup();
            else
                Wx.Unregister(child.Id);
        }
        Wx.Unregister(Id);
    }
}
