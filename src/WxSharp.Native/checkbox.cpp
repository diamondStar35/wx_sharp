// Check box. The accessible name is set from the managed side (accessible.cpp / Control.AccessibleName).
#include "internal.h"

wxsharp_handle wxsharp_checkbox_create(wxsharp_handle parent, int id, const char* label, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new wxCheckBox(p, id, Str(label), wxDefaultPosition, wxDefaultSize, MapCheckBoxStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

bool wxsharp_checkbox_get(wxsharp_handle ctrl) { return static_cast<wxCheckBox*>(ctrl)->GetValue(); }

int wxsharp_checkbox_get_3state(wxsharp_handle ctrl)
{
    return static_cast<int>(static_cast<wxCheckBox*>(ctrl)->Get3StateValue());
}

void wxsharp_checkbox_set_3state(wxsharp_handle ctrl, int state)
{
    static_cast<wxCheckBox*>(ctrl)->Set3StateValue(static_cast<wxCheckBoxState>(state));
}

bool wxsharp_checkbox_is_3state(wxsharp_handle ctrl) { return static_cast<wxCheckBox*>(ctrl)->Is3State(); }

bool wxsharp_checkbox_is_3rd_state_allowed_for_user(wxsharp_handle ctrl)
{
    return static_cast<wxCheckBox*>(ctrl)->Is3rdStateAllowedForUser();
}
void wxsharp_checkbox_set(wxsharp_handle ctrl, bool value) { static_cast<wxCheckBox*>(ctrl)->SetValue(value); }
void wxsharp_checkbox_set_transparent_part_colour(wxsharp_handle ctrl, unsigned int argb)
{
    static_cast<wxCheckBox*>(ctrl)->SetTransparentPartColour(ColourFromArgb(argb));
}
