///////////////////////////////////////////////////////////////////////////////
// Name:        wx/notifmsg.h
// Purpose:     class allowing to show notification messages to the user
// Author:      Vadim Zeitlin
// Created:     2007-11-19
// Copyright:   (c) 2007 Vadim Zeitlin <vadim@wxwidgets.org>
// Licence:     wxWindows licence
///////////////////////////////////////////////////////////////////////////////

#ifndef _WX_NOTIFMSG_H_
#define _WX_NOTIFMSG_H_

#include "wx/event.h"

#if wxUSE_NOTIFICATION_MESSAGE

class wxIcon;

class WXDLLIMPEXP_CORE wxNotificationMessageBase : public wxEvtHandler
{
public:
    wxNotificationMessageBase() { Init(); }
    wxNotificationMessageBase(const wxString& title,
                              const wxString& message = wxEmptyString,
                              wxWindow* parent = nullptr,
                              int flags = wxICON_INFORMATION)
    {
        Init();
        Create(title, message, parent, flags);
    }

    virtual ~wxNotificationMessageBase();

    void SetTitle(const wxString& title);
    void SetMessage(const wxString& message);
    void SetParent(wxWindow* parent);
    void SetFlags(int flags);
    virtual void SetIcon(const wxIcon& icon);
    virtual bool AddAction(wxWindowID actionid, const wxString& label = wxString());

    enum
    {
        Timeout_Auto = -1,
        Timeout_Never = 0
    };

    enum class DismissalReason
    {
        Unknown,
        ByUser,
        ByApp,
        TimedOut
    };

    bool Show(int timeout = Timeout_Auto);
    bool Close();

protected:
    void Create(const wxString& title = wxEmptyString,
                const wxString& message = wxEmptyString,
                wxWindow* parent = nullptr,
                int flags = wxICON_INFORMATION)
    {
        SetTitle(title);
        SetMessage(message);
        SetParent(parent);
        SetFlags(flags);
    }

    class wxNotificationMessageImpl* m_impl;

private:
    void Init() { m_impl = nullptr; }
    wxDECLARE_NO_COPY_CLASS(wxNotificationMessageBase);
};

wxDECLARE_EXPORTED_EVENT(WXDLLIMPEXP_CORE, wxEVT_NOTIFICATION_MESSAGE_CLICK, wxCommandEvent);
wxDECLARE_EXPORTED_EVENT(WXDLLIMPEXP_CORE, wxEVT_NOTIFICATION_MESSAGE_DISMISSED, wxCommandEvent);
wxDECLARE_EXPORTED_EVENT(WXDLLIMPEXP_CORE, wxEVT_NOTIFICATION_MESSAGE_ACTION, wxCommandEvent);

#if (defined(__WXGTK__) && wxUSE_LIBNOTIFY) || \
    (defined(__WXMSW__) && wxUSE_TASKBARICON && wxUSE_TASKBARICON_BALLOONS) || \
    defined(__WXOSX_COCOA__)
    #define wxHAS_NATIVE_NOTIFICATION_MESSAGE
#endif

#ifndef wxHAS_NATIVE_NOTIFICATION_MESSAGE
#include "wx/generic/notifmsg.h"
#endif

#if defined(__WXMSW__) && defined(wxHAS_NATIVE_NOTIFICATION_MESSAGE)
class WXDLLIMPEXP_FWD_CORE wxTaskBarIcon;
#endif

class WXDLLIMPEXP_CORE wxNotificationMessage : public
#ifdef wxHAS_NATIVE_NOTIFICATION_MESSAGE
    wxNotificationMessageBase
#else
    wxGenericNotificationMessage
#endif
{
public:
    wxNotificationMessage() { Init(); }
    wxNotificationMessage(const wxString& title,
                          const wxString& message = wxString(),
                          wxWindow* parent = nullptr,
                          int flags = wxICON_INFORMATION)
    {
        Init();
        Create(title, message, parent, flags);
    }

#if defined(__WXMSW__) && defined(wxHAS_NATIVE_NOTIFICATION_MESSAGE)
    static bool MSWUseToasts(const wxString& shortcutPath = wxString(),
                             const wxString& appId = wxString());
    static wxTaskBarIcon* UseTaskBarIcon(wxTaskBarIcon* icon);
#endif

private:
    void Init();
    wxDECLARE_NO_COPY_CLASS(wxNotificationMessage);
};

#endif // wxUSE_NOTIFICATION_MESSAGE
#endif // _WX_NOTIFMSG_H_
