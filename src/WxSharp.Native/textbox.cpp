// Single- or multi-line text field. Single-line boxes process Enter so it raises a TextEnter event.
#include "internal.h"

wxsharp_handle wxsharp_textbox_create(wxsharp_handle parent, const char* value, bool fill, int style, int id)
{
    auto* p = static_cast<wxWindow*>(parent);
    // Start from the caller's requested style; fill implies a multi-line box that owns the window, otherwise a
    // single-line box that raises TextEnter on Enter (so multi-line boxes keep Enter for newlines).
    long flags = MapTextBoxStyle(style);
    if (fill)
        flags |= wxTE_MULTILINE;
    else
        flags |= wxTE_PROCESS_ENTER;
    // fill: size to the parent's client area and stay out of any sizer, so it covers the window with no
    // relayout (an inline prompt that owns the frame); otherwise stack normally in the parent's sizer.
    wxTextCtrl* ctrl = fill
        ? new wxTextCtrl(p, wxID_ANY, Str(value), wxPoint(0, 0), p->GetClientSize(), flags)
        : new wxTextCtrl(p, wxID_ANY, Str(value), wxDefaultPosition, wxDefaultSize, flags);
    ctrl->Bind(wxEVT_TEXT, [id](wxCommandEvent&) { Fire(id, WXSHARP_EVT_TEXT); });
    // wxEVT_TEXT_ENTER only fires (and may only be bound) when the control processes Enter; binding it without
    // wxTE_PROCESS_ENTER trips a wx assert, so gate the bind on the flag.
    if (flags & wxTE_PROCESS_ENTER)
        ctrl->Bind(wxEVT_TEXT_ENTER, [id](wxCommandEvent& e) { Fire(id, WXSHARP_EVT_TEXT_ENTER); e.Skip(); });
    BindCommon(ctrl, id);
    if (!fill)
        AddToPanel(p, ctrl, wxLEFT | wxRIGHT | wxBOTTOM | wxEXPAND);
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
