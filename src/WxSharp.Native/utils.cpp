// The free functions wxWidgets puts at namespace scope: opening a file or a URL in whatever the user has
// chosen for it, running a command, reading the environment, and describing the machine.
#include "internal.h"
#include <wx/utils.h>
#include <wx/versioninfo.h>
#include <wx/mousestate.h>

namespace
{
    // wxWindowDisabler has no default constructor that can be re-armed, so the scope is held on the heap
    // for as long as the managed side keeps it.
    struct DisablerScope
    {
        wxWindowDisabler disabler;

        explicit DisablerScope(wxWindow* skip) : disabler(skip) {}
    };
}

// ---- Handing a file or a URL to the user's own choice of program -------------------------------------------
// Both of these ask the desktop what is registered for the thing, rather than guessing at a browser or an
// editor. That is what makes them respect the user's defaults, including an accessible browser they have
// deliberately set as the default.

bool wxsharp_launch_default_browser(const char* url, int flags)
{
    return wxLaunchDefaultBrowser(Str(url), flags);
}

bool wxsharp_launch_default_application(const char* path, int flags)
{
    return wxLaunchDefaultApplication(Str(path), flags);
}

// Returns the exit code when run synchronously, the process ID when asynchronous, and -1 or 0 on failure
// respectively - wxExecute's own convention, kept rather than normalised.
long long wxsharp_execute(const char* command, int flags)
{
    return wxExecute(Str(command), flags);
}

long long wxsharp_shell(const char* command)
{
    return wxShell(Str(command)) ? 0 : -1;
}

// ---- Feedback ---------------------------------------------------------------------------------------------

// The system alert sound. Worth preferring over a bespoke sound: the user may have replaced or silenced it,
// and a screen reader may be watching for it.
void wxsharp_bell() { wxBell(); }

bool wxsharp_get_key_state(int key)
{
    return wxGetKeyState(static_cast<wxKeyCode>(key));
}

void wxsharp_get_mouse_position(int* x, int* y)
{
    const wxPoint point = wxGetMousePosition();
    if (x) *x = point.x;
    if (y) *y = point.y;
}

void wxsharp_get_mouse_state(int* x, int* y, int* buttons, int* modifiers)
{
    const wxMouseState state = wxGetMouseState();
    if (x) *x = state.GetX();
    if (y) *y = state.GetY();
    if (buttons)
    {
        *buttons = (state.LeftIsDown() ? 1 : 0)
                 | (state.MiddleIsDown() ? 2 : 0)
                 | (state.RightIsDown() ? 4 : 0)
                 | (state.Aux1IsDown() ? 8 : 0)
                 | (state.Aux2IsDown() ? 16 : 0);
    }
    if (modifiers) *modifiers = state.GetModifiers();
}

// ---- Who and where ------------------------------------------------------------------------------------------

int wxsharp_get_user_id(char* buffer, int buffer_length) { return CopyToBuffer(wxGetUserId(), buffer, buffer_length); }
int wxsharp_get_user_name(char* buffer, int buffer_length) { return CopyToBuffer(wxGetUserName(), buffer, buffer_length); }
int wxsharp_get_host_name(char* buffer, int buffer_length) { return CopyToBuffer(wxGetHostName(), buffer, buffer_length); }
int wxsharp_get_full_host_name(char* buffer, int buffer_length) { return CopyToBuffer(wxGetFullHostName(), buffer, buffer_length); }
int wxsharp_get_email_address(char* buffer, int buffer_length) { return CopyToBuffer(wxGetEmailAddress(), buffer, buffer_length); }
int wxsharp_get_home_dir(char* buffer, int buffer_length) { return CopyToBuffer(wxGetHomeDir(), buffer, buffer_length); }

// ---- What the machine is ------------------------------------------------------------------------------------

int wxsharp_get_os_description(char* buffer, int buffer_length)
{
    return CopyToBuffer(wxGetOsDescription(), buffer, buffer_length);
}

int wxsharp_get_os_version(int* major, int* minor, int* micro)
{
    int ma = 0, mi = 0, mu = 0;
    const int id = wxGetOsVersion(&ma, &mi, &mu);
    if (major) *major = ma;
    if (minor) *minor = mi;
    if (micro) *micro = mu;
    return id;
}

bool wxsharp_check_os_version(int major, int minor, int micro)
{
    return wxCheckOsVersion(major, minor, micro);
}

bool wxsharp_is_platform_64bit() { return wxIsPlatform64Bit(); }
bool wxsharp_is_platform_little_endian() { return wxIsPlatformLittleEndian(); }

int wxsharp_get_cpu_architecture_name(char* buffer, int buffer_length)
{
    return CopyToBuffer(wxGetCpuArchitectureName(), buffer, buffer_length);
}

int wxsharp_get_native_cpu_architecture_name(char* buffer, int buffer_length)
{
    return CopyToBuffer(wxGetNativeCpuArchitectureName(), buffer, buffer_length);
}

int wxsharp_get_library_version(char* buffer, int buffer_length)
{
    return CopyToBuffer(wxGetLibraryVersionInfo().ToString(), buffer, buffer_length);
}

unsigned int wxsharp_get_process_id() { return static_cast<unsigned int>(wxGetProcessId()); }

long long wxsharp_get_free_memory() { return static_cast<long long>(wxGetFreeMemory().GetValue()); }

bool wxsharp_get_disk_space(const char* path, long long* total, long long* free_space)
{
    wxDiskspaceSize_t totalSize = 0, freeSize = 0;
    if (!wxGetDiskSpace(Str(path), &totalSize, &freeSize))
        return false;
    if (total) *total = static_cast<long long>(totalSize.GetValue());
    if (free_space) *free_space = static_cast<long long>(freeSize.GetValue());
    return true;
}

// ---- The environment ------------------------------------------------------------------------------------------

// Returns -1 when the variable is not set, which is what distinguishes that from a variable set to nothing.
int wxsharp_get_env(const char* name, char* buffer, int buffer_length)
{
    wxString value;
    if (!wxGetEnv(Str(name), &value))
        return -1;
    return CopyToBuffer(value, buffer, buffer_length);
}

bool wxsharp_set_env(const char* name, const char* value) { return wxSetEnv(Str(name), Str(value)); }
bool wxsharp_unset_env(const char* name) { return wxUnsetEnv(Str(name)); }

// ---- Sleeping -------------------------------------------------------------------------------------------------
// These block the calling thread outright, so on the UI thread they freeze the interface. They are here for
// completeness; a background thread is almost always the better answer.

void wxsharp_sleep(int seconds) { wxSleep(seconds); }
void wxsharp_milli_sleep(unsigned long milliseconds) { wxMilliSleep(milliseconds); }
void wxsharp_micro_sleep(unsigned long microseconds) { wxMicroSleep(microseconds); }

// ---- Windows --------------------------------------------------------------------------------------------------

wxsharp_handle wxsharp_find_window_by_name(const char* name, wxsharp_handle parent)
{
    return wxWindow::FindWindowByName(Str(name), static_cast<wxWindow*>(parent));
}

wxsharp_handle wxsharp_find_window_by_label(const char* label, wxsharp_handle parent)
{
    return wxWindow::FindWindowByLabel(Str(label), static_cast<wxWindow*>(parent));
}

wxsharp_handle wxsharp_find_window_at_point(int x, int y)
{
    return wxFindWindowAtPoint(wxPoint(x, y));
}

wxsharp_handle wxsharp_get_active_window() { return wxGetActiveWindow(); }

void wxsharp_enable_top_level_windows(bool enable) { wxEnableTopLevelWindows(enable); }

wxsharp_handle wxsharp_window_disabler_begin(wxsharp_handle skip)
{
    return new DisablerScope(static_cast<wxWindow*>(skip));
}

void wxsharp_window_disabler_end(wxsharp_handle scope)
{
    delete static_cast<DisablerScope*>(scope);
}

// Turns "&File" into "File" and "E&xit\tCtrl+Q" into "Exit". Useful anywhere a menu label has to be shown
// or spoken outside a menu, where the mnemonic ampersand would otherwise be read out.
int wxsharp_strip_menu_codes(const char* text, char* buffer, int buffer_length)
{
    return CopyToBuffer(wxStripMenuCodes(Str(text)), buffer, buffer_length);
}
