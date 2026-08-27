// Sizers. A sizer lays its items out in one direction or a grid; sizers nest, and a window adopts one
// explicitly. Items are wxSizerItem handles, so proportion, flags, border and visibility can be read and
// changed after the item is added, exactly as wxSizer allows.
#include "internal.h"
#include <wx/gbsizer.h>

namespace
{
    inline wxSizer* Sz(wxsharp_handle h) { return static_cast<wxSizer*>(h); }
    inline wxSizerItem* It(wxsharp_handle h) { return static_cast<wxSizerItem*>(h); }
    inline wxWindow* Win(wxsharp_handle h) { return static_cast<wxWindow*>(h); }
    inline wxGridBagSizer* Gb(wxsharp_handle h) { return static_cast<wxGridBagSizer*>(h); }

    int MapSizerFlags(int value)
    {
        int flags = 0;
        if (value & 1)     flags |= wxEXPAND;
        if (value & 2)     flags |= wxALIGN_CENTER;
        if (value & 4)     flags |= wxLEFT;
        if (value & 8)     flags |= wxTOP;
        if (value & 16)    flags |= wxRIGHT;
        if (value & 32)    flags |= wxBOTTOM;
        if (value & 64)    flags |= wxALIGN_LEFT;
        if (value & 128)   flags |= wxALIGN_RIGHT;
        if (value & 256)   flags |= wxALIGN_TOP;
        if (value & 512)   flags |= wxALIGN_BOTTOM;
        if (value & 1024)  flags |= wxALIGN_CENTER_VERTICAL;
        if (value & 2048)  flags |= wxALIGN_CENTER_HORIZONTAL;
        if (value & 4096)  flags |= wxSHAPED;
        if (value & 8192)  flags |= wxFIXED_MINSIZE;
        if (value & 16384) flags |= wxRESERVE_SPACE_EVEN_IF_HIDDEN;
        return flags;
    }

    // The inverse, so a sizer item can report the flags it was given in the managed vocabulary.
    int UnmapSizerFlags(int flags)
    {
        int value = 0;
        if ((flags & wxEXPAND) == wxEXPAND)                                 value |= 1;
        if ((flags & wxALIGN_CENTER) == wxALIGN_CENTER)                     value |= 2;
        if (flags & wxLEFT)                                                 value |= 4;
        if (flags & wxTOP)                                                  value |= 8;
        if (flags & wxRIGHT)                                                value |= 16;
        if (flags & wxBOTTOM)                                               value |= 32;
        if ((flags & wxALIGN_RIGHT) == wxALIGN_RIGHT)                       value |= 128;
        if ((flags & wxALIGN_BOTTOM) == wxALIGN_BOTTOM)                     value |= 512;
        if ((flags & wxALIGN_CENTER_VERTICAL) == wxALIGN_CENTER_VERTICAL)   value |= 1024;
        if ((flags & wxALIGN_CENTER_HORIZONTAL) == wxALIGN_CENTER_HORIZONTAL) value |= 2048;
        if (flags & wxSHAPED)                                               value |= 4096;
        if (flags & wxFIXED_MINSIZE)                                        value |= 8192;
        if (flags & wxRESERVE_SPACE_EVEN_IF_HIDDEN)                         value |= 16384;
        // wxALIGN_LEFT and wxALIGN_TOP are zero in wxWidgets, so they cannot be detected here; a caller
        // that asked for them reads back None for that axis, which is what they mean.
        return value;
    }

    inline int GrowMode(wxFlexSizerGrowMode mode)
    {
        return mode == wxFLEX_GROWMODE_NONE ? 0 : mode == wxFLEX_GROWMODE_SPECIFIED ? 1 : 2;
    }

    inline wxFlexSizerGrowMode MapGrowMode(int mode)
    {
        return mode == 0 ? wxFLEX_GROWMODE_NONE
             : mode == 1 ? wxFLEX_GROWMODE_SPECIFIED
                         : wxFLEX_GROWMODE_ALL;
    }

    inline int Direction(int wxDirection)
    {
        return wxDirection == wxHORIZONTAL ? 0 : wxDirection == wxVERTICAL ? 1 : 2;
    }

    inline int MapDirection(int direction)
    {
        return direction == 0 ? wxHORIZONTAL : direction == 1 ? wxVERTICAL : wxBOTH;
    }

    int CopyInts(const wxArrayInt& values, int* buffer, int buffer_length)
    {
        const int count = static_cast<int>(values.GetCount());
        if (buffer && buffer_length > 0)
        {
            const int n = std::min(count, buffer_length);
            for (int i = 0; i < n; ++i)
                buffer[i] = values[static_cast<size_t>(i)];
        }
        return count;
    }
}

// ---- Construction -------------------------------------------------------------------------------------

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

wxsharp_handle wxsharp_staticboxsizer_create(wxsharp_handle box, bool horizontal)
{
    return new wxStaticBoxSizer(static_cast<wxStaticBox*>(box), horizontal ? wxHORIZONTAL : wxVERTICAL);
}

wxsharp_handle wxsharp_gridbagsizer_create(int verticalGap, int horizontalGap)
{
    return new wxGridBagSizer(verticalGap, horizontalGap);
}

// ---- Adding, inserting, prepending --------------------------------------------------------------------

wxsharp_handle wxsharp_sizer_add_control(wxsharp_handle sizer, wxsharp_handle ctrl, int proportion,
                                         int flags, int border)
{
    return Sz(sizer)->Add(Win(ctrl), proportion, MapSizerFlags(flags), border);
}

wxsharp_handle wxsharp_sizer_add_sizer(wxsharp_handle sizer, wxsharp_handle child, int proportion,
                                       int flags, int border)
{
    return Sz(sizer)->Add(Sz(child), proportion, MapSizerFlags(flags), border);
}

wxsharp_handle wxsharp_sizer_add_spacer(wxsharp_handle sizer, int size) { return Sz(sizer)->AddSpacer(size); }
wxsharp_handle wxsharp_sizer_add_stretch_spacer(wxsharp_handle sizer, int proportion) { return Sz(sizer)->AddStretchSpacer(proportion); }

wxsharp_handle wxsharp_sizer_insert_control(wxsharp_handle sizer, int index, wxsharp_handle ctrl,
                                            int proportion, int flags, int border)
{
    return Sz(sizer)->Insert(static_cast<size_t>(index), Win(ctrl), proportion, MapSizerFlags(flags), border);
}

wxsharp_handle wxsharp_sizer_insert_sizer(wxsharp_handle sizer, int index, wxsharp_handle child,
                                          int proportion, int flags, int border)
{
    return Sz(sizer)->Insert(static_cast<size_t>(index), Sz(child), proportion, MapSizerFlags(flags), border);
}

wxsharp_handle wxsharp_sizer_insert_spacer(wxsharp_handle sizer, int index, int size)
{
    return Sz(sizer)->InsertSpacer(static_cast<size_t>(index), size);
}

wxsharp_handle wxsharp_sizer_insert_stretch_spacer(wxsharp_handle sizer, int index, int proportion)
{
    return Sz(sizer)->InsertStretchSpacer(static_cast<size_t>(index), proportion);
}

wxsharp_handle wxsharp_sizer_prepend_control(wxsharp_handle sizer, wxsharp_handle ctrl, int proportion,
                                             int flags, int border)
{
    return Sz(sizer)->Prepend(Win(ctrl), proportion, MapSizerFlags(flags), border);
}

wxsharp_handle wxsharp_sizer_prepend_sizer(wxsharp_handle sizer, wxsharp_handle child, int proportion,
                                           int flags, int border)
{
    return Sz(sizer)->Prepend(Sz(child), proportion, MapSizerFlags(flags), border);
}

wxsharp_handle wxsharp_sizer_prepend_spacer(wxsharp_handle sizer, int size) { return Sz(sizer)->PrependSpacer(size); }
wxsharp_handle wxsharp_sizer_prepend_stretch_spacer(wxsharp_handle sizer, int proportion) { return Sz(sizer)->PrependStretchSpacer(proportion); }

// ---- Removing -----------------------------------------------------------------------------------------

bool wxsharp_sizer_detach_control(wxsharp_handle sizer, wxsharp_handle ctrl) { return Sz(sizer)->Detach(Win(ctrl)); }
bool wxsharp_sizer_detach_sizer(wxsharp_handle sizer, wxsharp_handle child) { return Sz(sizer)->Detach(Sz(child)); }
bool wxsharp_sizer_detach_at(wxsharp_handle sizer, int index) { return Sz(sizer)->Detach(index); }
bool wxsharp_sizer_remove_sizer(wxsharp_handle sizer, wxsharp_handle child) { return Sz(sizer)->Remove(Sz(child)); }
bool wxsharp_sizer_remove_at(wxsharp_handle sizer, int index) { return Sz(sizer)->Remove(index); }
void wxsharp_sizer_clear(wxsharp_handle sizer, bool delete_windows) { Sz(sizer)->Clear(delete_windows); }
void wxsharp_sizer_delete_windows(wxsharp_handle sizer) { Sz(sizer)->DeleteWindows(); }

bool wxsharp_sizer_replace_control(wxsharp_handle sizer, wxsharp_handle oldCtrl, wxsharp_handle newCtrl,
                                   bool recursive)
{
    return Sz(sizer)->Replace(Win(oldCtrl), Win(newCtrl), recursive);
}

bool wxsharp_sizer_replace_sizer(wxsharp_handle sizer, wxsharp_handle oldSizer, wxsharp_handle newSizer,
                                 bool recursive)
{
    return Sz(sizer)->Replace(Sz(oldSizer), Sz(newSizer), recursive);
}

// ---- Finding items ------------------------------------------------------------------------------------

int wxsharp_sizer_item_count(wxsharp_handle sizer) { return static_cast<int>(Sz(sizer)->GetItemCount()); }
bool wxsharp_sizer_is_empty(wxsharp_handle sizer) { return Sz(sizer)->IsEmpty(); }

wxsharp_handle wxsharp_sizer_item_at(wxsharp_handle sizer, int index)
{
    if (index < 0 || static_cast<size_t>(index) >= Sz(sizer)->GetItemCount())
        return nullptr;
    return Sz(sizer)->GetItem(static_cast<size_t>(index));
}

wxsharp_handle wxsharp_sizer_item_for_control(wxsharp_handle sizer, wxsharp_handle ctrl, bool recursive)
{
    return Sz(sizer)->GetItem(Win(ctrl), recursive);
}

wxsharp_handle wxsharp_sizer_item_for_sizer(wxsharp_handle sizer, wxsharp_handle child, bool recursive)
{
    return Sz(sizer)->GetItem(Sz(child), recursive);
}

wxsharp_handle wxsharp_sizer_item_by_id(wxsharp_handle sizer, int id, bool recursive)
{
    return Sz(sizer)->GetItemById(id, recursive);
}

// ---- Visibility ---------------------------------------------------------------------------------------

bool wxsharp_sizer_show_control(wxsharp_handle sizer, wxsharp_handle ctrl, bool show, bool recursive)
{
    return Sz(sizer)->Show(Win(ctrl), show, recursive);
}

bool wxsharp_sizer_show_sizer(wxsharp_handle sizer, wxsharp_handle child, bool show, bool recursive)
{
    return Sz(sizer)->Show(Sz(child), show, recursive);
}

bool wxsharp_sizer_show_at(wxsharp_handle sizer, int index, bool show)
{
    return Sz(sizer)->Show(static_cast<size_t>(index), show);
}

void wxsharp_sizer_show_items(wxsharp_handle sizer, bool show) { Sz(sizer)->ShowItems(show); }
bool wxsharp_sizer_any_items_shown(wxsharp_handle sizer) { return Sz(sizer)->AreAnyItemsShown(); }
bool wxsharp_sizer_is_shown_control(wxsharp_handle sizer, wxsharp_handle ctrl) { return Sz(sizer)->IsShown(Win(ctrl)); }
bool wxsharp_sizer_is_shown_sizer(wxsharp_handle sizer, wxsharp_handle child) { return Sz(sizer)->IsShown(Sz(child)); }
bool wxsharp_sizer_is_shown_at(wxsharp_handle sizer, int index) { return Sz(sizer)->IsShown(static_cast<size_t>(index)); }

// ---- Layout and measurement ---------------------------------------------------------------------------

void wxsharp_sizer_layout(wxsharp_handle sizer) { Sz(sizer)->Layout(); }

void wxsharp_sizer_fit(wxsharp_handle sizer, wxsharp_handle window, int* width, int* height)
{
    const wxSize size = Sz(sizer)->Fit(Win(window));
    if (width) *width = size.x;
    if (height) *height = size.y;
}

void wxsharp_sizer_fit_inside(wxsharp_handle sizer, wxsharp_handle window) { Sz(sizer)->FitInside(Win(window)); }
void wxsharp_sizer_set_size_hints(wxsharp_handle sizer, wxsharp_handle window) { Sz(sizer)->SetSizeHints(Win(window)); }

void wxsharp_sizer_compute_fitting_client_size(wxsharp_handle sizer, wxsharp_handle window,
                                               int* width, int* height)
{
    const wxSize size = Sz(sizer)->ComputeFittingClientSize(Win(window));
    if (width) *width = size.x;
    if (height) *height = size.y;
}

void wxsharp_sizer_compute_fitting_window_size(wxsharp_handle sizer, wxsharp_handle window,
                                               int* width, int* height)
{
    const wxSize size = Sz(sizer)->ComputeFittingWindowSize(Win(window));
    if (width) *width = size.x;
    if (height) *height = size.y;
}

void wxsharp_sizer_get_min_size(wxsharp_handle sizer, int* width, int* height)
{
    const wxSize size = Sz(sizer)->GetMinSize();
    if (width) *width = size.x;
    if (height) *height = size.y;
}

void wxsharp_sizer_set_min_size(wxsharp_handle sizer, int width, int height)
{
    Sz(sizer)->SetMinSize(wxSize(width, height));
}

void wxsharp_sizer_get_size(wxsharp_handle sizer, int* width, int* height)
{
    const wxSize size = Sz(sizer)->GetSize();
    if (width) *width = size.x;
    if (height) *height = size.y;
}

void wxsharp_sizer_get_position(wxsharp_handle sizer, int* x, int* y)
{
    const wxPoint position = Sz(sizer)->GetPosition();
    if (x) *x = position.x;
    if (y) *y = position.y;
}

void wxsharp_sizer_set_dimension(wxsharp_handle sizer, int x, int y, int width, int height)
{
    Sz(sizer)->SetDimension(x, y, width, height);
}

bool wxsharp_sizer_set_item_min_size_control(wxsharp_handle sizer, wxsharp_handle ctrl, int width, int height)
{
    return Sz(sizer)->SetItemMinSize(Win(ctrl), width, height);
}

bool wxsharp_sizer_set_item_min_size_sizer(wxsharp_handle sizer, wxsharp_handle child, int width, int height)
{
    return Sz(sizer)->SetItemMinSize(Sz(child), width, height);
}

bool wxsharp_sizer_set_item_min_size_at(wxsharp_handle sizer, int index, int width, int height)
{
    return Sz(sizer)->SetItemMinSize(static_cast<size_t>(index), width, height);
}

wxsharp_handle wxsharp_sizer_containing_window(wxsharp_handle sizer) { return Sz(sizer)->GetContainingWindow(); }

// ---- Sizer items --------------------------------------------------------------------------------------

int  wxsharp_sizeritem_get_proportion(wxsharp_handle item) { return It(item)->GetProportion(); }
void wxsharp_sizeritem_set_proportion(wxsharp_handle item, int proportion) { It(item)->SetProportion(proportion); }
int  wxsharp_sizeritem_get_flags(wxsharp_handle item) { return UnmapSizerFlags(It(item)->GetFlag()); }
void wxsharp_sizeritem_set_flags(wxsharp_handle item, int flags) { It(item)->SetFlag(MapSizerFlags(flags)); }
int  wxsharp_sizeritem_get_border(wxsharp_handle item) { return It(item)->GetBorder(); }
void wxsharp_sizeritem_set_border(wxsharp_handle item, int border) { It(item)->SetBorder(border); }
int  wxsharp_sizeritem_get_id(wxsharp_handle item) { return It(item)->GetId(); }
void wxsharp_sizeritem_set_id(wxsharp_handle item, int id) { It(item)->SetId(id); }
bool wxsharp_sizeritem_is_window(wxsharp_handle item) { return It(item)->IsWindow(); }
bool wxsharp_sizeritem_is_sizer(wxsharp_handle item) { return It(item)->IsSizer(); }
bool wxsharp_sizeritem_is_spacer(wxsharp_handle item) { return It(item)->IsSpacer(); }
wxsharp_handle wxsharp_sizeritem_get_window(wxsharp_handle item) { return It(item)->GetWindow(); }
wxsharp_handle wxsharp_sizeritem_get_sizer(wxsharp_handle item) { return It(item)->GetSizer(); }
bool wxsharp_sizeritem_is_shown(wxsharp_handle item) { return It(item)->IsShown(); }
void wxsharp_sizeritem_show(wxsharp_handle item, bool show) { It(item)->Show(show); }

void wxsharp_sizeritem_get_min_size(wxsharp_handle item, int* width, int* height)
{
    const wxSize size = It(item)->GetMinSize();
    if (width) *width = size.x;
    if (height) *height = size.y;
}

void wxsharp_sizeritem_set_min_size(wxsharp_handle item, int width, int height)
{
    It(item)->SetMinSize(wxSize(width, height));
}

void wxsharp_sizeritem_get_size(wxsharp_handle item, int* width, int* height)
{
    const wxSize size = It(item)->GetSize();
    if (width) *width = size.x;
    if (height) *height = size.y;
}

void wxsharp_sizeritem_get_position(wxsharp_handle item, int* x, int* y)
{
    const wxPoint position = It(item)->GetPosition();
    if (x) *x = position.x;
    if (y) *y = position.y;
}

// ---- wxBoxSizer ---------------------------------------------------------------------------------------

int  wxsharp_boxsizer_get_orientation(wxsharp_handle sizer)
{
    return static_cast<wxBoxSizer*>(sizer)->GetOrientation() == wxVERTICAL ? 1 : 0;
}

void wxsharp_boxsizer_set_orientation(wxsharp_handle sizer, bool vertical)
{
    static_cast<wxBoxSizer*>(sizer)->SetOrientation(vertical ? wxVERTICAL : wxHORIZONTAL);
}

// ---- wxGridSizer --------------------------------------------------------------------------------------

int  wxsharp_gridsizer_get_rows(wxsharp_handle sizer) { return static_cast<wxGridSizer*>(sizer)->GetRows(); }
int  wxsharp_gridsizer_get_columns(wxsharp_handle sizer) { return static_cast<wxGridSizer*>(sizer)->GetCols(); }
void wxsharp_gridsizer_set_rows(wxsharp_handle sizer, int rows) { static_cast<wxGridSizer*>(sizer)->SetRows(rows); }
void wxsharp_gridsizer_set_columns(wxsharp_handle sizer, int columns) { static_cast<wxGridSizer*>(sizer)->SetCols(columns); }
int  wxsharp_gridsizer_get_vertical_gap(wxsharp_handle sizer) { return static_cast<wxGridSizer*>(sizer)->GetVGap(); }
int  wxsharp_gridsizer_get_horizontal_gap(wxsharp_handle sizer) { return static_cast<wxGridSizer*>(sizer)->GetHGap(); }
void wxsharp_gridsizer_set_vertical_gap(wxsharp_handle sizer, int gap) { static_cast<wxGridSizer*>(sizer)->SetVGap(gap); }
void wxsharp_gridsizer_set_horizontal_gap(wxsharp_handle sizer, int gap) { static_cast<wxGridSizer*>(sizer)->SetHGap(gap); }
int  wxsharp_gridsizer_effective_rows(wxsharp_handle sizer) { return static_cast<wxGridSizer*>(sizer)->GetEffectiveRowsCount(); }
int  wxsharp_gridsizer_effective_columns(wxsharp_handle sizer) { return static_cast<wxGridSizer*>(sizer)->GetEffectiveColsCount(); }

// ---- wxFlexGridSizer ----------------------------------------------------------------------------------

void wxsharp_flexgridsizer_add_growable_row(wxsharp_handle sizer, int row, int proportion)
{
    static_cast<wxFlexGridSizer*>(sizer)->AddGrowableRow(row, proportion);
}

void wxsharp_flexgridsizer_add_growable_column(wxsharp_handle sizer, int column, int proportion)
{
    static_cast<wxFlexGridSizer*>(sizer)->AddGrowableCol(column, proportion);
}

void wxsharp_flexgridsizer_remove_growable_row(wxsharp_handle sizer, int row)
{
    static_cast<wxFlexGridSizer*>(sizer)->RemoveGrowableRow(row);
}

void wxsharp_flexgridsizer_remove_growable_column(wxsharp_handle sizer, int column)
{
    static_cast<wxFlexGridSizer*>(sizer)->RemoveGrowableCol(column);
}

bool wxsharp_flexgridsizer_is_row_growable(wxsharp_handle sizer, int row)
{
    return static_cast<wxFlexGridSizer*>(sizer)->IsRowGrowable(row);
}

bool wxsharp_flexgridsizer_is_column_growable(wxsharp_handle sizer, int column)
{
    return static_cast<wxFlexGridSizer*>(sizer)->IsColGrowable(column);
}

int  wxsharp_flexgridsizer_get_flexible_direction(wxsharp_handle sizer)
{
    return Direction(static_cast<wxFlexGridSizer*>(sizer)->GetFlexibleDirection());
}

void wxsharp_flexgridsizer_set_flexible_direction(wxsharp_handle sizer, int direction)
{
    static_cast<wxFlexGridSizer*>(sizer)->SetFlexibleDirection(MapDirection(direction));
}

int  wxsharp_flexgridsizer_get_grow_mode(wxsharp_handle sizer)
{
    return GrowMode(static_cast<wxFlexGridSizer*>(sizer)->GetNonFlexibleGrowMode());
}

void wxsharp_flexgridsizer_set_grow_mode(wxsharp_handle sizer, int mode)
{
    static_cast<wxFlexGridSizer*>(sizer)->SetNonFlexibleGrowMode(MapGrowMode(mode));
}

int wxsharp_flexgridsizer_row_heights(wxsharp_handle sizer, int* buffer, int buffer_length)
{
    return CopyInts(static_cast<wxFlexGridSizer*>(sizer)->GetRowHeights(), buffer, buffer_length);
}

int wxsharp_flexgridsizer_column_widths(wxsharp_handle sizer, int* buffer, int buffer_length)
{
    return CopyInts(static_cast<wxFlexGridSizer*>(sizer)->GetColWidths(), buffer, buffer_length);
}

// ---- wxStaticBoxSizer ---------------------------------------------------------------------------------

wxsharp_handle wxsharp_staticboxsizer_get_box(wxsharp_handle sizer)
{
    return static_cast<wxStaticBoxSizer*>(sizer)->GetStaticBox();
}

// ---- wxGridBagSizer -----------------------------------------------------------------------------------

wxsharp_handle wxsharp_gridbagsizer_add_control(wxsharp_handle sizer, wxsharp_handle ctrl, int row, int column,
                                                int rowSpan, int columnSpan, int flags, int border)
{
    return Gb(sizer)->Add(Win(ctrl), wxGBPosition(row, column), wxGBSpan(rowSpan, columnSpan),
                          MapSizerFlags(flags), border);
}

wxsharp_handle wxsharp_gridbagsizer_add_sizer(wxsharp_handle sizer, wxsharp_handle child, int row, int column,
                                              int rowSpan, int columnSpan, int flags, int border)
{
    return Gb(sizer)->Add(Sz(child), wxGBPosition(row, column), wxGBSpan(rowSpan, columnSpan),
                          MapSizerFlags(flags), border);
}

// wxGridBagSizer addresses its items by window, nested sizer or index rather than by item pointer, so
// these mirror those overloads instead of taking a wxSizerItem.
void wxsharp_gridbagsizer_get_item_position_control(wxsharp_handle sizer, wxsharp_handle ctrl, int* row, int* column)
{
    const wxGBPosition position = Gb(sizer)->GetItemPosition(Win(ctrl));
    if (row) *row = position.GetRow();
    if (column) *column = position.GetCol();
}

void wxsharp_gridbagsizer_get_item_position_at(wxsharp_handle sizer, int index, int* row, int* column)
{
    const wxGBPosition position = Gb(sizer)->GetItemPosition(static_cast<size_t>(index));
    if (row) *row = position.GetRow();
    if (column) *column = position.GetCol();
}

bool wxsharp_gridbagsizer_set_item_position_control(wxsharp_handle sizer, wxsharp_handle ctrl, int row, int column)
{
    return Gb(sizer)->SetItemPosition(Win(ctrl), wxGBPosition(row, column));
}

bool wxsharp_gridbagsizer_set_item_position_at(wxsharp_handle sizer, int index, int row, int column)
{
    return Gb(sizer)->SetItemPosition(static_cast<size_t>(index), wxGBPosition(row, column));
}

void wxsharp_gridbagsizer_get_item_span_control(wxsharp_handle sizer, wxsharp_handle ctrl, int* rowSpan, int* columnSpan)
{
    const wxGBSpan span = Gb(sizer)->GetItemSpan(Win(ctrl));
    if (rowSpan) *rowSpan = span.GetRowspan();
    if (columnSpan) *columnSpan = span.GetColspan();
}

void wxsharp_gridbagsizer_get_item_span_at(wxsharp_handle sizer, int index, int* rowSpan, int* columnSpan)
{
    const wxGBSpan span = Gb(sizer)->GetItemSpan(static_cast<size_t>(index));
    if (rowSpan) *rowSpan = span.GetRowspan();
    if (columnSpan) *columnSpan = span.GetColspan();
}

bool wxsharp_gridbagsizer_set_item_span_control(wxsharp_handle sizer, wxsharp_handle ctrl, int rowSpan, int columnSpan)
{
    return Gb(sizer)->SetItemSpan(Win(ctrl), wxGBSpan(rowSpan, columnSpan));
}

bool wxsharp_gridbagsizer_set_item_span_at(wxsharp_handle sizer, int index, int rowSpan, int columnSpan)
{
    return Gb(sizer)->SetItemSpan(static_cast<size_t>(index), wxGBSpan(rowSpan, columnSpan));
}

wxsharp_handle wxsharp_gridbagsizer_find_item_control(wxsharp_handle sizer, wxsharp_handle ctrl)
{
    return Gb(sizer)->FindItem(Win(ctrl));
}

wxsharp_handle wxsharp_gridbagsizer_find_item_sizer(wxsharp_handle sizer, wxsharp_handle child)
{
    return Gb(sizer)->FindItem(Sz(child));
}

wxsharp_handle wxsharp_gridbagsizer_find_item_at_position(wxsharp_handle sizer, int row, int column)
{
    return Gb(sizer)->FindItemAtPosition(wxGBPosition(row, column));
}

wxsharp_handle wxsharp_gridbagsizer_find_item_at_point(wxsharp_handle sizer, int x, int y)
{
    return Gb(sizer)->FindItemAtPoint(wxPoint(x, y));
}

void wxsharp_gridbagsizer_get_cell_size(wxsharp_handle sizer, int row, int column, int* width, int* height)
{
    const wxSize size = Gb(sizer)->GetCellSize(row, column);
    if (width) *width = size.x;
    if (height) *height = size.y;
}

void wxsharp_gridbagsizer_get_empty_cell_size(wxsharp_handle sizer, int* width, int* height)
{
    const wxSize size = Gb(sizer)->GetEmptyCellSize();
    if (width) *width = size.x;
    if (height) *height = size.y;
}

void wxsharp_gridbagsizer_set_empty_cell_size(wxsharp_handle sizer, int width, int height)
{
    Gb(sizer)->SetEmptyCellSize(wxSize(width, height));
}

// Every item in a grid-bag sizer is a wxGBSizerItem, so the cast is sound for anything this sizer produced.
bool wxsharp_gridbagsizer_check_for_intersection(wxsharp_handle sizer, int row, int column,
                                                 int rowSpan, int columnSpan, wxsharp_handle exclude)
{
    return Gb(sizer)->CheckForIntersection(wxGBPosition(row, column), wxGBSpan(rowSpan, columnSpan),
                                           exclude ? static_cast<wxGBSizerItem*>(It(exclude)) : nullptr);
}

// ---- Adopting a sizer ---------------------------------------------------------------------------------

void wxsharp_window_set_sizer(wxsharp_handle window, wxsharp_handle sizer)
{
    auto* w = Win(window);
    w->SetSizer(Sz(sizer));
    w->Layout();
}

void wxsharp_window_set_sizer_and_fit(wxsharp_handle window, wxsharp_handle sizer)
{
    Win(window)->SetSizerAndFit(Sz(sizer));
}

wxsharp_handle wxsharp_window_get_sizer(wxsharp_handle window) { return Win(window)->GetSizer(); }
wxsharp_handle wxsharp_window_containing_sizer(wxsharp_handle window) { return Win(window)->GetContainingSizer(); }
