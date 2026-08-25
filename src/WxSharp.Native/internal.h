// Shared internals for the wxsharp implementation files. Not part of the public ABI.
#pragma once

#include <wx/wx.h>
#include "wxsharp.h"
#include <algorithm>
#include <climits>
#include <cstring>

// Event types reported to the managed side (kept in sync with EventKind in C#).
enum
{
    WXSHARP_EVT_CLICK = 1,       // button pressed
    WXSHARP_EVT_CLOSE = 2,       // window/dialog close requested
    WXSHARP_EVT_TEXT = 3,        // text control changed
    WXSHARP_EVT_TOGGLE = 4,      // checkbox toggled
    WXSHARP_EVT_SELECT = 5,      // choice/listbox/radio selection changed
    WXSHARP_EVT_SLIDER = 6,      // slider value changed
    WXSHARP_EVT_SHOWN = 7,       // window shown
    WXSHARP_EVT_ACTIVATE = 8,    // window activated (got focus)
    WXSHARP_EVT_DEACTIVATE = 9,  // window deactivated (lost focus)
    WXSHARP_EVT_RESIZE = 10,     // window resized
    WXSHARP_EVT_FOCUS_GAINED = 11, // control gained keyboard focus
    WXSHARP_EVT_FOCUS_LOST = 12,   // control lost keyboard focus
    WXSHARP_EVT_TEXT_ENTER = 13,   // Enter pressed in a text control
    WXSHARP_EVT_MOVE = 14,         // window moved
    WXSHARP_EVT_MAXIMIZE = 15,     // window maximized
    WXSHARP_EVT_MOUSE_DOWN = 16,   // left mouse button pressed on a control
    WXSHARP_EVT_MOUSE_UP = 17,     // left mouse button released on a control
    WXSHARP_EVT_MOUSE_RIGHT = 18,  // right mouse button pressed (context click)
    WXSHARP_EVT_MOUSE_DOUBLE = 19, // left double-click
    WXSHARP_EVT_MOUSE_ENTER = 20,  // pointer entered the control
    WXSHARP_EVT_MOUSE_LEAVE = 21,  // pointer left the control
    WXSHARP_EVT_MOUSE_MOVE = 22,   // pointer moved over the control
    WXSHARP_EVT_PAINT = 23,        // a canvas needs repainting (draw during the managed handler)
    WXSHARP_EVT_WHEEL_UP = 24,     // mouse wheel scrolled up/away
    WXSHARP_EVT_WHEEL_DOWN = 25,   // mouse wheel scrolled down/toward
};

// Key event kinds reported through the key callback (kept in sync with KeyKind in C#). The callback returns
// true to consume the key, false to let normal processing continue.
enum
{
    WXSHARP_KEY_HOOK = 1, // char hook on a top-level window (global shortcuts, before the focused control)
    WXSHARP_KEY_DOWN = 2, // key pressed while a control has focus
    WXSHARP_KEY_UP = 3,   // key released while a control has focus
};

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

// The managed callbacks (defined in app.cpp, set via wxsharp_set_event_handler / wxsharp_set_key_handler).
extern wxsharp_event_cb g_event_cb;
extern wxsharp_key_cb g_key_cb;

inline void Fire(int id, int evt)
{
    if (g_event_cb)
        g_event_cb(id, evt);
}

// Packs the modifier state of a key event into the bitfield the managed side expects (bit0 Ctrl, 1 Shift, 2 Alt).
inline int KeyMods(const wxKeyEvent& e)
{
    return (e.ControlDown() ? 1 : 0) | (e.ShiftDown() ? 2 : 0) | (e.AltDown() ? 4 : 0);
}

// Reports a key event to the managed key callback; returns true if the managed side consumed it.
inline bool FireKey(int id, int kind, const wxKeyEvent& e)
{
    return g_key_cb && g_key_cb(id, kind, e.GetKeyCode(), KeyMods(e));
}

// Forwards key presses on a top-level window to the managed key hook; if the managed side does not consume
// the key it is skipped so normal processing (tab navigation, typing, Esc-to-close, ...) still happens.
inline void BindKeyHook(wxWindow* top, int id)
{
    top->Bind(wxEVT_CHAR_HOOK, [id](wxKeyEvent& e)
    {
        if (!FireKey(id, WXSHARP_KEY_HOOK, e))
            e.Skip();
    });
}

inline wxString Str(const char* s)
{
    return wxString::FromUTF8(s ? s : "");
}

// Binds the mouse events every control shares. Skipped so the control's own click/hover handling still runs.
inline void BindMouse(wxWindow* ctrl, int id)
{
    ctrl->Bind(wxEVT_LEFT_DOWN, [id](wxMouseEvent& e) { Fire(id, WXSHARP_EVT_MOUSE_DOWN); e.Skip(); });
    ctrl->Bind(wxEVT_LEFT_UP, [id](wxMouseEvent& e) { Fire(id, WXSHARP_EVT_MOUSE_UP); e.Skip(); });
    ctrl->Bind(wxEVT_RIGHT_DOWN, [id](wxMouseEvent& e) { Fire(id, WXSHARP_EVT_MOUSE_RIGHT); e.Skip(); });
    ctrl->Bind(wxEVT_LEFT_DCLICK, [id](wxMouseEvent& e) { Fire(id, WXSHARP_EVT_MOUSE_DOUBLE); e.Skip(); });
    ctrl->Bind(wxEVT_ENTER_WINDOW, [id](wxMouseEvent& e) { Fire(id, WXSHARP_EVT_MOUSE_ENTER); e.Skip(); });
    ctrl->Bind(wxEVT_LEAVE_WINDOW, [id](wxMouseEvent& e) { Fire(id, WXSHARP_EVT_MOUSE_LEAVE); e.Skip(); });
    ctrl->Bind(wxEVT_MOTION, [id](wxMouseEvent& e) { Fire(id, WXSHARP_EVT_MOUSE_MOVE); e.Skip(); });
    ctrl->Bind(wxEVT_MOUSEWHEEL, [id](wxMouseEvent& e)
    {
        Fire(id, e.GetWheelRotation() > 0 ? WXSHARP_EVT_WHEEL_UP : WXSHARP_EVT_WHEEL_DOWN);
        e.Skip();
    });
}

// Binds the events every control shares - focus in/out, key down/up and the mouse events - to the managed
// callbacks. Called by each widget's create() so focus, keyboard and pointer input are reported uniformly and
// can drive managed behaviour (e.g. the custom slider's key handling). Unconsumed keys/events are skipped so
// native processing still runs.
inline void BindCommon(wxWindow* ctrl, int id)
{
    ctrl->Bind(wxEVT_SET_FOCUS, [id](wxFocusEvent& e) { Fire(id, WXSHARP_EVT_FOCUS_GAINED); e.Skip(); });
    ctrl->Bind(wxEVT_KILL_FOCUS, [id](wxFocusEvent& e) { Fire(id, WXSHARP_EVT_FOCUS_LOST); e.Skip(); });
    ctrl->Bind(wxEVT_KEY_DOWN, [id](wxKeyEvent& e) { if (!FireKey(id, WXSHARP_KEY_DOWN, e)) e.Skip(); });
    ctrl->Bind(wxEVT_KEY_UP, [id](wxKeyEvent& e) { if (!FireKey(id, WXSHARP_KEY_UP, e)) e.Skip(); });
    BindMouse(ctrl, id);
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

// ---- Style translation ----------------------------------------------------------------------------------
// The managed side passes stable, semantic style bits (defined by the C# style enums); these translate them to
// the wxWidgets style flags so the actual wx constants live in one place and never leak into managed code.
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
    return f ? f : wxLB_SINGLE;
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
    return (s & 1) ? wxCHK_3STATE : wxCHK_2STATE;
}

inline long MapChoiceStyle(int s)
{
    return (s & 1) ? wxCB_SORT : 0;
}

// ---- Font -----------------------------------------------------------------------------------------------
// Builds a wxFont from the managed Font description (0/default point size uses the system default).
inline wxFont MakeFont(int pointSize, int family, int weight, int style, bool underline, const char* face)
{
    wxFontInfo info(pointSize > 0 ? pointSize : wxNORMAL_FONT->GetPointSize());

    switch (family)
    {
        case 1:  info.Family(wxFONTFAMILY_ROMAN); break;
        case 2:  info.Family(wxFONTFAMILY_SCRIPT); break;
        case 3:  info.Family(wxFONTFAMILY_SWISS); break;
        case 4:  info.Family(wxFONTFAMILY_MODERN); break;
        case 5:  info.Family(wxFONTFAMILY_TELETYPE); break;
        default: info.Family(wxFONTFAMILY_DEFAULT); break;
    }

    switch (weight)
    {
        case 1: info.Light(); break;
        case 2: info.Bold(); break;
        default: break;
    }

    if (style == 1) info.Italic();
    else if (style == 2) info.Slant();
    if (underline) info.Underlined();
    if (face && *face) info.FaceName(Str(face));

    return wxFont(info);
}

// Adds a control to its parent panel's vertical sizer with a small border.
inline void AddToPanel(wxWindow* parent, wxWindow* ctrl, int flags)
{
    if (auto* sizer = parent->GetSizer())
        sizer->Add(ctrl, 0, flags, 6);
}

// Gives a top-level window a content panel (with a vertical sizer for controls to stack into) that fills it.
inline void SetupContentPanel(wxWindow* top)
{
    auto* panel = new wxPanel(top, wxID_ANY);
    panel->SetSizer(new wxBoxSizer(wxVERTICAL));
    auto* mainSizer = new wxBoxSizer(wxVERTICAL);
    mainSizer->Add(panel, 1, wxEXPAND | wxALL, 8);
    top->SetSizer(mainSizer);
}

// A bare content area: the vertical sizer lives directly on the window with no intermediate wxPanel (which a
// screen reader announces as a grouping). Controls become direct children of the window.
inline void SetupBareContent(wxWindow* top)
{
    top->SetSizer(new wxBoxSizer(wxVERTICAL));
}

// The container controls are created against: the content wxPanel if one was created, otherwise the window
// itself (bare mode - controls are direct children).
inline wxWindow* ContentPanel(wxWindow* top)
{
    auto& kids = top->GetChildren();
    if (!kids.IsEmpty())
    {
        wxWindow* first = kids.GetFirst()->GetData();
        if (wxDynamicCast(first, wxPanel))
            return first;
    }
    return top;
}

// Puts keyboard focus on the first focusable, visible control of a window/dialog. Hidden controls (e.g. the
// inline edit box before a prompt) are skipped - AcceptsFocus() alone ignores visibility, so focusing one
// would surface it; if nothing qualifies, focus stays on the frame.
inline void FocusFirst(wxWindow* top)
{
    auto* panel = ContentPanel(top);
    if (!panel)
        return;
    for (wxWindow* child : panel->GetChildren())
    {
        if (child->IsShown() && child->AcceptsFocus())
        {
            child->SetFocus();
            break;
        }
    }
}
