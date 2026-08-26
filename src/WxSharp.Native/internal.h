// Shared internals for the wxsharp implementation files. Not part of the public ABI.
#pragma once

#include <wx/wx.h>
#include <wx/dataview.h>
#include <wx/filedlg.h>
#include <wx/listctrl.h>
#include <wx/scrolwin.h>
#include <wx/treectrl.h>
#include "wxsharp.h"
#include <algorithm>
#include <climits>
#include <cstring>
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
