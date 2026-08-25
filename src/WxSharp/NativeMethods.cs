using System.Runtime.InteropServices;

namespace WxSharp;

// Source-generated P/Invoke into the wxsharp native shim (mirrors wxsharp.h). [LibraryImport] keeps the
// marshalling compile-time generated - no IL stubs, no reflection - so the binding stays Native-AOT clean.
// Handles are opaque nints; the event callback is an unmanaged function pointer.
internal static unsafe partial class NativeMethods
{
    private const string Library = "wxsharp";

    // ---- App ----
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_init();

    [LibraryImport(Library)]
    internal static partial void wxsharp_set_event_handler(delegate* unmanaged[Cdecl]<int, int, void> cb);

    [LibraryImport(Library)]
    internal static partial void wxsharp_set_key_handler(delegate* unmanaged[Cdecl]<int, int, int, int, byte> cb);

    [LibraryImport(Library)]
    internal static partial void wxsharp_pump();

    [LibraryImport(Library)]
    internal static partial void wxsharp_wait(int timeoutMs);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int wxsharp_message_box(string message, string caption, int style);

    [LibraryImport(Library)]
    internal static partial void wxsharp_shutdown();

    // ---- Window ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_window_create(string title, int width, int height, int id,
        [MarshalAs(UnmanagedType.U1)] bool withPanel);

    [LibraryImport(Library)]
    internal static partial nint wxsharp_window_panel(nint window);

    [LibraryImport(Library)]
    internal static partial void wxsharp_window_show(nint window, [MarshalAs(UnmanagedType.U1)] bool show);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_window_set_title(nint window, string title);

    [LibraryImport(Library)]
    internal static partial void wxsharp_window_layout(nint window);

    [LibraryImport(Library)]
    internal static partial void wxsharp_window_center(nint window);

    [LibraryImport(Library)]
    internal static partial void wxsharp_window_set_fullscreen(nint window, [MarshalAs(UnmanagedType.U1)] bool fullscreen);

    [LibraryImport(Library)]
    internal static partial nint wxsharp_window_native_handle(nint window);

    [LibraryImport(Library)]
    internal static partial void wxsharp_window_close(nint window);

    [LibraryImport(Library)]
    internal static partial void wxsharp_window_destroy(nint window);

    // ---- Dialog ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_dialog_create(string title, int width, int height, int id);

    [LibraryImport(Library)]
    internal static partial nint wxsharp_dialog_panel(nint dialog);

    [LibraryImport(Library)]
    internal static partial void wxsharp_dialog_layout(nint dialog);

    [LibraryImport(Library)]
    internal static partial void wxsharp_dialog_set_escape_id(nint dialog, int id);

    [LibraryImport(Library)]
    internal static partial void wxsharp_dialog_set_affirmative_id(nint dialog, int id);

    [LibraryImport(Library)]
    internal static partial int wxsharp_dialog_show_modal(nint dialog);

    [LibraryImport(Library)]
    internal static partial void wxsharp_dialog_show(nint dialog, [MarshalAs(UnmanagedType.U1)] bool show);

    [LibraryImport(Library)]
    internal static partial void wxsharp_dialog_end_modal(nint dialog, int result);

    [LibraryImport(Library)]
    internal static partial void wxsharp_dialog_destroy(nint dialog);

    // ---- Layout ----
    [LibraryImport(Library)]
    internal static partial nint wxsharp_panel_create(nint parent, [MarshalAs(UnmanagedType.U1)] bool horizontal, int id);

    // ---- Canvas ----
    [LibraryImport(Library)]
    internal static partial nint wxsharp_canvas_create(nint parent, int width, int height,
        [MarshalAs(UnmanagedType.U1)] bool fill, int id);

    [LibraryImport(Library)]
    internal static partial void wxsharp_canvas_clear(nint ctrl, uint argb);

    [LibraryImport(Library)]
    internal static partial void wxsharp_canvas_set_brush(nint ctrl, uint argb);

    [LibraryImport(Library)]
    internal static partial void wxsharp_canvas_set_pen(nint ctrl, uint argb, int width);

    [LibraryImport(Library)]
    internal static partial void wxsharp_canvas_set_text_colour(nint ctrl, uint argb);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_canvas_set_font(nint ctrl, int pointSize, int family, int weight,
        int style, [MarshalAs(UnmanagedType.U1)] bool underline, string face);

    [LibraryImport(Library)]
    internal static partial void wxsharp_canvas_draw_rectangle(nint ctrl, int x, int y, int width, int height);

    [LibraryImport(Library)]
    internal static partial void wxsharp_canvas_draw_rounded_rectangle(nint ctrl, int x, int y, int width, int height, int radius);

    [LibraryImport(Library)]
    internal static partial void wxsharp_canvas_draw_line(nint ctrl, int x1, int y1, int x2, int y2);

    [LibraryImport(Library)]
    internal static partial void wxsharp_canvas_draw_circle(nint ctrl, int x, int y, int radius);

    [LibraryImport(Library)]
    internal static partial void wxsharp_canvas_draw_ellipse(nint ctrl, int x, int y, int width, int height);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_canvas_draw_text(nint ctrl, string text, int x, int y);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_canvas_measure_text(nint ctrl, string text, out int width, out int height);

    // ---- Generic control ----
    [LibraryImport(Library)]
    internal static partial void wxsharp_control_enable(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool enable);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_show(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool show);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_focus(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_destroy(nint ctrl);

    // ---- Generic control: geometry ----
    [LibraryImport(Library)]
    internal static partial void wxsharp_control_get_size(nint ctrl, out int width, out int height);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_set_size(nint ctrl, int width, int height);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_get_client_size(nint ctrl, out int width, out int height);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_get_position(nint ctrl, out int x, out int y);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_set_position(nint ctrl, int x, int y);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_set_min_size(nint ctrl, int width, int height);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_set_max_size(nint ctrl, int width, int height);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_get_best_size(nint ctrl, out int width, out int height);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_fit(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_get_pointer_position(nint ctrl, out int x, out int y);

    // ---- Generic control: appearance ----
    [LibraryImport(Library)]
    internal static partial void wxsharp_control_set_background_colour(nint ctrl, uint argb);

    [LibraryImport(Library)]
    internal static partial uint wxsharp_control_get_background_colour(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_set_foreground_colour(nint ctrl, uint argb);

    [LibraryImport(Library)]
    internal static partial uint wxsharp_control_get_foreground_colour(nint ctrl);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_control_set_font(nint ctrl, int pointSize, int family, int weight,
        int style, [MarshalAs(UnmanagedType.U1)] bool underline, string face);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_control_set_tooltip(nint ctrl, string text);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_set_border(nint ctrl, int border);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_refresh(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool eraseBackground);

    // ---- Generic control: state ----
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_control_is_enabled(nint ctrl);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_control_is_shown(nint ctrl);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_control_has_focus(nint ctrl);

    // ---- Accessibility ----
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_custom_accessibility_available();

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_control_set_name(nint ctrl, string name);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_set_role(nint ctrl, int role);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_control_set_description(nint ctrl, string text);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_control_set_help(nint ctrl, string text);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_control_set_accessible_value(nint ctrl, string text);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_control_set_accessible_keyboard_shortcut(nint ctrl, string text);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_control_set_accessible_default_action(nint ctrl, string text);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_set_accessible_state(nint ctrl, uint state);

    // ---- Sizers ----
    [LibraryImport(Library)]
    internal static partial nint wxsharp_boxsizer_create([MarshalAs(UnmanagedType.U1)] bool horizontal);

    [LibraryImport(Library)]
    internal static partial void wxsharp_sizer_add_control(nint sizer, nint ctrl, int proportion,
        [MarshalAs(UnmanagedType.U1)] bool expand, [MarshalAs(UnmanagedType.U1)] bool center, int border);

    [LibraryImport(Library)]
    internal static partial void wxsharp_sizer_add_sizer(nint sizer, nint child, int proportion,
        [MarshalAs(UnmanagedType.U1)] bool expand, int border);

    [LibraryImport(Library)]
    internal static partial void wxsharp_sizer_add_spacer(nint sizer, int size);

    [LibraryImport(Library)]
    internal static partial void wxsharp_sizer_add_stretch_spacer(nint sizer, int proportion);

    [LibraryImport(Library)]
    internal static partial void wxsharp_window_set_sizer(nint window, nint sizer);

    // ---- Label ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_label_create(nint parent, string text, int style);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_label_set_text(nint ctrl, string text);

    [LibraryImport(Library)]
    internal static partial int wxsharp_label_get_text(nint ctrl, byte* buffer, int bufferLength);

    // ---- Button ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_button_create(nint parent, string label, int id);

    [LibraryImport(Library)]
    internal static partial void wxsharp_button_set_default(nint ctrl);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_button_set_label(nint ctrl, string label);

    [LibraryImport(Library)]
    internal static partial int wxsharp_button_get_label(nint ctrl, byte* buffer, int bufferLength);

    // ---- Text box ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_textbox_create(nint parent, string value,
        [MarshalAs(UnmanagedType.U1)] bool fill, int style, int id);

    [LibraryImport(Library)]
    internal static partial int wxsharp_textbox_get_value(nint ctrl, byte* buffer, int bufferLength);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_textbox_set_value(nint ctrl, string value);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_textbox_append(nint ctrl, string text);

    [LibraryImport(Library)]
    internal static partial void wxsharp_textbox_clear(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_textbox_select_all(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_textbox_set_editable(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool editable);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_textbox_write(nint ctrl, string text);

    [LibraryImport(Library)]
    internal static partial int wxsharp_textbox_length(nint ctrl);

    [LibraryImport(Library)]
    internal static partial int wxsharp_textbox_get_insertion_point(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_textbox_set_insertion_point(nint ctrl, int pos);

    [LibraryImport(Library)]
    internal static partial void wxsharp_textbox_set_insertion_point_end(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_textbox_get_selection(nint ctrl, out int from, out int to);

    [LibraryImport(Library)]
    internal static partial void wxsharp_textbox_set_selection(nint ctrl, int from, int to);

    [LibraryImport(Library)]
    internal static partial int wxsharp_textbox_get_selected_text(nint ctrl, byte* buffer, int bufferLength);

    // ---- Check box ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_checkbox_create(nint parent, string label, int style, int id);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_checkbox_get(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_checkbox_set(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool value);

    // ---- Radio button ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_radio_create(nint parent, string label, [MarshalAs(UnmanagedType.U1)] bool groupStart, int id);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_radio_get(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_radio_set(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool value);

    // ---- Slider ----
    [LibraryImport(Library)]
    internal static partial nint wxsharp_slider_create(nint parent, int minValue, int maxValue, int value, int style, int id);

    [LibraryImport(Library)]
    internal static partial int wxsharp_slider_get(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_slider_set(nint ctrl, int value);

    [LibraryImport(Library)]
    internal static partial int wxsharp_slider_get_min(nint ctrl);

    [LibraryImport(Library)]
    internal static partial int wxsharp_slider_get_max(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_slider_set_range(nint ctrl, int minValue, int maxValue);

    // ---- Choice ----
    [LibraryImport(Library)]
    internal static partial nint wxsharp_choice_create(nint parent, int style, int id);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_choice_append(nint ctrl, string item);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_choice_insert(nint ctrl, string item, int index);

    [LibraryImport(Library)]
    internal static partial void wxsharp_choice_delete(nint ctrl, int index);

    [LibraryImport(Library)]
    internal static partial void wxsharp_choice_clear(nint ctrl);

    [LibraryImport(Library)]
    internal static partial int wxsharp_choice_count(nint ctrl);

    [LibraryImport(Library)]
    internal static partial int wxsharp_choice_get_string(nint ctrl, int index, byte* buffer, int bufferLength);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_choice_set_string(nint ctrl, int index, string text);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int wxsharp_choice_find_string(nint ctrl, string text);

    [LibraryImport(Library)]
    internal static partial int wxsharp_choice_get_selection(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_choice_set_selection(nint ctrl, int index);

    // ---- List box ----
    [LibraryImport(Library)]
    internal static partial nint wxsharp_listbox_create(nint parent, int style, int id);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_listbox_append(nint ctrl, string item);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_listbox_insert(nint ctrl, string item, int index);

    [LibraryImport(Library)]
    internal static partial void wxsharp_listbox_delete(nint ctrl, int index);

    [LibraryImport(Library)]
    internal static partial void wxsharp_listbox_clear(nint ctrl);

    [LibraryImport(Library)]
    internal static partial int wxsharp_listbox_count(nint ctrl);

    [LibraryImport(Library)]
    internal static partial int wxsharp_listbox_get_string(nint ctrl, int index, byte* buffer, int bufferLength);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_listbox_set_string(nint ctrl, int index, string text);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int wxsharp_listbox_find_string(nint ctrl, string text);

    [LibraryImport(Library)]
    internal static partial int wxsharp_listbox_get_selection(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_listbox_set_selection(nint ctrl, int index);

    [LibraryImport(Library)]
    internal static partial int wxsharp_listbox_get_selections(nint ctrl, int* buffer, int bufferLength);

    [LibraryImport(Library)]
    internal static partial void wxsharp_listbox_select(nint ctrl, int index, [MarshalAs(UnmanagedType.U1)] bool select);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_listbox_is_selected(nint ctrl, int index);

    [LibraryImport(Library)]
    internal static partial void wxsharp_listbox_ensure_visible(nint ctrl, int index);

    // ---- Services ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_clipboard_set_text(string text);

    [LibraryImport(Library)]
    internal static partial int wxsharp_clipboard_get_text(byte* buffer, int bufferLength);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_file_dialog(nint parent, string title, string wildcard,
        [MarshalAs(UnmanagedType.U1)] bool save, byte* buffer, int bufferLength);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_dir_dialog(nint parent, string title, string initialDir,
        byte* buffer, int bufferLength);
}
