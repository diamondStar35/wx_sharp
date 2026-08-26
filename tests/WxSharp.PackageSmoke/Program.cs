using WxSharp;

const string expected = "WxSharp package — العربية — 日本語 — 🚀";

using var app = new App();
var frame = new Frame(title: expected);
var panel = new Panel(frame);
var label = new StaticText(panel, label: expected);
var layout = new BoxSizer();
layout.Add(label, flags: SizerFlags.All, border: 8);
panel.SetSizer(layout);

if (label.Label != expected)
    throw new InvalidOperationException($"Packaged UTF-8 round trip failed: '{label.Label}'.");

frame.Show();
Wx.CallAfter(frame.Close);
app.MainLoop();
Console.WriteLine("NuGet consumer smoke test passed.");
