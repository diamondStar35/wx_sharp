// wxsharp - a flat, UTF-8 C ABI over wxWidgets. Windows, dialogs and controls are opaque handles. Events
// cross the boundary as a versioned value-only structure, keeping the ABI friendly to Native AOT.
#pragma once

#ifndef __cplusplus
#  include <stdbool.h>
#endif

#if defined(WXSHARP_STATIC)
#  define WXSHARP_API
#elif defined(_WIN32) && defined(WXSHARP_BUILD)
#  define WXSHARP_API __declspec(dllexport)
#elif defined(_WIN32)
#  define WXSHARP_API __declspec(dllimport)
#elif defined(__GNUC__) && defined(WXSHARP_BUILD)
#  define WXSHARP_API __attribute__((visibility("default")))
#else
#  define WXSHARP_API
#endif

#ifdef __cplusplus
extern "C" {
#endif
    typedef void* wxsharp_handle;

    // One event crossing the boundary. Version 2 carries a payload area wide enough for the item, column,
    // selection and string data that list, tree, book and data-view events need; the 8-byte members are
    // grouped first so the layout is identical on every supported ABI without explicit packing.
    // `text` points at a UTF-8 buffer owned by the native side and is valid only for the duration of the
    // callback - copy it before returning.
    typedef struct wxsharp_event
    {
        unsigned int size;
        unsigned int version;
        long long token;
        long long item;
        long long old_item;
        double double_value;
        const char* text;
        int kind;
        int id;
        int x;
        int y;
        int width;
        int height;
        int key_code;
        int modifiers;
        int mouse_button;
        int wheel_delta;
        int active;
        int can_veto;
        int column;
        int selection;
        int old_selection;
        int int_value;
        int text_length;
        unsigned int uint_value;
    } wxsharp_event;

    // The event-struct layout the managed side is compiled against. Bumped on any change to the struct.
    enum { WXSHARP_EVENT_VERSION = 2 };

    // Modifier bits reported in wxsharp_event.modifiers.
    enum
    {
        WXSHARP_MOD_CONTROL = 1,
        WXSHARP_MOD_SHIFT = 2,
        WXSHARP_MOD_ALT = 4,
        WXSHARP_MOD_META = 8,
        WXSHARP_MOD_RAW_CONTROL = 16,
    };

    // Callback result flags, matching wxWidgets' own model: an event is handled unless the handler asks to
    // skip it, and skipping is what lets normal wx processing and command propagation continue. SKIP is
    // also the result when nothing was listening. VETO refuses the action a vetoable event announced.
    enum { WXSHARP_EVENT_SKIP = 1, WXSHARP_EVENT_VETO = 2 };
    typedef unsigned int (*wxsharp_event_cb)(const wxsharp_event* event_data);

    // ---- Event identifiers ----------------------------------------------------------------------------
    // Stable wrapper event IDs. Each maps to exactly one wxEventType through the table in events.cpp, so
    // adding an event is one table row here and one EventType<T> on the managed side. Values are permanent
    // ABI: append, never renumber.
    enum
    {
        // Window lifecycle and geometry.
        WXSHARP_EV_CLOSE = 1,
        WXSHARP_EV_SHOW = 2,
        WXSHARP_EV_ACTIVATE = 3,
        WXSHARP_EV_SIZE = 4,
        WXSHARP_EV_MOVE = 5,
        WXSHARP_EV_MAXIMIZE = 6,
        WXSHARP_EV_ICONIZE = 7,
        WXSHARP_EV_DESTROY = 8,
        WXSHARP_EV_SET_FOCUS = 9,
        WXSHARP_EV_KILL_FOCUS = 10,
        WXSHARP_EV_PAINT = 11,
        WXSHARP_EV_CONTEXT_MENU = 12,
        // Asked, on idle and whenever a menu is about to open, what state a command should be in. The
        // handler answers with wxsharp_updateui_*; see the note on those functions.
        WXSHARP_EV_UPDATE_UI = 13,
        WXSHARP_EV_IDLE = 14,
        WXSHARP_EV_CHILD_FOCUS = 15,
        WXSHARP_EV_NAVIGATION_KEY = 16,
        // Must be handled by anything that captures the mouse; wxWidgets asserts otherwise.
        WXSHARP_EV_MOUSE_CAPTURE_LOST = 17,
        WXSHARP_EV_MOUSE_CAPTURE_CHANGED = 18,
        // Paths are read back with wxsharp_dropfiles_path() during the callback.
        WXSHARP_EV_DROP_FILES = 19,
        WXSHARP_EV_HOTKEY = 20,
        WXSHARP_EV_HELP = 21,
        WXSHARP_EV_MENU_OPEN = 22,
        WXSHARP_EV_MENU_CLOSE = 23,
        WXSHARP_EV_MENU_HIGHLIGHT = 24,

        // Mouse.
        WXSHARP_EV_LEFT_DOWN = 31,
        WXSHARP_EV_LEFT_UP = 32,
        WXSHARP_EV_LEFT_DCLICK = 33,
        WXSHARP_EV_RIGHT_DOWN = 34,
        WXSHARP_EV_RIGHT_UP = 35,
        WXSHARP_EV_RIGHT_DCLICK = 36,
        WXSHARP_EV_MIDDLE_DOWN = 37,
        WXSHARP_EV_MIDDLE_UP = 38,
        WXSHARP_EV_MIDDLE_DCLICK = 39,
        WXSHARP_EV_MOTION = 40,
        WXSHARP_EV_ENTER_WINDOW = 41,
        WXSHARP_EV_LEAVE_WINDOW = 42,
        WXSHARP_EV_MOUSEWHEEL = 43,

        // Keyboard. CHAR_HOOK reaches the top-level window before the focused control sees the key.
        WXSHARP_EV_CHAR_HOOK = 51,
        WXSHARP_EV_KEY_DOWN = 52,
        WXSHARP_EV_KEY_UP = 53,
        WXSHARP_EV_CHAR = 54,

        // Control commands.
        WXSHARP_EV_BUTTON = 61,
        WXSHARP_EV_CHECKBOX = 62,
        WXSHARP_EV_CHOICE = 63,
        WXSHARP_EV_LISTBOX = 64,
        WXSHARP_EV_LISTBOX_DCLICK = 65,
        WXSHARP_EV_TEXT = 66,
        WXSHARP_EV_TEXT_ENTER = 67,
        WXSHARP_EV_MENU = 68,
        WXSHARP_EV_SLIDER = 69,
        WXSHARP_EV_RADIOBUTTON = 70,
        WXSHARP_EV_RADIOBOX = 71,
        WXSHARP_EV_COMBOBOX = 72,
        WXSHARP_EV_TOGGLEBUTTON = 73,
        WXSHARP_EV_CHECKLISTBOX = 74,
        WXSHARP_EV_SPINCTRL = 75,
        WXSHARP_EV_SPINCTRLDOUBLE = 76,
        WXSHARP_EV_SCROLL_THUMBTRACK = 77,
        WXSHARP_EV_SCROLL_CHANGED = 78,
        WXSHARP_EV_HYPERLINK = 79,
        WXSHARP_EV_SEARCH = 80,
        WXSHARP_EV_SEARCH_CANCEL = 81,
        WXSHARP_EV_DATE_CHANGED = 82,
        WXSHARP_EV_TIME_CHANGED = 83,
        WXSHARP_EV_COMBOBOX_DROPDOWN = 84,
        WXSHARP_EV_COMBOBOX_CLOSEUP = 85,
        WXSHARP_EV_TIMER = 86,
        WXSHARP_EV_SPIN = 87,
        WXSHARP_EV_SPIN_UP = 88,
        WXSHARP_EV_SPIN_DOWN = 89,
        WXSHARP_EV_SCROLLBAR = 90,
        WXSHARP_EV_SCROLL_TOP = 91,
        WXSHARP_EV_SCROLL_BOTTOM = 92,
        WXSHARP_EV_SCROLL_LINEUP = 93,
        WXSHARP_EV_SCROLL_LINEDOWN = 94,
        WXSHARP_EV_SCROLL_PAGEUP = 95,
        WXSHARP_EV_SCROLL_PAGEDOWN = 96,
        WXSHARP_EV_SCROLL_THUMBRELEASE = 97,
        WXSHARP_EV_TEXT_MAXLEN = 98,
        WXSHARP_EV_TEXT_URL = 99,
        WXSHARP_EV_LIST_INSERT_ITEM = 100,

        // Book controls.
        WXSHARP_EV_NOTEBOOK_PAGE_CHANGED = 101,
        WXSHARP_EV_NOTEBOOK_PAGE_CHANGING = 102,
        WXSHARP_EV_BOOKCTRL_PAGE_CHANGED = 103,
        WXSHARP_EV_BOOKCTRL_PAGE_CHANGING = 104,

        // wxListCtrl.
        WXSHARP_EV_LIST_ITEM_SELECTED = 111,
        WXSHARP_EV_LIST_ITEM_DESELECTED = 112,
        WXSHARP_EV_LIST_ITEM_ACTIVATED = 113,
        WXSHARP_EV_LIST_ITEM_FOCUSED = 114,
        WXSHARP_EV_LIST_ITEM_RIGHT_CLICK = 115,
        WXSHARP_EV_LIST_COL_CLICK = 116,
        WXSHARP_EV_LIST_KEY_DOWN = 117,
        WXSHARP_EV_LIST_BEGIN_LABEL_EDIT = 118,
        WXSHARP_EV_LIST_END_LABEL_EDIT = 119,
        WXSHARP_EV_LIST_BEGIN_DRAG = 120,
        WXSHARP_EV_LIST_BEGIN_RIGHT_DRAG = 121,
        WXSHARP_EV_LIST_ITEM_MIDDLE_CLICK = 122,
        WXSHARP_EV_LIST_ITEM_CHECKED = 123,
        WXSHARP_EV_LIST_ITEM_UNCHECKED = 124,
        WXSHARP_EV_LIST_COL_RIGHT_CLICK = 125,
        WXSHARP_EV_LIST_COL_BEGIN_DRAG = 126,
        WXSHARP_EV_LIST_COL_END_DRAG = 127,
        WXSHARP_EV_LIST_DELETE_ITEM = 128,
        WXSHARP_EV_LIST_DELETE_ALL_ITEMS = 129,
        WXSHARP_EV_LIST_CACHE_HINT = 130,

        // wxTreeCtrl.
        WXSHARP_EV_TREE_SEL_CHANGED = 131,
        WXSHARP_EV_TREE_SEL_CHANGING = 132,
        WXSHARP_EV_TREE_ITEM_ACTIVATED = 133,
        WXSHARP_EV_TREE_ITEM_EXPANDED = 134,
        WXSHARP_EV_TREE_ITEM_EXPANDING = 135,
        WXSHARP_EV_TREE_ITEM_COLLAPSED = 136,
        WXSHARP_EV_TREE_ITEM_COLLAPSING = 137,
        WXSHARP_EV_TREE_ITEM_RIGHT_CLICK = 138,
        WXSHARP_EV_TREE_KEY_DOWN = 139,
        WXSHARP_EV_TREE_BEGIN_LABEL_EDIT = 140,
        WXSHARP_EV_TREE_END_LABEL_EDIT = 141,
        WXSHARP_EV_TREE_ITEM_MENU = 142,
        WXSHARP_EV_TREE_BEGIN_DRAG = 143,
        WXSHARP_EV_TREE_END_DRAG = 144,
        WXSHARP_EV_TREE_ITEM_MIDDLE_CLICK = 145,
        WXSHARP_EV_TREE_DELETE_ITEM = 146,
        WXSHARP_EV_TREE_ITEM_GETTOOLTIP = 147,
        WXSHARP_EV_TREE_STATE_IMAGE_CLICK = 148,

        // wxDataViewCtrl.
        WXSHARP_EV_DATAVIEW_SELECTION_CHANGED = 151,
        WXSHARP_EV_DATAVIEW_ITEM_ACTIVATED = 152,
        WXSHARP_EV_DATAVIEW_ITEM_CONTEXT_MENU = 153,
        WXSHARP_EV_DATAVIEW_ITEM_EXPANDED = 154,
        WXSHARP_EV_DATAVIEW_ITEM_EXPANDING = 155,
        WXSHARP_EV_DATAVIEW_ITEM_COLLAPSED = 156,
        WXSHARP_EV_DATAVIEW_ITEM_COLLAPSING = 157,
        WXSHARP_EV_DATAVIEW_ITEM_EDITING_STARTED = 158,
        WXSHARP_EV_DATAVIEW_ITEM_EDITING_DONE = 159,
        WXSHARP_EV_DATAVIEW_ITEM_VALUE_CHANGED = 160,
        WXSHARP_EV_DATAVIEW_COLUMN_HEADER_CLICK = 161,
        WXSHARP_EV_DATAVIEW_COLUMN_HEADER_RIGHT_CLICK = 162,
        WXSHARP_EV_DATAVIEW_COLUMN_SORTED = 163,
        WXSHARP_EV_DATAVIEW_COLUMN_REORDERED = 164,

        // wxSplitterWindow.
        WXSHARP_EV_SPLITTER_SASH_POS_CHANGED = 171,
        WXSHARP_EV_SPLITTER_DCLICK = 172,
        WXSHARP_EV_SPLITTER_SASH_POS_CHANGING = 173,
        WXSHARP_EV_SPLITTER_UNSPLIT = 174,

        // wxGrid.
        WXSHARP_EV_GRID_CELL_CHANGED = 181,
        WXSHARP_EV_GRID_SELECT_CELL = 182,

        // wxToolBar.
        WXSHARP_EV_TOOL_ENTER = 191,
        WXSHARP_EV_TOOL_RCLICKED = 192,
        WXSHARP_EV_TOOL_DROPDOWN = 193,

        // wxTextCtrl clipboard interception.
        WXSHARP_EV_TEXT_COPY = 201,
        WXSHARP_EV_TEXT_CUT = 202,
        WXSHARP_EV_TEXT_PASTE = 203,
        WXSHARP_EV_CLIPBOARD_CHANGED = 204,

        // Internal: not bindable, delivered directly by the runtime.
        WXSHARP_EV_CALL_AFTER = 1001,
    };

    typedef struct wxsharp_accessible_request
    {
        unsigned int size;
        unsigned int version;
        long long token;
        int operation;
        int child_id;
        int argument;
        int x;
        int y;
        int width;
        int height;
        int int_value;
        unsigned int uint_value;
        char* buffer;
        int buffer_length;
        int required_length;
    } wxsharp_accessible_request;
    typedef int (*wxsharp_accessible_cb)(wxsharp_accessible_request* request);

    // A list/tree control asking a managed subclass for one of Phoenix's item virtuals. `operation` selects
    // text, image, checked state or tree comparison; strings use buffer/required_length and scalar answers
    // use result.
    typedef struct wxsharp_virtual_list_request
    {
        unsigned int size;
        unsigned int version;
        long long token;
        long long item;
        long long other_item;
        int column;
        char* buffer;
        int buffer_length;
        int required_length;
        int operation;
        int result;
    } wxsharp_virtual_list_request;
    typedef bool (*wxsharp_virtual_list_cb)(wxsharp_virtual_list_request* request);

    // A wxWidgets virtual member being asked of managed code. wxWidgets asks a window questions - may it
    // take focus, how big does it want to be, is its content valid, where is its client area - by calling
    // virtual members, and a managed subclass answers them here. `which` names the member; `handled` says
    // managed code answered, and leaving it clear lets wxWidgets run its own implementation.
    //
    // The set is the one wxPython supports (etgtools/tweaker_tools.py, addWindowVirtuals): overriding every
    // C++ virtual would bloat the wrapper for members no application overrides, so both projects wrap the
    // same considered subset. Members wxPython lists that are absent here are recorded in
    // docs/phoenix-parity.md with the reason, which is always a type the wrapper does not have yet.
    enum
    {
        // Public.
        WXSHARP_VIRT_ACCEPTS_FOCUS = 1,
        WXSHARP_VIRT_ACCEPTS_FOCUS_FROM_KEYBOARD = 2,
        WXSHARP_VIRT_ACCEPTS_FOCUS_RECURSIVELY = 3,
        WXSHARP_VIRT_VALIDATE = 4,
        WXSHARP_VIRT_TRANSFER_TO_WINDOW = 5,
        WXSHARP_VIRT_TRANSFER_FROM_WINDOW = 6,
        WXSHARP_VIRT_INIT_DIALOG = 7,
        WXSHARP_VIRT_CLIENT_AREA_ORIGIN = 8,
        WXSHARP_VIRT_ADD_CHILD = 9,
        WXSHARP_VIRT_REMOVE_CHILD = 10,
        WXSHARP_VIRT_INHERIT_ATTRIBUTES = 11,
        WXSHARP_VIRT_SHOULD_INHERIT_COLOURS = 12,
        WXSHARP_VIRT_ON_INTERNAL_IDLE = 13,
        WXSHARP_VIRT_MAIN_WINDOW_OF_COMPOSITE = 14,
        WXSHARP_VIRT_INFORM_FIRST_DIRECTION = 15,
        WXSHARP_VIRT_SET_CAN_FOCUS = 16,
        WXSHARP_VIRT_ENABLE_VISIBLE_FOCUS = 17,

        // Protected.
        WXSHARP_VIRT_DO_ENABLE = 18,
        WXSHARP_VIRT_DO_GET_POSITION = 19,
        WXSHARP_VIRT_DO_GET_SIZE = 20,
        WXSHARP_VIRT_DO_GET_CLIENT_SIZE = 21,
        WXSHARP_VIRT_BEST_SIZE = 22,
        WXSHARP_VIRT_BEST_CLIENT_SIZE = 23,
        WXSHARP_VIRT_DO_SET_SIZE = 24,
        WXSHARP_VIRT_DO_SET_CLIENT_SIZE = 25,
        WXSHARP_VIRT_DO_SET_SIZE_HINTS = 26,
        WXSHARP_VIRT_DO_MOVE_WINDOW = 27,
        WXSHARP_VIRT_DO_SET_WINDOW_VARIANT = 28,
        WXSHARP_VIRT_DEFAULT_BORDER = 29,
        WXSHARP_VIRT_DO_FREEZE = 30,
        WXSHARP_VIRT_DO_THAW = 31,
        WXSHARP_VIRT_HAS_TRANSPARENT_BACKGROUND = 32,
        WXSHARP_VIRT_DESTROY = 33,

        // Members that exist only on one class, so the mixin carrying them is layered on the generic one.
        WXSHARP_VIRT_SHOULD_PREVENT_APP_EXIT = 34,   // wxTopLevelWindow
        WXSHARP_VIRT_GET_CONTENT_WINDOW = 35,        // wxDialog
        WXSHARP_VIRT_ON_CREATE_STATUS_BAR = 36,      // wxFrame
        WXSHARP_VIRT_ON_CREATE_TOOL_BAR = 37,        // wxFrame
        WXSHARP_VIRT_DO_GIVE_HELP = 38,              // wxFrame
        WXSHARP_VIRT_SHOULD_SCROLL_TO_CHILD_ON_FOCUS = 39, // wxScrolled
        WXSHARP_VIRT_SIZE_FOR_SCROLL_TARGET = 40,    // wxScrolled
        WXSHARP_VIRT_GRID_COL_LINE_PEN = 41,         // wxGrid
        WXSHARP_VIRT_GRID_ROW_LINE_PEN = 42,         // wxGrid
        WXSHARP_VIRT_GRID_DEFAULT_LINE_PEN = 43,     // wxGrid

        // The validator a window carries, and the three members every event passes through.
        WXSHARP_VIRT_SET_VALIDATOR = 44,
        WXSHARP_VIRT_GET_VALIDATOR = 45,
        WXSHARP_VIRT_PROCESS_EVENT = 46,
        WXSHARP_VIRT_TRY_BEFORE = 47,
        WXSHARP_VIRT_TRY_AFTER = 48
    };

    // One question, and its answer. `args` carries the member's parameters; `result` its bool or int
    // return; `x`/`y` a returned point or size, or a pair of out parameters; `handle` a window in either
    // direction. Members with no parameters and no return use none of them.
    typedef struct wxsharp_virtual_request
    {
        unsigned int size;
        unsigned int version;
        long long token;
        long long handle;
        int which;
        int handled;
        int result;
        int x;
        int y;
        int args[6];

        // A string argument, valid only for the duration of the callback - a status bar's name, or the help
        // text a frame is being asked to show.
        const char* text;

        // A packed 0xAARRGGBB colour, for the members that answer with a pen.
        unsigned int uint_value;
    } wxsharp_virtual_request;
    typedef void (*wxsharp_virtual_cb)(wxsharp_virtual_request* request);
    typedef struct wxsharp_accelerator { int modifiers; int key_code; int command_id; } wxsharp_accelerator;

    // ---- App lifetime ---------------------------------------------------------------------------------
    WXSHARP_API bool wxsharp_init();
    WXSHARP_API void wxsharp_set_event_handler(wxsharp_event_cb cb);
    WXSHARP_API void wxsharp_set_accessible_handler(wxsharp_accessible_cb cb);
    WXSHARP_API void wxsharp_set_virtual_list_handler(wxsharp_virtual_list_cb cb);
    WXSHARP_API void wxsharp_set_virtual_handler(wxsharp_virtual_cb cb);
    // Runs wxWidgets' own implementation of one virtual, without dispatching to the managed override that
    // is asking for it. This is what "calling the base implementation" compiles to.
    WXSHARP_API void wxsharp_window_call_base(wxsharp_handle window, wxsharp_virtual_request* request);
    WXSHARP_API int wxsharp_post_command_event(wxsharp_handle window, int event_id, int id, int int_value,
                                               const char* text, bool process_now);
    WXSHARP_API int  wxsharp_main_loop();
    WXSHARP_API void wxsharp_exit_main_loop();
    WXSHARP_API void wxsharp_set_exit_on_frame_delete(bool value);
    WXSHARP_API void wxsharp_set_top_window(wxsharp_handle window);
    // Light or dark interface. Returns wxApp::AppearanceResult: 0 failure, 1 ok, 2 cannot change now.
    WXSHARP_API int  wxsharp_app_set_appearance(int appearance);
    WXSHARP_API bool wxsharp_app_enable_dark_mode(int flags);
    WXSHARP_API bool wxsharp_app_supports_dark_mode();
    WXSHARP_API void wxsharp_call_after(long long token);
    WXSHARP_API bool wxsharp_yield(bool only_if_needed);
    WXSHARP_API int  wxsharp_message_box(wxsharp_handle parent, const char* message, const char* caption,
                                          int style);
    WXSHARP_API void wxsharp_shutdown();


    // ---- wxWindow, the rest ---------------------------------------------------------------------------
    WXSHARP_API void wxsharp_window_freeze(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_thaw(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_is_frozen(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_clear_background(wxsharp_handle window);

    WXSHARP_API void wxsharp_window_get_rect(wxsharp_handle window, int* x, int* y, int* width, int* height);
    WXSHARP_API void wxsharp_window_get_client_rect(wxsharp_handle window, int* x, int* y, int* width, int* height);
    WXSHARP_API void wxsharp_window_get_screen_rect(wxsharp_handle window, int* x, int* y, int* width, int* height);
    WXSHARP_API void wxsharp_window_get_screen_position(wxsharp_handle window, int* x, int* y);
    // These convert in place: pass the point in, read the converted point back out.
    WXSHARP_API void wxsharp_window_client_to_screen(wxsharp_handle window, int* x, int* y);
    WXSHARP_API void wxsharp_window_screen_to_client(wxsharp_handle window, int* x, int* y);
    WXSHARP_API void wxsharp_window_get_virtual_size(wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_window_set_virtual_size(wxsharp_handle window, int width, int height);
    WXSHARP_API void wxsharp_window_get_best_virtual_size(wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_window_get_min_size(wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_window_get_max_size(wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_window_get_min_client_size(wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_window_set_min_client_size(wxsharp_handle window, int width, int height);
    WXSHARP_API void wxsharp_window_get_max_client_size(wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_window_set_max_client_size(wxsharp_handle window, int width, int height);
    WXSHARP_API void wxsharp_window_get_border_size(wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_window_set_client_size(wxsharp_handle window, int width, int height);
    WXSHARP_API void wxsharp_window_fit_inside(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_convert_dialog_to_pixels(wxsharp_handle window, int* x, int* y);
    WXSHARP_API void wxsharp_window_convert_pixels_to_dialog(wxsharp_handle window, int* x, int* y);

    WXSHARP_API void wxsharp_window_get_text_extent(wxsharp_handle window, const char* text, int* width,
                                                    int* height, int* descent, int* external_leading);
    WXSHARP_API int  wxsharp_window_get_char_height(wxsharp_handle window);
    WXSHARP_API int  wxsharp_window_get_char_width(wxsharp_handle window);

    WXSHARP_API void wxsharp_window_get_dpi(wxsharp_handle window, int* x, int* y);
    WXSHARP_API void wxsharp_window_from_dip(wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_window_to_dip(wxsharp_handle window, int* width, int* height);

    WXSHARP_API void wxsharp_window_raise(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_lower(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_is_shown_on_screen(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_close_any(wxsharp_handle window, bool force);
    WXSHARP_API void wxsharp_window_center_any(wxsharp_handle window, bool on_parent);

    WXSHARP_API bool wxsharp_window_navigate(wxsharp_handle window, bool forward, bool window_change);
    WXSHARP_API bool wxsharp_window_navigate_in(wxsharp_handle window, bool forward, bool window_change);

    WXSHARP_API void wxsharp_window_set_scrollbar(wxsharp_handle window, bool vertical, int position,
                                                  int thumb_size, int range, bool refresh);
    WXSHARP_API void wxsharp_window_set_scroll_pos(wxsharp_handle window, bool vertical, int position, bool refresh);
    WXSHARP_API int  wxsharp_window_get_scroll_pos(wxsharp_handle window, bool vertical);
    WXSHARP_API int  wxsharp_window_get_scroll_range(wxsharp_handle window, bool vertical);
    WXSHARP_API int  wxsharp_window_get_scroll_thumb(wxsharp_handle window, bool vertical);
    WXSHARP_API bool wxsharp_window_has_scrollbar(wxsharp_handle window, bool vertical);
    WXSHARP_API bool wxsharp_window_scroll_lines(wxsharp_handle window, int lines);
    WXSHARP_API bool wxsharp_window_scroll_pages(wxsharp_handle window, int pages);
    WXSHARP_API bool wxsharp_window_line_up(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_line_down(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_page_up(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_page_down(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_scroll_window(wxsharp_handle window, int dx, int dy);

    WXSHARP_API int  wxsharp_window_get_style_flags(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_set_style_flags(wxsharp_handle window, int style);
    WXSHARP_API bool wxsharp_window_has_style_flag(wxsharp_handle window, int flag);
    WXSHARP_API int  wxsharp_window_get_label(wxsharp_handle window, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_window_set_label(wxsharp_handle window, const char* label);
    WXSHARP_API int  wxsharp_window_get_class_name(wxsharp_handle window, char* buffer, int buffer_length);
    WXSHARP_API wxsharp_handle wxsharp_window_get_parent(wxsharp_handle window);
    WXSHARP_API int  wxsharp_window_get_help_text(wxsharp_handle window, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_window_set_help_text(wxsharp_handle window, const char* text);
    WXSHARP_API bool wxsharp_window_is_double_buffered(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_set_double_buffered(wxsharp_handle window, bool on);
    WXSHARP_API int  wxsharp_window_get_background_style(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_set_background_style(wxsharp_handle window, int style);
    WXSHARP_API int  wxsharp_window_get_variant(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_set_variant(wxsharp_handle window, int variant);
    WXSHARP_API bool wxsharp_window_can_set_transparent(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_set_transparent(wxsharp_handle window, int alpha);

    WXSHARP_API void wxsharp_window_warp_pointer(wxsharp_handle window, int x, int y);
    WXSHARP_API int  wxsharp_window_hit_test(wxsharp_handle window, int x, int y);
    WXSHARP_API int  wxsharp_window_popup_menu_selection(wxsharp_handle window, wxsharp_handle menu, int x, int y);


    // ---- wxTextEntry ----------------------------------------------------------------------------------
    // The editing surface wxTextCtrl, wxComboBox and wxSearchCtrl share. Reached by cross-casting from the
    // window handle; every call is a no-op on a window that is not a text entry.
    WXSHARP_API bool wxsharp_textentry_supported(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_textentry_get_value(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_textentry_set_value(wxsharp_handle ctrl, const char* value);
    // Sets the text without raising a text-changed event.
    WXSHARP_API void wxsharp_textentry_change_value(wxsharp_handle ctrl, const char* value);
    WXSHARP_API void wxsharp_textentry_write_text(wxsharp_handle ctrl, const char* text);
    WXSHARP_API void wxsharp_textentry_append_text(wxsharp_handle ctrl, const char* text);
    WXSHARP_API int  wxsharp_textentry_get_range(wxsharp_handle ctrl, int from, int to, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_textentry_replace(wxsharp_handle ctrl, int from, int to, const char* value);
    WXSHARP_API void wxsharp_textentry_remove(wxsharp_handle ctrl, int from, int to);
    WXSHARP_API void wxsharp_textentry_clear(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textentry_is_empty(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textentry_copy(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textentry_cut(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textentry_paste(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textentry_can_copy(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textentry_can_cut(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textentry_can_paste(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textentry_undo(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textentry_redo(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textentry_can_undo(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textentry_can_redo(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textentry_set_insertion_point(wxsharp_handle ctrl, int position);
    WXSHARP_API void wxsharp_textentry_set_insertion_point_end(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_textentry_get_insertion_point(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_textentry_get_last_position(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textentry_set_selection(wxsharp_handle ctrl, int from, int to);
    WXSHARP_API void wxsharp_textentry_get_selection(wxsharp_handle ctrl, int* from, int* to);
    WXSHARP_API void wxsharp_textentry_select_all(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textentry_select_none(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textentry_has_selection(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_textentry_get_selected_text(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_textentry_remove_selection(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textentry_is_editable(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textentry_set_editable(wxsharp_handle ctrl, bool editable);
    WXSHARP_API void wxsharp_textentry_set_max_length(wxsharp_handle ctrl, int length);
    WXSHARP_API void wxsharp_textentry_force_upper(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textentry_set_hint(wxsharp_handle ctrl, const char* hint);
    WXSHARP_API int  wxsharp_textentry_get_hint(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_textentry_set_margins(wxsharp_handle ctrl, int left, int top);
    WXSHARP_API void wxsharp_textentry_get_margins(wxsharp_handle ctrl, int* left, int* top);
    WXSHARP_API bool wxsharp_textentry_auto_complete(wxsharp_handle ctrl, const char* const* choices, int count);
    WXSHARP_API bool wxsharp_textentry_auto_complete_files(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textentry_auto_complete_directories(wxsharp_handle ctrl);

    // ---- Clipboard ------------------------------------------------------------------------------------
    // Formats: 0 text, 1 file names, 2 bitmap.
    WXSHARP_API bool wxsharp_clipboard_open();
    WXSHARP_API void wxsharp_clipboard_close();
    WXSHARP_API bool wxsharp_clipboard_is_opened();
    // Hands ownership to the system so the contents survive this application exiting.
    WXSHARP_API bool wxsharp_clipboard_flush();
    WXSHARP_API void wxsharp_clipboard_clear();
    WXSHARP_API bool wxsharp_clipboard_is_supported(int format);
    WXSHARP_API bool wxsharp_clipboard_is_supported_async(wxsharp_handle sink);
    WXSHARP_API void wxsharp_clipboard_use_primary_selection(bool primary);
    WXSHARP_API bool wxsharp_clipboard_set_text(const char* text);
    WXSHARP_API int  wxsharp_clipboard_get_text(char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_clipboard_set_files(const char* const* paths, int count);
    // Reads the file list and holds it until the next call; then fetch each path by index.
    WXSHARP_API int  wxsharp_clipboard_read_files();
    WXSHARP_API int  wxsharp_clipboard_get_file(int index, char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_clipboard_set_bitmap(wxsharp_handle bitmap);
    WXSHARP_API wxsharp_handle wxsharp_clipboard_get_bitmap();

    // ---- System settings ------------------------------------------------------------------------------
    // What the user's theme says, which is what an application has to follow to work in a high-contrast
    // scheme rather than fighting it.
    WXSHARP_API unsigned int wxsharp_system_colour(int which);
    WXSHARP_API int  wxsharp_system_metric(int which, wxsharp_handle window);
    WXSHARP_API int  wxsharp_system_screen_type();
    WXSHARP_API bool wxsharp_system_has_feature(int which);
    WXSHARP_API bool wxsharp_system_appearance_is_dark();
    WXSHARP_API int  wxsharp_system_appearance_name(char* buffer, int buffer_length);

    // ---- Event binding --------------------------------------------------------------------------------
    // Events are hooked on demand: the managed side binds an event ID the first time something subscribes to
    // it on a window and unbinds it when the last subscriber goes away, so an unobserved event never crosses
    // the boundary. bind() returns false when the event ID is unknown or cannot be bound to this window (for
    // example TEXT_ENTER on a control that does not process Enter). unbind_all() releases every binding on a
    // window and is called when the window is destroyed.
    WXSHARP_API bool wxsharp_window_bind(wxsharp_handle window, int event_id, long long token);
    WXSHARP_API bool wxsharp_window_unbind(wxsharp_handle window, int event_id);
    WXSHARP_API void wxsharp_window_unbind_all(wxsharp_handle window);
    // Binds an event on the application object rather than a window, for the events wxWidgets only ever
    // sends there.
    WXSHARP_API bool wxsharp_app_bind(int event_id, long long token);
    WXSHARP_API bool wxsharp_app_unbind(int event_id);
    // True when wx propagates this event up the parent chain once it is skipped (command events).
    WXSHARP_API bool wxsharp_event_propagates(int event_id);

    // ---- Answering an update-UI event -----------------------------------------------------------------
    // A wxUpdateUIEvent is a question, not a notification: the handler says what state the command should be
    // in and wxWidgets applies it. These act on the event currently being dispatched and return false when
    // called at any other time. wxWidgets only applies the answer to an event that comes back handled,
    // which is the ordinary case - a handler has to skip explicitly.
    WXSHARP_API bool wxsharp_updateui_enable(bool enable);
    WXSHARP_API bool wxsharp_updateui_check(bool check);
    WXSHARP_API bool wxsharp_updateui_show(bool show);
    WXSHARP_API bool wxsharp_updateui_set_text(const char* text);
    // How often update-UI events are sent, in milliseconds; -1 suppresses them, 0 means every idle cycle.
    WXSHARP_API void wxsharp_updateui_set_interval(int milliseconds);
    // false asks wx to send update-UI events only to windows that opted in, which is cheaper on a big UI.
    WXSHARP_API void wxsharp_updateui_set_process_all(bool process_all);

    // ---- Reading a drop-files event -------------------------------------------------------------------
    // Valid only during a DROP_FILES callback. count() is also reported as the event's item field.
    WXSHARP_API int  wxsharp_dropfiles_count();
    WXSHARP_API int  wxsharp_dropfiles_path(int index, char* buffer, int buffer_length);

    // ---- System-wide hot keys -------------------------------------------------------------------------
    // Delivers WXSHARP_EV_HOTKEY to this window even when the application is not focused. The modifiers are
    // the accelerator modifier bits.
    // Whether files dragged onto the window raise DROP_FILES. Off until asked for.
    WXSHARP_API void wxsharp_window_accept_dropped_files(wxsharp_handle window, bool accept);
    // Routes all mouse input to this window until released. Anything that captures MUST also handle
    // MOUSE_CAPTURE_LOST, because the capture can be taken away at any time.
    WXSHARP_API void wxsharp_window_capture_mouse(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_release_mouse(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_has_capture(wxsharp_handle window);

    // Sends update-UI events to this window (and its children when recursing) immediately, instead of
    // waiting for the next idle cycle.
    WXSHARP_API void wxsharp_window_update_ui(wxsharp_handle window, bool recurse);

    WXSHARP_API bool wxsharp_window_register_hotkey(wxsharp_handle window, int hotkey_id, int modifiers,
                                                    int key_code);
    WXSHARP_API bool wxsharp_window_unregister_hotkey(wxsharp_handle window, int hotkey_id);

    // ---- Frame ----------------------------------------------------------------------------------------
    // Child panels/controls and sizers are always created and assigned explicitly.
    WXSHARP_API wxsharp_handle wxsharp_window_create(wxsharp_handle parent, int id, const char* title,
                                                     int x, int y, int width, int height, int style,
                                                     long long token);
    WXSHARP_API void wxsharp_window_show(wxsharp_handle window, bool show);
    WXSHARP_API void wxsharp_window_set_title(wxsharp_handle window, const char* title);
    WXSHARP_API int  wxsharp_window_get_title(wxsharp_handle window, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_window_center(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_set_fullscreen(wxsharp_handle window, bool fullscreen); // borderless, hides any menu bar
    WXSHARP_API void* wxsharp_window_native_handle(wxsharp_handle window); // HWND, GtkWidget*, or NSView*
    WXSHARP_API void wxsharp_window_close(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_destroy(wxsharp_handle window);

    // ---- Dialog (modal or modeless) ------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_dialog_create(wxsharp_handle parent, int id, const char* title,
                                                     int x, int y, int width, int height, int style,
                                                     long long token);
    // Builds the platform's standard button row for a dialog - correct order, correct default and cancel
    // buttons, correct spacing - and returns it as a sizer to add to the dialog's layout.
    WXSHARP_API wxsharp_handle wxsharp_dialog_create_button_sizer(wxsharp_handle dialog, int flags);
    WXSHARP_API void wxsharp_dialog_set_title(wxsharp_handle dialog, const char* title);
    WXSHARP_API int  wxsharp_dialog_get_title(wxsharp_handle dialog, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_dialog_set_escape_id(wxsharp_handle dialog, int id);       // id returned when Esc is pressed
    WXSHARP_API void wxsharp_dialog_set_affirmative_id(wxsharp_handle dialog, int id);  // id activated when Enter is pressed
    WXSHARP_API int  wxsharp_dialog_show_modal(wxsharp_handle dialog); // blocks, returns EndModal's result
    WXSHARP_API void wxsharp_dialog_show(wxsharp_handle dialog, bool show); // modeless: returns immediately
    WXSHARP_API void wxsharp_dialog_end_modal(wxsharp_handle dialog, int result);
    WXSHARP_API void wxsharp_dialog_destroy(wxsharp_handle dialog);

    // ---- Explicit panel container ---------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_panel_create(wxsharp_handle parent, int id, int style, long long token);

    // Creation variants whose windows route the whitelisted virtuals above back to managed code. A control
    // has to opt in at construction, because C++ fixes its vtable there; the plain create functions stay
    // the zero-overhead path for a control that overrides nothing.
    WXSHARP_API wxsharp_handle wxsharp_custom_frame_create(wxsharp_handle parent, int id, const char* title,
                                                          int x, int y, int width, int height, int style,
                                                          long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_panel_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_button_create(wxsharp_handle parent, int id, const char* label, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_dialog_create(wxsharp_handle parent, int id, const char* title,
                                                           int x, int y, int width, int height, int style,
                                                           long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_activity_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_bitmapbutton_create(wxsharp_handle parent, int id, wxsharp_handle bitmap, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_checkbox_create(wxsharp_handle parent, int id, const char* label, int style, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_checklistbox_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_choice_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_combobox_create(wxsharp_handle parent, int id, const char* value, bool readOnly, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_dataviewlist_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_dataviewtree_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_datepicker_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_gauge_create(wxsharp_handle parent, int id, int range, int value, bool vertical, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_grid_create(wxsharp_handle parent, int id, int rows, int columns, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_hyperlink_create(wxsharp_handle parent, int id, const char* label, const char* url, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_label_create(wxsharp_handle parent, int id, const char* text, int style, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_listbox_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_notebook_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_radio_create(wxsharp_handle parent, int id, const char* label, bool group_start, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_radiobox_create(wxsharp_handle parent, int id, const char* label, const char* const* choices, int count, int columns, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_scrollbar_create(wxsharp_handle parent, int id, bool vertical, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_scrolled_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_searchctrl_create(wxsharp_handle parent, int id, const char* value, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_simplebook_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_slider_create(wxsharp_handle parent, int id, int min_value, int max_value, int value, int style, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_spinctrl_create(wxsharp_handle parent, int id, int minValue, int maxValue, int value, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_spinctrldouble_create(wxsharp_handle parent, int id, double minValue, double maxValue, double value, double increment, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_splitter_create(wxsharp_handle parent, int id, bool vertical, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_staticbitmap_create(wxsharp_handle parent, int id, wxsharp_handle bitmap, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_staticbox_create(wxsharp_handle parent, int id, const char* label, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_staticline_create(wxsharp_handle parent, int id, bool vertical, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_textbox_create(wxsharp_handle parent, int id, const char* value, int style, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_timepicker_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_togglebutton_create(wxsharp_handle parent, int id, const char* label, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_treectrl_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_listctrl_create(wxsharp_handle parent, int id, int style, long long token);

    // ---- Canvas -------------------------------------------------------------------------------------
    // A non-focusable, custom-drawn surface (skipped by assistive tech). It reports a Paint event; draw from
    // the managed handler with the functions below - they only take effect during that paint. A colour with
    // alpha 0 selects the transparent pen/brush. measure_text works any time (uses the control font).
    WXSHARP_API wxsharp_handle wxsharp_canvas_create(wxsharp_handle parent, int id, int width, int height, long long token);
    WXSHARP_API void wxsharp_canvas_clear(wxsharp_handle ctrl, unsigned int argb);
    WXSHARP_API void wxsharp_canvas_set_brush(wxsharp_handle ctrl, unsigned int argb);
    WXSHARP_API void wxsharp_canvas_set_pen(wxsharp_handle ctrl, unsigned int argb, int width);
    WXSHARP_API void wxsharp_canvas_set_text_colour(wxsharp_handle ctrl, unsigned int argb);
    WXSHARP_API void wxsharp_canvas_set_font(wxsharp_handle ctrl, wxsharp_handle font);
    WXSHARP_API void wxsharp_canvas_draw_rectangle(wxsharp_handle ctrl, int x, int y, int width, int height);
    WXSHARP_API void wxsharp_canvas_draw_rounded_rectangle(wxsharp_handle ctrl, int x, int y, int width, int height, int radius);
    WXSHARP_API void wxsharp_canvas_draw_line(wxsharp_handle ctrl, int x1, int y1, int x2, int y2);
    WXSHARP_API void wxsharp_canvas_draw_circle(wxsharp_handle ctrl, int x, int y, int radius);
    WXSHARP_API void wxsharp_canvas_draw_ellipse(wxsharp_handle ctrl, int x, int y, int width, int height);
    WXSHARP_API void wxsharp_canvas_draw_text(wxsharp_handle ctrl, const char* text, int x, int y);
    WXSHARP_API void wxsharp_canvas_measure_text(wxsharp_handle ctrl, const char* text, int* width, int* height);

    // ---- Generic control ops (every control is a wxWindow, so these apply to all of them) --------------
    WXSHARP_API void wxsharp_control_enable(wxsharp_handle ctrl, bool enable);
    WXSHARP_API void wxsharp_control_show(wxsharp_handle ctrl, bool show);
    WXSHARP_API void wxsharp_control_focus(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_control_accepts_focus(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_control_accepts_focus_from_keyboard(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_control_accepts_focus_recursively(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_control_has_flag(wxsharp_handle ctrl, int flag);
    WXSHARP_API void wxsharp_control_layout(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_control_destroy(wxsharp_handle ctrl); // hides and destroys the control (create-on-demand UI)

    // Geometry (sizes/positions in device pixels).
    WXSHARP_API void wxsharp_control_get_size(wxsharp_handle ctrl, int* width, int* height);
    WXSHARP_API void wxsharp_control_set_size(wxsharp_handle ctrl, int width, int height);
    WXSHARP_API void wxsharp_control_get_client_size(wxsharp_handle ctrl, int* width, int* height);
    WXSHARP_API void wxsharp_control_get_position(wxsharp_handle ctrl, int* x, int* y);
    WXSHARP_API void wxsharp_control_set_position(wxsharp_handle ctrl, int x, int y);
    WXSHARP_API void wxsharp_control_set_min_size(wxsharp_handle ctrl, int width, int height);
    WXSHARP_API void wxsharp_control_set_max_size(wxsharp_handle ctrl, int width, int height);
    WXSHARP_API void wxsharp_control_get_best_size(wxsharp_handle ctrl, int* width, int* height);
    WXSHARP_API wxsharp_handle wxsharp_control_get_font(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_control_fit(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_control_get_pointer_position(wxsharp_handle ctrl, int* x, int* y); // mouse in client coords

    // Appearance (colours are packed 0xAARRGGBB; the font is described by the managed Font).
    WXSHARP_API void wxsharp_control_set_background_colour(wxsharp_handle ctrl, unsigned int argb);
    WXSHARP_API unsigned int wxsharp_control_get_background_colour(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_control_set_foreground_colour(wxsharp_handle ctrl, unsigned int argb);
    WXSHARP_API unsigned int wxsharp_control_get_foreground_colour(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_control_set_font(wxsharp_handle ctrl, wxsharp_handle font);
    WXSHARP_API void wxsharp_control_set_tooltip(wxsharp_handle ctrl, const char* text);
    WXSHARP_API int  wxsharp_control_get_name(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_control_set_border(wxsharp_handle ctrl, int border); // WxSharp Border enum value
    WXSHARP_API void wxsharp_control_refresh(wxsharp_handle ctrl, bool erase_background);

    // State queries.
    WXSHARP_API bool wxsharp_control_is_enabled(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_control_is_shown(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_control_has_focus(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_control_get_id(wxsharp_handle ctrl);

    // ---- Accessibility -------------------------------------------------------------------------------
    // Phoenix/wxWidgets custom wxAccessible objects are available only when wxUSE_ACCESSIBILITY is enabled
    // (currently MSW). Standard controls still use each platform's native accessibility implementation.
    WXSHARP_API bool wxsharp_custom_accessibility_available();
    WXSHARP_API void wxsharp_control_set_name(wxsharp_handle ctrl, const char* name);        // accessible name
    WXSHARP_API void wxsharp_control_set_accessible(wxsharp_handle ctrl, long long token);
    WXSHARP_API void wxsharp_accessible_notify(int event_type, wxsharp_handle window, int object_type,
                                               int child_id);
    WXSHARP_API unsigned int wxsharp_accessible_probe(wxsharp_handle window);

    // ---- Sizers ---------------------------------------------------------------------------------------
    // Explicit layout: a box sizer lays items in one direction; add controls/sizers with a proportion
    // (0 = fixed), expand/centre, and border, plus fixed or stretchable spacers. A window adopts a sizer.
    // ---- Sizers ---------------------------------------------------------------------------------------
    // Items are wxSizerItem handles, so what a sizer was told about an item can be read back and changed.
    WXSHARP_API wxsharp_handle wxsharp_boxsizer_create(bool horizontal);
    WXSHARP_API wxsharp_handle wxsharp_gridsizer_create(int rows, int columns, int vertical_gap, int horizontal_gap);
    WXSHARP_API wxsharp_handle wxsharp_flexgridsizer_create(int rows, int columns, int vertical_gap, int horizontal_gap);
    WXSHARP_API wxsharp_handle wxsharp_staticboxsizer_create(wxsharp_handle box, bool horizontal);
    WXSHARP_API wxsharp_handle wxsharp_gridbagsizer_create(int vertical_gap, int horizontal_gap);

    WXSHARP_API wxsharp_handle wxsharp_sizer_add_control(wxsharp_handle sizer, wxsharp_handle ctrl, int proportion, int flags, int border);
    WXSHARP_API wxsharp_handle wxsharp_sizer_add_sizer(wxsharp_handle sizer, wxsharp_handle child, int proportion, int flags, int border);
    WXSHARP_API wxsharp_handle wxsharp_sizer_add_spacer(wxsharp_handle sizer, int size);
    WXSHARP_API wxsharp_handle wxsharp_sizer_add_stretch_spacer(wxsharp_handle sizer, int proportion);
    WXSHARP_API wxsharp_handle wxsharp_sizer_insert_control(wxsharp_handle sizer, int index, wxsharp_handle ctrl, int proportion, int flags, int border);
    WXSHARP_API wxsharp_handle wxsharp_sizer_insert_sizer(wxsharp_handle sizer, int index, wxsharp_handle child, int proportion, int flags, int border);
    WXSHARP_API wxsharp_handle wxsharp_sizer_insert_spacer(wxsharp_handle sizer, int index, int size);
    WXSHARP_API wxsharp_handle wxsharp_sizer_insert_stretch_spacer(wxsharp_handle sizer, int index, int proportion);
    WXSHARP_API wxsharp_handle wxsharp_sizer_prepend_control(wxsharp_handle sizer, wxsharp_handle ctrl, int proportion, int flags, int border);
    WXSHARP_API wxsharp_handle wxsharp_sizer_prepend_sizer(wxsharp_handle sizer, wxsharp_handle child, int proportion, int flags, int border);
    WXSHARP_API wxsharp_handle wxsharp_sizer_prepend_spacer(wxsharp_handle sizer, int size);
    WXSHARP_API wxsharp_handle wxsharp_sizer_prepend_stretch_spacer(wxsharp_handle sizer, int proportion);

    // Detach leaves the window or sizer alive; remove deletes a nested sizer.
    WXSHARP_API bool wxsharp_sizer_detach_control(wxsharp_handle sizer, wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_sizer_detach_sizer(wxsharp_handle sizer, wxsharp_handle child);
    WXSHARP_API bool wxsharp_sizer_detach_at(wxsharp_handle sizer, int index);
    WXSHARP_API bool wxsharp_sizer_remove_sizer(wxsharp_handle sizer, wxsharp_handle child);
    WXSHARP_API bool wxsharp_sizer_remove_at(wxsharp_handle sizer, int index);
    WXSHARP_API void wxsharp_sizer_clear(wxsharp_handle sizer, bool delete_windows);
    WXSHARP_API void wxsharp_sizer_delete_windows(wxsharp_handle sizer);
    WXSHARP_API bool wxsharp_sizer_replace_control(wxsharp_handle sizer, wxsharp_handle old_ctrl, wxsharp_handle new_ctrl, bool recursive);
    WXSHARP_API bool wxsharp_sizer_replace_sizer(wxsharp_handle sizer, wxsharp_handle old_sizer, wxsharp_handle new_sizer, bool recursive);

    WXSHARP_API int  wxsharp_sizer_item_count(wxsharp_handle sizer);
    WXSHARP_API bool wxsharp_sizer_is_empty(wxsharp_handle sizer);
    WXSHARP_API wxsharp_handle wxsharp_sizer_item_at(wxsharp_handle sizer, int index);
    WXSHARP_API wxsharp_handle wxsharp_sizer_item_for_control(wxsharp_handle sizer, wxsharp_handle ctrl, bool recursive);
    WXSHARP_API wxsharp_handle wxsharp_sizer_item_for_sizer(wxsharp_handle sizer, wxsharp_handle child, bool recursive);
    WXSHARP_API wxsharp_handle wxsharp_sizer_item_by_id(wxsharp_handle sizer, int id, bool recursive);

    WXSHARP_API bool wxsharp_sizer_show_control(wxsharp_handle sizer, wxsharp_handle ctrl, bool show, bool recursive);
    WXSHARP_API bool wxsharp_sizer_show_sizer(wxsharp_handle sizer, wxsharp_handle child, bool show, bool recursive);
    WXSHARP_API bool wxsharp_sizer_show_at(wxsharp_handle sizer, int index, bool show);
    WXSHARP_API void wxsharp_sizer_show_items(wxsharp_handle sizer, bool show);
    WXSHARP_API bool wxsharp_sizer_any_items_shown(wxsharp_handle sizer);
    WXSHARP_API bool wxsharp_sizer_is_shown_control(wxsharp_handle sizer, wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_sizer_is_shown_sizer(wxsharp_handle sizer, wxsharp_handle child);
    WXSHARP_API bool wxsharp_sizer_is_shown_at(wxsharp_handle sizer, int index);

    WXSHARP_API void wxsharp_sizer_layout(wxsharp_handle sizer);
    WXSHARP_API void wxsharp_sizer_fit(wxsharp_handle sizer, wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_sizer_fit_inside(wxsharp_handle sizer, wxsharp_handle window);
    WXSHARP_API void wxsharp_sizer_set_size_hints(wxsharp_handle sizer, wxsharp_handle window);
    WXSHARP_API void wxsharp_sizer_compute_fitting_client_size(wxsharp_handle sizer, wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_sizer_compute_fitting_window_size(wxsharp_handle sizer, wxsharp_handle window, int* width, int* height);
    WXSHARP_API void wxsharp_sizer_get_min_size(wxsharp_handle sizer, int* width, int* height);
    WXSHARP_API void wxsharp_sizer_set_min_size(wxsharp_handle sizer, int width, int height);
    WXSHARP_API void wxsharp_sizer_get_size(wxsharp_handle sizer, int* width, int* height);
    WXSHARP_API void wxsharp_sizer_get_position(wxsharp_handle sizer, int* x, int* y);
    WXSHARP_API void wxsharp_sizer_set_dimension(wxsharp_handle sizer, int x, int y, int width, int height);
    WXSHARP_API bool wxsharp_sizer_set_item_min_size_control(wxsharp_handle sizer, wxsharp_handle ctrl, int width, int height);
    WXSHARP_API bool wxsharp_sizer_set_item_min_size_sizer(wxsharp_handle sizer, wxsharp_handle child, int width, int height);
    WXSHARP_API bool wxsharp_sizer_set_item_min_size_at(wxsharp_handle sizer, int index, int width, int height);
    WXSHARP_API wxsharp_handle wxsharp_sizer_containing_window(wxsharp_handle sizer);

    // ---- Sizer items ----------------------------------------------------------------------------------
    WXSHARP_API int  wxsharp_sizeritem_get_proportion(wxsharp_handle item);
    WXSHARP_API void wxsharp_sizeritem_set_proportion(wxsharp_handle item, int proportion);
    WXSHARP_API int  wxsharp_sizeritem_get_flags(wxsharp_handle item);
    WXSHARP_API void wxsharp_sizeritem_set_flags(wxsharp_handle item, int flags);
    WXSHARP_API int  wxsharp_sizeritem_get_border(wxsharp_handle item);
    WXSHARP_API void wxsharp_sizeritem_set_border(wxsharp_handle item, int border);
    // A sizer item carries an ID of its own, separate from any window ID, and defaults to none.
    WXSHARP_API int  wxsharp_sizeritem_get_id(wxsharp_handle item);
    WXSHARP_API void wxsharp_sizeritem_set_id(wxsharp_handle item, int id);
    WXSHARP_API bool wxsharp_sizeritem_is_window(wxsharp_handle item);
    WXSHARP_API bool wxsharp_sizeritem_is_sizer(wxsharp_handle item);
    WXSHARP_API bool wxsharp_sizeritem_is_spacer(wxsharp_handle item);
    WXSHARP_API wxsharp_handle wxsharp_sizeritem_get_window(wxsharp_handle item);
    WXSHARP_API wxsharp_handle wxsharp_sizeritem_get_sizer(wxsharp_handle item);
    WXSHARP_API bool wxsharp_sizeritem_is_shown(wxsharp_handle item);
    WXSHARP_API void wxsharp_sizeritem_show(wxsharp_handle item, bool show);
    WXSHARP_API void wxsharp_sizeritem_get_min_size(wxsharp_handle item, int* width, int* height);
    WXSHARP_API void wxsharp_sizeritem_set_min_size(wxsharp_handle item, int width, int height);
    WXSHARP_API void wxsharp_sizeritem_get_size(wxsharp_handle item, int* width, int* height);
    WXSHARP_API void wxsharp_sizeritem_get_position(wxsharp_handle item, int* x, int* y);

    // ---- Sizer subclasses -----------------------------------------------------------------------------
    WXSHARP_API int  wxsharp_boxsizer_get_orientation(wxsharp_handle sizer);
    WXSHARP_API void wxsharp_boxsizer_set_orientation(wxsharp_handle sizer, bool vertical);
    WXSHARP_API int  wxsharp_gridsizer_get_rows(wxsharp_handle sizer);
    WXSHARP_API int  wxsharp_gridsizer_get_columns(wxsharp_handle sizer);
    WXSHARP_API void wxsharp_gridsizer_set_rows(wxsharp_handle sizer, int rows);
    WXSHARP_API void wxsharp_gridsizer_set_columns(wxsharp_handle sizer, int columns);
    WXSHARP_API int  wxsharp_gridsizer_get_vertical_gap(wxsharp_handle sizer);
    WXSHARP_API int  wxsharp_gridsizer_get_horizontal_gap(wxsharp_handle sizer);
    WXSHARP_API void wxsharp_gridsizer_set_vertical_gap(wxsharp_handle sizer, int gap);
    WXSHARP_API void wxsharp_gridsizer_set_horizontal_gap(wxsharp_handle sizer, int gap);
    WXSHARP_API int  wxsharp_gridsizer_effective_rows(wxsharp_handle sizer);
    WXSHARP_API int  wxsharp_gridsizer_effective_columns(wxsharp_handle sizer);
    WXSHARP_API void wxsharp_flexgridsizer_add_growable_row(wxsharp_handle sizer, int row, int proportion);
    WXSHARP_API void wxsharp_flexgridsizer_add_growable_column(wxsharp_handle sizer, int column, int proportion);
    WXSHARP_API void wxsharp_flexgridsizer_remove_growable_row(wxsharp_handle sizer, int row);
    WXSHARP_API void wxsharp_flexgridsizer_remove_growable_column(wxsharp_handle sizer, int column);
    WXSHARP_API bool wxsharp_flexgridsizer_is_row_growable(wxsharp_handle sizer, int row);
    WXSHARP_API bool wxsharp_flexgridsizer_is_column_growable(wxsharp_handle sizer, int column);
    // 0 horizontal, 1 vertical, 2 both.
    WXSHARP_API int  wxsharp_flexgridsizer_get_flexible_direction(wxsharp_handle sizer);
    WXSHARP_API void wxsharp_flexgridsizer_set_flexible_direction(wxsharp_handle sizer, int direction);
    // 0 none, 1 specified, 2 all - wxFlexSizerGrowMode.
    WXSHARP_API int  wxsharp_flexgridsizer_get_grow_mode(wxsharp_handle sizer);
    WXSHARP_API void wxsharp_flexgridsizer_set_grow_mode(wxsharp_handle sizer, int mode);
    WXSHARP_API int  wxsharp_flexgridsizer_row_heights(wxsharp_handle sizer, int* buffer, int buffer_length);
    WXSHARP_API int  wxsharp_flexgridsizer_column_widths(wxsharp_handle sizer, int* buffer, int buffer_length);
    WXSHARP_API wxsharp_handle wxsharp_staticboxsizer_get_box(wxsharp_handle sizer);

    WXSHARP_API wxsharp_handle wxsharp_gridbagsizer_add_control(wxsharp_handle sizer, wxsharp_handle ctrl, int row, int column, int row_span, int column_span, int flags, int border);
    WXSHARP_API wxsharp_handle wxsharp_gridbagsizer_add_sizer(wxsharp_handle sizer, wxsharp_handle child, int row, int column, int row_span, int column_span, int flags, int border);
    WXSHARP_API void wxsharp_gridbagsizer_get_item_position_control(wxsharp_handle sizer, wxsharp_handle ctrl, int* row, int* column);
    WXSHARP_API void wxsharp_gridbagsizer_get_item_position_at(wxsharp_handle sizer, int index, int* row, int* column);
    WXSHARP_API bool wxsharp_gridbagsizer_set_item_position_control(wxsharp_handle sizer, wxsharp_handle ctrl, int row, int column);
    WXSHARP_API bool wxsharp_gridbagsizer_set_item_position_at(wxsharp_handle sizer, int index, int row, int column);
    WXSHARP_API void wxsharp_gridbagsizer_get_item_span_control(wxsharp_handle sizer, wxsharp_handle ctrl, int* row_span, int* column_span);
    WXSHARP_API void wxsharp_gridbagsizer_get_item_span_at(wxsharp_handle sizer, int index, int* row_span, int* column_span);
    WXSHARP_API bool wxsharp_gridbagsizer_set_item_span_control(wxsharp_handle sizer, wxsharp_handle ctrl, int row_span, int column_span);
    WXSHARP_API bool wxsharp_gridbagsizer_set_item_span_at(wxsharp_handle sizer, int index, int row_span, int column_span);
    WXSHARP_API wxsharp_handle wxsharp_gridbagsizer_find_item_control(wxsharp_handle sizer, wxsharp_handle ctrl);
    WXSHARP_API wxsharp_handle wxsharp_gridbagsizer_find_item_sizer(wxsharp_handle sizer, wxsharp_handle child);
    WXSHARP_API wxsharp_handle wxsharp_gridbagsizer_find_item_at_position(wxsharp_handle sizer, int row, int column);
    WXSHARP_API wxsharp_handle wxsharp_gridbagsizer_find_item_at_point(wxsharp_handle sizer, int x, int y);
    WXSHARP_API void wxsharp_gridbagsizer_get_cell_size(wxsharp_handle sizer, int row, int column, int* width, int* height);
    WXSHARP_API void wxsharp_gridbagsizer_get_empty_cell_size(wxsharp_handle sizer, int* width, int* height);
    WXSHARP_API void wxsharp_gridbagsizer_set_empty_cell_size(wxsharp_handle sizer, int width, int height);
    WXSHARP_API bool wxsharp_gridbagsizer_check_for_intersection(wxsharp_handle sizer, int row, int column, int row_span, int column_span, wxsharp_handle exclude);

    WXSHARP_API void wxsharp_window_set_sizer(wxsharp_handle window, wxsharp_handle sizer);
    WXSHARP_API void wxsharp_window_set_sizer_and_fit(wxsharp_handle window, wxsharp_handle sizer);
    WXSHARP_API wxsharp_handle wxsharp_window_get_sizer(wxsharp_handle window);
    WXSHARP_API wxsharp_handle wxsharp_window_containing_sizer(wxsharp_handle window);

    // ---- Label ---------------------------------------------------------------------------------------
    // style: WxSharp Alignment enum (left/centre/right).
    WXSHARP_API wxsharp_handle wxsharp_label_create(wxsharp_handle parent, int id, const char* text, int style, long long token);
    WXSHARP_API void wxsharp_label_set_text(wxsharp_handle ctrl, const char* text);
    WXSHARP_API int  wxsharp_label_get_text(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_label_wrap(wxsharp_handle ctrl, int width);
    WXSHARP_API bool wxsharp_label_is_ellipsized(wxsharp_handle ctrl);

    // ---- Button --------------------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_button_create(wxsharp_handle parent, int id, const char* label, long long token);
    WXSHARP_API void wxsharp_button_set_default(wxsharp_handle ctrl); // make it the default (Enter activates it)
    WXSHARP_API void wxsharp_button_set_label(wxsharp_handle ctrl, const char* label);
    WXSHARP_API int  wxsharp_button_get_label(wxsharp_handle ctrl, char* buffer, int buffer_length);

    // ---- Text box ------------------------------------------------------------------------------------
    // style: WxSharp TextCtrlStyle flags (password, read-only, multi-line, alignment, ...).
    WXSHARP_API wxsharp_handle wxsharp_textbox_create(wxsharp_handle parent, int id, const char* value, int style, long long token);
    WXSHARP_API int  wxsharp_textbox_get_value(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_textbox_set_value(wxsharp_handle ctrl, const char* value);
    WXSHARP_API void wxsharp_textbox_append(wxsharp_handle ctrl, const char* text);
    WXSHARP_API void wxsharp_textbox_clear(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textbox_select_all(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textbox_set_editable(wxsharp_handle ctrl, bool editable);
    WXSHARP_API void wxsharp_textbox_write(wxsharp_handle ctrl, const char* text); // insert at the caret
    WXSHARP_API int  wxsharp_textbox_length(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_textbox_get_insertion_point(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textbox_set_insertion_point(wxsharp_handle ctrl, int pos);
    WXSHARP_API void wxsharp_textbox_set_insertion_point_end(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textbox_get_selection(wxsharp_handle ctrl, int* from, int* to);
    WXSHARP_API void wxsharp_textbox_set_selection(wxsharp_handle ctrl, int from, int to);
    WXSHARP_API int  wxsharp_textbox_get_selected_text(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API int  wxsharp_textbox_line_count(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_textbox_line_length(wxsharp_handle ctrl, int line);
    WXSHARP_API int  wxsharp_textbox_get_line_text(wxsharp_handle ctrl, int line, char* buffer, int buffer_length);
    // Scrolls so the given character position is visible, without moving the caret.
    WXSHARP_API void wxsharp_textbox_show_position(wxsharp_handle ctrl, int position);

    // A character style, flattened so it can cross the ABI by value. `flags` holds the wxTextAttrFlags bits
    // saying which of the other fields are actually set; anything unmarked is left to inherit, which is how
    // wxTextAttr itself behaves.
    typedef struct wxsharp_text_attr
    {
        unsigned int flags;
        unsigned int text_colour;
        unsigned int background_colour;
        int alignment;
        int left_indent;
        int left_sub_indent;
        int right_indent;
        // The font, as a handle owned by the caller for the duration of the call. Flattening it into
        // scalars used to drop strikethrough, encoding and pixel sizes, which the flags above promise.
        wxsharp_handle font;
    } wxsharp_text_attr;

    WXSHARP_API bool wxsharp_textbox_is_modified(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textbox_mark_dirty(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textbox_discard_edits(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_textbox_set_modified(wxsharp_handle ctrl, bool modified);
    WXSHARP_API bool wxsharp_textbox_is_multiline(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_textbox_position_to_xy(wxsharp_handle ctrl, int position, int* x, int* y);
    WXSHARP_API int wxsharp_textbox_xy_to_position(wxsharp_handle ctrl, int x, int y);
    WXSHARP_API int wxsharp_textbox_hit_test(wxsharp_handle ctrl, int x, int y, int* position);
    WXSHARP_API bool wxsharp_textbox_load_file(wxsharp_handle ctrl, const char* path);
    WXSHARP_API bool wxsharp_textbox_save_file(wxsharp_handle ctrl, const char* path);
    WXSHARP_API bool wxsharp_textbox_set_style(wxsharp_handle ctrl, int start, int end,
                                               const wxsharp_text_attr* style);
    WXSHARP_API bool wxsharp_textbox_get_style(wxsharp_handle ctrl, int position, wxsharp_text_attr* style);
    WXSHARP_API bool wxsharp_textbox_set_default_style(wxsharp_handle ctrl, const wxsharp_text_attr* style);
    WXSHARP_API void wxsharp_textbox_get_default_style(wxsharp_handle ctrl, wxsharp_text_attr* style);

    // ---- Colour names ----
    // wxColour understands both the standard colour names and #RRGGBB notation.
    WXSHARP_API bool wxsharp_colour_parse(const char* text, unsigned int* argb);
    WXSHARP_API int wxsharp_colour_name(unsigned int argb, char* buffer, int buffer_length);
    WXSHARP_API unsigned int wxsharp_colour_change_lightness(unsigned int argb, int alpha);
    WXSHARP_API unsigned int wxsharp_colour_make_disabled(unsigned int argb, unsigned char brightness);
    WXSHARP_API unsigned int wxsharp_colour_make_grey(unsigned int argb);
    WXSHARP_API unsigned int wxsharp_colour_make_mono(unsigned int argb, bool on);
    WXSHARP_API double wxsharp_colour_luminance(unsigned int argb);
    WXSHARP_API unsigned char wxsharp_colour_alpha_blend(unsigned char foreground, unsigned char background,
                                                         double alpha);

    // ---- Check box -----------------------------------------------------------------------------------
    // style: WxSharp CheckBoxStyle (two-state or three-state).
    WXSHARP_API wxsharp_handle wxsharp_checkbox_create(wxsharp_handle parent, int id, const char* label, int style, long long token);
    WXSHARP_API bool wxsharp_checkbox_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_checkbox_set(wxsharp_handle ctrl, bool value);
    // 0 unchecked, 1 checked, 2 undetermined - wxCheckBoxState.
    WXSHARP_API int  wxsharp_checkbox_get_3state(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_checkbox_set_3state(wxsharp_handle ctrl, int state);
    WXSHARP_API bool wxsharp_checkbox_is_3state(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_checkbox_is_3rd_state_allowed_for_user(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_checkbox_set_transparent_part_colour(wxsharp_handle ctrl, unsigned int argb);

    // ---- Radio button --------------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_radio_create(wxsharp_handle parent, int id, const char* label, bool group_start, long long token);
    WXSHARP_API bool wxsharp_radio_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_radio_set(wxsharp_handle ctrl, bool value);
    WXSHARP_API wxsharp_handle wxsharp_radio_get_first(wxsharp_handle ctrl);
    WXSHARP_API wxsharp_handle wxsharp_radio_get_last(wxsharp_handle ctrl);
    WXSHARP_API wxsharp_handle wxsharp_radio_get_previous(wxsharp_handle ctrl);
    WXSHARP_API wxsharp_handle wxsharp_radio_get_next(wxsharp_handle ctrl);

    // ---- Slider --------------------------------------------------------------------------------------
    // style: WxSharp SliderStyle flags (orientation, labels, ticks, ...). The accessible key/notify behaviour
    // is implemented by the managed CustomSlider on top of this plain control.
    WXSHARP_API wxsharp_handle wxsharp_slider_create(wxsharp_handle parent, int id, int min_value, int max_value, int value, int style, long long token);
    WXSHARP_API int  wxsharp_slider_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_slider_set(wxsharp_handle ctrl, int value);
    WXSHARP_API int  wxsharp_slider_get_min(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_slider_get_max(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_slider_set_range(wxsharp_handle ctrl, int min_value, int max_value);

    // ---- Choice (drop-down) --------------------------------------------------------------------------
    // style: WxSharp ChoiceStyle (sorted or not).
    WXSHARP_API wxsharp_handle wxsharp_choice_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API void wxsharp_choice_append(wxsharp_handle ctrl, const char* item);
    WXSHARP_API void wxsharp_choice_insert(wxsharp_handle ctrl, const char* item, int index);
    WXSHARP_API void wxsharp_choice_delete(wxsharp_handle ctrl, int index);
    WXSHARP_API void wxsharp_choice_clear(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_choice_count(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_choice_get_string(wxsharp_handle ctrl, int index, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_choice_set_string(wxsharp_handle ctrl, int index, const char* text);
    WXSHARP_API int  wxsharp_choice_find_string(wxsharp_handle ctrl, const char* text);
    WXSHARP_API int  wxsharp_choice_get_selection(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_choice_set_selection(wxsharp_handle ctrl, int index);

    // ---- List box ------------------------------------------------------------------------------------
    // style: WxSharp ListBoxStyle flags (selection mode, scrollbars, sort).
    WXSHARP_API wxsharp_handle wxsharp_listbox_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API void wxsharp_listbox_append(wxsharp_handle ctrl, const char* item);
    WXSHARP_API void wxsharp_listbox_insert(wxsharp_handle ctrl, const char* item, int index);
    WXSHARP_API void wxsharp_listbox_delete(wxsharp_handle ctrl, int index);
    WXSHARP_API void wxsharp_listbox_clear(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_listbox_count(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_listbox_get_string(wxsharp_handle ctrl, int index, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_listbox_set_string(wxsharp_handle ctrl, int index, const char* text);
    WXSHARP_API int  wxsharp_listbox_find_string(wxsharp_handle ctrl, const char* text);
    WXSHARP_API int  wxsharp_listbox_get_selection(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_listbox_set_selection(wxsharp_handle ctrl, int index);
    // Multi-selection (list boxes created with Multiple/Extended).
    WXSHARP_API int  wxsharp_listbox_get_selections(wxsharp_handle ctrl, int* buffer, int buffer_length);
    WXSHARP_API void wxsharp_listbox_select(wxsharp_handle ctrl, int index, bool select);
    WXSHARP_API bool wxsharp_listbox_is_selected(wxsharp_handle ctrl, int index);
    WXSHARP_API void wxsharp_listbox_ensure_visible(wxsharp_handle ctrl, int index);
    WXSHARP_API void wxsharp_listbox_deselect_all(wxsharp_handle ctrl);

    // ---- Extended common controls -------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_togglebutton_create(wxsharp_handle parent, int id, const char* label,
                                                            long long token);
    WXSHARP_API bool wxsharp_togglebutton_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_togglebutton_set(wxsharp_handle ctrl, bool value);
    WXSHARP_API wxsharp_handle wxsharp_gauge_create(wxsharp_handle parent, int id, int range, int value,
                                                    bool vertical, long long token);
    WXSHARP_API int  wxsharp_gauge_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_gauge_set(wxsharp_handle ctrl, int value);
    WXSHARP_API int  wxsharp_gauge_get_range(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_gauge_set_range(wxsharp_handle ctrl, int range);
    WXSHARP_API void wxsharp_gauge_pulse(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_gauge_is_vertical(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_gauge_get_bezel_face(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_gauge_set_bezel_face(wxsharp_handle ctrl, int width);
    WXSHARP_API int  wxsharp_gauge_get_shadow_width(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_gauge_set_shadow_width(wxsharp_handle ctrl, int width);
    WXSHARP_API wxsharp_handle wxsharp_spinctrl_create(wxsharp_handle parent, int id, int min_value,
                                                       int max_value, int value, long long token);
    WXSHARP_API int  wxsharp_spinctrl_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_spinctrl_set(wxsharp_handle ctrl, int value);
    WXSHARP_API void wxsharp_spinctrl_set_range(wxsharp_handle ctrl, int min_value, int max_value);
    WXSHARP_API int  wxsharp_spinctrl_get_min(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_spinctrl_get_max(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_spinctrl_get_increment(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_spinctrl_set_increment(wxsharp_handle ctrl, int increment);
    WXSHARP_API int  wxsharp_spinctrl_get_base(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_spinctrl_set_base(wxsharp_handle ctrl, int base);
    WXSHARP_API int  wxsharp_spinctrl_get_text_value(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_spinctrl_set_text_value(wxsharp_handle ctrl, const char* value);
    WXSHARP_API void wxsharp_spinctrl_set_selection(wxsharp_handle ctrl, int from, int to);
    WXSHARP_API wxsharp_handle wxsharp_combobox_create(wxsharp_handle parent, int id, const char* value,
                                                       bool read_only, long long token);
    WXSHARP_API int  wxsharp_combobox_get_value(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_combobox_set_value(wxsharp_handle ctrl, const char* value);
    WXSHARP_API void wxsharp_combobox_append(wxsharp_handle ctrl, const char* value);
    WXSHARP_API void wxsharp_combobox_insert(wxsharp_handle ctrl, const char* value, int index);
    WXSHARP_API void wxsharp_combobox_delete(wxsharp_handle ctrl, int index);
    WXSHARP_API void wxsharp_combobox_clear(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_combobox_get_string(wxsharp_handle ctrl, int index, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_combobox_set_string(wxsharp_handle ctrl, int index, const char* text);
    WXSHARP_API int  wxsharp_combobox_find_string(wxsharp_handle ctrl, const char* text);
    WXSHARP_API int  wxsharp_combobox_count(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_combobox_get_selection(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_combobox_set_selection(wxsharp_handle ctrl, int selection);
    WXSHARP_API wxsharp_handle wxsharp_searchctrl_create(wxsharp_handle parent, int id, const char* value,
                                                         long long token);
    WXSHARP_API int  wxsharp_searchctrl_get_value(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_searchctrl_set_value(wxsharp_handle ctrl, const char* value);
    WXSHARP_API void wxsharp_searchctrl_show_cancel(wxsharp_handle ctrl, bool show);
    WXSHARP_API void wxsharp_searchctrl_show_search(wxsharp_handle ctrl, bool show);
    WXSHARP_API bool wxsharp_searchctrl_is_cancel_visible(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_searchctrl_is_search_visible(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_searchctrl_get_descriptive_text(wxsharp_handle ctrl, char* buffer,
                                                              int buffer_length);
    WXSHARP_API void wxsharp_searchctrl_set_descriptive_text(wxsharp_handle ctrl, const char* text);
    WXSHARP_API wxsharp_handle wxsharp_searchctrl_get_menu(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_searchctrl_set_menu(wxsharp_handle ctrl, wxsharp_handle menu);
    WXSHARP_API void wxsharp_searchctrl_set_search_bitmap(wxsharp_handle ctrl, wxsharp_handle bitmap);
    WXSHARP_API void wxsharp_searchctrl_set_search_menu_bitmap(wxsharp_handle ctrl, wxsharp_handle bitmap);
    WXSHARP_API void wxsharp_searchctrl_set_cancel_bitmap(wxsharp_handle ctrl, wxsharp_handle bitmap);
    WXSHARP_API wxsharp_handle wxsharp_checklistbox_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API void wxsharp_checklistbox_append(wxsharp_handle ctrl, const char* value);
    WXSHARP_API int  wxsharp_checklistbox_count(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_checklistbox_is_checked(wxsharp_handle ctrl, int index);
    WXSHARP_API void wxsharp_checklistbox_check(wxsharp_handle ctrl, int index, bool value);
    WXSHARP_API wxsharp_handle wxsharp_radiobox_create(wxsharp_handle parent, int id, const char* label,
                                                       const char* const* choices, int count, int columns,
                                                       long long token);
    WXSHARP_API int  wxsharp_radiobox_get_selection(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_radiobox_set_selection(wxsharp_handle ctrl, int selection);
    WXSHARP_API wxsharp_handle wxsharp_staticbox_create(wxsharp_handle parent, int id, const char* label,
                                                        long long token);
    WXSHARP_API wxsharp_handle wxsharp_staticline_create(wxsharp_handle parent, int id, bool vertical,
                                                         long long token);
    WXSHARP_API void wxsharp_staticbox_get_borders(wxsharp_handle ctrl, int* top, int* other);
    WXSHARP_API bool wxsharp_staticline_is_vertical(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_staticline_default_size(void);
    WXSHARP_API wxsharp_handle wxsharp_activity_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API void wxsharp_activity_start(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_activity_stop(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_activity_is_running(wxsharp_handle ctrl);
    WXSHARP_API wxsharp_handle wxsharp_spinctrldouble_create(wxsharp_handle parent, int id, double min_value,
                                                             double max_value, double value, double increment,
                                                             long long token);
    WXSHARP_API double wxsharp_spinctrldouble_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_spinctrldouble_set(wxsharp_handle ctrl, double value);
    WXSHARP_API double wxsharp_spinctrldouble_get_min(wxsharp_handle ctrl);
    WXSHARP_API double wxsharp_spinctrldouble_get_max(wxsharp_handle ctrl);
    WXSHARP_API double wxsharp_spinctrldouble_get_increment(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_spinctrldouble_set_increment(wxsharp_handle ctrl, double increment);
    WXSHARP_API unsigned int wxsharp_spinctrldouble_get_digits(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_spinctrldouble_set_digits(wxsharp_handle ctrl, unsigned int digits);
    WXSHARP_API void wxsharp_spinctrldouble_set_range(wxsharp_handle ctrl, double min_value, double max_value);
    WXSHARP_API int  wxsharp_spinctrldouble_get_text_value(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_spinctrldouble_set_text_value(wxsharp_handle ctrl, const char* value);
    WXSHARP_API wxsharp_handle wxsharp_scrollbar_create(wxsharp_handle parent, int id, bool vertical,
                                                        long long token);
    WXSHARP_API void wxsharp_scrollbar_set(wxsharp_handle ctrl, int position, int thumb_size,
                                           int range, int page_size);
    WXSHARP_API int wxsharp_scrollbar_get_position(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_scrollbar_set_ex(wxsharp_handle ctrl, int position, int thumb_size,
                                              int range, int page_size, bool refresh);
    WXSHARP_API void wxsharp_scrollbar_set_position(wxsharp_handle ctrl, int position);
    WXSHARP_API int  wxsharp_scrollbar_get_thumb_size(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_scrollbar_get_range(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_scrollbar_get_page_size(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_scrollbar_is_vertical(wxsharp_handle ctrl);
    WXSHARP_API wxsharp_handle wxsharp_hyperlink_create(wxsharp_handle parent, int id, const char* label,
                                                        const char* url, long long token);
    WXSHARP_API int wxsharp_hyperlink_get_url(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_hyperlink_set_url(wxsharp_handle ctrl, const char* url);
    WXSHARP_API bool wxsharp_hyperlink_get_visited(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_hyperlink_set_visited(wxsharp_handle ctrl, bool visited);
    WXSHARP_API unsigned int wxsharp_hyperlink_get_normal_colour(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_hyperlink_set_normal_colour(wxsharp_handle ctrl, unsigned int colour);
    WXSHARP_API unsigned int wxsharp_hyperlink_get_hover_colour(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_hyperlink_set_hover_colour(wxsharp_handle ctrl, unsigned int colour);
    WXSHARP_API unsigned int wxsharp_hyperlink_get_visited_colour(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_hyperlink_set_visited_colour(wxsharp_handle ctrl, unsigned int colour);
    WXSHARP_API wxsharp_handle wxsharp_datepicker_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API wxsharp_handle wxsharp_timepicker_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API void wxsharp_datetime_get(wxsharp_handle ctrl, int* year, int* month, int* day,
                                          int* hour, int* minute, int* second);
    WXSHARP_API void wxsharp_datetime_set(wxsharp_handle ctrl, int year, int month, int day,
                                          int hour, int minute, int second);
    WXSHARP_API bool wxsharp_datepicker_get_range(wxsharp_handle ctrl, int* y1, int* m1, int* d1,
                                                  int* y2, int* m2, int* d2);
    WXSHARP_API void wxsharp_datepicker_set_range(wxsharp_handle ctrl, int y1, int m1, int d1,
                                                  int y2, int m2, int d2);
    WXSHARP_API void wxsharp_datepicker_set_null_text(wxsharp_handle ctrl, const char* text);

    // ---- Containers ---------------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_scrolled_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API void wxsharp_scrolled_set_rate(wxsharp_handle ctrl, int x_step, int y_step);
    WXSHARP_API void wxsharp_scrolled_scroll(wxsharp_handle ctrl, int x, int y);
    WXSHARP_API void wxsharp_scrolled_get_view_start(wxsharp_handle ctrl, int* x, int* y);
    WXSHARP_API void wxsharp_scrolled_set_scrollbars(wxsharp_handle ctrl, int pixels_x, int pixels_y,
                                                     int units_x, int units_y, int pos_x, int pos_y,
                                                     bool no_refresh);
    WXSHARP_API void wxsharp_scrolled_enable_scrolling(wxsharp_handle ctrl, bool x, bool y);
    WXSHARP_API void wxsharp_scrolled_show_scrollbars(wxsharp_handle ctrl, int x, int y);
    WXSHARP_API void wxsharp_scrolled_get_pixels_per_unit(wxsharp_handle ctrl, int* x, int* y);
    WXSHARP_API void wxsharp_scrolled_set_target_window(wxsharp_handle ctrl, wxsharp_handle target);
    WXSHARP_API void wxsharp_scrolled_set_scroll_page_size(wxsharp_handle ctrl, int orientation, int size);
    WXSHARP_API int  wxsharp_scrolled_get_scroll_page_size(wxsharp_handle ctrl, int orientation);
    WXSHARP_API wxsharp_handle wxsharp_splitter_create(wxsharp_handle parent, int id, bool vertical,
                                                       long long token);
    WXSHARP_API bool wxsharp_splitter_split(wxsharp_handle ctrl, wxsharp_handle first,
                                            wxsharp_handle second, int position);
    WXSHARP_API bool wxsharp_splitter_unsplit(wxsharp_handle ctrl, wxsharp_handle remove);
    WXSHARP_API int  wxsharp_splitter_get_position(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_splitter_set_position(wxsharp_handle ctrl, int position);
    WXSHARP_API wxsharp_handle wxsharp_notebook_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API bool wxsharp_notebook_add_page(wxsharp_handle ctrl, wxsharp_handle page, const char* text,
                                               bool select);
    WXSHARP_API bool wxsharp_notebook_delete_page(wxsharp_handle ctrl, int page);
    WXSHARP_API int  wxsharp_notebook_count(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_notebook_get_selection(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_notebook_set_selection(wxsharp_handle ctrl, int page);
    WXSHARP_API int  wxsharp_notebook_get_page_text(wxsharp_handle ctrl, int page, char* buffer,
                                                    int buffer_length);
    WXSHARP_API bool wxsharp_notebook_set_page_text(wxsharp_handle ctrl, int page, const char* text);
    WXSHARP_API wxsharp_handle wxsharp_simplebook_create(wxsharp_handle parent, int id, long long token);

    // ---- Data controls -------------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_listctrl_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API int  wxsharp_listctrl_insert_column(wxsharp_handle ctrl, int column, const char* heading,
                                                    int width);
    WXSHARP_API long long wxsharp_listctrl_insert_item(wxsharp_handle ctrl, long long index,
                                                       const char* text);
    WXSHARP_API bool wxsharp_listctrl_set_item(wxsharp_handle ctrl, long long item, int column,
                                               const char* text);
    WXSHARP_API int  wxsharp_listctrl_get_item(wxsharp_handle ctrl, long long item, int column,
                                               char* buffer, int buffer_length);
    WXSHARP_API long long wxsharp_listctrl_count(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_listctrl_delete_item(wxsharp_handle ctrl, long long item);
    WXSHARP_API void wxsharp_listctrl_clear(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_listctrl_select(wxsharp_handle ctrl, long long item, bool select);
    WXSHARP_API bool wxsharp_listctrl_is_selected(wxsharp_handle ctrl, long long item);
    WXSHARP_API int  wxsharp_listctrl_column_count(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_listctrl_delete_column(wxsharp_handle ctrl, int column);
    WXSHARP_API void wxsharp_listctrl_clear_columns(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_listctrl_get_column_width(wxsharp_handle ctrl, int column);
    // A negative width auto-sizes: -1 to the widest cell, -2 to the header.
    WXSHARP_API bool wxsharp_listctrl_set_column_width(wxsharp_handle ctrl, int column, int width);
    WXSHARP_API int  wxsharp_listctrl_get_column_heading(wxsharp_handle ctrl, int column, char* buffer,
                                                         int buffer_length);
    WXSHARP_API bool wxsharp_listctrl_set_column_heading(wxsharp_handle ctrl, int column, const char* heading);
    WXSHARP_API void wxsharp_listctrl_ensure_visible(wxsharp_handle ctrl, long long item);
    WXSHARP_API long long wxsharp_listctrl_get_focused(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_listctrl_set_focused(wxsharp_handle ctrl, long long item);
    WXSHARP_API int  wxsharp_listctrl_selected_count(wxsharp_handle ctrl);
    // Walks the selection: pass -1 to start, then the previous result. Returns -1 when there are no more.
    WXSHARP_API long long wxsharp_listctrl_next_selected(wxsharp_handle ctrl, long long after);
    // Virtual mode: tell the control how many rows there are and it will ask for each one as it draws.
    WXSHARP_API void wxsharp_listctrl_set_item_count(wxsharp_handle ctrl, long long count);
    WXSHARP_API void wxsharp_listctrl_refresh_item(wxsharp_handle ctrl, long long item);
    WXSHARP_API void wxsharp_listctrl_refresh_items(wxsharp_handle ctrl, long long from, long long to);
    WXSHARP_API wxsharp_handle wxsharp_treectrl_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API long long wxsharp_tree_add_root(wxsharp_handle ctrl, const char* text);
    WXSHARP_API long long wxsharp_tree_append(wxsharp_handle ctrl, long long parent, const char* text);
    WXSHARP_API void wxsharp_tree_delete(wxsharp_handle ctrl, long long item);
    WXSHARP_API void wxsharp_tree_delete_all(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_tree_get_text(wxsharp_handle ctrl, long long item, char* buffer,
                                           int buffer_length);
    WXSHARP_API void wxsharp_tree_set_text(wxsharp_handle ctrl, long long item, const char* text);
    WXSHARP_API void wxsharp_tree_expand(wxsharp_handle ctrl, long long item, bool expand);
    WXSHARP_API bool wxsharp_tree_is_expanded(wxsharp_handle ctrl, long long item);
    WXSHARP_API void wxsharp_tree_select(wxsharp_handle ctrl, long long item);
    WXSHARP_API long long wxsharp_tree_get_selection(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_tree_get_count(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_tree_expand_all(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_tree_collapse_all(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_tree_item_has_children(wxsharp_handle ctrl, long long item);
    WXSHARP_API void wxsharp_tree_unselect(wxsharp_handle ctrl);
    WXSHARP_API long long wxsharp_tree_get_root(wxsharp_handle ctrl);
    WXSHARP_API long long wxsharp_tree_get_parent(wxsharp_handle ctrl, long long item);
    WXSHARP_API long long wxsharp_tree_get_first_child(wxsharp_handle ctrl, long long item);
    WXSHARP_API long long wxsharp_tree_get_next_sibling(wxsharp_handle ctrl, long long item);
    WXSHARP_API long long wxsharp_tree_get_prev_sibling(wxsharp_handle ctrl, long long item);
    WXSHARP_API int  wxsharp_tree_child_count(wxsharp_handle ctrl, long long item, bool recursive);
    WXSHARP_API void wxsharp_tree_ensure_visible(wxsharp_handle ctrl, long long item);
    WXSHARP_API long long wxsharp_tree_insert(wxsharp_handle ctrl, long long parent, int position,
                                              const char* text);
    WXSHARP_API void wxsharp_tree_sort_children(wxsharp_handle ctrl, long long item);
    WXSHARP_API wxsharp_handle wxsharp_grid_create(wxsharp_handle parent, int id, int rows, int columns,
                                                   long long token);
    WXSHARP_API int  wxsharp_grid_rows(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_grid_columns(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_grid_append_rows(wxsharp_handle ctrl, int count);
    WXSHARP_API bool wxsharp_grid_append_columns(wxsharp_handle ctrl, int count);
    WXSHARP_API bool wxsharp_grid_delete_rows(wxsharp_handle ctrl, int position, int count);
    WXSHARP_API bool wxsharp_grid_delete_columns(wxsharp_handle ctrl, int position, int count);
    WXSHARP_API int  wxsharp_grid_get_value(wxsharp_handle ctrl, int row, int column, char* buffer,
                                            int buffer_length);
    WXSHARP_API void wxsharp_grid_set_value(wxsharp_handle ctrl, int row, int column, const char* value);
    WXSHARP_API void wxsharp_grid_set_row_label(wxsharp_handle ctrl, int row, const char* value);
    WXSHARP_API void wxsharp_grid_set_column_label(wxsharp_handle ctrl, int column, const char* value);
    WXSHARP_API wxsharp_handle wxsharp_dataviewlist_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API void wxsharp_dataviewlist_append_text_column(wxsharp_handle ctrl, const char* label,
                                                             int width, bool editable);
    WXSHARP_API void wxsharp_dataviewlist_append_row(wxsharp_handle ctrl, const char* const* values,
                                                     int count);
    WXSHARP_API int wxsharp_dataviewlist_count(wxsharp_handle ctrl);
    WXSHARP_API int wxsharp_dataviewlist_get_value(wxsharp_handle ctrl, int row, int column,
                                                   char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_dataviewlist_set_value(wxsharp_handle ctrl, int row, int column,
                                                    const char* value);
    WXSHARP_API void wxsharp_dataviewlist_delete_row(wxsharp_handle ctrl, int row);
    WXSHARP_API void wxsharp_dataviewlist_clear(wxsharp_handle ctrl);
    WXSHARP_API int wxsharp_dataviewlist_get_selection(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_dataviewlist_set_selection(wxsharp_handle ctrl, int row);
    WXSHARP_API wxsharp_handle wxsharp_dataviewtree_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API long long wxsharp_dataviewtree_append_container(wxsharp_handle ctrl, long long parent,
                                                                const char* text);
    WXSHARP_API long long wxsharp_dataviewtree_append_item(wxsharp_handle ctrl, long long parent,
                                                           const char* text);
    WXSHARP_API int wxsharp_dataviewtree_get_text(wxsharp_handle ctrl, long long item, char* buffer,
                                                  int buffer_length);
    WXSHARP_API void wxsharp_dataviewtree_set_text(wxsharp_handle ctrl, long long item, const char* text);
    WXSHARP_API void wxsharp_dataviewtree_delete(wxsharp_handle ctrl, long long item);
    WXSHARP_API void wxsharp_dataviewtree_clear(wxsharp_handle ctrl);
    WXSHARP_API long long wxsharp_dataviewtree_get_selection(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_dataviewtree_set_selection(wxsharp_handle ctrl, long long item);

    // ---- Menus and frame chrome ----------------------------------------------------------------------
    // Menu items are opaque wxMenuItem handles so the managed MenuItem can carry label, help, kind and
    // state without addressing items by loose integer ID. append/insert return the created item.
    WXSHARP_API wxsharp_handle wxsharp_menu_create();
    WXSHARP_API void wxsharp_menu_destroy(wxsharp_handle menu);
    WXSHARP_API wxsharp_handle wxsharp_menu_append(wxsharp_handle menu, int id, const char* text,
                                                   const char* help, int kind);
    WXSHARP_API wxsharp_handle wxsharp_menu_insert(wxsharp_handle menu, int position, int id, const char* text,
                                                   const char* help, int kind);
    WXSHARP_API wxsharp_handle wxsharp_menu_append_submenu(wxsharp_handle menu, int id, const char* text,
                                                           wxsharp_handle submenu, const char* help);
    WXSHARP_API wxsharp_handle wxsharp_menu_insert_submenu(wxsharp_handle menu, int position, int id,
                                                           const char* text, wxsharp_handle submenu,
                                                           const char* help);
    WXSHARP_API wxsharp_handle wxsharp_menu_append_separator(wxsharp_handle menu);
    WXSHARP_API wxsharp_handle wxsharp_menu_insert_separator(wxsharp_handle menu, int position);
    WXSHARP_API int  wxsharp_menu_count(wxsharp_handle menu);
    WXSHARP_API wxsharp_handle wxsharp_menu_item_at(wxsharp_handle menu, int position);
    WXSHARP_API wxsharp_handle wxsharp_menu_find_item(wxsharp_handle menu, int id); // searches submenus too
    // Detaches the item from the menu without deleting it; the caller then owns it.
    WXSHARP_API bool wxsharp_menu_remove(wxsharp_handle menu, wxsharp_handle item);
    // Detaches and deletes the item (and any submenu it owns).
    WXSHARP_API bool wxsharp_menu_delete(wxsharp_handle menu, wxsharp_handle item);
    WXSHARP_API void wxsharp_menu_enable(wxsharp_handle menu, int id, bool enable);
    WXSHARP_API void wxsharp_menu_check(wxsharp_handle menu, int id, bool check);
    WXSHARP_API bool wxsharp_menu_is_checked(wxsharp_handle menu, int id);
    WXSHARP_API int  wxsharp_menu_get_title(wxsharp_handle menu, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_menu_set_title(wxsharp_handle menu, const char* title);

    // ---- Menu items -----------------------------------------------------------------------------------
    WXSHARP_API int  wxsharp_menuitem_get_id(wxsharp_handle item);
    WXSHARP_API int  wxsharp_menuitem_get_kind(wxsharp_handle item); // 0 normal, 1 check, 2 radio, 3 separator
    WXSHARP_API int  wxsharp_menuitem_get_label(wxsharp_handle item, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_menuitem_set_label(wxsharp_handle item, const char* label);
    WXSHARP_API int  wxsharp_menuitem_get_help(wxsharp_handle item, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_menuitem_set_help(wxsharp_handle item, const char* help);
    WXSHARP_API bool wxsharp_menuitem_is_enabled(wxsharp_handle item);
    WXSHARP_API void wxsharp_menuitem_enable(wxsharp_handle item, bool enable);
    WXSHARP_API bool wxsharp_menuitem_is_checked(wxsharp_handle item);
    WXSHARP_API void wxsharp_menuitem_check(wxsharp_handle item, bool check);
    WXSHARP_API bool wxsharp_menuitem_is_checkable(wxsharp_handle item);
    WXSHARP_API wxsharp_handle wxsharp_menuitem_get_submenu(wxsharp_handle item);
    WXSHARP_API void wxsharp_menuitem_set_bitmap(wxsharp_handle item, wxsharp_handle bitmap);

    // ---- Menu bar -------------------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_menubar_create();
    WXSHARP_API void wxsharp_menubar_destroy(wxsharp_handle menu_bar);
    WXSHARP_API bool wxsharp_menubar_append(wxsharp_handle menu_bar, wxsharp_handle menu, const char* title);
    WXSHARP_API bool wxsharp_menubar_insert(wxsharp_handle menu_bar, int position, wxsharp_handle menu,
                                            const char* title);
    WXSHARP_API wxsharp_handle wxsharp_menubar_remove(wxsharp_handle menu_bar, int position);
    WXSHARP_API int  wxsharp_menubar_count(wxsharp_handle menu_bar);
    WXSHARP_API wxsharp_handle wxsharp_menubar_menu_at(wxsharp_handle menu_bar, int position);
    WXSHARP_API void wxsharp_menubar_enable_top(wxsharp_handle menu_bar, int position, bool enable);
    WXSHARP_API int  wxsharp_menubar_get_label_top(wxsharp_handle menu_bar, int position, char* buffer,
                                                   int buffer_length);
    WXSHARP_API void wxsharp_menubar_set_label_top(wxsharp_handle menu_bar, int position, const char* label);
    WXSHARP_API wxsharp_handle wxsharp_menubar_find_item(wxsharp_handle menu_bar, int id);
    WXSHARP_API void wxsharp_frame_set_menubar(wxsharp_handle frame, wxsharp_handle menu_bar);
    // Sends update-UI events to every item in the frame's menu bar. wxWidgets does this automatically when
    // a menu is about to open; this is for refreshing without waiting for that.
    WXSHARP_API void wxsharp_frame_update_menus(wxsharp_handle frame);

    // Shows a menu at a client-relative point (-1,-1 uses the current pointer position) and returns after
    // the menu is dismissed. Any command it produces is delivered to the window MENU handler.
    WXSHARP_API bool wxsharp_window_popup_menu(wxsharp_handle window, wxsharp_handle menu, int x, int y);
    WXSHARP_API wxsharp_handle wxsharp_statusbar_create(wxsharp_handle frame, int fields, long long token);
    WXSHARP_API void wxsharp_statusbar_set_text(wxsharp_handle status, const char* text, int field);
    WXSHARP_API int wxsharp_statusbar_get_text(wxsharp_handle status, int field, char* buffer, int buffer_length);
    WXSHARP_API wxsharp_handle wxsharp_toolbar_create(wxsharp_handle frame, long long token);
    WXSHARP_API void wxsharp_toolbar_add_tool(wxsharp_handle toolbar, int id, const char* label, const char* help, int kind);
    WXSHARP_API void wxsharp_toolbar_add_separator(wxsharp_handle toolbar);
    WXSHARP_API void wxsharp_toolbar_realize(wxsharp_handle toolbar);
    WXSHARP_API void wxsharp_toolbar_enable(wxsharp_handle toolbar, int id, bool enable);
    WXSHARP_API void wxsharp_toolbar_toggle(wxsharp_handle toolbar, int id, bool toggle);
    // ---- Accelerators ---------------------------------------------------------------------------------
    // Accelerator tables apply to any window, not just frames; passing count 0 clears the table.
    WXSHARP_API void wxsharp_window_set_accelerators(wxsharp_handle window,
                                                     const wxsharp_accelerator* entries, int count);
    // Parses a wx accelerator string ("Ctrl+Shift+P", "Alt+F4", "F11") into modifiers and a key code.
    // Returns false when the string is not a valid accelerator.
    WXSHARP_API bool wxsharp_accelerator_parse(const char* text, int* modifiers, int* key_code);
    // Formats modifiers and a key code back into a wx accelerator string.
    WXSHARP_API int  wxsharp_accelerator_format(int modifiers, int key_code, char* buffer, int buffer_length);
    // Allocates an application-unique window/command ID, as wxNewId does. release() returns it to the pool.
    WXSHARP_API int  wxsharp_new_id();
    WXSHARP_API void wxsharp_release_id(int id);
    // The platform value for a wxID_* stock identifier, looked up by the managed StandardId table.
    WXSHARP_API int  wxsharp_stock_id(int which);

    // ---- Timers --------------------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_timer_create(wxsharp_handle owner, int id, long long owner_token);
    WXSHARP_API void wxsharp_timer_destroy(wxsharp_handle timer);
    WXSHARP_API bool wxsharp_timer_start(wxsharp_handle timer, int milliseconds, bool one_shot);
    WXSHARP_API bool wxsharp_timer_start_once(wxsharp_handle timer, int milliseconds);
    WXSHARP_API void wxsharp_timer_stop(wxsharp_handle timer);
    WXSHARP_API bool wxsharp_timer_is_running(wxsharp_handle timer);
    WXSHARP_API bool wxsharp_timer_is_one_shot(wxsharp_handle timer);
    WXSHARP_API int wxsharp_timer_get_interval(wxsharp_handle timer);
    WXSHARP_API void wxsharp_timer_notify(wxsharp_handle timer);
    WXSHARP_API void wxsharp_timer_set_owner(wxsharp_handle timer, wxsharp_handle owner, int id,
                                             long long owner_token);

    // ---- Images and bitmap controls ------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_image_load(const char* path);
    WXSHARP_API void wxsharp_image_destroy(wxsharp_handle image);
    WXSHARP_API int wxsharp_image_width(wxsharp_handle image);
    WXSHARP_API int wxsharp_image_height(wxsharp_handle image);
    WXSHARP_API bool wxsharp_image_save(wxsharp_handle image, const char* path);
    WXSHARP_API wxsharp_handle wxsharp_bitmap_load(const char* path);
    WXSHARP_API wxsharp_handle wxsharp_bitmap_from_image(wxsharp_handle image);
    WXSHARP_API void wxsharp_bitmap_destroy(wxsharp_handle bitmap);
    WXSHARP_API int wxsharp_bitmap_width(wxsharp_handle bitmap);
    WXSHARP_API int wxsharp_bitmap_height(wxsharp_handle bitmap);
    WXSHARP_API wxsharp_handle wxsharp_staticbitmap_create(wxsharp_handle parent, int id,
                                                           wxsharp_handle bitmap, long long token);
    WXSHARP_API void wxsharp_staticbitmap_set(wxsharp_handle ctrl, wxsharp_handle bitmap);
    WXSHARP_API wxsharp_handle wxsharp_staticbitmap_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_staticbitmap_set_icon(wxsharp_handle ctrl, wxsharp_handle icon);
    WXSHARP_API wxsharp_handle wxsharp_staticbitmap_get_icon(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_staticbitmap_set_scale_mode(wxsharp_handle ctrl, int mode);
    WXSHARP_API int wxsharp_staticbitmap_get_scale_mode(wxsharp_handle ctrl);
    WXSHARP_API wxsharp_handle wxsharp_bitmapbutton_create(wxsharp_handle parent, int id,
                                                           wxsharp_handle bitmap, long long token);
    WXSHARP_API wxsharp_handle wxsharp_bitmapbutton_new_close(wxsharp_handle parent, int id,
                                                              const char* name, long long token);
    WXSHARP_API void wxsharp_bitmapbutton_set_margins(wxsharp_handle ctrl, int x, int y);
    WXSHARP_API int wxsharp_bitmapbutton_get_margin_x(wxsharp_handle ctrl);
    WXSHARP_API int wxsharp_bitmapbutton_get_margin_y(wxsharp_handle ctrl);
    WXSHARP_API wxsharp_handle wxsharp_icon_load(const char* path);
    WXSHARP_API void wxsharp_icon_destroy(wxsharp_handle icon);
    WXSHARP_API void wxsharp_frame_set_icon(wxsharp_handle frame, wxsharp_handle icon);

    // ---- The rest of wxFrame / wxTopLevelWindow ----
    WXSHARP_API void wxsharp_frame_iconize(wxsharp_handle frame, bool iconize);
    WXSHARP_API bool wxsharp_frame_is_iconized(wxsharp_handle frame);
    WXSHARP_API void wxsharp_frame_maximize(wxsharp_handle frame, bool maximize);
    WXSHARP_API bool wxsharp_frame_is_maximized(wxsharp_handle frame);
    WXSHARP_API bool wxsharp_frame_is_always_maximized(wxsharp_handle frame);
    WXSHARP_API void wxsharp_frame_restore(wxsharp_handle frame);
    WXSHARP_API bool wxsharp_frame_is_active(wxsharp_handle frame);
    WXSHARP_API bool wxsharp_frame_show_full_screen(wxsharp_handle frame, bool show, int style);
    WXSHARP_API bool wxsharp_frame_is_full_screen(wxsharp_handle frame);
    WXSHARP_API bool wxsharp_frame_enable_full_screen_view(wxsharp_handle frame, bool enable, int style);
    WXSHARP_API void wxsharp_frame_show_without_activating(wxsharp_handle frame);
    WXSHARP_API void wxsharp_frame_request_user_attention(wxsharp_handle frame, int flags);
    WXSHARP_API bool wxsharp_frame_enable_close_button(wxsharp_handle frame, bool enable);
    WXSHARP_API bool wxsharp_frame_enable_maximize_button(wxsharp_handle frame, bool enable);
    WXSHARP_API bool wxsharp_frame_enable_minimize_button(wxsharp_handle frame, bool enable);
    WXSHARP_API void wxsharp_frame_centre_on_screen(wxsharp_handle frame, int direction);
    WXSHARP_API int wxsharp_frame_get_content_protection(wxsharp_handle frame);
    WXSHARP_API bool wxsharp_frame_set_content_protection(wxsharp_handle frame, int protection);
    WXSHARP_API void wxsharp_frame_set_represented_filename(wxsharp_handle frame, const char* path);
    WXSHARP_API void wxsharp_frame_set_window_modality(wxsharp_handle frame, int modality);
    WXSHARP_API void wxsharp_frame_get_default_size(int* width, int* height);
    WXSHARP_API wxsharp_handle wxsharp_frame_get_default_item(wxsharp_handle frame);
    WXSHARP_API wxsharp_handle wxsharp_frame_set_default_item(wxsharp_handle frame, wxsharp_handle window);
    WXSHARP_API wxsharp_handle wxsharp_frame_get_icon(wxsharp_handle frame);
    WXSHARP_API void wxsharp_frame_set_icons(wxsharp_handle frame, wxsharp_handle* icons, int count);
    WXSHARP_API int wxsharp_frame_get_icons(wxsharp_handle frame);
    WXSHARP_API wxsharp_handle wxsharp_frame_get_icon_at(int index);
    WXSHARP_API wxsharp_handle wxsharp_frame_get_menubar(wxsharp_handle frame);
    WXSHARP_API wxsharp_handle wxsharp_frame_find_item_in_menubar(wxsharp_handle frame, int id);
    WXSHARP_API wxsharp_handle wxsharp_frame_get_statusbar(wxsharp_handle frame);
    WXSHARP_API void wxsharp_frame_set_statusbar(wxsharp_handle frame, wxsharp_handle bar);
    WXSHARP_API wxsharp_handle wxsharp_frame_create_statusbar(wxsharp_handle frame, int fields, int style,
                                                              int id, long long token);
    WXSHARP_API void wxsharp_frame_set_status_text(wxsharp_handle frame, const char* text, int field);
    WXSHARP_API void wxsharp_frame_push_status_text(wxsharp_handle frame, const char* text, int field);
    WXSHARP_API void wxsharp_frame_pop_status_text(wxsharp_handle frame, int field);
    WXSHARP_API void wxsharp_frame_set_status_widths(wxsharp_handle frame, const int* widths, int count);
    WXSHARP_API int wxsharp_frame_get_status_bar_pane(wxsharp_handle frame);
    WXSHARP_API void wxsharp_frame_set_status_bar_pane(wxsharp_handle frame, int pane);
    WXSHARP_API wxsharp_handle wxsharp_frame_get_toolbar(wxsharp_handle frame);
    WXSHARP_API void wxsharp_frame_set_toolbar(wxsharp_handle frame, wxsharp_handle bar);
    WXSHARP_API wxsharp_handle wxsharp_frame_create_toolbar(wxsharp_handle frame, int style, int id,
                                                            long long token);
    WXSHARP_API void wxsharp_frame_use_native_statusbar(bool native);
    WXSHARP_API bool wxsharp_frame_uses_native_statusbar(void);
    WXSHARP_API int wxsharp_frame_save_geometry(wxsharp_handle frame, char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_frame_restore_to_geometry(wxsharp_handle frame, const char* text);

    // ---- The wxWidgets free functions ----
    WXSHARP_API bool wxsharp_launch_default_browser(const char* url, int flags);
    WXSHARP_API bool wxsharp_launch_default_application(const char* path, int flags);
    WXSHARP_API long long wxsharp_execute(const char* command, int flags);
    WXSHARP_API long long wxsharp_shell(const char* command);
    WXSHARP_API void wxsharp_bell(void);
    WXSHARP_API bool wxsharp_get_key_state(int key);
    WXSHARP_API void wxsharp_get_mouse_position(int* x, int* y);
    WXSHARP_API void wxsharp_get_mouse_state(int* x, int* y, int* buttons, int* modifiers);
    WXSHARP_API int wxsharp_get_user_id(char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_get_user_name(char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_get_host_name(char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_get_full_host_name(char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_get_email_address(char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_get_home_dir(char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_get_os_description(char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_get_os_version(int* major, int* minor, int* micro);
    WXSHARP_API bool wxsharp_check_os_version(int major, int minor, int micro);
    WXSHARP_API bool wxsharp_is_platform_64bit(void);
    WXSHARP_API bool wxsharp_is_platform_little_endian(void);
    WXSHARP_API int wxsharp_get_cpu_architecture_name(char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_get_native_cpu_architecture_name(char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_get_library_version(char* buffer, int buffer_length);
    WXSHARP_API unsigned int wxsharp_get_process_id(void);
    WXSHARP_API long long wxsharp_get_free_memory(void);
    WXSHARP_API bool wxsharp_get_disk_space(const char* path, long long* total, long long* free_space);
    WXSHARP_API int wxsharp_get_env(const char* name, char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_set_env(const char* name, const char* value);
    WXSHARP_API bool wxsharp_unset_env(const char* name);
    WXSHARP_API void wxsharp_sleep(int seconds);
    WXSHARP_API void wxsharp_milli_sleep(unsigned long milliseconds);
    WXSHARP_API void wxsharp_micro_sleep(unsigned long microseconds);
    WXSHARP_API wxsharp_handle wxsharp_find_window_by_name(const char* name, wxsharp_handle parent);
    WXSHARP_API wxsharp_handle wxsharp_find_window_by_label(const char* label, wxsharp_handle parent);
    WXSHARP_API wxsharp_handle wxsharp_find_window_at_point(int x, int y);
    WXSHARP_API wxsharp_handle wxsharp_get_active_window(void);
    WXSHARP_API void wxsharp_enable_top_level_windows(bool enable);
    WXSHARP_API wxsharp_handle wxsharp_window_disabler_begin(wxsharp_handle skip);
    WXSHARP_API void wxsharp_window_disabler_end(wxsharp_handle scope);
    WXSHARP_API int wxsharp_strip_menu_codes(const char* text, char* buffer, int buffer_length);

    // ---- Language and translation ----
    // One entry from the wxWidgets language database, flattened so it can cross the ABI by value.
    typedef struct wxsharp_language_info
    {
        int language;
        int layout_direction;
        unsigned int win_lang;
        unsigned int win_sublang;
        char locale_tag[64];
        char canonical_name[64];
        char canonical_ref[64];
        char description[128];
        char description_native[128];
    } wxsharp_language_info;

    WXSHARP_API wxsharp_handle wxsharp_locale_create(int language, int flags);
    WXSHARP_API void wxsharp_locale_destroy(wxsharp_handle locale);
    WXSHARP_API bool wxsharp_locale_is_ok(wxsharp_handle locale);
    WXSHARP_API int wxsharp_locale_get_language(wxsharp_handle locale);
    WXSHARP_API int wxsharp_locale_get_name(wxsharp_handle locale, char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_locale_get_canonical_name(wxsharp_handle locale, char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_locale_get_locale(wxsharp_handle locale, char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_locale_get_sys_name(wxsharp_handle locale, char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_locale_add_catalog(wxsharp_handle locale, const char* domain,
                                                int msg_id_language);
    WXSHARP_API bool wxsharp_locale_is_loaded(wxsharp_handle locale, const char* domain);
    WXSHARP_API int wxsharp_locale_get_string(wxsharp_handle locale, const char* original,
                                              const char* domain, char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_locale_get_string_plural(wxsharp_handle locale, const char* singular,
                                                     const char* plural, unsigned int n, const char* domain,
                                                     char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_locale_get_header_value(wxsharp_handle locale, const char* header,
                                                    const char* domain, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_locale_add_catalog_lookup_path_prefix(const char* prefix);
    WXSHARP_API int wxsharp_locale_get_system_language(void);
    WXSHARP_API int wxsharp_locale_get_system_encoding_name(char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_locale_is_available(int language);
    WXSHARP_API int wxsharp_locale_get_language_name(int language, char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_locale_get_language_canonical_name(int language, char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_locale_get_language_info(int language, wxsharp_language_info* info);
    WXSHARP_API bool wxsharp_locale_find_language_info(const char* text, wxsharp_language_info* info);
    WXSHARP_API bool wxsharp_locale_find_language_info_by_tag(const char* tag,
                                                              wxsharp_language_info* info);
    WXSHARP_API int wxsharp_locale_get_info(int index, int category, char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_locale_get_os_info(int index, int category, char* buffer, int buffer_length);

    WXSHARP_API wxsharp_handle wxsharp_translations_get(void);
    WXSHARP_API wxsharp_handle wxsharp_translations_create(void);
    WXSHARP_API void wxsharp_translations_set(wxsharp_handle translations);
    WXSHARP_API void wxsharp_translations_set_language(wxsharp_handle translations, int language);
    WXSHARP_API void wxsharp_translations_set_language_named(wxsharp_handle translations,
                                                             const char* language);
    WXSHARP_API bool wxsharp_translations_add_catalog(wxsharp_handle translations, const char* domain,
                                                      int msg_id_language);
    WXSHARP_API bool wxsharp_translations_add_available_catalog(wxsharp_handle translations,
                                                                const char* domain, int msg_id_language);
    WXSHARP_API bool wxsharp_translations_add_std_catalog(wxsharp_handle translations);
    WXSHARP_API bool wxsharp_translations_is_loaded(wxsharp_handle translations, const char* domain);
    WXSHARP_API int wxsharp_translations_available_count(wxsharp_handle translations, const char* domain);
    WXSHARP_API int wxsharp_translations_available_at(int index, char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_translations_get_best_translation(wxsharp_handle translations,
                                                              const char* domain, int msg_id_language,
                                                              char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_translations_get_best_available_translation(wxsharp_handle translations,
                                                                        const char* domain, char* buffer,
                                                                        int buffer_length);
    WXSHARP_API int wxsharp_translations_get_translated_string(wxsharp_handle translations,
                                                               const char* original, const char* domain,
                                                               const char* context, char* buffer,
                                                               int buffer_length);
    WXSHARP_API int wxsharp_translations_get_translated_string_plural(wxsharp_handle translations,
                                                                      const char* original, unsigned int n,
                                                                      const char* domain, const char* context,
                                                                      char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_translations_get_header_value(wxsharp_handle translations, const char* header,
                                                          const char* domain, char* buffer,
                                                          int buffer_length);
    WXSHARP_API void wxsharp_translations_add_lookup_prefix(const char* prefix);
    WXSHARP_API int wxsharp_get_translation(const char* original, const char* domain, const char* context,
                                            char* buffer, int buffer_length);
    WXSHARP_API int wxsharp_get_translation_plural(const char* singular, const char* plural, unsigned int n,
                                                    const char* domain, const char* context, char* buffer,
                                                    int buffer_length);
    WXSHARP_API void wxsharp_begin_busy_cursor();
    WXSHARP_API void wxsharp_end_busy_cursor();
    WXSHARP_API wxsharp_handle wxsharp_progress_create(wxsharp_handle parent, const char* title,
                                                       const char* message, int maximum, int style,
                                                       long long token);
    WXSHARP_API wxsharp_handle wxsharp_custom_progress_create(wxsharp_handle parent, const char* title,
                                                              const char* message, int maximum, int style,
                                                              long long token);
    WXSHARP_API bool wxsharp_progress_update(wxsharp_handle progress, int value, const char* message,
                                             bool* continueRunning);
    WXSHARP_API bool wxsharp_progress_pulse(wxsharp_handle progress, const char* message,
                                            bool* continueRunning);
    WXSHARP_API bool wxsharp_progress_was_cancelled(wxsharp_handle progress);
    WXSHARP_API bool wxsharp_progress_was_skipped(wxsharp_handle progress);
    WXSHARP_API void wxsharp_progress_resume(wxsharp_handle progress);
    WXSHARP_API int  wxsharp_progress_get_value(wxsharp_handle progress);
    WXSHARP_API int  wxsharp_progress_get_range(wxsharp_handle progress);
    WXSHARP_API void wxsharp_progress_set_range(wxsharp_handle progress, int range);
    WXSHARP_API void wxsharp_progress_destroy(wxsharp_handle progress);


    // ---- Font ----------------------------------------------------------------------------------------
    // A font crosses as a handle. The family, style, weight and encoding values are wxWidgets' own, so
    // neither side translates them; a weight is the numeric 100-1000 scale, not a three-point enum.
    WXSHARP_API wxsharp_handle wxsharp_font_create_empty();
    WXSHARP_API wxsharp_handle wxsharp_font_create(double point_size, int pixel_width, int pixel_height,
                                                   bool use_pixels, int family, int style, int weight,
                                                   bool underlined, bool strikethrough, const char* face,
                                                   int encoding, int flags);
    WXSHARP_API wxsharp_handle wxsharp_font_create_from_native(const char* native_info);
    WXSHARP_API wxsharp_handle wxsharp_font_copy(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_destroy(wxsharp_handle font);
    WXSHARP_API bool wxsharp_font_is_ok(wxsharp_handle font);
    WXSHARP_API bool wxsharp_font_equals(wxsharp_handle a, wxsharp_handle b);

    WXSHARP_API int    wxsharp_font_get_point_size(wxsharp_handle font);
    WXSHARP_API void   wxsharp_font_set_point_size(wxsharp_handle font, int size);
    WXSHARP_API double wxsharp_font_get_fractional_point_size(wxsharp_handle font);
    WXSHARP_API void   wxsharp_font_set_fractional_point_size(wxsharp_handle font, double size);
    WXSHARP_API bool   wxsharp_font_is_using_size_in_pixels(wxsharp_handle font);
    WXSHARP_API void   wxsharp_font_get_pixel_size(wxsharp_handle font, int* width, int* height);
    WXSHARP_API void   wxsharp_font_set_pixel_size(wxsharp_handle font, int width, int height);
    WXSHARP_API void   wxsharp_font_set_symbolic_size(wxsharp_handle font, int size);
    WXSHARP_API void   wxsharp_font_set_symbolic_size_relative_to(wxsharp_handle font, int size, int base);

    WXSHARP_API int  wxsharp_font_get_family(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_set_family(wxsharp_handle font, int family);
    WXSHARP_API int  wxsharp_font_get_style(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_set_style(wxsharp_handle font, int style);
    WXSHARP_API int  wxsharp_font_get_numeric_weight(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_set_numeric_weight(wxsharp_handle font, int weight);
    WXSHARP_API int  wxsharp_font_get_weight(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_set_weight(wxsharp_handle font, int weight);
    WXSHARP_API bool wxsharp_font_get_underlined(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_set_underlined(wxsharp_handle font, bool value);
    WXSHARP_API bool wxsharp_font_get_strikethrough(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_set_strikethrough(wxsharp_handle font, bool value);
    WXSHARP_API int  wxsharp_font_get_encoding(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_set_encoding(wxsharp_handle font, int encoding);
    WXSHARP_API bool wxsharp_font_is_fixed_width(wxsharp_handle font);
    WXSHARP_API int  wxsharp_font_get_face_name(wxsharp_handle font, char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_font_set_face_name(wxsharp_handle font, const char* face);
    WXSHARP_API int  wxsharp_font_get_native_info(wxsharp_handle font, char* buffer, int buffer_length);
    WXSHARP_API int  wxsharp_font_get_native_info_user_desc(wxsharp_handle font, char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_font_set_native_info(wxsharp_handle font, const char* description);
    WXSHARP_API bool wxsharp_font_set_native_info_user_desc(wxsharp_handle font, const char* description);
    WXSHARP_API int  wxsharp_font_get_family_string(wxsharp_handle font, char* buffer, int buffer_length);
    WXSHARP_API int  wxsharp_font_get_style_string(wxsharp_handle font, char* buffer, int buffer_length);
    WXSHARP_API int  wxsharp_font_get_weight_string(wxsharp_handle font, char* buffer, int buffer_length);

    WXSHARP_API wxsharp_handle wxsharp_font_bold(wxsharp_handle font);
    WXSHARP_API wxsharp_handle wxsharp_font_italic(wxsharp_handle font);
    WXSHARP_API wxsharp_handle wxsharp_font_underlined(wxsharp_handle font);
    WXSHARP_API wxsharp_handle wxsharp_font_strikethrough(wxsharp_handle font);
    WXSHARP_API wxsharp_handle wxsharp_font_larger(wxsharp_handle font);
    WXSHARP_API wxsharp_handle wxsharp_font_smaller(wxsharp_handle font);
    WXSHARP_API wxsharp_handle wxsharp_font_scaled(wxsharp_handle font, float factor);
    WXSHARP_API wxsharp_handle wxsharp_font_base(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_make_bold(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_make_italic(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_make_underlined(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_make_strikethrough(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_make_larger(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_make_smaller(wxsharp_handle font);
    WXSHARP_API void wxsharp_font_scale(wxsharp_handle font, float factor);

    WXSHARP_API int  wxsharp_font_get_default_encoding();
    WXSHARP_API void wxsharp_font_set_default_encoding(int encoding);
    WXSHARP_API int  wxsharp_font_numeric_weight_of(int weight);
    WXSHARP_API int  wxsharp_font_weight_closest_to(int numeric_weight);
    WXSHARP_API int  wxsharp_font_adjust_to_symbolic_size(int size, int base);
    WXSHARP_API int  wxsharp_font_add_private(const char* filename);
    WXSHARP_API wxsharp_handle wxsharp_font_from_system(int which);

    // Listing the faces the platform has. The result set is held natively until the next call; read each
    // name back with wxsharp_font_enumerated_name().
    WXSHARP_API int  wxsharp_font_enumerate_facenames(int encoding, bool fixed_width_only);
    WXSHARP_API int  wxsharp_font_enumerate_encodings(const char* facename);
    WXSHARP_API int  wxsharp_font_enumerated_name(int index, char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_font_is_valid_facename(const char* facename);
    WXSHARP_API void wxsharp_font_invalidate_enumeration_cache();
    WXSHARP_API bool wxsharp_font_can_use_private();



    // ---- wxWindow, continued --------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_window_find_focus();
    WXSHARP_API wxsharp_handle wxsharp_window_find_by_id(long id, wxsharp_handle parent);
    WXSHARP_API wxsharp_handle wxsharp_window_find_child_by_id(wxsharp_handle window, long id);
    WXSHARP_API wxsharp_handle wxsharp_window_find_child_by_name(wxsharp_handle window, const char* name);
    WXSHARP_API wxsharp_handle wxsharp_window_get_capture();
    WXSHARP_API int  wxsharp_window_new_control_id(int count);
    WXSHARP_API void wxsharp_window_unreserve_control_id(int id, int count);

    WXSHARP_API wxsharp_handle wxsharp_window_top_level_parent(wxsharp_handle window);
    WXSHARP_API wxsharp_handle wxsharp_window_grand_parent(wxsharp_handle window);
    WXSHARP_API wxsharp_handle wxsharp_window_next_sibling(wxsharp_handle window);
    WXSHARP_API wxsharp_handle wxsharp_window_prev_sibling(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_reparent(wxsharp_handle window, wxsharp_handle parent);
    WXSHARP_API void wxsharp_window_destroy_children(wxsharp_handle window);
    WXSHARP_API int  wxsharp_window_child_count(wxsharp_handle window);
    WXSHARP_API wxsharp_handle wxsharp_window_child_at(wxsharp_handle window, int index);

    WXSHARP_API void wxsharp_window_move_before_in_tab_order(wxsharp_handle window, wxsharp_handle other);
    WXSHARP_API void wxsharp_window_move_after_in_tab_order(wxsharp_handle window, wxsharp_handle other);

    WXSHARP_API bool wxsharp_window_can_accept_focus(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_can_accept_focus_from_keyboard(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_can_be_focused(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_is_focusable(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_disable_focus_from_keyboard(wxsharp_handle window);

    WXSHARP_API void wxsharp_window_push_event_handler(wxsharp_handle window, wxsharp_handle handler);
    WXSHARP_API wxsharp_handle wxsharp_window_pop_event_handler(wxsharp_handle window, bool delete_handler);
    WXSHARP_API bool wxsharp_window_remove_event_handler(wxsharp_handle window, wxsharp_handle handler);
    WXSHARP_API wxsharp_handle wxsharp_window_get_event_handler(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_set_event_handler(wxsharp_handle window, wxsharp_handle handler);

    WXSHARP_API long wxsharp_window_get_extra_style(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_set_extra_style(wxsharp_handle window, long style);
    WXSHARP_API bool wxsharp_window_has_extra_style(wxsharp_handle window, int flag);
    WXSHARP_API void wxsharp_window_toggle_style(wxsharp_handle window, int flag);
    WXSHARP_API bool wxsharp_window_get_theme_enabled(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_set_theme_enabled(wxsharp_handle window, bool enable);
    WXSHARP_API bool wxsharp_window_is_retained(wxsharp_handle window);
    WXSHARP_API bool wxsharp_window_is_this_enabled(wxsharp_handle window);

    WXSHARP_API void wxsharp_window_set_initial_size(wxsharp_handle window, int width, int height);
    WXSHARP_API void wxsharp_window_invalidate_best_size(wxsharp_handle window);
    WXSHARP_API int  wxsharp_window_get_best_height(wxsharp_handle window, int width);
    WXSHARP_API int  wxsharp_window_get_best_width(wxsharp_handle window, int height);
    WXSHARP_API double wxsharp_window_content_scale_factor(wxsharp_handle window);
    WXSHARP_API double wxsharp_window_dpi_scale_factor(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_client_to_window_size(wxsharp_handle window, int width, int height,
                                                          int* out_w, int* out_h);
    WXSHARP_API void wxsharp_window_window_to_client_size(wxsharp_handle window, int width, int height,
                                                          int* out_w, int* out_h);
    WXSHARP_API void wxsharp_window_from_phys(wxsharp_handle window, int width, int height,
                                              int* out_w, int* out_h);
    WXSHARP_API void wxsharp_window_to_phys(wxsharp_handle window, int width, int height,
                                            int* out_w, int* out_h);

    WXSHARP_API bool wxsharp_window_can_scroll(wxsharp_handle window, int orientation);
    WXSHARP_API bool wxsharp_window_is_exposed(wxsharp_handle window, int x, int y, int width, int height);
    WXSHARP_API void wxsharp_window_update_client_rect(wxsharp_handle window, int* x, int* y,
                                                       int* width, int* height);

    WXSHARP_API bool wxsharp_window_show_with_effect(wxsharp_handle window, int effect,
                                                     unsigned int milliseconds);
    WXSHARP_API bool wxsharp_window_hide_with_effect(wxsharp_handle window, int effect,
                                                     unsigned int milliseconds);
    WXSHARP_API void wxsharp_window_enable_touch_events(wxsharp_handle window, int events);

    // ---- Platform services ----------------------------------------------------------------------------
    // Where the platform keeps files, the sounds and stock art it provides, the displays attached, and the
    // window furniture other classes hang off.

    WXSHARP_API int wxsharp_stdpaths_executable(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_config_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_user_config_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_data_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_local_data_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_user_data_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_user_local_data_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_plugins_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_resources_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_documents_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_temp_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_app_documents_dir(char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_user_dir(int which, char* buffer, int length);
    WXSHARP_API int wxsharp_stdpaths_localized_resources_dir(const char* language, int category,
                                                            char* buffer, int length);

    WXSHARP_API wxsharp_handle wxsharp_sound_create(const char* path);
    WXSHARP_API void wxsharp_sound_destroy(wxsharp_handle sound);
    WXSHARP_API bool wxsharp_sound_is_ok(wxsharp_handle sound);
    WXSHARP_API bool wxsharp_sound_play(wxsharp_handle sound, unsigned int flags);
    WXSHARP_API bool wxsharp_sound_play_file(const char* path, unsigned int flags);
    WXSHARP_API void wxsharp_sound_stop();

    WXSHARP_API unsigned int wxsharp_display_count();
    WXSHARP_API int  wxsharp_display_from_point(int x, int y);
    WXSHARP_API int  wxsharp_display_from_window(wxsharp_handle window);
    WXSHARP_API void wxsharp_display_geometry(unsigned int index, int* x, int* y, int* width, int* height);
    WXSHARP_API void wxsharp_display_client_area(unsigned int index, int* x, int* y, int* width, int* height);
    WXSHARP_API bool wxsharp_display_is_primary(unsigned int index);
    WXSHARP_API int  wxsharp_display_name(unsigned int index, char* buffer, int length);
    WXSHARP_API double wxsharp_display_scale_factor(unsigned int index);
    WXSHARP_API void wxsharp_display_ppi(unsigned int index, int* x, int* y);

    WXSHARP_API wxsharp_handle wxsharp_art_bitmap(const char* id, const char* client, int width, int height);
    WXSHARP_API wxsharp_handle wxsharp_art_icon(const char* id, const char* client, int width, int height);
    WXSHARP_API void wxsharp_art_native_size(const char* client, wxsharp_handle window, int* width, int* height);

    WXSHARP_API wxsharp_handle wxsharp_cursor_create_stock(int id);
    WXSHARP_API wxsharp_handle wxsharp_cursor_create_from_file(const char* path, int type,
                                                              int hotspot_x, int hotspot_y);
    WXSHARP_API void wxsharp_cursor_destroy(wxsharp_handle cursor);
    WXSHARP_API bool wxsharp_cursor_is_ok(wxsharp_handle cursor);
    WXSHARP_API void wxsharp_control_set_cursor(wxsharp_handle ctrl, wxsharp_handle cursor);
    WXSHARP_API wxsharp_handle wxsharp_control_get_cursor(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_cursor_set_global(wxsharp_handle cursor);

    WXSHARP_API wxsharp_handle wxsharp_imagelist_create(int width, int height, bool mask, int initial_count);
    WXSHARP_API void wxsharp_imagelist_destroy(wxsharp_handle list);
    WXSHARP_API int  wxsharp_imagelist_count(wxsharp_handle list);
    WXSHARP_API bool wxsharp_imagelist_remove(wxsharp_handle list, int index);
    WXSHARP_API bool wxsharp_imagelist_remove_all(wxsharp_handle list);
    WXSHARP_API int  wxsharp_imagelist_add_bitmap(wxsharp_handle list, wxsharp_handle bitmap);
    WXSHARP_API int  wxsharp_imagelist_add_icon(wxsharp_handle list, wxsharp_handle icon);
    WXSHARP_API bool wxsharp_imagelist_replace(wxsharp_handle list, int index, wxsharp_handle bitmap);
    WXSHARP_API void wxsharp_imagelist_size(wxsharp_handle list, int index, int* width, int* height);
    WXSHARP_API wxsharp_handle wxsharp_imagelist_get_bitmap(wxsharp_handle list, int index);
    WXSHARP_API void wxsharp_listctrl_set_image_list(wxsharp_handle ctrl, wxsharp_handle list, int which,
                                                     bool transfer);
    WXSHARP_API void wxsharp_treectrl_set_image_list(wxsharp_handle ctrl, wxsharp_handle list, bool transfer);
    WXSHARP_API void wxsharp_listctrl_set_item_image(wxsharp_handle ctrl, long long item, int image);
    WXSHARP_API void wxsharp_treectrl_set_item_image(wxsharp_handle ctrl, long long item, int image, int which);
    WXSHARP_API int  wxsharp_treectrl_get_item_image(wxsharp_handle ctrl, long long item, int which);

    WXSHARP_API void wxsharp_control_set_caret(wxsharp_handle ctrl, int width, int height);
    WXSHARP_API bool wxsharp_control_has_caret(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_caret_move(wxsharp_handle ctrl, int x, int y);
    WXSHARP_API void wxsharp_caret_show(wxsharp_handle ctrl, bool show);
    WXSHARP_API bool wxsharp_caret_is_visible(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_caret_position(wxsharp_handle ctrl, int* x, int* y);
    WXSHARP_API int  wxsharp_caret_get_blink_time();
    WXSHARP_API void wxsharp_caret_set_blink_time(int milliseconds);

    WXSHARP_API void wxsharp_about_box(const char* name, const char* version, const char* description,
                                       const char* copyright, const char* website,
                                       const char* website_label, const char* const* developers,
                                       int developer_count, wxsharp_handle parent);

    WXSHARP_API void wxsharp_rich_tooltip_show(wxsharp_handle window, const char* title, const char* message,
                                               int icon, int timeout_ms, int show_delay_ms);


    // ---- Common dialogs, as real windows --------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_filedlg_create(wxsharp_handle parent, const char* message,
                                                      const char* directory, const char* file,
                                                      const char* wildcard, int style, long long token);
    WXSHARP_API int  wxsharp_filedlg_get_path(wxsharp_handle dlg, char* buffer, int length);
    WXSHARP_API int  wxsharp_filedlg_get_directory(wxsharp_handle dlg, char* buffer, int length);
    WXSHARP_API int  wxsharp_filedlg_get_filename(wxsharp_handle dlg, char* buffer, int length);
    WXSHARP_API int  wxsharp_filedlg_get_wildcard(wxsharp_handle dlg, char* buffer, int length);
    WXSHARP_API int  wxsharp_filedlg_get_message(wxsharp_handle dlg, char* buffer, int length);
    WXSHARP_API void wxsharp_filedlg_set_path(wxsharp_handle dlg, const char* path);
    WXSHARP_API void wxsharp_filedlg_set_directory(wxsharp_handle dlg, const char* dir);
    WXSHARP_API void wxsharp_filedlg_set_filename(wxsharp_handle dlg, const char* name);
    WXSHARP_API void wxsharp_filedlg_set_wildcard(wxsharp_handle dlg, const char* wildcard);
    WXSHARP_API void wxsharp_filedlg_set_message(wxsharp_handle dlg, const char* message);
    WXSHARP_API int  wxsharp_filedlg_get_filter_index(wxsharp_handle dlg);
    WXSHARP_API void wxsharp_filedlg_set_filter_index(wxsharp_handle dlg, int index);
    WXSHARP_API int  wxsharp_filedlg_path_count(wxsharp_handle dlg);
    WXSHARP_API int  wxsharp_filedlg_path_at(wxsharp_handle dlg, int index, char* buffer, int length);
    WXSHARP_API int  wxsharp_filedlg_filename_at(wxsharp_handle dlg, int index, char* buffer, int length);

    WXSHARP_API wxsharp_handle wxsharp_dirdlg_create(wxsharp_handle parent, const char* message,
                                                     const char* default_path, int style, long long token);
    WXSHARP_API int  wxsharp_dirdlg_get_path(wxsharp_handle dlg, char* buffer, int length);
    WXSHARP_API void wxsharp_dirdlg_set_path(wxsharp_handle dlg, const char* path);
    WXSHARP_API int  wxsharp_dirdlg_get_message(wxsharp_handle dlg, char* buffer, int length);
    WXSHARP_API void wxsharp_dirdlg_set_message(wxsharp_handle dlg, const char* message);
    WXSHARP_API int  wxsharp_dirdlg_path_count(wxsharp_handle dlg);
    WXSHARP_API int  wxsharp_dirdlg_path_at(wxsharp_handle dlg, int index, char* buffer, int length);

    WXSHARP_API wxsharp_handle wxsharp_textdlg_create(wxsharp_handle parent, const char* message,
                                                      const char* caption, const char* value, int style,
                                                      long long token);
    WXSHARP_API int  wxsharp_textdlg_get_value(wxsharp_handle dlg, char* buffer, int length);
    WXSHARP_API void wxsharp_textdlg_set_value(wxsharp_handle dlg, const char* value);
    WXSHARP_API void wxsharp_textdlg_set_max_length(wxsharp_handle dlg, unsigned long length);
    WXSHARP_API void wxsharp_textdlg_force_upper(wxsharp_handle dlg);

    WXSHARP_API wxsharp_handle wxsharp_numdlg_create(wxsharp_handle parent, const char* message,
                                                     const char* prompt, const char* caption,
                                                     long long value, long long minimum, long long maximum,
                                                     long long token);
    WXSHARP_API long long wxsharp_numdlg_get_value(wxsharp_handle dlg);

    WXSHARP_API wxsharp_handle wxsharp_colourdlg_create(wxsharp_handle parent, unsigned int initial,
                                                        bool full, long long token);
    WXSHARP_API unsigned int wxsharp_colourdlg_get_colour(wxsharp_handle dlg);
    WXSHARP_API void wxsharp_colourdlg_set_colour(wxsharp_handle dlg, unsigned int colour);
    WXSHARP_API unsigned int wxsharp_colourdlg_get_custom(wxsharp_handle dlg, int index);
    WXSHARP_API void wxsharp_colourdlg_set_custom(wxsharp_handle dlg, int index, unsigned int colour);

    WXSHARP_API wxsharp_handle wxsharp_fontdlg_create(wxsharp_handle parent, wxsharp_handle initial,
                                                      long long token);
    WXSHARP_API wxsharp_handle wxsharp_fontdlg_get_font(wxsharp_handle dlg);
    WXSHARP_API unsigned int wxsharp_fontdlg_get_colour(wxsharp_handle dlg);
    WXSHARP_API void wxsharp_fontdlg_set_colour(wxsharp_handle dlg, unsigned int colour);
    WXSHARP_API void wxsharp_fontdlg_enable_effects(wxsharp_handle dlg, bool enable);
    WXSHARP_API void wxsharp_fontdlg_set_range(wxsharp_handle dlg, int minimum, int maximum);

    // ---- Services ------------------------------------------------------------------------------------
    // Shows a native open/save file dialog; returns true and writes the chosen path if confirmed.
    // Shows an open or save dialog and keeps the chosen paths until the next call, so a multiple selection
    // does not have to be squeezed into a caller-sized buffer. Returns how many paths were chosen, 0 when
    // the dialog was cancelled. Read each one back with wxsharp_file_dialog_result().
    WXSHARP_API int  wxsharp_file_dialog(wxsharp_handle parent, const char* title, const char* wildcard,
                                         const char* default_dir, const char* default_file, int style);
    WXSHARP_API int  wxsharp_file_dialog_result(int index, char* buffer, int buffer_length);
    // Shows a native folder-picker; returns true and writes the chosen path if confirmed.
    WXSHARP_API bool wxsharp_dir_dialog(wxsharp_handle parent, const char* title, const char* initial_dir,
                                        char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_text_entry_dialog(wxsharp_handle parent, const char* message,
                                               const char* caption, const char* value, bool password,
                                               char* buffer, int buffer_length);
    WXSHARP_API bool wxsharp_number_entry_dialog(wxsharp_handle parent, const char* message,
                                                 const char* prompt, const char* caption, long long value,
                                                 long long minimum, long long maximum, long long* result);
    WXSHARP_API bool wxsharp_colour_dialog(wxsharp_handle parent, unsigned int initial,
                                           unsigned int* result);
#ifdef __cplusplus
}
#endif
