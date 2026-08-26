// System services: the clipboard and the native file open/save dialog.
#include "internal.h"
#include <wx/clipbrd.h>
#include <wx/filedlg.h>
#include <wx/dirdlg.h>
#include <wx/textdlg.h>
#include <wx/numdlg.h>
#include <wx/colordlg.h>

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

namespace
{
    // The paths from the last file dialog. Holding them here means a multiple selection does not have to be
    // squeezed into a buffer the caller sized before it knew how many files there would be.
    wxArrayString& LastFileDialogResult()
    {
        static wxArrayString paths;
        return paths;
    }
}

int wxsharp_file_dialog(wxsharp_handle parent, const char* title, const char* wildcard,
                        const char* default_dir, const char* default_file, int style)
{
    wxArrayString& paths = LastFileDialogResult();
    paths.Clear();

    const wxString filter = (wildcard && *wildcard) ? Str(wildcard) : wxString("All files (*.*)|*.*");
    wxFileDialog dlg(static_cast<wxWindow*>(parent), Str(title),
                     default_dir ? Str(default_dir) : wxString(),
                     default_file ? Str(default_file) : wxString(),
                     filter, MapFileDialogStyle(style));
    if (dlg.ShowModal() != wxID_OK)
        return 0;

    if (dlg.GetWindowStyleFlag() & wxFD_MULTIPLE)
        dlg.GetPaths(paths);
    else
        paths.Add(dlg.GetPath());
    return static_cast<int>(paths.GetCount());
}

int wxsharp_file_dialog_result(int index, char* buffer, int buffer_length)
{
    const wxArrayString& paths = LastFileDialogResult();
    if (index < 0 || static_cast<size_t>(index) >= paths.GetCount())
        return 0;
    return CopyToBuffer(paths[static_cast<size_t>(index)], buffer, buffer_length);
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

bool wxsharp_text_entry_dialog(wxsharp_handle parent, const char* message, const char* caption,
                               const char* value, bool password, char* buffer, int bufferLength)
{
    wxTextEntryDialog dialog(static_cast<wxWindow*>(parent), Str(message), Str(caption), Str(value),
        password ? wxTextEntryDialogStyle | wxTE_PASSWORD : wxTextEntryDialogStyle);
    if (dialog.ShowModal() != wxID_OK) return false;
    CopyToBuffer(dialog.GetValue(), buffer, bufferLength); return true;
}

bool wxsharp_number_entry_dialog(wxsharp_handle parent, const char* message, const char* prompt,
                                 const char* caption, long long value, long long minimum,
                                 long long maximum, long long* result)
{
    const long answer = wxGetNumberFromUser(Str(message), Str(prompt), Str(caption),
        static_cast<long>(value), static_cast<long>(minimum), static_cast<long>(maximum),
        static_cast<wxWindow*>(parent));
    if (answer == -1) return false;
    *result = answer; return true;
}

bool wxsharp_colour_dialog(wxsharp_handle parent, unsigned int initial, unsigned int* result)
{
    const wxColour selected = wxGetColourFromUser(static_cast<wxWindow*>(parent), ColourFromArgb(initial));
    if (!selected.IsOk()) return false;
    *result = ArgbFromColour(selected); return true;
}
