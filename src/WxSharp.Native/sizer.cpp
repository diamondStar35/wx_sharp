// Sizers: explicit layout for callers that want more than the parent's default vertical stack. A box sizer
// lays its items out in one direction; items are added with a proportion (0 = fixed, >0 = share of free
// space), optional expand/centre, and a border. Sizers nest, and a window adopts one via set_sizer.
#include "internal.h"

wxsharp_handle wxsharp_boxsizer_create(bool horizontal)
{
    return new wxBoxSizer(horizontal ? wxHORIZONTAL : wxVERTICAL);
}

void wxsharp_sizer_add_control(wxsharp_handle sizer, wxsharp_handle ctrl, int proportion, bool expand, bool center, int border)
{
    int flags = 0;
    if (expand) flags |= wxEXPAND;
    if (center) flags |= wxALIGN_CENTER;
    if (border > 0) flags |= wxALL;
    static_cast<wxSizer*>(sizer)->Add(static_cast<wxWindow*>(ctrl), proportion, flags, border);
}

void wxsharp_sizer_add_sizer(wxsharp_handle sizer, wxsharp_handle child, int proportion, bool expand, int border)
{
    int flags = 0;
    if (expand) flags |= wxEXPAND;
    if (border > 0) flags |= wxALL;
    static_cast<wxSizer*>(sizer)->Add(static_cast<wxSizer*>(child), proportion, flags, border);
}

void wxsharp_sizer_add_spacer(wxsharp_handle sizer, int size) { static_cast<wxSizer*>(sizer)->AddSpacer(size); }
void wxsharp_sizer_add_stretch_spacer(wxsharp_handle sizer, int proportion) { static_cast<wxSizer*>(sizer)->AddStretchSpacer(proportion); }

void wxsharp_window_set_sizer(wxsharp_handle window, wxsharp_handle sizer)
{
    auto* w = static_cast<wxWindow*>(window);
    w->SetSizer(static_cast<wxSizer*>(sizer));
    w->Layout();
}
