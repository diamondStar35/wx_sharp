// Operations common to every widget: enable/show/focus and safe destruction. The accessible-name and other
// accessibility setters live in accessible.cpp; focus events are bound per control via TrackWindow().
#include "internal.h"

void wxsharp_control_enable(wxsharp_handle ctrl, bool enable) { static_cast<wxWindow*>(ctrl)->Enable(enable); }
void wxsharp_control_show(wxsharp_handle ctrl, bool show) { static_cast<wxWindow*>(ctrl)->Show(show); }
void wxsharp_control_focus(wxsharp_handle ctrl) { static_cast<wxWindow*>(ctrl)->SetFocus(); }
bool wxsharp_control_accepts_focus(wxsharp_handle ctrl) { return static_cast<wxWindow*>(ctrl)->AcceptsFocus(); }
bool wxsharp_control_accepts_focus_from_keyboard(wxsharp_handle ctrl) { return static_cast<wxWindow*>(ctrl)->AcceptsFocusFromKeyboard(); }
bool wxsharp_control_accepts_focus_recursively(wxsharp_handle ctrl) { return static_cast<wxWindow*>(ctrl)->AcceptsFocusRecursively(); }
bool wxsharp_control_has_flag(wxsharp_handle ctrl, int flag) { return static_cast<wxWindow*>(ctrl)->HasFlag(flag); }
void wxsharp_control_layout(wxsharp_handle ctrl) { static_cast<wxWindow*>(ctrl)->Layout(); }
int wxsharp_control_get_id(wxsharp_handle ctrl) { return static_cast<wxWindow*>(ctrl)->GetId(); }

bool wxsharp_control_destroy(wxsharp_handle ctrl)
{
    // Exactly what wxWindow::Destroy does: detach from any sizer and schedule safe deletion. The window
    // stays visible until that happens, as it does in wxWidgets.
    return static_cast<wxWindow*>(ctrl)->Destroy();
}
