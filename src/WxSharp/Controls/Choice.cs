using System;

namespace WxSharp;

/// <summary>A drop-down list of items; <see cref="SelectedIndex"/> is -1 when nothing is selected.</summary>
public class Choice : Control
{
    public event EventHandler<CommandEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.ChoiceSelected, value);
        remove => RemoveHandler(WxEvents.ChoiceSelected, value);
    }

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

    /// <summary>Inserts an item before <paramref name="index"/>.</summary>
    public void Insert(string item, int index) => NativeMethods.wxsharp_choice_insert(Handle, item, index);

    /// <summary>Removes the item at <paramref name="index"/>.</summary>
    public void RemoveAt(int index) => NativeMethods.wxsharp_choice_delete(Handle, index);

    public void Clear() => NativeMethods.wxsharp_choice_clear(Handle);

    public int Count => NativeMethods.wxsharp_choice_count(Handle);

    /// <summary>Gets or replaces the text of the item at <paramref name="index"/>.</summary>
    public string this[int index]
    {
        get => GetItem(index);
        set => NativeMethods.wxsharp_choice_set_string(Handle, index, value);
    }

    /// <summary>The index of the first item equal to <paramref name="text"/> (case-insensitive), or -1.</summary>
    public int IndexOf(string text) => NativeMethods.wxsharp_choice_find_string(Handle, text);

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
