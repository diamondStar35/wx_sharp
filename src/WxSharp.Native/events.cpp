// The event table: the one place where a wrapper event ID is tied to a wxWidgets event type.
//
// Every bindable event is one row below. A row names the wxEventType, the function that copies that event
// class's payload into the flat wxsharp_event, and whether wx propagates it up the parent chain. Adding an
// event to the wrapper is a row here plus an EventType<T> on the managed side - nothing else.
//
// Binding is lazy. wxsharp_window_bind() connects a small sink object the first time the managed side
// subscribes to an event on a window, and wxsharp_window_unbind() disconnects it when the last subscriber
// goes away, so an event nothing is listening for never crosses the boundary. Propagation is left entirely
// to wxWidgets: an unhandled command event is skipped and travels up the real parent chain, which is what
// Phoenix does, rather than being re-dispatched from managed code.

#include "internal.h"

#include <wx/bookctrl.h>
#include <wx/checklst.h>
#include <wx/combobox.h>
#include <wx/dataview.h>
#include <wx/dateevt.h>
#include <wx/grid.h>
#include <wx/hyperlink.h>
#include <wx/listctrl.h>
#include <wx/notebook.h>
#include <wx/spinctrl.h>
#include <wx/splitter.h>
#include <wx/srchctrl.h>
#include <wx/tglbtn.h>
#include <wx/toolbar.h>
#include <wx/treectrl.h>

#include <string>
#include <unordered_map>

namespace
{
    // The UTF-8 buffer wxsharp_event::text points at. Reused per event: the managed handler runs
    // synchronously inside the callback and copies what it needs before returning.
    std::string& TextBuffer()
    {
        static std::string buffer;
        return buffer;
    }

    void SetText(wxsharp_event& data, const wxString& value)
    {
        std::string& buffer = TextBuffer();
        const wxScopedCharBuffer utf8 = value.utf8_str();
        buffer.assign(utf8.data(), utf8.length());
        data.text = buffer.c_str();
        data.text_length = static_cast<int>(buffer.size());
    }

    // ---- Payload fillers --------------------------------------------------------------------------------
    // Each takes the wxEvent its row is bound to, so the downcast is always valid.

    void FillNone(wxEvent&, wxsharp_event&) {}

    void FillClose(wxEvent& e, wxsharp_event& data)
    {
        data.can_veto = static_cast<wxCloseEvent&>(e).CanVeto() ? 1 : 0;
    }

    void FillShow(wxEvent& e, wxsharp_event& data)
    {
        data.active = static_cast<wxShowEvent&>(e).IsShown() ? 1 : 0;
    }

    void FillActivate(wxEvent& e, wxsharp_event& data)
    {
        data.active = static_cast<wxActivateEvent&>(e).GetActive() ? 1 : 0;
    }

    void FillIconize(wxEvent& e, wxsharp_event& data)
    {
        data.active = static_cast<wxIconizeEvent&>(e).IsIconized() ? 1 : 0;
    }

    void FillSize(wxEvent& e, wxsharp_event& data)
    {
        const wxSize size = static_cast<wxSizeEvent&>(e).GetSize();
        data.width = size.x;
        data.height = size.y;
    }

    void FillMove(wxEvent& e, wxsharp_event& data)
    {
        const wxPoint position = static_cast<wxMoveEvent&>(e).GetPosition();
        data.x = position.x;
        data.y = position.y;
    }

    void FillContextMenu(wxEvent& e, wxsharp_event& data)
    {
        // wx reports screen coordinates here, and (-1, -1) when the menu was requested from the keyboard.
        const wxPoint position = static_cast<wxContextMenuEvent&>(e).GetPosition();
        data.x = position.x;
        data.y = position.y;
    }

    void FillMouse(wxEvent& e, wxsharp_event& data)
    {
        auto& mouse = static_cast<wxMouseEvent&>(e);
        data.x = mouse.GetX();
        data.y = mouse.GetY();
        data.modifiers = Mods(mouse);
        data.wheel_delta = mouse.GetWheelRotation();
        data.int_value = mouse.GetWheelDelta();
        data.mouse_button = mouse.LeftIsDown() || mouse.GetButton() == wxMOUSE_BTN_LEFT ? 1
                          : mouse.RightIsDown() || mouse.GetButton() == wxMOUSE_BTN_RIGHT ? 2
                          : mouse.MiddleIsDown() || mouse.GetButton() == wxMOUSE_BTN_MIDDLE ? 3
                          : 0;
    }

    void FillKey(wxEvent& e, wxsharp_event& data)
    {
        auto& key = static_cast<wxKeyEvent&>(e);
        data.key_code = key.GetKeyCode();
        data.modifiers = Mods(key);
        data.uint_value = static_cast<unsigned int>(key.GetUnicodeKey());
        data.int_value = static_cast<int>(key.GetRawKeyCode());
        data.x = key.GetX();
        data.y = key.GetY();
    }

    void FillCommand(wxEvent& e, wxsharp_event& data)
    {
        auto& command = static_cast<wxCommandEvent&>(e);
        data.int_value = command.GetInt();
        data.selection = command.GetInt(); // list-like controls report the index here
        data.item = command.GetInt();
        SetText(data, command.GetString());
    }

    void FillSpin(wxEvent& e, wxsharp_event& data)
    {
        auto& spin = static_cast<wxSpinEvent&>(e);
        data.int_value = spin.GetPosition();
        data.double_value = spin.GetPosition();
    }

    void FillSpinDouble(wxEvent& e, wxsharp_event& data)
    {
        auto& spin = static_cast<wxSpinDoubleEvent&>(e);
        data.double_value = spin.GetValue();
        data.int_value = static_cast<int>(spin.GetValue());
    }

    void FillScroll(wxEvent& e, wxsharp_event& data)
    {
        auto& scroll = static_cast<wxScrollEvent&>(e);
        data.int_value = scroll.GetPosition();
        data.selection = scroll.GetPosition();
    }

    void FillHyperlink(wxEvent& e, wxsharp_event& data)
    {
        SetText(data, static_cast<wxHyperlinkEvent&>(e).GetURL());
    }

    void FillDate(wxEvent& e, wxsharp_event& data)
    {
        const wxDateTime date = static_cast<wxDateEvent&>(e).GetDate();
        // Milliseconds since the Unix epoch; the managed side rebuilds a DateTime from it.
        data.item = date.IsValid() ? date.GetValue().GetValue() : 0;
        data.active = date.IsValid() ? 1 : 0;
    }

    void FillBook(wxEvent& e, wxsharp_event& data)
    {
        auto& book = static_cast<wxBookCtrlEvent&>(e);
        data.selection = book.GetSelection();
        data.old_selection = book.GetOldSelection();
        data.int_value = book.GetSelection();
    }

    void FillList(wxEvent& e, wxsharp_event& data)
    {
        auto& list = static_cast<wxListEvent&>(e);
        data.item = list.GetIndex();
        data.selection = static_cast<int>(list.GetIndex());
        data.column = list.GetColumn();
        data.key_code = list.GetKeyCode();
        data.int_value = list.GetKeyCode();
        SetText(data, list.GetLabel());
    }

    void FillTree(wxEvent& e, wxsharp_event& data)
    {
        auto& tree = static_cast<wxTreeEvent&>(e);
        data.item = TreeValue(tree.GetItem());
        data.old_item = TreeValue(tree.GetOldItem());
        data.key_code = tree.GetKeyCode();
        data.int_value = tree.GetKeyCode();
        SetText(data, tree.GetLabel());
    }

    void FillDataView(wxEvent& e, wxsharp_event& data)
    {
        auto& view = static_cast<wxDataViewEvent&>(e);
        data.item = DataViewValue(view.GetItem());
        data.column = view.GetColumn();
    }

    void FillSplitter(wxEvent& e, wxsharp_event& data)
    {
        auto& splitter = static_cast<wxSplitterEvent&>(e);
        data.int_value = splitter.GetSashPosition();
        data.selection = splitter.GetSashPosition();
    }

    // The update-UI event currently being dispatched, so the managed handler can answer it. Dispatch is
    // synchronous and single-threaded, so one pointer is enough.
    wxUpdateUIEvent* g_update_ui = nullptr;

    // The paths of the drop-files event currently being dispatched.
    wxArrayString& DroppedFiles()
    {
        static wxArrayString paths;
        return paths;
    }

    void FillUpdateUI(wxEvent& e, wxsharp_event& data)
    {
        auto& update = static_cast<wxUpdateUIEvent&>(e);
        // What wx currently believes, so a handler can answer conditionally.
        data.active = update.GetEnabled() ? 1 : 0;
        data.int_value = update.GetChecked() ? 1 : 0;
        SetText(data, update.GetText());
    }

    void FillIdle(wxEvent& e, wxsharp_event& data)
    {
        data.active = static_cast<wxIdleEvent&>(e).MoreRequested() ? 1 : 0;
    }

    void FillMenu(wxEvent& e, wxsharp_event& data)
    {
        auto& menu = static_cast<wxMenuEvent&>(e);
        data.int_value = menu.GetMenuId();
        data.selection = menu.GetMenuId();
        data.item = reinterpret_cast<intptr_t>(menu.GetMenu());
        data.active = menu.IsPopup() ? 1 : 0;
    }

    void FillChildFocus(wxEvent& e, wxsharp_event& data)
    {
        data.item = reinterpret_cast<intptr_t>(static_cast<wxChildFocusEvent&>(e).GetWindow());
    }

    void FillNavigationKey(wxEvent& e, wxsharp_event& data)
    {
        auto& nav = static_cast<wxNavigationKeyEvent&>(e);
        data.active = nav.GetDirection() ? 1 : 0;      // true means forward
        data.int_value = nav.IsWindowChange() ? 1 : 0; // Ctrl+Tab rather than Tab
        data.item = reinterpret_cast<intptr_t>(nav.GetCurrentFocus());
    }

    void FillDropFiles(wxEvent& e, wxsharp_event& data)
    {
        auto& drop = static_cast<wxDropFilesEvent&>(e);
        wxArrayString& paths = DroppedFiles();
        paths.Clear();
        const int count = drop.GetNumberOfFiles();
        wxString* files = drop.GetFiles();
        for (int i = 0; i < count && files; ++i)
            paths.Add(files[i]);
        data.item = count;
        data.int_value = count;
        const wxPoint position = drop.GetPosition();
        data.x = position.x;
        data.y = position.y;
    }

    void FillHelp(wxEvent& e, wxsharp_event& data)
    {
        const wxPoint position = static_cast<wxHelpEvent&>(e).GetPosition();
        data.x = position.x;
        data.y = position.y;
    }

    void FillTextUrl(wxEvent& e, wxsharp_event& data)
    {
        auto& url = static_cast<wxTextUrlEvent&>(e);
        data.selection = static_cast<int>(url.GetURLStart());
        data.old_selection = static_cast<int>(url.GetURLEnd());
    }

    void FillToolTip(wxEvent& e, wxsharp_event& data)
    {
        auto& tree = static_cast<wxTreeEvent&>(e);
        data.item = TreeValue(tree.GetItem());
        SetText(data, tree.GetToolTip());
    }

    void FillGrid(wxEvent& e, wxsharp_event& data)
    {
        auto& grid = static_cast<wxGridEvent&>(e);
        data.item = grid.GetRow();
        data.column = grid.GetCol();
        data.selection = grid.GetRow();
    }

    // ---- The table --------------------------------------------------------------------------------------

    enum RowFlags
    {
        ROW_NONE = 0,
        ROW_PROPAGATES = 1, // a command event: wx sends it up the parent chain once skipped
        ROW_VETOABLE = 2,   // CANCEL from the managed handler calls Veto()
    };

    struct EventRow
    {
        int id;
        wxEventType type;
        void (*fill)(wxEvent&, wxsharp_event&);
        unsigned int flags;
    };

    // Built on first use rather than at static-initialisation time: the wxEVT_* values are objects with
    // their own static initialisers, and their order relative to ours is not defined across translation
    // units.
    const std::unordered_map<int, EventRow>& Rows()
    {
        static const std::unordered_map<int, EventRow> rows = []
        {
            std::unordered_map<int, EventRow> map;
            auto add = [&map](int id, wxEventType type, void (*fill)(wxEvent&, wxsharp_event&),
                              unsigned int flags = ROW_NONE)
            {
                map.emplace(id, EventRow{ id, type, fill, flags });
            };

            // Window lifecycle and geometry. DESTROY and PAINT are deliberately absent: they are reported
            // by the owning window itself (see TrackWindow) and by the canvas, which owns the device
            // context for the duration of the paint.
            add(WXSHARP_EV_CLOSE, wxEVT_CLOSE_WINDOW, FillClose, ROW_VETOABLE);
            add(WXSHARP_EV_SHOW, wxEVT_SHOW, FillShow);
            add(WXSHARP_EV_ACTIVATE, wxEVT_ACTIVATE, FillActivate);
            add(WXSHARP_EV_SIZE, wxEVT_SIZE, FillSize);
            add(WXSHARP_EV_MOVE, wxEVT_MOVE, FillMove);
            add(WXSHARP_EV_MAXIMIZE, wxEVT_MAXIMIZE, FillNone);
            add(WXSHARP_EV_ICONIZE, wxEVT_ICONIZE, FillIconize);
            add(WXSHARP_EV_SET_FOCUS, wxEVT_SET_FOCUS, FillNone);
            add(WXSHARP_EV_KILL_FOCUS, wxEVT_KILL_FOCUS, FillNone);
            add(WXSHARP_EV_CONTEXT_MENU, wxEVT_CONTEXT_MENU, FillContextMenu, ROW_PROPAGATES);
            add(WXSHARP_EV_UPDATE_UI, wxEVT_UPDATE_UI, FillUpdateUI, ROW_PROPAGATES);
            add(WXSHARP_EV_IDLE, wxEVT_IDLE, FillIdle);
            add(WXSHARP_EV_CHILD_FOCUS, wxEVT_CHILD_FOCUS, FillChildFocus, ROW_PROPAGATES);
            add(WXSHARP_EV_NAVIGATION_KEY, wxEVT_NAVIGATION_KEY, FillNavigationKey);
            add(WXSHARP_EV_MOUSE_CAPTURE_LOST, wxEVT_MOUSE_CAPTURE_LOST, FillNone);
            add(WXSHARP_EV_MOUSE_CAPTURE_CHANGED, wxEVT_MOUSE_CAPTURE_CHANGED, FillNone);
            add(WXSHARP_EV_DROP_FILES, wxEVT_DROP_FILES, FillDropFiles);
            add(WXSHARP_EV_HOTKEY, wxEVT_HOTKEY, FillKey);
            add(WXSHARP_EV_HELP, wxEVT_HELP, FillHelp, ROW_PROPAGATES);
            add(WXSHARP_EV_MENU_OPEN, wxEVT_MENU_OPEN, FillMenu);
            add(WXSHARP_EV_MENU_CLOSE, wxEVT_MENU_CLOSE, FillMenu);
            add(WXSHARP_EV_MENU_HIGHLIGHT, wxEVT_MENU_HIGHLIGHT, FillMenu);

            // Mouse.
            add(WXSHARP_EV_LEFT_DOWN, wxEVT_LEFT_DOWN, FillMouse);
            add(WXSHARP_EV_LEFT_UP, wxEVT_LEFT_UP, FillMouse);
            add(WXSHARP_EV_LEFT_DCLICK, wxEVT_LEFT_DCLICK, FillMouse);
            add(WXSHARP_EV_RIGHT_DOWN, wxEVT_RIGHT_DOWN, FillMouse);
            add(WXSHARP_EV_RIGHT_UP, wxEVT_RIGHT_UP, FillMouse);
            add(WXSHARP_EV_RIGHT_DCLICK, wxEVT_RIGHT_DCLICK, FillMouse);
            add(WXSHARP_EV_MIDDLE_DOWN, wxEVT_MIDDLE_DOWN, FillMouse);
            add(WXSHARP_EV_MIDDLE_UP, wxEVT_MIDDLE_UP, FillMouse);
            add(WXSHARP_EV_MIDDLE_DCLICK, wxEVT_MIDDLE_DCLICK, FillMouse);
            add(WXSHARP_EV_MOTION, wxEVT_MOTION, FillMouse);
            add(WXSHARP_EV_ENTER_WINDOW, wxEVT_ENTER_WINDOW, FillMouse);
            add(WXSHARP_EV_LEAVE_WINDOW, wxEVT_LEAVE_WINDOW, FillMouse);
            add(WXSHARP_EV_MOUSEWHEEL, wxEVT_MOUSEWHEEL, FillMouse);

            // Keyboard.
            add(WXSHARP_EV_CHAR_HOOK, wxEVT_CHAR_HOOK, FillKey);
            add(WXSHARP_EV_KEY_DOWN, wxEVT_KEY_DOWN, FillKey);
            add(WXSHARP_EV_KEY_UP, wxEVT_KEY_UP, FillKey);
            add(WXSHARP_EV_CHAR, wxEVT_CHAR, FillKey);

            // Control commands. All of these propagate: binding them on a parent frame works, as it does
            // in Phoenix.
            add(WXSHARP_EV_BUTTON, wxEVT_BUTTON, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_CHECKBOX, wxEVT_CHECKBOX, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_CHOICE, wxEVT_CHOICE, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_LISTBOX, wxEVT_LISTBOX, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_LISTBOX_DCLICK, wxEVT_LISTBOX_DCLICK, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_TEXT, wxEVT_TEXT, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_TEXT_ENTER, wxEVT_TEXT_ENTER, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_MENU, wxEVT_MENU, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_SLIDER, wxEVT_SLIDER, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_RADIOBUTTON, wxEVT_RADIOBUTTON, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_RADIOBOX, wxEVT_RADIOBOX, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_COMBOBOX, wxEVT_COMBOBOX, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_TOGGLEBUTTON, wxEVT_TOGGLEBUTTON, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_CHECKLISTBOX, wxEVT_CHECKLISTBOX, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_SPINCTRL, wxEVT_SPINCTRL, FillSpin, ROW_PROPAGATES);
            add(WXSHARP_EV_SPINCTRLDOUBLE, wxEVT_SPINCTRLDOUBLE, FillSpinDouble, ROW_PROPAGATES);
            add(WXSHARP_EV_SCROLL_THUMBTRACK, wxEVT_SCROLL_THUMBTRACK, FillScroll);
            add(WXSHARP_EV_SCROLL_CHANGED, wxEVT_SCROLL_CHANGED, FillScroll);
            add(WXSHARP_EV_HYPERLINK, wxEVT_HYPERLINK, FillHyperlink, ROW_PROPAGATES);
            add(WXSHARP_EV_SEARCH, wxEVT_SEARCHCTRL_SEARCH_BTN, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_SEARCH_CANCEL, wxEVT_SEARCHCTRL_CANCEL_BTN, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_DATE_CHANGED, wxEVT_DATE_CHANGED, FillDate, ROW_PROPAGATES);
            add(WXSHARP_EV_TIME_CHANGED, wxEVT_TIME_CHANGED, FillDate, ROW_PROPAGATES);
            add(WXSHARP_EV_COMBOBOX_DROPDOWN, wxEVT_COMBOBOX_DROPDOWN, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_COMBOBOX_CLOSEUP, wxEVT_COMBOBOX_CLOSEUP, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_SPIN, wxEVT_SPIN, FillSpin, ROW_PROPAGATES);
            add(WXSHARP_EV_SPIN_UP, wxEVT_SPIN_UP, FillSpin, ROW_PROPAGATES);
            add(WXSHARP_EV_SPIN_DOWN, wxEVT_SPIN_DOWN, FillSpin, ROW_PROPAGATES);
            add(WXSHARP_EV_SCROLLBAR, wxEVT_SCROLLBAR, FillScroll, ROW_PROPAGATES);
            add(WXSHARP_EV_SCROLL_TOP, wxEVT_SCROLL_TOP, FillScroll);
            add(WXSHARP_EV_SCROLL_BOTTOM, wxEVT_SCROLL_BOTTOM, FillScroll);
            add(WXSHARP_EV_SCROLL_LINEUP, wxEVT_SCROLL_LINEUP, FillScroll);
            add(WXSHARP_EV_SCROLL_LINEDOWN, wxEVT_SCROLL_LINEDOWN, FillScroll);
            add(WXSHARP_EV_SCROLL_PAGEUP, wxEVT_SCROLL_PAGEUP, FillScroll);
            add(WXSHARP_EV_SCROLL_PAGEDOWN, wxEVT_SCROLL_PAGEDOWN, FillScroll);
            add(WXSHARP_EV_SCROLL_THUMBRELEASE, wxEVT_SCROLL_THUMBRELEASE, FillScroll);
            add(WXSHARP_EV_TEXT_MAXLEN, wxEVT_TEXT_MAXLEN, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_TEXT_URL, wxEVT_TEXT_URL, FillTextUrl, ROW_PROPAGATES);
            add(WXSHARP_EV_TEXT_COPY, wxEVT_TEXT_COPY, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_TEXT_CUT, wxEVT_TEXT_CUT, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_TEXT_PASTE, wxEVT_TEXT_PASTE, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_TOOL_ENTER, wxEVT_TOOL_ENTER, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_TOOL_RCLICKED, wxEVT_TOOL_RCLICKED, FillCommand, ROW_PROPAGATES);
            add(WXSHARP_EV_TOOL_DROPDOWN, wxEVT_TOOL_DROPDOWN, FillCommand, ROW_PROPAGATES);

            // Book controls. The "changing" variants can be vetoed to refuse the page change.
            add(WXSHARP_EV_NOTEBOOK_PAGE_CHANGED, wxEVT_NOTEBOOK_PAGE_CHANGED, FillBook, ROW_PROPAGATES);
            add(WXSHARP_EV_NOTEBOOK_PAGE_CHANGING, wxEVT_NOTEBOOK_PAGE_CHANGING, FillBook,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_BOOKCTRL_PAGE_CHANGED, wxEVT_BOOKCTRL_PAGE_CHANGED, FillBook, ROW_PROPAGATES);
            add(WXSHARP_EV_BOOKCTRL_PAGE_CHANGING, wxEVT_BOOKCTRL_PAGE_CHANGING, FillBook,
                ROW_PROPAGATES | ROW_VETOABLE);

            // wxListCtrl.
            add(WXSHARP_EV_LIST_ITEM_SELECTED, wxEVT_LIST_ITEM_SELECTED, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_ITEM_DESELECTED, wxEVT_LIST_ITEM_DESELECTED, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_ITEM_ACTIVATED, wxEVT_LIST_ITEM_ACTIVATED, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_ITEM_FOCUSED, wxEVT_LIST_ITEM_FOCUSED, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_ITEM_RIGHT_CLICK, wxEVT_LIST_ITEM_RIGHT_CLICK, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_COL_CLICK, wxEVT_LIST_COL_CLICK, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_KEY_DOWN, wxEVT_LIST_KEY_DOWN, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_BEGIN_LABEL_EDIT, wxEVT_LIST_BEGIN_LABEL_EDIT, FillList,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_LIST_END_LABEL_EDIT, wxEVT_LIST_END_LABEL_EDIT, FillList,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_LIST_BEGIN_DRAG, wxEVT_LIST_BEGIN_DRAG, FillList,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_LIST_BEGIN_RIGHT_DRAG, wxEVT_LIST_BEGIN_RDRAG, FillList,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_LIST_ITEM_MIDDLE_CLICK, wxEVT_LIST_ITEM_MIDDLE_CLICK, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_ITEM_CHECKED, wxEVT_LIST_ITEM_CHECKED, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_ITEM_UNCHECKED, wxEVT_LIST_ITEM_UNCHECKED, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_COL_RIGHT_CLICK, wxEVT_LIST_COL_RIGHT_CLICK, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_COL_BEGIN_DRAG, wxEVT_LIST_COL_BEGIN_DRAG, FillList,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_LIST_COL_END_DRAG, wxEVT_LIST_COL_END_DRAG, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_DELETE_ITEM, wxEVT_LIST_DELETE_ITEM, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_DELETE_ALL_ITEMS, wxEVT_LIST_DELETE_ALL_ITEMS, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_INSERT_ITEM, wxEVT_LIST_INSERT_ITEM, FillList, ROW_PROPAGATES);
            add(WXSHARP_EV_LIST_CACHE_HINT, wxEVT_LIST_CACHE_HINT, FillList, ROW_PROPAGATES);

            // wxTreeCtrl.
            add(WXSHARP_EV_TREE_SEL_CHANGED, wxEVT_TREE_SEL_CHANGED, FillTree, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_SEL_CHANGING, wxEVT_TREE_SEL_CHANGING, FillTree,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_TREE_ITEM_ACTIVATED, wxEVT_TREE_ITEM_ACTIVATED, FillTree, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_ITEM_EXPANDED, wxEVT_TREE_ITEM_EXPANDED, FillTree, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_ITEM_EXPANDING, wxEVT_TREE_ITEM_EXPANDING, FillTree,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_TREE_ITEM_COLLAPSED, wxEVT_TREE_ITEM_COLLAPSED, FillTree, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_ITEM_COLLAPSING, wxEVT_TREE_ITEM_COLLAPSING, FillTree,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_TREE_ITEM_RIGHT_CLICK, wxEVT_TREE_ITEM_RIGHT_CLICK, FillTree, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_KEY_DOWN, wxEVT_TREE_KEY_DOWN, FillTree, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_BEGIN_LABEL_EDIT, wxEVT_TREE_BEGIN_LABEL_EDIT, FillTree,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_TREE_END_LABEL_EDIT, wxEVT_TREE_END_LABEL_EDIT, FillTree,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_TREE_ITEM_MENU, wxEVT_TREE_ITEM_MENU, FillTree, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_BEGIN_DRAG, wxEVT_TREE_BEGIN_DRAG, FillTree, ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_TREE_END_DRAG, wxEVT_TREE_END_DRAG, FillTree, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_ITEM_MIDDLE_CLICK, wxEVT_TREE_ITEM_MIDDLE_CLICK, FillTree, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_DELETE_ITEM, wxEVT_TREE_DELETE_ITEM, FillTree, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_ITEM_GETTOOLTIP, wxEVT_TREE_ITEM_GETTOOLTIP, FillToolTip, ROW_PROPAGATES);
            add(WXSHARP_EV_TREE_STATE_IMAGE_CLICK, wxEVT_TREE_STATE_IMAGE_CLICK, FillTree, ROW_PROPAGATES);

            // wxDataViewCtrl.
            add(WXSHARP_EV_DATAVIEW_SELECTION_CHANGED, wxEVT_DATAVIEW_SELECTION_CHANGED, FillDataView,
                ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_ITEM_ACTIVATED, wxEVT_DATAVIEW_ITEM_ACTIVATED, FillDataView,
                ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_ITEM_CONTEXT_MENU, wxEVT_DATAVIEW_ITEM_CONTEXT_MENU, FillDataView,
                ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_ITEM_EXPANDED, wxEVT_DATAVIEW_ITEM_EXPANDED, FillDataView, ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_ITEM_EXPANDING, wxEVT_DATAVIEW_ITEM_EXPANDING, FillDataView,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_DATAVIEW_ITEM_COLLAPSED, wxEVT_DATAVIEW_ITEM_COLLAPSED, FillDataView, ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_ITEM_COLLAPSING, wxEVT_DATAVIEW_ITEM_COLLAPSING, FillDataView,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_DATAVIEW_ITEM_EDITING_STARTED, wxEVT_DATAVIEW_ITEM_EDITING_STARTED, FillDataView,
                ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_ITEM_EDITING_DONE, wxEVT_DATAVIEW_ITEM_EDITING_DONE, FillDataView,
                ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_ITEM_VALUE_CHANGED, wxEVT_DATAVIEW_ITEM_VALUE_CHANGED, FillDataView,
                ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_COLUMN_HEADER_CLICK, wxEVT_DATAVIEW_COLUMN_HEADER_CLICK, FillDataView,
                ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_COLUMN_HEADER_RIGHT_CLICK, wxEVT_DATAVIEW_COLUMN_HEADER_RIGHT_CLICK,
                FillDataView, ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_COLUMN_SORTED, wxEVT_DATAVIEW_COLUMN_SORTED, FillDataView, ROW_PROPAGATES);
            add(WXSHARP_EV_DATAVIEW_COLUMN_REORDERED, wxEVT_DATAVIEW_COLUMN_REORDERED, FillDataView,
                ROW_PROPAGATES);

            // wxSplitterWindow.
            add(WXSHARP_EV_SPLITTER_SASH_POS_CHANGED, wxEVT_SPLITTER_SASH_POS_CHANGED, FillSplitter,
                ROW_PROPAGATES);
            add(WXSHARP_EV_SPLITTER_DCLICK, wxEVT_SPLITTER_DOUBLECLICKED, FillSplitter,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_SPLITTER_SASH_POS_CHANGING, wxEVT_SPLITTER_SASH_POS_CHANGING, FillSplitter,
                ROW_PROPAGATES | ROW_VETOABLE);
            add(WXSHARP_EV_SPLITTER_UNSPLIT, wxEVT_SPLITTER_UNSPLIT, FillSplitter, ROW_PROPAGATES);

            // wxGrid.
            add(WXSHARP_EV_GRID_CELL_CHANGED, wxEVT_GRID_CELL_CHANGED, FillGrid, ROW_PROPAGATES);
            add(WXSHARP_EV_GRID_SELECT_CELL, wxEVT_GRID_SELECT_CELL, FillGrid, ROW_PROPAGATES);

            return map;
        }();
        return rows;
    }

    const EventRow* FindRow(int eventId)
    {
        const auto& rows = Rows();
        const auto found = rows.find(eventId);
        return found == rows.end() ? nullptr : &found->second;
    }

    // ---- The sink ---------------------------------------------------------------------------------------
    // One per (window, event id). Connected with the legacy dynamic API because the row's wxEventType is a
    // runtime value, not a compile-time tag, which the templated Bind() overloads require.

    class EventSink : public wxEvtHandler
    {
    public:
        EventSink(long long token, const EventRow* row) : m_token(token), m_row(row) {}

        void OnEvent(wxEvent& e)
        {
            // Everything this handler needs is read up front: the managed callback may unbind this event,
            // which destroys the sink before the call returns.
            const EventRow* row = m_row;
            const long long token = m_token;

            wxsharp_event data = {};
            data.size = sizeof(data);
            data.version = WXSHARP_EVENT_VERSION;
            data.token = token;
            data.kind = row->id;
            data.id = e.GetId();
            row->fill(e, data);

            // An update-UI event is answered rather than merely observed, so the handler needs to reach the
            // event itself. Published for the duration of the callback and withdrawn straight after.
            wxUpdateUIEvent* const previousUpdateUI = g_update_ui;
            if (row->id == WXSHARP_EV_UPDATE_UI)
                g_update_ui = static_cast<wxUpdateUIEvent*>(&e);

            // No handler at all is the same as every handler skipping.
            const unsigned int result = g_event_cb ? g_event_cb(&data) : WXSHARP_EVENT_SKIP;

            if (row->id == WXSHARP_EV_UPDATE_UI)
                g_update_ui = previousUpdateUI;
            else if (row->id == WXSHARP_EV_DROP_FILES)
                DroppedFiles().Clear();

            if ((row->flags & ROW_VETOABLE) && (result & WXSHARP_EVENT_VETO))
            {
                // Every vetoable event we bind derives from wxNotifyEvent apart from wxCloseEvent, which
                // only allows a veto when the close can be refused at all.
                if (row->id == WXSHARP_EV_CLOSE)
                {
                    auto& close = static_cast<wxCloseEvent&>(e);
                    if (close.CanVeto())
                    {
                        close.Veto();
                        return;
                    }
                }
                else
                {
                    static_cast<wxNotifyEvent&>(e).Veto();
                    return;
                }
            }

            // wxWidgets' default is that a handled event stops here; Skip() is what asks for the normal
            // processing - base-class behaviour, command propagation to the parent - to carry on.
            if (result & WXSHARP_EVENT_SKIP)
                e.Skip();
        }

    private:
        long long m_token;
        const EventRow* m_row;
    };

    // Live bindings, so unbind and window teardown can find and release them.
    std::unordered_map<wxWindow*, std::unordered_map<int, EventSink*>>& Bindings()
    {
        static std::unordered_map<wxWindow*, std::unordered_map<int, EventSink*>> bindings;
        return bindings;
    }

    // wxEVT_TEXT_ENTER may only be bound on a control that was created with wxTE_PROCESS_ENTER; binding it
    // on any other control trips a wx assert rather than simply never firing.
    bool CanBind(wxWindow* window, int eventId)
    {
        if (eventId != WXSHARP_EV_TEXT_ENTER)
            return true;
        return (window->GetWindowStyleFlag() & wxTE_PROCESS_ENTER) != 0;
    }
}

void WxSharpReleaseBindings(wxWindow* window)
{
    auto& bindings = Bindings();
    const auto found = bindings.find(window);
    if (found == bindings.end())
        return;
    for (const auto& entry : found->second)
    {
        const EventRow* row = FindRow(entry.first);
        if (row)
            window->Disconnect(wxID_ANY, wxID_ANY, row->type,
                               wxEventHandler(EventSink::OnEvent), nullptr, entry.second);
        delete entry.second;
    }
    bindings.erase(found);
}

bool wxsharp_window_bind(wxsharp_handle window, int event_id, long long token)
{
    auto* target = static_cast<wxWindow*>(window);
    if (!target)
        return false;
    const EventRow* row = FindRow(event_id);
    if (!row || !CanBind(target, event_id))
        return false;

    auto& forWindow = Bindings()[target];
    if (forWindow.find(event_id) != forWindow.end())
        return true; // already hooked; the managed side refcounts its own subscribers

    auto* sink = new EventSink(token, row);
    target->Connect(wxID_ANY, wxID_ANY, row->type, wxEventHandler(EventSink::OnEvent), nullptr, sink);
    forWindow.emplace(event_id, sink);
    return true;
}

bool wxsharp_window_unbind(wxsharp_handle window, int event_id)
{
    auto* target = static_cast<wxWindow*>(window);
    if (!target)
        return false;
    auto& bindings = Bindings();
    const auto forWindow = bindings.find(target);
    if (forWindow == bindings.end())
        return false;
    const auto entry = forWindow->second.find(event_id);
    if (entry == forWindow->second.end())
        return false;

    const EventRow* row = FindRow(event_id);
    if (row)
        target->Disconnect(wxID_ANY, wxID_ANY, row->type,
                           wxEventHandler(EventSink::OnEvent), nullptr, entry->second);
    delete entry->second;
    forWindow->second.erase(entry);
    if (forWindow->second.empty())
        bindings.erase(forWindow);
    return true;
}

void wxsharp_window_unbind_all(wxsharp_handle window)
{
    WxSharpReleaseBindings(static_cast<wxWindow*>(window));
}

bool wxsharp_updateui_enable(bool enable)
{
    if (!g_update_ui) return false;
    g_update_ui->Enable(enable);
    return true;
}

bool wxsharp_updateui_check(bool check)
{
    if (!g_update_ui) return false;
    g_update_ui->Check(check);
    return true;
}

bool wxsharp_updateui_show(bool show)
{
    if (!g_update_ui) return false;
    g_update_ui->Show(show);
    return true;
}

bool wxsharp_updateui_set_text(const char* text)
{
    if (!g_update_ui) return false;
    g_update_ui->SetText(Str(text));
    return true;
}

void wxsharp_updateui_set_interval(int milliseconds)
{
    wxUpdateUIEvent::SetUpdateInterval(milliseconds);
}

void wxsharp_updateui_set_process_all(bool process_all)
{
    wxUpdateUIEvent::SetMode(process_all ? wxUPDATE_UI_PROCESS_ALL : wxUPDATE_UI_PROCESS_SPECIFIED);
}

int wxsharp_dropfiles_count()
{
    return static_cast<int>(DroppedFiles().GetCount());
}

int wxsharp_dropfiles_path(int index, char* buffer, int buffer_length)
{
    const wxArrayString& paths = DroppedFiles();
    if (index < 0 || static_cast<size_t>(index) >= paths.GetCount())
        return 0;
    return CopyToBuffer(paths[static_cast<size_t>(index)], buffer, buffer_length);
}

void wxsharp_window_accept_dropped_files(wxsharp_handle window, bool accept)
{
    static_cast<wxWindow*>(window)->DragAcceptFiles(accept);
}

void wxsharp_window_capture_mouse(wxsharp_handle window) { static_cast<wxWindow*>(window)->CaptureMouse(); }
void wxsharp_window_release_mouse(wxsharp_handle window) { static_cast<wxWindow*>(window)->ReleaseMouse(); }
bool wxsharp_window_has_capture(wxsharp_handle window) { return static_cast<wxWindow*>(window)->HasCapture(); }

void wxsharp_window_update_ui(wxsharp_handle window, bool recurse)
{
    static_cast<wxWindow*>(window)->UpdateWindowUI(recurse ? wxUPDATE_UI_RECURSE : wxUPDATE_UI_NONE);
}

bool wxsharp_window_register_hotkey(wxsharp_handle window, int hotkey_id, int modifiers, int key_code)
{
#if wxUSE_HOTKEY
    return static_cast<wxWindow*>(window)->RegisterHotKey(hotkey_id, modifiers, key_code);
#else
    (void)window; (void)hotkey_id; (void)modifiers; (void)key_code;
    return false;
#endif
}

bool wxsharp_window_unregister_hotkey(wxsharp_handle window, int hotkey_id)
{
#if wxUSE_HOTKEY
    return static_cast<wxWindow*>(window)->UnregisterHotKey(hotkey_id);
#else
    (void)window; (void)hotkey_id;
    return false;
#endif
}

bool wxsharp_event_propagates(int event_id)
{
    const EventRow* row = FindRow(event_id);
    return row && (row->flags & ROW_PROPAGATES) != 0;
}
