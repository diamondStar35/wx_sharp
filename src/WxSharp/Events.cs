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
    internal const int ClipboardChanged = 204;

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

/// <summary>A virtual list control asking for the text of one cell.</summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeVirtualListRequest
{
    internal uint Size, Version;
    internal long Token;
    internal long Item;
    internal long OtherItem;
    internal int Column;
    internal byte* Buffer;
    internal int BufferLength;
    internal int RequiredLength;
    internal int Operation;
    internal int Result;
}

/// <summary>Which wxWidgets virtual member is being asked of managed code. The values match the
/// <c>WXSHARP_VIRT_*</c> constants in <c>wxsharp.h</c>.</summary>
internal enum VirtualMember
{
    AcceptsFocus = 1,
    AcceptsFocusFromKeyboard = 2,
    AcceptsFocusRecursively = 3,
    Validate = 4,
    TransferDataToWindow = 5,
    TransferDataFromWindow = 6,
    InitDialog = 7,
    ClientAreaOrigin = 8,
    AddChild = 9,
    RemoveChild = 10,
    InheritAttributes = 11,
    ShouldInheritColours = 12,
    OnInternalIdle = 13,
    MainWindowOfCompositeControl = 14,
    InformFirstDirection = 15,
    SetCanFocus = 16,
    EnableVisibleFocus = 17,
    DoEnable = 18,
    DoGetPosition = 19,
    DoGetSize = 20,
    DoGetClientSize = 21,
    BestSize = 22,
    BestClientSize = 23,
    DoSetSize = 24,
    DoSetClientSize = 25,
    DoSetSizeHints = 26,
    DoMoveWindow = 27,
    DoSetWindowVariant = 28,
    DefaultBorder = 29,
    DoFreeze = 30,
    DoThaw = 31,
    HasTransparentBackground = 32,
    Destroy = 33,

    // Members that exist on one class rather than on wxWindow.
    ShouldPreventAppExit = 34,
    GetContentWindow = 35,
    OnCreateStatusBar = 36,
    OnCreateToolBar = 37,
    DoGiveHelp = 38,
    ShouldScrollToChildOnFocus = 39,
    SizeAvailableForScrollTarget = 40,
    GridColLinePen = 41,
    GridRowLinePen = 42,
    GridDefaultLinePen = 43,

    SetValidator = 44,
    GetValidator = 45,
    ProcessEvent = 46,
    TryBefore = 47,
    TryAfter = 48,
}

/// <summary>One virtual member being asked of a managed subclass, and its answer. Leaving
/// <see cref="Handled"/> clear lets wxWidgets run its own implementation.</summary>
internal unsafe struct NativeVirtualRequest
{
    internal uint Size, Version;
    internal long Token;
    internal long Handle;
    internal int Which;
    internal int Handled;
    internal int Result;
    internal int X;
    internal int Y;
    internal fixed int Args[6];

    // A string argument, owned natively and valid only for the duration of the callback.
    internal byte* Text;

    // A packed 0xAARRGGBB colour, for the members that answer with a pen.
    internal uint UintValue;
}

/// <summary>Builds the arguments for one event kind. Held by <see cref="EventType{TEventArgs}"/> so the
/// managed side needs no reflection or type switch to construct them.</summary>
internal delegate WxEventArgs EventArgsFactory(EvtHandler source, in NativeEvent e);

/// <summary>The modifier keys held down when an input event was raised.</summary>
[Flags]
public enum KeyModifiers
{
    /// <summary>No modifier keys held.</summary>
    None = 0,
    /// <summary>The Control key (Command on macOS). See <see cref="RawControl"/> for the physical key.</summary>
    Control = 1,
    /// <summary>The Shift key.</summary>
    Shift = 2,
    /// <summary>The Alt (Option) key.</summary>
    Alt = 4,
    /// <summary>The Windows key, or Command on macOS.</summary>
    Meta = 8,
    /// <summary>The physical Control key. Identical to <see cref="Control"/> except on macOS, where
    /// <see cref="Control"/> means Command.</summary>
    RawControl = 16,
}

/// <summary>Base class for all WxSharp event arguments, following <c>wxEvent</c>. Carries the source and
/// command ID, and the <see cref="Skip"/> mechanism that decides whether processing continues.</summary>
public class WxEventArgs : EventArgs
{
    /// <summary>What raised the event, following <c>wxEvent.GetEventObject</c>. Usually the window the
    /// event happened on; an application-level event such as <c>wxEVT_ACTIVATE_APP</c> reports the
    /// <see cref="App"/>. Use <see cref="SourceWindow"/> when a window is what you need.</summary>
    public EvtHandler Source { get; }

    /// <summary>The window that raised the event, or null for an application-level event.</summary>
    public Window? SourceWindow => Source as Window;
    /// <summary>The command ID the event carries, following <c>wxEvent.GetId</c>. Identifies which control
    /// or menu item it came from.</summary>
    public int Id { get; }

    /// <summary>Whether this handler asked for normal processing to continue. False unless
    /// <see cref="Skip"/> was called.</summary>
    public bool Skipped { get; private set; }

    internal WxEventArgs(EvtHandler source, int id) { Source = source; Id = id; }
    internal WxEventArgs(EvtHandler source, in NativeEvent e) : this(source, e.Id) { }

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

    internal NotifyEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) { }
}

/// <summary>Identifies one kind of event and knows how to build its arguments. Pass one to
/// <see cref="EvtHandler.Bind{T}"/>; the <see cref="WxEvents"/> catalogue holds the full set.</summary>
public sealed class EventType<TEventArgs> where TEventArgs : WxEventArgs
{
    internal int EventId { get; }
    internal EventArgsFactory Factory { get; }
    internal EventType(int eventId, EventArgsFactory factory) { EventId = eventId; Factory = factory; }
}

/// <summary>A live subscription returned by <see cref="EvtHandler.Bind{T}"/>. Dispose it to unsubscribe;
/// it is also undone automatically when the owning window is destroyed.</summary>
public sealed class EventBinding : IDisposable
{
    private EvtHandler? _handler;
    internal int EventId { get; }
    internal long Token { get; }
    internal EventBinding(EvtHandler handler, int eventId, long token)
    {
        _handler = handler; EventId = eventId; Token = token;
    }
    /// <summary>Removes this subscription so its handler stops being called. Safe to call more than once.</summary>
    public void Dispose()
    {
        var handler = _handler;
        if (handler is null) return;
        _handler = null;
        handler.RemoveBinding(EventId, Token);
    }
}

// ---- Event argument types ---------------------------------------------------------------------------------

/// <summary>Arguments for a command event — button clicks, menu selections, list and text changes, and the
/// rest. Following <c>wxCommandEvent</c>.</summary>
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

    internal CommandEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
    {
        Value = e.IntValue;
        Selection = e.Selection;
        Text = e.GetText();
    }
}

/// <summary>A page change in a notebook or other book control.</summary>
public sealed class BookEventArgs : NotifyEventArgs
{
    /// <summary>The index of the page being switched to.</summary>
    public int Selection { get; }
    /// <summary>The index of the page being switched away from.</summary>
    public int PreviousSelection { get; }
    internal BookEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
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

    internal CloseEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) => CanVeto = e.CanVeto != 0;
}

/// <summary>Arguments for a keyboard event, following <c>wxKeyEvent</c>.</summary>
public sealed class KeyEventArgs : WxEventArgs
{
    /// <summary>The raw wxWidgets key code. Use <see cref="Code"/> for the typed <see cref="Key"/>.</summary>
    public int KeyCode { get; }
    /// <summary>The key as a <see cref="Key"/> value.</summary>
    public Key Code => (Key)KeyCode;
    /// <summary>The modifier keys held down when the key event occurred.</summary>
    public KeyModifiers Modifiers { get; }

    /// <summary>The character the key produces, or <c>'\0'</c> for a key with no character (an arrow, a
    /// function key). Only meaningful on a <c>Char</c> event.</summary>
    public char UnicodeKey { get; }

    /// <summary>The platform's own scan code, for keys wxWidgets does not name.</summary>
    public int RawKeyCode { get; }

    /// <summary>Where the pointer was when the key was pressed, in client coordinates.</summary>
    public Point Position { get; }

    /// <summary>Whether Control (Command on macOS) was held.</summary>
    public bool Control => (Modifiers & KeyModifiers.Control) != 0;
    /// <summary>Whether Shift was held.</summary>
    public bool Shift => (Modifiers & KeyModifiers.Shift) != 0;
    /// <summary>Whether Alt (Option) was held.</summary>
    public bool Alt => (Modifiers & KeyModifiers.Alt) != 0;
    /// <summary>Whether the Meta (Windows/Command) key was held.</summary>
    public bool Meta => (Modifiers & KeyModifiers.Meta) != 0;

    internal KeyEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
    {
        KeyCode = e.KeyCode;
        Modifiers = (KeyModifiers)e.Modifiers;
        UnicodeKey = (char)e.UintValue;
        RawKeyCode = e.IntValue;
        Position = new Point(e.X, e.Y);
    }
}

/// <summary>Which mouse button an event concerns, following the <c>wxMOUSE_BTN_*</c> values.</summary>
public enum MouseButton
{
    /// <summary>No button (a move or wheel event).</summary>
    None = 0,
    /// <summary>The left button.</summary>
    Left = 1,
    /// <summary>The right button.</summary>
    Right = 2,
    /// <summary>The middle button.</summary>
    Middle = 3,
}

/// <summary>Arguments for a mouse event, following <c>wxMouseEvent</c>.</summary>
public sealed class MouseEventArgs : WxEventArgs
{
    /// <summary>The pointer position in client coordinates.</summary>
    public Point Position { get; }
    /// <summary>Which button the event concerns.</summary>
    public MouseButton Button { get; }
    /// <summary>The modifier keys held during the event.</summary>
    public KeyModifiers Modifiers { get; }

    /// <summary>How far the wheel turned. Positive is away from the user.</summary>
    public int WheelRotation { get; }

    /// <summary>The rotation that counts as one notch, for turning <see cref="WheelRotation"/> into lines.</summary>
    public int WheelDelta { get; }

    /// <summary>Whether Control (Command on macOS) was held.</summary>
    public bool Control => (Modifiers & KeyModifiers.Control) != 0;
    /// <summary>Whether Shift was held.</summary>
    public bool Shift => (Modifiers & KeyModifiers.Shift) != 0;
    /// <summary>Whether Alt (Option) was held.</summary>
    public bool Alt => (Modifiers & KeyModifiers.Alt) != 0;

    internal MouseEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
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

    internal ContextMenuEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
    {
        ScreenPosition = new Point(e.X, e.Y);
        FromKeyboard = e.X < 0 && e.Y < 0;
    }
}

/// <summary>Arguments for a resize event, following <c>wxSizeEvent</c>.</summary>
public sealed class SizeEventArgs : WxEventArgs
{
    /// <summary>The window's new size.</summary>
    public Size Size { get; }
    internal SizeEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) => Size = new Size(e.Width, e.Height);
}

/// <summary>Arguments for a move event, following <c>wxMoveEvent</c>.</summary>
public sealed class MoveEventArgs : WxEventArgs
{
    /// <summary>The window's new top-left position.</summary>
    public Point Position { get; }
    internal MoveEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) => Position = new Point(e.X, e.Y);
}

/// <summary>Arguments for an activation event, following <c>wxActivateEvent</c>.</summary>
public sealed class ActivateEventArgs : WxEventArgs
{
    /// <summary>True when the window is being activated, false when deactivated.</summary>
    public bool Active { get; }
    internal ActivateEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) => Active = e.Active != 0;
}

/// <summary>Arguments for a show/hide event, following <c>wxShowEvent</c>.</summary>
public sealed class ShowEventArgs : WxEventArgs
{
    /// <summary>True when the window is being shown, false when hidden.</summary>
    public bool Shown { get; }
    internal ShowEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) => Shown = e.Active != 0;
}

/// <summary>Arguments for a paint event, following <c>wxPaintEvent</c>. Issue drawing calls from the
/// handler.</summary>
public sealed class PaintEventArgs : WxEventArgs
{
    internal PaintEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) { }
}

/// <summary>An event from a <see cref="ListCtrl"/>.</summary>
public sealed class ListEventArgs : NotifyEventArgs
{
    /// <summary>The zero-based index of the item the event concerns.</summary>
    public long Index { get; }
    /// <summary>The column the event concerns, for a report-view list.</summary>
    public int Column { get; }

    /// <summary>The item's label, for the label-editing events.</summary>
    public string Label { get; }

    /// <summary>The key pressed, for <see cref="WxEvents.ListKeyDown"/>.</summary>
    public Key Code { get; }

    internal ListEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
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
    /// <summary>The tree item the event concerns.</summary>
    public TreeItemId Item { get; }

    /// <summary>The previously selected item, for the selection-changing events.</summary>
    public TreeItemId PreviousItem { get; }

    /// <summary>The item's label, for the label-editing events.</summary>
    public string Label { get; }
    /// <summary>The key pressed, for <see cref="WxEvents.TreeKeyDown"/>.</summary>
    public Key Code { get; }

    internal TreeEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
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
    /// <summary>The data-view item the event concerns.</summary>
    public DataViewItem Item { get; }
    /// <summary>The column the event concerns, or null-equivalent for a whole-row event.</summary>
    public int Column { get; }
    internal DataViewEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
    {
        Item = new DataViewItem(e.Item);
        Column = e.Column;
    }
}

/// <summary>A value change from a spin control or a scrollbar.</summary>
public sealed class SpinEventArgs : WxEventArgs
{
    /// <summary>The control's new integer value.</summary>
    public int Value { get; }
    /// <summary>The control's new value as a double, for a <c>wxSpinCtrlDouble</c>.</summary>
    public double DoubleValue { get; }
    internal SpinEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
    {
        Value = e.IntValue;
        DoubleValue = e.DoubleValue;
    }
}

/// <summary>Arguments for a scrollbar or scroll-event, following <c>wxScrollEvent</c>.</summary>
public sealed class ScrollEventArgs : WxEventArgs
{
    /// <summary>The scroll position the event reports.</summary>
    public int Position { get; }
    internal ScrollEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) => Position = e.IntValue;
}

/// <summary>Arguments for a splitter-window event, following <c>wxSplitterEvent</c>. Veto to refuse the
/// change.</summary>
public sealed class SplitterEventArgs : NotifyEventArgs
{
    /// <summary>The sash position the event reports, in pixels.</summary>
    public int SashPosition { get; }
    internal SplitterEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) => SashPosition = e.IntValue;
}

/// <summary>Arguments for a grid event, following <c>wxGridEvent</c>.</summary>
public sealed class GridEventArgs : WxEventArgs
{
    /// <summary>The row the event concerns.</summary>
    public int Row { get; }
    /// <summary>The column the event concerns.</summary>
    public int Column { get; }
    internal GridEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
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
    internal DateEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
        => Date = e.Active != 0 ? DateTimeOffset.FromUnixTimeMilliseconds(e.Item).UtcDateTime : null;
}

/// <summary>Arguments for a hyperlink-clicked event, following <c>wxHyperlinkEvent</c>.</summary>
public sealed class HyperlinkEventArgs : WxEventArgs
{
    /// <summary>The URL that was clicked.</summary>
    public string Url { get; }
    internal HyperlinkEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) => Url = e.GetText();
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
    internal UpdateUIEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
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

    /// <summary>Enables or disables the command. Shorthand for setting <see cref="Enabled"/>.</summary>
    public void Enable(bool enable = true) { _enabled = enable; NativeMethods.wxsharp_updateui_enable(enable); }
    /// <summary>Ticks or unticks a check/radio command. Shorthand for setting <see cref="Checked"/>.</summary>
    public void Check(bool check = true) { _checked = check; NativeMethods.wxsharp_updateui_check(check); }
    /// <summary>Shows or hides the command. Shorthand for setting <see cref="Shown"/>.</summary>
    public void Show(bool show = true) { _shown = show; NativeMethods.wxsharp_updateui_show(show); }
    /// <summary>Sets the command's label. Shorthand for setting <see cref="Text"/>.</summary>
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
    internal IdleEventArgs(EvtHandler source, in NativeEvent e) : base(source, e) => MoreRequested = e.Active != 0;
}

/// <summary>A menu is opening, closing, or an item in it is highlighted.</summary>
public sealed class MenuEventArgs : WxEventArgs
{
    /// <summary>The highlighted item's command ID, for the highlight event. -1 for open and close.</summary>
    public int MenuId { get; }

    /// <summary>True when this is a context menu rather than one on the menu bar.</summary>
    public bool IsPopup { get; }

    internal MenuEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
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

    internal unsafe DropFilesEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
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

    internal NavigationKeyEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
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
    internal HelpEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
        => ScreenPosition = new Point(e.X, e.Y);
}

/// <summary>A URL was clicked in a rich text control.</summary>
public sealed class TextUrlEventArgs : WxEventArgs
{
    /// <summary>The first character of the URL in the control's text.</summary>
    public int Start { get; }

    /// <summary>One past the last character of the URL.</summary>
    public int End { get; }

    internal TextUrlEventArgs(EvtHandler source, in NativeEvent e) : base(source, e)
    {
        Start = e.Selection;
        End = e.OldSelection;
    }
}

// ---- The event catalogue ----------------------------------------------------------------------------------

/// <summary>Every event a window can be bound to, following Phoenix's <c>wx.EVT_*</c> naming. Pass one to
/// <see cref="EvtHandler.Bind{T}"/>; the typed <c>event</c> members on each control are shorthand for the same
/// thing. Binding a command event on a parent works, because wxWidgets propagates it up the real parent
/// chain exactly as it does in Phoenix.</summary>
public static class WxEvents
{
    private static EventType<T> Make<T>(int id, EventArgsFactory factory) where T : WxEventArgs => new(id, factory);

    private static WxEventArgs Command(EvtHandler w, in NativeEvent e) => new CommandEventArgs(w, e);
    private static WxEventArgs Key(EvtHandler w, in NativeEvent e) => new KeyEventArgs(w, e);
    private static WxEventArgs Mouse(EvtHandler w, in NativeEvent e) => new MouseEventArgs(w, e);
    private static WxEventArgs Book(EvtHandler w, in NativeEvent e) => new BookEventArgs(w, e);
    private static WxEventArgs List(EvtHandler w, in NativeEvent e) => new ListEventArgs(w, e);
    private static WxEventArgs Tree(EvtHandler w, in NativeEvent e) => new TreeEventArgs(w, e);
    private static WxEventArgs DataView(EvtHandler w, in NativeEvent e) => new DataViewEventArgs(w, e);
    private static WxEventArgs Spin_(EvtHandler w, in NativeEvent e) => new SpinEventArgs(w, e);
    private static WxEventArgs Scroll(EvtHandler w, in NativeEvent e) => new ScrollEventArgs(w, e);
    private static WxEventArgs Splitter(EvtHandler w, in NativeEvent e) => new SplitterEventArgs(w, e);
    private static WxEventArgs Grid(EvtHandler w, in NativeEvent e) => new GridEventArgs(w, e);
    private static WxEventArgs Date(EvtHandler w, in NativeEvent e) => new DateEventArgs(w, e);
    private static WxEventArgs Link(EvtHandler w, in NativeEvent e) => new HyperlinkEventArgs(w, e);
    private static WxEventArgs Plain(EvtHandler w, in NativeEvent e) => new WxEventArgs(w, e);
    private static WxEventArgs Close(EvtHandler w, in NativeEvent e) => new CloseEventArgs(w, e);
    private static WxEventArgs Show(EvtHandler w, in NativeEvent e) => new ShowEventArgs(w, e);
    private static WxEventArgs Activate(EvtHandler w, in NativeEvent e) => new ActivateEventArgs(w, e);
    private static WxEventArgs Resize(EvtHandler w, in NativeEvent e) => new SizeEventArgs(w, e);
    private static WxEventArgs Move(EvtHandler w, in NativeEvent e) => new MoveEventArgs(w, e);
    private static WxEventArgs Repaint(EvtHandler w, in NativeEvent e) => new PaintEventArgs(w, e);
    private static WxEventArgs Context(EvtHandler w, in NativeEvent e) => new ContextMenuEventArgs(w, e);
    private static WxEventArgs UpdateUIArgs(EvtHandler w, in NativeEvent e) => new UpdateUIEventArgs(w, e);
    private static WxEventArgs IdleArgs(EvtHandler w, in NativeEvent e) => new IdleEventArgs(w, e);
    private static WxEventArgs MenuArgs(EvtHandler w, in NativeEvent e) => new MenuEventArgs(w, e);
    private static WxEventArgs DropFilesArgs(EvtHandler w, in NativeEvent e) => new DropFilesEventArgs(w, e);
    private static WxEventArgs NavigationArgs(EvtHandler w, in NativeEvent e) => new NavigationKeyEventArgs(w, e);
    private static WxEventArgs HelpArgs(EvtHandler w, in NativeEvent e) => new HelpEventArgs(w, e);
    private static WxEventArgs TextUrlArgs(EvtHandler w, in NativeEvent e) => new TextUrlEventArgs(w, e);

    // Window lifecycle and geometry.
    /// <summary>Fires when a top-level window is asked to close (<c>wxEVT_CLOSE_WINDOW</c>). Veto on the
    /// args to keep it open.</summary>
    public static EventType<CloseEventArgs> Closing { get; } = Make<CloseEventArgs>(EventId.Close, Close);
    /// <summary>Fires when the window is shown or hidden (<c>wxEVT_SHOW</c>).</summary>
    public static EventType<ShowEventArgs> Shown { get; } = Make<ShowEventArgs>(EventId.Show, Show);
    /// <summary>Fires when the window is activated or deactivated (<c>wxEVT_ACTIVATE</c>).</summary>
    public static EventType<ActivateEventArgs> Activated { get; } = Make<ActivateEventArgs>(EventId.Activate, Activate);
    /// <summary>Fires when the window is resized (<c>wxEVT_SIZE</c>). Skip it to let layout run.</summary>
    public static EventType<SizeEventArgs> SizeChanged { get; } = Make<SizeEventArgs>(EventId.Size, Resize);
    /// <summary>Fires when the window moves (<c>wxEVT_MOVE</c>).</summary>
    public static EventType<MoveEventArgs> Moved { get; } = Make<MoveEventArgs>(EventId.Move, Move);
    /// <summary>Fires when the window is maximized (<c>wxEVT_MAXIMIZE</c>).</summary>
    public static EventType<WxEventArgs> Maximized { get; } = Make<WxEventArgs>(EventId.Maximize, Plain);
    /// <summary>Fires when the window is minimized or restored (<c>wxEVT_ICONIZE</c>).</summary>
    public static EventType<ActivateEventArgs> Iconized { get; } = Make<ActivateEventArgs>(EventId.Iconize, Activate);
    /// <summary>Fires when the window is being destroyed (<c>wxEVT_DESTROY</c>).</summary>
    public static EventType<WxEventArgs> Destroyed { get; } = Make<WxEventArgs>(EventId.Destroy, Plain);
    /// <summary>Fires when the window gains keyboard focus (<c>wxEVT_SET_FOCUS</c>).</summary>
    public static EventType<WxEventArgs> GotFocus { get; } = Make<WxEventArgs>(EventId.SetFocus, Plain);
    /// <summary>Fires when the window loses keyboard focus (<c>wxEVT_KILL_FOCUS</c>).</summary>
    public static EventType<WxEventArgs> LostFocus { get; } = Make<WxEventArgs>(EventId.KillFocus, Plain);
    /// <summary>Fires when the window must repaint (<c>wxEVT_PAINT</c>). Draw from the handler.</summary>
    public static EventType<PaintEventArgs> Paint { get; } = Make<PaintEventArgs>(EventId.Paint, Repaint);
    /// <summary>Fires when a context menu is requested, by right-click or the menu key
    /// (<c>wxEVT_CONTEXT_MENU</c>).</summary>
    public static EventType<ContextMenuEventArgs> ContextMenu { get; } = Make<ContextMenuEventArgs>(EventId.ContextMenu, Context);

    /// <summary>Asks what state a command should be in. Bind it with the command's ID and answer from the
    /// application's own state; wxWidgets applies the answer everywhere that command appears.</summary>
    public static EventType<UpdateUIEventArgs> UpdateUI { get; } = Make<UpdateUIEventArgs>(EventId.UpdateUI, UpdateUIArgs);
    /// <summary>Fires when the application is idle (<c>wxEVT_IDLE</c>). Where background work and update-UI
    /// processing happen.</summary>
    public static EventType<IdleEventArgs> Idle { get; } = Make<IdleEventArgs>(EventId.Idle, IdleArgs);
    /// <summary>Fires when focus moves to a descendant window (<c>wxEVT_CHILD_FOCUS</c>).</summary>
    public static EventType<WxEventArgs> ChildFocus { get; } = Make<WxEventArgs>(EventId.ChildFocus, Plain);
    /// <summary>Fires on Tab/Shift+Tab navigation between controls (<c>wxEVT_NAVIGATION_KEY</c>).</summary>
    public static EventType<NavigationKeyEventArgs> NavigationKey { get; } = Make<NavigationKeyEventArgs>(EventId.NavigationKey, NavigationArgs);
    /// <summary>The mouse capture was taken away. Any window that calls <see cref="Window.CaptureMouse"/>
    /// must handle this; wxWidgets asserts if it does not.</summary>
    public static EventType<WxEventArgs> MouseCaptureLost { get; } = Make<WxEventArgs>(EventId.MouseCaptureLost, Plain);
    /// <summary>Fires when another window takes the mouse capture (<c>wxEVT_MOUSE_CAPTURE_CHANGED</c>).</summary>
    public static EventType<WxEventArgs> MouseCaptureChanged { get; } = Make<WxEventArgs>(EventId.MouseCaptureChanged, Plain);
    /// <summary>Fires when files are dropped on a window that accepts them (<c>wxEVT_DROP_FILES</c>).</summary>
    public static EventType<DropFilesEventArgs> DropFiles { get; } = Make<DropFilesEventArgs>(EventId.DropFiles, DropFilesArgs);
    /// <summary>Fires when a registered system-wide hot key is pressed (<c>wxEVT_HOTKEY</c>).</summary>
    public static EventType<KeyEventArgs> HotKey { get; } = Make<KeyEventArgs>(EventId.HotKey, Key);
    /// <summary>Fires when context help is requested (<c>wxEVT_HELP</c>).</summary>
    public static EventType<HelpEventArgs> Help { get; } = Make<HelpEventArgs>(EventId.Help, HelpArgs);
    /// <summary>A menu is about to open. The moment to rebuild a dynamic menu, before the user sees it.</summary>
    public static EventType<MenuEventArgs> MenuOpened { get; } = Make<MenuEventArgs>(EventId.MenuOpen, MenuArgs);
    /// <summary>Fires when a menu has closed (<c>wxEVT_MENU_CLOSE</c>).</summary>
    public static EventType<MenuEventArgs> MenuClosed { get; } = Make<MenuEventArgs>(EventId.MenuClose, MenuArgs);
    /// <summary>An item is highlighted as the user moves through a menu. Paired with the item's help string,
    /// this is what puts a description in the status bar.</summary>
    public static EventType<MenuEventArgs> MenuHighlighted { get; } = Make<MenuEventArgs>(EventId.MenuHighlight, MenuArgs);

    // Mouse.
    /// <summary>Left button pressed (<c>wxEVT_LEFT_DOWN</c>).</summary>
    public static EventType<MouseEventArgs> MouseDown { get; } = Make<MouseEventArgs>(EventId.LeftDown, Mouse);
    /// <summary>Left button released (<c>wxEVT_LEFT_UP</c>).</summary>
    public static EventType<MouseEventArgs> MouseUp { get; } = Make<MouseEventArgs>(EventId.LeftUp, Mouse);
    /// <summary>Left button double-clicked (<c>wxEVT_LEFT_DCLICK</c>).</summary>
    public static EventType<MouseEventArgs> DoubleClicked { get; } = Make<MouseEventArgs>(EventId.LeftDoubleClick, Mouse);
    /// <summary>Right button pressed (<c>wxEVT_RIGHT_DOWN</c>).</summary>
    public static EventType<MouseEventArgs> RightDown { get; } = Make<MouseEventArgs>(EventId.RightDown, Mouse);
    /// <summary>Right button released (<c>wxEVT_RIGHT_UP</c>).</summary>
    public static EventType<MouseEventArgs> RightUp { get; } = Make<MouseEventArgs>(EventId.RightUp, Mouse);
    /// <summary>Right button double-clicked (<c>wxEVT_RIGHT_DCLICK</c>).</summary>
    public static EventType<MouseEventArgs> RightDoubleClicked { get; } = Make<MouseEventArgs>(EventId.RightDoubleClick, Mouse);
    /// <summary>Middle button pressed (<c>wxEVT_MIDDLE_DOWN</c>).</summary>
    public static EventType<MouseEventArgs> MiddleDown { get; } = Make<MouseEventArgs>(EventId.MiddleDown, Mouse);
    /// <summary>Middle button released (<c>wxEVT_MIDDLE_UP</c>).</summary>
    public static EventType<MouseEventArgs> MiddleUp { get; } = Make<MouseEventArgs>(EventId.MiddleUp, Mouse);
    /// <summary>Middle button double-clicked (<c>wxEVT_MIDDLE_DCLICK</c>).</summary>
    public static EventType<MouseEventArgs> MiddleDoubleClicked { get; } = Make<MouseEventArgs>(EventId.MiddleDoubleClick, Mouse);
    /// <summary>Pointer moved over the window (<c>wxEVT_MOTION</c>).</summary>
    public static EventType<MouseEventArgs> MouseMoved { get; } = Make<MouseEventArgs>(EventId.Motion, Mouse);
    /// <summary>Pointer entered the window (<c>wxEVT_ENTER_WINDOW</c>).</summary>
    public static EventType<MouseEventArgs> MouseEntered { get; } = Make<MouseEventArgs>(EventId.EnterWindow, Mouse);
    /// <summary>Pointer left the window (<c>wxEVT_LEAVE_WINDOW</c>).</summary>
    public static EventType<MouseEventArgs> MouseLeft { get; } = Make<MouseEventArgs>(EventId.LeaveWindow, Mouse);
    /// <summary>Mouse wheel turned (<c>wxEVT_MOUSEWHEEL</c>).</summary>
    public static EventType<MouseEventArgs> MouseWheel { get; } = Make<MouseEventArgs>(EventId.MouseWheel, Mouse);

    // Keyboard. CharHook reaches a top-level window before the focused control sees the key, which is where
    // application-wide shortcuts belong; Char reports the character a key produces after translation.
    /// <summary>A key seen by the top-level window before the focused control (<c>wxEVT_CHAR_HOOK</c>).
    /// Where application-wide shortcuts belong; skip it to let the key reach the control.</summary>
    public static EventType<KeyEventArgs> CharHook { get; } = Make<KeyEventArgs>(EventId.CharHook, Key);
    /// <summary>A key was pressed down while this control had focus (<c>wxEVT_KEY_DOWN</c>). Reports the key,
    /// not the character; use <see cref="Char"/> for text input.</summary>
    public static EventType<KeyEventArgs> KeyDown { get; } = Make<KeyEventArgs>(EventId.KeyDown, Key);
    /// <summary>A key was released (<c>wxEVT_KEY_UP</c>).</summary>
    public static EventType<KeyEventArgs> KeyUp { get; } = Make<KeyEventArgs>(EventId.KeyUp, Key);
    /// <summary>A character was produced by a key press, after translation (<c>wxEVT_CHAR</c>). This is the
    /// one to use for text the user typed.</summary>
    public static EventType<KeyEventArgs> Char { get; } = Make<KeyEventArgs>(EventId.Char, Key);

    // Control commands.
    /// <summary>A button was clicked (<c>wxEVT_BUTTON</c>).</summary>
    public static EventType<CommandEventArgs> ButtonClicked { get; } = Make<CommandEventArgs>(EventId.Button, Command);
    /// <summary>A checkbox was toggled (<c>wxEVT_CHECKBOX</c>).</summary>
    public static EventType<CommandEventArgs> CheckBoxToggled { get; } = Make<CommandEventArgs>(EventId.CheckBox, Command);
    /// <summary>A choice (drop-down) selection changed (<c>wxEVT_CHOICE</c>).</summary>
    public static EventType<CommandEventArgs> ChoiceSelected { get; } = Make<CommandEventArgs>(EventId.Choice, Command);
    /// <summary>A list box selection changed (<c>wxEVT_LISTBOX</c>).</summary>
    public static EventType<CommandEventArgs> ListBoxSelected { get; } = Make<CommandEventArgs>(EventId.ListBox, Command);
    /// <summary>A list box row was double-clicked or activated (<c>wxEVT_LISTBOX_DCLICK</c>).</summary>
    public static EventType<CommandEventArgs> ListBoxDoubleClicked { get; } = Make<CommandEventArgs>(EventId.ListBoxDoubleClick, Command);
    /// <summary>A text control's contents changed (<c>wxEVT_TEXT</c>).</summary>
    public static EventType<CommandEventArgs> TextChanged { get; } = Make<CommandEventArgs>(EventId.Text, Command);
    /// <summary>Enter was pressed in a text control (<c>wxEVT_TEXT_ENTER</c>).</summary>
    public static EventType<CommandEventArgs> TextEntered { get; } = Make<CommandEventArgs>(EventId.TextEnter, Command);
    /// <summary>A menu item or toolbar button was invoked (<c>wxEVT_MENU</c>). Bind with the command ID.</summary>
    public static EventType<CommandEventArgs> MenuCommand { get; } = Make<CommandEventArgs>(EventId.Menu, Command);
    /// <summary>A slider's value changed (<c>wxEVT_SLIDER</c>).</summary>
    public static EventType<CommandEventArgs> SliderChanged { get; } = Make<CommandEventArgs>(EventId.Slider, Command);
    /// <summary>A radio button was selected (<c>wxEVT_RADIOBUTTON</c>).</summary>
    public static EventType<CommandEventArgs> RadioButtonSelected { get; } = Make<CommandEventArgs>(EventId.RadioButton, Command);
    /// <summary>A radio box selection changed (<c>wxEVT_RADIOBOX</c>).</summary>
    public static EventType<CommandEventArgs> RadioBoxSelected { get; } = Make<CommandEventArgs>(EventId.RadioBox, Command);
    /// <summary>A combo box selection changed (<c>wxEVT_COMBOBOX</c>).</summary>
    public static EventType<CommandEventArgs> ComboBoxSelected { get; } = Make<CommandEventArgs>(EventId.ComboBox, Command);
    /// <summary>A combo box's drop-down list opened (<c>wxEVT_COMBOBOX_DROPDOWN</c>).</summary>
    public static EventType<CommandEventArgs> ComboBoxDropDown { get; } = Make<CommandEventArgs>(EventId.ComboBoxDropDown, Command);
    /// <summary>A combo box's drop-down list closed (<c>wxEVT_COMBOBOX_CLOSEUP</c>).</summary>
    public static EventType<CommandEventArgs> ComboBoxCloseUp { get; } = Make<CommandEventArgs>(EventId.ComboBoxCloseUp, Command);
    /// <summary>A toggle button changed state (<c>wxEVT_TOGGLEBUTTON</c>).</summary>
    public static EventType<CommandEventArgs> ToggleButtonToggled { get; } = Make<CommandEventArgs>(EventId.ToggleButton, Command);
    /// <summary>An item in a checklist box was ticked or unticked (<c>wxEVT_CHECKLISTBOX</c>).</summary>
    public static EventType<CommandEventArgs> CheckListBoxToggled { get; } = Make<CommandEventArgs>(EventId.CheckListBox, Command);
    /// <summary>A spin control's value changed (<c>wxEVT_SPINCTRL</c>).</summary>
    public static EventType<SpinEventArgs> SpinChanged { get; } = Make<SpinEventArgs>(EventId.SpinCtrl, Spin_);
    /// <summary>A floating-point spin control's value changed (<c>wxEVT_SPINCTRLDOUBLE</c>).</summary>
    public static EventType<SpinEventArgs> SpinDoubleChanged { get; } = Make<SpinEventArgs>(EventId.SpinCtrlDouble, Spin_);
    /// <summary>A scrollbar thumb is being dragged (<c>wxEVT_SCROLL_THUMBTRACK</c>).</summary>
    public static EventType<ScrollEventArgs> ScrollThumbTrack { get; } = Make<ScrollEventArgs>(EventId.ScrollThumbTrack, Scroll);
    /// <summary>A scrollbar settled on a new position (<c>wxEVT_SCROLL_CHANGED</c>).</summary>
    public static EventType<ScrollEventArgs> ScrollChanged { get; } = Make<ScrollEventArgs>(EventId.ScrollChanged, Scroll);
    /// <summary>A hyperlink control was clicked (<c>wxEVT_HYPERLINK</c>).</summary>
    public static EventType<HyperlinkEventArgs> HyperlinkClicked { get; } = Make<HyperlinkEventArgs>(EventId.Hyperlink, Link);
    /// <summary>The search button of a search control was pressed (<c>wxEVT_SEARCH</c>).</summary>
    public static EventType<CommandEventArgs> Search { get; } = Make<CommandEventArgs>(EventId.Search, Command);
    /// <summary>The cancel button of a search control was pressed (<c>wxEVT_SEARCH_CANCEL</c>).</summary>
    public static EventType<CommandEventArgs> SearchCancelled { get; } = Make<CommandEventArgs>(EventId.SearchCancel, Command);
    /// <summary>A date picker's value changed (<c>wxEVT_DATE_CHANGED</c>).</summary>
    public static EventType<DateEventArgs> DateChanged { get; } = Make<DateEventArgs>(EventId.DateChanged, Date);
    /// <summary>A time picker's value changed (<c>wxEVT_TIME_CHANGED</c>).</summary>
    public static EventType<DateEventArgs> TimeChanged { get; } = Make<DateEventArgs>(EventId.TimeChanged, Date);
    /// <summary>A <see cref="WxSharp.Timer"/> ticked (<c>wxEVT_TIMER</c>). Usually consumed via the timer's
    /// own event; bind here only for a shared handler by ID.</summary>
    public static EventType<CommandEventArgs> Timer { get; } = Make<CommandEventArgs>(EventId.Timer, Command);
    /// <summary>The system clipboard's contents changed (<c>wxEVT_CLIPBOARD_CHANGED</c>).</summary>
    public static EventType<WxEventArgs> ClipboardChanged { get; } = Make<WxEventArgs>(EventId.ClipboardChanged, Plain);
    /// <summary>A spin button was turned (<c>wxEVT_SPIN</c>).</summary>
    public static EventType<SpinEventArgs> Spin { get; } = Make<SpinEventArgs>(EventId.Spin, Spin_);
    /// <summary>A spin button's up arrow was pressed (<c>wxEVT_SPIN_UP</c>).</summary>
    public static EventType<SpinEventArgs> SpinUp { get; } = Make<SpinEventArgs>(EventId.SpinUp, Spin_);
    /// <summary>A spin button's down arrow was pressed (<c>wxEVT_SPIN_DOWN</c>).</summary>
    public static EventType<SpinEventArgs> SpinDown { get; } = Make<SpinEventArgs>(EventId.SpinDown, Spin_);
    /// <summary>A standalone scrollbar moved (<c>wxEVT_SCROLL</c>).</summary>
    public static EventType<ScrollEventArgs> ScrollBarChanged { get; } = Make<ScrollEventArgs>(EventId.ScrollBar, Scroll);
    /// <summary>A scrollbar was moved to the top/left (<c>wxEVT_SCROLL_TOP</c>).</summary>
    public static EventType<ScrollEventArgs> ScrollToTop { get; } = Make<ScrollEventArgs>(EventId.ScrollTop, Scroll);
    /// <summary>A scrollbar was moved to the bottom/right (<c>wxEVT_SCROLL_BOTTOM</c>).</summary>
    public static EventType<ScrollEventArgs> ScrollToBottom { get; } = Make<ScrollEventArgs>(EventId.ScrollBottom, Scroll);
    /// <summary>A scrollbar line-up/left button was pressed (<c>wxEVT_SCROLL_LINEUP</c>).</summary>
    public static EventType<ScrollEventArgs> ScrollLineUp { get; } = Make<ScrollEventArgs>(EventId.ScrollLineUp, Scroll);
    /// <summary>A scrollbar line-down/right button was pressed (<c>wxEVT_SCROLL_LINEDOWN</c>).</summary>
    public static EventType<ScrollEventArgs> ScrollLineDown { get; } = Make<ScrollEventArgs>(EventId.ScrollLineDown, Scroll);
    /// <summary>A scrollbar page-up/left region was clicked (<c>wxEVT_SCROLL_PAGEUP</c>).</summary>
    public static EventType<ScrollEventArgs> ScrollPageUp { get; } = Make<ScrollEventArgs>(EventId.ScrollPageUp, Scroll);
    /// <summary>A scrollbar page-down/right region was clicked (<c>wxEVT_SCROLL_PAGEDOWN</c>).</summary>
    public static EventType<ScrollEventArgs> ScrollPageDown { get; } = Make<ScrollEventArgs>(EventId.ScrollPageDown, Scroll);
    /// <summary>A scrollbar thumb was released after dragging (<c>wxEVT_SCROLL_THUMBRELEASE</c>).</summary>
    public static EventType<ScrollEventArgs> ScrollThumbReleased { get; } = Make<ScrollEventArgs>(EventId.ScrollThumbRelease, Scroll);
    /// <summary>The user tried to type past a text control's length limit (<c>wxEVT_TEXT_MAXLEN</c>).</summary>
    public static EventType<CommandEventArgs> TextMaxLengthReached { get; } = Make<CommandEventArgs>(EventId.TextMaxLength, Command);
    /// <summary>A URL in a rich text control was clicked (<c>wxEVT_TEXT_URL</c>).</summary>
    public static EventType<TextUrlEventArgs> TextUrlClicked { get; } = Make<TextUrlEventArgs>(EventId.TextUrl, TextUrlArgs);
    /// <summary>Text was copied to the clipboard from a control (<c>wxEVT_TEXT_COPY</c>).</summary>
    public static EventType<CommandEventArgs> TextCopy { get; } = Make<CommandEventArgs>(EventId.TextCopy, Command);
    /// <summary>Text was cut to the clipboard from a control (<c>wxEVT_TEXT_CUT</c>).</summary>
    public static EventType<CommandEventArgs> TextCut { get; } = Make<CommandEventArgs>(EventId.TextCut, Command);
    /// <summary>Text was pasted into a control (<c>wxEVT_TEXT_PASTE</c>).</summary>
    public static EventType<CommandEventArgs> TextPaste { get; } = Make<CommandEventArgs>(EventId.TextPaste, Command);
    /// <summary>The pointer entered a toolbar tool (<c>wxEVT_TOOL_ENTER</c>).</summary>
    public static EventType<CommandEventArgs> ToolEntered { get; } = Make<CommandEventArgs>(EventId.ToolEnter, Command);
    /// <summary>A toolbar tool was right-clicked (<c>wxEVT_TOOL_RCLICKED</c>).</summary>
    public static EventType<CommandEventArgs> ToolRightClicked { get; } = Make<CommandEventArgs>(EventId.ToolRightClick, Command);
    /// <summary>A toolbar tool's drop-down arrow was pressed (<c>wxEVT_TOOL_DROPDOWN</c>).</summary>
    public static EventType<CommandEventArgs> ToolDropDown { get; } = Make<CommandEventArgs>(EventId.ToolDropDown, Command);

    // Book controls.
    /// <summary>A notebook's visible page changed (<c>wxEVT_NOTEBOOK_PAGE_CHANGED</c>).</summary>
    public static EventType<BookEventArgs> NotebookPageChanged { get; } = Make<BookEventArgs>(EventId.NotebookPageChanged, Book);
    /// <summary>A notebook's page is about to change (<c>wxEVT_NOTEBOOK_PAGE_CHANGING</c>). Veto to keep the
    /// current page.</summary>
    public static EventType<BookEventArgs> NotebookPageChanging { get; } = Make<BookEventArgs>(EventId.NotebookPageChanging, Book);
    /// <summary>A book control's visible page changed (<c>wxEVT_BOOKCTRL_PAGE_CHANGED</c>).</summary>
    public static EventType<BookEventArgs> BookPageChanged { get; } = Make<BookEventArgs>(EventId.BookPageChanged, Book);
    /// <summary>A book control's page is about to change (<c>wxEVT_BOOKCTRL_PAGE_CHANGING</c>). Veto to keep
    /// the current page.</summary>
    public static EventType<BookEventArgs> BookPageChanging { get; } = Make<BookEventArgs>(EventId.BookPageChanging, Book);

    // wxListCtrl.
    /// <summary>A list item was selected (<c>wxEVT_LIST_ITEM_SELECTED</c>).</summary>
    public static EventType<ListEventArgs> ListItemSelected { get; } = Make<ListEventArgs>(EventId.ListItemSelected, List);
    /// <summary>A list item was deselected (<c>wxEVT_LIST_ITEM_DESELECTED</c>).</summary>
    public static EventType<ListEventArgs> ListItemDeselected { get; } = Make<ListEventArgs>(EventId.ListItemDeselected, List);
    /// <summary>A list item was activated by double-click or Enter (<c>wxEVT_LIST_ITEM_ACTIVATED</c>).</summary>
    public static EventType<ListEventArgs> ListItemActivated { get; } = Make<ListEventArgs>(EventId.ListItemActivated, List);
    /// <summary>Keyboard focus moved to a list item (<c>wxEVT_LIST_ITEM_FOCUSED</c>).</summary>
    public static EventType<ListEventArgs> ListItemFocused { get; } = Make<ListEventArgs>(EventId.ListItemFocused, List);
    /// <summary>A list item was right-clicked (<c>wxEVT_LIST_ITEM_RIGHT_CLICK</c>).</summary>
    public static EventType<ListEventArgs> ListItemRightClicked { get; } = Make<ListEventArgs>(EventId.ListItemRightClick, List);
    /// <summary>A column header was clicked (<c>wxEVT_LIST_COL_CLICK</c>).</summary>
    public static EventType<ListEventArgs> ListColumnClicked { get; } = Make<ListEventArgs>(EventId.ListColumnClick, List);
    /// <summary>A key was pressed in the list (<c>wxEVT_LIST_KEY_DOWN</c>).</summary>
    public static EventType<ListEventArgs> ListKeyDown { get; } = Make<ListEventArgs>(EventId.ListKeyDown, List);
    /// <summary>Label editing began on an item (<c>wxEVT_LIST_BEGIN_LABEL_EDIT</c>). Veto to forbid it.</summary>
    public static EventType<ListEventArgs> ListBeginLabelEdit { get; } = Make<ListEventArgs>(EventId.ListBeginLabelEdit, List);
    /// <summary>Label editing finished (<c>wxEVT_LIST_END_LABEL_EDIT</c>). Veto to reject the new label.</summary>
    public static EventType<ListEventArgs> ListEndLabelEdit { get; } = Make<ListEventArgs>(EventId.ListEndLabelEdit, List);
    /// <summary>A drag of an item began with the left button (<c>wxEVT_LIST_BEGIN_DRAG</c>).</summary>
    public static EventType<ListEventArgs> ListBeginDrag { get; } = Make<ListEventArgs>(EventId.ListBeginDrag, List);
    /// <summary>A drag of an item began with the right button (<c>wxEVT_LIST_BEGIN_RDRAG</c>).</summary>
    public static EventType<ListEventArgs> ListBeginRightDrag { get; } = Make<ListEventArgs>(EventId.ListBeginRightDrag, List);
    /// <summary>A list item was middle-clicked (<c>wxEVT_LIST_ITEM_MIDDLE_CLICK</c>).</summary>
    public static EventType<ListEventArgs> ListItemMiddleClicked { get; } = Make<ListEventArgs>(EventId.ListItemMiddleClick, List);
    /// <summary>A checklist item was ticked (<c>wxEVT_LIST_ITEM_CHECKED</c>).</summary>
    public static EventType<ListEventArgs> ListItemChecked { get; } = Make<ListEventArgs>(EventId.ListItemChecked, List);
    /// <summary>A checklist item was unticked (<c>wxEVT_LIST_ITEM_UNCHECKED</c>).</summary>
    public static EventType<ListEventArgs> ListItemUnchecked { get; } = Make<ListEventArgs>(EventId.ListItemUnchecked, List);
    /// <summary>A column header was right-clicked (<c>wxEVT_LIST_COL_RIGHT_CLICK</c>).</summary>
    public static EventType<ListEventArgs> ListColumnRightClicked { get; } = Make<ListEventArgs>(EventId.ListColumnRightClick, List);
    /// <summary>A column divider began being dragged to resize (<c>wxEVT_LIST_COL_BEGIN_DRAG</c>).</summary>
    public static EventType<ListEventArgs> ListColumnBeginDrag { get; } = Make<ListEventArgs>(EventId.ListColumnBeginDrag, List);
    /// <summary>A column resize drag finished (<c>wxEVT_LIST_COL_END_DRAG</c>).</summary>
    public static EventType<ListEventArgs> ListColumnEndDrag { get; } = Make<ListEventArgs>(EventId.ListColumnEndDrag, List);
    /// <summary>A list item was deleted (<c>wxEVT_LIST_DELETE_ITEM</c>).</summary>
    public static EventType<ListEventArgs> ListItemDeleted { get; } = Make<ListEventArgs>(EventId.ListDeleteItem, List);
    /// <summary>All list items were deleted (<c>wxEVT_LIST_DELETE_ALL_ITEMS</c>).</summary>
    public static EventType<ListEventArgs> ListAllItemsDeleted { get; } = Make<ListEventArgs>(EventId.ListDeleteAllItems, List);
    /// <summary>A list item was inserted (<c>wxEVT_LIST_INSERT_ITEM</c>).</summary>
    public static EventType<ListEventArgs> ListItemInserted { get; } = Make<ListEventArgs>(EventId.ListInsertItem, List);
    /// <summary>A virtual list is about to draw a range of rows and is asking for them to be prepared.</summary>
    public static EventType<ListEventArgs> ListCacheHint { get; } = Make<ListEventArgs>(EventId.ListCacheHint, List);

    // wxTreeCtrl.
    /// <summary>The selected tree item changed (<c>wxEVT_TREE_SEL_CHANGED</c>).</summary>
    public static EventType<TreeEventArgs> TreeSelectionChanged { get; } = Make<TreeEventArgs>(EventId.TreeSelectionChanged, Tree);
    /// <summary>The tree selection is about to change (<c>wxEVT_TREE_SEL_CHANGING</c>). Veto to prevent it.</summary>
    public static EventType<TreeEventArgs> TreeSelectionChanging { get; } = Make<TreeEventArgs>(EventId.TreeSelectionChanging, Tree);
    /// <summary>A tree item was activated by double-click or Enter (<c>wxEVT_TREE_ITEM_ACTIVATED</c>).</summary>
    public static EventType<TreeEventArgs> TreeItemActivated { get; } = Make<TreeEventArgs>(EventId.TreeItemActivated, Tree);
    /// <summary>A tree item was expanded (<c>wxEVT_TREE_ITEM_EXPANDED</c>).</summary>
    public static EventType<TreeEventArgs> TreeItemExpanded { get; } = Make<TreeEventArgs>(EventId.TreeItemExpanded, Tree);
    /// <summary>A tree item is about to expand (<c>wxEVT_TREE_ITEM_EXPANDING</c>). Veto to prevent it.</summary>
    public static EventType<TreeEventArgs> TreeItemExpanding { get; } = Make<TreeEventArgs>(EventId.TreeItemExpanding, Tree);
    /// <summary>A tree item was collapsed (<c>wxEVT_TREE_ITEM_COLLAPSED</c>).</summary>
    public static EventType<TreeEventArgs> TreeItemCollapsed { get; } = Make<TreeEventArgs>(EventId.TreeItemCollapsed, Tree);
    /// <summary>A tree item is about to collapse (<c>wxEVT_TREE_ITEM_COLLAPSING</c>). Veto to prevent it.</summary>
    public static EventType<TreeEventArgs> TreeItemCollapsing { get; } = Make<TreeEventArgs>(EventId.TreeItemCollapsing, Tree);
    /// <summary>A tree item was right-clicked (<c>wxEVT_TREE_ITEM_RIGHT_CLICK</c>).</summary>
    public static EventType<TreeEventArgs> TreeItemRightClicked { get; } = Make<TreeEventArgs>(EventId.TreeItemRightClick, Tree);
    /// <summary>A key was pressed in the tree (<c>wxEVT_TREE_KEY_DOWN</c>).</summary>
    public static EventType<TreeEventArgs> TreeKeyDown { get; } = Make<TreeEventArgs>(EventId.TreeKeyDown, Tree);
    /// <summary>Label editing began on a tree item (<c>wxEVT_TREE_BEGIN_LABEL_EDIT</c>). Veto to forbid it.</summary>
    public static EventType<TreeEventArgs> TreeBeginLabelEdit { get; } = Make<TreeEventArgs>(EventId.TreeBeginLabelEdit, Tree);
    /// <summary>Label editing finished on a tree item (<c>wxEVT_TREE_END_LABEL_EDIT</c>). Veto to reject the
    /// new label.</summary>
    public static EventType<TreeEventArgs> TreeEndLabelEdit { get; } = Make<TreeEventArgs>(EventId.TreeEndLabelEdit, Tree);
    /// <summary>A context menu was asked for on a tree item, by right-click or by the keyboard's menu key.</summary>
    public static EventType<TreeEventArgs> TreeItemMenu { get; } = Make<TreeEventArgs>(EventId.TreeItemMenu, Tree);
    /// <summary>A drag of a tree item began (<c>wxEVT_TREE_BEGIN_DRAG</c>).</summary>
    public static EventType<TreeEventArgs> TreeBeginDrag { get; } = Make<TreeEventArgs>(EventId.TreeBeginDrag, Tree);
    /// <summary>A tree item drag finished (<c>wxEVT_TREE_END_DRAG</c>).</summary>
    public static EventType<TreeEventArgs> TreeEndDrag { get; } = Make<TreeEventArgs>(EventId.TreeEndDrag, Tree);
    /// <summary>A tree item was middle-clicked (<c>wxEVT_TREE_ITEM_MIDDLE_CLICK</c>).</summary>
    public static EventType<TreeEventArgs> TreeItemMiddleClicked { get; } = Make<TreeEventArgs>(EventId.TreeItemMiddleClick, Tree);
    /// <summary>A tree item was deleted (<c>wxEVT_TREE_DELETE_ITEM</c>).</summary>
    public static EventType<TreeEventArgs> TreeItemDeleted { get; } = Make<TreeEventArgs>(EventId.TreeDeleteItem, Tree);
    /// <summary>A tooltip is needed for a tree item (<c>wxEVT_TREE_ITEM_GETTOOLTIP</c>).</summary>
    public static EventType<TreeEventArgs> TreeItemToolTip { get; } = Make<TreeEventArgs>(EventId.TreeItemToolTip, Tree);
    /// <summary>A tree item's state image was clicked (<c>wxEVT_TREE_STATE_IMAGE_CLICK</c>).</summary>
    public static EventType<TreeEventArgs> TreeStateImageClicked { get; } = Make<TreeEventArgs>(EventId.TreeStateImageClick, Tree);

    // wxDataViewCtrl.
    /// <summary>The data-view selection changed (<c>wxEVT_DATAVIEW_SELECTION_CHANGED</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewSelectionChanged { get; } = Make<DataViewEventArgs>(EventId.DataViewSelectionChanged, DataView);
    /// <summary>A data-view item was activated (<c>wxEVT_DATAVIEW_ITEM_ACTIVATED</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewItemActivated { get; } = Make<DataViewEventArgs>(EventId.DataViewItemActivated, DataView);
    /// <summary>A context menu was requested on a data-view item (<c>wxEVT_DATAVIEW_ITEM_CONTEXT_MENU</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewItemContextMenu { get; } = Make<DataViewEventArgs>(EventId.DataViewItemContextMenu, DataView);
    /// <summary>A data-view item was expanded (<c>wxEVT_DATAVIEW_ITEM_EXPANDED</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewItemExpanded { get; } = Make<DataViewEventArgs>(EventId.DataViewItemExpanded, DataView);
    /// <summary>A data-view item is about to expand (<c>wxEVT_DATAVIEW_ITEM_EXPANDING</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewItemExpanding { get; } = Make<DataViewEventArgs>(EventId.DataViewItemExpanding, DataView);
    /// <summary>A data-view item was collapsed (<c>wxEVT_DATAVIEW_ITEM_COLLAPSED</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewItemCollapsed { get; } = Make<DataViewEventArgs>(EventId.DataViewItemCollapsed, DataView);
    /// <summary>A data-view item is about to collapse (<c>wxEVT_DATAVIEW_ITEM_COLLAPSING</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewItemCollapsing { get; } = Make<DataViewEventArgs>(EventId.DataViewItemCollapsing, DataView);
    /// <summary>Editing of a data-view cell began (<c>wxEVT_DATAVIEW_ITEM_EDITING_STARTED</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewEditingStarted { get; } = Make<DataViewEventArgs>(EventId.DataViewItemEditingStarted, DataView);
    /// <summary>Editing of a data-view cell finished (<c>wxEVT_DATAVIEW_ITEM_EDITING_DONE</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewEditingDone { get; } = Make<DataViewEventArgs>(EventId.DataViewItemEditingDone, DataView);
    /// <summary>A data-view cell value changed (<c>wxEVT_DATAVIEW_ITEM_VALUE_CHANGED</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewValueChanged { get; } = Make<DataViewEventArgs>(EventId.DataViewItemValueChanged, DataView);
    /// <summary>A data-view column header was clicked (<c>wxEVT_DATAVIEW_COLUMN_HEADER_CLICK</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewColumnHeaderClicked { get; } = Make<DataViewEventArgs>(EventId.DataViewColumnHeaderClick, DataView);
    /// <summary>A data-view column header was right-clicked (<c>wxEVT_DATAVIEW_COLUMN_HEADER_RIGHT_CLICK</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewColumnHeaderRightClicked { get; } = Make<DataViewEventArgs>(EventId.DataViewColumnHeaderRightClick, DataView);
    /// <summary>A data-view column was sorted (<c>wxEVT_DATAVIEW_COLUMN_SORTED</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewColumnSorted { get; } = Make<DataViewEventArgs>(EventId.DataViewColumnSorted, DataView);
    /// <summary>Data-view columns were reordered by dragging (<c>wxEVT_DATAVIEW_COLUMN_REORDERED</c>).</summary>
    public static EventType<DataViewEventArgs> DataViewColumnReordered { get; } = Make<DataViewEventArgs>(EventId.DataViewColumnReordered, DataView);

    // wxSplitterWindow and wxGrid.
    /// <summary>The splitter's sash settled at a new position (<c>wxEVT_SPLITTER_SASH_POS_CHANGED</c>).</summary>
    public static EventType<SplitterEventArgs> SashPositionChanged { get; } = Make<SplitterEventArgs>(EventId.SplitterSashPositionChanged, Splitter);
    /// <summary>The sash was double-clicked, which normally unsplits (<c>wxEVT_SPLITTER_DCLICK</c>). Veto to
    /// keep the split.</summary>
    public static EventType<SplitterEventArgs> SashDoubleClicked { get; } = Make<SplitterEventArgs>(EventId.SplitterDoubleClick, Splitter);
    /// <summary>The sash is being dragged. Veto to refuse the new position.</summary>
    public static EventType<SplitterEventArgs> SashPositionChanging { get; } = Make<SplitterEventArgs>(EventId.SplitterSashPositionChanging, Splitter);
    /// <summary>The splitter was unsplit back to a single pane (<c>wxEVT_SPLITTER_UNSPLIT</c>).</summary>
    public static EventType<SplitterEventArgs> Unsplit { get; } = Make<SplitterEventArgs>(EventId.SplitterUnsplit, Splitter);
    /// <summary>A grid cell's value changed (<c>wxEVT_GRID_CELL_CHANGED</c>).</summary>
    public static EventType<GridEventArgs> GridCellChanged { get; } = Make<GridEventArgs>(EventId.GridCellChanged, Grid);
    /// <summary>A grid cell was selected (<c>wxEVT_GRID_SELECT_CELL</c>).</summary>
    public static EventType<GridEventArgs> GridCellSelected { get; } = Make<GridEventArgs>(EventId.GridSelectCell, Grid);
}
