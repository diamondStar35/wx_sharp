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
        _threadId = Environment.CurrentManagedThreadId;
        if (!NativeMethods.wxsharp_init()) throw new InvalidOperationException("wxWidgets initialization failed.");
        unsafe { NativeMethods.wxsharp_set_event_handler(&Dispatch); }
        unsafe { NativeMethods.wxsharp_set_accessible_handler(&Accessible.Dispatch); }
        Current = this;
        NativeMethods.wxsharp_set_exit_on_frame_delete(true);
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

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe uint Dispatch(NativeEvent* native)
    {
        var app = Current;
        if (app is null || native is null || native->Version != 1) return 0;
        try
        {
            if (native->Kind == EventKind.CallAfter)
            {
                Action? action = null;
                lock (DeferredActions)
                    if (DeferredActions.Remove(native->Token, out var found)) action = found;
                action?.Invoke();
                return 0;
            }
            if (!Windows.TryGetValue(native->Token, out var window)) return 0;
            var boundResult = window.DispatchBindings(in *native);
            var result = native->Kind == EventKind.Destroyed || boundResult == 0
                ? boundResult | window.Dispatch(in *native)
                : boundResult;
            if (result == 0 && IsCommandEvent(native->Kind))
                for (var parent = window.Parent; parent is not null && result == 0; parent = parent.Parent)
                    result = parent.DispatchBindings(in *native, window);
            return result;
        }
        catch (Exception ex) { app.RecordCallbackException(ex); return 1; }
    }

    private void RunOnExit() { if (!_onExitCalled) { _onExitCalled = true; _ = OnExit(); } }

    private static bool IsCommandEvent(EventKind kind) => kind is EventKind.Click or EventKind.Text or
        EventKind.Toggle or EventKind.Select or EventKind.Slider or EventKind.TextEnter or EventKind.Menu or
        EventKind.Timer;

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
