// wxWidgets application lifetime. Managed App owns this one-shot application and enters wx's real loop.
#include "internal.h"
#ifdef __WXMSW__
#include <windows.h>
#endif

wxsharp_event_cb g_event_cb = nullptr;

namespace
{
    bool g_initialized = false;

    class WxSharpApp : public wxApp
    {
    public:
        bool OnInit() override { return true; }
    };

    void EnableCommonControlsV6()
    {
#ifdef __WXMSW__
        HMODULE self = nullptr;
        if (!::GetModuleHandleExW(
                GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
                reinterpret_cast<LPCWSTR>(&EnableCommonControlsV6), &self) || self == nullptr)
            return;
        ACTCTXW ctx = {};
        ctx.cbSize = sizeof(ctx);
        ctx.dwFlags = ACTCTX_FLAG_HMODULE_VALID | ACTCTX_FLAG_RESOURCE_NAME_VALID;
        ctx.hModule = self;
        ctx.lpResourceName = MAKEINTRESOURCEW(100);
        HANDLE handle = ::CreateActCtxW(&ctx);
        if (handle != INVALID_HANDLE_VALUE)
        {
            ULONG_PTR cookie = 0;
            ::ActivateActCtx(handle, &cookie);
        }
#endif
    }
}

bool wxsharp_init()
{
    if (g_initialized)
        return true;
    EnableCommonControlsV6();
    wxApp::SetInstance(new WxSharpApp());
    int argc = 0;
    wxChar** argv = nullptr;
    if (!wxEntryStart(argc, argv))
        return false;
    if (!wxTheApp->CallOnInit())
    {
        wxEntryCleanup();
        return false;
    }
    wxInitAllImageHandlers();
    g_initialized = true;
    return true;
}

void wxsharp_set_event_handler(wxsharp_event_cb cb) { g_event_cb = cb; }

int wxsharp_main_loop()
{
    if (!g_initialized || !wxTheApp || wxTopLevelWindows.empty())
        return 0;
    return wxTheApp->MainLoop();
}

void wxsharp_exit_main_loop()
{
    if (wxTheApp && wxTheApp->IsMainLoopRunning())
        wxTheApp->ExitMainLoop();
}

void wxsharp_set_exit_on_frame_delete(bool value)
{
    if (wxTheApp)
        wxTheApp->SetExitOnFrameDelete(value);
}

void wxsharp_set_top_window(wxsharp_handle window)
{
    if (wxTheApp)
        wxTheApp->SetTopWindow(static_cast<wxWindow*>(window));
}

void wxsharp_call_after(long long token)
{
    if (wxTheApp)
        wxTheApp->CallAfter([token]() { Fire(token, WXSHARP_EV_CALL_AFTER); });
}

bool wxsharp_yield(bool only_if_needed)
{
    return wxTheApp && wxTheApp->Yield(only_if_needed);
}

int wxsharp_message_box(wxsharp_handle parent, const char* message, const char* caption, int style)
{
    // Passing the parent is what makes the box modal to the right window and puts it in the right place in
    // the window hierarchy, which is also how a screen reader knows what it belongs to.
    return wxMessageBox(Str(message), Str(caption), style, static_cast<wxWindow*>(parent));
}

void wxsharp_shutdown()
{
    if (!g_initialized)
        return;
    g_event_cb = nullptr;
    if (wxTheApp)
        wxTheApp->OnExit();
    wxEntryCleanup();
    g_initialized = false;
}
