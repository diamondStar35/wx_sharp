// The channel a managed subclass answers wxWidgets' virtual members through.
//
// wxWidgets asks a window questions by calling virtual members: may it take focus, how big does it want to
// be, is the data in it valid. A wrapper that cannot forward those cannot host a real custom control, and
// the accessibility cases are the ones that hurt most - a control that refuses keyboard focus so screen
// reader users are not made to tab through it has no other expression.
//
// wxPython solves this by making a whitelist of wxWindow virtuals overridable from Python
// (etgtools/tweaker_tools.py, addWindowVirtuals) rather than all of them; this is the same whitelist, and
// the same reasoning. Overriding is opt-in at construction because C++ fixes a vtable there, so the plain
// create functions remain exactly as cheap as they were.
#include "internal.h"
#include <wx/activityindicator.h>
#include <wx/bmpbuttn.h>
#include <wx/checklst.h>
#include <wx/combobox.h>
#include <wx/datectrl.h>
#include <wx/gauge.h>
#include <wx/grid.h>
#include <wx/hyperlink.h>
#include <wx/notebook.h>
#include <wx/radiobox.h>
#include <wx/simplebook.h>
#include <wx/spinctrl.h>
#include <wx/splitter.h>
#include <wx/srchctrl.h>
#include <wx/statbmp.h>
#include <wx/statbox.h>
#include <wx/statline.h>
#include <wx/tglbtn.h>
#include <wx/timectrl.h>

wxsharp_virtual_cb g_virtual_cb = nullptr;

void wxsharp_set_virtual_handler(wxsharp_virtual_cb cb) { g_virtual_cb = cb; }

namespace
{
    // wxTreeCtrl::SortChildren() on MSW checks wx runtime class information and bypasses
    // OnCompareItems() for an exact wxTreeCtrl. A real wx class identity is therefore part of making the
    // managed override work; ordinary C++ inheritance alone is not sufficient on this port.
    class WxSharpTreeCtrl : public Overridable<wxTreeCtrl>
    {
    public:
        WxSharpTreeCtrl(long long token, wxWindow* parent, int id, long style)
            : Overridable<wxTreeCtrl>(token, parent, id, wxDefaultPosition, wxDefaultSize, style),
              m_token(token) {}

        int OnCompareItems(const wxTreeItemId& first, const wxTreeItemId& second) override
        {
            if (!g_virtual_list_cb)
                return wxTreeCtrl::OnCompareItems(first, second);
            wxsharp_virtual_list_request request = {};
            request.size = sizeof(request);
            request.version = 1;
            request.token = m_token;
            request.item = TreeValue(first);
            request.other_item = TreeValue(second);
            request.operation = 10;
            return g_virtual_list_cb(&request) ? request.result
                                               : wxTreeCtrl::OnCompareItems(first, second);
        }

        wxDECLARE_ABSTRACT_CLASS(WxSharpTreeCtrl);

    private:
        long long m_token;
    };

    wxIMPLEMENT_ABSTRACT_CLASS(WxSharpTreeCtrl, wxTreeCtrl);
}

// Runs wxWidgets' own implementation of one virtual on behalf of a managed override, without dispatching
// back to it. A window with no overrides installed cannot be asked - nothing can be overriding it, so
// nothing can be calling base on it - and says so by leaving `handled` clear.
void wxsharp_window_call_base(wxsharp_handle window, wxsharp_virtual_request* request)
{
    if (!request || request->version != 1 || request->size < sizeof(wxsharp_virtual_request) || !window)
        return;

    request->handled = 0;
    auto* w = static_cast<wxWindow*>(window);

    if (auto* overridable = dynamic_cast<OverridableWindow*>(w))
    {
        overridable->CallBase(*request);
        return;
    }

    // No override hooks on this window, so nothing can be overriding these and calling the virtual cannot
    // dispatch anywhere but wxWidgets' own implementation. Only the public members appear here: a protected
    // one is reachable only from a subclass, and every subclass is built with the hooks above.
    request->handled = 1;
    switch (request->which)
    {
        case WXSHARP_VIRT_ACCEPTS_FOCUS:               request->result = w->AcceptsFocus(); break;
        case WXSHARP_VIRT_ACCEPTS_FOCUS_FROM_KEYBOARD: request->result = w->AcceptsFocusFromKeyboard(); break;
        case WXSHARP_VIRT_ACCEPTS_FOCUS_RECURSIVELY:   request->result = w->AcceptsFocusRecursively(); break;
        case WXSHARP_VIRT_VALIDATE:                    request->result = w->Validate(); break;
        case WXSHARP_VIRT_TRANSFER_TO_WINDOW:          request->result = w->TransferDataToWindow(); break;
        case WXSHARP_VIRT_TRANSFER_FROM_WINDOW:        request->result = w->TransferDataFromWindow(); break;
        case WXSHARP_VIRT_SHOULD_INHERIT_COLOURS:      request->result = w->ShouldInheritColours(); break;
        case WXSHARP_VIRT_INIT_DIALOG:                 w->InitDialog(); break;
        case WXSHARP_VIRT_INHERIT_ATTRIBUTES:          w->InheritAttributes(); break;
        case WXSHARP_VIRT_ON_INTERNAL_IDLE:            w->OnInternalIdle(); break;
        case WXSHARP_VIRT_SET_CAN_FOCUS:               w->SetCanFocus(request->args[0] != 0); break;
        case WXSHARP_VIRT_ENABLE_VISIBLE_FOCUS:        w->EnableVisibleFocus(request->args[0] != 0); break;
        case WXSHARP_VIRT_DESTROY:                     request->result = w->Destroy(); break;

        case WXSHARP_VIRT_CLIENT_AREA_ORIGIN:
        {
            const wxPoint origin = w->GetClientAreaOrigin();
            request->x = origin.x;
            request->y = origin.y;
            break;
        }
        case WXSHARP_VIRT_ADD_CHILD:
            w->AddChild(static_cast<wxWindowBase*>(reinterpret_cast<void*>(
                static_cast<intptr_t>(request->handle))));
            break;
        case WXSHARP_VIRT_REMOVE_CHILD:
            w->RemoveChild(static_cast<wxWindowBase*>(reinterpret_cast<void*>(
                static_cast<intptr_t>(request->handle))));
            break;
        case WXSHARP_VIRT_MAIN_WINDOW_OF_COMPOSITE:
            request->handle = static_cast<long long>(reinterpret_cast<intptr_t>(
                w->GetMainWindowOfCompositeControl()));
            break;
        case WXSHARP_VIRT_INFORM_FIRST_DIRECTION:
            request->result = w->InformFirstDirection(request->args[0], request->args[1], request->args[2]);
            break;

        default: request->handled = 0; break;
    }
}

wxsharp_handle wxsharp_custom_frame_create(wxsharp_handle parent, int id, const char* title,
                                           int x, int y, int width, int height, int style, long long token)
{
    auto* frame = new OverridableFrame<wxFrame>(token, static_cast<wxWindow*>(parent), id, Str(title),
                                           wxPoint(x, y), wxSize(width, height), MapFrameStyle(style));
    TrackWindow(frame, token);
    return frame;
}

wxsharp_handle wxsharp_custom_panel_create(wxsharp_handle parent, int id, int style, long long token)
{
    auto* panel = new Overridable<wxPanel>(token, static_cast<wxWindow*>(parent), id, wxDefaultPosition,
                                           wxDefaultSize, MapPanelStyle(style));
    TrackWindow(panel, token);
    return panel;
}

wxsharp_handle wxsharp_custom_progress_create(wxsharp_handle parent, const char* title,
                                              const char* message, int maximum, int style,
                                              long long token)
{
    return Common(new Overridable<wxProgressDialog>(token, Str(title), Str(message), maximum,
                                                     static_cast<wxWindow*>(parent),
                                                     MapProgressStyle(style)), token);
}

wxsharp_handle wxsharp_custom_button_create(wxsharp_handle parent, int id, const char* label, long long token)
{
    auto* button = new Overridable<wxButton>(token, static_cast<wxWindow*>(parent), id, Str(label));
    TrackWindow(button, token);
    return button;
}

wxsharp_handle wxsharp_custom_dialog_create(wxsharp_handle parent, int id, const char* title,
                                            int x, int y, int width, int height, int style, long long token)
{
    auto* dlg = new OverridableDialog<wxDialog>(token, static_cast<wxWindow*>(parent), id, Str(title),
                                          wxPoint(x, y), wxSize(width, height), MapDialogStyle(style));
    TrackWindow(dlg, token);
    return dlg;
}

// ---- Overridable twins ----------------------------------------------------------------------------------
// One per window class, each differing from the plain create by exactly which class it constructs. Managed
// code picks between them by the type being constructed, so an exact Button still gets a plain wxButton and
// pays nothing for hooks it cannot use.
//
// Absent, and each for a reason: Canvas, whose WxSharpCanvas refuses focus by design and would have that
// routed to managed code; StatusBar, ToolBar and the close button, which wxWidgets builds through factories
// rather than `new`; and the menus, sizers, timers and locales, which are not windows.

wxsharp_handle wxsharp_custom_activity_create(wxsharp_handle parent, int id, long long token)
{ return Common(new Overridable<wxActivityIndicator>(token, static_cast<wxWindow*>(parent), id), token); 
}

wxsharp_handle wxsharp_custom_bitmapbutton_create(wxsharp_handle parent, int id, wxsharp_handle bitmap, long long token)
{
    auto* control = Common(new Overridable<wxBitmapButton>(token, static_cast<wxWindow*>(parent), id, *static_cast<wxBitmap*>(bitmap)), token);
    return control;
}

wxsharp_handle wxsharp_custom_checkbox_create(wxsharp_handle parent, int id, const char* label, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new Overridable<wxCheckBox>(token, p, id, Str(label), wxDefaultPosition, wxDefaultSize, MapCheckBoxStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

wxsharp_handle wxsharp_custom_checklistbox_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new Overridable<wxCheckListBox>(token, static_cast<wxWindow*>(parent), id), token);
    return control;
}

wxsharp_handle wxsharp_custom_choice_create(wxsharp_handle parent, int id, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new Overridable<wxChoice>(token, p, id, wxDefaultPosition, wxDefaultSize, 0, nullptr, MapChoiceStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

wxsharp_handle wxsharp_custom_combobox_create(wxsharp_handle parent, int id, const char* value, bool readOnly, long long token)
{
    auto* control = Common(new Overridable<wxComboBox>(token, static_cast<wxWindow*>(parent), id, Str(value), wxDefaultPosition,
        wxDefaultSize, 0, nullptr, readOnly ? wxCB_READONLY : 0), token);
    return control;
}

wxsharp_handle wxsharp_custom_dataviewlist_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new Overridable<wxDataViewListCtrl>(token, static_cast<wxWindow*>(parent), id), token);
    return control;
}

wxsharp_handle wxsharp_custom_dataviewtree_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new Overridable<wxDataViewTreeCtrl>(token, static_cast<wxWindow*>(parent), id), token);
    return control;
}

wxsharp_handle wxsharp_custom_datepicker_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new Overridable<wxDatePickerCtrl>(token, static_cast<wxWindow*>(parent), id), token);
    return control;
}

wxsharp_handle wxsharp_custom_gauge_create(wxsharp_handle parent, int id, int range, int value, bool vertical, long long token)
{
    auto* control = Common(new Overridable<wxGauge>(token, static_cast<wxWindow*>(parent), id, range, wxDefaultPosition,
        wxDefaultSize, vertical ? wxGA_VERTICAL : wxGA_HORIZONTAL), token);
    control->SetValue(value);
    return control;
}

wxsharp_handle wxsharp_custom_grid_create(wxsharp_handle parent, int id, int rows, int columns, long long token)
{
    auto* control = Common(new OverridableGrid<wxGrid>(token, static_cast<wxWindow*>(parent), id), token);
    control->CreateGrid(rows, columns);
    return control;
}

wxsharp_handle wxsharp_custom_hyperlink_create(wxsharp_handle parent, int id, const char* label, const char* url, long long token)
{
    auto* control = Common(new Overridable<wxHyperlinkCtrl>(token, static_cast<wxWindow*>(parent), id, Str(label), Str(url)), token);
    return control;
}

wxsharp_handle wxsharp_custom_label_create(wxsharp_handle parent, int id, const char* text, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new Overridable<wxStaticText>(token, p, id, Str(text), wxDefaultPosition, wxDefaultSize, MapAlignment(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

wxsharp_handle wxsharp_custom_listbox_create(wxsharp_handle parent, int id, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new Overridable<wxListBox>(token, p, id, wxDefaultPosition, wxDefaultSize, 0, nullptr, MapListBoxStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

wxsharp_handle wxsharp_custom_notebook_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new Overridable<wxNotebook>(token, static_cast<wxWindow*>(parent), id), token);
    return control;
}

wxsharp_handle wxsharp_custom_radio_create(wxsharp_handle parent, int id, const char* label, bool group_start, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new Overridable<wxRadioButton>(token, p, id, Str(label), wxDefaultPosition, wxDefaultSize,
                                   group_start ? wxRB_GROUP : 0);
    TrackWindow(ctrl, token);
    return ctrl;
}

wxsharp_handle wxsharp_custom_radiobox_create(wxsharp_handle parent, int id, const char* label, const char* const* choices,
                                       int count, int columns, long long token)
{
    wxArrayString items;
    for (int i = 0; i < count; ++i) items.Add(Str(choices[i]));
    auto* control = Common(new Overridable<wxRadioBox>(token, static_cast<wxWindow*>(parent), id, Str(label), wxDefaultPosition,
        wxDefaultSize, items, columns > 0 ? columns : 1, wxRA_SPECIFY_COLS), token);
    return control;
}

wxsharp_handle wxsharp_custom_scrollbar_create(wxsharp_handle parent, int id, bool vertical, long long token)
{
    auto* control = Common(new Overridable<wxScrollBar>(token, static_cast<wxWindow*>(parent), id, wxDefaultPosition,
        wxDefaultSize, vertical ? wxSB_VERTICAL : wxSB_HORIZONTAL), token);
    return control;
}

wxsharp_handle wxsharp_custom_scrolled_create(wxsharp_handle parent, int id, int style, long long token)
{
    return Common(new OverridableScrolled<wxScrolledWindow>(token, static_cast<wxWindow*>(parent), id, wxDefaultPosition, wxDefaultSize,
                                       MapScrolledStyle(style)), token);
}

wxsharp_handle wxsharp_custom_searchctrl_create(wxsharp_handle parent, int id, const char* value, long long token)
{
    auto* control = Common(new Overridable<wxSearchCtrl>(token, static_cast<wxWindow*>(parent), id, Str(value)), token);
    return control;
}

wxsharp_handle wxsharp_custom_simplebook_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new Overridable<wxSimplebook>(token, static_cast<wxWindow*>(parent), id), token);
    return control;
}

wxsharp_handle wxsharp_custom_slider_create(wxsharp_handle parent, int id, int min_value, int max_value, int value, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    auto* ctrl = new Overridable<wxSlider>(token, p, id, value, min_value, max_value,
                              wxDefaultPosition, wxDefaultSize, MapSliderStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

wxsharp_handle wxsharp_custom_spinctrl_create(wxsharp_handle parent, int id, int minValue, int maxValue, int value, long long token)
{
    auto* control = Common(new Overridable<wxSpinCtrl>(token, static_cast<wxWindow*>(parent), id, wxEmptyString,
        wxDefaultPosition, wxDefaultSize, wxSP_ARROW_KEYS, minValue, maxValue, value), token);
    return control;
}

wxsharp_handle wxsharp_custom_spinctrldouble_create(wxsharp_handle parent, int id, double minValue, double maxValue,
                                             double value, double increment, long long token)
{
    auto* control = Common(new Overridable<wxSpinCtrlDouble>(token, static_cast<wxWindow*>(parent), id, wxEmptyString,
        wxDefaultPosition, wxDefaultSize, wxSP_ARROW_KEYS, minValue, maxValue, value, increment), token);
    return control;
}

wxsharp_handle wxsharp_custom_splitter_create(wxsharp_handle parent, int id, bool vertical, long long token)
{
    auto* control = Common(new Overridable<wxSplitterWindow>(token, static_cast<wxWindow*>(parent), id), token);
    control->SetSplitMode(vertical ? wxSPLIT_VERTICAL : wxSPLIT_HORIZONTAL);
    return control;
}

wxsharp_handle wxsharp_custom_staticbitmap_create(wxsharp_handle parent, int id, wxsharp_handle bitmap, long long token)
{
    return Common(new Overridable<wxStaticBitmap>(token, static_cast<wxWindow*>(parent), id, *static_cast<wxBitmap*>(bitmap)), token);
}

wxsharp_handle wxsharp_custom_staticbox_create(wxsharp_handle parent, int id, const char* label, long long token)
{ return Common(new Overridable<wxStaticBox>(token, static_cast<wxWindow*>(parent), id, Str(label)), token); 
}

wxsharp_handle wxsharp_custom_staticline_create(wxsharp_handle parent, int id, bool vertical, long long token)
{ return Common(new Overridable<wxStaticLine>(token, static_cast<wxWindow*>(parent), id, wxDefaultPosition, wxDefaultSize, vertical ? wxLI_VERTICAL : wxLI_HORIZONTAL), token); 
}

wxsharp_handle wxsharp_custom_textbox_create(wxsharp_handle parent, int id, const char* value, int style, long long token)
{
    auto* p = static_cast<wxWindow*>(parent);
    // The style is passed through exactly as given. wxWidgets does not add wxTE_PROCESS_ENTER by itself,
    // and adding it would stop Enter reaching a dialog's default button; ask for TextCtrlStyle.ProcessEnter
    // when the control should handle Enter instead.
    auto* ctrl = new Overridable<wxTextCtrl>(token, p, id, Str(value), wxDefaultPosition, wxDefaultSize,
                                MapTextBoxStyle(style));
    TrackWindow(ctrl, token);
    return ctrl;
}

wxsharp_handle wxsharp_custom_timepicker_create(wxsharp_handle parent, int id, long long token)
{
    auto* control = Common(new Overridable<wxTimePickerCtrl>(token, static_cast<wxWindow*>(parent), id), token);
    return control;
}

wxsharp_handle wxsharp_custom_togglebutton_create(wxsharp_handle parent, int id, const char* label, long long token)
{
    auto* control = Common(new Overridable<wxToggleButton>(token, static_cast<wxWindow*>(parent), id, Str(label)), token);
    return control;
}

wxsharp_handle wxsharp_custom_treectrl_create(wxsharp_handle parent, int id, int style, long long token)
{
    return Common(new WxSharpTreeCtrl(token, static_cast<wxWindow*>(parent), id,
                                      MapTreeCtrlStyle(style)), token);
}
