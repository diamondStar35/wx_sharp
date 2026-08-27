// The rest of wxFrame and wxTopLevelWindow: the window state a shell integration cares about (minimised,
// maximised, full screen), the frame-owned bars, and geometry persistence.
#include "internal.h"
#include <wx/frame.h>
#include <wx/statusbr.h>
#include <wx/toolbar.h>
#include <wx/menu.h>
#include <wx/iconbndl.h>
#include <map>
#include <string>

namespace
{
    wxFrame* Fr(wxsharp_handle h) { return static_cast<wxFrame*>(h); }
    wxTopLevelWindow* Top(wxsharp_handle h) { return static_cast<wxTopLevelWindow*>(h); }

    // The icons from the last wxsharp_frame_get_icons call, so each can be handed over one at a time.
    wxIconBundle& LastIcons()
    {
        static wxIconBundle icons;
        return icons;
    }

    // A GeometryStore that keeps the values in a string, so window placement can be saved to whatever
    // settings file the application already has. The format is "name=value;" repeated - the names are
    // wxWidgets' own and vary by platform, which is exactly why they are not exposed individually.
    class StringGeometryStore : public wxTopLevelWindow::GeometryStore
    {
    public:
        StringGeometryStore() = default;

        explicit StringGeometryStore(const wxString& text)
        {
            wxString rest = text;
            while (!rest.empty())
            {
                wxString entry = rest.BeforeFirst(wxT(';'), &rest);
                if (entry.empty())
                    continue;
                wxString value;
                const wxString name = entry.BeforeFirst(wxT('='), &value);
                long parsed = 0;
                if (!name.empty() && value.ToLong(&parsed))
                    values_[std::string(name.utf8_string())] = static_cast<int>(parsed);
            }
        }

        bool SaveValue(const wxString& name, int value) override
        {
            values_[std::string(name.utf8_string())] = value;
            return true;
        }

        bool RestoreValue(const wxString& name, int* value) const override
        {
            const auto found = values_.find(std::string(name.utf8_string()));
            if (found == values_.end())
                return false;
            if (value) *value = found->second;
            return true;
        }

        wxString ToText() const
        {
            wxString text;
            for (const auto& pair : values_)
            {
                text += wxString::FromUTF8(pair.first.c_str());
                text += wxString::Format(wxT("=%d;"), pair.second);
            }
            return text;
        }

    private:
        std::map<std::string, int> values_;
    };
}

// ---- Window state -----------------------------------------------------------------------------------------

void wxsharp_frame_iconize(wxsharp_handle frame, bool iconize) { Top(frame)->Iconize(iconize); }
bool wxsharp_frame_is_iconized(wxsharp_handle frame) { return Top(frame)->IsIconized(); }
void wxsharp_frame_maximize(wxsharp_handle frame, bool maximize) { Top(frame)->Maximize(maximize); }
bool wxsharp_frame_is_maximized(wxsharp_handle frame) { return Top(frame)->IsMaximized(); }
bool wxsharp_frame_is_always_maximized(wxsharp_handle frame) { return Top(frame)->IsAlwaysMaximized(); }
void wxsharp_frame_restore(wxsharp_handle frame) { Top(frame)->Restore(); }
bool wxsharp_frame_is_active(wxsharp_handle frame) { return Top(frame)->IsActive(); }

bool wxsharp_frame_show_full_screen(wxsharp_handle frame, bool show, int style)
{
    return Top(frame)->ShowFullScreen(show, style);
}

bool wxsharp_frame_is_full_screen(wxsharp_handle frame) { return Top(frame)->IsFullScreen(); }

bool wxsharp_frame_enable_full_screen_view(wxsharp_handle frame, bool enable, int style)
{
    return Top(frame)->EnableFullScreenView(enable, style);
}

void wxsharp_frame_show_without_activating(wxsharp_handle frame) { Top(frame)->ShowWithoutActivating(); }

// Flashes the taskbar button when the window is not in front. The polite way to say "something happened
// here" without stealing focus from whatever the user is doing.
void wxsharp_frame_request_user_attention(wxsharp_handle frame, int flags)
{
    Top(frame)->RequestUserAttention(flags);
}

bool wxsharp_frame_enable_close_button(wxsharp_handle frame, bool enable) { return Top(frame)->EnableCloseButton(enable); }
bool wxsharp_frame_enable_maximize_button(wxsharp_handle frame, bool enable) { return Top(frame)->EnableMaximizeButton(enable); }
bool wxsharp_frame_enable_minimize_button(wxsharp_handle frame, bool enable) { return Top(frame)->EnableMinimizeButton(enable); }

void wxsharp_frame_centre_on_screen(wxsharp_handle frame, int direction) { Top(frame)->CentreOnScreen(direction); }

int wxsharp_frame_get_content_protection(wxsharp_handle frame)
{
    return static_cast<int>(Top(frame)->GetContentProtection());
}

bool wxsharp_frame_set_content_protection(wxsharp_handle frame, int protection)
{
    return Top(frame)->SetContentProtection(static_cast<wxContentProtection>(protection));
}

void wxsharp_frame_set_represented_filename(wxsharp_handle frame, const char* path)
{
    Top(frame)->SetRepresentedFilename(Str(path));
}

void wxsharp_frame_set_window_modality(wxsharp_handle frame, int modality)
{
    Fr(frame)->SetWindowModality(static_cast<wxWindowMode>(modality));
}

void wxsharp_frame_get_default_size(int* width, int* height)
{
    const wxSize size = wxTopLevelWindow::GetDefaultSize();
    if (width) *width = size.x;
    if (height) *height = size.y;
}

wxsharp_handle wxsharp_frame_get_default_item(wxsharp_handle frame) { return Top(frame)->GetDefaultItem(); }

wxsharp_handle wxsharp_frame_set_default_item(wxsharp_handle frame, wxsharp_handle window)
{
    return Top(frame)->SetDefaultItem(static_cast<wxWindow*>(window));
}

// ---- Icons ------------------------------------------------------------------------------------------------
// A frame holds a bundle so the platform can pick the size it wants; these move one icon at a time across
// the boundary rather than exposing wxIconBundle itself.

wxsharp_handle wxsharp_frame_get_icon(wxsharp_handle frame)
{
    const wxIcon icon = Top(frame)->GetIcon();
    return icon.IsOk() ? new wxIcon(icon) : nullptr;
}

void wxsharp_frame_set_icons(wxsharp_handle frame, wxsharp_handle* icons, int count)
{
    wxIconBundle bundle;
    for (int i = 0; i < count; ++i)
    {
        if (icons[i] != nullptr)
            bundle.AddIcon(*static_cast<wxIcon*>(icons[i]));
    }
    Top(frame)->SetIcons(bundle);
}

int wxsharp_frame_get_icons(wxsharp_handle frame)
{
    LastIcons() = Top(frame)->GetIcons();
    return static_cast<int>(LastIcons().GetIconCount());
}

wxsharp_handle wxsharp_frame_get_icon_at(int index)
{
    const wxIconBundle& icons = LastIcons();
    if (index < 0 || static_cast<size_t>(index) >= icons.GetIconCount())
        return nullptr;
    return new wxIcon(icons.GetIconByIndex(index));
}

// ---- The frame-owned bars ---------------------------------------------------------------------------------

wxsharp_handle wxsharp_frame_get_menubar(wxsharp_handle frame) { return Fr(frame)->GetMenuBar(); }

wxsharp_handle wxsharp_frame_find_item_in_menubar(wxsharp_handle frame, int id)
{
    return Fr(frame)->FindItemInMenuBar(id);
}

wxsharp_handle wxsharp_frame_get_statusbar(wxsharp_handle frame) { return Fr(frame)->GetStatusBar(); }

void wxsharp_frame_set_statusbar(wxsharp_handle frame, wxsharp_handle bar)
{
    Fr(frame)->SetStatusBar(static_cast<wxStatusBar*>(bar));
}

wxsharp_handle wxsharp_frame_create_statusbar(wxsharp_handle frame, int fields, int style, int id,
                                              long long token)
{
    auto* bar = Fr(frame)->CreateStatusBar(fields, style, id);
    TrackWindow(bar, token);
    return bar;
}

void wxsharp_frame_set_status_text(wxsharp_handle frame, const char* text, int field)
{
    Fr(frame)->SetStatusText(Str(text), field);
}

void wxsharp_frame_push_status_text(wxsharp_handle frame, const char* text, int field)
{
    Fr(frame)->PushStatusText(Str(text), field);
}

void wxsharp_frame_pop_status_text(wxsharp_handle frame, int field) { Fr(frame)->PopStatusText(field); }

void wxsharp_frame_set_status_widths(wxsharp_handle frame, const int* widths, int count)
{
    if (widths == nullptr || count <= 0)
        return;
    Fr(frame)->SetStatusWidths(count, widths);
}

int wxsharp_frame_get_status_bar_pane(wxsharp_handle frame) { return Fr(frame)->GetStatusBarPane(); }
void wxsharp_frame_set_status_bar_pane(wxsharp_handle frame, int pane) { Fr(frame)->SetStatusBarPane(pane); }

wxsharp_handle wxsharp_frame_get_toolbar(wxsharp_handle frame) { return Fr(frame)->GetToolBar(); }

void wxsharp_frame_set_toolbar(wxsharp_handle frame, wxsharp_handle bar)
{
    Fr(frame)->SetToolBar(static_cast<wxToolBar*>(bar));
}

wxsharp_handle wxsharp_frame_create_toolbar(wxsharp_handle frame, int style, int id, long long token)
{
    auto* bar = Fr(frame)->CreateToolBar(style, id);
    TrackWindow(bar, token);
    return bar;
}

// Whether new frames use the platform status bar or wxWidgets' own drawing. Windows only; a no-op
// elsewhere, which is why it is guarded rather than conditionally exported.
void wxsharp_frame_use_native_statusbar(bool native)
{
#ifdef __WXMSW__
    wxFrame::UseNativeStatusBar(native);
#else
    (void)native;
#endif
}

bool wxsharp_frame_uses_native_statusbar()
{
#ifdef __WXMSW__
    return wxFrame::UsesNativeStatusBar();
#else
    return true;
#endif
}

// ---- Geometry persistence ---------------------------------------------------------------------------------

int wxsharp_frame_save_geometry(wxsharp_handle frame, char* buffer, int buffer_length)
{
    StringGeometryStore store;
    if (!Top(frame)->SaveGeometry(store))
        return -1;
    return CopyToBuffer(store.ToText(), buffer, buffer_length);
}

bool wxsharp_frame_restore_to_geometry(wxsharp_handle frame, const char* text)
{
    StringGeometryStore store{ Str(text) };
    return Top(frame)->RestoreToGeometry(store);
}
