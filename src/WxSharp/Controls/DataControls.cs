using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WxSharp;

/// <summary>A multi-column list. Selection and focus are separate things here: the selection is what the
/// user has picked, focus is the row the keyboard and the screen reader are on, and moving one does not move
/// the other.</summary>
public class ListCtrl : Control
{
    /// <summary>A row became selected. Fires once per row, so a range selection raises it repeatedly.</summary>
    public event EventHandler<ListEventArgs> ItemSelected
    {
        add => AddHandler(WxEvents.ListItemSelected, value);
        remove => RemoveHandler(WxEvents.ListItemSelected, value);
    }

    /// <summary>A row stopped being selected.</summary>
    public event EventHandler<ListEventArgs> ItemDeselected
    {
        add => AddHandler(WxEvents.ListItemDeselected, value);
        remove => RemoveHandler(WxEvents.ListItemDeselected, value);
    }

    /// <summary>A row was activated by double-click or Enter - "open this one", as distinct from selecting it.</summary>
    public event EventHandler<ListEventArgs> ItemActivated
    {
        add => AddHandler(WxEvents.ListItemActivated, value);
        remove => RemoveHandler(WxEvents.ListItemActivated, value);
    }

    /// <summary>The keyboard focus moved to a row. This is what assistive technology follows, and it fires
    /// even when the selection does not move.</summary>
    public event EventHandler<ListEventArgs> ItemFocused
    {
        add => AddHandler(WxEvents.ListItemFocused, value);
        remove => RemoveHandler(WxEvents.ListItemFocused, value);
    }

    /// <summary>A row was right-clicked. For the keyboard route, handle <see cref="Window.ContextMenu"/>.</summary>
    public event EventHandler<ListEventArgs> ItemRightClicked
    {
        add => AddHandler(WxEvents.ListItemRightClicked, value);
        remove => RemoveHandler(WxEvents.ListItemRightClicked, value);
    }

    /// <summary>A column header was clicked - the usual signal to re-sort.</summary>
    public event EventHandler<ListEventArgs> ColumnClicked
    {
        add => AddHandler(WxEvents.ListColumnClicked, value);
        remove => RemoveHandler(WxEvents.ListColumnClicked, value);
    }

    /// <summary>A key was pressed in the list. <see cref="ListEventArgs.Index"/> is the focused row.</summary>
    public event EventHandler<ListEventArgs> ItemKeyDown
    {
        add => AddHandler(WxEvents.ListKeyDown, value);
        remove => RemoveHandler(WxEvents.ListKeyDown, value);
    }

    /// <summary>Label editing is starting. Veto to refuse.</summary>
    public event EventHandler<ListEventArgs> BeginLabelEdit
    {
        add => AddHandler(WxEvents.ListBeginLabelEdit, value);
        remove => RemoveHandler(WxEvents.ListBeginLabelEdit, value);
    }

    /// <summary>Label editing finished. <see cref="ListEventArgs.Label"/> is the new text; veto to reject it.</summary>
    public event EventHandler<ListEventArgs> EndLabelEdit
    {
        add => AddHandler(WxEvents.ListEndLabelEdit, value);
        remove => RemoveHandler(WxEvents.ListEndLabelEdit, value);
    }

    /// <summary>A row check box was ticked. Requires check boxes to have been enabled on the control.</summary>
    public event EventHandler<ListEventArgs> ItemChecked
    {
        add => AddHandler(WxEvents.ListItemChecked, value);
        remove => RemoveHandler(WxEvents.ListItemChecked, value);
    }

    public event EventHandler<ListEventArgs> ItemUnchecked
    {
        add => AddHandler(WxEvents.ListItemUnchecked, value);
        remove => RemoveHandler(WxEvents.ListItemUnchecked, value);
    }

    public event EventHandler<ListEventArgs> ItemMiddleClicked
    {
        add => AddHandler(WxEvents.ListItemMiddleClicked, value);
        remove => RemoveHandler(WxEvents.ListItemMiddleClicked, value);
    }

    public event EventHandler<ListEventArgs> ColumnRightClicked
    {
        add => AddHandler(WxEvents.ListColumnRightClicked, value);
        remove => RemoveHandler(WxEvents.ListColumnRightClicked, value);
    }

    public event EventHandler<ListEventArgs> ItemDeleted
    {
        add => AddHandler(WxEvents.ListItemDeleted, value);
        remove => RemoveHandler(WxEvents.ListItemDeleted, value);
    }

    public event EventHandler<ListEventArgs> AllItemsDeleted
    {
        add => AddHandler(WxEvents.ListAllItemsDeleted, value);
        remove => RemoveHandler(WxEvents.ListAllItemsDeleted, value);
    }

    /// <summary>A drag is starting. Veto to refuse it.</summary>
    public event EventHandler<ListEventArgs> BeginDrag
    {
        add => AddHandler(WxEvents.ListBeginDrag, value);
        remove => RemoveHandler(WxEvents.ListBeginDrag, value);
    }

    public ListCtrl(Window parent, int id = WindowId.Any, ListCtrlStyle style = ListCtrlStyle.Default)
        : base(parent, id)
        => Initialize(NativeMethods.wxsharp_listctrl_create(parent.Handle, id, (int)style, Token));

    public long Count => NativeMethods.wxsharp_listctrl_count(Handle);

    // ---- Columns -------------------------------------------------------------------------------------

    public int ColumnCount => NativeMethods.wxsharp_listctrl_column_count(Handle);
    public int InsertColumn(int column, string heading, int width = -1) => NativeMethods.wxsharp_listctrl_insert_column(Handle, column, heading, width);
    public bool RemoveColumn(int column) => NativeMethods.wxsharp_listctrl_delete_column(Handle, column);
    public void ClearColumns() => NativeMethods.wxsharp_listctrl_clear_columns(Handle);
    public int GetColumnWidth(int column) => NativeMethods.wxsharp_listctrl_get_column_width(Handle, column);
    public bool SetColumnWidth(int column, int width) => NativeMethods.wxsharp_listctrl_set_column_width(Handle, column, width);

    /// <summary>Sizes a column to its widest cell, or to its header when <paramref name="useHeader"/> is true.</summary>
    public bool AutoSizeColumn(int column, bool useHeader = false)
        => NativeMethods.wxsharp_listctrl_set_column_width(Handle, column, useHeader ? -2 : -1);

    public unsafe string GetColumnHeading(int column)
    {
        var length = NativeMethods.wxsharp_listctrl_get_column_heading(Handle, column, null, 0);
        if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1];
        fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_listctrl_get_column_heading(Handle, column, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }

    public bool SetColumnHeading(int column, string heading) => NativeMethods.wxsharp_listctrl_set_column_heading(Handle, column, heading);

    // ---- Rows ----------------------------------------------------------------------------------------

    public long AddItem(string text) => NativeMethods.wxsharp_listctrl_insert_item(Handle, Count, text);
    public long InsertItem(long index, string text) => NativeMethods.wxsharp_listctrl_insert_item(Handle, index, text);
    public bool SetItem(long item, int column, string text) => NativeMethods.wxsharp_listctrl_set_item(Handle, item, column, text);
    public unsafe string GetItem(long item, int column = 0)
    {
        var length = NativeMethods.wxsharp_listctrl_get_item(Handle, item, column, null, 0); if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_listctrl_get_item(Handle, item, column, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }
    public bool RemoveAt(long item) => NativeMethods.wxsharp_listctrl_delete_item(Handle, item);
    public void Clear() => NativeMethods.wxsharp_listctrl_clear(Handle);

    // ---- Selection, focus and visibility -------------------------------------------------------------

    public void SetSelected(long item, bool selected = true) => NativeMethods.wxsharp_listctrl_select(Handle, item, selected);
    public bool IsSelected(long item) => NativeMethods.wxsharp_listctrl_is_selected(Handle, item);
    public int SelectedCount => NativeMethods.wxsharp_listctrl_selected_count(Handle);

    /// <summary>The single selected row, or -1 when the selection is empty or holds more than one row.</summary>
    public long SelectedIndex
    {
        get
        {
            var first = NativeMethods.wxsharp_listctrl_next_selected(Handle, -1);
            return first >= 0 && NativeMethods.wxsharp_listctrl_next_selected(Handle, first) < 0 ? first : -1;
        }
        set
        {
            foreach (var selected in GetSelectedIndices()) SetSelected(selected, false);
            if (value >= 0) { SetSelected(value); SetFocused(value); }
        }
    }

    /// <summary>Every selected row, in order.</summary>
    public long[] GetSelectedIndices()
    {
        var count = SelectedCount;
        if (count <= 0) return Array.Empty<long>();
        var items = new List<long>(count);
        for (var item = NativeMethods.wxsharp_listctrl_next_selected(Handle, -1); item >= 0;
             item = NativeMethods.wxsharp_listctrl_next_selected(Handle, item))
            items.Add(item);
        return items.ToArray();
    }

    /// <summary>The row the keyboard is on, or -1. Distinct from the selection.</summary>
    public long FocusedIndex => NativeMethods.wxsharp_listctrl_get_focused(Handle);

    /// <summary>Moves keyboard focus to a row without changing the selection. This is what a screen reader
    /// follows, so it is the call that makes it announce the row.</summary>
    public void SetFocused(long item) => NativeMethods.wxsharp_listctrl_set_focused(Handle, item);

    /// <summary>Scrolls so a row is visible.</summary>
    public void EnsureVisible(long item) => NativeMethods.wxsharp_listctrl_ensure_visible(Handle, item);
}

public readonly record struct TreeItemId(long Value)
{
    public bool IsValid => Value != 0;
    public static TreeItemId None => default;
}

public class TreeCtrl : Control
{
    public event EventHandler<TreeEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.TreeSelectionChanged, value);
        remove => RemoveHandler(WxEvents.TreeSelectionChanged, value);
    }

    /// <summary>The selection is about to move. Veto to keep it where it is.</summary>
    public event EventHandler<TreeEventArgs> SelectionChanging
    {
        add => AddHandler(WxEvents.TreeSelectionChanging, value);
        remove => RemoveHandler(WxEvents.TreeSelectionChanging, value);
    }

    /// <summary>An item was activated by double-click or Enter.</summary>
    public event EventHandler<TreeEventArgs> ItemActivated
    {
        add => AddHandler(WxEvents.TreeItemActivated, value);
        remove => RemoveHandler(WxEvents.TreeItemActivated, value);
    }

    /// <summary>An item is about to expand. Veto to refuse, or fill in its children first for a tree that
    /// is built on demand.</summary>
    public event EventHandler<TreeEventArgs> ItemExpanding
    {
        add => AddHandler(WxEvents.TreeItemExpanding, value);
        remove => RemoveHandler(WxEvents.TreeItemExpanding, value);
    }

    public event EventHandler<TreeEventArgs> ItemExpanded
    {
        add => AddHandler(WxEvents.TreeItemExpanded, value);
        remove => RemoveHandler(WxEvents.TreeItemExpanded, value);
    }

    /// <summary>An item is about to collapse. Veto to refuse.</summary>
    public event EventHandler<TreeEventArgs> ItemCollapsing
    {
        add => AddHandler(WxEvents.TreeItemCollapsing, value);
        remove => RemoveHandler(WxEvents.TreeItemCollapsing, value);
    }

    public event EventHandler<TreeEventArgs> ItemCollapsed
    {
        add => AddHandler(WxEvents.TreeItemCollapsed, value);
        remove => RemoveHandler(WxEvents.TreeItemCollapsed, value);
    }

    public event EventHandler<TreeEventArgs> ItemRightClicked
    {
        add => AddHandler(WxEvents.TreeItemRightClicked, value);
        remove => RemoveHandler(WxEvents.TreeItemRightClicked, value);
    }

    /// <summary>A key was pressed in the tree. <see cref="TreeEventArgs.Item"/> is the focused item.</summary>
    public event EventHandler<TreeEventArgs> ItemKeyDown
    {
        add => AddHandler(WxEvents.TreeKeyDown, value);
        remove => RemoveHandler(WxEvents.TreeKeyDown, value);
    }

    /// <summary>Label editing is starting. Veto to refuse. Requires TreeCtrlStyle.EditLabels.</summary>
    public event EventHandler<TreeEventArgs> BeginLabelEdit
    {
        add => AddHandler(WxEvents.TreeBeginLabelEdit, value);
        remove => RemoveHandler(WxEvents.TreeBeginLabelEdit, value);
    }

    /// <summary>Label editing finished. <see cref="TreeEventArgs.Label"/> is the new text; veto to reject it.</summary>
    public event EventHandler<TreeEventArgs> EndLabelEdit
    {
        add => AddHandler(WxEvents.TreeEndLabelEdit, value);
        remove => RemoveHandler(WxEvents.TreeEndLabelEdit, value);
    }

    /// <summary>A context menu was asked for on an item, by right-click or by the keyboard menu key. Prefer this over <see cref="ItemRightClicked"/>, which misses the keyboard route.</summary>
    public event EventHandler<TreeEventArgs> ItemMenu
    {
        add => AddHandler(WxEvents.TreeItemMenu, value);
        remove => RemoveHandler(WxEvents.TreeItemMenu, value);
    }

    /// <summary>A tooltip is wanted for an item.</summary>
    public event EventHandler<TreeEventArgs> ItemToolTip
    {
        add => AddHandler(WxEvents.TreeItemToolTip, value);
        remove => RemoveHandler(WxEvents.TreeItemToolTip, value);
    }

    /// <summary>An item was deleted.</summary>
    public event EventHandler<TreeEventArgs> ItemDeleted
    {
        add => AddHandler(WxEvents.TreeItemDeleted, value);
        remove => RemoveHandler(WxEvents.TreeItemDeleted, value);
    }

    public event EventHandler<TreeEventArgs> ItemMiddleClicked
    {
        add => AddHandler(WxEvents.TreeItemMiddleClicked, value);
        remove => RemoveHandler(WxEvents.TreeItemMiddleClicked, value);
    }

    /// <summary>A drag is starting. Veto to refuse it.</summary>
    public event EventHandler<TreeEventArgs> BeginDrag
    {
        add => AddHandler(WxEvents.TreeBeginDrag, value);
        remove => RemoveHandler(WxEvents.TreeBeginDrag, value);
    }

    public event EventHandler<TreeEventArgs> EndDrag
    {
        add => AddHandler(WxEvents.TreeEndDrag, value);
        remove => RemoveHandler(WxEvents.TreeEndDrag, value);
    }

    public TreeCtrl(Window parent, int id = WindowId.Any, TreeCtrlStyle style = TreeCtrlStyle.Default)
        : base(parent, id)
        => Initialize(NativeMethods.wxsharp_treectrl_create(parent.Handle, id, (int)style, Token));

    /// <summary>The root item. Hidden from the user when the control was created with
    /// <see cref="TreeCtrlStyle.HideRoot"/>, but still the parent to add top-level items to.</summary>
    public TreeItemId Root => new(NativeMethods.wxsharp_tree_get_root(Handle));

    public TreeItemId AddRoot(string text) => new(NativeMethods.wxsharp_tree_add_root(Handle, text));
    public TreeItemId Add(TreeItemId parent, string text) => new(NativeMethods.wxsharp_tree_append(Handle, parent.Value, text));

    /// <summary>Inserts a child before <paramref name="position"/> rather than at the end.</summary>
    public TreeItemId Insert(TreeItemId parent, int position, string text)
        => new(NativeMethods.wxsharp_tree_insert(Handle, parent.Value, position, text));

    // ---- Walking the tree ------------------------------------------------------------------------------

    public TreeItemId GetParent(TreeItemId item) => new(NativeMethods.wxsharp_tree_get_parent(Handle, item.Value));
    public TreeItemId GetFirstChild(TreeItemId item) => new(NativeMethods.wxsharp_tree_get_first_child(Handle, item.Value));
    public TreeItemId GetNextSibling(TreeItemId item) => new(NativeMethods.wxsharp_tree_get_next_sibling(Handle, item.Value));
    public TreeItemId GetPreviousSibling(TreeItemId item) => new(NativeMethods.wxsharp_tree_get_prev_sibling(Handle, item.Value));

    /// <summary>How many children an item has, counting the whole subtree when <paramref name="recursive"/>
    /// is true.</summary>
    public int GetChildCount(TreeItemId item, bool recursive = false)
        => NativeMethods.wxsharp_tree_child_count(Handle, item.Value, recursive);

    /// <summary>An item's children, in order.</summary>
    public TreeItemId[] GetChildren(TreeItemId item)
    {
        var children = new List<TreeItemId>();
        for (var child = GetFirstChild(item); child.IsValid; child = GetNextSibling(child))
            children.Add(child);
        return children.ToArray();
    }

    /// <summary>Scrolls and expands ancestors so an item is visible.</summary>
    public void EnsureVisible(TreeItemId item) => NativeMethods.wxsharp_tree_ensure_visible(Handle, item.Value);

    /// <summary>Clears the selection.</summary>
    public void Unselect() => NativeMethods.wxsharp_tree_unselect(Handle);
    public void Remove(TreeItemId item) => NativeMethods.wxsharp_tree_delete(Handle, item.Value);
    /// <summary>Deletes every item, root included. <see cref="Root"/> becomes invalid until a new one is added.</summary>
    public void Clear() => NativeMethods.wxsharp_tree_delete_all(Handle);
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
    public event EventHandler<DataViewEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.DataViewSelectionChanged, value);
        remove => RemoveHandler(WxEvents.DataViewSelectionChanged, value);
    }
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
}

public readonly record struct DataViewItem(long Value)
{
    public bool IsValid => Value != 0;
    public static DataViewItem Root => default;
}

public class DataViewTreeCtrl : Control
{
    public event EventHandler<DataViewEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.DataViewSelectionChanged, value);
        remove => RemoveHandler(WxEvents.DataViewSelectionChanged, value);
    }
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
}
