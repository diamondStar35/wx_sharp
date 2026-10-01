using System;

namespace WxSharp;

/// <summary>The icon a rich tooltip shows, following the <c>wxICON_</c> values <c>wxRichToolTip</c>
/// accepts.</summary>
public enum RichToolTipIcon
{
    /// <summary>No icon.</summary>
    None = 0,
    /// <summary>The information icon (<c>wxICON_INFORMATION</c>).</summary>
    Information = 0x00000800,
    /// <summary>The warning icon (<c>wxICON_WARNING</c>).</summary>
    Warning = 0x00000100,
    /// <summary>The error icon (<c>wxICON_ERROR</c>).</summary>
    Error = 0x00000200,
}

/// <summary>What an about box says about the application, following <c>wxAboutDialogInfo</c>.</summary>
public sealed class AboutInfo
{
    /// <summary>The application's name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>The version string, shown next to the name.</summary>
    public string Version { get; set; } = string.Empty;
    /// <summary>A short description of the application.</summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>The copyright notice.</summary>
    public string Copyright { get; set; } = string.Empty;
    /// <summary>The project or support web site URL.</summary>
    public string WebSite { get; set; } = string.Empty;

    /// <summary>What to show instead of the bare URL. Empty shows the URL itself.</summary>
    public string WebSiteLabel { get; set; } = string.Empty;

    /// <summary>The list of developers to credit. Setting any makes some platforms fall back to a generic
    /// dialog instead of the native one.</summary>
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
    /// <summary>Shows the about dialog populated from <paramref name="info"/>, modal to
    /// <paramref name="parent"/> when given.</summary>
    public static void Show(AboutInfo info, Window? parent = null)
    {
        ArgumentNullException.ThrowIfNull(info);
        var app = App.RequireCurrent();
        app.VerifyAccess();
        NativeMethods.ShowAboutBox(info, parent);
    }
}
