using System;

namespace WxSharp;

/// <summary>How a <see cref="ProgressDialog"/> is built. <see cref="Default"/> is wxWidgets' own default,
/// <c>wxPD_APP_MODAL | wxPD_AUTO_HIDE</c>; cancelling and skipping are not part of it, because each adds a
/// button whose press the caller then has to read and act on.</summary>
[Flags]
public enum ProgressDialogStyle
{
    /// <summary>wxWidgets' default: modal to the application, and hidden once the work completes.</summary>
    Default = 1 << 30,
    /// <summary>Adds a Cancel button. Read <see cref="ProgressUpdate.Continue"/> to see it pressed.</summary>
    CanAbort = 1,
    /// <summary>Adds a Skip button. Read <see cref="ProgressUpdate.Skipped"/> to see it pressed.</summary>
    CanSkip = 2,
    /// <summary>Disables the application's other windows while the dialog is up.</summary>
    AppModal = 4,
    /// <summary>Hides the dialog as soon as the maximum is reached.</summary>
    AutoHide = 8,
    /// <summary>Shows how long the work has been running.</summary>
    ElapsedTime = 16,
    /// <summary>Shows how long the work is estimated to take in total.</summary>
    EstimatedTime = 32,
    /// <summary>Shows how much longer the work is estimated to take.</summary>
    RemainingTime = 64,
    /// <summary>Asks for a smooth gauge rather than a segmented one.</summary>
    Smooth = 128,
}

/// <summary>What the user did while a step of work was being reported. <c>wxProgressDialog.Update</c>
/// answers both questions at once, and both matter: a caller that reads only one either ignores a Cancel
/// or treats a Skip as an abort.</summary>
/// <param name="Continue">False once Cancel has been pressed. Stop the work and close the dialog.</param>
/// <param name="Skipped">True when Skip was pressed for this step. Move on to the next one.</param>
public readonly record struct ProgressUpdate(bool Continue, bool Skipped);

/// <summary>A dialog reporting the progress of a long operation, following <c>wxProgressDialog</c>. It is a
/// window like any other, so <see cref="Window.Raise"/>, <see cref="Window.Title"/> and the rest are
/// inherited rather than repeated here.</summary>
public class ProgressDialog : Window
{
    public ProgressDialog(string title, string message, int maximum = 100, Window? parent = null,
        ProgressDialogStyle style = ProgressDialogStyle.Default) : base(parent, WindowId.Any)
    {
        Initialize(GetType() == typeof(ProgressDialog)
            ? NativeMethods.wxsharp_progress_create(parent?.Handle ?? 0, title, message, maximum,
                (int)style, Token)
            : NativeMethods.wxsharp_custom_progress_create(parent?.Handle ?? 0, title, message, maximum,
                (int)style, Token));
    }

    /// <summary>Reports progress and pumps the dialog's own messages, so it stays responsive inside a
    /// synchronous loop. Follows <c>wxProgressDialog.Update</c>.</summary>
    public ProgressUpdate Update(int value, string message = "")
    {
        Verify();
        var skipped = NativeMethods.wxsharp_progress_update(Handle, value, message, out var keepGoing);
        return new ProgressUpdate(keepGoing, skipped);
    }

    /// <summary>Reports that work is continuing without knowing how far along it is, moving the gauge back
    /// and forth. Follows <c>wxProgressDialog.Pulse</c>.</summary>
    public ProgressUpdate Pulse(string message = "")
    {
        Verify();
        var skipped = NativeMethods.wxsharp_progress_pulse(Handle, message, out var keepGoing);
        return new ProgressUpdate(keepGoing, skipped);
    }

    /// <summary>Whether Cancel has been pressed. Follows <c>wxProgressDialog.WasCancelled</c>.</summary>
    public bool WasCancelled { get { Verify(); return NativeMethods.wxsharp_progress_was_cancelled(Handle); } }

    /// <summary>Whether Skip was pressed for the step just reported. Follows
    /// <c>wxProgressDialog.WasSkipped</c>.</summary>
    public bool WasSkipped { get { Verify(); return NativeMethods.wxsharp_progress_was_skipped(Handle); } }

    /// <summary>Un-cancels the dialog, so reporting can continue after a Cancel the caller decided not to
    /// honour. Follows <c>wxProgressDialog.Resume</c>.</summary>
    public void Resume() { Verify(); NativeMethods.wxsharp_progress_resume(Handle); }

    /// <summary>The progress reported so far. Follows <c>wxProgressDialog.GetValue</c>.</summary>
    public int Value { get { Verify(); return NativeMethods.wxsharp_progress_get_value(Handle); } }

    /// <summary>Closes the dialog and destroys it at once, rather than scheduling the deletion the way
    /// <see cref="Window.Destroy"/> does for an ordinary window. An app-modal progress dialog holds the rest
    /// of the application disabled for as long as it exists, so waiting for the next idle cycle would leave
    /// a caller that has finished with it looking at a frozen interface.</summary>
    public override bool Destroy()
    {
        if (IsDead) return false;
        Verify();
        NativeMethods.wxsharp_progress_destroy(Handle);
        InvalidateFromNative();
        return true;
    }

    /// <summary>The message currently shown above the bar - the last one handed to <see cref="Update"/> or
    /// <see cref="Pulse"/>, or the one the dialog was created with. Follows
    /// <c>wxProgressDialog.GetMessage</c>.</summary>
    public unsafe string Message
    {
        get
        {
            Verify();
            var length = NativeMethods.wxsharp_progress_get_message(Handle, null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_progress_get_message(Handle, p, buffer.Length);
            return Utf8String.Decode(buffer, length);
        }
    }

    /// <summary>The value that counts as complete. Follows <c>wxProgressDialog.GetRange</c> and
    /// <c>SetRange</c>, which is how a total that is only discovered part-way through is applied.</summary>
    public int Range
    {
        get { Verify(); return NativeMethods.wxsharp_progress_get_range(Handle); }
        set { Verify(); NativeMethods.wxsharp_progress_set_range(Handle, value); }
    }
}
