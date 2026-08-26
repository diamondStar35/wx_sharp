using System.Collections.Generic;
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
    var multiline = new TextCtrl(panel, value: "one\ntwo\nthree", style: TextCtrlStyle.MultiLine);
    var list = new ListCtrl(panel, style: ListCtrlStyle.Report | ListCtrlStyle.SingleSelection);
    list.InsertColumn(0, "Name", 160); var listItem = list.AddItem(expected);
    var tree = new TreeCtrl(panel, style: TreeCtrlStyle.HasButtons | TreeCtrlStyle.LinesAtRoot);
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
    // Menus: items as objects, a submenu, check state, and a stock ID that carries the platform's own
    // label and accelerator.
    var closeId = IdManager.NewId();
    var recentId = IdManager.NewId();
    var fileMenu = new Menu();
    var closeItem = fileMenu.Append(closeId, "&Close\tCtrl+W", "Close the window");
    var recentMenu = new Menu();
    recentMenu.Append(recentId, "&Nothing yet");
    var recentItem = fileMenu.AppendSubMenu(recentMenu, "&Recent");
    fileMenu.AppendSeparator();
    var checkItem = fileMenu.AppendCheckItem(IdManager.NewId(), "&Repeat");
    var exitItem = fileMenu.Append(StandardId.Exit, "E&xit");
    var menuBar = new MenuBar();
    if (!menuBar.Append(fileMenu, "&File")) throw new InvalidOperationException("Menu creation failed.");
    frame.SetMenuBar(menuBar);

    if (closeItem.Id != closeId || closeItem.Kind != MenuItemKind.Normal || !closeItem.Label.Contains("Ctrl+W"))
        throw new InvalidOperationException("Menu item identity or accelerator label did not round-trip.");
    if (recentItem.SubMenu is null || recentItem.SubMenu.Count != 1)
        throw new InvalidOperationException("Submenu was not attached.");
    if (fileMenu.Count != 5 || fileMenu[2].Kind != MenuItemKind.Separator)
        throw new InvalidOperationException("Menu contents did not match what was appended.");
    if (!checkItem.IsCheckable || checkItem.Checked) throw new InvalidOperationException("Check item started checked.");
    if (menuBar.FindItem(closeId)?.Id != closeId || fileMenu.FindItem(recentId)?.Id != recentId)
        throw new InvalidOperationException("Menu item lookup failed, or did not search submenus.");
    if (exitItem.Id != StandardId.Exit || StandardId.Exit == StandardId.Ok)
        throw new InvalidOperationException("Stock identifiers did not resolve.");
    closeItem.Help = expected;
    if (closeItem.Help != expected) throw new InvalidOperationException("Menu item help did not round-trip.");
    if (menuBar.Count != 1 || menuBar.GetLabelTop(0) is not { Length: > 0 })
        throw new InvalidOperationException("Menu bar contents did not round-trip.");

    // Accelerators: parsed from the strings a user-configurable shortcut would be stored as, and installed
    // on the frame. wxAcceleratorEntry is the parser, so what round-trips here is what wx itself accepts.
    if (!AcceleratorEntry.TryParse("Ctrl+Shift+P", closeId, out var parsed) ||
        parsed.Modifiers != (AcceleratorModifiers.Control | AcceleratorModifiers.Shift))
        throw new InvalidOperationException("Accelerator parsing failed.");
    if (AcceleratorEntry.TryParse("not an accelerator", closeId, out _))
        throw new InvalidOperationException("Accelerator parsing accepted nonsense.");
    if (!parsed.ToString().Contains("Shift"))
        throw new InvalidOperationException($"Accelerator formatting failed: '{parsed}'.");
    frame.SetAcceleratorTable(parsed, AcceleratorEntry.Parse("F11", StandardId.Exit));
    frame.SetAcceleratorTable();

    // ---- Update UI: wxWidgets asks what state a command should be in, and the answer is applied
    // everywhere that command appears. Bound by command ID, exactly as in Phoenix.
    var trackLoaded = false;
    var repeating = false;
    var closeLabel = "&Close\tCtrl+W";

    // One handler per command: a second binding on the same ID would never run, because the first one
    // does not skip. That is wxWidgets' handler order, and it applies to update-UI like any other event.
    using var closeState = frame.Bind(WxEvents.UpdateUI, (_, e) =>
    {
        e.Enable(trackLoaded);
        e.SetText(closeLabel);
    }, closeId);
    using var repeatState = frame.Bind(WxEvents.UpdateUI, (_, e) => e.Check(repeating), checkItem.Id);

    frame.DoMenuUpdates();
    if (closeItem.Enabled || checkItem.Checked)
        throw new InvalidOperationException(
            "Update-UI did not disable the command its handler said was unavailable.");

    trackLoaded = true;
    repeating = true;
    closeLabel = "&Stop\tCtrl+W";
    frame.DoMenuUpdates();
    if (!closeItem.Enabled || !checkItem.Checked)
        throw new InvalidOperationException(
            "Update-UI did not follow the state its handlers read after that state changed.");
    if (!closeItem.Label.StartsWith("&Stop", StringComparison.Ordinal))
        throw new InvalidOperationException($"Update-UI did not relabel the command: '{closeItem.Label}'.");

    // Disposing the binding stops the answering, and wx keeps the last answer.
    repeatState.Dispose();
    repeating = false;
    frame.DoMenuUpdates();
    if (!checkItem.Checked)
        throw new InvalidOperationException("Disposing an update-UI binding should stop it answering.");
    var statusBar = new StatusBar(frame);
    statusBar.SetText(expected);
    var layout = new BoxSizer(Orientation.Vertical);
    layout.Add(label, flags: SizerFlags.All | SizerFlags.AlignCenterHorizontal, border: 8);
    layout.Add(multiline, proportion: 1, flags: SizerFlags.Expand | SizerFlags.All, border: 2);
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

    // wxWindow.Name is what the platform's accessibility bridge reports as the accessible name.
    label.Name = expected;
    if (label.Name != expected) throw new InvalidOperationException("Window name did not round-trip.");
    if (Wx.SupportsCustomAccessibility)
    {
        label.Accessible = new SmokeAccessible(expected);
        if (!label.Accessible.ValidateBridge()) throw new InvalidOperationException("Accessible reverse callback bridge failed.");
        Accessible.NotifyEvent(AccessibleEvent.NameChanged, label);
    }
    frame.Show();

    // Unbinding must release the native hook as well as the managed subscriber.
    var afterUnbind = 0;
    void CountMotion(object? sender, MouseEventArgs e) => afterUnbind++;
    panel.MouseMove += CountMotion;
    panel.MouseMove -= CountMotion;

    // A typed `event` accessor and an explicit Bind are the same subscriber list, and a programmatic value
    // change on a CustomSlider must reach both - a plain wxSlider says nothing here, which is exactly the
    // silence a screen reader would otherwise get. The first handler skips so the second one runs; without
    // that it would consume the event, as any wxWidgets handler does.
    var sliderNotifications = 0;
    var slider = new CustomSlider(panel, value: 5, maxValue: 20);
    slider.ValueChanged += (_, e) => { sliderNotifications++; e.Skip(); };
    using var sliderBinding = slider.Bind(WxEvents.SliderChanged, (_, _) => sliderNotifications++);
    slider.Value = 12;
    if (slider.Value != 12 || sliderNotifications != 2)
        throw new InvalidOperationException(
            $"A programmatic slider change notified {sliderNotifications} of 2 subscribers.");
    slider.Destroy();

    // ---- Keyboard: the generated Key enum, checked against wxWidgets' own accelerator parser rather
    // than against itself, so a wrong code cannot agree with a wrong expectation.
    foreach (var (text, code) in new[] { ("F24", Key.F24), ("Delete", Key.Delete), ("Home", Key.Home),
                                         ("Page Up", Key.PageUp), ("Num Enter", Key.NumpadEnter) })
    {
        if (!AcceleratorEntry.TryParse("Ctrl+" + text, closeId, out var accel) || accel.KeyCode != (int)code)
            throw new InvalidOperationException($"Key.{code} does not match what wxWidgets parses for '{text}'.");
    }
    if ((int)Key.Enter != 13 || (int)Key.None != 0 || (int)Key.WindowsLeft == 0 || (int)Key.MediaPlayPause == 0)
        throw new InvalidOperationException("The generated key table is missing values it should have.");

    // ---- wxListCtrl: columns, and focus as a thing separate from selection.
    list.InsertColumn(1, "Size", 80);
    if (list.ColumnCount != 2 || list.GetColumnHeading(0) != "Name")
        throw new InvalidOperationException("List columns did not round-trip.");
    list.SetColumnHeading(1, expected);
    if (list.GetColumnHeading(1) != expected)
        throw new InvalidOperationException("A UTF-8 column heading did not round-trip.");
    if (!list.AutoSizeColumn(0) || list.GetColumnWidth(0) <= 0)
        throw new InvalidOperationException("Column auto-sizing failed.");
    var secondRow = list.AddItem("second");
    list.SetSelected(listItem);
    list.SetFocused(secondRow);
    list.EnsureVisible(secondRow);
    if (list.SelectedCount != 1 || list.SelectedIndex != listItem || list.FocusedIndex != secondRow)
        throw new InvalidOperationException(
            $"Selection and focus are not independent: selected {list.SelectedIndex}, focused {list.FocusedIndex}.");
    if (list.GetSelectedIndices() is not [var onlySelected] || onlySelected != listItem)
        throw new InvalidOperationException("The selection walk did not return exactly the selected row.");
    if (!list.RemoveColumn(1) || list.ColumnCount != 1)
        throw new InvalidOperationException("Removing a column failed.");

    // ---- wxTreeCtrl: walking the tree.
    var second = tree.Add(root, "second");
    var inserted = tree.Insert(root, 0, "first");
    if (tree.GetChildCount(root) != 3 || tree.GetParent(child) != root)
        throw new InvalidOperationException("Tree parentage or child count is wrong.");
    if (tree.GetFirstChild(root) != inserted || tree.GetNextSibling(inserted) != child ||
        tree.GetPreviousSibling(second) != child)
        throw new InvalidOperationException("Tree sibling order does not match the insertion order.");
    if (tree.GetChildren(root) is not [_, _, _])
        throw new InvalidOperationException("Enumerating tree children returned the wrong count.");
    tree.EnsureVisible(child);
    tree.Unselect();

    // ---- A virtual list asks for the rows it is drawing rather than storing them, so the count can be
    // far larger than anything held in memory.
    var virtualList = new CountingListCtrl(panel);
    virtualList.InsertColumn(0, "Row", 120);
    virtualList.SetItemCount(1_000_000);
    virtualList.RefreshItems(0, 9);
    if (virtualList.Count != 1_000_000)
        throw new InvalidOperationException($"A virtual list reported {virtualList.Count} rows.");
    if (virtualList.GetItem(7) != "row 7")
        throw new InvalidOperationException(
            $"The virtual list did not ask for its text: got '{virtualList.GetItem(7)}'.");
    if (virtualList.Asked == 0)
        throw new InvalidOperationException("OnGetItemText was never called.");
    virtualList.Destroy();

    // ---- A three-state check box can actually report its third state.
    var triState = new CheckBox(panel, label: "Tri", style: CheckBoxStyle.ThreeState);
    if (!triState.IsThreeState || triState.State != CheckBoxState.Unchecked)
        throw new InvalidOperationException("A three-state check box did not start unchecked.");
    triState.State = CheckBoxState.Undetermined;
    if (triState.State != CheckBoxState.Undetermined)
        throw new InvalidOperationException("The indeterminate state did not round-trip.");
    triState.State = CheckBoxState.Checked;
    if (triState.State != CheckBoxState.Checked || !triState.Checked)
        throw new InvalidOperationException("The checked state did not round-trip.");

    // A two-state box refuses the third state rather than asserting inside wxWidgets.
    var twoState = new CheckBox(panel, label: "Two");
    twoState.State = CheckBoxState.Undetermined;
    if (twoState.IsThreeState || twoState.State == CheckBoxState.Undetermined)
        throw new InvalidOperationException("A two-state check box accepted the indeterminate state.");
    triState.Destroy();
    twoState.Destroy();

    // ---- wxComboBox: the item operations it was missing.
    combo.Insert("inserted", 0);
    if (combo.Count != 2 || combo[0] != "inserted" || combo.IndexOf("second") != 1)
        throw new InvalidOperationException(
            $"Combo box item insertion or lookup failed: {combo.Count} items, first is '{combo[0]}'.");
    combo[0] = expected;
    if (combo[0] != expected) throw new InvalidOperationException("Combo box item text did not round-trip.");
    combo.RemoveAt(0);
    if (combo.Count != 1) throw new InvalidOperationException("Combo box item removal failed.");

    // ---- wxTextCtrl: lines.
    if (multiline.LineCount != 3 || multiline.GetLineText(1) != "two" || multiline.GetLineLength(2) != 5)
        throw new InvalidOperationException(
            $"Line access failed: {multiline.LineCount} lines, line 1 is '{multiline.GetLineText(1)}'.");
    if (multiline.GetLineText(9) != string.Empty || multiline.GetLineLength(9) != -1)
        throw new InvalidOperationException("Out-of-range line access should be empty, not an error.");
    multiline.ScrollToEnd();

    // ---- Dialog styles and the platform's own button row.
    using (var dialog = new Dialog(frame, title: expected,
        style: DialogStyle.Default | DialogStyle.ResizeBorder))
    {
        var buttons = dialog.CreateButtonSizer(ButtonSizerFlags.OkCancel);
        if (buttons is null) throw new InvalidOperationException("The standard button row was not built.");
        var dialogLayout = new BoxSizer(Orientation.Vertical);
        dialogLayout.Add(new StaticText(dialog, label: expected), flags: SizerFlags.All, border: 8);
        dialogLayout.Add(buttons, flags: SizerFlags.Expand | SizerFlags.All, border: 8);
        dialog.SetSizer(dialogLayout);
        if (dialog.Title != expected) throw new InvalidOperationException("Dialog title did not round-trip.");
    }

    // Handler order and consumption: the second handler runs only because the first skips, and the third
    // never runs because the second does not.
    var order = new List<string>();
    var consumeProbe = new Panel(panel);
    using var probeFirst = consumeProbe.Bind(WxEvents.ContextMenu, (_, e) => { order.Add("first"); e.Skip(); });
    using var probeSecond = consumeProbe.Bind(WxEvents.ContextMenu, (_, _) => order.Add("second"));
    using var probeThird = consumeProbe.Bind(WxEvents.ContextMenu, (_, e) => { order.Add("third"); e.Skip(); });

    var closeAttempts = 0;
    var boundCloseAttempts = 0;
    using var closeBinding = frame.Bind(WxEvents.Closing, (_, e) => { boundCloseAttempts++; e.Skip(); });
    frame.Closing += (_, e) =>
    {
        closeAttempts++;
        // wxWidgets' model, which the wrapper follows: an event is handled unless the handler skips it, so
        // returning without Skip() here would consume the close and the frame would never go away.
        if (closeAttempts == 1) { e.Veto(); Wx.CallAfter(frame.Close); }
        else e.Skip();
    };

    var timerTicks = 0;
    var timer = new WxSharp.Timer(frame);
    timer.Tick += (_, _) => { timerTicks++; timer.Stop(); frame.Close(); };

    // Queue from a worker to verify thread-safe UI marshaling and a genuinely blocking native MainLoop.
    Task.Run(() => Wx.CallAfter(() => timer.Start(10))).GetAwaiter().GetResult();
    app.MainLoop();
    VerifyLifecycle(app);
    if (afterUnbind != 0)
        throw new InvalidOperationException("A handler kept receiving events after it was unsubscribed.");
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

sealed class CountingListCtrl(Window parent) : ListCtrl(parent, style: ListCtrlStyle.Report | ListCtrlStyle.Virtual)
{
    public int Asked { get; private set; }

    protected override string OnGetItemText(long item, int column)
    {
        Asked++;
        return $"row {item}";
    }
}

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
