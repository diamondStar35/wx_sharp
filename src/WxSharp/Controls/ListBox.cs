using System;

namespace WxSharp;

/// <summary>A scrollable list of items. Single-selection by default; pass <see cref="ListBoxStyle.Multiple"/>
/// or <see cref="ListBoxStyle.Extended"/> for multi-selection (then use <see cref="GetSelectedIndices"/>).
/// <see cref="SelectedIndex"/> is -1 when nothing is selected.</summary>
public class ListBox : Control
{
    public event EventHandler<CommandEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.ListBoxSelected, value);
        remove => RemoveHandler(WxEvents.ListBoxSelected, value);
    }

    /// <summary>An item was activated by double-click or by Enter - the "open this one" gesture, as distinct
    /// from merely moving the selection onto it.</summary>
    public event EventHandler<CommandEventArgs> ItemActivated
    {
        add => AddHandler(WxEvents.ListBoxDoubleClicked, value);
        remove => RemoveHandler(WxEvents.ListBoxDoubleClicked, value);
    }

    public ListBox(Window parent, int id = WindowId.Any, ListBoxStyle style = ListBoxStyle.Single,
        Point? position = null, Size? size = null) : base(parent, id)
    {
        Initialize(NativeMethods.wxsharp_listbox_create(parent.Handle, id, (int)style, Token));
        ApplyInitialGeometry(position, size);
    }

    /// <summary>Appends an item to the end.</summary>
    public void Add(string item) => NativeMethods.wxsharp_listbox_append(Handle, item);

    /// <summary>Inserts an item before <paramref name="index"/>.</summary>
    public void Insert(string item, int index) => NativeMethods.wxsharp_listbox_insert(Handle, item, index);

    /// <summary>Removes the item at <paramref name="index"/>.</summary>
    public void RemoveAt(int index) => NativeMethods.wxsharp_listbox_delete(Handle, index);

    public void Clear() => NativeMethods.wxsharp_listbox_clear(Handle);

    public int Count => NativeMethods.wxsharp_listbox_count(Handle);

    /// <summary>Gets or replaces the text of the item at <paramref name="index"/>.</summary>
    public string this[int index]
    {
        get => GetItem(index);
        set => NativeMethods.wxsharp_listbox_set_string(Handle, index, value);
    }

    /// <summary>The index of the first item equal to <paramref name="text"/> (case-insensitive), or -1.</summary>
    public int IndexOf(string text) => NativeMethods.wxsharp_listbox_find_string(Handle, text);

    /// <summary>The selected item's index, or -1. For multi-select boxes this is the focused item; use
    /// <see cref="GetSelectedIndices"/> for the full set.</summary>
    public int SelectedIndex
    {
        get => NativeMethods.wxsharp_listbox_get_selection(Handle);
        set => NativeMethods.wxsharp_listbox_set_selection(Handle, value);
    }

    /// <summary>All selected indices (for a multi-selection list box).</summary>
    public unsafe int[] GetSelectedIndices()
    {
        var count = NativeMethods.wxsharp_listbox_get_selections(Handle, null, 0);
        if (count <= 0)
            return Array.Empty<int>();
        var indices = new int[count];
        fixed (int* p = indices)
            _ = NativeMethods.wxsharp_listbox_get_selections(Handle, p, count);
        return indices;
    }

    /// <summary>Selects or deselects a single item (for a multi-selection list box).</summary>
    public void SetSelected(int index, bool selected) => NativeMethods.wxsharp_listbox_select(Handle, index, selected);

    /// <summary>Whether the item at <paramref name="index"/> is selected.</summary>
    public bool IsSelected(int index) => NativeMethods.wxsharp_listbox_is_selected(Handle, index);

    /// <summary>Scrolls the list so the item at <paramref name="index"/> is visible.</summary>
    public void EnsureVisible(int index) => NativeMethods.wxsharp_listbox_ensure_visible(Handle, index);

    private unsafe string GetItem(int index)
    {
        var length = NativeMethods.wxsharp_listbox_get_string(Handle, index, null, 0);
        if (length <= 0)
            return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_listbox_get_string(Handle, index, p, length + 1);
        return Utf8String.Decode(buffer, length);
    }
}
