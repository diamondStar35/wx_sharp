// Explicit panel container. Layout exists only when the caller assigns a sizer.
#include "internal.h"

wxsharp_handle wxsharp_panel_create(wxsharp_handle parent, int id, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* panel = new wxPanel(p, id);
    BindCommon(panel, token);
    return panel;
}
