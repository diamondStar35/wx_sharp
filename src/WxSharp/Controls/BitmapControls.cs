using System;

namespace WxSharp;

public enum StaticBitmapScaleMode
{
    None = 0,
    Fill = 1,
    AspectFit = 2,
    AspectFill = 3,
}

public class StaticBitmap : Control
{
    public StaticBitmap(Window parent, Bitmap bitmap, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(StaticBitmap)
            ? NativeMethods.wxsharp_staticbitmap_create(parent.Handle, id, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)), Token)
            : NativeMethods.wxsharp_custom_staticbitmap_create(parent.Handle, id, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)), Token));
    public void SetBitmap(Bitmap bitmap) => NativeMethods.wxsharp_staticbitmap_set(Handle, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)));
    public Bitmap GetBitmap() => Bitmap.Attach(NativeMethods.wxsharp_staticbitmap_get(Handle));
    public void SetIcon(Icon icon) => NativeMethods.wxsharp_staticbitmap_set_icon(Handle, icon?.Handle ?? throw new ArgumentNullException(nameof(icon)));
    public Icon GetIcon() => Icon.Attach(NativeMethods.wxsharp_staticbitmap_get_icon(Handle));
    public StaticBitmapScaleMode ScaleMode
    {
        get => (StaticBitmapScaleMode)NativeMethods.wxsharp_staticbitmap_get_scale_mode(Handle);
        set => NativeMethods.wxsharp_staticbitmap_set_scale_mode(Handle, (int)value);
    }
    public StaticBitmapScaleMode GetScaleMode() => ScaleMode;
    public void SetScaleMode(StaticBitmapScaleMode mode) => ScaleMode = mode;
}

public class BitmapButton : Control
{
    public event EventHandler<CommandEventArgs> Click
    {
        add => AddHandler(WxEvents.ButtonClicked, value);
        remove => RemoveHandler(WxEvents.ButtonClicked, value);
    }
    public BitmapButton(Window parent, Bitmap bitmap, int id = WindowId.Any) : base(parent, id)
        => Initialize(GetType() == typeof(BitmapButton)
            ? NativeMethods.wxsharp_bitmapbutton_create(parent.Handle, id, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)), Token)
            : NativeMethods.wxsharp_custom_bitmapbutton_create(parent.Handle, id, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)), Token));
    private BitmapButton(Window parent, int id, string name) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_bitmapbutton_new_close(parent.Handle, id, name, Token));
    public static BitmapButton NewCloseButton(Window parent, int id = WindowId.Any, string name = "")
    {
        ArgumentNullException.ThrowIfNull(parent); ArgumentNullException.ThrowIfNull(name);
        return new BitmapButton(parent, id, name);
    }
    public void SetMargins(int x, int y) => NativeMethods.wxsharp_bitmapbutton_set_margins(Handle, x, y);
    public int GetMarginX() => NativeMethods.wxsharp_bitmapbutton_get_margin_x(Handle);
    public int GetMarginY() => NativeMethods.wxsharp_bitmapbutton_get_margin_y(Handle);
}
