using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WxSharp;

public enum AccessibleStatus { Fail = 0, False = 1, Ok = 2, NotImplemented = 3, NotSupported = 4, InvalidArgument = 5 }
public enum AccessibleNavigationDirection { Down, FirstChild, LastChild, Left, Next, Previous, Right, Up }
[Flags]
public enum AccessibleSelection { None = 0, TakeFocus = 1, TakeSelection = 2, ExtendSelection = 4, AddSelection = 8, RemoveSelection = 16 }
public enum AccessibleObjectType
{
    Window = 0, SystemMenu = -1, TitleBar = -2, Menu = -3, Client = -4, VerticalScrollBar = -5,
    HorizontalScrollBar = -6, SizeGrip = -7, Caret = -8, Cursor = -9, Alert = -10, Sound = -11,
}
public enum AccessibleEvent
{
    Create = 0x8000, Destroy = 0x8001, Show = 0x8002, Hide = 0x8003, Reorder = 0x8004,
    Focus = 0x8005, Selection = 0x8006, SelectionAdd = 0x8007, SelectionRemove = 0x8008,
    SelectionWithin = 0x8009, StateChanged = 0x800A, LocationChanged = 0x800B,
    NameChanged = 0x800C, DescriptionChanged = 0x800D, ValueChanged = 0x800E,
    ParentChanged = 0x800F, HelpChanged = 0x8010, DefaultActionChanged = 0x8011,
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

    public Window? Window => _window;
    public virtual AccessibleStatus GetChildCount(out int count) { count = 0; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus GetName(int childId, out string name) { name = string.Empty; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus GetDescription(int childId, out string description) { description = string.Empty; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus GetHelpText(int childId, out string helpText) { helpText = string.Empty; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus GetValue(int childId, out string value) { value = string.Empty; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus GetKeyboardShortcut(int childId, out string shortcut) { shortcut = string.Empty; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus GetDefaultAction(int childId, out string action) { action = string.Empty; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus GetRole(int childId, out AccessibleRole role) { role = AccessibleRole.Default; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus GetState(int childId, out AccessibleState state) { state = AccessibleState.None; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus GetLocation(int childId, out Rect location) { location = default; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus HitTest(Point screenPoint, out int childId) { childId = 0; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus Navigate(AccessibleNavigationDirection direction, int fromId, out int toId) { toId = 0; return AccessibleStatus.NotImplemented; }
    public virtual AccessibleStatus Select(int childId, AccessibleSelection selection) => AccessibleStatus.NotImplemented;
    public virtual AccessibleStatus DoDefaultAction(int childId) => AccessibleStatus.NotImplemented;
    public virtual AccessibleStatus GetFocus(out int childId) { childId = 0; return AccessibleStatus.NotImplemented; }
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
