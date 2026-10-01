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
    /// <summary>The "add bookmark" icon.</summary>
    public const string AddBookmark = "wxART_ADD_BOOKMARK";
    /// <summary>The "remove bookmark" icon.</summary>
    public const string DelBookmark = "wxART_DEL_BOOKMARK";
    /// <summary>The help viewer's side-panel toggle icon.</summary>
    public const string HelpSidePanel = "wxART_HELP_SIDE_PANEL";
    /// <summary>The help viewer's settings icon.</summary>
    public const string HelpSettings = "wxART_HELP_SETTINGS";
    /// <summary>The icon for a help book.</summary>
    public const string HelpBook = "wxART_HELP_BOOK";
    /// <summary>The icon for a folder within a help book.</summary>
    public const string HelpFolder = "wxART_HELP_FOLDER";
    /// <summary>The icon for a help page.</summary>
    public const string HelpPage = "wxART_HELP_PAGE";
    /// <summary>The "go back" navigation arrow.</summary>
    public const string GoBack = "wxART_GO_BACK";
    /// <summary>The "go forward" navigation arrow.</summary>
    public const string GoForward = "wxART_GO_FORWARD";
    /// <summary>The "go up" navigation arrow.</summary>
    public const string GoUp = "wxART_GO_UP";
    /// <summary>The "go down" navigation arrow.</summary>
    public const string GoDown = "wxART_GO_DOWN";
    /// <summary>The "go to parent" navigation arrow.</summary>
    public const string GoToParent = "wxART_GO_TO_PARENT";
    /// <summary>The "go home" icon.</summary>
    public const string GoHome = "wxART_GO_HOME";
    /// <summary>The "first" icon, for jumping to the start.</summary>
    public const string GotoFirst = "wxART_GOTO_FIRST";
    /// <summary>The "last" icon, for jumping to the end.</summary>
    public const string GotoLast = "wxART_GOTO_LAST";
    /// <summary>The "open file" icon.</summary>
    public const string FileOpen = "wxART_FILE_OPEN";
    /// <summary>The "save file" icon.</summary>
    public const string FileSave = "wxART_FILE_SAVE";
    /// <summary>The "save file as" icon.</summary>
    public const string FileSaveAs = "wxART_FILE_SAVE_AS";
    /// <summary>The "print" icon.</summary>
    public const string Print = "wxART_PRINT";
    /// <summary>The generic "help" icon.</summary>
    public const string Help = "wxART_HELP";
    /// <summary>The icon for a tip or hint, as used by a tip-of-the-day dialog.</summary>
    public const string Tip = "wxART_TIP";
    /// <summary>The "report view" icon, for the detailed columnar file view.</summary>
    public const string ReportView = "wxART_REPORT_VIEW";
    /// <summary>The "list view" icon, for the plain list file view.</summary>
    public const string ListView = "wxART_LIST_VIEW";
    /// <summary>The "new folder" icon.</summary>
    public const string NewDir = "wxART_NEW_DIR";
    /// <summary>The icon for a hard disk.</summary>
    public const string Harddisk = "wxART_HARDDISK";
    /// <summary>The icon for a floppy disk.</summary>
    public const string Floppy = "wxART_FLOPPY";
    /// <summary>The icon for a CD-ROM drive.</summary>
    public const string Cdrom = "wxART_CDROM";
    /// <summary>The icon for a removable drive.</summary>
    public const string Removable = "wxART_REMOVABLE";
    /// <summary>The icon for a closed folder.</summary>
    public const string Folder = "wxART_FOLDER";
    /// <summary>The icon for an open folder.</summary>
    public const string FolderOpen = "wxART_FOLDER_OPEN";
    /// <summary>The "go to parent directory" icon.</summary>
    public const string GoDirUp = "wxART_GO_DIR_UP";
    /// <summary>The icon for an executable file.</summary>
    public const string ExecutableFile = "wxART_EXECUTABLE_FILE";
    /// <summary>The icon for an ordinary file.</summary>
    public const string NormalFile = "wxART_NORMAL_FILE";
    /// <summary>A tick (check) mark.</summary>
    public const string TickMark = "wxART_TICK_MARK";
    /// <summary>A cross mark.</summary>
    public const string CrossMark = "wxART_CROSS_MARK";
    /// <summary>The error icon, as shown in an error message box.</summary>
    public const string Error = "wxART_ERROR";
    /// <summary>The question icon, as shown in a confirmation dialog.</summary>
    public const string Question = "wxART_QUESTION";
    /// <summary>The warning icon.</summary>
    public const string Warning = "wxART_WARNING";
    /// <summary>The information icon.</summary>
    public const string Information = "wxART_INFORMATION";
    /// <summary>The placeholder drawn when a requested image cannot be found.</summary>
    public const string MissingImage = "wxART_MISSING_IMAGE";
    /// <summary>The "copy" icon.</summary>
    public const string Copy = "wxART_COPY";
    /// <summary>The "cut" icon.</summary>
    public const string Cut = "wxART_CUT";
    /// <summary>The "paste" icon.</summary>
    public const string Paste = "wxART_PASTE";
    /// <summary>The "delete" icon.</summary>
    public const string Delete = "wxART_DELETE";
    /// <summary>The "new" icon.</summary>
    public const string New = "wxART_NEW";
    /// <summary>The "undo" icon.</summary>
    public const string Undo = "wxART_UNDO";
    /// <summary>The "redo" icon.</summary>
    public const string Redo = "wxART_REDO";
    /// <summary>The "plus" (add) icon.</summary>
    public const string Plus = "wxART_PLUS";
    /// <summary>The "minus" (remove) icon.</summary>
    public const string Minus = "wxART_MINUS";
    /// <summary>The "close" icon.</summary>
    public const string Close = "wxART_CLOSE";
    /// <summary>The "quit" icon.</summary>
    public const string Quit = "wxART_QUIT";
    /// <summary>The "find" icon.</summary>
    public const string Find = "wxART_FIND";
    /// <summary>The "find and replace" icon.</summary>
    public const string FindAndReplace = "wxART_FIND_AND_REPLACE";
    /// <summary>The "full screen" icon.</summary>
    public const string FullScreen = "wxART_FULL_SCREEN";
    /// <summary>The "edit" icon.</summary>
    public const string Edit = "wxART_EDIT";
    /// <summary>The wxWidgets logo.</summary>
    public const string WxLogo = "wxART_WX_LOGO";
    /// <summary>The "refresh" icon.</summary>
    public const string Refresh = "wxART_REFRESH";
    /// <summary>The "stop" icon.</summary>
    public const string Stop = "wxART_STOP";
}

/// <summary>Where the art is going to be used, following the <c>wxART_</c> client identifiers. The
/// platform picks a size and sometimes a different image for each, so naming the right one matters more
/// than it looks.</summary>
public static class ArtClient
{
    /// <summary>Art bound for a toolbar button.</summary>
    public const string Toolbar = "wxART_TOOLBAR_C";
    /// <summary>Art bound for a menu item.</summary>
    public const string Menu = "wxART_MENU_C";
    /// <summary>Art bound for a window's title-bar/frame icon.</summary>
    public const string FrameIcon = "wxART_FRAME_ICON_C";
    /// <summary>Art bound for a common dialog.</summary>
    public const string CmnDialog = "wxART_CMN_DIALOG_C";
    /// <summary>Art bound for the help browser.</summary>
    public const string HelpBrowser = "wxART_HELP_BROWSER_C";
    /// <summary>Art bound for a message box.</summary>
    public const string MessageBox = "wxART_MESSAGE_BOX_C";
    /// <summary>Art bound for a button.</summary>
    public const string Button = "wxART_BUTTON_C";
    /// <summary>Art bound for a list control.</summary>
    public const string List = "wxART_LIST_C";
    /// <summary>Art for any use not covered by the other clients; the default.</summary>
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
