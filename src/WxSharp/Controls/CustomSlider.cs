using System;

namespace WxSharp;

/// <summary>An accessible slider built in the managed wrapper by inheriting from <see cref="Slider"/> - the
/// reference example of a custom control on the WxSharp event foundation. Unlike a plain slider it
/// (1) raises <see cref="Slider.ValueChanged"/> when the value is set in code, which a native slider stays
/// silent about, so a screen reader would otherwise hear nothing, and (2) handles the arrow, page, Home and
/// End keys itself, so movement and feedback are the same regardless of platform key handling.</summary>
public class CustomSlider : Slider
{
    /// <summary>How far the arrow keys move the value.</summary>
    public int LineStep { get; set; } = 1;

    /// <summary>How far the Page Up/Down keys move the value.</summary>
    public int PageStep { get; set; } = 10;

    public CustomSlider(Window parent, int id = WindowId.Any, int value = 0, int minValue = 0, int maxValue = 100,
        SliderStyle style = SliderStyle.Horizontal, Point? position = null, Size? size = null)
        : base(parent, id, value, minValue, maxValue, style, position, size)
    {
        KeyDown += OnKeyDown;
    }

    /// <summary>Sets the value (clamped to the range) and always raises <see cref="Slider.ValueChanged"/>,
    /// including for a programmatic set.</summary>
    public override int Value
    {
        get => base.Value;
        set
        {
            base.Value = Math.Clamp(value, Minimum, Maximum);
            NotifyValueChanged();
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        int target = e.Code switch
        {
            Key.Up or Key.Right => Value + LineStep,
            Key.Down or Key.Left => Value - LineStep,
            Key.PageUp => Value + PageStep,
            Key.PageDown => Value - PageStep,
            Key.Home => Minimum,
            Key.End => Maximum,
            _ => int.MinValue,
        };

        if (target == int.MinValue)
        {
            e.Skip();      // not a movement key - let the control have it
            return;
        }

        Value = target;    // clamps, sets, and raises ValueChanged. Not skipping consumes the key, so the
                           // native slider does not also move.
    }
}
