// Shared internals for the wxsharp implementation files. Not part of the public ABI.
#pragma once

#include <wx/wx.h>
#include <wx/dataview.h>
#include <wx/filedlg.h>
#include <wx/listctrl.h>
#include <wx/grid.h>
#include <wx/progdlg.h>
#include <wx/scrolwin.h>
#include <wx/treectrl.h>
#include "wxsharp.h"
#include <algorithm>
#include <climits>
#include <cstring>
#include <utility>
#include <vector>

// Reports one event to the managed callback. Used for the handful of events the table in events.cpp does
// not own - window destruction, canvas paints, timer ticks and CallAfter - where the native side decides
// when to fire. Everything else goes through EventSink there.
extern wxsharp_event_cb g_event_cb;

inline unsigned int Fire(long long token, int kind, int id = wxID_ANY, int x = 0, int y = 0,
                         int width = 0, int height = 0, int keyCode = 0, int modifiers = 0,
                         int mouseButton = 0, int wheelDelta = 0, bool active = false, bool canVeto = false)
{
    if (!g_event_cb)
        return 0;
    wxsharp_event eventData = {};
    eventData.size = sizeof(eventData);
    eventData.version = WXSHARP_EVENT_VERSION;
    eventData.token = token;
    eventData.kind = kind;
    eventData.id = id;
    eventData.x = x;
    eventData.y = y;
    eventData.width = width;
    eventData.height = height;
    eventData.key_code = keyCode;
    eventData.modifiers = modifiers;
    eventData.mouse_button = mouseButton;
    eventData.wheel_delta = wheelDelta;
    eventData.active = active ? 1 : 0;
    eventData.can_veto = canVeto ? 1 : 0;
    return g_event_cb(&eventData);
}

// Packs the modifier state of a key or mouse event into the bitfield the managed side expects. RawControl
// is Ctrl everywhere except macOS, where Control and Command are distinct and Ctrl maps to Command.
inline int Mods(const wxKeyboardState& e)
{
    return (e.ControlDown() ? WXSHARP_MOD_CONTROL : 0)
         | (e.ShiftDown() ? WXSHARP_MOD_SHIFT : 0)
         | (e.AltDown() ? WXSHARP_MOD_ALT : 0)
         | (e.MetaDown() ? WXSHARP_MOD_META : 0)
         | (e.RawControlDown() ? WXSHARP_MOD_RAW_CONTROL : 0);
}

// Releases every lazily-created event binding on a window. Defined in events.cpp; called when the window is
// destroyed, after the managed side has been told, so no sink outlives the window it is connected to.
void WxSharpReleaseBindings(wxWindow* window);

// The one event hook every window carries. Destruction has to be observed unconditionally: it is what tells
// the managed Window its handle is gone and what releases the window's other bindings. Everything else is
// bound on demand through wxsharp_window_bind().
inline void TrackWindow(wxWindow* window, long long token)
{
    window->Bind(wxEVT_DESTROY, [window, token](wxWindowDestroyEvent& e)
    {
        // wxWindowDestroyEvent propagates, so a parent sees its children being destroyed too.
        if (e.GetEventObject() == window)
        {
            Fire(token, WXSHARP_EV_DESTROY, e.GetId());
            WxSharpReleaseBindings(window);
        }
        e.Skip();
    });
}

// Tracks a freshly constructed window and hands it straight back, so a create function stays one
// expression. It returns T* rather than a plain handle so the caller can keep calling typed members on it -
// wxGrid::CreateGrid, for instance, which has to run after construction.
template<typename T> T* Common(T* control, long long token)
{
    TrackWindow(control, token);
    return control;
}

// ---- Overridable virtuals -------------------------------------------------------------------------------
// The channel a managed subclass answers wxWidgets' virtual members through. Installed by
// wxsharp_set_virtual_handler(); null until an App is constructed, and null in a process that never
// subclasses anything, in which case every window below falls through to wxWidgets' own implementation.
extern wxsharp_virtual_cb g_virtual_cb;
extern wxsharp_virtual_list_cb g_virtual_list_cb;

// Lets a managed override reach wxWidgets' own answer. "Calling the base implementation" has to mean the
// C++ base, reached without virtual dispatch - going back through the virtual would land in the managed
// override again, so the override would re-enter itself and never see wxWidgets' answer at all. Every
// window built by Overridable below implements this.
class OverridableWindow
{
public:
    virtual ~OverridableWindow() = default;
    virtual void CallBase(wxsharp_virtual_request& request) = 0;
};

// Mixed into a wx class to make the members wxPython supports overriding (etgtools/tweaker_tools.py,
// addWindowVirtuals) answerable from managed code. Each forwards the question and runs the base
// implementation when managed code declines, so a subclass overriding one member keeps wxWidgets'
// behaviour for the rest.
//
// The base constructor runs before m_token is set; that is safe, because C++ has not installed this vtable
// yet, so nothing here can be reached from it.
template <class Base>
class Overridable : public Base, public OverridableWindow
{
public:
    template <typename... Args>
    Overridable(long long token, Args&&... args)
        : Base(std::forward<Args>(args)...), m_token(token) {}

    // ---- Public virtuals --------------------------------------------------------------------------------

    bool AcceptsFocus() const override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_ACCEPTS_FOCUS, r) ? r.result != 0 : Base::AcceptsFocus();
    }

    bool AcceptsFocusFromKeyboard() const override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_ACCEPTS_FOCUS_FROM_KEYBOARD, r) ? r.result != 0
                                                                : Base::AcceptsFocusFromKeyboard();
    }

    bool AcceptsFocusRecursively() const override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_ACCEPTS_FOCUS_RECURSIVELY, r) ? r.result != 0
                                                              : Base::AcceptsFocusRecursively();
    }

    bool Validate() override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_VALIDATE, r) ? r.result != 0 : Base::Validate();
    }

    bool TransferDataToWindow() override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_TRANSFER_TO_WINDOW, r) ? r.result != 0 : Base::TransferDataToWindow();
    }

    bool TransferDataFromWindow() override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_TRANSFER_FROM_WINDOW, r) ? r.result != 0 : Base::TransferDataFromWindow();
    }

    void InitDialog() override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_INIT_DIALOG, r)) Base::InitDialog();
    }

    wxPoint GetClientAreaOrigin() const override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_CLIENT_AREA_ORIGIN, r) ? wxPoint(r.x, r.y) : Base::GetClientAreaOrigin();
    }

    void AddChild(wxWindowBase* child) override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_ADD_CHILD, r, AsHandle(child))) Base::AddChild(child);
    }

    void RemoveChild(wxWindowBase* child) override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_REMOVE_CHILD, r, AsHandle(child))) Base::RemoveChild(child);
    }

    void InheritAttributes() override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_INHERIT_ATTRIBUTES, r)) Base::InheritAttributes();
    }

    bool ShouldInheritColours() const override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_SHOULD_INHERIT_COLOURS, r) ? r.result != 0 : Base::ShouldInheritColours();
    }

    void OnInternalIdle() override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_ON_INTERNAL_IDLE, r)) Base::OnInternalIdle();
    }

    wxWindow* GetMainWindowOfCompositeControl() override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_MAIN_WINDOW_OF_COMPOSITE, r))
            return Base::GetMainWindowOfCompositeControl();
        return static_cast<wxWindow*>(AsWindow(r.handle));
    }

    bool InformFirstDirection(int direction, int size, int availableOtherDir) override
    {
        wxsharp_virtual_request r;
        const int args[3] = { direction, size, availableOtherDir };
        return Ask(WXSHARP_VIRT_INFORM_FIRST_DIRECTION, r, 0, args, 3)
            ? r.result != 0
            : Base::InformFirstDirection(direction, size, availableOtherDir);
    }

    void SetCanFocus(bool canFocus) override
    {
        wxsharp_virtual_request r;
        const int args[1] = { canFocus ? 1 : 0 };
        if (!Ask(WXSHARP_VIRT_SET_CAN_FOCUS, r, 0, args, 1)) Base::SetCanFocus(canFocus);
    }

    void EnableVisibleFocus(bool enabled) override
    {
        wxsharp_virtual_request r;
        const int args[1] = { enabled ? 1 : 0 };
        if (!Ask(WXSHARP_VIRT_ENABLE_VISIBLE_FOCUS, r, 0, args, 1)) Base::EnableVisibleFocus(enabled);
    }

    bool Destroy() override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_DESTROY, r) ? r.result != 0 : Base::Destroy();
    }

    // ---- The base-call side -----------------------------------------------------------------------------
    // Runs wxWidgets' own implementation of one member. Reached only from a managed override asking for it,
    // and never dispatches, so an override calling base cannot re-enter itself.
    void CallBase(wxsharp_virtual_request& r) override
    {
        r.handled = 1;
        switch (r.which)
        {
            case WXSHARP_VIRT_ACCEPTS_FOCUS:               r.result = Base::AcceptsFocus(); break;
            case WXSHARP_VIRT_ACCEPTS_FOCUS_FROM_KEYBOARD: r.result = Base::AcceptsFocusFromKeyboard(); break;
            case WXSHARP_VIRT_ACCEPTS_FOCUS_RECURSIVELY:   r.result = Base::AcceptsFocusRecursively(); break;
            case WXSHARP_VIRT_VALIDATE:                    r.result = Base::Validate(); break;
            case WXSHARP_VIRT_TRANSFER_TO_WINDOW:          r.result = Base::TransferDataToWindow(); break;
            case WXSHARP_VIRT_TRANSFER_FROM_WINDOW:        r.result = Base::TransferDataFromWindow(); break;
            case WXSHARP_VIRT_INIT_DIALOG:                 Base::InitDialog(); break;
            case WXSHARP_VIRT_INHERIT_ATTRIBUTES:          Base::InheritAttributes(); break;
            case WXSHARP_VIRT_SHOULD_INHERIT_COLOURS:      r.result = Base::ShouldInheritColours(); break;
            case WXSHARP_VIRT_ON_INTERNAL_IDLE:            Base::OnInternalIdle(); break;
            case WXSHARP_VIRT_DO_FREEZE:                   Base::DoFreeze(); break;
            case WXSHARP_VIRT_DO_THAW:                     Base::DoThaw(); break;
            case WXSHARP_VIRT_HAS_TRANSPARENT_BACKGROUND:  r.result = Base::HasTransparentBackground(); break;
            case WXSHARP_VIRT_DEFAULT_BORDER:              r.result = Base::GetDefaultBorder(); break;
            case WXSHARP_VIRT_DESTROY:                     r.result = Base::Destroy(); break;

            case WXSHARP_VIRT_CLIENT_AREA_ORIGIN:
            {
                const wxPoint origin = Base::GetClientAreaOrigin();
                r.x = origin.x; r.y = origin.y;
                break;
            }
            case WXSHARP_VIRT_BEST_SIZE:
            {
                const wxSize best = Base::DoGetBestSize();
                r.x = best.x; r.y = best.y;
                break;
            }
            case WXSHARP_VIRT_BEST_CLIENT_SIZE:
            {
                const wxSize best = Base::DoGetBestClientSize();
                r.x = best.x; r.y = best.y;
                break;
            }
            case WXSHARP_VIRT_DO_GET_POSITION:    Base::DoGetPosition(&r.x, &r.y); break;
            case WXSHARP_VIRT_DO_GET_SIZE:        Base::DoGetSize(&r.x, &r.y); break;
            case WXSHARP_VIRT_DO_GET_CLIENT_SIZE: Base::DoGetClientSize(&r.x, &r.y); break;

            case WXSHARP_VIRT_ADD_CHILD:    Base::AddChild(AsWindow(r.handle)); break;
            case WXSHARP_VIRT_REMOVE_CHILD: Base::RemoveChild(AsWindow(r.handle)); break;
            case WXSHARP_VIRT_MAIN_WINDOW_OF_COMPOSITE:
                r.handle = AsHandle(Base::GetMainWindowOfCompositeControl());
                break;

            case WXSHARP_VIRT_INFORM_FIRST_DIRECTION:
                r.result = Base::InformFirstDirection(r.args[0], r.args[1], r.args[2]);
                break;
            case WXSHARP_VIRT_SET_CAN_FOCUS:        Base::SetCanFocus(r.args[0] != 0); break;
            case WXSHARP_VIRT_ENABLE_VISIBLE_FOCUS: Base::EnableVisibleFocus(r.args[0] != 0); break;
            case WXSHARP_VIRT_DO_ENABLE:            Base::DoEnable(r.args[0] != 0); break;
            case WXSHARP_VIRT_DO_SET_CLIENT_SIZE:   Base::DoSetClientSize(r.args[0], r.args[1]); break;
            case WXSHARP_VIRT_DO_MOVE_WINDOW:
                Base::DoMoveWindow(r.args[0], r.args[1], r.args[2], r.args[3]);
                break;
            case WXSHARP_VIRT_DO_SET_SIZE:
                Base::DoSetSize(r.args[0], r.args[1], r.args[2], r.args[3], r.args[4]);
                break;
            case WXSHARP_VIRT_DO_SET_SIZE_HINTS:
                Base::DoSetSizeHints(r.args[0], r.args[1], r.args[2], r.args[3], r.args[4], r.args[5]);
                break;
            case WXSHARP_VIRT_DO_SET_WINDOW_VARIANT:
                Base::DoSetWindowVariant(static_cast<wxWindowVariant>(r.args[0]));
                break;

            default: r.handled = 0; break;
        }
    }

    // ---- Protected virtuals -----------------------------------------------------------------------------

protected:
    void DoEnable(bool enable) override
    {
        wxsharp_virtual_request r;
        const int args[1] = { enable ? 1 : 0 };
        if (!Ask(WXSHARP_VIRT_DO_ENABLE, r, 0, args, 1)) Base::DoEnable(enable);
    }

    void DoGetPosition(int* x, int* y) const override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_DO_GET_POSITION, r)) { Base::DoGetPosition(x, y); return; }
        if (x) *x = r.x;
        if (y) *y = r.y;
    }

    void DoGetSize(int* width, int* height) const override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_DO_GET_SIZE, r)) { Base::DoGetSize(width, height); return; }
        if (width) *width = r.x;
        if (height) *height = r.y;
    }

    void DoGetClientSize(int* width, int* height) const override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_DO_GET_CLIENT_SIZE, r)) { Base::DoGetClientSize(width, height); return; }
        if (width) *width = r.x;
        if (height) *height = r.y;
    }

    wxSize DoGetBestSize() const override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_BEST_SIZE, r) ? wxSize(r.x, r.y) : Base::DoGetBestSize();
    }

    wxSize DoGetBestClientSize() const override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_BEST_CLIENT_SIZE, r) ? wxSize(r.x, r.y) : Base::DoGetBestClientSize();
    }

    void DoSetSize(int x, int y, int width, int height, int sizeFlags) override
    {
        wxsharp_virtual_request r;
        const int args[5] = { x, y, width, height, sizeFlags };
        if (!Ask(WXSHARP_VIRT_DO_SET_SIZE, r, 0, args, 5)) Base::DoSetSize(x, y, width, height, sizeFlags);
    }

    void DoSetClientSize(int width, int height) override
    {
        wxsharp_virtual_request r;
        const int args[2] = { width, height };
        if (!Ask(WXSHARP_VIRT_DO_SET_CLIENT_SIZE, r, 0, args, 2)) Base::DoSetClientSize(width, height);
    }

    void DoSetSizeHints(int minW, int minH, int maxW, int maxH, int incW, int incH) override
    {
        wxsharp_virtual_request r;
        const int args[6] = { minW, minH, maxW, maxH, incW, incH };
        if (!Ask(WXSHARP_VIRT_DO_SET_SIZE_HINTS, r, 0, args, 6))
            Base::DoSetSizeHints(minW, minH, maxW, maxH, incW, incH);
    }

    void DoMoveWindow(int x, int y, int width, int height) override
    {
        wxsharp_virtual_request r;
        const int args[4] = { x, y, width, height };
        if (!Ask(WXSHARP_VIRT_DO_MOVE_WINDOW, r, 0, args, 4)) Base::DoMoveWindow(x, y, width, height);
    }

    void DoSetWindowVariant(wxWindowVariant variant) override
    {
        wxsharp_virtual_request r;
        const int args[1] = { static_cast<int>(variant) };
        if (!Ask(WXSHARP_VIRT_DO_SET_WINDOW_VARIANT, r, 0, args, 1)) Base::DoSetWindowVariant(variant);
    }

    wxBorder GetDefaultBorder() const override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_DEFAULT_BORDER, r) ? static_cast<wxBorder>(r.result)
                                                   : Base::GetDefaultBorder();
    }

    void DoFreeze() override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_DO_FREEZE, r)) Base::DoFreeze();
    }

    void DoThaw() override
    {
        wxsharp_virtual_request r;
        if (!Ask(WXSHARP_VIRT_DO_THAW, r)) Base::DoThaw();
    }

    bool HasTransparentBackground() override
    {
        wxsharp_virtual_request r;
        return Ask(WXSHARP_VIRT_HAS_TRANSPARENT_BACKGROUND, r) ? r.result != 0
                                                               : Base::HasTransparentBackground();
    }

protected:
    static long long AsHandle(wxWindowBase* window)
    {
        return static_cast<long long>(reinterpret_cast<intptr_t>(window));
    }

    static wxWindowBase* AsWindow(long long handle)
    {
        return static_cast<wxWindowBase*>(reinterpret_cast<void*>(static_cast<intptr_t>(handle)));
    }

    // Puts one question to managed code. Returns true when it answered.
    bool Ask(int which, wxsharp_virtual_request& r, long long handle = 0,
             const int* args = nullptr, int argCount = 0, const char* text = nullptr) const
    {
        if (!g_virtual_cb)
            return false;
        r = wxsharp_virtual_request();
        r.size = sizeof(r);
        r.version = 1;
        r.token = m_token;
        r.which = which;
        r.handle = handle;
        r.text = text;
        for (int i = 0; i < argCount; ++i)
            r.args[i] = args[i];
        g_virtual_cb(&r);
        return r.handled != 0;
    }

    long long m_token;
};

// Tree and data-view items cross the ABI as the integer value of their opaque ID.
inline wxTreeItemId TreeId(long long value)
{
    return wxTreeItemId(reinterpret_cast<void*>(static_cast<intptr_t>(value)));
}

inline long long TreeValue(const wxTreeItemId& value)
{
    return static_cast<long long>(reinterpret_cast<intptr_t>(value.GetID()));
}

inline wxDataViewItem DataViewId(long long value)
{
    return wxDataViewItem(reinterpret_cast<void*>(static_cast<intptr_t>(value)));
}

inline long long DataViewValue(const wxDataViewItem& value)
{
    return static_cast<long long>(reinterpret_cast<intptr_t>(value.GetID()));
}

// Copies a wx string into a caller buffer (up to length-1, null-terminated) and returns its full length, so
// the caller can size a buffer exactly. Used by every "get text" accessor.
inline int CopyToBuffer(const wxString& s, char* buffer, int buffer_length)
{
    const wxScopedCharBuffer utf8 = s.utf8_str();
    const size_t utf8Length = utf8.length();
    const int len = utf8Length > static_cast<size_t>(INT_MAX)
        ? INT_MAX
        : static_cast<int>(utf8Length);
    if (buffer && buffer_length > 0)
    {
        const int n = std::min(len, buffer_length - 1);
        if (n > 0)
            std::memcpy(buffer, utf8.data(), static_cast<size_t>(n));
        buffer[n] = '\0';
    }
    return len;
}

inline wxString Str(const char* s)
{
    return wxString::FromUTF8(s ? s : "");
}

// ---- Colour ---------------------------------------------------------------------------------------------
// Colours cross the ABI as a packed 0xAARRGGBB integer.
inline wxColour ColourFromArgb(unsigned int v)
{
    return wxColour(static_cast<unsigned char>((v >> 16) & 0xFF),
                    static_cast<unsigned char>((v >> 8) & 0xFF),
                    static_cast<unsigned char>(v & 0xFF),
                    static_cast<unsigned char>((v >> 24) & 0xFF));
}

inline unsigned int ArgbFromColour(const wxColour& c)
{
    return (static_cast<unsigned int>(c.Alpha()) << 24) | (static_cast<unsigned int>(c.Red()) << 16)
         | (static_cast<unsigned int>(c.Green()) << 8) | static_cast<unsigned int>(c.Blue());
}

// ---- Class-specific virtuals ----------------------------------------------------------------------------
// Some members exist on one class rather than on wxWindow, so they cannot live in the mixin above - a
// wxButton has no ShouldPreventAppExit to override. Each is layered on it instead, adding its own members
// and extending CallBase for them while leaving the whole wxWindow set intact underneath.

template <class Base>
class OverridableTopLevel : public Overridable<Base>
{
    using Super = Overridable<Base>;

public:
    using Super::Super;

    // Whether closing this window should be allowed to end the application. A window doing work in the
    // background answers false so the application outlives it.
    bool ShouldPreventAppExit() const override
    {
        wxsharp_virtual_request r;
        return this->Ask(WXSHARP_VIRT_SHOULD_PREVENT_APP_EXIT, r) ? r.result != 0
                                                                  : Base::ShouldPreventAppExit();
    }

    void CallBase(wxsharp_virtual_request& r) override
    {
        if (r.which == WXSHARP_VIRT_SHOULD_PREVENT_APP_EXIT)
        {
            r.handled = 1;
            r.result = Base::ShouldPreventAppExit();
            return;
        }
        Super::CallBase(r);
    }
};

template <class Base>
class OverridableFrame : public OverridableTopLevel<Base>
{
    using Super = OverridableTopLevel<Base>;

public:
    using Super::Super;

    // wxWidgets asks the frame to build its own bars, which is the hook for supplying a subclass of one.
    wxStatusBar* OnCreateStatusBar(int number, long style, wxWindowID id, const wxString& name) override
    {
        wxsharp_virtual_request r;
        const int args[3] = { number, static_cast<int>(style), id };
        if (!this->Ask(WXSHARP_VIRT_ON_CREATE_STATUS_BAR, r, 0, args, 3, name.utf8_str()))
            return Base::OnCreateStatusBar(number, style, id, name);
        return static_cast<wxStatusBar*>(reinterpret_cast<void*>(static_cast<intptr_t>(r.handle)));
    }

    wxToolBar* OnCreateToolBar(long style, wxWindowID id, const wxString& name) override
    {
        wxsharp_virtual_request r;
        const int args[2] = { static_cast<int>(style), id };
        if (!this->Ask(WXSHARP_VIRT_ON_CREATE_TOOL_BAR, r, 0, args, 2, name.utf8_str()))
            return Base::OnCreateToolBar(style, id, name);
        return static_cast<wxToolBar*>(reinterpret_cast<void*>(static_cast<intptr_t>(r.handle)));
    }

    // How a frame shows help for the menu item or tool under the pointer. Overriding it is how help goes
    // somewhere other than the status bar - spoken, for instance.
    void DoGiveHelp(const wxString& text, bool show) override
    {
        wxsharp_virtual_request r;
        const int args[1] = { show ? 1 : 0 };
        if (!this->Ask(WXSHARP_VIRT_DO_GIVE_HELP, r, 0, args, 1, text.utf8_str()))
            Base::DoGiveHelp(text, show);
    }

    void CallBase(wxsharp_virtual_request& r) override
    {
        r.handled = 1;
        switch (r.which)
        {
            case WXSHARP_VIRT_ON_CREATE_STATUS_BAR:
                r.handle = static_cast<long long>(reinterpret_cast<intptr_t>(
                    Base::OnCreateStatusBar(r.args[0], r.args[1], r.args[2], Str(r.text))));
                return;
            case WXSHARP_VIRT_ON_CREATE_TOOL_BAR:
                r.handle = static_cast<long long>(reinterpret_cast<intptr_t>(
                    Base::OnCreateToolBar(r.args[0], r.args[1], Str(r.text))));
                return;
            case WXSHARP_VIRT_DO_GIVE_HELP:
                Base::DoGiveHelp(Str(r.text), r.args[0] != 0);
                return;
            default:
                r.handled = 0;
                Super::CallBase(r);
                return;
        }
    }
};

template <class Base>
class OverridableDialog : public OverridableTopLevel<Base>
{
    using Super = OverridableTopLevel<Base>;

public:
    using Super::Super;

    // The window a dialog's standard button sizer and content are added to. A dialog that wraps its content
    // in a panel returns that, so wxWidgets puts things in the right place.
    wxWindow* GetContentWindow() const override
    {
        wxsharp_virtual_request r;
        if (!this->Ask(WXSHARP_VIRT_GET_CONTENT_WINDOW, r))
            return Base::GetContentWindow();
        return static_cast<wxWindow*>(reinterpret_cast<void*>(static_cast<intptr_t>(r.handle)));
    }

    void CallBase(wxsharp_virtual_request& r) override
    {
        if (r.which == WXSHARP_VIRT_GET_CONTENT_WINDOW)
        {
            r.handled = 1;
            r.handle = static_cast<long long>(reinterpret_cast<intptr_t>(Base::GetContentWindow()));
            return;
        }
        Super::CallBase(r);
    }
};

template <class Base>
class OverridableScrolled : public Overridable<Base>
{
    using Super = Overridable<Base>;

public:
    using Super::Super;

    // Whether the window scrolls itself to bring a newly focused child into view. A window that manages its
    // own scrolling answers false so wxWidgets does not fight it.
    bool ShouldScrollToChildOnFocus(wxWindow* child) override
    {
        wxsharp_virtual_request r;
        return this->Ask(WXSHARP_VIRT_SHOULD_SCROLL_TO_CHILD_ON_FOCUS, r, this->AsHandle(child))
            ? r.result != 0
            : Base::ShouldScrollToChildOnFocus(child);
    }

    wxSize GetSizeAvailableForScrollTarget(const wxSize& size) override
    {
        wxsharp_virtual_request r;
        const int args[2] = { size.x, size.y };
        return this->Ask(WXSHARP_VIRT_SIZE_FOR_SCROLL_TARGET, r, 0, args, 2)
            ? wxSize(r.x, r.y)
            : Base::GetSizeAvailableForScrollTarget(size);
    }

    void CallBase(wxsharp_virtual_request& r) override
    {
        switch (r.which)
        {
            case WXSHARP_VIRT_SHOULD_SCROLL_TO_CHILD_ON_FOCUS:
                r.handled = 1;
                r.result = Base::ShouldScrollToChildOnFocus(
                    static_cast<wxWindow*>(reinterpret_cast<void*>(static_cast<intptr_t>(r.handle))));
                return;
            case WXSHARP_VIRT_SIZE_FOR_SCROLL_TARGET:
            {
                r.handled = 1;
                const wxSize available = Base::GetSizeAvailableForScrollTarget(wxSize(r.args[0], r.args[1]));
                r.x = available.x;
                r.y = available.y;
                return;
            }
            default:
                Super::CallBase(r);
                return;
        }
    }
};



// wxGrid asks for the pen to draw each grid line with, which is how a grid highlights a column or draws a
// rule differently from the rest. Declared here but defined against wxGrid in extras.cpp, where the header
// is included.
template <class Base>
class OverridableGrid : public Overridable<Base>
{
    using Super = Overridable<Base>;

public:
    using Super::Super;

    wxPen GetColGridLinePen(int col) override
    {
        wxsharp_virtual_request r;
        const int args[1] = { col };
        return this->Ask(WXSHARP_VIRT_GRID_COL_LINE_PEN, r, 0, args, 1) ? PenFrom(r)
                                                                        : Base::GetColGridLinePen(col);
    }

    wxPen GetRowGridLinePen(int row) override
    {
        wxsharp_virtual_request r;
        const int args[1] = { row };
        return this->Ask(WXSHARP_VIRT_GRID_ROW_LINE_PEN, r, 0, args, 1) ? PenFrom(r)
                                                                        : Base::GetRowGridLinePen(row);
    }

    wxPen GetDefaultGridLinePen() override
    {
        wxsharp_virtual_request r;
        return this->Ask(WXSHARP_VIRT_GRID_DEFAULT_LINE_PEN, r) ? PenFrom(r)
                                                                : Base::GetDefaultGridLinePen();
    }

    void CallBase(wxsharp_virtual_request& r) override
    {
        r.handled = 1;
        switch (r.which)
        {
            case WXSHARP_VIRT_GRID_COL_LINE_PEN:     ToRequest(Base::GetColGridLinePen(r.args[0]), r); return;
            case WXSHARP_VIRT_GRID_ROW_LINE_PEN:     ToRequest(Base::GetRowGridLinePen(r.args[0]), r); return;
            case WXSHARP_VIRT_GRID_DEFAULT_LINE_PEN: ToRequest(Base::GetDefaultGridLinePen(), r); return;
            default:
                r.handled = 0;
                Super::CallBase(r);
                return;
        }
    }

private:
    // A pen crosses as its colour and width, which is what the managed Pen carries.
    static wxPen PenFrom(const wxsharp_virtual_request& r)
    {
        return wxPen(ColourFromArgb(r.uint_value), r.result > 0 ? r.result : 1);
    }

    static void ToRequest(const wxPen& pen, wxsharp_virtual_request& r)
    {
        r.uint_value = ArgbFromColour(pen.GetColour());
        r.result = pen.GetWidth();
    }
};

// ---- Style translation ----------------------------------------------------------------------------------
// The managed side passes stable, semantic style bits (defined by the C# style enums); these translate them to
// the wxWidgets style flags so the actual wx constants live in one place and never leak into managed code.
// The managed style enums set this bit to mean "whatever wxWidgets uses by default for this class".
// wxWidgets spells these as constants - wxTR_DEFAULT_STYLE and friends - whose values are fixed when the
// library is compiled for a platform. wxPython can expose them the same way because it ships a binary per
// platform; one managed assembly serving several cannot, so the value is resolved here instead. It is
// seeded, not returned, so Default | SomeOtherFlag composes exactly as it does in C++.
enum { WXSHARP_STYLE_PLATFORM_DEFAULT = 1 << 30 };

inline long MapFrameStyle(int s)
{
    long f = (s & WXSHARP_STYLE_PLATFORM_DEFAULT) ? wxDEFAULT_FRAME_STYLE : 0;
    if (s & 1)   f |= wxCAPTION;
    if (s & 2)   f |= wxMINIMIZE_BOX;
    if (s & 4)   f |= wxMAXIMIZE_BOX;
    if (s & 8)   f |= wxCLOSE_BOX;
    if (s & 16)  f |= wxSYSTEM_MENU;
    if (s & 32)  f |= wxRESIZE_BORDER;
    if (s & 64)  f |= wxSTAY_ON_TOP;
    if (s & 128) f |= wxFRAME_TOOL_WINDOW;
    if (s & 256) f |= wxFRAME_NO_TASKBAR;
    if (s & 512) f |= wxFRAME_FLOAT_ON_PARENT;
    return f;
}

inline long MapDialogStyle(int s)
{
    long f = (s & WXSHARP_STYLE_PLATFORM_DEFAULT) ? wxDEFAULT_DIALOG_STYLE : 0;
    if (s & 1)  f |= wxCAPTION;
    if (s & 2)  f |= wxCLOSE_BOX;
    if (s & 4)  f |= wxSYSTEM_MENU;
    if (s & 8)  f |= wxRESIZE_BORDER;
    if (s & 16) f |= wxSTAY_ON_TOP;
    if (s & 32) f |= wxMAXIMIZE_BOX;
    if (s & 64) f |= wxMINIMIZE_BOX;
    return f;
}

inline long MapPanelStyle(int s)
{
    return (s & (WXSHARP_STYLE_PLATFORM_DEFAULT | 1)) ? wxTAB_TRAVERSAL : 0;
}

inline long MapScrolledStyle(int s)
{
    long f = (s & WXSHARP_STYLE_PLATFORM_DEFAULT) ? wxScrolledWindowStyle : 0;
    if (s & 1) f |= wxHSCROLL;
    if (s & 2) f |= wxVSCROLL;
    if (s & 4) f |= wxTAB_TRAVERSAL;
    return f;
}

inline long MapListCtrlStyle(int s)
{
    long f = (s & WXSHARP_STYLE_PLATFORM_DEFAULT) ? wxLC_ICON : 0;
    if (s & 1)    f |= wxLC_REPORT;
    if (s & 2)    f |= wxLC_LIST;
    if (s & 4)    f |= wxLC_ICON;
    if (s & 8)    f |= wxLC_SMALL_ICON;
    if (s & 16)   f |= wxLC_SINGLE_SEL;
    if (s & 32)   f |= wxLC_NO_HEADER;
    if (s & 64)   f |= wxLC_EDIT_LABELS;
    if (s & 128)  f |= wxLC_VIRTUAL;
    if (s & 256)  f |= wxLC_HRULES;
    if (s & 512)  f |= wxLC_VRULES;
    if (s & 1024) f |= wxLC_SORT_ASCENDING;
    return f;
}

inline long MapTreeCtrlStyle(int s)
{
    long f = (s & WXSHARP_STYLE_PLATFORM_DEFAULT) ? wxTR_DEFAULT_STYLE : 0;
    if (s & 1)   f |= wxTR_HAS_BUTTONS;
    if (s & 2)   f |= wxTR_HIDE_ROOT;
    if (s & 4)   f |= wxTR_LINES_AT_ROOT;
    if (s & 8)   f |= wxTR_ROW_LINES;
    if (s & 16)  f |= wxTR_EDIT_LABELS;
    if (s & 32)  f |= wxTR_MULTIPLE;
    if (s & 64)  f |= wxTR_FULL_ROW_HIGHLIGHT;
    if (s & 128) f |= wxTR_TWIST_BUTTONS;
    if (s & 256) f |= wxTR_NO_LINES;
    return f;
}

inline long MapProgressStyle(int s)
{
    // wxWidgets' own default is wxPD_APP_MODAL | wxPD_AUTO_HIDE. Cancelling and skipping are not part of
    // it: each adds a button, and a caller that does not read the result would be showing a Cancel button
    // that does nothing.
    long f = (s & WXSHARP_STYLE_PLATFORM_DEFAULT) ? (wxPD_APP_MODAL | wxPD_AUTO_HIDE) : 0;
    if (s & 1)   f |= wxPD_CAN_ABORT;
    if (s & 2)   f |= wxPD_CAN_SKIP;
    if (s & 4)   f |= wxPD_APP_MODAL;
    if (s & 8)   f |= wxPD_AUTO_HIDE;
    if (s & 16)  f |= wxPD_ELAPSED_TIME;
    if (s & 32)  f |= wxPD_ESTIMATED_TIME;
    if (s & 64)  f |= wxPD_REMAINING_TIME;
    if (s & 128) f |= wxPD_SMOOTH;
    return f;
}

inline long MapFileDialogStyle(int s)
{
    long f = (s & 2) ? wxFD_SAVE : wxFD_OPEN;
    if (s & 4)   f |= wxFD_MULTIPLE;
    if (s & 8)   f |= wxFD_FILE_MUST_EXIST;
    if (s & 16)  f |= wxFD_OVERWRITE_PROMPT;
    if (s & 32)  f |= wxFD_CHANGE_DIR;
    if (s & 64)  f |= wxFD_PREVIEW;
    if (s & 128) f |= wxFD_SHOW_HIDDEN;
    if (s & 256) f |= wxFD_NO_FOLLOW;
    return f;
}

inline long MapBorder(int b)
{
    switch (b)
    {
        case 1:  return wxBORDER_NONE;
        case 2:  return wxBORDER_SIMPLE;
        case 3:  return wxBORDER_SUNKEN;
        case 4:  return wxBORDER_RAISED;
        case 5:  return wxBORDER_STATIC;
        case 6:  return wxBORDER_THEME;
        default: return wxBORDER_DEFAULT;
    }
}

inline long MapTextBoxStyle(int s)
{
    long f = 0;
    if (s & 1)    f |= wxTE_MULTILINE;
    if (s & 2)    f |= wxTE_PASSWORD;
    if (s & 4)    f |= wxTE_READONLY;
    if (s & 8)    f |= wxTE_PROCESS_ENTER;
    if (s & 16)   f |= wxTE_PROCESS_TAB;
    if (s & 32)   f |= wxTE_RICH2;
    if (s & 64)   f |= wxTE_RIGHT;
    if (s & 128)  f |= wxTE_CENTRE;
    if (s & 256)  f |= wxTE_NOHIDESEL;
    if (s & 512)  f |= wxTE_AUTO_URL;
    if (s & 1024) f |= wxTE_DONTWRAP;
    return f;
}

inline long MapSliderStyle(int s)
{
    long f = (s & 1) ? wxSL_VERTICAL : wxSL_HORIZONTAL;
    if (s & 2)  f |= wxSL_LABELS;
    if (s & 4)  f |= wxSL_AUTOTICKS;
    if (s & 8)  f |= wxSL_INVERSE;
    if (s & 16) f |= wxSL_MIN_MAX_LABELS;
    return f;
}

inline long MapListBoxStyle(int s)
{
    long f = 0;
    if (s & 1)  f |= wxLB_MULTIPLE;
    if (s & 2)  f |= wxLB_EXTENDED;
    if (s & 4)  f |= wxLB_SORT;
    if (s & 8)  f |= wxLB_ALWAYS_SB;
    if (s & 16) f |= wxLB_HSCROLL;
    if (s & 32) f |= wxLB_NEEDED_SB;
    return f;
}

inline long MapAlignment(int a)
{
    switch (a)
    {
        case 1:  return wxALIGN_CENTRE_HORIZONTAL;
        case 2:  return wxALIGN_RIGHT;
        default: return wxALIGN_LEFT;
    }
}

inline long MapCheckBoxStyle(int s)
{
    long f = (s & 1) ? wxCHK_3STATE : wxCHK_2STATE;
    if (s & 2) f |= wxCHK_ALLOW_3RD_STATE_FOR_USER;
    return f;
}

inline long MapChoiceStyle(int s)
{
    return (s & 1) ? wxCB_SORT : 0;
}
