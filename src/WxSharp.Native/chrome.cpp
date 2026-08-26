// Frame chrome: menus, menu items, the menu bar, status and tool bars, accelerators, and ID allocation.
//
// Menu items cross the ABI as opaque wxMenuItem handles rather than as loose integer IDs, so the managed
// MenuItem can carry a label, help string, kind and state, and so submenus can be built and rearranged the
// way wxMenu itself allows.
#include "internal.h"
#include <wx/accel.h>
#include <wx/menu.h>
#include <wx/statusbr.h>
#include <wx/toolbar.h>

namespace
{
    inline wxMenu* Menu(wxsharp_handle h) { return static_cast<wxMenu*>(h); }
    inline wxMenuBar* Bar(wxsharp_handle h) { return static_cast<wxMenuBar*>(h); }
    inline wxMenuItem* Item(wxsharp_handle h) { return static_cast<wxMenuItem*>(h); }

    // 0 normal, 1 check, 2 radio - the managed MenuItemKind. Anything else is treated as normal.
    inline wxItemKind ItemKind(int kind)
    {
        return kind == 1 ? wxITEM_CHECK : kind == 2 ? wxITEM_RADIO : wxITEM_NORMAL;
    }

    inline int KindValue(wxItemKind kind)
    {
        switch (kind)
        {
            case wxITEM_CHECK: return 1;
            case wxITEM_RADIO: return 2;
            case wxITEM_SEPARATOR: return 3;
            default: return 0;
        }
    }
}

// ---- Menus ------------------------------------------------------------------------------------------

wxsharp_handle wxsharp_menu_create() { return new wxMenu(); }
void wxsharp_menu_destroy(wxsharp_handle menu) { delete Menu(menu); }

wxsharp_handle wxsharp_menu_append(wxsharp_handle menu, int id, const char* text, const char* help, int kind)
{
    return Menu(menu)->Append(id, Str(text), Str(help), ItemKind(kind));
}

wxsharp_handle wxsharp_menu_insert(wxsharp_handle menu, int position, int id, const char* text,
                                   const char* help, int kind)
{
    return Menu(menu)->Insert(static_cast<size_t>(position), id, Str(text), Str(help), ItemKind(kind));
}

wxsharp_handle wxsharp_menu_append_submenu(wxsharp_handle menu, int id, const char* text,
                                           wxsharp_handle submenu, const char* help)
{
    // The parent menu takes ownership of the submenu, as wxWidgets does.
    return Menu(menu)->Append(id, Str(text), Menu(submenu), Str(help));
}

wxsharp_handle wxsharp_menu_insert_submenu(wxsharp_handle menu, int position, int id, const char* text,
                                           wxsharp_handle submenu, const char* help)
{
    return Menu(menu)->Insert(static_cast<size_t>(position), id, Str(text), Menu(submenu), Str(help));
}

wxsharp_handle wxsharp_menu_append_separator(wxsharp_handle menu) { return Menu(menu)->AppendSeparator(); }

wxsharp_handle wxsharp_menu_insert_separator(wxsharp_handle menu, int position)
{
    return Menu(menu)->InsertSeparator(static_cast<size_t>(position));
}

int wxsharp_menu_count(wxsharp_handle menu) { return static_cast<int>(Menu(menu)->GetMenuItemCount()); }

wxsharp_handle wxsharp_menu_item_at(wxsharp_handle menu, int position)
{
    if (position < 0 || static_cast<size_t>(position) >= Menu(menu)->GetMenuItemCount())
        return nullptr;
    return Menu(menu)->FindItemByPosition(static_cast<size_t>(position));
}

wxsharp_handle wxsharp_menu_find_item(wxsharp_handle menu, int id) { return Menu(menu)->FindItem(id); }

bool wxsharp_menu_remove(wxsharp_handle menu, wxsharp_handle item)
{
    return Menu(menu)->Remove(Item(item)) != nullptr;
}

bool wxsharp_menu_delete(wxsharp_handle menu, wxsharp_handle item) { return Menu(menu)->Delete(Item(item)); }

void wxsharp_menu_enable(wxsharp_handle menu, int id, bool enable) { Menu(menu)->Enable(id, enable); }
void wxsharp_menu_check(wxsharp_handle menu, int id, bool check) { Menu(menu)->Check(id, check); }
bool wxsharp_menu_is_checked(wxsharp_handle menu, int id) { return Menu(menu)->IsChecked(id); }

int wxsharp_menu_get_title(wxsharp_handle menu, char* buffer, int buffer_length)
{
    return CopyToBuffer(Menu(menu)->GetTitle(), buffer, buffer_length);
}

void wxsharp_menu_set_title(wxsharp_handle menu, const char* title) { Menu(menu)->SetTitle(Str(title)); }

// ---- Menu items -------------------------------------------------------------------------------------

int wxsharp_menuitem_get_id(wxsharp_handle item) { return Item(item)->GetId(); }
int wxsharp_menuitem_get_kind(wxsharp_handle item) { return KindValue(Item(item)->GetKind()); }

int wxsharp_menuitem_get_label(wxsharp_handle item, char* buffer, int buffer_length)
{
    // The full item text, keeping the mnemonic and any "\tCtrl+O" accelerator suffix intact.
    return CopyToBuffer(Item(item)->GetItemLabel(), buffer, buffer_length);
}

void wxsharp_menuitem_set_label(wxsharp_handle item, const char* label)
{
    Item(item)->SetItemLabel(Str(label));
}

int wxsharp_menuitem_get_help(wxsharp_handle item, char* buffer, int buffer_length)
{
    return CopyToBuffer(Item(item)->GetHelp(), buffer, buffer_length);
}

void wxsharp_menuitem_set_help(wxsharp_handle item, const char* help) { Item(item)->SetHelp(Str(help)); }

bool wxsharp_menuitem_is_enabled(wxsharp_handle item) { return Item(item)->IsEnabled(); }
void wxsharp_menuitem_enable(wxsharp_handle item, bool enable) { Item(item)->Enable(enable); }
bool wxsharp_menuitem_is_checkable(wxsharp_handle item) { return Item(item)->IsCheckable(); }

bool wxsharp_menuitem_is_checked(wxsharp_handle item)
{
    return Item(item)->IsCheckable() && Item(item)->IsChecked();
}

void wxsharp_menuitem_check(wxsharp_handle item, bool check)
{
    if (Item(item)->IsCheckable())
        Item(item)->Check(check);
}

wxsharp_handle wxsharp_menuitem_get_submenu(wxsharp_handle item) { return Item(item)->GetSubMenu(); }

void wxsharp_menuitem_set_bitmap(wxsharp_handle item, wxsharp_handle bitmap)
{
    Item(item)->SetBitmap(bitmap ? *static_cast<wxBitmap*>(bitmap) : wxNullBitmap);
}

// ---- Menu bar ---------------------------------------------------------------------------------------

wxsharp_handle wxsharp_menubar_create() { return new wxMenuBar(); }
void wxsharp_menubar_destroy(wxsharp_handle menuBar) { delete Bar(menuBar); }

bool wxsharp_menubar_append(wxsharp_handle menuBar, wxsharp_handle menu, const char* title)
{
    return Bar(menuBar)->Append(Menu(menu), Str(title));
}

bool wxsharp_menubar_insert(wxsharp_handle menuBar, int position, wxsharp_handle menu, const char* title)
{
    return Bar(menuBar)->Insert(static_cast<size_t>(position), Menu(menu), Str(title));
}

wxsharp_handle wxsharp_menubar_remove(wxsharp_handle menuBar, int position)
{
    if (position < 0 || static_cast<size_t>(position) >= Bar(menuBar)->GetMenuCount())
        return nullptr;
    return Bar(menuBar)->Remove(static_cast<size_t>(position));
}

int wxsharp_menubar_count(wxsharp_handle menuBar) { return static_cast<int>(Bar(menuBar)->GetMenuCount()); }

wxsharp_handle wxsharp_menubar_menu_at(wxsharp_handle menuBar, int position)
{
    if (position < 0 || static_cast<size_t>(position) >= Bar(menuBar)->GetMenuCount())
        return nullptr;
    return Bar(menuBar)->GetMenu(static_cast<size_t>(position));
}

void wxsharp_menubar_enable_top(wxsharp_handle menuBar, int position, bool enable)
{
    Bar(menuBar)->EnableTop(static_cast<size_t>(position), enable);
}

int wxsharp_menubar_get_label_top(wxsharp_handle menuBar, int position, char* buffer, int buffer_length)
{
    return CopyToBuffer(Bar(menuBar)->GetMenuLabel(static_cast<size_t>(position)), buffer, buffer_length);
}

void wxsharp_menubar_set_label_top(wxsharp_handle menuBar, int position, const char* label)
{
    Bar(menuBar)->SetMenuLabel(static_cast<size_t>(position), Str(label));
}

wxsharp_handle wxsharp_menubar_find_item(wxsharp_handle menuBar, int id)
{
    return Bar(menuBar)->FindItem(id);
}

void wxsharp_frame_set_menubar(wxsharp_handle frame, wxsharp_handle menuBar)
{
    static_cast<wxFrame*>(frame)->SetMenuBar(Bar(menuBar));
}

void wxsharp_frame_update_menus(wxsharp_handle frame)
{
    static_cast<wxFrame*>(frame)->DoMenuUpdates();
}

bool wxsharp_window_popup_menu(wxsharp_handle window, wxsharp_handle menu, int x, int y)
{
    // (-1, -1) means "at the pointer", which is also what a keyboard-invoked context menu wants.
    const wxPoint position = (x < 0 && y < 0) ? wxDefaultPosition : wxPoint(x, y);
    return static_cast<wxWindow*>(window)->PopupMenu(Menu(menu), position);
}

// ---- Status and tool bars ---------------------------------------------------------------------------

wxsharp_handle wxsharp_statusbar_create(wxsharp_handle frame, int fields, long long token)
{
    auto* status = static_cast<wxFrame*>(frame)->CreateStatusBar(fields);
    TrackWindow(status, token);
    return status;
}
void wxsharp_statusbar_set_text(wxsharp_handle status, const char* text, int field) { static_cast<wxStatusBar*>(status)->SetStatusText(Str(text), field); }
int wxsharp_statusbar_get_text(wxsharp_handle status, int field, char* buffer, int length) { return CopyToBuffer(static_cast<wxStatusBar*>(status)->GetStatusText(field), buffer, length); }

wxsharp_handle wxsharp_toolbar_create(wxsharp_handle frame, long long token)
{
    auto* toolbar = static_cast<wxFrame*>(frame)->CreateToolBar();
    TrackWindow(toolbar, token);
    return toolbar;
}
void wxsharp_toolbar_add_tool(wxsharp_handle toolbar, int id, const char* label, const char* help, int kind)
{
    static_cast<wxToolBar*>(toolbar)->AddTool(id, Str(label), wxNullBitmap, Str(help), ItemKind(kind));
}
void wxsharp_toolbar_add_separator(wxsharp_handle toolbar) { static_cast<wxToolBar*>(toolbar)->AddSeparator(); }
void wxsharp_toolbar_realize(wxsharp_handle toolbar) { static_cast<wxToolBar*>(toolbar)->Realize(); }
void wxsharp_toolbar_enable(wxsharp_handle toolbar, int id, bool enable) { static_cast<wxToolBar*>(toolbar)->EnableTool(id, enable); }
void wxsharp_toolbar_toggle(wxsharp_handle toolbar, int id, bool toggle) { static_cast<wxToolBar*>(toolbar)->ToggleTool(id, toggle); }

// ---- Accelerators -----------------------------------------------------------------------------------

void wxsharp_window_set_accelerators(wxsharp_handle window, const wxsharp_accelerator* entries, int count)
{
    auto* target = static_cast<wxWindow*>(window);
    if (count <= 0 || !entries)
    {
        target->SetAcceleratorTable(wxNullAcceleratorTable);
        return;
    }
    std::vector<wxAcceleratorEntry> native;
    native.reserve(static_cast<size_t>(count));
    for (int i = 0; i < count; ++i)
        native.emplace_back(entries[i].modifiers, entries[i].key_code, entries[i].command_id);
    target->SetAcceleratorTable(wxAcceleratorTable(count, native.data()));
}

bool wxsharp_accelerator_parse(const char* text, int* modifiers, int* key_code)
{
    // wxAcceleratorEntry::FromString wants a full menu-item accelerator, so give it a label to hang off.
    wxAcceleratorEntry entry;
    if (!entry.FromString(wxString(wxT("\t")) + Str(text)))
        return false;
    if (modifiers) *modifiers = entry.GetFlags();
    if (key_code) *key_code = entry.GetKeyCode();
    return true;
}

int wxsharp_accelerator_format(int modifiers, int key_code, char* buffer, int buffer_length)
{
    const wxAcceleratorEntry entry(modifiers, key_code, wxID_ANY);
    return CopyToBuffer(entry.ToString(), buffer, buffer_length);
}

// ---- Identifiers ------------------------------------------------------------------------------------

int wxsharp_new_id() { return wxWindow::NewControlId(1); }
void wxsharp_release_id(int id) { wxWindow::UnreserveControlId(id, 1); }

// Stock IDs are looked up by ordinal so the managed StandardId values stay stable even though the wxID_*
// numbers themselves are wxWidgets' business. Keep this switch and the managed table in the same order.
int wxsharp_stock_id(int which)
{
    switch (which)
    {
        case 0:  return wxID_ANY;
        case 1:  return wxID_OK;
        case 2:  return wxID_CANCEL;
        case 3:  return wxID_YES;
        case 4:  return wxID_NO;
        case 5:  return wxID_APPLY;
        case 6:  return wxID_CLOSE;
        case 7:  return wxID_HELP;
        case 8:  return wxID_EXIT;
        case 9:  return wxID_NEW;
        case 10: return wxID_OPEN;
        case 11: return wxID_SAVE;
        case 12: return wxID_SAVEAS;
        case 13: return wxID_PREFERENCES;
        case 14: return wxID_ABOUT;
        case 15: return wxID_UNDO;
        case 16: return wxID_REDO;
        case 17: return wxID_CUT;
        case 18: return wxID_COPY;
        case 19: return wxID_PASTE;
        case 20: return wxID_DELETE;
        case 21: return wxID_SELECTALL;
        case 22: return wxID_FIND;
        case 23: return wxID_REPLACE;
        case 24: return wxID_ADD;
        case 25: return wxID_REMOVE;
        case 26: return wxID_EDIT;
        case 27: return wxID_REFRESH;
        case 28: return wxID_PROPERTIES;
        case 29: return wxID_PRINT;
        case 30: return wxID_STOP;
        case 31: return wxID_CLEAR;
        case 32: return wxID_UP;
        case 33: return wxID_DOWN;
        case 34: return wxID_BACKWARD;
        case 35: return wxID_FORWARD;
        case 36: return wxID_APPLY;
        case 37: return wxID_REVERT;
        case 38: return wxID_NONE;
        default: return wxID_ANY;
    }
}
