#ifdef _WIN32
#include <initguid.h>
#endif
#include "../internal.h"
#include "accessibility.h"

#ifdef __WXMSW__
DialogAccessibility::~DialogAccessibility() {
    if (!properties) return;
    const MSAAPROPID name = PROPID_ACC_NAME;
    for (HWND window : named) {
        if (!IsWindow(window)) continue;
        const HRESULT hr = properties->ClearHwndProps(window, static_cast<DWORD>(OBJID_CLIENT), CHILDID_SELF, &name, 1);
        if (FAILED(hr)) wxLogApiError(wxS("IAccPropServices::ClearHwndProps"), hr);
    }
    properties->Release();
}

HRESULT DialogAccessibility::Apply(IFileDialog* dialog, const std::vector<wxString>& labels) {
    if (labels.empty() || named.size() >= labels.size()) return S_OK;
    if (!properties) {
        const HRESULT hr = CoCreateInstance(CLSID_AccPropServices, nullptr, CLSCTX_INPROC_SERVER,
                                            IID_PPV_ARGS(&properties));
        if (FAILED(hr)) return hr;
    }
    IOleWindow* native = nullptr;
    HRESULT hr = dialog->QueryInterface(IID_PPV_ARGS(&native));
    if (FAILED(hr)) return hr;
    HWND window = nullptr;
    hr = native->GetWindow(&window);
    native->Release();
    if (FAILED(hr) || !window) return S_FALSE; // Initial folder events can precede HWND creation.
    IAccessible* root = nullptr;
    hr = AccessibleObjectFromWindow(window, static_cast<DWORD>(OBJID_CLIENT), IID_PPV_ARGS(&root));
    if (FAILED(hr)) return hr;
    hr = Walk(root, labels, {}, 0);
    root->Release();
    return hr;
}

HRESULT DialogAccessibility::Walk(IAccessible* element, const std::vector<wxString>& labels,
                                  const wxString& inherited, int depth) {
    if (depth > 32 || named.size() >= labels.size()) return S_OK;
    VARIANT self{};
    self.vt = VT_I4;
    self.lVal = CHILDID_SELF;
    VARIANT role{};
    HRESULT hr = element->get_accRole(self, &role);
    if (FAILED(hr)) return hr;
    const long kind = role.vt == VT_I4 ? role.lVal : 0;
    VariantClear(&role);
    wxString label = inherited;
    if (kind == ROLE_SYSTEM_GROUPING) {
        BSTR name = nullptr;
        hr = element->get_accName(self, &name);
        if (FAILED(hr)) return hr;
        const wxString caption(name ? name : L"");
        SysFreeString(name);
        if (std::find(labels.begin(), labels.end(), caption) != labels.end()) label = caption;
    }
    if (!label.empty() && (kind == ROLE_SYSTEM_COMBOBOX || kind == ROLE_SYSTEM_TEXT)) {
        HWND window = nullptr;
        hr = WindowFromAccessibleObject(element, &window);
        if (FAILED(hr)) return hr;
        wchar_t type[32]{};
        if (window && GetClassNameW(window, type, 32) &&
            (_wcsicmp(type, L"ComboBox") == 0 || _wcsicmp(type, L"Edit") == 0)) {
            hr = properties->SetHwndPropStr(window, static_cast<DWORD>(OBJID_CLIENT), CHILDID_SELF, PROPID_ACC_NAME, label.wc_str());
            if (FAILED(hr)) return hr;
            named.insert(window);
        }
        return S_OK;
    }
    // Skip filesystem entries. The shell also uses an outline for the custom control area itself.
    if (kind == ROLE_SYSTEM_LIST || kind == ROLE_SYSTEM_OUTLINEITEM || kind == ROLE_SYSTEM_TABLE ||
        kind == ROLE_SYSTEM_MENUBAR || kind == ROLE_SYSTEM_MENUPOPUP)
        return S_OK;
    long count = 0;
    hr = element->get_accChildCount(&count);
    if (FAILED(hr)) return hr;
    for (long start = 0; start < count && named.size() < labels.size(); start += 32) {
        VARIANT children[32]{};
        long obtained = 0;
        hr = AccessibleChildren(element, start, std::min(32L, count - start), children, &obtained);
        if (FAILED(hr)) return hr;
        for (long index = 0; index < obtained; ++index) {
            if (SUCCEEDED(hr) && children[index].vt == VT_DISPATCH && children[index].pdispVal) {
                IAccessible* child = nullptr;
                const HRESULT query = children[index].pdispVal->QueryInterface(IID_PPV_ARGS(&child));
                if (SUCCEEDED(query)) {
                    hr = Walk(child, labels, label, depth + 1);
                    child->Release();
                } else if (query != E_NOINTERFACE) hr = query;
            }
            VariantClear(&children[index]);
        }
        if (FAILED(hr)) return hr;
    }
    return S_OK;
}
#endif
