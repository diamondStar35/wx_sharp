// wxTextEntry - the editing surface wxTextCtrl, wxComboBox and wxSearchCtrl all share.
//
// wxTextEntry is a mix-in rather than a window base, so it is reached by cross-casting from the wxWindow the
// managed side holds. Every function here refuses politely when the window is not a text entry at all, which
// keeps a wrong handle from being undefined behaviour.
#include "internal.h"
#include <wx/textentry.h>

namespace
{
    inline wxTextEntryBase* TE(wxsharp_handle h)
    {
        return dynamic_cast<wxTextEntryBase*>(static_cast<wxWindow*>(h));
    }
}

bool wxsharp_textentry_supported(wxsharp_handle ctrl) { return TE(ctrl) != nullptr; }

// ---- Value ----------------------------------------------------------------------------------------------

int wxsharp_textentry_get_value(wxsharp_handle ctrl, char* buffer, int buffer_length)
{
    auto* entry = TE(ctrl);
    return CopyToBuffer(entry ? entry->GetValue() : wxString(), buffer, buffer_length);
}

void wxsharp_textentry_set_value(wxsharp_handle ctrl, const char* value)
{
    if (auto* entry = TE(ctrl)) entry->SetValue(Str(value));
}

// Sets the text without raising a text-changed event, which is how a field is synchronised from the model
// without the handler firing straight back.
void wxsharp_textentry_change_value(wxsharp_handle ctrl, const char* value)
{
    if (auto* entry = TE(ctrl)) entry->ChangeValue(Str(value));
}

void wxsharp_textentry_write_text(wxsharp_handle ctrl, const char* text)
{
    if (auto* entry = TE(ctrl)) entry->WriteText(Str(text));
}

void wxsharp_textentry_append_text(wxsharp_handle ctrl, const char* text)
{
    if (auto* entry = TE(ctrl)) entry->AppendText(Str(text));
}

int wxsharp_textentry_get_range(wxsharp_handle ctrl, int from, int to, char* buffer, int buffer_length)
{
    auto* entry = TE(ctrl);
    return CopyToBuffer(entry ? entry->GetRange(from, to) : wxString(), buffer, buffer_length);
}

void wxsharp_textentry_replace(wxsharp_handle ctrl, int from, int to, const char* value)
{
    if (auto* entry = TE(ctrl)) entry->Replace(from, to, Str(value));
}

void wxsharp_textentry_remove(wxsharp_handle ctrl, int from, int to)
{
    if (auto* entry = TE(ctrl)) entry->Remove(from, to);
}

void wxsharp_textentry_clear(wxsharp_handle ctrl) { if (auto* entry = TE(ctrl)) entry->Clear(); }
bool wxsharp_textentry_is_empty(wxsharp_handle ctrl) { auto* e = TE(ctrl); return !e || e->IsEmpty(); }

// ---- Clipboard and undo ---------------------------------------------------------------------------------

void wxsharp_textentry_copy(wxsharp_handle ctrl) { if (auto* e = TE(ctrl)) e->Copy(); }
void wxsharp_textentry_cut(wxsharp_handle ctrl) { if (auto* e = TE(ctrl)) e->Cut(); }
void wxsharp_textentry_paste(wxsharp_handle ctrl) { if (auto* e = TE(ctrl)) e->Paste(); }
bool wxsharp_textentry_can_copy(wxsharp_handle ctrl) { auto* e = TE(ctrl); return e && e->CanCopy(); }
bool wxsharp_textentry_can_cut(wxsharp_handle ctrl) { auto* e = TE(ctrl); return e && e->CanCut(); }
bool wxsharp_textentry_can_paste(wxsharp_handle ctrl) { auto* e = TE(ctrl); return e && e->CanPaste(); }
void wxsharp_textentry_undo(wxsharp_handle ctrl) { if (auto* e = TE(ctrl)) e->Undo(); }
void wxsharp_textentry_redo(wxsharp_handle ctrl) { if (auto* e = TE(ctrl)) e->Redo(); }
bool wxsharp_textentry_can_undo(wxsharp_handle ctrl) { auto* e = TE(ctrl); return e && e->CanUndo(); }
bool wxsharp_textentry_can_redo(wxsharp_handle ctrl) { auto* e = TE(ctrl); return e && e->CanRedo(); }

// ---- Caret and selection --------------------------------------------------------------------------------

void wxsharp_textentry_set_insertion_point(wxsharp_handle ctrl, int position)
{
    if (auto* e = TE(ctrl)) e->SetInsertionPoint(position);
}

void wxsharp_textentry_set_insertion_point_end(wxsharp_handle ctrl)
{
    if (auto* e = TE(ctrl)) e->SetInsertionPointEnd();
}

int wxsharp_textentry_get_insertion_point(wxsharp_handle ctrl)
{
    auto* e = TE(ctrl);
    return e ? static_cast<int>(e->GetInsertionPoint()) : 0;
}

int wxsharp_textentry_get_last_position(wxsharp_handle ctrl)
{
    auto* e = TE(ctrl);
    return e ? static_cast<int>(e->GetLastPosition()) : 0;
}

void wxsharp_textentry_set_selection(wxsharp_handle ctrl, int from, int to)
{
    if (auto* e = TE(ctrl)) e->SetSelection(from, to);
}

void wxsharp_textentry_get_selection(wxsharp_handle ctrl, int* from, int* to)
{
    long f = 0, t = 0;
    if (auto* e = TE(ctrl)) e->GetSelection(&f, &t);
    if (from) *from = static_cast<int>(f);
    if (to) *to = static_cast<int>(t);
}

void wxsharp_textentry_select_all(wxsharp_handle ctrl) { if (auto* e = TE(ctrl)) e->SelectAll(); }
void wxsharp_textentry_select_none(wxsharp_handle ctrl) { if (auto* e = TE(ctrl)) e->SelectNone(); }
bool wxsharp_textentry_has_selection(wxsharp_handle ctrl) { auto* e = TE(ctrl); return e && e->HasSelection(); }

int wxsharp_textentry_get_selected_text(wxsharp_handle ctrl, char* buffer, int buffer_length)
{
    auto* e = TE(ctrl);
    return CopyToBuffer(e ? e->GetStringSelection() : wxString(), buffer, buffer_length);
}

void wxsharp_textentry_remove_selection(wxsharp_handle ctrl) { if (auto* e = TE(ctrl)) e->RemoveSelection(); }

// ---- Editing constraints and presentation ---------------------------------------------------------------

bool wxsharp_textentry_is_editable(wxsharp_handle ctrl) { auto* e = TE(ctrl); return e && e->IsEditable(); }
void wxsharp_textentry_set_editable(wxsharp_handle ctrl, bool editable) { if (auto* e = TE(ctrl)) e->SetEditable(editable); }

void wxsharp_textentry_set_max_length(wxsharp_handle ctrl, int length)
{
    if (auto* e = TE(ctrl)) e->SetMaxLength(length < 0 ? 0 : static_cast<unsigned long>(length));
}

void wxsharp_textentry_force_upper(wxsharp_handle ctrl) { if (auto* e = TE(ctrl)) e->ForceUpper(); }

bool wxsharp_textentry_set_hint(wxsharp_handle ctrl, const char* hint)
{
    auto* e = TE(ctrl);
    return e && e->SetHint(Str(hint));
}

int wxsharp_textentry_get_hint(wxsharp_handle ctrl, char* buffer, int buffer_length)
{
    auto* e = TE(ctrl);
    return CopyToBuffer(e ? e->GetHint() : wxString(), buffer, buffer_length);
}

bool wxsharp_textentry_set_margins(wxsharp_handle ctrl, int left, int top)
{
    auto* e = TE(ctrl);
    return e && e->SetMargins(left, top);
}

void wxsharp_textentry_get_margins(wxsharp_handle ctrl, int* left, int* top)
{
    wxPoint margins(-1, -1);
    if (auto* e = TE(ctrl)) margins = e->GetMargins();
    if (left) *left = margins.x;
    if (top) *top = margins.y;
}

// ---- Completion -----------------------------------------------------------------------------------------

bool wxsharp_textentry_auto_complete(wxsharp_handle ctrl, const char* const* choices, int count)
{
    auto* e = TE(ctrl);
    if (!e) return false;
    wxArrayString items;
    for (int i = 0; i < count; ++i)
        items.Add(Str(choices[i]));
    return e->AutoComplete(items);
}

bool wxsharp_textentry_auto_complete_files(wxsharp_handle ctrl)
{
    auto* e = TE(ctrl);
    return e && e->AutoCompleteFileNames();
}

bool wxsharp_textentry_auto_complete_directories(wxsharp_handle ctrl)
{
    auto* e = TE(ctrl);
    return e && e->AutoCompleteDirectories();
}
