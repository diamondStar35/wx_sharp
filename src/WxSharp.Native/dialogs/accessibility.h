#pragma once
#ifdef __WXMSW__
#include <oleacc.h>
#include <shobjidl.h>
#include <set>

// The shell draws group captions but leaves their hosted choice/edit HWNDs unnamed.
// Direct annotation names those standard controls without replacing their providers.
class DialogAccessibility {
public:
    ~DialogAccessibility();
    HRESULT Apply(IFileDialog* dialog, const std::vector<wxString>& labels);
private:
    HRESULT Walk(IAccessible* element, const std::vector<wxString>& labels,
                 const wxString& inherited, int depth);
    IAccPropServices* properties = nullptr;
    std::set<HWND> named;
};
#endif
