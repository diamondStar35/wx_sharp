using System;
using System.Runtime.InteropServices;

namespace WxSharp;

internal enum EventKind
{
    Click = 1, Close = 2, Text = 3, Toggle = 4, Select = 5, Slider = 6, Shown = 7,
    Activate = 8, Deactivate = 9, Resize = 10, FocusGained = 11, FocusLost = 12, TextEnter = 13,
    Move = 14, Maximize = 15, MouseDown = 16, MouseUp = 17, MouseRight = 18, MouseDouble = 19,
    MouseEnter = 20, MouseLeave = 21, MouseMove = 22, Paint = 23, MouseWheel = 24,
    Destroyed = 26, CallAfter = 27, KeyHook = 28, KeyDown = 29, KeyUp = 30, Menu = 31, Timer = 32,
}

[StructLayout(LayoutKind.Sequential)]
internal struct NativeEvent
{
    internal uint Size, Version;
    internal long Token;
    internal EventKind Kind;
    internal int Id, X, Y, Width, Height, KeyCode, Modifiers, MouseButton, WheelDelta, Active, CanVeto;
}

[StructLayout(LayoutKind.Sequential)]
internal struct NativeAccelerator { internal int Modifiers, KeyCode, CommandId; }

public class WxEventArgs : EventArgs
{
    public Window Source { get; }
    public int Id { get; }
    public bool Handled { get; set; }
    internal WxEventArgs(Window source, int id) { Source = source; Id = id; }
    public void Skip(bool skip = true) => Handled = !skip;
}

public sealed class EventType<TEventArgs> where TEventArgs : WxEventArgs
{
    internal EventKind Kind { get; }
    internal EventType(EventKind kind) => Kind = kind;
}

public static class WxEvents
{
    public static EventType<CommandEventArgs> ButtonClicked { get; } = new(EventKind.Click);
    public static EventType<CloseEventArgs> Closing { get; } = new(EventKind.Close);
    public static EventType<CommandEventArgs> TextChanged { get; } = new(EventKind.Text);
    public static EventType<CommandEventArgs> Toggled { get; } = new(EventKind.Toggle);
    public static EventType<CommandEventArgs> SelectionChanged { get; } = new(EventKind.Select);
    public static EventType<CommandEventArgs> ValueChanged { get; } = new(EventKind.Slider);
    public static EventType<WxEventArgs> Shown { get; } = new(EventKind.Shown);
    public static EventType<ActivateEventArgs> Activated { get; } = new(EventKind.Activate);
    public static EventType<ActivateEventArgs> Deactivated { get; } = new(EventKind.Deactivate);
    public static EventType<SizeEventArgs> SizeChanged { get; } = new(EventKind.Resize);
    public static EventType<WxEventArgs> GotFocus { get; } = new(EventKind.FocusGained);
    public static EventType<WxEventArgs> LostFocus { get; } = new(EventKind.FocusLost);
    public static EventType<CommandEventArgs> TextEntered { get; } = new(EventKind.TextEnter);
    public static EventType<MoveEventArgs> Moved { get; } = new(EventKind.Move);
    public static EventType<MouseEventArgs> MouseDown { get; } = new(EventKind.MouseDown);
    public static EventType<MouseEventArgs> MouseUp { get; } = new(EventKind.MouseUp);
    public static EventType<MouseEventArgs> RightClicked { get; } = new(EventKind.MouseRight);
    public static EventType<MouseEventArgs> DoubleClicked { get; } = new(EventKind.MouseDouble);
    public static EventType<MouseEventArgs> MouseEntered { get; } = new(EventKind.MouseEnter);
    public static EventType<MouseEventArgs> MouseLeft { get; } = new(EventKind.MouseLeave);
    public static EventType<MouseEventArgs> MouseMoved { get; } = new(EventKind.MouseMove);
    public static EventType<MouseEventArgs> MouseWheel { get; } = new(EventKind.MouseWheel);
    public static EventType<KeyEventArgs> KeyDown { get; } = new(EventKind.KeyDown);
    public static EventType<KeyEventArgs> KeyUp { get; } = new(EventKind.KeyUp);
    public static EventType<PaintEventArgs> Paint { get; } = new(EventKind.Paint);
    public static EventType<WxEventArgs> Destroyed { get; } = new(EventKind.Destroyed);
    public static EventType<CommandEventArgs> MenuCommand { get; } = new(EventKind.Menu);
    public static EventType<CommandEventArgs> Timer { get; } = new(EventKind.Timer);
}

public sealed class EventBinding : IDisposable
{
    private Window? _window;
    internal long Token { get; }
    internal EventBinding(Window window, long token) { _window = window; Token = token; }
    public void Dispose()
    {
        var window = _window; if (window is null) return;
        _window = null; window.RemoveBinding(Token);
    }
}

public sealed class CommandEventArgs : WxEventArgs
{
    internal CommandEventArgs(Window source, int id) : base(source, id) { }
}

public sealed class SelectionEventArgs : WxEventArgs
{
    public int Selection { get; }
    public int PreviousSelection { get; }
    internal SelectionEventArgs(Window source, int id, int selection, int previousSelection) : base(source, id)
    {
        Selection = selection;
        PreviousSelection = previousSelection;
    }
}

public sealed class CloseEventArgs : WxEventArgs
{
    public bool CanCancel { get; }
    public bool Cancel { get; set; }
    internal CloseEventArgs(Window source, int id, bool canCancel) : base(source, id) => CanCancel = canCancel;
}

public sealed class KeyEventArgs : WxEventArgs
{
    public int KeyCode { get; }
    public Key Code => (Key)KeyCode;
    public bool Control { get; }
    public bool Shift { get; }
    public bool Alt { get; }
    internal KeyEventArgs(Window source, in NativeEvent e) : base(source, e.Id)
    {
        KeyCode = e.KeyCode;
        Control = (e.Modifiers & 1) != 0; Shift = (e.Modifiers & 2) != 0; Alt = (e.Modifiers & 4) != 0;
    }
}

public enum MouseButton { None = 0, Left = 1, Right = 2, Middle = 3 }

public sealed class MouseEventArgs : WxEventArgs
{
    public Point Position { get; }
    public MouseButton Button { get; }
    public int WheelDelta { get; }
    public bool Control { get; }
    public bool Shift { get; }
    public bool Alt { get; }
    internal MouseEventArgs(Window source, in NativeEvent e) : base(source, e.Id)
    {
        Position = new Point(e.X, e.Y); Button = (MouseButton)e.MouseButton; WheelDelta = e.WheelDelta;
        Control = (e.Modifiers & 1) != 0; Shift = (e.Modifiers & 2) != 0; Alt = (e.Modifiers & 4) != 0;
    }
}

public sealed class SizeEventArgs : WxEventArgs
{
    public Size Size { get; }
    internal SizeEventArgs(Window source, in NativeEvent e) : base(source, e.Id) => Size = new Size(e.Width, e.Height);
}

public sealed class MoveEventArgs : WxEventArgs
{
    public Point Position { get; }
    internal MoveEventArgs(Window source, in NativeEvent e) : base(source, e.Id) => Position = new Point(e.X, e.Y);
}

public sealed class ActivateEventArgs : WxEventArgs
{
    public bool Active { get; }
    internal ActivateEventArgs(Window source, in NativeEvent e) : base(source, e.Id) => Active = e.Active != 0;
}

public sealed class PaintEventArgs : WxEventArgs
{
    internal PaintEventArgs(Window source, int id) : base(source, id) { }
}

public enum Key
{
    Back = 8, Tab = 9, Enter = 13, Escape = 27, Space = 32, Delete = 127,
    End = 312, Home = 313, Left = 314, Up = 315, Right = 316, Down = 317, Insert = 322,
    F1 = 340, F2 = 341, F3 = 342, F4 = 343, F5 = 344, F6 = 345, F7 = 346, F8 = 347,
    F9 = 348, F10 = 349, F11 = 350, F12 = 351, PageUp = 366, PageDown = 367,
}
