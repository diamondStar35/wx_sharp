using System;

namespace WxSharp;

/// <summary>A colour from the user's theme, following <c>wxSystemColour</c>. The values are wxWidgets'.</summary>
public enum SystemColour
{
    ScrollBar = 0,
    Desktop = 1,
    ActiveCaption = 2,
    InactiveCaption = 3,
    Menu = 4,
    Window = 5,
    WindowFrame = 6,
    MenuText = 7,
    WindowText = 8,
    CaptionText = 9,
    ActiveBorder = 10,
    InactiveBorder = 11,
    AppWorkspace = 12,
    Highlight = 13,
    HighlightText = 14,
    ButtonFace = 15,
    ButtonShadow = 16,
    GrayText = 17,
    ButtonText = 18,
    InactiveCaptionText = 19,
    ButtonHighlight = 20,
    ThreeDDarkShadow = 21,
    ThreeDLight = 22,
    InfoText = 23,
    InfoBackground = 24,
    ListBox = 25,
    HotLight = 26,
    ListBoxText = 38,
    ListBoxHighlightText = 39,
}

/// <summary>A measurement from the current theme or hardware, following <c>wxSystemMetric</c>.</summary>
public enum SystemMetric
{
    MouseButtons = 1,
    Border = 2,
    CursorX = 3,
    CursorY = 4,
    DClickX = 5,
    DClickY = 6,
    DragX = 7,
    DragY = 8,
    EdgeX = 9,
    EdgeY = 10,
    HScrollArrowX = 11,
    HScrollArrowY = 12,
    HThumbX = 13,
    IconX = 14,
    IconY = 15,
    IconSpacingX = 16,
    IconSpacingY = 17,
    WindowMinX = 18,
    WindowMinY = 19,
    ScreenX = 20,
    ScreenY = 21,
    FrameSizeX = 22,
    FrameSizeY = 23,
    SmallIconX = 24,
    SmallIconY = 25,
    HScrollY = 26,
    VScrollX = 27,
    VScrollArrowX = 28,
    VScrollArrowY = 29,
    VThumbY = 30,
    CaptionY = 31,
    MenuY = 32,
    NetworkPresent = 33,
    PenWindowsPresent = 34,
    ShowSounds = 35,
    SwapButtons = 36,
    DClickMSec = 37,
    CaretOnMSec = 38,
    CaretOffMSec = 39,
    CaretTimeoutMSec = 40,
}

/// <summary>Roughly how big the display is, following <c>wxSystemScreenType</c>.</summary>
public enum SystemScreenType
{
    None = 0,
    Tiny = 1,
    PdaSmall = 2,
    PdaLarge = 3,
    DesktopSmall = 4,
    DesktopLarge = 5,
}

/// <summary>An optional platform capability, following <c>wxSystemFeature</c>.</summary>
public enum SystemFeature
{
    CanDrawFrameDecorations = 1,
    CanIconizeFrame = 2,
    TabletPresent = 3,
}

/// <summary>What the user's theme says, following <c>wxSystemSettings</c>.</summary>
///
/// <remarks>
/// An application that hard-codes colours stops working in a high-contrast scheme, which is exactly the
/// scheme some people rely on. Ask for the colours here instead, and the interface follows whatever the user
/// has chosen — including a dark theme, which <see cref="IsDarkAppearance"/> reports.
/// </remarks>
/// <summary>One of the fonts the platform itself uses, following <c>wxSystemFont</c>. A themed interface
/// starts from these rather than from a hard-coded family and size, so it follows whatever the user has
/// chosen.</summary>
public enum SystemFont
{
    OemFixed = 10,
    AnsiFixed = 11,
    AnsiVariable = 12,
    System = 13,
    DeviceDefault = 14,
    SystemFixed = 16,
    /// <summary>The font dialogs and controls are drawn in - what an application should normally use.</summary>
    DefaultGui = 17,
}

public static class SystemSettings
{
    /// <summary>A colour from the current theme.</summary>
    /// <summary>One of the platform's own fonts. Following <c>wxSystemSettings.GetFont</c>; the caller owns
    /// the returned font and should dispose it.</summary>
    public static Font GetFont(SystemFont which)
    {
        _ = App.RequireCurrent();
        return Font.Attach(NativeMethods.wxsharp_font_from_system((int)which));
    }

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
