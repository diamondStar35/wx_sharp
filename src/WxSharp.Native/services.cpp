// System services: the clipboard and the native file open/save dialog.
#include "internal.h"
#include <wx/clipbrd.h>
#include <wx/filedlg.h>
#include <wx/dirdlg.h>

void wxsharp_clipboard_set_text(const char* text)
{
    if (wxTheClipboard->Open())
    {
        wxTheClipboard->SetData(new wxTextDataObject(Str(text)));
        wxTheClipboard->Close();
    }
}

int wxsharp_clipboard_get_text(char* buffer, int buffer_length)
{
    wxString s;
    if (wxTheClipboard->Open())
    {
        if (wxTheClipboard->IsSupported(wxDF_UNICODETEXT))
        {
            wxTextDataObject data;
            wxTheClipboard->GetData(data);
            s = data.GetText();
        }
        wxTheClipboard->Close();
    }
    return CopyToBuffer(s, buffer, buffer_length);
}

bool wxsharp_file_dialog(wxsharp_handle parent, const char* title, const char* wildcard, bool save,
                      char* buffer, int buffer_length)
{
    auto* p = static_cast<wxWindow*>(parent);
    const long style = save ? (wxFD_SAVE | wxFD_OVERWRITE_PROMPT) : (wxFD_OPEN | wxFD_FILE_MUST_EXIST);
    const wxString filter = (wildcard && *wildcard) ? Str(wildcard) : wxString("All files (*.*)|*.*");
    wxFileDialog dlg(p, Str(title), wxEmptyString, wxEmptyString, filter, style);
    if (dlg.ShowModal() != wxID_OK)
        return false;
    CopyToBuffer(dlg.GetPath(), buffer, buffer_length);
    return true;
}

bool wxsharp_dir_dialog(wxsharp_handle parent, const char* title, const char* initial_dir,
                     char* buffer, int buffer_length)
{
    auto* p = static_cast<wxWindow*>(parent);
    const wxString initial = (initial_dir && *initial_dir) ? Str(initial_dir) : wxString();
    wxDirDialog dlg(p, Str(title), initial);
    if (dlg.ShowModal() != wxID_OK)
        return false;
    CopyToBuffer(dlg.GetPath(), buffer, buffer_length);
    return true;
}
