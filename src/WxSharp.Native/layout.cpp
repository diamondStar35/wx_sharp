// Layout panels: a sub-container with its own horizontal or vertical sizer. Controls created against it stack
// in that direction, and panels nest, so rows-within-columns (and richer arrangements) compose from these.
#include "internal.h"

wxsharp_handle wxsharp_panel_create(wxsharp_handle parent, bool horizontal, int id)
{
    (void)id; // a layout panel emits no events of its own
    auto* p = static_cast<wxWindow*>(parent);
    auto* panel = new wxPanel(p, wxID_ANY);
    panel->SetSizer(new wxBoxSizer(horizontal ? wxHORIZONTAL : wxVERTICAL));
    AddToPanel(p, panel, wxEXPAND | wxALL);
    return panel;
}
