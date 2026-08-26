# WxSharp

WxSharp is a Native AOT-compatible .NET wrapper for
[wxWidgets](https://www.wxwidgets.org/). A small native library exposes a
stable UTF-8 C ABI, while the public API remains idiomatic C#.

> WxSharp is in early development. The managed API and native ABI may change
> before the first stable release. The initial distributable target is Windows
> x64; Linux and macOS builds are experimental and are not packaged yet.

## Features

- Source-generated P/Invoke without reflection-based marshalling
- Native windows, dialogs, menus, toolbars, status bars, sizers, timers, and clipboard services
- Common controls, notebook/splitter/scrolled containers, and list, tree, grid, and data-view controls
- Strongly typed events plus Native AOT-safe `Bind`/`Unbind` with ID and ID-range filtering
- A blocking native event loop with UI-thread marshaling through `Wx.CallAfter`
- Native UTF-8 strings across the managed/native boundary
- Native AOT compatibility
- Phoenix-inspired custom accessible children, navigation, hit testing, selection, focus, actions, and notifications

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

The Windows package includes `wx.dll` and the wxWidgets `base` and `core`
DLLs as `win-x64` native runtime assets. Each is built with the MSVC runtime
linked statically; Windows system DLLs remain operating-system dependencies.

## Accessibility

Standard controls use wxWidgets' native accessibility implementation. On
Windows, applications can attach an `Accessible` implementation to a window
and override the Phoenix/wxAccessible contract for virtual children, roles,
states, names, values, screen locations, hit testing, navigation, selection,
focus, keyboard shortcuts, and default actions. Accessibility notifications
are sent with `Accessible.Notify`.

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
