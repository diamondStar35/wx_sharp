// The shell owns customized controls and layout; no private wxWidgets ABI is used.
#include "../internal.h"
#include <map>
#include <memory>
#ifdef __WXMSW__
#include <shobjidl.h>
#include <wx/dirdlg.h>
#include "accessibility.h"

namespace {
struct Control {
    int kind;
    DWORD group;
    std::vector<DWORD> items;
    int selection = -1;
    DWORD label = 0;
    wxString name;
};

struct ItemDialog {
    IFileDialog* dialog = nullptr;
    IFileDialogCustomize* custom = nullptr;
    wxDialog* window;
    wxsharp_item_dialog_cb callback;
    long long token;
    std::map<DWORD, Control> controls;
    std::vector<wxString> paths;
    std::vector<wxString> extensions;
    DWORD next = 1, radio = 0;
    bool folders;
    bool save = false;
    int filter = 0;
    DialogAccessibility accessibility;

    ItemDialog(wxDialog* w, bool f, wxsharp_item_dialog_cb cb, long long t)
        : window(w), callback(cb), token(t), folders(f) {}
    ~ItemDialog() {
        if (custom) custom->Release();
        if (dialog) dialog->Release();
    }
    bool Notify(int operation, int id = 0, int value = 0) {
        return callback && callback(token, operation, id, value) != 0;
    }
    HRESULT NameControls() {
        std::vector<wxString> labels;
        for (const auto& entry : controls)
            if (entry.second.label) labels.push_back(entry.second.name);
        return accessibility.Apply(dialog, labels);
    }
    wxString Current() {
        IShellItem* item = nullptr;
        if (FAILED(dialog->GetCurrentSelection(&item)) || !item) dialog->GetFolder(&item);
        if (!item) return {};
        PWSTR text = nullptr;
        wxString path;
        if (SUCCEEDED(item->GetDisplayName(SIGDN_FILESYSPATH, &text))) {
            path = text;
            CoTaskMemFree(text);
        }
        item->Release();
        return path;
    }
    HRESULT Configure() {
        save = !folders && window->HasFlag(wxFD_SAVE);
        HRESULT hr = CoCreateInstance(save ? CLSID_FileSaveDialog : CLSID_FileOpenDialog, nullptr,
                                     CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&dialog));
        if (FAILED(hr)) return hr;
        hr = dialog->QueryInterface(IID_PPV_ARGS(&custom));
        if (FAILED(hr)) return hr;
        DWORD flags = FOS_FORCEFILESYSTEM | FOS_NOCHANGEDIR | FOS_PATHMUSTEXIST;
        wxString directory, title;
        if (folders) {
            auto* dir = static_cast<wxDirDialog*>(window);
            directory = dir->GetPath();
            title = dir->GetMessage();
            flags |= FOS_PICKFOLDERS | FOS_PATHMUSTEXIST;
            if (window->HasFlag(wxDD_MULTIPLE)) flags |= FOS_ALLOWMULTISELECT;
            if (window->HasFlag(wxDD_SHOW_HIDDEN)) flags |= FOS_FORCESHOWHIDDEN;
        } else {
            auto* file = static_cast<wxFileDialog*>(window);
            directory = file->GetDirectory();
            title = file->GetMessage();
            if (file->HasFlag(wxFD_FILE_MUST_EXIST)) flags |= FOS_FILEMUSTEXIST;
            if (file->HasFlag(wxFD_MULTIPLE)) flags |= FOS_ALLOWMULTISELECT;
            if (file->HasFlag(wxFD_OVERWRITE_PROMPT)) flags |= FOS_OVERWRITEPROMPT;
            if (file->HasFlag(wxFD_SHOW_HIDDEN)) flags |= FOS_FORCESHOWHIDDEN;
            if (file->HasFlag(wxFD_NO_FOLLOW)) flags |= FOS_NODEREFERENCELINKS;
            if (!file->GetFilename().empty()) {
                hr = dialog->SetFileName(file->GetFilename().wc_str());
                if (FAILED(hr)) return hr;
            }
            wxArrayString wildcards = wxSplit(file->GetWildcard(), '|');
            std::vector<COMDLG_FILTERSPEC> specs;
            for (size_t i = 0; i + 1 < wildcards.size(); i += 2) {
                specs.push_back({wildcards[i].wc_str(), wildcards[i + 1].wc_str()});
                const auto patterns = wxSplit(wildcards[i + 1], ';');
                wxString extension;
                if (!patterns.empty() && patterns[0].StartsWith("*.")) {
                    const auto candidate = patterns[0].Mid(2);
                    if (candidate.find_first_of("*?") == wxString::npos) extension = candidate;
                }
                extensions.push_back(extension);
            }
            if (!specs.empty()) {
                hr = dialog->SetFileTypes(static_cast<UINT>(specs.size()), specs.data());
                if (FAILED(hr)) return hr;
                hr = dialog->SetFileTypeIndex(static_cast<UINT>(file->GetFilterIndex() + 1));
                if (FAILED(hr)) return hr;
                hr = UpdateExtension();
                if (FAILED(hr)) return hr;
            }
        }
        hr = dialog->SetOptions(flags);
        if (FAILED(hr)) return hr;
        if (!title.empty()) {
            hr = dialog->SetTitle(title.wc_str());
            if (FAILED(hr)) return hr;
        }
        IShellItem* initial = nullptr;
        if (!directory.empty() && SUCCEEDED(SHCreateItemFromParsingName(directory.wc_str(), nullptr, IID_PPV_ARGS(&initial)))) {
            // A stale initial folder must not prevent choosing another location.
            dialog->SetFolder(initial);
            initial->Release();
        }
        return S_OK;
    }
    HRESULT UpdateExtension() {
        if (!save || extensions.empty()) return S_OK;
        UINT index = 1;
        HRESULT hr = dialog->GetFileTypeIndex(&index);
        if (FAILED(hr)) return hr;
        if (!index || index > extensions.size()) return E_INVALIDARG;
        return dialog->SetDefaultExtension(extensions[index - 1].wc_str());
    }
};

class Events final : public IFileDialogEvents, public IFileDialogControlEvents {
    LONG refs = 1;
    ItemDialog& state;
public:
    explicit Events(ItemDialog& s) : state(s) {}
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID iid, void** out) override {
        if (!out) return E_POINTER;
        *out = nullptr;
        if (iid == IID_IUnknown || iid == IID_IFileDialogEvents) *out = static_cast<IFileDialogEvents*>(this);
        else if (iid == IID_IFileDialogControlEvents) *out = static_cast<IFileDialogControlEvents*>(this);
        else return E_NOINTERFACE;
        AddRef();
        return S_OK;
    }
    ULONG STDMETHODCALLTYPE AddRef() override { return InterlockedIncrement(&refs); }
    ULONG STDMETHODCALLTYPE Release() override {
        ULONG n = InterlockedDecrement(&refs);
        if (!n) delete this;
        return n;
    }
    HRESULT Update() {
        if (!state.Notify(2)) state.dialog->Close(E_ABORT);
        const HRESULT hr = state.NameControls();
        if (FAILED(hr)) state.dialog->Close(hr);
        return S_OK;
    }
    HRESULT STDMETHODCALLTYPE OnFileOk(IFileDialog*) override { return state.Notify(3) ? S_OK : E_ABORT; }
    HRESULT STDMETHODCALLTYPE OnFolderChanging(IFileDialog*, IShellItem*) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE OnFolderChange(IFileDialog*) override { return Update(); }
    HRESULT STDMETHODCALLTYPE OnSelectionChange(IFileDialog*) override { return Update(); }
    HRESULT STDMETHODCALLTYPE OnTypeChange(IFileDialog*) override {
        HRESULT hr = state.UpdateExtension();
        return FAILED(hr) ? hr : Update();
    }
    HRESULT STDMETHODCALLTYPE OnShareViolation(IFileDialog*, IShellItem*, FDE_SHAREVIOLATION_RESPONSE* r) override { *r = FDESVR_DEFAULT; return S_OK; }
    HRESULT STDMETHODCALLTYPE OnOverwrite(IFileDialog*, IShellItem*, FDE_OVERWRITE_RESPONSE* r) override { *r = FDEOR_DEFAULT; return S_OK; }
    HRESULT STDMETHODCALLTYPE OnButtonClicked(IFileDialogCustomize*, DWORD id) override { state.Notify(4, id); return S_OK; }
    HRESULT STDMETHODCALLTYPE OnCheckButtonToggled(IFileDialogCustomize*, DWORD id, BOOL value) override { state.Notify(4, id, value); return S_OK; }
    HRESULT STDMETHODCALLTYPE OnItemSelected(IFileDialogCustomize*, DWORD id, DWORD item) override {
        auto found = state.controls.find(item);
        if (found != state.controls.end() && found->second.kind == 3) state.Notify(4, item, 1);
        else {
            auto choice = state.controls.find(id);
            if (choice != state.controls.end()) {
                const auto& items = choice->second.items;
                auto it = std::find(items.begin(), items.end(), item);
                if (it != items.end()) {
                    choice->second.selection = static_cast<int>(it - items.begin());
                    state.Notify(4, id, choice->second.selection);
                }
            }
        }
        return S_OK;
    }
    HRESULT STDMETHODCALLTYPE OnControlActivating(IFileDialogCustomize*, DWORD) override { return S_OK; }
};
}

wxsharp_handle wxsharp_item_dialog_create(wxsharp_handle window, bool folders, wxsharp_item_dialog_cb cb, long long token) {
    auto state = std::make_unique<ItemDialog>(static_cast<wxDialog*>(window), folders, cb, token);
    if (FAILED(state->Configure())) return nullptr;
    return state.release();
}
void wxsharp_item_dialog_destroy(wxsharp_handle context) { delete static_cast<ItemDialog*>(context); }
void wxsharp_item_dialog_close(wxsharp_handle context) {
    static_cast<ItemDialog*>(context)->dialog->Close(HRESULT_FROM_WIN32(ERROR_CANCELLED));
}
int wxsharp_item_dialog_show(wxsharp_handle context) {
    auto& s = *static_cast<ItemDialog*>(context);
    if (!s.Notify(1)) return E_ABORT;
    auto* events = new Events(s);
    DWORD cookie;
    HRESULT hr = s.dialog->Advise(events, &cookie);
    events->Release();
    if (FAILED(hr)) return hr;
    auto* owner = s.window->GetParent();
    // Initial folder events may precede creation of the custom control HWNDs.
    const auto active = std::make_shared<bool>(true);
    wxTheApp->CallAfter([&s, active] {
        if (!*active) return;
        const HRESULT names = s.NameControls();
        if (FAILED(names)) s.dialog->Close(names);
    });
    hr = s.dialog->Show(owner ? static_cast<HWND>(owner->GetHandle()) : nullptr);
    *active = false;
    s.dialog->Unadvise(cookie);
    if (hr == HRESULT_FROM_WIN32(ERROR_CANCELLED)) return wxID_CANCEL;
    if (FAILED(hr)) return hr;
    UINT filter = 1;
    if (SUCCEEDED(s.dialog->GetFileTypeIndex(&filter))) s.filter = static_cast<int>(filter) - 1;
    IFileOpenDialog* open = nullptr;
    if (SUCCEEDED(s.dialog->QueryInterface(IID_PPV_ARGS(&open)))) {
        IShellItemArray* results = nullptr;
        hr = open->GetResults(&results);
        open->Release();
        if (FAILED(hr)) return hr;
        DWORD count = 0;
        hr = results->GetCount(&count);
        if (FAILED(hr) || !count) { results->Release(); return FAILED(hr) ? hr : E_UNEXPECTED; }
        for (DWORD i = 0; i < count; ++i) {
            IShellItem* item = nullptr;
            hr = results->GetItemAt(i, &item);
            if (FAILED(hr)) break;
            PWSTR path = nullptr;
            hr = item->GetDisplayName(SIGDN_FILESYSPATH, &path);
            item->Release();
            if (FAILED(hr)) break;
            s.paths.emplace_back(path);
            CoTaskMemFree(path);
        }
        results->Release();
    } else {
        IShellItem* item = nullptr;
        hr = s.dialog->GetResult(&item);
        if (SUCCEEDED(hr)) {
            PWSTR path = nullptr;
            hr = item->GetDisplayName(SIGDN_FILESYSPATH, &path);
            item->Release();
            if (SUCCEEDED(hr)) { s.paths.emplace_back(path); CoTaskMemFree(path); }
        }
    }
    if (FAILED(hr)) return hr;
    if (!s.paths.empty()) {
        if (s.folders) static_cast<wxDirDialog*>(s.window)->SetPath(s.paths.front());
        else static_cast<wxFileDialog*>(s.window)->SetPath(s.paths.front());
        if (s.window->HasFlag(s.folders ? wxDD_CHANGE_DIR : wxFD_CHANGE_DIR))
            wxSetWorkingDirectory(s.folders ? s.paths.front() : wxPathOnly(s.paths.front()));
    }
    return wxID_OK;
}
int wxsharp_item_dialog_count(wxsharp_handle context) { return static_cast<int>(static_cast<ItemDialog*>(context)->paths.size()); }
int wxsharp_item_dialog_filter(wxsharp_handle context) {
    auto& s = *static_cast<ItemDialog*>(context);
    UINT index = 1;
    if (SUCCEEDED(s.dialog->GetFileTypeIndex(&index))) s.filter = static_cast<int>(index) - 1;
    return s.filter;
}
int wxsharp_item_dialog_text(wxsharp_handle context, int kind, int index, char* buffer, int length) {
    auto& s = *static_cast<ItemDialog*>(context);
    wxString value;
    if (kind == 0 && index >= 0 && static_cast<size_t>(index) < s.paths.size()) value = s.paths[index];
    else if (kind == 1) value = s.Current();
    else if (kind == 2) {
        PWSTR text = nullptr;
        HRESULT hr = s.custom->GetEditBoxText(index, &text);
        if (FAILED(hr)) return hr;
        value = text;
        CoTaskMemFree(text);
    }
    return CopyToBuffer(value, buffer, length);
}
int wxsharp_item_dialog_add(wxsharp_handle context, int kind, const char* text) {
    auto& s = *static_cast<ItemDialog*>(context);
    const DWORD id = s.next++;
    if (kind != 3) s.radio = 0;
    const wxString label = Str(text);
    HRESULT hr = E_INVALIDARG;
    DWORD group = 0;
    DWORD labelGroup = 0;
    if ((kind == 4 || kind == 5) && !label.empty()) {
        // Shell choice/edit labels need a visual group; SetControlLabel alone does not render them.
        labelGroup = s.next++;
        hr = s.custom->StartVisualGroup(labelGroup, label.wc_str());
        if (FAILED(hr)) return hr;
    }
    switch (kind) {
        case 1: hr = s.custom->AddPushButton(id, label.wc_str()); break;
        case 2: hr = s.custom->AddCheckButton(id, label.wc_str(), FALSE); break;
        case 3:
            if (!s.radio) { s.radio = s.next++; hr = s.custom->AddRadioButtonList(s.radio); if (FAILED(hr)) return hr; }
            group = s.radio;
            hr = s.custom->AddControlItem(group, id, label.wc_str());
            break;
        case 4: hr = s.custom->AddComboBox(id); break;
        case 5: hr = s.custom->AddEditBox(id, L""); break;
        case 6: hr = s.custom->AddText(id, label.wc_str()); break;
    }
    if (labelGroup) {
        const HRESULT end = s.custom->EndVisualGroup();
        if (SUCCEEDED(hr)) hr = end;
    }
    if (FAILED(hr)) return hr;
    s.controls.emplace(id, Control{kind, group, {}, -1, labelGroup, label});
    return static_cast<int>(id);
}
int wxsharp_item_dialog_items(wxsharp_handle context, int id, const char* const* items, int count) {
    auto& s = *static_cast<ItemDialog*>(context);
    auto& control = s.controls.at(id);
    for (DWORD item : control.items) {
        HRESULT hr = s.custom->RemoveControlItem(id, item);
        if (FAILED(hr)) return hr;
    }
    control.items.clear();
    control.selection = -1;
    for (int i = 0; i < count; ++i) {
        DWORD item = s.next++;
        wxString label = Str(items[i]);
        HRESULT hr = s.custom->AddControlItem(id, item, label.wc_str());
        if (FAILED(hr)) return hr;
        control.items.push_back(item);
    }
    return S_OK;
}
int wxsharp_item_dialog_number(wxsharp_handle context, int id, int property, bool write, int* value) {
    auto& s = *static_cast<ItemDialog*>(context);
    auto& control = s.controls.at(id);
    if (property == 1 || (control.kind == 3 && property == 2)) {
        DWORD target = control.group ? control.group : static_cast<DWORD>(id);
        if (write) {
            if (control.group) return *value ? s.custom->SetSelectedControlItem(target, id) : E_INVALIDARG;
            if (*value < 0 || static_cast<size_t>(*value) >= control.items.size()) return E_INVALIDARG;
            HRESULT hr = s.custom->SetSelectedControlItem(target, control.items[*value]);
            if (SUCCEEDED(hr)) control.selection = *value;
            return hr;
        }
        DWORD selected = 0;
        HRESULT hr = s.custom->GetSelectedControlItem(target, &selected);
        // Windows also returns E_FAIL for a choice whose items were replaced without selecting one.
        if (hr == E_UNEXPECTED || (hr == E_FAIL && control.selection == -1)) {
            *value = control.group ? 0 : -1;
            return S_OK;
        }
        if (FAILED(hr)) return hr;
        if (control.group) *value = selected == static_cast<DWORD>(id);
        else {
            auto it = std::find(control.items.begin(), control.items.end(), selected);
            *value = it == control.items.end() ? -1 : static_cast<int>(it - control.items.begin());
        }
        return S_OK;
    }
    if (property == 2) {
        if (write) return s.custom->SetCheckButtonState(id, *value != 0);
        BOOL checked = FALSE;
        HRESULT hr = s.custom->GetCheckButtonState(id, &checked);
        *value = checked;
        return hr;
    }
    CDCONTROLSTATEF flags;
    const DWORD target = control.label ? control.label : static_cast<DWORD>(id);
    HRESULT hr = control.group ? s.custom->GetControlItemState(control.group, id, &flags) : s.custom->GetControlState(target, &flags);
    if (FAILED(hr)) return hr;
    const DWORD bit = property == 3 ? CDCS_VISIBLE : CDCS_ENABLED;
    if (!write) { *value = (flags & bit) != 0; return S_OK; }
    flags = static_cast<CDCONTROLSTATEF>(*value ? flags | bit : flags & ~bit);
    return control.group ? s.custom->SetControlItemState(control.group, id, flags) : s.custom->SetControlState(target, flags);
}
int wxsharp_item_dialog_set_text(wxsharp_handle context, int id, const char* text) {
    auto& s = *static_cast<ItemDialog*>(context);
    wxString value = Str(text);
    return s.controls.at(id).kind == 5 ? s.custom->SetEditBoxText(id, value.wc_str()) : s.custom->SetControlLabel(id, value.wc_str());
}
#else
wxsharp_handle wxsharp_item_dialog_create(wxsharp_handle, bool, wxsharp_item_dialog_cb, long long) { return nullptr; }
void wxsharp_item_dialog_destroy(wxsharp_handle) {}
void wxsharp_item_dialog_close(wxsharp_handle) {}
int wxsharp_item_dialog_show(wxsharp_handle) { return -1; }
int wxsharp_item_dialog_count(wxsharp_handle) { return 0; }
int wxsharp_item_dialog_filter(wxsharp_handle) { return 0; }
int wxsharp_item_dialog_text(wxsharp_handle, int, int, char*, int) { return 0; }
int wxsharp_item_dialog_add(wxsharp_handle, int, const char*) { return -1; }
int wxsharp_item_dialog_items(wxsharp_handle, int, const char* const*, int) { return -1; }
int wxsharp_item_dialog_number(wxsharp_handle, int, int, bool, int*) { return -1; }
int wxsharp_item_dialog_set_text(wxsharp_handle, int, const char*) { return -1; }
#endif
