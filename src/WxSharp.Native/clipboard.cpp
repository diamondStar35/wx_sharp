// The clipboard, and the system colours and metrics a theme-aware application needs.
//
// wxClipboard is exposed through the standard data objects rather than the wxDataObject hierarchy, so text,
// file lists and bitmaps each get a typed pair of calls. Every operation here is a real wxWidgets one; what
// is not offered yet is defining a custom format.
#include "internal.h"
#include <wx/clipbrd.h>
#include <wx/dataobj.h>
#include <wx/settings.h>
#include <memory>

namespace
{
    // The paths from the last successful file read, so the managed side can size each one exactly.
    wxArrayString& ClipboardFiles()
    {
        static wxArrayString paths;
        return paths;
    }

    // 0 text, 1 file names, 2 bitmap.
    wxDataFormat FormatFor(int format)
    {
        switch (format)
        {
            case 1: return wxDF_FILENAME;
            case 2: return wxDF_BITMAP;
            default: return wxDF_UNICODETEXT;
        }
    }

}

bool wxsharp_clipboard_open() { return wxTheClipboard->Open(); }
void wxsharp_clipboard_close() { wxTheClipboard->Close(); }
bool wxsharp_clipboard_is_opened() { return wxTheClipboard->IsOpened(); }
bool wxsharp_clipboard_flush() { return wxTheClipboard->Flush(); }

void wxsharp_clipboard_clear()
{
    wxTheClipboard->Clear();
}

bool wxsharp_clipboard_is_supported(int format)
{
    return wxTheClipboard->IsSupported(FormatFor(format));
}

bool wxsharp_clipboard_is_supported_async(wxsharp_handle sink)
{
    return wxTheClipboard->IsSupportedAsync(static_cast<wxEvtHandler*>(static_cast<wxWindow*>(sink)));
}

void wxsharp_clipboard_use_primary_selection(bool primary)
{
    wxTheClipboard->UsePrimarySelection(primary);
}

// ---- Text -----------------------------------------------------------------------------------------------

bool wxsharp_clipboard_set_text(const char* text)
{
    auto data = std::make_unique<wxTextDataObject>(Str(text));
    if (!wxTheClipboard->SetData(data.get())) return false;
    data.release();
    return true;
}

int wxsharp_clipboard_get_text(char* buffer, int buffer_length)
{
    wxString value;
    if (wxTheClipboard->IsSupported(wxDF_UNICODETEXT))
    {
        wxTextDataObject data;
        if (wxTheClipboard->GetData(data))
            value = data.GetText();
    }
    return CopyToBuffer(value, buffer, buffer_length);
}

// ---- File lists -----------------------------------------------------------------------------------------

bool wxsharp_clipboard_set_files(const char* const* paths, int count)
{
    auto data = std::make_unique<wxFileDataObject>();
    for (int i = 0; i < count; ++i)
        data->AddFile(Str(paths[i]));
    if (!wxTheClipboard->SetData(data.get())) return false;
    data.release();
    return true;
}

// Reads the file list and holds it until the next call, so each path can be fetched at its own length.
int wxsharp_clipboard_read_files()
{
    wxArrayString& paths = ClipboardFiles();
    paths.Clear();

    if (wxTheClipboard->IsSupported(wxDF_FILENAME))
    {
        wxFileDataObject data;
        if (wxTheClipboard->GetData(data))
            paths = data.GetFilenames();
    }
    return static_cast<int>(paths.GetCount());
}

int wxsharp_clipboard_get_file(int index, char* buffer, int buffer_length)
{
    const wxArrayString& paths = ClipboardFiles();
    if (index < 0 || static_cast<size_t>(index) >= paths.GetCount())
        return 0;
    return CopyToBuffer(paths[static_cast<size_t>(index)], buffer, buffer_length);
}

// ---- Bitmaps --------------------------------------------------------------------------------------------

bool wxsharp_clipboard_set_bitmap(wxsharp_handle bitmap)
{
    if (!bitmap) return false;
    auto data = std::make_unique<wxBitmapDataObject>(*static_cast<wxBitmap*>(bitmap));
    if (!wxTheClipboard->SetData(data.get())) return false;
    data.release();
    return true;
}

// Returns a new bitmap the caller owns, or null when the clipboard holds no image.
wxsharp_handle wxsharp_clipboard_get_bitmap()
{
    if (!wxTheClipboard->IsSupported(wxDF_BITMAP))
        return nullptr;
    wxBitmapDataObject data;
    if (!wxTheClipboard->GetData(data))
        return nullptr;
    const wxBitmap bitmap = data.GetBitmap();
    return bitmap.IsOk() ? new wxBitmap(bitmap) : nullptr;
}

// ---- System colours, fonts and metrics ------------------------------------------------------------------
// An application that follows the user's theme - including a high-contrast one - has to ask for these
// rather than hard-coding colours.

unsigned int wxsharp_system_colour(int which)
{
    return ArgbFromColour(wxSystemSettings::GetColour(static_cast<wxSystemColour>(which)));
}

int wxsharp_system_metric(int which, wxsharp_handle window)
{
    return wxSystemSettings::GetMetric(static_cast<wxSystemMetric>(which),
                                       static_cast<wxWindow*>(window));
}

// 0 none, 1 tiny, 2 small, 3 medium, 4 large - wxSystemScreenType.
int wxsharp_system_screen_type() { return static_cast<int>(wxSystemSettings::GetScreenType()); }

bool wxsharp_system_has_feature(int which)
{
    return wxSystemSettings::HasFeature(static_cast<wxSystemFeature>(which));
}

// True when the user is running a dark theme, so an application can pick colours that still read.
bool wxsharp_system_appearance_is_dark()
{
    return wxSystemSettings::GetAppearance().IsDark();
}

int wxsharp_system_appearance_name(char* buffer, int buffer_length)
{
    return CopyToBuffer(wxSystemSettings::GetAppearance().GetName(), buffer, buffer_length);
}
