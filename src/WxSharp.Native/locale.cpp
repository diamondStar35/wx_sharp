// Language and translation: wxLocale, wxTranslations, and the language database behind both.
//
// wxWidgets reads GNU gettext .mo catalogs, so an application that already ships a locale/<lang>/LC_MESSAGES
// tree keeps working unchanged. wxLocale additionally sets the C runtime locale, which is why it is still
// worth having even though wxWidgets now marks it as superseded by wxTranslations.
#include "internal.h"
#include <wx/intl.h>
#include <wx/translation.h>
#include <wx/uilocale.h>

namespace
{
    wxLocale* Loc(wxsharp_handle h) { return static_cast<wxLocale*>(h); }
    wxTranslations* Tr(wxsharp_handle h) { return static_cast<wxTranslations*>(h); }

    // The languages from the last wxsharp_translations_available_count call.
    wxArrayString& LastAvailable()
    {
        static wxArrayString languages;
        return languages;
    }

    void FillLanguageInfo(const wxLanguageInfo& info, wxsharp_language_info* out)
    {
        *out = wxsharp_language_info();
        out->language = info.Language;
        out->layout_direction = static_cast<int>(info.LayoutDirection);
#ifdef __WINDOWS__
        out->win_lang = info.WinLang;
        out->win_sublang = info.WinSublang;
#endif
        CopyToBuffer(info.LocaleTag, out->locale_tag, sizeof(out->locale_tag));
        CopyToBuffer(info.CanonicalName, out->canonical_name, sizeof(out->canonical_name));
        CopyToBuffer(info.CanonicalRef, out->canonical_ref, sizeof(out->canonical_ref));
        CopyToBuffer(info.Description, out->description, sizeof(out->description));
        CopyToBuffer(info.DescriptionNative, out->description_native, sizeof(out->description_native));
    }
}

// ---- wxLocale ---------------------------------------------------------------------------------------------

wxsharp_handle wxsharp_locale_create(int language, int flags)
{
    auto* locale = new wxLocale();
    locale->Init(language, flags);
    return locale;
}

void wxsharp_locale_destroy(wxsharp_handle locale) { delete Loc(locale); }

bool wxsharp_locale_is_ok(wxsharp_handle locale) { return Loc(locale)->IsOk(); }
int wxsharp_locale_get_language(wxsharp_handle locale) { return Loc(locale)->GetLanguage(); }

int wxsharp_locale_get_name(wxsharp_handle locale, char* buffer, int buffer_length)
{
    return CopyToBuffer(Loc(locale)->GetName(), buffer, buffer_length);
}

int wxsharp_locale_get_canonical_name(wxsharp_handle locale, char* buffer, int buffer_length)
{
    return CopyToBuffer(Loc(locale)->GetCanonicalName(), buffer, buffer_length);
}

int wxsharp_locale_get_locale(wxsharp_handle locale, char* buffer, int buffer_length)
{
    return CopyToBuffer(Loc(locale)->GetLocale(), buffer, buffer_length);
}

int wxsharp_locale_get_sys_name(wxsharp_handle locale, char* buffer, int buffer_length)
{
    return CopyToBuffer(Loc(locale)->GetSysName(), buffer, buffer_length);
}

bool wxsharp_locale_add_catalog(wxsharp_handle locale, const char* domain, int msg_id_language)
{
    if (msg_id_language < 0)
        return Loc(locale)->AddCatalog(Str(domain));
    return Loc(locale)->AddCatalog(Str(domain), static_cast<wxLanguage>(msg_id_language));
}

bool wxsharp_locale_is_loaded(wxsharp_handle locale, const char* domain)
{
    return Loc(locale)->IsLoaded(Str(domain));
}

int wxsharp_locale_get_string(wxsharp_handle locale, const char* original, const char* domain,
                              char* buffer, int buffer_length)
{
    return CopyToBuffer(Loc(locale)->GetString(Str(original), Str(domain)), buffer, buffer_length);
}

int wxsharp_locale_get_string_plural(wxsharp_handle locale, const char* singular, const char* plural,
                                     unsigned int n, const char* domain, char* buffer, int buffer_length)
{
    return CopyToBuffer(Loc(locale)->GetString(Str(singular), Str(plural), n, Str(domain)),
                        buffer, buffer_length);
}

int wxsharp_locale_get_header_value(wxsharp_handle locale, const char* header, const char* domain,
                                    char* buffer, int buffer_length)
{
    return CopyToBuffer(Loc(locale)->GetHeaderValue(Str(header), Str(domain)), buffer, buffer_length);
}

// ---- The language database (all static on wxLocale) ---------------------------------------------------------

void wxsharp_locale_add_catalog_lookup_path_prefix(const char* prefix)
{
    wxLocale::AddCatalogLookupPathPrefix(Str(prefix));
}

int wxsharp_locale_get_system_language() { return wxLocale::GetSystemLanguage(); }

int wxsharp_locale_get_system_encoding_name(char* buffer, int buffer_length)
{
    return CopyToBuffer(wxLocale::GetSystemEncodingName(), buffer, buffer_length);
}

bool wxsharp_locale_is_available(int language) { return wxLocale::IsAvailable(language); }

int wxsharp_locale_get_language_name(int language, char* buffer, int buffer_length)
{
    return CopyToBuffer(wxLocale::GetLanguageName(language), buffer, buffer_length);
}

int wxsharp_locale_get_language_canonical_name(int language, char* buffer, int buffer_length)
{
    return CopyToBuffer(wxLocale::GetLanguageCanonicalName(language), buffer, buffer_length);
}

bool wxsharp_locale_get_language_info(int language, wxsharp_language_info* info)
{
    const wxLanguageInfo* found = wxLocale::GetLanguageInfo(language);
    if (found == nullptr || info == nullptr)
        return false;
    FillLanguageInfo(*found, info);
    return true;
}

// The POSIX form only: "fr", "fr_FR", or the English description. wxWidgets parses this by splitting on
// underscores and dots, so a dashed BCP 47 tag does not resolve here - that is the overload below.
bool wxsharp_locale_find_language_info(const char* text, wxsharp_language_info* info)
{
    const wxLanguageInfo* found = wxLocale::FindLanguageInfo(Str(text));
    if (found == nullptr || info == nullptr)
        return false;
    FillLanguageInfo(*found, info);
    return true;
}

// The other wxUILocale::FindLanguageInfo overload, which takes a BCP 47 tag ("pt-BR") rather than the
// POSIX form ("pt_BR") the one above wants. Two entry points because wxWidgets has two - neither accepts
// both spellings.
bool wxsharp_locale_find_language_info_by_tag(const char* tag, wxsharp_language_info* info)
{
    const wxLanguageInfo* found = wxUILocale::FindLanguageInfo(wxLocaleIdent::FromTag(Str(tag)));
    if (found == nullptr || info == nullptr)
        return false;
    FillLanguageInfo(*found, info);
    return true;
}

int wxsharp_locale_get_info(int index, int category, char* buffer, int buffer_length)
{
    return CopyToBuffer(wxLocale::GetInfo(static_cast<wxLocaleInfo>(index),
                                          static_cast<wxLocaleCategory>(category)),
                        buffer, buffer_length);
}

// The same, but read from the locale the OS is set to rather than the one the C runtime has. They usually
// agree; they differ when no locale has been set, where GetInfo falls back to "C".
int wxsharp_locale_get_os_info(int index, int category, char* buffer, int buffer_length)
{
    return CopyToBuffer(wxLocale::GetOSInfo(static_cast<wxLocaleInfo>(index),
                                            static_cast<wxLocaleCategory>(category)),
                        buffer, buffer_length);
}

// ---- wxTranslations ---------------------------------------------------------------------------------------

wxsharp_handle wxsharp_translations_get() { return wxTranslations::Get(); }

wxsharp_handle wxsharp_translations_create() { return new wxTranslations(); }

void wxsharp_translations_set(wxsharp_handle translations) { wxTranslations::Set(Tr(translations)); }

void wxsharp_translations_set_language(wxsharp_handle translations, int language)
{
    Tr(translations)->SetLanguage(static_cast<wxLanguage>(language));
}

void wxsharp_translations_set_language_named(wxsharp_handle translations, const char* language)
{
    Tr(translations)->SetLanguage(Str(language));
}

bool wxsharp_translations_add_catalog(wxsharp_handle translations, const char* domain, int msg_id_language)
{
    return Tr(translations)->AddCatalog(Str(domain), static_cast<wxLanguage>(msg_id_language));
}

bool wxsharp_translations_add_available_catalog(wxsharp_handle translations, const char* domain,
                                                int msg_id_language)
{
    return Tr(translations)->AddAvailableCatalog(Str(domain), static_cast<wxLanguage>(msg_id_language));
}

bool wxsharp_translations_add_std_catalog(wxsharp_handle translations)
{
    return Tr(translations)->AddStdCatalog();
}

bool wxsharp_translations_is_loaded(wxsharp_handle translations, const char* domain)
{
    return Tr(translations)->IsLoaded(Str(domain));
}

int wxsharp_translations_available_count(wxsharp_handle translations, const char* domain)
{
    LastAvailable() = Tr(translations)->GetAvailableTranslations(Str(domain));
    return static_cast<int>(LastAvailable().GetCount());
}

int wxsharp_translations_available_at(int index, char* buffer, int buffer_length)
{
    const wxArrayString& languages = LastAvailable();
    if (index < 0 || static_cast<size_t>(index) >= languages.GetCount())
        return 0;
    return CopyToBuffer(languages[index], buffer, buffer_length);
}

int wxsharp_translations_get_best_translation(wxsharp_handle translations, const char* domain,
                                              int msg_id_language, char* buffer, int buffer_length)
{
    return CopyToBuffer(Tr(translations)->GetBestTranslation(Str(domain),
                                                             static_cast<wxLanguage>(msg_id_language)),
                        buffer, buffer_length);
}

int wxsharp_translations_get_best_available_translation(wxsharp_handle translations, const char* domain,
                                                        char* buffer, int buffer_length)
{
    return CopyToBuffer(Tr(translations)->GetBestAvailableTranslation(Str(domain)), buffer, buffer_length);
}

// Returns -1 when no catalog has this string, which is what lets the caller tell "translated to the same
// text" apart from "not translated at all".
int wxsharp_translations_get_translated_string(wxsharp_handle translations, const char* original,
                                               const char* domain, const char* context,
                                               char* buffer, int buffer_length)
{
    const wxString* found = Tr(translations)->GetTranslatedString(Str(original), Str(domain), Str(context));
    if (found == nullptr)
        return -1;
    return CopyToBuffer(*found, buffer, buffer_length);
}

int wxsharp_translations_get_translated_string_plural(wxsharp_handle translations, const char* original,
                                                      unsigned int n, const char* domain,
                                                      const char* context, char* buffer, int buffer_length)
{
    const wxString* found = Tr(translations)->GetTranslatedString(Str(original), n, Str(domain), Str(context));
    if (found == nullptr)
        return -1;
    return CopyToBuffer(*found, buffer, buffer_length);
}

int wxsharp_translations_get_header_value(wxsharp_handle translations, const char* header, const char* domain,
                                          char* buffer, int buffer_length)
{
    return CopyToBuffer(Tr(translations)->GetHeaderValue(Str(header), Str(domain)), buffer, buffer_length);
}

void wxsharp_translations_add_lookup_prefix(const char* prefix)
{
    wxFileTranslationsLoader::AddCatalogLookupPathPrefix(Str(prefix));
}

// The free function every translated string goes through, so a caller does not have to hold a
// wxTranslations to ask for one.
int wxsharp_get_translation(const char* original, const char* domain, const char* context,
                            char* buffer, int buffer_length)
{
    return CopyToBuffer(wxGetTranslation(Str(original), Str(domain), Str(context)), buffer, buffer_length);
}

int wxsharp_get_translation_plural(const char* singular, const char* plural, unsigned int n,
                                   const char* domain, const char* context, char* buffer, int buffer_length)
{
    return CopyToBuffer(wxGetTranslation(Str(singular), Str(plural), n, Str(domain), Str(context)),
                        buffer, buffer_length);
}
