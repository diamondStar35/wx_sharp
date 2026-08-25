// App lifetime: bringing wxWidgets up inside a host that owns the real loop, pumping/waiting, and teardown.
#include "internal.h"
#include <wx/evtloop.h>
#ifdef __WXMSW__
#include <windows.h>
#endif

wxsharp_event_cb g_event_cb = nullptr;
wxsharp_key_cb g_key_cb = nullptr;

namespace
{
    bool g_initialized = false;
    wxEventLoop* g_loop = nullptr;

    // The host (the .NET client) owns argument parsing and passes its own flags (e.g. --host 127.0.0.1) on the
    // process command line. wxApp's default OnInit runs wxCmdLineParser over that command line and rejects any
    // option it doesn't recognise, which would make CallOnInit fail and bring init down. We don't use wx's
    // command-line handling at all, so override OnInit to skip it and just succeed.
    class WxSharpApp : public wxApp
    {
    public:
        bool OnInit() override { return true; }
    };

    // On Windows, activate the Common-Controls v6 manifest embedded in this DLL.
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

    // The host owns the real loop, so we create an event loop and make it active ourselves; wxsharp_pump drives
    // it the way wxApp::MainLoop would, so tab traversal, accelerators, idle and repaint behave natively.
    g_loop = new wxEventLoop();
    wxEventLoopBase::SetActive(g_loop);

    g_initialized = true;
    return true;
}

void wxsharp_set_event_handler(wxsharp_event_cb cb)
{
    g_event_cb = cb;
}

void wxsharp_set_key_handler(wxsharp_key_cb cb)
{
    g_key_cb = cb;
}

void wxsharp_pump()
{
    if (!g_loop)
        return;
    while (g_loop->Pending())
        g_loop->Dispatch();
    if (wxTheApp)
        wxTheApp->ProcessPendingEvents();
    g_loop->ProcessIdle();
}

void wxsharp_wait(int timeout_ms)
{
#ifdef __WXMSW__
    ::MsgWaitForMultipleObjectsEx(
        0, nullptr,
        timeout_ms < 0 ? INFINITE : static_cast<DWORD>(timeout_ms),
        QS_ALLINPUT, MWMO_INPUTAVAILABLE);
#else
    if (!g_loop)
        return;
    if (timeout_ms < 0)
        g_loop->Dispatch();
    else if (timeout_ms > 0)
        g_loop->DispatchTimeout(static_cast<unsigned long>(timeout_ms));
#endif
}

int wxsharp_message_box(const char* message, const char* caption, int style)
{
    return wxMessageBox(Str(message), Str(caption), style);
}

void wxsharp_shutdown()
{
    if (!g_initialized)
        return;
    wxEventLoopBase::SetActive(nullptr);
    delete g_loop;
    g_loop = nullptr;
    wxTheApp->OnExit();
    wxEntryCleanup();
    g_initialized = false;
}
