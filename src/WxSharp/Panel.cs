namespace WxSharp;

/// <summary>A layout sub-container with its own horizontal or vertical sizer; controls created against it
/// stack in that direction. Nest panels (a horizontal panel inside a vertical one, …) for richer layouts.</summary>
public sealed class Panel : Container
{
    public Panel(Container parent, bool horizontal = false)
    {
        var handle = NativeMethods.wxsharp_panel_create(parent.Panel, horizontal, Id);
        AttachContainer(handle, handle); // the panel is its own content area
        parent.Track(this);
    }
}
