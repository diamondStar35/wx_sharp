using System;

namespace WxSharp;

/// <summary>Names the stock art wxWidgets can supply, following the <c>wxART_</c> identifiers.</summary>
///
/// <remarks>
/// Asking the platform for its own icon is what makes a toolbar or a message look native. It also follows
/// the user's theme without any work, and stays legible in high contrast - which a shipped PNG does not.
/// </remarks>
public static class ArtId
{
    public const string AddBookmark = "wxART_ADD_BOOKMARK";
    public const string DelBookmark = "wxART_DEL_BOOKMARK";
    public const string HelpSidePanel = "wxART_HELP_SIDE_PANEL";
    public const string HelpSettings = "wxART_HELP_SETTINGS";
    public const string HelpBook = "wxART_HELP_BOOK";
    public const string HelpFolder = "wxART_HELP_FOLDER";
    public const string HelpPage = "wxART_HELP_PAGE";
    public const string GoBack = "wxART_GO_BACK";
    public const string GoForward = "wxART_GO_FORWARD";
    public const string GoUp = "wxART_GO_UP";
    public const string GoDown = "wxART_GO_DOWN";
    public const string GoToParent = "wxART_GO_TO_PARENT";
    public const string GoHome = "wxART_GO_HOME";
    public const string GotoFirst = "wxART_GOTO_FIRST";
    public const string GotoLast = "wxART_GOTO_LAST";
    public const string FileOpen = "wxART_FILE_OPEN";
    public const string FileSave = "wxART_FILE_SAVE";
    public const string FileSaveAs = "wxART_FILE_SAVE_AS";
    public const string Print = "wxART_PRINT";
    public const string Help = "wxART_HELP";
    public const string Tip = "wxART_TIP";
    public const string ReportView = "wxART_REPORT_VIEW";
    public const string ListView = "wxART_LIST_VIEW";
    public const string NewDir = "wxART_NEW_DIR";
    public const string Harddisk = "wxART_HARDDISK";
    public const string Floppy = "wxART_FLOPPY";
    public const string Cdrom = "wxART_CDROM";
    public const string Removable = "wxART_REMOVABLE";
    public const string Folder = "wxART_FOLDER";
    public const string FolderOpen = "wxART_FOLDER_OPEN";
    public const string GoDirUp = "wxART_GO_DIR_UP";
    public const string ExecutableFile = "wxART_EXECUTABLE_FILE";
    public const string NormalFile = "wxART_NORMAL_FILE";
    public const string TickMark = "wxART_TICK_MARK";
    public const string CrossMark = "wxART_CROSS_MARK";
    public const string Error = "wxART_ERROR";
    public const string Question = "wxART_QUESTION";
    public const string Warning = "wxART_WARNING";
    public const string Information = "wxART_INFORMATION";
    public const string MissingImage = "wxART_MISSING_IMAGE";
    public const string Copy = "wxART_COPY";
    public const string Cut = "wxART_CUT";
    public const string Paste = "wxART_PASTE";
    public const string Delete = "wxART_DELETE";
    public const string New = "wxART_NEW";
    public const string Undo = "wxART_UNDO";
    public const string Redo = "wxART_REDO";
    public const string Plus = "wxART_PLUS";
    public const string Minus = "wxART_MINUS";
    public const string Close = "wxART_CLOSE";
    public const string Quit = "wxART_QUIT";
    public const string Find = "wxART_FIND";
    public const string FindAndReplace = "wxART_FIND_AND_REPLACE";
    public const string FullScreen = "wxART_FULL_SCREEN";
    public const string Edit = "wxART_EDIT";
    public const string WxLogo = "wxART_WX_LOGO";
    public const string Refresh = "wxART_REFRESH";
    public const string Stop = "wxART_STOP";
}

/// <summary>Where the art is going to be used, following the <c>wxART_</c> client identifiers. The
/// platform picks a size and sometimes a different image for each, so naming the right one matters more
/// than it looks.</summary>
public static class ArtClient
{
    public const string Toolbar = "wxART_TOOLBAR_C";
    public const string Menu = "wxART_MENU_C";
    public const string FrameIcon = "wxART_FRAME_ICON_C";
    public const string CmnDialog = "wxART_CMN_DIALOG_C";
    public const string HelpBrowser = "wxART_HELP_BROWSER_C";
    public const string MessageBox = "wxART_MESSAGE_BOX_C";
    public const string Button = "wxART_BUTTON_C";
    public const string List = "wxART_LIST_C";
    public const string Other = "wxART_OTHER_C";
}

/// <summary>The platform's own stock icons, following <c>wxArtProvider</c>.</summary>
public static class ArtProvider
{
    /// <summary>The stock bitmap for an ID, or null when the platform has none. Pass a
    /// <see cref="ArtClient"/> so the platform can pick the right size and image for where it will be
    /// used; leave the size unset to get the platform's own.</summary>
    public static Bitmap? GetBitmap(string id, string client = ArtClient.Other, Size? size = null)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(client);
        _ = App.RequireCurrent();
        var wanted = size ?? new Size(0, 0);
        var handle = NativeMethods.wxsharp_art_bitmap(id, client, wanted.Width, wanted.Height);
        return handle == 0 ? null : Bitmap.Attach(handle);
    }

    /// <summary>The stock icon for an ID, or null when the platform has none.</summary>
    public static Icon? GetIcon(string id, string client = ArtClient.Other, Size? size = null)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(client);
        _ = App.RequireCurrent();
        var wanted = size ?? new Size(0, 0);
        var handle = NativeMethods.wxsharp_art_icon(id, client, wanted.Width, wanted.Height);
        return handle == 0 ? null : Icon.Attach(handle);
    }

    /// <summary>The size the platform draws art at for a given use - what a toolbar built by hand should
    /// size its images to. Follows <c>wxArtProvider.GetNativeSizeHint</c>.</summary>
    public static Size GetNativeSizeHint(string client, Window? window = null)
    {
        ArgumentNullException.ThrowIfNull(client);
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_art_native_size(client, window?.Handle ?? 0, out var w, out var h);
        return new Size(w, h);
    }
}
