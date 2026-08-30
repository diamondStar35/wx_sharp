using System;
using System.Collections.Generic;

namespace WxSharp;

/// <summary>Builds a non-owning wrapper around a window wxWidgets created on its own.</summary>
///
/// <remarks>
/// Several wxWidgets calls hand back windows this binding never constructed - the OK and Cancel buttons
/// behind <see cref="Dialog.CreateButtonSizer"/>, the label window of a <see cref="StaticBox"/>, whatever
/// <see cref="Window.FindWindowById(int, Window?)"/> turns up. Without a wrapper those lookups can only
/// return null, so callers cannot bind an event or read a property on them.
///
/// The wrapper is created for the window's own wxWidgets class where that class is one this binding knows,
/// so a wxButton comes back as a <see cref="Button"/> and pattern matching on the result works. Anything
/// else falls back to <see cref="PlainWindow"/>, which still carries the handle and its events.
/// </remarks>
internal static class AdoptedWindowFactory
{
    /// <summary>A window whose wxWidgets class has no dedicated wrapper here. It is still a real
    /// <see cref="Window"/>: properties and events work, it just has no class-specific API.</summary>
    private sealed class PlainWindow(nint handle, Window? parent) : Window(handle, parent);

    private static readonly Dictionary<string, Func<nint, Window?, Window>> ByClassName = new(StringComparer.Ordinal)
    {
        ["wxButton"] = (h, p) => new Button(h, p),
        ["wxBitmapButton"] = (h, p) => new Button(h, p),
        ["wxToggleButton"] = (h, p) => new ToggleButton(h, p),
        ["wxCheckBox"] = (h, p) => new CheckBox(h, p),
        ["wxRadioButton"] = (h, p) => new RadioButton(h, p),
        ["wxStaticText"] = (h, p) => new StaticText(h, p),
        ["wxStaticBox"] = (h, p) => new StaticBox(h, p),
        ["wxTextCtrl"] = (h, p) => new TextCtrl(h, p),
        ["wxChoice"] = (h, p) => new Choice(h, p),
        ["wxComboBox"] = (h, p) => new ComboBox(h, p),
        ["wxListBox"] = (h, p) => new ListBox(h, p),
        ["wxSpinCtrl"] = (h, p) => new SpinCtrl(h, p),
        ["wxSlider"] = (h, p) => new Slider(h, p),
        ["wxGauge"] = (h, p) => new Gauge(h, p),
        ["wxPanel"] = (h, p) => new Panel(h, p),
    };

    internal static Window Create(nint handle, Window? parent)
    {
        // Read the class before constructing anything: the constructor registers the handle, and a throw
        // afterwards would leave a half-built wrapper in the lookup table.
        var className = PeekClassName(handle);
        return className is not null && ByClassName.TryGetValue(className, out var factory)
            ? factory(handle, parent)
            : new PlainWindow(handle, parent);
    }

    private static unsafe string? PeekClassName(nint handle)
    {
        var length = NativeMethods.wxsharp_window_get_class_name(handle, null, 0);
        if (length <= 0) return null;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_window_get_class_name(handle, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
}
