using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WxSharp;

// Shared marshalling for the bulk item setters on the wxControlWithItems-derived controls - ListBox and
// Choice. The whole list is converted to a native array of UTF-8 string pointers and handed to the shim in a
// single ABI crossing, which reaches wxWidgets as one wxArrayString it preallocates and fills natively. This
// is what makes filling a list O(1) boundary crossings instead of one per item, mirroring wxPython's
// wxArrayString conversion.
internal static unsafe class ItemsInterop
{
    /// <summary>Marshals <paramref name="items"/> to a native UTF-8 string array and calls a bulk setter
    /// that takes (handle, items, count).</summary>
    internal static void Invoke(nint handle, IEnumerable<string> items,
        delegate*<nint, byte**, int, void> native)
    {
        ArgumentNullException.ThrowIfNull(items);
        var list = Materialize(items, out var count);
        if (count == 0) { native(handle, null, 0); return; }
        var pointers = new nint[count];
        try
        {
            for (var i = 0; i < count; i++)
                pointers[i] = Marshal.StringToCoTaskMemUTF8(list[i] ?? string.Empty);
            fixed (nint* p = pointers)
                native(handle, (byte**)p, count);
        }
        finally
        {
            for (var i = 0; i < count; i++)
                if (pointers[i] != 0) Marshal.FreeCoTaskMem(pointers[i]);
        }
    }

    /// <summary>As the three-argument <c>Invoke</c> above, for a bulk setter that also takes a position:
    /// (handle, items, count, index).</summary>
    internal static void Invoke(nint handle, IEnumerable<string> items, int index,
        delegate*<nint, byte**, int, int, void> native)
    {
        ArgumentNullException.ThrowIfNull(items);
        var list = Materialize(items, out var count);
        if (count == 0) return;
        var pointers = new nint[count];
        try
        {
            for (var i = 0; i < count; i++)
                pointers[i] = Marshal.StringToCoTaskMemUTF8(list[i] ?? string.Empty);
            fixed (nint* p = pointers)
                native(handle, (byte**)p, count, index);
        }
        finally
        {
            for (var i = 0; i < count; i++)
                if (pointers[i] != 0) Marshal.FreeCoTaskMem(pointers[i]);
        }
    }

    private static IReadOnlyList<string> Materialize(IEnumerable<string> items, out int count)
    {
        var list = items as IReadOnlyList<string> ?? new List<string>(items);
        count = list.Count;
        return list;
    }
}
