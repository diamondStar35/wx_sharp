# WxSharp

Native desktop GUI for .NET, built on [wxWidgets](https://www.wxwidgets.org/).

WxSharp gives .NET applications real native controls — the same buttons, lists
and menus the operating system draws for every other application — with an API
that follows [wxPython Phoenix](https://github.com/wxWidgets/Phoenix) closely
enough that porting between them is mostly mechanical.

> **Early development.** The API may change before the first stable release.
> Windows x64 is the packaged target; Linux and macOS build but are not shipped
> yet.

## Why

- **Native controls, not drawn ones.** Your application looks like the platform,
  respects the user's theme and font size, and behaves the way they expect.
- **Accessible by default.** Because the controls are the platform's own, screen
  readers, magnifiers and high-contrast modes work with no extra effort. For
  custom controls, the full `wxAccessible` contract is available.
- **Familiar if you know wxPython.** Class names, event names and behaviour
  follow Phoenix, so its documentation and examples largely apply.
- **Ready for Native AOT.** No reflection-based marshalling, so applications
  publish to a single self-contained executable with fast startup.

## Native dialog customization

`FileDialog.SetCustomizeHook()` attaches a `FileDialogCustomizeHook`, following
Phoenix. Override `AddCustomControls` to create buttons, checkboxes, radio buttons,
choices, edit fields, and static text through the supplied `FileDialogCustomize`.
Override `UpdateCustomControls` to react to navigation and selection changes, and
`TransferDataFromCustomControls` to copy accepted values before the controls go away.
The hook is retained by its dialog. Controls are not windows and belong to one showing;
using them after it ends throws `ObjectDisposedException`.

On Windows, labelled choices and edit fields expose their own accessible names
through both Microsoft Active Accessibility and UI Automation.

The Windows implementation also supports `DirDialog.SetCustomizeHook`,
`DirDialog.CurrentlySelectedPath`, labelled choices, and `FileDialogChoice.Set`
for replacing choices while asynchronous information is being loaded. These are
extensions to Phoenix. Customization currently returns `false` on other platforms.
File and folder picking stays in the modern Windows Common Item Dialog, with its
standard confirmation and cancellation buttons. Dialogs without a hook are unchanged.

```csharp
sealed class Options : FileDialogCustomizeHook
{
    private FileDialogCheckBox? _enabled;
    public bool Enabled { get; private set; }

    public override void AddCustomControls(FileDialogCustomize customizer)
        => _enabled = customizer.AddCheckBox("Enable option");

    public override void TransferDataFromCustomControls()
        => Enabled = _enabled!.Checked;
}
```

Run the dialog checks with `dotnet test --project tests/WxSharp.Tests.csproj -c Release`
after staging the native wrapper with `scripts/build-wrapper-windows.ps1`.

## Requirements

- .NET 8, 9, or 10
- Windows x64 (Linux and macOS are experimental)

Building the native layer from source additionally needs CMake 3.22+ and Visual
Studio 2022 or newer with the *Desktop development with C++* workload.

## Getting started

```csharp
using System;
using WxSharp;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var app = new App();

        var frame = new Frame(title: "Hello from WxSharp", size: new Size(360, 200));
        var panel = new Panel(frame);

        var message = new StaticText(panel, label: "Hello!");
        var close = new Button(panel, label: "Close");
        close.Click += (_, _) => frame.Close();

        var layout = new BoxSizer(Orientation.Vertical);
        layout.Add(message, flags: SizerFlags.All, border: 8);
        layout.Add(close, flags: SizerFlags.All, border: 8);
        panel.SetSizer(layout);

        frame.Show();
        app.MainLoop();
    }
}
```

Create one `App` on the main thread, build the window hierarchy explicitly, and
enter the event loop. Nothing is created behind your back: if you want a panel,
make one; if you want a layout, assign a sizer. All UI work happens on the `App`
thread, and `Wx.CallAfter` is the thread-safe way in from a background thread.

For an independent modeless dialog, create a `Dialog` with `parent: null` and
`DialogStyle.Default | DialogStyle.NoParent`, then call `Show()`. Without
`NoParent`, wxWidgets can choose the active or main window as the native owner
even when the managed parent is null. `ShowModal()` still disables other
application windows regardless of ownership.

`Window.NativeHandle` exposes the platform window handle (HWND on Windows) for
interop with native dialogs and other APIs that require an owner window.

A few windows do come from wxWidgets itself — the buttons behind
`Dialog.CreateButtonSizer`, for one. Lookups such as `Window.FindWindowById` and
`Window.FindFocus` wrap those on demand, typed as the class wxWidgets reports,
so `FindWindowById(StandardId.Ok, dialog) is Button` holds and you can bind its
`Click`. Such a wrapper reports `IsAdopted`, and disposing it detaches rather
than destroying a window it does not own.

On Windows the entry point needs `[STAThread]`, the same as any other desktop
UI framework there — which means an explicit `Main` rather than top-level
statements. .NET otherwise starts on a multi-threaded apartment, where the
clipboard, drag and drop, and the shell dialogs cannot work. `App` checks for
this at startup and says so rather than letting it fail later.

## What's included

Windows, dialogs and panels. Menus, submenus, context menus, toolbars and status
bars. Buttons, text fields, check boxes, radio buttons, choices, combo boxes,
sliders, gauges, spinners, search fields, date and time pickers. List, tree,
grid and data-view controls. Notebooks, splitters and scrolled windows. Sizers,
timers, the clipboard, and the standard file, folder, text, number, colour and
progress dialogs.

Accelerator tables parse and format the same shortcut strings wxWidgets accepts,
so keyboard shortcuts can be stored as text and configured by the user.

`TreeCtrl.SetItemHasChildren` marks unloaded branches as expandable. Handle
`ItemExpanding` to load their children on demand without inserting placeholder rows.

Virtual lists expose `TopItem` and `CountPerPage` so callers can refresh only
visible rows when their backing data changes.
`ListCtrl.SetDataSource` also accepts a managed list and a cell-text function without a subclass.
Append to that source on the UI thread and call `UpdateItemCount` to publish only the added rows.
The `CacheHint` event supplies `CacheFrom` and `CacheTo` for asynchronous paging.

Trees also expose hit testing, subtree expansion and collapse, `Toggle`,
`CollapseAndReset`, and `DeleteChildren`. Deleting descendants releases their
managed item data. A hidden root is only a container: expand or collapse its
subtree with `ExpandAllChildren` or `CollapseAllChildren`, rather than calling
`Expand` or `Collapse` on the root itself. Invalid direct hidden-root operations
are rejected before entering the native control.

## Building from source

The repository includes prebuilt wxWidgets binaries for Windows x64, so a normal
build does not compile wxWidgets.

```powershell
# Everything: native layer, managed libraries, tests, and a NuGet package.
.\scripts\build-windows.ps1 -Configuration Release -MsvcRoot D:\path\to\msvc

# Just the native layer, while working on it.
.\scripts\build-wrapper-windows.ps1 -MsvcRoot D:\path\to\msvc

# Just the managed projects.
dotnet build WxSharp.slnx -c Release
```

To build only the C++ shim as a static archive, configure the native project with
`-DWXSHARP_BUILD_STATIC=ON`. This does not statically link wxWidgets: consumers must still link the
wxWidgets import libraries and deploy its runtime DLLs. The archive is intended for Native AOT hosts
that want to link the C ABI shim into their executable.

Packages are written to `build/packages`. Rebuilding the bundled wxWidgets
binaries is a separate, deliberate step:

```powershell
.\scripts\build-wxwidgets-windows.ps1 -MsvcRoot D:\path\to\msvc
```

## List rendering

On Windows, the bundled wxWidgets 3.3.3 report-list dark renderer paints a whole
row for each column notification. WxSharp routes that painter once per row,
preserving its native colours, selection, focus and column order while avoiding
repeated drawing. The optimization applies to that version's dark-mode path;
other versions and modes use their original rendering.

## Distribution

The Windows package carries `wx.dll` and the two wxWidgets DLLs as `win-x64`
runtime assets. Each links the MSVC runtime statically, so applications do not
need the Visual C++ redistributable — only Windows itself.

## Documentation

[wxPython Phoenix parity](docs/phoenix-parity.md) describes how WxSharp behaves
relative to wxPython, and what is not wrapped yet.

To see how much of a given wxWidgets class is covered:

```
python scripts/coverage-report.py --type ListCtrl
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Please report suspected vulnerabilities
as described in [SECURITY.md](SECURITY.md).

## Licence

WxSharp is licensed under the [Apache License 2.0](LICENSE), and may be used in
commercial, proprietary and open-source projects on those terms.

The bundled wxWidgets components are distributed under the
[wxWindows Library Licence](https://www.wxwidgets.org/about/licence/), whose
exception allows binaries linked with wxWidgets to be distributed under your
application's own terms. Distributions must retain the applicable copyright and
licence notices; the wxWidgets licence text ships with the dependency and in the
NuGet package.
