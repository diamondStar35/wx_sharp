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
    public int Id { get; private set; }
    public event EventHandler? Tick;
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
    public bool IsRunning => NativeMethods.wxsharp_timer_is_running(Handle);
    public bool IsOneShot() => NativeMethods.wxsharp_timer_is_one_shot(Handle);
    public int Interval => NativeMethods.wxsharp_timer_get_interval(Handle);
    public bool Start(int milliseconds = -1, bool oneShot = false)
    {
        if (milliseconds < -1 || milliseconds == 0) throw new ArgumentOutOfRangeException(nameof(milliseconds));
        return NativeMethods.wxsharp_timer_start(Handle, milliseconds, oneShot);
    }
    public bool StartOnce(int milliseconds = -1)
    {
        if (milliseconds < -1 || milliseconds == 0) throw new ArgumentOutOfRangeException(nameof(milliseconds));
        return NativeMethods.wxsharp_timer_start_once(Handle, milliseconds);
    }
    public virtual void Notify() => NativeMethods.wxsharp_timer_notify(Handle);
    public EvtHandler GetOwner() => _owner;
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
    public void Stop() => NativeMethods.wxsharp_timer_stop(Handle);
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
