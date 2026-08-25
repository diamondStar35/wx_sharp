// Top-level window (wxFrame) with a vertical content panel.
#include "internal.h"

wxsharp_handle wxsharp_window_create(const char* title, int width, int height, int id, bool with_panel)
{
    auto* frame = new wxFrame(nullptr, wxID_ANY, Str(title), wxDefaultPosition, wxSize(width, height));
    if (with_panel)
        SetupContentPanel(frame);
    else
        SetupBareContent(frame);
    frame->Center();
    BindKeyHook(frame, id);

    // Lifecycle events the app can hook (the events do their normal processing too, via Skip).
    frame->Bind(wxEVT_SHOW, [id](wxShowEvent& e)
    {
        if (e.IsShown())
            Fire(id, WXSHARP_EVT_SHOWN);
        e.Skip();
    });
    frame->Bind(wxEVT_ACTIVATE, [id](wxActivateEvent& e)
    {
        Fire(id, e.GetActive() ? WXSHARP_EVT_ACTIVATE : WXSHARP_EVT_DEACTIVATE);
        e.Skip();
    });
    frame->Bind(wxEVT_SIZE, [id](wxSizeEvent& e)
    {
        Fire(id, WXSHARP_EVT_RESIZE);
        e.Skip();
    });
    frame->Bind(wxEVT_MOVE, [id](wxMoveEvent& e)
    {
        Fire(id, WXSHARP_EVT_MOVE);
        e.Skip();
    });
    frame->Bind(wxEVT_MAXIMIZE, [id](wxMaximizeEvent& e)
    {
        Fire(id, WXSHARP_EVT_MAXIMIZE);
        e.Skip();
    });
    frame->Bind(wxEVT_CLOSE_WINDOW, [id](wxCloseEvent& e)
    {
        Fire(id, WXSHARP_EVT_CLOSE);
        if (auto* f = wxDynamicCast(e.GetEventObject(), wxFrame))
            f->Destroy();
    });
    return frame;
}

wxsharp_handle wxsharp_window_panel(wxsharp_handle window)
{
    return ContentPanel(static_cast<wxWindow*>(window));
}

void wxsharp_window_show(wxsharp_handle window, bool show)
{
    auto* frame = static_cast<wxFrame*>(window);
    frame->Show(show);
    if (show)
    {
        // Bring it forward and focus the first control, so the user can tab/type without clicking first.
        frame->Raise();
        FocusFirst(frame);
    }
}

void wxsharp_window_set_title(wxsharp_handle window, const char* title)
{
    static_cast<wxFrame*>(window)->SetTitle(Str(title));
}

void wxsharp_window_layout(wxsharp_handle window) { static_cast<wxFrame*>(window)->Layout(); }
void wxsharp_window_center(wxsharp_handle window) { static_cast<wxFrame*>(window)->Center(); }
void wxsharp_window_close(wxsharp_handle window) { static_cast<wxFrame*>(window)->Close(); }
void wxsharp_window_destroy(wxsharp_handle window) { static_cast<wxFrame*>(window)->Destroy(); }

void wxsharp_window_set_fullscreen(wxsharp_handle window, bool fullscreen)
{
    // wxFULLSCREEN_ALL drops the caption, borders, tool/status bars and any menu bar - true borderless.
    static_cast<wxFrame*>(window)->ShowFullScreen(fullscreen, wxFULLSCREEN_ALL);
}

void* wxsharp_window_native_handle(wxsharp_handle window)
{
    return static_cast<wxFrame*>(window)->GetHandle();
}
