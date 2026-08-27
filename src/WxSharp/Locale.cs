using System;
using System.Runtime.InteropServices;

namespace WxSharp;

/// <summary>Which way a language is written, following <c>wxLayoutDirection</c>.</summary>
public enum LayoutDirection
{
    Default = 0,
    LeftToRight = 1,
    RightToLeft = 2,
}

/// <summary>A locale-dependent formatting detail, following <c>wxLocaleInfo</c>.</summary>
public enum LocaleInfo
{
    /// <summary>The digit-grouping separator.</summary>
    ThousandsSeparator = 0,

    /// <summary>The decimal point.</summary>
    DecimalPoint = 1,

    /// <summary>The <c>strftime</c> format for a short date.</summary>
    ShortDateFormat = 2,

    /// <summary>The <c>strftime</c> format for a long date.</summary>
    LongDateFormat = 3,

    /// <summary>The <c>strftime</c> format for a date and time together.</summary>
    DateTimeFormat = 4,

    /// <summary>The <c>strftime</c> format for a time.</summary>
    TimeFormat = 5,
}

/// <summary>Which kind of value a <see cref="LocaleInfo"/> is being asked about, following
/// <c>wxLocaleCategory</c>. Numbers and money are formatted differently in some languages, so the same
/// separator question has two answers.</summary>
public enum LocaleCategory
{
    Number = 0,
    Date = 1,
    Money = 2,

    /// <summary>For the values that only make sense in one category.</summary>
    Default = 3,
}

/// <summary>What a <see cref="Locale"/> loads on construction, following <c>wxLocaleInitFlags</c>.</summary>
[Flags]
public enum LocaleInitFlags
{
    /// <summary>Do not load wxWidgets' own catalogue of stock strings.</summary>
    DontLoadDefault = 0x0000,

    /// <summary>Load wxWidgets' own catalogue, so its stock dialogs and button labels are translated
    /// too.</summary>
    LoadDefault = 0x0001,
}

/// <summary>One entry in wxWidgets' language database, following <c>wxLanguageInfo</c>.</summary>
///
/// <remarks>
/// <see cref="Description"/> is the language's name in English, which is what a settings dialog usually
/// shows; <see cref="DescriptionNative"/> is its name in itself, which is what a speaker of it would rather
/// read. Listing both is the accessible choice — someone who cannot read the English name can still find
/// their language.
/// </remarks>
public sealed class LanguageInfo
{
    internal unsafe LanguageInfo(in NativeLanguageInfo native)
    {
        Language = (Language)native.Language;
        LayoutDirection = (LayoutDirection)native.LayoutDirection;
        WindowsLanguage = native.WinLang;
        WindowsSubLanguage = native.WinSublang;
        fixed (NativeLanguageInfo* p = &native)
        {
            LocaleTag = Utf8String.DecodeFixed(p->LocaleTag, NativeLanguageInfo.TagLength);
            CanonicalName = Utf8String.DecodeFixed(p->CanonicalName, NativeLanguageInfo.TagLength);
            CanonicalRef = Utf8String.DecodeFixed(p->CanonicalRef, NativeLanguageInfo.TagLength);
            Description = Utf8String.DecodeFixed(p->Description, NativeLanguageInfo.NameLength);
            DescriptionNative = Utf8String.DecodeFixed(p->DescriptionNative, NativeLanguageInfo.NameLength);
        }
    }

    public Language Language { get; }

    /// <summary>The BCP 47-style tag, such as <c>en-US</c>.</summary>
    public string LocaleTag { get; }

    /// <summary>The canonical name, such as <c>en_US</c>.</summary>
    public string CanonicalName { get; }

    /// <summary>The canonical name including a region, where <see cref="CanonicalName"/> names only the
    /// language. Empty when the region is unknown or already part of the name.</summary>
    public string CanonicalRef { get; }

    /// <summary>The language's name in English.</summary>
    public string Description { get; }

    /// <summary>The language's name in that language.</summary>
    public string DescriptionNative { get; }

    /// <summary>Which way this language is written. A right-to-left language needs the interface mirrored,
    /// which wxWidgets does for you once the locale is set.</summary>
    public LayoutDirection LayoutDirection { get; }

    /// <summary>The Win32 primary language identifier. Zero off Windows.</summary>
    public uint WindowsLanguage { get; }

    /// <summary>The Win32 sublanguage identifier. Zero off Windows.</summary>
    public uint WindowsSubLanguage { get; }

    /// <summary>Returns <see cref="CanonicalRef"/> when it is set, and <see cref="CanonicalName"/>
    /// otherwise.</summary>
    public string CanonicalWithRegion => CanonicalRef.Length > 0 ? CanonicalRef : CanonicalName;

    public override string ToString() => $"{CanonicalName} ({Description})";
}

/// <summary>The application's language, following <c>wxLocale</c>.</summary>
///
/// <remarks>
/// Creating one changes the process's locale and keeps it changed until the object is disposed, so an
/// application holds a single <see cref="Locale"/> for as long as it runs and replaces it when the user
/// picks a different language.
///
/// wxWidgets reads GNU gettext <c>.mo</c> catalogues, so a project already shipping a
/// <c>locale/&lt;lang&gt;/LC_MESSAGES/&lt;domain&gt;.mo</c> tree needs no conversion: point
/// <see cref="AddCatalogLookupPathPrefix"/> at the <c>locale</c> directory and call
/// <see cref="AddCatalog"/> with the domain.
///
/// wxWidgets now treats this class as superseded by <see cref="Translations"/>, which does the catalogue
/// half without touching the C runtime locale. <see cref="Locale"/> is still what sets number, date and
/// currency formatting, so it remains the one to use when those should follow the language too.
/// </remarks>
public sealed class Locale : IDisposable
{
    private nint _handle;

    /// <summary>Sets the process locale to a language and loads its catalogues.</summary>
    public Locale(Language language = Language.Default, LocaleInitFlags flags = LocaleInitFlags.LoadDefault)
    {
        _ = App.RequireCurrent();
        _handle = NativeMethods.wxsharp_locale_create((int)language, (int)flags);
        if (_handle == 0) throw new InvalidOperationException("wxWidgets could not create the locale.");
    }

    private nint Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_handle == 0, this);
            return _handle;
        }
    }

    /// <summary>Whether the platform actually had this locale and switched to it. A false here means the
    /// catalogues may still load while dates and numbers keep the previous formatting.</summary>
    public bool IsOk => NativeMethods.wxsharp_locale_is_ok(Handle);

    /// <summary>The language this locale was set to.</summary>
    public Language Language => (Language)NativeMethods.wxsharp_locale_get_language(Handle);

    /// <summary>The short name, such as <c>en_US</c>. The same as <see cref="CanonicalName"/>.</summary>
    public unsafe string Name => Read(NativeMethods.wxsharp_locale_get_name);

    /// <summary>The canonical name, such as <c>en_US</c>.</summary>
    public unsafe string CanonicalName => Read(NativeMethods.wxsharp_locale_get_canonical_name);

    /// <summary>The descriptive name this locale was created with.</summary>
    public unsafe string Description => Read(NativeMethods.wxsharp_locale_get_locale);

    /// <summary>The name in the form the C runtime uses.</summary>
    public unsafe string SystemName => Read(NativeMethods.wxsharp_locale_get_sys_name);

    /// <summary>Loads a message catalogue for a domain, searching the prefixes added with
    /// <see cref="AddCatalogLookupPathPrefix"/>. False when no catalogue was found.</summary>
    public bool AddCatalog(string domain, Language? messageIdLanguage = null)
        => NativeMethods.wxsharp_locale_add_catalog(Handle, domain ?? string.Empty,
            messageIdLanguage is Language language ? (int)language : -1);

    /// <summary>Whether a domain's catalogue is loaded.</summary>
    public bool IsLoaded(string domain) => NativeMethods.wxsharp_locale_is_loaded(Handle, domain ?? string.Empty);

    /// <summary>The translation of a string, or the string itself when no catalogue has it.</summary>
    public unsafe string GetString(string original, string domain = "")
    {
        original ??= string.Empty;
        domain ??= string.Empty;
        var length = NativeMethods.wxsharp_locale_get_string(Handle, original, domain, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_locale_get_string(Handle, original, domain, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>The translation of a string that has a plural, choosing the form for <paramref name="n"/>
    /// by the rule the catalogue declares. Languages disagree about how many plural forms they have, which
    /// is exactly why this cannot be done by testing <c>n == 1</c> in the caller.</summary>
    public unsafe string GetString(string singular, string plural, uint n, string domain = "")
    {
        singular ??= string.Empty;
        plural ??= string.Empty;
        domain ??= string.Empty;
        var length = NativeMethods.wxsharp_locale_get_string_plural(Handle, singular, plural, n, domain, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_locale_get_string_plural(Handle, singular, plural, n, domain, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>A field from a catalogue's <c>.po</c> header, such as <c>Plural-Forms</c>.</summary>
    public unsafe string GetHeaderValue(string header, string domain = "")
    {
        header ??= string.Empty;
        domain ??= string.Empty;
        var length = NativeMethods.wxsharp_locale_get_header_value(Handle, header, domain, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_locale_get_header_value(Handle, header, domain, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    public void Dispose()
    {
        if (_handle == 0) return;
        NativeMethods.wxsharp_locale_destroy(_handle);
        _handle = 0;
    }

    private unsafe string Read(ReadString read)
    {
        var handle = Handle;
        var length = read(handle, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = read(handle, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    private unsafe delegate int ReadString(nint handle, byte* buffer, int bufferLength);

    // ---- The language database ------------------------------------------------------------------------

    /// <summary>Adds a directory to search for message catalogues. Each is looked for under
    /// <c>prefix/&lt;lang&gt;/LC_MESSAGES</c>, <c>prefix/LC_MESSAGES</c> and <c>prefix</c>, in that order.
    /// Only affects later calls to <see cref="AddCatalog"/>.</summary>
    public static void AddCatalogLookupPathPrefix(string prefix)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_locale_add_catalog_lookup_path_prefix(prefix ?? string.Empty);
    }

    /// <summary>The language the operating system says the user prefers, or <see cref="WxSharp.Language.Unknown"/>
    /// when it cannot be worked out.</summary>
    public static Language SystemLanguage
    {
        get { _ = App.RequireCurrent(); return (Language)NativeMethods.wxsharp_locale_get_system_language(); }
    }

    /// <summary>The name of the system's default text encoding, or an empty string when unknown.</summary>
    public static unsafe string SystemEncodingName
    {
        get
        {
            _ = App.RequireCurrent();
            var length = NativeMethods.wxsharp_locale_get_system_encoding_name(null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_locale_get_system_encoding_name(p, buffer.Length);
            return Utf8String.Decode(buffer, length);
        }
    }

    /// <summary>Whether the operating system and C runtime actually provide this language. Worth checking
    /// before offering it in a language picker.</summary>
    public static bool IsAvailable(Language language)
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_locale_is_available((int)language);
    }

    /// <summary>The language's name in English, or an empty string when it is not in the database.</summary>
    public static unsafe string GetLanguageName(Language language)
    {
        _ = App.RequireCurrent();
        var length = NativeMethods.wxsharp_locale_get_language_name((int)language, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_locale_get_language_name((int)language, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>The language's ISO code, or an empty string when it is not in the database.</summary>
    public static unsafe string GetLanguageCanonicalName(Language language)
    {
        _ = App.RequireCurrent();
        var length = NativeMethods.wxsharp_locale_get_language_canonical_name((int)language, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_locale_get_language_canonical_name((int)language, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>A formatting detail of the current locale, such as the decimal point. Falls back to the US
    /// answer when the platform cannot say. Read these rather than hard-coding a separator: a build that
    /// assumes "." is the decimal point is wrong in most of Europe.</summary>
    public static unsafe string GetInfo(LocaleInfo info, LocaleCategory category = LocaleCategory.Default)
    {
        _ = App.RequireCurrent();
        var length = NativeMethods.wxsharp_locale_get_info((int)info, (int)category, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_locale_get_info((int)info, (int)category, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>The same as <see cref="GetInfo"/>, but read from the locale the operating system is set to
    /// rather than the one the C runtime has. The two agree unless no locale has been set, where
    /// <see cref="GetInfo"/> falls back to the C locale and this still reports the user's.</summary>
    public static unsafe string GetOSInfo(LocaleInfo info, LocaleCategory category = LocaleCategory.Default)
    {
        _ = App.RequireCurrent();
        var length = NativeMethods.wxsharp_locale_get_os_info((int)info, (int)category, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_locale_get_os_info((int)info, (int)category, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>Everything the database knows about a language, or null when it does not know it.</summary>
    public static unsafe LanguageInfo? GetLanguageInfo(Language language)
    {
        _ = App.RequireCurrent();
        NativeLanguageInfo native;
        return NativeMethods.wxsharp_locale_get_language_info((int)language, &native)
            ? new LanguageInfo(native)
            : null;
    }

    /// <summary>Looks up a language by its POSIX name: an ISO code (<c>fr</c>), a code with a region
    /// (<c>fr_FR</c>), or the English description. Null when nothing matches.</summary>
    ///
    /// <remarks>
    /// This is the underscore form only — <c>pt-BR</c> does not resolve here, because wxWidgets parses this
    /// argument by splitting on <c>_</c> and <c>.</c>. Use <see cref="FindLanguageInfoByTag"/> for the
    /// dashed BCP 47 form, or <see cref="FindLanguage"/> to accept either.
    /// </remarks>
    public static unsafe LanguageInfo? FindLanguageInfo(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        _ = App.RequireCurrent();
        NativeLanguageInfo native;
        return NativeMethods.wxsharp_locale_find_language_info(text, &native)
            ? new LanguageInfo(native)
            : null;
    }

    /// <summary>Looks up a language by its BCP 47 tag, such as <c>pt-BR</c> or <c>zh-Hant</c>. This is the
    /// form a web request or a modern configuration file uses. Null when nothing matches.</summary>
    public static unsafe LanguageInfo? FindLanguageInfoByTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return null;
        _ = App.RequireCurrent();
        NativeLanguageInfo native;
        return NativeMethods.wxsharp_locale_find_language_info_by_tag(tag, &native)
            ? new LanguageInfo(native)
            : null;
    }

    /// <summary>Looks up a language written either way, trying <see cref="FindLanguageInfo"/> and then
    /// <see cref="FindLanguageInfoByTag"/>. wxWidgets keeps the two spellings apart and neither entry point
    /// accepts both, so this is here for the common case of a language code read from a settings file whose
    /// spelling is not known in advance.</summary>
    public static LanguageInfo? FindLanguage(string text)
        => FindLanguageInfo(text) ?? FindLanguageInfoByTag(text);
}

/// <summary>The flat form of <see cref="LanguageInfo"/> that crosses the native boundary.</summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeLanguageInfo
{
    internal const int TagLength = 64;
    internal const int NameLength = 128;

    public int Language;
    public int LayoutDirection;
    public uint WinLang;
    public uint WinSublang;
    public fixed byte LocaleTag[TagLength];
    public fixed byte CanonicalName[TagLength];
    public fixed byte CanonicalRef[TagLength];
    public fixed byte Description[NameLength];
    public fixed byte DescriptionNative[NameLength];
}
