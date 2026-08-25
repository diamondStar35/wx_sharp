# wxPython Phoenix parity

Phoenix is the behavioral reference for WxSharp, but the projects do not yet
have the same API surface. Phoenix wraps most of wxWidgets; WxSharp currently
wraps a focused set of windows, dialogs, controls, sizers, drawing operations,
events, and services.

This document distinguishes behavioral compatibility from API completeness.

## Implemented behavior

| Area | WxSharp behavior |
|---|---|
| Strings | The native boundary is UTF-8 on every platform; managed strings round-trip without relying on the platform width of `wchar_t`. |
| Standard controls | Controls remain native wxWidgets controls and therefore use the native MSW, GTK, or macOS accessibility bridge, as Phoenix does. |
| Custom accessibility availability | Reported by `Wx.SupportsCustomAccessibility`. This follows Phoenix and wxWidgets: custom `wxAccessible` objects are available with `wxUSE_ACCESSIBILITY`, currently on MSW. |
| Accessible roles | All Phoenix/wxWidgets `wxAccRole` values are represented by `AccessibleRole`. |
| Accessible states | All Phoenix/wxWidgets state flags are represented by `AccessibleState`. |
| Accessible metadata | Name, role, description, help, value, keyboard shortcut, default-action name, and state can be supplied to the custom accessible object. |
| Accessibility events | Metadata updates emit the corresponding wxWidgets name, description, help, value, accelerator, default-action, or state notification. |
| Unsupported custom accessibility | Standard native accessibility stays enabled. APIs requiring a custom `wxAccessible` object report `PlatformNotSupportedException`, matching Phoenix's not-implemented fallback rather than silently succeeding. |
| Native handle | `Window.NativeHandle` exposes the wx port's native handle: HWND, GTK widget pointer, or macOS view pointer. |

## Not yet wrapped

Phoenix's custom `wxAccessible` class also permits application-defined child
trees and overrides for hit testing, location, navigation, child and parent
lookup, selection, focus, and invoking a default action. WxSharp does not yet
expose those callback-driven features.

More generally, controls and modules absent from `src/WxSharp` are not implied
to be implemented merely because they exist in Phoenix. New wrappers should
follow Phoenix's defaults, ownership rules, event propagation, and
platform-specific not-implemented behavior, with tests added to this parity
matrix.

## Reference files

The most relevant Phoenix definitions are:

- `etg/access.py`, which exposes the virtual `wxAccessible` contract and
  generates not-implemented stubs when `wxUSE_ACCESSIBILITY` is disabled.
- `etg/window.py`, which exposes `GetAccessible`, `GetOrCreateAccessible`,
  `CreateAccessible`, and `SetAccessible`, with the non-MSW fallback.
- wxWidgets `include/wx/access.h`, which defines roles, states, statuses,
  selection flags, events, and the overridable accessible-object behavior.
