# Contributing

Thank you for helping improve WxSharp.

## Development setup

Install the .NET SDK selected by `global.json`, CMake 3.22 or newer, and Visual
Studio with the Desktop development with C++ workload. The Windows wxWidgets
SDK is included in the repository.

Before opening a pull request, run:

```powershell
dotnet build WxSharp.slnx -c Release
.\scripts\build-windows.ps1 -Configuration Release
```

Keep the declarations in `src/WxSharp/NativeMethods.cs` synchronized with the
ABI exported by `src/WxSharp.Native/wxsharp.h`. ABI changes should be described
clearly in the pull request.

## Pull requests

- Keep changes focused and include tests when a test harness exists for the
  affected behavior.
- Do not commit build outputs or user-specific CMake presets. The deliberately
  reduced Windows SDK under `third-party/Windows` is the only vendored native
  dependency exception.
- Update the README for user-visible API or build changes.
