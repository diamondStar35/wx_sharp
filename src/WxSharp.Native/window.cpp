// Top-level frame. Content and layout are explicit.
#include "internal.h"

wxsharp_handle wxsharp_window_create(wxsharp_handle parent, int id, const char* title,
                                     int x, int y, int width, int height, int style, long long token)
{
    auto* frame = new wxFrame(static_cast<wxWindow*>(parent), id, Str(title), wxPoint(x, y),
                              wxSize(width, height), MapFrameStyle(style));
    TrackWindow(frame, token);
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
