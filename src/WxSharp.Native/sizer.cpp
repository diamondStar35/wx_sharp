// Explicit sizers. A box sizer lays items in one direction; sizers nest and a window adopts one explicitly.
#include "internal.h"
#include <wx/gbsizer.h>

wxsharp_handle wxsharp_boxsizer_create(bool horizontal)
{
    return new wxBoxSizer(horizontal ? wxHORIZONTAL : wxVERTICAL);
}

wxsharp_handle wxsharp_gridsizer_create(int rows, int columns, int verticalGap, int horizontalGap)
{
    return new wxGridSizer(rows, columns, verticalGap, horizontalGap);
}

wxsharp_handle wxsharp_flexgridsizer_create(int rows, int columns, int verticalGap, int horizontalGap)
{
    return new wxFlexGridSizer(rows, columns, verticalGap, horizontalGap);
}

void wxsharp_flexgridsizer_add_growable_row(wxsharp_handle sizer, int row, int proportion)
{
    static_cast<wxFlexGridSizer*>(sizer)->AddGrowableRow(row, proportion);
}

void wxsharp_flexgridsizer_add_growable_column(wxsharp_handle sizer, int column, int proportion)
{
    static_cast<wxFlexGridSizer*>(sizer)->AddGrowableCol(column, proportion);
}

wxsharp_handle wxsharp_staticboxsizer_create(wxsharp_handle box, bool horizontal)
{
    return new wxStaticBoxSizer(static_cast<wxStaticBox*>(box), horizontal ? wxHORIZONTAL : wxVERTICAL);
}

namespace
{
    int MapSizerFlags(int value)
    {
        int flags = 0;
        if (value & 1) flags |= wxEXPAND;
        if (value & 2) flags |= wxALIGN_CENTER;
        if (value & 4) flags |= wxLEFT;
        if (value & 8) flags |= wxTOP;
        if (value & 16) flags |= wxRIGHT;
        if (value & 32) flags |= wxBOTTOM;
        return flags;
    }
}

void wxsharp_sizer_add_control(wxsharp_handle sizer, wxsharp_handle ctrl, int proportion, int flags, int border)
{
    static_cast<wxSizer*>(sizer)->Add(static_cast<wxWindow*>(ctrl), proportion, MapSizerFlags(flags), border);
}

void wxsharp_sizer_add_sizer(wxsharp_handle sizer, wxsharp_handle child, int proportion, int flags, int border)
{
    static_cast<wxSizer*>(sizer)->Add(static_cast<wxSizer*>(child), proportion, MapSizerFlags(flags), border);
}

void wxsharp_sizer_add_spacer(wxsharp_handle sizer, int size) { static_cast<wxSizer*>(sizer)->AddSpacer(size); }
void wxsharp_sizer_add_stretch_spacer(wxsharp_handle sizer, int proportion) { static_cast<wxSizer*>(sizer)->AddStretchSpacer(proportion); }

void wxsharp_window_set_sizer(wxsharp_handle window, wxsharp_handle sizer)
{
    auto* w = static_cast<wxWindow*>(window);
    w->SetSizer(static_cast<wxSizer*>(sizer));
    w->Layout();
}

wxsharp_handle wxsharp_gridbagsizer_create(int verticalGap, int horizontalGap)
{
    return new wxGridBagSizer(verticalGap, horizontalGap);
}

void wxsharp_gridbagsizer_add_control(wxsharp_handle sizer, wxsharp_handle ctrl, int row, int column,
                                      int rowSpan, int columnSpan, int flags, int border)
{
    static_cast<wxGridBagSizer*>(sizer)->Add(static_cast<wxWindow*>(ctrl), wxGBPosition(row, column),
        wxGBSpan(rowSpan, columnSpan), MapSizerFlags(flags), border);
}
