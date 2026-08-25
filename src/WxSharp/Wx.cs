using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WxSharp;

/// <summary>The managed entry point to the native wxWidgets toolkit. Initialise once on the UI thread, pump
/// once per host-loop tick (or wait), and shut down on exit.</summary>
public static class Wx
{
    /// <summary>Whether this wxWidgets build exposes Phoenix-compatible custom accessibility objects.
    /// Standard controls retain platform-native accessibility when this is false.</summary>
    public static bool SupportsCustomAccessibility => NativeMethods.wxsharp_custom_accessibility_available();

    // id -> target, so the single native event/key callbacks can route to the right window/control. Touched
    // only on the UI thread (the pump runs there), so no locking is needed.
    private static readonly System.Collections.Generic.Dictionary<int, EventTarget> Targets = new();
    private static int _nextId = 1;
    private static bool _initialized;

    internal static int Register(EventTarget target)
    {
        var id = _nextId++;
        Targets[id] = target;
        return id;
    }

    internal static void Unregister(int id) => Targets.Remove(id);

    /// <summary>Brings wxWidgets up and wires the event/key callbacks. Idempotent; false if it failed.</summary>
    public static unsafe bool Init()
    {
        if (_initialized)
            return true;
        if (!NativeMethods.wxsharp_init())
            return false;
        NativeMethods.wxsharp_set_event_handler(&Dispatch);
        NativeMethods.wxsharp_set_key_handler(&DispatchKey);
        _initialized = true;
        return true;
    }

    /// <summary>Processes pending native events without blocking; call once per host-loop tick.</summary>
    public static void Pump() => NativeMethods.wxsharp_pump();

    /// <summary>Blocks until input arrives or up to <paramref name="timeoutMs"/> (negative = forever), so a
    /// UI-driven host loop idles without polling and wakes the instant input arrives. Call after Pump.</summary>
    public static void Wait(int timeoutMs) => NativeMethods.wxsharp_wait(timeoutMs);

    /// <summary>Shows a native, accessible message box and returns the button pressed.</summary>
    public static MessageBoxStyle MessageBox(string message, string caption, MessageBoxStyle style = MessageBoxStyle.Ok)
        => (MessageBoxStyle)NativeMethods.wxsharp_message_box(message, caption, (int)style);

    /// <summary>Tears wxWidgets down. Safe to call even if never initialised.</summary>
    public static void Shutdown()
    {
        if (!_initialized)
            return;
        NativeMethods.wxsharp_shutdown();
        _initialized = false;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void Dispatch(int id, int evt)
    {
        if (Targets.TryGetValue(id, out var target))
            target.OnNativeEvent((EventKind)evt);
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static byte DispatchKey(int id, int kind, int keyCode, int modifiers)
    {
        if (!Targets.TryGetValue(id, out var target))
            return 0;

        var args = new KeyEventArgs(
            keyCode,
            control: (modifiers & 1) != 0,
            shift: (modifiers & 2) != 0,
            alt: (modifiers & 4) != 0);

        // A char-hook comes from a top-level window; key down/up come from a focused control.
        bool consumed = (KeyKind)kind switch
        {
            KeyKind.Hook => target is Container container && container.RaiseKeyDown(args),
            KeyKind.Down => target is Control down && down.RaiseKeyDown(args),
            KeyKind.Up => target is Control up && up.RaiseKeyUp(args),
            _ => false,
        };
        return consumed ? (byte)1 : (byte)0;
    }
}
