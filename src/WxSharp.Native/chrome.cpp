#include "internal.h"
#include <wx/menu.h>
#include <wx/statusbr.h>
#include <wx/toolbar.h>
#include <wx/accel.h>

wxsharp_handle wxsharp_menu_create() { return new wxMenu(); }
void wxsharp_menu_destroy(wxsharp_handle menu) { delete static_cast<wxMenu*>(menu); }
void wxsharp_menu_append(wxsharp_handle menu, int id, const char* text, const char* help, int kind)
{
    const wxItemKind itemKind = kind == 1 ? wxITEM_CHECK : kind == 2 ? wxITEM_RADIO : wxITEM_NORMAL;
    static_cast<wxMenu*>(menu)->Append(id, Str(text), Str(help), itemKind);
}
void wxsharp_menu_append_separator(wxsharp_handle menu) { static_cast<wxMenu*>(menu)->AppendSeparator(); }
void wxsharp_menu_enable(wxsharp_handle menu, int id, bool enable) { static_cast<wxMenu*>(menu)->Enable(id, enable); }
void wxsharp_menu_check(wxsharp_handle menu, int id, bool check) { static_cast<wxMenu*>(menu)->Check(id, check); }
bool wxsharp_menu_is_checked(wxsharp_handle menu, int id) { return static_cast<wxMenu*>(menu)->IsChecked(id); }
wxsharp_handle wxsharp_menubar_create() { return new wxMenuBar(); }
void wxsharp_menubar_destroy(wxsharp_handle menuBar) { delete static_cast<wxMenuBar*>(menuBar); }
bool wxsharp_menubar_append(wxsharp_handle menuBar, wxsharp_handle menu, const char* title) { return static_cast<wxMenuBar*>(menuBar)->Append(static_cast<wxMenu*>(menu), Str(title)); }
void wxsharp_frame_set_menubar(wxsharp_handle frame, wxsharp_handle menuBar) { static_cast<wxFrame*>(frame)->SetMenuBar(static_cast<wxMenuBar*>(menuBar)); }
wxsharp_handle wxsharp_statusbar_create(wxsharp_handle frame, int fields, long long token)
{
    auto* status = static_cast<wxFrame*>(frame)->CreateStatusBar(fields); BindCommon(status, token); return status;
}
void wxsharp_statusbar_set_text(wxsharp_handle status, const char* text, int field) { static_cast<wxStatusBar*>(status)->SetStatusText(Str(text), field); }
int wxsharp_statusbar_get_text(wxsharp_handle status, int field, char* buffer, int length) { return CopyToBuffer(static_cast<wxStatusBar*>(status)->GetStatusText(field), buffer, length); }
wxsharp_handle wxsharp_toolbar_create(wxsharp_handle frame, long long token)
{
    auto* toolbar = static_cast<wxFrame*>(frame)->CreateToolBar(); BindCommon(toolbar, token); return toolbar;
}
void wxsharp_toolbar_add_tool(wxsharp_handle toolbar, int id, const char* label, const char* help, int kind)
{
    const wxItemKind itemKind = kind == 1 ? wxITEM_CHECK : kind == 2 ? wxITEM_RADIO : wxITEM_NORMAL;
    static_cast<wxToolBar*>(toolbar)->AddTool(id, Str(label), wxNullBitmap, Str(help), itemKind);
}
void wxsharp_toolbar_add_separator(wxsharp_handle toolbar) { static_cast<wxToolBar*>(toolbar)->AddSeparator(); }
void wxsharp_toolbar_realize(wxsharp_handle toolbar) { static_cast<wxToolBar*>(toolbar)->Realize(); }
void wxsharp_toolbar_enable(wxsharp_handle toolbar, int id, bool enable) { static_cast<wxToolBar*>(toolbar)->EnableTool(id, enable); }
void wxsharp_toolbar_toggle(wxsharp_handle toolbar, int id, bool toggle) { static_cast<wxToolBar*>(toolbar)->ToggleTool(id, toggle); }
void wxsharp_frame_set_accelerators(wxsharp_handle frame, const wxsharp_accelerator* entries, int count)
{
    std::vector<wxAcceleratorEntry> native; native.reserve(static_cast<size_t>(count));
    for (int i = 0; i < count; ++i)
        native.emplace_back(entries[i].modifiers, entries[i].key_code, entries[i].command_id);
    static_cast<wxFrame*>(frame)->SetAcceleratorTable(wxAcceleratorTable(count, native.data()));
}
