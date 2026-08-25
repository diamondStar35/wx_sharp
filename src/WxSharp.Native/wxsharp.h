// wxsharp - a flat, UTF-8 C ABI over wxWidgets. Windows, dialogs and controls
// are opaque handles; every event is reported through one callback as (managed-id, event-type). The managed
// WxSharp binding drives this through [LibraryImport]; nothing here is specific to one application.
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
    typedef void (*wxsharp_event_cb)(int id, int evt);
    // Key hook: reports a key event as (id, kind, key_code, modifiers). kind is a WXSHARP_KEY_* value - a
    // char-hook on a top-level window, or a key down/up on a focused control (modifiers: bit0 Ctrl, bit1 Shift,
    // bit2 Alt). Return true to consume the key, false to let it be processed normally (tab nav, typing, ...).
    typedef bool (*wxsharp_key_cb)(int id, int kind, int key_code, int modifiers);

    // ---- App lifetime ---------------------------------------------------------------------------------
    WXSHARP_API bool wxsharp_init();
    WXSHARP_API void wxsharp_set_event_handler(wxsharp_event_cb cb);
    WXSHARP_API void wxsharp_set_key_handler(wxsharp_key_cb cb);
    WXSHARP_API void wxsharp_pump();                 // drain pending events (call per host-loop tick)
    WXSHARP_API void wxsharp_wait(int timeout_ms);   // idle until input/timeout (negative = forever)
    WXSHARP_API int  wxsharp_message_box(const char* message, const char* caption, int style);
    WXSHARP_API void wxsharp_shutdown();

    // ---- Window (top-level frame with a vertical content panel) ---------------------------------------
    // Lifecycle events (Shown/Activate/Deactivate/Resize/Close) are reported through the event callback.
    // with_panel false gives a bare frame (no content wxPanel, which a screen reader would announce).
    WXSHARP_API wxsharp_handle wxsharp_window_create(const char* title, int width, int height, int id, bool with_panel);
    WXSHARP_API wxsharp_handle wxsharp_window_panel(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_show(wxsharp_handle window, bool show);
    WXSHARP_API void wxsharp_window_set_title(wxsharp_handle window, const char* title);
    WXSHARP_API void wxsharp_window_layout(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_center(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_set_fullscreen(wxsharp_handle window, bool fullscreen); // borderless, hides any menu bar
    WXSHARP_API void* wxsharp_window_native_handle(wxsharp_handle window); // HWND, GtkWidget*, or NSView*
    WXSHARP_API void wxsharp_window_close(wxsharp_handle window);
    WXSHARP_API void wxsharp_window_destroy(wxsharp_handle window);

    // ---- Dialog (modal or modeless) ------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_dialog_create(const char* title, int width, int height, int id);
    WXSHARP_API wxsharp_handle wxsharp_dialog_panel(wxsharp_handle dialog);
    WXSHARP_API void wxsharp_dialog_layout(wxsharp_handle dialog);
    WXSHARP_API void wxsharp_dialog_set_escape_id(wxsharp_handle dialog, int id);       // id returned when Esc is pressed
    WXSHARP_API void wxsharp_dialog_set_affirmative_id(wxsharp_handle dialog, int id);  // id activated when Enter is pressed
    WXSHARP_API int  wxsharp_dialog_show_modal(wxsharp_handle dialog); // blocks, returns EndModal's result
    WXSHARP_API void wxsharp_dialog_show(wxsharp_handle dialog, bool show); // modeless: returns immediately
    WXSHARP_API void wxsharp_dialog_end_modal(wxsharp_handle dialog, int result);
    WXSHARP_API void wxsharp_dialog_destroy(wxsharp_handle dialog);

    // ---- Layout ---------------------------------------------------------------------------------------
    // A sub-panel with its own horizontal/vertical sizer; controls created against it stack in that
    // direction. Nest panels for rows-within-columns and richer layouts.
    WXSHARP_API wxsharp_handle wxsharp_panel_create(wxsharp_handle parent, bool horizontal, int id);

    // ---- Canvas -------------------------------------------------------------------------------------
    // A non-focusable, custom-drawn surface (skipped by assistive tech). It reports a Paint event; draw from
    // the managed handler with the functions below - they only take effect during that paint. A colour with
    // alpha 0 selects the transparent pen/brush. measure_text works any time (uses the control font).
    WXSHARP_API wxsharp_handle wxsharp_canvas_create(wxsharp_handle parent, int width, int height, bool fill, int id);
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

    // ---- Sizers ---------------------------------------------------------------------------------------
    // Explicit layout: a box sizer lays items in one direction; add controls/sizers with a proportion
    // (0 = fixed), expand/centre, and border, plus fixed or stretchable spacers. A window adopts a sizer.
    WXSHARP_API wxsharp_handle wxsharp_boxsizer_create(bool horizontal);
    WXSHARP_API void wxsharp_sizer_add_control(wxsharp_handle sizer, wxsharp_handle ctrl, int proportion, bool expand, bool center, int border);
    WXSHARP_API void wxsharp_sizer_add_sizer(wxsharp_handle sizer, wxsharp_handle child, int proportion, bool expand, int border);
    WXSHARP_API void wxsharp_sizer_add_spacer(wxsharp_handle sizer, int size);
    WXSHARP_API void wxsharp_sizer_add_stretch_spacer(wxsharp_handle sizer, int proportion);
    WXSHARP_API void wxsharp_window_set_sizer(wxsharp_handle window, wxsharp_handle sizer);

    // ---- Label ---------------------------------------------------------------------------------------
    // style: WxSharp Alignment enum (left/centre/right).
    WXSHARP_API wxsharp_handle wxsharp_label_create(wxsharp_handle parent, const char* text, int style);
    WXSHARP_API void wxsharp_label_set_text(wxsharp_handle ctrl, const char* text);
    WXSHARP_API int  wxsharp_label_get_text(wxsharp_handle ctrl, char* buffer, int buffer_length);

    // ---- Button --------------------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_button_create(wxsharp_handle parent, const char* label, int id);
    WXSHARP_API void wxsharp_button_set_default(wxsharp_handle ctrl); // make it the default (Enter activates it)
    WXSHARP_API void wxsharp_button_set_label(wxsharp_handle ctrl, const char* label);
    WXSHARP_API int  wxsharp_button_get_label(wxsharp_handle ctrl, char* buffer, int buffer_length);

    // ---- Text box ------------------------------------------------------------------------------------
    // fill: cover the parent's client area outside any sizer (an inline prompt that owns the window).
    // style: WxSharp TextBoxStyle flags (password, read-only, multi-line, alignment, ...).
    WXSHARP_API wxsharp_handle wxsharp_textbox_create(wxsharp_handle parent, const char* value, bool fill, int style, int id);
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
    WXSHARP_API wxsharp_handle wxsharp_checkbox_create(wxsharp_handle parent, const char* label, int style, int id);
    WXSHARP_API bool wxsharp_checkbox_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_checkbox_set(wxsharp_handle ctrl, bool value);

    // ---- Radio button --------------------------------------------------------------------------------
    WXSHARP_API wxsharp_handle wxsharp_radio_create(wxsharp_handle parent, const char* label, bool group_start, int id);
    WXSHARP_API bool wxsharp_radio_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_radio_set(wxsharp_handle ctrl, bool value);

    // ---- Slider --------------------------------------------------------------------------------------
    // style: WxSharp SliderStyle flags (orientation, labels, ticks, ...). The accessible key/notify behaviour
    // is implemented by the managed CustomSlider on top of this plain control.
    WXSHARP_API wxsharp_handle wxsharp_slider_create(wxsharp_handle parent, int min_value, int max_value, int value, int style, int id);
    WXSHARP_API int  wxsharp_slider_get(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_slider_set(wxsharp_handle ctrl, int value);
    WXSHARP_API int  wxsharp_slider_get_min(wxsharp_handle ctrl);
    WXSHARP_API int  wxsharp_slider_get_max(wxsharp_handle ctrl);
    WXSHARP_API void wxsharp_slider_set_range(wxsharp_handle ctrl, int min_value, int max_value);

    // ---- Choice (drop-down) --------------------------------------------------------------------------
    // style: WxSharp ChoiceStyle (sorted or not).
    WXSHARP_API wxsharp_handle wxsharp_choice_create(wxsharp_handle parent, int style, int id);
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
    WXSHARP_API wxsharp_handle wxsharp_listbox_create(wxsharp_handle parent, int style, int id);
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

    // ---- Services ------------------------------------------------------------------------------------
    WXSHARP_API void wxsharp_clipboard_set_text(const char* text);
    WXSHARP_API int  wxsharp_clipboard_get_text(char* buffer, int buffer_length);
    // Shows a native open/save file dialog; returns true and writes the chosen path if confirmed.
    WXSHARP_API bool wxsharp_file_dialog(wxsharp_handle parent, const char* title, const char* wildcard,
                                   bool save, char* buffer, int buffer_length);
    // Shows a native folder-picker; returns true and writes the chosen path if confirmed.
    WXSHARP_API bool wxsharp_dir_dialog(wxsharp_handle parent, const char* title, const char* initial_dir,
                                  char* buffer, int buffer_length);
#ifdef __cplusplus
}
#endif
