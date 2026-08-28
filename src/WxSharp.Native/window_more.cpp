// The parts of wxWindow the wrapper had not reached: finding windows and focus, the child list and tab
// order, the event-handler chain, extra styles, and the sizing and DPI conversions.
#include "internal.h"

namespace
{
    wxWindow* W(wxsharp_handle h) { return static_cast<wxWindow*>(h); }
}

// ---- Finding windows ------------------------------------------------------------------------------------
// These are static in wxWidgets because a window can be found without holding one. Each returns the raw
// pointer; the managed side maps it back to the wrapper that owns it, or null when wxWidgets made it alone.

wxsharp_handle wxsharp_window_find_focus() { return wxWindow::FindFocus(); }

wxsharp_handle wxsharp_window_find_by_id(long id, wxsharp_handle parent)
{
    return wxWindow::FindWindowById(id, W(parent));
}

wxsharp_handle wxsharp_window_find_child_by_id(wxsharp_handle window, long id)
{
    return W(window)->FindWindow(id);
}

wxsharp_handle wxsharp_window_find_child_by_name(wxsharp_handle window, const char* name)
{
    return W(window)->FindWindow(Str(name));
}

wxsharp_handle wxsharp_window_get_capture() { return wxWindow::GetCapture(); }

int  wxsharp_window_new_control_id(int count) { return wxWindow::NewControlId(count); }
void wxsharp_window_unreserve_control_id(int id, int count) { wxWindow::UnreserveControlId(id, count); }

// ---- Family ---------------------------------------------------------------------------------------------

// wxWidgets exposes this as a free function rather than a member.
wxsharp_handle wxsharp_window_top_level_parent(wxsharp_handle window) { return wxGetTopLevelParent(W(window)); }
wxsharp_handle wxsharp_window_grand_parent(wxsharp_handle window) { return W(window)->GetGrandParent(); }
wxsharp_handle wxsharp_window_next_sibling(wxsharp_handle window) { return W(window)->GetNextSibling(); }
wxsharp_handle wxsharp_window_prev_sibling(wxsharp_handle window) { return W(window)->GetPrevSibling(); }
bool wxsharp_window_reparent(wxsharp_handle window, wxsharp_handle parent) { return W(window)->Reparent(W(parent)); }
void wxsharp_window_destroy_children(wxsharp_handle window) { W(window)->DestroyChildren(); }

int wxsharp_window_child_count(wxsharp_handle window)
{
    return static_cast<int>(W(window)->GetChildren().GetCount());
}

// The child list is read one at a time rather than marshalled as an array, which is how every other
// collection crosses this boundary.
wxsharp_handle wxsharp_window_child_at(wxsharp_handle window, int index)
{
    const wxWindowList& children = W(window)->GetChildren();
    if (index < 0 || static_cast<size_t>(index) >= children.GetCount())
        return nullptr;
    return children.Item(static_cast<size_t>(index))->GetData();
}

// ---- Tab order ------------------------------------------------------------------------------------------
// Where a control sits in the tab order is a real accessibility decision, and it is set by position rather
// than by an index, so a control can be moved without renumbering everything after it.

void wxsharp_window_move_before_in_tab_order(wxsharp_handle window, wxsharp_handle other)
{
    W(window)->MoveBeforeInTabOrder(W(other));
}

void wxsharp_window_move_after_in_tab_order(wxsharp_handle window, wxsharp_handle other)
{
    W(window)->MoveAfterInTabOrder(W(other));
}

// ---- Focus ----------------------------------------------------------------------------------------------
// wxWidgets separates what a window says about itself from what is actually possible: AcceptsFocus is the
// window's own answer, while CanAcceptFocus also accounts for it being hidden or disabled.

bool wxsharp_window_can_accept_focus(wxsharp_handle window) { return W(window)->CanAcceptFocus(); }
bool wxsharp_window_can_accept_focus_from_keyboard(wxsharp_handle window) { return W(window)->CanAcceptFocusFromKeyboard(); }
bool wxsharp_window_can_be_focused(wxsharp_handle window) { return W(window)->CanBeFocused(); }
bool wxsharp_window_is_focusable(wxsharp_handle window) { return W(window)->IsFocusable(); }
void wxsharp_window_disable_focus_from_keyboard(wxsharp_handle window) { W(window)->DisableFocusFromKeyboard(); }

// ---- Event handler chain --------------------------------------------------------------------------------
// A window can have handlers pushed in front of it, which is how a modal filter or a recorder intercepts
// events without subclassing anything.

void wxsharp_window_push_event_handler(wxsharp_handle window, wxsharp_handle handler)
{
    W(window)->PushEventHandler(static_cast<wxEvtHandler*>(handler));
}

wxsharp_handle wxsharp_window_pop_event_handler(wxsharp_handle window, bool delete_handler)
{
    return W(window)->PopEventHandler(delete_handler);
}

bool wxsharp_window_remove_event_handler(wxsharp_handle window, wxsharp_handle handler)
{
    return W(window)->RemoveEventHandler(static_cast<wxEvtHandler*>(handler));
}

wxsharp_handle wxsharp_window_get_event_handler(wxsharp_handle window) { return W(window)->GetEventHandler(); }
void wxsharp_window_set_event_handler(wxsharp_handle window, wxsharp_handle handler)
{
    W(window)->SetEventHandler(static_cast<wxEvtHandler*>(handler));
}

// ---- Styles ---------------------------------------------------------------------------------------------
// The extra style bits, which are separate from the creation style and can be changed afterwards.

long wxsharp_window_get_extra_style(wxsharp_handle window) { return W(window)->GetExtraStyle(); }
void wxsharp_window_set_extra_style(wxsharp_handle window, long style) { W(window)->SetExtraStyle(style); }
bool wxsharp_window_has_extra_style(wxsharp_handle window, int flag) { return W(window)->HasExtraStyle(flag); }
void wxsharp_window_toggle_style(wxsharp_handle window, int flag) { W(window)->ToggleWindowStyle(flag); }
bool wxsharp_window_get_theme_enabled(wxsharp_handle window) { return W(window)->GetThemeEnabled(); }
void wxsharp_window_set_theme_enabled(wxsharp_handle window, bool enable) { W(window)->SetThemeEnabled(enable); }
bool wxsharp_window_is_retained(wxsharp_handle window) { return W(window)->IsRetained(); }
bool wxsharp_window_is_this_enabled(wxsharp_handle window) { return W(window)->IsThisEnabled(); }

// ---- Sizing and DPI -------------------------------------------------------------------------------------

void wxsharp_window_set_initial_size(wxsharp_handle window, int width, int height)
{
    W(window)->SetInitialSize(wxSize(width, height));
}

void wxsharp_window_invalidate_best_size(wxsharp_handle window) { W(window)->InvalidateBestSize(); }
int  wxsharp_window_get_best_height(wxsharp_handle window, int width) { return W(window)->GetBestHeight(width); }
int  wxsharp_window_get_best_width(wxsharp_handle window, int height) { return W(window)->GetBestWidth(height); }
double wxsharp_window_content_scale_factor(wxsharp_handle window) { return W(window)->GetContentScaleFactor(); }
double wxsharp_window_dpi_scale_factor(wxsharp_handle window) { return W(window)->GetDPIScaleFactor(); }

void wxsharp_window_client_to_window_size(wxsharp_handle window, int width, int height, int* out_w, int* out_h)
{
    const wxSize s = W(window)->ClientToWindowSize(wxSize(width, height));
    if (out_w) *out_w = s.x;
    if (out_h) *out_h = s.y;
}

void wxsharp_window_window_to_client_size(wxsharp_handle window, int width, int height, int* out_w, int* out_h)
{
    const wxSize s = W(window)->WindowToClientSize(wxSize(width, height));
    if (out_w) *out_w = s.x;
    if (out_h) *out_h = s.y;
}

// Physical pixels, which are what a window is actually drawn in on a scaled display. Distinct from the
// DPI-independent units FromDip and ToDip work in.
void wxsharp_window_from_phys(wxsharp_handle window, int width, int height, int* out_w, int* out_h)
{
    const wxSize s = wxWindow::FromPhys(wxSize(width, height), W(window));
    if (out_w) *out_w = s.x;
    if (out_h) *out_h = s.y;
}

void wxsharp_window_to_phys(wxsharp_handle window, int width, int height, int* out_w, int* out_h)
{
    const wxSize s = wxWindow::ToPhys(wxSize(width, height), W(window));
    if (out_w) *out_w = s.x;
    if (out_h) *out_h = s.y;
}

// ---- Painting and scrolling -----------------------------------------------------------------------------

bool wxsharp_window_can_scroll(wxsharp_handle window, int orientation)
{
    return W(window)->CanScroll(orientation == 1 ? wxVERTICAL : wxHORIZONTAL);
}

bool wxsharp_window_is_exposed(wxsharp_handle window, int x, int y, int width, int height)
{
    return W(window)->IsExposed(x, y, width, height);
}

void wxsharp_window_update_client_rect(wxsharp_handle window, int* x, int* y, int* width, int* height)
{
    const wxRect r = W(window)->GetUpdateClientRect();
    if (x) *x = r.x;
    if (y) *y = r.y;
    if (width) *width = r.width;
    if (height) *height = r.height;
}

// ---- Effects and events ---------------------------------------------------------------------------------

bool wxsharp_window_show_with_effect(wxsharp_handle window, int effect, unsigned int milliseconds)
{
    return W(window)->ShowWithEffect(static_cast<wxShowEffect>(effect), milliseconds);
}

bool wxsharp_window_hide_with_effect(wxsharp_handle window, int effect, unsigned int milliseconds)
{
    return W(window)->HideWithEffect(static_cast<wxShowEffect>(effect), milliseconds);
}


void wxsharp_window_enable_touch_events(wxsharp_handle window, int events)
{
    W(window)->EnableTouchEvents(events);
}
