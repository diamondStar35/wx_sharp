using System;

namespace WxSharp;

/// <summary>Which way a <see cref="FlexGridSizer"/> is allowed to grow, following
/// <c>wxFlexGridSizer.SetFlexibleDirection</c>.</summary>
public enum FlexDirection
{
    Horizontal = 0,
    Vertical = 1,
    /// <summary>Both, which is the default.</summary>
    Both = 2,
}

/// <summary>What a <see cref="FlexGridSizer"/> does with rows or columns that were not made growable,
/// following <c>wxFlexSizerGrowMode</c>.</summary>
public enum FlexGrowMode
{
    /// <summary>They do not grow at all.</summary>
    None = 0,
    /// <summary>Only the ones named by <see cref="FlexGridSizer.AddGrowableRow"/> grow. The default.</summary>
    Specified = 1,
    /// <summary>They all grow equally.</summary>
    All = 2,
}

/// <summary>A grid where every cell is the same size.</summary>
public class GridSizer : Sizer
{
    public GridSizer(int rows, int columns, int verticalGap = 0, int horizontalGap = 0)
        : base(Create(rows, columns, verticalGap, horizontalGap)) { }

    /// <summary>The requested row count. 0 means "as many as the columns require".</summary>
    public int Rows
    {
        get => NativeMethods.wxsharp_gridsizer_get_rows(Handle);
        set => NativeMethods.wxsharp_gridsizer_set_rows(Handle, value);
    }

    /// <summary>The requested column count. 0 means "as many as the rows require".</summary>
    public int Columns
    {
        get => NativeMethods.wxsharp_gridsizer_get_columns(Handle);
        set => NativeMethods.wxsharp_gridsizer_set_columns(Handle, value);
    }

    /// <summary>The gap between rows, in pixels.</summary>
    public int VerticalGap
    {
        get => NativeMethods.wxsharp_gridsizer_get_vertical_gap(Handle);
        set => NativeMethods.wxsharp_gridsizer_set_vertical_gap(Handle, value);
    }

    /// <summary>The gap between columns, in pixels.</summary>
    public int HorizontalGap
    {
        get => NativeMethods.wxsharp_gridsizer_get_horizontal_gap(Handle);
        set => NativeMethods.wxsharp_gridsizer_set_horizontal_gap(Handle, value);
    }

    /// <summary>The row count the sizer actually uses, once the items present are taken into account.</summary>
    public int EffectiveRows => NativeMethods.wxsharp_gridsizer_effective_rows(Handle);

    /// <summary>The column count the sizer actually uses.</summary>
    public int EffectiveColumns => NativeMethods.wxsharp_gridsizer_effective_columns(Handle);

    private protected GridSizer(nint handle) : base(handle) { }

    private static nint Create(int rows, int columns, int verticalGap, int horizontalGap)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rows); ArgumentOutOfRangeException.ThrowIfNegative(columns);
        if (rows == 0 && columns == 0) throw new ArgumentException("Rows and columns cannot both be zero.");
        ArgumentOutOfRangeException.ThrowIfNegative(verticalGap); ArgumentOutOfRangeException.ThrowIfNegative(horizontalGap);
        return NativeMethods.wxsharp_gridsizer_create(rows, columns, verticalGap, horizontalGap);
    }
}

/// <summary>A grid whose rows and columns size themselves to their contents, and where chosen rows and
/// columns take the spare space.</summary>
public class FlexGridSizer : GridSizer
{
    public FlexGridSizer(int rows, int columns, int verticalGap = 0, int horizontalGap = 0)
        : base(Create(rows, columns, verticalGap, horizontalGap)) { }

    /// <summary>Lets a row take spare vertical space, in proportion to the other growable rows.</summary>
    public void AddGrowableRow(int row, int proportion = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        NativeMethods.wxsharp_flexgridsizer_add_growable_row(Handle, row, proportion);
    }

    /// <summary>Lets a column take spare horizontal space.</summary>
    public void AddGrowableColumn(int column, int proportion = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        NativeMethods.wxsharp_flexgridsizer_add_growable_column(Handle, column, proportion);
    }

    public void RemoveGrowableRow(int row) => NativeMethods.wxsharp_flexgridsizer_remove_growable_row(Handle, row);
    public void RemoveGrowableColumn(int column) => NativeMethods.wxsharp_flexgridsizer_remove_growable_column(Handle, column);
    public bool IsRowGrowable(int row) => NativeMethods.wxsharp_flexgridsizer_is_row_growable(Handle, row);
    public bool IsColumnGrowable(int column) => NativeMethods.wxsharp_flexgridsizer_is_column_growable(Handle, column);

    /// <summary>Which directions the sizer may grow in. Defaults to <see cref="FlexDirection.Both"/>.</summary>
    public FlexDirection FlexibleDirection
    {
        get => (FlexDirection)NativeMethods.wxsharp_flexgridsizer_get_flexible_direction(Handle);
        set => NativeMethods.wxsharp_flexgridsizer_set_flexible_direction(Handle, (int)value);
    }

    /// <summary>What happens to rows and columns that were not made growable.</summary>
    public FlexGrowMode NonFlexibleGrowMode
    {
        get => (FlexGrowMode)NativeMethods.wxsharp_flexgridsizer_get_grow_mode(Handle);
        set => NativeMethods.wxsharp_flexgridsizer_set_grow_mode(Handle, (int)value);
    }

    /// <summary>The heights the sizer last gave its rows.</summary>
    public unsafe int[] GetRowHeights() => ReadInts(NativeMethods.wxsharp_flexgridsizer_row_heights);

    /// <summary>The widths the sizer last gave its columns.</summary>
    public unsafe int[] GetColumnWidths() => ReadInts(NativeMethods.wxsharp_flexgridsizer_column_widths);

    private unsafe delegate int IntReader(nint sizer, int* buffer, int length);

    private unsafe int[] ReadInts(IntReader read)
    {
        var count = read(Handle, null, 0);
        if (count <= 0) return Array.Empty<int>();
        var values = new int[count];
        fixed (int* p = values) _ = read(Handle, p, count);
        return values;
    }

    // wxGridBagSizer derives from wxFlexGridSizer, so it comes in through here with its own native handle.
    private protected FlexGridSizer(nint handle) : base(handle) { }

    private static nint Create(int rows, int columns, int verticalGap, int horizontalGap)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rows); ArgumentOutOfRangeException.ThrowIfNegative(columns);
        if (rows == 0 && columns == 0) throw new ArgumentException("Rows and columns cannot both be zero.");
        return NativeMethods.wxsharp_flexgridsizer_create(rows, columns, verticalGap, horizontalGap);
    }
}

/// <summary>A box sizer that draws a labelled frame around its contents.</summary>
public class StaticBoxSizer : Sizer
{
    public StaticBoxSizer(StaticBox box, Orientation orientation = Orientation.Vertical)
        : base(NativeMethods.wxsharp_staticboxsizer_create(box?.Handle ?? throw new ArgumentNullException(nameof(box)),
            orientation == Orientation.Horizontal))
        => Box = box;

    /// <summary>The frame this sizer draws. Its label is the frame's caption.</summary>
    public StaticBox Box { get; }
}

/// <summary>A grid where each item names its own cell and may span several, following
/// <c>wxGridBagSizer</c>.</summary>
public class GridBagSizer : FlexGridSizer
{
    public GridBagSizer(int verticalGap = 0, int horizontalGap = 0)
        : base(NativeMethods.wxsharp_gridbagsizer_create(verticalGap, horizontalGap)) { }

    /// <summary>Places a window at a cell, optionally spanning several rows or columns.</summary>
    public SizerItem AddAt(Window window, int row, int column, int rowSpan = 1, int columnSpan = 1,
        SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(window);
        ValidateCell(row, column, rowSpan, columnSpan, border);
        return new SizerItem(NativeMethods.wxsharp_gridbagsizer_add_control(Handle, window.Handle, row, column,
            rowSpan, columnSpan, (int)flags, border));
    }

    /// <summary>Places a nested sizer at a cell.</summary>
    public SizerItem AddAt(Sizer child, int row, int column, int rowSpan = 1, int columnSpan = 1,
        SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(child);
        ValidateCell(row, column, rowSpan, columnSpan, border);
        return new SizerItem(NativeMethods.wxsharp_gridbagsizer_add_sizer(Handle, child.Handle, row, column,
            rowSpan, columnSpan, (int)flags, border));
    }

    /// <summary>The cell a window occupies.</summary>
    public (int Row, int Column) GetItemPosition(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        NativeMethods.wxsharp_gridbagsizer_get_item_position_control(Handle, window.Handle, out var row, out var column);
        return (row, column);
    }

    public (int Row, int Column) GetItemPositionAt(int index)
    {
        NativeMethods.wxsharp_gridbagsizer_get_item_position_at(Handle, index, out var row, out var column);
        return (row, column);
    }

    /// <summary>Moves a window to another cell. Ask <see cref="CheckForIntersection"/> first: wxWidgets
    /// asserts when the target cell is already taken, rather than quietly refusing.</summary>
    public bool SetItemPosition(Window window, int row, int column)
    {
        ArgumentNullException.ThrowIfNull(window);
        return NativeMethods.wxsharp_gridbagsizer_set_item_position_control(Handle, window.Handle, row, column);
    }

    public bool SetItemPositionAt(int index, int row, int column)
        => NativeMethods.wxsharp_gridbagsizer_set_item_position_at(Handle, index, row, column);

    /// <summary>How many rows and columns a window spans.</summary>
    public (int RowSpan, int ColumnSpan) GetItemSpan(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        NativeMethods.wxsharp_gridbagsizer_get_item_span_control(Handle, window.Handle, out var rows, out var columns);
        return (rows, columns);
    }

    public (int RowSpan, int ColumnSpan) GetItemSpanAt(int index)
    {
        NativeMethods.wxsharp_gridbagsizer_get_item_span_at(Handle, index, out var rows, out var columns);
        return (rows, columns);
    }

    /// <summary>Changes a window's span. As with <see cref="SetItemPosition"/>, check for an intersection
    /// first - wxWidgets asserts on an overlap.</summary>
    public bool SetItemSpan(Window window, int rowSpan, int columnSpan)
    {
        ArgumentNullException.ThrowIfNull(window);
        return NativeMethods.wxsharp_gridbagsizer_set_item_span_control(Handle, window.Handle, rowSpan, columnSpan);
    }

    public bool SetItemSpanAt(int index, int rowSpan, int columnSpan)
        => NativeMethods.wxsharp_gridbagsizer_set_item_span_at(Handle, index, rowSpan, columnSpan);

    /// <summary>The item holding a window, or null when the sizer does not hold it.</summary>
    public SizerItem? FindItem(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        return SizerItem.From(NativeMethods.wxsharp_gridbagsizer_find_item_control(Handle, window.Handle));
    }

    /// <summary>The item holding a nested sizer.</summary>
    public SizerItem? FindItem(Sizer child)
    {
        ArgumentNullException.ThrowIfNull(child);
        return SizerItem.From(NativeMethods.wxsharp_gridbagsizer_find_item_sizer(Handle, child.Handle));
    }

    /// <summary>The item occupying a cell, or null when it is empty.</summary>
    public SizerItem? FindItemAtPosition(int row, int column)
        => SizerItem.From(NativeMethods.wxsharp_gridbagsizer_find_item_at_position(Handle, row, column));

    /// <summary>The item under a point, in the sizer's coordinates.</summary>
    public SizerItem? FindItemAtPoint(Point point)
        => SizerItem.From(NativeMethods.wxsharp_gridbagsizer_find_item_at_point(Handle, point.X, point.Y));

    /// <summary>The size of one cell, after a layout.</summary>
    public Size GetCellSize(int row, int column)
    {
        NativeMethods.wxsharp_gridbagsizer_get_cell_size(Handle, row, column, out var w, out var h);
        return new Size(w, h);
    }

    /// <summary>The size given to cells that hold nothing.</summary>
    public Size EmptyCellSize
    {
        get
        {
            NativeMethods.wxsharp_gridbagsizer_get_empty_cell_size(Handle, out var w, out var h);
            return new Size(w, h);
        }
        set => NativeMethods.wxsharp_gridbagsizer_set_empty_cell_size(Handle, value.Width, value.Height);
    }

    /// <summary>Whether a cell range would overlap anything already placed - worth asking before moving an
    /// item, since <see cref="SetItemPosition"/> refuses rather than overlapping.</summary>
    public bool CheckForIntersection(int row, int column, int rowSpan = 1, int columnSpan = 1,
        SizerItem? exclude = null)
        => NativeMethods.wxsharp_gridbagsizer_check_for_intersection(Handle, row, column, rowSpan, columnSpan,
            exclude?.Handle ?? 0);

    private static void ValidateCell(int row, int column, int rowSpan, int columnSpan, int border)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowSpan);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columnSpan);
        ArgumentOutOfRangeException.ThrowIfNegative(border);
    }
}
