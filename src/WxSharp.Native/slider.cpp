// Generic slider (a plain wxSlider). The accessible behaviour that used to live in a native subclass - firing
// on programmatic changes and uniform arrow/page/home/end handling - now lives in the managed CustomSlider,
// built on the key events the managed side binds on demand. This keeps the native side a thin wrapper.
#include "internal.h"

wxsharp_handle wxsharp_slider_create(wxsharp_handle parent, int id, int min_value, int max_value, int value, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new wxSlider(p, id, value, min_value, max_value,
                              wxDefaultPosition, wxDefaultSize, MapSliderStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

int wxsharp_slider_get(wxsharp_handle ctrl) { return static_cast<wxSlider*>(ctrl)->GetValue(); }
void wxsharp_slider_set(wxsharp_handle ctrl, int value) { static_cast<wxSlider*>(ctrl)->SetValue(value); }

int wxsharp_slider_get_min(wxsharp_handle ctrl) { return static_cast<wxSlider*>(ctrl)->GetMin(); }
int wxsharp_slider_get_max(wxsharp_handle ctrl) { return static_cast<wxSlider*>(ctrl)->GetMax(); }

void wxsharp_slider_set_range(wxsharp_handle ctrl, int min_value, int max_value)
{
    static_cast<wxSlider*>(ctrl)->SetRange(min_value, max_value);
}
