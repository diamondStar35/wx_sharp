using System.Runtime.InteropServices;

namespace WxSharp;

internal static unsafe partial class NativeMethods
{
    [LibraryImport(Library)]
    internal static partial nint wxsharp_item_dialog_create(nint dialog, [MarshalAs(UnmanagedType.U1)] bool folders, delegate* unmanaged[Cdecl]<long, int, int, int, int> callback, long token);
    [LibraryImport(Library)]
    internal static partial void wxsharp_item_dialog_destroy(nint context);
    [LibraryImport(Library)]
    internal static partial int wxsharp_item_dialog_show(nint context);
    [LibraryImport(Library)]
    internal static partial void wxsharp_item_dialog_close(nint context);
    [LibraryImport(Library)]
    internal static partial int wxsharp_item_dialog_count(nint context);
    [LibraryImport(Library)]
    internal static partial int wxsharp_item_dialog_filter(nint context);
    [LibraryImport(Library)]
    internal static partial int wxsharp_item_dialog_text(nint context, int kind, int index, byte* buffer, int length);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int wxsharp_item_dialog_add(nint context, int kind, string label);
    [LibraryImport(Library)]
    internal static partial int wxsharp_item_dialog_items(nint context, int id, byte** items, int count);
    [LibraryImport(Library)]
    internal static partial int wxsharp_item_dialog_number(nint context, int id, int property, [MarshalAs(UnmanagedType.U1)] bool write, int* value);
    [LibraryImport(Library, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int wxsharp_item_dialog_set_text(nint context, int id, string value);
}
