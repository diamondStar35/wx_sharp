using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WxSharp;

/// <summary>The result of an <see cref="Accessible"/> query, following <c>wxAccStatus</c>.</summary>
public enum AccessibleStatus
{
    /// <summary>The query failed.</summary>
    Fail = 0,
    /// <summary>The query succeeded with a "false" answer.</summary>
    False = 1,
    /// <summary>The query succeeded.</summary>
    Ok = 2,
    /// <summary>This object does not implement the query; fall back to the default.</summary>
    NotImplemented = 3,
    /// <summary>The query is not supported.</summary>
    NotSupported = 4,
    /// <summary>An argument was invalid.</summary>
    InvalidArgument = 5,
}

/// <summary>A direction for <see cref="Accessible.Navigate"/>, following <c>wxNavDir</c>.</summary>
public enum AccessibleNavigationDirection
{
    /// <summary>Spatially below.</summary>
    Down,
    /// <summary>The first child.</summary>
    FirstChild,
    /// <summary>The last child.</summary>
    LastChild,
    /// <summary>Spatially to the left.</summary>
    Left,
    /// <summary>The next sibling.</summary>
    Next,
    /// <summary>The previous sibling.</summary>
    Previous,
    /// <summary>Spatially to the right.</summary>
    Right,
    /// <summary>Spatially above.</summary>
    Up,
}

/// <summary>What a <see cref="Accessible.Select"/> call should do, following <c>wxAccSelectionFlags</c>.</summary>
[Flags]
public enum AccessibleSelection
{
    /// <summary>No change.</summary>
    None = 0,
    /// <summary>Give the object keyboard focus.</summary>
    TakeFocus = 1,
    /// <summary>Make this the whole selection.</summary>
    TakeSelection = 2,
    /// <summary>Extend the selection to this object.</summary>
    ExtendSelection = 4,
    /// <summary>Add this object to the selection.</summary>
    AddSelection = 8,
    /// <summary>Remove this object from the selection.</summary>
    RemoveSelection = 16,
}

/// <summary>A standard window part, following the <c>OBJID_*</c> values used by <c>wxAccessible</c>.</summary>
public enum AccessibleObjectType
{
    /// <summary>The window itself.</summary>
    Window = 0,
    /// <summary>The window (system) menu.</summary>
    SystemMenu = -1,
    /// <summary>The title bar.</summary>
    TitleBar = -2,
    /// <summary>The menu bar.</summary>
    Menu = -3,
    /// <summary>The client area.</summary>
    Client = -4,
    /// <summary>The vertical scrollbar.</summary>
    VerticalScrollBar = -5,
    /// <summary>The horizontal scrollbar.</summary>
    HorizontalScrollBar = -6,
    /// <summary>The resize grip.</summary>
    SizeGrip = -7,
    /// <summary>The text caret.</summary>
    Caret = -8,
    /// <summary>The mouse cursor.</summary>
    Cursor = -9,
    /// <summary>An alert.</summary>
    Alert = -10,
    /// <summary>A sound.</summary>
    Sound = -11,
}

/// <summary>An accessibility event to raise with <see cref="Accessible.NotifyEvent"/>, following the
/// <c>EVENT_OBJECT_*</c> values.</summary>
public enum AccessibleEvent
{
    /// <summary>An object was created.</summary>
    Create = 0x8000,
    /// <summary>An object was destroyed.</summary>
    Destroy = 0x8001,
    /// <summary>An object was shown.</summary>
    Show = 0x8002,
    /// <summary>An object was hidden.</summary>
    Hide = 0x8003,
    /// <summary>Children were reordered.</summary>
    Reorder = 0x8004,
    /// <summary>Focus moved.</summary>
    Focus = 0x8005,
    /// <summary>The selection changed.</summary>
    Selection = 0x8006,
    /// <summary>An item was added to the selection.</summary>
    SelectionAdd = 0x8007,
    /// <summary>An item was removed from the selection.</summary>
    SelectionRemove = 0x8008,
    /// <summary>The selection changed within a container.</summary>
    SelectionWithin = 0x8009,
    /// <summary>An object's state changed.</summary>
    StateChanged = 0x800A,
    /// <summary>An object moved or resized.</summary>
    LocationChanged = 0x800B,
    /// <summary>An object's name changed.</summary>
    NameChanged = 0x800C,
    /// <summary>An object's description changed.</summary>
    DescriptionChanged = 0x800D,
    /// <summary>An object's value changed.</summary>
    ValueChanged = 0x800E,
    /// <summary>An object's parent changed.</summary>
    ParentChanged = 0x800F,
    /// <summary>An object's help text changed.</summary>
    HelpChanged = 0x8010,
    /// <summary>An object's default action changed.</summary>
    DefaultActionChanged = 0x8011,
    /// <summary>An object's accelerator changed.</summary>
    AcceleratorChanged = 0x8012,
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeAccessibleRequest
{
    internal uint Size, Version;
    internal long Token;
    internal int Operation, ChildId, Argument, X, Y, Width, Height, IntValue;
    internal uint UIntValue;
    internal byte* Buffer;
    internal int BufferLength, RequiredLength;
}

/// <summary>A Phoenix-compatible custom accessible object. Child IDs start at 1; 0 represents this object.</summary>
public abstract class Accessible
{
    private static readonly ConcurrentDictionary<long, Accessible> Registry = new();
    private static long _nextToken;
    private Window? _window;
    internal long Token { get; private set; }

    /// <summary>The window this object provides accessibility for, or null when it is not attached.</summary>
    public Window? Window => _window;
    /// <summary>Override to report how many children this object has.</summary>
    public virtual AccessibleStatus GetChildCount(out int count) { count = 0; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to report the name of a child (or this object, child 0).</summary>
    public virtual AccessibleStatus GetName(int childId, out string name) { name = string.Empty; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to report the description of a child.</summary>
    public virtual AccessibleStatus GetDescription(int childId, out string description) { description = string.Empty; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to report the help text of a child.</summary>
    public virtual AccessibleStatus GetHelpText(int childId, out string helpText) { helpText = string.Empty; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to report the value of a child.</summary>
    public virtual AccessibleStatus GetValue(int childId, out string value) { value = string.Empty; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to report the keyboard shortcut of a child.</summary>
    public virtual AccessibleStatus GetKeyboardShortcut(int childId, out string shortcut) { shortcut = string.Empty; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to report the default action name of a child.</summary>
    public virtual AccessibleStatus GetDefaultAction(int childId, out string action) { action = string.Empty; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to report the role of a child.</summary>
    public virtual AccessibleStatus GetRole(int childId, out AccessibleRole role) { role = AccessibleRole.Default; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to report the state flags of a child.</summary>
    public virtual AccessibleStatus GetState(int childId, out AccessibleState state) { state = AccessibleState.None; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to report the screen rectangle of a child.</summary>
    public virtual AccessibleStatus GetLocation(int childId, out Rect location) { location = default; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to map a screen point to the child under it.</summary>
    public virtual AccessibleStatus HitTest(Point screenPoint, out int childId) { childId = 0; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to navigate from one child in a direction to another.</summary>
    public virtual AccessibleStatus Navigate(AccessibleNavigationDirection direction, int fromId, out int toId) { toId = 0; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to change the selection or focus of a child.</summary>
    public virtual AccessibleStatus Select(int childId, AccessibleSelection selection) => AccessibleStatus.NotImplemented;
    /// <summary>Override to perform a child's default action.</summary>
    public virtual AccessibleStatus DoDefaultAction(int childId) => AccessibleStatus.NotImplemented;
    /// <summary>Override to report which child has focus.</summary>
    public virtual AccessibleStatus GetFocus(out int childId) { childId = 0; return AccessibleStatus.NotImplemented; }
    /// <summary>Override to report the currently selected children.</summary>
    public virtual AccessibleStatus GetSelections(out IReadOnlyList<int> childIds) { childIds = Array.Empty<int>(); return AccessibleStatus.NotImplemented; }

    /// <summary>Tells the platform that something about a window changed, following
    /// <c>wxAccessible.NotifyEvent</c>. Static, and takes the window, exactly as wxWidgets does.</summary>
    public static void NotifyEvent(AccessibleEvent eventType, Window window,
        AccessibleObjectType objectType = AccessibleObjectType.Client, int objectId = 0)
    {
        ArgumentNullException.ThrowIfNull(window);
        window.OwnerApp.VerifyAccess();
        NativeMethods.wxsharp_accessible_notify((int)eventType, window.Handle, (int)objectType, objectId);
    }

    /// <summary>Runs a small native query that exercises the reverse-callback bridge. Test support; not part
    /// of the wxAccessible contract.</summary>
    internal bool ValidateBridge()
    {
        var window = _window ?? throw new InvalidOperationException("The accessible object is not attached to a window.");
        window.OwnerApp.VerifyAccess();
        return NativeMethods.wxsharp_accessible_probe(window.Handle) == 0x0F;
    }

    internal void Attach(Window window)
    {
        if (_window is not null && !ReferenceEquals(_window, window))
            throw new InvalidOperationException("An Accessible instance can only be attached to one window.");
        if (Token == 0) { Token = System.Threading.Interlocked.Increment(ref _nextToken); Registry[Token] = this; }
        _window = window;
    }
    internal void Detach(Window window)
    {
        if (!ReferenceEquals(_window, window)) return;
        _window = null; if (Token != 0) Registry.TryRemove(Token, out _); Token = 0;
    }
    internal static void ClearRegistry() { Registry.Clear(); _nextToken = 0; }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static unsafe int Dispatch(NativeAccessibleRequest* request)
    {
        if (request is null || request->Version != 1 || request->Size < (uint)sizeof(NativeAccessibleRequest) ||
            !Registry.TryGetValue(request->Token, out var accessible))
            return (int)AccessibleStatus.NotImplemented;
        try
        {
            return (int)accessible.Handle(request);
        }
        catch (Exception ex)
        {
            App.Current?.RecordCallbackException(ex);
            return (int)AccessibleStatus.Fail;
        }
    }

    private unsafe AccessibleStatus Handle(NativeAccessibleRequest* request)
    {
        switch (request->Operation)
        {
            case 1: { var status = GetChildCount(out var count); request->IntValue = count; return status; }
            case 2: return WriteString(request, GetName(request->ChildId, out var name), name);
            case 3: return WriteString(request, GetDescription(request->ChildId, out var description), description);
            case 4: return WriteString(request, GetHelpText(request->ChildId, out var help), help);
            case 5: return WriteString(request, GetValue(request->ChildId, out var value), value);
            case 6: return WriteString(request, GetKeyboardShortcut(request->ChildId, out var shortcut), shortcut);
            case 7: return WriteString(request, GetDefaultAction(request->ChildId, out var action), action);
            case 8: { var status = GetRole(request->ChildId, out var role); request->IntValue = (int)role; return status; }
            case 9: { var status = GetState(request->ChildId, out var state); request->UIntValue = (uint)state; return status; }
            case 10: { var status = GetLocation(request->ChildId, out var rect); request->X = rect.X; request->Y = rect.Y; request->Width = rect.Width; request->Height = rect.Height; return status; }
            case 11: { var status = HitTest(new Point(request->X, request->Y), out var child); request->IntValue = child; return status; }
            case 12: { var status = Navigate((AccessibleNavigationDirection)request->Argument, request->ChildId, out var target); request->IntValue = target; return status; }
            case 13: return Select(request->ChildId, (AccessibleSelection)request->Argument);
            case 14: return DoDefaultAction(request->ChildId);
            case 15: { var status = GetFocus(out var child); request->IntValue = child; return status; }
            case 16:
                {
                    var status = GetSelections(out var children); if (status != AccessibleStatus.Ok) return status;
                    request->RequiredLength = checked(children.Count * sizeof(int));
                    if (request->Buffer is null || request->BufferLength < request->RequiredLength) return status;
                    var ids = new Span<int>(request->Buffer, children.Count);
                    for (var i = 0; i < ids.Length; ++i) ids[i] = children[i];
                    return status;
                }
            default: return AccessibleStatus.NotImplemented;
        }
    }

    private static unsafe AccessibleStatus WriteString(NativeAccessibleRequest* request, AccessibleStatus status, string value)
    {
        if (status != AccessibleStatus.Ok) return status;
        value ??= string.Empty;
        var length = Encoding.UTF8.GetByteCount(value); request->RequiredLength = length;
        if (request->Buffer is null || request->BufferLength <= 0) return status;
        var span = new Span<byte>(request->Buffer, request->BufferLength);
        var written = Encoding.UTF8.GetBytes(value, span[..Math.Max(0, Math.Min(length, span.Length - 1))]);
        span[written] = 0; return status;
    }
}
