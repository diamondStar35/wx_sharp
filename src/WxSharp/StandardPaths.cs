using System;

namespace WxSharp;

/// <summary>One of the user's own folders, following <c>wxStandardPaths.Dir</c>.</summary>
public enum UserDirectory
{
    Cache = 0,
    Documents = 1,
    Desktop = 2,
    Downloads = 3,
    Music = 4,
    Pictures = 5,
    Videos = 6,
}

/// <summary>Which resources a localized resource directory holds, following
/// <c>wxStandardPaths.ResourceCat</c>.</summary>
public enum ResourceCategory
{
    None = 0,
    /// <summary>Gettext message catalogues, which is where <see cref="Locale"/> looks.</summary>
    Messages = 1,
}

/// <summary>Where the platform expects an application to keep things, following
/// <c>wxStandardPaths</c>.</summary>
///
/// <remarks>
/// These differ per platform in ways that are easy to get wrong from memory, and getting them wrong is not
/// cosmetic: on Windows, writing settings beside the executable puts them somewhere the user cannot back up
/// and may not be able to write to at all.
///
/// The distinction that matters most is roaming versus local: <see cref="UserConfigDirectory"/> and
/// <see cref="UserDataDirectory"/> follow the user between machines, while
/// <see cref="UserLocalDataDirectory"/> stays put. Caches and anything large or machine-specific belong in
/// the local one.
/// </remarks>
public static unsafe class StandardPaths
{
    /// <summary>The running executable, including its file name.</summary>
    public static string ExecutablePath => Read(NativeMethods.wxsharp_stdpaths_executable);

    /// <summary>System-wide configuration, shared by every user.</summary>
    public static string ConfigDirectory => Read(NativeMethods.wxsharp_stdpaths_config_dir);

    /// <summary>This user's configuration, which follows them between machines.</summary>
    public static string UserConfigDirectory => Read(NativeMethods.wxsharp_stdpaths_user_config_dir);

    /// <summary>Read-only data installed with the application.</summary>
    public static string DataDirectory => Read(NativeMethods.wxsharp_stdpaths_data_dir);

    /// <summary>Machine-specific data shared by every user.</summary>
    public static string LocalDataDirectory => Read(NativeMethods.wxsharp_stdpaths_local_data_dir);

    /// <summary>This user's application data, which follows them between machines.</summary>
    public static string UserDataDirectory => Read(NativeMethods.wxsharp_stdpaths_user_data_dir);

    /// <summary>This user's application data on this machine only - where caches and anything large or
    /// machine-specific belong, so they are not copied between machines.</summary>
    public static string UserLocalDataDirectory => Read(NativeMethods.wxsharp_stdpaths_user_local_data_dir);

    /// <summary>Where loadable plugins are installed.</summary>
    public static string PluginsDirectory => Read(NativeMethods.wxsharp_stdpaths_plugins_dir);

    /// <summary>Application resources - icons, sounds and the like.</summary>
    public static string ResourcesDirectory => Read(NativeMethods.wxsharp_stdpaths_resources_dir);

    /// <summary>The user's documents folder.</summary>
    public static string DocumentsDirectory => Read(NativeMethods.wxsharp_stdpaths_documents_dir);

    /// <summary>The system's temporary directory.</summary>
    public static string TempDirectory => Read(NativeMethods.wxsharp_stdpaths_temp_dir);

    /// <summary>This application's own folder inside the user's documents.</summary>
    public static string AppDocumentsDirectory => Read(NativeMethods.wxsharp_stdpaths_app_documents_dir);

    /// <summary>One of the user's own folders - downloads, music, pictures and so on. Following
    /// <c>wxStandardPaths.GetUserDir</c>, which is the only portable way to find these.</summary>
    public static string GetUserDirectory(UserDirectory which)
    {
        _ = App.RequireCurrent();
        return Read((byte* buffer, int length) =>
            NativeMethods.wxsharp_stdpaths_user_dir((int)which, buffer, length));
    }

    /// <summary>Where a language's resources live, which is where <see cref="Locale"/> looks for message
    /// catalogues. Follows <c>wxStandardPaths.GetLocalizedResourcesDir</c>.</summary>
    public static string GetLocalizedResourcesDirectory(string language,
        ResourceCategory category = ResourceCategory.None)
    {
        ArgumentNullException.ThrowIfNull(language);
        _ = App.RequireCurrent();
        return Read((byte* buffer, int length) =>
            NativeMethods.wxsharp_stdpaths_localized_resources_dir(language, (int)category, buffer, length));
    }

    private static unsafe string Read(ReadPath read)
    {
        _ = App.RequireCurrent();
        var length = read(null, 0);
        if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1];
        fixed (byte* buffer = bytes) _ = read(buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }

    private unsafe delegate int ReadPath(byte* buffer, int bufferLength);
}
