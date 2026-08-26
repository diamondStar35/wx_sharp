using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WxSharp;

public class ListCtrl : Control
{
    public event EventHandler<CommandEventArgs>? SelectionChanged;
    public ListCtrl(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_listctrl_create(parent.Handle, id, Token));
    public long Count => NativeMethods.wxsharp_listctrl_count(Handle);
    public int InsertColumn(int column, string heading, int width = -1) => NativeMethods.wxsharp_listctrl_insert_column(Handle, column, heading, width);
    public long AddItem(string text) => NativeMethods.wxsharp_listctrl_insert_item(Handle, Count, text);
    public bool SetItem(long item, int column, string text) => NativeMethods.wxsharp_listctrl_set_item(Handle, item, column, text);
    public unsafe string GetItem(long item, int column = 0)
    {
        var length = NativeMethods.wxsharp_listctrl_get_item(Handle, item, column, null, 0); if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_listctrl_get_item(Handle, item, column, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }
    public bool RemoveAt(long item) => NativeMethods.wxsharp_listctrl_delete_item(Handle, item);
    public void Clear() => NativeMethods.wxsharp_listctrl_clear(Handle);
    public void SetSelected(long item, bool selected = true) => NativeMethods.wxsharp_listctrl_select(Handle, item, selected);
    public bool IsSelected(long item) => NativeMethods.wxsharp_listctrl_is_selected(Handle, item);
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Select
        ? RaiseCommand(new CommandEventArgs(this, e.Id), SelectionChanged) : base.Dispatch(e);
}

public readonly record struct TreeItemId(long Value)
{
    public bool IsValid => Value != 0;
    public static TreeItemId None => default;
}

public class TreeCtrl : Control
{
    public event EventHandler<CommandEventArgs>? SelectionChanged;
    public TreeCtrl(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_treectrl_create(parent.Handle, id, Token));
    public TreeItemId Root { get; private set; }
    public TreeItemId AddRoot(string text) => Root = new TreeItemId(NativeMethods.wxsharp_tree_add_root(Handle, text));
    public TreeItemId Add(TreeItemId parent, string text) => new(NativeMethods.wxsharp_tree_append(Handle, parent.Value, text));
    public void Remove(TreeItemId item) => NativeMethods.wxsharp_tree_delete(Handle, item.Value);
    public void Clear() { NativeMethods.wxsharp_tree_delete_all(Handle); Root = default; }
    public unsafe string GetText(TreeItemId item)
    {
        var length = NativeMethods.wxsharp_tree_get_text(Handle, item.Value, null, 0); if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_tree_get_text(Handle, item.Value, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }
    public void SetText(TreeItemId item, string text) => NativeMethods.wxsharp_tree_set_text(Handle, item.Value, text);
    public void Expand(TreeItemId item, bool expand = true) => NativeMethods.wxsharp_tree_expand(Handle, item.Value, expand);
    public bool IsExpanded(TreeItemId item) => NativeMethods.wxsharp_tree_is_expanded(Handle, item.Value);
    public TreeItemId Selection { get => new(NativeMethods.wxsharp_tree_get_selection(Handle)); set => NativeMethods.wxsharp_tree_select(Handle, value.Value); }
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Select
        ? RaiseCommand(new CommandEventArgs(this, e.Id), SelectionChanged) : base.Dispatch(e);
}

public class Grid : Control
{
    public Grid(Window parent, int rows = 0, int columns = 0, int id = WindowId.Any) : base(parent, id)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rows); ArgumentOutOfRangeException.ThrowIfNegative(columns);
        Initialize(NativeMethods.wxsharp_grid_create(parent.Handle, id, rows, columns, Token));
    }
    public int RowCount => NativeMethods.wxsharp_grid_rows(Handle);
    public int ColumnCount => NativeMethods.wxsharp_grid_columns(Handle);
    public bool AddRows(int count = 1) => NativeMethods.wxsharp_grid_append_rows(Handle, count);
    public bool AddColumns(int count = 1) => NativeMethods.wxsharp_grid_append_columns(Handle, count);
    public bool RemoveRows(int position, int count = 1) => NativeMethods.wxsharp_grid_delete_rows(Handle, position, count);
    public bool RemoveColumns(int position, int count = 1) => NativeMethods.wxsharp_grid_delete_columns(Handle, position, count);
    public unsafe string this[int row, int column]
    {
        get
        {
            var length = NativeMethods.wxsharp_grid_get_value(Handle, row, column, null, 0); if (length <= 0) return string.Empty;
            var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_grid_get_value(Handle, row, column, buffer, bytes.Length);
            return Utf8String.Decode(bytes, length);
        }
        set => NativeMethods.wxsharp_grid_set_value(Handle, row, column, value);
    }
    public void SetRowLabel(int row, string value) => NativeMethods.wxsharp_grid_set_row_label(Handle, row, value);
    public void SetColumnLabel(int column, string value) => NativeMethods.wxsharp_grid_set_column_label(Handle, column, value);
}

public class DataViewListCtrl : Control
{
    public event EventHandler<CommandEventArgs>? SelectionChanged;
    public DataViewListCtrl(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_dataviewlist_create(parent.Handle, id, Token));
    public int Count => NativeMethods.wxsharp_dataviewlist_count(Handle);
    public int SelectedIndex { get => NativeMethods.wxsharp_dataviewlist_get_selection(Handle); set => NativeMethods.wxsharp_dataviewlist_set_selection(Handle, value); }
    public void AddTextColumn(string label, int width = 120, bool editable = false)
        => NativeMethods.wxsharp_dataviewlist_append_text_column(Handle, label, width, editable);
    public unsafe void AddRow(IReadOnlyList<string> values)
    {
        ArgumentNullException.ThrowIfNull(values); var native = new nint[values.Count];
        try
        {
            for (var i = 0; i < native.Length; ++i) native[i] = Marshal.StringToCoTaskMemUTF8(values[i]);
            fixed (nint* pointers = native) NativeMethods.wxsharp_dataviewlist_append_row(Handle, pointers, native.Length);
        }
        finally { foreach (var value in native) if (value != 0) Marshal.FreeCoTaskMem(value); }
    }
    public unsafe string this[int row, int column]
    {
        get
        {
            var length = NativeMethods.wxsharp_dataviewlist_get_value(Handle, row, column, null, 0); if (length <= 0) return string.Empty;
            var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_dataviewlist_get_value(Handle, row, column, buffer, bytes.Length);
            return Utf8String.Decode(bytes, length);
        }
        set => NativeMethods.wxsharp_dataviewlist_set_value(Handle, row, column, value);
    }
    public void RemoveAt(int row) => NativeMethods.wxsharp_dataviewlist_delete_row(Handle, row);
    public void Clear() => NativeMethods.wxsharp_dataviewlist_clear(Handle);
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Select
        ? RaiseCommand(new CommandEventArgs(this, e.Id), SelectionChanged) : base.Dispatch(e);
}

public readonly record struct DataViewItem(long Value)
{
    public bool IsValid => Value != 0;
    public static DataViewItem Root => default;
}

public class DataViewTreeCtrl : Control
{
    public event EventHandler<CommandEventArgs>? SelectionChanged;
    public DataViewTreeCtrl(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_dataviewtree_create(parent.Handle, id, Token));
    public DataViewItem AddContainer(DataViewItem parent, string text) => new(NativeMethods.wxsharp_dataviewtree_append_container(Handle, parent.Value, text));
    public DataViewItem AddItem(DataViewItem parent, string text) => new(NativeMethods.wxsharp_dataviewtree_append_item(Handle, parent.Value, text));
    public unsafe string GetText(DataViewItem item)
    {
        var length = NativeMethods.wxsharp_dataviewtree_get_text(Handle, item.Value, null, 0); if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_dataviewtree_get_text(Handle, item.Value, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }
    public void SetText(DataViewItem item, string text) => NativeMethods.wxsharp_dataviewtree_set_text(Handle, item.Value, text);
    public void Remove(DataViewItem item) => NativeMethods.wxsharp_dataviewtree_delete(Handle, item.Value);
    public void Clear() => NativeMethods.wxsharp_dataviewtree_clear(Handle);
    public DataViewItem Selection { get => new(NativeMethods.wxsharp_dataviewtree_get_selection(Handle)); set => NativeMethods.wxsharp_dataviewtree_set_selection(Handle, value.Value); }
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Select
        ? RaiseCommand(new CommandEventArgs(this, e.Id), SelectionChanged) : base.Dispatch(e);
}
