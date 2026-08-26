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
    typedef struct wxsharp_event
    {
        unsigned int size;
        unsigned int version;
        long long token;
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
    } wxsharp_event;

    // Callback result flags. HANDLED prevents normal wx processing/command propagation; CANCEL vetoes a
    // close event when can_veto is true.
    enum { WXSHARP_EVENT_HANDLED = 1, WXSHARP_EVENT_CANCEL = 2 };
    typedef unsigned int (*wxsharp_event_cb)(const wxsharp_event* event_data);

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
    WXSHARP_API int  wxsharp_message_box(const char* message, const char* caption, int style);
    WXSHARP_API void wxsharp_shutdown();

    // ---- Frame ----------------------------------------------------------------------------------------
    // Child panels/controls and sizers are always created and assigned explicitly.
    WXSHARP_API wxsharp_handle wxsharp_window_create(wxsharp_handle parent, int id, const char* title,
                                                     int x, int y, int width, int height, long long token);
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
                                                     int x, int y, int width, int height, long long token);
    WXSHARP_API void wxsharp_dialog_set_title(wxsharp_handle dialog, const char* title);
    WXSHARP_API int  wxsharp_dialog_get_title(wxsharp_handle dialog, char* buffer, int buffer_length);
    WXSHARP_API void wxsharp_dialog_set_escape_id(wxsharp_handle dialog, int id);       // id returned when Esc is pressed
    WXSHARP_API void wxsharp_dialog_set_affirmative_id(wxsharp_handle dialog, int id);  // id activated when Enter is pressed
    WXSHARP_API int  wxsharp_dialog_show_modal(wxsharp_handle dialog); // blocks, returns EndModal's result
    WXSHARP_API void wxsharp_dialog_show(wxsharp_handle dialog, bool show); // modeless: returns immediately
    WXSHARP_API void wxsharp_dialog_end_modal(wxsharp_handle dialog, int result);
    WXSHARP_API void wxsharp_dialog_destroy(wxsharp_handle dialog);

    // ---- Explicit panel container ---------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_panel_create(wxsharp_handle parent, int id, long long token);

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
    WXSHARP_API void wxsharp_control_set_role(wxsharp_handle ctrl, int role);                   // WxSharp role enum (0 = default)
    WXSHARP_API void wxsharp_control_set_description(wxsharp_handle ctrl, const char* text);
    WXSHARP_API void wxsharp_control_set_help(wxsharp_handle ctrl, const char* text);
    WXSHARP_API void wxsharp_control_set_accessible_value(wxsharp_handle ctrl, const char* text);
    WXSHARP_API void wxsharp_control_set_accessible_keyboard_shortcut(wxsharp_handle ctrl, const char* text);
    WXSHARP_API void wxsharp_control_set_accessible_default_action(wxsharp_handle ctrl, const char* text);
    WXSHARP_API void wxsharp_control_set_accessible_state(wxsharp_handle ctrl, unsigned int state);
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
    WXSHARP_API void wxsharp_combobox_clear(wxsharp_handle ctrl);
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
    WXSHARP_API wxsharp_handle wxsharp_scrolled_create(wxsharp_handle parent, int id, long long token);
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
    WXSHARP_API wxsharp_handle wxsharp_listctrl_create(wxsharp_handle parent, int id, long long token);
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
    WXSHARP_API wxsharp_handle wxsharp_treectrl_create(wxsharp_handle parent, int id, long long token);
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
    WXSHARP_API wxsharp_handle wxsharp_menu_create();
    WXSHARP_API void wxsharp_menu_destroy(wxsharp_handle menu);
    WXSHARP_API void wxsharp_menu_append(wxsharp_handle menu, int id, const char* text, const char* help, int kind);
    WXSHARP_API void wxsharp_menu_append_separator(wxsharp_handle menu);
    WXSHARP_API void wxsharp_menu_enable(wxsharp_handle menu, int id, bool enable);
    WXSHARP_API void wxsharp_menu_check(wxsharp_handle menu, int id, bool check);
    WXSHARP_API bool wxsharp_menu_is_checked(wxsharp_handle menu, int id);
    WXSHARP_API wxsharp_handle wxsharp_menubar_create();
    WXSHARP_API void wxsharp_menubar_destroy(wxsharp_handle menu_bar);
    WXSHARP_API bool wxsharp_menubar_append(wxsharp_handle menu_bar, wxsharp_handle menu, const char* title);
    WXSHARP_API void wxsharp_frame_set_menubar(wxsharp_handle frame, wxsharp_handle menu_bar);
    WXSHARP_API wxsharp_handle wxsharp_statusbar_create(wxsharp_handle frame, int fields, long long token);
    WXSHARP_API void wxsharp_statusbar_set_text(wxsharp_handle status, const char* text, int field);
    WXSHARP_API int wxsharp_statusbar_get_text(wxsharp_handle status, int field, char* buffer, int buffer_length);
    WXSHARP_API wxsharp_handle wxsharp_toolbar_create(wxsharp_handle frame, long long token);
    WXSHARP_API void wxsharp_toolbar_add_tool(wxsharp_handle toolbar, int id, const char* label, const char* help, int kind);
    WXSHARP_API void wxsharp_toolbar_add_separator(wxsharp_handle toolbar);
    WXSHARP_API void wxsharp_toolbar_realize(wxsharp_handle toolbar);
    WXSHARP_API void wxsharp_toolbar_enable(wxsharp_handle toolbar, int id, bool enable);
    WXSHARP_API void wxsharp_toolbar_toggle(wxsharp_handle toolbar, int id, bool toggle);
    WXSHARP_API void wxsharp_frame_set_accelerators(wxsharp_handle frame,
                                                    const wxsharp_accelerator* entries, int count);

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
    WXSHARP_API bool wxsharp_file_dialog(wxsharp_handle parent, const char* title, const char* wildcard,
                                   bool save, char* buffer, int buffer_length);
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
