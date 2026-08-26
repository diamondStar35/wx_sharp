// Drop-down choice.
#include "internal.h"

wxsharp_handle wxsharp_choice_create(wxsharp_handle parent, int id, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new wxChoice(p, id, wxDefaultPosition, wxDefaultSize, 0, nullptr, MapChoiceStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

void wxsharp_choice_append(wxsharp_handle ctrl, const char* item) { static_cast<wxChoice*>(ctrl)->Append(Str(item)); }
void wxsharp_choice_insert(wxsharp_handle ctrl, const char* item, int index) { static_cast<wxChoice*>(ctrl)->Insert(Str(item), index); }
void wxsharp_choice_delete(wxsharp_handle ctrl, int index) { static_cast<wxChoice*>(ctrl)->Delete(index); }
void wxsharp_choice_clear(wxsharp_handle ctrl) { static_cast<wxChoice*>(ctrl)->Clear(); }
int wxsharp_choice_count(wxsharp_handle ctrl) { return static_cast<int>(static_cast<wxChoice*>(ctrl)->GetCount()); }

int wxsharp_choice_get_string(wxsharp_handle ctrl, int index, char* buffer, int buffer_length)
{
    return CopyToBuffer(static_cast<wxChoice*>(ctrl)->GetString(index), buffer, buffer_length);
}

void wxsharp_choice_set_string(wxsharp_handle ctrl, int index, const char* text)
{
    static_cast<wxChoice*>(ctrl)->SetString(index, Str(text));
}

int wxsharp_choice_find_string(wxsharp_handle ctrl, const char* text)
{
    return static_cast<wxChoice*>(ctrl)->FindString(Str(text));
}

int wxsharp_choice_get_selection(wxsharp_handle ctrl) { return static_cast<wxChoice*>(ctrl)->GetSelection(); }
void wxsharp_choice_set_selection(wxsharp_handle ctrl, int index) { static_cast<wxChoice*>(ctrl)->SetSelection(index); }
