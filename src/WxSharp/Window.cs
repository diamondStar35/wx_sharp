using System;
using System.Collections.Generic;

namespace WxSharp;

public static class WindowId { public const int Any = -1; }

/// <summary>Common base for every native wx window, container, and control.</summary>
public abstract class Window : IDisposable
{
    private readonly List<Window> _children = new();
    private readonly List<BindingEntry> _bindings = new();
    private long _nextBindingToken;
    private nint _handle;
    private bool _destroyed;
    private Accessible? _accessible;
    internal long Token { get; }
    internal App OwnerApp { get; }
    public int Id { get; private set; }
    public Window? Parent { get; }

    public event EventHandler<WxEventArgs>? Destroyed;
    public event EventHandler<WxEventArgs>? GotFocus;
    public event EventHandler<WxEventArgs>? LostFocus;
    public event EventHandler<KeyEventArgs>? KeyDown;
    public event EventHandler<KeyEventArgs>? KeyUp;
    public event EventHandler<MouseEventArgs>? MouseDown;
    public event EventHandler<MouseEventArgs>? MouseUp;
    public event EventHandler<MouseEventArgs>? RightClick;
    public event EventHandler<MouseEventArgs>? DoubleClick;
    public event EventHandler<MouseEventArgs>? MouseEnter;
    public event EventHandler<MouseEventArgs>? MouseLeave;
    public event EventHandler<MouseEventArgs>? MouseMove;
    public event EventHandler<MouseEventArgs>? MouseWheel;
    public event EventHandler<CommandEventArgs>? Command;
    internal event Action? Invalidated;

    public Accessible? Accessible
    {
        get => _accessible;
        set
        {
            Verify();
            if (!Wx.SupportsCustomAccessibility && value is not null)
                throw new PlatformNotSupportedException("Custom wxAccessible objects are unavailable on this platform.");
            _accessible?.Detach(this);
            _accessible = value;
            value?.Attach(this);
            NativeMethods.wxsharp_control_set_accessible(_handle, value?.Token ?? 0);
        }
    }

    public EventBinding Bind<TEventArgs>(EventType<TEventArgs> eventType, EventHandler<TEventArgs> handler,
        int id = WindowId.Any, int lastId = WindowId.Any) where TEventArgs : WxEventArgs
    {
        ArgumentNullException.ThrowIfNull(eventType); ArgumentNullException.ThrowIfNull(handler); Verify();
        if (lastId != WindowId.Any && id == WindowId.Any)
            throw new ArgumentException("An ID range requires a starting ID.", nameof(id));
        if (lastId != WindowId.Any && lastId < id)
            throw new ArgumentOutOfRangeException(nameof(lastId));
        var token = ++_nextBindingToken;
        _bindings.Add(new BindingEntry(token, eventType.Kind, id, lastId, handler,
            args => handler(args.Source, (TEventArgs)args)));
        return new EventBinding(this, token);
    }

    public bool Unbind<TEventArgs>(EventType<TEventArgs> eventType, EventHandler<TEventArgs>? handler = null,
        int id = WindowId.Any, int lastId = WindowId.Any) where TEventArgs : WxEventArgs
    {
        ArgumentNullException.ThrowIfNull(eventType); Verify();
        // Delegate identity is deliberately not inferred from a recreated bound-method delegate. Use the
        // returned EventBinding for exact removal; this overload removes matching type/range registrations.
        var index = _bindings.FindIndex(entry => entry.Kind == eventType.Kind && entry.Id == id &&
            entry.LastId == lastId && (handler is null || entry.Original.Equals(handler)));
        if (index < 0) return false;
        _bindings.RemoveAt(index); return true;
    }

    protected Window(Window? parent, int id)
    {
        OwnerApp = App.Current ?? throw new InvalidOperationException("Create an App before creating windows or controls.");
        OwnerApp.VerifyAccess();
        Parent = parent;
        if (parent is not null)
        {
            parent.EnsureAlive();
            if (parent.OwnerApp != OwnerApp) throw new ArgumentException("Parent belongs to another App.", nameof(parent));
            parent._children.Add(this);
        }
        Id = id;
        Token = App.Register(this);
    }

    internal nint Handle { get { EnsureAlive(); return _handle; } }
    protected void Initialize(nint handle)
    {
        if (handle == 0) { App.Unregister(Token); throw new InvalidOperationException("wxWidgets failed to create the window."); }
        _handle = handle;
        Id = NativeMethods.wxsharp_control_get_id(handle);
    }
    protected void ApplyInitialGeometry(Point? position, Size? size)
    {
        if (position is Point p) Position = p;
        if (size is Size s) Size = s;
    }

    public bool Enabled
    {
        get { Verify(); return NativeMethods.wxsharp_control_is_enabled(_handle); }
        set { Verify(); NativeMethods.wxsharp_control_enable(_handle, value); }
    }
    public bool Visible
    {
        get { Verify(); return NativeMethods.wxsharp_control_is_shown(_handle); }
        set { Verify(); NativeMethods.wxsharp_control_show(_handle, value); }
    }
    public bool HasFocus { get { Verify(); return NativeMethods.wxsharp_control_has_focus(_handle); } }
    public void Show(bool show = true) { Verify(); NativeMethods.wxsharp_control_show(_handle, show); }
    public void Hide() => Show(false);
    public void Focus() { Verify(); NativeMethods.wxsharp_control_focus(_handle); }
    public void Layout() { Verify(); NativeMethods.wxsharp_control_layout(_handle); }

    public Size Size
    {
        get { Verify(); NativeMethods.wxsharp_control_get_size(_handle, out var w, out var h); return new Size(w, h); }
        set { Verify(); NativeMethods.wxsharp_control_set_size(_handle, value.Width, value.Height); }
    }
    public Size ClientSize
    {
        get { Verify(); NativeMethods.wxsharp_control_get_client_size(_handle, out var w, out var h); return new Size(w, h); }
    }
    public Point Position
    {
        get { Verify(); NativeMethods.wxsharp_control_get_position(_handle, out var x, out var y); return new Point(x, y); }
        set { Verify(); NativeMethods.wxsharp_control_set_position(_handle, value.X, value.Y); }
    }
    public Size MinSize { set { Verify(); NativeMethods.wxsharp_control_set_min_size(_handle, value.Width, value.Height); } }
    public Size MaxSize { set { Verify(); NativeMethods.wxsharp_control_set_max_size(_handle, value.Width, value.Height); } }
    public Size BestSize
    {
        get { Verify(); NativeMethods.wxsharp_control_get_best_size(_handle, out var w, out var h); return new Size(w, h); }
    }
    public Point MousePosition
    {
        get { Verify(); NativeMethods.wxsharp_control_get_pointer_position(_handle, out var x, out var y); return new Point(x, y); }
    }
    public void Fit() { Verify(); NativeMethods.wxsharp_control_fit(_handle); }
    public void Refresh(bool eraseBackground = true) { Verify(); NativeMethods.wxsharp_control_refresh(_handle, eraseBackground); }

    public Colour BackgroundColour
    {
        get { Verify(); return Colour.FromArgb(NativeMethods.wxsharp_control_get_background_colour(_handle)); }
        set { Verify(); NativeMethods.wxsharp_control_set_background_colour(_handle, value.ToArgb()); }
    }
    public Colour ForegroundColour
    {
        get { Verify(); return Colour.FromArgb(NativeMethods.wxsharp_control_get_foreground_colour(_handle)); }
        set { Verify(); NativeMethods.wxsharp_control_set_foreground_colour(_handle, value.ToArgb()); }
    }
    public string ToolTip { set { Verify(); NativeMethods.wxsharp_control_set_tooltip(_handle, value); } }
    public Border Border { set { Verify(); NativeMethods.wxsharp_control_set_border(_handle, (int)value); } }
    public void SetFont(Font font)
    {
        Verify(); NativeMethods.wxsharp_control_set_font(_handle, font.PointSize, (int)font.Family, (int)font.Weight,
            (int)font.Style, font.Underline, font.Face ?? string.Empty);
    }

    public string AccessibleName { set { Verify(); NativeMethods.wxsharp_control_set_name(_handle, value); } }
    public AccessibleRole AccessibleRole { set { EnsureAccessibility(); Verify(); NativeMethods.wxsharp_control_set_role(_handle, (int)value); } }
    public string AccessibleDescription { set { EnsureAccessibility(); Verify(); NativeMethods.wxsharp_control_set_description(_handle, value); } }
    public string AccessibleHelp { set { EnsureAccessibility(); Verify(); NativeMethods.wxsharp_control_set_help(_handle, value); } }
    public string AccessibleValue { set { EnsureAccessibility(); Verify(); NativeMethods.wxsharp_control_set_accessible_value(_handle, value); } }
    public string AccessibleKeyboardShortcut { set { EnsureAccessibility(); Verify(); NativeMethods.wxsharp_control_set_accessible_keyboard_shortcut(_handle, value); } }
    public string AccessibleDefaultAction { set { EnsureAccessibility(); Verify(); NativeMethods.wxsharp_control_set_accessible_default_action(_handle, value); } }
    public AccessibleState AccessibleState { set { EnsureAccessibility(); Verify(); NativeMethods.wxsharp_control_set_accessible_state(_handle, (uint)value); } }

    public void SetSizer(Sizer sizer)
    {
        ArgumentNullException.ThrowIfNull(sizer); Verify(); NativeMethods.wxsharp_window_set_sizer(_handle, sizer.Handle);
    }

    public virtual void Destroy()
    {
        if (_destroyed) return;
        Verify(); NativeMethods.wxsharp_control_destroy(_handle); Invalidate(true);
    }
    public void Dispose() { Destroy(); GC.SuppressFinalize(this); }

    internal virtual uint Dispatch(in NativeEvent e)
    {
        switch (e.Kind)
        {
            case EventKind.Destroyed: Invalidate(true); return 0;
            case EventKind.FocusGained: return Raise(new WxEventArgs(this, e.Id), GotFocus);
            case EventKind.FocusLost: return Raise(new WxEventArgs(this, e.Id), LostFocus);
            case EventKind.KeyHook or EventKind.KeyDown:
                var down = new KeyEventArgs(this, e); OnKeyDown(down); return down.Handled ? 1u : 0u;
            case EventKind.KeyUp:
                var up = new KeyEventArgs(this, e); OnKeyUp(up); return up.Handled ? 1u : 0u;
            case EventKind.MouseDown: return RaiseMouse(e, MouseDown);
            case EventKind.MouseUp: return RaiseMouse(e, MouseUp);
            case EventKind.MouseRight: return RaiseMouse(e, RightClick);
            case EventKind.MouseDouble: return RaiseMouse(e, DoubleClick);
            case EventKind.MouseEnter: return RaiseMouse(e, MouseEnter);
            case EventKind.MouseLeave: return RaiseMouse(e, MouseLeave);
            case EventKind.MouseMove: return RaiseMouse(e, MouseMove);
            case EventKind.MouseWheel: return RaiseMouse(e, MouseWheel);
            default: return 0;
        }
    }

    internal uint DispatchBindings(in NativeEvent e, Window? source = null)
    {
        if (_bindings.Count == 0) return 0;
        var args = (source ?? this).CreateEventArgs(in e);
        foreach (var binding in _bindings.ToArray())
        {
            if (binding.Kind != e.Kind || !binding.Matches(e.Id)) continue;
            binding.Handler(args);
            if (args.Handled) return EventResult(args);
        }
        return EventResult(args);
    }

    internal void RemoveBinding(long token)
    {
        if (_destroyed) return;
        Verify(); _bindings.RemoveAll(entry => entry.Token == token);
    }

    private WxEventArgs CreateEventArgs(in NativeEvent e) => e.Kind switch
    {
        EventKind.Close => new CloseEventArgs(this, e.Id, e.CanVeto != 0),
        EventKind.KeyHook or EventKind.KeyDown or EventKind.KeyUp => new KeyEventArgs(this, e),
        EventKind.MouseDown or EventKind.MouseUp or EventKind.MouseRight or EventKind.MouseDouble or
            EventKind.MouseEnter or EventKind.MouseLeave or EventKind.MouseMove or EventKind.MouseWheel => new MouseEventArgs(this, e),
        EventKind.Resize => new SizeEventArgs(this, e),
        EventKind.Move => new MoveEventArgs(this, e),
        EventKind.Activate or EventKind.Deactivate => new ActivateEventArgs(this, e),
        EventKind.Paint => new PaintEventArgs(this, e.Id),
        EventKind.Click or EventKind.Text or EventKind.Toggle or EventKind.Select or EventKind.Slider or EventKind.Menu or EventKind.Timer or
            EventKind.TextEnter => new CommandEventArgs(this, e.Id),
        _ => new WxEventArgs(this, e.Id),
    };

    private static uint EventResult(WxEventArgs args)
    {
        var result = args.Handled ? 1u : 0u;
        if (args is CloseEventArgs { Cancel: true, CanCancel: true }) result |= 2u;
        return result;
    }

    protected virtual void OnKeyDown(KeyEventArgs e) => KeyDown?.Invoke(this, e);
    protected virtual void OnKeyUp(KeyEventArgs e) => KeyUp?.Invoke(this, e);
    internal void InvalidateFromAppShutdown() => Invalidate(false);
    internal void EnsureAlive() => ObjectDisposedException.ThrowIf(_destroyed || _handle == 0, this);
    private void Verify() { OwnerApp.VerifyAccess(); EnsureAlive(); }
    private uint RaiseMouse(in NativeEvent e, EventHandler<MouseEventArgs>? handler)
    {
        var args = new MouseEventArgs(this, e); handler?.Invoke(this, args); return args.Handled ? 1u : 0u;
    }
    protected static uint Raise<T>(T args, EventHandler<T>? handler) where T : WxEventArgs
    {
        handler?.Invoke(args.Source, args); return args.Handled ? 1u : 0u;
    }
    protected uint RaiseCommand(CommandEventArgs args, EventHandler<CommandEventArgs>? handler)
    {
        handler?.Invoke(this, args);
        return PropagateCommand(args);
    }
    protected uint PropagateCommand(CommandEventArgs args)
    {
        for (var parent = Parent; !args.Handled && parent is not null; parent = parent.Parent)
            parent.Command?.Invoke(args.Source, args);
        return args.Handled ? 1u : 0u;
    }
    private void Invalidate(bool raiseEvent)
    {
        if (_destroyed) return;
        foreach (var child in _children.ToArray()) child.Invalidate(raiseEvent);
        _children.Clear(); Parent?._children.Remove(this); App.Unregister(Token);
        _bindings.Clear();
        _accessible?.Detach(this); _accessible = null;
        Invalidated?.Invoke(); Invalidated = null;
        OwnerApp.NotifyWindowInvalidated(this);
        _destroyed = true; _handle = 0;
        if (raiseEvent) Destroyed?.Invoke(this, new WxEventArgs(this, Id));
    }
    private static void EnsureAccessibility()
    {
        if (!Wx.SupportsCustomAccessibility) throw new PlatformNotSupportedException("Custom wxAccessible objects are unavailable on this platform.");
    }

    private sealed record BindingEntry(long Token, EventKind Kind, int Id, int LastId, Delegate Original,
        Action<WxEventArgs> Handler)
    {
        internal bool Matches(int eventId) => Id == WindowId.Any || eventId == Id ||
            (LastId != WindowId.Any && eventId >= Id && eventId <= LastId);
    }
}

/// <summary>Base class for standard controls.</summary>
public abstract class Control : Window
{
    protected Control(Window parent, int id) : base(parent, id) { }
}
