namespace WxSharp;

// The native shim reports every event through one callback as (managed-id, event-kind). These values mirror
// the WXSHARP_EVT_* enum in the native internal.h; keep the two in sync.
internal enum EventKind
{
    Click = 1,        // button pressed
    Close = 2,        // window/dialog close requested
    Text = 3,         // text control changed
    Toggle = 4,       // checkbox toggled
    Select = 5,       // choice/listbox/radio selection changed
    Slider = 6,       // slider value changed
    Shown = 7,        // window shown
    Activate = 8,     // window activated (got focus)
    Deactivate = 9,   // window deactivated (lost focus)
    Resize = 10,      // window resized
    FocusGained = 11, // control gained keyboard focus
    FocusLost = 12,   // control lost keyboard focus
    TextEnter = 13,   // Enter pressed in a single-line text control
    Move = 14,        // window moved
    Maximize = 15,    // window maximized
    MouseDown = 16,   // left button pressed on a control
    MouseUp = 17,     // left button released on a control
    MouseRight = 18,  // right button pressed (context click)
    MouseDouble = 19, // left double-click
    MouseEnter = 20,  // pointer entered the control
    MouseLeave = 21,  // pointer left the control
    MouseMove = 22,   // pointer moved over the control
    Paint = 23,       // a canvas needs repainting
    WheelUp = 24,     // mouse wheel scrolled up/away
    WheelDown = 25,   // mouse wheel scrolled down/toward
}

// The kind of key event the native key callback reports (mirrors the WXSHARP_KEY_* enum in internal.h).
internal enum KeyKind
{
    Hook = 1, // char hook on a top-level window (global shortcuts)
    Down = 2, // key pressed on a focused control
    Up = 3,   // key released on a focused control
}
