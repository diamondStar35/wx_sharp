// Radio button. Pass group_start on the first button of a mutually-exclusive group.
#include "internal.h"

wxsharp_handle wxsharp_radio_create(wxsharp_handle parent, const char* label, bool group_start, int id)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new wxRadioButton(p, wxID_ANY, Str(label), wxDefaultPosition, wxDefaultSize,
                                   group_start ? wxRB_GROUP : 0);
    ctrl->Bind(wxEVT_RADIOBUTTON, [id](wxCommandEvent&) { Fire(id, WXSHARP_EVT_SELECT); });
    BindCommon(ctrl, id);
    AddToPanel(p, ctrl, wxALL);
    return ctrl;
}

bool wxsharp_radio_get(wxsharp_handle ctrl) { return static_cast<wxRadioButton*>(ctrl)->GetValue(); }
void wxsharp_radio_set(wxsharp_handle ctrl, bool value) { static_cast<wxRadioButton*>(ctrl)->SetValue(value); }
