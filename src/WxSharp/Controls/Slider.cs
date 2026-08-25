using System;

namespace WxSharp;

/// <summary>A slider over an integer range - a thin wrapper over the native control. It raises
/// <see cref="ValueChanged"/> when the user drags it or moves it with the keyboard. For the accessible variant
/// that also fires on programmatic changes and handles Home/End and arrow keys uniformly, use
/// <see cref="CustomSlider"/>.</summary>
public class Slider : Control
{
    /// <summary>Raised when the value changes.</summary>
    public event Action? ValueChanged;

    public Slider(Container parent, int min, int max, int value, SliderStyle style = SliderStyle.Horizontal)
        => Init(parent, NativeMethods.wxsharp_slider_create(parent.Panel, min, max, value, (int)style, Id));

    public virtual int Value
    {
        get => NativeMethods.wxsharp_slider_get(Handle);
        set => NativeMethods.wxsharp_slider_set(Handle, value);
    }

    public int Minimum => NativeMethods.wxsharp_slider_get_min(Handle);
    public int Maximum => NativeMethods.wxsharp_slider_get_max(Handle);

    /// <summary>Changes the range; the current value is clamped into it by the control.</summary>
    public void SetRange(int min, int max) => NativeMethods.wxsharp_slider_set_range(Handle, min, max);

    /// <summary>Raises <see cref="ValueChanged"/>. A custom slider calls this after a programmatic change so it
    /// notifies like a user change would.</summary>
    protected virtual void OnValueChanged() => ValueChanged?.Invoke();

    private protected override void OnEvent(EventKind evt)
    {
        if (evt == EventKind.Slider)
            OnValueChanged();
    }
}
