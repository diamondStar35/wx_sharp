// Platform services that need no window of their own: where the platform keeps files, the sounds and stock
// art it provides, the displays attached, and the small pieces of window furniture - cursors, carets,
// image lists and rich tooltips - that other classes hang off.
#include "internal.h"
#include <wx/aboutdlg.h>
#include <wx/artprov.h>
#include <wx/busyinfo.h>
#include <wx/caret.h>
#include <wx/cursor.h>
#include <wx/display.h>
#include <wx/imaglist.h>
#include <wx/richtooltip.h>
#include <wx/sound.h>
#include <wx/stdpaths.h>

// ---- wxStandardPaths ------------------------------------------------------------------------------------
// Where the platform expects an application to keep things. An application that guesses at these gets them
// wrong on at least one platform, and on Windows writes somewhere the user cannot back up.

namespace
{
    const wxStandardPaths& Paths() { return wxStandardPaths::Get(); }
}

int wxsharp_stdpaths_executable(char* buffer, int length) { return CopyToBuffer(Paths().GetExecutablePath(), buffer, length); }
int wxsharp_stdpaths_config_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetConfigDir(), buffer, length); }
int wxsharp_stdpaths_user_config_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetUserConfigDir(), buffer, length); }
int wxsharp_stdpaths_data_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetDataDir(), buffer, length); }
int wxsharp_stdpaths_local_data_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetLocalDataDir(), buffer, length); }
int wxsharp_stdpaths_user_data_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetUserDataDir(), buffer, length); }
int wxsharp_stdpaths_user_local_data_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetUserLocalDataDir(), buffer, length); }
int wxsharp_stdpaths_plugins_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetPluginsDir(), buffer, length); }
int wxsharp_stdpaths_resources_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetResourcesDir(), buffer, length); }
int wxsharp_stdpaths_documents_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetDocumentsDir(), buffer, length); }
int wxsharp_stdpaths_temp_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetTempDir(), buffer, length); }
int wxsharp_stdpaths_app_documents_dir(char* buffer, int length) { return CopyToBuffer(Paths().GetAppDocumentsDir(), buffer, length); }

int wxsharp_stdpaths_user_dir(int which, char* buffer, int length)
{
    return CopyToBuffer(Paths().GetUserDir(static_cast<wxStandardPaths::Dir>(which)), buffer, length);
}

int wxsharp_stdpaths_localized_resources_dir(const char* language, int category, char* buffer, int length)
{
    return CopyToBuffer(
        Paths().GetLocalizedResourcesDir(Str(language),
                                         static_cast<wxStandardPaths::ResourceCat>(category)),
        buffer, length);
}

// ---- wxSound --------------------------------------------------------------------------------------------
// A WAV file, played synchronously or in the background. Not a media player: wxWidgets reads one format and
// offers no position or volume, which is why anything richer needs a real audio library instead.

wxsharp_handle wxsharp_sound_create(const char* path)
{
    auto* sound = new wxSound(Str(path));
    if (!sound->IsOk()) { delete sound; return nullptr; }
    return sound;
}

void wxsharp_sound_destroy(wxsharp_handle sound) { delete static_cast<wxSound*>(sound); }
bool wxsharp_sound_is_ok(wxsharp_handle sound) { return static_cast<wxSound*>(sound)->IsOk(); }
bool wxsharp_sound_play(wxsharp_handle sound, unsigned int flags) { return static_cast<wxSound*>(sound)->Play(flags); }
bool wxsharp_sound_play_file(const char* path, unsigned int flags) { return wxSound::Play(Str(path), flags); }
// wxWidgets offers no way to ask what is playing, only how to stop it.
void wxsharp_sound_stop() { wxSound::Stop(); }

// ---- wxDisplay ------------------------------------------------------------------------------------------
// The monitors attached, which is what a window has to consult before restoring a saved position: a screen
// that was there last time may not be now.

unsigned int wxsharp_display_count() { return wxDisplay::GetCount(); }
int wxsharp_display_from_point(int x, int y) { return wxDisplay::GetFromPoint(wxPoint(x, y)); }
int wxsharp_display_from_window(wxsharp_handle window) { return wxDisplay::GetFromWindow(static_cast<wxWindow*>(window)); }

void wxsharp_display_geometry(unsigned int index, int* x, int* y, int* width, int* height)
{
    const wxRect r = wxDisplay(index).GetGeometry();
    if (x) *x = r.x;
    if (y) *y = r.y;
    if (width) *width = r.width;
    if (height) *height = r.height;
}

// The area a maximised window gets, which is the geometry less the taskbar and any other reserved edge.
void wxsharp_display_client_area(unsigned int index, int* x, int* y, int* width, int* height)
{
    const wxRect r = wxDisplay(index).GetClientArea();
    if (x) *x = r.x;
    if (y) *y = r.y;
    if (width) *width = r.width;
    if (height) *height = r.height;
}

bool wxsharp_display_is_primary(unsigned int index) { return wxDisplay(index).IsPrimary(); }
int  wxsharp_display_name(unsigned int index, char* buffer, int length) { return CopyToBuffer(wxDisplay(index).GetName(), buffer, length); }
double wxsharp_display_scale_factor(unsigned int index) { return wxDisplay(index).GetScaleFactor(); }

void wxsharp_display_ppi(unsigned int index, int* x, int* y)
{
    const wxSize ppi = wxDisplay(index).GetPPI();
    if (x) *x = ppi.x;
    if (y) *y = ppi.y;
}

// ---- wxArtProvider --------------------------------------------------------------------------------------
// The platform's own icons. Using these rather than shipping images is what makes a toolbar or a message
// look native, follow the user's theme, and stay legible in high contrast.

wxsharp_handle wxsharp_art_bitmap(const char* id, const char* client, int width, int height)
{
    const wxSize size = (width > 0 && height > 0) ? wxSize(width, height) : wxDefaultSize;
    wxBitmap bitmap = wxArtProvider::GetBitmap(Str(id), Str(client), size);
    if (!bitmap.IsOk())
        return nullptr;
    return new wxBitmap(bitmap);
}

wxsharp_handle wxsharp_art_icon(const char* id, const char* client, int width, int height)
{
    const wxSize size = (width > 0 && height > 0) ? wxSize(width, height) : wxDefaultSize;
    wxIcon icon = wxArtProvider::GetIcon(Str(id), Str(client), size);
    if (!icon.IsOk())
        return nullptr;
    return new wxIcon(icon);
}

void wxsharp_art_native_size(const char* client, wxsharp_handle window, int* width, int* height)
{
    const wxSize size = wxArtProvider::GetNativeSizeHint(Str(client), static_cast<wxWindow*>(window));
    if (width) *width = size.x;
    if (height) *height = size.y;
}

// ---- wxCursor -------------------------------------------------------------------------------------------

wxsharp_handle wxsharp_cursor_create_stock(int id)
{
    auto* cursor = new wxCursor(static_cast<wxStockCursor>(id));
    if (!cursor->IsOk()) { delete cursor; return nullptr; }
    return cursor;
}

wxsharp_handle wxsharp_cursor_create_from_file(const char* path, int type, int hotspot_x, int hotspot_y)
{
    auto* cursor = new wxCursor(Str(path), static_cast<wxBitmapType>(type), hotspot_x, hotspot_y);
    if (!cursor->IsOk()) { delete cursor; return nullptr; }
    return cursor;
}

void wxsharp_cursor_destroy(wxsharp_handle cursor) { delete static_cast<wxCursor*>(cursor); }
bool wxsharp_cursor_is_ok(wxsharp_handle cursor) { return static_cast<wxCursor*>(cursor)->IsOk(); }

void wxsharp_control_set_cursor(wxsharp_handle ctrl, wxsharp_handle cursor)
{
    static_cast<wxWindow*>(ctrl)->SetCursor(cursor ? *static_cast<wxCursor*>(cursor) : wxNullCursor);
}

wxsharp_handle wxsharp_control_get_cursor(wxsharp_handle ctrl)
{
    return new wxCursor(static_cast<wxWindow*>(ctrl)->GetCursor());
}

// The busy cursor applies to the whole application rather than one window, which is why it is not a
// property of anything.
void wxsharp_cursor_set_global(wxsharp_handle cursor)
{
    ::wxSetCursor(cursor ? *static_cast<wxCursor*>(cursor) : wxNullCursor);
}

// ---- wxImageList ----------------------------------------------------------------------------------------
// The images a list, tree, notebook or toolbar draws beside its items. wxWidgets addresses them by index
// into a list the control holds, rather than by handing each item a bitmap.

wxsharp_handle wxsharp_imagelist_create(int width, int height, bool mask, int initial_count)
{
    return new wxImageList(width, height, mask, initial_count);
}

void wxsharp_imagelist_destroy(wxsharp_handle list) { delete static_cast<wxImageList*>(list); }
int  wxsharp_imagelist_count(wxsharp_handle list) { return static_cast<wxImageList*>(list)->GetImageCount(); }
bool wxsharp_imagelist_remove(wxsharp_handle list, int index) { return static_cast<wxImageList*>(list)->Remove(index); }
bool wxsharp_imagelist_remove_all(wxsharp_handle list) { return static_cast<wxImageList*>(list)->RemoveAll(); }

int wxsharp_imagelist_add_bitmap(wxsharp_handle list, wxsharp_handle bitmap)
{
    return static_cast<wxImageList*>(list)->Add(*static_cast<wxBitmap*>(bitmap));
}

int wxsharp_imagelist_add_icon(wxsharp_handle list, wxsharp_handle icon)
{
    return static_cast<wxImageList*>(list)->Add(*static_cast<wxIcon*>(icon));
}

bool wxsharp_imagelist_replace(wxsharp_handle list, int index, wxsharp_handle bitmap)
{
    return static_cast<wxImageList*>(list)->Replace(index, *static_cast<wxBitmap*>(bitmap));
}

void wxsharp_imagelist_size(wxsharp_handle list, int index, int* width, int* height)
{
    int w = 0, h = 0;
    static_cast<wxImageList*>(list)->GetSize(index, w, h);
    if (width) *width = w;
    if (height) *height = h;
}

wxsharp_handle wxsharp_imagelist_get_bitmap(wxsharp_handle list, int index)
{
    wxBitmap bitmap = static_cast<wxImageList*>(list)->GetBitmap(index);
    return bitmap.IsOk() ? new wxBitmap(bitmap) : nullptr;
}

// The control takes ownership of the list, which is what wxWidgets' AssignImageList means; SetImageList
// leaves it with the caller. Both are exposed because the difference decides who has to keep it alive.
void wxsharp_listctrl_set_image_list(wxsharp_handle ctrl, wxsharp_handle list, int which, bool transfer)
{
    auto* control = static_cast<wxListCtrl*>(ctrl);
    auto* images = static_cast<wxImageList*>(list);
    if (transfer) control->AssignImageList(images, which);
    else control->SetImageList(images, which);
}

void wxsharp_treectrl_set_image_list(wxsharp_handle ctrl, wxsharp_handle list, bool transfer)
{
    auto* control = static_cast<wxTreeCtrl*>(ctrl);
    auto* images = static_cast<wxImageList*>(list);
    if (transfer) control->AssignImageList(images);
    else control->SetImageList(images);
}

void wxsharp_listctrl_set_item_image(wxsharp_handle ctrl, long long item, int image)
{
    static_cast<wxListCtrl*>(ctrl)->SetItemImage(static_cast<long>(item), image);
}

void wxsharp_treectrl_set_item_image(wxsharp_handle ctrl, long long item, int image, int which)
{
    static_cast<wxTreeCtrl*>(ctrl)->SetItemImage(TreeId(item), image,
                                                 static_cast<wxTreeItemIcon>(which));
}

int wxsharp_treectrl_get_item_image(wxsharp_handle ctrl, long long item, int which)
{
    return static_cast<wxTreeCtrl*>(ctrl)->GetItemImage(TreeId(item), static_cast<wxTreeItemIcon>(which));
}

// ---- wxCaret --------------------------------------------------------------------------------------------
// The blinking insertion point in a custom-drawn control. Keeping it where the text is matters beyond the
// look: assistive technology and the platform's own input methods both follow it.

void wxsharp_control_set_caret(wxsharp_handle ctrl, int width, int height)
{
    auto* window = static_cast<wxWindow*>(ctrl);
    window->SetCaret(width > 0 && height > 0 ? new wxCaret(window, width, height) : nullptr);
}

bool wxsharp_control_has_caret(wxsharp_handle ctrl) { return static_cast<wxWindow*>(ctrl)->GetCaret() != nullptr; }

void wxsharp_caret_move(wxsharp_handle ctrl, int x, int y)
{
    if (auto* caret = static_cast<wxWindow*>(ctrl)->GetCaret()) caret->Move(x, y);
}

void wxsharp_caret_show(wxsharp_handle ctrl, bool show)
{
    if (auto* caret = static_cast<wxWindow*>(ctrl)->GetCaret()) caret->Show(show);
}

bool wxsharp_caret_is_visible(wxsharp_handle ctrl)
{
    auto* caret = static_cast<wxWindow*>(ctrl)->GetCaret();
    return caret && caret->IsVisible();
}

void wxsharp_caret_position(wxsharp_handle ctrl, int* x, int* y)
{
    if (auto* caret = static_cast<wxWindow*>(ctrl)->GetCaret())
    {
        const wxPoint p = caret->GetPosition();
        if (x) *x = p.x;
        if (y) *y = p.y;
    }
}

int  wxsharp_caret_get_blink_time() { return wxCaret::GetBlinkTime(); }
void wxsharp_caret_set_blink_time(int milliseconds) { wxCaret::SetBlinkTime(milliseconds); }

// ---- wxAboutBox -----------------------------------------------------------------------------------------
// The platform's own about dialog, which on some platforms is a native panel rather than a window wxWidgets
// draws - so it looks right and reads right without being laid out by hand.

void wxsharp_about_box(const char* name, const char* version, const char* description,
                       const char* copyright, const char* website, const char* website_label,
                       const char* const* developers, int developer_count, wxsharp_handle parent)
{
    wxAboutDialogInfo info;
    if (name && *name) info.SetName(Str(name));
    if (version && *version) info.SetVersion(Str(version));
    if (description && *description) info.SetDescription(Str(description));
    if (copyright && *copyright) info.SetCopyright(Str(copyright));
    if (website && *website)
        info.SetWebSite(Str(website), website_label && *website_label ? Str(website_label) : wxString());
    for (int i = 0; i < developer_count; ++i)
        if (developers && developers[i]) info.AddDeveloper(Str(developers[i]));
    wxAboutBox(info, static_cast<wxWindow*>(parent));
}

// ---- wxRichToolTip --------------------------------------------------------------------------------------
// A tooltip with a title, an icon and more than one line - what a validation message wants, rather than the
// single line an ordinary tooltip allows.

void wxsharp_rich_tooltip_show(wxsharp_handle window, const char* title, const char* message,
                               int icon, int timeout_ms, int show_delay_ms)
{
    wxRichToolTip tip(Str(title), Str(message));
    if (icon != 0) tip.SetIcon(icon);
    if (timeout_ms > 0 || show_delay_ms > 0)
        tip.SetTimeout(static_cast<unsigned>(timeout_ms > 0 ? timeout_ms : 0),
                       static_cast<unsigned>(show_delay_ms > 0 ? show_delay_ms : 0));
    tip.ShowFor(static_cast<wxWindow*>(window));
}
