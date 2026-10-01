using System;

namespace WxSharp;

/// <summary>Roles exposed by Phoenix's <c>wxAccessible</c> wrapper. Built-in controls normally infer their
/// role; use an override for custom-drawn controls.</summary>
public enum AccessibleRole
{
    /// <summary>No role; the control's role is left unspecified (<c>wxROLE_NONE</c>).</summary>
    Default = 0,
    /// <summary>An alert or condition the user should be notified about (<c>wxROLE_SYSTEM_ALERT</c>).</summary>
    Alert = 1,
    /// <summary>An animation control (<c>wxROLE_SYSTEM_ANIMATION</c>).</summary>
    Animation = 2,
    /// <summary>The main window of an application (<c>wxROLE_SYSTEM_APPLICATION</c>).</summary>
    Application = 3,
    /// <summary>A window border (<c>wxROLE_SYSTEM_BORDER</c>).</summary>
    Border = 4,
    /// <summary>A button that drops down a list of items (<c>wxROLE_SYSTEM_BUTTONDROPDOWN</c>).</summary>
    ButtonDropDown = 5,
    /// <summary>A button that drops down a grid (<c>wxROLE_SYSTEM_BUTTONDROPDOWNGRID</c>).</summary>
    ButtonDropDownGrid = 6,
    /// <summary>A button that drops down a menu (<c>wxROLE_SYSTEM_BUTTONMENU</c>).</summary>
    ButtonMenu = 7,
    /// <summary>The caret, the blinking text-insertion marker (<c>wxROLE_SYSTEM_CARET</c>).</summary>
    Caret = 8,
    /// <summary>A cell within a table (<c>wxROLE_SYSTEM_CELL</c>).</summary>
    Cell = 9,
    /// <summary>A single character (<c>wxROLE_SYSTEM_CHARACTER</c>).</summary>
    Character = 10,
    /// <summary>A chart (<c>wxROLE_SYSTEM_CHART</c>).</summary>
    Chart = 11,
    /// <summary>A check box (<c>wxROLE_SYSTEM_CHECKBUTTON</c>). <see cref="CheckBox"/> is an alias.</summary>
    CheckButton = 12,
    /// <summary>The client area of a window (<c>wxROLE_SYSTEM_CLIENT</c>).</summary>
    Client = 13,
    /// <summary>A clock (<c>wxROLE_SYSTEM_CLOCK</c>).</summary>
    Clock = 14,
    /// <summary>A column of cells (<c>wxROLE_SYSTEM_COLUMN</c>).</summary>
    Column = 15,
    /// <summary>A column header (<c>wxROLE_SYSTEM_COLUMNHEADER</c>).</summary>
    ColumnHeader = 16,
    /// <summary>A combo box (<c>wxROLE_SYSTEM_COMBOBOX</c>).</summary>
    ComboBox = 17,
    /// <summary>The mouse cursor (<c>wxROLE_SYSTEM_CURSOR</c>).</summary>
    Cursor = 18,
    /// <summary>A diagram (<c>wxROLE_SYSTEM_DIAGRAM</c>).</summary>
    Diagram = 19,
    /// <summary>A dial control (<c>wxROLE_SYSTEM_DIAL</c>).</summary>
    Dial = 20,
    /// <summary>A dialog box (<c>wxROLE_SYSTEM_DIALOG</c>).</summary>
    Dialog = 21,
    /// <summary>A document window (<c>wxROLE_SYSTEM_DOCUMENT</c>).</summary>
    Document = 22,
    /// <summary>The drop-down list portion of a combo box (<c>wxROLE_SYSTEM_DROPLIST</c>).</summary>
    DropList = 23,
    /// <summary>A mathematical equation (<c>wxROLE_SYSTEM_EQUATION</c>).</summary>
    Equation = 24,
    /// <summary>A graphic or image (<c>wxROLE_SYSTEM_GRAPHIC</c>).</summary>
    Graphic = 25,
    /// <summary>A sizing grip (<c>wxROLE_SYSTEM_GRIP</c>).</summary>
    Grip = 26,
    /// <summary>A logical grouping of controls (<c>wxROLE_SYSTEM_GROUPING</c>).</summary>
    Grouping = 27,
    /// <summary>A help balloon (<c>wxROLE_SYSTEM_HELPBALLOON</c>).</summary>
    HelpBalloon = 28,
    /// <summary>A hot-key entry field (<c>wxROLE_SYSTEM_HOTKEYFIELD</c>).</summary>
    HotKeyField = 29,
    /// <summary>An indicator, such as a pointer graphic (<c>wxROLE_SYSTEM_INDICATOR</c>).</summary>
    Indicator = 30,
    /// <summary>A hyperlink (<c>wxROLE_SYSTEM_LINK</c>).</summary>
    Link = 31,
    /// <summary>A list box (<c>wxROLE_SYSTEM_LIST</c>).</summary>
    List = 32,
    /// <summary>An item within a list (<c>wxROLE_SYSTEM_LISTITEM</c>).</summary>
    ListItem = 33,
    /// <summary>A menu bar (<c>wxROLE_SYSTEM_MENUBAR</c>).</summary>
    MenuBar = 34,
    /// <summary>An item within a menu (<c>wxROLE_SYSTEM_MENUITEM</c>).</summary>
    MenuItem = 35,
    /// <summary>A pop-up menu (<c>wxROLE_SYSTEM_MENUPOPUP</c>).</summary>
    MenuPopup = 36,
    /// <summary>An outline or tree control (<c>wxROLE_SYSTEM_OUTLINE</c>).</summary>
    Outline = 37,
    /// <summary>An item within an outline or tree (<c>wxROLE_SYSTEM_OUTLINEITEM</c>).</summary>
    OutlineItem = 38,
    /// <summary>A single tab in a tab control (<c>wxROLE_SYSTEM_PAGETAB</c>).</summary>
    PageTab = 39,
    /// <summary>The tab strip of a tab control (<c>wxROLE_SYSTEM_PAGETABLIST</c>).</summary>
    PageTabList = 40,
    /// <summary>A pane within a frame or window (<c>wxROLE_SYSTEM_PANE</c>).</summary>
    Pane = 41,
    /// <summary>A progress bar (<c>wxROLE_SYSTEM_PROGRESSBAR</c>).</summary>
    ProgressBar = 42,
    /// <summary>A property page (<c>wxROLE_SYSTEM_PROPERTYPAGE</c>).</summary>
    PropertyPage = 43,
    /// <summary>A push button (<c>wxROLE_SYSTEM_PUSHBUTTON</c>). <see cref="Button"/> is an alias.</summary>
    PushButton = 44,
    /// <summary>A radio button (<c>wxROLE_SYSTEM_RADIOBUTTON</c>).</summary>
    RadioButton = 45,
    /// <summary>A row of cells (<c>wxROLE_SYSTEM_ROW</c>).</summary>
    Row = 46,
    /// <summary>A row header (<c>wxROLE_SYSTEM_ROWHEADER</c>).</summary>
    RowHeader = 47,
    /// <summary>A scroll bar (<c>wxROLE_SYSTEM_SCROLLBAR</c>).</summary>
    ScrollBar = 48,
    /// <summary>A separator between items (<c>wxROLE_SYSTEM_SEPARATOR</c>).</summary>
    Separator = 49,
    /// <summary>A slider (<c>wxROLE_SYSTEM_SLIDER</c>).</summary>
    Slider = 50,
    /// <summary>A sound notification (<c>wxROLE_SYSTEM_SOUND</c>).</summary>
    Sound = 51,
    /// <summary>A spin button (<c>wxROLE_SYSTEM_SPINBUTTON</c>).</summary>
    SpinButton = 52,
    /// <summary>Static, read-only text (<c>wxROLE_SYSTEM_STATICTEXT</c>). <see cref="Label"/> is an alias.</summary>
    StaticText = 53,
    /// <summary>A status bar (<c>wxROLE_SYSTEM_STATUSBAR</c>).</summary>
    StatusBar = 54,
    /// <summary>A table (<c>wxROLE_SYSTEM_TABLE</c>).</summary>
    Table = 55,
    /// <summary>Editable text (<c>wxROLE_SYSTEM_TEXT</c>).</summary>
    Text = 56,
    /// <summary>A window title bar (<c>wxROLE_SYSTEM_TITLEBAR</c>).</summary>
    TitleBar = 57,
    /// <summary>A tool bar (<c>wxROLE_SYSTEM_TOOLBAR</c>).</summary>
    ToolBar = 58,
    /// <summary>A tooltip (<c>wxROLE_SYSTEM_TOOLTIP</c>).</summary>
    ToolTip = 59,
    /// <summary>Blank space between other objects (<c>wxROLE_SYSTEM_WHITESPACE</c>).</summary>
    WhiteSpace = 60,
    /// <summary>A window frame (<c>wxROLE_SYSTEM_WINDOW</c>).</summary>
    Window = 61,

    /// <summary>Alias for <see cref="PushButton"/>.</summary>
    Button = PushButton,
    /// <summary>Alias for <see cref="CheckButton"/>.</summary>
    CheckBox = CheckButton,
    /// <summary>Alias for <see cref="StaticText"/>.</summary>
    Label = StaticText,
}

/// <summary>State flags exposed by Phoenix's <c>wxAccessible.GetState</c>.</summary>
[Flags]
public enum AccessibleState : uint
{
    /// <summary>No state flags set.</summary>
    None = 0,
    /// <summary>A high-priority alert the user should act on immediately (<c>wxACC_STATE_SYSTEM_ALERT_HIGH</c>).</summary>
    AlertHigh = 0x00000001,
    /// <summary>A medium-priority alert (<c>wxACC_STATE_SYSTEM_ALERT_MEDIUM</c>).</summary>
    AlertMedium = 0x00000002,
    /// <summary>A low-priority alert (<c>wxACC_STATE_SYSTEM_ALERT_LOW</c>).</summary>
    AlertLow = 0x00000004,
    /// <summary>The object's appearance is animating (<c>wxACC_STATE_SYSTEM_ANIMATED</c>).</summary>
    Animated = 0x00000008,
    /// <summary>The object is busy and cannot accept input right now (<c>wxACC_STATE_SYSTEM_BUSY</c>).</summary>
    Busy = 0x00000010,
    /// <summary>The object is checked (<c>wxACC_STATE_SYSTEM_CHECKED</c>).</summary>
    Checked = 0x00000020,
    /// <summary>The object is collapsed, hiding its children (<c>wxACC_STATE_SYSTEM_COLLAPSED</c>). See <see cref="Expanded"/>.</summary>
    Collapsed = 0x00000040,
    /// <summary>The object is the default action, such as the default button (<c>wxACC_STATE_SYSTEM_DEFAULT</c>).</summary>
    Default = 0x00000080,
    /// <summary>The object is expanded, showing its children (<c>wxACC_STATE_SYSTEM_EXPANDED</c>). See <see cref="Collapsed"/>.</summary>
    Expanded = 0x00000100,
    /// <summary>The object supports extending a selection (<c>wxACC_STATE_SYSTEM_EXTSELECTABLE</c>).</summary>
    ExtSelectable = 0x00000200,
    /// <summary>The object is floating, not fixed in place (<c>wxACC_STATE_SYSTEM_FLOATING</c>).</summary>
    Floating = 0x00000400,
    /// <summary>The object can receive keyboard focus (<c>wxACC_STATE_SYSTEM_FOCUSABLE</c>). See <see cref="Focused"/>.</summary>
    Focusable = 0x00000800,
    /// <summary>The object currently has keyboard focus (<c>wxACC_STATE_SYSTEM_FOCUSED</c>). See <see cref="Focusable"/>.</summary>
    Focused = 0x00001000,
    /// <summary>The object is hot-tracked, highlighted as the mouse passes over it (<c>wxACC_STATE_SYSTEM_HOTTRACKED</c>).</summary>
    HotTracked = 0x00002000,
    /// <summary>The object is programmatically hidden (<c>wxACC_STATE_SYSTEM_INVISIBLE</c>).</summary>
    Invisible = 0x00004000,
    /// <summary>The object scrolls its contents like a marquee (<c>wxACC_STATE_SYSTEM_MARQUEED</c>).</summary>
    Marqueed = 0x00008000,
    /// <summary>The object is in a mixed or indeterminate state, such as a tri-state check box (<c>wxACC_STATE_SYSTEM_MIXED</c>).</summary>
    Mixed = 0x00010000,
    /// <summary>The object supports selecting more than one child at a time (<c>wxACC_STATE_SYSTEM_MULTISELECTABLE</c>).</summary>
    MultiSelectable = 0x00020000,
    /// <summary>The object is scrolled off-screen (<c>wxACC_STATE_SYSTEM_OFFSCREEN</c>).</summary>
    Offscreen = 0x00040000,
    /// <summary>The object is pressed (<c>wxACC_STATE_SYSTEM_PRESSED</c>).</summary>
    Pressed = 0x00080000,
    /// <summary>The object's content is protected, such as a password field (<c>wxACC_STATE_SYSTEM_PROTECTED</c>).</summary>
    Protected = 0x00100000,
    /// <summary>The object is read-only (<c>wxACC_STATE_SYSTEM_READONLY</c>).</summary>
    ReadOnly = 0x00200000,
    /// <summary>The object can be selected (<c>wxACC_STATE_SYSTEM_SELECTABLE</c>). See <see cref="Selected"/>.</summary>
    Selectable = 0x00400000,
    /// <summary>The object is selected (<c>wxACC_STATE_SYSTEM_SELECTED</c>). See <see cref="Selectable"/>.</summary>
    Selected = 0x00800000,
    /// <summary>The object announces itself audibly (<c>wxACC_STATE_SYSTEM_SELFVOICING</c>).</summary>
    SelfVoicing = 0x01000000,
    /// <summary>The object is present but unavailable, such as a disabled control (<c>wxACC_STATE_SYSTEM_UNAVAILABLE</c>).</summary>
    Unavailable = 0x02000000,
}
