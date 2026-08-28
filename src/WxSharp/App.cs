using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace WxSharp;

/// <summary>Owns one wxWidgets application and its blocking native event loop.</summary>
/// <summary>Which interface appearance an application asks the platform for, following
/// <c>wxApp.Appearance</c>.</summary>
public enum Appearance
{
    /// <summary>Follow whatever the user has chosen.</summary>
    System = 0,
    Light = 1,
    Dark = 2,
}

/// <summary>What the platform did with a request for an appearance, following
/// <c>wxApp.AppearanceResult</c>. The two failures are worth telling apart: one means the platform cannot
/// do it at all, the other that it is too late to ask.</summary>
public enum AppearanceResult
{
    /// <summary>The platform does not support choosing an appearance.</summary>
    Failure = 0,
    /// <summary>The appearance was applied.</summary>
    Ok = 1,
    /// <summary>Supported, but not once the application has started - ask before creating a window.</summary>
    CannotChange = 2,
}

/// <summary>How far <see cref="App.EnableDarkMode"/> should go, following the <c>wxApp.DarkMode_</c>
/// flags.</summary>
public enum DarkMode
{
    /// <summary>Use dark mode when the system is using it.</summary>
    Auto = 0,
    /// <summary>Use dark mode whatever the system setting says.</summary>
    Always = 1,
}

public class App : EvtHandler, IDisposable
{
    private static readonly Dictionary<long, EvtHandler> Handlers = new();
    private static readonly Dictionary<nint, Window> ByHandle = new();
    private static readonly Dictionary<long, Action> DeferredActions = new();
    private static long _nextToken;
    private readonly int _threadId;
    private readonly object _lifecycleGate = new();
    private bool _running, _hasRun, _disposed, _onExitCalled;
    private ExceptionDispatchInfo? _callbackException;
    private Window? _topWindow;
    private bool _exitOnFrameDelete = true;

    public static App? Current { get; private set; }

    public App()
    {
        if (Current is not null) throw new InvalidOperationException("Only one App may exist at a time.");
        RequireSingleThreadedApartment();
        _threadId = Environment.CurrentManagedThreadId;
        if (!NativeMethods.wxsharp_init()) throw new InvalidOperationException("wxWidgets initialization failed.");
        unsafe { NativeMethods.wxsharp_set_event_handler(&Dispatch); }
        unsafe { NativeMethods.wxsharp_set_accessible_handler(&Accessible.Dispatch); }
        unsafe { NativeMethods.wxsharp_set_virtual_list_handler(&DispatchVirtualList); }
        unsafe { NativeMethods.wxsharp_set_virtual_handler(&DispatchVirtual); }
        Current = this;
        // Registered like any other handler so an app-level event resolves back through the same token map.
        Token = Register(this);
        NativeMethods.wxsharp_set_exit_on_frame_delete(true);
    }

    // Windows GUI toolkits run in a single-threaded apartment; wxWidgets brings OLE up during
    // initialization and needs one too. .NET starts Main in a multi-threaded apartment unless it is marked,
    // and the apartment cannot be changed once the thread is running - so this has to be caught here, where
    // the remedy can be named, rather than later as an unexplained clipboard or drag-and-drop failure.
    private static void RequireSingleThreadedApartment()
    {
        if (!OperatingSystem.IsWindows()) return;
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) return;
        throw new InvalidOperationException(
            "A WxSharp application must run on a single-threaded apartment thread. Mark the entry point " +
            "with [STAThread], which requires an explicit Main method rather than top-level statements.");
    }

    public bool ExitOnFrameDelete
    {
        get => _exitOnFrameDelete;
        set { VerifyAccess(); ThrowIfDisposed(); _exitOnFrameDelete = value; NativeMethods.wxsharp_set_exit_on_frame_delete(value); }
    }


    // ---- Appearance -------------------------------------------------------------------------------------

    /// <summary>Asks for a light or dark interface, following <c>wxApp.SetAppearance</c>. This is the
    /// portable request: it tells the platform which appearance the application wants, and the platform
    /// decides how far that reaches.</summary>
    ///
    /// <remarks>
    /// Ask before creating any window. Several platforms only honour the request while the application is
    /// starting and answer <see cref="AppearanceResult.CannotChange"/> afterwards, which is a different
    /// answer from <see cref="AppearanceResult.Failure"/> and worth acting on differently: the first means
    /// "too late", the second means "not supported here".
    ///
    /// On Windows this themes the window frame but leaves the controls wxWidgets draws itself alone; see
    /// <see cref="EnableDarkMode"/> for the fuller treatment.
    /// </remarks>
    public AppearanceResult SetAppearance(Appearance appearance)
    {
        VerifyAccess();
        ThrowIfDisposed();
        return (AppearanceResult)NativeMethods.wxsharp_app_set_appearance((int)appearance);
    }

    /// <summary>Turns on Windows' own dark mode, following <c>wxApp.MSWEnableDarkMode</c>. It goes further
    /// than <see cref="SetAppearance"/>: the controls wxWidgets draws itself are themed too, not just the
    /// window frame.</summary>
    ///
    /// <remarks>
    /// Call it before creating any window. Returns false where it could not be enabled, and where the
    /// platform has no such thing - everywhere but Windows, which <see cref="SupportsDarkMode"/> reports
    /// separately so a caller can tell the two apart.
    ///
    /// wxWidgets still calls this experimental, so treat a false as normal rather than as an error, and
    /// leave the interface working either way. Reading <see cref="SystemSettings.IsDarkAppearance"/> is how
    /// to find out what the user actually ended up with; the colours an interface draws with should come
    /// from <see cref="SystemSettings.GetColour"/> rather than being chosen for a mode.
    /// </remarks>
    /// <param name="mode">Whether to follow the system setting or force dark regardless.</param>
    public bool EnableDarkMode(DarkMode mode = DarkMode.Auto)
    {
        VerifyAccess();
        ThrowIfDisposed();
        return NativeMethods.wxsharp_app_enable_dark_mode((int)mode);
    }

    /// <summary>Whether this platform has the dark mode <see cref="EnableDarkMode"/> turns on. True on
    /// Windows only, which is what wxWidgets implements it for.</summary>
    public static bool SupportsDarkMode => NativeMethods.wxsharp_app_supports_dark_mode();

    public Window? TopWindow
    {
        get => _topWindow;
        set
        {
            VerifyAccess(); ThrowIfDisposed();
            if (value is not null && value.OwnerApp != this) throw new ArgumentException("The top window must belong to this App.", nameof(value));
            _topWindow = value;
            NativeMethods.wxsharp_set_top_window(value?.Handle ?? 0);
        }
    }

    protected virtual bool OnInit() => true;
    protected virtual int OnExit() => 0;

    public int MainLoop()
    {
        VerifyAccess(); ThrowIfDisposed();
        if (_hasRun) throw new InvalidOperationException("An App event loop can only run once.");
        _hasRun = true;
        var result = 0;
        ExceptionDispatchInfo? directException = null;
        try
        {
            if (OnInit() && _callbackException is null)
            {
                _running = true;
                result = NativeMethods.wxsharp_main_loop();
            }
        }
        catch (Exception ex) { directException = ExceptionDispatchInfo.Capture(ex); }
        finally
        {
            _running = false;
            try { RunOnExit(); }
            catch (Exception ex) { directException ??= ExceptionDispatchInfo.Capture(ex); }
            Shutdown();
        }
        directException?.Throw();
        _callbackException?.Throw();
        return result;
    }

    public void ExitMainLoop()
    {
        VerifyAccess(); ThrowIfDisposed();
        if (_running) NativeMethods.wxsharp_exit_main_loop();
    }

    public void Dispose()
    {
        if (_disposed) return;
        VerifyAccess();
        if (_running) { NativeMethods.wxsharp_exit_main_loop(); return; }
        Shutdown();
        GC.SuppressFinalize(this);
    }

    internal void VerifyAccess()
    {
        if (Environment.CurrentManagedThreadId != _threadId)
            throw new InvalidOperationException("wxWidgets objects may only be used on the App UI thread. Use Wx.CallAfter from worker threads.");
    }

    // ---- What EvtHandler needs from the application ----------------------------------------------------
    // wxApp is a wxEvtHandler like any window, so the machinery is shared; only the bind target differs.
    // The events that reach it - the application being activated, the session ending - are never sent to a
    // window, which is why they need a target of their own.

    internal override App OwnerApp => this;

    private protected override bool BindNative(int eventId)
        => NativeMethods.wxsharp_app_bind(eventId, Token);

    private protected override void UnbindNative(int eventId)
        => _ = NativeMethods.wxsharp_app_unbind(eventId);

    private protected override bool IsDead => _disposed;

    private protected override void Verify() { VerifyAccess(); ThrowIfDisposed(); }

    internal static App RequireCurrent()
    {
        var app = Current ?? throw new InvalidOperationException("Create an App before using wxWidgets.");
        app.VerifyAccess(); app.ThrowIfDisposed(); return app;
    }

    internal static long Register(EvtHandler handler)
    {
        var app = Current ?? throw new InvalidOperationException("Create an App before creating windows or controls.");
        app.VerifyAccess(); app.ThrowIfDisposed();
        var token = ++_nextToken;
        Handlers.Add(token, handler);
        return token;
    }

    internal static void Unregister(long token)
    {
        if (Handlers.Remove(token, out var handler) && handler is Window window &&
            window.NativeHandleForLookup != 0)
            ByHandle.Remove(window.NativeHandleForLookup);
    }

    // wxWidgets hands back raw wxWindow pointers from calls like GetDefaultItem; this maps one back to the
    // wrapper that owns it. Anything wxWidgets created on its own has no wrapper and comes back null.
    internal static void MapHandle(nint handle, Window window) => ByHandle[handle] = window;

    internal static Window? Lookup(nint handle)
        => handle != 0 && ByHandle.TryGetValue(handle, out var window) ? window : null;

    internal void NotifyWindowInvalidated(Window window)
    {
        if (!ReferenceEquals(_topWindow, window)) return;
        _topWindow = null;
        if (!_disposed) NativeMethods.wxsharp_set_top_window(0);
    }

    internal static void Queue(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        var app = Current ?? throw new InvalidOperationException("Create an App before queuing UI work.");
        lock (app._lifecycleGate)
        {
            app.ThrowIfDisposed();
            var token = Interlocked.Increment(ref _nextToken);
            lock (DeferredActions) DeferredActions.Add(token, action);
            NativeMethods.wxsharp_call_after(token);
        }
    }

    internal void RecordCallbackException(Exception ex)
    {
        _callbackException ??= ExceptionDispatchInfo.Capture(ex);
        NativeMethods.wxsharp_exit_main_loop();
    }

    // The single entry point from native code. Events reach exactly the window they were raised on;
    // wxWidgets, not this method, walks the parent chain for command events that go unhandled.
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe uint Dispatch(NativeEvent* native)
    {
        var app = Current;
        if (app is null || native is null || native->Version != NativeEvent.ExpectedVersion ||
            native->Size < (uint)sizeof(NativeEvent)) return 0;
        uint result;
        try
        {
            if (native->Kind == EventId.CallAfter)
            {
                Action? action = null;
                lock (DeferredActions)
                    if (DeferredActions.Remove(native->Token, out var found)) action = found;
                action?.Invoke();
                return 0;
            }
            if (!Handlers.TryGetValue(native->Token, out var handler)) return 0;
            result = handler.Dispatch(in *native);
        }
        catch (Exception ex) { app.RecordCallbackException(ex); result = 1; }

        // Retire the wrapper even if its final Destroyed subscriber threw. The C++ object is going away
        // regardless; leaving its old address mapped would turn the next property access into use-after-free.
        if (native->Kind == EventId.Destroy && Handlers.TryGetValue(native->Token, out var destroyed) &&
            destroyed is Window window)
        {
            try { window.InvalidateFromNative(); }
            catch (Exception ex) { app.RecordCallbackException(ex); }
        }
        return result;
    }

    // A list/tree item virtual asking for data it is about to draw or compare. Answered synchronously on
    // the UI thread, so handlers must stay cheap.
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe byte DispatchVirtualList(NativeVirtualListRequest* request)
    {
        var app = Current;
        if (app is null || request is null || request->Version != 1 ||
            request->Size < (uint)sizeof(NativeVirtualListRequest)) return 0;
        try
        {
            if (!Handlers.TryGetValue(request->Token, out var window)) return 0;
            if (request->Operation == 10 && window is TreeCtrl tree)
            {
                request->Result = tree.CompareItems(new TreeItemId(request->Item),
                    new TreeItemId(request->OtherItem));
                return 1;
            }
            if (window is not ListCtrl list) return 0;
            switch (request->Operation)
            {
                case 2: request->Result = list.GetVirtualItemImage(request->Item); return 1;
                case 3: request->Result = list.GetVirtualItemColumnImage(request->Item, request->Column); return 1;
                case 4: request->Result = list.GetVirtualItemIsChecked(request->Item) ? 1 : 0; return 1;
                case 1: break;
                default: return 0;
            }
            var text = list.GetVirtualItemText(request->Item, request->Column);
            var required = System.Text.Encoding.UTF8.GetByteCount(text);
            request->RequiredLength = required;
            if (request->Buffer is not null && request->BufferLength > required)
            {
                var destination = new Span<byte>(request->Buffer, request->BufferLength);
                System.Text.Encoding.UTF8.GetBytes(text, destination);
                destination[required] = 0;
            }
            return 1;
        }
        catch (Exception ex) { app.RecordCallbackException(ex); return 0; }
    }

    // Answers one wxWidgets virtual on behalf of a managed subclass. The managed base methods make
    // qualified native base calls, so an unoverridden member and an override calling base both reach the
    // same wxWidgets implementation without recursion. An exception cannot unwind into C++, so it is
    // recorded and treated as "no opinion", leaving wxWidgets to run its own implementation.
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void DispatchVirtual(NativeVirtualRequest* request)
    {
        var app = Current;
        if (app is null || request is null || request->Version != 1 ||
            request->Size < (uint)sizeof(NativeVirtualRequest)) return;
        try
        {
            // Only windows have overridable virtuals; the App is registered in the same map but answers none.
            if (!Handlers.TryGetValue(request->Token, out var handler) || handler is not Window window) return;
            if (window.TryAnswerVirtual(ref *request)) request->Handled = 1;
        }
        catch (Exception ex) { app.RecordCallbackException(ex); }
    }

    private void RunOnExit() { if (!_onExitCalled) { _onExitCalled = true; _ = OnExit(); } }

    private void Shutdown()
    {
        lock (_lifecycleGate)
        {
            if (_disposed) return;
            _disposed = true;
        }
        foreach (var window in new List<Window>(Handlers.Values.OfType<Window>())) window.InvalidateFromAppShutdown();
        Handlers.Clear();
        lock (DeferredActions) DeferredActions.Clear();
        Accessible.ClearRegistry();
        NativeMethods.wxsharp_shutdown();
        Current = null;
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);
}
