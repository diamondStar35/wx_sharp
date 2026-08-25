using WxSharp;

const string expected = "WxSharp package — العربية — 日本語 — 🚀";

if (!Wx.Init())
    throw new InvalidOperationException("Packaged wxWidgets initialization failed.");

try
{
    var window = new Window(expected);
    var label = new Label(window, expected);

    if (label.Text != expected)
        throw new InvalidOperationException($"Packaged UTF-8 round trip failed: '{label.Text}'.");

    window.Destroy();
    Wx.Pump();
    Console.WriteLine("NuGet consumer smoke test passed.");
}
finally
{
    Wx.Shutdown();
}

