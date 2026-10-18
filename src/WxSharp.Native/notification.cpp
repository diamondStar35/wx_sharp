// Desktop notifications.
#include "internal.h"
#include <wx/notifmsg.h>

bool wxsharp_notification_use_toasts(const char* shortcut_path, const char* app_id)
{
#ifdef __WXMSW__
    return wxNotificationMessage::MSWUseToasts(Str(shortcut_path), Str(app_id));
#else
    (void)shortcut_path;
    (void)app_id;
    return false;
#endif
}

bool wxsharp_notification_show(wxsharp_handle parent, const char* title, const char* message,
                               int flags, int timeout)
{
#if wxUSE_NOTIFICATION_MESSAGE
    wxNotificationMessage notification(
        Str(title), Str(message), static_cast<wxWindow*>(parent), flags);
    return notification.Show(timeout);
#else
    (void)parent;
    (void)title;
    (void)message;
    (void)flags;
    (void)timeout;
    return false;
#endif
}
