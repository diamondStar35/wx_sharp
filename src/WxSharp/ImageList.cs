using System;

namespace WxSharp;

/// <summary>Which of a list control's image lists is being set, following the <c>wxIMAGE_LIST_</c>
/// values.</summary>
public enum ImageListKind
{
    /// <summary>The large images icon view uses.</summary>
    Normal = 0,
    /// <summary>The small images report and list views use.</summary>
    Small = 1,
    /// <summary>State images, drawn beside the item's own.</summary>
    State = 2,
}

/// <summary>Which of a tree item's images is being set, following <c>wxTreeItemIcon</c>.</summary>
public enum TreeItemIcon
{
    Normal = 0,
    Selected = 1,
    Expanded = 2,
    SelectedExpanded = 3,
}

/// <summary>The images a list, tree, notebook or toolbar draws beside its items, following
/// <c>wxImageList</c>.</summary>
///
/// <remarks>
/// wxWidgets addresses these by index into a list the control holds rather than giving each item its own
/// bitmap, which is why this type exists at all. Every image in a list is the same size, fixed when the list
/// is created; anything larger is cropped and anything smaller is padded.
///
/// A control can either borrow the list or take ownership of it - see <see cref="ListCtrl.SetImageList"/>.
/// Borrowing means you must keep it alive for as long as the control uses it.
/// </remarks>
public sealed class ImageList : IDisposable
{
    private nint _handle;

    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(ImageList));

    /// <summary>Creates a list of images of a fixed size.</summary>
    /// <param name="mask">Whether to keep each image's transparency mask.</param>
    /// <param name="initialCount">A hint at how many images will be added; not a limit.</param>
    public ImageList(int width, int height, bool mask = true, int initialCount = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        _ = App.RequireCurrent();
        _handle = NativeMethods.wxsharp_imagelist_create(width, height, mask, initialCount);
        if (_handle == 0) throw new InvalidOperationException("wxWidgets failed to create the image list.");
    }

    /// <summary>How many images the list holds.</summary>
    public int Count => NativeMethods.wxsharp_imagelist_count(Handle);

    /// <summary>Adds a bitmap and returns its index, which is what the control refers to it by.</summary>
    public int Add(Bitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        return NativeMethods.wxsharp_imagelist_add_bitmap(Handle, bitmap.Handle);
    }

    /// <summary>Adds an icon and returns its index.</summary>
    public int Add(Icon icon)
    {
        ArgumentNullException.ThrowIfNull(icon);
        return NativeMethods.wxsharp_imagelist_add_icon(Handle, icon.Handle);
    }

    /// <summary>Replaces one image, keeping its index so nothing referring to it has to change.</summary>
    public bool Replace(int index, Bitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        return NativeMethods.wxsharp_imagelist_replace(Handle, index, bitmap.Handle);
    }

    /// <summary>Removes one image. Every later index shifts down, so anything holding one has to be
    /// updated - which is why replacing is usually the better move.</summary>
    public bool RemoveAt(int index) => NativeMethods.wxsharp_imagelist_remove(Handle, index);

    /// <summary>Removes every image.</summary>
    public bool Clear() => NativeMethods.wxsharp_imagelist_remove_all(Handle);

    /// <summary>The size of an image, which is the list's own size for every valid index.</summary>
    public Size GetSize(int index)
    {
        NativeMethods.wxsharp_imagelist_size(Handle, index, out var w, out var h);
        return new Size(w, h);
    }

    /// <summary>A copy of one image, or null when the index is not in the list.</summary>
    public Bitmap? GetBitmap(int index)
    {
        var handle = NativeMethods.wxsharp_imagelist_get_bitmap(Handle, index);
        return handle == 0 ? null : Bitmap.Attach(handle);
    }

    public void Dispose()
    {
        if (_handle != 0) NativeMethods.wxsharp_imagelist_destroy(_handle);
        _handle = 0;
    }

    /// <summary>Releases the handle without destroying the list, for when a control has taken ownership of
    /// it.</summary>
    internal void Detach() => _handle = 0;
}
