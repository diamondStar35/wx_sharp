using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace WxSharp;

internal sealed unsafe class DialogCustomization
{
    private static readonly Dictionary<long, DialogCustomization> Active = new();
    private static long _next;
    private readonly Dialog _dialog;
    private readonly FileDialogCustomizeHook _hook;
    private bool _building;
    private ExceptionDispatchInfo? _failure;
    internal nint Context { get; private set; }
    internal bool IsActive => Context != 0;
    internal bool OwnerAlive => _dialog.NativeHandleForLookup != 0;
    internal App App => _dialog.OwnerApp;
    internal Dictionary<int, FileDialogCustomControl> Controls { get; } = new();
    internal string[]? Paths { get; private set; }
    internal void ResetResults() => Paths = null;
    internal void ReleaseDeadOwner()
    {
        if (!OwnerAlive) _hook.Attachment = null;
    }

    internal DialogCustomization(Dialog dialog, FileDialogCustomizeHook hook)
    {
        if (hook.Attachment is not null) throw new InvalidOperationException("This hook is already attached to a dialog.");
        _dialog = dialog;
        _hook = hook;
        hook.Attachment = this;
        _dialog.Destroyed += OnDestroyed;
    }
    internal void Detach()
    {
        App.VerifyAccess();
        if (IsActive) throw new InvalidOperationException("Cannot replace a customization hook while its dialog is showing.");
        _hook.Attachment = null;
        _dialog.Destroyed -= OnDestroyed;
    }
    private void OnDestroyed(object? sender, WxEventArgs args)
    {
        if (IsActive) NativeMethods.wxsharp_item_dialog_close(Context);
        _hook.Attachment = null;
        args.Skip();
    }
    internal int Show(bool folders)
    {
        App.VerifyAccess();
        if (IsActive) throw new InvalidOperationException("The dialog is already showing.");
        Paths = null;
        _failure = null;
        var token = Interlocked.Increment(ref _next);
        Context = NativeMethods.wxsharp_item_dialog_create(_dialog.Handle, folders, &Dispatch, token);
        if (Context == 0) throw new InvalidOperationException("Could not initialize the Windows Common Item Dialog.");
        Active.Add(token, this);
        try
        {
            var result = NativeMethods.wxsharp_item_dialog_show(Context);
            _failure?.Throw();
            Check(result);
            if (result == StandardId.Ok)
            {
                Paths = new string[NativeMethods.wxsharp_item_dialog_count(Context)];
                for (var i = 0; i < Paths.Length; i++) Paths[i] = Read(0, i);
                if (!folders) NativeMethods.wxsharp_filedlg_set_filter_index(_dialog.Handle, NativeMethods.wxsharp_item_dialog_filter(Context));
            }
            return result;
        }
        finally
        {
            foreach (var control in Controls.Values) control.Invalidate();
            Controls.Clear();
            Active.Remove(token);
            NativeMethods.wxsharp_item_dialog_destroy(Context);
            Context = 0;
        }
    }
    internal void VerifyBuilding()
    {
        App.VerifyAccess();
        if (!_building) throw new InvalidOperationException("Controls can only be added in AddCustomControls.");
    }
    internal string CurrentPath => IsActive ? Read(1, 0) : string.Empty;
    internal string Read(int kind, int index)
    {
        App.VerifyAccess();
        if (!IsActive) throw new InvalidOperationException("The dialog is not showing.");
        var length = NativeMethods.wxsharp_item_dialog_text(Context, kind, index, null, 0);
        Check(length);
        if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1];
        fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_item_dialog_text(Context, kind, index, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }
    internal static int Check(int result) { Marshal.ThrowExceptionForHR(result); return result; }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static int Dispatch(long token, int operation, int id, int value)
    {
        if (!Active.TryGetValue(token, out var session)) return 0;
        try
        {
            switch (operation)
            {
                case 1:
                    session._building = true;
                    try { session._hook.AddCustomControls(new FileDialogCustomize(session)); }
                    finally { session._building = false; }
                    break;
                case 2: session._hook.UpdateCustomControls(); break;
                case 3: session._hook.TransferDataFromCustomControls(); break;
                case 4:
                    if (session.Controls.TryGetValue(id, out var control))
                        control.Dispatch(new NativeEvent { Kind = control.EventKind, Id = id, IntValue = value, Selection = value });
                    break;
            }
            return 1;
        }
        catch (Exception exception)
        {
            session._failure ??= ExceptionDispatchInfo.Capture(exception);
            NativeMethods.wxsharp_item_dialog_close(session.Context);
            return 0;
        }
    }
}
