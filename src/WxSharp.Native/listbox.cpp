// Single-selection list box.
#include "internal.h"

wxsharp_handle wxsharp_listbox_create(wxsharp_handle parent, int id, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new wxListBox(p, id, wxDefaultPosition, wxDefaultSize, 0, nullptr, MapListBoxStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

void wxsharp_listbox_append(wxsharp_handle ctrl, const char* item) { static_cast<wxListBox*>(ctrl)->Append(Str(item)); }
void wxsharp_listbox_insert(wxsharp_handle ctrl, const char* item, int index) { static_cast<wxListBox*>(ctrl)->Insert(Str(item), index); }
void wxsharp_listbox_delete(wxsharp_handle ctrl, int index) { static_cast<wxListBox*>(ctrl)->Delete(index); }
void wxsharp_listbox_clear(wxsharp_handle ctrl) { static_cast<wxListBox*>(ctrl)->Clear(); }
int wxsharp_listbox_count(wxsharp_handle ctrl) { return static_cast<int>(static_cast<wxListBox*>(ctrl)->GetCount()); }

int wxsharp_listbox_get_string(wxsharp_handle ctrl, int index, char* buffer, int buffer_length)
{
    return CopyToBuffer(static_cast<wxListBox*>(ctrl)->GetString(index), buffer, buffer_length);
}

void wxsharp_listbox_set_string(wxsharp_handle ctrl, int index, const char* text)
{
    static_cast<wxListBox*>(ctrl)->SetString(index, Str(text));
}

int wxsharp_listbox_find_string(wxsharp_handle ctrl, const char* text)
{
    return static_cast<wxListBox*>(ctrl)->FindString(Str(text));
}

int wxsharp_listbox_get_selection(wxsharp_handle ctrl) { return static_cast<wxListBox*>(ctrl)->GetSelection(); }
void wxsharp_listbox_set_selection(wxsharp_handle ctrl, int index) { static_cast<wxListBox*>(ctrl)->SetSelection(index); }

// Multi-selection. Fills up to buffer_length selected indices and returns the total count (call with a null
// buffer to size it first). select/is_selected/ensure_visible operate on a single item in a multi-select box.
int wxsharp_listbox_get_selections(wxsharp_handle ctrl, int* buffer, int buffer_length)
{
    wxArrayInt selections;
    const int count = static_cast<wxListBox*>(ctrl)->GetSelections(selections);
    if (buffer && buffer_length > 0)
    {
        const int n = std::min(count, buffer_length);
        for (int i = 0; i < n; ++i)
            buffer[i] = selections[i];
    }
    return count;
}

void wxsharp_listbox_select(wxsharp_handle ctrl, int index, bool select)
{
    static_cast<wxListBox*>(ctrl)->SetSelection(index, select);
}

bool wxsharp_listbox_is_selected(wxsharp_handle ctrl, int index)
{
    return static_cast<wxListBox*>(ctrl)->IsSelected(index);
}

void wxsharp_listbox_deselect_all(wxsharp_handle ctrl)
{
    auto* box = static_cast<wxListBox*>(ctrl);
    // wxListBox::DeselectAll only exists on a multi-selection box; on a single-selection one clearing the
    // selection is SetSelection(wxNOT_FOUND), and calling the wrong one asserts.
    if (box->HasMultipleSelection())
        box->DeselectAll();
    else
        box->SetSelection(wxNOT_FOUND);
}

void wxsharp_listbox_ensure_visible(wxsharp_handle ctrl, int index)
{
    static_cast<wxListBox*>(ctrl)->EnsureVisible(index);
}
