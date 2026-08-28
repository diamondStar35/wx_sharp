using System;

namespace WxSharp;

public enum Orientation { Horizontal, Vertical }

/// <summary>How a sizer treats one item: whether it grows, how it is aligned in the space it is given, and
/// which of its edges get the border.</summary>
///
/// <remarks>
/// Alignment only applies across a sizer's direction, never along it - the item's position along the
/// direction is what the sizer is deciding. So a vertical <see cref="BoxSizer"/> takes
/// <see cref="SizerFlags.AlignLeft"/>, <see cref="SizerFlags.AlignRight"/> and
/// <see cref="SizerFlags.AlignCenterHorizontal"/>, and a horizontal one takes the top, bottom and
/// <see cref="SizerFlags.AlignCenterVertical"/> counterparts. wxWidgets asserts at run time when the wrong
/// axis is used, rather than quietly ignoring it.
/// </remarks>
[Flags]
public enum SizerFlags
{
    None = 0,

    /// <summary>Grow to fill the space across the sizer's direction.</summary>
    Expand = 1,

    /// <summary>Centre in both directions.</summary>
    AlignCenter = 2,

    BorderLeft = 4,
    BorderTop = 8,
    BorderRight = 16,
    BorderBottom = 32,

    AlignLeft = 64,
    AlignRight = 128,
    AlignTop = 256,
    AlignBottom = 512,

    /// <summary>Centre vertically - what a label beside a taller control usually wants.</summary>
    AlignCenterVertical = 1024,
    AlignCenterHorizontal = 2048,

    /// <summary>Keep the item's aspect ratio while it grows.</summary>
    Shaped = 4096,

    /// <summary>Never shrink the item below its initial best size.</summary>
    FixedMinSize = 8192,

    /// <summary>Keep the item's space reserved while it is hidden, so the layout does not jump.</summary>
    ReserveSpaceEvenIfHidden = 16384,

    /// <summary>A border on all four edges.</summary>
    All = BorderLeft | BorderTop | BorderRight | BorderBottom,
}

public abstract class Sizer
{
    private readonly App _owner;
    private readonly nint _handle;

    internal nint Handle
    {
        get
        {
            var current = App.RequireCurrent();
            ObjectDisposedException.ThrowIf(current != _owner, this);
            return _handle;
        }
    }

    /// <summary>Wraps a sizer wxWidgets built for us, such as a dialog's standard button row.</summary>
    internal static Sizer Attach(nint handle) => new AttachedSizer(handle);

    private sealed class AttachedSizer : Sizer
    {
        internal AttachedSizer(nint handle) : base(handle) { }
    }

    private protected Sizer(nint handle)
    {
        _owner = App.RequireCurrent();
        _handle = handle != 0 ? handle : throw new InvalidOperationException("wxWidgets failed to create the sizer.");
    }

    // ---- Adding ---------------------------------------------------------------------------------------

    /// <summary>Adds a window at the end.</summary>
    public SizerItem Add(Window window, int proportion = 0, SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(window); Validate(proportion, border);
        return new SizerItem(NativeMethods.wxsharp_sizer_add_control(Handle, window.Handle, proportion, (int)flags, border));
    }

    /// <summary>Adds a nested sizer at the end.</summary>
    public SizerItem Add(Sizer child, int proportion = 0, SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(child); Validate(proportion, border);
        return new SizerItem(NativeMethods.wxsharp_sizer_add_sizer(Handle, child.Handle, proportion, (int)flags, border));
    }

    /// <summary>Adds a fixed gap.</summary>
    public SizerItem AddSpacer(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(size);
        return new SizerItem(NativeMethods.wxsharp_sizer_add_spacer(Handle, size));
    }

    /// <summary>Adds a gap that grows to take spare space, pushing what follows it along.</summary>
    public SizerItem AddStretchSpacer(int proportion = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(proportion);
        return new SizerItem(NativeMethods.wxsharp_sizer_add_stretch_spacer(Handle, proportion));
    }

    // ---- Inserting and prepending ---------------------------------------------------------------------

    public SizerItem Insert(int index, Window window, int proportion = 0, SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(window); ValidateIndex(index); Validate(proportion, border);
        return new SizerItem(NativeMethods.wxsharp_sizer_insert_control(Handle, index, window.Handle, proportion, (int)flags, border));
    }

    public SizerItem Insert(int index, Sizer child, int proportion = 0, SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(child); ValidateIndex(index); Validate(proportion, border);
        return new SizerItem(NativeMethods.wxsharp_sizer_insert_sizer(Handle, index, child.Handle, proportion, (int)flags, border));
    }

    public SizerItem InsertSpacer(int index, int size)
    {
        ValidateIndex(index); ArgumentOutOfRangeException.ThrowIfNegative(size);
        return new SizerItem(NativeMethods.wxsharp_sizer_insert_spacer(Handle, index, size));
    }

    public SizerItem InsertStretchSpacer(int index, int proportion = 1)
    {
        ValidateIndex(index); ArgumentOutOfRangeException.ThrowIfNegativeOrZero(proportion);
        return new SizerItem(NativeMethods.wxsharp_sizer_insert_stretch_spacer(Handle, index, proportion));
    }

    public SizerItem Prepend(Window window, int proportion = 0, SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(window); Validate(proportion, border);
        return new SizerItem(NativeMethods.wxsharp_sizer_prepend_control(Handle, window.Handle, proportion, (int)flags, border));
    }

    public SizerItem Prepend(Sizer child, int proportion = 0, SizerFlags flags = SizerFlags.None, int border = 0)
    {
        ArgumentNullException.ThrowIfNull(child); Validate(proportion, border);
        return new SizerItem(NativeMethods.wxsharp_sizer_prepend_sizer(Handle, child.Handle, proportion, (int)flags, border));
    }

    public SizerItem PrependSpacer(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(size);
        return new SizerItem(NativeMethods.wxsharp_sizer_prepend_spacer(Handle, size));
    }

    public SizerItem PrependStretchSpacer(int proportion = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(proportion);
        return new SizerItem(NativeMethods.wxsharp_sizer_prepend_stretch_spacer(Handle, proportion));
    }

    // ---- Removing -------------------------------------------------------------------------------------

    /// <summary>Takes a window out of the layout without destroying it, so it can be added somewhere else.</summary>
    public bool Detach(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        return NativeMethods.wxsharp_sizer_detach_control(Handle, window.Handle);
    }

    /// <summary>Takes a nested sizer out without destroying it.</summary>
    public bool Detach(Sizer child)
    {
        ArgumentNullException.ThrowIfNull(child);
        return NativeMethods.wxsharp_sizer_detach_sizer(Handle, child.Handle);
    }

    /// <summary>Takes the item at <paramref name="index"/> out without destroying what it held.</summary>
    public bool DetachAt(int index) => NativeMethods.wxsharp_sizer_detach_at(Handle, index);

    /// <summary>Removes a nested sizer and destroys it. wxWidgets does not allow removing a window this
    /// way - use <see cref="Detach(Window)"/>, which is what the window's own lifetime expects.</summary>
    public bool Remove(Sizer child)
    {
        ArgumentNullException.ThrowIfNull(child);
        return NativeMethods.wxsharp_sizer_remove_sizer(Handle, child.Handle);
    }

    /// <summary>Removes the item at <paramref name="index"/>, destroying a nested sizer if that is what it
    /// held.</summary>
    public bool RemoveAt(int index) => NativeMethods.wxsharp_sizer_remove_at(Handle, index);

    /// <summary>Empties the sizer. Windows are detached and left alive unless
    /// <paramref name="deleteWindows"/> says otherwise.</summary>
    public void Clear(bool deleteWindows = false) => NativeMethods.wxsharp_sizer_clear(Handle, deleteWindows);

    /// <summary>Destroys every window the sizer holds.</summary>
    public void DeleteWindows() => NativeMethods.wxsharp_sizer_delete_windows(Handle);

    /// <summary>Swaps one window for another, keeping the position, proportion, flags and border.</summary>
    public bool Replace(Window oldWindow, Window newWindow, bool recursive = false)
    {
        ArgumentNullException.ThrowIfNull(oldWindow); ArgumentNullException.ThrowIfNull(newWindow);
        return NativeMethods.wxsharp_sizer_replace_control(Handle, oldWindow.Handle, newWindow.Handle, recursive);
    }

    /// <summary>Swaps one nested sizer for another.</summary>
    public bool Replace(Sizer oldSizer, Sizer newSizer, bool recursive = false)
    {
        ArgumentNullException.ThrowIfNull(oldSizer); ArgumentNullException.ThrowIfNull(newSizer);
        return NativeMethods.wxsharp_sizer_replace_sizer(Handle, oldSizer.Handle, newSizer.Handle, recursive);
    }

    // ---- Finding items --------------------------------------------------------------------------------

    /// <summary>How many items the sizer holds, spacers included.</summary>
    public int ItemCount => NativeMethods.wxsharp_sizer_item_count(Handle);

    public bool IsEmpty => NativeMethods.wxsharp_sizer_is_empty(Handle);

    /// <summary>The item at <paramref name="index"/>, or null when there is none.</summary>
    public SizerItem? GetItem(int index) => SizerItem.From(NativeMethods.wxsharp_sizer_item_at(Handle, index));

    /// <summary>The item holding <paramref name="window"/>, searching nested sizers when asked.</summary>
    public SizerItem? GetItem(Window window, bool recursive = false)
    {
        ArgumentNullException.ThrowIfNull(window);
        return SizerItem.From(NativeMethods.wxsharp_sizer_item_for_control(Handle, window.Handle, recursive));
    }

    /// <summary>The item holding <paramref name="child"/>.</summary>
    public SizerItem? GetItem(Sizer child, bool recursive = false)
    {
        ArgumentNullException.ThrowIfNull(child);
        return SizerItem.From(NativeMethods.wxsharp_sizer_item_for_sizer(Handle, child.Handle, recursive));
    }

    /// <summary>The item whose <see cref="SizerItem.Id"/> is <paramref name="id"/>. That is the item's own
    /// identifier, not a window ID - <see cref="GetItem(Window, bool)"/> is what finds an item by window.</summary>
    public SizerItem? GetItemById(int id, bool recursive = false)
        => SizerItem.From(NativeMethods.wxsharp_sizer_item_by_id(Handle, id, recursive));

    // ---- Visibility -----------------------------------------------------------------------------------

    /// <summary>Shows or hides a window and takes it out of the layout, so what is left closes the gap.</summary>
    public bool Show(Window window, bool show = true, bool recursive = false)
    {
        ArgumentNullException.ThrowIfNull(window);
        return NativeMethods.wxsharp_sizer_show_control(Handle, window.Handle, show, recursive);
    }

    public bool Show(Sizer child, bool show = true, bool recursive = false)
    {
        ArgumentNullException.ThrowIfNull(child);
        return NativeMethods.wxsharp_sizer_show_sizer(Handle, child.Handle, show, recursive);
    }

    public bool ShowAt(int index, bool show = true) => NativeMethods.wxsharp_sizer_show_at(Handle, index, show);

    public bool Hide(Window window, bool recursive = false) => Show(window, false, recursive);
    public bool Hide(Sizer child, bool recursive = false) => Show(child, false, recursive);
    public bool HideAt(int index) => ShowAt(index, false);

    /// <summary>Shows or hides everything in the sizer at once.</summary>
    public void ShowItems(bool show = true) => NativeMethods.wxsharp_sizer_show_items(Handle, show);

    /// <summary>Whether anything in the sizer is still visible.</summary>
    public bool AreAnyItemsShown() => NativeMethods.wxsharp_sizer_any_items_shown(Handle);

    public bool IsShown(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        return NativeMethods.wxsharp_sizer_is_shown_control(Handle, window.Handle);
    }

    public bool IsShown(Sizer child)
    {
        ArgumentNullException.ThrowIfNull(child);
        return NativeMethods.wxsharp_sizer_is_shown_sizer(Handle, child.Handle);
    }

    public bool IsShownAt(int index) => NativeMethods.wxsharp_sizer_is_shown_at(Handle, index);

    // ---- Layout and measurement -----------------------------------------------------------------------

    /// <summary>Recalculates the layout. Call it after changing what the sizer holds.</summary>
    public void Layout() => NativeMethods.wxsharp_sizer_layout(Handle);

    /// <summary>Resizes <paramref name="window"/> to the sizer's minimum size and returns that size.</summary>
    public Size Fit(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        NativeMethods.wxsharp_sizer_fit(Handle, window.Handle, out var w, out var h);
        return new Size(w, h);
    }

    /// <summary>Resizes the window's virtual area rather than the window itself - for a scrolled window.</summary>
    public void FitInside(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        NativeMethods.wxsharp_sizer_fit_inside(Handle, window.Handle);
    }

    /// <summary>Makes the sizer's minimum size the window's minimum size, so the user cannot shrink it
    /// below what the layout needs.</summary>
    public void SetSizeHints(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        NativeMethods.wxsharp_sizer_set_size_hints(Handle, window.Handle);
    }

    /// <summary>The client size the window would need to fit this sizer, without resizing anything.</summary>
    public Size ComputeFittingClientSize(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        NativeMethods.wxsharp_sizer_compute_fitting_client_size(Handle, window.Handle, out var w, out var h);
        return new Size(w, h);
    }

    /// <summary>The whole window size needed to fit this sizer, including decoration.</summary>
    public Size ComputeFittingWindowSize(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        NativeMethods.wxsharp_sizer_compute_fitting_window_size(Handle, window.Handle, out var w, out var h);
        return new Size(w, h);
    }

    /// <summary>The smallest size that fits everything the sizer holds.</summary>
    public Size MinSize
    {
        get { NativeMethods.wxsharp_sizer_get_min_size(Handle, out var w, out var h); return new Size(w, h); }
        set => NativeMethods.wxsharp_sizer_set_min_size(Handle, value.Width, value.Height);
    }

    /// <summary>The size the sizer currently occupies.</summary>
    public Size Size
    {
        get { NativeMethods.wxsharp_sizer_get_size(Handle, out var w, out var h); return new Size(w, h); }
    }

    /// <summary>Where the sizer sits in its window.</summary>
    public Point Position
    {
        get { NativeMethods.wxsharp_sizer_get_position(Handle, out var x, out var y); return new Point(x, y); }
    }

    /// <summary>Places and sizes the sizer explicitly, instead of letting a window do it.</summary>
    public void SetDimension(int x, int y, int width, int height)
        => NativeMethods.wxsharp_sizer_set_dimension(Handle, x, y, width, height);

    /// <summary>Sets the minimum size of one item.</summary>
    public bool SetItemMinSize(Window window, Size size)
    {
        ArgumentNullException.ThrowIfNull(window);
        return NativeMethods.wxsharp_sizer_set_item_min_size_control(Handle, window.Handle, size.Width, size.Height);
    }

    public bool SetItemMinSize(Sizer child, Size size)
    {
        ArgumentNullException.ThrowIfNull(child);
        return NativeMethods.wxsharp_sizer_set_item_min_size_sizer(Handle, child.Handle, size.Width, size.Height);
    }

    public bool SetItemMinSizeAt(int index, Size size)
        => NativeMethods.wxsharp_sizer_set_item_min_size_at(Handle, index, size.Width, size.Height);

    /// <summary>Whether this sizer, or one it is nested in, has been given to a window yet.</summary>
    public bool HasContainingWindow => NativeMethods.wxsharp_sizer_containing_window(Handle) != 0;
    public Window? ContainingWindow => App.Lookup(NativeMethods.wxsharp_sizer_containing_window(Handle));
    public Window? GetContainingWindow() => ContainingWindow;

    private static void Validate(int proportion, int border)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(proportion);
        ArgumentOutOfRangeException.ThrowIfNegative(border);
    }

    private static void ValidateIndex(int index) => ArgumentOutOfRangeException.ThrowIfNegative(index);
}
