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
| Events | One wrapper event ID per wxWidgets event type, resolved through a table in `events.cpp`. Typed `event` members and `Window.Bind`/`Unbind` share a single subscriber list; both support ID and ID-range filtering without reflection. |
| Event hooking | Lazy. An event is connected natively on its first subscriber and disconnected on its last, so nothing crosses the boundary unobserved. A handful - window destruction, canvas paints, timer ticks - are reported unconditionally because the native side owns when they happen. |
| Handling and skipping | wxWidgets' model exactly: an event is handled, and stops, unless a handler calls `Skip()`. Every event is treated the same way; none is special-cased. A second handler on the same window and ID runs only if the first skipped. |
| Event propagation | wxWidgets'. A skipped command event travels up the real parent chain, as in Phoenix; the wrapper does not re-dispatch to parents itself. |
| Command state | `wxEVT_UPDATE_UI` is wrapped as the event Phoenix exposes, with `UpdateUIEventArgs` carrying `Enabled`, `Checked`, `Shown` and `Text` as Phoenix's added properties do, and `SetUpdateInterval`/`SetMode` as statics. Answering it requires no special handling: a handler that does not skip is handled, which is what wxWidgets needs before it applies the answer. |
| Menu lifecycle | `Frame.MenuOpened`, `MenuClosed` and `MenuHighlighted` follow `wxEVT_MENU_OPEN`/`_CLOSE`/`_HIGHLIGHT`, so a dynamic menu can be rebuilt before it is shown and item help can drive a status bar. |
| Event coverage | 145 of the 249 `wxEVT_*` types wxWidgets declares. The absent ones are the gesture, touch, stylus and joystick families, palette and session events, and the grid events, none of which the wrapped widgets need. |
| Vetoing | `NotifyEventArgs.Veto()` on every event wxWidgets lets a handler refuse - closes, book-control page changes, tree expansion and selection, list label edits, splitter double-clicks. `CloseEventArgs.CanCancel` reports when a close cannot be refused. |
| Menus | `Menu` holds `MenuItem` objects with label, help, kind, enabled and check state; submenus, insertion, removal and lookup follow `wxMenu`. `MenuBar` supports insert, remove, top-level labels and enabling. `Window.PopupMenu` shows a context menu, and `WxEvents.ContextMenu` reports keyboard invocation separately from a right-click. |
| Identifiers | `StandardId` exposes the `wxID_*` stock identifiers, so stock items keep the platform label, icon, accelerator and macOS menu placement. `IdManager.NewId`/`Release` matches `wx.NewIdRef` reservation. |
| Accelerators | `AcceleratorEntry.TryParse`/`ToString` use `wxAcceleratorEntry`, so the accepted syntax is wxWidgets' own. Tables install on any window through `Window.SetAcceleratorTable`, not only frames. |
| Key codes | `Key` names every `WXK_*` value. The enum is generated from the wxWidgets headers, so the codes are the platform's own rather than a transcription. |
| Keyboard events | `CharHook`, `KeyDown`, `Char` and `KeyUp` are separate events with Phoenix's semantics: char-hook reaches a top-level window before the focused control, and `Char` reports the translated character. `KeyEventArgs` carries the modifier set including Meta and RawControl, plus `UnicodeKey` and `RawKeyCode`. |
| Creation styles | `FrameStyle`, `DialogStyle`, `PanelStyle`, `ScrolledStyle`, `ListCtrlStyle`, `TreeCtrlStyle` and `FileDialogStyle` are semantic enums translated to wx flags natively, so no wx constants leak into managed code. Each carries a `Default` matching the wxWidgets default. |
| Sizer flags | The full set: expand, the six alignment flags, shaped, fixed-min-size and reserve-space-even-if-hidden, plus per-edge borders. wxWidgets' own run-time check for alignment on the wrong axis still applies. |
| Standard dialog buttons | `Dialog.CreateButtonSizer` returns the platform's button row, so order, spacing, the default button and the order a screen reader reads them in are wxWidgets' rather than the caller's. |
| File dialogs | `FileDialog` carries styles, a default directory and file name, and multiple selection. The native side holds the result set until the next call, so a multiple selection is not truncated by a caller-sized buffer. |
| List and tree controls | Selection and keyboard focus are distinct, as in wxWidgets: `ListCtrl.SetFocused` moves what assistive technology follows without changing the selection. Columns, label editing, activation, expansion and key events are all exposed; the vetoable ones carry `Veto()`. |
| Strings | The native boundary is UTF-8 on every platform; managed strings round-trip without relying on the platform width of `wchar_t`. |
| Standard controls | Controls remain native wxWidgets controls and therefore use the native MSW, GTK, or macOS accessibility bridge, as Phoenix does. |
| Custom accessibility availability | Reported by `Wx.SupportsCustomAccessibility`. This follows Phoenix and wxWidgets: custom `wxAccessible` objects are available with `wxUSE_ACCESSIBILITY`, currently on MSW. |
| Accessible roles | All Phoenix/wxWidgets `wxAccRole` values are represented by `AccessibleRole`. |
| Accessible states | All Phoenix/wxWidgets state flags are represented by `AccessibleState`. |
| Accessibility surface | Exactly what wxPython exposes: the `wxAccessible` contract as `Accessible`, and `wxWindow`'s four hooks - `Accessible` (get/set), `GetOrCreateAccessible`, and an overridable `CreateAccessible`. Nothing more; there are no per-property shortcuts, because there are none in wxPython. |
| Accessible names | The wrapper sets no control's name on its own. wxWidgets attaches no `wxAccessible` to a control by default, which is what lets the platform's own provider report a check box's or button's label - so `Window.Name` sets the window name and nothing else, and never attaches one. |
| Accessibility events | `Accessible.NotifyEvent` is static and takes the window, following `wxAccessible.NotifyEvent`. |
| Custom accessible objects | Derive from `Accessible` and assign `Window.Accessible` to provide virtual children, string properties, roles, states, screen locations, hit testing, navigation, selection, focus, and default actions through Native AOT-safe reverse callbacks. |
| Unsupported custom accessibility | Standard native accessibility stays enabled. The `wxWindow` accessibility hooks throw `NotImplementedException` where wxWidgets was built without accessibility, which is the direct analogue of the `NotImplementedError` wxPython raises there (`etg/window.py` wraps each in `wxPyRaiseNotImplemented`). |
| Native handle | `Frame.NativeHandle` exposes the wx port's native handle: HWND, GTK widget pointer, or macOS view pointer. |
| Creation styles | Every `Default` is resolved by wxWidgets natively, not composed in managed code, so it is the platform's real default - `wxDEFAULT_FRAME_STYLE`, `wxDEFAULT_DIALOG_STYLE`, `wxScrolledWindowStyle`, `wxLC_ICON`, `wxTR_DEFAULT_STYLE`, `wxTAB_TRAVERSAL`. `wxTR_DEFAULT_STYLE` in particular is a different set on Windows, GTK and macOS. |
| Style pass-through | No style is added behind the caller's back. A single-line `TextCtrl` does not silently gain `wxTE_PROCESS_ENTER`, and a `ListBox` with no flags gets no flags - both matching wxWidgets. |
| Accessible names | The wrapper does not set a control's name on its own. wxWidgets already reports a check box's or radio button's own label to the platform bridge; `Window.Name` follows `wxWindow.Name` for the cases where a control needs a name its label does not give it. |
| Dialog results | `Dialog.ShowModal` returns the command ID the dialog ended with, as an `int`, exactly as Phoenix does - not a two-value enum that could not represent a custom button. |
| Window destruction | `Window.Destroy` schedules deletion and leaves the window visible until it happens, as `wxWindow.Destroy` does. |
| Timer identity | `Timer` passes its ID through unchanged. As in wxWidgets, a window running more than one timer needs to give each an ID, or every handler sees every tick. |
| Sizer defaults | `BoxSizer` defaults to horizontal, which is what wxWidgets and Phoenix use. |

## Current coverage boundary

WxSharp now covers common controls, menus and frame chrome, notebook/splitter/
scrolled containers, timers, list/tree/grid controls, and a concrete data-view
list store. Typed events remain the preferred C# API, with `Bind` available for
dynamic event selection.

The event catalogue in `WxEvents` follows Phoenix's `wx.EVT_*` naming and covers
window lifecycle and geometry, the full mouse set, the four keyboard events
(`CharHook`, `KeyDown`, `KeyUp`, `Char`), control commands, book controls, and
the list, tree, data-view, splitter and grid events. Adding another is one row
in the table in `events.cpp` and one `EventType<T>` here.

Run `python scripts/coverage-report.py` for a measured diff of the managed
surface against the wxWidgets headers, per type. It is the authority on what is
missing; the notes here describe behaviour rather than completeness.

### Deliberate differences

Two, both recorded rather than hidden:

- `App.MainLoop()` returns 0 when no top-level window exists, instead of
  blocking forever as wxWidgets would. A hang is a worse answer than a return.
- `CustomSlider` has no wxWidgets counterpart. It is a control built on the
  wrapper out of events wxWidgets already raises, the way an application would
  build one - not a claimed wx class.

The remaining gap in the `wxAccessible` contract is `GetChild` and `GetParent`,
which return accessible *objects* rather than child IDs. Bridging those needs a
token protocol the reverse callback does not have yet; the virtual-children
model (`GetChildCount` plus ID-addressed getters) is complete.

Known gaps in this area: there is no equivalent of `wx.PostEvent` for
synthesising a command, no `wxLocale` wrapper for wxWidgets' own stock strings,
and clipboard support is text-only. Command-event propagation and vetoing are
implemented but are not covered by an automated test, because triggering either
needs a synthesised event the smoke test cannot produce.

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
