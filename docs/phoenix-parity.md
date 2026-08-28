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
| Event handlers | `EvtHandler` is the base, as `wxEvtHandler` is: `Window` and `App` both derive from it, so an application can bind events of its own and a `Timer` can be owned by either. `WxEventArgs.Source` is therefore an `EvtHandler` - wx's `GetEventObject` - with `SourceWindow` for the common case. |
| Application events | Bound on the application rather than a window, because wxWidgets only ever sends them there. |
| Raising events | `Wx.PostEvent` queues a command event and `Wx.ProcessEvent` runs one immediately, following `wx.PostEvent` and `wxEvtHandler.ProcessEvent`. Only command events can be synthesised: the other classes carry state wxWidgets fills in from a real occurrence - a key event's scan code, a mouse event's position - and the type system enforces it, since both take an `EventType<CommandEventArgs>`. |
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
| Overridable virtuals | 33 members from wxPython's supported set (`etgtools/tweaker_tools.py`, `addWindowVirtuals`): `Destroy`; the focus trio; `Validate`; `TransferDataToWindow`/`FromWindow`; `InitDialog`; `GetClientAreaOrigin`; `AddChild`/`RemoveChild`; `InheritAttributes`; `ShouldInheritColours`; `OnInternalIdle`; `GetMainWindowOfCompositeControl`; `InformFirstDirection`; `SetCanFocus`; `EnableVisibleFocus`; and the protected `DoEnable`, `DoGetPosition`, `DoGetSize`, `DoGetClientSize`, `DoGetBestSize`, `DoGetBestClientSize`, `DoSetSize`, `DoSetClientSize`, `DoSetSizeHints`, `DoMoveWindow`, `DoSetWindowVariant`, `GetDefaultBorder`, `DoFreeze`, `DoThaw` and `HasTransparentBackground`. Each base implementation is wxWidgets' own answer, reached without recursive virtual dispatch. Raw child pointers are preserved when a wx-created child has no managed wrapper. |
| Virtual dispatch cost | Opt-in at construction, because C++ fixes a vtable there. Each wrapped window class builds an overriding native twin only for a managed subclass; exact framework classes retain their ordinary wxWidgets vtable. |
| Item-control virtuals | `ListCtrl` forwards Phoenix's `OnGetItemText`, `OnGetItemImage`, `OnGetItemColumnImage` and `OnGetItemIsChecked`. `TreeCtrl.OnCompareItems` is forwarded too, using a distinct wx runtime class so the MSW `SortChildren` exact-class optimization cannot bypass it. `OnGetItemAttr` remains tied to the not-yet-wrapped `wxItemAttr` type. |
| Common dialogs | `FileDialog`, `DirDialog`, `TextEntryDialog`, `NumberEntryDialog`, `ColourDialog` and `FontDialog` are real `Dialog` subclasses, as they are in wxWidgets and Phoenix - not one-shot helpers. Each can be configured before it is shown and read back afterwards: a file dialog reports its directory and file name separately, every path of a multiple selection, and which wildcard filter the user chose; a colour dialog carries the sixteen custom colours an application should persist between invocations. Being dialogs, they inherit everything `Window` offers. |
| Coverage measurement | The report's header scan honoured access specifiers regardless of nesting, so `wxWindowBase`'s nested `ChildrenRepositioningGuard` and its `private:` hid every member declared after it. `wxWindow` measured as 115/115 complete while `FindFocus`, `GetChildren`, the event-handler chain and the tab-order members were all absent. The scan now tracks brace depth, and the `SKIP` list no longer hides real API. |
| Standard paths | `StandardPaths` wraps `wxStandardPaths`, which is where an application's settings and data belong. Guessing at these gets them wrong on at least one platform, and on Windows puts them somewhere the user cannot back up. The distinction that matters is roaming versus local: config and user data follow the user between machines, local data does not, and caches belong in the local one. |
| Sound | `Sound` wraps `wxSound` - one format, no position, volume or mixing, so it suits short interface feedback and nothing else. Windows hands the path to the system without checking it, so loading and playing a file that does not exist both report success and simply never play; that is wxWidgets' behaviour and is documented on the type rather than papered over. |
| Displays | `Display` wraps `wxDisplay`. Its real use is checking a saved window position before restoring it: a window put back on a screen that is no longer attached is invisible with no way for the user to retrieve it, which `Display.GetFromPoint` returning null is how to catch. `ClientArea` rather than `Geometry` is what a window should be sized against, since it excludes the taskbar. |
| Stock art | `ArtProvider` wraps `wxArtProvider`, with `ArtId` and `ArtClient` naming the identifiers. Asking the platform for its own icon is what makes a toolbar look native, follow the user's theme, and stay legible in high contrast - none of which a shipped image does. |
| Cursors | `Window.Cursor` and the `Cursor` type close what the parity doc previously recorded as blocked. A cursor is a genuine hint about what a control does, but it says nothing to a screen reader and nothing at all to a keyboard user, so it should never be the only signal. |
| Image lists | `ImageList` wraps `wxImageList`, and `ListCtrl` and `TreeCtrl` take one. wxWidgets addresses item images by index into a list the control holds rather than per item, which is why the type exists; ownership is explicit, because a borrowed list has to outlive the control using it. |
| Carets | `Window.SetCaret` and the caret members close another recorded gap. A caret is not only the blinking line: the platform's input methods and assistive technology both follow it to know where typing will go, so a custom-drawn text control needs one. The blink rate is the user's own setting - including not at all - so it is read rather than chosen. |
| About boxes | `AboutBox` wraps `wxAboutBox`, which on some platforms is a native panel rather than a window wxWidgets draws. Filling in only the simple fields keeps that native dialog; adding developers makes wxWidgets fall back to a generic one on some platforms, which is a trade to make deliberately. |
| Rich tooltips | `Window.ShowRichToolTip` wraps `wxRichToolTip` - a title, an icon and more than one line, which is what a validation message wants and what the single-line `ToolTip` cannot carry. |
| Appearance | `App.SetAppearance` is the portable request, following `wxApp.SetAppearance`, and answers with wxWidgets' own three-way result: several platforms honour it only before the first window exists, so "too late" is a different answer from "not supported here" and worth acting on differently. |
| Dark mode | `App.EnableDarkMode` wraps `wxApp.MSWEnableDarkMode`, which goes further than the appearance request: the controls wxWidgets draws itself are themed too, not just the window frame. It is Windows-only and wxWidgets still calls it experimental, so it returns false rather than throwing where it is unavailable, and `App.SupportsDarkMode` reports the platform's claim separately so a caller can tell a refusal from a platform that has no such thing. What the user actually ended up with is `SystemSettings.IsDarkAppearance`; an interface should still take its colours from `SystemSettings.GetColour` rather than choosing them for a mode. |
| Class-specific virtuals | Members that exist on one class rather than on wxWindow are layered on the window set rather than folded into it, because a `wxButton` has no `ShouldPreventAppExit` to override. `Frame` adds `ShouldPreventAppExit`, `OnCreateStatusBar`, `OnCreateToolBar` and `DoGiveHelp`; `Dialog` adds `ShouldPreventAppExit` and `GetContentWindow`; `ScrolledWindow` adds `ShouldScrollToChildOnFocus` and `GetSizeAvailableForScrollTarget`; `Grid` adds the three grid-line pens. Each carries its own payload - a string, a window, a size or a pen - so the callback grew a text pointer and a packed colour rather than gaining a channel per class. |
| `DoGiveHelp` | Overridable, which is how menu and tool help goes somewhere other than the status bar - spoken, for instance, which is what an accessible application wants. |
| Progress dialogs and modality | `ProgressDialog` destroys itself immediately rather than scheduling it. An app-modal progress dialog holds a `wxWindowDisabler` for as long as it exists, so deferring the deletion to the next idle cycle leaves every other window disabled with nothing to say why. |
| Virtuals not wrapped | Five of wxPython's list still need a type the wrapper does not have: `SetValidator` and `GetValidator` need `wxValidator`, and `ProcessEvent`, `TryBefore` and `TryAfter` take a live `wxEvent&` while events currently cross this ABI as per-kind value snapshots. |
| Type-specific virtuals not wrapped | Phoenix also re-enables `Dialog.GetContentWindow`; `Frame.OnCreateStatusBar`, `OnCreateToolBar` and `DoGiveHelp`; the four `ScrolledWindow` drawing/auto-scroll hooks; three grid-line-pen hooks; and `TopLevelWindow.ShouldPreventAppExit`. These need dedicated per-class callback contracts rather than the common `wxWindow` request shape and remain explicit parity work. |
| Unsupported custom accessibility | Standard native accessibility stays enabled. The `wxWindow` accessibility hooks throw `NotImplementedException` where wxWidgets was built without accessibility, which is the direct analogue of the `NotImplementedError` wxPython raises there (`etg/window.py` wraps each in `wxPyRaiseNotImplemented`). |
| Native handle | `Frame.NativeHandle` exposes the wx port's native handle: HWND, GTK widget pointer, or macOS view pointer. |
| Creation styles | Every `Default` is resolved by wxWidgets natively, not composed in managed code, so it is the platform's real default - `wxDEFAULT_FRAME_STYLE`, `wxDEFAULT_DIALOG_STYLE`, `wxScrolledWindowStyle`, `wxLC_ICON`, `wxTR_DEFAULT_STYLE`, `wxTAB_TRAVERSAL`. `wxTR_DEFAULT_STYLE` in particular is a different set on Windows, GTK and macOS. |
| Fonts | `Font` is a real `wxFont` behind a handle, not a description of one, so it carries fractional and pixel sizes, the numeric weight, the encoding, strikethrough, `IsFixedWidth` and the platform's own font description - none of which the six flattened scalars it used to be could hold. `FontInfo` mirrors `wxFontInfo`. The derivations return a new font and the `Make…` forms change it in place, exactly as wxWidgets splits them; `Underlined()` is the derivation and `IsUnderlined` the property, which is how wxPython resolves the same collision (`etg/font.py`). |
| Font enum values | `FontFamily`, `FontStyle` and `FontWeight` carry wxWidgets' own values rather than wrapper-private codes, which is what let three duplicated mapping tables go. `FontWeight` is therefore the numeric 100-1000 scale, not three names. `wxFontStyle` is the trap: it borrows the deprecated `wxNORMAL`/`wxITALIC`/`wxSLANT` constants, between which `wxLIGHT` and `wxBOLD` sit, so italic is 93 and not 94. The smoke test round-trips every value through wxWidgets so a drifting one cannot pass unnoticed. |
| Font resolution | Asking for a family or style you do not get back is wxWidgets answering, not the wrapper losing it: `Default` resolves to a real family, `Teletype` and `Modern` are one family on MSW, and `Slant` is only distinct from `Italic` where a slanted face exists. |
| Text attributes | `TextAttr` carries a font handle, so `TextAttrFlags.FontStrikethrough`, `FontEncoding` and `FontPixelSize` finally mean something - the flattened form declared them but could never deliver them. |
| Canvas text | `Canvas.MeasureText` measures in the font that will draw the text: the one set by `SetTextFont` during a paint, and the control's otherwise. It used to draw on the device context while measuring on the window, so any text drawn in a canvas font was measured wrongly. |
| System fonts | `SystemSettings.GetFont` exposes the platform's own fonts, which is where a themed interface has to start rather than from a hard-coded family and size. |
| Progress dialogs | `ProgressDialog` is a `Window`, and `Update`/`Pulse` return both of the answers `wxProgressDialog` gives - whether to continue, and whether this step was skipped - as `ProgressUpdate`. Reading only one would either ignore a Cancel or mistake a Skip for an abort. Its style defaults to wxWidgets' own `wxPD_APP_MODAL | wxPD_AUTO_HIDE`; cancelling and skipping are opt-in. Destruction uses the same scheduled `wxWindow.Destroy` path as Phoenix. |
| Item data | `TreeCtrl` and `ListCtrl` hold `SetItemData`/`GetItemData` values on the managed side rather than in `wxTreeItemData`. Handing a managed object's address to C++ to keep across a garbage collection is a lifetime bug that surfaces under load; the item's own ID is a key that needs none of it, and the API is the one Phoenix has. |
| Style pass-through | No style is added behind the caller's back. A single-line `TextCtrl` does not silently gain `wxTE_PROCESS_ENTER`, and a `ListBox` with no flags gets no flags - both matching wxWidgets. |
| Accessible names | The wrapper does not set a control's name on its own. wxWidgets already reports a check box's or radio button's own label to the platform bridge; `Window.Name` follows `wxWindow.Name` for the cases where a control needs a name its label does not give it. |
| Dialog results | `Dialog.ShowModal` returns the command ID the dialog ended with, as an `int`, exactly as Phoenix does - not a two-value enum that could not represent a custom button. |
| Window destruction | `Window.Destroy` schedules deletion and leaves the window visible until it happens, as `wxWindow.Destroy` does. |
| Timer identity | `Timer` passes its ID through unchanged. As in wxWidgets, a window running more than one timer needs to give each an ID, or every handler sees every tick. |
| Sizer defaults | `BoxSizer` defaults to horizontal, which is what wxWidgets and Phoenix use. |
| Sizers | The full `wxSizer` surface: insert, prepend, detach, remove, replace, clear, show and hide by window, nested sizer or index, plus layout, fitting and minimum sizes. `wxSizerItem` is wrapped as `SizerItem` and returned by everything that adds to a sizer, so proportion, flags, border and visibility can be read back and changed. `wxBoxSizer`, `wxGridSizer`, `wxFlexGridSizer`, `wxStaticBoxSizer` and `wxGridBagSizer` are complete. |
| Sizer item identity | `SizerItem.Id` is the item's own identifier, as `wxSizerItem::GetId` is - not the window's ID, and unset until assigned. `Sizer.GetItemById` searches that; `Sizer.GetItem(Window)` is what finds an item by window. |
| Window surface | `wxWindow` is complete apart from the members needing a type the wrapper does not have yet. Coordinate spaces (`Rect`, `ClientRect`, `ScreenRect`, `ClientToScreen`, `ScreenToClient`), `Freeze`/`Thaw`, DPI scaling (`FromDip`, `ToDip`, `Dpi`), text metrics, scrolling, `Navigate`, z-order, background style, window variant and transparency all follow wxWidgets. `Close` and `Center` are on `Window`, where wxWidgets puts them, rather than only on `Frame`. |
| Text entry | `wxTextEntry` is a mix-in in wxWidgets, so it is `ITextEntry` here, implemented by `TextCtrl`, `ComboBox` and `SearchCtrl`. `SearchCtrl` derives from `Control`, not `TextCtrl`: `wxSearchCtrl` derives from `wxControl` plus the `wxTextEntry` mix-in (`wx/srchctrl.h`), and Phoenix declares it `SearchCtrl(Control)` for the same reason. So it has the text-entry surface without `wxTextCtrl`'s own members - `IsMultiLine` and the line and styling calls are not part of it. |
| `ComboBox.Clear` | Empties the item list *and* the field. `wxComboBox` inherits `Clear` from both its bases and resolves it to one method that does both, and so does this. |
| `ComboBox.SelectedText` | Reports the selected item rather than the highlighted text, which is `wxComboBox`'s own resolution of inheriting `GetStringSelection` twice. `Selection` plus `GetRange` reads the highlighted text. |
| `ChangeValue` | Sets the text without raising `TextChanged`, as `wxTextEntry::ChangeValue` does; assigning `Value` raises it. |
| Single-threaded apartment | On Windows, `App` requires the entry point to be marked `[STAThread]` and says so at startup. wxWidgets brings OLE up during initialization, and .NET otherwise starts on a multi-threaded apartment where it cannot. Python has no equivalent constraint, which is why Phoenix says nothing about it. |
| Language lookup | wxWidgets has two `FindLanguageInfo` overloads and neither takes both spellings: the string one parses the POSIX form (`pt_BR`) by splitting on `_` and `.`, and the `wxLocaleIdent` one takes the BCP 47 tag (`pt-BR`). Both are exposed, as `FindLanguageInfo` and `FindLanguageInfoByTag`; `FindLanguage` tries them in that order for a code whose spelling is not known in advance. |
| `Locale` versus `Translations` | Both load the same gettext `.mo` catalogues. `Locale` additionally sets the C runtime locale, so dates, numbers and currency follow the language too; `Translations` only translates. wxWidgets marks `wxLocale` as superseded but still ships it, and so does this. |
| Frame geometry | `wxTopLevelWindow::SaveGeometry` takes a `GeometryStore` the caller implements. The wrapper supplies one that serialises to an opaque string, so placement can go straight into whatever settings file an application already has. The contents are wxWidgets' own field names and vary by platform. |
| Frame-owned bars | `Frame.StatusBar` and `Frame.ToolBar` hand back the same wrapper each time rather than a fresh one around the same native object. A bar wxWidgets made without the wrapper knowing is adopted on first read. |
| Free functions | The namespace-scope `wx*` functions are static members of `Wx`, which is where wxPython puts them too. `wxLaunchDefaultBrowser` and `wxLaunchDefaultApplication` ask the desktop what is registered rather than naming a program, so the user's own default is honoured. |
| `Wx.GetEnv` | Returns null for an unset variable and an empty string for one set to nothing. On Windows only the first is reachable: the platform deletes a variable given an empty value rather than storing it. |
| `Wx.FindWindowByName` | Returns the wrapper that owns the window, or null when wxWidgets created it without one. Controls of the same kind share a default name and wxWidgets returns the first match, so a search is only meaningful against a name the caller set. |
| `Wx.Sleep` | Wrapped, and documented as the wrong tool: it blocks the calling thread outright, freezing the interface and anything assistive technology is reading from it. |

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

### Types not wrapped yet

Some members are absent only because the type they take or return is. Each is
recorded here rather than quietly skipped:

- `wxDropTarget` and the `wxDataObject` family — blocks drag-and-drop and any
  clipboard format beyond text, file lists and bitmaps, which `Clipboard`
  offers directly. `DropFiles` covers the common drop case without them.
- `wxPalette` — blocks the palette accessors; only relevant on paletted displays.
- `wxHelpProvider` — `Window.HelpText` goes to a help provider, and wxWidgets
  installs none by default, so it currently keeps nothing.
- `wxAcceleratorTable` as an object — accelerators are set from an array, and
  the installed table cannot be read back.

`FontEnumerator` wraps `wxFontEnumerator`'s statics - `GetFacenames`, `GetEncodings`,
`IsValidFacename` and `InvalidateCache`. wxWidgets also shapes it as a class to
derive from, with `OnFacename` and `OnFontEncoding` callbacks that collect
results; the statics do that collecting already, and are the form wxPython
recommends, so only they are wrapped. This is the one way to know a face exists
before asking for it: assigning an unavailable `Font.FaceName` leaves the font
unchanged, and only `Font.TrySetFaceName` reports it afterwards.

`Wx` carries the wxWidgets free functions: launching a URL or a file with the
user's own default program, running a command, the system bell, key and mouse
state, who and where the machine is, the OS and CPU description, environment
variables, window lookup, `wxWindowDisabler` as a scope, and `StripMenuCodes`
for showing a menu label anywhere outside a menu. Process and system
termination - `wxKill` and `wxShutdown` - are not wrapped.

`Frame` is complete against `wxFrame` and `wxTopLevelWindow`: window state, the title-bar buttons, full
screen, user attention, icon bundles, the frame-owned menu, status and tool bars, and geometry persistence.
`Locale` and `Translations` are complete against `wxLocale` and `wxTranslations`, and `Language` is generated
from `wx/language.h` so its 912 values stay in step with the header rather than being transcribed. Because
wxWidgets reads GNU gettext catalogues, a project already shipping a
`locale/<lang>/LC_MESSAGES/<domain>.mo` tree needs no conversion.

`Colour` is complete against `wxColour`, including name and `#RRGGBB` parsing
and the transforms a themed interface derives its palette with — lightening,
disabling, greyscale and alpha blending are all computed by wxWidgets rather
than reimplemented, so they match exactly. `IsOk` is always true: `wxColour` has
an uninitialised state to guard against and a value type has none.

`Clipboard` covers `wxClipboard` for text, file lists and bitmaps, including
holding it open across several operations and `Flush` so content outlives the
process. `SystemSettings` covers `wxSystemSettings`, which is what a theme-aware
and high-contrast-safe interface has to read its colours from. `TextAttr` covers
`wxTextAttr`, keeping its set-or-inherit model: a style overrides only what it
was given.

Single-instance detection and inter-process messaging are deliberately not
wrapped. On Windows `wxSingleInstanceChecker` is `CreateMutex` plus a test for
`ERROR_ALREADY_EXISTS`, which `System.Threading.Mutex` already gives; and
`wxServer`/`wxClient`/`wxConnection` resolve to DDE there, because `wx/ipc.h`
defaults `wxUSE_DDE_FOR_IPC` to 1 on Windows and the TCP classes are not
compiled in at all. A named pipe through `System.IO.Pipes` is both simpler and
closer to what such an application actually needs, and neither wxWidgets class
would be carrying its weight. Command-event propagation and
consumption are covered by the smoke test, which raises the events it needs
with `Wx.ProcessEvent` and `Wx.PostEvent`.

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
