#include "internal.h"
#include <wx/activityindicator.h>
#include <wx/checklst.h>
#include <wx/combobox.h>
#include <wx/dataview.h>
#include <wx/gauge.h>
#include <wx/datectrl.h>
#include <wx/dateevt.h>
#include <wx/hyperlink.h>
#include <wx/grid.h>
#include <wx/listctrl.h>
#include <wx/notebook.h>
#include <wx/radiobox.h>
#include <wx/scrolwin.h>
#include <wx/srchctrl.h>
#include <wx/simplebook.h>
#include <wx/statbox.h>
#include <wx/statline.h>
#include <wx/spinctrl.h>
#include <wx/splitter.h>
#include <wx/timectrl.h>
#include <wx/tglbtn.h>
#include <wx/treectrl.h>
#include <wx/timer.h>
#include <wx/statbmp.h>
#include <wx/bmpbuttn.h>
#include <wx/progdlg.h>

namespace
{
    template<typename T> T* Common(T* control, long long token)
    {
        BindCommon(control, token);
        return control;
    }

    wxTreeItemId TreeId(long long value)
    {
        return wxTreeItemId(reinterpret_cast<void*>(static_cast<intptr_t>(value)));
    }

    long long TreeValue(const wxTreeItemId& value)
    {
        return static_cast<long long>(reinterpret_cast<intptr_t>(value.GetID()));
    }
}

wxsharp_handle wxsharp_togglebutton_create(wxsharp_handle parent, int id, const char* label, long long token)
{
    auto* control = Common(new wxToggleButton(static_cast<wxWindow*>(parent), id, Str(label)), token);
    control->Bind(wxEVT_TOGGLEBUTTON, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_TOGGLE, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
bool wxsharp_togglebutton_get(wxsharp_handle ctrl) { return static_cast<wxToggleButton*>(ctrl)->GetValue(); }
void wxsharp_togglebutton_set(wxsharp_handle ctrl, bool value) { static_cast<wxToggleButton*>(ctrl)->SetValue(value); }

wxsharp_handle wxsharp_gauge_create(wxsharp_handle parent, int id, int range, int value, bool vertical, long long token)
{
    auto* control = Common(new wxGauge(static_cast<wxWindow*>(parent), id, range, wxDefaultPosition,
        wxDefaultSize, vertical ? wxGA_VERTICAL : wxGA_HORIZONTAL), token);
    control->SetValue(value);
    return control;
}
int wxsharp_gauge_get(wxsharp_handle ctrl) { return static_cast<wxGauge*>(ctrl)->GetValue(); }
void wxsharp_gauge_set(wxsharp_handle ctrl, int value) { static_cast<wxGauge*>(ctrl)->SetValue(value); }
int wxsharp_gauge_get_range(wxsharp_handle ctrl) { return static_cast<wxGauge*>(ctrl)->GetRange(); }
void wxsharp_gauge_set_range(wxsharp_handle ctrl, int range) { static_cast<wxGauge*>(ctrl)->SetRange(range); }
void wxsharp_gauge_pulse(wxsharp_handle ctrl) { static_cast<wxGauge*>(ctrl)->Pulse(); }

wxsharp_handle wxsharp_spinctrl_create(wxsharp_handle parent, int id, int minValue, int maxValue, int value, long long token)
{
    auto* control = Common(new wxSpinCtrl(static_cast<wxWindow*>(parent), id, wxEmptyString,
        wxDefaultPosition, wxDefaultSize, wxSP_ARROW_KEYS, minValue, maxValue, value), token);
    control->Bind(wxEVT_SPINCTRL, [token](wxSpinEvent& e) { if (!(Fire(token, WXSHARP_EVT_SLIDER, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
int wxsharp_spinctrl_get(wxsharp_handle ctrl) { return static_cast<wxSpinCtrl*>(ctrl)->GetValue(); }
void wxsharp_spinctrl_set(wxsharp_handle ctrl, int value) { static_cast<wxSpinCtrl*>(ctrl)->SetValue(value); }
void wxsharp_spinctrl_set_range(wxsharp_handle ctrl, int minValue, int maxValue) { static_cast<wxSpinCtrl*>(ctrl)->SetRange(minValue, maxValue); }

wxsharp_handle wxsharp_combobox_create(wxsharp_handle parent, int id, const char* value, bool readOnly, long long token)
{
    auto* control = Common(new wxComboBox(static_cast<wxWindow*>(parent), id, Str(value), wxDefaultPosition,
        wxDefaultSize, 0, nullptr, readOnly ? wxCB_READONLY : 0), token);
    control->Bind(wxEVT_COMBOBOX, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    control->Bind(wxEVT_TEXT, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_TEXT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
int wxsharp_combobox_get_value(wxsharp_handle ctrl, char* buffer, int length) { return CopyToBuffer(static_cast<wxComboBox*>(ctrl)->GetValue(), buffer, length); }
void wxsharp_combobox_set_value(wxsharp_handle ctrl, const char* value) { static_cast<wxComboBox*>(ctrl)->SetValue(Str(value)); }
void wxsharp_combobox_append(wxsharp_handle ctrl, const char* value) { static_cast<wxComboBox*>(ctrl)->Append(Str(value)); }
void wxsharp_combobox_clear(wxsharp_handle ctrl) { static_cast<wxComboBox*>(ctrl)->Clear(); }
int wxsharp_combobox_count(wxsharp_handle ctrl) { return static_cast<int>(static_cast<wxComboBox*>(ctrl)->GetCount()); }
int wxsharp_combobox_get_selection(wxsharp_handle ctrl) { return static_cast<wxComboBox*>(ctrl)->GetSelection(); }
void wxsharp_combobox_set_selection(wxsharp_handle ctrl, int value) { static_cast<wxComboBox*>(ctrl)->SetSelection(value); }

wxsharp_handle wxsharp_searchctrl_create(wxsharp_handle parent, int id, const char* value, long long token)
{
    auto* control = Common(new wxSearchCtrl(static_cast<wxWindow*>(parent), id, Str(value)), token);
    control->Bind(wxEVT_TEXT, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_TEXT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    control->Bind(wxEVT_SEARCHCTRL_SEARCH_BTN, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_TEXT_ENTER, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
int wxsharp_searchctrl_get_value(wxsharp_handle ctrl, char* buffer, int length) { return CopyToBuffer(static_cast<wxSearchCtrl*>(ctrl)->GetValue(), buffer, length); }
void wxsharp_searchctrl_set_value(wxsharp_handle ctrl, const char* value) { static_cast<wxSearchCtrl*>(ctrl)->SetValue(Str(value)); }
void wxsharp_searchctrl_show_cancel(wxsharp_handle ctrl, bool show) { static_cast<wxSearchCtrl*>(ctrl)->ShowCancelButton(show); }
void wxsharp_searchctrl_show_search(wxsharp_handle ctrl, bool show) { static_cast<wxSearchCtrl*>(ctrl)->ShowSearchButton(show); }

wxsharp_handle wxsharp_checklistbox_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxCheckListBox(static_cast<wxWindow*>(parent), id), token);
    control->Bind(wxEVT_CHECKLISTBOX, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_TOGGLE, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    control->Bind(wxEVT_LISTBOX, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
void wxsharp_checklistbox_append(wxsharp_handle ctrl, const char* value) { static_cast<wxCheckListBox*>(ctrl)->Append(Str(value)); }
int wxsharp_checklistbox_count(wxsharp_handle ctrl) { return static_cast<int>(static_cast<wxCheckListBox*>(ctrl)->GetCount()); }
bool wxsharp_checklistbox_is_checked(wxsharp_handle ctrl, int index) { return static_cast<wxCheckListBox*>(ctrl)->IsChecked(static_cast<unsigned int>(index)); }
void wxsharp_checklistbox_check(wxsharp_handle ctrl, int index, bool value) { static_cast<wxCheckListBox*>(ctrl)->Check(static_cast<unsigned int>(index), value); }

wxsharp_handle wxsharp_radiobox_create(wxsharp_handle parent, int id, const char* label, const char* const* choices,
                                       int count, int columns, long long token)
{
    wxArrayString items;
    for (int i = 0; i < count; ++i) items.Add(Str(choices[i]));
    auto* control = Common(new wxRadioBox(static_cast<wxWindow*>(parent), id, Str(label), wxDefaultPosition,
        wxDefaultSize, items, columns > 0 ? columns : 1, wxRA_SPECIFY_COLS), token);
    control->Bind(wxEVT_RADIOBOX, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
int wxsharp_radiobox_get_selection(wxsharp_handle ctrl) { return static_cast<wxRadioBox*>(ctrl)->GetSelection(); }
void wxsharp_radiobox_set_selection(wxsharp_handle ctrl, int selection) { static_cast<wxRadioBox*>(ctrl)->SetSelection(selection); }

wxsharp_handle wxsharp_staticbox_create(wxsharp_handle parent, int id, const char* label, long long token) { return Common(new wxStaticBox(static_cast<wxWindow*>(parent), id, Str(label)), token); }
wxsharp_handle wxsharp_staticline_create(wxsharp_handle parent, int id, bool vertical, long long token) { return Common(new wxStaticLine(static_cast<wxWindow*>(parent), id, wxDefaultPosition, wxDefaultSize, vertical ? wxLI_VERTICAL : wxLI_HORIZONTAL), token); }
wxsharp_handle wxsharp_activity_create(wxsharp_handle parent, int id, long long token) { return Common(new wxActivityIndicator(static_cast<wxWindow*>(parent), id), token); }
void wxsharp_activity_start(wxsharp_handle ctrl) { static_cast<wxActivityIndicator*>(ctrl)->Start(); }
void wxsharp_activity_stop(wxsharp_handle ctrl) { static_cast<wxActivityIndicator*>(ctrl)->Stop(); }
bool wxsharp_activity_is_running(wxsharp_handle ctrl) { return static_cast<wxActivityIndicator*>(ctrl)->IsRunning(); }

wxsharp_handle wxsharp_spinctrldouble_create(wxsharp_handle parent, int id, double minValue, double maxValue,
                                             double value, double increment, long long token)
{
    auto* control = Common(new wxSpinCtrlDouble(static_cast<wxWindow*>(parent), id, wxEmptyString,
        wxDefaultPosition, wxDefaultSize, wxSP_ARROW_KEYS, minValue, maxValue, value, increment), token);
    control->Bind(wxEVT_SPINCTRLDOUBLE, [token](wxSpinDoubleEvent& e) { if (!(Fire(token, WXSHARP_EVT_SLIDER, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
double wxsharp_spinctrldouble_get(wxsharp_handle ctrl) { return static_cast<wxSpinCtrlDouble*>(ctrl)->GetValue(); }
void wxsharp_spinctrldouble_set(wxsharp_handle ctrl, double value) { static_cast<wxSpinCtrlDouble*>(ctrl)->SetValue(value); }
wxsharp_handle wxsharp_scrollbar_create(wxsharp_handle parent, int id, bool vertical, long long token)
{
    auto* control = Common(new wxScrollBar(static_cast<wxWindow*>(parent), id, wxDefaultPosition,
        wxDefaultSize, vertical ? wxSB_VERTICAL : wxSB_HORIZONTAL), token);
    control->Bind(wxEVT_SCROLL_THUMBTRACK, [token](wxScrollEvent& e) { if (!(Fire(token, WXSHARP_EVT_SLIDER, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
void wxsharp_scrollbar_set(wxsharp_handle ctrl, int position, int thumbSize, int range, int pageSize) { static_cast<wxScrollBar*>(ctrl)->SetScrollbar(position, thumbSize, range, pageSize); }
int wxsharp_scrollbar_get_position(wxsharp_handle ctrl) { return static_cast<wxScrollBar*>(ctrl)->GetThumbPosition(); }
wxsharp_handle wxsharp_hyperlink_create(wxsharp_handle parent, int id, const char* label, const char* url, long long token)
{
    auto* control = Common(new wxHyperlinkCtrl(static_cast<wxWindow*>(parent), id, Str(label), Str(url)), token);
    control->Bind(wxEVT_HYPERLINK, [token](wxHyperlinkEvent& e) { if (!(Fire(token, WXSHARP_EVT_CLICK, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
int wxsharp_hyperlink_get_url(wxsharp_handle ctrl, char* buffer, int length) { return CopyToBuffer(static_cast<wxHyperlinkCtrl*>(ctrl)->GetURL(), buffer, length); }
void wxsharp_hyperlink_set_url(wxsharp_handle ctrl, const char* url) { static_cast<wxHyperlinkCtrl*>(ctrl)->SetURL(Str(url)); }
wxsharp_handle wxsharp_datepicker_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxDatePickerCtrl(static_cast<wxWindow*>(parent), id), token);
    control->Bind(wxEVT_DATE_CHANGED, [token](wxDateEvent& e) { if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
wxsharp_handle wxsharp_timepicker_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxTimePickerCtrl(static_cast<wxWindow*>(parent), id), token);
    control->Bind(wxEVT_TIME_CHANGED, [token](wxDateEvent& e) { if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
void wxsharp_datetime_get(wxsharp_handle ctrl, int* year, int* month, int* day, int* hour, int* minute, int* second)
{
    wxDateTime value;
    if (auto* date = dynamic_cast<wxDatePickerCtrl*>(static_cast<wxWindow*>(ctrl))) value = date->GetValue();
    else value = static_cast<wxTimePickerCtrl*>(ctrl)->GetValue();
    *year = value.GetYear(); *month = static_cast<int>(value.GetMonth()) + 1; *day = value.GetDay();
    *hour = value.GetHour(); *minute = value.GetMinute(); *second = value.GetSecond();
}
void wxsharp_datetime_set(wxsharp_handle ctrl, int year, int month, int day, int hour, int minute, int second)
{
    wxDateTime value(static_cast<wxDateTime::wxDateTime_t>(day), static_cast<wxDateTime::Month>(month - 1), year,
                     static_cast<wxDateTime::wxDateTime_t>(hour), static_cast<wxDateTime::wxDateTime_t>(minute),
                     static_cast<wxDateTime::wxDateTime_t>(second));
    if (auto* date = dynamic_cast<wxDatePickerCtrl*>(static_cast<wxWindow*>(ctrl))) date->SetValue(value);
    else static_cast<wxTimePickerCtrl*>(ctrl)->SetValue(value);
}

wxsharp_handle wxsharp_scrolled_create(wxsharp_handle parent, int id, long long token) { return Common(new wxScrolledWindow(static_cast<wxWindow*>(parent), id), token); }
void wxsharp_scrolled_set_rate(wxsharp_handle ctrl, int x, int y) { static_cast<wxScrolledWindow*>(ctrl)->SetScrollRate(x, y); }
void wxsharp_scrolled_scroll(wxsharp_handle ctrl, int x, int y) { static_cast<wxScrolledWindow*>(ctrl)->Scroll(x, y); }
void wxsharp_scrolled_get_view_start(wxsharp_handle ctrl, int* x, int* y) { static_cast<wxScrolledWindow*>(ctrl)->GetViewStart(x, y); }

wxsharp_handle wxsharp_splitter_create(wxsharp_handle parent, int id, bool vertical, long long token)
{
    auto* control = Common(new wxSplitterWindow(static_cast<wxWindow*>(parent), id), token);
    control->SetSplitMode(vertical ? wxSPLIT_VERTICAL : wxSPLIT_HORIZONTAL);
    return control;
}
bool wxsharp_splitter_split(wxsharp_handle ctrl, wxsharp_handle first, wxsharp_handle second, int position)
{
    auto* splitter = static_cast<wxSplitterWindow*>(ctrl);
    return splitter->GetSplitMode() == wxSPLIT_VERTICAL
        ? splitter->SplitVertically(static_cast<wxWindow*>(first), static_cast<wxWindow*>(second), position)
        : splitter->SplitHorizontally(static_cast<wxWindow*>(first), static_cast<wxWindow*>(second), position);
}
bool wxsharp_splitter_unsplit(wxsharp_handle ctrl, wxsharp_handle remove) { return static_cast<wxSplitterWindow*>(ctrl)->Unsplit(static_cast<wxWindow*>(remove)); }
int wxsharp_splitter_get_position(wxsharp_handle ctrl) { return static_cast<wxSplitterWindow*>(ctrl)->GetSashPosition(); }
void wxsharp_splitter_set_position(wxsharp_handle ctrl, int position) { static_cast<wxSplitterWindow*>(ctrl)->SetSashPosition(position); }

wxsharp_handle wxsharp_notebook_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxNotebook(static_cast<wxWindow*>(parent), id), token);
    control->Bind(wxEVT_NOTEBOOK_PAGE_CHANGED, [token](wxBookCtrlEvent& e) { if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId(), e.GetOldSelection(), e.GetSelection()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
bool wxsharp_notebook_add_page(wxsharp_handle ctrl, wxsharp_handle page, const char* text, bool select) { return static_cast<wxBookCtrlBase*>(ctrl)->AddPage(static_cast<wxWindow*>(page), Str(text), select); }
bool wxsharp_notebook_delete_page(wxsharp_handle ctrl, int page) { return static_cast<wxBookCtrlBase*>(ctrl)->DeletePage(static_cast<size_t>(page)); }
int wxsharp_notebook_count(wxsharp_handle ctrl) { return static_cast<int>(static_cast<wxBookCtrlBase*>(ctrl)->GetPageCount()); }
int wxsharp_notebook_get_selection(wxsharp_handle ctrl) { return static_cast<wxBookCtrlBase*>(ctrl)->GetSelection(); }
int wxsharp_notebook_set_selection(wxsharp_handle ctrl, int page) { return static_cast<wxBookCtrlBase*>(ctrl)->SetSelection(static_cast<size_t>(page)); }
int wxsharp_notebook_get_page_text(wxsharp_handle ctrl, int page, char* buffer, int length) { return CopyToBuffer(static_cast<wxBookCtrlBase*>(ctrl)->GetPageText(static_cast<size_t>(page)), buffer, length); }
bool wxsharp_notebook_set_page_text(wxsharp_handle ctrl, int page, const char* text) { return static_cast<wxBookCtrlBase*>(ctrl)->SetPageText(static_cast<size_t>(page), Str(text)); }
wxsharp_handle wxsharp_simplebook_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxSimplebook(static_cast<wxWindow*>(parent), id), token);
    control->Bind(wxEVT_BOOKCTRL_PAGE_CHANGED, [token](wxBookCtrlEvent& e) { if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId(), e.GetOldSelection(), e.GetSelection()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}

wxsharp_handle wxsharp_listctrl_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxListCtrl(static_cast<wxWindow*>(parent), id, wxDefaultPosition, wxDefaultSize, wxLC_REPORT), token);
    control->Bind(wxEVT_LIST_ITEM_SELECTED, [token](wxListEvent& e) { if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId(), static_cast<int>(e.GetIndex())) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
int wxsharp_listctrl_insert_column(wxsharp_handle ctrl, int column, const char* heading, int width) { return static_cast<int>(static_cast<wxListCtrl*>(ctrl)->InsertColumn(column, Str(heading), wxLIST_FORMAT_LEFT, width)); }
long long wxsharp_listctrl_insert_item(wxsharp_handle ctrl, long long index, const char* text) { return static_cast<wxListCtrl*>(ctrl)->InsertItem(static_cast<long>(index), Str(text)); }
bool wxsharp_listctrl_set_item(wxsharp_handle ctrl, long long item, int column, const char* text) { return static_cast<wxListCtrl*>(ctrl)->SetItem(static_cast<long>(item), column, Str(text)); }
int wxsharp_listctrl_get_item(wxsharp_handle ctrl, long long item, int column, char* buffer, int length) { return CopyToBuffer(static_cast<wxListCtrl*>(ctrl)->GetItemText(static_cast<long>(item), column), buffer, length); }
long long wxsharp_listctrl_count(wxsharp_handle ctrl) { return static_cast<wxListCtrl*>(ctrl)->GetItemCount(); }
bool wxsharp_listctrl_delete_item(wxsharp_handle ctrl, long long item) { return static_cast<wxListCtrl*>(ctrl)->DeleteItem(static_cast<long>(item)); }
void wxsharp_listctrl_clear(wxsharp_handle ctrl) { static_cast<wxListCtrl*>(ctrl)->DeleteAllItems(); }
void wxsharp_listctrl_select(wxsharp_handle ctrl, long long item, bool select) { static_cast<wxListCtrl*>(ctrl)->SetItemState(static_cast<long>(item), select ? wxLIST_STATE_SELECTED : 0, wxLIST_STATE_SELECTED); }
bool wxsharp_listctrl_is_selected(wxsharp_handle ctrl, long long item) { return (static_cast<wxListCtrl*>(ctrl)->GetItemState(static_cast<long>(item), wxLIST_STATE_SELECTED) & wxLIST_STATE_SELECTED) != 0; }

wxsharp_handle wxsharp_treectrl_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxTreeCtrl(static_cast<wxWindow*>(parent), id), token);
    control->Bind(wxEVT_TREE_SEL_CHANGED, [token](wxTreeEvent& e) { if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
long long wxsharp_tree_add_root(wxsharp_handle ctrl, const char* text) { return TreeValue(static_cast<wxTreeCtrl*>(ctrl)->AddRoot(Str(text))); }
long long wxsharp_tree_append(wxsharp_handle ctrl, long long parent, const char* text) { return TreeValue(static_cast<wxTreeCtrl*>(ctrl)->AppendItem(TreeId(parent), Str(text))); }
void wxsharp_tree_delete(wxsharp_handle ctrl, long long item) { static_cast<wxTreeCtrl*>(ctrl)->Delete(TreeId(item)); }
void wxsharp_tree_delete_all(wxsharp_handle ctrl) { static_cast<wxTreeCtrl*>(ctrl)->DeleteAllItems(); }
int wxsharp_tree_get_text(wxsharp_handle ctrl, long long item, char* buffer, int length) { return CopyToBuffer(static_cast<wxTreeCtrl*>(ctrl)->GetItemText(TreeId(item)), buffer, length); }
void wxsharp_tree_set_text(wxsharp_handle ctrl, long long item, const char* text) { static_cast<wxTreeCtrl*>(ctrl)->SetItemText(TreeId(item), Str(text)); }
void wxsharp_tree_expand(wxsharp_handle ctrl, long long item, bool expand) { if (expand) static_cast<wxTreeCtrl*>(ctrl)->Expand(TreeId(item)); else static_cast<wxTreeCtrl*>(ctrl)->Collapse(TreeId(item)); }
bool wxsharp_tree_is_expanded(wxsharp_handle ctrl, long long item) { return static_cast<wxTreeCtrl*>(ctrl)->IsExpanded(TreeId(item)); }
void wxsharp_tree_select(wxsharp_handle ctrl, long long item) { static_cast<wxTreeCtrl*>(ctrl)->SelectItem(TreeId(item)); }
long long wxsharp_tree_get_selection(wxsharp_handle ctrl) { return TreeValue(static_cast<wxTreeCtrl*>(ctrl)->GetSelection()); }

wxsharp_handle wxsharp_grid_create(wxsharp_handle parent, int id, int rows, int columns, long long token)
{
    auto* control = Common(new wxGrid(static_cast<wxWindow*>(parent), id), token);
    control->CreateGrid(rows, columns);
    return control;
}
int wxsharp_grid_rows(wxsharp_handle ctrl) { return static_cast<wxGrid*>(ctrl)->GetNumberRows(); }
int wxsharp_grid_columns(wxsharp_handle ctrl) { return static_cast<wxGrid*>(ctrl)->GetNumberCols(); }
bool wxsharp_grid_append_rows(wxsharp_handle ctrl, int count) { return static_cast<wxGrid*>(ctrl)->AppendRows(count); }
bool wxsharp_grid_append_columns(wxsharp_handle ctrl, int count) { return static_cast<wxGrid*>(ctrl)->AppendCols(count); }
bool wxsharp_grid_delete_rows(wxsharp_handle ctrl, int position, int count) { return static_cast<wxGrid*>(ctrl)->DeleteRows(position, count); }
bool wxsharp_grid_delete_columns(wxsharp_handle ctrl, int position, int count) { return static_cast<wxGrid*>(ctrl)->DeleteCols(position, count); }
int wxsharp_grid_get_value(wxsharp_handle ctrl, int row, int column, char* buffer, int length) { return CopyToBuffer(static_cast<wxGrid*>(ctrl)->GetCellValue(row, column), buffer, length); }
void wxsharp_grid_set_value(wxsharp_handle ctrl, int row, int column, const char* value) { static_cast<wxGrid*>(ctrl)->SetCellValue(row, column, Str(value)); }
void wxsharp_grid_set_row_label(wxsharp_handle ctrl, int row, const char* value) { static_cast<wxGrid*>(ctrl)->SetRowLabelValue(row, Str(value)); }
void wxsharp_grid_set_column_label(wxsharp_handle ctrl, int column, const char* value) { static_cast<wxGrid*>(ctrl)->SetColLabelValue(column, Str(value)); }

wxsharp_handle wxsharp_dataviewlist_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxDataViewListCtrl(static_cast<wxWindow*>(parent), id), token);
    control->Bind(wxEVT_DATAVIEW_SELECTION_CHANGED, [token](wxDataViewEvent& e)
    {
        if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip();
    });
    return control;
}
void wxsharp_dataviewlist_append_text_column(wxsharp_handle ctrl, const char* label, int width, bool editable)
{
    static_cast<wxDataViewListCtrl*>(ctrl)->AppendTextColumn(Str(label), wxDATAVIEW_CELL_INERT,
        width, wxALIGN_LEFT, editable ? wxDATAVIEW_COL_RESIZABLE : wxDATAVIEW_COL_RESIZABLE);
    if (editable)
    {
        auto* column = static_cast<wxDataViewListCtrl*>(ctrl)->GetColumn(static_cast<unsigned int>(static_cast<wxDataViewListCtrl*>(ctrl)->GetColumnCount() - 1));
        if (auto* renderer = column->GetRenderer()) renderer->SetMode(wxDATAVIEW_CELL_EDITABLE);
    }
}
void wxsharp_dataviewlist_append_row(wxsharp_handle ctrl, const char* const* values, int count)
{
    wxVector<wxVariant> row; row.reserve(static_cast<size_t>(count));
    for (int i = 0; i < count; ++i) row.push_back(wxVariant(Str(values[i])));
    static_cast<wxDataViewListCtrl*>(ctrl)->AppendItem(row);
}
int wxsharp_dataviewlist_count(wxsharp_handle ctrl) { return static_cast<int>(static_cast<wxDataViewListCtrl*>(ctrl)->GetItemCount()); }
int wxsharp_dataviewlist_get_value(wxsharp_handle ctrl, int row, int column, char* buffer, int length)
{
    wxVariant value; static_cast<wxDataViewListCtrl*>(ctrl)->GetValue(value, static_cast<unsigned int>(row), static_cast<unsigned int>(column));
    return CopyToBuffer(value.GetString(), buffer, length);
}
void wxsharp_dataviewlist_set_value(wxsharp_handle ctrl, int row, int column, const char* value) { static_cast<wxDataViewListCtrl*>(ctrl)->SetValue(wxVariant(Str(value)), static_cast<unsigned int>(row), static_cast<unsigned int>(column)); }
void wxsharp_dataviewlist_delete_row(wxsharp_handle ctrl, int row) { static_cast<wxDataViewListCtrl*>(ctrl)->DeleteItem(static_cast<unsigned int>(row)); }
void wxsharp_dataviewlist_clear(wxsharp_handle ctrl) { static_cast<wxDataViewListCtrl*>(ctrl)->DeleteAllItems(); }
int wxsharp_dataviewlist_get_selection(wxsharp_handle ctrl)
{
    auto* view = static_cast<wxDataViewListCtrl*>(ctrl); const wxDataViewItem item = view->GetSelection();
    return item.IsOk() ? static_cast<int>(view->ItemToRow(item)) : -1;
}
void wxsharp_dataviewlist_set_selection(wxsharp_handle ctrl, int row)
{
    auto* view = static_cast<wxDataViewListCtrl*>(ctrl); view->Select(view->RowToItem(static_cast<unsigned int>(row)));
}

namespace
{
    wxDataViewItem DataViewId(long long value) { return wxDataViewItem(reinterpret_cast<void*>(static_cast<intptr_t>(value))); }
    long long DataViewValue(const wxDataViewItem& value) { return static_cast<long long>(reinterpret_cast<intptr_t>(value.GetID())); }
}
wxsharp_handle wxsharp_dataviewtree_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxDataViewTreeCtrl(static_cast<wxWindow*>(parent), id), token);
    control->Bind(wxEVT_DATAVIEW_SELECTION_CHANGED, [token](wxDataViewEvent& e)
    {
        if (!(Fire(token, WXSHARP_EVT_SELECT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip();
    });
    return control;
}
long long wxsharp_dataviewtree_append_container(wxsharp_handle ctrl, long long parent, const char* text) { return DataViewValue(static_cast<wxDataViewTreeCtrl*>(ctrl)->AppendContainer(DataViewId(parent), Str(text))); }
long long wxsharp_dataviewtree_append_item(wxsharp_handle ctrl, long long parent, const char* text) { return DataViewValue(static_cast<wxDataViewTreeCtrl*>(ctrl)->AppendItem(DataViewId(parent), Str(text))); }
int wxsharp_dataviewtree_get_text(wxsharp_handle ctrl, long long item, char* buffer, int length) { return CopyToBuffer(static_cast<wxDataViewTreeCtrl*>(ctrl)->GetItemText(DataViewId(item)), buffer, length); }
void wxsharp_dataviewtree_set_text(wxsharp_handle ctrl, long long item, const char* text) { static_cast<wxDataViewTreeCtrl*>(ctrl)->SetItemText(DataViewId(item), Str(text)); }
void wxsharp_dataviewtree_delete(wxsharp_handle ctrl, long long item) { static_cast<wxDataViewTreeCtrl*>(ctrl)->DeleteItem(DataViewId(item)); }
void wxsharp_dataviewtree_clear(wxsharp_handle ctrl) { static_cast<wxDataViewTreeCtrl*>(ctrl)->DeleteAllItems(); }
long long wxsharp_dataviewtree_get_selection(wxsharp_handle ctrl) { return DataViewValue(static_cast<wxDataViewTreeCtrl*>(ctrl)->GetSelection()); }
void wxsharp_dataviewtree_set_selection(wxsharp_handle ctrl, long long item) { static_cast<wxDataViewTreeCtrl*>(ctrl)->Select(DataViewId(item)); }

namespace
{
    class WxSharpTimer final : public wxTimer
    {
    public:
        WxSharpTimer(int id, long long token) : wxTimer(nullptr, id), m_token(token) {}
        void Notify() override { Fire(m_token, WXSHARP_EVT_TIMER, GetId()); }
    private:
        long long m_token;
    };
}
wxsharp_handle wxsharp_timer_create(int id, long long ownerToken) { return new WxSharpTimer(id, ownerToken); }
void wxsharp_timer_destroy(wxsharp_handle timer) { delete static_cast<WxSharpTimer*>(timer); }
bool wxsharp_timer_start(wxsharp_handle timer, int milliseconds, bool oneShot) { return static_cast<WxSharpTimer*>(timer)->Start(milliseconds, oneShot); }
void wxsharp_timer_stop(wxsharp_handle timer) { static_cast<WxSharpTimer*>(timer)->Stop(); }
bool wxsharp_timer_is_running(wxsharp_handle timer) { return static_cast<WxSharpTimer*>(timer)->IsRunning(); }
int wxsharp_timer_get_interval(wxsharp_handle timer) { return static_cast<WxSharpTimer*>(timer)->GetInterval(); }

wxsharp_handle wxsharp_image_load(const char* path)
{
    auto* image = new wxImage(Str(path)); if (!image->IsOk()) { delete image; return nullptr; } return image;
}
void wxsharp_image_destroy(wxsharp_handle image) { delete static_cast<wxImage*>(image); }
int wxsharp_image_width(wxsharp_handle image) { return static_cast<wxImage*>(image)->GetWidth(); }
int wxsharp_image_height(wxsharp_handle image) { return static_cast<wxImage*>(image)->GetHeight(); }
bool wxsharp_image_save(wxsharp_handle image, const char* path) { return static_cast<wxImage*>(image)->SaveFile(Str(path)); }
wxsharp_handle wxsharp_bitmap_load(const char* path)
{
    auto* bitmap = new wxBitmap(Str(path), wxBITMAP_TYPE_ANY); if (!bitmap->IsOk()) { delete bitmap; return nullptr; } return bitmap;
}
wxsharp_handle wxsharp_bitmap_from_image(wxsharp_handle image) { return new wxBitmap(*static_cast<wxImage*>(image)); }
void wxsharp_bitmap_destroy(wxsharp_handle bitmap) { delete static_cast<wxBitmap*>(bitmap); }
int wxsharp_bitmap_width(wxsharp_handle bitmap) { return static_cast<wxBitmap*>(bitmap)->GetWidth(); }
int wxsharp_bitmap_height(wxsharp_handle bitmap) { return static_cast<wxBitmap*>(bitmap)->GetHeight(); }
wxsharp_handle wxsharp_staticbitmap_create(wxsharp_handle parent, int id, wxsharp_handle bitmap, long long token)
{
    return Common(new wxStaticBitmap(static_cast<wxWindow*>(parent), id, *static_cast<wxBitmap*>(bitmap)), token);
}
void wxsharp_staticbitmap_set(wxsharp_handle ctrl, wxsharp_handle bitmap) { static_cast<wxStaticBitmap*>(ctrl)->SetBitmap(*static_cast<wxBitmap*>(bitmap)); }
wxsharp_handle wxsharp_bitmapbutton_create(wxsharp_handle parent, int id, wxsharp_handle bitmap, long long token)
{
    auto* control = Common(new wxBitmapButton(static_cast<wxWindow*>(parent), id, *static_cast<wxBitmap*>(bitmap)), token);
    control->Bind(wxEVT_BUTTON, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_CLICK, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    return control;
}
wxsharp_handle wxsharp_icon_load(const char* path)
{
    auto* icon = new wxIcon(Str(path), wxBITMAP_TYPE_ANY); if (!icon->IsOk()) { delete icon; return nullptr; } return icon;
}
void wxsharp_icon_destroy(wxsharp_handle icon) { delete static_cast<wxIcon*>(icon); }
void wxsharp_frame_set_icon(wxsharp_handle frame, wxsharp_handle icon) { static_cast<wxFrame*>(frame)->SetIcon(*static_cast<wxIcon*>(icon)); }
void wxsharp_begin_busy_cursor() { wxBeginBusyCursor(); }
void wxsharp_end_busy_cursor() { if (wxIsBusy()) wxEndBusyCursor(); }
wxsharp_handle wxsharp_progress_create(wxsharp_handle parent, const char* title, const char* message, int maximum)
{
    return new wxProgressDialog(Str(title), Str(message), maximum, static_cast<wxWindow*>(parent),
        wxPD_APP_MODAL | wxPD_AUTO_HIDE | wxPD_CAN_ABORT | wxPD_ELAPSED_TIME | wxPD_REMAINING_TIME);
}
bool wxsharp_progress_update(wxsharp_handle progress, int value, const char* message, bool* continueRunning)
{
    bool skip = false; *continueRunning = static_cast<wxProgressDialog*>(progress)->Update(value, Str(message), &skip); return skip;
}
bool wxsharp_progress_pulse(wxsharp_handle progress, const char* message, bool* continueRunning)
{
    bool skip = false; *continueRunning = static_cast<wxProgressDialog*>(progress)->Pulse(Str(message), &skip); return skip;
}
void wxsharp_progress_destroy(wxsharp_handle progress) { delete static_cast<wxProgressDialog*>(progress); }
