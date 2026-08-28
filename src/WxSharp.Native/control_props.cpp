// Generic wxWindow property surface shared by every control: geometry, colours, font, tooltip, border and
// state queries. These operate on any control handle (all controls are wxWindows), so the managed Control base
// can expose them uniformly instead of each control reimplementing them.
#include "internal.h"

// ---- Geometry -------------------------------------------------------------------------------------------
void wxsharp_control_get_size(wxsharp_handle ctrl, int* width, int* height)
{
    const wxSize s = static_cast<wxWindow*>(ctrl)->GetSize();
    if (width) *width = s.x;
    if (height) *height = s.y;
}

void wxsharp_control_set_size(wxsharp_handle ctrl, int width, int height)
{
    static_cast<wxWindow*>(ctrl)->SetSize(width, height);
}

void wxsharp_control_get_client_size(wxsharp_handle ctrl, int* width, int* height)
{
    const wxSize s = static_cast<wxWindow*>(ctrl)->GetClientSize();
    if (width) *width = s.x;
    if (height) *height = s.y;
}

void wxsharp_control_get_position(wxsharp_handle ctrl, int* x, int* y)
{
    const wxPoint p = static_cast<wxWindow*>(ctrl)->GetPosition();
    if (x) *x = p.x;
    if (y) *y = p.y;
}

void wxsharp_control_set_position(wxsharp_handle ctrl, int x, int y)
{
    static_cast<wxWindow*>(ctrl)->Move(x, y);
}

void wxsharp_control_set_min_size(wxsharp_handle ctrl, int width, int height)
{
    static_cast<wxWindow*>(ctrl)->SetMinSize(wxSize(width, height));
}

void wxsharp_control_set_max_size(wxsharp_handle ctrl, int width, int height)
{
    static_cast<wxWindow*>(ctrl)->SetMaxSize(wxSize(width, height));
}

void wxsharp_control_get_best_size(wxsharp_handle ctrl, int* width, int* height)
{
    const wxSize s = static_cast<wxWindow*>(ctrl)->GetBestSize();
    if (width) *width = s.x;
    if (height) *height = s.y;
}

// The window's font, as a handle the caller owns. This is what makes the usual adjustment possible - take
// the window's own font, embolden or resize it, put it back - so a heading follows the user's chosen font
// rather than replacing it with a hard-coded one.
wxsharp_handle wxsharp_control_get_font(wxsharp_handle ctrl)
{
    return new wxFont(static_cast<wxWindow*>(ctrl)->GetFont());
}


void wxsharp_control_fit(wxsharp_handle ctrl) { static_cast<wxWindow*>(ctrl)->Fit(); }

// The mouse pointer's current position in the control's client coordinates (used for hover hit-testing on a
// canvas, where the move event itself carries no coordinates).
void wxsharp_control_get_pointer_position(wxsharp_handle ctrl, int* x, int* y)
{
    const wxPoint p = static_cast<wxWindow*>(ctrl)->ScreenToClient(::wxGetMousePosition());
    if (x) *x = p.x;
    if (y) *y = p.y;
}

// ---- Colours --------------------------------------------------------------------------------------------
void wxsharp_control_set_background_colour(wxsharp_handle ctrl, unsigned int argb)
{
    auto* w = static_cast<wxWindow*>(ctrl);
    w->SetBackgroundColour(ColourFromArgb(argb));
    w->Refresh();
}

unsigned int wxsharp_control_get_background_colour(wxsharp_handle ctrl)
{
    return ArgbFromColour(static_cast<wxWindow*>(ctrl)->GetBackgroundColour());
}

void wxsharp_control_set_foreground_colour(wxsharp_handle ctrl, unsigned int argb)
{
    auto* w = static_cast<wxWindow*>(ctrl);
    w->SetForegroundColour(ColourFromArgb(argb));
    w->Refresh();
}

unsigned int wxsharp_control_get_foreground_colour(wxsharp_handle ctrl)
{
    return ArgbFromColour(static_cast<wxWindow*>(ctrl)->GetForegroundColour());
}

// ---- Font, tooltip, border ------------------------------------------------------------------------------
void wxsharp_control_set_font(wxsharp_handle ctrl, wxsharp_handle font)
{
    static_cast<wxWindow*>(ctrl)->SetFont(*static_cast<wxFont*>(font));
}

void wxsharp_control_set_tooltip(wxsharp_handle ctrl, const char* text)
{
    static_cast<wxWindow*>(ctrl)->SetToolTip(Str(text));
}

int wxsharp_control_get_name(wxsharp_handle ctrl, char* buffer, int buffer_length)
{
    return CopyToBuffer(static_cast<wxWindow*>(ctrl)->GetName(), buffer, buffer_length);
}

void wxsharp_control_set_border(wxsharp_handle ctrl, int border)
{
    auto* w = static_cast<wxWindow*>(ctrl);
    const long style = (w->GetWindowStyleFlag() & ~wxBORDER_MASK) | MapBorder(border);
    w->SetWindowStyleFlag(style);
    w->Refresh();
}

void wxsharp_control_refresh(wxsharp_handle ctrl, bool erase_background)
{
    static_cast<wxWindow*>(ctrl)->Refresh(erase_background);
}

// ---- State queries --------------------------------------------------------------------------------------
bool wxsharp_control_is_enabled(wxsharp_handle ctrl) { return static_cast<wxWindow*>(ctrl)->IsEnabled(); }
bool wxsharp_control_is_shown(wxsharp_handle ctrl) { return static_cast<wxWindow*>(ctrl)->IsShown(); }
bool wxsharp_control_has_focus(wxsharp_handle ctrl) { return static_cast<wxWindow*>(ctrl)->HasFocus(); }
