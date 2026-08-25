# WxSharp

WxSharp is a Native AOT-compatible .NET wrapper for
[wxWidgets](https://www.wxwidgets.org/). A small native library exposes a
stable UTF-8 C ABI, while the public API remains idiomatic C#.

> WxSharp is in early development. The managed API and native ABI may change
> before the first stable release. The initial distributable target is Windows
> x64; Linux and macOS builds are experimental and are not packaged yet.

## Features

- Source-generated P/Invoke without reflection-based marshalling
- Windows, dialogs, common controls, sizers, clipboard, and file dialogs
- Keyboard, mouse, focus, resize, and paint events
- Native UTF-8 strings across the managed/native boundary
- Native AOT compatibility
- wxPython Phoenix-inspired accessibility metadata and notifications

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

This builds the native and managed libraries, stages runtime dependencies,
runs native and managed smoke tests, creates a NuGet package, validates its
contents, and tests it from an independent consumer project. Packages are
written to `build/packages`. A native deployment containing the test
executable, `wxsharp.dll`, and the two wxWidgets DLLs is written to
`build/standalone-test/win-x64`.

After wxWidgets has been built once, use the wrapper-only command during
native wrapper development:

```powershell
.\scripts\build-wrapper-windows.ps1 `
    -MsvcRoot D:\path\to\msvc
```

This recompiles and relinks only `WxSharp.Native`; it reuses the existing two
wxWidgets DLLs.

To build only the managed projects:

```powershell
dotnet build WxSharp.slnx -c Release
```

## Usage

WxSharp must be initialized and used from the UI thread. The host owns the
event loop:

```csharp
using WxSharp;

if (!Wx.Init())
    throw new InvalidOperationException("wxWidgets initialization failed.");

var running = true;
var window = new Window("Hello from WxSharp");
var button = new Button(window, "Close");

button.Click += window.Close;
window.Closed += () => running = false;

window.Layout();
window.Center();
window.Show();

while (running)
{
    Wx.Pump();
    Wx.Wait(16);
}

Wx.Shutdown();
```

The Windows package includes `wxsharp.dll` and the wxWidgets `base` and `core`
DLLs as `win-x64` native runtime assets. Each is built with the MSVC runtime
linked statically; Windows system DLLs remain operating-system dependencies.

## Accessibility

Standard controls use wxWidgets' native accessibility implementation. On
Windows, WxSharp also exposes Phoenix-compatible roles, states, metadata, and
change notifications through wxWidgets' custom accessibility API.

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
