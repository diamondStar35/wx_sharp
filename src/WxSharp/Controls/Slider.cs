using System;

namespace WxSharp;

/// <summary>A slider over an integer range - a thin wrapper over the native control. It raises
/// <see cref="ValueChanged"/> when the user drags it or moves it with the keyboard. For the accessible variant
/// that also fires on programmatic changes and handles Home/End and arrow keys uniformly, use
/// <see cref="CustomSlider"/>.</summary>
public class Slider : Control
{
    /// <summary>Raised when the value changes.</summary>
    public event EventHandler<CommandEventArgs> ValueChanged
    {
        add => AddHandler(WxEvents.SliderChanged, value);
        remove => RemoveHandler(WxEvents.SliderChanged, value);
    }

    /// <summary>Dragging finished. Seek on this rather than on every ValueChanged when the work is expensive - a media seek, for instance - so scrubbing stays responsive.</summary>
    public event EventHandler<ScrollEventArgs> ThumbReleased
    {
        add => AddHandler(WxEvents.ScrollThumbReleased, value);
        remove => RemoveHandler(WxEvents.ScrollThumbReleased, value);
    }

    public Slider(Window parent, int id = WindowId.Any, int value = 0, int minValue = 0, int maxValue = 100,
        SliderStyle style = SliderStyle.Horizontal, Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_slider_create(parent.Handle, id, minValue, maxValue, value, (int)style, Token));
        ApplyInitialGeometry(position, size);
    }

    public virtual int Value
    {
        get => NativeMethods.wxsharp_slider_get(Handle);
        set => NativeMethods.wxsharp_slider_set(Handle, value);
    }

    public int Minimum => NativeMethods.wxsharp_slider_get_min(Handle);
    public int Maximum => NativeMethods.wxsharp_slider_get_max(Handle);

    /// <summary>Changes the range; the current value is clamped into it by the control.</summary>
    public void SetRange(int min, int max) => NativeMethods.wxsharp_slider_set_range(Handle, min, max);

    /// <summary>Raises <see cref="ValueChanged"/> for the current value as though the user had moved the
    /// slider. A native slider says nothing when its value is set in code, so a subclass that wants a
    /// programmatic change announced calls this.</summary>
    protected void NotifyValueChanged()
    {
        var current = Value;
        var synthesised = new NativeEvent
        {
            Version = NativeEvent.ExpectedVersion,
            Kind = EventId.Slider,
            Id = Id,
            IntValue = current,
            Selection = current,
        };
        _ = RaiseLocal(in synthesised);
    }
}
