using System;

namespace WxSharp;

public sealed class Timer : IDisposable
{
    private readonly Window _owner;
    private readonly EventBinding _binding;
    private nint _handle;
    public int Id { get; }
    public event EventHandler? Tick;
    public Timer(Window owner, int id = WindowId.Any)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _owner = owner; Id = id == WindowId.Any ? unchecked((int)(owner.Token & 0x3fffffff) + 10000) : id;
        _handle = NativeMethods.wxsharp_timer_create(Id, owner.Token);
        if (_handle == 0) throw new InvalidOperationException("wxWidgets failed to create the timer.");
        _binding = owner.Bind(WxEvents.Timer, (_, _) => Tick?.Invoke(this, EventArgs.Empty), Id);
        owner.Invalidated += Dispose;
    }
    public bool IsRunning => NativeMethods.wxsharp_timer_is_running(Handle);
    public int Interval => NativeMethods.wxsharp_timer_get_interval(Handle);
    public bool Start(int milliseconds, bool oneShot = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(milliseconds);
        return NativeMethods.wxsharp_timer_start(Handle, milliseconds, oneShot);
    }
    public void Stop() => NativeMethods.wxsharp_timer_stop(Handle);
    public void Dispose()
    {
        if (_handle == 0) return;
        _owner.OwnerApp.VerifyAccess(); _owner.Invalidated -= Dispose; _binding.Dispose();
        NativeMethods.wxsharp_timer_destroy(_handle); _handle = 0;
    }
    private nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Timer));
}
