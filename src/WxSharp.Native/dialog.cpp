// Modal or modeless dialog with explicit child and sizer ownership.
#include "internal.h"

wxsharp_handle wxsharp_dialog_create(wxsharp_handle parent, int id, const char* title,
                                     int x, int y, int width, int height, long long token)
{
    auto* dlg = new wxDialog(static_cast<wxWindow*>(parent), id, Str(title), wxPoint(x, y), wxSize(width, height));
    BindCommon(dlg, token);
    BindKeyHook(dlg, token);
    dlg->Bind(wxEVT_CLOSE_WINDOW, [token](wxCloseEvent& e)
    {
        const unsigned int result = Fire(token, WXSHARP_EVT_CLOSE, e.GetId(), 0, 0, 0, 0, 0, 0, 0, 0,
                                         false, e.CanVeto());
        if ((result & WXSHARP_EVENT_CANCEL) && e.CanVeto()) e.Veto(); else e.Skip();
    });
    return dlg;
}

void wxsharp_dialog_set_escape_id(wxsharp_handle dialog, int id) { static_cast<wxDialog*>(dialog)->SetEscapeId(id); }
void wxsharp_dialog_set_affirmative_id(wxsharp_handle dialog, int id) { static_cast<wxDialog*>(dialog)->SetAffirmativeId(id); }
void wxsharp_dialog_set_title(wxsharp_handle dialog, const char* title) { static_cast<wxDialog*>(dialog)->SetTitle(Str(title)); }
int wxsharp_dialog_get_title(wxsharp_handle dialog, char* buffer, int buffer_length)
{
    return CopyToBuffer(static_cast<wxDialog*>(dialog)->GetTitle(), buffer, buffer_length);
}
int wxsharp_dialog_show_modal(wxsharp_handle dialog) { return static_cast<wxDialog*>(dialog)->ShowModal(); }
void wxsharp_dialog_show(wxsharp_handle dialog, bool show) { static_cast<wxDialog*>(dialog)->Show(show); }
void wxsharp_dialog_end_modal(wxsharp_handle dialog, int result) { static_cast<wxDialog*>(dialog)->EndModal(result); }
void wxsharp_dialog_destroy(wxsharp_handle dialog) { static_cast<wxDialog*>(dialog)->Destroy(); }
