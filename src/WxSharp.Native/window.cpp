// Top-level frame. Content and layout are explicit.
#include "internal.h"

wxsharp_handle wxsharp_window_create(wxsharp_handle parent, int id, const char* title,
                                     int x, int y, int width, int height, long long token)
{
    auto* frame = new wxFrame(static_cast<wxWindow*>(parent), id, Str(title), wxPoint(x, y), wxSize(width, height));
    BindCommon(frame, token);
    BindKeyHook(frame, token);
    frame->Bind(wxEVT_SHOW, [token](wxShowEvent& e)
    {
        if (e.IsShown()) Fire(token, WXSHARP_EVT_SHOWN, e.GetId(), 0, 0, 0, 0, 0, 0, 0, 0, true);
        e.Skip();
    });
    frame->Bind(wxEVT_ACTIVATE, [token](wxActivateEvent& e)
    {
        Fire(token, e.GetActive() ? WXSHARP_EVT_ACTIVATE : WXSHARP_EVT_DEACTIVATE,
             e.GetId(), 0, 0, 0, 0, 0, 0, 0, 0, e.GetActive());
        e.Skip();
    });
    frame->Bind(wxEVT_SIZE, [token](wxSizeEvent& e)
    {
        const wxSize size = e.GetSize();
        Fire(token, WXSHARP_EVT_RESIZE, e.GetId(), 0, 0, size.x, size.y);
        e.Skip();
    });
    frame->Bind(wxEVT_MOVE, [token](wxMoveEvent& e)
    {
        const wxPoint position = e.GetPosition();
        Fire(token, WXSHARP_EVT_MOVE, e.GetId(), position.x, position.y);
        e.Skip();
    });
    frame->Bind(wxEVT_MAXIMIZE, [token](wxMaximizeEvent& e) { Fire(token, WXSHARP_EVT_MAXIMIZE, e.GetId()); e.Skip(); });
    frame->Bind(wxEVT_CLOSE_WINDOW, [token](wxCloseEvent& e)
    {
        const unsigned int result = Fire(token, WXSHARP_EVT_CLOSE, e.GetId(), 0, 0, 0, 0, 0, 0, 0, 0,
                                         false, e.CanVeto());
        if ((result & WXSHARP_EVENT_CANCEL) && e.CanVeto()) e.Veto(); else e.Skip();
    });
    frame->Bind(wxEVT_MENU, [token](wxCommandEvent& e)
    {
        if (!(Fire(token, WXSHARP_EVT_MENU, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip();
    });
    return frame;
}

void wxsharp_window_show(wxsharp_handle window, bool show) { static_cast<wxFrame*>(window)->Show(show); }
void wxsharp_window_set_title(wxsharp_handle window, const char* title) { static_cast<wxFrame*>(window)->SetTitle(Str(title)); }
int wxsharp_window_get_title(wxsharp_handle window, char* buffer, int buffer_length)
{
    return CopyToBuffer(static_cast<wxFrame*>(window)->GetTitle(), buffer, buffer_length);
}
void wxsharp_window_center(wxsharp_handle window) { static_cast<wxFrame*>(window)->Center(); }
void wxsharp_window_close(wxsharp_handle window) { static_cast<wxFrame*>(window)->Close(); }
void wxsharp_window_destroy(wxsharp_handle window) { static_cast<wxFrame*>(window)->Destroy(); }
void wxsharp_window_set_fullscreen(wxsharp_handle window, bool fullscreen)
{
    static_cast<wxFrame*>(window)->ShowFullScreen(fullscreen, wxFULLSCREEN_ALL);
}
void* wxsharp_window_native_handle(wxsharp_handle window) { return static_cast<wxFrame*>(window)->GetHandle(); }
