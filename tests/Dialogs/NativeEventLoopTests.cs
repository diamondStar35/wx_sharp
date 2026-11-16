using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace WxSharp.Tests.Dialogs;

[TestClass, DoNotParallelize]
public sealed partial class NativeEventLoopTests
{
    private static Action? _tick;
    private static Exception? _failure;

    [TestMethod]
    public void CustomizedPickerProcessesWorkerUpdatesAndNativeCancellation()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var app = new App();
                using var owner = new Frame();
                using var dialog = new DirDialog(owner, defaultPath: Path.GetTempPath());
                var hook = new UpdateHook();
                Assert.IsTrue(dialog.SetCustomizeHook(hook));
                _failure = null;
                _tick = () =>
                {
                    _tick = null;
                    var picker = GetWindow(owner.NativeHandle, 6); // GW_ENABLEDPOPUP.
                    _ = CheckLabels(picker, hook);
                };
                try { Assert.AreEqual(StandardId.Cancel, dialog.ShowModal()); }
                finally { KillTimer(0, hook.Timer); _tick = null; }
                if (_failure is not null) ExceptionDispatchInfo.Capture(_failure).Throw();
                Assert.IsTrue(hook.FolderUpdates > 0);
                Assert.AreEqual(0, hook.Transfers);
                Assert.IsTrue(owner.Enabled);
            }
            catch (Exception exception) { failure = exception; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private sealed class UpdateHook : FileDialogCustomizeHook
    {
        internal nuint Timer;
        internal bool Updated;
        internal int FolderUpdates, Transfers;
        public override unsafe void AddCustomControls(FileDialogCustomize customizer)
        {
            customizer.AddChoice(["DVD-Video", "DVD-Audio"], "Disc type").SelectedIndex = 0;
            var choice = customizer.AddChoice(["Loading"], "Title");
            choice.SelectedIndex = 0;
            customizer.AddTextCtrl("Description").Value = "Example";
            _ = Task.Run(() => Wx.CallAfter(() => { choice.Set(["Ready"]); choice.SelectedIndex = 0; Updated = true; }));
            // The timer cancels this test's own native dialog through its API; no input is synthesized.
            Timer = SetTimer(0, 0, 1000, &OnTimer);
            Assert.AreNotEqual((nuint)0, Timer);
        }
        public override void UpdateCustomControls() => FolderUpdates++;
        public override void TransferDataFromCustomControls() => Transfers++;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void OnTimer(nint window, uint message, nuint id, uint time)
    {
        try { _tick?.Invoke(); }
        catch (Exception exception) { _failure = exception; }
    }
    private static async Task CheckLabels(nint picker, UpdateHook hook)
    {
        try
        {
            Assert.IsTrue(hook.Updated, "Worker CallAfter did not reach the native dialog loop.");
            Assert.AreNotEqual((nint)0, picker);
            // UI Automation must run off the UI thread so its provider can process requests.
            var elements = await Task.Run(() => Automation.VisibleElements(picker)).WaitAsync(TimeSpan.FromSeconds(10));
            foreach (var (label, type, role) in new[] { ("Disc type", 50003, 46), ("Title", 50003, 46), ("Description", 50004, 42) })
            {
                Assert.IsTrue(elements.Any(element => element.Name == label), $"The native picker did not display its {label} label.");
                Assert.IsTrue(elements.Any(element => element.Type == type && element.Focusable
                    && element.Name == label && element.LegacyName == label && element.LegacyRole == role),
                    $"The {label} field must expose its own name in both UI Automation and MSAA. "
                    + string.Join("; ", elements.Where(element => element.Type == type).Select(element => $"UIA='{element.Name}', MSAA='{element.LegacyName}'")));
            }
        }
        catch (Exception exception) { _failure = exception; }
        finally { Wx.CallAfter(() => NativeMethods.wxsharp_item_dialog_close(hook.Attachment!.Context)); }
    }
    [LibraryImport("user32.dll")]
    private static partial nint GetWindow(nint window, uint command);
    [LibraryImport("user32.dll")]
    private static unsafe partial nuint SetTimer(nint window, nuint id, uint milliseconds,
        delegate* unmanaged[Stdcall]<nint, uint, nuint, uint, void> callback);
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool KillTimer(nint window, nuint id);
}
