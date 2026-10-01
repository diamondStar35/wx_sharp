using System;

namespace WxSharp;

/// <summary>A colour from the user's theme, following <c>wxSystemColour</c>. The values are wxWidgets'.</summary>
public enum SystemColour
{
    /// <summary>Scrollbar track colour (<c>wxSYS_COLOUR_SCROLLBAR</c>).</summary>
    ScrollBar = 0,
    /// <summary>Desktop background colour (<c>wxSYS_COLOUR_DESKTOP</c>).</summary>
    Desktop = 1,
    /// <summary>Active title bar colour (<c>wxSYS_COLOUR_ACTIVECAPTION</c>).</summary>
    ActiveCaption = 2,
    /// <summary>Inactive title bar colour (<c>wxSYS_COLOUR_INACTIVECAPTION</c>).</summary>
    InactiveCaption = 3,
    /// <summary>Menu background colour (<c>wxSYS_COLOUR_MENU</c>).</summary>
    Menu = 4,
    /// <summary>Window background colour (<c>wxSYS_COLOUR_WINDOW</c>).</summary>
    Window = 5,
    /// <summary>Window frame colour (<c>wxSYS_COLOUR_WINDOWFRAME</c>).</summary>
    WindowFrame = 6,
    /// <summary>Menu text colour (<c>wxSYS_COLOUR_MENUTEXT</c>).</summary>
    MenuText = 7,
    /// <summary>Window text colour (<c>wxSYS_COLOUR_WINDOWTEXT</c>).</summary>
    WindowText = 8,
    /// <summary>Title bar text colour (<c>wxSYS_COLOUR_CAPTIONTEXT</c>).</summary>
    CaptionText = 9,
    /// <summary>Active window border colour (<c>wxSYS_COLOUR_ACTIVEBORDER</c>).</summary>
    ActiveBorder = 10,
    /// <summary>Inactive window border colour (<c>wxSYS_COLOUR_INACTIVEBORDER</c>).</summary>
    InactiveBorder = 11,
    /// <summary>MDI application workspace colour (<c>wxSYS_COLOUR_APPWORKSPACE</c>).</summary>
    AppWorkspace = 12,
    /// <summary>Selection background colour (<c>wxSYS_COLOUR_HIGHLIGHT</c>).</summary>
    Highlight = 13,
    /// <summary>Selection text colour (<c>wxSYS_COLOUR_HIGHLIGHTTEXT</c>).</summary>
    HighlightText = 14,
    /// <summary>Button face colour (<c>wxSYS_COLOUR_BTNFACE</c>).</summary>
    ButtonFace = 15,
    /// <summary>Button shadow colour (<c>wxSYS_COLOUR_BTNSHADOW</c>).</summary>
    ButtonShadow = 16,
    /// <summary>Disabled (greyed) text colour (<c>wxSYS_COLOUR_GRAYTEXT</c>).</summary>
    GrayText = 17,
    /// <summary>Button text colour (<c>wxSYS_COLOUR_BTNTEXT</c>).</summary>
    ButtonText = 18,
    /// <summary>Inactive title bar text colour (<c>wxSYS_COLOUR_INACTIVECAPTIONTEXT</c>).</summary>
    InactiveCaptionText = 19,
    /// <summary>Button highlight colour (<c>wxSYS_COLOUR_BTNHIGHLIGHT</c>).</summary>
    ButtonHighlight = 20,
    /// <summary>Dark 3-D shadow colour (<c>wxSYS_COLOUR_3DDKSHADOW</c>).</summary>
    ThreeDDarkShadow = 21,
    /// <summary>Light 3-D edge colour (<c>wxSYS_COLOUR_3DLIGHT</c>).</summary>
    ThreeDLight = 22,
    /// <summary>Tooltip text colour (<c>wxSYS_COLOUR_INFOTEXT</c>).</summary>
    InfoText = 23,
    /// <summary>Tooltip background colour (<c>wxSYS_COLOUR_INFOBK</c>).</summary>
    InfoBackground = 24,
    /// <summary>List box background colour (<c>wxSYS_COLOUR_LISTBOX</c>).</summary>
    ListBox = 25,
    /// <summary>Hot-tracked item colour (<c>wxSYS_COLOUR_HOTLIGHT</c>).</summary>
    HotLight = 26,
    /// <summary>List box text colour (<c>wxSYS_COLOUR_LISTBOXTEXT</c>).</summary>
    ListBoxText = 38,
    /// <summary>Selected list box text colour (<c>wxSYS_COLOUR_LISTBOXHIGHLIGHTTEXT</c>).</summary>
    ListBoxHighlightText = 39,
}

/// <summary>A measurement from the current theme or hardware, following <c>wxSystemMetric</c>.</summary>
public enum SystemMetric
{
    /// <summary>Number of mouse buttons (<c>wxSYS_MOUSE_BUTTONS</c>).</summary>
    MouseButtons = 1,
    /// <summary>Window border thickness (<c>wxSYS_BORDER_X</c>/<c>_Y</c>).</summary>
    Border = 2,
    /// <summary>Cursor width (<c>wxSYS_CURSOR_X</c>).</summary>
    CursorX = 3,
    /// <summary>Cursor height (<c>wxSYS_CURSOR_Y</c>).</summary>
    CursorY = 4,
    /// <summary>Double-click horizontal tolerance (<c>wxSYS_DCLICK_X</c>).</summary>
    DClickX = 5,
    /// <summary>Double-click vertical tolerance (<c>wxSYS_DCLICK_Y</c>).</summary>
    DClickY = 6,
    /// <summary>Drag-start horizontal threshold (<c>wxSYS_DRAG_X</c>).</summary>
    DragX = 7,
    /// <summary>Drag-start vertical threshold (<c>wxSYS_DRAG_Y</c>).</summary>
    DragY = 8,
    /// <summary>3-D edge width (<c>wxSYS_EDGE_X</c>).</summary>
    EdgeX = 9,
    /// <summary>3-D edge height (<c>wxSYS_EDGE_Y</c>).</summary>
    EdgeY = 10,
    /// <summary>Horizontal scrollbar arrow width (<c>wxSYS_HSCROLL_ARROW_X</c>).</summary>
    HScrollArrowX = 11,
    /// <summary>Horizontal scrollbar arrow height (<c>wxSYS_HSCROLL_ARROW_Y</c>).</summary>
    HScrollArrowY = 12,
    /// <summary>Horizontal scrollbar thumb width (<c>wxSYS_HTHUMB_X</c>).</summary>
    HThumbX = 13,
    /// <summary>Icon width (<c>wxSYS_ICON_X</c>).</summary>
    IconX = 14,
    /// <summary>Icon height (<c>wxSYS_ICON_Y</c>).</summary>
    IconY = 15,
    /// <summary>Icon grid horizontal spacing (<c>wxSYS_ICONSPACING_X</c>).</summary>
    IconSpacingX = 16,
    /// <summary>Icon grid vertical spacing (<c>wxSYS_ICONSPACING_Y</c>).</summary>
    IconSpacingY = 17,
    /// <summary>Minimum window width (<c>wxSYS_WINDOWMIN_X</c>).</summary>
    WindowMinX = 18,
    /// <summary>Minimum window height (<c>wxSYS_WINDOWMIN_Y</c>).</summary>
    WindowMinY = 19,
    /// <summary>Screen width in pixels (<c>wxSYS_SCREEN_X</c>).</summary>
    ScreenX = 20,
    /// <summary>Screen height in pixels (<c>wxSYS_SCREEN_Y</c>).</summary>
    ScreenY = 21,
    /// <summary>Resizable frame border width (<c>wxSYS_FRAMESIZE_X</c>).</summary>
    FrameSizeX = 22,
    /// <summary>Resizable frame border height (<c>wxSYS_FRAMESIZE_Y</c>).</summary>
    FrameSizeY = 23,
    /// <summary>Small icon width (<c>wxSYS_SMALLICON_X</c>).</summary>
    SmallIconX = 24,
    /// <summary>Small icon height (<c>wxSYS_SMALLICON_Y</c>).</summary>
    SmallIconY = 25,
    /// <summary>Horizontal scrollbar height (<c>wxSYS_HSCROLL_Y</c>).</summary>
    HScrollY = 26,
    /// <summary>Vertical scrollbar width (<c>wxSYS_VSCROLL_X</c>).</summary>
    VScrollX = 27,
    /// <summary>Vertical scrollbar arrow width (<c>wxSYS_VSCROLL_ARROW_X</c>).</summary>
    VScrollArrowX = 28,
    /// <summary>Vertical scrollbar arrow height (<c>wxSYS_VSCROLL_ARROW_Y</c>).</summary>
    VScrollArrowY = 29,
    /// <summary>Vertical scrollbar thumb height (<c>wxSYS_VTHUMB_Y</c>).</summary>
    VThumbY = 30,
    /// <summary>Title bar height (<c>wxSYS_CAPTION_Y</c>).</summary>
    CaptionY = 31,
    /// <summary>Menu bar height (<c>wxSYS_MENU_Y</c>).</summary>
    MenuY = 32,
    /// <summary>Non-zero if a network is present (<c>wxSYS_NETWORK_PRESENT</c>).</summary>
    NetworkPresent = 33,
    /// <summary>Non-zero if Windows for Pen Computing is present (<c>wxSYS_PENWINDOWS_PRESENT</c>).</summary>
    PenWindowsPresent = 34,
    /// <summary>Non-zero if "show sounds" accessibility is on (<c>wxSYS_SHOW_SOUNDS</c>).</summary>
    ShowSounds = 35,
    /// <summary>Non-zero if the mouse buttons are swapped (<c>wxSYS_SWAP_BUTTONS</c>).</summary>
    SwapButtons = 36,
    /// <summary>Double-click interval in milliseconds (<c>wxSYS_DCLICK_MSEC</c>).</summary>
    DClickMSec = 37,
    /// <summary>Caret on time in milliseconds (<c>wxSYS_CARET_ON_MSEC</c>).</summary>
    CaretOnMSec = 38,
    /// <summary>Caret off time in milliseconds (<c>wxSYS_CARET_OFF_MSEC</c>).</summary>
    CaretOffMSec = 39,
    /// <summary>Caret blink timeout in milliseconds (<c>wxSYS_CARET_TIMEOUT_MSEC</c>).</summary>
    CaretTimeoutMSec = 40,
}

/// <summary>Roughly how big the display is, following <c>wxSystemScreenType</c>.</summary>
public enum SystemScreenType
{
    /// <summary>Screen size unknown.</summary>
    None = 0,
    /// <summary>A tiny screen, such as a smartwatch.</summary>
    Tiny = 1,
    /// <summary>A small handheld screen in portrait.</summary>
    PdaSmall = 2,
    /// <summary>A larger handheld screen.</summary>
    PdaLarge = 3,
    /// <summary>A small desktop or laptop screen.</summary>
    DesktopSmall = 4,
    /// <summary>A large desktop screen.</summary>
    DesktopLarge = 5,
}

/// <summary>An optional platform capability, following <c>wxSystemFeature</c>.</summary>
public enum SystemFeature
{
    /// <summary>The platform can draw window frame decorations itself (<c>wxSYS_CAN_DRAW_FRAME_DECORATIONS</c>).</summary>
    CanDrawFrameDecorations = 1,
    /// <summary>Frames can be iconized (<c>wxSYS_CAN_ICONIZE_FRAME</c>).</summary>
    CanIconizeFrame = 2,
    /// <summary>A tablet is present (<c>wxSYS_TABLET_PRESENT</c>).</summary>
    TabletPresent = 3,
}

/// <summary>One of the fonts the platform itself uses, following <c>wxSystemFont</c>. A themed interface
/// starts from these rather than from a hard-coded family and size, so it follows whatever the user has
/// chosen.</summary>
public enum SystemFont
{
    /// <summary>The OEM fixed-pitch font (<c>wxSYS_OEM_FIXED_FONT</c>).</summary>
    OemFixed = 10,
    /// <summary>The ANSI fixed-pitch font (<c>wxSYS_ANSI_FIXED_FONT</c>).</summary>
    AnsiFixed = 11,
    /// <summary>The ANSI variable-pitch font (<c>wxSYS_ANSI_VAR_FONT</c>).</summary>
    AnsiVariable = 12,
    /// <summary>The system font (<c>wxSYS_SYSTEM_FONT</c>).</summary>
    System = 13,
    /// <summary>The device default font (<c>wxSYS_DEVICE_DEFAULT_FONT</c>).</summary>
    DeviceDefault = 14,
    /// <summary>The fixed-pitch system font (<c>wxSYS_SYSTEM_FIXED_FONT</c>).</summary>
    SystemFixed = 16,
    /// <summary>The font dialogs and controls are drawn in - what an application should normally use.</summary>
    DefaultGui = 17,
}

/// <summary>What the user's theme says, following <c>wxSystemSettings</c>.</summary>
///
/// <remarks>
/// An application that hard-codes colours stops working in a high-contrast scheme, which is exactly the
/// scheme some people rely on. Ask for the colours here instead, and the interface follows whatever the user
/// has chosen — including a dark theme, which <see cref="IsDarkAppearance"/> reports.
/// </remarks>
public static class SystemSettings
{
    /// <summary>One of the platform's own fonts. Following <c>wxSystemSettings.GetFont</c>; the caller owns
    /// the returned font and should dispose it.</summary>
    public static Font GetFont(SystemFont which)
    {
        _ = App.RequireCurrent();
        return Font.Attach(NativeMethods.wxsharp_font_from_system((int)which));
    }

    /// <summary>A colour from the current theme. Use this instead of a hard-coded colour so the interface
    /// follows the user's scheme.</summary>
    public static Colour GetColour(SystemColour which)
    {
        _ = App.RequireCurrent();
        return Colour.FromArgb(NativeMethods.wxsharp_system_colour((int)which));
    }

    /// <summary>A metric from the current theme or hardware. Some are per-window, so pass the window when
    /// there is one; -1 comes back when the platform does not know.</summary>
    public static int GetMetric(SystemMetric which, Window? window = null)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_system_metric((int)which, window?.Handle ?? 0);
    }

    /// <summary>Roughly how big the display is.</summary>
    public static SystemScreenType ScreenType
    {
        get { _ = App.RequireCurrent(); return (SystemScreenType)NativeMethods.wxsharp_system_screen_type(); }
    }

    /// <summary>Whether the platform offers an optional capability.</summary>
    public static bool HasFeature(SystemFeature feature)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_system_has_feature((int)feature);
    }

    /// <summary>Whether the user is running a dark theme. Worth checking before choosing any colour of your
    /// own, so it still reads against the background the system will draw.</summary>
    public static bool IsDarkAppearance
    {
        get { _ = App.RequireCurrent(); return NativeMethods.wxsharp_system_appearance_is_dark(); }
    }

    /// <summary>The platform's name for the current appearance, where it has one.</summary>
    public static unsafe string AppearanceName
    {
        get
        {
            _ = App.RequireCurrent();
            var length = NativeMethods.wxsharp_system_appearance_name(null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_system_appearance_name(p, buffer.Length);
            return Utf8String.Decode(buffer, length);
        }
    }
}
