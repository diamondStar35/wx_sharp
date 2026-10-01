using System;
using System.Collections.Generic;

namespace WxSharp;

/// <summary>A drop-down list of items; <see cref="SelectedIndex"/> is -1 when nothing is selected.</summary>
public class Choice : Control
{
    /// <summary>Wraps a Choice wxWidgets created itself. See <see cref="Window.Adopt"/>.</summary>
    internal Choice(nint existingHandle, Window? parent) : base(existingHandle, parent) { }

    /// <summary>The selected item changed through the drop-down, following <c>wxEVT_CHOICE</c>. Not raised for
    /// a change made in code via <see cref="SelectedIndex"/>.</summary>
    public event EventHandler<CommandEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.ChoiceSelected, value);
        remove => RemoveHandler(WxEvents.ChoiceSelected, value);
    }

    /// <summary>Creates an empty drop-down. Pass <see cref="ChoiceStyle.Sorted"/> to keep items in
    /// alphabetical order as they are added.</summary>
    public Choice(Window parent, int id = WindowId.Any, ChoiceStyle style = ChoiceStyle.Unsorted,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(GetType() == typeof(Choice)
            ? NativeMethods.wxsharp_choice_create(parent.Handle, id, (int)style, Token)
            : NativeMethods.wxsharp_custom_choice_create(parent.Handle, id, (int)style, Token));
        ApplyInitialGeometry(position, size);
    }

    /// <summary>Appends an item to the end.</summary>
    public void Add(string item) => NativeMethods.wxsharp_choice_append(Handle, item);

    /// <summary>Replaces every item in one go, following <c>wxChoice.Set</c>. The whole list crosses the
    /// boundary once as a single wxArrayString rather than one call per item.</summary>
    public void Set(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        Verify();
        unsafe { ItemsInterop.Invoke(Handle, items, &NativeMethods.wxsharp_choice_set); }
    }

    /// <summary>Appends many items at once in a single boundary crossing.</summary>
    public void AppendRange(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        Verify();
        unsafe { ItemsInterop.Invoke(Handle, items, &NativeMethods.wxsharp_choice_append_many); }
    }

    /// <summary>Inserts many items before <paramref name="index"/> in one go.</summary>
    public void InsertRange(IEnumerable<string> items, int index)
    {
        ArgumentNullException.ThrowIfNull(items);
        Verify();
        unsafe { ItemsInterop.Invoke(Handle, items, index, &NativeMethods.wxsharp_choice_insert_many); }
    }

    /// <summary>Inserts an item before <paramref name="index"/>.</summary>
    public void Insert(string item, int index) => NativeMethods.wxsharp_choice_insert(Handle, item, index);

    /// <summary>Removes the item at <paramref name="index"/>.</summary>
    public void RemoveAt(int index) => NativeMethods.wxsharp_choice_delete(Handle, index);

    /// <summary>Removes every item.</summary>
    public void Clear() => NativeMethods.wxsharp_choice_clear(Handle);

    /// <summary>How many items the drop-down holds.</summary>
    public int Count => NativeMethods.wxsharp_choice_count(Handle);

    /// <summary>Gets or replaces the text of the item at <paramref name="index"/>.</summary>
    public string this[int index]
    {
        get => GetItem(index);
        set => NativeMethods.wxsharp_choice_set_string(Handle, index, value);
    }

    /// <summary>The index of the first item equal to <paramref name="text"/> (case-insensitive), or -1.</summary>
    public int IndexOf(string text) => NativeMethods.wxsharp_choice_find_string(Handle, text);

    /// <summary>The index of the selected item, or -1 when nothing is selected.</summary>
    public int SelectedIndex
    {
        get => NativeMethods.wxsharp_choice_get_selection(Handle);
        set => NativeMethods.wxsharp_choice_set_selection(Handle, value);
    }

    private unsafe string GetItem(int index)
    {
        var length = NativeMethods.wxsharp_choice_get_string(Handle, index, null, 0);
        if (length <= 0)
            return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_choice_get_string(Handle, index, p, length + 1);
        return Utf8String.Decode(buffer, length);
    }
}
