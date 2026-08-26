using System;

namespace WxSharp;

public sealed class ProgressDialog : IDisposable
{
    private nint _handle;
    public ProgressDialog(string title, string message, int maximum = 100, Window? parent = null)
    {
        _ = App.RequireCurrent(); ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximum);
        _handle = NativeMethods.wxsharp_progress_create(parent?.Handle ?? 0, title, message, maximum);
        if (_handle == 0) throw new InvalidOperationException("wxWidgets failed to create the progress dialog.");
    }
    public bool Update(int value, string message = "")
    {
        _ = NativeMethods.wxsharp_progress_update(Handle, value, message, out var continueRunning);
        return continueRunning;
    }
    public bool Pulse(string message = "")
    {
        _ = NativeMethods.wxsharp_progress_pulse(Handle, message, out var continueRunning);
        return continueRunning;
    }
    public void Dispose() { if (_handle != 0) NativeMethods.wxsharp_progress_destroy(_handle); _handle = 0; }
    private nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(ProgressDialog));
}
