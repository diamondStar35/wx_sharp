using System;

namespace WxSharp;

/// <summary>A native dialog with a vertical content area. Use it modally - add controls (wire buttons to
/// <see cref="EndModal"/>), lay out, then <see cref="ShowModal"/> blocks and returns the result - or modeless
/// via <see cref="Show"/>, driven by the host loop like a window and reporting <see cref="Closed"/>. Call
/// <see cref="Destroy"/> when finished with it.</summary>
public sealed class Dialog : Container
{
    /// <summary>Raised when a modeless dialog is closed (it is hidden, not destroyed - reshow or destroy it).</summary>
    public event Action? Closed;

    public Dialog(string title, int width = 400, int height = 300)
    {
        var handle = NativeMethods.wxsharp_dialog_create(title, width, height, Id);
        AttachContainer(handle, NativeMethods.wxsharp_dialog_panel(handle));
    }

    /// <summary>Re-flows the content after controls have been added.</summary>
    public void Layout() => NativeMethods.wxsharp_dialog_layout(Handle);

    /// <summary>Sets the result returned when Esc is pressed (e.g. <see cref="DialogResult.Cancel"/> for
    /// Esc-to-close). Opt-in - nothing happens on Esc unless you call this.</summary>
    public void SetEscapeId(DialogResult result) => NativeMethods.wxsharp_dialog_set_escape_id(Handle, (int)result);

    /// <summary>Sets the action triggered when Enter is pressed (e.g. <see cref="DialogResult.Ok"/>).</summary>
    public void SetAffirmativeId(DialogResult result) => NativeMethods.wxsharp_dialog_set_affirmative_id(Handle, (int)result);

    /// <summary>Runs the dialog modally (blocking) and returns the result it ended with.</summary>
    public DialogResult ShowModal() => (DialogResult)NativeMethods.wxsharp_dialog_show_modal(Handle);

    /// <summary>Shows or hides the dialog modelessly (non-blocking); pump the host loop to keep it live.</summary>
    public void Show(bool show = true) => NativeMethods.wxsharp_dialog_show(Handle, show);

    public void Hide() => NativeMethods.wxsharp_dialog_show(Handle, false);

    /// <summary>Ends a modal loop with a result (call from a button's handler).</summary>
    public void EndModal(DialogResult result) => NativeMethods.wxsharp_dialog_end_modal(Handle, (int)result);

    public void Destroy()
    {
        NativeMethods.wxsharp_dialog_destroy(Handle);
        Cleanup();
    }

    internal override void OnNativeEvent(EventKind evt)
    {
        if (evt == EventKind.Close)
            Closed?.Invoke();
    }
}
