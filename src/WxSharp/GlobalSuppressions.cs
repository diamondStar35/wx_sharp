using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "KeyEventArgs represents data supplied to key event handlers.",
    Scope = "type",
    Target = "~T:WxSharp.KeyEventArgs")]

[assembly: SuppressMessage(
    "Naming",
    "CA1720:Identifier contains type name",
    Justification = "Single is the established domain term for single-selection controls.",
    Scope = "member",
    Target = "~F:WxSharp.ListBoxStyle.Single")]

[assembly: SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "SizerFlags is the public flags enum used when adding items to a wxWidgets sizer.",
    Scope = "type",
    Target = "~T:WxSharp.SizerFlags")]

// ---- Enum values wxWidgets deliberately gives two names ---------------------------------------------------
// wx/defs.h defines these as aliases of each other - WXK_BACK is WXK_CONTROL_H, WXK_CONTROL is WXK_COMMAND -
// and code written against wxWidgets uses whichever name reads better in context. Renaming or dropping one
// half would make the enum disagree with the header it is generated from.
[assembly: SuppressMessage(
    "Design",
    "CA1069:Enums values should not be duplicated",
    Justification = "wxWidgets defines these key codes as aliases of one another; Key is generated from wx/defs.h.",
    Scope = "type",
    Target = "~T:WxSharp.Key")]

// ---- Flags enums -------------------------------------------------------------------------------------------
// Each of these is a set of bit flags, which is exactly what the suffix says. CA1711 objects to the suffix in
// general; here it is the accurate name and matches the wxWidgets constants it wraps.
[assembly: SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "A bit-flags enum over the wxBROWSER_* constants.",
    Scope = "type",
    Target = "~T:WxSharp.BrowserFlags")]

[assembly: SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "A bit-flags enum over the wxEXEC_* constants.",
    Scope = "type",
    Target = "~T:WxSharp.ExecuteFlags")]

[assembly: SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "A bit-flags enum over the wxLocaleInitFlags values.",
    Scope = "type",
    Target = "~T:WxSharp.LocaleInitFlags")]

[assembly: SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "A bit-flags enum over the wxTextAttrFlags values, which say which parts of a style are set.",
    Scope = "type",
    Target = "~T:WxSharp.TextAttrFlags")]

[assembly: SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "The public wxFontFlag enum name is retained to match wxWidgets and Phoenix.",
    Scope = "type",
    Target = "~T:WxSharp.FontFlag")]

[assembly: SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "A bit-flags enum over the wxSizerFlags button constants.",
    Scope = "type",
    Target = "~T:WxSharp.ButtonSizerFlags")]

// ---- Names that come from wxWidgets -------------------------------------------------------------------------
// Renaming these would break the correspondence with the wxWidgets API the wrapper is deliberately mirroring,
// which is the whole point of the surface. C# itself has no trouble with any of them.
[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "wxAccessible::Select is the wxWidgets method this overrides.",
    Scope = "member",
    Target = "~M:WxSharp.Accessible.Select(System.Int32,WxSharp.AccessibleSelection)~WxSharp.AccessibleStatus")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "from and to are the parameter names wxTextEntry uses for a character range.",
    Scope = "member",
    Target = "~M:WxSharp.ITextEntry.GetRange(System.Int32,System.Int32)~System.String")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "from and to are the parameter names wxTextEntry uses for a character range.",
    Scope = "member",
    Target = "~M:WxSharp.ITextEntry.Replace(System.Int32,System.Int32,System.String)")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "from and to are the parameter names wxTextEntry uses for a character range.",
    Scope = "member",
    Target = "~M:WxSharp.ITextEntry.Remove(System.Int32,System.Int32)")]

[assembly: SuppressMessage(
    "Naming",
    "CA1720:Identifier contains type name",
    Justification = "Char is the wxEVT_CHAR event, named as wxWidgets and wxPython name it.",
    Scope = "member",
    Target = "~P:WxSharp.WxEvents.Char")]

[assembly: SuppressMessage(
    "Naming",
    "CA1720:Identifier contains type name",
    Justification = "Decimal is the numeric keypad decimal key, WXK_DECIMAL.",
    Scope = "member",
    Target = "~F:WxSharp.Key.Decimal")]

[assembly: SuppressMessage(
    "Performance",
    "CA1822:Mark members as static",
    Justification = "wxColour::IsOk is an instance method; a Colour is always valid, but the shape follows wxWidgets.",
    Scope = "member",
    Target = "~P:WxSharp.Colour.IsOk")]
