// Push button.
#include "internal.h"

wxsharp_handle wxsharp_button_create(wxsharp_handle parent, int id, const char* label, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new wxButton(p, id, Str(label));
    TrackWindow(ctrl, token);
    return ctrl;
}

void wxsharp_button_set_default(wxsharp_handle ctrl) { static_cast<wxButton*>(ctrl)->SetDefault(); }
void wxsharp_button_set_label(wxsharp_handle ctrl, const char* label) { static_cast<wxButton*>(ctrl)->SetLabel(Str(label)); }

int wxsharp_button_get_label(wxsharp_handle ctrl, char* buffer, int buffer_length)
{
    return CopyToBuffer(static_cast<wxButton*>(ctrl)->GetLabel(), buffer, buffer_length);
}
