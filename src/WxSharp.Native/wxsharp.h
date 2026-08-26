// wxsharp - a flat, UTF-8 C ABI over wxWidgets. Windows, dialogs and controls are opaque handles. Events
// cross the boundary as a versioned value-only structure, keeping the ABI friendly to Native AOT.
#pragma once

#ifndef __cplusplus
#  include <stdbool.h>
#endif

#if defined(_WIN32) && defined(WXSHARP_BUILD)
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
    typedef struct wxsharp_accelerator { int modifiers; int key_code; int command_id; } wxsharp_accelerator;

    // ---- App lifetime ---------------------------------------------------------------------------------
    WXSHARP_API bool wxsharp_init();
    WXSHARP_API void wxsharp_set_event_handler(wxsharp_event_cb cb);
    WXSHARP_API void wxsharp_set_accessible_handler(wxsharp_accessible_cb cb);
    WXSHARP_API int  wxsharp_main_loop();
    WXSHARP_API void wxsharp_exit_main_loop();
    WXSHARP_API void wxsharp_set_exit_on_frame_delete(bool value);
    WXSHARP_API void wxsharp_set_top_window(wxsharp_handle window);
    WXSHARP_API void wxsharp_call_after(long long token);
    WXSHARP_API bool wxsharp_yield(bool only_if_needed);
    WXSHARP_API int  wxsharp_message_box(wxsharp_handle parent, const char* message, const char* caption,
                                          int style);
    WXSHARP_API void wxsharp_shutdown();

    // ---- Event binding --------------------------------------------------------------------------------
    // Events are hooked on demand: the managed side binds an event ID the first time something subscribes to
    // it on a window and unbinds it when the last subscriber goes away, so an unobserved event never crosses
    // the boundary. bind() returns false when the event ID is unknown or cannot be bound to this window (for
    // example TEXT_ENTER on a control that does not process Enter). unbind_all() releases every binding on a
    // window and is called when the window is destroyed.
    WXSHARP_API bool wxsharp_window_bind(wxsharp_handle window, int event_id, long long token);
    WXSHARP_API bool wxsharp_window_unbind(wxsharp_handle window, int event_id);
    WXSHARP_API void wxsharp_window_unbind_all(wxsharp_handle window);
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

    // ---- Canvas -------------------------------------------------------------------------------------
    // A non-focusable, custom-drawn surface (skipped by assistive tech). It reports a Paint event; draw from
    // the managed handler with the functions below - they only take effect during that paint. A colour with
    // alpha 0 selects the transparent pen/brush. measure_text works any time (uses the control font).
    WXSHARP_API wxsharp_handle wxsharp_canvas_create(wxsharp_handle parent, int id, int width, int height, long long token);
    WXSHARP_API void wxsharp_canvas_clear(wxsharp_handle ctrl, unsigned int argb);
    WXSHARP_API void wxsharp_canvas_set_brush(wxsharp_handle ctrl, unsigned int argb);
    WXSHARP_API void wxsharp_canvas_set_pen(wxsharp_handle ctrl, unsigned int argb, int width);
    WXSHARP_API void wxsharp_canvas_set_text_colour(wxsharp_handle ctrl, unsigned int argb);
    WXSHARP_API void wxsharp_canvas_set_font(wxsharp_handle ctrl, int point_size, int family, int weight,
                                            int style, bool underline, const char* face);
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
    WXSHARP_API void wxsharp_control_layout(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_control_destroy(wxsharp_handle ctrl); // hides and destroys the control (create-on-demand UI)

    // Geometry (sizes/positions in device pixels).
    WXSHARP_API void wxsharp_control_get_size(wxsharp_handle ctrl, int* width, int* height);
    WXSHARP_API void wxsharp_control_set_size(wxsharp_handle ctrl, int width, int height);
    WXSHARP_API void wxsharp_control_get_client_size(wxsharp_handle ctrl, int* width, int* height);
    WXSHARP_API void wxsharp_control_get_position(wxsharp_handle ctrl, int* x, int* y);
    WXSHARP_API void wxsharp_control_set_position(wxsharp_handle ctrl, int x, int y);
    WXSHARP_API void wxsharp_control_set_min_size(wxsharp_handle ctrl, int width, int height);
    WXSHARP_API void wxsharp_control_set_max_size(wxsharp_handle ctrl, int width, int height);
    WXSHARP_API void wxsharp_control_get_best_size(wxsharp_handle ctrl, int* width, int* height);
    WXSHARP_API void wxsharp_control_fit(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_control_get_pointer_position(wxsharp_handle ctrl, int* x, int* y); // mouse in client coords

    // Appearance (colours are packed 0xAARRGGBB; the font is described by the managed Font).
    WXSHARP_API void wxsharp_control_set_background_colour(wxsharp_handle ctrl, unsigned int argb);
    WXSHARP_API unsigned int wxsharp_control_get_background_colour(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_control_set_foreground_colour(wxsharp_handle ctrl, unsigned int argb);
    WXSHARP_API unsigned int wxsharp_control_get_foreground_colour(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_control_set_font(wxsharp_handle ctrl, int point_size, int family, int weight,
                                             int style, bool underline, const char* face);
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
    WXSHARP_API wxsharp_handle wxsharp_boxsizer_create(bool horizontal);
    WXSHARP_API wxsharp_handle wxsharp_gridsizer_create(int rows, int columns, int vertical_gap,
                                                        int horizontal_gap);
    WXSHARP_API wxsharp_handle wxsharp_flexgridsizer_create(int rows, int columns, int vertical_gap,
                                                            int horizontal_gap);
    WXSHARP_API void wxsharp_flexgridsizer_add_growable_row(wxsharp_handle sizer, int row, int proportion);
    WXSHARP_API void wxsharp_flexgridsizer_add_growable_column(wxsharp_handle sizer, int column, int proportion);
    WXSHARP_API wxsharp_handle wxsharp_staticboxsizer_create(wxsharp_handle box, bool horizontal);
    WXSHARP_API wxsharp_handle wxsharp_gridbagsizer_create(int vertical_gap, int horizontal_gap);
    WXSHARP_API void wxsharp_gridbagsizer_add_control(wxsharp_handle sizer, wxsharp_handle ctrl,
                                                      int row, int column, int row_span, int column_span,
                                                      int flags, int border);
    WXSHARP_API void wxsharp_sizer_add_control(wxsharp_handle sizer, wxsharp_handle ctrl, int proportion, int flags, int border);
    WXSHARP_API void wxsharp_sizer_add_sizer(wxsharp_handle sizer, wxsharp_handle child, int proportion, int flags, int border);
    WXSHARP_API void wxsharp_sizer_add_spacer(wxsharp_handle sizer, int size);
    WXSHARP_API void wxsharp_sizer_add_stretch_spacer(wxsharp_handle sizer, int proportion);
    WXSHARP_API void wxsharp_window_set_sizer(wxsharp_handle window, wxsharp_handle sizer);

    // ---- Label ---------------------------------------------------------------------------------------
    // style: WxSharp Alignment enum (left/centre/right).
    WXSHARP_API wxsharp_handle wxsharp_label_create(wxsharp_handle parent, int id, const char* text, int style, long long token);
    WXSHARP_API void wxsharp_label_set_text(wxsharp_handle ctrl, const char* text);
    WXSHARP_API int  wxsharp_label_get_text(wxsharp_handle ctrl, char* buffer, int buffer_length);

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

    // ---- Check box -----------------------------------------------------------------------------------
    // style: WxSharp CheckBoxStyle (two-state or three-state).
    WXSHARP_API wxsharp_handle wxsharp_checkbox_create(wxsharp_handle parent, int id, const char* label, int style, long long token);
    WXSHARP_API bool wxsharp_checkbox_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_checkbox_set(wxsharp_handle ctrl, bool value);

    // ---- Radio button --------------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_radio_create(wxsharp_handle parent, int id, const char* label, bool group_start, long long token);
    WXSHARP_API bool wxsharp_radio_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_radio_set(wxsharp_handle ctrl, bool value);

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
    WXSHARP_API wxsharp_handle wxsharp_spinctrl_create(wxsharp_handle parent, int id, int min_value,
                                                       int max_value, int value, long long token);
    WXSHARP_API int  wxsharp_spinctrl_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_spinctrl_set(wxsharp_handle ctrl, int value);
    WXSHARP_API void wxsharp_spinctrl_set_range(wxsharp_handle ctrl, int min_value, int max_value);
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
    WXSHARP_API wxsharp_handle wxsharp_activity_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API void wxsharp_activity_start(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_activity_stop(wxsharp_handle ctrl);
    WXSHARP_API bool wxsharp_activity_is_running(wxsharp_handle ctrl);
    WXSHARP_API wxsharp_handle wxsharp_spinctrldouble_create(wxsharp_handle parent, int id, double min_value,
                                                             double max_value, double value, double increment,
                                                             long long token);
    WXSHARP_API double wxsharp_spinctrldouble_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_spinctrldouble_set(wxsharp_handle ctrl, double value);
    WXSHARP_API wxsharp_handle wxsharp_scrollbar_create(wxsharp_handle parent, int id, bool vertical,
                                                        long long token);
    WXSHARP_API void wxsharp_scrollbar_set(wxsharp_handle ctrl, int position, int thumb_size,
                                           int range, int page_size);
    WXSHARP_API int wxsharp_scrollbar_get_position(wxsharp_handle ctrl);
    WXSHARP_API wxsharp_handle wxsharp_hyperlink_create(wxsharp_handle parent, int id, const char* label,
                                                        const char* url, long long token);
    WXSHARP_API int wxsharp_hyperlink_get_url(wxsharp_handle ctrl, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_hyperlink_set_url(wxsharp_handle ctrl, const char* url);
    WXSHARP_API wxsharp_handle wxsharp_datepicker_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API wxsharp_handle wxsharp_timepicker_create(wxsharp_handle parent, int id, long long token);
    WXSHARP_API void wxsharp_datetime_get(wxsharp_handle ctrl, int* year, int* month, int* day,
                                          int* hour, int* minute, int* second);
    WXSHARP_API void wxsharp_datetime_set(wxsharp_handle ctrl, int year, int month, int day,
                                          int hour, int minute, int second);

    // ---- Containers ---------------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_scrolled_create(wxsharp_handle parent, int id, int style, long long token);
    WXSHARP_API void wxsharp_scrolled_set_rate(wxsharp_handle ctrl, int x_step, int y_step);
    WXSHARP_API void wxsharp_scrolled_scroll(wxsharp_handle ctrl, int x, int y);
    WXSHARP_API void wxsharp_scrolled_get_view_start(wxsharp_handle ctrl, int* x, int* y);
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
    WXSHARP_API wxsharp_handle wxsharp_timer_create(int id, long long owner_token);
    WXSHARP_API void wxsharp_timer_destroy(wxsharp_handle timer);
    WXSHARP_API bool wxsharp_timer_start(wxsharp_handle timer, int milliseconds, bool one_shot);
    WXSHARP_API void wxsharp_timer_stop(wxsharp_handle timer);
    WXSHARP_API bool wxsharp_timer_is_running(wxsharp_handle timer);
    WXSHARP_API int wxsharp_timer_get_interval(wxsharp_handle timer);

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
    WXSHARP_API wxsharp_handle wxsharp_bitmapbutton_create(wxsharp_handle parent, int id,
                                                           wxsharp_handle bitmap, long long token);
    WXSHARP_API wxsharp_handle wxsharp_icon_load(const char* path);
    WXSHARP_API void wxsharp_icon_destroy(wxsharp_handle icon);
    WXSHARP_API void wxsharp_frame_set_icon(wxsharp_handle frame, wxsharp_handle icon);
    WXSHARP_API void wxsharp_begin_busy_cursor();
    WXSHARP_API void wxsharp_end_busy_cursor();
    WXSHARP_API wxsharp_handle wxsharp_progress_create(wxsharp_handle parent, const char* title,
                                                       const char* message, int maximum);
    WXSHARP_API bool wxsharp_progress_update(wxsharp_handle progress, int value, const char* message,
                                             bool* continue_running);
    WXSHARP_API bool wxsharp_progress_pulse(wxsharp_handle progress, const char* message,
                                            bool* continue_running);
    WXSHARP_API void wxsharp_progress_destroy(wxsharp_handle progress);

    // ---- Services ------------------------------------------------------------------------------------
    WXSHARP_API void wxsharp_clipboard_set_text(const char* text);
    WXSHARP_API int  wxsharp_clipboard_get_text(char* buffer, int buffer_length);
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
