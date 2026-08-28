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
        => Initialize(GetType() == typeof(ListCtrl)
            ? NativeMethods.wxsharp_listctrl_create(parent.Handle, id, (int)style, Token)
            : NativeMethods.wxsharp_custom_listctrl_create(parent.Handle, id, (int)style, Token));

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
    public bool RemoveAt(long item)
    {
        _itemData?.Remove(item);
        return NativeMethods.wxsharp_listctrl_delete_item(Handle, item);
    }
    public void Clear()
    {
        _itemData?.Clear();
        NativeMethods.wxsharp_listctrl_clear(Handle);
    }

    /// <summary>Removes every row <em>and</em> every column, following <c>wxListCtrl.ClearAll</c>.
    /// <see cref="Clear"/> removes only the rows, which is the distinction wxWidgets draws.</summary>
    public void ClearAll()
    {
        Clear();
        NativeMethods.wxsharp_listctrl_clear_columns(Handle);
    }

    // Held managed-side for the same reason as TreeCtrl's: wxListCtrl's item data is a native pointer-sized
    // value, and handing it a managed object's address to keep across a collection is a lifetime bug
    // waiting for load. The row index is the key, so the data moves with nothing and is dropped with the row.
    private Dictionary<long, object?>? _itemData;

    /// <summary>Attaches an arbitrary object to a row, following <c>wxListCtrl.SetItemData</c>.</summary>
    public void SetItemData(long item, object? data)
    {
        Verify();
        (_itemData ??= [])[item] = data;
    }

    /// <summary>The object attached to a row by <see cref="SetItemData"/>, or null. Follows
    /// <c>wxListCtrl.GetItemData</c>.</summary>
    public object? GetItemData(long item)
    {
        Verify();
        return _itemData is not null && _itemData.TryGetValue(item, out var data) ? data : null;
    }

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

    /// <summary>The first selected row, or -1 when nothing is selected. Follows
    /// <c>wxListCtrl.GetFirstSelected</c>; pair it with <see cref="GetNextSelected"/> to walk a
    /// multiple selection without materialising it.</summary>
    public long GetFirstSelected() => NativeMethods.wxsharp_listctrl_next_selected(Handle, -1);

    /// <summary>The next selected row after <paramref name="item"/>, or -1 when there is none. Follows
    /// <c>wxListCtrl.GetNextSelected</c>.</summary>
    public long GetNextSelected(long item) => NativeMethods.wxsharp_listctrl_next_selected(Handle, item);

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

    // ---- Virtual mode --------------------------------------------------------------------------------

    /// <summary>Tells a virtual list how many rows it has. The control then asks
    /// <see cref="OnGetItemText"/> for each row as it draws it, so a list of any size costs only what is
    /// on screen. Requires <see cref="ListCtrlStyle.Virtual"/>.</summary>
    public void SetItemCount(long count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        NativeMethods.wxsharp_listctrl_set_item_count(Handle, count);
    }

    /// <summary>Supplies the text of one cell of a virtual list. Override it when the control was created
    /// with <see cref="ListCtrlStyle.Virtual"/>; it is called only for the rows actually being drawn, so it
    /// must be quick and free of side effects. Follows <c>wxListCtrl.OnGetItemText</c>.</summary>
    protected virtual string OnGetItemText(long item, int column) => string.Empty;

    /// <summary>Supplies the image-list index for an item in virtual mode. Phoenix exposes this protected
    /// <c>wxListCtrl.OnGetItemImage</c> hook; -1 means no image.</summary>
    protected virtual int OnGetItemImage(long item) => -1;

    /// <summary>Supplies the image-list index for a particular column in virtual mode.</summary>
    protected virtual int OnGetItemColumnImage(long item, int column)
        => column == 0 ? OnGetItemImage(item) : -1;

    /// <summary>Supplies the checked state for an item in a virtual list with check boxes.</summary>
    protected virtual bool OnGetItemIsChecked(long item) => false;

    internal string GetVirtualItemText(long item, int column) => OnGetItemText(item, column) ?? string.Empty;
    internal int GetVirtualItemImage(long item) => OnGetItemImage(item);
    internal int GetVirtualItemColumnImage(long item, int column) => OnGetItemColumnImage(item, column);
    internal bool GetVirtualItemIsChecked(long item) => OnGetItemIsChecked(item);

    /// <summary>Redraws one row, after the data behind a virtual list changed.</summary>
    public void RefreshItem(long item) => NativeMethods.wxsharp_listctrl_refresh_item(Handle, item);

    /// <summary>Redraws an inclusive range of rows.</summary>
    public void RefreshItems(long from, long to) => NativeMethods.wxsharp_listctrl_refresh_items(Handle, from, to);

    /// <summary>Gives the control the images its items are drawn with, following
    /// <c>wxListCtrl.SetImageList</c> and <c>AssignImageList</c>.</summary>
    /// <param name="transfer">True to hand the list to the control, which then destroys it. False to lend
    /// it, in which case you must keep it alive for as long as the control uses it.</param>
    public void SetImageList(ImageList images, ImageListKind kind = ImageListKind.Small, bool transfer = true)
    {
        ArgumentNullException.ThrowIfNull(images);
        Verify();
        NativeMethods.wxsharp_listctrl_set_image_list(Handle, images.Handle, (int)kind, transfer);
        if (transfer) images.Detach();
    }

    /// <summary>Draws one of the image list's images beside a row. Follows
    /// <c>wxListCtrl.SetItemImage</c>.</summary>
    public void SetItemImage(long item, int image)
        => NativeMethods.wxsharp_listctrl_set_item_image(Handle, item, image);
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
        => Initialize(GetType() == typeof(TreeCtrl)
            ? NativeMethods.wxsharp_treectrl_create(parent.Handle, id, (int)style, Token)
            : NativeMethods.wxsharp_custom_treectrl_create(parent.Handle, id, (int)style, Token));

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
    public void Remove(TreeItemId item)
    {
        // Drop the item's data with the item. wxWidgets is free to hand the same ID out again, so leaving
        // it behind would both leak and let a new item inherit a deleted one's data.
        _itemData?.Remove(item.Value);
        NativeMethods.wxsharp_tree_delete(Handle, item.Value);
    }
    /// <summary>Deletes every item, root included. <see cref="Root"/> becomes invalid until a new one is added.</summary>
    public void Clear()
    {
        _itemData?.Clear();
        NativeMethods.wxsharp_tree_delete_all(Handle);
    }
    public unsafe string GetText(TreeItemId item)
    {
        var length = NativeMethods.wxsharp_tree_get_text(Handle, item.Value, null, 0); if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_tree_get_text(Handle, item.Value, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }
    public void SetText(TreeItemId item, string text) => NativeMethods.wxsharp_tree_set_text(Handle, item.Value, text);
    public void SortChildren(TreeItemId item) => NativeMethods.wxsharp_tree_sort_children(Handle, item.Value);

    /// <summary>Compares two children during <see cref="SortChildren"/>. Override to provide the ordering,
    /// following Phoenix's <c>wxTreeCtrl.OnCompareItems</c>.</summary>
    protected virtual int OnCompareItems(TreeItemId first, TreeItemId second)
        => string.CompareOrdinal(GetText(first), GetText(second));

    internal int CompareItems(TreeItemId first, TreeItemId second) => OnCompareItems(first, second);
    public void Expand(TreeItemId item, bool expand = true) => NativeMethods.wxsharp_tree_expand(Handle, item.Value, expand);
    public bool IsExpanded(TreeItemId item) => NativeMethods.wxsharp_tree_is_expanded(Handle, item.Value);
    public TreeItemId Selection { get => new(NativeMethods.wxsharp_tree_get_selection(Handle)); set => NativeMethods.wxsharp_tree_select(Handle, value.Value); }

    /// <summary>How many items the tree holds, root included. Follows <c>wxTreeCtrl.GetCount</c>.</summary>
    public int Count => NativeMethods.wxsharp_tree_get_count(Handle);

    /// <summary>Expands every item. Follows <c>wxTreeCtrl.ExpandAll</c>.</summary>
    public void ExpandAll() => NativeMethods.wxsharp_tree_expand_all(Handle);

    /// <summary>Collapses every item. Follows <c>wxTreeCtrl.CollapseAll</c>.</summary>
    public void CollapseAll() => NativeMethods.wxsharp_tree_collapse_all(Handle);

    /// <summary>Whether an item has children, or has been marked as having them. Follows
    /// <c>wxTreeCtrl.ItemHasChildren</c>, which is not the same question as
    /// <see cref="GetChildCount"/> being non-zero: a tree filled on demand marks a branch as having
    /// children before it has any, so the expander is drawn and the branch can be opened.</summary>
    public bool ItemHasChildren(TreeItemId item) => NativeMethods.wxsharp_tree_item_has_children(Handle, item.Value);

    // Item data is held here rather than in wxWidgets. wxTreeItemData is an owned native object, and
    // handing a managed object's address to C++ to hold across a garbage collection is the kind of lifetime
    // bug that only shows up under load. A dictionary keyed by the item's own ID gives the same API with
    // none of that, and the tree's item IDs are stable for as long as the item exists.
    private Dictionary<long, object?>? _itemData;

    /// <summary>Attaches an arbitrary object to an item, following <c>wxTreeCtrl.SetItemData</c>. This is
    /// how a tree row is tied to whatever it stands for - the page a settings tree shows, the record a
    /// result row came from - without a parallel lookup table in the caller.</summary>
    public void SetItemData(TreeItemId item, object? data)
    {
        Verify();
        if (!item.IsValid) throw new ArgumentException("The item is not valid.", nameof(item));
        (_itemData ??= [])[item.Value] = data;
    }

    /// <summary>The object attached to an item by <see cref="SetItemData"/>, or null. Follows
    /// <c>wxTreeCtrl.GetItemData</c>.</summary>
    public object? GetItemData(TreeItemId item)
    {
        Verify();
        return _itemData is not null && _itemData.TryGetValue(item.Value, out var data) ? data : null;
    }

    /// <summary>Gives the control the images its items are drawn with, following
    /// <c>wxTreeCtrl.SetImageList</c> and <c>AssignImageList</c>.</summary>
    /// <param name="transfer">True to hand the list to the control, which then destroys it. False to lend
    /// it, in which case you must keep it alive for as long as the control uses it.</param>
    public void SetImageList(ImageList images, bool transfer = true)
    {
        ArgumentNullException.ThrowIfNull(images);
        Verify();
        NativeMethods.wxsharp_treectrl_set_image_list(Handle, images.Handle, transfer);
        if (transfer) images.Detach();
    }

    /// <summary>Draws one of the image list's images beside an item. A tree item has four, so a branch can
    /// look different open than closed. Follows <c>wxTreeCtrl.SetItemImage</c>.</summary>
    public void SetItemImage(TreeItemId item, int image, TreeItemIcon which = TreeItemIcon.Normal)
        => NativeMethods.wxsharp_treectrl_set_item_image(Handle, item.Value, image, (int)which);

    /// <summary>The image drawn beside an item, or -1 when it has none.</summary>
    public int GetItemImage(TreeItemId item, TreeItemIcon which = TreeItemIcon.Normal)
        => NativeMethods.wxsharp_treectrl_get_item_image(Handle, item.Value, (int)which);
}

public class Grid : Control
{
    public Grid(Window parent, int rows = 0, int columns = 0, int id = WindowId.Any) : base(parent, id)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rows); ArgumentOutOfRangeException.ThrowIfNegative(columns);
        Initialize(GetType() == typeof(Grid)
            ? NativeMethods.wxsharp_grid_create(parent.Handle, id, rows, columns, Token)
            : NativeMethods.wxsharp_custom_grid_create(parent.Handle, id, rows, columns, Token));
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

    // ---- Overridable wxGrid virtuals --------------------------------------------------------------------

    /// <summary>The pen the line to the right of a column is drawn with. Overriding it is how a grid marks
    /// one column differently from the rest. Follows <c>wxGrid.GetColGridLinePen</c>.</summary>
    public virtual Pen GetColGridLinePen(int column) => PenFrom(CallBase(VirtualMember.GridColLinePen, column));

    /// <summary>The pen the line below a row is drawn with. Follows
    /// <c>wxGrid.GetRowGridLinePen</c>.</summary>
    public virtual Pen GetRowGridLinePen(int row) => PenFrom(CallBase(VirtualMember.GridRowLinePen, row));

    /// <summary>The pen every other grid line is drawn with. Follows
    /// <c>wxGrid.GetDefaultGridLinePen</c>.</summary>
    public virtual Pen GetDefaultGridLinePen() => PenFrom(CallBase(VirtualMember.GridDefaultLinePen));

    private static Pen PenFrom(in NativeVirtualRequest request)
        => new(Colour.FromArgb(request.UintValue), request.Result > 0 ? request.Result : 1);

    internal override unsafe bool TryAnswerVirtual(ref NativeVirtualRequest request)
    {
        switch ((VirtualMember)request.Which)
        {
            case VirtualMember.GridColLinePen: return Answer(GetColGridLinePen(request.Args[0]), ref request);
            case VirtualMember.GridRowLinePen: return Answer(GetRowGridLinePen(request.Args[0]), ref request);
            case VirtualMember.GridDefaultLinePen: return Answer(GetDefaultGridLinePen(), ref request);
            default: return base.TryAnswerVirtual(ref request);
        }

        static bool Answer(Pen pen, ref NativeVirtualRequest request)
        {
            request.UintValue = pen.Colour.ToArgb();
            request.Result = pen.Width;
            return true;
        }
    }
}

public class DataViewListCtrl : Control
{
    public event EventHandler<DataViewEventArgs> SelectionChanged
    {
        add => AddHandler(WxEvents.DataViewSelectionChanged, value);
        remove => RemoveHandler(WxEvents.DataViewSelectionChanged, value);
    }
    public DataViewListCtrl(Window parent, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(DataViewListCtrl)
            ? NativeMethods.wxsharp_dataviewlist_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_dataviewlist_create(parent.Handle, id, Token));
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
        => Initialize(GetType() == typeof(DataViewTreeCtrl)
            ? NativeMethods.wxsharp_dataviewtree_create(parent.Handle, id, Token)
            : NativeMethods.wxsharp_custom_dataviewtree_create(parent.Handle, id, Token));
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
