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
public abstract partial class Window : EvtHandler, IDisposable
{
    private readonly List<Window> _children = new();
    [ThreadStatic] private static VirtualMember _dispatchingVirtual;
    [ThreadStatic] private static nint _dispatchingWindowHandle;
    [ThreadStatic] private static bool _mainWindowBaseCalled;
    [ThreadStatic] private static nint _mainWindowBaseHandle;
    private protected nint _handle;
    private bool _destroyed;
    private Accessible? _accessible;
    private Sizer? _sizer;
    internal override App OwnerApp { get; }
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

    // ---- What EvtHandler needs from a window -----------------------------------------------------------

    private protected override bool BindNative(int eventId)
        => NativeMethods.wxsharp_window_bind(_handle, eventId, Token);

    private protected override void UnbindNative(int eventId)
    {
        if (_handle != 0) _ = NativeMethods.wxsharp_window_unbind(_handle, eventId);
    }

    private protected override bool IsDead => _destroyed || _handle == 0;

    private protected override void Verify() { OwnerApp.VerifyAccess(); EnsureAlive(); }

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
        if (handle == 0)
        {
            Parent?._children.Remove(this);
            App.Unregister(Token);
            throw new InvalidOperationException("wxWidgets failed to create the window.");
        }
        _handle = handle;
        App.MapHandle(handle, this);
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
    public bool HasFlag(int flag) { Verify(); return NativeMethods.wxsharp_control_has_flag(_handle, flag); }
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
        ArgumentNullException.ThrowIfNull(font);
        Verify();
        NativeMethods.wxsharp_control_set_font(_handle, font.Handle);
    }

    /// <summary>The font this window draws its text in, following <c>wxWindow.Font</c>. Reading it is what
    /// makes the usual adjustment possible - take the window's own font, embolden or resize it, and put it
    /// back - so a heading follows the user's chosen font and size rather than replacing them with a
    /// hard-coded one.</summary>
    public Font Font
    {
        get
        {
            Verify();
            // wxWidgets hands back a copy, so the caller owns it and should dispose it like any other font.
            return Font.Attach(NativeMethods.wxsharp_control_get_font(_handle));
        }
        set => SetFont(value);
    }

    // ---- Overridable wxWidgets virtuals ---------------------------------------------------------------

    // wxWidgets asks a window these questions by calling virtual members, and the answers decide real
    // behaviour: whether Tab stops here, how big a sizer makes this, where the client area starts, whether
    // a dialog may close. Each is overridable, and each base implementation is wxWidgets' own answer - so
    // overriding and calling base behave exactly as they do in C++.
    //
    // The set is the one wxPython supports (etgtools/tweaker_tools.py, addWindowVirtuals) rather than every
    // C++ virtual: wrapping all of them would cost code size for members no application overrides, and both
    // projects settled on the same subset. The few wxPython lists that are absent here need a type the
    // wrapper does not have yet, and are recorded in docs/phoenix-parity.md with the reason.
    //
    // Only a window whose most-derived type is a subclass is built with these hooks installed, so an exact
    // Button or Panel pays nothing.

    /// <summary>Whether this window can take the keyboard focus at all. Follows
    /// <c>wxWindow.AcceptsFocus</c>.</summary>
    public virtual bool AcceptsFocus() => BaseBool(VirtualMember.AcceptsFocus);

    /// <summary>Whether Tab should stop on this window. A control reachable another way - a transport
    /// button with a menu equivalent and a shortcut, say - can return false and stay clickable, which keeps
    /// it out of the tab order without hiding it. Follows <c>wxWindow.AcceptsFocusFromKeyboard</c>.</summary>
    public virtual bool AcceptsFocusFromKeyboard() => BaseBool(VirtualMember.AcceptsFocusFromKeyboard);

    /// <summary>Whether this window or any of its children can take focus. Follows
    /// <c>wxWindow.AcceptsFocusRecursively</c>.</summary>
    public virtual bool AcceptsFocusRecursively() => BaseBool(VirtualMember.AcceptsFocusRecursively);

    /// <summary>Validates this window's contents, as <c>wxWindow.Validate</c> does. wxWidgets calls it on a
    /// dialog before it closes with an affirmative result, and returning false keeps the dialog open.</summary>
    public virtual bool Validate() => BaseBool(VirtualMember.Validate);

    /// <summary>Moves data into this window's controls, as <c>wxWindow.TransferDataToWindow</c> does.
    /// wxWidgets calls it when a dialog is shown.</summary>
    public virtual bool TransferDataToWindow() => BaseBool(VirtualMember.TransferDataToWindow);

    /// <summary>Reads data back out of this window's controls, as
    /// <c>wxWindow.TransferDataFromWindow</c> does. wxWidgets calls it when a dialog closes.</summary>
    public virtual bool TransferDataFromWindow() => BaseBool(VirtualMember.TransferDataFromWindow);

    /// <summary>Prepares a dialog for display, which by default transfers data into its controls. Follows
    /// <c>wxWindow.InitDialog</c>.</summary>
    public virtual void InitDialog() => BaseVoid(VirtualMember.InitDialog);

    /// <summary>Where this window's client area starts, relative to its top-left corner. Non-zero for a
    /// window with decoration of its own. Follows <c>wxWindow.GetClientAreaOrigin</c>.</summary>
    public virtual Point GetClientAreaOrigin()
    {
        var request = CallBase(VirtualMember.ClientAreaOrigin);
        return new Point(request.X, request.Y);
    }

    /// <summary>Called when a child window is added. The child may be null when wxWidgets created it
    /// without the wrapper knowing. Follows <c>wxWindow.AddChild</c>.</summary>
    public virtual void AddChild(Window? child) => BaseWithWindow(VirtualMember.AddChild, child);

    /// <summary>Called when a child window is removed. Follows <c>wxWindow.RemoveChild</c>.</summary>
    public virtual void RemoveChild(Window? child) => BaseWithWindow(VirtualMember.RemoveChild, child);

    /// <summary>Applies the parent's font and colours to this window where it has none of its own. Follows
    /// <c>wxWindow.InheritAttributes</c>.</summary>
    public virtual void InheritAttributes() => BaseVoid(VirtualMember.InheritAttributes);

    /// <summary>Whether this window takes its colours from its parent. A control that paints itself
    /// returns false so a themed parent does not tint it. Follows
    /// <c>wxWindow.ShouldInheritColours</c>.</summary>
    public virtual bool ShouldInheritColours() => BaseBool(VirtualMember.ShouldInheritColours);

    /// <summary>Runs when the event queue is empty, before idle events. Follows
    /// <c>wxWindow.OnInternalIdle</c>; keep it cheap, because it runs often.</summary>
    public virtual void OnInternalIdle() => BaseVoid(VirtualMember.OnInternalIdle);

    /// <summary>For a control built out of several windows, the one that stands for the whole. Follows
    /// <c>wxWindow.GetMainWindowOfCompositeControl</c>.</summary>
    public virtual Window? GetMainWindowOfCompositeControl()
    {
        var request = CallBase(VirtualMember.MainWindowOfCompositeControl);
        if (_dispatchingVirtual == VirtualMember.MainWindowOfCompositeControl)
        {
            _mainWindowBaseCalled = true;
            _mainWindowBaseHandle = (nint)request.Handle;
        }
        return App.Lookup((nint)request.Handle);
    }

    /// <summary>Tells the window how much room there is on one axis before the other is decided, so a
    /// control whose height depends on its width can answer sensibly. Follows
    /// <c>wxWindow.InformFirstDirection</c>.</summary>
    public virtual bool InformFirstDirection(int direction, int size, int availableOtherDirection)
        => CallBase(VirtualMember.InformFirstDirection, direction, size, availableOtherDirection).Result != 0;

    /// <summary>Called by wxWidgets to record whether this window may hold focus. Follows
    /// <c>wxWindow.SetCanFocus</c>.</summary>
    public virtual void SetCanFocus(bool canFocus)
        => CallBase(VirtualMember.SetCanFocus, canFocus ? 1 : 0);

    /// <summary>Whether the window draws a visible focus indicator. Follows
    /// <c>wxWindow.EnableVisibleFocus</c>.</summary>
    public virtual void EnableVisibleFocus(bool enabled)
        => CallBase(VirtualMember.EnableVisibleFocus, enabled ? 1 : 0);

    /// <summary>The implementation behind <see cref="Enabled"/>. Follows <c>wxWindow.DoEnable</c>.</summary>
    protected virtual void DoEnable(bool enable) => CallBase(VirtualMember.DoEnable, enable ? 1 : 0);

    /// <summary>The implementation behind <see cref="Position"/>. Follows
    /// <c>wxWindow.DoGetPosition</c>.</summary>
    protected virtual Point DoGetPosition() => PointFrom(VirtualMember.DoGetPosition);

    /// <summary>The implementation behind <see cref="Size"/>. Follows <c>wxWindow.DoGetSize</c>.</summary>
    protected virtual Size DoGetSize() => SizeFrom(VirtualMember.DoGetSize);

    /// <summary>The implementation behind <see cref="ClientSize"/>. Follows
    /// <c>wxWindow.DoGetClientSize</c>.</summary>
    protected virtual Size DoGetClientSize() => SizeFrom(VirtualMember.DoGetClientSize);

    /// <summary>The size this window would like to be, which is what a sizer uses as its minimum. Follows
    /// <c>wxWindow.DoGetBestSize</c>; override it in a custom-drawn control that knows its own extent.</summary>
    protected virtual Size DoGetBestSize() => SizeFrom(VirtualMember.BestSize);

    /// <summary>The best size of the client area, without borders or scrollbars. Follows
    /// <c>wxWindow.DoGetBestClientSize</c>; overriding this rather than <see cref="DoGetBestSize"/> lets
    /// wxWidgets add the decoration for you.</summary>
    protected virtual Size DoGetBestClientSize() => SizeFrom(VirtualMember.BestClientSize);

    /// <summary>The implementation behind every resize. Follows <c>wxWindow.DoSetSize</c>.</summary>
    protected virtual void DoSetSize(int x, int y, int width, int height, int sizeFlags)
        => CallBase(VirtualMember.DoSetSize, x, y, width, height, sizeFlags);

    /// <summary>The implementation behind <see cref="ClientSize"/> being assigned. Follows
    /// <c>wxWindow.DoSetClientSize</c>.</summary>
    protected virtual void DoSetClientSize(int width, int height)
        => CallBase(VirtualMember.DoSetClientSize, width, height);

    /// <summary>The implementation behind the size hints. Follows
    /// <c>wxWindow.DoSetSizeHints</c>.</summary>
    protected virtual void DoSetSizeHints(int minWidth, int minHeight, int maxWidth, int maxHeight,
        int incrementWidth, int incrementHeight)
        => CallBase(VirtualMember.DoSetSizeHints, minWidth, minHeight, maxWidth, maxHeight, incrementWidth,
            incrementHeight);

    /// <summary>Moves and resizes the native window. Follows <c>wxWindow.DoMoveWindow</c>.</summary>
    protected virtual void DoMoveWindow(int x, int y, int width, int height)
        => CallBase(VirtualMember.DoMoveWindow, x, y, width, height);

    /// <summary>The implementation behind <see cref="Variant"/>. Follows
    /// <c>wxWindow.DoSetWindowVariant</c>.</summary>
    protected virtual void DoSetWindowVariant(WindowVariant variant)
        => CallBase(VirtualMember.DoSetWindowVariant, (int)variant);

    /// <summary>The border this window uses when none was asked for. Follows
    /// <c>wxWindow.GetDefaultBorder</c>.</summary>
    protected virtual Border GetDefaultBorder() => (Border)CallBase(VirtualMember.DefaultBorder).Result;

    /// <summary>The implementation behind <see cref="Freeze"/>. Follows <c>wxWindow.DoFreeze</c>.</summary>
    protected virtual void DoFreeze() => BaseVoid(VirtualMember.DoFreeze);

    /// <summary>The implementation behind <see cref="Thaw"/>. Follows <c>wxWindow.DoThaw</c>.</summary>
    protected virtual void DoThaw() => BaseVoid(VirtualMember.DoThaw);

    /// <summary>Whether the window's background shows what is behind it, which decides whether wxWidgets
    /// erases it. Follows <c>wxWindow.HasTransparentBackground</c>.</summary>
    protected virtual bool HasTransparentBackground()
        => BaseBool(VirtualMember.HasTransparentBackground);

    // ---- Reaching wxWidgets' own implementation --------------------------------------------------------

    private protected bool BaseBool(VirtualMember member) => CallBase(member).Result != 0;

    private protected void BaseVoid(VirtualMember member) => CallBase(member);

    private void BaseWithWindow(VirtualMember member, Window? window)
        => _ = CallBaseWithWindow(member, window);

    /// <summary>Runs wxWidgets' own implementation of a member that takes a window, and reports back.</summary>
    private protected unsafe NativeVirtualRequest CallBaseWithWindow(VirtualMember member, Window? window)
    {
        Verify();
        var request = NewRequest(member);
        request.Handle = window?.NativeHandleForLookup ?? 0;
        NativeMethods.wxsharp_window_call_base(_handle, &request);
        return request;
    }

    private Point PointFrom(VirtualMember member)
    {
        var request = CallBase(member);
        return new Point(request.X, request.Y);
    }

    private Size SizeFrom(VirtualMember member)
    {
        var request = CallBase(member);
        return new Size(request.X, request.Y);
    }

    // Runs wxWidgets' own implementation of one member. It never dispatches back to managed code, so an
    // override calling its base cannot re-enter itself - which going through the ordinary accessor would do,
    // because that accessor lands on the very virtual that is asking.
    private protected unsafe NativeVirtualRequest CallBase(VirtualMember member, params ReadOnlySpan<int> args)
    {
        Verify();
        var request = NewRequest(member);
        for (var i = 0; i < args.Length && i < 6; i++) request.Args[i] = args[i];
        NativeMethods.wxsharp_window_call_base(_handle, &request);
        return request;
    }

    /// <summary>Runs wxWidgets' own implementation of a member that takes a string.</summary>
    private protected unsafe NativeVirtualRequest CallBaseWithText(VirtualMember member, string text,
        params ReadOnlySpan<int> args)
    {
        Verify();
        var request = NewRequest(member);
        for (var i = 0; i < args.Length && i < 6; i++) request.Args[i] = args[i];
        var bytes = System.Text.Encoding.UTF8.GetBytes(text + " ");
        fixed (byte* buffer = bytes)
        {
            request.Text = buffer;
            NativeMethods.wxsharp_window_call_base(_handle, &request);
        }
        return request;
    }

    /// <summary>Reads a string argument the native side passed in. Valid only during the callback.</summary>
    private protected static unsafe string ReadText(in NativeVirtualRequest request)
        => request.Text is null ? string.Empty : Utf8String.DecodeNullTerminated(request.Text);

    private NativeVirtualRequest NewRequest(VirtualMember member)
    {
        unsafe
        {
            return new NativeVirtualRequest
            {
                Size = (uint)sizeof(NativeVirtualRequest),
                Version = 1,
                Token = Token,
                Which = (int)member,
            };
        }
    }

    // Answers one question from the native side.
    internal virtual bool TryAnswerVirtual(ref NativeVirtualRequest request)
    {
        // Nothing is answerable before Initialize() has run: the native window is still inside its own
        // constructor, so this managed object's constructor has not finished either and an override could
        // read a field it has not been given yet. Declining leaves wxWidgets to answer, which is right.
        if (_handle == 0) return false;

        var previousMember = _dispatchingVirtual;
        var previousHandle = _dispatchingWindowHandle;
        var previousMainCalled = _mainWindowBaseCalled;
        var previousMainHandle = _mainWindowBaseHandle;
        _dispatchingVirtual = (VirtualMember)request.Which;
        _dispatchingWindowHandle = (nint)request.Handle;
        _mainWindowBaseCalled = false;
        _mainWindowBaseHandle = 0;

        try
        {
            unsafe
            {
                switch ((VirtualMember)request.Which)
                {
                case VirtualMember.AcceptsFocus: request.Result = AcceptsFocus() ? 1 : 0; return true;
                case VirtualMember.AcceptsFocusFromKeyboard: request.Result = AcceptsFocusFromKeyboard() ? 1 : 0; return true;
                case VirtualMember.AcceptsFocusRecursively: request.Result = AcceptsFocusRecursively() ? 1 : 0; return true;
                case VirtualMember.Validate: request.Result = Validate() ? 1 : 0; return true;
                case VirtualMember.TransferDataToWindow: request.Result = TransferDataToWindow() ? 1 : 0; return true;
                case VirtualMember.TransferDataFromWindow: request.Result = TransferDataFromWindow() ? 1 : 0; return true;
                case VirtualMember.ShouldInheritColours: request.Result = ShouldInheritColours() ? 1 : 0; return true;
                case VirtualMember.HasTransparentBackground: request.Result = HasTransparentBackground() ? 1 : 0; return true;
                case VirtualMember.DefaultBorder: request.Result = (int)GetDefaultBorder(); return true;
                case VirtualMember.Destroy: request.Result = Destroy() ? 1 : 0; return true;

                case VirtualMember.InitDialog: InitDialog(); return true;
                case VirtualMember.InheritAttributes: InheritAttributes(); return true;
                case VirtualMember.OnInternalIdle: OnInternalIdle(); return true;
                case VirtualMember.DoFreeze: DoFreeze(); return true;
                case VirtualMember.DoThaw: DoThaw(); return true;

                case VirtualMember.ClientAreaOrigin:
                {
                    var origin = GetClientAreaOrigin();
                    return Fill(ref request, origin.X, origin.Y);
                }
                case VirtualMember.DoGetPosition:
                {
                    var position = DoGetPosition();
                    return Fill(ref request, position.X, position.Y);
                }
                case VirtualMember.DoGetSize:
                {
                    var size = DoGetSize();
                    return Fill(ref request, size.Width, size.Height);
                }
                case VirtualMember.DoGetClientSize:
                {
                    var size = DoGetClientSize();
                    return Fill(ref request, size.Width, size.Height);
                }
                case VirtualMember.BestSize:
                {
                    var size = DoGetBestSize();
                    return Fill(ref request, size.Width, size.Height);
                }
                case VirtualMember.BestClientSize:
                {
                    var size = DoGetBestClientSize();
                    return Fill(ref request, size.Width, size.Height);
                }

                case VirtualMember.AddChild:
                    AddChild(App.Lookup((nint)request.Handle));
                    return true;
                case VirtualMember.RemoveChild:
                    RemoveChild(App.Lookup((nint)request.Handle));
                    return true;
                case VirtualMember.MainWindowOfCompositeControl:
                {
                    var main = GetMainWindowOfCompositeControl();
                    request.Handle = main?.NativeHandleForLookup ??
                        (_mainWindowBaseCalled ? _mainWindowBaseHandle : 0);
                    return true;
                }

                case VirtualMember.InformFirstDirection:
                    request.Result = InformFirstDirection(request.Args[0], request.Args[1], request.Args[2]) ? 1 : 0;
                    return true;
                case VirtualMember.SetCanFocus: SetCanFocus(request.Args[0] != 0); return true;
                case VirtualMember.EnableVisibleFocus: EnableVisibleFocus(request.Args[0] != 0); return true;
                case VirtualMember.DoEnable: DoEnable(request.Args[0] != 0); return true;
                case VirtualMember.DoSetClientSize: DoSetClientSize(request.Args[0], request.Args[1]); return true;
                case VirtualMember.DoMoveWindow:
                    DoMoveWindow(request.Args[0], request.Args[1], request.Args[2], request.Args[3]);
                    return true;
                case VirtualMember.DoSetSize:
                    DoSetSize(request.Args[0], request.Args[1], request.Args[2], request.Args[3], request.Args[4]);
                    return true;
                case VirtualMember.DoSetSizeHints:
                    DoSetSizeHints(request.Args[0], request.Args[1], request.Args[2], request.Args[3],
                        request.Args[4], request.Args[5]);
                    return true;
                case VirtualMember.DoSetWindowVariant:
                    DoSetWindowVariant((WindowVariant)request.Args[0]);
                    return true;

                    default: return false;
                }
            }
        }
        finally
        {
            _dispatchingVirtual = previousMember;
            _dispatchingWindowHandle = previousHandle;
            _mainWindowBaseCalled = previousMainCalled;
            _mainWindowBaseHandle = previousMainHandle;
        }

        static bool Fill(ref NativeVirtualRequest request, int x, int y)
        {
            request.X = x;
            request.Y = y;
            return true;
        }
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

    /// <summary>Gives the window a sizer to lay its children out with, and lays them out once. The window
    /// takes ownership of the sizer.</summary>
    public void SetSizer(Sizer sizer)
    {
        ArgumentNullException.ThrowIfNull(sizer);
        Verify();
        NativeMethods.wxsharp_window_set_sizer(_handle, sizer.Handle);
        _sizer = sizer;
    }

    /// <summary>Gives the window a sizer and resizes the window to the size that sizer needs - the usual
    /// last line of a dialog's constructor. Follows <c>wxWindow.SetSizerAndFit</c>.</summary>
    public void SetSizerAndFit(Sizer sizer)
    {
        ArgumentNullException.ThrowIfNull(sizer);
        Verify();
        NativeMethods.wxsharp_window_set_sizer_and_fit(_handle, sizer.Handle);
        _sizer = sizer;
    }

    /// <summary>The sizer laying out this window's children, or null when it has none.</summary>
    public Sizer? GetSizer()
    {
        Verify();
        // The wrapper object is kept so callers get back the same instance they assigned; wxWidgets only
        // knows the native pointer.
        return NativeMethods.wxsharp_window_get_sizer(_handle) == 0 ? null : _sizer;
    }

    /// <summary>The sizer this window is an item of, or null when it is not in one. Follows
    /// <c>wxWindow.GetContainingSizer</c>.</summary>
    public Sizer? GetContainingSizer()
    {
        Verify();
        var handle = NativeMethods.wxsharp_window_containing_sizer(_handle);
        return handle == 0 ? null : Sizer.Attach(handle);
    }

    /// <summary>Schedules this window for deletion and reports whether wxWidgets accepted the request.
    /// This is virtual in wxWidgets and Phoenix, and native calls are forwarded to an override too.</summary>
    public virtual bool Destroy()
    {
        if (_destroyed) return false;
        var request = CallBase(VirtualMember.Destroy);
        if (request.Result == 0) return false;
        Invalidate();
        return true;
    }
    public void Dispose() { _ = Destroy(); GC.SuppressFinalize(this); }

    internal void InvalidateFromAppShutdown() => Invalidate();
    internal nint NativeHandleForLookup => _handle;

    internal void InvalidateFromNative() => Invalidate();
    internal void EnsureAlive() => ObjectDisposedException.ThrowIf(_destroyed || _handle == 0, this);

    private void Invalidate()
    {
        if (_destroyed) return;
        foreach (var child in _children.ToArray()) child.Invalidate();
        _children.Clear(); Parent?._children.Remove(this); App.Unregister(Token);
        // The native side releases its own event sinks when the window is destroyed; this only drops the
        // managed subscriber lists.
        ClearSubscriptions();
        _accessible?.Detach(this); _accessible = null;
        _sizer = null;
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

}

/// <summary>Base class for standard controls.</summary>
public abstract class Control : Window
{
    protected Control(Window parent, int id) : base(parent, id) { }
}
