using System;

namespace WxSharp;

/// <summary>A wxTimer owned by a window or by the application. Ticks arrive on the UI thread as
/// <see cref="WxEvents.Timer"/> events carrying this timer's ID.</summary>
///
/// <remarks>
/// Each timer has an ID of its own, and its events carry it. Passing <see cref="WindowId.Any"/> - the
/// default - takes one from <see cref="IdManager"/> rather than passing wxID_ANY down, because wxWidgets
/// does the same thing internally: <c>wxTimerImpl::SetOwner</c> replaces wxID_ANY with <c>wxNewId()</c>, so
/// the events arrive carrying a real ID whatever was asked for. Binding the handler for wxID_ANY would then
/// match every other timer on the same owner as well as this one, and two timers would each be run by the
/// other's tick. An ID taken here is returned to the pool when the timer is disposed.
///
/// The owner is any <see cref="EvtHandler"/>, which is what <c>wxTimer</c> takes. Owning one from the
/// <see cref="App"/> is how a timer outlives every window - a debounce that has to keep running while the
/// interface is being rebuilt, for instance.
/// </remarks>
public class Timer : IDisposable
{
    private EvtHandler _owner;
    private EventBinding _binding;
    private nint _handle;
    private bool _ownsId;
    /// <summary>This timer's ID, carried by its <see cref="WxEvents.Timer"/> events. Settles to a real ID
    /// even when <see cref="WindowId.Any"/> was requested.</summary>
    public int Id { get; private set; }
    /// <summary>Raised on the UI thread each time the timer fires.</summary>
    public event EventHandler? Tick;
    /// <summary>Creates a timer owned by <paramref name="owner"/>. Pass an <see cref="App"/> as the owner for
    /// a timer that outlives every window. The default <paramref name="id"/> takes one from
    /// <see cref="IdManager"/>.</summary>
    public Timer(EvtHandler owner, int id = WindowId.Any)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _ownsId = id == WindowId.Any;
        _owner = owner; Id = _ownsId ? IdManager.NewId() : id;
        _handle = NativeMethods.wxsharp_timer_create(OwnerHandle(owner), Id, owner.Token);
        if (_handle == 0) throw new InvalidOperationException("wxWidgets failed to create the timer.");
        // Read back what wxWidgets settled on, so the binding below is made for the ID the events will
        // actually carry rather than the one that was asked for.
        Id = NativeMethods.wxsharp_timer_get_id(_handle);
        _binding = owner.Bind(WxEvents.Timer, (_, _) => Tick?.Invoke(this, EventArgs.Empty), Id);
        if (owner is Window window) window.Invalidated += Dispose;
    }

    // A null handle tells the native side to own the timer from the application, which is what an
    // App-owned timer means; an application outlives its windows, so there is nothing to be invalidated by.
    private static nint OwnerHandle(EvtHandler owner) => owner is Window window ? window.Handle : 0;
    /// <summary>Whether the timer is currently running.</summary>
    public bool IsRunning => NativeMethods.wxsharp_timer_is_running(Handle);
    /// <summary>Whether the timer is set to fire only once rather than repeatedly.</summary>
    public bool IsOneShot() => NativeMethods.wxsharp_timer_is_one_shot(Handle);
    /// <summary>The interval between ticks, in milliseconds.</summary>
    public int Interval => NativeMethods.wxsharp_timer_get_interval(Handle);
    /// <summary>Starts the timer. <paramref name="milliseconds"/> is the interval (-1 reuses the last one);
    /// <paramref name="oneShot"/> fires once instead of repeating. Returns false if the timer could not
    /// start.</summary>
    public bool Start(int milliseconds = -1, bool oneShot = false)
    {
        if (milliseconds < -1 || milliseconds == 0) throw new ArgumentOutOfRangeException(nameof(milliseconds));
        return NativeMethods.wxsharp_timer_start(Handle, milliseconds, oneShot);
    }
    /// <summary>Starts the timer to fire exactly once after <paramref name="milliseconds"/> (-1 reuses the
    /// last interval). Shorthand for <see cref="Start(int, bool)"/> with one-shot set.</summary>
    public bool StartOnce(int milliseconds = -1)
    {
        if (milliseconds < -1 || milliseconds == 0) throw new ArgumentOutOfRangeException(nameof(milliseconds));
        return NativeMethods.wxsharp_timer_start_once(Handle, milliseconds);
    }
    /// <summary>Fires the timer's notification now, as wxWidgets does on each tick. Override in a subclass to
    /// react without binding <see cref="Tick"/>.</summary>
    public virtual void Notify() => NativeMethods.wxsharp_timer_notify(Handle);
    /// <summary>The handler that owns this timer and receives its events.</summary>
    public EvtHandler GetOwner() => _owner;
    /// <summary>Re-points the timer at a new owner (and optional ID). The new owner must belong to the same
    /// <see cref="App"/>.</summary>
    public void SetOwner(EvtHandler owner, int id = WindowId.Any)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _owner.OwnerApp.VerifyAccess(); owner.OwnerApp.VerifyAccess();
        if (!ReferenceEquals(_owner.OwnerApp, owner.OwnerApp)) throw new ArgumentException("Owner belongs to another App.", nameof(owner));
        if (_owner is Window previous) previous.Invalidated -= Dispose;
        _binding.Dispose();
        ReleaseId();
        _ownsId = id == WindowId.Any;
        _owner = owner; Id = _ownsId ? IdManager.NewId() : id;
        NativeMethods.wxsharp_timer_set_owner(Handle, OwnerHandle(owner), Id, owner.Token);
        Id = NativeMethods.wxsharp_timer_get_id(Handle);
        _binding = owner.Bind(WxEvents.Timer, (_, _) => Tick?.Invoke(this, EventArgs.Empty), Id);
        if (owner is Window window) window.Invalidated += Dispose;
    }
    /// <summary>Stops the timer. Safe to call when it is not running.</summary>
    public void Stop() => NativeMethods.wxsharp_timer_stop(Handle);
    /// <summary>Stops and destroys the timer, returning its ID to the pool if it owns one.</summary>
    public void Dispose()
    {
        if (_handle == 0) return;
        _owner.OwnerApp.VerifyAccess();
        if (_owner is Window owner) owner.Invalidated -= Dispose;
        _binding.Dispose();
        NativeMethods.wxsharp_timer_destroy(_handle); _handle = 0;
        ReleaseId();
        GC.SuppressFinalize(this);
    }

    private void ReleaseId()
    {
        if (!_ownsId) return;
        _ownsId = false;
        IdManager.Release(Id);
    }
    private nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Timer));
}
