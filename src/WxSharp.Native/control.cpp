// Operations common to every widget: enable/show/focus and safe destruction. The accessible-name and other
// accessibility setters live in accessible.cpp; focus events are bound per control via TrackWindow().
#include "internal.h"

void wxsharp_control_enable(wxsharp_handle ctrl, bool enable) { static_cast<wxWindow*>(ctrl)->Enable(enable); }
void wxsharp_control_show(wxsharp_handle ctrl, bool show) { static_cast<wxWindow*>(ctrl)->Show(show); }
void wxsharp_control_focus(wxsharp_handle ctrl) { static_cast<wxWindow*>(ctrl)->SetFocus(); }
void wxsharp_control_layout(wxsharp_handle ctrl) { static_cast<wxWindow*>(ctrl)->Layout(); }
int wxsharp_control_get_id(wxsharp_handle ctrl) { return static_cast<wxWindow*>(ctrl)->GetId(); }

void wxsharp_control_destroy(wxsharp_handle ctrl)
{
    // Exactly what wxWindow::Destroy does: detach from any sizer and schedule safe deletion. The window
    // stays visible until that happens, as it does in wxWidgets.
    static_cast<wxWindow*>(ctrl)->Destroy();
}
