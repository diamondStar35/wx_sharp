using System;

namespace WxSharp;

/// <summary>A generic custom-drawn surface. It raises <see cref="Paint"/> when it needs repainting; draw from
/// that handler with the <c>Draw*</c>/<c>Set*</c> methods (they only take effect during a paint). The canvas
/// refuses keyboard focus and is skipped by assistive technology, so it is a purely visual layer - it never
/// affects tab order or speech. Call <see cref="Control.Refresh"/> to request a repaint after state changes;
/// use the mouse events plus <see cref="Control.MousePosition"/> for hover and click hit-testing.
///
/// Modelled on the wxWidgets/Phoenix custom-paint examples (an <c>OnPaint</c> handler drawing with a device
/// context), but with the drawing driven from managed code.</summary>
public class Canvas : Control
{
    /// <summary>Raised when the canvas must repaint. Issue draw calls from the handler.</summary>
    public event EventHandler<PaintEventArgs> Paint
    {
        add => AddHandler(WxEvents.Paint, value);
        remove => RemoveHandler(WxEvents.Paint, value);
    }

    /// <summary>Creates a canvas at an optional position and size. Add it to a sizer for managed layout.</summary>
    public Canvas(Window parent, int id = WindowId.Any, Point? position = null, Size? size = null) : base(parent, id)
    {
        var initialSize = size ?? new Size(100, 100);
        Initialize(NativeMethods.wxsharp_canvas_create(parent.Handle, id, initialSize.Width, initialSize.Height, Token));
        ApplyInitialGeometry(position, null);
    }

    // ---- Draw state (valid during a Paint handler) -------------------------------------------------------

    /// <summary>Clears the whole surface to <paramref name="color"/>.</summary>
    public void Clear(Colour color) => NativeMethods.wxsharp_canvas_clear(Handle, color.ToArgb());

    /// <summary>Sets the fill colour for subsequent shapes. A colour with alpha 0 fills nothing (no fill).</summary>
    public void SetBrush(Colour color) => NativeMethods.wxsharp_canvas_set_brush(Handle, color.ToArgb());
    /// <summary>Sets the fill for subsequent shapes from a <see cref="Brush"/>.</summary>
    public void SetBrush(Brush brush) => SetBrush(brush.Colour);

    /// <summary>Sets the outline colour and width for subsequent shapes and lines. A colour with alpha 0 draws
    /// no outline.</summary>
    public void SetPen(Colour color, int width = 1) => NativeMethods.wxsharp_canvas_set_pen(Handle, color.ToArgb(), width);
    /// <summary>Sets the outline for subsequent shapes and lines from a <see cref="Pen"/>.</summary>
    public void SetPen(Pen pen) => SetPen(pen.Colour, pen.Width);

    /// <summary>Sets the colour for subsequent <see cref="DrawText"/> calls.</summary>
    public void SetTextColour(Colour color) => NativeMethods.wxsharp_canvas_set_text_colour(Handle, color.ToArgb());

    /// <summary>Sets the font for subsequent <see cref="DrawText"/> calls during this paint.
    /// <see cref="MeasureText"/> measures in this same font, so laying out and drawing agree.</summary>
    public void SetTextFont(Font font)
    {
        ArgumentNullException.ThrowIfNull(font);
        NativeMethods.wxsharp_canvas_set_font(Handle, font.Handle);
    }

    // ---- Primitives --------------------------------------------------------------------------------------

    /// <summary>Draws a rectangle at <paramref name="x"/>,<paramref name="y"/> with the current pen and
    /// brush.</summary>
    public void DrawRectangle(int x, int y, int width, int height)
        => NativeMethods.wxsharp_canvas_draw_rectangle(Handle, x, y, width, height);

    /// <summary>Draws a rectangle with rounded corners of the given <paramref name="radius"/>.</summary>
    public void DrawRoundedRectangle(int x, int y, int width, int height, int radius)
        => NativeMethods.wxsharp_canvas_draw_rounded_rectangle(Handle, x, y, width, height, radius);

    /// <summary>Draws a straight line between two points with the current pen.</summary>
    public void DrawLine(int x1, int y1, int x2, int y2)
        => NativeMethods.wxsharp_canvas_draw_line(Handle, x1, y1, x2, y2);

    /// <summary>Draws a circle of <paramref name="radius"/> centred at <paramref name="x"/>,<paramref name="y"/>.</summary>
    public void DrawCircle(int x, int y, int radius)
        => NativeMethods.wxsharp_canvas_draw_circle(Handle, x, y, radius);

    /// <summary>Draws an ellipse filling the given bounding box.</summary>
    public void DrawEllipse(int x, int y, int width, int height)
        => NativeMethods.wxsharp_canvas_draw_ellipse(Handle, x, y, width, height);

    /// <summary>Draws <paramref name="text"/> with its top-left at <paramref name="x"/>,<paramref name="y"/>,
    /// in the current text font and colour.</summary>
    public void DrawText(string text, int x, int y)
        => NativeMethods.wxsharp_canvas_draw_text(Handle, text, x, y);

    /// <summary>Measures <paramref name="text"/> in whatever font will actually draw it: the one set by
    /// <see cref="SetTextFont"/> during a paint, and the control's otherwise - so this works outside a
    /// paint too, and callers can lay out before the first one.</summary>
    public Size MeasureText(string text)
    {
        NativeMethods.wxsharp_canvas_measure_text(Handle, text, out var w, out var h);
        return new Size(w, h);
    }
}
