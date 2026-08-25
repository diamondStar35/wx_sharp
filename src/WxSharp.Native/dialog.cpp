// Modal dialog (wxDialog) with the same vertical content panel as a window. ShowModal runs a nested event
// loop and returns the result EndModal was given (wxID_CANCEL if the dialog is closed or escaped).
#include "internal.h"

wxsharp_handle wxsharp_dialog_create(const char* title, int width, int height, int id)
{
    auto* dlg = new wxDialog(nullptr, wxID_ANY, Str(title), wxDefaultPosition, wxSize(width, height));
    // Controls go on a content wxPanel (like the access_hub settings panels): it gives them the proper
    // container for tab traversal and MSAA, so screen readers announce them with their labels.
    SetupContentPanel(dlg);
    dlg->Center();

    // Key hook: report to managed first; if not consumed, close a modal dialog on Esc using its escape id
    // (the wrapper sets it via SetEscapeId). Our hook would otherwise pre-empt wxDialog's own Esc handling.
    dlg->Bind(wxEVT_CHAR_HOOK, [id, dlg](wxKeyEvent& e)
    {
        if (FireKey(id, WXSHARP_KEY_HOOK, e))
            return;
        if (e.GetKeyCode() == WXK_ESCAPE && dlg->IsModal())
        {
            const int escapeId = dlg->GetEscapeId();
            if (escapeId != wxID_NONE)
            {
                dlg->EndModal(escapeId == wxID_ANY ? wxID_CANCEL : escapeId);
                return;
            }
        }
        e.Skip();
    });

    dlg->Bind(wxEVT_CLOSE_WINDOW, [id](wxCloseEvent& e)
    {
        auto* d = wxDynamicCast(e.GetEventObject(), wxDialog);
        if (d && d->IsModal())
        {
            e.Skip(); // modal close is handled by the modal loop (EndModal)
            return;
        }
        // Modeless: report the close and hide (the app destroys it when done), so it can be reused.
        Fire(id, WXSHARP_EVT_CLOSE);
        if (d)
            d->Hide();
    });
    return dlg;
}

// Primitives mirroring wxDialog - the wrapper/app decides whether to use them (e.g. escape id = Cancel for
// Esc-to-close, affirmative id = Ok for Enter). No behaviour is imposed here.
void wxsharp_dialog_set_escape_id(wxsharp_handle dialog, int id)
{
    static_cast<wxDialog*>(dialog)->SetEscapeId(id);
}

void wxsharp_dialog_set_affirmative_id(wxsharp_handle dialog, int id)
{
    static_cast<wxDialog*>(dialog)->SetAffirmativeId(id);
}

wxsharp_handle wxsharp_dialog_panel(wxsharp_handle dialog)
{
    return ContentPanel(static_cast<wxWindow*>(dialog));
}

void wxsharp_dialog_layout(wxsharp_handle dialog)
{
    static_cast<wxDialog*>(dialog)->Layout();
}

int wxsharp_dialog_show_modal(wxsharp_handle dialog)
{
    auto* dlg = static_cast<wxDialog*>(dialog);
    dlg->Layout();
    FocusFirst(dlg);
    return dlg->ShowModal();
}

void wxsharp_dialog_show(wxsharp_handle dialog, bool show)
{
    auto* dlg = static_cast<wxDialog*>(dialog);
    dlg->Show(show);
    if (show)
    {
        dlg->Raise();
        FocusFirst(dlg);
    }
}

void wxsharp_dialog_end_modal(wxsharp_handle dialog, int result)
{
    static_cast<wxDialog*>(dialog)->EndModal(result);
}

void wxsharp_dialog_destroy(wxsharp_handle dialog)
{
    static_cast<wxDialog*>(dialog)->Destroy();
}
