// Check box. The accessible name is set from the managed side (accessible.cpp / Control.AccessibleName).
#include "internal.h"

wxsharp_handle wxsharp_checkbox_create(wxsharp_handle parent, int id, const char* label, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new wxCheckBox(p, id, Str(label), wxDefaultPosition, wxDefaultSize, MapCheckBoxStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

bool wxsharp_checkbox_get(wxsharp_handle ctrl) { return static_cast<wxCheckBox*>(ctrl)->GetValue(); }
void wxsharp_checkbox_set(wxsharp_handle ctrl, bool value) { static_cast<wxCheckBox*>(ctrl)->SetValue(value); }
