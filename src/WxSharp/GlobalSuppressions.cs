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
