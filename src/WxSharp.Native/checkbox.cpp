// Check box. The accessible name is set from the managed side (accessible.cpp / Control.AccessibleName).
#include "internal.h"

wxsharp_handle wxsharp_checkbox_create(wxsharp_handle parent, const char* label, int style, int id)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new wxCheckBox(p, wxID_ANY, Str(label), wxDefaultPosition, wxDefaultSize, MapCheckBoxStyle(style));
    ctrl->Bind(wxEVT_CHECKBOX, [id](wxCommandEvent&) { Fire(id, WXSHARP_EVT_TOGGLE); });
    BindCommon(ctrl, id);
    AddToPanel(p, ctrl, wxALL);
    return ctrl;
}

bool wxsharp_checkbox_get(wxsharp_handle ctrl) { return static_cast<wxCheckBox*>(ctrl)->GetValue(); }
void wxsharp_checkbox_set(wxsharp_handle ctrl, bool value) { static_cast<wxCheckBox*>(ctrl)->SetValue(value); }
