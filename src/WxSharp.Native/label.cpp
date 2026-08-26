// Static text label.
#include "internal.h"

wxsharp_handle wxsharp_label_create(wxsharp_handle parent, int id, const char* text, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new wxStaticText(p, id, Str(text), wxDefaultPosition, wxDefaultSize, MapAlignment(style));
    BindCommon(ctrl, token);
    return ctrl;
}

void wxsharp_label_set_text(wxsharp_handle ctrl, const char* text)
{
    static_cast<wxStaticText*>(ctrl)->SetLabel(Str(text));
}

int wxsharp_label_get_text(wxsharp_handle ctrl, char* buffer, int buffer_length)
{
    return CopyToBuffer(static_cast<wxStaticText*>(ctrl)->GetLabel(), buffer, buffer_length);
}
