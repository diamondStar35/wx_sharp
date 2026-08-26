using System;

namespace WxSharp;

// Style enums carry stable, semantic values that the native side translates to the real wxWidgets style flags
// (see the Map* helpers in internal.h). Keeping the mapping native means no wx magic numbers leak into the
// managed wrapper, and the enums stay readable.

/// <summary>The border drawn around a control. Set through <see cref="Control.Border"/>.</summary>
public enum Border
{
    /// <summary>The platform default border for the control.</summary>
    Default = 0,
    None = 1,
    Simple = 2,
    Sunken = 3,
    Raised = 4,
    Static = 5,
    /// <summary>The current theme's border (e.g. the themed edge on Windows).</summary>
    Theme = 6,
}

/// <summary>Horizontal alignment of a control's text.</summary>
public enum Alignment
{
    Left = 0,
    Centre = 1,
    Right = 2,
}

/// <summary>Creation styles for a <see cref="TextCtrl"/>. Some (multi-line, password) can only be chosen at
/// creation time.</summary>
[Flags]
public enum TextCtrlStyle
{
    None = 0,
    MultiLine = 1 << 0,
    Password = 1 << 1,
    ReadOnly = 1 << 2,
    /// <summary>Raise <see cref="TextCtrl.EnterPressed"/> on Enter (implied for single-line boxes).</summary>
    ProcessEnter = 1 << 3,
    /// <summary>Let Tab be typed into the box instead of moving focus.</summary>
    ProcessTab = 1 << 4,
    Rich = 1 << 5,
    AlignRight = 1 << 6,
    AlignCentre = 1 << 7,
    /// <summary>Keep the selection visible when the box loses focus.</summary>
    ShowSelectionAlways = 1 << 8,
    /// <summary>Turn URLs into clickable links (rich text).</summary>
    AutoUrl = 1 << 9,
    /// <summary>Don't wrap long lines; show a horizontal scrollbar instead.</summary>
    DontWrap = 1 << 10,
}

/// <summary>Whether a <see cref="CheckBox"/> supports the third, indeterminate state.</summary>
public enum CheckBoxStyle
{
    TwoState = 0,
    ThreeState = 1,
}

/// <summary>Creation styles for a <see cref="Slider"/>.</summary>
[Flags]
public enum SliderStyle
{
    Horizontal = 0,
    Vertical = 1 << 0,
    /// <summary>Show value labels alongside the slider.</summary>
    Labels = 1 << 1,
    /// <summary>Draw tick marks.</summary>
    Ticks = 1 << 2,
    /// <summary>Invert the direction (min and max swap ends).</summary>
    Inverse = 1 << 3,
    /// <summary>Show the min and max labels at the ends.</summary>
    MinMaxLabels = 1 << 4,
}

/// <summary>Creation styles for a <see cref="ListBox"/> - selection mode and scrollbars.</summary>
[Flags]
public enum ListBoxStyle
{
    /// <summary>Single selection (the default).</summary>
    Single = 0,
    /// <summary>Toggle multiple items independently.</summary>
    Multiple = 1 << 0,
    /// <summary>Range selection with Shift/Ctrl.</summary>
    Extended = 1 << 1,
    /// <summary>Keep items sorted alphabetically.</summary>
    Sort = 1 << 2,
    /// <summary>Always show the vertical scrollbar.</summary>
    AlwaysScrollbar = 1 << 3,
    /// <summary>Show a horizontal scrollbar when needed.</summary>
    HorizontalScrollbar = 1 << 4,
    /// <summary>Show the vertical scrollbar only when needed.</summary>
    ScrollbarWhenNeeded = 1 << 5,
}

/// <summary>Creation styles for a <see cref="Choice"/> drop-down.</summary>
public enum ChoiceStyle
{
    Unsorted = 0,
    Sorted = 1,
}
