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
    internal static partial void wxsharp_set_virtual_handler(delegate* unmanaged[Cdecl]<NativeVirtualRequest*, void> cb);

    [LibraryImport(Library)]
    internal static partial void wxsharp_window_call_base(nint window, NativeVirtualRequest* request);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_frame_create(nint parent, int id, string title, int x, int y,
                                                            int width, int height, int style, long token);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int wxsharp_post_command_event(nint window, int eventId, int id, int value,
        string text, [MarshalAs(UnmanagedType.U1)] bool processNow);

    [LibraryImport(Library)]
    internal static partial nint wxsharp_custom_panel_create(nint parent, int id, int style, long token);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_button_create(nint parent, int id, string label, long token);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_dialog_create(nint parent, int id, string title, int x, int y,
                                                             int width, int height, int style, long token);

    [LibraryImport(Library)]
    internal static partial int wxsharp_main_loop();

    [LibraryImport(Library)]
    internal static partial void wxsharp_exit_main_loop();

    [LibraryImport(Library)]
    internal static partial void wxsharp_set_exit_on_frame_delete([MarshalAs(UnmanagedType.U1)] bool value);

    [LibraryImport(Library)]
    internal static partial void wxsharp_set_top_window(nint window);

    [LibraryImport(Library)] internal static partial int wxsharp_app_set_appearance(int appearance);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_app_enable_dark_mode(int flags);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_app_supports_dark_mode();

    // ---- Platform services -------------------------------------------------------------------------
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_executable(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_config_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_user_config_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_data_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_local_data_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_user_data_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_user_local_data_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_plugins_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_resources_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_documents_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_temp_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_app_documents_dir(byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_stdpaths_user_dir(int which, byte* buffer, int length);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_stdpaths_localized_resources_dir(string language, int category, byte* buffer, int length);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_sound_create(string path);
    [LibraryImport(Library)] internal static partial void wxsharp_sound_destroy(nint sound);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sound_is_ok(nint sound);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sound_play(nint sound, uint flags);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sound_play_file(string path, uint flags);
    [LibraryImport(Library)] internal static partial void wxsharp_sound_stop();

    [LibraryImport(Library)] internal static partial uint wxsharp_display_count();
    [LibraryImport(Library)] internal static partial int wxsharp_display_from_point(int x, int y);
    [LibraryImport(Library)] internal static partial int wxsharp_display_from_window(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_display_geometry(uint index, out int x, out int y, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_display_client_area(uint index, out int x, out int y, out int width, out int height);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_display_is_primary(uint index);
    [LibraryImport(Library)] internal static partial int wxsharp_display_name(uint index, byte* buffer, int length);
    [LibraryImport(Library)] internal static partial double wxsharp_display_scale_factor(uint index);
    [LibraryImport(Library)] internal static partial void wxsharp_display_ppi(uint index, out int x, out int y);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_art_bitmap(string id, string client, int width, int height);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_art_icon(string id, string client, int width, int height);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_art_native_size(string client, nint window, out int width, out int height);

    [LibraryImport(Library)] internal static partial nint wxsharp_cursor_create_stock(int id);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_cursor_create_from_file(string path, int type, int hotspotX, int hotspotY);
    [LibraryImport(Library)] internal static partial void wxsharp_cursor_destroy(nint cursor);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_cursor_is_ok(nint cursor);
    [LibraryImport(Library)] internal static partial void wxsharp_control_set_cursor(nint ctrl, nint cursor);
    [LibraryImport(Library)] internal static partial nint wxsharp_control_get_cursor(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_cursor_set_global(nint cursor);

    [LibraryImport(Library)] internal static partial nint wxsharp_imagelist_create(int width, int height, [MarshalAs(UnmanagedType.U1)] bool mask, int initialCount);
    [LibraryImport(Library)] internal static partial void wxsharp_imagelist_destroy(nint list);
    [LibraryImport(Library)] internal static partial int wxsharp_imagelist_count(nint list);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_imagelist_remove(nint list, int index);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_imagelist_remove_all(nint list);
    [LibraryImport(Library)] internal static partial int wxsharp_imagelist_add_bitmap(nint list, nint bitmap);
    [LibraryImport(Library)] internal static partial int wxsharp_imagelist_add_icon(nint list, nint icon);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_imagelist_replace(nint list, int index, nint bitmap);
    [LibraryImport(Library)] internal static partial void wxsharp_imagelist_size(nint list, int index, out int width, out int height);
    [LibraryImport(Library)] internal static partial nint wxsharp_imagelist_get_bitmap(nint list, int index);
    [LibraryImport(Library)] internal static partial void wxsharp_listctrl_set_image_list(nint ctrl, nint list, int which, [MarshalAs(UnmanagedType.U1)] bool transfer);
    [LibraryImport(Library)] internal static partial void wxsharp_treectrl_set_image_list(nint ctrl, nint list, [MarshalAs(UnmanagedType.U1)] bool transfer);
    [LibraryImport(Library)] internal static partial void wxsharp_listctrl_set_item_image(nint ctrl, long item, int image);
    [LibraryImport(Library)] internal static partial void wxsharp_treectrl_set_item_image(nint ctrl, long item, int image, int which);
    [LibraryImport(Library)] internal static partial int wxsharp_treectrl_get_item_image(nint ctrl, long item, int which);

    [LibraryImport(Library)] internal static partial void wxsharp_control_set_caret(nint ctrl, int width, int height);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_control_has_caret(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_caret_move(nint ctrl, int x, int y);
    [LibraryImport(Library)] internal static partial void wxsharp_caret_show(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool show);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_caret_is_visible(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_caret_position(nint ctrl, out int x, out int y);
    [LibraryImport(Library)] internal static partial int wxsharp_caret_get_blink_time();
    [LibraryImport(Library)] internal static partial void wxsharp_caret_set_blink_time(int milliseconds);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_about_box(string name, string version, string description, string copyright, string website, string websiteLabel, byte** developers, int developerCount, nint parent);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_rich_tooltip_show(nint window, string title, string message, int icon, int timeoutMs, int showDelayMs);

    // ---- wxWindow, continued -----------------------------------------------------------------------
    [LibraryImport(Library)] internal static partial nint wxsharp_window_find_focus();
    [LibraryImport(Library)] internal static partial nint wxsharp_window_find_by_id(int id, nint parent);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_find_child_by_id(nint window, int id);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_window_find_child_by_name(nint window, string name);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_get_capture();
    [LibraryImport(Library)] internal static partial int wxsharp_window_new_control_id(int count);
    [LibraryImport(Library)] internal static partial void wxsharp_window_unreserve_control_id(int id, int count);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_top_level_parent(nint window);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_grand_parent(nint window);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_next_sibling(nint window);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_prev_sibling(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_reparent(nint window, nint parent);
    [LibraryImport(Library)] internal static partial void wxsharp_window_destroy_children(nint window);
    [LibraryImport(Library)] internal static partial int wxsharp_window_child_count(nint window);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_child_at(nint window, int index);
    [LibraryImport(Library)] internal static partial void wxsharp_window_move_before_in_tab_order(nint window, nint other);
    [LibraryImport(Library)] internal static partial void wxsharp_window_move_after_in_tab_order(nint window, nint other);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_can_accept_focus(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_can_accept_focus_from_keyboard(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_can_be_focused(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_is_focusable(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_disable_focus_from_keyboard(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_push_event_handler(nint window, nint handler);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_pop_event_handler(nint window, [MarshalAs(UnmanagedType.U1)] bool deleteHandler);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_remove_event_handler(nint window, nint handler);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_get_event_handler(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_event_handler(nint window, nint handler);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_extra_style(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_extra_style(nint window, int style);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_has_extra_style(nint window, int flag);
    [LibraryImport(Library)] internal static partial void wxsharp_window_toggle_style(nint window, int flag);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_get_theme_enabled(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_theme_enabled(nint window, [MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_is_retained(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_is_this_enabled(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_initial_size(nint window, int width, int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_invalidate_best_size(nint window);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_best_height(nint window, int width);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_best_width(nint window, int height);
    [LibraryImport(Library)] internal static partial double wxsharp_window_content_scale_factor(nint window);
    [LibraryImport(Library)] internal static partial double wxsharp_window_dpi_scale_factor(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_client_to_window_size(nint window, int width, int height, out int outW, out int outH);
    [LibraryImport(Library)] internal static partial void wxsharp_window_window_to_client_size(nint window, int width, int height, out int outW, out int outH);
    [LibraryImport(Library)] internal static partial void wxsharp_window_from_phys(nint window, int width, int height, out int outW, out int outH);
    [LibraryImport(Library)] internal static partial void wxsharp_window_to_phys(nint window, int width, int height, out int outW, out int outH);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_can_scroll(nint window, int orientation);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_is_exposed(nint window, int x, int y, int width, int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_update_client_rect(nint window, out int x, out int y, out int width, out int height);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_show_with_effect(nint window, int effect, uint milliseconds);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_hide_with_effect(nint window, int effect, uint milliseconds);
    [LibraryImport(Library)] internal static partial void wxsharp_window_enable_touch_events(nint window, int events);

    // ---- Common dialogs ----------------------------------------------------------------------------
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_filedlg_create(nint parent, string message, string directory, string file, string wildcard, int style, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_filedlg_get_path(nint dlg, byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_filedlg_get_directory(nint dlg, byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_filedlg_get_filename(nint dlg, byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_filedlg_get_wildcard(nint dlg, byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_filedlg_get_message(nint dlg, byte* buffer, int length);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_filedlg_set_path(nint dlg, string path);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_filedlg_set_directory(nint dlg, string dir);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_filedlg_set_filename(nint dlg, string name);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_filedlg_set_wildcard(nint dlg, string wildcard);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_filedlg_set_message(nint dlg, string message);
    [LibraryImport(Library)] internal static partial int wxsharp_filedlg_get_filter_index(nint dlg);
    [LibraryImport(Library)] internal static partial void wxsharp_filedlg_set_filter_index(nint dlg, int index);
    [LibraryImport(Library)] internal static partial int wxsharp_filedlg_path_count(nint dlg);
    [LibraryImport(Library)] internal static partial int wxsharp_filedlg_path_at(nint dlg, int index, byte* buffer, int length);
    [LibraryImport(Library)] internal static partial int wxsharp_filedlg_filename_at(nint dlg, int index, byte* buffer, int length);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_dirdlg_create(nint parent, string message, string defaultPath, int style, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_dirdlg_get_path(nint dlg, byte* buffer, int length);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_dirdlg_set_path(nint dlg, string path);
    [LibraryImport(Library)] internal static partial int wxsharp_dirdlg_get_message(nint dlg, byte* buffer, int length);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_dirdlg_set_message(nint dlg, string message);
    [LibraryImport(Library)] internal static partial int wxsharp_dirdlg_path_count(nint dlg);
    [LibraryImport(Library)] internal static partial int wxsharp_dirdlg_path_at(nint dlg, int index, byte* buffer, int length);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_textdlg_create(nint parent, string message, string caption, string value, int style, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_textdlg_get_value(nint dlg, byte* buffer, int length);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_textdlg_set_value(nint dlg, string value);
    [LibraryImport(Library)] internal static partial void wxsharp_textdlg_set_max_length(nint dlg, ulong length);
    [LibraryImport(Library)] internal static partial void wxsharp_textdlg_force_upper(nint dlg);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_numdlg_create(nint parent, string message, string prompt, string caption, long value, long minimum, long maximum, long token);
    [LibraryImport(Library)] internal static partial long wxsharp_numdlg_get_value(nint dlg);

    [LibraryImport(Library)] internal static partial nint wxsharp_colourdlg_create(nint parent, uint initial, [MarshalAs(UnmanagedType.U1)] bool full, long token);
    [LibraryImport(Library)] internal static partial uint wxsharp_colourdlg_get_colour(nint dlg);
    [LibraryImport(Library)] internal static partial void wxsharp_colourdlg_set_colour(nint dlg, uint colour);
    [LibraryImport(Library)] internal static partial uint wxsharp_colourdlg_get_custom(nint dlg, int index);
    [LibraryImport(Library)] internal static partial void wxsharp_colourdlg_set_custom(nint dlg, int index, uint colour);

    [LibraryImport(Library)] internal static partial nint wxsharp_fontdlg_create(nint parent, nint initial, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_fontdlg_get_font(nint dlg);
    [LibraryImport(Library)] internal static partial uint wxsharp_fontdlg_get_colour(nint dlg);
    [LibraryImport(Library)] internal static partial void wxsharp_fontdlg_set_colour(nint dlg, uint colour);
    [LibraryImport(Library)] internal static partial void wxsharp_fontdlg_enable_effects(nint dlg, [MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)] internal static partial void wxsharp_fontdlg_set_range(nint dlg, int minimum, int maximum);



    // The developer list is the only array of strings this ABI carries, so it is marshalled by hand rather
    // than teaching the generator a shape used once.
    internal static unsafe void ShowAboutBox(AboutInfo info, Window? parent)
    {
        var developers = info.Developers;
        var handles = new System.Runtime.InteropServices.GCHandle[developers.Length];
        var pointers = stackalloc byte*[developers.Length == 0 ? 1 : developers.Length];
        try
        {
            for (var i = 0; i < developers.Length; i++)
            {
                var utf8 = System.Text.Encoding.UTF8.GetBytes((developers[i] ?? string.Empty) + "\0");
                handles[i] = System.Runtime.InteropServices.GCHandle.Alloc(
                    utf8, System.Runtime.InteropServices.GCHandleType.Pinned);
                pointers[i] = (byte*)handles[i].AddrOfPinnedObject();
            }
            wxsharp_about_box(info.Name, info.Version, info.Description, info.Copyright, info.WebSite,
                info.WebSiteLabel, pointers, developers.Length, parent?.Handle ?? 0);
        }
        finally
        {
            foreach (var handle in handles)
                if (handle.IsAllocated) handle.Free();
        }
    }


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

        [LibraryImport(Library)] internal static partial void wxsharp_canvas_set_font(nint ctrl, nint font);

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
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_control_accepts_focus(nint ctrl);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_control_accepts_focus_from_keyboard(nint ctrl);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_control_accepts_focus_recursively(nint ctrl);
        [LibraryImport(Library)] internal static partial nint wxsharp_control_get_font(nint ctrl);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_control_has_flag(nint ctrl, int flag);

    [LibraryImport(Library)]
    internal static partial void wxsharp_control_layout(nint ctrl);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_control_destroy(nint ctrl);

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

        [LibraryImport(Library)] internal static partial void wxsharp_control_set_font(nint ctrl, nint font);
    // ---- Font --------------------------------------------------------------------------------------
    [LibraryImport(Library)] internal static partial nint wxsharp_font_create_empty();
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_font_create(double pointSize, int pixelWidth, int pixelHeight, [MarshalAs(UnmanagedType.U1)] bool usePixels, int family, int style, int weight, [MarshalAs(UnmanagedType.U1)] bool underlined, [MarshalAs(UnmanagedType.U1)] bool strikethrough, string face, int encoding, int flags);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_font_create_from_native(string nativeInfo);
    [LibraryImport(Library)] internal static partial nint wxsharp_font_copy(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_destroy(nint font);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_is_ok(nint font);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_equals(nint a, nint b);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_point_size(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_point_size(nint font, int size);
    [LibraryImport(Library)] internal static partial double wxsharp_font_get_fractional_point_size(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_fractional_point_size(nint font, double size);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_is_using_size_in_pixels(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_get_pixel_size(nint font, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_pixel_size(nint font, int width, int height);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_symbolic_size(nint font, int size);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_symbolic_size_relative_to(nint font, int size, int basePointSize);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_family(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_family(nint font, int family);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_style(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_style(nint font, int style);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_numeric_weight(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_numeric_weight(nint font, int weight);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_weight(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_weight(nint font, int weight);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_get_underlined(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_underlined(nint font, [MarshalAs(UnmanagedType.U1)] bool value);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_get_strikethrough(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_strikethrough(nint font, [MarshalAs(UnmanagedType.U1)] bool value);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_encoding(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_encoding(nint font, int encoding);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_is_fixed_width(nint font);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_face_name(nint font, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_set_face_name(nint font, string face);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_native_info(nint font, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_native_info_user_desc(nint font, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_set_native_info(nint font, string description);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_set_native_info_user_desc(nint font, string description);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_family_string(nint font, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_style_string(nint font, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_weight_string(nint font, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial nint wxsharp_font_bold(nint font);
    [LibraryImport(Library)] internal static partial nint wxsharp_font_italic(nint font);
    [LibraryImport(Library)] internal static partial nint wxsharp_font_underlined(nint font);
    [LibraryImport(Library)] internal static partial nint wxsharp_font_strikethrough(nint font);
    [LibraryImport(Library)] internal static partial nint wxsharp_font_larger(nint font);
    [LibraryImport(Library)] internal static partial nint wxsharp_font_smaller(nint font);
    [LibraryImport(Library)] internal static partial nint wxsharp_font_scaled(nint font, float factor);
    [LibraryImport(Library)] internal static partial nint wxsharp_font_base(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_make_bold(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_make_italic(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_make_underlined(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_make_strikethrough(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_make_larger(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_make_smaller(nint font);
    [LibraryImport(Library)] internal static partial void wxsharp_font_scale(nint font, float factor);
    [LibraryImport(Library)] internal static partial int wxsharp_font_get_default_encoding();
    [LibraryImport(Library)] internal static partial void wxsharp_font_set_default_encoding(int encoding);
    [LibraryImport(Library)] internal static partial int wxsharp_font_numeric_weight_of(int weight);
    [LibraryImport(Library)] internal static partial int wxsharp_font_weight_closest_to(int numericWeight);
    [LibraryImport(Library)] internal static partial int wxsharp_font_adjust_to_symbolic_size(int size, int basePointSize);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_font_add_private(string filename);
    [LibraryImport(Library)] internal static partial nint wxsharp_font_from_system(int which);
    [LibraryImport(Library)] internal static partial int wxsharp_font_enumerate_facenames(int encoding, [MarshalAs(UnmanagedType.U1)] bool fixedWidthOnly);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_font_enumerate_encodings(string facename);
    [LibraryImport(Library)] internal static partial int wxsharp_font_enumerated_name(int index, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_is_valid_facename(string facename);
    [LibraryImport(Library)] internal static partial void wxsharp_font_invalidate_enumeration_cache();
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_font_can_use_private();


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


    // ---- wxWindow, the rest ----
    [LibraryImport(Library)] internal static partial void wxsharp_window_freeze(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_thaw(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_is_frozen(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_clear_background(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_rect(nint window, out int x, out int y, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_client_rect(nint window, out int x, out int y, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_screen_rect(nint window, out int x, out int y, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_screen_position(nint window, out int x, out int y);
    [LibraryImport(Library)] internal static partial void wxsharp_window_client_to_screen(nint window, ref int x, ref int y);
    [LibraryImport(Library)] internal static partial void wxsharp_window_screen_to_client(nint window, ref int x, ref int y);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_virtual_size(nint window, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_virtual_size(nint window, int width, int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_best_virtual_size(nint window, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_min_size(nint window, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_max_size(nint window, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_min_client_size(nint window, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_min_client_size(nint window, int width, int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_max_client_size(nint window, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_max_client_size(nint window, int width, int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_border_size(nint window, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_client_size(nint window, int width, int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_fit_inside(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_convert_dialog_to_pixels(nint window, ref int x, ref int y);
    [LibraryImport(Library)] internal static partial void wxsharp_window_convert_pixels_to_dialog(nint window, ref int x, ref int y);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_window_get_text_extent(nint window, string text, out int width, out int height, out int descent, out int externalLeading);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_char_height(nint window);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_char_width(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_get_dpi(nint window, out int x, out int y);
    [LibraryImport(Library)] internal static partial void wxsharp_window_from_dip(nint window, ref int width, ref int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_to_dip(nint window, ref int width, ref int height);
    [LibraryImport(Library)] internal static partial void wxsharp_window_raise(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_lower(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_is_shown_on_screen(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_close_any(nint window, [MarshalAs(UnmanagedType.U1)] bool force);
    [LibraryImport(Library)] internal static partial void wxsharp_window_center_any(nint window, [MarshalAs(UnmanagedType.U1)] bool onParent);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_navigate(nint window, [MarshalAs(UnmanagedType.U1)] bool forward, [MarshalAs(UnmanagedType.U1)] bool windowChange);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_navigate_in(nint window, [MarshalAs(UnmanagedType.U1)] bool forward, [MarshalAs(UnmanagedType.U1)] bool windowChange);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_scrollbar(nint window, [MarshalAs(UnmanagedType.U1)] bool vertical, int position, int thumbSize, int range, [MarshalAs(UnmanagedType.U1)] bool refresh);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_scroll_pos(nint window, [MarshalAs(UnmanagedType.U1)] bool vertical, int position, [MarshalAs(UnmanagedType.U1)] bool refresh);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_scroll_pos(nint window, [MarshalAs(UnmanagedType.U1)] bool vertical);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_scroll_range(nint window, [MarshalAs(UnmanagedType.U1)] bool vertical);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_scroll_thumb(nint window, [MarshalAs(UnmanagedType.U1)] bool vertical);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_has_scrollbar(nint window, [MarshalAs(UnmanagedType.U1)] bool vertical);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_scroll_lines(nint window, int lines);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_scroll_pages(nint window, int pages);
    [LibraryImport(Library)] internal static partial void wxsharp_window_scroll_window(nint window, int dx, int dy);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_style_flags(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_style_flags(nint window, int style);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_has_style_flag(nint window, int flag);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_label(nint window, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_class_name(nint window, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_get_parent(nint window);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_window_set_label(nint window, string label);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_help_text(nint window, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_window_set_help_text(nint window, string text);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_is_double_buffered(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_double_buffered(nint window, [MarshalAs(UnmanagedType.U1)] bool on);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_background_style(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_set_background_style(nint window, int style);
    [LibraryImport(Library)] internal static partial int wxsharp_window_get_variant(nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_variant(nint window, int variant);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_can_set_transparent(nint window);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_set_transparent(nint window, int alpha);
    [LibraryImport(Library)] internal static partial void wxsharp_window_warp_pointer(nint window, int x, int y);
    [LibraryImport(Library)] internal static partial int wxsharp_window_hit_test(nint window, int x, int y);
    [LibraryImport(Library)] internal static partial int wxsharp_window_popup_menu_selection(nint window, nint menu, int x, int y);


    // ---- wxTextCtrl: what is specific to it rather than shared through wxTextEntry ----
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textbox_is_modified(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textbox_mark_dirty(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textbox_discard_edits(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textbox_set_modified(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool modified);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textbox_is_multiline(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textbox_position_to_xy(nint ctrl, int position, out int x, out int y);
    [LibraryImport(Library)] internal static partial int wxsharp_textbox_xy_to_position(nint ctrl, int x, int y);
    [LibraryImport(Library)] internal static partial int wxsharp_textbox_hit_test(nint ctrl, int x, int y, out int position);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textbox_load_file(nint ctrl, string path);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textbox_save_file(nint ctrl, string path);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textbox_set_style(nint ctrl, int start, int end, NativeTextAttr* style);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textbox_get_style(nint ctrl, int position, NativeTextAttr* style);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textbox_set_default_style(nint ctrl, NativeTextAttr* style);
    [LibraryImport(Library)] internal static partial void wxsharp_textbox_get_default_style(nint ctrl, NativeTextAttr* style);

    // ---- Colour names ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_colour_parse(string text, out uint argb);
    [LibraryImport(Library)] internal static partial int wxsharp_colour_name(uint argb, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial uint wxsharp_colour_change_lightness(uint argb, int alpha);
    [LibraryImport(Library)] internal static partial uint wxsharp_colour_make_disabled(uint argb, byte brightness);
    [LibraryImport(Library)] internal static partial uint wxsharp_colour_make_grey(uint argb);
    [LibraryImport(Library)] internal static partial uint wxsharp_colour_make_mono(uint argb, [MarshalAs(UnmanagedType.U1)] bool on);
    [LibraryImport(Library)] internal static partial double wxsharp_colour_luminance(uint argb);
    [LibraryImport(Library)] internal static partial byte wxsharp_colour_alpha_blend(byte foreground, byte background, double alpha);

    // ---- The wxWidgets free functions ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_launch_default_browser(string url, int flags);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_launch_default_application(string path, int flags);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial long wxsharp_execute(string command, int flags);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial long wxsharp_shell(string command);
    [LibraryImport(Library)] internal static partial void wxsharp_bell();
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_get_key_state(int key);
    [LibraryImport(Library)] internal static partial void wxsharp_get_mouse_position(out int x, out int y);
    [LibraryImport(Library)] internal static partial void wxsharp_get_mouse_state(out int x, out int y, out int buttons, out int modifiers);
    [LibraryImport(Library)] internal static partial int wxsharp_get_user_id(byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_get_user_name(byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_get_host_name(byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_get_full_host_name(byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_get_email_address(byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_get_home_dir(byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_get_os_description(byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_get_os_version(out int major, out int minor, out int micro);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_check_os_version(int major, int minor, int micro);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_is_platform_64bit();
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_is_platform_little_endian();
    [LibraryImport(Library)] internal static partial int wxsharp_get_cpu_architecture_name(byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_get_native_cpu_architecture_name(byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_get_library_version(byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial uint wxsharp_get_process_id();
    [LibraryImport(Library)] internal static partial long wxsharp_get_free_memory();
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_get_disk_space(string path, out long total, out long freeSpace);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_get_env(string name, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_set_env(string name, string value);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_unset_env(string name);
    [LibraryImport(Library)] internal static partial void wxsharp_sleep(int seconds);
    [LibraryImport(Library)] internal static partial void wxsharp_milli_sleep(ulong milliseconds);
    [LibraryImport(Library)] internal static partial void wxsharp_micro_sleep(ulong microseconds);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_find_window_by_name(string name, nint parent);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_find_window_by_label(string label, nint parent);
    [LibraryImport(Library)] internal static partial nint wxsharp_find_window_at_point(int x, int y);
    [LibraryImport(Library)] internal static partial nint wxsharp_get_active_window();
    [LibraryImport(Library)] internal static partial void wxsharp_enable_top_level_windows([MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_disabler_begin(nint skip);
    [LibraryImport(Library)] internal static partial void wxsharp_window_disabler_end(nint scope);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_strip_menu_codes(string text, byte* buffer, int bufferLength);

    // ---- The rest of wxFrame / wxTopLevelWindow ----
    [LibraryImport(Library)] internal static partial void wxsharp_frame_iconize(nint frame, [MarshalAs(UnmanagedType.U1)] bool iconize);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_is_iconized(nint frame);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_maximize(nint frame, [MarshalAs(UnmanagedType.U1)] bool maximize);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_is_maximized(nint frame);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_is_always_maximized(nint frame);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_restore(nint frame);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_is_active(nint frame);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_show_full_screen(nint frame, [MarshalAs(UnmanagedType.U1)] bool show, int style);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_is_full_screen(nint frame);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_enable_full_screen_view(nint frame, [MarshalAs(UnmanagedType.U1)] bool enable, int style);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_show_without_activating(nint frame);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_request_user_attention(nint frame, int flags);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_enable_close_button(nint frame, [MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_enable_maximize_button(nint frame, [MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_enable_minimize_button(nint frame, [MarshalAs(UnmanagedType.U1)] bool enable);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_centre_on_screen(nint frame, int direction);
    [LibraryImport(Library)] internal static partial int wxsharp_frame_get_content_protection(nint frame);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_set_content_protection(nint frame, int protection);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_frame_set_represented_filename(nint frame, string path);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_set_window_modality(nint frame, int modality);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_get_default_size(out int width, out int height);
    [LibraryImport(Library)] internal static partial nint wxsharp_frame_get_default_item(nint frame);
    [LibraryImport(Library)] internal static partial nint wxsharp_frame_set_default_item(nint frame, nint window);
    [LibraryImport(Library)] internal static partial nint wxsharp_frame_get_icon(nint frame);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_set_icons(nint frame, nint* icons, int count);
    [LibraryImport(Library)] internal static partial int wxsharp_frame_get_icons(nint frame);
    [LibraryImport(Library)] internal static partial nint wxsharp_frame_get_icon_at(int index);
    [LibraryImport(Library)] internal static partial nint wxsharp_frame_get_menubar(nint frame);
    [LibraryImport(Library)] internal static partial nint wxsharp_frame_find_item_in_menubar(nint frame, int id);
    [LibraryImport(Library)] internal static partial nint wxsharp_frame_get_statusbar(nint frame);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_set_statusbar(nint frame, nint bar);
    [LibraryImport(Library)] internal static partial nint wxsharp_frame_create_statusbar(nint frame, int fields, int style, int id, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_frame_set_status_text(nint frame, string text, int field);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_frame_push_status_text(nint frame, string text, int field);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_pop_status_text(nint frame, int field);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_set_status_widths(nint frame, int* widths, int count);
    [LibraryImport(Library)] internal static partial int wxsharp_frame_get_status_bar_pane(nint frame);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_set_status_bar_pane(nint frame, int pane);
    [LibraryImport(Library)] internal static partial nint wxsharp_frame_get_toolbar(nint frame);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_set_toolbar(nint frame, nint bar);
    [LibraryImport(Library)] internal static partial nint wxsharp_frame_create_toolbar(nint frame, int style, int id, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_use_native_statusbar([MarshalAs(UnmanagedType.U1)] bool native);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_uses_native_statusbar();
    [LibraryImport(Library)] internal static partial int wxsharp_frame_save_geometry(nint frame, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_frame_restore_to_geometry(nint frame, string text);

    // ---- wxLocale ----
    [LibraryImport(Library)] internal static partial nint wxsharp_locale_create(int language, int flags);
    [LibraryImport(Library)] internal static partial void wxsharp_locale_destroy(nint locale);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_locale_is_ok(nint locale);
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_language(nint locale);
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_name(nint locale, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_canonical_name(nint locale, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_locale(nint locale, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_sys_name(nint locale, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_locale_add_catalog(nint locale, string domain, int msgIdLanguage);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_locale_is_loaded(nint locale, string domain);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_locale_get_string(nint locale, string original, string domain, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_locale_get_string_plural(nint locale, string singular, string plural, uint n, string domain, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_locale_get_header_value(nint locale, string header, string domain, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_locale_add_catalog_lookup_path_prefix(string prefix);
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_system_language();
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_system_encoding_name(byte* buffer, int bufferLength);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_locale_is_available(int language);
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_language_name(int language, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_language_canonical_name(int language, byte* buffer, int bufferLength);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_locale_get_language_info(int language, NativeLanguageInfo* info);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_locale_find_language_info(string text, NativeLanguageInfo* info);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_locale_find_language_info_by_tag(string tag, NativeLanguageInfo* info);
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_info(int index, int category, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_locale_get_os_info(int index, int category, byte* buffer, int bufferLength);

    // ---- wxTranslations ----
    [LibraryImport(Library)] internal static partial nint wxsharp_translations_get();
    [LibraryImport(Library)] internal static partial nint wxsharp_translations_create();
    [LibraryImport(Library)] internal static partial void wxsharp_translations_set(nint translations);
    [LibraryImport(Library)] internal static partial void wxsharp_translations_set_language(nint translations, int language);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_translations_set_language_named(nint translations, string language);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_translations_add_catalog(nint translations, string domain, int msgIdLanguage);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_translations_add_available_catalog(nint translations, string domain, int msgIdLanguage);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_translations_add_std_catalog(nint translations);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_translations_is_loaded(nint translations, string domain);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_translations_available_count(nint translations, string domain);
    [LibraryImport(Library)] internal static partial int wxsharp_translations_available_at(int index, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_translations_get_best_translation(nint translations, string domain, int msgIdLanguage, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_translations_get_best_available_translation(nint translations, string domain, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_translations_get_translated_string(nint translations, string original, string domain, string context, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_translations_get_translated_string_plural(nint translations, string original, uint n, string domain, string context, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_translations_get_header_value(nint translations, string header, string domain, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_translations_add_lookup_prefix(string prefix);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_get_translation(string original, string domain, string context, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial int wxsharp_get_translation_plural(string singular, string plural, uint n, string domain, string context, byte* buffer, int bufferLength);

    // ---- wxTextEntry ----
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_supported(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_textentry_get_value(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_textentry_set_value(nint ctrl, string value);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_textentry_change_value(nint ctrl, string value);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_textentry_write_text(nint ctrl, string text);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_textentry_append_text(nint ctrl, string text);
    [LibraryImport(Library)] internal static partial int wxsharp_textentry_get_range(nint ctrl, int from, int to, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_textentry_replace(nint ctrl, int from, int to, string value);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_remove(nint ctrl, int from, int to);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_clear(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_is_empty(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_copy(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_cut(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_paste(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_can_copy(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_can_cut(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_can_paste(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_undo(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_redo(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_can_undo(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_can_redo(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_set_insertion_point(nint ctrl, int position);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_set_insertion_point_end(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_textentry_get_insertion_point(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_textentry_get_last_position(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_set_selection(nint ctrl, int from, int to);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_get_selection(nint ctrl, out int from, out int to);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_select_all(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_select_none(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_has_selection(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_textentry_get_selected_text(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_remove_selection(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_is_editable(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_set_editable(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool editable);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_set_max_length(nint ctrl, int length);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_force_upper(nint ctrl);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_set_hint(nint ctrl, string hint);
    [LibraryImport(Library)] internal static partial int wxsharp_textentry_get_hint(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_set_margins(nint ctrl, int left, int top);
    [LibraryImport(Library)] internal static partial void wxsharp_textentry_get_margins(nint ctrl, out int left, out int top);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_auto_complete(nint ctrl, byte** choices, int count);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_auto_complete_files(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_textentry_auto_complete_directories(nint ctrl);

    // ---- Clipboard ----
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_clipboard_open();
    [LibraryImport(Library)] internal static partial void wxsharp_clipboard_close();
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_clipboard_is_opened();
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_clipboard_flush();
    [LibraryImport(Library)] internal static partial void wxsharp_clipboard_clear();
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_clipboard_is_supported(int format);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_clipboard_is_supported_async(nint sink);
    [LibraryImport(Library)] internal static partial void wxsharp_clipboard_use_primary_selection([MarshalAs(UnmanagedType.U1)] bool primary);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_clipboard_set_text(string text);
    [LibraryImport(Library)] internal static partial int wxsharp_clipboard_get_text(byte* buffer, int bufferLength);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_clipboard_set_files(byte** paths, int count);
    [LibraryImport(Library)] internal static partial int wxsharp_clipboard_read_files();
    [LibraryImport(Library)] internal static partial int wxsharp_clipboard_get_file(int index, byte* buffer, int bufferLength);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_clipboard_set_bitmap(nint bitmap);
    [LibraryImport(Library)] internal static partial nint wxsharp_clipboard_get_bitmap();

    // ---- System settings ----
    [LibraryImport(Library)] internal static partial uint wxsharp_system_colour(int which);
    [LibraryImport(Library)] internal static partial int wxsharp_system_metric(int which, nint window);
    [LibraryImport(Library)] internal static partial int wxsharp_system_screen_type();
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_system_has_feature(int which);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_system_appearance_is_dark();
    [LibraryImport(Library)] internal static partial int wxsharp_system_appearance_name(byte* buffer, int bufferLength);

    // ---- Sizers ----






    // ---- Label ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_label_create(nint parent, int id, string text, int style, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_label_create(nint parent, int id, string text, int style, long token);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_label_set_text(nint ctrl, string text);

    [LibraryImport(Library)]
    internal static partial int wxsharp_label_get_text(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial void wxsharp_label_wrap(nint ctrl, int width);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_label_is_ellipsized(nint ctrl);

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
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_textbox_create(nint parent, int id, string value, int style, long token);

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
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_checkbox_create(nint parent, int id, string label, int style, long token);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_checkbox_get(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_checkbox_set(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool value);

    // ---- Radio button ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_radio_create(nint parent, int id, string label,
        [MarshalAs(UnmanagedType.U1)] bool groupStart, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_radio_create(nint parent, int id, string label,
        [MarshalAs(UnmanagedType.U1)] bool groupStart, long token);

    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_radio_get(nint ctrl);

    [LibraryImport(Library)]
    internal static partial void wxsharp_radio_set(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool value);

    [LibraryImport(Library)] internal static partial nint wxsharp_radio_get_first(nint ctrl);
    [LibraryImport(Library)] internal static partial nint wxsharp_radio_get_last(nint ctrl);
    [LibraryImport(Library)] internal static partial nint wxsharp_radio_get_previous(nint ctrl);
    [LibraryImport(Library)] internal static partial nint wxsharp_radio_get_next(nint ctrl);

    // ---- Slider ----
    [LibraryImport(Library)]
    internal static partial nint wxsharp_slider_create(nint parent, int id, int minValue, int maxValue,
        int value, int style, long token);
    [LibraryImport(Library)]
    internal static partial nint wxsharp_custom_slider_create(nint parent, int id, int minValue, int maxValue,
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
    [LibraryImport(Library)]
    internal static partial nint wxsharp_custom_choice_create(nint parent, int id, int style, long token);

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
    [LibraryImport(Library)]
    internal static partial nint wxsharp_custom_listbox_create(nint parent, int id, int style, long token);

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
    [LibraryImport(Library)] internal static partial void wxsharp_listbox_deselect_all(nint ctrl);

    // ---- Extended common controls ----
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_togglebutton_create(nint parent, int id, string label, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_togglebutton_create(nint parent, int id, string label, long token);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_togglebutton_get(nint ctrl);
    [LibraryImport(Library)]
    internal static partial void wxsharp_togglebutton_set(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool value);

    [LibraryImport(Library)]
    internal static partial nint wxsharp_gauge_create(nint parent, int id, int range, int value,
        [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)]
    internal static partial nint wxsharp_custom_gauge_create(nint parent, int id, int range, int value,
        [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_gauge_get(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_gauge_set(nint ctrl, int value);
    [LibraryImport(Library)] internal static partial int wxsharp_gauge_get_range(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_gauge_set_range(nint ctrl, int range);
    [LibraryImport(Library)] internal static partial void wxsharp_gauge_pulse(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_gauge_is_vertical(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_gauge_get_bezel_face(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_gauge_set_bezel_face(nint ctrl, int width);
    [LibraryImport(Library)] internal static partial int wxsharp_gauge_get_shadow_width(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_gauge_set_shadow_width(nint ctrl, int width);

    [LibraryImport(Library)]
    internal static partial nint wxsharp_spinctrl_create(nint parent, int id, int minValue, int maxValue,
        int value, long token);
    [LibraryImport(Library)]
    internal static partial nint wxsharp_custom_spinctrl_create(nint parent, int id, int minValue, int maxValue,
        int value, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_spinctrl_get(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrl_set(nint ctrl, int value);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrl_set_range(nint ctrl, int minValue, int maxValue);
    [LibraryImport(Library)] internal static partial int wxsharp_spinctrl_get_min(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_spinctrl_get_max(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_spinctrl_get_increment(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrl_set_increment(nint ctrl, int increment);
    [LibraryImport(Library)] internal static partial int wxsharp_spinctrl_get_base(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_spinctrl_set_base(nint ctrl, int numberBase);
    [LibraryImport(Library)] internal static partial int wxsharp_spinctrl_get_text_value(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_spinctrl_set_text_value(nint ctrl, string value);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrl_set_selection(nint ctrl, int from, int to);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_combobox_create(nint parent, int id, string value,
        [MarshalAs(UnmanagedType.U1)] bool readOnly, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_combobox_create(nint parent, int id, string value,
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
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_searchctrl_create(nint parent, int id, string value, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_searchctrl_get_value(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void wxsharp_searchctrl_set_value(nint ctrl, string value);
    [LibraryImport(Library)] internal static partial void wxsharp_searchctrl_show_cancel(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool show);
    [LibraryImport(Library)] internal static partial void wxsharp_searchctrl_show_search(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool show);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_searchctrl_is_cancel_visible(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_searchctrl_is_search_visible(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_searchctrl_get_descriptive_text(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_searchctrl_set_descriptive_text(nint ctrl, string text);
    [LibraryImport(Library)] internal static partial nint wxsharp_searchctrl_get_menu(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_searchctrl_set_menu(nint ctrl, nint menu);
    [LibraryImport(Library)] internal static partial void wxsharp_searchctrl_set_search_bitmap(nint ctrl, nint bitmap);
    [LibraryImport(Library)] internal static partial void wxsharp_searchctrl_set_search_menu_bitmap(nint ctrl, nint bitmap);
    [LibraryImport(Library)] internal static partial void wxsharp_searchctrl_set_cancel_bitmap(nint ctrl, nint bitmap);

    [LibraryImport(Library)] internal static partial nint wxsharp_checklistbox_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_checklistbox_create(nint parent, int id, long token);
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
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_radiobox_create(nint parent, int id, string label, nint* choices,
        int count, int columns, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_radiobox_get_selection(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_radiobox_set_selection(nint ctrl, int selection);

    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_staticbox_create(nint parent, int id, string label, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint wxsharp_custom_staticbox_create(nint parent, int id, string label, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_staticline_create(nint parent, int id, [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_staticline_create(nint parent, int id, [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_staticbox_get_borders(nint ctrl, out int top, out int other);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_staticline_is_vertical(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_staticline_default_size();
    [LibraryImport(Library)] internal static partial nint wxsharp_activity_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_activity_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_activity_start(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_activity_stop(nint ctrl);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_activity_is_running(nint ctrl);
    [LibraryImport(Library)] internal static partial nint wxsharp_spinctrldouble_create(nint parent, int id, double minValue, double maxValue, double value, double increment, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_spinctrldouble_create(nint parent, int id, double minValue, double maxValue, double value, double increment, long token);
    [LibraryImport(Library)] internal static partial double wxsharp_spinctrldouble_get(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrldouble_set(nint ctrl, double value);
    [LibraryImport(Library)] internal static partial double wxsharp_spinctrldouble_get_min(nint ctrl);
    [LibraryImport(Library)] internal static partial double wxsharp_spinctrldouble_get_max(nint ctrl);
    [LibraryImport(Library)] internal static partial double wxsharp_spinctrldouble_get_increment(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrldouble_set_increment(nint ctrl, double increment);
    [LibraryImport(Library)] internal static partial uint wxsharp_spinctrldouble_get_digits(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrldouble_set_digits(nint ctrl, uint digits);
    [LibraryImport(Library)] internal static partial void wxsharp_spinctrldouble_set_range(nint ctrl, double minimum, double maximum);
    [LibraryImport(Library)] internal static partial int wxsharp_spinctrldouble_get_text_value(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_spinctrldouble_set_text_value(nint ctrl, string value);
    [LibraryImport(Library)] internal static partial nint wxsharp_scrollbar_create(nint parent, int id, [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_scrollbar_create(nint parent, int id, [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_scrollbar_set(nint ctrl, int position, int thumbSize, int range, int pageSize);
    [LibraryImport(Library)] internal static partial int wxsharp_scrollbar_get_position(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_scrollbar_set_ex(nint ctrl, int position, int thumbSize, int range, int pageSize, [MarshalAs(UnmanagedType.U1)] bool refresh);
    [LibraryImport(Library)] internal static partial void wxsharp_scrollbar_set_position(nint ctrl, int position);
    [LibraryImport(Library)] internal static partial int wxsharp_scrollbar_get_thumb_size(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_scrollbar_get_range(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_scrollbar_get_page_size(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_scrollbar_is_vertical(nint ctrl);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_hyperlink_create(nint parent, int id, string label, string url, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_custom_hyperlink_create(nint parent, int id, string label, string url, long token);
    [LibraryImport(Library)] internal static partial int wxsharp_hyperlink_get_url(nint ctrl, byte* buffer, int bufferLength);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_hyperlink_set_url(nint ctrl, string url);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_hyperlink_get_visited(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_hyperlink_set_visited(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool visited);
    [LibraryImport(Library)] internal static partial uint wxsharp_hyperlink_get_normal_colour(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_hyperlink_set_normal_colour(nint ctrl, uint colour);
    [LibraryImport(Library)] internal static partial uint wxsharp_hyperlink_get_hover_colour(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_hyperlink_set_hover_colour(nint ctrl, uint colour);
    [LibraryImport(Library)] internal static partial uint wxsharp_hyperlink_get_visited_colour(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_hyperlink_set_visited_colour(nint ctrl, uint colour);
    [LibraryImport(Library)] internal static partial nint wxsharp_datepicker_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_datepicker_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_timepicker_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_timepicker_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_datetime_get(nint ctrl, out int year, out int month, out int day, out int hour, out int minute, out int second);
    [LibraryImport(Library)] internal static partial void wxsharp_datetime_set(nint ctrl, int year, int month, int day, int hour, int minute, int second);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_datepicker_get_range(nint ctrl, out int y1, out int m1, out int d1, out int y2, out int m2, out int d2);
    [LibraryImport(Library)] internal static partial void wxsharp_datepicker_set_range(nint ctrl, int y1, int m1, int d1, int y2, int m2, int d2);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial void wxsharp_datepicker_set_null_text(nint ctrl, string text);

    // ---- Containers ----
    [LibraryImport(Library)] internal static partial nint wxsharp_scrolled_create(nint parent, int id, int style, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_scrolled_create(nint parent, int id, int style, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_set_rate(nint ctrl, int xStep, int yStep);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_scroll(nint ctrl, int x, int y);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_get_view_start(nint ctrl, out int x, out int y);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_set_scrollbars(nint ctrl, int pixelsX, int pixelsY, int unitsX, int unitsY, int posX, int posY, [MarshalAs(UnmanagedType.U1)] bool noRefresh);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_enable_scrolling(nint ctrl, [MarshalAs(UnmanagedType.U1)] bool x, [MarshalAs(UnmanagedType.U1)] bool y);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_show_scrollbars(nint ctrl, int x, int y);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_get_pixels_per_unit(nint ctrl, out int x, out int y);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_set_target_window(nint ctrl, nint target);
    [LibraryImport(Library)] internal static partial void wxsharp_scrolled_set_scroll_page_size(nint ctrl, int orientation, int size);
    [LibraryImport(Library)] internal static partial int wxsharp_scrolled_get_scroll_page_size(nint ctrl, int orientation);
    [LibraryImport(Library)] internal static partial nint wxsharp_splitter_create(nint parent, int id, [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_splitter_create(nint parent, int id, [MarshalAs(UnmanagedType.U1)] bool vertical, long token);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_splitter_split(nint ctrl, nint first, nint second, int position);
    [LibraryImport(Library)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_splitter_unsplit(nint ctrl, nint remove);
    [LibraryImport(Library)] internal static partial int wxsharp_splitter_get_position(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_splitter_set_position(nint ctrl, int position);
    [LibraryImport(Library)] internal static partial nint wxsharp_notebook_create(nint parent, int id, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_notebook_create(nint parent, int id, long token);
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
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_simplebook_create(nint parent, int id, long token);

    // ---- Data controls ----
    [LibraryImport(Library)] internal static partial nint wxsharp_listctrl_create(nint parent, int id, int style, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_listctrl_create(nint parent, int id, int style, long token);
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
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_treectrl_create(nint parent, int id, int style, long token);
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
    [LibraryImport(Library)] internal static partial int wxsharp_tree_get_count(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_tree_expand_all(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_tree_collapse_all(nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_tree_item_has_children(nint ctrl, long item);

    [LibraryImport(Library)] internal static partial nint wxsharp_grid_create(nint parent, int id, int rows, int columns, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_grid_create(nint parent, int id, int rows, int columns, long token);
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
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_dataviewlist_create(nint parent, int id, long token);
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
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_dataviewtree_create(nint parent, int id, long token);
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
    [LibraryImport(Library)] internal static partial void wxsharp_tree_sort_children(nint ctrl, long item);

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


    // ---- Sizers ----
    [LibraryImport(Library)] internal static partial nint wxsharp_boxsizer_create([MarshalAs(UnmanagedType.U1)] bool horizontal);
    [LibraryImport(Library)] internal static partial nint wxsharp_gridsizer_create(int rows, int columns, int verticalGap, int horizontalGap);
    [LibraryImport(Library)] internal static partial nint wxsharp_flexgridsizer_create(int rows, int columns, int verticalGap, int horizontalGap);
    [LibraryImport(Library)] internal static partial nint wxsharp_staticboxsizer_create(nint box, [MarshalAs(UnmanagedType.U1)] bool horizontal);
    [LibraryImport(Library)] internal static partial nint wxsharp_gridbagsizer_create(int verticalGap, int horizontalGap);

    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_add_control(nint sizer, nint ctrl, int proportion, int flags, int border);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_add_sizer(nint sizer, nint child, int proportion, int flags, int border);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_add_spacer(nint sizer, int size);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_add_stretch_spacer(nint sizer, int proportion);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_insert_control(nint sizer, int index, nint ctrl, int proportion, int flags, int border);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_insert_sizer(nint sizer, int index, nint child, int proportion, int flags, int border);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_insert_spacer(nint sizer, int index, int size);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_insert_stretch_spacer(nint sizer, int index, int proportion);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_prepend_control(nint sizer, nint ctrl, int proportion, int flags, int border);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_prepend_sizer(nint sizer, nint child, int proportion, int flags, int border);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_prepend_spacer(nint sizer, int size);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_prepend_stretch_spacer(nint sizer, int proportion);

    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_detach_control(nint sizer, nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_detach_sizer(nint sizer, nint child);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_detach_at(nint sizer, int index);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_remove_sizer(nint sizer, nint child);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_remove_at(nint sizer, int index);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_clear(nint sizer, [MarshalAs(UnmanagedType.U1)] bool deleteWindows);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_delete_windows(nint sizer);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_replace_control(nint sizer, nint oldCtrl, nint newCtrl, [MarshalAs(UnmanagedType.U1)] bool recursive);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_replace_sizer(nint sizer, nint oldSizer, nint newSizer, [MarshalAs(UnmanagedType.U1)] bool recursive);

    [LibraryImport(Library)] internal static partial int wxsharp_sizer_item_count(nint sizer);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_is_empty(nint sizer);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_item_at(nint sizer, int index);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_item_for_control(nint sizer, nint ctrl, [MarshalAs(UnmanagedType.U1)] bool recursive);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_item_for_sizer(nint sizer, nint child, [MarshalAs(UnmanagedType.U1)] bool recursive);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_item_by_id(nint sizer, int id, [MarshalAs(UnmanagedType.U1)] bool recursive);

    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_show_control(nint sizer, nint ctrl, [MarshalAs(UnmanagedType.U1)] bool show, [MarshalAs(UnmanagedType.U1)] bool recursive);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_show_sizer(nint sizer, nint child, [MarshalAs(UnmanagedType.U1)] bool show, [MarshalAs(UnmanagedType.U1)] bool recursive);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_show_at(nint sizer, int index, [MarshalAs(UnmanagedType.U1)] bool show);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_show_items(nint sizer, [MarshalAs(UnmanagedType.U1)] bool show);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_any_items_shown(nint sizer);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_is_shown_control(nint sizer, nint ctrl);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_is_shown_sizer(nint sizer, nint child);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_is_shown_at(nint sizer, int index);

    [LibraryImport(Library)] internal static partial void wxsharp_sizer_layout(nint sizer);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_fit(nint sizer, nint window, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_fit_inside(nint sizer, nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_set_size_hints(nint sizer, nint window);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_compute_fitting_client_size(nint sizer, nint window, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_compute_fitting_window_size(nint sizer, nint window, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_get_min_size(nint sizer, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_set_min_size(nint sizer, int width, int height);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_get_size(nint sizer, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_get_position(nint sizer, out int x, out int y);
    [LibraryImport(Library)] internal static partial void wxsharp_sizer_set_dimension(nint sizer, int x, int y, int width, int height);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_set_item_min_size_control(nint sizer, nint ctrl, int width, int height);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_set_item_min_size_sizer(nint sizer, nint child, int width, int height);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizer_set_item_min_size_at(nint sizer, int index, int width, int height);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizer_containing_window(nint sizer);

    // ---- Sizer items ----
    [LibraryImport(Library)] internal static partial int wxsharp_sizeritem_get_proportion(nint item);
    [LibraryImport(Library)] internal static partial void wxsharp_sizeritem_set_proportion(nint item, int proportion);
    [LibraryImport(Library)] internal static partial int wxsharp_sizeritem_get_flags(nint item);
    [LibraryImport(Library)] internal static partial void wxsharp_sizeritem_set_flags(nint item, int flags);
    [LibraryImport(Library)] internal static partial int wxsharp_sizeritem_get_border(nint item);
    [LibraryImport(Library)] internal static partial void wxsharp_sizeritem_set_border(nint item, int border);
    [LibraryImport(Library)] internal static partial int wxsharp_sizeritem_get_id(nint item);
    [LibraryImport(Library)] internal static partial void wxsharp_sizeritem_set_id(nint item, int id);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizeritem_is_window(nint item);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizeritem_is_sizer(nint item);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizeritem_is_spacer(nint item);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizeritem_get_window(nint item);
    [LibraryImport(Library)] internal static partial nint wxsharp_sizeritem_get_sizer(nint item);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_sizeritem_is_shown(nint item);
    [LibraryImport(Library)] internal static partial void wxsharp_sizeritem_show(nint item, [MarshalAs(UnmanagedType.U1)] bool show);
    [LibraryImport(Library)] internal static partial void wxsharp_sizeritem_get_min_size(nint item, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_sizeritem_set_min_size(nint item, int width, int height);
    [LibraryImport(Library)] internal static partial void wxsharp_sizeritem_get_size(nint item, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_sizeritem_get_position(nint item, out int x, out int y);

    // ---- Sizer subclasses ----
    [LibraryImport(Library)] internal static partial int wxsharp_boxsizer_get_orientation(nint sizer);
    [LibraryImport(Library)] internal static partial void wxsharp_boxsizer_set_orientation(nint sizer, [MarshalAs(UnmanagedType.U1)] bool vertical);
    [LibraryImport(Library)] internal static partial int wxsharp_gridsizer_get_rows(nint sizer);
    [LibraryImport(Library)] internal static partial int wxsharp_gridsizer_get_columns(nint sizer);
    [LibraryImport(Library)] internal static partial void wxsharp_gridsizer_set_rows(nint sizer, int rows);
    [LibraryImport(Library)] internal static partial void wxsharp_gridsizer_set_columns(nint sizer, int columns);
    [LibraryImport(Library)] internal static partial int wxsharp_gridsizer_get_vertical_gap(nint sizer);
    [LibraryImport(Library)] internal static partial int wxsharp_gridsizer_get_horizontal_gap(nint sizer);
    [LibraryImport(Library)] internal static partial void wxsharp_gridsizer_set_vertical_gap(nint sizer, int gap);
    [LibraryImport(Library)] internal static partial void wxsharp_gridsizer_set_horizontal_gap(nint sizer, int gap);
    [LibraryImport(Library)] internal static partial int wxsharp_gridsizer_effective_rows(nint sizer);
    [LibraryImport(Library)] internal static partial int wxsharp_gridsizer_effective_columns(nint sizer);
    [LibraryImport(Library)] internal static partial void wxsharp_flexgridsizer_add_growable_row(nint sizer, int row, int proportion);
    [LibraryImport(Library)] internal static partial void wxsharp_flexgridsizer_add_growable_column(nint sizer, int column, int proportion);
    [LibraryImport(Library)] internal static partial void wxsharp_flexgridsizer_remove_growable_row(nint sizer, int row);
    [LibraryImport(Library)] internal static partial void wxsharp_flexgridsizer_remove_growable_column(nint sizer, int column);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_flexgridsizer_is_row_growable(nint sizer, int row);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_flexgridsizer_is_column_growable(nint sizer, int column);
    [LibraryImport(Library)] internal static partial int wxsharp_flexgridsizer_get_flexible_direction(nint sizer);
    [LibraryImport(Library)] internal static partial void wxsharp_flexgridsizer_set_flexible_direction(nint sizer, int direction);
    [LibraryImport(Library)] internal static partial int wxsharp_flexgridsizer_get_grow_mode(nint sizer);
    [LibraryImport(Library)] internal static partial void wxsharp_flexgridsizer_set_grow_mode(nint sizer, int mode);
    [LibraryImport(Library)] internal static partial int wxsharp_flexgridsizer_row_heights(nint sizer, int* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial int wxsharp_flexgridsizer_column_widths(nint sizer, int* buffer, int bufferLength);
    [LibraryImport(Library)] internal static partial nint wxsharp_staticboxsizer_get_box(nint sizer);

    [LibraryImport(Library)] internal static partial nint wxsharp_gridbagsizer_add_control(nint sizer, nint ctrl, int row, int column, int rowSpan, int columnSpan, int flags, int border);
    [LibraryImport(Library)] internal static partial nint wxsharp_gridbagsizer_add_sizer(nint sizer, nint child, int row, int column, int rowSpan, int columnSpan, int flags, int border);
    [LibraryImport(Library)] internal static partial void wxsharp_gridbagsizer_get_item_position_control(nint sizer, nint ctrl, out int row, out int column);
    [LibraryImport(Library)] internal static partial void wxsharp_gridbagsizer_get_item_position_at(nint sizer, int index, out int row, out int column);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_gridbagsizer_set_item_position_control(nint sizer, nint ctrl, int row, int column);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_gridbagsizer_set_item_position_at(nint sizer, int index, int row, int column);
    [LibraryImport(Library)] internal static partial void wxsharp_gridbagsizer_get_item_span_control(nint sizer, nint ctrl, out int rowSpan, out int columnSpan);
    [LibraryImport(Library)] internal static partial void wxsharp_gridbagsizer_get_item_span_at(nint sizer, int index, out int rowSpan, out int columnSpan);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_gridbagsizer_set_item_span_control(nint sizer, nint ctrl, int rowSpan, int columnSpan);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_gridbagsizer_set_item_span_at(nint sizer, int index, int rowSpan, int columnSpan);
    [LibraryImport(Library)] internal static partial nint wxsharp_gridbagsizer_find_item_control(nint sizer, nint ctrl);
    [LibraryImport(Library)] internal static partial nint wxsharp_gridbagsizer_find_item_sizer(nint sizer, nint child);
    [LibraryImport(Library)] internal static partial nint wxsharp_gridbagsizer_find_item_at_position(nint sizer, int row, int column);
    [LibraryImport(Library)] internal static partial nint wxsharp_gridbagsizer_find_item_at_point(nint sizer, int x, int y);
    [LibraryImport(Library)] internal static partial void wxsharp_gridbagsizer_get_cell_size(nint sizer, int row, int column, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_gridbagsizer_get_empty_cell_size(nint sizer, out int width, out int height);
    [LibraryImport(Library)] internal static partial void wxsharp_gridbagsizer_set_empty_cell_size(nint sizer, int width, int height);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_gridbagsizer_check_for_intersection(nint sizer, int row, int column, int rowSpan, int columnSpan, nint exclude);

    [LibraryImport(Library)] internal static partial void wxsharp_window_set_sizer(nint window, nint sizer);
    [LibraryImport(Library)] internal static partial void wxsharp_window_set_sizer_and_fit(nint window, nint sizer);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_get_sizer(nint window);
    [LibraryImport(Library)] internal static partial nint wxsharp_window_containing_sizer(nint window);

    // ---- Event binding ----
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_bind(nint window, int eventId, long token);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_window_unbind(nint window, int eventId);
    [LibraryImport(Library)] internal static partial void wxsharp_window_unbind_all(nint window);

    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_app_bind(int eventId, long token);

    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)]
    internal static partial bool wxsharp_app_unbind(int eventId);
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
    [LibraryImport(Library)] internal static partial void wxsharp_checkbox_set_transparent_part_colour(nint ctrl, uint argb);

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
    [LibraryImport(Library)] internal static partial nint wxsharp_timer_create(nint owner, int id, long ownerToken);
    [LibraryImport(Library)] internal static partial void wxsharp_timer_destroy(nint timer);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_timer_start(nint timer, int milliseconds, [MarshalAs(UnmanagedType.U1)] bool oneShot);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_timer_start_once(nint timer, int milliseconds);
    [LibraryImport(Library)] internal static partial void wxsharp_timer_stop(nint timer);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_timer_is_running(nint timer);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_timer_is_one_shot(nint timer);
    [LibraryImport(Library)] internal static partial int wxsharp_timer_get_interval(nint timer);
    [LibraryImport(Library)] internal static partial void wxsharp_timer_notify(nint timer);
    [LibraryImport(Library)] internal static partial void wxsharp_timer_set_owner(nint timer, nint owner, int id, long ownerToken);
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
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_staticbitmap_create(nint parent, int id, nint bitmap, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_staticbitmap_set(nint ctrl, nint bitmap);
    [LibraryImport(Library)] internal static partial nint wxsharp_staticbitmap_get(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_staticbitmap_set_icon(nint ctrl, nint icon);
    [LibraryImport(Library)] internal static partial nint wxsharp_staticbitmap_get_icon(nint ctrl);
    [LibraryImport(Library)] internal static partial void wxsharp_staticbitmap_set_scale_mode(nint ctrl, int mode);
    [LibraryImport(Library)] internal static partial int wxsharp_staticbitmap_get_scale_mode(nint ctrl);
    [LibraryImport(Library)] internal static partial nint wxsharp_bitmapbutton_create(nint parent, int id, nint bitmap, long token);
    [LibraryImport(Library)] internal static partial nint wxsharp_custom_bitmapbutton_create(nint parent, int id, nint bitmap, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_bitmapbutton_new_close(nint parent, int id, string name, long token);
    [LibraryImport(Library)] internal static partial void wxsharp_bitmapbutton_set_margins(nint ctrl, int x, int y);
    [LibraryImport(Library)] internal static partial int wxsharp_bitmapbutton_get_margin_x(nint ctrl);
    [LibraryImport(Library)] internal static partial int wxsharp_bitmapbutton_get_margin_y(nint ctrl);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_icon_load(string path);
    [LibraryImport(Library)] internal static partial void wxsharp_icon_destroy(nint icon);
    [LibraryImport(Library)] internal static partial void wxsharp_frame_set_icon(nint frame, nint icon);
    [LibraryImport(Library)] internal static partial void wxsharp_begin_busy_cursor();
    [LibraryImport(Library)] internal static partial void wxsharp_end_busy_cursor();
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_progress_create(nint parent, string title, string message, int maximum, int style, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)] internal static partial nint wxsharp_custom_progress_create(nint parent, string title, string message, int maximum, int style, long token);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_progress_update(nint progress, int value, string message, [MarshalAs(UnmanagedType.U1)] out bool continueRunning);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_progress_pulse(nint progress, string message, [MarshalAs(UnmanagedType.U1)] out bool continueRunning);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_progress_was_cancelled(nint progress);
    [LibraryImport(Library)][return: MarshalAs(UnmanagedType.U1)] internal static partial bool wxsharp_progress_was_skipped(nint progress);
    [LibraryImport(Library)] internal static partial void wxsharp_progress_resume(nint progress);
    [LibraryImport(Library)] internal static partial int wxsharp_progress_get_value(nint progress);
    [LibraryImport(Library)] internal static partial int wxsharp_progress_get_range(nint progress);
    [LibraryImport(Library)] internal static partial void wxsharp_progress_set_range(nint progress, int range);
    [LibraryImport(Library)] internal static partial void wxsharp_progress_destroy(nint progress);

    // ---- Services ----


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
