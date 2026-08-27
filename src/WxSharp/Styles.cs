using System;

namespace WxSharp;

// Style enums carry stable, semantic values that the native side translates to the real wxWidgets style flags
// (see the Map* helpers in internal.h). Keeping the mapping native means no wx magic numbers leak into the
// managed wrapper, and the enums stay readable.

/// <summary>Creation styles for a <see cref="Frame"/>. These decide the window's decoration, so they can
/// only be chosen when the frame is created.</summary>
[Flags]
public enum FrameStyle
{
    /// <summary>No decoration at all: no title bar, no border, no buttons.</summary>
    None = 0,
    /// <summary>A title bar.</summary>
    Caption = 1 << 0,
    MinimizeBox = 1 << 1,
    MaximizeBox = 1 << 2,
    CloseBox = 1 << 3,
    /// <summary>The window menu on the title bar, which is also the keyboard route to move and size.</summary>
    SystemMenu = 1 << 4,
    /// <summary>A border the user can drag to resize.</summary>
    ResizeBorder = 1 << 5,
    StayOnTop = 1 << 6,
    /// <summary>A small-caption tool window, kept out of the task bar and the window list.</summary>
    ToolWindow = 1 << 7,
    NoTaskBar = 1 << 8,
    /// <summary>Float above the parent window rather than above everything.</summary>
    FloatOnParent = 1 << 9,

    /// <summary>wxWidgets' own <c>wxDEFAULT_FRAME_STYLE</c>. Resolved natively so it is exactly what
    /// wxWidgets uses on this platform, and combines with other flags as it does in C++.</summary>
    Default = 1 << 30,
}

/// <summary>Creation styles for a <see cref="Dialog"/>.</summary>
[Flags]
public enum DialogStyle
{
    None = 0,
    Caption = 1 << 0,
    CloseBox = 1 << 1,
    SystemMenu = 1 << 2,
    /// <summary>Let the user resize the dialog. Worth adding whenever the content can grow - a list of
    /// results, a long message - so it can be made readable at a larger font size.</summary>
    ResizeBorder = 1 << 3,
    StayOnTop = 1 << 4,
    MaximizeBox = 1 << 5,
    MinimizeBox = 1 << 6,

    /// <summary>wxWidgets' own <c>wxDEFAULT_DIALOG_STYLE</c>, resolved natively.</summary>
    Default = 1 << 30,
}

/// <summary>Creation styles for a <see cref="Panel"/>.</summary>
[Flags]
public enum PanelStyle
{
    None = 0,
    /// <summary>Let Tab move between the panel's children. Turning it off makes the panel's contents
    /// unreachable from the keyboard.</summary>
    TabTraversal = 1 << 0,

    /// <summary>wxWidgets' own default for a panel, which is <c>wxTAB_TRAVERSAL</c>.</summary>
    Default = 1 << 30,
}

/// <summary>Creation styles for a <see cref="ScrolledWindow"/>.</summary>
[Flags]
public enum ScrolledStyle
{
    None = 0,
    Horizontal = 1 << 0,
    Vertical = 1 << 1,
    TabTraversal = 1 << 2,

    /// <summary>wxWidgets' own <c>wxScrolledWindowStyle</c>: both scrollbars, and nothing else.</summary>
    Default = 1 << 30,
}

/// <summary>Creation styles for a <see cref="ListCtrl"/>. The view mode and the selection mode can only be
/// chosen at creation time.</summary>
[Flags]
public enum ListCtrlStyle
{
    /// <summary>Multi-column rows with a header.</summary>
    Report = 1 << 0,
    /// <summary>A single-column list.</summary>
    List = 1 << 1,
    Icon = 1 << 2,
    SmallIcon = 1 << 3,
    /// <summary>Only one row can be selected at a time.</summary>
    SingleSelection = 1 << 4,
    NoHeader = 1 << 5,
    EditLabels = 1 << 6,
    /// <summary>Virtual mode: the control asks for the text of the rows it is about to draw instead of
    /// storing them, which is what makes a very long list affordable.</summary>
    Virtual = 1 << 7,
    HorizontalRules = 1 << 8,
    VerticalRules = 1 << 9,
    SortAscending = 1 << 10,

    /// <summary>wxWidgets' own default, which is <see cref="Icon"/>. Most applications want
    /// <see cref="Report"/> and should say so.</summary>
    Default = 1 << 30,
}

/// <summary>Creation styles for a <see cref="TreeCtrl"/>.</summary>
[Flags]
public enum TreeCtrlStyle
{
    None = 0,
    /// <summary>Show the expand/collapse buttons.</summary>
    HasButtons = 1 << 0,
    /// <summary>Hide the root item, so the first level reads as the top level.</summary>
    HideRoot = 1 << 1,
    LinesAtRoot = 1 << 2,
    RowLines = 1 << 3,
    EditLabels = 1 << 4,
    Multiple = 1 << 5,
    FullRowHighlight = 1 << 6,
    TwistButtons = 1 << 7,
    NoLines = 1 << 8,

    /// <summary>wxWidgets' own <c>wxTR_DEFAULT_STYLE</c>. Resolved natively because it is a different set
    /// on Windows, GTK and macOS, and one managed assembly serves all three.</summary>
    Default = 1 << 30,
}

/// <summary>Creation styles for a <see cref="FileDialog"/>.</summary>
[Flags]
public enum FileDialogStyle
{
    /// <summary>Choose an existing file.</summary>
    Open = 1 << 0,
    /// <summary>Choose a name to save under.</summary>
    Save = 1 << 1,
    /// <summary>Let more than one file be chosen. Only meaningful with <see cref="Open"/>.</summary>
    Multiple = 1 << 2,
    /// <summary>Refuse a name that does not exist.</summary>
    FileMustExist = 1 << 3,
    /// <summary>Ask before overwriting an existing file.</summary>
    OverwritePrompt = 1 << 4,
    /// <summary>Change the process working directory to the one chosen.</summary>
    ChangeDirectory = 1 << 5,
    ShowPreview = 1 << 6,
    ShowHidden = 1 << 7,
    /// <summary>Return the link itself rather than what it points at.</summary>
    NoFollowLinks = 1 << 8,

    /// <summary>An open dialog that insists the file exists.</summary>
    DefaultOpen = Open | FileMustExist,
    /// <summary>A save dialog that asks before overwriting.</summary>
    DefaultSave = Save | OverwritePrompt,
}

/// <summary>Which buttons a dialog's standard button row carries.</summary>
[Flags]
public enum ButtonSizerFlags
{
    None = 0,
    Ok = 1 << 0,
    Cancel = 1 << 1,
    Yes = 1 << 2,
    No = 1 << 3,
    Apply = 1 << 4,
    Close = 1 << 5,
    Help = 1 << 6,
    /// <summary>Do not make any of them the default button.</summary>
    NoDefault = 1 << 7,

    OkCancel = Ok | Cancel,
    YesNo = Yes | No,
}

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
[Flags]
public enum CheckBoxStyle
{
    TwoState = 0,

    /// <summary>Allow the indeterminate state, read and written through <see cref="CheckBox.State"/>.</summary>
    ThreeState = 1 << 0,

    /// <summary>Let the user cycle into the indeterminate state as well. Without this the third state can
    /// only be set in code, which is the usual arrangement.</summary>
    AllowThirdStateForUser = 1 << 1,
}

/// <summary>The state of a three-state <see cref="CheckBox"/>, following <c>wxCheckBoxState</c>.</summary>
public enum CheckBoxState
{
    Unchecked = 0,
    Checked = 1,
    /// <summary>Neither checked nor unchecked - "mixed", as a screen reader announces it.</summary>
    Undetermined = 2,
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

/// <summary>How a status bar looks and behaves, following the <c>wxSTB_*</c> flags.</summary>
[Flags]
public enum StatusBarStyle
{
    None = 0,

    /// <summary>Show the resize grip in the corner.</summary>
    SizeGrip = 0x0010,

    /// <summary>Show a tooltip with the whole text when a field is too narrow for it. Worth keeping: a
    /// truncated field otherwise silently loses the end of what it says.</summary>
    ShowTips = 0x0020,

    /// <summary>Cut text that does not fit at the start.</summary>
    EllipsizeStart = 0x0040,

    /// <summary>Cut text that does not fit in the middle.</summary>
    EllipsizeMiddle = 0x0080,

    /// <summary>Cut text that does not fit at the end.</summary>
    EllipsizeEnd = 0x0100,

    /// <summary>Repaint the whole bar when it is resized.</summary>
    FullRepaintOnResize = 0x00010000,

    Default = SizeGrip | EllipsizeEnd | ShowTips | FullRepaintOnResize,
}

/// <summary>How a toolbar is laid out, following the <c>wxTB_*</c> flags.</summary>
[Flags]
public enum ToolBarStyle
{
    None = 0,

    /// <summary>Lay the tools out in a row along the top.</summary>
    Horizontal = 0x0004,

    /// <summary>Lay the tools out in a column down the left.</summary>
    Vertical = 0x0008,

    /// <summary>Down the left. The same as <see cref="Vertical"/>.</summary>
    Left = Vertical,

    /// <summary>Flat buttons, without a raised border.</summary>
    Flat = 0x0020,

    /// <summary>Let the user drag the bar out of the frame. GTK only.</summary>
    Dockable = 0x0040,

    /// <summary>Hide the icons.</summary>
    NoIcons = 0x0080,

    /// <summary>Show each tool's label. Labels help anyone who does not recognise the icon, which is most
    /// people the first time and everyone using a magnifier.</summary>
    Text = 0x0100,

    /// <summary>Drop the divider between the bar and the window. Windows only.</summary>
    NoDivider = 0x0200,

    /// <summary>Skip the automatic alignment. Windows only.</summary>
    NoAlign = 0x0400,

    /// <summary>Put the label beside the icon rather than under it.</summary>
    HorizontalLayout = 0x0800,

    /// <summary>Labels beside the icons.</summary>
    HorizontalText = HorizontalLayout | Text,

    /// <summary>Do not show tooltips.</summary>
    NoToolTips = 0x1000,

    /// <summary>Along the bottom of the window.</summary>
    Bottom = 0x2000,

    /// <summary>Down the right of the window.</summary>
    Right = 0x4000,

    Default = Horizontal,
}
