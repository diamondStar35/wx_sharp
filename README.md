# WxSharp

WxSharp is a Native AOT-compatible .NET wrapper for
[wxWidgets](https://www.wxwidgets.org/). A small native library exposes a
stable UTF-8 C ABI, while the public API remains idiomatic C#.

> WxSharp is in early development. The managed API and native ABI may change
> before the first stable release. The initial distributable target is Windows
> x64; Linux and macOS builds are experimental and are not packaged yet.

## Features

- Source-generated P/Invoke without reflection-based marshalling
- Native windows, dialogs, menus and submenus, toolbars, status bars, sizers, timers, and clipboard services
- Common controls, notebook/splitter/scrolled containers, and list, tree, grid, and data-view controls
- A table-driven event system: one wrapper event ID per wxWidgets event type, hooked natively only while
  something is subscribed, with wxWidgets' own parent-chain propagation
- Strongly typed events plus Native AOT-safe `Bind`/`Unbind` with ID and ID-range filtering
- Menu items as objects, stock identifiers, runtime ID allocation, and accelerator tables parsed from and
  formatted back to the strings a user-configurable shortcut is stored as
- Every wxWidgets key code, generated from the headers rather than transcribed
- Creation styles for frames, dialogs, panels, scrolled windows, list and tree controls, and the full sizer
  alignment set
- A blocking native event loop with UI-thread marshaling through `Wx.CallAfter`
- Native UTF-8 strings across the managed/native boundary
- Native AOT compatibility
- The `wxAccessible` contract as wxPython exposes it: custom accessible children, navigation, hit testing, selection, focus, actions, and notifications

## Requirements

- [.NET 8, 9, or 10 SDK](https://dotnet.microsoft.com/download)
- [CMake](https://cmake.org/) 3.22 or newer
- Visual Studio 2022 or newer with the Desktop development with C++ workload

The repository includes the Windows x64 wxWidgets `base` and `core` DLLs,
import libraries, and wrapper-required headers. They are built from the pinned
wxWidgets 3.3.3 source release with the MSVC runtime linked statically. The
source pipeline reproduces these binaries, and the resulting package does not
require Visual C++ redistributable DLLs.

## Build

Run the complete Windows pipeline from PowerShell:

```powershell
.\scripts\build-windows.ps1 -Configuration Release `
    -MsvcRoot D:\path\to\msvc
```

This reuses the pinned wxWidgets binaries in `third-party/Windows`, builds the
native wrapper and managed libraries, stages runtime dependencies, runs native
and managed smoke tests, creates a NuGet package, validates its contents, and
tests it from an independent consumer project. It does not rebuild wxWidgets.
Packages are written to `build/packages`. A native deployment containing the test
executable, `wx.dll`, and the two wxWidgets DLLs is written to
`build/standalone-test/win-x64`.

After wxWidgets has been built once, use the wrapper-only command during
native wrapper development:

```powershell
.\scripts\build-wrapper-windows.ps1 `
    -MsvcRoot D:\path\to\msvc
```

This recompiles and relinks only `wx.dll`; it reuses the existing two
wxWidgets DLLs.

Rebuilding the pinned wxWidgets binaries is an explicit dependency-maintenance
operation, not part of normal wrapper or packaging builds:

```powershell
.\scripts\build-wxwidgets-windows.ps1 -MsvcRoot D:\path\to\msvc
```

To build only the managed projects:

```powershell
dotnet build WxSharp.slnx -c Release
```

## Usage

Create one `App` on the UI thread, build an explicit window hierarchy, and
enter its blocking event loop. wxWidgets handles waiting and message dispatch:

```csharp
using WxSharp;

using var app = new App();

var frame = new Frame(title: "Hello from WxSharp");
var panel = new Panel(frame);
var message = new StaticText(panel, label: "Hello!");
var close = new Button(panel, label: "Close");

var layout = new BoxSizer(Orientation.Vertical);
layout.Add(message, flags: SizerFlags.All, border: 8);
layout.Add(close, flags: SizerFlags.All, border: 8);
panel.SetSizer(layout);

close.Click += (_, _) => frame.Close();
frame.Show();
app.MainLoop();
```

Controls are never inserted into hidden panels or implicit layouts. Create a
`Panel` where one is wanted, add children to a sizer, and assign that sizer
explicitly. All UI access stays on the `App` thread; `Wx.CallAfter` is the
thread-safe way to schedule work from another thread.

No style is added behind your back. Every style enum's `Default` is whatever
wxWidgets uses for that class on the current platform, resolved natively rather
than composed in managed code - so `TreeCtrlStyle.Default` is the real
`wxTR_DEFAULT_STYLE`, which differs between Windows, GTK and macOS. Where
wxWidgets' default is not what most applications want, the wrapper says so and
leaves the choice alone: `ListCtrlStyle.Default` is `wxLC_ICON`, and a report
view has to ask for `ListCtrlStyle.Report`.

The Windows package includes `wx.dll` and the wxWidgets `base` and `core`
DLLs as `win-x64` native runtime assets. Each is built with the MSVC runtime
linked statically; Windows system DLLs remain operating-system dependencies.

## Events

Every control's typed `event` members are shorthand for `Bind`, and both reach
the same subscriber list. An event type is hooked natively the first time
something subscribes to it on a window and unhooked when the last subscriber
goes away, so an event nothing listens for never crosses the boundary.

Command events propagate the way wxWidgets propagates them: an unhandled one
travels up the real parent chain, so binding on a parent catches its children.

```csharp
// One button, by name.
open.Click += (_, _) => Open();

// Every menu command on the frame, filtered by ID.
frame.Bind(WxEvents.MenuCommand, (_, _) => Open(), openId);

// Every button in the dialog, by ID range.
dialog.Bind(WxEvents.ButtonClicked, OnAnyButton, firstId, lastId);
```

Handling an event stops it. That is wxWidgets' model and Phoenix's: a handler
consumes the event unless it calls `Skip()`, which asks for normal processing to
continue - the control's own behaviour, the next handler, and propagation to the
parent. Bind `Closing` or `SizeChanged` and return without skipping, and the
window will not close or will not lay out. Skip whenever the handler is
observing rather than deciding.

Events wxWidgets lets you refuse - a page change, a tree expansion, a sash drag -
carry `Veto()`; `Closing` carries `Veto()` and `CanVeto`.

`WxEvents` carries 145 of the event types wxWidgets declares, including the ones
an ordinary application needs beyond the obvious: `UpdateUI`, `Idle`,
`MenuOpened`/`MenuClosed`/`MenuHighlighted`, `MouseCaptureLost` (mandatory if
you call `CaptureMouse`), `DropFiles`, `HotKey`, `NavigationKey`, `ChildFocus`,
the full scroll set, and the complete list, tree and data-view families.

## Menus and shortcuts

`Menu` holds `MenuItem` objects, so labels, help strings, check state and
submenus are addressed directly rather than by loose integer ID. `StandardId`
exposes the wxWidgets stock identifiers, which carry the platform's own label,
icon and accelerator - and are what a screen reader announces - while
`IdManager.NewId` allocates IDs for commands an application invents at runtime.

```csharp
var fileMenu = new Menu();
fileMenu.Append(StandardId.Open, "&Open...\tCtrl+O", "Open a file");
fileMenu.AppendSubMenu(recentMenu, "&Recent");
fileMenu.AppendSeparator();
fileMenu.Append(StandardId.Exit, "E&xit");

var menuBar = new MenuBar();
menuBar.Append(fileMenu, "&File");
frame.SetMenuBar(menuBar);

// Accelerators are parsed by wxWidgets, so what round-trips is what wx accepts.
if (AcceleratorEntry.TryParse(settings.PlayPauseShortcut, playPauseId, out var entry))
    frame.SetAcceleratorTable(entry);
```

Accelerator tables install on any window, dialogs included. Show a context menu
with `Window.PopupMenu`, from the `ContextMenu` event - which fires for the
keyboard's menu key as well as a right-click, and reports which it was.

## Command state

Menu items and toolbar buttons go stale when their state is pushed: every code
path that could change the answer has to remember to update them. wxWidgets
inverts that with `wxEVT_UPDATE_UI`, and so does WxSharp. Answer the question
for a command ID, and wxWidgets asks whenever it needs to know - on idle, and
every time a menu is about to open:

```csharp
frame.Bind(WxEvents.UpdateUI, (_, e) => e.Enable(playlist.Count > 0), playId);
frame.Bind(WxEvents.UpdateUI, (_, e) => e.Check(settings.Repeat), repeatId);
```

Nothing calls `Enable` or `Checked =`. Stop, end-of-playlist, load-failed and
clear all change `playlist`, and the menu follows on its own, because nothing
had to be remembered. `UpdateUIEventArgs` carries `Enabled`, `Checked`, `Shown`
and `Text`, and `UpdateUIEventArgs.SetUpdateInterval` and `SetMode` are the
static controls `wxUpdateUIEvent` provides.

One handler per command ID: a second binding on the same ID never runs, because
the first one consumed the event. For a command whose state is decided in
exactly one place, `MenuItem.Enabled` is still there.

`Frame.MenuOpened` fires before a menu is shown, which is where a dynamic menu -
a recent-files list - should be rebuilt.

## Keyboard

`KeyEventArgs.Code` is a `Key`, and `Key` names every code wxWidgets defines -
media keys, the numeric keypad, F13 through F24, the Windows keys. The values
are generated from the wxWidgets headers at build time rather than transcribed,
because a wrong code here would silently bind a shortcut to the wrong key.

Four keyboard events, with different jobs:

| Event | Where it fires | Use it for |
|---|---|---|
| `CharHook` | The top-level window, before the focused control | Application-wide shortcuts |
| `KeyDown` | The focused control | Control-specific key handling |
| `Char` | The focused control, after translation | The character a key produced |
| `KeyUp` | The focused control | Release, for press-and-hold |

`KeyEventArgs` carries `Modifiers` (including `Meta` and `RawControl`),
`UnicodeKey`, and `RawKeyCode` for hardware wxWidgets does not name.

## Accessibility

Standard controls use wxWidgets' native accessibility implementation, and the
wrapper stays out of the way: it attaches no `wxAccessible` to a control and
sets no control's name, because that is exactly what lets the platform's own
provider report a button's or check box's label. This matches wxPython, where a
plain `wx.CheckBox(label=...)` is announced correctly with no help from the
application.

The accessibility surface is what wxPython exposes and nothing else. Subclass
`Accessible` to implement the `wxAccessible` contract - virtual children, roles,
states, names, values, screen locations, hit testing, navigation, selection,
focus, keyboard shortcuts and default actions - and assign it to
`Window.Accessible`, or return one from an overridden `CreateAccessible`.
Notifications go through the static `Accessible.NotifyEvent`.

Where wxWidgets was built without accessibility, those hooks throw
`NotImplementedException`, the analogue of the `NotImplementedError` wxPython
raises there.

See [wxPython Phoenix parity](docs/phoenix-parity.md) for current behavior and
known gaps.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Report suspected vulnerabilities as
described in [SECURITY.md](SECURITY.md).

## License

WxSharp is licensed under the [Apache License 2.0](LICENSE). It may be used,
modified, and distributed in commercial, proprietary, and open-source
projects subject to that licence's terms.

The bundled wxWidgets components are distributed under the
[wxWindows Library Licence](https://www.wxwidgets.org/about/licence/). Its
exception permits binary applications linked with wxWidgets to be distributed
under the application's own terms, including proprietary terms.

Distributions must retain the applicable copyright and licence notices. The
wxWidgets licence text is stored with the Windows dependency and included in
the NuGet package. Application authors remain responsible for including the
required notices with products they distribute.
