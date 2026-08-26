using System.Runtime.InteropServices;
using WxSharp;

const string expected = "WxSharp — العربية — 日本語 — 🚀";

var nativeLibrary = Environment.GetEnvironmentVariable("WXSHARP_NATIVE_LIBRARY");
if (!string.IsNullOrEmpty(nativeLibrary))
{
    NativeLibrary.SetDllImportResolver(typeof(Wx).Assembly, (_, assembly, searchPath) =>
        NativeLibrary.Load(nativeLibrary, assembly, searchPath));
}

if (args.Contains("--callback-exception"))
{
    using var app = new SmokeApp();
    var frame = new Frame(title: "Callback exception");
    frame.Closing += (_, _) => throw new ExpectedCallbackException();
    frame.Show();
    Wx.CallAfter(frame.Close);
    try
    {
        app.MainLoop();
        throw new InvalidOperationException("MainLoop did not rethrow the callback exception.");
    }
    catch (ExpectedCallbackException) { }
    VerifyLifecycle(app);
    Console.WriteLine("Callback exception smoke test passed.");
    return;
}

if (args.Contains("--init-false"))
{
    using var app = new RejectingApp();
    if (app.MainLoop() != 0 || !app.OnInitCalled || !app.OnExitCalled || App.Current is not null)
        throw new InvalidOperationException("OnInit(false) did not perform one-shot cleanup.");
    Console.WriteLine("OnInit(false) smoke test passed.");
    return;
}

using (var app = new App()) { }
if (App.Current is not null)
    throw new InvalidOperationException("Disposing an App without a loop did not clean up.");

using (var app = new SmokeApp())
{
    var frame = new Frame(title: expected, size: new Size(480, 360));
    var panel = new Panel(frame);
    var label = new StaticText(panel, label: expected);
    var toggle = new ToggleButton(panel, "Toggle");
    var gauge = new Gauge(panel, value: 25);
    var spin = new SpinCtrl(panel, value: 3, minimum: 1, maximum: 10);
    var combo = new ComboBox(panel, expected);
    combo.Add("second");
    var search = new SearchCtrl(panel, "query");
    var checklist = new CheckListBox(panel);
    checklist.Add("checked"); checklist.SetChecked(0);
    string[] radioChoices = ["one", "two"];
    var radio = new RadioBox(panel, "Choice", radioChoices);
    var activity = new ActivityIndicator(panel);
    var list = new ListCtrl(panel);
    list.InsertColumn(0, "Name", 160); var listItem = list.AddItem(expected);
    var tree = new TreeCtrl(panel);
    var root = tree.AddRoot("root"); var child = tree.Add(root, expected);
    var grid = new Grid(panel, 1, 1); grid[0, 0] = expected;
    var dataView = new DataViewListCtrl(panel);
    dataView.AddTextColumn("Value"); string[] row = [expected]; dataView.AddRow(row);
    var dataTree = new DataViewTreeCtrl(panel);
    var dataRoot = dataTree.AddContainer(DataViewItem.Root, "root");
    var dataChild = dataTree.AddItem(dataRoot, expected);
    var simpleBook = new SimpleBook(panel);
    var simplePage = new Panel(simpleBook);
    if (!simpleBook.AddPage(simplePage, "Page", true)) throw new InvalidOperationException("SimpleBook page creation failed.");
    using var fileMenu = new Menu();
    fileMenu.Add(1001, "&Close");
    using var menuBar = new MenuBar();
    if (!menuBar.Add(fileMenu, "&File")) throw new InvalidOperationException("Menu creation failed.");
    frame.SetMenuBar(menuBar);
    var statusBar = new StatusBar(frame);
    statusBar.SetText(expected);
    var layout = new BoxSizer(Orientation.Vertical);
    layout.Add(label, flags: SizerFlags.All, border: 8);
    foreach (var control in new Window[] { toggle, gauge, spin, combo, search, checklist, radio, activity, list, tree, grid, dataView, dataTree, simpleBook })
        layout.Add(control, flags: SizerFlags.Expand | SizerFlags.All, border: 2);
    panel.SetSizer(layout);

    if (label.Label != expected)
        throw new InvalidOperationException($"UTF-8 round trip failed: '{label.Label}'.");
    if (frame.Title != expected || frame.Id == WindowId.Any || label.Id == WindowId.Any)
        throw new InvalidOperationException("Title or generated wx window IDs did not round-trip.");
    if (spin.Value != 3 || combo.Value != expected || !checklist.IsChecked(0) ||
        list.GetItem(listItem) != expected || tree.GetText(child) != expected || grid[0, 0] != expected ||
        statusBar.GetText() != expected || dataView[0, 0] != expected || dataTree.GetText(dataChild) != expected ||
        simpleBook.Count != 1)
        throw new InvalidOperationException("Expanded control state did not round-trip.");

    label.AccessibleName = expected;
    if (Wx.SupportsCustomAccessibility)
    {
        label.Accessible = new SmokeAccessible(expected);
        if (!label.Accessible.ValidateBridge()) throw new InvalidOperationException("Accessible reverse callback bridge failed.");
        label.Accessible.Notify(AccessibleEvent.NameChanged);
    }
    frame.Show();

    var closeAttempts = 0;
    var boundCloseAttempts = 0;
    using var closeBinding = frame.Bind(WxEvents.Closing, (_, _) => boundCloseAttempts++);
    frame.Closing += (_, e) =>
    {
        closeAttempts++;
        if (closeAttempts == 1) { e.Cancel = true; Wx.CallAfter(frame.Close); }
    };

    var timerTicks = 0;
    var timer = new WxSharp.Timer(frame);
    timer.Tick += (_, _) => { timerTicks++; timer.Stop(); frame.Close(); };

    // Queue from a worker to verify thread-safe UI marshaling and a genuinely blocking native MainLoop.
    Task.Run(() => Wx.CallAfter(() => timer.Start(10))).GetAwaiter().GetResult();
    app.MainLoop();
    VerifyLifecycle(app);
    if (closeAttempts != 2)
        throw new InvalidOperationException("Close veto did not preserve the frame for a second close request.");
    if (boundCloseAttempts != 2)
        throw new InvalidOperationException("Generic Bind did not receive both close attempts.");
    if (timerTicks != 1)
        throw new InvalidOperationException("Timer did not dispatch exactly once.");
}

Console.WriteLine($"Smoke test passed; custom accessibility: {Wx.SupportsCustomAccessibility}.");

static void VerifyLifecycle(SmokeApp app)
{
    if (!app.OnInitCalled || !app.OnExitCalled || App.Current is not null)
        throw new InvalidOperationException("App lifecycle hooks or automatic cleanup did not run.");
}

sealed class ExpectedCallbackException : Exception { }

class SmokeApp : App
{
    public bool OnInitCalled { get; private set; }
    public bool OnExitCalled { get; private set; }
    protected override bool OnInit() { OnInitCalled = true; return true; }
    protected override int OnExit() { OnExitCalled = true; return 0; }
}

sealed class RejectingApp : SmokeApp
{
    protected override bool OnInit() { base.OnInit(); return false; }
}

sealed class SmokeAccessible(string name) : Accessible
{
    public override AccessibleStatus GetChildCount(out int count) { count = 0; return AccessibleStatus.Ok; }
    public override AccessibleStatus GetName(int childId, out string value) { value = name; return AccessibleStatus.Ok; }
    public override AccessibleStatus GetRole(int childId, out AccessibleRole role) { role = AccessibleRole.StaticText; return AccessibleStatus.Ok; }
    public override AccessibleStatus GetState(int childId, out AccessibleState state) { state = AccessibleState.Focusable; return AccessibleStatus.Ok; }
}
