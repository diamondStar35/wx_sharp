# wxPython Phoenix parity

Phoenix is the behavioral reference for WxSharp, but the projects do not yet
have the same API surface. Phoenix wraps most of wxWidgets; WxSharp currently
wraps a focused set of windows, dialogs, controls, sizers, drawing operations,
events, and services.

This document distinguishes behavioral compatibility from API completeness.

## Implemented behavior

| Area | WxSharp behavior |
|---|---|
| Application lifecycle | Constructing `App` initializes wxWidgets; `App.MainLoop()` enters wxWidgets' real blocking event loop and exits normally when the final frame closes. No manual message pump is required. |
| Initialization hooks | Subclass `App` and override `OnInit`/`OnExit`, or create windows before calling `MainLoop`, matching Phoenix's high-level application model without invoking virtual methods from a C# constructor. |
| Queued UI work | `Wx.CallAfter` posts managed work to the wx event queue and may be called from worker threads. |
| Window ownership and layout | `Frame`, `Dialog`, `Panel`, and controls follow native parent ownership. Panels and sizers are explicit; the wrapper creates no hidden content panel or automatic vertical stack. |
| Events | C# `EventHandler<TEventArgs>` events carry native IDs and typed payloads. `Handled` controls native processing, command events propagate when unhandled, and `CloseEventArgs.Cancel` vetoes permitted closes. `Window.Bind`/`Unbind` adds Phoenix-style event types with ID and ID-range filtering without reflection. |
| Strings | The native boundary is UTF-8 on every platform; managed strings round-trip without relying on the platform width of `wchar_t`. |
| Standard controls | Controls remain native wxWidgets controls and therefore use the native MSW, GTK, or macOS accessibility bridge, as Phoenix does. |
| Custom accessibility availability | Reported by `Wx.SupportsCustomAccessibility`. This follows Phoenix and wxWidgets: custom `wxAccessible` objects are available with `wxUSE_ACCESSIBILITY`, currently on MSW. |
| Accessible roles | All Phoenix/wxWidgets `wxAccRole` values are represented by `AccessibleRole`. |
| Accessible states | All Phoenix/wxWidgets state flags are represented by `AccessibleState`. |
| Accessible metadata | Name, role, description, help, value, keyboard shortcut, default-action name, and state can be supplied to the custom accessible object. |
| Accessibility events | Metadata updates emit the corresponding wxWidgets name, description, help, value, accelerator, default-action, or state notification. |
| Custom accessible objects | Derive from `Accessible` and assign `Window.Accessible` to provide virtual children, string properties, roles, states, screen locations, hit testing, navigation, selection, focus, and default actions through Native AOT-safe reverse callbacks. |
| Unsupported custom accessibility | Standard native accessibility stays enabled. APIs requiring a custom `wxAccessible` object report `PlatformNotSupportedException`, matching Phoenix's not-implemented fallback rather than silently succeeding. |
| Native handle | `Frame.NativeHandle` exposes the wx port's native handle: HWND, GTK widget pointer, or macOS view pointer. |

## Current coverage boundary

WxSharp now covers common controls, menus and frame chrome, notebook/splitter/
scrolled containers, timers, list/tree/grid controls, and a concrete data-view
list store. Typed events remain the preferred C# API, with `Bind` available for
dynamic event selection.

Advanced Phoenix modules such as AUI, ribbon, property grid, rich text, STC,
HTML/webview, media, OpenGL, printing, custom grid tables, and custom data-view
models are not part of this coverage phase. New wrappers should continue to
follow Phoenix defaults, ownership, propagation, and platform-specific
not-implemented behavior.

## Reference files

The most relevant Phoenix definitions are:

- `etg/access.py`, which exposes the virtual `wxAccessible` contract and
  generates not-implemented stubs when `wxUSE_ACCESSIBILITY` is disabled.
- `etg/window.py`, which exposes `GetAccessible`, `GetOrCreateAccessible`,
  `CreateAccessible`, and `SetAccessible`, with the non-MSW fallback.
- wxWidgets `include/wx/access.h`, which defines roles, states, statuses,
  selection flags, events, and the overridable accessible-object behavior.
