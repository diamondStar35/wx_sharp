// Accessibility: a wxAccessible the managed side populates so assistive technology announces meaningful
// information (name, role, description, help, value) for any control - including custom-drawn or mislabeled
// ones. Attached lazily; wx owns it once set. Where wxUSE_ACCESSIBILITY is off, only the window name applies.
#include "internal.h"

#if wxUSE_ACCESSIBILITY
#include <wx/access.h>

namespace
{
    class WxSharpAccessible : public wxAccessible
    {
    public:
        explicit WxSharpAccessible(wxWindow* win) : wxAccessible(win) {}

        wxString name, description, help, value, keyboardShortcut, defaultAction;
        wxAccRole role = wxROLE_NONE;
        long state = 0;
        bool hasRole = false;
        bool hasState = false;

        wxAccStatus GetName(int childId, wxString* out) override
        {
            if (childId == 0 && !name.IsEmpty()) { *out = name; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED; // fall back to wx's default (the window name)
        }
        wxAccStatus GetRole(int childId, wxAccRole* out) override
        {
            if (childId == 0 && hasRole) { *out = role; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetDescription(int childId, wxString* out) override
        {
            if (childId == 0 && !description.IsEmpty()) { *out = description; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetHelpText(int childId, wxString* out) override
        {
            if (childId == 0 && !help.IsEmpty()) { *out = help; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetValue(int childId, wxString* out) override
        {
            if (childId == 0 && !value.IsEmpty()) { *out = value; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetKeyboardShortcut(int childId, wxString* out) override
        {
            if (childId == 0 && !keyboardShortcut.IsEmpty()) { *out = keyboardShortcut; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetDefaultAction(int childId, wxString* out) override
        {
            if (childId == 0 && !defaultAction.IsEmpty()) { *out = defaultAction; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetState(int childId, long* out) override
        {
            if (childId == 0 && hasState) { *out = state; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
    };

    WxSharpAccessible* Ensure(wxsharp_handle ctrl)
    {
        auto* w = static_cast<wxWindow*>(ctrl);
        if (auto* existing = dynamic_cast<WxSharpAccessible*>(w->GetAccessible()))
            return existing;
        auto* acc = new WxSharpAccessible(w);
        w->SetAccessible(acc); // wx takes ownership
        return acc;
    }

    wxAccRole MapRole(int role)
    {
        return role >= wxROLE_NONE && role <= wxROLE_SYSTEM_WINDOW
            ? static_cast<wxAccRole>(role)
            : wxROLE_NONE;
    }

    void Notify(wxWindow* window, int eventType)
    {
        wxAccessible::NotifyEvent(eventType, window, wxOBJID_CLIENT, wxACC_SELF);
    }
}
#endif

bool wxsharp_custom_accessibility_available()
{
#if wxUSE_ACCESSIBILITY
    return true;
#else
    return false;
#endif
}

// The window name is what wx's default accessible reports; set it too so the name works even when the full
// accessibility framework is compiled out.
void wxsharp_control_set_name(wxsharp_handle ctrl, const char* name)
{
    static_cast<wxWindow*>(ctrl)->SetName(Str(name));
#if wxUSE_ACCESSIBILITY
    Ensure(ctrl)->name = Str(name);
    Notify(static_cast<wxWindow*>(ctrl), wxACC_EVENT_OBJECT_NAMECHANGE);
#endif
}

void wxsharp_control_set_role(wxsharp_handle ctrl, int role)
{
#if wxUSE_ACCESSIBILITY
    auto* acc = Ensure(ctrl);
    acc->hasRole = role != 0;
    acc->role = MapRole(role);
    Notify(static_cast<wxWindow*>(ctrl), wxACC_EVENT_OBJECT_STATECHANGE);
#else
    (void)ctrl; (void)role;
#endif
}

void wxsharp_control_set_description(wxsharp_handle ctrl, const char* text)
{
#if wxUSE_ACCESSIBILITY
    Ensure(ctrl)->description = Str(text);
    Notify(static_cast<wxWindow*>(ctrl), wxACC_EVENT_OBJECT_DESCRIPTIONCHANGE);
#else
    (void)ctrl; (void)text;
#endif
}

void wxsharp_control_set_help(wxsharp_handle ctrl, const char* text)
{
#if wxUSE_ACCESSIBILITY
    Ensure(ctrl)->help = Str(text);
    Notify(static_cast<wxWindow*>(ctrl), wxACC_EVENT_OBJECT_HELPCHANGE);
#else
    (void)ctrl; (void)text;
#endif
}

void wxsharp_control_set_accessible_value(wxsharp_handle ctrl, const char* text)
{
#if wxUSE_ACCESSIBILITY
    Ensure(ctrl)->value = Str(text);
    Notify(static_cast<wxWindow*>(ctrl), wxACC_EVENT_OBJECT_VALUECHANGE);
#else
    (void)ctrl; (void)text;
#endif
}

void wxsharp_control_set_accessible_keyboard_shortcut(wxsharp_handle ctrl, const char* text)
{
#if wxUSE_ACCESSIBILITY
    Ensure(ctrl)->keyboardShortcut = Str(text);
    Notify(static_cast<wxWindow*>(ctrl), wxACC_EVENT_OBJECT_ACCELERATORCHANGE);
#else
    (void)ctrl; (void)text;
#endif
}

void wxsharp_control_set_accessible_default_action(wxsharp_handle ctrl, const char* text)
{
#if wxUSE_ACCESSIBILITY
    Ensure(ctrl)->defaultAction = Str(text);
    Notify(static_cast<wxWindow*>(ctrl), wxACC_EVENT_OBJECT_DEFACTIONCHANGE);
#else
    (void)ctrl; (void)text;
#endif
}

void wxsharp_control_set_accessible_state(wxsharp_handle ctrl, unsigned int state)
{
#if wxUSE_ACCESSIBILITY
    auto* acc = Ensure(ctrl);
    acc->hasState = true;
    acc->state = static_cast<long>(state);
    Notify(static_cast<wxWindow*>(ctrl), wxACC_EVENT_OBJECT_STATECHANGE);
#else
    (void)ctrl; (void)state;
#endif
}
