using System;

namespace WxSharp;

/// <summary>The icon a rich tooltip shows, following the <c>wxICON_</c> values <c>wxRichToolTip</c>
/// accepts.</summary>
public enum RichToolTipIcon
{
    None = 0,
    Information = 0x00000800,
    Warning = 0x00000100,
    Error = 0x00000200,
}

/// <summary>What an about box says about the application, following <c>wxAboutDialogInfo</c>.</summary>
public sealed class AboutInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Copyright { get; set; } = string.Empty;
    public string WebSite { get; set; } = string.Empty;

    /// <summary>What to show instead of the bare URL. Empty shows the URL itself.</summary>
    public string WebSiteLabel { get; set; } = string.Empty;

    public string[] Developers { get; set; } = [];
}

/// <summary>The platform's standard about dialog, following <c>wxAboutBox</c>.</summary>
///
/// <remarks>
/// Worth using rather than laying one out by hand: on some platforms this is a native panel rather than a
/// window wxWidgets draws, so it looks right, reads right to a screen reader, and puts the fields where the
/// user expects them with no work.
///
/// Filling in only the simple fields keeps the native dialog on every platform; adding developers makes
/// wxWidgets fall back to a generic one on some of them, which is a trade worth making deliberately.
/// </remarks>
public static class AboutBox
{
    public static void Show(AboutInfo info, Window? parent = null)
    {
        ArgumentNullException.ThrowIfNull(info);
        var app = App.RequireCurrent();
        app.VerifyAccess();
        NativeMethods.ShowAboutBox(info, parent);
    }
}
