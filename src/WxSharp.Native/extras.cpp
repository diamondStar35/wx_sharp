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
}

wxsharp_handle wxsharp_togglebutton_create(wxsharp_handle parent, int id, const char* label, long long token)
{
    auto* control = Common(new wxToggleButton(static_cast<wxWindow*>(parent), id, Str(label)), token);
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
bool wxsharp_gauge_is_vertical(wxsharp_handle ctrl) { return static_cast<wxGauge*>(ctrl)->IsVertical(); }
#if defined(_MSC_VER)
#pragma warning(push)
#pragma warning(disable: 4996)
#elif defined(__GNUC__)
#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wdeprecated-declarations"
#endif
int wxsharp_gauge_get_bezel_face(wxsharp_handle ctrl) { return static_cast<wxGauge*>(ctrl)->GetBezelFace(); }
void wxsharp_gauge_set_bezel_face(wxsharp_handle ctrl, int width) { static_cast<wxGauge*>(ctrl)->SetBezelFace(width); }
int wxsharp_gauge_get_shadow_width(wxsharp_handle ctrl) { return static_cast<wxGauge*>(ctrl)->GetShadowWidth(); }
void wxsharp_gauge_set_shadow_width(wxsharp_handle ctrl, int width) { static_cast<wxGauge*>(ctrl)->SetShadowWidth(width); }
#if defined(_MSC_VER)
#pragma warning(pop)
#elif defined(__GNUC__)
#pragma GCC diagnostic pop
#endif

wxsharp_handle wxsharp_spinctrl_create(wxsharp_handle parent, int id, int minValue, int maxValue, int value, long long token)
{
    auto* control = Common(new wxSpinCtrl(static_cast<wxWindow*>(parent), id, wxEmptyString,
        wxDefaultPosition, wxDefaultSize, wxSP_ARROW_KEYS, minValue, maxValue, value), token);
    return control;
}
int wxsharp_spinctrl_get(wxsharp_handle ctrl) { return static_cast<wxSpinCtrl*>(ctrl)->GetValue(); }
void wxsharp_spinctrl_set(wxsharp_handle ctrl, int value) { static_cast<wxSpinCtrl*>(ctrl)->SetValue(value); }
void wxsharp_spinctrl_set_range(wxsharp_handle ctrl, int minValue, int maxValue) { static_cast<wxSpinCtrl*>(ctrl)->SetRange(minValue, maxValue); }
int wxsharp_spinctrl_get_min(wxsharp_handle ctrl) { return static_cast<wxSpinCtrl*>(ctrl)->GetMin(); }
int wxsharp_spinctrl_get_max(wxsharp_handle ctrl) { return static_cast<wxSpinCtrl*>(ctrl)->GetMax(); }
int wxsharp_spinctrl_get_increment(wxsharp_handle ctrl) { return static_cast<wxSpinCtrl*>(ctrl)->GetIncrement(); }
void wxsharp_spinctrl_set_increment(wxsharp_handle ctrl, int increment) { static_cast<wxSpinCtrl*>(ctrl)->SetIncrement(increment); }
int wxsharp_spinctrl_get_base(wxsharp_handle ctrl) { return static_cast<wxSpinCtrl*>(ctrl)->GetBase(); }
bool wxsharp_spinctrl_set_base(wxsharp_handle ctrl, int base) { return static_cast<wxSpinCtrl*>(ctrl)->SetBase(base); }
int wxsharp_spinctrl_get_text_value(wxsharp_handle ctrl, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxSpinCtrl*>(ctrl)->GetTextValue(), buffer, length);
}
void wxsharp_spinctrl_set_text_value(wxsharp_handle ctrl, const char* value)
{
    static_cast<wxSpinCtrl*>(ctrl)->SetValue(Str(value));
}
void wxsharp_spinctrl_set_selection(wxsharp_handle ctrl, int from, int to)
{
    static_cast<wxSpinCtrl*>(ctrl)->SetSelection(from, to);
}

wxsharp_handle wxsharp_combobox_create(wxsharp_handle parent, int id, const char* value, bool readOnly, long long token)
{
    auto* control = Common(new wxComboBox(static_cast<wxWindow*>(parent), id, Str(value), wxDefaultPosition,
        wxDefaultSize, 0, nullptr, readOnly ? wxCB_READONLY : 0), token);
    return control;
}
int wxsharp_combobox_get_value(wxsharp_handle ctrl, char* buffer, int length) { return CopyToBuffer(static_cast<wxComboBox*>(ctrl)->GetValue(), buffer, length); }
void wxsharp_combobox_set_value(wxsharp_handle ctrl, const char* value) { static_cast<wxComboBox*>(ctrl)->SetValue(Str(value)); }
void wxsharp_combobox_append(wxsharp_handle ctrl, const char* value) { static_cast<wxComboBox*>(ctrl)->Append(Str(value)); }
void wxsharp_combobox_insert(wxsharp_handle ctrl, const char* value, int index) { static_cast<wxComboBox*>(ctrl)->Insert(Str(value), index); }
void wxsharp_combobox_delete(wxsharp_handle ctrl, int index) { static_cast<wxComboBox*>(ctrl)->Delete(index); }
void wxsharp_combobox_clear(wxsharp_handle ctrl) { static_cast<wxComboBox*>(ctrl)->Clear(); }
int wxsharp_combobox_get_string(wxsharp_handle ctrl, int index, char* buffer, int buffer_length) { return CopyToBuffer(static_cast<wxComboBox*>(ctrl)->GetString(index), buffer, buffer_length); }
void wxsharp_combobox_set_string(wxsharp_handle ctrl, int index, const char* text) { static_cast<wxComboBox*>(ctrl)->SetString(index, Str(text)); }
int wxsharp_combobox_find_string(wxsharp_handle ctrl, const char* text) { return static_cast<wxComboBox*>(ctrl)->FindString(Str(text)); }
int wxsharp_combobox_count(wxsharp_handle ctrl) { return static_cast<int>(static_cast<wxComboBox*>(ctrl)->GetCount()); }
int wxsharp_combobox_get_selection(wxsharp_handle ctrl) { return static_cast<wxComboBox*>(ctrl)->GetSelection(); }
void wxsharp_combobox_set_selection(wxsharp_handle ctrl, int value) { static_cast<wxComboBox*>(ctrl)->SetSelection(value); }

wxsharp_handle wxsharp_searchctrl_create(wxsharp_handle parent, int id, const char* value, long long token)
{
    auto* control = Common(new wxSearchCtrl(static_cast<wxWindow*>(parent), id, Str(value)), token);
    return control;
}
int wxsharp_searchctrl_get_value(wxsharp_handle ctrl, char* buffer, int length) { return CopyToBuffer(static_cast<wxSearchCtrl*>(ctrl)->GetValue(), buffer, length); }
void wxsharp_searchctrl_set_value(wxsharp_handle ctrl, const char* value) { static_cast<wxSearchCtrl*>(ctrl)->SetValue(Str(value)); }
void wxsharp_searchctrl_show_cancel(wxsharp_handle ctrl, bool show) { static_cast<wxSearchCtrl*>(ctrl)->ShowCancelButton(show); }
void wxsharp_searchctrl_show_search(wxsharp_handle ctrl, bool show) { static_cast<wxSearchCtrl*>(ctrl)->ShowSearchButton(show); }
bool wxsharp_searchctrl_is_cancel_visible(wxsharp_handle ctrl) { return static_cast<wxSearchCtrl*>(ctrl)->IsCancelButtonVisible(); }
bool wxsharp_searchctrl_is_search_visible(wxsharp_handle ctrl) { return static_cast<wxSearchCtrl*>(ctrl)->IsSearchButtonVisible(); }
int wxsharp_searchctrl_get_descriptive_text(wxsharp_handle ctrl, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxSearchCtrl*>(ctrl)->GetDescriptiveText(), buffer, length);
}
void wxsharp_searchctrl_set_descriptive_text(wxsharp_handle ctrl, const char* text)
{
    static_cast<wxSearchCtrl*>(ctrl)->SetDescriptiveText(Str(text));
}
wxsharp_handle wxsharp_searchctrl_get_menu(wxsharp_handle ctrl)
{
    return static_cast<wxSearchCtrl*>(ctrl)->GetMenu();
}
void wxsharp_searchctrl_set_menu(wxsharp_handle ctrl, wxsharp_handle menu)
{
    static_cast<wxSearchCtrl*>(ctrl)->SetMenu(static_cast<wxMenu*>(menu));
}
void wxsharp_searchctrl_set_search_bitmap(wxsharp_handle ctrl, wxsharp_handle bitmap)
{
    static_cast<wxSearchCtrl*>(ctrl)->SetSearchBitmap(*static_cast<wxBitmap*>(bitmap));
}
void wxsharp_searchctrl_set_search_menu_bitmap(wxsharp_handle ctrl, wxsharp_handle bitmap)
{
    static_cast<wxSearchCtrl*>(ctrl)->SetSearchMenuBitmap(*static_cast<wxBitmap*>(bitmap));
}
void wxsharp_searchctrl_set_cancel_bitmap(wxsharp_handle ctrl, wxsharp_handle bitmap)
{
    static_cast<wxSearchCtrl*>(ctrl)->SetCancelBitmap(*static_cast<wxBitmap*>(bitmap));
}

wxsharp_handle wxsharp_checklistbox_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxCheckListBox(static_cast<wxWindow*>(parent), id), token);
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
    return control;
}
int wxsharp_radiobox_get_selection(wxsharp_handle ctrl) { return static_cast<wxRadioBox*>(ctrl)->GetSelection(); }
void wxsharp_radiobox_set_selection(wxsharp_handle ctrl, int selection) { static_cast<wxRadioBox*>(ctrl)->SetSelection(selection); }

wxsharp_handle wxsharp_staticbox_create(wxsharp_handle parent, int id, const char* label, long long token) { return Common(new wxStaticBox(static_cast<wxWindow*>(parent), id, Str(label)), token); }
wxsharp_handle wxsharp_staticline_create(wxsharp_handle parent, int id, bool vertical, long long token) { return Common(new wxStaticLine(static_cast<wxWindow*>(parent), id, wxDefaultPosition, wxDefaultSize, vertical ? wxLI_VERTICAL : wxLI_HORIZONTAL), token); }
void wxsharp_staticbox_get_borders(wxsharp_handle ctrl, int* top, int* other) { static_cast<wxStaticBox*>(ctrl)->GetBordersForSizer(top, other); }
bool wxsharp_staticline_is_vertical(wxsharp_handle ctrl) { return static_cast<wxStaticLine*>(ctrl)->IsVertical(); }
int wxsharp_staticline_default_size() { return wxStaticLine::GetDefaultSize(); }
wxsharp_handle wxsharp_activity_create(wxsharp_handle parent, int id, long long token) { return Common(new wxActivityIndicator(static_cast<wxWindow*>(parent), id), token); }
void wxsharp_activity_start(wxsharp_handle ctrl) { static_cast<wxActivityIndicator*>(ctrl)->Start(); }
void wxsharp_activity_stop(wxsharp_handle ctrl) { static_cast<wxActivityIndicator*>(ctrl)->Stop(); }
bool wxsharp_activity_is_running(wxsharp_handle ctrl) { return static_cast<wxActivityIndicator*>(ctrl)->IsRunning(); }

wxsharp_handle wxsharp_spinctrldouble_create(wxsharp_handle parent, int id, double minValue, double maxValue,
                                             double value, double increment, long long token)
{
    auto* control = Common(new wxSpinCtrlDouble(static_cast<wxWindow*>(parent), id, wxEmptyString,
        wxDefaultPosition, wxDefaultSize, wxSP_ARROW_KEYS, minValue, maxValue, value, increment), token);
    return control;
}
double wxsharp_spinctrldouble_get(wxsharp_handle ctrl) { return static_cast<wxSpinCtrlDouble*>(ctrl)->GetValue(); }
void wxsharp_spinctrldouble_set(wxsharp_handle ctrl, double value) { static_cast<wxSpinCtrlDouble*>(ctrl)->SetValue(value); }
double wxsharp_spinctrldouble_get_min(wxsharp_handle ctrl) { return static_cast<wxSpinCtrlDouble*>(ctrl)->GetMin(); }
double wxsharp_spinctrldouble_get_max(wxsharp_handle ctrl) { return static_cast<wxSpinCtrlDouble*>(ctrl)->GetMax(); }
double wxsharp_spinctrldouble_get_increment(wxsharp_handle ctrl) { return static_cast<wxSpinCtrlDouble*>(ctrl)->GetIncrement(); }
void wxsharp_spinctrldouble_set_increment(wxsharp_handle ctrl, double increment) { static_cast<wxSpinCtrlDouble*>(ctrl)->SetIncrement(increment); }
unsigned int wxsharp_spinctrldouble_get_digits(wxsharp_handle ctrl) { return static_cast<wxSpinCtrlDouble*>(ctrl)->GetDigits(); }
void wxsharp_spinctrldouble_set_digits(wxsharp_handle ctrl, unsigned int digits) { static_cast<wxSpinCtrlDouble*>(ctrl)->SetDigits(digits); }
void wxsharp_spinctrldouble_set_range(wxsharp_handle ctrl, double minValue, double maxValue)
{
    static_cast<wxSpinCtrlDouble*>(ctrl)->SetRange(minValue, maxValue);
}
int wxsharp_spinctrldouble_get_text_value(wxsharp_handle ctrl, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxSpinCtrlDouble*>(ctrl)->GetTextValue(), buffer, length);
}
void wxsharp_spinctrldouble_set_text_value(wxsharp_handle ctrl, const char* value)
{
    static_cast<wxSpinCtrlDouble*>(ctrl)->SetValue(Str(value));
}
wxsharp_handle wxsharp_scrollbar_create(wxsharp_handle parent, int id, bool vertical, long long token)
{
    auto* control = Common(new wxScrollBar(static_cast<wxWindow*>(parent), id, wxDefaultPosition,
        wxDefaultSize, vertical ? wxSB_VERTICAL : wxSB_HORIZONTAL), token);
    return control;
}
void wxsharp_scrollbar_set(wxsharp_handle ctrl, int position, int thumbSize, int range, int pageSize) { static_cast<wxScrollBar*>(ctrl)->SetScrollbar(position, thumbSize, range, pageSize); }
int wxsharp_scrollbar_get_position(wxsharp_handle ctrl) { return static_cast<wxScrollBar*>(ctrl)->GetThumbPosition(); }
void wxsharp_scrollbar_set_ex(wxsharp_handle ctrl, int position, int thumbSize, int range, int pageSize, bool refresh)
{
    static_cast<wxScrollBar*>(ctrl)->SetScrollbar(position, thumbSize, range, pageSize, refresh);
}
void wxsharp_scrollbar_set_position(wxsharp_handle ctrl, int position) { static_cast<wxScrollBar*>(ctrl)->SetThumbPosition(position); }
int wxsharp_scrollbar_get_thumb_size(wxsharp_handle ctrl) { return static_cast<wxScrollBar*>(ctrl)->GetThumbSize(); }
int wxsharp_scrollbar_get_range(wxsharp_handle ctrl) { return static_cast<wxScrollBar*>(ctrl)->GetRange(); }
int wxsharp_scrollbar_get_page_size(wxsharp_handle ctrl) { return static_cast<wxScrollBar*>(ctrl)->GetPageSize(); }
bool wxsharp_scrollbar_is_vertical(wxsharp_handle ctrl) { return static_cast<wxScrollBar*>(ctrl)->IsVertical(); }
wxsharp_handle wxsharp_hyperlink_create(wxsharp_handle parent, int id, const char* label, const char* url, long long token)
{
    auto* control = Common(new wxHyperlinkCtrl(static_cast<wxWindow*>(parent), id, Str(label), Str(url)), token);
    return control;
}
int wxsharp_hyperlink_get_url(wxsharp_handle ctrl, char* buffer, int length) { return CopyToBuffer(static_cast<wxHyperlinkCtrl*>(ctrl)->GetURL(), buffer, length); }
void wxsharp_hyperlink_set_url(wxsharp_handle ctrl, const char* url) { static_cast<wxHyperlinkCtrl*>(ctrl)->SetURL(Str(url)); }
bool wxsharp_hyperlink_get_visited(wxsharp_handle ctrl) { return static_cast<wxHyperlinkCtrl*>(ctrl)->GetVisited(); }
void wxsharp_hyperlink_set_visited(wxsharp_handle ctrl, bool visited) { static_cast<wxHyperlinkCtrl*>(ctrl)->SetVisited(visited); }
unsigned int wxsharp_hyperlink_get_normal_colour(wxsharp_handle ctrl) { return ArgbFromColour(static_cast<wxHyperlinkCtrl*>(ctrl)->GetNormalColour()); }
void wxsharp_hyperlink_set_normal_colour(wxsharp_handle ctrl, unsigned int colour) { static_cast<wxHyperlinkCtrl*>(ctrl)->SetNormalColour(ColourFromArgb(colour)); }
unsigned int wxsharp_hyperlink_get_hover_colour(wxsharp_handle ctrl) { return ArgbFromColour(static_cast<wxHyperlinkCtrl*>(ctrl)->GetHoverColour()); }
void wxsharp_hyperlink_set_hover_colour(wxsharp_handle ctrl, unsigned int colour) { static_cast<wxHyperlinkCtrl*>(ctrl)->SetHoverColour(ColourFromArgb(colour)); }
unsigned int wxsharp_hyperlink_get_visited_colour(wxsharp_handle ctrl) { return ArgbFromColour(static_cast<wxHyperlinkCtrl*>(ctrl)->GetVisitedColour()); }
void wxsharp_hyperlink_set_visited_colour(wxsharp_handle ctrl, unsigned int colour) { static_cast<wxHyperlinkCtrl*>(ctrl)->SetVisitedColour(ColourFromArgb(colour)); }
wxsharp_handle wxsharp_datepicker_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxDatePickerCtrl(static_cast<wxWindow*>(parent), id), token);
    return control;
}
wxsharp_handle wxsharp_timepicker_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxTimePickerCtrl(static_cast<wxWindow*>(parent), id), token);
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
bool wxsharp_datepicker_get_range(wxsharp_handle ctrl, int* y1, int* m1, int* d1, int* y2, int* m2, int* d2)
{
    wxDateTime lower, upper;
    if (!static_cast<wxDatePickerCtrl*>(ctrl)->GetRange(&lower, &upper)) return false;
    *y1 = lower.GetYear(); *m1 = static_cast<int>(lower.GetMonth()) + 1; *d1 = lower.GetDay();
    *y2 = upper.GetYear(); *m2 = static_cast<int>(upper.GetMonth()) + 1; *d2 = upper.GetDay();
    return true;
}
void wxsharp_datepicker_set_range(wxsharp_handle ctrl, int y1, int m1, int d1, int y2, int m2, int d2)
{
    const wxDateTime lower(static_cast<wxDateTime::wxDateTime_t>(d1), static_cast<wxDateTime::Month>(m1 - 1), y1);
    const wxDateTime upper(static_cast<wxDateTime::wxDateTime_t>(d2), static_cast<wxDateTime::Month>(m2 - 1), y2);
    static_cast<wxDatePickerCtrl*>(ctrl)->SetRange(lower, upper);
}
void wxsharp_datepicker_set_null_text(wxsharp_handle ctrl, const char* text)
{
    static_cast<wxDatePickerCtrl*>(ctrl)->SetNullText(Str(text));
}

wxsharp_handle wxsharp_scrolled_create(wxsharp_handle parent, int id, int style, long long token)
{
    return Common(new wxScrolledWindow(static_cast<wxWindow*>(parent), id, wxDefaultPosition, wxDefaultSize,
                                       MapScrolledStyle(style)), token);
}
void wxsharp_scrolled_set_rate(wxsharp_handle ctrl, int x, int y) { static_cast<wxScrolledWindow*>(ctrl)->SetScrollRate(x, y); }
void wxsharp_scrolled_scroll(wxsharp_handle ctrl, int x, int y) { static_cast<wxScrolledWindow*>(ctrl)->Scroll(x, y); }
void wxsharp_scrolled_get_view_start(wxsharp_handle ctrl, int* x, int* y) { static_cast<wxScrolledWindow*>(ctrl)->GetViewStart(x, y); }
void wxsharp_scrolled_set_scrollbars(wxsharp_handle ctrl, int pixels_x, int pixels_y, int units_x,
                                     int units_y, int pos_x, int pos_y, bool no_refresh)
{
    static_cast<wxScrolledWindow*>(ctrl)->SetScrollbars(pixels_x, pixels_y, units_x, units_y, pos_x, pos_y,
                                                        no_refresh);
}
void wxsharp_scrolled_enable_scrolling(wxsharp_handle ctrl, bool x, bool y) { static_cast<wxScrolledWindow*>(ctrl)->EnableScrolling(x, y); }
namespace
{
    // The managed enums are semantic; the wx flags they mean are resolved here, so no wxWidgets constant
    // has to be transcribed into managed code where it could drift from the header.
    wxScrollbarVisibility ScrollbarVisibility(int v)
    {
        switch (v)
        {
            case 0:  return wxSHOW_SB_NEVER;
            case 2:  return wxSHOW_SB_ALWAYS;
            default: return wxSHOW_SB_DEFAULT;
        }
    }

    int ScrollOrientation(int v) { return v == 1 ? wxVERTICAL : wxHORIZONTAL; }
}

void wxsharp_scrolled_show_scrollbars(wxsharp_handle ctrl, int x, int y)
{
    static_cast<wxScrolledWindow*>(ctrl)->ShowScrollbars(ScrollbarVisibility(x), ScrollbarVisibility(y));
}
void wxsharp_scrolled_get_pixels_per_unit(wxsharp_handle ctrl, int* x, int* y) { static_cast<wxScrolledWindow*>(ctrl)->GetScrollPixelsPerUnit(x, y); }
void wxsharp_scrolled_set_target_window(wxsharp_handle ctrl, wxsharp_handle target) { static_cast<wxScrolledWindow*>(ctrl)->SetTargetWindow(static_cast<wxWindow*>(target)); }
void wxsharp_scrolled_set_scroll_page_size(wxsharp_handle ctrl, int orientation, int size) { static_cast<wxScrolledWindow*>(ctrl)->SetScrollPageSize(ScrollOrientation(orientation), size); }
int  wxsharp_scrolled_get_scroll_page_size(wxsharp_handle ctrl, int orientation) { return static_cast<wxScrolledWindow*>(ctrl)->GetScrollPageSize(ScrollOrientation(orientation)); }

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
    return control;
}

wxsharp_virtual_list_cb g_virtual_list_cb = nullptr;

namespace
{
    // Only wxLC_VIRTUAL makes wxWidgets ask these questions; for an ordinary list the override never runs.
    class WxSharpListCtrl : public wxListCtrl
    {
    public:
        WxSharpListCtrl(wxWindow* parent, int id, long style, long long token)
            : wxListCtrl(parent, id, wxDefaultPosition, wxDefaultSize, style), m_token(token) {}

    protected:
        wxString OnGetItemText(long item, long column) const override
        {
            if (!g_virtual_list_cb)
                return wxString();

            // Ask once with a stack buffer, and again only if the text did not fit.
            char stack[512];
            wxsharp_virtual_list_request request = {};
            request.size = sizeof(request);
            request.version = 1;
            request.token = m_token;
            request.item = item;
            request.column = static_cast<int>(column);
            request.operation = 1;
            request.buffer = stack;
            request.buffer_length = static_cast<int>(sizeof(stack));
            if (!g_virtual_list_cb(&request))
                return wxString();
            if (request.required_length < static_cast<int>(sizeof(stack)))
                return wxString::FromUTF8(stack);

            std::vector<char> heap(static_cast<size_t>(request.required_length) + 1);
            request.buffer = heap.data();
            request.buffer_length = static_cast<int>(heap.size());
            if (!g_virtual_list_cb(&request))
                return wxString();
            return wxString::FromUTF8(heap.data());
        }

        int OnGetItemImage(long item) const override
        {
            return AskInteger(2, item, 0, -1);
        }

        int OnGetItemColumnImage(long item, long column) const override
        {
            return AskInteger(3, item, column, -1);
        }

        bool OnGetItemIsChecked(long item) const override
        {
            return AskInteger(4, item, 0, 0) != 0;
        }

    private:
        int AskInteger(int operation, long item, long column, int fallback) const
        {
            if (!g_virtual_list_cb)
                return fallback;
            wxsharp_virtual_list_request request = {};
            request.size = sizeof(request);
            request.version = 1;
            request.token = m_token;
            request.item = item;
            request.column = static_cast<int>(column);
            request.operation = operation;
            request.result = fallback;
            return g_virtual_list_cb(&request) ? request.result : fallback;
        }

        long long m_token;
    };
}

void wxsharp_set_virtual_list_handler(wxsharp_virtual_list_cb cb) { g_virtual_list_cb = cb; }

wxsharp_handle wxsharp_listctrl_create(wxsharp_handle parent, int id, int style, long long token)
{
    return Common(new WxSharpListCtrl(static_cast<wxWindow*>(parent), id, MapListCtrlStyle(style), token),
                  token);
}

// wxLC_VIRTUAL asks its control for cell text, so the overridable list control has to build on the subclass
// that answers that rather than on wxListCtrl - otherwise a virtual list that is also subclassed would lose
// its rows. Its constructor takes the token last, where Overridable takes it first, so it is passed twice:
// once to route the virtuals and once for the cell-text callback.
wxsharp_handle wxsharp_custom_listctrl_create(wxsharp_handle parent, int id, int style, long long token)
{
    return Common(new Overridable<WxSharpListCtrl>(token, static_cast<wxWindow*>(parent), id,
                                                   MapListCtrlStyle(style), token),
                  token);
}

void wxsharp_listctrl_set_item_count(wxsharp_handle ctrl, long long count)
{
    static_cast<wxListCtrl*>(ctrl)->SetItemCount(static_cast<long>(count));
}

void wxsharp_listctrl_refresh_item(wxsharp_handle ctrl, long long item)
{
    static_cast<wxListCtrl*>(ctrl)->RefreshItem(static_cast<long>(item));
}

void wxsharp_listctrl_refresh_items(wxsharp_handle ctrl, long long from, long long to)
{
    static_cast<wxListCtrl*>(ctrl)->RefreshItems(static_cast<long>(from), static_cast<long>(to));
}

int wxsharp_listctrl_column_count(wxsharp_handle ctrl) { return static_cast<wxListCtrl*>(ctrl)->GetColumnCount(); }
bool wxsharp_listctrl_delete_column(wxsharp_handle ctrl, int column) { return static_cast<wxListCtrl*>(ctrl)->DeleteColumn(column); }
void wxsharp_listctrl_clear_columns(wxsharp_handle ctrl) { static_cast<wxListCtrl*>(ctrl)->DeleteAllColumns(); }
int wxsharp_listctrl_get_column_width(wxsharp_handle ctrl, int column) { return static_cast<wxListCtrl*>(ctrl)->GetColumnWidth(column); }

// A negative width auto-sizes: -1 to the widest cell, -2 to the header.
bool wxsharp_listctrl_set_column_width(wxsharp_handle ctrl, int column, int width)
{
    const int resolved = width == -1 ? wxLIST_AUTOSIZE : width == -2 ? wxLIST_AUTOSIZE_USEHEADER : width;
    return static_cast<wxListCtrl*>(ctrl)->SetColumnWidth(column, resolved);
}

int wxsharp_listctrl_get_column_heading(wxsharp_handle ctrl, int column, char* buffer, int buffer_length)
{
    wxListItem item;
    item.SetMask(wxLIST_MASK_TEXT);
    if (!static_cast<wxListCtrl*>(ctrl)->GetColumn(column, item))
        return CopyToBuffer(wxString(), buffer, buffer_length);
    return CopyToBuffer(item.GetText(), buffer, buffer_length);
}

bool wxsharp_listctrl_set_column_heading(wxsharp_handle ctrl, int column, const char* heading)
{
    auto* list = static_cast<wxListCtrl*>(ctrl);
    wxListItem item;
    item.SetMask(wxLIST_MASK_TEXT);
    if (!list->GetColumn(column, item))
        return false;
    item.SetText(Str(heading));
    return list->SetColumn(column, item);
}

void wxsharp_listctrl_ensure_visible(wxsharp_handle ctrl, long long item) { static_cast<wxListCtrl*>(ctrl)->EnsureVisible(static_cast<long>(item)); }
long long wxsharp_listctrl_get_focused(wxsharp_handle ctrl) { return static_cast<wxListCtrl*>(ctrl)->GetNextItem(-1, wxLIST_NEXT_ALL, wxLIST_STATE_FOCUSED); }

// Focus is what a screen reader follows, and it is separate from selection.
void wxsharp_listctrl_set_focused(wxsharp_handle ctrl, long long item)
{
    static_cast<wxListCtrl*>(ctrl)->SetItemState(static_cast<long>(item), wxLIST_STATE_FOCUSED, wxLIST_STATE_FOCUSED);
}

int wxsharp_listctrl_selected_count(wxsharp_handle ctrl) { return static_cast<wxListCtrl*>(ctrl)->GetSelectedItemCount(); }
long long wxsharp_listctrl_next_selected(wxsharp_handle ctrl, long long after)
{
    return static_cast<wxListCtrl*>(ctrl)->GetNextItem(static_cast<long>(after), wxLIST_NEXT_ALL, wxLIST_STATE_SELECTED);
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

wxsharp_handle wxsharp_treectrl_create(wxsharp_handle parent, int id, int style, long long token)
{
    return Common(new wxTreeCtrl(static_cast<wxWindow*>(parent), id, wxDefaultPosition, wxDefaultSize,
                                 MapTreeCtrlStyle(style)), token);
}

void wxsharp_tree_unselect(wxsharp_handle ctrl) { static_cast<wxTreeCtrl*>(ctrl)->UnselectAll(); }
long long wxsharp_tree_get_root(wxsharp_handle ctrl) { return TreeValue(static_cast<wxTreeCtrl*>(ctrl)->GetRootItem()); }
long long wxsharp_tree_get_parent(wxsharp_handle ctrl, long long item) { return TreeValue(static_cast<wxTreeCtrl*>(ctrl)->GetItemParent(TreeId(item))); }

long long wxsharp_tree_get_first_child(wxsharp_handle ctrl, long long item)
{
    wxTreeItemIdValue cookie;
    return TreeValue(static_cast<wxTreeCtrl*>(ctrl)->GetFirstChild(TreeId(item), cookie));
}

long long wxsharp_tree_get_next_sibling(wxsharp_handle ctrl, long long item) { return TreeValue(static_cast<wxTreeCtrl*>(ctrl)->GetNextSibling(TreeId(item))); }
long long wxsharp_tree_get_prev_sibling(wxsharp_handle ctrl, long long item) { return TreeValue(static_cast<wxTreeCtrl*>(ctrl)->GetPrevSibling(TreeId(item))); }
int wxsharp_tree_child_count(wxsharp_handle ctrl, long long item, bool recursive) { return static_cast<int>(static_cast<wxTreeCtrl*>(ctrl)->GetChildrenCount(TreeId(item), recursive)); }
void wxsharp_tree_ensure_visible(wxsharp_handle ctrl, long long item) { static_cast<wxTreeCtrl*>(ctrl)->EnsureVisible(TreeId(item)); }
void wxsharp_tree_sort_children(wxsharp_handle ctrl, long long item) { static_cast<wxTreeCtrl*>(ctrl)->SortChildren(TreeId(item)); }
long long wxsharp_tree_insert(wxsharp_handle ctrl, long long parent, int position, const char* text) { return TreeValue(static_cast<wxTreeCtrl*>(ctrl)->InsertItem(TreeId(parent), static_cast<size_t>(position), Str(text))); }
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
int wxsharp_tree_get_count(wxsharp_handle ctrl) { return static_cast<int>(static_cast<wxTreeCtrl*>(ctrl)->GetCount()); }
void wxsharp_tree_expand_all(wxsharp_handle ctrl) { static_cast<wxTreeCtrl*>(ctrl)->ExpandAll(); }
void wxsharp_tree_collapse_all(wxsharp_handle ctrl) { static_cast<wxTreeCtrl*>(ctrl)->CollapseAll(); }
bool wxsharp_tree_item_has_children(wxsharp_handle ctrl, long long item)
{
    return static_cast<wxTreeCtrl*>(ctrl)->ItemHasChildren(TreeId(item));
}

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

wxsharp_handle wxsharp_dataviewtree_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new wxDataViewTreeCtrl(static_cast<wxWindow*>(parent), id), token);
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
    // A timer's owner is any wxEvtHandler, which is why a null window handle here means the application:
    // wxApp is an event handler too, and an application-owned timer has no window to hang from.
    wxEvtHandler* TimerOwner(wxsharp_handle owner)
    {
        return owner ? static_cast<wxEvtHandler*>(static_cast<wxWindow*>(owner))
                     : static_cast<wxEvtHandler*>(wxTheApp);
    }

    class WxSharpTimer final : public wxTimer
    {
    public:
        WxSharpTimer(wxEvtHandler* owner, int id, long long token) : wxTimer(owner, id), m_token(token) {}
        void Notify() override { Fire(m_token, WXSHARP_EV_TIMER, GetId()); }
        void SetManagedOwner(wxEvtHandler* owner, int id, long long token)
        {
            SetOwner(owner, id);
            m_token = token;
        }
    private:
        long long m_token;
    };
}
wxsharp_handle wxsharp_timer_create(wxsharp_handle owner, int id, long long ownerToken)
{
    return new WxSharpTimer(TimerOwner(owner), id, ownerToken);
}
void wxsharp_timer_destroy(wxsharp_handle timer) { delete static_cast<WxSharpTimer*>(timer); }
bool wxsharp_timer_start(wxsharp_handle timer, int milliseconds, bool oneShot) { return static_cast<WxSharpTimer*>(timer)->Start(milliseconds, oneShot); }
bool wxsharp_timer_start_once(wxsharp_handle timer, int milliseconds) { return static_cast<WxSharpTimer*>(timer)->StartOnce(milliseconds); }
void wxsharp_timer_stop(wxsharp_handle timer) { static_cast<WxSharpTimer*>(timer)->Stop(); }
bool wxsharp_timer_is_running(wxsharp_handle timer) { return static_cast<WxSharpTimer*>(timer)->IsRunning(); }
bool wxsharp_timer_is_one_shot(wxsharp_handle timer) { return static_cast<WxSharpTimer*>(timer)->IsOneShot(); }
int wxsharp_timer_get_interval(wxsharp_handle timer) { return static_cast<WxSharpTimer*>(timer)->GetInterval(); }
void wxsharp_timer_notify(wxsharp_handle timer) { static_cast<WxSharpTimer*>(timer)->Notify(); }
int wxsharp_timer_get_id(wxsharp_handle timer) { return static_cast<WxSharpTimer*>(timer)->GetId(); }
void wxsharp_timer_set_owner(wxsharp_handle timer, wxsharp_handle owner, int id, long long ownerToken)
{
    static_cast<WxSharpTimer*>(timer)->SetManagedOwner(TimerOwner(owner), id, ownerToken);
}

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
wxsharp_handle wxsharp_staticbitmap_get(wxsharp_handle ctrl) { return new wxBitmap(static_cast<wxStaticBitmap*>(ctrl)->GetBitmap()); }
void wxsharp_staticbitmap_set_icon(wxsharp_handle ctrl, wxsharp_handle icon) { static_cast<wxStaticBitmap*>(ctrl)->SetIcon(*static_cast<wxIcon*>(icon)); }
wxsharp_handle wxsharp_staticbitmap_get_icon(wxsharp_handle ctrl) { return new wxIcon(static_cast<wxStaticBitmap*>(ctrl)->GetIcon()); }
void wxsharp_staticbitmap_set_scale_mode(wxsharp_handle ctrl, int mode) { static_cast<wxStaticBitmap*>(ctrl)->SetScaleMode(static_cast<wxStaticBitmap::ScaleMode>(mode)); }
int wxsharp_staticbitmap_get_scale_mode(wxsharp_handle ctrl) { return static_cast<int>(static_cast<wxStaticBitmap*>(ctrl)->GetScaleMode()); }
wxsharp_handle wxsharp_bitmapbutton_create(wxsharp_handle parent, int id, wxsharp_handle bitmap, long long token)
{
    auto* control = Common(new wxBitmapButton(static_cast<wxWindow*>(parent), id, *static_cast<wxBitmap*>(bitmap)), token);
    return control;
}
wxsharp_handle wxsharp_bitmapbutton_new_close(wxsharp_handle parent, int id, const char* name, long long token)
{
    return Common(wxBitmapButton::NewCloseButton(static_cast<wxWindow*>(parent), id, Str(name)), token);
}
void wxsharp_bitmapbutton_set_margins(wxsharp_handle ctrl, int x, int y) { static_cast<wxBitmapButton*>(ctrl)->SetMargins(x, y); }
int wxsharp_bitmapbutton_get_margin_x(wxsharp_handle ctrl) { return static_cast<wxBitmapButton*>(ctrl)->GetMarginX(); }
int wxsharp_bitmapbutton_get_margin_y(wxsharp_handle ctrl) { return static_cast<wxBitmapButton*>(ctrl)->GetMarginY(); }
wxsharp_handle wxsharp_icon_load(const char* path)
{
    auto* icon = new wxIcon(Str(path), wxBITMAP_TYPE_ANY); if (!icon->IsOk()) { delete icon; return nullptr; } return icon;
}
void wxsharp_icon_destroy(wxsharp_handle icon) { delete static_cast<wxIcon*>(icon); }
void wxsharp_frame_set_icon(wxsharp_handle frame, wxsharp_handle icon) { static_cast<wxFrame*>(frame)->SetIcon(*static_cast<wxIcon*>(icon)); }
void wxsharp_begin_busy_cursor() { wxBeginBusyCursor(); }
void wxsharp_end_busy_cursor() { if (wxIsBusy()) wxEndBusyCursor(); }
wxsharp_handle wxsharp_progress_create(wxsharp_handle parent, const char* title, const char* message,
                                       int maximum, int style, long long token)
{
    auto* dlg = new wxProgressDialog(Str(title), Str(message), maximum, static_cast<wxWindow*>(parent),
                                     MapProgressStyle(style));
    TrackWindow(dlg, token);
    return dlg;
}
bool wxsharp_progress_update(wxsharp_handle progress, int value, const char* message, bool* continueRunning)
{
    bool skip = false; *continueRunning = static_cast<wxProgressDialog*>(progress)->Update(value, Str(message), &skip); return skip;
}
bool wxsharp_progress_pulse(wxsharp_handle progress, const char* message, bool* continueRunning)
{
    bool skip = false; *continueRunning = static_cast<wxProgressDialog*>(progress)->Pulse(Str(message), &skip); return skip;
}
bool wxsharp_progress_was_cancelled(wxsharp_handle progress) { return static_cast<wxProgressDialog*>(progress)->WasCancelled(); }
bool wxsharp_progress_was_skipped(wxsharp_handle progress) { return static_cast<wxProgressDialog*>(progress)->WasSkipped(); }
void wxsharp_progress_resume(wxsharp_handle progress) { static_cast<wxProgressDialog*>(progress)->Resume(); }
int wxsharp_progress_get_value(wxsharp_handle progress) { return static_cast<wxProgressDialog*>(progress)->GetValue(); }
int wxsharp_progress_get_range(wxsharp_handle progress) { return static_cast<wxProgressDialog*>(progress)->GetRange(); }
void wxsharp_progress_set_range(wxsharp_handle progress, int range) { static_cast<wxProgressDialog*>(progress)->SetRange(range); }
int wxsharp_progress_get_message(wxsharp_handle progress, char* buffer, int buffer_length)
{
    return CopyToBuffer(static_cast<wxProgressDialog*>(progress)->GetMessage(), buffer, buffer_length);
}

// Destroyed at once rather than scheduled. An app-modal progress dialog holds a wxWindowDisabler for as long
// as it exists, so deferring the deletion to the next idle cycle leaves the rest of the application disabled
// until then - a caller that has finished with the dialog finds its own windows dead with no way to say why.
void wxsharp_progress_destroy(wxsharp_handle progress) { delete static_cast<wxProgressDialog*>(progress); }
// Destroyed at once rather than scheduled. An app-modal progress dialog holds a wxWindowDisabler for as
// long as it exists, so deferring its deletion to the next idle cycle would leave the rest of the
// application disabled until then - a caller that has finished with the dialog would find its own windows
// dead with no way to say why.
