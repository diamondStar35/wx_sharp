// Modal or modeless dialog with explicit child and sizer ownership.
#include "internal.h"

wxsharp_handle wxsharp_dialog_create(wxsharp_handle parent, int id, const char* title,
                                     int x, int y, int width, int height, int style, long long token)
{
    auto* dlg = new wxDialog(static_cast<wxWindow*>(parent), id, Str(title), wxPoint(x, y),
                             wxSize(width, height), MapDialogStyle(style));
    TrackWindow(dlg, token);
    return dlg;
}

// The platform's own button row: wx decides the order, the spacing, and which button is default, which is
// also what determines the order a screen reader reads them in.
wxsharp_handle wxsharp_dialog_create_button_sizer(wxsharp_handle dialog, int flags)
{
    long buttons = 0;
    if (flags & 1)   buttons |= wxOK;
    if (flags & 2)   buttons |= wxCANCEL;
    if (flags & 4)   buttons |= wxYES;
    if (flags & 8)   buttons |= wxNO;
    if (flags & 16)  buttons |= wxAPPLY;
    if (flags & 32)  buttons |= wxCLOSE;
    if (flags & 64)  buttons |= wxHELP;
    if (flags & 128) buttons |= wxNO_DEFAULT;
    return static_cast<wxDialog*>(dialog)->CreateButtonSizer(buttons);
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
