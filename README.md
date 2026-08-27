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

Packages are written to `build/packages`. Rebuilding the bundled wxWidgets
binaries is a separate, deliberate step:

```powershell
.\scripts\build-wxwidgets-windows.ps1 -MsvcRoot D:\path\to\msvc
```

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
