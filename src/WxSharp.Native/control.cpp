// Operations common to every widget: enable/show/focus and safe destruction. The accessible-name and other
// accessibility setters live in accessible.cpp; focus events are bound per control via BindCommon().
#include "internal.h"

void wxsharp_control_enable(wxsharp_handle ctrl, bool enable) { static_cast<wxWindow*>(ctrl)->Enable(enable); }
void wxsharp_control_show(wxsharp_handle ctrl, bool show) { static_cast<wxWindow*>(ctrl)->Show(show); }
void wxsharp_control_focus(wxsharp_handle ctrl) { static_cast<wxWindow*>(ctrl)->SetFocus(); }
void wxsharp_control_layout(wxsharp_handle ctrl) { static_cast<wxWindow*>(ctrl)->Layout(); }
int wxsharp_control_get_id(wxsharp_handle ctrl) { return static_cast<wxWindow*>(ctrl)->GetId(); }

void wxsharp_control_destroy(wxsharp_handle ctrl)
{
    auto* w = static_cast<wxWindow*>(ctrl);
    w->Hide();
    w->Destroy(); // detaches from its sizer and schedules safe deletion
}
