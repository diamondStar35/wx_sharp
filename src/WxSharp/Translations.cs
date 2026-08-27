using System;

namespace WxSharp;

/// <summary>Message catalogues, following <c>wxTranslations</c>.</summary>
///
/// <remarks>
/// This is the half of <see cref="Locale"/> that does the actual translating, without changing the C
/// runtime locale. wxWidgets reads GNU gettext <c>.mo</c> files, so a project already shipping a
/// <c>locale/&lt;lang&gt;/LC_MESSAGES/&lt;domain&gt;.mo</c> tree can point
/// <see cref="AddCatalogLookupPathPrefix"/> at it and carry on:
///
/// <code>
/// Translations.AddCatalogLookupPathPrefix(Path.Combine(AppContext.BaseDirectory, "locale"));
/// var translations = new Translations();
/// translations.SetLanguage("fr");
/// translations.AddCatalog("myapp");
/// Translations.Current = translations;
///
/// // and then, wherever a string is shown:
/// static string _(string text) => Translations.Get(text);
/// </code>
///
/// A translation is looked up in every loaded catalogue unless a domain is named, latest first.
/// </remarks>
public sealed class Translations
{
    private readonly nint _handle;

    /// <summary>Creates an empty set of catalogues. Assign it to <see cref="Current"/> for
    /// <see cref="Get(string, string, string)"/> to use it.</summary>
    public Translations()
    {
        _ = App.RequireCurrent();
        _handle = NativeMethods.wxsharp_translations_create();
        if (_handle == 0) throw new InvalidOperationException("wxWidgets could not create the translations.");
    }

    private Translations(nint handle) => _handle = handle;

    /// <summary>The set of catalogues the application is currently using, if any.</summary>
    ///
    /// <remarks>
    /// Assigning hands ownership to wxWidgets, which destroys whatever was there before — so do not keep
    /// using an instance after replacing it. A <see cref="Locale"/> installs its own while it lives, and
    /// reading this while one exists returns that.
    /// </remarks>
    public static Translations? Current
    {
        get
        {
            _ = App.RequireCurrent();
            var handle = NativeMethods.wxsharp_translations_get();
            return handle == 0 ? null : new Translations(handle);
        }
        set
        {
            _ = App.RequireCurrent();
            NativeMethods.wxsharp_translations_set(value?._handle ?? 0);
        }
    }

    /// <summary>Chooses which language to load catalogues for. Pass
    /// <see cref="WxSharp.Language.Default"/> to follow the operating system.</summary>
    public void SetLanguage(Language language)
        => NativeMethods.wxsharp_translations_set_language(_handle, (int)language);

    /// <summary>Chooses the language by canonical name, such as <c>fr</c> or <c>pt_BR</c>. An empty string
    /// means the system default.</summary>
    public void SetLanguage(string language)
        => NativeMethods.wxsharp_translations_set_language_named(_handle, language ?? string.Empty);

    /// <summary>Loads a domain's catalogue. Also returns true when no catalogue is needed because the
    /// chosen language is the one the messages are already written in — use
    /// <see cref="AddAvailableCatalog"/> to require that a file was actually found.</summary>
    public bool AddCatalog(string domain, Language messageIdLanguage = Language.EnglishUs)
        => NativeMethods.wxsharp_translations_add_catalog(_handle, domain ?? string.Empty, (int)messageIdLanguage);

    /// <summary>Loads a domain's catalogue, returning false unless a file was found.</summary>
    public bool AddAvailableCatalog(string domain, Language messageIdLanguage = Language.EnglishUs)
        => NativeMethods.wxsharp_translations_add_available_catalog(_handle, domain ?? string.Empty,
            (int)messageIdLanguage);

    /// <summary>Loads wxWidgets' own catalogue, so its stock dialogs and button labels are translated
    /// too.</summary>
    public bool AddStdCatalog() => NativeMethods.wxsharp_translations_add_std_catalog(_handle);

    /// <summary>Whether a domain's catalogue is loaded.</summary>
    public bool IsLoaded(string domain)
        => NativeMethods.wxsharp_translations_is_loaded(_handle, domain ?? string.Empty);

    /// <summary>Every language this application ships a catalogue for. This is what a language picker
    /// should be built from rather than listing the <c>locale</c> directory by hand.</summary>
    public unsafe string[] GetAvailableTranslations(string domain)
    {
        var count = NativeMethods.wxsharp_translations_available_count(_handle, domain ?? string.Empty);
        if (count <= 0) return Array.Empty<string>();

        var languages = new string[count];
        for (var i = 0; i < count; ++i)
        {
            var length = NativeMethods.wxsharp_translations_available_at(i, null, 0);
            if (length <= 0) { languages[i] = string.Empty; continue; }
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_translations_available_at(i, p, buffer.Length);
            languages[i] = Utf8String.Decode(buffer, length);
        }
        return languages;
    }

    /// <summary>The language whose catalogue best matches what the user prefers, taking the messages'
    /// own language into account. Empty when nothing suits.</summary>
    public unsafe string GetBestTranslation(string domain, Language messageIdLanguage = Language.EnglishUs)
    {
        domain ??= string.Empty;
        var length = NativeMethods.wxsharp_translations_get_best_translation(_handle, domain,
            (int)messageIdLanguage, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_translations_get_best_translation(_handle, domain,
                (int)messageIdLanguage, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>The best match among the catalogues that actually exist, ignoring the messages' own
    /// language. Empty when none matches.</summary>
    public unsafe string GetBestAvailableTranslation(string domain)
    {
        domain ??= string.Empty;
        var length = NativeMethods.wxsharp_translations_get_best_available_translation(_handle, domain, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_translations_get_best_available_translation(_handle, domain, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>The translation of a string, or null when no catalogue has it. Unlike
    /// <see cref="Get(string, string, string)"/> this distinguishes "translated to the same words" from
    /// "not translated at all", which is what a completeness check needs.</summary>
    public unsafe string? GetTranslatedString(string original, string domain = "", string context = "")
    {
        original ??= string.Empty;
        domain ??= string.Empty;
        context ??= string.Empty;
        var length = NativeMethods.wxsharp_translations_get_translated_string(_handle, original, domain,
            context, null, 0);
        if (length < 0) return null;
        if (length == 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_translations_get_translated_string(_handle, original, domain, context,
                p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>The plural form of a translation for <paramref name="n"/>, or null when no catalogue has
    /// the string.</summary>
    public unsafe string? GetTranslatedString(string original, uint n, string domain = "", string context = "")
    {
        original ??= string.Empty;
        domain ??= string.Empty;
        context ??= string.Empty;
        var length = NativeMethods.wxsharp_translations_get_translated_string_plural(_handle, original, n,
            domain, context, null, 0);
        if (length < 0) return null;
        if (length == 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_translations_get_translated_string_plural(_handle, original, n, domain,
                context, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>A field from a catalogue's <c>.po</c> header, such as <c>Plural-Forms</c>.</summary>
    public unsafe string GetHeaderValue(string header, string domain = "")
    {
        header ??= string.Empty;
        domain ??= string.Empty;
        var length = NativeMethods.wxsharp_translations_get_header_value(_handle, header, domain, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_translations_get_header_value(_handle, header, domain, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    // ---- The free functions ---------------------------------------------------------------------------

    /// <summary>Adds a directory to search for message catalogues. Each is looked for under
    /// <c>prefix/&lt;lang&gt;/LC_MESSAGES</c>, <c>prefix/LC_MESSAGES</c> and <c>prefix</c>, in that
    /// order.</summary>
    public static void AddCatalogLookupPathPrefix(string prefix)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_translations_add_lookup_prefix(prefix ?? string.Empty);
    }

    /// <summary>The translation of a string, following <c>wxGetTranslation</c>. Returns the string itself
    /// when there is no translation, so it is safe to wrap every user-visible string in it from the
    /// start.</summary>
    ///
    /// <remarks>
    /// <paramref name="context"/> disambiguates a word that translates differently depending on where it
    /// appears — "Open" as a menu command and "Open" as a state, for instance, which many languages render
    /// differently and English does not.
    /// </remarks>
    public static unsafe string Get(string original, string domain = "", string context = "")
    {
        original ??= string.Empty;
        domain ??= string.Empty;
        context ??= string.Empty;
        _ = App.RequireCurrent();
        var length = NativeMethods.wxsharp_get_translation(original, domain, context, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_get_translation(original, domain, context, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    /// <summary>The translation of a string with a plural, choosing the form for <paramref name="n"/> by
    /// the rule the catalogue declares. Languages disagree about how many plural forms they have and where
    /// the boundaries fall, which is why this cannot be replaced by testing <c>n == 1</c> in the
    /// caller.</summary>
    public static unsafe string Get(string singular, string plural, uint n, string domain = "",
        string context = "")
    {
        singular ??= string.Empty;
        plural ??= string.Empty;
        domain ??= string.Empty;
        context ??= string.Empty;
        _ = App.RequireCurrent();
        var length = NativeMethods.wxsharp_get_translation_plural(singular, plural, n, domain, context, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_get_translation_plural(singular, plural, n, domain, context, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
}
