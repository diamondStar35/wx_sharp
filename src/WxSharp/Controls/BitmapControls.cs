using System;

namespace WxSharp;

/// <summary>How a <see cref="StaticBitmap"/> fits its image into the control, following
/// <c>wxStaticBitmap</c>'s scale modes.</summary>
public enum StaticBitmapScaleMode
{
    /// <summary>Draw the image at its natural size, cropped if larger than the control.</summary>
    None = 0,
    /// <summary>Stretch the image to fill the control, ignoring aspect ratio.</summary>
    Fill = 1,
    /// <summary>Scale to fit inside the control, keeping aspect ratio (letterboxed).</summary>
    AspectFit = 2,
    /// <summary>Scale to cover the control, keeping aspect ratio (cropped).</summary>
    AspectFill = 3,
}

/// <summary>A control that displays a bitmap or icon, following <c>wxStaticBitmap</c>.</summary>
public class StaticBitmap : Control
{
    /// <summary>Creates a <c>wxStaticBitmap</c> child of <paramref name="parent"/> showing
    /// <paramref name="bitmap"/>.</summary>
    public StaticBitmap(Window parent, Bitmap bitmap, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(StaticBitmap)
            ? NativeMethods.wxsharp_staticbitmap_create(parent.Handle, id, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)), Token)
            : NativeMethods.wxsharp_custom_staticbitmap_create(parent.Handle, id, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)), Token));
    /// <summary>Replaces the displayed image.</summary>
    public void SetBitmap(Bitmap bitmap) => NativeMethods.wxsharp_staticbitmap_set(Handle, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)));
    /// <summary>The displayed image. The caller owns the returned bitmap.</summary>
    public Bitmap GetBitmap() => Bitmap.Attach(NativeMethods.wxsharp_staticbitmap_get(Handle));
    /// <summary>Displays an icon instead of a bitmap.</summary>
    public void SetIcon(Icon icon) => NativeMethods.wxsharp_staticbitmap_set_icon(Handle, icon?.Handle ?? throw new ArgumentNullException(nameof(icon)));
    /// <summary>The displayed image as an icon. The caller owns the returned icon.</summary>
    public Icon GetIcon() => Icon.Attach(NativeMethods.wxsharp_staticbitmap_get_icon(Handle));
    /// <summary>How the image is scaled to fit the control.</summary>
    public StaticBitmapScaleMode ScaleMode
    {
        get => (StaticBitmapScaleMode)NativeMethods.wxsharp_staticbitmap_get_scale_mode(Handle);
        set => NativeMethods.wxsharp_staticbitmap_set_scale_mode(Handle, (int)value);
    }
    /// <summary>Returns the scale mode.</summary>
    public StaticBitmapScaleMode GetScaleMode() => ScaleMode;
    /// <summary>Sets the scale mode.</summary>
    public void SetScaleMode(StaticBitmapScaleMode mode) => ScaleMode = mode;
}

/// <summary>A button that shows a bitmap instead of a text label, following <c>wxBitmapButton</c>.</summary>
public class BitmapButton : Control
{
    /// <summary>Raised when the button is clicked (<c>wxEVT_BUTTON</c>).</summary>
    public event EventHandler<CommandEventArgs> Click
    {
        add => AddHandler(WxEvents.ButtonClicked, value);
        remove => RemoveHandler(WxEvents.ButtonClicked, value);
    }
    /// <summary>Creates a <c>wxBitmapButton</c> child of <paramref name="parent"/> showing
    /// <paramref name="bitmap"/>.</summary>
    public BitmapButton(Window parent, Bitmap bitmap, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(BitmapButton)
            ? NativeMethods.wxsharp_bitmapbutton_create(parent.Handle, id, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)), Token)
            : NativeMethods.wxsharp_custom_bitmapbutton_create(parent.Handle, id, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)), Token));
    private BitmapButton(Window parent, int id, string name) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_bitmapbutton_new_close(parent.Handle, id, name, Token));
    /// <summary>Creates the platform's standard close button, following <c>wxBitmapButton.NewCloseButton</c>.</summary>
    public static BitmapButton NewCloseButton(Window parent, int id = WindowId.Any, string name = "")
    {
        ArgumentNullException.ThrowIfNull(parent); ArgumentNullException.ThrowIfNull(name);
        return new BitmapButton(parent, id, name);
    }
    /// <summary>Sets the margins around the bitmap, in pixels.</summary>
    public void SetMargins(int x, int y) => NativeMethods.wxsharp_bitmapbutton_set_margins(Handle, x, y);
    /// <summary>Replaces the bitmap the button shows, following <c>wxBitmapButton.SetBitmapLabel</c>.</summary>
    public void SetBitmap(Bitmap bitmap) => NativeMethods.wxsharp_bitmapbutton_set_bitmap(Handle, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)));
    /// <summary>The horizontal margin around the bitmap.</summary>
    public int GetMarginX() => NativeMethods.wxsharp_bitmapbutton_get_margin_x(Handle);
    /// <summary>The vertical margin around the bitmap.</summary>
    public int GetMarginY() => NativeMethods.wxsharp_bitmapbutton_get_margin_y(Handle);
}
