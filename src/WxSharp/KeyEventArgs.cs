namespace WxSharp;

/// <summary>A key press reported to a <see cref="Container.KeyDown"/> handler. Set <see cref="Handled"/> to
/// consume the key. Compare <see cref="Code"/> against <see cref="Key"/> for the named keys; letters/digits
/// arrive as their character code (e.g. 'A' = 65).</summary>
public sealed class KeyEventArgs
{
    public int KeyCode { get; }
    public bool Control { get; }
    public bool Shift { get; }
    public bool Alt { get; }
    public bool Handled { get; set; }

    internal KeyEventArgs(int keyCode, bool control, bool shift, bool alt)
    {
        KeyCode = keyCode;
        Control = control;
        Shift = shift;
        Alt = alt;
    }

    /// <summary>The key as a named value (cast of <see cref="KeyCode"/>); unnamed keys keep their raw code.</summary>
    public Key Code => (Key)KeyCode;
}

/// <summary>Named key codes (the wxWidgets <c>WXK_*</c> values). Letters and digits are not listed - they
/// come through as their character code.</summary>
public enum Key
{
    Back = 8,
    Tab = 9,
    Enter = 13,
    Escape = 27,
    Space = 32,
    Delete = 127,
    End = 312,
    Home = 313,
    Left = 314,
    Up = 315,
    Right = 316,
    Down = 317,
    Insert = 322,
    F1 = 340,
    F2 = 341,
    F3 = 342,
    F4 = 343,
    F5 = 344,
    F6 = 345,
    F7 = 346,
    F8 = 347,
    F9 = 348,
    F10 = 349,
    F11 = 350,
    F12 = 351,
    PageUp = 366,
    PageDown = 367,
}
