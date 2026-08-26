using System;

namespace WxSharp;

/// <summary>An accessible slider built in the managed wrapper by inheriting from <see cref="Slider"/> - the
/// reference example of a custom control on the WxSharp key/event foundation. Unlike a plain slider it
/// (1) raises <see cref="Slider.ValueChanged"/> when the value is set in code (a native slider stays silent
/// then, so a screen reader wouldn't hear it), and (2) handles the arrow, page, Home and End keys itself via
/// <see cref="Control.OnKeyDown"/>, so movement and feedback are consistent regardless of platform key
/// handling.</summary>
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
    }

    /// <summary>Sets the value (clamped to the range) and always raises <see cref="Slider.ValueChanged"/>,
    /// including for a programmatic set.</summary>
    public override int Value
    {
        get => base.Value;
        set
        {
            base.Value = Math.Clamp(value, Minimum, Maximum);
            var args = new CommandEventArgs(this, Id);
            OnValueChanged(args);
            _ = PropagateCommand(args);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
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
            base.OnKeyDown(e); // not a movement key - raise KeyDown and let it fall through
            return;
        }

        Value = target;   // clamps, sets, and raises ValueChanged
        e.Handled = true;  // consume so the native slider doesn't move a second time
    }
}
