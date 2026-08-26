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

        long long token = 0;

        wxString name, description, help, value, keyboardShortcut, defaultAction;
        wxAccRole role = wxROLE_NONE;
        long state = 0;
        bool hasRole = false;
        bool hasState = false;

        wxAccStatus GetName(int childId, wxString* out) override
        {
            if (token && QueryString(2, childId, out) != wxACC_NOT_IMPLEMENTED) return lastStatus;
            if (childId == 0 && !name.IsEmpty()) { *out = name; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED; // fall back to wx's default (the window name)
        }
        wxAccStatus GetRole(int childId, wxAccRole* out) override
        {
            if (token) { wxsharp_accessible_request q = Request(8, childId); lastStatus = Query(q); if (lastStatus != wxACC_NOT_IMPLEMENTED) { *out = static_cast<wxAccRole>(q.int_value); return lastStatus; } }
            if (childId == 0 && hasRole) { *out = role; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetDescription(int childId, wxString* out) override
        {
            if (token && QueryString(3, childId, out) != wxACC_NOT_IMPLEMENTED) return lastStatus;
            if (childId == 0 && !description.IsEmpty()) { *out = description; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetHelpText(int childId, wxString* out) override
        {
            if (token && QueryString(4, childId, out) != wxACC_NOT_IMPLEMENTED) return lastStatus;
            if (childId == 0 && !help.IsEmpty()) { *out = help; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetValue(int childId, wxString* out) override
        {
            if (token && QueryString(5, childId, out) != wxACC_NOT_IMPLEMENTED) return lastStatus;
            if (childId == 0 && !value.IsEmpty()) { *out = value; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetKeyboardShortcut(int childId, wxString* out) override
        {
            if (token && QueryString(6, childId, out) != wxACC_NOT_IMPLEMENTED) return lastStatus;
            if (childId == 0 && !keyboardShortcut.IsEmpty()) { *out = keyboardShortcut; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetDefaultAction(int childId, wxString* out) override
        {
            if (token && QueryString(7, childId, out) != wxACC_NOT_IMPLEMENTED) return lastStatus;
            if (childId == 0 && !defaultAction.IsEmpty()) { *out = defaultAction; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }
        wxAccStatus GetState(int childId, long* out) override
        {
            if (token) { wxsharp_accessible_request q = Request(9, childId); lastStatus = Query(q); if (lastStatus != wxACC_NOT_IMPLEMENTED) { *out = static_cast<long>(q.uint_value); return lastStatus; } }
            if (childId == 0 && hasState) { *out = state; return wxACC_OK; }
            return wxACC_NOT_IMPLEMENTED;
        }

        wxAccStatus GetChildCount(int* count) override
        {
            wxsharp_accessible_request q = Request(1, 0); lastStatus = Query(q);
            if (lastStatus == wxACC_OK) *count = q.int_value;
            return lastStatus;
        }
        wxAccStatus GetChild(int childId, wxAccessible** child) override
        {
            if (childId <= 0) return wxACC_INVALID_ARG;
            *child = nullptr; return wxACC_OK; // managed children are simple integer elements
        }
        wxAccStatus GetLocation(wxRect& rect, int childId) override
        {
            wxsharp_accessible_request q = Request(10, childId); lastStatus = Query(q);
            if (lastStatus == wxACC_OK) rect = wxRect(q.x, q.y, q.width, q.height);
            return lastStatus;
        }
        wxAccStatus HitTest(const wxPoint& point, int* childId, wxAccessible** child) override
        {
            wxsharp_accessible_request q = Request(11, 0); q.x = point.x; q.y = point.y; lastStatus = Query(q);
            if (lastStatus == wxACC_OK) { *childId = q.int_value; *child = nullptr; }
            return lastStatus;
        }
        wxAccStatus Navigate(wxNavDir direction, int fromId, int* toId, wxAccessible** toObject) override
        {
            wxsharp_accessible_request q = Request(12, fromId); q.argument = static_cast<int>(direction); lastStatus = Query(q);
            if (lastStatus == wxACC_OK) { *toId = q.int_value; *toObject = nullptr; }
            return lastStatus;
        }
        wxAccStatus Select(int childId, wxAccSelectionFlags flags) override
        {
            wxsharp_accessible_request q = Request(13, childId); q.argument = static_cast<int>(flags); return Query(q);
        }
        wxAccStatus DoDefaultAction(int childId) override
        {
            wxsharp_accessible_request q = Request(14, childId); return Query(q);
        }
        wxAccStatus GetFocus(int* childId, wxAccessible** child) override
        {
            wxsharp_accessible_request q = Request(15, 0); lastStatus = Query(q);
            if (lastStatus == wxACC_OK) { *childId = q.int_value; *child = q.int_value == 0 ? this : nullptr; }
            return lastStatus;
        }
#if wxUSE_VARIANT
        wxAccStatus GetSelections(wxVariant* selections) override
        {
            wxsharp_accessible_request q = Request(16, 0); lastStatus = Query(q);
            if (lastStatus != wxACC_OK) return lastStatus;
            const int count = q.required_length / static_cast<int>(sizeof(int));
            if (count <= 0) { selections->MakeNull(); return lastStatus; }
            std::vector<int> ids(static_cast<size_t>(count)); q.buffer = reinterpret_cast<char*>(ids.data());
            q.buffer_length = q.required_length; lastStatus = Query(q);
            if (lastStatus != wxACC_OK) return lastStatus;
            if (count == 1) { *selections = wxVariant(static_cast<long>(ids[0])); return lastStatus; }
            selections->NullList(); for (int id : ids) selections->Append(wxVariant(static_cast<long>(id)));
            return lastStatus;
        }
#endif

    private:
        wxAccStatus lastStatus = wxACC_NOT_IMPLEMENTED;
        wxsharp_accessible_request Request(int operation, int childId) const
        {
            wxsharp_accessible_request q = {}; q.size = sizeof(q); q.version = 1; q.token = token;
            q.operation = operation; q.child_id = childId; return q;
        }
        static wxAccStatus Query(wxsharp_accessible_request& q);
        wxAccStatus QueryString(int operation, int childId, wxString* out)
        {
            wxsharp_accessible_request q = Request(operation, childId); lastStatus = Query(q);
            if (lastStatus != wxACC_OK) return lastStatus;
            if (q.required_length <= 0) { out->clear(); return lastStatus; }
            std::vector<char> bytes(static_cast<size_t>(q.required_length) + 1);
            q.buffer = bytes.data(); q.buffer_length = static_cast<int>(bytes.size()); lastStatus = Query(q);
            if (lastStatus == wxACC_OK) *out = Str(bytes.data());
            return lastStatus;
        }
    };

    wxsharp_accessible_cb g_accessible_cb = nullptr;
    wxAccStatus WxSharpAccessible::Query(wxsharp_accessible_request& q)
    {
        return g_accessible_cb ? static_cast<wxAccStatus>(g_accessible_cb(&q)) : wxACC_NOT_IMPLEMENTED;
    }

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

void wxsharp_set_accessible_handler(wxsharp_accessible_cb cb)
{
#if wxUSE_ACCESSIBILITY
    g_accessible_cb = cb;
#else
    (void)cb;
#endif
}

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

void wxsharp_control_set_accessible(wxsharp_handle ctrl, long long token)
{
#if wxUSE_ACCESSIBILITY
    Ensure(ctrl)->token = token;
#else
    (void)ctrl; (void)token;
#endif
}

void wxsharp_accessible_notify(int eventType, wxsharp_handle window, int objectType, int childId)
{
#if wxUSE_ACCESSIBILITY
    wxAccessible::NotifyEvent(eventType, static_cast<wxWindow*>(window),
        static_cast<wxAccObject>(objectType), childId);
#else
    (void)eventType; (void)window; (void)objectType; (void)childId;
#endif
}

unsigned int wxsharp_accessible_probe(wxsharp_handle window)
{
#if wxUSE_ACCESSIBILITY
    auto* accessible = static_cast<wxWindow*>(window)->GetAccessible();
    if (!accessible) return 0;
    unsigned int result = 0; int count = 0; wxString name; wxAccRole role = wxROLE_NONE; long state = 0;
    if (accessible->GetChildCount(&count) == wxACC_OK) result |= 1;
    if (accessible->GetName(wxACC_SELF, &name) == wxACC_OK) result |= 2;
    if (accessible->GetRole(wxACC_SELF, &role) == wxACC_OK) result |= 4;
    if (accessible->GetState(wxACC_SELF, &state) == wxACC_OK) result |= 8;
    return result;
#else
    (void)window; return 0;
#endif
}
