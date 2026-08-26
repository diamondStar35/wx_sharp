// A generic drawing canvas. A non-focusable wxWindow that, when it needs painting, reports a Paint event to
// the managed side; during that callback the managed code issues draw calls that render onto a buffered device
// context held for the duration of the paint. This mirrors the wxWidgets/Phoenix custom-paint examples (an
// OnPaint handler drawing with a wxDC), but with the drawing driven from managed code. It refuses focus so
// assistive technology skips it - a purely visual layer that never affects keyboard navigation or speech.
#include "internal.h"
#include <wx/dcbuffer.h>

namespace
{
    class WxSharpCanvas : public wxWindow
    {
    public:
        WxSharpCanvas(wxWindow* parent, int id, const wxSize& size, long long token)
            : wxWindow(parent, id, wxDefaultPosition, size, wxFULL_REPAINT_ON_RESIZE),
              m_token(token), m_dc(nullptr)
        {
            SetBackgroundStyle(wxBG_STYLE_PAINT); // required for wxAutoBufferedPaintDC
            Bind(wxEVT_PAINT, &WxSharpCanvas::OnPaint, this);
            Bind(wxEVT_SIZE, [this](wxSizeEvent& e) { Refresh(); e.Skip(); });
        }

        // Not focusable and skipped by keyboard traversal, so it stays out of the reader's and tab order's way.
        bool AcceptsFocus() const override { return false; }
        bool AcceptsFocusFromKeyboard() const override { return false; }

        // The device context valid only during a paint; null at any other time (draw calls then no-op).
        wxDC* Dc() const { return m_dc; }

    private:
        void OnPaint(wxPaintEvent&)
        {
            wxAutoBufferedPaintDC dc(this);
            // Sensible defaults so text uses the control's font/colour unless the managed side overrides them.
            dc.SetFont(GetFont());
            dc.SetTextForeground(GetForegroundColour());
            dc.SetBackgroundMode(wxTRANSPARENT);
            m_dc = &dc;
            Fire(m_token, WXSHARP_EV_PAINT, GetId());
            m_dc = nullptr;
        }

        long long m_token;
        wxDC* m_dc;
    };

    inline wxDC* Dc(wxsharp_handle h) { return static_cast<WxSharpCanvas*>(h)->Dc(); }
}

wxsharp_handle wxsharp_canvas_create(wxsharp_handle parent, int id, int width, int height, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* canvas = new WxSharpCanvas(p, id, wxSize(width, height), token);
    TrackWindow(canvas, token);
    return canvas;
}

// Draw state and primitives. Each no-ops unless called during a paint (a colour with alpha 0 selects the
// transparent pen/brush, so you can fill without an outline or stroke without a fill).
void wxsharp_canvas_clear(wxsharp_handle h, unsigned int argb)
{
    if (auto* dc = Dc(h)) { dc->SetBackground(wxBrush(ColourFromArgb(argb))); dc->Clear(); }
}

void wxsharp_canvas_set_brush(wxsharp_handle h, unsigned int argb)
{
    if (auto* dc = Dc(h)) dc->SetBrush((argb >> 24) == 0 ? *wxTRANSPARENT_BRUSH : wxBrush(ColourFromArgb(argb)));
}

void wxsharp_canvas_set_pen(wxsharp_handle h, unsigned int argb, int width)
{
    if (auto* dc = Dc(h)) dc->SetPen((argb >> 24) == 0 ? *wxTRANSPARENT_PEN : wxPen(ColourFromArgb(argb), width));
}

void wxsharp_canvas_set_text_colour(wxsharp_handle h, unsigned int argb)
{
    if (auto* dc = Dc(h)) dc->SetTextForeground(ColourFromArgb(argb));
}

void wxsharp_canvas_set_font(wxsharp_handle h, int point_size, int family, int weight, int style,
                             bool underline, const char* face)
{
    if (auto* dc = Dc(h)) dc->SetFont(MakeFont(point_size, family, weight, style, underline, face));
}

void wxsharp_canvas_draw_rectangle(wxsharp_handle h, int x, int y, int width, int height)
{
    if (auto* dc = Dc(h)) dc->DrawRectangle(x, y, width, height);
}

void wxsharp_canvas_draw_rounded_rectangle(wxsharp_handle h, int x, int y, int width, int height, int radius)
{
    if (auto* dc = Dc(h)) dc->DrawRoundedRectangle(x, y, width, height, radius);
}

void wxsharp_canvas_draw_line(wxsharp_handle h, int x1, int y1, int x2, int y2)
{
    if (auto* dc = Dc(h)) dc->DrawLine(x1, y1, x2, y2);
}

void wxsharp_canvas_draw_circle(wxsharp_handle h, int x, int y, int radius)
{
    if (auto* dc = Dc(h)) dc->DrawCircle(x, y, radius);
}

void wxsharp_canvas_draw_ellipse(wxsharp_handle h, int x, int y, int width, int height)
{
    if (auto* dc = Dc(h)) dc->DrawEllipse(x, y, width, height);
}

void wxsharp_canvas_draw_text(wxsharp_handle h, const char* text, int x, int y)
{
    if (auto* dc = Dc(h)) dc->DrawText(Str(text), x, y);
}

// Measures text in the canvas's current (control) font - works at any time, so callers can lay out before a
// paint. Set the control font via the generic control-font setter to keep measuring and drawing consistent.
void wxsharp_canvas_measure_text(wxsharp_handle h, const char* text, int* width, int* height)
{
    const wxSize s = static_cast<wxWindow*>(h)->GetTextExtent(Str(text));
    if (width) *width = s.x;
    if (height) *height = s.y;
}
