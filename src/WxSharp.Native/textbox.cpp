// Single- or multi-line text field. Single-line boxes process Enter so it raises a TextEnter event.
#include "internal.h"

wxsharp_handle wxsharp_textbox_create(wxsharp_handle parent, int id, const char* value, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    long flags = MapTextBoxStyle(style);
    if (!(flags & wxTE_MULTILINE))
        flags |= wxTE_PROCESS_ENTER;
    auto* ctrl = new wxTextCtrl(p, id, Str(value), wxDefaultPosition, wxDefaultSize, flags);
    ctrl->Bind(wxEVT_TEXT, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_TEXT, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    // wxEVT_TEXT_ENTER only fires (and may only be bound) when the control processes Enter; binding it without
    // wxTE_PROCESS_ENTER trips a wx assert, so gate the bind on the flag.
    if (flags & wxTE_PROCESS_ENTER)
        ctrl->Bind(wxEVT_TEXT_ENTER, [token](wxCommandEvent& e) { if (!(Fire(token, WXSHARP_EVT_TEXT_ENTER, e.GetId()) & WXSHARP_EVENT_HANDLED)) e.Skip(); });
    BindCommon(ctrl, token);
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
