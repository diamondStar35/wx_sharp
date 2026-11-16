using System.Runtime.ExceptionServices;

namespace WxSharp.Tests.Dialogs;

[TestClass, DoNotParallelize]
public sealed class CustomizationTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void NativeControlsRoundTripWithoutShowingAWindow(bool folders)
        => OnSta(() =>
        {
            using var app = new App();
            using var owner = new Frame();
            using Dialog dialog = folders ? new DirDialog(owner) : new FileDialog(owner);
            var hook = new ProbeHook();
            Assert.IsTrue(folders ? ((DirDialog)dialog).SetCustomizeHook(hook) : ((FileDialog)dialog).SetCustomizeHook(hook));
            Assert.Throws<ProbeCompleteException>(() => dialog.ShowModal());
            Assert.AreEqual(1, hook.AddCalls);
            Assert.AreEqual(0, hook.TransferCalls);
            Assert.IsNotNull(hook.CheckBox);
            Assert.Throws<ObjectDisposedException>(() => _ = hook.CheckBox.Checked);
            Assert.Throws<ProbeCompleteException>(() => dialog.ShowModal());
            Assert.AreEqual(2, hook.AddCalls);
            Assert.IsTrue(owner.Enabled);
        });

    [TestMethod]
    public void HookCannotBeAttachedToTwoDialogs()
        => OnSta(() =>
        {
            using var app = new App();
            using var first = new FileDialog();
            using var second = new DirDialog();
            var hook = new ProbeHook();
            Assert.IsTrue(first.SetCustomizeHook(hook));
            Assert.Throws<InvalidOperationException>(() => second.SetCustomizeHook(hook));
            Assert.IsTrue(first.SetCustomizeHook(null));
            Assert.IsTrue(second.SetCustomizeHook(hook));
        });

    private sealed class ProbeCompleteException : Exception;

    private sealed class ProbeHook : FileDialogCustomizeHook
    {
        internal int AddCalls, TransferCalls;
        internal FileDialogCheckBox? CheckBox;
        public override void AddCustomControls(FileDialogCustomize customizer)
        {
            AddCalls++;
            var button = customizer.AddButton("Command");
            button.Click += (_, _) => Assert.Fail("No input should be generated.");
            button.Disable();
            button.Enable();
            CheckBox = customizer.AddCheckBox("Enabled");
            CheckBox.Toggled += (_, _) => Assert.Fail("Programmatic changes must not produce input.");
            CheckBox.Checked = true;
            Assert.IsTrue(CheckBox.Checked);
            var first = customizer.AddRadioButton("First");
            var second = customizer.AddRadioButton("Second");
            first.Value = true;
            Assert.IsTrue(first.Value);
            second.Value = true;
            Assert.IsFalse(first.Value);
            Assert.IsTrue(second.Value);
            var choice = customizer.AddChoice(["One", "Two"], "Choices");
            choice.SelectionChanged += (_, _) => Assert.Fail("Programmatic changes must not produce input.");
            choice.SelectedIndex = 1;
            Assert.AreEqual(1, choice.SelectedIndex);
            choice.Set(["Replacement"]);
            Assert.AreEqual(-1, choice.SelectedIndex);
            choice.SelectedIndex = 0;
            Assert.AreEqual(0, choice.SelectedIndex);
            choice.SelectedIndex = -1;
            Assert.AreEqual(-1, choice.SelectedIndex);
            Assert.Throws<ArgumentOutOfRangeException>(() => choice.SelectedIndex = 3);
            var edit = customizer.AddTextCtrl("Text");
            edit.Value = "Unicode: \u0623\u0647\u0644\u0627";
            Assert.AreEqual("Unicode: \u0623\u0647\u0644\u0627", edit.Value);
            var text = customizer.AddStaticText("Status");
            text.SetLabelText("Updated");
            text.Hide();
            text.Show();
            // Finish before IFileDialog::Show: exercise the real COM controls without touching the desktop.
            throw new ProbeCompleteException();
        }
        public override void TransferDataFromCustomControls() => TransferCalls++;
    }

    private static void OnSta(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() => { try { action(); } catch (Exception ex) { failure = ex; } });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure is not null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
