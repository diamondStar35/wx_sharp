using System;
using System.Runtime.InteropServices;

namespace WxSharp;

/// <summary>Wrapper event identifiers, mirroring the <c>WXSHARP_EV_*</c> values in <c>wxsharp.h</c>. Each one
/// maps to exactly one wxWidgets event type through the table in <c>events.cpp</c>.</summary>
internal static class EventId
{
    internal const int Close = 1;
    internal const int Show = 2;
    internal const int Activate = 3;
    internal const int Size = 4;
    internal const int Move = 5;
    internal const int Maximize = 6;
    internal const int Iconize = 7;
    internal const int Destroy = 8;
    internal const int SetFocus = 9;
    internal const int KillFocus = 10;
    internal const int Paint = 11;
    internal const int ContextMenu = 12;
    internal const int UpdateUI = 13;
    internal const int Idle = 14;
    internal const int ChildFocus = 15;
    internal const int NavigationKey = 16;
    internal const int MouseCaptureLost = 17;
    internal const int MouseCaptureChanged = 18;
    internal const int DropFiles = 19;
    internal const int HotKey = 20;
    internal const int Help = 21;
    internal const int MenuOpen = 22;
    internal const int MenuClose = 23;
    internal const int MenuHighlight = 24;

    internal const int LeftDown = 31;
    internal const int LeftUp = 32;
    internal const int LeftDoubleClick = 33;
    internal const int RightDown = 34;
    internal const int RightUp = 35;
    internal const int RightDoubleClick = 36;
    internal const int MiddleDown = 37;
    internal const int MiddleUp = 38;
    internal const int MiddleDoubleClick = 39;
    internal const int Motion = 40;
    internal const int EnterWindow = 41;
    internal const int LeaveWindow = 42;
    internal const int MouseWheel = 43;

    internal const int CharHook = 51;
    internal const int KeyDown = 52;
    internal const int KeyUp = 53;
    internal const int Char = 54;

    internal const int Button = 61;
    internal const int CheckBox = 62;
    internal const int Choice = 63;
    internal const int ListBox = 64;
    internal const int ListBoxDoubleClick = 65;
    internal const int Text = 66;
    internal const int TextEnter = 67;
    internal const int Menu = 68;
    internal const int Slider = 69;
    internal const int RadioButton = 70;
    internal const int RadioBox = 71;
    internal const int ComboBox = 72;
    internal const int ToggleButton = 73;
    internal const int CheckListBox = 74;
    internal const int SpinCtrl = 75;
    internal const int SpinCtrlDouble = 76;
    internal const int ScrollThumbTrack = 77;
    internal const int ScrollChanged = 78;
    internal const int Hyperlink = 79;
    internal const int Search = 80;
    internal const int SearchCancel = 81;
    internal const int DateChanged = 82;
    internal const int TimeChanged = 83;
    internal const int ComboBoxDropDown = 84;
    internal const int ComboBoxCloseUp = 85;
    internal const int Timer = 86;
    internal const int Spin = 87;
    internal const int SpinUp = 88;
    internal const int SpinDown = 89;
    internal const int ScrollBar = 90;
    internal const int ScrollTop = 91;
    internal const int ScrollBottom = 92;
    internal const int ScrollLineUp = 93;
    internal const int ScrollLineDown = 94;
    internal const int ScrollPageUp = 95;
    internal const int ScrollPageDown = 96;
    internal const int ScrollThumbRelease = 97;
    internal const int TextMaxLength = 98;
    internal const int TextUrl = 99;
    internal const int ListInsertItem = 100;

    internal const int NotebookPageChanged = 101;
    internal const int NotebookPageChanging = 102;
    internal const int BookPageChanged = 103;
    internal const int BookPageChanging = 104;

    internal const int ListItemSelected = 111;
    internal const int ListItemDeselected = 112;
    internal const int ListItemActivated = 113;
    internal const int ListItemFocused = 114;
    internal const int ListItemRightClick = 115;
    internal const int ListColumnClick = 116;
    internal const int ListKeyDown = 117;
    internal const int ListBeginLabelEdit = 118;
    internal const int ListEndLabelEdit = 119;
    internal const int ListBeginDrag = 120;
    internal const int ListBeginRightDrag = 121;
    internal const int ListItemMiddleClick = 122;
    internal const int ListItemChecked = 123;
    internal const int ListItemUnchecked = 124;
    internal const int ListColumnRightClick = 125;
    internal const int ListColumnBeginDrag = 126;
    internal const int ListColumnEndDrag = 127;
    internal const int ListDeleteItem = 128;
    internal const int ListDeleteAllItems = 129;
    internal const int ListCacheHint = 130;

    internal const int TreeSelectionChanged = 131;
    internal const int TreeSelectionChanging = 132;
    internal const int TreeItemActivated = 133;
    internal const int TreeItemExpanded = 134;
    internal const int TreeItemExpanding = 135;
    internal const int TreeItemCollapsed = 136;
    internal const int TreeItemCollapsing = 137;
    internal const int TreeItemRightClick = 138;
    internal const int TreeKeyDown = 139;
    internal const int TreeBeginLabelEdit = 140;
    internal const int TreeEndLabelEdit = 141;
    internal const int TreeItemMenu = 142;
    internal const int TreeBeginDrag = 143;
    internal const int TreeEndDrag = 144;
    internal const int TreeItemMiddleClick = 145;
    internal const int TreeDeleteItem = 146;
    internal const int TreeItemToolTip = 147;
    internal const int TreeStateImageClick = 148;

    internal const int DataViewSelectionChanged = 151;
    internal const int DataViewItemActivated = 152;
    internal const int DataViewItemContextMenu = 153;
    internal const int DataViewItemExpanded = 154;
    internal const int DataViewItemExpanding = 155;
    internal const int DataViewItemCollapsed = 156;
    internal const int DataViewItemCollapsing = 157;
    internal const int DataViewItemEditingStarted = 158;
    internal const int DataViewItemEditingDone = 159;
    internal const int DataViewItemValueChanged = 160;
    internal const int DataViewColumnHeaderClick = 161;
    internal const int DataViewColumnHeaderRightClick = 162;
    internal const int DataViewColumnSorted = 163;
    internal const int DataViewColumnReordered = 164;

    internal const int SplitterSashPositionChanged = 171;
    internal const int SplitterDoubleClick = 172;
    internal const int SplitterSashPositionChanging = 173;
    internal const int SplitterUnsplit = 174;

    internal const int GridCellChanged = 181;
    internal const int GridSelectCell = 182;

    internal const int ToolEnter = 191;
    internal const int ToolRightClick = 192;
    internal const int ToolDropDown = 193;

    internal const int TextCopy = 201;
    internal const int TextCut = 202;
    internal const int TextPaste = 203;

    internal const int CallAfter = 1001;

    /// <summary>Events the native side reports without being asked. Destruction has to be observed for every
    /// window; paints are driven by the canvas, which owns the device context; timer ticks come from the
    /// timer object rather than from a window. Subscribing to these must not attempt a native bind.</summary>
    internal static bool IsAlwaysReported(int eventId)
        => eventId is Destroy or Paint or Timer;
}

/// <summary>The layout of <c>wxsharp_event</c> (version 2). Field order and types must match exactly.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct NativeEvent
{
    internal uint Size;
    internal uint Version;
    internal long Token;
    internal long Item;
    internal long OldItem;
    internal double DoubleValue;
    internal nint Text;
    internal int Kind;
    internal int Id;
    internal int X, Y, Width, Height;
    internal int KeyCode, Modifiers, MouseButton, WheelDelta, Active, CanVeto;
    internal int Column, Selection, OldSelection, IntValue, TextLength;
    internal uint UintValue;

    internal const uint ExpectedVersion = 2;

    /// <summary>Copies the event's UTF-8 payload. The native buffer only lives for the callback.</summary>
    internal readonly string GetText()
        => Text == 0 || TextLength <= 0 ? string.Empty : Utf8String.Decode(Text, TextLength);
}

[StructLayout(LayoutKind.Sequential)]
internal struct NativeAccelerator { internal int Modifiers, KeyCode, CommandId; }

/// <summary>Builds the arguments for one event kind. Held by <see cref="EventType{TEventArgs}"/> so the
/// managed side needs no reflection or type switch to construct them.</summary>
internal delegate WxEventArgs EventArgsFactory(Window source, in NativeEvent e);

/// <summary>The modifier keys held down when an input event was raised.</summary>
[Flags]
public enum KeyModifiers
{
    None = 0,
    Control = 1,
    Shift = 2,
    Alt = 4,
    /// <summary>The Windows key, or Command on macOS.</summary>
    Meta = 8,
    /// <summary>The physical Control key. Identical to <see cref="Control"/> except on macOS, where
    /// <see cref="Control"/> means Command.</summary>
    RawControl = 16,
}

public class WxEventArgs : EventArgs
{
    public Window Source { get; }
    public int Id { get; }

    /// <summary>Whether this handler asked for normal processing to continue. False unless
    /// <see cref="Skip"/> was called.</summary>
    public bool Skipped { get; private set; }

    internal WxEventArgs(Window source, int id) { Source = source; Id = id; }
    internal WxEventArgs(Window source, in NativeEvent e) : this(source, e.Id) { }

    /// <summary>Asks for the event to be processed as though this handler had not run: the control's own
    /// behaviour, the next handler, and - for a command event - propagation to the parent.</summary>
    ///
    /// <remarks>
    /// Handling an event stops it. That is wxWidgets' model and Phoenix's, and it is easy to trip over:
    /// binding <see cref="WxEvents.SizeChanged"/> or <see cref="WxEvents.Closing"/> and returning without
    /// calling this consumes the event, and the window will not lay out or will not close. Skip whenever
    /// the handler is observing rather than deciding.
    /// </remarks>
    public void Skip(bool skip = true) => Skipped = skip;

    /// <summary>Clears the flag before each handler, so one handler skipping does not decide for the next.</summary>
    internal void ResetSkipped() => Skipped = false;
}

/// <summary>Base for the events wxWidgets lets a handler refuse, following <c>wxNotifyEvent</c>. The action
/// goes ahead unless <see cref="Veto"/> is called.</summary>
public abstract class NotifyEventArgs : WxEventArgs
{
    /// <summary>Whether the action the event announced is still allowed.</summary>
    public bool IsAllowed { get; private set; } = true;

    /// <summary>Refuses the action.</summary>
    public void Veto() => IsAllowed = false;

    /// <summary>Allows the action, undoing an earlier <see cref="Veto"/>.</summary>
    public void Allow() => IsAllowed = true;

    internal NotifyEventArgs(Window source, in NativeEvent e) : base(source, e) { }
}

public sealed class EventType<TEventArgs> where TEventArgs : WxEventArgs
{
    internal int EventId { get; }
    internal EventArgsFactory Factory { get; }
    internal EventType(int eventId, EventArgsFactory factory) { EventId = eventId; Factory = factory; }
}

public sealed class EventBinding : IDisposable
{
    private Window? _window;
    internal int EventId { get; }
    internal long Token { get; }
    internal EventBinding(Window window, int eventId, long token)
    {
        _window = window; EventId = eventId; Token = token;
    }
    public void Dispose()
    {
        var window = _window;
        if (window is null) return;
        _window = null;
        window.RemoveBinding(EventId, Token);
    }
}

// ---- Event argument types ---------------------------------------------------------------------------------

public sealed class CommandEventArgs : WxEventArgs
{
    /// <summary>The event's integer payload: a list index, a checkbox state, a spin value - whatever the
    /// control reports. Matches Phoenix's <c>GetInt()</c>.</summary>
    public int Value { get; }

    /// <summary>The selected index for a list-like control, or -1 when there is none.</summary>
    public int Selection { get; }

    /// <summary>The event's string payload, empty when the control reports none.</summary>
    public string Text { get; }

    /// <summary>True when the command came from a control that is now checked.</summary>
    public bool IsChecked => Value != 0;

    internal CommandEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Value = e.IntValue;
        Selection = e.Selection;
        Text = e.GetText();
    }
}

/// <summary>A page change in a notebook or other book control.</summary>
public sealed class BookEventArgs : NotifyEventArgs
{
    public int Selection { get; }
    public int PreviousSelection { get; }
    internal BookEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Selection = e.Selection;
        PreviousSelection = e.OldSelection;
    }
}

/// <summary>A window is being asked to close, following <c>wxCloseEvent</c>.</summary>
public sealed class CloseEventArgs : WxEventArgs
{
    /// <summary>False when the close cannot be refused - a session shutdown, for instance.
    /// <see cref="Veto"/> then has no effect.</summary>
    public bool CanVeto { get; }

    /// <summary>Whether this handler has refused the close.</summary>
    public bool Vetoed { get; private set; }

    /// <summary>Refuses the close. Only meaningful when <see cref="CanVeto"/> is true.</summary>
    public void Veto(bool veto = true) => Vetoed = veto;

    internal CloseEventArgs(Window source, in NativeEvent e) : base(source, e) => CanVeto = e.CanVeto != 0;
}

public sealed class KeyEventArgs : WxEventArgs
{
    public int KeyCode { get; }
    public Key Code => (Key)KeyCode;
    public KeyModifiers Modifiers { get; }

    /// <summary>The character the key produces, or <c>'\0'</c> for a key with no character (an arrow, a
    /// function key). Only meaningful on a <c>Char</c> event.</summary>
    public char UnicodeKey { get; }

    /// <summary>The platform's own scan code, for keys wxWidgets does not name.</summary>
    public int RawKeyCode { get; }

    /// <summary>Where the pointer was when the key was pressed, in client coordinates.</summary>
    public Point Position { get; }

    public bool Control => (Modifiers & KeyModifiers.Control) != 0;
    public bool Shift => (Modifiers & KeyModifiers.Shift) != 0;
    public bool Alt => (Modifiers & KeyModifiers.Alt) != 0;
    public bool Meta => (Modifiers & KeyModifiers.Meta) != 0;

    internal KeyEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        KeyCode = e.KeyCode;
        Modifiers = (KeyModifiers)e.Modifiers;
        UnicodeKey = (char)e.UintValue;
        RawKeyCode = e.IntValue;
        Position = new Point(e.X, e.Y);
    }
}

public enum MouseButton { None = 0, Left = 1, Right = 2, Middle = 3 }

public sealed class MouseEventArgs : WxEventArgs
{
    public Point Position { get; }
    public MouseButton Button { get; }
    public KeyModifiers Modifiers { get; }

    /// <summary>How far the wheel turned. Positive is away from the user.</summary>
    public int WheelRotation { get; }

    /// <summary>The rotation that counts as one notch, for turning <see cref="WheelRotation"/> into lines.</summary>
    public int WheelDelta { get; }

    public bool Control => (Modifiers & KeyModifiers.Control) != 0;
    public bool Shift => (Modifiers & KeyModifiers.Shift) != 0;
    public bool Alt => (Modifiers & KeyModifiers.Alt) != 0;

    internal MouseEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Position = new Point(e.X, e.Y);
        Button = (MouseButton)e.MouseButton;
        Modifiers = (KeyModifiers)e.Modifiers;
        WheelRotation = e.WheelDelta;
        WheelDelta = e.IntValue == 0 ? 120 : e.IntValue;
    }
}

/// <summary>A context menu was requested, by right-click or by the keyboard's menu key.</summary>
public sealed class ContextMenuEventArgs : WxEventArgs
{
    /// <summary>Where the menu was asked for, in screen coordinates.</summary>
    public Point ScreenPosition { get; }

    /// <summary>True when the request came from the keyboard rather than the pointer, in which case the menu
    /// belongs at the focused item rather than under the mouse.</summary>
    public bool FromKeyboard { get; }

    internal ContextMenuEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        ScreenPosition = new Point(e.X, e.Y);
        FromKeyboard = e.X < 0 && e.Y < 0;
    }
}

public sealed class SizeEventArgs : WxEventArgs
{
    public Size Size { get; }
    internal SizeEventArgs(Window source, in NativeEvent e) : base(source, e) => Size = new Size(e.Width, e.Height);
}

public sealed class MoveEventArgs : WxEventArgs
{
    public Point Position { get; }
    internal MoveEventArgs(Window source, in NativeEvent e) : base(source, e) => Position = new Point(e.X, e.Y);
}

public sealed class ActivateEventArgs : WxEventArgs
{
    public bool Active { get; }
    internal ActivateEventArgs(Window source, in NativeEvent e) : base(source, e) => Active = e.Active != 0;
}

public sealed class ShowEventArgs : WxEventArgs
{
    public bool Shown { get; }
    internal ShowEventArgs(Window source, in NativeEvent e) : base(source, e) => Shown = e.Active != 0;
}

public sealed class PaintEventArgs : WxEventArgs
{
    internal PaintEventArgs(Window source, in NativeEvent e) : base(source, e) { }
}

/// <summary>An event from a <see cref="ListCtrl"/>.</summary>
public sealed class ListEventArgs : NotifyEventArgs
{
    public long Index { get; }
    public int Column { get; }

    /// <summary>The item's label, for the label-editing events.</summary>
    public string Label { get; }

    /// <summary>The key pressed, for <see cref="WxEvents.ListKeyDown"/>.</summary>
    public Key Code { get; }

    internal ListEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Index = e.Item;
        Column = e.Column;
        Label = e.GetText();
        Code = (Key)e.KeyCode;
    }
}

/// <summary>An event from a <see cref="TreeCtrl"/>.</summary>
public sealed class TreeEventArgs : NotifyEventArgs
{
    public TreeItemId Item { get; }

    /// <summary>The previously selected item, for the selection-changing events.</summary>
    public TreeItemId PreviousItem { get; }

    public string Label { get; }
    public Key Code { get; }

    internal TreeEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Item = new TreeItemId(e.Item);
        PreviousItem = new TreeItemId(e.OldItem);
        Label = e.GetText();
        Code = (Key)e.KeyCode;
    }
}

/// <summary>An event from a data-view control.</summary>
public sealed class DataViewEventArgs : WxEventArgs
{
    public DataViewItem Item { get; }
    public int Column { get; }
    internal DataViewEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Item = new DataViewItem(e.Item);
        Column = e.Column;
    }
}

/// <summary>A value change from a spin control or a scrollbar.</summary>
public sealed class SpinEventArgs : WxEventArgs
{
    public int Value { get; }
    public double DoubleValue { get; }
    internal SpinEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Value = e.IntValue;
        DoubleValue = e.DoubleValue;
    }
}

public sealed class ScrollEventArgs : WxEventArgs
{
    public int Position { get; }
    internal ScrollEventArgs(Window source, in NativeEvent e) : base(source, e) => Position = e.IntValue;
}

public sealed class SplitterEventArgs : NotifyEventArgs
{
    public int SashPosition { get; }
    internal SplitterEventArgs(Window source, in NativeEvent e) : base(source, e) => SashPosition = e.IntValue;
}

public sealed class GridEventArgs : WxEventArgs
{
    public int Row { get; }
    public int Column { get; }
    internal GridEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Row = (int)e.Item;
        Column = e.Column;
    }
}

/// <summary>A date or time change from a picker control.</summary>
public sealed class DateEventArgs : WxEventArgs
{
    /// <summary>The new value, or null when the picker holds no date.</summary>
    public DateTime? Date { get; }
    internal DateEventArgs(Window source, in NativeEvent e) : base(source, e)
        => Date = e.Active != 0 ? DateTimeOffset.FromUnixTimeMilliseconds(e.Item).UtcDateTime : null;
}

public sealed class HyperlinkEventArgs : WxEventArgs
{
    public string Url { get; }
    internal HyperlinkEventArgs(Window source, in NativeEvent e) : base(source, e) => Url = e.GetText();
}

/// <summary>The question wxWidgets asks about a command's state, on idle and whenever a menu is about to
/// open. A handler answers it; wxWidgets applies the answer to every menu item, toolbar button and other
/// control carrying that command ID.</summary>
///
/// <remarks>
/// This inverts how UI state is normally kept correct. Instead of remembering to disable "Play" from every
/// code path that could stop playback, one handler answers "should Play be enabled?" whenever the question
/// arises. Nothing can be forgotten, because nothing has to be remembered.
///
/// The properties and answer methods act on the live wxWidgets event, so they only take effect while the
/// event is being delivered. As with any event, the handler must not skip it: wxWidgets applies the answer
/// only to an event that comes back handled, which is the default.
/// </remarks>
public sealed class UpdateUIEventArgs : WxEventArgs
{
    internal UpdateUIEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        _enabled = e.Active != 0;
        _checked = e.IntValue != 0;
        _text = e.GetText();
    }

    private bool _enabled;
    private bool _checked;
    private bool _shown = true;
    private string _text;

    /// <summary>Whether the command should be available.</summary>
    public bool Enabled { get => _enabled; set => Enable(value); }

    /// <summary>Whether a check or radio command should be ticked.</summary>
    public bool Checked { get => _checked; set => Check(value); }

    /// <summary>Whether the command should be visible at all.</summary>
    public bool Shown { get => _shown; set => Show(value); }

    /// <summary>The command's label. Keep the accelerator suffix if the item has one, because replacing the
    /// text replaces all of it.</summary>
    public string Text { get => _text; set => SetText(value); }

    public void Enable(bool enable = true) { _enabled = enable; NativeMethods.wxsharp_updateui_enable(enable); }
    public void Check(bool check = true) { _checked = check; NativeMethods.wxsharp_updateui_check(check); }
    public void Show(bool show = true) { _shown = show; NativeMethods.wxsharp_updateui_show(show); }
    public void SetText(string text)
    {
        _text = text ?? string.Empty;
        NativeMethods.wxsharp_updateui_set_text(_text);
    }

    /// <summary>How often wxWidgets asks, in milliseconds. 0 means every idle cycle (the default) and -1
    /// suppresses the events. Follows <c>wxUpdateUIEvent.SetUpdateInterval</c>.</summary>
    public static void SetUpdateInterval(int milliseconds)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_updateui_set_interval(milliseconds);
    }

    /// <summary>Whether every window is asked, or only those that opt in. Follows
    /// <c>wxUpdateUIEvent.SetMode</c>.</summary>
    public static void SetMode(UpdateUIMode mode)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_updateui_set_process_all(mode == UpdateUIMode.ProcessAll);
    }
}

/// <summary>Which windows receive update-UI events, following <c>wxUpdateUIMode</c>.</summary>
public enum UpdateUIMode
{
    /// <summary>Every window is asked. The default.</summary>
    ProcessAll,
    /// <summary>Only windows that asked to be. Cheaper on a large interface.</summary>
    ProcessSpecified,
}

/// <summary>The application has nothing else to do. Where background work belongs, and where wxWidgets
/// drives <see cref="WxEvents.UpdateUI"/> from.</summary>
public sealed class IdleEventArgs : WxEventArgs
{
    /// <summary>Whether something has already asked to be woken again immediately.</summary>
    public bool MoreRequested { get; }
    internal IdleEventArgs(Window source, in NativeEvent e) : base(source, e) => MoreRequested = e.Active != 0;
}

/// <summary>A menu is opening, closing, or an item in it is highlighted.</summary>
public sealed class MenuEventArgs : WxEventArgs
{
    /// <summary>The highlighted item's command ID, for the highlight event. -1 for open and close.</summary>
    public int MenuId { get; }

    /// <summary>True when this is a context menu rather than one on the menu bar.</summary>
    public bool IsPopup { get; }

    internal MenuEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        MenuId = e.IntValue;
        IsPopup = e.Active != 0;
    }
}

/// <summary>Files were dragged onto a window.</summary>
public sealed class DropFilesEventArgs : WxEventArgs
{
    /// <summary>The dropped paths.</summary>
    public string[] Files { get; }

    /// <summary>Where they were dropped, in client coordinates.</summary>
    public Point Position { get; }

    internal unsafe DropFilesEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Position = new Point(e.X, e.Y);
        var count = NativeMethods.wxsharp_dropfiles_count();
        Files = count <= 0 ? Array.Empty<string>() : new string[count];
        for (var i = 0; i < count; ++i)
        {
            var length = NativeMethods.wxsharp_dropfiles_path(i, null, 0);
            if (length <= 0) { Files[i] = string.Empty; continue; }
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_dropfiles_path(i, p, buffer.Length);
            Files[i] = Utf8String.Decode(buffer, length);
        }
    }
}

/// <summary>Keyboard navigation between controls - Tab, Shift+Tab, and the window-change variants.</summary>
public sealed class NavigationKeyEventArgs : WxEventArgs
{
    /// <summary>True for Tab, false for Shift+Tab.</summary>
    public bool Forward { get; }

    /// <summary>True when this is Ctrl+Tab, which moves between panes rather than between controls.</summary>
    public bool IsWindowChange { get; }

    internal NavigationKeyEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Forward = e.Active != 0;
        IsWindowChange = e.IntValue != 0;
    }
}

/// <summary>Context help was requested - by the help key, or the title bar's question mark.</summary>
public sealed class HelpEventArgs : WxEventArgs
{
    /// <summary>Where help was asked for, in screen coordinates.</summary>
    public Point ScreenPosition { get; }
    internal HelpEventArgs(Window source, in NativeEvent e) : base(source, e)
        => ScreenPosition = new Point(e.X, e.Y);
}

/// <summary>A URL was clicked in a rich text control.</summary>
public sealed class TextUrlEventArgs : WxEventArgs
{
    /// <summary>The first character of the URL in the control's text.</summary>
    public int Start { get; }

    /// <summary>One past the last character of the URL.</summary>
    public int End { get; }

    internal TextUrlEventArgs(Window source, in NativeEvent e) : base(source, e)
    {
        Start = e.Selection;
        End = e.OldSelection;
    }
}

// ---- The event catalogue ----------------------------------------------------------------------------------

/// <summary>Every event a window can be bound to, following Phoenix's <c>wx.EVT_*</c> naming. Pass one to
/// <see cref="Window.Bind{T}"/>; the typed <c>event</c> members on each control are shorthand for the same
/// thing. Binding a command event on a parent works, because wxWidgets propagates it up the real parent
/// chain exactly as it does in Phoenix.</summary>
public static class WxEvents
{
    private static EventType<T> Make<T>(int id, EventArgsFactory factory) where T : WxEventArgs => new(id, factory);

    private static WxEventArgs Command(Window w, in NativeEvent e) => new CommandEventArgs(w, e);
    private static WxEventArgs Key(Window w, in NativeEvent e) => new KeyEventArgs(w, e);
    private static WxEventArgs Mouse(Window w, in NativeEvent e) => new MouseEventArgs(w, e);
    private static WxEventArgs Book(Window w, in NativeEvent e) => new BookEventArgs(w, e);
    private static WxEventArgs List(Window w, in NativeEvent e) => new ListEventArgs(w, e);
    private static WxEventArgs Tree(Window w, in NativeEvent e) => new TreeEventArgs(w, e);
    private static WxEventArgs DataView(Window w, in NativeEvent e) => new DataViewEventArgs(w, e);
    private static WxEventArgs Spin_(Window w, in NativeEvent e) => new SpinEventArgs(w, e);
    private static WxEventArgs Scroll(Window w, in NativeEvent e) => new ScrollEventArgs(w, e);
    private static WxEventArgs Splitter(Window w, in NativeEvent e) => new SplitterEventArgs(w, e);
    private static WxEventArgs Grid(Window w, in NativeEvent e) => new GridEventArgs(w, e);
    private static WxEventArgs Date(Window w, in NativeEvent e) => new DateEventArgs(w, e);
    private static WxEventArgs Link(Window w, in NativeEvent e) => new HyperlinkEventArgs(w, e);
    private static WxEventArgs Plain(Window w, in NativeEvent e) => new WxEventArgs(w, e);
    private static WxEventArgs Close(Window w, in NativeEvent e) => new CloseEventArgs(w, e);
    private static WxEventArgs Show(Window w, in NativeEvent e) => new ShowEventArgs(w, e);
    private static WxEventArgs Activate(Window w, in NativeEvent e) => new ActivateEventArgs(w, e);
    private static WxEventArgs Resize(Window w, in NativeEvent e) => new SizeEventArgs(w, e);
    private static WxEventArgs Move(Window w, in NativeEvent e) => new MoveEventArgs(w, e);
    private static WxEventArgs Repaint(Window w, in NativeEvent e) => new PaintEventArgs(w, e);
    private static WxEventArgs Context(Window w, in NativeEvent e) => new ContextMenuEventArgs(w, e);
    private static WxEventArgs UpdateUIArgs(Window w, in NativeEvent e) => new UpdateUIEventArgs(w, e);
    private static WxEventArgs IdleArgs(Window w, in NativeEvent e) => new IdleEventArgs(w, e);
    private static WxEventArgs MenuArgs(Window w, in NativeEvent e) => new MenuEventArgs(w, e);
    private static WxEventArgs DropFilesArgs(Window w, in NativeEvent e) => new DropFilesEventArgs(w, e);
    private static WxEventArgs NavigationArgs(Window w, in NativeEvent e) => new NavigationKeyEventArgs(w, e);
    private static WxEventArgs HelpArgs(Window w, in NativeEvent e) => new HelpEventArgs(w, e);
    private static WxEventArgs TextUrlArgs(Window w, in NativeEvent e) => new TextUrlEventArgs(w, e);

    // Window lifecycle and geometry.
    public static EventType<CloseEventArgs> Closing { get; } = Make<CloseEventArgs>(EventId.Close, Close);
    public static EventType<ShowEventArgs> Shown { get; } = Make<ShowEventArgs>(EventId.Show, Show);
    public static EventType<ActivateEventArgs> Activated { get; } = Make<ActivateEventArgs>(EventId.Activate, Activate);
    public static EventType<SizeEventArgs> SizeChanged { get; } = Make<SizeEventArgs>(EventId.Size, Resize);
    public static EventType<MoveEventArgs> Moved { get; } = Make<MoveEventArgs>(EventId.Move, Move);
    public static EventType<WxEventArgs> Maximized { get; } = Make<WxEventArgs>(EventId.Maximize, Plain);
    public static EventType<ActivateEventArgs> Iconized { get; } = Make<ActivateEventArgs>(EventId.Iconize, Activate);
    public static EventType<WxEventArgs> Destroyed { get; } = Make<WxEventArgs>(EventId.Destroy, Plain);
    public static EventType<WxEventArgs> GotFocus { get; } = Make<WxEventArgs>(EventId.SetFocus, Plain);
    public static EventType<WxEventArgs> LostFocus { get; } = Make<WxEventArgs>(EventId.KillFocus, Plain);
    public static EventType<PaintEventArgs> Paint { get; } = Make<PaintEventArgs>(EventId.Paint, Repaint);
    public static EventType<ContextMenuEventArgs> ContextMenu { get; } = Make<ContextMenuEventArgs>(EventId.ContextMenu, Context);

    /// <summary>Asks what state a command should be in. Bind it with the command's ID and answer from the
    /// application's own state; wxWidgets applies the answer everywhere that command appears.</summary>
    public static EventType<UpdateUIEventArgs> UpdateUI { get; } = Make<UpdateUIEventArgs>(EventId.UpdateUI, UpdateUIArgs);
    public static EventType<IdleEventArgs> Idle { get; } = Make<IdleEventArgs>(EventId.Idle, IdleArgs);
    public static EventType<WxEventArgs> ChildFocus { get; } = Make<WxEventArgs>(EventId.ChildFocus, Plain);
    public static EventType<NavigationKeyEventArgs> NavigationKey { get; } = Make<NavigationKeyEventArgs>(EventId.NavigationKey, NavigationArgs);
    /// <summary>The mouse capture was taken away. Any window that calls <see cref="Window.CaptureMouse"/>
    /// must handle this; wxWidgets asserts if it does not.</summary>
    public static EventType<WxEventArgs> MouseCaptureLost { get; } = Make<WxEventArgs>(EventId.MouseCaptureLost, Plain);
    public static EventType<WxEventArgs> MouseCaptureChanged { get; } = Make<WxEventArgs>(EventId.MouseCaptureChanged, Plain);
    public static EventType<DropFilesEventArgs> DropFiles { get; } = Make<DropFilesEventArgs>(EventId.DropFiles, DropFilesArgs);
    public static EventType<KeyEventArgs> HotKey { get; } = Make<KeyEventArgs>(EventId.HotKey, Key);
    public static EventType<HelpEventArgs> Help { get; } = Make<HelpEventArgs>(EventId.Help, HelpArgs);
    /// <summary>A menu is about to open. The moment to rebuild a dynamic menu, before the user sees it.</summary>
    public static EventType<MenuEventArgs> MenuOpened { get; } = Make<MenuEventArgs>(EventId.MenuOpen, MenuArgs);
    public static EventType<MenuEventArgs> MenuClosed { get; } = Make<MenuEventArgs>(EventId.MenuClose, MenuArgs);
    /// <summary>An item is highlighted as the user moves through a menu. Paired with the item's help string,
    /// this is what puts a description in the status bar.</summary>
    public static EventType<MenuEventArgs> MenuHighlighted { get; } = Make<MenuEventArgs>(EventId.MenuHighlight, MenuArgs);

    // Mouse.
    public static EventType<MouseEventArgs> MouseDown { get; } = Make<MouseEventArgs>(EventId.LeftDown, Mouse);
    public static EventType<MouseEventArgs> MouseUp { get; } = Make<MouseEventArgs>(EventId.LeftUp, Mouse);
    public static EventType<MouseEventArgs> DoubleClicked { get; } = Make<MouseEventArgs>(EventId.LeftDoubleClick, Mouse);
    public static EventType<MouseEventArgs> RightDown { get; } = Make<MouseEventArgs>(EventId.RightDown, Mouse);
    public static EventType<MouseEventArgs> RightUp { get; } = Make<MouseEventArgs>(EventId.RightUp, Mouse);
    public static EventType<MouseEventArgs> RightDoubleClicked { get; } = Make<MouseEventArgs>(EventId.RightDoubleClick, Mouse);
    public static EventType<MouseEventArgs> MiddleDown { get; } = Make<MouseEventArgs>(EventId.MiddleDown, Mouse);
    public static EventType<MouseEventArgs> MiddleUp { get; } = Make<MouseEventArgs>(EventId.MiddleUp, Mouse);
    public static EventType<MouseEventArgs> MiddleDoubleClicked { get; } = Make<MouseEventArgs>(EventId.MiddleDoubleClick, Mouse);
    public static EventType<MouseEventArgs> MouseMoved { get; } = Make<MouseEventArgs>(EventId.Motion, Mouse);
    public static EventType<MouseEventArgs> MouseEntered { get; } = Make<MouseEventArgs>(EventId.EnterWindow, Mouse);
    public static EventType<MouseEventArgs> MouseLeft { get; } = Make<MouseEventArgs>(EventId.LeaveWindow, Mouse);
    public static EventType<MouseEventArgs> MouseWheel { get; } = Make<MouseEventArgs>(EventId.MouseWheel, Mouse);

    // Keyboard. CharHook reaches a top-level window before the focused control sees the key, which is where
    // application-wide shortcuts belong; Char reports the character a key produces after translation.
    public static EventType<KeyEventArgs> CharHook { get; } = Make<KeyEventArgs>(EventId.CharHook, Key);
    public static EventType<KeyEventArgs> KeyDown { get; } = Make<KeyEventArgs>(EventId.KeyDown, Key);
    public static EventType<KeyEventArgs> KeyUp { get; } = Make<KeyEventArgs>(EventId.KeyUp, Key);
    public static EventType<KeyEventArgs> Char { get; } = Make<KeyEventArgs>(EventId.Char, Key);

    // Control commands.
    public static EventType<CommandEventArgs> ButtonClicked { get; } = Make<CommandEventArgs>(EventId.Button, Command);
    public static EventType<CommandEventArgs> CheckBoxToggled { get; } = Make<CommandEventArgs>(EventId.CheckBox, Command);
    public static EventType<CommandEventArgs> ChoiceSelected { get; } = Make<CommandEventArgs>(EventId.Choice, Command);
    public static EventType<CommandEventArgs> ListBoxSelected { get; } = Make<CommandEventArgs>(EventId.ListBox, Command);
    public static EventType<CommandEventArgs> ListBoxDoubleClicked { get; } = Make<CommandEventArgs>(EventId.ListBoxDoubleClick, Command);
    public static EventType<CommandEventArgs> TextChanged { get; } = Make<CommandEventArgs>(EventId.Text, Command);
    public static EventType<CommandEventArgs> TextEntered { get; } = Make<CommandEventArgs>(EventId.TextEnter, Command);
    public static EventType<CommandEventArgs> MenuCommand { get; } = Make<CommandEventArgs>(EventId.Menu, Command);
    public static EventType<CommandEventArgs> SliderChanged { get; } = Make<CommandEventArgs>(EventId.Slider, Command);
    public static EventType<CommandEventArgs> RadioButtonSelected { get; } = Make<CommandEventArgs>(EventId.RadioButton, Command);
    public static EventType<CommandEventArgs> RadioBoxSelected { get; } = Make<CommandEventArgs>(EventId.RadioBox, Command);
    public static EventType<CommandEventArgs> ComboBoxSelected { get; } = Make<CommandEventArgs>(EventId.ComboBox, Command);
    public static EventType<CommandEventArgs> ComboBoxDropDown { get; } = Make<CommandEventArgs>(EventId.ComboBoxDropDown, Command);
    public static EventType<CommandEventArgs> ComboBoxCloseUp { get; } = Make<CommandEventArgs>(EventId.ComboBoxCloseUp, Command);
    public static EventType<CommandEventArgs> ToggleButtonToggled { get; } = Make<CommandEventArgs>(EventId.ToggleButton, Command);
    public static EventType<CommandEventArgs> CheckListBoxToggled { get; } = Make<CommandEventArgs>(EventId.CheckListBox, Command);
    public static EventType<SpinEventArgs> SpinChanged { get; } = Make<SpinEventArgs>(EventId.SpinCtrl, Spin_);
    public static EventType<SpinEventArgs> SpinDoubleChanged { get; } = Make<SpinEventArgs>(EventId.SpinCtrlDouble, Spin_);
    public static EventType<ScrollEventArgs> ScrollThumbTrack { get; } = Make<ScrollEventArgs>(EventId.ScrollThumbTrack, Scroll);
    public static EventType<ScrollEventArgs> ScrollChanged { get; } = Make<ScrollEventArgs>(EventId.ScrollChanged, Scroll);
    public static EventType<HyperlinkEventArgs> HyperlinkClicked { get; } = Make<HyperlinkEventArgs>(EventId.Hyperlink, Link);
    public static EventType<CommandEventArgs> Search { get; } = Make<CommandEventArgs>(EventId.Search, Command);
    public static EventType<CommandEventArgs> SearchCancelled { get; } = Make<CommandEventArgs>(EventId.SearchCancel, Command);
    public static EventType<DateEventArgs> DateChanged { get; } = Make<DateEventArgs>(EventId.DateChanged, Date);
    public static EventType<DateEventArgs> TimeChanged { get; } = Make<DateEventArgs>(EventId.TimeChanged, Date);
    public static EventType<CommandEventArgs> Timer { get; } = Make<CommandEventArgs>(EventId.Timer, Command);
    public static EventType<SpinEventArgs> Spin { get; } = Make<SpinEventArgs>(EventId.Spin, Spin_);
    public static EventType<SpinEventArgs> SpinUp { get; } = Make<SpinEventArgs>(EventId.SpinUp, Spin_);
    public static EventType<SpinEventArgs> SpinDown { get; } = Make<SpinEventArgs>(EventId.SpinDown, Spin_);
    public static EventType<ScrollEventArgs> ScrollBarChanged { get; } = Make<ScrollEventArgs>(EventId.ScrollBar, Scroll);
    public static EventType<ScrollEventArgs> ScrollToTop { get; } = Make<ScrollEventArgs>(EventId.ScrollTop, Scroll);
    public static EventType<ScrollEventArgs> ScrollToBottom { get; } = Make<ScrollEventArgs>(EventId.ScrollBottom, Scroll);
    public static EventType<ScrollEventArgs> ScrollLineUp { get; } = Make<ScrollEventArgs>(EventId.ScrollLineUp, Scroll);
    public static EventType<ScrollEventArgs> ScrollLineDown { get; } = Make<ScrollEventArgs>(EventId.ScrollLineDown, Scroll);
    public static EventType<ScrollEventArgs> ScrollPageUp { get; } = Make<ScrollEventArgs>(EventId.ScrollPageUp, Scroll);
    public static EventType<ScrollEventArgs> ScrollPageDown { get; } = Make<ScrollEventArgs>(EventId.ScrollPageDown, Scroll);
    public static EventType<ScrollEventArgs> ScrollThumbReleased { get; } = Make<ScrollEventArgs>(EventId.ScrollThumbRelease, Scroll);
    public static EventType<CommandEventArgs> TextMaxLengthReached { get; } = Make<CommandEventArgs>(EventId.TextMaxLength, Command);
    public static EventType<TextUrlEventArgs> TextUrlClicked { get; } = Make<TextUrlEventArgs>(EventId.TextUrl, TextUrlArgs);
    public static EventType<CommandEventArgs> TextCopy { get; } = Make<CommandEventArgs>(EventId.TextCopy, Command);
    public static EventType<CommandEventArgs> TextCut { get; } = Make<CommandEventArgs>(EventId.TextCut, Command);
    public static EventType<CommandEventArgs> TextPaste { get; } = Make<CommandEventArgs>(EventId.TextPaste, Command);
    public static EventType<CommandEventArgs> ToolEntered { get; } = Make<CommandEventArgs>(EventId.ToolEnter, Command);
    public static EventType<CommandEventArgs> ToolRightClicked { get; } = Make<CommandEventArgs>(EventId.ToolRightClick, Command);
    public static EventType<CommandEventArgs> ToolDropDown { get; } = Make<CommandEventArgs>(EventId.ToolDropDown, Command);

    // Book controls.
    public static EventType<BookEventArgs> NotebookPageChanged { get; } = Make<BookEventArgs>(EventId.NotebookPageChanged, Book);
    public static EventType<BookEventArgs> NotebookPageChanging { get; } = Make<BookEventArgs>(EventId.NotebookPageChanging, Book);
    public static EventType<BookEventArgs> BookPageChanged { get; } = Make<BookEventArgs>(EventId.BookPageChanged, Book);
    public static EventType<BookEventArgs> BookPageChanging { get; } = Make<BookEventArgs>(EventId.BookPageChanging, Book);

    // wxListCtrl.
    public static EventType<ListEventArgs> ListItemSelected { get; } = Make<ListEventArgs>(EventId.ListItemSelected, List);
    public static EventType<ListEventArgs> ListItemDeselected { get; } = Make<ListEventArgs>(EventId.ListItemDeselected, List);
    public static EventType<ListEventArgs> ListItemActivated { get; } = Make<ListEventArgs>(EventId.ListItemActivated, List);
    public static EventType<ListEventArgs> ListItemFocused { get; } = Make<ListEventArgs>(EventId.ListItemFocused, List);
    public static EventType<ListEventArgs> ListItemRightClicked { get; } = Make<ListEventArgs>(EventId.ListItemRightClick, List);
    public static EventType<ListEventArgs> ListColumnClicked { get; } = Make<ListEventArgs>(EventId.ListColumnClick, List);
    public static EventType<ListEventArgs> ListKeyDown { get; } = Make<ListEventArgs>(EventId.ListKeyDown, List);
    public static EventType<ListEventArgs> ListBeginLabelEdit { get; } = Make<ListEventArgs>(EventId.ListBeginLabelEdit, List);
    public static EventType<ListEventArgs> ListEndLabelEdit { get; } = Make<ListEventArgs>(EventId.ListEndLabelEdit, List);
    public static EventType<ListEventArgs> ListBeginDrag { get; } = Make<ListEventArgs>(EventId.ListBeginDrag, List);
    public static EventType<ListEventArgs> ListBeginRightDrag { get; } = Make<ListEventArgs>(EventId.ListBeginRightDrag, List);
    public static EventType<ListEventArgs> ListItemMiddleClicked { get; } = Make<ListEventArgs>(EventId.ListItemMiddleClick, List);
    public static EventType<ListEventArgs> ListItemChecked { get; } = Make<ListEventArgs>(EventId.ListItemChecked, List);
    public static EventType<ListEventArgs> ListItemUnchecked { get; } = Make<ListEventArgs>(EventId.ListItemUnchecked, List);
    public static EventType<ListEventArgs> ListColumnRightClicked { get; } = Make<ListEventArgs>(EventId.ListColumnRightClick, List);
    public static EventType<ListEventArgs> ListColumnBeginDrag { get; } = Make<ListEventArgs>(EventId.ListColumnBeginDrag, List);
    public static EventType<ListEventArgs> ListColumnEndDrag { get; } = Make<ListEventArgs>(EventId.ListColumnEndDrag, List);
    public static EventType<ListEventArgs> ListItemDeleted { get; } = Make<ListEventArgs>(EventId.ListDeleteItem, List);
    public static EventType<ListEventArgs> ListAllItemsDeleted { get; } = Make<ListEventArgs>(EventId.ListDeleteAllItems, List);
    public static EventType<ListEventArgs> ListItemInserted { get; } = Make<ListEventArgs>(EventId.ListInsertItem, List);
    /// <summary>A virtual list is about to draw a range of rows and is asking for them to be prepared.</summary>
    public static EventType<ListEventArgs> ListCacheHint { get; } = Make<ListEventArgs>(EventId.ListCacheHint, List);

    // wxTreeCtrl.
    public static EventType<TreeEventArgs> TreeSelectionChanged { get; } = Make<TreeEventArgs>(EventId.TreeSelectionChanged, Tree);
    public static EventType<TreeEventArgs> TreeSelectionChanging { get; } = Make<TreeEventArgs>(EventId.TreeSelectionChanging, Tree);
    public static EventType<TreeEventArgs> TreeItemActivated { get; } = Make<TreeEventArgs>(EventId.TreeItemActivated, Tree);
    public static EventType<TreeEventArgs> TreeItemExpanded { get; } = Make<TreeEventArgs>(EventId.TreeItemExpanded, Tree);
    public static EventType<TreeEventArgs> TreeItemExpanding { get; } = Make<TreeEventArgs>(EventId.TreeItemExpanding, Tree);
    public static EventType<TreeEventArgs> TreeItemCollapsed { get; } = Make<TreeEventArgs>(EventId.TreeItemCollapsed, Tree);
    public static EventType<TreeEventArgs> TreeItemCollapsing { get; } = Make<TreeEventArgs>(EventId.TreeItemCollapsing, Tree);
    public static EventType<TreeEventArgs> TreeItemRightClicked { get; } = Make<TreeEventArgs>(EventId.TreeItemRightClick, Tree);
    public static EventType<TreeEventArgs> TreeKeyDown { get; } = Make<TreeEventArgs>(EventId.TreeKeyDown, Tree);
    public static EventType<TreeEventArgs> TreeBeginLabelEdit { get; } = Make<TreeEventArgs>(EventId.TreeBeginLabelEdit, Tree);
    public static EventType<TreeEventArgs> TreeEndLabelEdit { get; } = Make<TreeEventArgs>(EventId.TreeEndLabelEdit, Tree);
    /// <summary>A context menu was asked for on a tree item, by right-click or by the keyboard's menu key.</summary>
    public static EventType<TreeEventArgs> TreeItemMenu { get; } = Make<TreeEventArgs>(EventId.TreeItemMenu, Tree);
    public static EventType<TreeEventArgs> TreeBeginDrag { get; } = Make<TreeEventArgs>(EventId.TreeBeginDrag, Tree);
    public static EventType<TreeEventArgs> TreeEndDrag { get; } = Make<TreeEventArgs>(EventId.TreeEndDrag, Tree);
    public static EventType<TreeEventArgs> TreeItemMiddleClicked { get; } = Make<TreeEventArgs>(EventId.TreeItemMiddleClick, Tree);
    public static EventType<TreeEventArgs> TreeItemDeleted { get; } = Make<TreeEventArgs>(EventId.TreeDeleteItem, Tree);
    public static EventType<TreeEventArgs> TreeItemToolTip { get; } = Make<TreeEventArgs>(EventId.TreeItemToolTip, Tree);
    public static EventType<TreeEventArgs> TreeStateImageClicked { get; } = Make<TreeEventArgs>(EventId.TreeStateImageClick, Tree);

    // wxDataViewCtrl.
    public static EventType<DataViewEventArgs> DataViewSelectionChanged { get; } = Make<DataViewEventArgs>(EventId.DataViewSelectionChanged, DataView);
    public static EventType<DataViewEventArgs> DataViewItemActivated { get; } = Make<DataViewEventArgs>(EventId.DataViewItemActivated, DataView);
    public static EventType<DataViewEventArgs> DataViewItemContextMenu { get; } = Make<DataViewEventArgs>(EventId.DataViewItemContextMenu, DataView);
    public static EventType<DataViewEventArgs> DataViewItemExpanded { get; } = Make<DataViewEventArgs>(EventId.DataViewItemExpanded, DataView);
    public static EventType<DataViewEventArgs> DataViewItemExpanding { get; } = Make<DataViewEventArgs>(EventId.DataViewItemExpanding, DataView);
    public static EventType<DataViewEventArgs> DataViewItemCollapsed { get; } = Make<DataViewEventArgs>(EventId.DataViewItemCollapsed, DataView);
    public static EventType<DataViewEventArgs> DataViewItemCollapsing { get; } = Make<DataViewEventArgs>(EventId.DataViewItemCollapsing, DataView);
    public static EventType<DataViewEventArgs> DataViewEditingStarted { get; } = Make<DataViewEventArgs>(EventId.DataViewItemEditingStarted, DataView);
    public static EventType<DataViewEventArgs> DataViewEditingDone { get; } = Make<DataViewEventArgs>(EventId.DataViewItemEditingDone, DataView);
    public static EventType<DataViewEventArgs> DataViewValueChanged { get; } = Make<DataViewEventArgs>(EventId.DataViewItemValueChanged, DataView);
    public static EventType<DataViewEventArgs> DataViewColumnHeaderClicked { get; } = Make<DataViewEventArgs>(EventId.DataViewColumnHeaderClick, DataView);
    public static EventType<DataViewEventArgs> DataViewColumnHeaderRightClicked { get; } = Make<DataViewEventArgs>(EventId.DataViewColumnHeaderRightClick, DataView);
    public static EventType<DataViewEventArgs> DataViewColumnSorted { get; } = Make<DataViewEventArgs>(EventId.DataViewColumnSorted, DataView);
    public static EventType<DataViewEventArgs> DataViewColumnReordered { get; } = Make<DataViewEventArgs>(EventId.DataViewColumnReordered, DataView);

    // wxSplitterWindow and wxGrid.
    public static EventType<SplitterEventArgs> SashPositionChanged { get; } = Make<SplitterEventArgs>(EventId.SplitterSashPositionChanged, Splitter);
    public static EventType<SplitterEventArgs> SashDoubleClicked { get; } = Make<SplitterEventArgs>(EventId.SplitterDoubleClick, Splitter);
    /// <summary>The sash is being dragged. Veto to refuse the new position.</summary>
    public static EventType<SplitterEventArgs> SashPositionChanging { get; } = Make<SplitterEventArgs>(EventId.SplitterSashPositionChanging, Splitter);
    public static EventType<SplitterEventArgs> Unsplit { get; } = Make<SplitterEventArgs>(EventId.SplitterUnsplit, Splitter);
    public static EventType<GridEventArgs> GridCellChanged { get; } = Make<GridEventArgs>(EventId.GridCellChanged, Grid);
    public static EventType<GridEventArgs> GridCellSelected { get; } = Make<GridEventArgs>(EventId.GridSelectCell, Grid);
}
