using System;

namespace WxSharp;

public class GridSizer : Sizer
{
    public GridSizer(int rows, int columns, int verticalGap = 0, int horizontalGap = 0)
        : base(Create(rows, columns, verticalGap, horizontalGap)) { }
    private static nint Create(int rows, int columns, int verticalGap, int horizontalGap)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rows); ArgumentOutOfRangeException.ThrowIfNegative(columns);
        if (rows == 0 && columns == 0) throw new ArgumentException("Rows and columns cannot both be zero.");
        ArgumentOutOfRangeException.ThrowIfNegative(verticalGap); ArgumentOutOfRangeException.ThrowIfNegative(horizontalGap);
        return NativeMethods.wxsharp_gridsizer_create(rows, columns, verticalGap, horizontalGap);
    }
}

public class FlexGridSizer : Sizer
{
    public FlexGridSizer(int rows, int columns, int verticalGap = 0, int horizontalGap = 0)
        : base(Create(rows, columns, verticalGap, horizontalGap)) { }
    public void AddGrowableRow(int row, int proportion = 1) => NativeMethods.wxsharp_flexgridsizer_add_growable_row(Handle, row, proportion);
    public void AddGrowableColumn(int column, int proportion = 1) => NativeMethods.wxsharp_flexgridsizer_add_growable_column(Handle, column, proportion);
    private static nint Create(int rows, int columns, int verticalGap, int horizontalGap)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rows); ArgumentOutOfRangeException.ThrowIfNegative(columns);
        if (rows == 0 && columns == 0) throw new ArgumentException("Rows and columns cannot both be zero.");
        return NativeMethods.wxsharp_flexgridsizer_create(rows, columns, verticalGap, horizontalGap);
    }
}

public class StaticBoxSizer : Sizer
{
    public StaticBoxSizer(StaticBox box, Orientation orientation = Orientation.Vertical)
        : base(NativeMethods.wxsharp_staticboxsizer_create(box?.Handle ?? throw new ArgumentNullException(nameof(box)),
            orientation == Orientation.Horizontal))
    { }
}

public class GridBagSizer : Sizer
{
    public GridBagSizer(int verticalGap = 0, int horizontalGap = 0)
        : base(NativeMethods.wxsharp_gridbagsizer_create(verticalGap, horizontalGap)) { }
    public void AddAt(Window window, int row, int column, int rowSpan = 1, int columnSpan = 1,
        SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(window); ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfNegative(column); ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowSpan);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columnSpan); ArgumentOutOfRangeException.ThrowIfNegative(border);
        NativeMethods.wxsharp_gridbagsizer_add_control(Handle, window.Handle, row, column, rowSpan, columnSpan,
            (int)flags, border);
    }
}
