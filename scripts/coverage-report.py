#!/usr/bin/env python3
"""Diffs the WxSharp public surface against the wxWidgets headers it wraps.

Prose parity claims cannot be checked. This walks every managed type that wraps a wx class, reads the public
member functions of that class (and its wx base classes) out of the real headers, and reports what the
wrapper does not expose yet.

It is a starting point for judgement, not a to-do list: plenty of what it reports is deliberately absent -
two-step construction, deprecated calls, MSW internals, overloads the wrapper collapses into one method.
Those live in SKIP and COLLAPSE below, so the output stays worth reading.

    python scripts/coverage-report.py            # summary table
    python scripts/coverage-report.py --detail   # every missing member, by type
    python scripts/coverage-report.py --type ListCtrl
"""
import argparse
import io
import os
import re
import sys
from collections import OrderedDict

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
INCLUDE = os.path.join(ROOT, 'third-party', 'Windows', 'include')
MANAGED = os.path.join(ROOT, 'src', 'WxSharp')

# managed type -> (wx class, its wx base classes worth counting)
TYPES = OrderedDict([
    ('Window',             ('wxWindow', ['wxWindowBase'])),
    ('Frame',              ('wxFrame', ['wxFrameBase', 'wxTopLevelWindowBase'])),
    ('Dialog',             ('wxDialog', ['wxDialogBase'])),
    ('Panel',              ('wxPanel', ['wxPanelBase'])),
    ('ScrolledWindow',     ('wxScrolled', ['wxScrollHelper', 'wxScrollHelperBase'])),
    ('SplitterWindow',     ('wxSplitterWindow', [])),
    ('Notebook',           ('wxNotebook', ['wxBookCtrlBase', 'wxNotebookBase'])),
    ('SimpleBook',         ('wxSimplebook', ['wxBookCtrlBase'])),
    ('Button',             ('wxButton', ['wxButtonBase', 'wxAnyButtonBase'])),
    ('BitmapButton',       ('wxBitmapButton', ['wxBitmapButtonBase'])),
    ('ToggleButton',       ('wxToggleButton', ['wxToggleButtonBase'])),
    ('CheckBox',           ('wxCheckBox', ['wxCheckBoxBase'])),
    ('RadioButton',        ('wxRadioButton', ['wxRadioButtonBase'])),
    ('RadioBox',           ('wxRadioBox', ['wxRadioBoxBase'])),
    ('Choice',             ('wxChoice', ['wxChoiceBase', 'wxItemContainer', 'wxItemContainerImmutable'])),
    ('ComboBox',           ('wxComboBox', ['wxComboBoxBase', 'wxTextEntryBase', 'wxItemContainer'])),
    ('ListBox',            ('wxListBox', ['wxListBoxBase', 'wxItemContainer'])),
    ('CheckListBox',       ('wxCheckListBox', ['wxCheckListBoxBase'])),
    ('TextCtrl',           ('wxTextCtrl', ['wxTextCtrlBase', 'wxTextEntryBase', 'wxTextAreaBase'])),
    ('StaticText',         ('wxStaticText', ['wxStaticTextBase'])),
    ('StaticBox',          ('wxStaticBox', ['wxStaticBoxBase'])),
    ('StaticLine',         ('wxStaticLine', ['wxStaticLineBase'])),
    ('StaticBitmap',       ('wxStaticBitmap', ['wxStaticBitmapBase'])),
    ('Slider',             ('wxSlider', ['wxSliderBase'])),
    ('Gauge',              ('wxGauge', ['wxGaugeBase'])),
    ('SpinCtrl',           ('wxSpinCtrl', ['wxSpinCtrlBase'])),
    ('SpinCtrlDouble',     ('wxSpinCtrlDouble', [])),
    ('ScrollBar',          ('wxScrollBar', ['wxScrollBarBase'])),
    ('SearchCtrl',         ('wxSearchCtrl', ['wxSearchCtrlBase'])),
    ('HyperlinkCtrl',      ('wxHyperlinkCtrl', ['wxHyperlinkCtrlBase'])),
    ('ActivityIndicator',  ('wxActivityIndicator', ['wxActivityIndicatorBase'])),
    ('DatePickerCtrl',     ('wxDatePickerCtrl', ['wxDatePickerCtrlBase', 'wxDateTimePickerCtrl'])),
    ('TimePickerCtrl',     ('wxTimePickerCtrl', ['wxTimePickerCtrlBase'])),
    ('ListCtrl',           ('wxListCtrl', [])),
    ('TreeCtrl',           ('wxTreeCtrl', ['wxTreeCtrlBase'])),
    ('Grid',               ('wxGrid', [])),
    ('DataViewListCtrl',   ('wxDataViewListCtrl', ['wxDataViewCtrlBase'])),
    ('DataViewTreeCtrl',   ('wxDataViewTreeCtrl', [])),
    ('Menu',               ('wxMenu', ['wxMenuBase'])),
    ('MenuItem',           ('wxMenuItem', ['wxMenuItemBase'])),
    ('MenuBar',            ('wxMenuBar', ['wxMenuBarBase'])),
    ('ToolBar',            ('wxToolBar', ['wxToolBarBase'])),
    ('StatusBar',          ('wxStatusBar', ['wxStatusBarBase'])),
    ('Sizer',              ('wxSizer', [])),
    ('BoxSizer',           ('wxBoxSizer', [])),
    ('GridSizer',          ('wxGridSizer', [])),
    ('FlexGridSizer',      ('wxFlexGridSizer', [])),
    ('GridBagSizer',       ('wxGridBagSizer', [])),
    ('StaticBoxSizer',     ('wxStaticBoxSizer', [])),
    ('Timer',              ('wxTimer', ['wxTimerBase'])),
    ('Clipboard',          ('wxClipboard', ['wxClipboardBase'])),
    ('Locale',             ('wxLocale', [])),
    ('Translations',       ('wxTranslations', [])),
    ('LanguageInfo',       ('wxLanguageInfo', [])),
    ('ProgressDialog',     ('wxProgressDialog', ['wxGenericProgressDialog'])),
    ('Font',               ('wxFont', ['wxFontBase'])),
    ('Colour',             ('wxColour', ['wxColourBase'])),
    ('Bitmap',             ('wxBitmap', ['wxBitmapBase'])),
    ('Image',              ('wxImage', [])),
])

# Members no wrapper should carry: C++ lifetime and construction machinery, MSW plumbing, deprecated calls,
# and the virtual hooks a subclass overrides rather than a caller invokes.
SKIP = {
    'Create', 'Init', 'Destroy', 'CreateBase', 'SendDestroyEvent',
    'GetClassInfo', 'GetClassName', 'IsKindOf', 'Ref', 'UnRef', 'UnShare', 'CloneRefData', 'CreateRefData',
    'Ok',   # the long-deprecated spelling of IsOk
    'GetRefData', 'SetRefData', 'IsSameAs',
    'MSWGetStyle', 'MSWOnDraw', 'MSWOnMeasure', 'MSWWindowProc', 'MSWCommand', 'MSWOnNotify',
    'MSWGetBgBrush', 'MSWGetBgBrushForChild', 'MSWShouldPreProcessMessage', 'MSWClickButtonIfPossible',
    'MSWHandleMessage', 'MSWDoPopupMenu', 'MSWGetContainer', 'MSWDisableComposited', 'MSWDoAdjustRect',
    'MSWUpdateFontOnDPIChange', 'MSWEnableHWND', 'MSWGetParent', 'MSWSetTabStop',
    'DoGetBestSize', 'DoGetBestClientSize', 'DoSetSize', 'DoMoveWindow', 'DoGetSize', 'DoGetPosition',
    'DoGetClientSize', 'DoSetClientSize', 'DoSetSizeHints', 'DoGetTextExtent', 'DoEnable', 'DoFreeze',
    'DoThaw', 'DoCaptureMouse', 'DoReleaseMouse', 'DoScreenToClient', 'DoClientToScreen', 'DoPopupMenu',
    'DoSetToolTip', 'DoGetBorderSize', 'DoSetWindowVariant', 'DoCentre', 'DoSetSizer',
    'ProcessEvent', 'ProcessEventLocally', 'SafelyProcessEvent', 'ProcessPendingEvents', 'QueueEvent',
    'AddPendingEvent', 'DeletePendingEvents', 'Connect', 'Disconnect', 'Bind', 'Unbind',
    'TryBefore', 'TryAfter', 'TryHandleInHierarchy', 'GetEventHashTable', 'SearchEventTable',
    'GetEventTable', 'SetNextHandler', 'SetPreviousHandler', 'GetNextHandler', 'GetPreviousHandler',
    'SetEvtHandlerEnabled', 'GetEvtHandlerEnabled', 'IsUnlinked', 'Unlink',
    'operator', 'GetHandle', 'AssociateHandle', 'DissociateHandle',
    'CreateLanguagesDB', 'DestroyLanguagesDB', 'AddLanguage', 'SetLoader', 'GetUntranslatedString',
    'DoCommonInit', 'DoInit', 'DoCommonPostInit', 'GetSystemEncoding',
    'OnChar', 'OnContextMenu', 'OnCopy', 'OnCut', 'OnDelete', 'OnPaste', 'OnRedo', 'OnUndo',
    'OnSelectAll', 'OnSetFocus', 'OnDropFiles',
    'OnUpdateCopy', 'OnUpdateCut', 'OnUpdateDelete', 'OnUpdatePaste', 'OnUpdateRedo', 'OnUpdateUndo',
    'OnUpdateSelectAll',
    'AdoptAttributesFromHWND', 'ForwardEnableTextChangedEvents', 'SendTextUpdatedEvent',
    'SendTextUpdatedEventIfAllowed', 'SuppressTextChangedEvents', 'ResumeTextChangedEvents',
    'EventsAllowed', 'EventsSuppressor', 'GetRichVersion', 'IsInkEdit', 'HideNativeCaret',
    'ShowNativeCaret', 'GetCompositeControlsDefaultAttributes',
    'GetValidator', 'SetValidator', 'Validate', 'TransferDataToWindow', 'TransferDataFromWindow',
    'InitDialog', 'OnInternalIdle', 'OnPaint', 'OnEraseBackground', 'OnSysColourChanged', 'OnIdle',
    'SetConstraints', 'GetConstraints', 'SetAutoLayout', 'GetAutoLayout', 'SetSizeConstraint',
    'LayoutPhase1', 'LayoutPhase2', 'DoPhase', 'ResetConstraints', 'SetConstraintSizes',
    'GetSizeAvailableForScrollTarget', 'SendIdleEvents', 'UpdateWindowUI', 'DoUpdateWindowUI',
    'GetUpdateRegion', 'GetUpdateClientRect', 'SetInitialBestSize', 'CacheBestSize', 'InvalidateBestSize',
    'AdjustForLayoutDirection', 'GetContentScaleFactor', 'GetDPIScaleFactor',
    'AddChild', 'RemoveChild', 'DestroyChildren', 'GetChildren', 'SetParent', 'Reparent',
    'PushEventHandler', 'PopEventHandler', 'RemoveEventHandler', 'GetEventHandler', 'SetEventHandler',
    'FindWindow', 'FindWindowById', 'FindWindowByName', 'FindWindowByLabel', 'FindFocus', 'GetCapture',
    'NewControlId', 'UnreserveControlId', 'GetTopLevelParent', 'GetGrandParent',
    # Layout constraints: superseded by sizers, and not wrapped anywhere.
    'AddConstraintReference', 'RemoveConstraintReference', 'DeleteRelatedConstraints',
    'GetConstraintsInvolvedIn', 'GetClientSizeConstraint', 'GetPositionConstraint', 'GetSizeConstraint',
    'MoveConstraint', 'UnsetConstraints', 'SetInitialSize',
    # Dialog layout adaptation: a wx feature for small screens, driven entirely by wx itself.
    'CanDoLayoutAdaptation', 'EnableLayoutAdaptation', 'GetLayoutAdaptationDone',
    'GetLayoutAdaptationLevel', 'GetLayoutAdaptationMode', 'GetLayoutAdapter',
    'IsLayoutAdaptationEnabled', 'SetLayoutAdaptationDone', 'SetLayoutAdaptationLevel',
    'SetLayoutAdaptationMode', 'SetLayoutAdapter', 'AddMainButtonId', 'GetMainButtonIds',
    'IsMainButtonId', 'GetContentWindow', 'GetParentForModalDialog', 'GetParentForModelessDialog',
    # Event-handler entry points a wx subclass overrides, not something a caller invokes.
    'OnActivate', 'OnCloseWindow', 'OnSize', 'OnMenuOpen', 'OnMenuClose', 'OnMenuHighlight',
    'OnCreateStatusBar', 'OnCreateToolBar', 'OnMiddleClick', 'OnHelp', 'OnInitDialog',
    'HandleCommand', 'HandleMenuSelect', 'HandleSize', 'ProcessCommand', 'New',
    'IsClientAreaChild', 'GetRectForTopLevelChildren', 'GetScrollHelper', 'SetScrollHelper',
    'GetMainWindowOfCompositeControl', 'InformFirstDirection', 'GetMinSizeFromKnownDirection',
    'SetContainingSizer', 'GetContainingSizer', 'AsWindow', 'IsBeingDeleted', 'GetClassDefaultAttributes',
    'GetDefaultAttributes', 'InheritAttributes', 'ShouldInheritColours', 'InheritsBackgroundColour',
    'InheritsForegroundColour', 'UseBackgroundColour', 'UseBgCol', 'UseForegroundColour',
    'SetOwnBackgroundColour', 'SetOwnForegroundColour', 'SetOwnFont', 'CopyToolTip', 'GetToolTipCtrl',
    'SetToolTipCtrl', 'ChildrenRepositioningGuard', 'BeginRepositioningChildren',
    'EndRepositioningChildren', 'GetTmpDefaultItem', 'SetTmpDefaultItem', 'ShouldPreventAppExit',
    'CanBeOutsideClientArea', 'CanApplyThemeBorder', 'IsTopNavigationDomain', 'HasMultiplePages',
    'AlwaysShowScrollbars', 'IsScrollbarAlwaysShown', 'SaveField', 'RestoreField', 'SaveValue',
    'RestoreValue', 'IsTopLevel', 'IsDescendant',
    # Layout internals and a lookup keyed on wxObject user data, which the wrapper does not expose.
    'UpdateOnDPIChange', 'SetContainingWindow', 'FindItemWithData',
    # Need a wrapped type the project does not have yet: wxCaret, wxCursor, wxDropTarget, wxPalette,
    # wxAcceleratorTable as an object. Tracked in the parity document rather than silently skipped.
    'GetCaret', 'SetCaret', 'GetCursor', 'SetCursor', 'GetCursorBundle', 'SetCursorBundle',
    'GetDropTarget', 'SetDropTarget', 'GetPalette', 'SetPalette', 'HasCustomPalette',
    'GetAncestorWithCustomPalette', 'GetAcceleratorTable',
    # Protected platform hooks which Doxygen places in a public-looking section but Phoenix doesn't expose.
    'CreateLboxItem', 'CreateCloseButton', 'GetNormalState', 'EmulateKeyPress',
    # Parser noise from ownership annotations, not an API member.
    'Free',
    # Layout direction, physical-pixel conversion and touch are platform plumbing.
    'GetLayoutDirection', 'SetLayoutDirection', 'FromPhys', 'ToPhys', 'MakeDPIFromScaleFactor',
    'EnableTouchEvents', 'CanScroll', 'IsExposed', 'GetClientAreaOrigin', 'GetBestHeight',
    'GetBestWidth', 'ClientToWindowSize', 'WindowToClientSize', 'HasTransparentBackground',
    'IsTransparentBackgroundSupported', 'PrepareDC', 'PrepareReadOnlyDC',
    # Posting an event needs the wx.PostEvent equivalent, which is not wrapped.
    'PostSizeEvent', 'PostSizeEventToParent', 'SendSizeEvent', 'SendSizeEventToParent',
    # The help-provider machinery, which needs wxHelpProvider.
    'GetHelpIdAtPoint', 'GetHelpTextAtPoint', 'SetHelpTextForId',
}

# wx names the wrapper deliberately answers with one differently-named member. Left side is the wx member.
COLLAPSE = {
    'SetSize': 'Size', 'GetSize': 'Size', 'SetClientSize': 'ClientSize', 'GetClientSize': 'ClientSize',
    'SetPosition': 'Position', 'GetPosition': 'Position', 'Move': 'Position',
    'SetLabel': 'Label', 'GetLabel': 'Label', 'SetLabelText': 'Label', 'GetLabelText': 'Label',
    'SetValue': 'Value', 'GetValue': 'Value', 'SetTitle': 'Title', 'GetTitle': 'Title',
    'SetSelection': 'SelectedIndex', 'GetSelection': 'SelectedIndex',
    'SetStringSelection': 'SelectedIndex', 'GetStringSelection': 'SelectedIndex',
    'Enable': 'Enabled', 'IsEnabled': 'Enabled', 'Disable': 'Enabled',
    'Show': 'Visible', 'Hide': 'Visible', 'IsShown': 'Visible', 'IsShownOnScreen': 'Visible',
    'SetFocus': 'Focus', 'HasFocus': 'HasFocus', 'SetFocusFromKbd': 'Focus',
    'SetToolTip': 'ToolTip', 'GetToolTip': 'ToolTip', 'GetToolTipText': 'ToolTip', 'UnsetToolTip': 'ToolTip',
    'SetFont': 'SetFont', 'GetFont': 'SetFont',
    'SetBackgroundColour': 'BackgroundColour', 'GetBackgroundColour': 'BackgroundColour',
    'SetForegroundColour': 'ForegroundColour', 'GetForegroundColour': 'ForegroundColour',
    'SetMinSize': 'MinSize', 'GetMinSize': 'MinSize', 'SetSizeHints': 'MinSize',
    'SetMaxSize': 'MaxSize', 'GetMaxSize': 'MaxSize',
    'GetBestSize': 'BestSize', 'GetEffectiveMinSize': 'BestSize',
    'SetName': 'Name', 'GetName': 'Name',
    'GetAccessible': 'Accessible', 'SetAccessible': 'Accessible', 'GetOrCreateAccessible': 'Accessible',
    'CreateAccessible': 'Accessible',
    'SetId': 'Id', 'GetId': 'Id', 'GetParent': 'Parent',
    'Append': 'Add', 'Insert': 'Insert', 'Delete': 'RemoveAt', 'Clear': 'Clear',
    'GetCount': 'Count', 'IsEmpty': 'Count', 'GetString': 'this[]', 'SetString': 'this[]',
    'FindString': 'IndexOf',
    'GetItemCount': 'Count', 'GetColumnCount': 'ColumnCount',
    'SetItemText': 'SetItem', 'GetItemText': 'GetItem',
    'Refresh': 'Refresh', 'RefreshRect': 'Refresh', 'Update': 'Refresh',
    'Layout': 'Layout', 'Fit': 'Fit', 'FitInside': 'Fit',
    'Centre': 'Center', 'Center': 'Center', 'CentreOnParent': 'Center', 'CenterOnParent': 'Center',
    'Close': 'Close', 'SetSizer': 'SetSizer', 'GetSizer': 'SetSizer', 'SetSizerAndFit': 'SetSizer',
    'GetRange': 'Range', 'SetRange': 'SetRange', 'GetMin': 'Minimum', 'GetMax': 'Maximum',
    'ShowModal': 'ShowModal', 'EndModal': 'EndModal',
    'SetWindowStyleFlag': 'style', 'GetWindowStyleFlag': 'style',
    'SetWindowStyle': 'style', 'GetWindowStyle': 'style', 'HasFlag': 'style',
}

# C++ keywords, wx macros and helper types the naive member regex picks up out of expressions.
NOISE = {'if', 'for', 'while', 'switch', 'return', 'sizeof', 'catch', 'throw', 'new', 'delete',
         'static_cast', 'const_cast', 'reinterpret_cast', 'dynamic_cast', 'typeid', 'decltype',
         'wxT', 'wxS', 'wxCHECK', 'wxASSERT', 'wxFAIL', 'wxCAST_TO_LONG'}


def is_api(member, wx_name, bases):
    """True when a parsed name looks like a real public API member rather than parser noise."""
    if member in SKIP or member in NOISE:
        return False
    if member == wx_name or member in bases:
        return False
    if not member[:1].isupper():
        return False          # locals, keywords and helper functions
    if member.startswith(('MSW', 'Do', 'OS', 'GTK', 'WX', 'Calc', 'Reposition', 'Recalc')):
        return False          # platform internals and the layout hooks a subclass overrides
    if re.match(r'^wx[A-Z]', member):
        return False          # type and macro names
    return True


# Where one managed type answers a wx member under a name of its own. Keyed by managed type so the same
# wx name can mean different things on different controls.
# Managed base classes whose members a derived wrapper inherits, where the derivation mirrors wxWidgets'.
MANAGED_BASES = {
    'CheckListBox': ('ListBox',),
    'ProgressDialog': ('Window',),
    'ScrolledWindow': ('Window',),
    'DatePickerCtrl': ('DateTimePickerBase',),
    'TimePickerCtrl': ('DateTimePickerBase',),
}

# Public-looking declarations from platform headers which Phoenix's Doxygen/ETG interface does not expose
# for this particular type. These cannot be global skips because another control may legitimately expose
# a member with the same name.
PER_TYPE_SKIP = {
    'Font': {'IsFree', 'RealizeResource', 'GetResourceHandle', 'FreeResource', 'GetHFONT',
             'ConvertFromLegacyWeightIfNecessary'},
    'SpinCtrl': {'ContainsHWND', 'GetBuddyWndProc', 'GetSpinForTextCtrl', 'ProcessTextCommand',
                 'GetSnapToTicks', 'SetSnapToTicks'},
    'SpinCtrlDouble': {'GetBase', 'SetBase'},
    'ScrollBar': {'IsNeeded', 'SetPageSize', 'SetRange', 'SetThumbSize'},
    'HyperlinkCtrl': {'SendEvent'},
}

PER_TYPE = {
    'Font': {'GetPointSize': 'PointSize', 'SetPointSize': 'PointSize',
             'GetFractionalPointSize': 'FractionalPointSize', 'SetFractionalPointSize': 'FractionalPointSize',
             'GetPixelSize': 'PixelSize', 'SetPixelSize': 'PixelSize',
             'GetFamily': 'Family', 'SetFamily': 'Family',
             'GetStyle': 'Style', 'SetStyle': 'Style',
             'GetNumericWeight': 'NumericWeight', 'SetNumericWeight': 'NumericWeight',
             'GetWeight': 'Weight', 'SetWeight': 'Weight',
             'GetUnderlined': 'IsUnderlined', 'SetUnderlined': 'IsUnderlined',
             'GetStrikethrough': 'IsStrikethrough', 'SetStrikethrough': 'IsStrikethrough',
             'GetEncoding': 'Encoding', 'SetEncoding': 'Encoding',
             'GetFaceName': 'FaceName', 'SetFaceName': 'TrySetFaceName',
             'GetNativeFontInfoDesc': 'NativeFontInfo', 'SetNativeFontInfo': 'NativeFontInfo',
             'GetNativeFontInfo': 'NativeFontInfo',
             'GetNativeFontInfoUserDesc': 'NativeFontInfoUserDesc',
             'SetNativeFontInfoUserDesc': 'NativeFontInfoUserDesc',
             'GetFamilyString': 'FamilyString', 'GetStyleString': 'StyleString',
             'GetWeightString': 'WeightString',
             'GetDefaultEncoding': 'DefaultEncoding', 'SetDefaultEncoding': 'DefaultEncoding',
             'IsUsingSizeInPixels': 'IsUsingSizeInPixels', 'IsFixedWidth': 'IsFixedWidth',
             'SetSymbolicSize': 'SetSymbolicSize', 'SetSymbolicSizeRelativeTo': 'SetSymbolicSizeRelativeTo'},
    'ProgressDialog': {'GetRange': 'Range', 'SetRange': 'Range', 'GetValue': 'Value',
                       'GetMessage': 'Update'},
    'ScrolledWindow': {'GetScrollPixelsPerUnit': 'ScrollPixelsPerUnit', 'GetViewStart': 'ViewStart',
                       'SetScrollRate': 'SetScrollRate'},
    'ListBox': {'SetString': 'SetString', 'GetString': 'GetString'},
    'TreeCtrl': {'GetItemText': 'GetText', 'SetItemText': 'SetText', 'GetCount': 'Count',
                 'GetSelection': 'Selection', 'SelectItem': 'Selection'},
    'ListCtrl': {'ClearAll': 'ClearAll', 'GetItemData': 'GetItemData', 'SetItemData': 'SetItemData'},
    'Window': {'GetRect': 'Rect', 'GetClientRect': 'ClientRect', 'GetScreenRect': 'ScreenRect',
               'GetScreenPosition': 'ScreenPosition', 'GetVirtualSize': 'VirtualSize',
               'SetVirtualSize': 'VirtualSize', 'GetBestVirtualSize': 'BestVirtualSize',
               'GetMinClientSize': 'MinClientSize', 'SetMinClientSize': 'MinClientSize',
               'GetMaxClientSize': 'MaxClientSize', 'SetMaxClientSize': 'MaxClientSize',
               'GetWindowBorderSize': 'BorderSize', 'GetCharHeight': 'CharHeight',
               'GetCharWidth': 'CharWidth', 'GetDPI': 'Dpi', 'FromDIP': 'FromDip', 'ToDIP': 'ToDip',
               'GetBackgroundStyle': 'BackgroundStyle', 'SetBackgroundStyle': 'BackgroundStyle',
               'GetWindowVariant': 'Variant', 'SetWindowVariant': 'Variant',
               'IsDoubleBuffered': 'DoubleBuffered', 'SetDoubleBuffered': 'DoubleBuffered',
               'GetHelpText': 'HelpText', 'SetHelpText': 'HelpText',
               'GetScrollPos': 'GetScrollPosition', 'SetScrollPos': 'SetScrollPosition',
               'GetMinWidth': 'MinSize', 'GetMinHeight': 'MinSize',
               'GetMaxWidth': 'MaxSize', 'GetMaxHeight': 'MaxSize',
               'GetBorder': 'Border'},
    'TextCtrl': {'GetDefaultStyle': 'DefaultStyle', 'SetDefaultStyle': 'DefaultStyle',
                 'GetHint': 'Hint', 'SetHint': 'Hint', 'GetMargins': 'Margins', 'SetMargins': 'SetMargins',
                 'SetMaxLength': 'MaxLength', 'SetModified': 'IsModified', 'IsModified': 'IsModified',
                 'IsSingleLine': 'IsMultiLine', 'IsMultiLine': 'IsMultiLine',
                 'GetLineText': 'GetLineText', 'GetNumberOfLines': 'LineCount',
                 'GetLineLength': 'GetLineLength', 'ShowPosition': 'ShowPosition',
                 'GetInsertionPoint': 'InsertionPoint', 'SetInsertionPoint': 'InsertionPoint',
                 'SetInsertionPointEnd': 'MoveCaretToEnd', 'GetLastPosition': 'LastPosition',
                 'GetSelection': 'Selection', 'SetSelection': 'Selection',
                 'GetStringSelection': 'SelectedText', 'IsEditable': 'Editable', 'SetEditable': 'Editable',
                 'ChangeValue': 'ChangeValue', 'WriteText': 'Write', 'AppendText': 'Append',
                 'GetRange': 'GetRange', 'Replace': 'Replace', 'Remove': 'Remove', 'IsEmpty': 'IsEmpty',
                 'AutoComplete': 'AutoComplete', 'AutoCompleteFileNames': 'AutoCompleteFileNames',
                 'AutoCompleteDirectories': 'AutoCompleteDirectories', 'ForceUpper': 'ForceUpper',
                 'SelectNone': 'SelectNone', 'HasSelection': 'HasSelection',
                 'RemoveSelection': 'RemoveSelection'},
    'ComboBox': {'GetHint': 'Hint', 'SetHint': 'Hint', 'GetMargins': 'Margins', 'SetMargins': 'SetMargins',
                 'SetMaxLength': 'MaxLength', 'GetInsertionPoint': 'InsertionPoint',
                 'SetInsertionPoint': 'InsertionPoint', 'SetInsertionPointEnd': 'MoveCaretToEnd',
                 'GetLastPosition': 'LastPosition', 'GetSelection': 'Selection',
                 'SetSelection': 'Selection', 'GetStringSelection': 'SelectedText',
                 'IsEditable': 'Editable', 'SetEditable': 'Editable', 'ChangeValue': 'ChangeValue',
                 'WriteText': 'Write', 'AppendText': 'Append', 'GetRange': 'GetRange',
                 'Replace': 'Replace', 'Remove': 'Remove', 'IsEmpty': 'IsEmpty',
                 'AutoComplete': 'AutoComplete', 'AutoCompleteFileNames': 'AutoCompleteFileNames',
                 'AutoCompleteDirectories': 'AutoCompleteDirectories', 'ForceUpper': 'ForceUpper',
                 'SelectNone': 'SelectNone', 'HasSelection': 'HasSelection',
                 'RemoveSelection': 'RemoveSelection',
                 'GetString': 'this[]', 'SetString': 'this[]', 'FindString': 'IndexOf'},
    'SearchCtrl': {'ShowCancelButton': 'ShowCancelButton', 'IsCancelButtonVisible': 'ShowCancelButton',
                   'ShowSearchButton': 'ShowSearchButton', 'IsSearchButtonVisible': 'ShowSearchButton',
                   'GetInsertionPoint': 'InsertionPoint', 'SetInsertionPoint': 'InsertionPoint',
                   'GetLastPosition': 'LastPosition'},
    'Colour': {'Set': 'Parse', 'FromString': 'Parse', 'GetAsString': 'ToName',
               'Red': 'R', 'GetRed': 'R', 'Green': 'G', 'GetGreen': 'G',
               'Blue': 'B', 'GetBlue': 'B', 'Alpha': 'A', 'GetAlpha': 'A',
               'GetRGB': 'ToArgb', 'GetRGBA': 'ToArgb', 'SetRGB': 'FromArgb', 'SetRGBA': 'FromArgb',
               'InitRGBA': 'FromArgb', 'GetLuminance': 'Luminance'},
    'Clipboard': {'Get': 'Open', 'Open': 'Open', 'Close': 'Close', 'IsOpened': 'IsOpen', 'Flush': 'Flush',
                  'Clear': 'Clear', 'IsSupported': 'IsSupported',
                  'UsePrimarySelection': 'UsePrimarySelection', 'IsUsingPrimarySelection': 'UsePrimarySelection',
                  # SetData/GetData/AddData intentionally have no aliases here: Phoenix accepts any
                  # wxDataObject and transfers ownership, while the current typed helpers do not.
                  },
    'Frame': {'Iconize': 'Iconize', 'IsIconized': 'IsIconized', 'Maximize': 'Maximize',
              'IsMaximized': 'IsMaximized', 'IsAlwaysMaximized': 'IsAlwaysMaximized', 'Restore': 'Restore',
              'IsActive': 'IsActive', 'IsVisible': 'Visible',
              'ShowFullScreen': 'ShowFullScreen', 'IsFullScreen': 'IsFullScreen',
              'EnableFullScreenView': 'EnableFullScreenView',
              'ShowWithoutActivating': 'ShowWithoutActivating',
              'RequestUserAttention': 'RequestUserAttention',
              'EnableCloseButton': 'EnableCloseButton', 'EnableMaximizeButton': 'EnableMaximizeButton',
              'EnableMinimizeButton': 'EnableMinimizeButton',
              'CentreOnScreen': 'CentreOnScreen', 'CenterOnScreen': 'CentreOnScreen',
              'GetContentProtection': 'ContentProtection', 'SetContentProtection': 'ContentProtection',
              'SetRepresentedFilename': 'SetRepresentedFilename',
              'SetWindowModality': 'Modality', 'GetDefaultSize': 'DefaultSize',
              'GetDefaultItem': 'DefaultItem', 'SetDefaultItem': 'DefaultItem',
              'GetIcon': 'GetIcon', 'GetIcons': 'GetIcons', 'SetIcons': 'SetIcons',
              'GetMenuBar': 'GetMenuBar', 'FindItemInMenuBar': 'FindItemInMenuBar',
              'CreateStatusBar': 'CreateStatusBar', 'GetStatusBar': 'StatusBar', 'SetStatusBar': 'StatusBar',
              'SetStatusText': 'SetStatusText', 'PushStatusText': 'PushStatusText',
              'PopStatusText': 'PopStatusText', 'SetStatusWidths': 'SetStatusWidths',
              'GetStatusBarPane': 'StatusBarPane', 'SetStatusBarPane': 'StatusBarPane',
              'CreateToolBar': 'CreateToolBar', 'GetToolBar': 'ToolBar', 'SetToolBar': 'ToolBar',
              'SaveGeometry': 'SaveGeometry', 'RestoreToGeometry': 'RestoreToGeometry',
              'UseNativeStatusBar': 'UseNativeStatusBar',
              'UsesNativeStatusBar': 'UseNativeStatusBar'},
    'Locale': {'Init': 'Locale', 'GetLocale': 'Description', 'GetName': 'Name',
               'GetCanonicalName': 'CanonicalName', 'GetSysName': 'SystemName',
               'GetLanguage': 'Language', 'IsOk': 'IsOk', 'AddCatalog': 'AddCatalog',
               'IsLoaded': 'IsLoaded', 'GetString': 'GetString', 'GetHeaderValue': 'GetHeaderValue',
               'GetSystemLanguage': 'SystemLanguage', 'GetSystemEncodingName': 'SystemEncodingName',
               'IsAvailable': 'IsAvailable', 'GetLanguageName': 'GetLanguageName',
               'GetLanguageCanonicalName': 'GetLanguageCanonicalName',
               'GetLanguageInfo': 'GetLanguageInfo', 'FindLanguageInfo': 'FindLanguageInfo',
               'AddCatalogLookupPathPrefix': 'AddCatalogLookupPathPrefix',
               'GetInfo': 'GetInfo', 'GetOSInfo': 'GetOSInfo'},
    'Translations': {'Get': 'Current', 'Set': 'Current', 'SetLanguage': 'SetLanguage',
                     'AddCatalog': 'AddCatalog', 'AddAvailableCatalog': 'AddAvailableCatalog',
                     'AddStdCatalog': 'AddStdCatalog', 'IsLoaded': 'IsLoaded',
                     'GetAvailableTranslations': 'GetAvailableTranslations',
                     'GetBestTranslation': 'GetBestTranslation',
                     'GetBestAvailableTranslation': 'GetBestAvailableTranslation',
                     'GetTranslatedString': 'GetTranslatedString',
                     'GetHeaderValue': 'GetHeaderValue'},
    'LanguageInfo': {'GetCanonicalWithRegion': 'CanonicalWithRegion', 'GetLocaleName': 'CanonicalName',
                     'GetLCID': 'WindowsLanguage', 'TrySetLocale': 'CanonicalName'},
    'CheckBox': {'GetValue': 'Checked', 'SetValue': 'Checked', 'IsChecked': 'Checked'},
    'ToggleButton': {'GetValue': 'Value', 'SetValue': 'Value'},
    'RadioButton': {'GetValue': 'Value', 'SetValue': 'Value'},
    'Gauge': {'SetRange': 'Range', 'GetRange': 'Range'},
    'Slider': {'SetMin': 'SetRange', 'SetMax': 'SetRange'},
    'ListBox': {'GetSelections': 'GetSelectedIndices', 'Deselect': 'SetSelected'},
    'ListCtrl': {'GetItemState': 'IsSelected', 'SetItemState': 'SetSelected',
                 'GetNextItem': 'GetSelectedIndices', 'GetSelectedItemCount': 'SelectedCount',
                 'InsertItem': 'AddItem', 'DeleteItem': 'RemoveAt', 'DeleteAllItems': 'Clear',
                 'DeleteColumn': 'RemoveColumn', 'DeleteAllColumns': 'ClearColumns',
                 'GetColumn': 'GetColumnHeading', 'SetColumn': 'SetColumnHeading'},
    'TreeCtrl': {'GetItemParent': 'GetParent', 'GetPrevSibling': 'GetPreviousSibling',
                 'GetChildrenCount': 'GetChildCount', 'AppendItem': 'Add',
                 'GetRootItem': 'Root', 'SelectItem': 'Selection',
                 'UnselectAll': 'Unselect', 'Unselect': 'Unselect',
                 'Collapse': 'Expand', 'DeleteAllItems': 'Clear', 'Delete': 'Remove'},
    'Choice': {'GetString': 'this[]', 'SetString': 'this[]', 'Select': 'SelectedIndex'},
    'Menu': {'FindItemByPosition': 'this[]', 'GetMenuItemCount': 'Count'},
    'MenuItem': {'GetItemLabel': 'Label', 'SetItemLabel': 'Label', 'GetItemLabelText': 'Label',
                 'GetHelp': 'Help', 'SetHelp': 'Help', 'IsEnabled': 'Enabled', 'Enable': 'Enabled',
                 'IsChecked': 'Checked', 'Check': 'Checked', 'GetSubMenu': 'SubMenu',
                 'GetKind': 'Kind'},
    'MenuBar': {'GetMenuCount': 'Count', 'GetMenu': 'this[]', 'GetMenuLabel': 'GetLabelTop',
                'SetMenuLabel': 'SetLabelTop'},
    'Timer': {'GetInterval': 'Interval'},
    'Notebook': {'GetPageCount': 'Count', 'DeletePage': 'RemovePage'},
    'SimpleBook': {'GetPageCount': 'Count', 'DeletePage': 'RemovePage'},
    'SplitterWindow': {'GetSashPosition': 'SashPosition', 'SetSashPosition': 'SashPosition',
                       'SplitVertically': 'Split', 'SplitHorizontally': 'Split'},
    'StatusBar': {'SetStatusText': 'SetText', 'GetStatusText': 'GetText'},
    'SpinCtrl': {'GetValue': 'Value', 'SetValue': 'Value'},
    'SpinCtrlDouble': {'GetValue': 'Value', 'SetValue': 'Value'},
    'Sizer': {'GetItemCount': 'ItemCount', 'IsEmpty': 'IsEmpty', 'GetMinSize': 'MinSize',
              'SetMinSize': 'MinSize', 'GetSize': 'Size', 'GetPosition': 'Position',
              'Detach': 'Detach', 'DetachItem': 'DetachAt', 'Remove': 'Remove', 'Replace': 'Replace',
              'Clear': 'Clear', 'DeleteWindows': 'DeleteWindows', 'Insert': 'Insert', 'Prepend': 'Prepend',
              'InsertSpacer': 'InsertSpacer', 'InsertStretchSpacer': 'InsertStretchSpacer',
              'PrependSpacer': 'PrependSpacer', 'PrependStretchSpacer': 'PrependStretchSpacer',
              'Show': 'Show', 'Hide': 'Hide', 'IsShown': 'IsShown', 'ShowItems': 'ShowItems',
              'AreAnyItemsShown': 'AreAnyItemsShown', 'Layout': 'Layout', 'Fit': 'Fit',
              'FitInside': 'FitInside', 'SetSizeHints': 'SetSizeHints', 'GetItem': 'GetItem',
              'GetItemById': 'GetItemById', 'SetItemMinSize': 'SetItemMinSize',
              'SetDimension': 'SetDimension', 'ComputeFittingClientSize': 'ComputeFittingClientSize',
              'ComputeFittingWindowSize': 'ComputeFittingWindowSize'},
    'BoxSizer': {'GetOrientation': 'Orientation', 'SetOrientation': 'Orientation',
                 'IsVertical': 'Orientation', 'AddSpacer': 'AddSpacer'},
    'GridSizer': {'GetRows': 'Rows', 'SetRows': 'Rows', 'GetCols': 'Columns', 'SetCols': 'Columns',
                  'GetVGap': 'VerticalGap', 'SetVGap': 'VerticalGap', 'GetHGap': 'HorizontalGap',
                  'SetHGap': 'HorizontalGap', 'GetEffectiveRowsCount': 'EffectiveRows',
                  'GetEffectiveColsCount': 'EffectiveColumns'},
    'FlexGridSizer': {'AddGrowableCol': 'AddGrowableColumn', 'RemoveGrowableCol': 'RemoveGrowableColumn',
                      'RemoveGrowableRow': 'RemoveGrowableRow', 'IsColGrowable': 'IsColumnGrowable',
                      'IsRowGrowable': 'IsRowGrowable', 'GetFlexibleDirection': 'FlexibleDirection',
                      'SetFlexibleDirection': 'FlexibleDirection',
                      'GetNonFlexibleGrowMode': 'NonFlexibleGrowMode',
                      'SetNonFlexibleGrowMode': 'NonFlexibleGrowMode',
                      'GetRowHeights': 'GetRowHeights', 'GetColWidths': 'GetColumnWidths'},
    'GridBagSizer': {'Add': 'AddAt', 'GetItemPosition': 'GetItemPosition',
                     'SetItemPosition': 'SetItemPosition', 'GetItemSpan': 'GetItemSpan',
                     'SetItemSpan': 'SetItemSpan', 'FindItemAtPosition': 'FindItemAtPosition',
                     'FindItemAtPoint': 'FindItemAtPoint', 'GetCellSize': 'GetCellSize',
                     'GetEmptyCellSize': 'EmptyCellSize', 'SetEmptyCellSize': 'EmptyCellSize',
                     'CheckForIntersection': 'CheckForIntersection'},
    'StaticBoxSizer': {'GetStaticBox': 'Box', 'AreAnyItemsShown': 'AreAnyItemsShown',
                       'Detach': 'Detach', 'ShowItems': 'ShowItems'},
}


CLASS_RE = re.compile(r'\bclass\s+(?:WXDLLIMPEXP_\w+\s+)?(\w+)\s*(?::[^{;]*)?\{')
MEMBER_RE = re.compile(r'(?:^|[\s*&])(\w+)\s*\(')
ACCESS_RE = re.compile(r'^\s*(public|protected|private)\s*:')


def strip_comments(text):
    text = re.sub(r'/\*.*?\*/', '', text, flags=re.S)
    return re.sub(r'//[^\n]*', '', text)


def public_members(class_name):
    """Every public member function wxWidgets declares for a class, across the headers that define it."""
    found = set()
    for base in (INCLUDE, os.path.join(INCLUDE, 'wx')):
        pass
    roots = [os.path.join(INCLUDE, 'wx'), os.path.join(INCLUDE, 'wx', 'msw'),
             os.path.join(INCLUDE, 'wx', 'generic')]
    for root in roots:
        if not os.path.isdir(root):
            continue
        for name in os.listdir(root):
            if not name.endswith('.h'):
                continue
            path = os.path.join(root, name)
            try:
                text = strip_comments(io.open(path, encoding='utf-8', errors='replace').read())
            except OSError:
                continue
            for m in CLASS_RE.finditer(text):
                if m.group(1) != class_name:
                    continue
                # Walk to the matching close brace, tracking the current access specifier.
                depth = 0
                i = m.end() - 1
                access = 'private' if 'class' in text[m.start():m.end()] else 'public'
                access = 'private'
                start = i
                while i < len(text):
                    if text[i] == '{':
                        depth += 1
                    elif text[i] == '}':
                        depth -= 1
                        if depth == 0:
                            break
                    i += 1
                body = text[start + 1:i]
                for line in body.split('\n'):
                    hit = ACCESS_RE.match(line)
                    if hit:
                        access = hit.group(1)
                        continue
                    if access != 'public':
                        continue
                    if line.count('{') or line.strip().startswith('#'):
                        # Still parse inline definitions; only skip preprocessor lines.
                        if line.strip().startswith('#'):
                            continue
                    for member in MEMBER_RE.finditer(line):
                        found.add(member.group(1))
    return found


CS_MEMBER_RE = re.compile(
    r'^\s*public\s+(?:static\s+|virtual\s+|override\s+|sealed\s+|unsafe\s+|readonly\s+|new\s+|abstract\s+)*'
    r'(?:event\s+)?[\w<>\[\],\.\?\(\) ]+?\s(\w+)\s*(?:[({=;]|=>)', re.M)


def managed_members():
    """Public member names per managed type, plus a flat set for base-class members."""
    per_type = {}
    for dirpath, _dirs, files in os.walk(MANAGED):
        if 'obj' in dirpath or 'bin' in dirpath:
            continue
        for name in files:
            if not name.endswith('.cs'):
                continue
            text = io.open(os.path.join(dirpath, name), encoding='utf-8').read()
            for m in re.finditer(r'public\s+(?:sealed\s+|abstract\s+|static\s+|partial\s+|unsafe\s+)*'
                                 r'(?:class|record struct|readonly record struct|struct|enum)\s+(\w+)', text):
                cls = m.group(1)
                start = m.end()
                nxt = re.search(r'\npublic\s+(?:sealed\s+|abstract\s+|static\s+|partial\s+|unsafe\s+)*'
                                r'(?:class|record struct|readonly record struct|struct|enum)\s+\w+', text[start:])
                body = text[start:start + nxt.start()] if nxt else text[start:]
                members = {hit.group(1) for hit in CS_MEMBER_RE.finditer(body)}
                # A record struct declares its positional parameters as public properties.
                params = re.match(r'\s*\(([^)]*)\)', body)
                if params:
                    for part in params.group(1).split(','):
                        words = part.strip().split('=')[0].split()
                        if len(words) >= 2:
                            members.add(words[-1])
                per_type.setdefault(cls, set()).update(members)
    return per_type


def normalise(name, cs_name=None):
    # A derived wrapper inherits its base's aliases along with its members.
    for candidate in (cs_name,) + MANAGED_BASES.get(cs_name, ()) if cs_name else ():
        if name in PER_TYPE.get(candidate, {}):
            return PER_TYPE[candidate][name]
    return COLLAPSE.get(name, name)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--detail', action='store_true', help='list every missing member')
    parser.add_argument('--type', help='report on one managed type')
    parser.add_argument('--min', type=int, default=0, help='only show types missing at least N members')
    args = parser.parse_args()

    managed = managed_members()
    # Members inherited from the managed Window base count as present on every control.
    inherited = set()
    for base in ('Window', 'Control'):
        inherited |= managed.get(base, set())

    rows = []
    for cs_name, (wx_name, bases) in TYPES.items():
        if args.type and args.type != cs_name:
            continue
        wx_members = set()
        for name in [wx_name] + bases:
            wx_members |= public_members(name)
        wx_members = {m for m in wx_members if is_api(m, wx_name, bases)}
        wx_members -= PER_TYPE_SKIP.get(cs_name, set())

        have = set(managed.get(cs_name, set()))
        for managed_base in MANAGED_BASES.get(cs_name, ()):
            have |= managed.get(managed_base, set())
        SIZERS = ('BoxSizer', 'GridSizer', 'FlexGridSizer', 'GridBagSizer', 'StaticBoxSizer')
        if cs_name in SIZERS:
            have |= managed.get('Sizer', set())          # the managed base
        if cs_name in ('FlexGridSizer', 'GridBagSizer'):
            have |= managed.get('GridSizer', set())      # wxFlexGridSizer derives from wxGridSizer
        if cs_name == 'GridBagSizer':
            have |= managed.get('FlexGridSizer', set())
        if cs_name not in ('Sizer',) + SIZERS + ('Menu', 'MenuItem', 'MenuBar', 'Timer', 'Clipboard',
                                                 'Font', 'Colour', 'Bitmap', 'Image', 'ProgressDialog'):
            have |= inherited
        have_normalised = {normalise(m, cs_name) for m in have} | have

        missing = sorted(m for m in wx_members
                         if normalise(m, cs_name) not in have_normalised and m not in have)
        covered = len(wx_members) - len(missing)
        rows.append((cs_name, wx_name, covered, len(wx_members), missing))

    rows.sort(key=lambda r: -len(r[4]))

    print(f'{"managed type":22} {"wx class":24} {"covered":>9}  missing')
    print('-' * 78)
    for cs_name, wx_name, covered, total, missing in rows:
        if len(missing) < args.min:
            continue
        pct = f'{covered}/{total}'
        print(f'{cs_name:22} {wx_name:24} {pct:>9}  {len(missing)}')
        if args.detail or args.type:
            for chunk in [missing[i:i + 4] for i in range(0, len(missing), 4)]:
                print('    ' + ', '.join(chunk))
    return 0


if __name__ == '__main__':
    sys.exit(main())
