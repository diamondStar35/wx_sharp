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
