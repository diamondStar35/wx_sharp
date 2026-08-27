using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace WxSharp;

/// <summary>Owns one wxWidgets application and its blocking native event loop.</summary>
public class App : IDisposable
{
    private static readonly Dictionary<long, Window> Windows = new();
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
        Current = this;
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

    internal static App RequireCurrent()
    {
        var app = Current ?? throw new InvalidOperationException("Create an App before using wxWidgets.");
        app.VerifyAccess(); app.ThrowIfDisposed(); return app;
    }

    internal static long Register(Window window)
    {
        var app = Current ?? throw new InvalidOperationException("Create an App before creating windows or controls.");
        app.VerifyAccess(); app.ThrowIfDisposed();
        var token = ++_nextToken;
        Windows.Add(token, window);
        return token;
    }

    internal static void Unregister(long token) => Windows.Remove(token);

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

    private void RecordCallbackException(Exception ex)
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
        if (app is null || native is null || native->Version != NativeEvent.ExpectedVersion) return 0;
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
            if (!Windows.TryGetValue(native->Token, out var window)) return 0;
            var result = window.Dispatch(in *native);
            // Destruction is reported whether or not anything subscribed: it is what retires the managed
            // wrapper, and it must happen after any handler has had its last look at the window.
            if (native->Kind == EventId.Destroy) window.InvalidateFromNative();
            return result;
        }
        catch (Exception ex) { app.RecordCallbackException(ex); return 1; }
    }

    // A virtual list control asking for a cell it is about to draw. Answered synchronously on the UI
    // thread, so the handler must stay cheap - which is what wxListCtrl.OnGetItemText requires too.
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe byte DispatchVirtualList(NativeVirtualListRequest* request)
    {
        var app = Current;
        if (app is null || request is null || request->Version != 1) return 0;
        try
        {
            if (!Windows.TryGetValue(request->Token, out var window) || window is not ListCtrl list)
                return 0;
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

    private void RunOnExit() { if (!_onExitCalled) { _onExitCalled = true; _ = OnExit(); } }

    private void Shutdown()
    {
        lock (_lifecycleGate)
        {
            if (_disposed) return;
            _disposed = true;
        }
        foreach (var window in new List<Window>(Windows.Values)) window.InvalidateFromAppShutdown();
        Windows.Clear();
        lock (DeferredActions) DeferredActions.Clear();
        Accessible.ClearRegistry();
        NativeMethods.wxsharp_shutdown();
        Current = null;
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);
}
