// The common dialogs as real windows.
//
// These used to be one-shot helpers that built a dialog, showed it and threw it away, which meant a caller
// could not configure one before showing it, read anything back but the single value it was built for, or
// treat it like the window it is. Each is now constructed, configured, shown and read exactly as wxWidgets
// intends - and because each is a wxDialog, everything wxWindow offers comes with it.
#include "internal.h"
#include <wx/colordlg.h>
#include <wx/dirdlg.h>
#include <wx/fontdlg.h>
#include <wx/numdlg.h>
#include <wx/textdlg.h>

// ---- wxFileDialog ---------------------------------------------------------------------------------------

wxsharp_handle wxsharp_filedlg_create(wxsharp_handle parent, const char* message, const char* directory,
                                      const char* file, const char* wildcard, int style, long long token)
{
    auto* dlg = new wxFileDialog(static_cast<wxWindow*>(parent), Str(message), Str(directory), Str(file),
                                 wildcard && *wildcard ? Str(wildcard) : wxString(wxFileSelectorDefaultWildcardStr),
                                 MapFileDialogStyle(style));
    TrackWindow(dlg, token);
    return dlg;
}

int wxsharp_filedlg_get_path(wxsharp_handle dlg, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxFileDialog*>(dlg)->GetPath(), buffer, length);
}

int wxsharp_filedlg_get_directory(wxsharp_handle dlg, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxFileDialog*>(dlg)->GetDirectory(), buffer, length);
}

int wxsharp_filedlg_get_filename(wxsharp_handle dlg, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxFileDialog*>(dlg)->GetFilename(), buffer, length);
}

int wxsharp_filedlg_get_wildcard(wxsharp_handle dlg, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxFileDialog*>(dlg)->GetWildcard(), buffer, length);
}

int wxsharp_filedlg_get_message(wxsharp_handle dlg, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxFileDialog*>(dlg)->GetMessage(), buffer, length);
}

void wxsharp_filedlg_set_path(wxsharp_handle dlg, const char* path) { static_cast<wxFileDialog*>(dlg)->SetPath(Str(path)); }
void wxsharp_filedlg_set_directory(wxsharp_handle dlg, const char* dir) { static_cast<wxFileDialog*>(dlg)->SetDirectory(Str(dir)); }
void wxsharp_filedlg_set_filename(wxsharp_handle dlg, const char* name) { static_cast<wxFileDialog*>(dlg)->SetFilename(Str(name)); }
void wxsharp_filedlg_set_wildcard(wxsharp_handle dlg, const char* wildcard) { static_cast<wxFileDialog*>(dlg)->SetWildcard(Str(wildcard)); }
void wxsharp_filedlg_set_message(wxsharp_handle dlg, const char* message) { static_cast<wxFileDialog*>(dlg)->SetMessage(Str(message)); }
int  wxsharp_filedlg_get_filter_index(wxsharp_handle dlg) { return static_cast<wxFileDialog*>(dlg)->GetFilterIndex(); }
void wxsharp_filedlg_set_filter_index(wxsharp_handle dlg, int index) { static_cast<wxFileDialog*>(dlg)->SetFilterIndex(index); }

// A multiple selection is read back one path at a time, so it is never truncated by a caller-sized buffer.
int wxsharp_filedlg_path_count(wxsharp_handle dlg)
{
    wxArrayString paths;
    static_cast<wxFileDialog*>(dlg)->GetPaths(paths);
    return static_cast<int>(paths.GetCount());
}

int wxsharp_filedlg_path_at(wxsharp_handle dlg, int index, char* buffer, int length)
{
    wxArrayString paths;
    static_cast<wxFileDialog*>(dlg)->GetPaths(paths);
    if (index < 0 || static_cast<size_t>(index) >= paths.GetCount())
        return 0;
    return CopyToBuffer(paths[static_cast<size_t>(index)], buffer, length);
}

int wxsharp_filedlg_filename_at(wxsharp_handle dlg, int index, char* buffer, int length)
{
    wxArrayString names;
    static_cast<wxFileDialog*>(dlg)->GetFilenames(names);
    if (index < 0 || static_cast<size_t>(index) >= names.GetCount())
        return 0;
    return CopyToBuffer(names[static_cast<size_t>(index)], buffer, length);
}

// ---- wxDirDialog ----------------------------------------------------------------------------------------

wxsharp_handle wxsharp_dirdlg_create(wxsharp_handle parent, const char* message, const char* default_path,
                                     int style, long long token)
{
    long flags = wxDD_DEFAULT_STYLE;
    if (style & 1) flags |= wxDD_DIR_MUST_EXIST;
    if (style & 2) flags |= wxDD_CHANGE_DIR;
    if (style & 4) flags |= wxDD_MULTIPLE;
    if (style & 8) flags |= wxDD_SHOW_HIDDEN;
    auto* dlg = new wxDirDialog(static_cast<wxWindow*>(parent), Str(message), Str(default_path), flags);
    TrackWindow(dlg, token);
    return dlg;
}

int wxsharp_dirdlg_get_path(wxsharp_handle dlg, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxDirDialog*>(dlg)->GetPath(), buffer, length);
}

void wxsharp_dirdlg_set_path(wxsharp_handle dlg, const char* path) { static_cast<wxDirDialog*>(dlg)->SetPath(Str(path)); }

int wxsharp_dirdlg_get_message(wxsharp_handle dlg, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxDirDialog*>(dlg)->GetMessage(), buffer, length);
}

void wxsharp_dirdlg_set_message(wxsharp_handle dlg, const char* message) { static_cast<wxDirDialog*>(dlg)->SetMessage(Str(message)); }

int wxsharp_dirdlg_path_count(wxsharp_handle dlg)
{
    wxArrayString paths;
    static_cast<wxDirDialog*>(dlg)->GetPaths(paths);
    return static_cast<int>(paths.GetCount());
}

int wxsharp_dirdlg_path_at(wxsharp_handle dlg, int index, char* buffer, int length)
{
    wxArrayString paths;
    static_cast<wxDirDialog*>(dlg)->GetPaths(paths);
    if (index < 0 || static_cast<size_t>(index) >= paths.GetCount())
        return 0;
    return CopyToBuffer(paths[static_cast<size_t>(index)], buffer, length);
}

// ---- wxTextEntryDialog ----------------------------------------------------------------------------------

wxsharp_handle wxsharp_textdlg_create(wxsharp_handle parent, const char* message, const char* caption,
                                      const char* value, int style, long long token)
{
    long flags = wxTextEntryDialogStyle;
    if (style & 1) flags |= wxTE_MULTILINE;
    if (style & 2) flags |= wxTE_PASSWORD;
    auto* dlg = new wxTextEntryDialog(static_cast<wxWindow*>(parent), Str(message), Str(caption),
                                      Str(value), flags);
    TrackWindow(dlg, token);
    return dlg;
}

int wxsharp_textdlg_get_value(wxsharp_handle dlg, char* buffer, int length)
{
    return CopyToBuffer(static_cast<wxTextEntryDialog*>(dlg)->GetValue(), buffer, length);
}

void wxsharp_textdlg_set_value(wxsharp_handle dlg, const char* value)
{
    static_cast<wxTextEntryDialog*>(dlg)->SetValue(Str(value));
}

// Caps what can be typed, which is worth setting when the value goes somewhere with a limit of its own.
void wxsharp_textdlg_set_max_length(wxsharp_handle dlg, unsigned long length)
{
    static_cast<wxTextEntryDialog*>(dlg)->SetMaxLength(length);
}

void wxsharp_textdlg_force_upper(wxsharp_handle dlg) { static_cast<wxTextEntryDialog*>(dlg)->ForceUpper(); }

// ---- wxNumberEntryDialog --------------------------------------------------------------------------------

wxsharp_handle wxsharp_numdlg_create(wxsharp_handle parent, const char* message, const char* prompt,
                                     const char* caption, long long value, long long minimum,
                                     long long maximum, long long token)
{
    auto* dlg = new wxNumberEntryDialog(static_cast<wxWindow*>(parent), Str(message), Str(prompt),
                                        Str(caption), value, minimum, maximum);
    TrackWindow(dlg, token);
    return dlg;
}

long long wxsharp_numdlg_get_value(wxsharp_handle dlg)
{
    return static_cast<wxNumberEntryDialog*>(dlg)->GetValue();
}

// ---- wxColourDialog -------------------------------------------------------------------------------------

wxsharp_handle wxsharp_colourdlg_create(wxsharp_handle parent, unsigned int initial, bool full,
                                        long long token)
{
    wxColourData data;
    data.SetColour(ColourFromArgb(initial));
    data.SetChooseFull(full);
    auto* dlg = new wxColourDialog(static_cast<wxWindow*>(parent), &data);
    TrackWindow(dlg, token);
    return dlg;
}

unsigned int wxsharp_colourdlg_get_colour(wxsharp_handle dlg)
{
    return ArgbFromColour(static_cast<wxColourDialog*>(dlg)->GetColourData().GetColour());
}

void wxsharp_colourdlg_set_colour(wxsharp_handle dlg, unsigned int colour)
{
    static_cast<wxColourDialog*>(dlg)->GetColourData().SetColour(ColourFromArgb(colour));
}

// The palette the user built up, which an application should carry between invocations so their custom
// colours are still there next time.
unsigned int wxsharp_colourdlg_get_custom(wxsharp_handle dlg, int index)
{
    return ArgbFromColour(static_cast<wxColourDialog*>(dlg)->GetColourData().GetCustomColour(index));
}

void wxsharp_colourdlg_set_custom(wxsharp_handle dlg, int index, unsigned int colour)
{
    static_cast<wxColourDialog*>(dlg)->GetColourData().SetCustomColour(index, ColourFromArgb(colour));
}

// ---- wxFontDialog ---------------------------------------------------------------------------------------

wxsharp_handle wxsharp_fontdlg_create(wxsharp_handle parent, wxsharp_handle initial, long long token)
{
    wxFontData data;
    if (initial)
        data.SetInitialFont(*static_cast<wxFont*>(initial));
    auto* dlg = new wxFontDialog(static_cast<wxWindow*>(parent), data);
    TrackWindow(dlg, token);
    return dlg;
}

wxsharp_handle wxsharp_fontdlg_get_font(wxsharp_handle dlg)
{
    return new wxFont(static_cast<wxFontDialog*>(dlg)->GetFontData().GetChosenFont());
}

unsigned int wxsharp_fontdlg_get_colour(wxsharp_handle dlg)
{
    return ArgbFromColour(static_cast<wxFontDialog*>(dlg)->GetFontData().GetColour());
}

void wxsharp_fontdlg_set_colour(wxsharp_handle dlg, unsigned int colour)
{
    static_cast<wxFontDialog*>(dlg)->GetFontData().SetColour(ColourFromArgb(colour));
}

void wxsharp_fontdlg_enable_effects(wxsharp_handle dlg, bool enable)
{
    static_cast<wxFontDialog*>(dlg)->GetFontData().EnableEffects(enable);
}

void wxsharp_fontdlg_set_range(wxsharp_handle dlg, int minimum, int maximum)
{
    static_cast<wxFontDialog*>(dlg)->GetFontData().SetRange(minimum, maximum);
}
