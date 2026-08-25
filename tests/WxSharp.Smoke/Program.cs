using WxSharp;
using System.Runtime.InteropServices;

const string expected = "WxSharp — العربية — 日本語 — 🚀";

var nativeLibrary = Environment.GetEnvironmentVariable("WXSHARP_NATIVE_LIBRARY");
if (!string.IsNullOrEmpty(nativeLibrary))
{
    NativeLibrary.SetDllImportResolver(typeof(Wx).Assembly, (_, assembly, searchPath) =>
        NativeLibrary.Load(nativeLibrary, assembly, searchPath));
}

if (!Wx.Init())
    throw new InvalidOperationException("wxWidgets initialization failed.");

try
{
    var window = new Window(expected);
    var label = new Label(window, expected);

    if (label.Text != expected)
        throw new InvalidOperationException($"UTF-8 round trip failed: '{label.Text}'.");

    label.AccessibleName = expected;
    window.Layout();
    window.Destroy();
    Wx.Pump();

    Console.WriteLine($"Smoke test passed; custom accessibility: {Wx.SupportsCustomAccessibility}.");
}
finally
{
    Wx.Shutdown();
}
