// The rest of wxWindow: geometry in every coordinate space, freezing, scrolling, DPI, keyboard navigation
// and the odds and ends that live on the base class. Split from control_props.cpp, which holds the small
// set every widget needed from the beginning.
#include "internal.h"

namespace
{
    inline wxWindow* W(wxsharp_handle h) { return static_cast<wxWindow*>(h); }

    inline void OutSize(const wxSize& size, int* width, int* height)
    {
        if (width) *width = size.x;
        if (height) *height = size.y;
    }

    inline void OutPoint(const wxPoint& point, int* x, int* y)
    {
        if (x) *x = point.x;
        if (y) *y = point.y;
    }

    inline void OutRect(const wxRect& rect, int* x, int* y, int* width, int* height)
    {
        if (x) *x = rect.x;
        if (y) *y = rect.y;
        if (width) *width = rect.width;
        if (height) *height = rect.height;
    }
}

// ---- Repaint batching -----------------------------------------------------------------------------------
// Freezing suppresses redraws until the matching Thaw, which is what makes filling a long list affordable.

void wxsharp_window_freeze(wxsharp_handle window) { W(window)->Freeze(); }
void wxsharp_window_thaw(wxsharp_handle window) { W(window)->Thaw(); }
bool wxsharp_window_is_frozen(wxsharp_handle window) { return W(window)->IsFrozen(); }
void wxsharp_window_clear_background(wxsharp_handle window) { W(window)->ClearBackground(); }

// ---- Geometry -------------------------------------------------------------------------------------------

void wxsharp_window_get_rect(wxsharp_handle window, int* x, int* y, int* width, int* height)
{
    OutRect(W(window)->GetRect(), x, y, width, height);
}

void wxsharp_window_get_client_rect(wxsharp_handle window, int* x, int* y, int* width, int* height)
{
    OutRect(W(window)->GetClientRect(), x, y, width, height);
}

void wxsharp_window_get_screen_rect(wxsharp_handle window, int* x, int* y, int* width, int* height)
{
    OutRect(W(window)->GetScreenRect(), x, y, width, height);
}

void wxsharp_window_get_screen_position(wxsharp_handle window, int* x, int* y)
{
    OutPoint(W(window)->GetScreenPosition(), x, y);
}

void wxsharp_window_client_to_screen(wxsharp_handle window, int* x, int* y)
{
    const wxPoint result = W(window)->ClientToScreen(wxPoint(x ? *x : 0, y ? *y : 0));
    OutPoint(result, x, y);
}

void wxsharp_window_screen_to_client(wxsharp_handle window, int* x, int* y)
{
    const wxPoint result = W(window)->ScreenToClient(wxPoint(x ? *x : 0, y ? *y : 0));
    OutPoint(result, x, y);
}

void wxsharp_window_get_virtual_size(wxsharp_handle window, int* width, int* height)
{
    OutSize(W(window)->GetVirtualSize(), width, height);
}

void wxsharp_window_set_virtual_size(wxsharp_handle window, int width, int height)
{
    W(window)->SetVirtualSize(width, height);
}

void wxsharp_window_get_best_virtual_size(wxsharp_handle window, int* width, int* height)
{
    OutSize(W(window)->GetBestVirtualSize(), width, height);
}

void wxsharp_window_get_min_size(wxsharp_handle window, int* width, int* height)
{
    OutSize(W(window)->GetMinSize(), width, height);
}

void wxsharp_window_get_max_size(wxsharp_handle window, int* width, int* height)
{
    OutSize(W(window)->GetMaxSize(), width, height);
}

void wxsharp_window_get_min_client_size(wxsharp_handle window, int* width, int* height)
{
    OutSize(W(window)->GetMinClientSize(), width, height);
}

void wxsharp_window_set_min_client_size(wxsharp_handle window, int width, int height)
{
    W(window)->SetMinClientSize(wxSize(width, height));
}

void wxsharp_window_get_max_client_size(wxsharp_handle window, int* width, int* height)
{
    OutSize(W(window)->GetMaxClientSize(), width, height);
}

void wxsharp_window_set_max_client_size(wxsharp_handle window, int width, int height)
{
    W(window)->SetMaxClientSize(wxSize(width, height));
}

void wxsharp_window_get_border_size(wxsharp_handle window, int* width, int* height)
{
    OutSize(W(window)->GetWindowBorderSize(), width, height);
}

void wxsharp_window_set_client_size(wxsharp_handle window, int width, int height)
{
    W(window)->SetClientSize(width, height);
}

void wxsharp_window_fit_inside(wxsharp_handle window) { W(window)->FitInside(); }

void wxsharp_window_convert_dialog_to_pixels(wxsharp_handle window, int* x, int* y)
{
    OutPoint(W(window)->ConvertDialogToPixels(wxPoint(x ? *x : 0, y ? *y : 0)), x, y);
}

void wxsharp_window_convert_pixels_to_dialog(wxsharp_handle window, int* x, int* y)
{
    OutPoint(W(window)->ConvertPixelsToDialog(wxPoint(x ? *x : 0, y ? *y : 0)), x, y);
}

// ---- Text metrics ---------------------------------------------------------------------------------------

void wxsharp_window_get_text_extent(wxsharp_handle window, const char* text, int* width, int* height,
                                    int* descent, int* external_leading)
{
    int w = 0, h = 0, d = 0, l = 0;
    W(window)->GetTextExtent(Str(text), &w, &h, &d, &l);
    if (width) *width = w;
    if (height) *height = h;
    if (descent) *descent = d;
    if (external_leading) *external_leading = l;
}

int wxsharp_window_get_char_height(wxsharp_handle window) { return W(window)->GetCharHeight(); }
int wxsharp_window_get_char_width(wxsharp_handle window) { return W(window)->GetCharWidth(); }

// ---- DPI ------------------------------------------------------------------------------------------------
// A size expressed in device-independent pixels only means the same thing on every display after being run
// through these.

void wxsharp_window_get_dpi(wxsharp_handle window, int* x, int* y)
{
    OutSize(W(window)->GetDPI(), x, y);
}

void wxsharp_window_from_dip(wxsharp_handle window, int* width, int* height)
{
    OutSize(W(window)->FromDIP(wxSize(width ? *width : 0, height ? *height : 0)), width, height);
}

void wxsharp_window_to_dip(wxsharp_handle window, int* width, int* height)
{
    OutSize(W(window)->ToDIP(wxSize(width ? *width : 0, height ? *height : 0)), width, height);
}

// ---- Z-order, visibility and lifetime -------------------------------------------------------------------

void wxsharp_window_raise(wxsharp_handle window) { W(window)->Raise(); }
void wxsharp_window_lower(wxsharp_handle window) { W(window)->Lower(); }
bool wxsharp_window_is_shown_on_screen(wxsharp_handle window) { return W(window)->IsShownOnScreen(); }
bool wxsharp_window_close_any(wxsharp_handle window, bool force) { return W(window)->Close(force); }
void wxsharp_window_center_any(wxsharp_handle window, bool on_parent)
{
    if (on_parent) W(window)->CentreOnParent();
    else W(window)->Centre();
}

// ---- Keyboard navigation --------------------------------------------------------------------------------

bool wxsharp_window_navigate(wxsharp_handle window, bool forward, bool window_change)
{
    int flags = forward ? wxNavigationKeyEvent::IsForward : wxNavigationKeyEvent::IsBackward;
    if (window_change) flags |= wxNavigationKeyEvent::WinChange;
    return W(window)->Navigate(flags);
}

bool wxsharp_window_navigate_in(wxsharp_handle window, bool forward, bool window_change)
{
    int flags = forward ? wxNavigationKeyEvent::IsForward : wxNavigationKeyEvent::IsBackward;
    if (window_change) flags |= wxNavigationKeyEvent::WinChange;
    return W(window)->NavigateIn(flags);
}

// ---- Scrolling ------------------------------------------------------------------------------------------

void wxsharp_window_set_scrollbar(wxsharp_handle window, bool vertical, int position, int thumb_size,
                                  int range, bool refresh)
{
    W(window)->SetScrollbar(vertical ? wxVERTICAL : wxHORIZONTAL, position, thumb_size, range, refresh);
}

void wxsharp_window_set_scroll_pos(wxsharp_handle window, bool vertical, int position, bool refresh)
{
    W(window)->SetScrollPos(vertical ? wxVERTICAL : wxHORIZONTAL, position, refresh);
}

int wxsharp_window_get_scroll_pos(wxsharp_handle window, bool vertical)
{
    return W(window)->GetScrollPos(vertical ? wxVERTICAL : wxHORIZONTAL);
}

int wxsharp_window_get_scroll_range(wxsharp_handle window, bool vertical)
{
    return W(window)->GetScrollRange(vertical ? wxVERTICAL : wxHORIZONTAL);
}

int wxsharp_window_get_scroll_thumb(wxsharp_handle window, bool vertical)
{
    return W(window)->GetScrollThumb(vertical ? wxVERTICAL : wxHORIZONTAL);
}

bool wxsharp_window_has_scrollbar(wxsharp_handle window, bool vertical)
{
    return W(window)->HasScrollbar(vertical ? wxVERTICAL : wxHORIZONTAL);
}

bool wxsharp_window_scroll_lines(wxsharp_handle window, int lines) { return W(window)->ScrollLines(lines); }
bool wxsharp_window_scroll_pages(wxsharp_handle window, int pages) { return W(window)->ScrollPages(pages); }
bool wxsharp_window_line_up(wxsharp_handle window) { return W(window)->LineUp(); }
bool wxsharp_window_line_down(wxsharp_handle window) { return W(window)->LineDown(); }
bool wxsharp_window_page_up(wxsharp_handle window) { return W(window)->PageUp(); }
bool wxsharp_window_page_down(wxsharp_handle window) { return W(window)->PageDown(); }

void wxsharp_window_scroll_window(wxsharp_handle window, int dx, int dy)
{
    W(window)->ScrollWindow(dx, dy);
}

// ---- Styles, label and appearance -----------------------------------------------------------------------

int  wxsharp_window_get_style_flags(wxsharp_handle window) { return static_cast<int>(W(window)->GetWindowStyleFlag()); }
void wxsharp_window_set_style_flags(wxsharp_handle window, int style) { W(window)->SetWindowStyleFlag(style); }
bool wxsharp_window_has_style_flag(wxsharp_handle window, int flag) { return W(window)->HasFlag(flag); }

int wxsharp_window_get_label(wxsharp_handle window, char* buffer, int buffer_length)
{
    return CopyToBuffer(W(window)->GetLabel(), buffer, buffer_length);
}

void wxsharp_window_set_label(wxsharp_handle window, const char* label) { W(window)->SetLabel(Str(label)); }

wxsharp_handle wxsharp_window_get_parent(wxsharp_handle window) { return W(window)->GetParent(); }

// The runtime class name, e.g. "wxButton". Lets a binding recognise a window wxWidgets created on its own -
// the buttons behind wxDialog::CreateButtonSizer, for instance - and wrap it as the right type.
int wxsharp_window_get_class_name(wxsharp_handle window, char* buffer, int buffer_length)
{
    return CopyToBuffer(W(window)->GetClassInfo()->GetClassName(), buffer, buffer_length);
}

int wxsharp_window_get_help_text(wxsharp_handle window, char* buffer, int buffer_length)
{
    return CopyToBuffer(W(window)->GetHelpText(), buffer, buffer_length);
}

void wxsharp_window_set_help_text(wxsharp_handle window, const char* text) { W(window)->SetHelpText(Str(text)); }

bool wxsharp_window_is_double_buffered(wxsharp_handle window) { return W(window)->IsDoubleBuffered(); }
void wxsharp_window_set_double_buffered(wxsharp_handle window, bool on) { W(window)->SetDoubleBuffered(on); }

// 0 erase, 1 system, 2 paint, 3 transparent - wxBackgroundStyle.
int wxsharp_window_get_background_style(wxsharp_handle window)
{
    switch (W(window)->GetBackgroundStyle())
    {
        case wxBG_STYLE_SYSTEM: return 1;
        case wxBG_STYLE_PAINT: return 2;
        case wxBG_STYLE_TRANSPARENT: return 3;
        default: return 0;
    }
}

bool wxsharp_window_set_background_style(wxsharp_handle window, int style)
{
    const wxBackgroundStyle mapped = style == 1 ? wxBG_STYLE_SYSTEM
                                   : style == 2 ? wxBG_STYLE_PAINT
                                   : style == 3 ? wxBG_STYLE_TRANSPARENT
                                                : wxBG_STYLE_ERASE;
    return W(window)->SetBackgroundStyle(mapped);
}

int wxsharp_window_get_variant(wxsharp_handle window) { return static_cast<int>(W(window)->GetWindowVariant()); }
void wxsharp_window_set_variant(wxsharp_handle window, int variant)
{
    W(window)->SetWindowVariant(static_cast<wxWindowVariant>(variant));
}

bool wxsharp_window_can_set_transparent(wxsharp_handle window) { return W(window)->CanSetTransparent(); }
bool wxsharp_window_set_transparent(wxsharp_handle window, int alpha)
{
    return W(window)->SetTransparent(static_cast<wxByte>(alpha));
}

// ---- Pointer --------------------------------------------------------------------------------------------

void wxsharp_window_warp_pointer(wxsharp_handle window, int x, int y) { W(window)->WarpPointer(x, y); }

// 0 nowhere, 1 horizontal scrollbar, 2 vertical scrollbar, 3 border - wxHitTest.
int wxsharp_window_hit_test(wxsharp_handle window, int x, int y)
{
    switch (W(window)->HitTest(x, y))
    {
        case wxHT_WINDOW_HORZ_SCROLLBAR: return 1;
        case wxHT_WINDOW_VERT_SCROLLBAR: return 2;
        case wxHT_WINDOW_CORNER: return 3;
        case wxHT_WINDOW_INSIDE: return 4;
        default: return 0;
    }
}

// Shows a menu and returns the command chosen, or wxID_NONE when it was dismissed - the blocking form of
// PopupMenu, which is easier when the caller just wants an answer.
int wxsharp_window_popup_menu_selection(wxsharp_handle window, wxsharp_handle menu, int x, int y)
{
    const wxPoint position = (x < 0 && y < 0) ? wxDefaultPosition : wxPoint(x, y);
    return W(window)->GetPopupMenuSelectionFromUser(*static_cast<wxMenu*>(menu), position);
}
