// Single- or multi-line text field. Single-line boxes process Enter so it raises a TextEnter event.
#include "internal.h"

wxsharp_handle wxsharp_textbox_create(wxsharp_handle parent, int id, const char* value, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    // The style is passed through exactly as given. wxWidgets does not add wxTE_PROCESS_ENTER by itself,
    // and adding it would stop Enter reaching a dialog's default button; ask for TextCtrlStyle.ProcessEnter
    // when the control should handle Enter instead.
    auto* ctrl = new wxTextCtrl(p, id, Str(value), wxDefaultPosition, wxDefaultSize,
                                MapTextBoxStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

int wxsharp_textbox_get_value(wxsharp_handle ctrl, char* buffer, int buffer_length)
{
    return CopyToBuffer(static_cast<wxTextCtrl*>(ctrl)->GetValue(), buffer, buffer_length);
}

void wxsharp_textbox_set_value(wxsharp_handle ctrl, const char* value) { static_cast<wxTextCtrl*>(ctrl)->SetValue(Str(value)); }
void wxsharp_textbox_append(wxsharp_handle ctrl, const char* text) { static_cast<wxTextCtrl*>(ctrl)->AppendText(Str(text)); }
void wxsharp_textbox_clear(wxsharp_handle ctrl) { static_cast<wxTextCtrl*>(ctrl)->Clear(); }
void wxsharp_textbox_select_all(wxsharp_handle ctrl) { static_cast<wxTextCtrl*>(ctrl)->SelectAll(); }
void wxsharp_textbox_set_editable(wxsharp_handle ctrl, bool editable) { static_cast<wxTextCtrl*>(ctrl)->SetEditable(editable); }

// Writes text at the insertion point (replacing any selection) and moves the caret past it.
void wxsharp_textbox_write(wxsharp_handle ctrl, const char* text) { static_cast<wxTextCtrl*>(ctrl)->WriteText(Str(text)); }

// The number of characters (the position just past the last one).
int wxsharp_textbox_length(wxsharp_handle ctrl) { return static_cast<wxTextCtrl*>(ctrl)->GetLastPosition(); }

int wxsharp_textbox_get_insertion_point(wxsharp_handle ctrl) { return static_cast<wxTextCtrl*>(ctrl)->GetInsertionPoint(); }
void wxsharp_textbox_set_insertion_point(wxsharp_handle ctrl, int pos) { static_cast<wxTextCtrl*>(ctrl)->SetInsertionPoint(pos); }
void wxsharp_textbox_set_insertion_point_end(wxsharp_handle ctrl) { static_cast<wxTextCtrl*>(ctrl)->SetInsertionPointEnd(); }

// The current selection as [from, to). from == to means an empty selection (just the caret).
void wxsharp_textbox_get_selection(wxsharp_handle ctrl, int* from, int* to)
{
    long f = 0, t = 0;
    static_cast<wxTextCtrl*>(ctrl)->GetSelection(&f, &t);
    if (from) *from = static_cast<int>(f);
    if (to) *to = static_cast<int>(t);
}

void wxsharp_textbox_set_selection(wxsharp_handle ctrl, int from, int to)
{
    static_cast<wxTextCtrl*>(ctrl)->SetSelection(from, to);
}

int wxsharp_textbox_get_selected_text(wxsharp_handle ctrl, char* buffer, int buffer_length)
{
    return CopyToBuffer(static_cast<wxTextCtrl*>(ctrl)->GetStringSelection(), buffer, buffer_length);
}

int wxsharp_textbox_line_count(wxsharp_handle ctrl) { return static_cast<wxTextCtrl*>(ctrl)->GetNumberOfLines(); }

int wxsharp_textbox_line_length(wxsharp_handle ctrl, int line)
{
    auto* text = static_cast<wxTextCtrl*>(ctrl);
    return line >= 0 && line < text->GetNumberOfLines() ? text->GetLineLength(line) : -1;
}

int wxsharp_textbox_get_line_text(wxsharp_handle ctrl, int line, char* buffer, int buffer_length)
{
    auto* text = static_cast<wxTextCtrl*>(ctrl);
    if (line < 0 || line >= text->GetNumberOfLines())
        return CopyToBuffer(wxString(), buffer, buffer_length);
    return CopyToBuffer(text->GetLineText(line), buffer, buffer_length);
}

// Scrolls without moving the caret - for following appended output without stealing the insertion point.
void wxsharp_textbox_show_position(wxsharp_handle ctrl, int position)
{
    static_cast<wxTextCtrl*>(ctrl)->ShowPosition(position);
}

// ---- The rest of wxTextCtrl -------------------------------------------------------------------------------
// Everything below is specific to wxTextCtrl rather than shared through wxTextEntry: the modified flag, the
// position/coordinate conversions, file loading, and character styling.

namespace
{
    wxTextCtrl* Tc(wxsharp_handle h) { return static_cast<wxTextCtrl*>(h); }

    // Turns the flat struct that crosses the ABI into a real wxTextAttr, setting only what the caller marked
    // as present so wxWidgets' own "unset means inherit" behaviour survives the trip.
    wxTextAttr ToTextAttr(const wxsharp_text_attr* a)
    {
        wxTextAttr attr;
        if (a == nullptr)
            return attr;

        if (a->flags & wxTEXT_ATTR_TEXT_COLOUR)
            attr.SetTextColour(ColourFromArgb(a->text_colour));
        if (a->flags & wxTEXT_ATTR_BACKGROUND_COLOUR)
            attr.SetBackgroundColour(ColourFromArgb(a->background_colour));
        if (a->flags & wxTEXT_ATTR_ALIGNMENT)
            attr.SetAlignment(static_cast<wxTextAttrAlignment>(a->alignment));
        if (a->flags & wxTEXT_ATTR_LEFT_INDENT)
            attr.SetLeftIndent(a->left_indent, a->left_sub_indent);
        if (a->flags & wxTEXT_ATTR_RIGHT_INDENT)
            attr.SetRightIndent(a->right_indent);
        if ((a->flags & wxTEXT_ATTR_FONT) && a->font)
            attr.SetFont(*static_cast<wxFont*>(a->font), a->flags & wxTEXT_ATTR_FONT);
        return attr;
    }

    void FromTextAttr(const wxTextAttr& attr, wxsharp_text_attr* out)
    {
        if (out == nullptr)
            return;
        *out = wxsharp_text_attr();
        out->flags = attr.GetFlags();
        out->text_colour = ArgbFromColour(attr.GetTextColour());
        out->background_colour = ArgbFromColour(attr.GetBackgroundColour());
        out->alignment = static_cast<int>(attr.GetAlignment());
        out->left_indent = attr.GetLeftIndent();
        out->left_sub_indent = attr.GetLeftSubIndent();
        out->right_indent = attr.GetRightIndent();

        // The font comes back as a handle the managed side owns and disposes. Handing out a copy rather
        // than a pointer into the attribute keeps it valid after wxTextAttr goes out of scope.
        if (attr.HasFont() && attr.GetFont().IsOk())
            out->font = new wxFont(attr.GetFont());
    }
}

bool wxsharp_textbox_is_modified(wxsharp_handle ctrl) { return Tc(ctrl)->IsModified(); }
void wxsharp_textbox_mark_dirty(wxsharp_handle ctrl) { Tc(ctrl)->MarkDirty(); }
void wxsharp_textbox_discard_edits(wxsharp_handle ctrl) { Tc(ctrl)->DiscardEdits(); }
void wxsharp_textbox_set_modified(wxsharp_handle ctrl, bool modified) { Tc(ctrl)->SetModified(modified); }

bool wxsharp_textbox_is_multiline(wxsharp_handle ctrl) { return Tc(ctrl)->IsMultiLine(); }

// The line and column a character position falls on. False when the position is out of range.
bool wxsharp_textbox_position_to_xy(wxsharp_handle ctrl, int position, int* x, int* y)
{
    long column = 0, line = 0;
    const bool ok = Tc(ctrl)->PositionToXY(position, &column, &line);
    if (x) *x = static_cast<int>(column);
    if (y) *y = static_cast<int>(line);
    return ok;
}

int wxsharp_textbox_xy_to_position(wxsharp_handle ctrl, int x, int y)
{
    return static_cast<int>(Tc(ctrl)->XYToPosition(x, y));
}

// Which character a point lands on. The result is a wxTextCtrlHitTestResult; -2 means the platform does not
// implement it.
int wxsharp_textbox_hit_test(wxsharp_handle ctrl, int x, int y, int* position)
{
    long pos = 0;
    const wxTextCtrlHitTestResult result = Tc(ctrl)->HitTest(wxPoint(x, y), &pos);
    if (position) *position = static_cast<int>(pos);
    return static_cast<int>(result);
}

bool wxsharp_textbox_load_file(wxsharp_handle ctrl, const char* path)
{
    return Tc(ctrl)->LoadFile(Str(path));
}

// An empty path saves back over the file the control was last loaded from, as wxTextCtrl does.
bool wxsharp_textbox_save_file(wxsharp_handle ctrl, const char* path)
{
    return Tc(ctrl)->SaveFile(Str(path));
}

bool wxsharp_textbox_set_style(wxsharp_handle ctrl, int start, int end, const wxsharp_text_attr* style)
{
    return Tc(ctrl)->SetStyle(start, end, ToTextAttr(style));
}

bool wxsharp_textbox_get_style(wxsharp_handle ctrl, int position, wxsharp_text_attr* style)
{
    wxTextAttr attr;
    if (!Tc(ctrl)->GetStyle(position, attr))
        return false;
    FromTextAttr(attr, style);
    return true;
}

bool wxsharp_textbox_set_default_style(wxsharp_handle ctrl, const wxsharp_text_attr* style)
{
    return Tc(ctrl)->SetDefaultStyle(ToTextAttr(style));
}

void wxsharp_textbox_get_default_style(wxsharp_handle ctrl, wxsharp_text_attr* style)
{
    FromTextAttr(Tc(ctrl)->GetDefaultStyle(), style);
}

// ---- Colour names -----------------------------------------------------------------------------------------
// wxColour parses the standard colour names and the #RRGGBB / rgb(...) notations, and names a colour back
// when it matches one of the standard ones.

bool wxsharp_colour_parse(const char* text, unsigned int* argb)
{
    wxColour colour;
    if (!colour.Set(Str(text)))
        return false;
    if (argb) *argb = ArgbFromColour(colour);
    return true;
}

int wxsharp_colour_name(unsigned int argb, char* buffer, int buffer_length)
{
    const wxColour colour = ColourFromArgb(argb);
    return CopyToBuffer(colour.GetAsString(wxC2S_NAME | wxC2S_CSS_SYNTAX), buffer, buffer_length);
}

// The colour transforms wxWidgets already implements, kept native so the results match exactly rather than
// being reimplemented from the documented formulas.
unsigned int wxsharp_colour_change_lightness(unsigned int argb, int alpha)
{
    return ArgbFromColour(ColourFromArgb(argb).ChangeLightness(alpha));
}

unsigned int wxsharp_colour_make_disabled(unsigned int argb, unsigned char brightness)
{
    wxColour colour = ColourFromArgb(argb);
    return ArgbFromColour(colour.MakeDisabled(brightness));
}

unsigned int wxsharp_colour_make_grey(unsigned int argb)
{
    wxColour colour = ColourFromArgb(argb);
    unsigned char r = colour.Red(), g = colour.Green(), b = colour.Blue();
    wxColour::MakeGrey(&r, &g, &b);
    return ArgbFromColour(wxColour(r, g, b, colour.Alpha()));
}

unsigned int wxsharp_colour_make_mono(unsigned int argb, bool on)
{
    wxColour colour = ColourFromArgb(argb);
    unsigned char r = colour.Red(), g = colour.Green(), b = colour.Blue();
    wxColour::MakeMono(&r, &g, &b, on);
    return ArgbFromColour(wxColour(r, g, b, colour.Alpha()));
}

double wxsharp_colour_luminance(unsigned int argb)
{
    return ColourFromArgb(argb).GetLuminance();
}

unsigned char wxsharp_colour_alpha_blend(unsigned char foreground, unsigned char background, double alpha)
{
    return wxColour::AlphaBlend(foreground, background, alpha);
}
