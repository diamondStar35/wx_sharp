using System.Runtime.InteropServices;

namespace WxSharp;

// Source-generated P/Invoke into the wxsharp native shim (mirrors wxsharp.h). [LibraryImport] keeps the
// marshalling compile-time generated - no IL stubs, no reflection - so the binding stays Native-AOT clean.
// Handles are opaque nints; the event callback is an unmanaged function pointer.
internal static unsafe partial class NativeMethods
{
    private const string Library = "wx";

    // ---- App ----
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_init();

    [LibraryImport(Library)]
    internal static partial void wxsharp_set_event_handler(delegate* unmanaged[Cdecl]<NativeEvent*, uint> cb);

    [LibraryImport(Library)]
    internal static partial void wxsharp_set_accessible_handler(delegate* unmanaged[Cdecl]<NativeAccessibleRequest*, int> cb);

    [LibraryImport(Library)]
    internal static partial void wxsharp_set_virtual_list_handler(delegate* unmanaged[Cdecl]<NativeVirtualListRequest*, byte> cb);

    [LibraryImport(Library)]
    internal static partial int wxsharp_main_loop();

    [LibraryImport(Library)]
    internal static partial void wxsharp_exit_main_loop();

    [LibraryImport(Library)]
    internal static partial void wxsharp_set_exit_on_frame_delete([MarshalAs(UnmanagedType.U1)] bool value);

    [LibraryImport(Library)]
    internal static partial void wxsharp_set_top_window(nint window);

    [LibraryImport(Library)]
    internal static partial void wxsharp_call_after(long token);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_yield([MarshalAs(UnmanagedType.U1)] bool onlyIfNeeded);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int wxsharp_message_box(nint parent, string message, string caption, int style);

    [LibraryImport(Library)]
    internal static partial void wxsharp_shutdown();

    // ---- Window ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_window_create(nint parent, int id, string title, int x, int y,
        int width, int height, int style, long token);

    [LibraryImport(Library)]
    internal static partial void wxsharp_window_show(nint window, [MarshalAs(UnmanagedType.U1)] bool show);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_window_set_title(nint window, string title);

    [LibraryImport(Library)]
    internal static partial int wxsharp_window_get_title(nint window, byte* buffer, int bufferLength);

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
    internal static partial nint wxsharp_dialog_create(nint parent, int id, string title, int x, int y,
        int width, int height, int style, long token);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_dialog_set_title(nint dialog, string title);

    [LibraryImport(Library)]
    internal static partial int wxsharp_dialog_get_title(nint dialog, byte* buffer, int bufferLength);

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
    internal static partial nint wxsharp_panel_create(nint parent, int id, int style, long token);

    // ---- Canvas ----
    [LibraryImport(Library)]
    internal static partial nint wxsharp_canvas_create(nint parent, int id, int width, int height, long token);

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
    internal static partial void wxsharp_control_layout(nint ctrl);

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

    [LibraryImport(Library)]
    internal static partial int wxsharp_control_get_id(nint ctrl);

    // ---- Accessibility ----
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_custom_accessibility_available();

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_control_set_name(nint ctrl, string name);

    [LibraryImport(Library)]
    internal static partial int wxsharp_control_get_name(nint ctrl, byte* buffer, int bufferLength);








    [LibraryImport(Library)]
    internal static partial void wxsharp_control_set_accessible(nint ctrl, long token);

    [LibraryImport(Library)]
    internal static partial void wxsharp_accessible_notify(int eventType, nint window, int objectType, int childId);
    [LibraryImport(Library)] internal static partial uint wxsharp_accessible_probe(nint window);

    // ---- Sizers ----
    [LibraryImport(Library)]
    internal static partial nint wxsharp_boxsizer_create([MarshalAs(UnmanagedType.U1)] bool horizontal);
    [LibraryImport(Library)] internal static partial nint wxsharp_gridsizer_create(int rows, int columns, int verticalGap, int horizontalGap);
    [LibraryImport(Library)] internal static partial nint wxsharp_flexgridsizer_create(int rows, int columns, int verticalGap, int horizontalGap);
    [LibraryImport(Library)] internal static partial void wxsharp_flexgridsizer_add_growable_row(nint sizer, int row, int proportion);
    [LibraryImport(Library)] internal static partial void wxsharp_flexgridsizer_add_growable_column(nint sizer, int column, int proportion);
    [LibraryImport(Library)] internal static partial nint wxsharp_staticboxsizer_create(nint box, [MarshalAs(UnmanagedType.U1)] bool horizontal);
    [LibraryImport(Library)] internal static partial nint wxsharp_gridbagsizer_create(int verticalGap, int horizontalGap);
    [LibraryImport(Library)] internal static partial void wxsharp_gridbagsizer_add_control(nint sizer, nint ctrl, int row, int column, int rowSpan, int columnSpan, int flags, int border);

    [LibraryImport(Library)]
    internal static partial void wxsharp_sizer_add_control(nint sizer, nint ctrl, int proportion, int flags, int border);

    [LibraryImport(Library)]
    internal static partial void wxsharp_sizer_add_sizer(nint sizer, nint child, int proportion, int flags, int border);

    [LibraryImport(Library)]
    internal static partial void wxsharp_sizer_add_spacer(nint sizer, int size);

    [LibraryImport(Library)]
    internal static partial void wxsharp_sizer_add_stretch_spacer(nint sizer, int proportion);

    [LibraryImport(Library)]
    internal static partial void wxsharp_window_set_sizer(nint window, nint sizer);

    // ---- Label ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_label_create(nint parent, int id, string text, int style, long token);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_label_set_text(nint ctrl, string text);

    [LibraryImport(Library)]
    internal static partial int wxsharp_label_get_text(nint ctrl, byte* buffer, int bufferLength);

    // ---- Button ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_button_create(nint parent, int id, string label, long token);

    [LibraryImport(Library)]
    internal static partial void wxsharp_button_set_default(nint ctrl);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_button_set_label(nint ctrl, string label);

    [LibraryImport(Library)]
    internal static partial int wxsharp_button_get_label(nint ctrl, byte* buffer, int bufferLength);

    // ---- Text box ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_textbox_create(nint parent, int id, string value, int style, long token);

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
    internal static partial nint wxsharp_checkbox_create(nint parent, int id, string label, int style, long token);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_checkbox_get(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_checkbox_set(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool value);

    // ---- Radio button ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_radio_create(nint parent, int id, string label,
        [MarshalAs(UnmanagedType.U1)] bool groupStart, long token);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_radio_get(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_radio_set(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool value);

    // ---- Slider ----
    [LibraryImport(Library)]
    internal static partial nint wxsharp_slider_create(nint parent, int id, int minValue, int maxValue,
        int value, int style, long token);

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
    internal static partial nint wxsharp_choice_create(nint parent, int id, int style, long token);

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
    internal static partial nint wxsharp_listbox_create(nint parent, int id, int style, long token);

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

    // ---- Extended common controls ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_togglebutton_create(nint parent, int id, string label, long token);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_togglebutton_get(nint ctrl);
    [LibraryImport(Library)]
    internal static partial void wxsharp_togglebutton_set(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool value);

    [LibraryImport(Library)]
    internal static partial nint wxsharp_gauge_create(nint parent, int id, int range, int value,
        [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_gauge_get(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_gauge_set(nint ctrl, int value);
    [LibraryImport(Library)] internal static partial int wxsharp_gauge_get_range(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_gauge_set_range(nint ctrl, int range);
    [LibraryImport(Library)] internal static partial void wxsharp_gauge_pulse(nint ctrl);

    [LibraryImport(Library)]
    internal static partial nint wxsharp_spinctrl_create(nint parent, int id, int minValue, int maxValue,
        int value, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_spinctrl_get(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrl_set(nint ctrl, int value);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrl_set_range(nint ctrl, int minValue, int maxValue);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_combobox_create(nint parent, int id, string value,
        [MarshalAs(UnmanagedType.U1)] bool readOnly, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_combobox_get_value(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_combobox_set_value(nint ctrl, string value);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_combobox_append(nint ctrl, string value);
    [LibraryImport(Library)] internal static partial void wxsharp_combobox_clear(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_combobox_count(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_combobox_get_selection(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_combobox_set_selection(nint ctrl, int selection);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_searchctrl_create(nint parent, int id, string value, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_searchctrl_get_value(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_searchctrl_set_value(nint ctrl, string value);
    [LibraryImport(Library)] internal static partial void wxsharp_searchctrl_show_cancel(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool show);
    [LibraryImport(Library)] internal static partial void wxsharp_searchctrl_show_search(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool show);

    [LibraryImport(Library)] internal static partial nint wxsharp_checklistbox_create(nint parent, int id, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_checklistbox_append(nint ctrl, string value);
    [LibraryImport(Library)] internal static partial int wxsharp_checklistbox_count(nint ctrl);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_checklistbox_is_checked(nint ctrl, int index);
    [LibraryImport(Library)] internal static partial void wxsharp_checklistbox_check(nint ctrl, int index, [MarshalAs(UnmanagedType.U1)] bool value);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_radiobox_create(nint parent, int id, string label, nint* choices,
        int count, int columns, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_radiobox_get_selection(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_radiobox_set_selection(nint ctrl, int selection);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_staticbox_create(nint parent, int id, string label, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_staticline_create(nint parent, int id, [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_activity_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_activity_start(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_activity_stop(nint ctrl);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_activity_is_running(nint ctrl);
    [LibraryImport(Library)] internal static partial nint wxsharp_spinctrldouble_create(nint parent, int id, double minValue, double maxValue, double value, double increment, long token);
    [LibraryImport(Library)] internal static partial double wxsharp_spinctrldouble_get(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrldouble_set(nint ctrl, double value);
    [LibraryImport(Library)] internal static partial nint wxsharp_scrollbar_create(nint parent, int id, [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_scrollbar_set(nint ctrl, int position, int thumbSize, int range, int pageSize);
    [LibraryImport(Library)] internal static partial int wxsharp_scrollbar_get_position(nint ctrl);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_hyperlink_create(nint parent, int id, string label, string url, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_hyperlink_get_url(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_hyperlink_set_url(nint ctrl, string url);
    [LibraryImport(Library)] internal static partial nint wxsharp_datepicker_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_timepicker_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_datetime_get(nint ctrl, out int year, out int month, out int day, out int hour, out int minute, out int second);
    [LibraryImport(Library)] internal static partial void wxsharp_datetime_set(nint ctrl, int year, int month, int day, int hour, int minute, int second);

    // ---- Containers ----
    [LibraryImport(Library)] internal static partial nint wxsharp_scrolled_create(nint parent, int id, int style, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_set_rate(nint ctrl, int xStep, int yStep);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_scroll(nint ctrl, int x, int y);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_get_view_start(nint ctrl, out int x, out int y);
    [LibraryImport(Library)] internal static partial nint wxsharp_splitter_create(nint parent, int id, [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_splitter_split(nint ctrl, nint first, nint second, int position);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_splitter_unsplit(nint ctrl, nint remove);
    [LibraryImport(Library)] internal static partial int wxsharp_splitter_get_position(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_splitter_set_position(nint ctrl, int position);
    [LibraryImport(Library)] internal static partial nint wxsharp_notebook_create(nint parent, int id, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_notebook_add_page(nint ctrl, nint page, string text, [MarshalAs(UnmanagedType.U1)] bool select);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_notebook_delete_page(nint ctrl, int page);
    [LibraryImport(Library)] internal static partial int wxsharp_notebook_count(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_notebook_get_selection(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_notebook_set_selection(nint ctrl, int page);
    [LibraryImport(Library)] internal static partial int wxsharp_notebook_get_page_text(nint ctrl, int page, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_notebook_set_page_text(nint ctrl, int page, string text);
    [LibraryImport(Library)] internal static partial nint wxsharp_simplebook_create(nint parent, int id, long token);

    // ---- Data controls ----
    [LibraryImport(Library)] internal static partial nint wxsharp_listctrl_create(nint parent, int id, int style, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int wxsharp_listctrl_insert_column(nint ctrl, int column, string heading, int width);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial long wxsharp_listctrl_insert_item(nint ctrl, long index, string text);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_listctrl_set_item(nint ctrl, long item, int column, string text);
    [LibraryImport(Library)] internal static partial int wxsharp_listctrl_get_item(nint ctrl, long item, int column, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial long wxsharp_listctrl_count(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_listctrl_delete_item(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial void wxsharp_listctrl_clear(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_listctrl_select(nint ctrl, long item, [MarshalAs(UnmanagedType.U1)] bool select);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_listctrl_is_selected(nint ctrl, long item);

    [LibraryImport(Library)] internal static partial nint wxsharp_treectrl_create(nint parent, int id, int style, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial long wxsharp_tree_add_root(nint ctrl, string text);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial long wxsharp_tree_append(nint ctrl, long parent, string text);
    [LibraryImport(Library)] internal static partial void wxsharp_tree_delete(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial void wxsharp_tree_delete_all(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_tree_get_text(nint ctrl, long item, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_tree_set_text(nint ctrl, long item, string text);
    [LibraryImport(Library)] internal static partial void wxsharp_tree_expand(nint ctrl, long item, [MarshalAs(UnmanagedType.U1)] bool expand);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_tree_is_expanded(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial void wxsharp_tree_select(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial long wxsharp_tree_get_selection(nint ctrl);

    [LibraryImport(Library)] internal static partial nint wxsharp_grid_create(nint parent, int id, int rows, int columns, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_grid_rows(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_grid_columns(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_grid_append_rows(nint ctrl, int count);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_grid_append_columns(nint ctrl, int count);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_grid_delete_rows(nint ctrl, int position, int count);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_grid_delete_columns(nint ctrl, int position, int count);
    [LibraryImport(Library)] internal static partial int wxsharp_grid_get_value(nint ctrl, int row, int column, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_grid_set_value(nint ctrl, int row, int column, string value);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_grid_set_row_label(nint ctrl, int row, string value);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_grid_set_column_label(nint ctrl, int column, string value);
    [LibraryImport(Library)] internal static partial nint wxsharp_dataviewlist_create(nint parent, int id, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_dataviewlist_append_text_column(nint ctrl, string label, int width, [MarshalAs(UnmanagedType.U1)] bool editable);
    [LibraryImport(Library)] internal static partial void wxsharp_dataviewlist_append_row(nint ctrl, nint* values, int count);
    [LibraryImport(Library)] internal static partial int wxsharp_dataviewlist_count(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_dataviewlist_get_value(nint ctrl, int row, int column, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_dataviewlist_set_value(nint ctrl, int row, int column, string value);
    [LibraryImport(Library)] internal static partial void wxsharp_dataviewlist_delete_row(nint ctrl, int row);
    [LibraryImport(Library)] internal static partial void wxsharp_dataviewlist_clear(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_dataviewlist_get_selection(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_dataviewlist_set_selection(nint ctrl, int row);
    [LibraryImport(Library)] internal static partial nint wxsharp_dataviewtree_create(nint parent, int id, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial long wxsharp_dataviewtree_append_container(nint ctrl, long parent, string text);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial long wxsharp_dataviewtree_append_item(nint ctrl, long parent, string text);
    [LibraryImport(Library)] internal static partial int wxsharp_dataviewtree_get_text(nint ctrl, long item, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_dataviewtree_set_text(nint ctrl, long item, string text);
    [LibraryImport(Library)] internal static partial void wxsharp_dataviewtree_delete(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial void wxsharp_dataviewtree_clear(nint ctrl);
    [LibraryImport(Library)] internal static partial long wxsharp_dataviewtree_get_selection(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_dataviewtree_set_selection(nint ctrl, long item);

    // ---- Menus and frame chrome ----
    // ---- Dialog button sizer ----
    [LibraryImport(Library)] internal static partial nint wxsharp_dialog_create_button_sizer(nint dialog, int flags);

    // ---- wxListCtrl columns, focus and selection ----
    [LibraryImport(Library)] internal static partial int wxsharp_listctrl_column_count(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_listctrl_delete_column(nint ctrl, int column);
    [LibraryImport(Library)] internal static partial void wxsharp_listctrl_clear_columns(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_listctrl_get_column_width(nint ctrl, int column);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_listctrl_set_column_width(nint ctrl, int column, int width);
    [LibraryImport(Library)] internal static partial int wxsharp_listctrl_get_column_heading(nint ctrl, int column, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_listctrl_set_column_heading(nint ctrl, int column, string heading);
    [LibraryImport(Library)] internal static partial void wxsharp_listctrl_ensure_visible(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial long wxsharp_listctrl_get_focused(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_listctrl_set_focused(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial int wxsharp_listctrl_selected_count(nint ctrl);
    [LibraryImport(Library)] internal static partial long wxsharp_listctrl_next_selected(nint ctrl, long after);

    // ---- wxTreeCtrl navigation ----
    [LibraryImport(Library)] internal static partial void wxsharp_tree_unselect(nint ctrl);
    [LibraryImport(Library)] internal static partial long wxsharp_tree_get_root(nint ctrl);
    [LibraryImport(Library)] internal static partial long wxsharp_tree_get_parent(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial long wxsharp_tree_get_first_child(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial long wxsharp_tree_get_next_sibling(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial long wxsharp_tree_get_prev_sibling(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial int wxsharp_tree_child_count(nint ctrl, long item, [MarshalAs(UnmanagedType.U1)] bool recursive);
    [LibraryImport(Library)] internal static partial void wxsharp_tree_ensure_visible(nint ctrl, long item);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial long wxsharp_tree_insert(nint ctrl, long parent, int position, string text);

    // ---- wxComboBox items ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_combobox_insert(nint ctrl, string value, int index);
    [LibraryImport(Library)] internal static partial void wxsharp_combobox_delete(nint ctrl, int index);
    [LibraryImport(Library)] internal static partial int wxsharp_combobox_get_string(nint ctrl, int index, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_combobox_set_string(nint ctrl, int index, string text);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_combobox_find_string(nint ctrl, string text);

    // ---- wxTextCtrl lines ----
    [LibraryImport(Library)] internal static partial int wxsharp_textbox_line_count(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_textbox_line_length(nint ctrl, int line);
    [LibraryImport(Library)] internal static partial int wxsharp_textbox_get_line_text(nint ctrl, int line, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial void wxsharp_textbox_show_position(nint ctrl, int position);

    // ---- Event binding ----
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_bind(nint window, int eventId, long token);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_unbind(nint window, int eventId);
    [LibraryImport(Library)] internal static partial void wxsharp_window_unbind_all(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_event_propagates(int eventId);

    // ---- Virtual list controls ----
    [LibraryImport(Library)] internal static partial void wxsharp_listctrl_set_item_count(nint ctrl, long count);
    [LibraryImport(Library)] internal static partial void wxsharp_listctrl_refresh_item(nint ctrl, long item);
    [LibraryImport(Library)] internal static partial void wxsharp_listctrl_refresh_items(nint ctrl, long from, long to);

    // ---- Check box third state ----
    [LibraryImport(Library)] internal static partial int wxsharp_checkbox_get_3state(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_checkbox_set_3state(nint ctrl, int state);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_checkbox_is_3state(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_checkbox_is_3rd_state_allowed_for_user(nint ctrl);

    // ---- Update UI, dropped files, hot keys ----
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_updateui_enable([MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_updateui_check([MarshalAs(UnmanagedType.U1)] bool check);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_updateui_show([MarshalAs(UnmanagedType.U1)] bool show);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_updateui_set_text(string text);
    [LibraryImport(Library)] internal static partial void wxsharp_updateui_set_interval(int milliseconds);
    [LibraryImport(Library)] internal static partial void wxsharp_updateui_set_process_all([MarshalAs(UnmanagedType.U1)] bool processAll);
    [LibraryImport(Library)] internal static partial void wxsharp_window_update_ui(nint window, [MarshalAs(UnmanagedType.U1)] bool recurse);
    [LibraryImport(Library)] internal static partial void wxsharp_window_accept_dropped_files(nint window, [MarshalAs(UnmanagedType.U1)] bool accept);
    [LibraryImport(Library)] internal static partial void wxsharp_window_capture_mouse(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_release_mouse(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_has_capture(nint window);
    [LibraryImport(Library)] internal static partial int wxsharp_dropfiles_count();
    [LibraryImport(Library)] internal static partial int wxsharp_dropfiles_path(int index, byte* buffer, int bufferLength);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_register_hotkey(nint window, int hotKeyId, int modifiers, int keyCode);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_unregister_hotkey(nint window, int hotKeyId);

    // ---- Menus ----
    [LibraryImport(Library)] internal static partial nint wxsharp_menu_create();
    [LibraryImport(Library)] internal static partial void wxsharp_menu_destroy(nint menu);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_menu_append(nint menu, int id, string text, string help, int kind);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_menu_insert(nint menu, int position, int id, string text, string help, int kind);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_menu_append_submenu(nint menu, int id, string text, nint submenu, string help);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_menu_insert_submenu(nint menu, int position, int id, string text, nint submenu, string help);
    [LibraryImport(Library)] internal static partial nint wxsharp_menu_append_separator(nint menu);
    [LibraryImport(Library)] internal static partial nint wxsharp_menu_insert_separator(nint menu, int position);
    [LibraryImport(Library)] internal static partial int wxsharp_menu_count(nint menu);
    [LibraryImport(Library)] internal static partial nint wxsharp_menu_item_at(nint menu, int position);
    [LibraryImport(Library)] internal static partial nint wxsharp_menu_find_item(nint menu, int id);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_menu_remove(nint menu, nint item);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_menu_delete(nint menu, nint item);
    [LibraryImport(Library)] internal static partial void wxsharp_menu_enable(nint menu, int id, [MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)] internal static partial void wxsharp_menu_check(nint menu, int id, [MarshalAs(UnmanagedType.U1)] bool check);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_menu_is_checked(nint menu, int id);
    [LibraryImport(Library)] internal static partial int wxsharp_menu_get_title(nint menu, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_menu_set_title(nint menu, string title);

    // ---- Menu items ----
    [LibraryImport(Library)] internal static partial int wxsharp_menuitem_get_id(nint item);
    [LibraryImport(Library)] internal static partial int wxsharp_menuitem_get_kind(nint item);
    [LibraryImport(Library)] internal static partial int wxsharp_menuitem_get_label(nint item, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_menuitem_set_label(nint item, string label);
    [LibraryImport(Library)] internal static partial int wxsharp_menuitem_get_help(nint item, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_menuitem_set_help(nint item, string help);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_menuitem_is_enabled(nint item);
    [LibraryImport(Library)] internal static partial void wxsharp_menuitem_enable(nint item, [MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_menuitem_is_checked(nint item);
    [LibraryImport(Library)] internal static partial void wxsharp_menuitem_check(nint item, [MarshalAs(UnmanagedType.U1)] bool check);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_menuitem_is_checkable(nint item);
    [LibraryImport(Library)] internal static partial nint wxsharp_menuitem_get_submenu(nint item);
    [LibraryImport(Library)] internal static partial void wxsharp_menuitem_set_bitmap(nint item, nint bitmap);

    // ---- Menu bar ----
    [LibraryImport(Library)] internal static partial nint wxsharp_menubar_create();
    [LibraryImport(Library)] internal static partial void wxsharp_menubar_destroy(nint menuBar);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_menubar_append(nint menuBar, nint menu, string title);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_menubar_insert(nint menuBar, int position, nint menu, string title);
    [LibraryImport(Library)] internal static partial nint wxsharp_menubar_remove(nint menuBar, int position);
    [LibraryImport(Library)] internal static partial int wxsharp_menubar_count(nint menuBar);
    [LibraryImport(Library)] internal static partial nint wxsharp_menubar_menu_at(nint menuBar, int position);
    [LibraryImport(Library)] internal static partial void wxsharp_menubar_enable_top(nint menuBar, int position, [MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)] internal static partial int wxsharp_menubar_get_label_top(nint menuBar, int position, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_menubar_set_label_top(nint menuBar, int position, string label);
    [LibraryImport(Library)] internal static partial nint wxsharp_menubar_find_item(nint menuBar, int id);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_set_menubar(nint frame, nint menuBar);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_update_menus(nint frame);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_popup_menu(nint window, nint menu, int x, int y);
    [LibraryImport(Library)] internal static partial nint wxsharp_statusbar_create(nint frame, int fields, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_statusbar_set_text(nint status, string text, int field);
    [LibraryImport(Library)] internal static partial int wxsharp_statusbar_get_text(nint status, int field, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial nint wxsharp_toolbar_create(nint frame, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_toolbar_add_tool(nint toolbar, int id, string label, string help, int kind);
    [LibraryImport(Library)] internal static partial void wxsharp_toolbar_add_separator(nint toolbar);
    [LibraryImport(Library)] internal static partial void wxsharp_toolbar_realize(nint toolbar);
    [LibraryImport(Library)] internal static partial void wxsharp_toolbar_enable(nint toolbar, int id, [MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)] internal static partial void wxsharp_toolbar_toggle(nint toolbar, int id, [MarshalAs(UnmanagedType.U1)] bool toggle);
    // ---- Accelerators and identifiers ----
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_accelerators(nint window, NativeAccelerator* entries, int count);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_accelerator_parse(string text, out int modifiers, out int keyCode);
    [LibraryImport(Library)] internal static partial int wxsharp_accelerator_format(int modifiers, int keyCode, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_new_id();
    [LibraryImport(Library)] internal static partial void wxsharp_release_id(int id);
    [LibraryImport(Library)] internal static partial int wxsharp_stock_id(int which);
    [LibraryImport(Library)] internal static partial nint wxsharp_timer_create(int id, long ownerToken);
    [LibraryImport(Library)] internal static partial void wxsharp_timer_destroy(nint timer);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_timer_start(nint timer, int milliseconds, [MarshalAs(UnmanagedType.U1)] bool oneShot);
    [LibraryImport(Library)] internal static partial void wxsharp_timer_stop(nint timer);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_timer_is_running(nint timer);
    [LibraryImport(Library)] internal static partial int wxsharp_timer_get_interval(nint timer);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_image_load(string path);
    [LibraryImport(Library)] internal static partial void wxsharp_image_destroy(nint image);
    [LibraryImport(Library)] internal static partial int wxsharp_image_width(nint image);
    [LibraryImport(Library)] internal static partial int wxsharp_image_height(nint image);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_image_save(nint image, string path);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_bitmap_load(string path);
    [LibraryImport(Library)] internal static partial nint wxsharp_bitmap_from_image(nint image);
    [LibraryImport(Library)] internal static partial void wxsharp_bitmap_destroy(nint bitmap);
    [LibraryImport(Library)] internal static partial int wxsharp_bitmap_width(nint bitmap);
    [LibraryImport(Library)] internal static partial int wxsharp_bitmap_height(nint bitmap);
    [LibraryImport(Library)] internal static partial nint wxsharp_staticbitmap_create(nint parent, int id, nint bitmap, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_staticbitmap_set(nint ctrl, nint bitmap);
    [LibraryImport(Library)] internal static partial nint wxsharp_bitmapbutton_create(nint parent, int id, nint bitmap, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_icon_load(string path);
    [LibraryImport(Library)] internal static partial void wxsharp_icon_destroy(nint icon);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_set_icon(nint frame, nint icon);
    [LibraryImport(Library)] internal static partial void wxsharp_begin_busy_cursor();
    [LibraryImport(Library)] internal static partial void wxsharp_end_busy_cursor();
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_progress_create(nint parent, string title, string message, int maximum);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_progress_update(nint progress, int value, string message, [MarshalAs(UnmanagedType.U1)] out bool continueRunning);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_progress_pulse(nint progress, string message, [MarshalAs(UnmanagedType.U1)] out bool continueRunning);
    [LibraryImport(Library)] internal static partial void wxsharp_progress_destroy(nint progress);

    // ---- Services ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_clipboard_set_text(string text);

    [LibraryImport(Library)]
    internal static partial int wxsharp_clipboard_get_text(byte* buffer, int bufferLength);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int wxsharp_file_dialog(nint parent, string title, string wildcard,
        string defaultDir, string defaultFile, int style);

    [LibraryImport(Library)]
    internal static partial int wxsharp_file_dialog_result(int index, byte* buffer, int bufferLength);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_dir_dialog(nint parent, string title, string initialDir,
        byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_text_entry_dialog(nint parent, string message, string caption,
        string value, [MarshalAs(UnmanagedType.U1)] bool password, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_number_entry_dialog(nint parent, string message, string prompt,
        string caption, long value, long minimum, long maximum, out long result);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_colour_dialog(nint parent, uint initial, out uint result);
}
