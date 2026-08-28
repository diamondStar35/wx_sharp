// wxWidgets application lifetime. Managed App owns this one-shot application and enters wx's real loop.
#include "internal.h"
#include <cstdio>
#include <cstdlib>
#ifdef __WXMSW__
#include <windows.h>
#endif

wxsharp_event_cb g_event_cb = nullptr;

namespace
{
    bool g_initialized = false;

#if wxDEBUG_LEVEL
    void NonInteractiveAssertHandler(const wxString& file, int line, const wxString& func,
                                     const wxString& condition, const wxString& message)
    {
        const wxScopedCharBuffer fileUtf8 = file.utf8_str();
        const wxScopedCharBuffer funcUtf8 = func.utf8_str();
        const wxScopedCharBuffer conditionUtf8 = condition.utf8_str();
        const wxScopedCharBuffer messageUtf8 = message.utf8_str();
        std::fprintf(stderr,
                     "wxWidgets assertion failed at %s(%d) in %s: %s%s%s\n",
                     fileUtf8.data() ? fileUtf8.data() : "<unknown>", line,
                     funcUtf8.data() ? funcUtf8.data() : "<unknown>",
                     conditionUtf8.data() ? conditionUtf8.data() : "<unknown>",
                     message.empty() ? "" : " -- ",
                     messageUtf8.data() ? messageUtf8.data() : "");
        std::fflush(stderr);
        std::_Exit(86);
    }
#endif

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
#if wxDEBUG_LEVEL
    // Automated GUI tests must never open wxWidgets' modal Debug Alert: if the desktop is unattended or
    // the alert is behind the test window, that dialog can make the entire session appear frozen. This is
    // deliberately opt-in so applications retain wxWidgets/Phoenix's normal assertion behaviour.
    if (std::getenv("WXSHARP_TEST_NONINTERACTIVE"))
        wxSetAssertHandler(NonInteractiveAssertHandler);
#endif
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
    wxsharp_set_accessible_handler(nullptr);
    wxsharp_set_virtual_list_handler(nullptr);
    wxsharp_set_virtual_handler(nullptr);
    if (wxTheApp)
        wxTheApp->OnExit();
    wxEntryCleanup();
    g_initialized = false;
}
