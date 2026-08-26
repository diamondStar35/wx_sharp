using System;

namespace WxSharp;

public class StaticBitmap : Control
{
    public StaticBitmap(Window parent, Bitmap bitmap, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_staticbitmap_create(parent.Handle, id, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)), Token));
    public void SetBitmap(Bitmap bitmap) => NativeMethods.wxsharp_staticbitmap_set(Handle, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)));
}

public class BitmapButton : Control
{
    public event EventHandler<CommandEventArgs>? Click;
    public BitmapButton(Window parent, Bitmap bitmap, int id = WindowId.Any) : base(parent, id)
        => Initialize(NativeMethods.wxsharp_bitmapbutton_create(parent.Handle, id, bitmap?.Handle ?? throw new ArgumentNullException(nameof(bitmap)), Token));
    internal override uint Dispatch(in NativeEvent e) => e.Kind == EventKind.Click
        ? RaiseCommand(new CommandEventArgs(this, e.Id), Click) : base.Dispatch(e);
}
