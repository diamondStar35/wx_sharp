using System;
using System.Collections.Generic;

namespace WxSharp;

public static class WindowId { public const int Any = -1; }

/// <summary>Common base for every native wx window, container, and control.</summary>
///
/// <remarks>
/// There is one event path. The typed <c>event</c> members on this class and on every control are shorthand
/// for <see cref="Bind{TEventArgs}"/>, and both end up in the same per-event subscriber list. An event type
/// is hooked natively the first time something subscribes to it on this window and unhooked when the last
/// subscriber goes away, so an event nothing is listening for never crosses the boundary.
///
/// Handling and propagation are wxWidgets'. An event is handled - and so stops - unless a handler calls
/// <see cref="WxEventArgs.Skip"/>; a skipped command event then travels up the real parent chain, so binding
/// <see cref="WxEvents.ButtonClicked"/> on a frame catches its buttons, exactly as in Phoenix. The wrapper
/// does not re-dispatch events to parents itself, and treats every event the same way.
/// </remarks>
public abstract class Window : IDisposable
{
    private readonly List<Window> _children = new();
    private readonly Dictionary<int, List<Subscription>> _subscriptions = new();
    private readonly Dictionary<int, EventArgsFactory> _factories = new();
    private long _nextBindingToken;
    private nint _handle;
    private bool _destroyed;
    private Accessible? _accessible;
    internal long Token { get; }
    internal App OwnerApp { get; }
    public int Id { get; private set; }
    public Window? Parent { get; }

    internal event Action? Invalidated;

    // ---- Events shared by every window ---------------------------------------------------------------

    /// <summary>Raised when the native window has been destroyed. The wrapper object is unusable afterwards.</summary>
    public event EventHandler<WxEventArgs> Destroyed
    {
        add => AddHandler(WxEvents.Destroyed, value);
        remove => RemoveHandler(WxEvents.Destroyed, value);
    }

    public event EventHandler<WxEventArgs> GotFocus
    {
        add => AddHandler(WxEvents.GotFocus, value);
        remove => RemoveHandler(WxEvents.GotFocus, value);
    }

    public event EventHandler<WxEventArgs> LostFocus
    {
        add => AddHandler(WxEvents.LostFocus, value);
        remove => RemoveHandler(WxEvents.LostFocus, value);
    }

    /// <summary>A key pressed while this control has focus. For an application-wide shortcut that must beat
    /// the focused control, use <see cref="CharHook"/> on the top-level window instead.</summary>
    public event EventHandler<KeyEventArgs> KeyDown
    {
        add => AddHandler(WxEvents.KeyDown, value);
        remove => RemoveHandler(WxEvents.KeyDown, value);
    }

    public event EventHandler<KeyEventArgs> KeyUp
    {
        add => AddHandler(WxEvents.KeyUp, value);
        remove => RemoveHandler(WxEvents.KeyUp, value);
    }

    /// <summary>Every key reaching this top-level window, before the focused control sees it. The key is
    /// consumed unless the handler calls <see cref="WxEventArgs.Skip"/>.</summary>
    public event EventHandler<KeyEventArgs> CharHook
    {
        add => AddHandler(WxEvents.CharHook, value);
        remove => RemoveHandler(WxEvents.CharHook, value);
    }

    /// <summary>The character a key produces, after the platform has translated it.</summary>
    public event EventHandler<KeyEventArgs> Char
    {
        add => AddHandler(WxEvents.Char, value);
        remove => RemoveHandler(WxEvents.Char, value);
    }

    public event EventHandler<MouseEventArgs> MouseDown
    {
        add => AddHandler(WxEvents.MouseDown, value);
        remove => RemoveHandler(WxEvents.MouseDown, value);
    }

    public event EventHandler<MouseEventArgs> MouseUp
    {
        add => AddHandler(WxEvents.MouseUp, value);
        remove => RemoveHandler(WxEvents.MouseUp, value);
    }

    public event EventHandler<MouseEventArgs> RightClick
    {
        add => AddHandler(WxEvents.RightDown, value);
        remove => RemoveHandler(WxEvents.RightDown, value);
    }

    public event EventHandler<MouseEventArgs> DoubleClick
    {
        add => AddHandler(WxEvents.DoubleClicked, value);
        remove => RemoveHandler(WxEvents.DoubleClicked, value);
    }

    public event EventHandler<MouseEventArgs> MouseEnter
    {
        add => AddHandler(WxEvents.MouseEntered, value);
        remove => RemoveHandler(WxEvents.MouseEntered, value);
    }

    public event EventHandler<MouseEventArgs> MouseLeave
    {
        add => AddHandler(WxEvents.MouseLeft, value);
        remove => RemoveHandler(WxEvents.MouseLeft, value);
    }

    public event EventHandler<MouseEventArgs> MouseMove
    {
        add => AddHandler(WxEvents.MouseMoved, value);
        remove => RemoveHandler(WxEvents.MouseMoved, value);
    }

    public event EventHandler<MouseEventArgs> MouseWheel
    {
        add => AddHandler(WxEvents.MouseWheel, value);
        remove => RemoveHandler(WxEvents.MouseWheel, value);
    }

    /// <summary>A context menu was requested here, by right-click or by the keyboard's menu key. Show one
    /// with <see cref="PopupMenu"/>.</summary>
    public event EventHandler<ContextMenuEventArgs> ContextMenu
    {
        add => AddHandler(WxEvents.ContextMenu, value);
        remove => RemoveHandler(WxEvents.ContextMenu, value);
    }

    public event EventHandler<SizeEventArgs> Resized
    {
        add => AddHandler(WxEvents.SizeChanged, value);
        remove => RemoveHandler(WxEvents.SizeChanged, value);
    }

    public event EventHandler<MoveEventArgs> Moved
    {
        add => AddHandler(WxEvents.Moved, value);
        remove => RemoveHandler(WxEvents.Moved, value);
    }

    /// <summary>Asked what state a command should be in, on idle and whenever a menu is about to open.
    /// Bind it with a command ID to answer for one command:
    /// <code>
    /// frame.Bind(WxEvents.UpdateUI, (_, e) =&gt; e.Enable(playlist.Count &gt; 0), playId);
    /// </code></summary>
    public event EventHandler<UpdateUIEventArgs> UpdateUI
    {
        add => AddHandler(WxEvents.UpdateUI, value);
        remove => RemoveHandler(WxEvents.UpdateUI, value);
    }

    /// <summary>The mouse capture was taken away. Handling this is mandatory for any window that calls
    /// <see cref="CaptureMouse"/>; wxWidgets asserts otherwise.</summary>
    public event EventHandler<WxEventArgs> MouseCaptureLost
    {
        add => AddHandler(WxEvents.MouseCaptureLost, value);
        remove => RemoveHandler(WxEvents.MouseCaptureLost, value);
    }

    /// <summary>Files were dragged onto this window. The window must be accepting them - see
    /// <see cref="DragAcceptFiles"/>.</summary>
    public event EventHandler<DropFilesEventArgs> FilesDropped
    {
        add => AddHandler(WxEvents.DropFiles, value);
        remove => RemoveHandler(WxEvents.DropFiles, value);
    }

    /// <summary>A registered system-wide hot key was pressed. See <see cref="RegisterHotKey"/>.</summary>
    public event EventHandler<KeyEventArgs> HotKeyPressed
    {
        add => AddHandler(WxEvents.HotKey, value);
        remove => RemoveHandler(WxEvents.HotKey, value);
    }

    // ---- Binding -------------------------------------------------------------------------------------

    /// <summary>Subscribes to <paramref name="eventType"/> on this window, optionally filtered to one command
    /// ID or an inclusive ID range. Dispose the returned binding to unsubscribe.</summary>
    public EventBinding Bind<TEventArgs>(EventType<TEventArgs> eventType, EventHandler<TEventArgs> handler,
        int id = WindowId.Any, int lastId = WindowId.Any) where TEventArgs : WxEventArgs
    {
        ArgumentNullException.ThrowIfNull(eventType);
        ArgumentNullException.ThrowIfNull(handler);
        Verify();
        if (lastId != WindowId.Any && id == WindowId.Any)
            throw new ArgumentException("An ID range requires a starting ID.", nameof(id));
        if (lastId != WindowId.Any && lastId < id)
            throw new ArgumentOutOfRangeException(nameof(lastId));

        var token = ++_nextBindingToken;
        Subscribe(eventType.EventId, eventType.Factory,
            new Subscription(token, id, lastId, handler, args => handler(args.Source, (TEventArgs)args)));
        return new EventBinding(this, eventType.EventId, token);
    }

    /// <summary>Removes a subscription added by <see cref="Bind{TEventArgs}"/> with the same event type,
    /// handler and ID filter. Returns false when no such subscription exists.</summary>
    public bool Unbind<TEventArgs>(EventType<TEventArgs> eventType, EventHandler<TEventArgs>? handler = null,
        int id = WindowId.Any, int lastId = WindowId.Any) where TEventArgs : WxEventArgs
    {
        ArgumentNullException.ThrowIfNull(eventType);
        Verify();
        if (!_subscriptions.TryGetValue(eventType.EventId, out var list)) return false;
        var index = list.FindIndex(entry => entry.Id == id && entry.LastId == lastId &&
            (handler is null || entry.Original.Equals(handler)));
        if (index < 0) return false;
        list.RemoveAt(index);
        ReleaseIfUnused(eventType.EventId, list);
        return true;
    }

    /// <summary>Backs a typed <c>event</c> accessor. Subscriptions added this way are removed by handler
    /// identity, which is what <c>-=</c> gives us.</summary>
    private protected void AddHandler<TEventArgs>(EventType<TEventArgs> eventType, EventHandler<TEventArgs> handler)
        where TEventArgs : WxEventArgs
    {
        ArgumentNullException.ThrowIfNull(handler);
        Verify();
        Subscribe(eventType.EventId, eventType.Factory,
            new Subscription(++_nextBindingToken, WindowId.Any, WindowId.Any, handler,
                args => handler(args.Source, (TEventArgs)args)));
    }

    private protected void RemoveHandler<TEventArgs>(EventType<TEventArgs> eventType, EventHandler<TEventArgs> handler)
        where TEventArgs : WxEventArgs
    {
        if (_destroyed || handler is null) return;
        if (!_subscriptions.TryGetValue(eventType.EventId, out var list)) return;
        var index = list.FindIndex(entry => entry.Original.Equals(handler));
        if (index < 0) return;
        list.RemoveAt(index);
        ReleaseIfUnused(eventType.EventId, list);
    }

    private void Subscribe(int eventId, EventArgsFactory factory, Subscription subscription)
    {
        if (!_subscriptions.TryGetValue(eventId, out var list))
        {
            list = new List<Subscription>();
            _subscriptions[eventId] = list;
            _factories[eventId] = factory;
            // The first subscriber is what hooks the event natively. A few events are reported whether or
            // not anyone asked, so they need no hook.
            if (!EventId.IsAlwaysReported(eventId) &&
                !NativeMethods.wxsharp_window_bind(_handle, eventId, Token))
            {
                _subscriptions.Remove(eventId);
                _factories.Remove(eventId);
                throw new NotSupportedException(
                    $"This window cannot report event {eventId}. Text-entry events, for example, require a " +
                    "control created with TextCtrlStyle.ProcessEnter.");
            }
        }
        list.Add(subscription);
    }

    private void ReleaseIfUnused(int eventId, List<Subscription> list)
    {
        if (list.Count != 0) return;
        _subscriptions.Remove(eventId);
        _factories.Remove(eventId);
        if (!EventId.IsAlwaysReported(eventId) && !_destroyed && _handle != 0)
            _ = NativeMethods.wxsharp_window_unbind(_handle, eventId);
    }

    internal void RemoveBinding(int eventId, long token)
    {
        if (_destroyed) return;
        OwnerApp.VerifyAccess();
        if (!_subscriptions.TryGetValue(eventId, out var list)) return;
        list.RemoveAll(entry => entry.Token == token);
        ReleaseIfUnused(eventId, list);
    }

    /// <summary>Delivers one native event to this window's subscribers. Returns the ABI result flags:
    /// bit 0 asks wxWidgets to skip the event, bit 1 vetoes it.</summary>
    internal uint Dispatch(in NativeEvent e)
    {
        // Nothing listening is the same as every handler skipping.
        if (!_subscriptions.TryGetValue(e.Kind, out var list) || list.Count == 0) return SkipResult;

        var args = _factories[e.Kind](this, in e);
        var skipped = true;
        // Copied so a handler may unsubscribe, or subscribe, while the event is being delivered.
        foreach (var subscription in list.ToArray())
        {
            if (!subscription.Matches(e.Id)) continue;
            // Each handler decides for itself, exactly as separate wxWidgets bindings would: the next one
            // runs only if this one skipped.
            args.ResetSkipped();
            subscription.Invoke(args);
            skipped = args.Skipped;
            if (!skipped) break;
        }
        return Result(args, skipped);
    }

    private const uint SkipResult = 1;
    private const uint VetoResult = 2;

    /// <summary>Delivers a synthesised event to this window's own subscribers without involving wxWidgets.
    /// For controls that must announce a change the native control stays silent about - a programmatic value
    /// change a screen reader still needs to hear, for instance.</summary>
    private protected uint RaiseLocal(in NativeEvent e) => Dispatch(in e);

    private static uint Result(WxEventArgs args, bool skipped)
    {
        var result = skipped ? SkipResult : 0u;
        if (args is NotifyEventArgs { IsAllowed: false } ||
            args is CloseEventArgs { Vetoed: true, CanVeto: true })
            result |= VetoResult;
        return result;
    }

    // ---- Construction and lifetime -------------------------------------------------------------------

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

    /// <summary>The custom accessible object attached to this window, following
    /// <c>wxWindow.GetAccessible</c> / <c>SetAccessible</c>. The window takes ownership of what is assigned.
    /// Throws <see cref="NotImplementedException"/> where wxWidgets was built without accessibility, which is
    /// what wxPython does there.</summary>
    public Accessible? Accessible
    {
        get { Verify(); RequireAccessibility(); return _accessible; }
        set
        {
            Verify();
            RequireAccessibility();
            _accessible?.Detach(this);
            _accessible = value;
            value?.Attach(this);
            NativeMethods.wxsharp_control_set_accessible(_handle, value?.Token ?? 0);
        }
    }

    /// <summary>Returns the accessible object, asking <see cref="CreateAccessible"/> for one if none has
    /// been set. Follows <c>wxWindow.GetOrCreateAccessible</c>; may return null, in which case the platform's
    /// own accessible is used.</summary>
    public Accessible? GetOrCreateAccessible()
    {
        Verify();
        RequireAccessibility();
        if (_accessible is null && CreateAccessible() is Accessible created)
            Accessible = created;
        return _accessible;
    }

    /// <summary>Override to supply an accessible object for this window. Returns null by default, as
    /// <c>wxWindow.CreateAccessible</c> does, leaving the platform's own accessible in place.</summary>
    protected virtual Accessible? CreateAccessible() => null;

    // ---- State and geometry --------------------------------------------------------------------------

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

    // ---- Update UI ---------------------------------------------------------------------------------

    /// <summary>Sends update-UI events to this window now, rather than waiting for the next idle cycle.
    /// Follows <c>wxWindow.UpdateWindowUI</c>.</summary>
    public void UpdateWindowUI(bool recurse = false)
    {
        Verify();
        NativeMethods.wxsharp_window_update_ui(_handle, recurse);
    }

    // ---- Dropped files and mouse capture -----------------------------------------------------------------

    /// <summary>Whether files dragged onto this window raise <see cref="FilesDropped"/>. Off until asked
    /// for. Follows <c>wxWindow.DragAcceptFiles</c>.</summary>
    public void DragAcceptFiles(bool accept = true)
    {
        Verify();
        NativeMethods.wxsharp_window_accept_dropped_files(_handle, accept);
    }

    /// <summary>Routes all mouse input to this window until <see cref="ReleaseMouse"/>. Anything that
    /// captures must also handle <see cref="MouseCaptureLost"/>: the capture can be taken away at any time,
    /// and wxWidgets asserts if nothing is listening.</summary>
    public void CaptureMouse() { Verify(); NativeMethods.wxsharp_window_capture_mouse(_handle); }

    /// <summary>Gives back a capture taken by <see cref="CaptureMouse"/>.</summary>
    public void ReleaseMouse() { Verify(); NativeMethods.wxsharp_window_release_mouse(_handle); }

    /// <summary>Whether this window currently holds the mouse capture.</summary>
    public bool HasCapture { get { Verify(); return NativeMethods.wxsharp_window_has_capture(_handle); } }

    // ---- System-wide hot keys ----------------------------------------------------------------------------

    /// <summary>Claims a key combination system-wide, so it reaches this window through
    /// <see cref="HotKeyPressed"/> even when the application is not focused. Returns false when another
    /// application already owns the combination, which is normal and worth reporting to the user rather than
    /// treating as an error.</summary>
    public bool RegisterHotKey(int hotKeyId, AcceleratorModifiers modifiers, int keyCode)
    {
        Verify();
        return NativeMethods.wxsharp_window_register_hotkey(_handle, hotKeyId, (int)modifiers, keyCode);
    }

    /// <summary>Releases a combination claimed by <see cref="RegisterHotKey"/>.</summary>
    public bool UnregisterHotKey(int hotKeyId)
    {
        Verify();
        return NativeMethods.wxsharp_window_unregister_hotkey(_handle, hotKeyId);
    }

    // ---- Menus and accelerators ----------------------------------------------------------------------

    /// <summary>Shows <paramref name="menu"/> at <paramref name="position"/> in client coordinates and returns once
    /// it is dismissed. Passing null uses the pointer's position, which is also right for a menu opened from
    /// the keyboard. Commands the menu produces arrive as <see cref="WxEvents.MenuCommand"/>.</summary>
    public bool PopupMenu(Menu menu, Point? position = null)
    {
        ArgumentNullException.ThrowIfNull(menu);
        Verify();
        var point = position ?? new Point(-1, -1);
        return NativeMethods.wxsharp_window_popup_menu(_handle, menu.Handle, point.X, point.Y);
    }

    /// <summary>Installs an accelerator table on this window, replacing any previous one. Passing no entries
    /// clears it. Accelerators work on dialogs as well as frames.</summary>
    public unsafe void SetAcceleratorTable(params AcceleratorEntry[] accelerators)
    {
        ArgumentNullException.ThrowIfNull(accelerators);
        Verify();
        if (accelerators.Length == 0)
        {
            NativeMethods.wxsharp_window_set_accelerators(_handle, null, 0);
            return;
        }
        var native = new NativeAccelerator[accelerators.Length];
        for (var i = 0; i < native.Length; ++i)
            native[i] = new NativeAccelerator
            {
                Modifiers = (int)accelerators[i].Modifiers,
                KeyCode = accelerators[i].KeyCode,
                CommandId = accelerators[i].CommandId,
            };
        fixed (NativeAccelerator* entries = native)
            NativeMethods.wxsharp_window_set_accelerators(_handle, entries, native.Length);
    }

    // ---- Accessibility -------------------------------------------------------------------------------

    /// <summary>The window's name, following <c>wxWindow.Name</c>. On platforms with an accessibility
    /// bridge this is what the window reports as its accessible name, so it is worth setting on a control
    /// whose own label does not describe it.</summary>
    public unsafe string Name
    {
        get
        {
            Verify();
            var length = NativeMethods.wxsharp_control_get_name(_handle, null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_control_get_name(_handle, p, buffer.Length);
            return Utf8String.Decode(buffer, length);
        }
        set { Verify(); NativeMethods.wxsharp_control_set_name(_handle, value ?? string.Empty); }
    }

    public void SetSizer(Sizer sizer)
    {
        ArgumentNullException.ThrowIfNull(sizer); Verify(); NativeMethods.wxsharp_window_set_sizer(_handle, sizer.Handle);
    }

    public virtual void Destroy()
    {
        if (_destroyed) return;
        Verify(); NativeMethods.wxsharp_control_destroy(_handle); Invalidate();
    }
    public void Dispose() { Destroy(); GC.SuppressFinalize(this); }

    internal void InvalidateFromAppShutdown() => Invalidate();
    internal void InvalidateFromNative() => Invalidate();
    internal void EnsureAlive() => ObjectDisposedException.ThrowIf(_destroyed || _handle == 0, this);
    private protected void Verify() { OwnerApp.VerifyAccess(); EnsureAlive(); }

    private void Invalidate()
    {
        if (_destroyed) return;
        foreach (var child in _children.ToArray()) child.Invalidate();
        _children.Clear(); Parent?._children.Remove(this); App.Unregister(Token);
        // The native side releases its own event sinks when the window is destroyed; this only drops the
        // managed subscriber lists.
        _subscriptions.Clear(); _factories.Clear();
        _accessible?.Detach(this); _accessible = null;
        Invalidated?.Invoke(); Invalidated = null;
        OwnerApp.NotifyWindowInvalidated(this);
        _destroyed = true; _handle = 0;
    }

    // wxPython raises NotImplementedError from the accessibility hooks where wxUSE_ACCESSIBILITY is off.
    private static void RequireAccessibility()
    {
        if (!Wx.SupportsCustomAccessibility)
            throw new NotImplementedException("wxWidgets was built without accessibility support on this platform.");
    }

    private sealed record Subscription(long Token, int Id, int LastId, Delegate Original, Action<WxEventArgs> Invoke)
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
