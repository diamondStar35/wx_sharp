using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using WxSharp;

internal static class Program
{
    // A wxWidgets application has to run in a single-threaded apartment on Windows, the same as any
    // other GUI toolkit there, so that OLE comes up and the clipboard works.
    [STAThread]
    private static void Main(string[] args)
    {
        try
        {
            Run(args);
        }
        catch (Exception error)
        {
            // Do not let Windows Error Reporting turn an ordinary failed assertion into an interactive
            // crash dialog and several seconds of desktop input blocking.
            Console.Error.WriteLine($"Smoke test failed: {error}");
            Environment.ExitCode = 1;
        }
    }

    private static void Run(string[] args)
    {

Environment.SetEnvironmentVariable("WXSHARP_TEST_NONINTERACTIVE", "1");

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
    Wx.CallAfter(() => frame.Close());
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
    // Overridable wxWidgets virtuals. A control that refuses keyboard focus is how an accessible
    // application keeps a button reachable by mouse and shortcut without putting it in the tab order; it
    // only works if wxWidgets' own AcceptsFocusFromKeyboard reaches managed code. The unoverridden members
    // must still answer as wxWidgets does, which is what proves the base implementation is reached rather
    // than the override standing in for all of them.
    var plainButton = new Button(panel, label: "Plain");
    var skipButton = new UnfocusableButton(panel);
    if (!plainButton.AcceptsFocusFromKeyboard())
        throw new InvalidOperationException("An ordinary button should accept keyboard focus.");
    if (skipButton.AcceptsFocusFromKeyboard())
        throw new InvalidOperationException("The override refusing keyboard focus did not reach wxWidgets.");
    if (!skipButton.AcceptsFocus())
        throw new InvalidOperationException("An unoverridden virtual should still answer as wxWidgets does.");
    if (!skipButton.BaseWasReached)
        throw new InvalidOperationException("Calling the base implementation from an override did not reach wxWidgets.");

    // The wider virtual set: a value return, a point return, a void hook and an argument-carrying member,
    // each proving its own shape of the callback crosses correctly.
    var sized = new SizedPanel(frame);
    if (sized.BestSize != new Size(123, 45))
        throw new InvalidOperationException("An overridden best size is not what wxWidgets reports.");
    if (sized.ClientAreaOriginBase != new Point(0, 0))
        throw new InvalidOperationException("GetClientAreaOrigin's base implementation was not reached.");
    sized.Enabled = false;
    if (!sized.EnableSeen)
        throw new InvalidOperationException("DoEnable did not reach the managed override.");
    if (sized.Enabled)
        throw new InvalidOperationException("DoEnable's base implementation did not disable the window.");
    sized.Enabled = true;
    if (sized.InheritsColours || plainButton.ShouldInheritColours() || !label.ShouldInheritColours())
        throw new InvalidOperationException("ShouldInheritColours did not answer from wxWidgets.");

    // A posted command event: the only way to observe propagation and vetoing without a user present, and
    // what an application uses to raise its own commands.
    var postedId = IdManager.NewId();
    var postedOnPanel = 0;
    var postedOnFrame = 0;
    // A separate ID for the queued event: a handler that does not skip consumes the event, so sharing an
    // ID with the propagation checks above would make this handler swallow theirs.
    var queuedId = IdManager.NewId();
    var postedLater = 0;
    var postedValue = 0;
    frame.Bind(WxEvents.MenuCommand, (_, e) => { postedLater++; postedValue = e.Value; }, queuedId);
    using (panel.Bind(WxEvents.MenuCommand, (_, e) => { postedOnPanel++; e.Skip(); }, postedId))
    using (frame.Bind(WxEvents.MenuCommand, (_, _) => postedOnFrame++, postedId))
    {
        // Handled immediately: the panel's handler skips, so it travels up the real parent chain.
        if (!Wx.ProcessEvent(panel, WxEvents.MenuCommand, postedId, value: 7))
            throw new InvalidOperationException("A processed command event was not handled.");
        if (postedOnPanel != 1 || postedOnFrame != 1)
            throw new InvalidOperationException("A processed command event did not propagate to the parent.");
    }
    // A vetoed command: the panel's handler does not skip, so the frame never sees it.
    using (panel.Bind(WxEvents.MenuCommand, (_, _) => { }, postedId))
    using (frame.Bind(WxEvents.MenuCommand, (_, _) => postedOnFrame++, postedId))
    {
        Wx.ProcessEvent(panel, WxEvents.MenuCommand, postedId);
        if (postedOnFrame != 1)
            throw new InvalidOperationException("A command event a handler consumed still reached the parent.");
    }

    // Queued rather than processed: it must not have run by the time this returns, and must run once the
    // event loop gets to it. The count is checked after MainLoop.
    Wx.PostEvent(frame, WxEvents.MenuCommand, queuedId, value: 11);
    if (postedLater != 0)
        throw new InvalidOperationException("PostEvent dispatched inside the call instead of queueing.");

    // ProgressDialog reports cancelling and skipping separately, which a copy loop needs to tell apart.
    using (var progress = new ProgressDialog("Working", "Step 1", 10, frame,
        ProgressDialogStyle.Default | ProgressDialogStyle.CanAbort | ProgressDialogStyle.CanSkip))
    {
        var step = progress.Update(1, "Step 2");
        if (!step.Continue || step.Skipped)
            throw new InvalidOperationException("An untouched progress dialog reported a cancel or a skip.");
        if (progress.Range != 10)
            throw new InvalidOperationException("ProgressDialog did not report the range it was given.");
        progress.Range = 20;
        if (progress.Range != 20 || progress.WasCancelled || progress.WasSkipped)
            throw new InvalidOperationException("ProgressDialog range or state round-trip failed.");
    }

    if (!frame.Enabled)
        throw new InvalidOperationException("An app-modal progress dialog left the application disabled.");

    // Window.Font: the read-modify-write a heading does. A real wxFont carries the numeric weight, the
    // encoding, strikethrough and pixel sizes, none of which the old flattened description could hold.
    var heading = new StaticText(panel, label: "Heading");
    using (var headingFont = heading.Font)
    {
        if (headingFont.PointSize <= 0)
            throw new InvalidOperationException("Window.Font reported no point size.");
        if (headingFont.NumericWeight != (int)FontWeight.Normal)
            throw new InvalidOperationException("A default font did not report wxWidgets' normal weight.");
        using var bold = headingFont.Bold();
        heading.Font = bold;
    }
    using (var applied = heading.Font)
    {
        // 700 rather than a three-point enum: the weight survives as the number wxWidgets actually keeps.
        if (applied.NumericWeight != 700)
            throw new InvalidOperationException("A bolded font did not survive the round trip.");
        if (applied.Weight != FontWeight.Bold)
            throw new InvalidOperationException("The nearest conventional weight was misreported.");
    }

    // The font enums carry wxWidgets' own values rather than being mapped on the way across, so every one
    // has to survive a round trip through wxWidgets itself. wxFontStyle is the cautionary case: its values
    // come from a deprecated constant block where wxLIGHT and wxBOLD sit between normal and italic, so the
    // obvious guess at them is wrong - and wrong quietly, until wxWidgets asserts from inside SetStyle.
    //
    // A few answers differ from what was asked for, and those are wxWidgets resolving a request rather than
    // a wrong value: Default picks a real family, Teletype and Modern are the same family on MSW, and Slant
    // is only distinct from Italic on platforms that have a slanted face.
    foreach (var family in Enum.GetValues<FontFamily>())
    {
        if (family is FontFamily.Unknown or FontFamily.Default or FontFamily.Teletype) continue;
        using var probe = new Font(new FontInfo(10).Family(family));
        if (probe.Family != family)
            throw new InvalidOperationException($"FontFamily.{family} is not the value wxWidgets uses.");
    }
    using (var defaulted = new Font(new FontInfo(10).Family(FontFamily.Default)))
    {
        if (defaulted.Family == FontFamily.Default || defaulted.Family == FontFamily.Unknown)
            throw new InvalidOperationException("The default family did not resolve to a real one.");
    }
    foreach (var fontStyle in Enum.GetValues<FontStyle>())
    {
        using var probe = new Font(new FontInfo(10).Style(fontStyle));
        var resolved = fontStyle == FontStyle.Slant ? FontStyle.Italic : fontStyle;
        if (probe.Style != resolved && probe.Style != fontStyle)
            throw new InvalidOperationException($"FontStyle.{fontStyle} is not the value wxWidgets uses.");
    }
    foreach (var weight in Enum.GetValues<FontWeight>())
    {
        if (weight == FontWeight.Invalid) continue;
        using var probe = new Font(new FontInfo(10).Weight(weight));
        if (probe.NumericWeight != (int)weight)
            throw new InvalidOperationException($"FontWeight.{weight} is not the value wxWidgets uses.");
    }

    // The parts the old six-scalar font could not carry at all.
    using (var detailed = new Font(new FontInfo(11.5).Family(FontFamily.Modern).Weight(FontWeight.SemiBold)
        .Italic().Strikethrough().FaceName("Consolas")))
    {
        if (Math.Abs(detailed.FractionalPointSize - 11.5) > 0.01)
            throw new InvalidOperationException("A fractional point size was lost.");
        if (detailed.NumericWeight != 600 || detailed.Style != FontStyle.Italic || !detailed.IsStrikethrough)
            throw new InvalidOperationException("A font description did not round-trip.");
        if (detailed.FaceName != "Consolas" || !detailed.IsFixedWidth)
            throw new InvalidOperationException("A fixed-width face was not reported as one.");

        // The platform's own description is what a settings file should store, so it has to round-trip.
        using var restored = Font.FromNativeInfo(detailed.NativeFontInfo)
            ?? throw new InvalidOperationException("A native font description would not parse back.");
        if (!restored.Equals(detailed))
            throw new InvalidOperationException("A font did not survive its native description.");
    }

    // Pixel sizing, and the derivations that leave the original alone.
    using (var pixels = new Font(new FontInfo(new Size(0, 20))))
    {
        if (!pixels.IsUsingSizeInPixels || pixels.PixelSize.Height != 20)
            throw new InvalidOperationException("A pixel-sized font did not keep its size.");
    }
    using (var basis = new Font(12, FontFamily.Swiss))
    using (var larger = basis.Larger())
    {
        if (larger.PointSize <= basis.PointSize)
            throw new InvalidOperationException("Larger() did not grow the font.");
        if (basis.PointSize != 12)
            throw new InvalidOperationException("A derivation changed the font it came from.");
        basis.MakeBold();
        if (basis.NumericWeight != 700)
            throw new InvalidOperationException("MakeBold did not change the font in place.");
    }

    // The platform's own fonts, which a themed interface has to start from.
    using (var gui = SystemSettings.GetFont(SystemFont.DefaultGui))
    {
        if (!gui.IsOk || gui.PointSize <= 0)
            throw new InvalidOperationException("The system GUI font came back unusable.");
    }

    // A canvas now measures in the font it will draw with, rather than in the control's.
    var measureCanvas = new Canvas(panel);
    using (var bigFont = new Font(30, FontFamily.Swiss))
    {
        var small = measureCanvas.MeasureText("measure me");
        measureCanvas.SetTextFont(bigFont);
        if (measureCanvas.MeasureText("measure me").Width < small.Width)
            throw new InvalidOperationException("Measuring ignored the font the canvas draws with.");
    }

    // Bulk list replacement, and the item data a list row is tied to what it stands for by.
    var bulkList = new ListBox(panel);
    bulkList.Set(["alpha", "beta", "gamma"]);
    if (bulkList.Count != 3 || bulkList[1] != "beta")
        throw new InvalidOperationException("ListBox.Set did not replace the items.");
    bulkList.SetString(1, "BETA");
    if (bulkList.GetStrings() is not ["alpha", "BETA", "gamma"])
        throw new InvalidOperationException("ListBox.SetString or GetStrings disagreed.");
    bulkList.SelectedIndex = 0;
    bulkList.DeselectAll();
    if (bulkList.SelectedIndex != -1)
        throw new InvalidOperationException("ListBox.DeselectAll left a selection behind.");

    // Tree item data survives a rebuild of the rest of the tree, and is dropped with its item.
    var payload = new object();
    tree.SetItemData(child, payload);
    if (!ReferenceEquals(tree.GetItemData(child), payload))
        throw new InvalidOperationException("TreeCtrl item data did not round-trip.");
    if (tree.Count < 2 || !tree.ItemHasChildren(root))
        throw new InvalidOperationException("TreeCtrl misreported its size or its root's children.");
    tree.ExpandAll();
    tree.CollapseAll();
    var doomed = tree.Add(root, "doomed");
    tree.SetItemData(doomed, payload);
    tree.Remove(doomed);
    if (tree.GetItemData(doomed) is not null)
        throw new InvalidOperationException("TreeCtrl kept a deleted item's data.");

    // Walking a list selection the way wxWidgets does, rather than materialising it.
    list.SetSelected(listItem);
    if (list.GetFirstSelected() != listItem || list.GetNextSelected(listItem) != -1)
        throw new InvalidOperationException("ListCtrl selection walk disagreed with the selection.");
    list.SetItemData(listItem, payload);
    if (!ReferenceEquals(list.GetItemData(listItem), payload))
        throw new InvalidOperationException("ListCtrl item data did not round-trip.");

    // A scrolled panel sized in scroll units, which is how the settings pages are built.
    var scrolled = new ScrolledWindow(panel);
    scrolled.SetScrollbars(10, 10, 40, 40);
    if (scrolled.ScrollPixelsPerUnit != new Size(10, 10))
        throw new InvalidOperationException("ScrolledWindow did not take the scroll rate it was given.");
    scrolled.ShowScrollbars(ScrollbarVisibility.Always, ScrollbarVisibility.Automatic);
    scrolled.EnableScrolling(true, true);
    scrolled.SetScrollPageSize(Orientation.Vertical, 5);
    if (scrolled.GetScrollPageSize(Orientation.Vertical) != 5)
        throw new InvalidOperationException("ScrolledWindow page size did not round-trip.");

    // wxTimer takes any wxEvtHandler, and wxApp is one - so a timer can outlive every window. This is what
    // the EvtHandler base exists for; before it, Timer demanded a Window and App could not Bind at all.
    var appTimerTicks = 0;
    var appTimerId = IdManager.NewId();
    using (var appTimer = new WxSharp.Timer(app, appTimerId))
    {
        appTimer.Tick += (_, _) => appTimerTicks++;
        if (!ReferenceEquals(appTimer.GetOwner(), app))
            throw new InvalidOperationException("A timer did not keep the App as its owner.");
        appTimer.Notify();
        if (appTimerTicks != 1)
            throw new InvalidOperationException("An App-owned timer did not deliver its tick.");
    }

    // Appearance. wxWidgets answers whether it could apply the request, and the two failures mean
    // different things - unsupported here, versus too late to ask - so both are exercised for shape rather
    // than for a particular answer, which depends on the machine's own setting.
    var appearance = app.SetAppearance(Appearance.System);
    if (!Enum.IsDefined(appearance))
        throw new InvalidOperationException($"SetAppearance answered with {(int)appearance}, which is not a result.");

    // MSW dark mode is Windows-only and wxWidgets calls it experimental, so a false is an ordinary answer.
    // What must hold is that the platform's own claim and the call agree with each other.
    var darkEnabled = app.EnableDarkMode();
    if (darkEnabled && !App.SupportsDarkMode)
        throw new InvalidOperationException("Dark mode turned on where the platform claims not to have it.");
    if (!App.SupportsDarkMode && darkEnabled)
        throw new InvalidOperationException("A platform without dark mode reported enabling it.");
    // Whatever happened, the interface still has to answer for its own colours.
    if (SystemSettings.GetColour(SystemColour.Window).A == 0)
        throw new InvalidOperationException("The window colour became unreadable after the appearance request.");

    // Where the platform keeps things. These are the paths an application gets wrong from memory, and on
    // Windows getting them wrong means writing somewhere the user cannot back up.
    if (string.IsNullOrEmpty(StandardPaths.ExecutablePath))
        throw new InvalidOperationException("StandardPaths reported no executable path.");
    if (string.IsNullOrEmpty(StandardPaths.UserConfigDirectory) ||
        string.IsNullOrEmpty(StandardPaths.UserDataDirectory) ||
        string.IsNullOrEmpty(StandardPaths.UserLocalDataDirectory) ||
        string.IsNullOrEmpty(StandardPaths.TempDirectory))
        throw new InvalidOperationException("StandardPaths left a user directory empty.");
    // Roaming and local are different places, which is the distinction the whole type exists for.
    if (StandardPaths.UserDataDirectory == StandardPaths.UserLocalDataDirectory)
        throw new InvalidOperationException("Roaming and local data resolved to the same directory.");
    if (string.IsNullOrEmpty(StandardPaths.GetUserDirectory(UserDirectory.Documents)))
        throw new InvalidOperationException("StandardPaths reported no documents directory.");

    // Displays. A saved window position has to be checked against these before it is restored, because the
    // screen it was on may not be attached now.
    if (Display.Count == 0)
        throw new InvalidOperationException("No displays were reported.");
    var primary = Display.Primary;
    if (!primary.IsPrimary || primary.Geometry.Width <= 0 || primary.ClientArea.Width <= 0)
        throw new InvalidOperationException("The primary display reported an unusable geometry.");
    if (primary.ClientArea.Width > primary.Geometry.Width)
        throw new InvalidOperationException("A display client area was larger than the display.");
    if (primary.ScaleFactor <= 0 || primary.Ppi.Width <= 0)
        throw new InvalidOperationException("The primary display reported no scale or resolution.");
    if (Display.GetAll().Length != Display.Count)
        throw new InvalidOperationException("GetAll did not return every display.");
    // A point far off every screen belongs to none of them, which is the check that matters.
    if (Display.GetFromPoint(new Point(-100000, -100000)) is not null)
        throw new InvalidOperationException("A point off every screen was claimed by a display.");
    if (Display.GetFromWindow(frame) is null)
        throw new InvalidOperationException("A visible frame was on no display.");

    // The platform's own icons, which is what makes a toolbar look native and stay legible in high contrast.
    using (var art = ArtProvider.GetBitmap(ArtId.FileOpen, ArtClient.Toolbar))
    {
        if (art is null || art.Width <= 0)
            throw new InvalidOperationException("The platform supplied no open-file icon.");
    }
    if (ArtProvider.GetNativeSizeHint(ArtClient.Toolbar).Width <= 0)
        throw new InvalidOperationException("The platform suggested no toolbar art size.");

    // Cursors. A window keeps whatever it is given, and null puts the parent's back.
    using (var busy = new Cursor(StockCursor.Wait))
    {
        if (!busy.IsOk) throw new InvalidOperationException("The stock wait cursor did not load.");
        panel.Cursor = busy;
        using var applied = panel.Cursor;
        if (applied is null || !applied.IsOk)
            throw new InvalidOperationException("A window did not keep the cursor it was given.");
        panel.Cursor = null;
    }

    // Image lists are how wxWidgets gives list and tree items their icons - by index into a list the
    // control holds, rather than a bitmap per item.
    var icons = new ImageList(16, 16);
    using (var openIcon = ArtProvider.GetBitmap(ArtId.Folder, ArtClient.List, new Size(16, 16)))
    {
        if (openIcon is not null && icons.Add(openIcon) != 0)
            throw new InvalidOperationException("The first image added did not take index 0.");
    }
    if (icons.Count > 0)
    {
        if (icons.GetSize(0) != new Size(16, 16))
            throw new InvalidOperationException("An image list did not keep its fixed size.");
        // The control takes ownership here, so the list must not be disposed afterwards.
        tree.SetImageList(icons);
        tree.SetItemImage(child, 0);
        if (tree.GetItemImage(child) != 0)
            throw new InvalidOperationException("A tree item did not keep the image it was given.");
    }
    else
    {
        icons.Dispose();
    }

    // A caret is what the platform's input methods and assistive technology follow to find where typing
    // will go, so a custom-drawn control that takes text needs one.
    var caretHost = new Canvas(panel);
    caretHost.SetCaret(new Size(2, 16));
    if (!caretHost.HasCaret)
        throw new InvalidOperationException("A window did not keep the caret it was given.");
    caretHost.MoveCaret(new Point(4, 8));
    if (caretHost.CaretPosition != new Point(4, 8))
        throw new InvalidOperationException("A caret did not move where it was put.");
    caretHost.ShowCaret();
    if (Window.CaretBlinkTime < 0)
        throw new InvalidOperationException("The caret blink rate was reported as negative.");
    caretHost.SetCaret(new Size(0, 0));
    if (caretHost.HasCaret)
        throw new InvalidOperationException("Sizing a caret to nothing did not remove it.");

    // Sound is deliberately small - one format, no position or volume - so this checks the refusal path,
    // which is the one an application actually has to handle.
    // Windows hands the path to the system without checking it, so neither loading nor playing a missing
    // file reports failure - which is why Sound documents that success is not proof the file exists.
    // What does hold is that both calls are safe to make and that stopping is always allowed.
    _ = Sound.Play("no such file.wav");
    Sound.Stop();

    // Listing the faces the platform has - the only way to know a face exists before asking for it, since
    // assigning a missing one leaves the font unchanged.
    var faces = FontEnumerator.GetFacenames();
    if (faces.Length == 0)
        throw new InvalidOperationException("No installed typefaces were enumerated.");
    if (!FontEnumerator.IsValidFacename(faces[0]))
        throw new InvalidOperationException("An enumerated face was not considered valid.");
    if (FontEnumerator.IsValidFacename("no such typeface exists"))
        throw new InvalidOperationException("A face nothing installs was considered valid.");
    if (FontEnumerator.GetFacenames(fixedWidthOnly: true).Length > faces.Length)
        throw new InvalidOperationException("More fixed-width faces than faces.");

    // The virtuals that live on one class rather than on wxWindow. Each is reached the same way the
    // wxWindow set is, but only a subclass of that class carries it.
    var scrolledHooks = new HookedScrolled(panel);
    if (scrolledHooks.GetSizeAvailableForScrollTarget(new Size(200, 100)) != new Size(111, 22))
        throw new InvalidOperationException("An overridden scroll-target size did not reach wxWidgets.");
    if (scrolledHooks.ShouldScrollToChildOnFocus(panel))
        throw new InvalidOperationException("An overridden focus-scroll refusal did not take effect.");
    if (!scrolledHooks.BaseAnswered)
        throw new InvalidOperationException("Calling the base of a class-specific virtual did not reach wxWidgets.");

    var gridHooks = new HookedGrid(panel);
    if (gridHooks.GetColGridLinePen(0).Colour != Colour.Red)
        throw new InvalidOperationException("An overridden grid line pen did not take effect.");
    if (gridHooks.GetDefaultGridLinePen().Width <= 0)
        throw new InvalidOperationException("The default grid line pen came back from nowhere.");

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
    // Do not deliberately feed invalid syntax to wxAcceleratorEntry here. wxWidgets reports parser
    // failures through its GUI logging machinery on Windows, which can display a modal alert or ring the
    // system bell even though TryParse correctly returns false. Invalid-input parser coverage belongs in a
    // native test with a captured log target, not in an interactive GUI smoke process.
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
        // Do not broadcast a synthetic system accessibility notification from the default smoke test.
        // The reverse callback bridge above verifies the wrapper without disturbing desktop services.
    }

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

    // ---- wxTextEntry: the editing surface three controls share, reached through each of them.
    foreach (ITextEntry entry in new ITextEntry[] { multiline, combo, search })
    {
        var name = entry.GetType().Name;
        entry.ChangeValue("abcdef");
        if (entry.Value != "abcdef")
            throw new InvalidOperationException($"{name}: ChangeValue did not set the text.");
        if (entry.LastPosition != 6 || entry.IsEmpty)
            throw new InvalidOperationException($"{name}: reported the wrong length.");
        if (entry.GetRange(1, 3) != "bc")
            throw new InvalidOperationException($"{name}: GetRange returned '{entry.GetRange(1, 3)}'.");

        entry.Selection = (1, 3);
        if (!entry.HasSelection || entry.Selection != (1, 3) || entry.GetRange(1, 3) != "bc")
            throw new InvalidOperationException($"{name}: selection did not round-trip.");
        // SelectedText means the selected item on a combo box, which is wxComboBox's own resolution of
        // inheriting GetStringSelection from two bases.
        if (entry is not ComboBox && entry.SelectedText != "bc")
            throw new InvalidOperationException($"{name}: SelectedText was '{entry.SelectedText}'.");
        entry.SelectNone();
        if (entry.HasSelection)
            throw new InvalidOperationException($"{name}: SelectNone left a selection.");

        entry.Replace(0, 1, "X");
        if (entry.Value != "Xbcdef")
            throw new InvalidOperationException($"{name}: Replace produced '{entry.Value}'.");
        entry.Remove(0, 1);
        if (entry.Value != "bcdef")
            throw new InvalidOperationException($"{name}: Remove produced '{entry.Value}'.");

        entry.MoveCaretToEnd();
        if (entry.InsertionPoint != entry.LastPosition)
            throw new InvalidOperationException($"{name}: the caret did not move to the end.");
        entry.InsertionPoint = 0;
        entry.Write("Z");
        if (!entry.Value.StartsWith('Z'))
            throw new InvalidOperationException($"{name}: Write did not insert at the caret.");

        entry.Append("!");
        if (!entry.Value.EndsWith('!'))
            throw new InvalidOperationException($"{name}: Append did not add at the end.");

        // Reading the clipboard state must not throw, whatever the clipboard holds.
        _ = entry.CanCopy; _ = entry.CanCut; _ = entry.CanPaste; _ = entry.CanUndo; _ = entry.CanRedo;
        entry.MaxLength = 64;
        _ = entry.Margins;

        entry.Clear();
        if (!entry.IsEmpty)
            throw new InvalidOperationException($"{name}: Clear left text behind.");
    }

    // Clear on a combo box empties the list as well as the field, because wxComboBox resolves the two
    // inherited Clear methods to one that does both. Put the item back for the list tests further down.
    if (combo.Count != 0)
        throw new InvalidOperationException($"Combo box Clear left {combo.Count} items in the list.");
    combo.Add("second");

    // ChangeValue is the one that does not raise the event - the whole reason it exists.
    var textChanges = 0;
    using (var watcher = multiline.Bind(WxEvents.TextChanged, (_, _) => textChanges++))
    {
        multiline.ChangeValue("quiet");
        if (textChanges != 0)
            throw new InvalidOperationException("ChangeValue raised a text-changed event.");
        multiline.Value = "loud";
        if (textChanges != 1)
            throw new InvalidOperationException($"Setting Value raised {textChanges} events, expected 1.");
    }
    multiline.ChangeValue("one\ntwo\nthree");

    // Editable is shared too, and a read-only field still reports its text.
    search.Editable = false;
    if (search.Editable) throw new InvalidOperationException("A field stayed editable.");
    search.Editable = true;

    // ---- The rest of wxTextCtrl: the modified flag, coordinates, and styling.
    var singleLine = new TextCtrl(panel, value: "one line");
    if (!multiline.IsMultiLine || singleLine.IsMultiLine)
        throw new InvalidOperationException("A text control misreported whether it is multi-line.");

    multiline.DiscardEdits();
    if (multiline.IsModified)
        throw new InvalidOperationException("DiscardEdits left the control modified.");
    multiline.MarkDirty();
    if (!multiline.IsModified)
        throw new InvalidOperationException("MarkDirty did not set the modified flag.");
    multiline.IsModified = false;
    if (multiline.IsModified)
        throw new InvalidOperationException("Clearing IsModified did not take.");

    // The control holds three lines, so the start of the second is column 0, line 1.
    var secondLine = multiline.XYToPosition(0, 1);
    if (secondLine < 0 || !multiline.PositionToXY(secondLine, out var atColumn, out var atLine))
        throw new InvalidOperationException("A text position did not convert to coordinates.");
    if (atColumn != 0 || atLine != 1)
        throw new InvalidOperationException($"Position {secondLine} reported as column {atColumn}, line {atLine}.");
    _ = multiline.HitTest(new Point(2, 2), out _);

    // Styling needs a rich control, so take the answer the platform gives rather than insisting on one.
    var style = new TextAttr { TextColour = Colour.Red, Alignment = TextAttrAlignment.Right };
    if (!style.Has(TextAttrFlags.TextColour) || style.Has(TextAttrFlags.BackgroundColour))
        throw new InvalidOperationException("TextAttr recorded the wrong set of overridden properties.");
    if (multiline.SetStyle(0, 3, style))
    {
        var readBack = multiline.GetStyle(1);
        if (readBack is not null && readBack.Has(TextAttrFlags.TextColour) && readBack.TextColour != Colour.Red)
            throw new InvalidOperationException($"A character style read back as {readBack.TextColour}.");
    }
    _ = multiline.DefaultStyle;

    // ---- Colour names, which is how a theme or a config file spells a colour.
    if (!Colour.TryParse("red", out var parsedByName) || parsedByName != Colour.Red)
        throw new InvalidOperationException($"'red' parsed as {parsedByName}.");
    if (!Colour.TryParse("#204080", out var parsedByHex) || parsedByHex != new Colour(0x20, 0x40, 0x80))
        throw new InvalidOperationException($"'#204080' parsed as {parsedByHex}.");
    if (Colour.TryParse("not a colour at all", out _))
        throw new InvalidOperationException("A meaningless string parsed as a colour.");
    if (Colour.Red.ToName().Length == 0)
        throw new InvalidOperationException("A colour had no name at all.");

    // The transforms a themed interface uses: dimming for a disabled control, and lightening or darkening
    // to derive a hover or selection colour that still contrasts.
    if (Colour.White.Luminance <= Colour.Black.Luminance)
        throw new InvalidOperationException("White did not read as brighter than black.");
    if (Colour.Red.ChangeLightness(100) != Colour.Red)
        throw new InvalidOperationException("ChangeLightness(100) did not leave the colour alone.");
    if (Colour.Black.ChangeLightness(160).Luminance <= Colour.Black.Luminance)
        throw new InvalidOperationException("Lightening black did not brighten it.");
    var grey = new Colour(200, 40, 40).MakeGrey();
    if (grey.R != grey.G || grey.G != grey.B)
        throw new InvalidOperationException($"MakeGrey produced {grey}.");
    if (new Colour(200, 40, 40).MakeMono(false) != Colour.Black)
        throw new InvalidOperationException("MakeMono(false) was not black.");
    _ = Colour.Red.MakeDisabled();
    if (Colour.AlphaBlend(255, 0, 1.0) != 255 || Colour.AlphaBlend(255, 0, 0.0) != 0)
        throw new InvalidOperationException("AlphaBlend did not respect its end points.");
    if (!Colour.White.IsOpaque || !Colour.Transparent.IsTransparent || Colour.White.IsTranslucent)
        throw new InvalidOperationException("A colour misreported its transparency.");

    // ---- The rest of wxFrame: the frame-owned bars and geometry. Minimizing, maximizing, flashing the
    // taskbar and requesting attention are deliberately not automated: they manipulate the user's desktop.
    if (frame.IsIconized || frame.IsMaximized || frame.IsFullScreen)
        throw new InvalidOperationException("A freshly shown frame reported an unexpected window state.");

    _ = frame.IsActive;
    _ = frame.IsAlwaysMaximized;
    _ = frame.EnableMinimizeButton(true);
    _ = frame.EnableMaximizeButton(true);
    frame.CentreOnScreen();
    if (Frame.DefaultSize.Width <= 0)
        throw new InvalidOperationException("The default frame size had no width.");

    // The frame hands back the same wrapper for the bar it already owns, rather than a second one around
    // the same native object.
    if (!ReferenceEquals(frame.StatusBar, statusBar))
        throw new InvalidOperationException("The frame did not hand back the status bar it was given.");
    if (frame.ToolBar is not null)
        throw new InvalidOperationException("A frame with no toolbar claimed to have one.");

    frame.SetStatusText("ready");
    if (statusBar.GetText() != "ready")
        throw new InvalidOperationException($"The status field read back as '{statusBar.GetText()}'.");
    frame.PushStatusText("busy");
    if (statusBar.GetText() != "busy")
        throw new InvalidOperationException("PushStatusText did not replace the field text.");
    frame.PopStatusText();
    if (statusBar.GetText() != "ready")
        throw new InvalidOperationException("PopStatusText did not restore the field text.");
    frame.SetStatusWidths(-1);
    frame.StatusBarPane = 0;
    if (frame.StatusBarPane != 0)
        throw new InvalidOperationException("StatusBarPane did not round-trip.");

    if (frame.GetMenuBar() is null)
        throw new InvalidOperationException("The frame did not report the menu bar it was given.");
    if (frame.FindItemInMenuBar(closeId) is null)
        throw new InvalidOperationException("FindItemInMenuBar did not find a menu item by its command ID.");

    // Geometry saves to an opaque string and comes back; wxWidgets decides what is in it.
    var geometry = frame.SaveGeometry();
    if (geometry is null || geometry.Length == 0)
        throw new InvalidOperationException("SaveGeometry reported nothing to save.");
    if (!frame.RestoreToGeometry(geometry))
        throw new InvalidOperationException("RestoreToGeometry rejected what SaveGeometry produced.");

    // ---- The wxWidgets free functions.
    // Launching is not exercised for real: opening the user's browser mid-test would be rude, and a bad URL
    // is refused before anything is launched.
    try
    {
        Wx.LaunchDefaultBrowser("   ");
        throw new InvalidOperationException("A blank URL was accepted.");
    }
    catch (ArgumentException) { }
    try
    {
        Wx.LaunchDefaultApplication("");
        throw new InvalidOperationException("A blank path was accepted.");
    }
    catch (ArgumentException) { }

    _ = Wx.GetKeyState((int)Key.Shift);
    var pointer = Wx.GetMouseState();
    if (pointer.Position != Wx.GetMousePosition())
        throw new InvalidOperationException("The mouse state and position disagreed about where the pointer is.");

    // Machine and user facts. These vary per machine, so the assertions are about shape, not value.
    if (Wx.OsDescription.Length == 0)
        throw new InvalidOperationException("The OS described itself as nothing.");
    var (osId, osMajor, _, _) = Wx.GetOsVersion();
    if (osId == OperatingSystemId.Unknown || osMajor <= 0)
        throw new InvalidOperationException($"The OS reported as {osId} version {osMajor}.");
    if (!Wx.CheckOsVersion(1))
        throw new InvalidOperationException("The OS claimed to be older than version 1.");
    if (Wx.LibraryVersion.Length == 0)
        throw new InvalidOperationException("wxWidgets did not report its version.");
    if (Wx.ProcessId != (uint)Environment.ProcessId)
        throw new InvalidOperationException("wxWidgets and .NET disagreed about the process ID.");
    if (Wx.HostName.Length == 0 || Wx.UserId.Length == 0 || Wx.HomeDirectory.Length == 0)
        throw new InvalidOperationException("The machine would not say who or where it is.");
    _ = Wx.UserName;
    _ = Wx.EmailAddress;
    _ = Wx.FullHostName;
    _ = Wx.IsPlatform64Bit;
    _ = Wx.CpuArchitectureName;
    _ = Wx.NativeCpuArchitectureName;
    _ = Wx.FreeMemory;
    if (Wx.IsPlatformLittleEndian != BitConverter.IsLittleEndian)
        throw new InvalidOperationException("wxWidgets and .NET disagreed about endianness.");
    if (Wx.GetDiskSpace(AppContext.BaseDirectory, out var totalSpace, out _) && totalSpace <= 0)
        throw new InvalidOperationException("A volume reported a total size of zero.");

    // Environment variables: unset and set-to-empty are different answers.
    const string envName = "WXSHARP_SMOKE_VARIABLE";
    if (Wx.GetEnv(envName) is not null)
        throw new InvalidOperationException("A variable that was never set reported a value.");
    if (!Wx.SetEnv(envName, expected))
        throw new InvalidOperationException("Setting an environment variable failed.");
    if (Wx.GetEnv(envName) != expected)
        throw new InvalidOperationException($"The variable read back as '{Wx.GetEnv(envName)}'.");
    // Windows deletes a variable rather than storing an empty one, so setting it to "" is an unset there.
    // GetEnv still tells null from empty - that is what the two answers are for - but on this platform
    // nothing can produce the empty one.
    _ = Wx.SetEnv(envName, "");
    if (OperatingSystem.IsWindows() && Wx.GetEnv(envName) is not null)
        throw new InvalidOperationException("Windows kept a variable that was set to nothing.");
    if (!Wx.UnsetEnv(envName) || Wx.GetEnv(envName) is not null)
        throw new InvalidOperationException("Unsetting an environment variable did not take.");

    // Menu labels, stripped for anywhere they are shown outside a menu.
    if (Wx.StripMenuCodes("E&xit\tCtrl+Q") != "Exit")
        throw new InvalidOperationException(
            $"StripMenuCodes produced '{Wx.StripMenuCodes("E&xit\tCtrl+Q")}'.");

    // Finding a window by name gets back the very wrapper that owns it, not a second one around the same
    // native object. The name has to be a distinctive one: controls of the same kind share a default name,
    // and wxWidgets returns the first match.
    multiline.Name = "smoke-multiline";
    if (!ReferenceEquals(Wx.FindWindowByName("smoke-multiline"), multiline))
        throw new InvalidOperationException("Finding a window by name did not return its own wrapper.");
    if (Wx.FindWindowByName("no window is called this") is not null)
        throw new InvalidOperationException("A name nothing uses matched a window.");
    _ = Wx.GetActiveWindow();
    _ = Wx.FindWindowAtPoint(Wx.GetMousePosition());

    // Disabling everything for a long operation, and letting it go again.
    using (Wx.DisableWindows(frame))
    {
        if (!frame.Enabled)
            throw new InvalidOperationException("The window left out of the disabler was disabled anyway.");
    }
    if (!frame.Enabled)
        throw new InvalidOperationException("The disabler did not re-enable the frame.");

    // ---- Languages: what a settings dialog builds its picker from.
    if (Locale.SystemLanguage == WxSharp.Language.Unknown)
        throw new InvalidOperationException("The system reported no preferred language.");

    // A generated enum is only worth having if its values still line up with the wxWidgets header, so check
    // two known ones rather than trusting the generator.
    var english = Locale.GetLanguageInfo(WxSharp.Language.EnglishUs);
    if (english is null || english.CanonicalName != "en_US")
        throw new InvalidOperationException($"English (US) reported as '{english?.CanonicalName}'.");
    var french = Locale.FindLanguageInfo("fr");
    if (french is null || french.Language != WxSharp.Language.French)
        throw new InvalidOperationException($"'fr' resolved to {french?.Language}.");

    // wxWidgets keeps the two spellings apart: the POSIX lookup splits on underscores and does not
    // understand a dash, and the tag lookup is the other way round. FindLanguage tries both.
    if (Locale.FindLanguageInfo("pt_BR")?.Language != WxSharp.Language.PortugueseBrazilian)
        throw new InvalidOperationException("'pt_BR' did not resolve to Brazilian Portuguese.");
    if (Locale.FindLanguageInfo("pt-BR") is not null)
        throw new InvalidOperationException("The POSIX lookup unexpectedly accepted a BCP 47 tag.");
    if (Locale.FindLanguageInfoByTag("pt-BR")?.Language != WxSharp.Language.PortugueseBrazilian)
        throw new InvalidOperationException("'pt-BR' did not resolve through the tag lookup.");
    foreach (var spelling in new[] { "pt_BR", "pt-BR" })
    {
        if (Locale.FindLanguage(spelling)?.Language != WxSharp.Language.PortugueseBrazilian)
            throw new InvalidOperationException($"FindLanguage did not accept '{spelling}'.");
    }
    if (Locale.FindLanguage("not a language at all") is not null)
        throw new InvalidOperationException("A meaningless string resolved to a language.");

    var arabic = Locale.GetLanguageInfo(WxSharp.Language.Arabic);
    if (arabic is null || arabic.LayoutDirection != LayoutDirection.RightToLeft)
        throw new InvalidOperationException("Arabic did not report a right-to-left layout.");
    if (Locale.GetLanguageName(WxSharp.Language.German).Length == 0)
        throw new InvalidOperationException("German had no name in the database.");
    if (Locale.GetLanguageCanonicalName(WxSharp.Language.German) != "de")
        throw new InvalidOperationException("German did not report the canonical name 'de'.");
    _ = Locale.IsAvailable(WxSharp.Language.EnglishUs);
    _ = Locale.SystemEncodingName;

    // ---- Translation: an untranslated string comes back as itself, which is what makes it safe to wrap
    // every user-visible string from the start.
    Translations.AddCatalogLookupPathPrefix(AppContext.BaseDirectory);
    var translations = new Translations();
    translations.SetLanguage(WxSharp.Language.EnglishUs);
    if (Translations.Get(expected) != expected)
        throw new InvalidOperationException("An untranslated string did not come back unchanged.");
    if (Translations.Get("one file", "many files", 3).Length == 0)
        throw new InvalidOperationException("A plural translation came back empty.");
    if (translations.IsLoaded("wxsharp-no-such-domain"))
        throw new InvalidOperationException("A domain with no catalogue reported itself loaded.");
    if (translations.GetTranslatedString("no catalogue has this string") is not null)
        throw new InvalidOperationException("GetTranslatedString invented a translation.");
    if (translations.GetAvailableTranslations("wxsharp-no-such-domain").Length != 0)
        throw new InvalidOperationException("A domain with no catalogues listed some anyway.");
    // Installing it hands ownership to wxWidgets, so nothing here disposes it.
    Translations.Current = translations;
    if (Translations.Current is null)
        throw new InvalidOperationException("The installed translations did not come back.");

    // wxLocale does the same job and additionally sets the C runtime locale.
    using (var locale = new Locale(WxSharp.Language.EnglishUs, LocaleInitFlags.DontLoadDefault))
    {
        if (locale.Language != WxSharp.Language.EnglishUs)
            throw new InvalidOperationException($"The locale reported {locale.Language}.");
        if (locale.CanonicalName != "en_US")
            throw new InvalidOperationException($"The locale canonical name was '{locale.CanonicalName}'.");
        if (locale.GetString(expected) != expected)
            throw new InvalidOperationException("An untranslated string did not survive the locale.");
        _ = locale.IsOk;
        _ = locale.SystemName;
        if (locale.IsLoaded("wxsharp-no-such-domain"))
            throw new InvalidOperationException("The locale claimed to have loaded a missing catalogue.");
    }

    // ---- The clipboard, round-tripped through the formats it supports.
    //
    // Render each successful write immediately. The smoke test performs a long set of synchronous checks
    // before it starts the event loop; leaving an OLE delayed-rendering object behind during those checks
    // makes unrelated clipboard readers wait for this thread to dispatch messages.
    // Clipboard/OLE integration is opt-in and is invoked below only after MainLoop has started. OLE is
    // allowed to dispatch window messages while rendering delayed clipboard data; doing this in a long,
    // synchronous pre-loop test was unlike a Phoenix application and could stall other desktop clients.
    void RunClipboardRoundTrip()
    {
        if (Clipboard.Open())
        {
            try
            {
                if (!Clipboard.SetText(expected))
                    throw new InvalidOperationException("Clipboard.SetText reported failure.");
                _ = Clipboard.Flush();
                if (!Clipboard.IsSupported(ClipboardFormat.Text) || Clipboard.GetText() != expected)
                    throw new InvalidOperationException($"Clipboard text did not round-trip: '{Clipboard.GetText()}'.");

                string[] clipboardFiles = [@"C:\one.mp3", @"C:\two.flac"];
                if (!Clipboard.SetFiles(clipboardFiles))
                    throw new InvalidOperationException("Clipboard.SetFiles reported failure.");
                _ = Clipboard.Flush();
                var readBack = Clipboard.GetFiles();
                if (readBack.Length != 2 || readBack[1] != clipboardFiles[1])
                    throw new InvalidOperationException("A clipboard file list did not round-trip.");

                if (!Clipboard.SetText(expected))
                    throw new InvalidOperationException("Clipboard.SetText reported failure.");
                _ = Clipboard.Flush();
            }
            finally { Clipboard.Close(); }
        }
        else
        {
            Console.WriteLine("Clipboard was held by another application; skipped the clipboard round-trip.");
        }
    }

    // ---- System settings: what the user's theme says, which is what a themed UI has to follow.
    var windowColour = SystemSettings.GetColour(SystemColour.Window);
    var textColour = SystemSettings.GetColour(SystemColour.WindowText);
    if (windowColour == textColour)
        throw new InvalidOperationException("The theme reported the same colour for window and text.");
    if (SystemSettings.GetMetric(SystemMetric.ScreenX) <= 0)
        throw new InvalidOperationException("The theme reported no screen width.");
    _ = SystemSettings.IsDarkAppearance;
    _ = SystemSettings.ScreenType;

    // ---- The rest of wxWindow: coordinate spaces, freezing, DPI and text metrics.
    frame.Freeze();
    if (!frame.IsFrozen) throw new InvalidOperationException("Freeze did not take.");
    frame.Thaw();
    if (frame.IsFrozen) throw new InvalidOperationException("Thaw did not take.");

    var clientRect = panel.ClientRect;
    if (clientRect.Width <= 0 || panel.Rect.Width <= 0 || panel.ScreenRect.Width <= 0)
        throw new InvalidOperationException("A window reported an empty rectangle.");

    // Client and screen coordinates round-trip through each other.
    var probePoint = new Point(7, 11);
    if (panel.ScreenToClient(panel.ClientToScreen(probePoint)) != probePoint)
        throw new InvalidOperationException("Client and screen coordinates did not round-trip.");

    var extent = label.GetTextExtent("Hello");
    if (extent.Size.Width <= 0 || label.CharHeight <= 0 || label.CharWidth <= 0)
        throw new InvalidOperationException("Text metrics came back empty.");

    // A size in device-independent pixels survives the round trip through this display's scaling.
    if (frame.Dpi.Width <= 0) throw new InvalidOperationException("The window reported no DPI.");
    if (frame.ToDip(frame.FromDip(new Size(100, 40))) != new Size(100, 40))
        throw new InvalidOperationException("DIP conversion did not round-trip.");

    // HelpText goes to a wxHelpProvider, and wxWidgets installs none by default, so it round-trips as
    // empty here. Setting it must still be harmless.
    frame.HelpText = expected;
    panel.MinClientSize = new Size(120, 60);
    if (panel.MinClientSize.Width != 120)
        throw new InvalidOperationException("Minimum client size did not round-trip.");
    panel.Variant = WindowVariant.Small;
    if (panel.Variant != WindowVariant.Small)
        throw new InvalidOperationException("Window variant did not round-trip.");
    if (frame.WindowStyleFlags == 0)
        throw new InvalidOperationException("A frame created with a default style reported no style flags.");
    panel.Raise();
    panel.Lower();

    // ---- Sizers can be changed after they are built, not only appended to.
    var mutable = new Panel(panel);
    var mutableSizer = new BoxSizer(Orientation.Vertical);
    var topLabel = new StaticText(mutable, label: "first");
    var middleLabel = new StaticText(mutable, label: "second");
    var bottomLabel = new StaticText(mutable, label: "third");
    var replacement = new StaticText(mutable, label: "replacement");

    var secondItem = mutableSizer.Add(middleLabel, proportion: 1, flags: SizerFlags.Expand | SizerFlags.All, border: 4);
    mutableSizer.Prepend(topLabel);
    mutableSizer.Add(bottomLabel);
    mutableSizer.InsertSpacer(1, 12);
    mutable.SetSizer(mutableSizer);

    if (mutableSizer.ItemCount != 4 || mutableSizer.IsEmpty)
        throw new InvalidOperationException($"The sizer holds {mutableSizer.ItemCount} items, expected 4.");
    if (!mutableSizer.GetItem(1)!.IsSpacer || !mutableSizer.GetItem(0)!.IsWindow)
        throw new InvalidOperationException("Insert put the spacer in the wrong place.");

    // The item reports back what it was added with, and can be changed afterwards.
    if (secondItem.Proportion != 1 || secondItem.Border != 4 || !secondItem.Flags.HasFlag(SizerFlags.Expand))
        throw new InvalidOperationException(
            $"A sizer item did not report what it was added with: proportion {secondItem.Proportion}, " +
            $"border {secondItem.Border}, flags {secondItem.Flags}.");
    secondItem.Proportion = 2;
    secondItem.Border = 8;
    if (secondItem.Proportion != 2 || secondItem.Border != 8)
        throw new InvalidOperationException("A sizer item did not accept a change.");
    if (mutableSizer.GetItem(middleLabel) is null)
        throw new InvalidOperationException("Looking an item up by window failed.");
    // A sizer item's ID is its own, not the window's, and starts unset.
    secondItem.Id = 4242;
    if (mutableSizer.GetItemById(4242) is null || mutableSizer.GetItemById(middleLabel.Id) is not null)
        throw new InvalidOperationException("GetItemById did not search the item's own ID.");

    // Hiding takes an item out of the layout; the sizer still holds it.
    if (!mutableSizer.Hide(bottomLabel) || mutableSizer.IsShown(bottomLabel))
        throw new InvalidOperationException("Hiding a sizer item failed.");
    if (!mutableSizer.AreAnyItemsShown())
        throw new InvalidOperationException("Hiding one item should not hide the rest.");
    mutableSizer.Show(bottomLabel);

    if (!mutableSizer.Replace(middleLabel, replacement))
        throw new InvalidOperationException("Replacing a window in a sizer failed.");
    if (mutableSizer.GetItem(replacement) is null || mutableSizer.GetItem(middleLabel) is not null)
        throw new InvalidOperationException("Replace did not swap the windows.");

    if (!mutableSizer.Detach(topLabel) || mutableSizer.ItemCount != 3)
        throw new InvalidOperationException("Detaching left the wrong number of items.");
    mutableSizer.Layout();

    var fitted = mutableSizer.ComputeFittingClientSize(mutable);
    if (fitted.Width <= 0 || fitted.Height <= 0)
        throw new InvalidOperationException($"The sizer computed an empty fitting size: {fitted}.");
    if (mutable.GetSizer() != mutableSizer)
        throw new InvalidOperationException("A window did not report the sizer it was given.");
    if (replacement.GetContainingSizer() is null)
        throw new InvalidOperationException("A window in a sizer did not report a containing sizer.");

    mutableSizer.Clear();
    if (!mutableSizer.IsEmpty) throw new InvalidOperationException("Clear left items behind.");

    // ---- A flex grid grows only the rows and columns it was told to.
    var flexPanel = new Panel(panel);
    var flex = new FlexGridSizer(2, 2, verticalGap: 4, horizontalGap: 6);
    for (var i = 0; i < 4; ++i) flex.Add(new StaticText(flexPanel, label: $"cell {i}"));
    flex.AddGrowableRow(1);
    flex.AddGrowableColumn(0);
    if (!flex.IsRowGrowable(1) || flex.IsRowGrowable(0) || !flex.IsColumnGrowable(0))
        throw new InvalidOperationException("Growable rows and columns did not round-trip.");
    flex.RemoveGrowableRow(1);
    if (flex.IsRowGrowable(1)) throw new InvalidOperationException("Removing a growable row failed.");
    if (flex.VerticalGap != 4 || flex.HorizontalGap != 6 || flex.Columns != 2)
        throw new InvalidOperationException("Grid gaps or dimensions did not round-trip.");
    flex.NonFlexibleGrowMode = FlexGrowMode.All;
    flex.FlexibleDirection = FlexDirection.Vertical;
    if (flex.NonFlexibleGrowMode != FlexGrowMode.All || flex.FlexibleDirection != FlexDirection.Vertical)
        throw new InvalidOperationException("Flex grow mode or direction did not round-trip.");
    flexPanel.SetSizerAndFit(flex);
    if (flex.GetColumnWidths().Length != 2)
        throw new InvalidOperationException("A laid-out flex grid did not report its column widths.");

    // ---- A grid-bag sizer places items by cell, and refuses to overlap them.
    var bagPanel = new Panel(panel);
    var bag = new GridBagSizer();
    var wide = new StaticText(bagPanel, label: "spans two");
    bag.AddAt(wide, 0, 0, rowSpan: 1, columnSpan: 2);
    var corner = new StaticText(bagPanel, label: "corner");
    bag.AddAt(corner, 1, 1);
    if (bag.GetItemPosition(corner) != (1, 1) || bag.GetItemSpan(wide) != (1, 2))
        throw new InvalidOperationException("Grid-bag position or span did not round-trip.");
    if (!bag.CheckForIntersection(0, 1))
        throw new InvalidOperationException("A cell covered by a span should report an intersection.");
    if (bag.CheckForIntersection(2, 0))
        throw new InvalidOperationException("A free cell should report no intersection.");
    // Asking first is the point: wxWidgets asserts rather than quietly refusing if the cell is taken.
    if (!bag.SetItemPosition(corner, 2, 0) || bag.GetItemPosition(corner) != (2, 0))
        throw new InvalidOperationException("Moving an item to a free cell failed.");
    if (bag.FindItemAtPosition(2, 0) is null || bag.FindItemAtPosition(5, 5) is not null)
        throw new InvalidOperationException("Finding an item by cell failed.");
    bagPanel.SetSizer(bag);

    mutable.Destroy();
    flexPanel.Destroy();
    bagPanel.Destroy();

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

    // wxWidgets asserts if Set3StateValue(Undetermined) is called on a two-state box, so only verify its
    // reported mode here; Phoenix passes that invalid call through unchanged too.
    var twoState = new CheckBox(panel, label: "Two");
    if (twoState.IsThreeState)
        throw new InvalidOperationException("A two-state check box reported three-state mode.");
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
        if (closeAttempts == 1) { e.Veto(); Wx.CallAfter(() => frame.Close()); }
        else e.Skip();
    };

    var timerTicks = 0;
    var timer = new WxSharp.Timer(frame);
    timer.Tick += (_, _) => { timerTicks++; timer.Stop(); frame.Close(); };

    // Keep the native window hidden while the synchronous API checks run. Showing it earlier created an
    // unresponsive top-level window because MainLoop was not dispatching messages yet, making the desktop
    // appear frozen. It is shown only immediately before entering the event loop.
    frame.Show();

    // Queue from a worker to verify thread-safe UI marshaling and a genuinely blocking native MainLoop.
    Task.Run(() => Wx.CallAfter(() =>
    {
        if (args.Contains("--clipboard"))
            RunClipboardRoundTrip();
        timer.Start(10);
    })).GetAwaiter().GetResult();
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
    if (postedLater != 1 || postedValue != 11)
        throw new InvalidOperationException("A queued command event did not reach its handler from the event loop.");
}

Console.WriteLine($"Smoke test passed; custom accessibility: {Wx.SupportsCustomAccessibility}.");

static void VerifyLifecycle(SmokeApp app)
{
    if (!app.OnInitCalled || !app.OnExitCalled || App.Current is not null)
        throw new InvalidOperationException("App lifecycle hooks or automatic cleanup did not run.");
}
}
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

// Exercises the shapes the virtual channel has to carry: a size returned to wxWidgets, a point read back
// from it, a void hook with an argument, and a plain bool query left to wxWidgets entirely.
sealed class SizedPanel : Panel
{
    public SizedPanel(Window parent) : base(parent) { }

    public bool EnableSeen { get; private set; }

    // Read through the base implementation, so it is wxWidgets' answer rather than an override's.
    public Point ClientAreaOriginBase => base.GetClientAreaOrigin();

    public bool InheritsColours => ShouldInheritColours();

    protected override Size DoGetBestSize() => new(123, 45);

    protected override void DoEnable(bool enable)
    {
        EnableSeen = true;
        base.DoEnable(enable);
    }
}

// The wxScrolled virtuals, which exist on that class rather than on wxWindow.
sealed class HookedScrolled : ScrolledWindow
{
    public HookedScrolled(Window parent) : base(parent) { }

    public bool BaseAnswered { get; private set; }

    public override Size GetSizeAvailableForScrollTarget(Size size) => new(111, 22);

    public override bool ShouldScrollToChildOnFocus(Window? child)
    {
        // wxWidgets' own answer still has to be reachable from inside the override.
        BaseAnswered = base.ShouldScrollToChildOnFocus(child) || true;
        return false;
    }
}

// wxGrid asks for the pen each line is drawn with, which is how a grid marks one column out from the rest.
sealed class HookedGrid : Grid
{
    public HookedGrid(Window parent) : base(parent, 2, 2) { }

    public override Pen GetColGridLinePen(int column) => new(Colour.Red, 2);
}

// Refuses to be tabbed onto, the way an accessible application's transport buttons do, while staying a
// perfectly ordinary button in every other respect. It also calls the base implementation, which has to
// reach wxWidgets rather than recursing back into this override.
sealed class UnfocusableButton : Button
{
    public UnfocusableButton(Window parent) : base(parent, label: "No tab stop") { }

    public bool BaseWasReached { get; private set; }

    public override bool AcceptsFocusFromKeyboard()
    {
        BaseWasReached = base.AcceptsFocusFromKeyboard();
        return false;
    }
}
