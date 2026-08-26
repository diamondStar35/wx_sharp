// Radio button. Pass group_start on the first button of a mutually-exclusive group.
#include "internal.h"

wxsharp_handle wxsharp_radio_create(wxsharp_handle parent, int id, const char* label, bool group_start, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new wxRadioButton(p, id, Str(label), wxDefaultPosition, wxDefaultSize,
                                   group_start ? wxRB_GROUP : 0);
    TrackWindow(ctrl, token);
    return ctrl;
}

bool wxsharp_radio_get(wxsharp_handle ctrl) { return static_cast<wxRadioButton*>(ctrl)->GetValue(); }
void wxsharp_radio_set(wxsharp_handle ctrl, bool value) { static_cast<wxRadioButton*>(ctrl)->SetValue(value); }
