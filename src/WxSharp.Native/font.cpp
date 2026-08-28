// wxFont.
//
// A font crosses this boundary as a handle rather than as a description. It used to be six loose scalars,
// which cost three duplicated family/style/weight mapping tables and silently lost everything wxWidgets
// knows that those six could not carry: fractional and pixel sizes, encodings, strikethrough, the real
// numeric weight, and whether the face is fixed-width. The family, style, weight and encoding values are
// wxWidgets' own here, so nothing has to be translated on either side.
//
// wxFont is a wxGDIObject and copies share their data, so handing one across is a cheap copy, not a
// duplicate of the font's storage.
#include "internal.h"
#include <wx/settings.h>
#include <wx/fontenum.h>

// These values cross the ABI without translation. Fail the native build immediately if a future wxWidgets
// version changes one instead of silently interpreting a managed enum as another encoding or font style.
static_assert(wxFONTFAMILY_DEFAULT == 70 && wxFONTFAMILY_UNKNOWN == 77);
static_assert(wxFONTSTYLE_NORMAL == 90 && wxFONTSTYLE_ITALIC == 93 && wxFONTSTYLE_SLANT == 94);
static_assert(wxFONTWEIGHT_NORMAL == 400 && wxFONTWEIGHT_BOLD == 700 && wxFONTWEIGHT_EXTRAHEAVY == 1000);
static_assert(wxFONTFLAG_ITALIC == 1 && wxFONTFLAG_SLANT == 2 && wxFONTFLAG_LIGHT == 4 &&
              wxFONTFLAG_BOLD == 8 && wxFONTFLAG_ANTIALIASED == 16 &&
              wxFONTFLAG_NOT_ANTIALIASED == 32 && wxFONTFLAG_UNDERLINED == 64 &&
              wxFONTFLAG_STRIKETHROUGH == 128);
static_assert(wxFONTENCODING_SYSTEM == -1 && wxFONTENCODING_DEFAULT == 0);
static_assert(wxFONTENCODING_ISO8859_1 == 1 && wxFONTENCODING_CP1252 == 33 && wxFONTENCODING_UTF8 == 43);

namespace
{
    wxFont* Font(wxsharp_handle handle) { return static_cast<wxFont*>(handle); }

    // Every derivation returns a new font the caller owns, which is what the managed side disposes.
    wxsharp_handle Own(const wxFont& font) { return new wxFont(font); }
}

wxsharp_handle wxsharp_font_create_empty() { return new wxFont(); }

wxsharp_handle wxsharp_font_create(double point_size, int pixel_width, int pixel_height, bool use_pixels,
                                   int family, int style, int weight, bool underlined, bool strikethrough,
                                   const char* face, int encoding, int flags)
{
    wxFontInfo info = use_pixels ? wxFontInfo(wxSize(pixel_width, pixel_height))
                                 : point_size == -1 ? wxFontInfo() : wxFontInfo(point_size);
    info.Family(static_cast<wxFontFamily>(family))
        .AllFlags(flags)
        .Weight(weight)
        .Encoding(static_cast<wxFontEncoding>(encoding));
    (void)style;
    (void)underlined;
    (void)strikethrough;
    if (face && *face)
        info.FaceName(Str(face));
    return new wxFont(info);
}

wxsharp_handle wxsharp_font_create_from_native(const char* native_info)
{
    auto* font = new wxFont(Str(native_info));
    if (!font->IsOk()) { delete font; return nullptr; }
    return font;
}

wxsharp_handle wxsharp_font_copy(wxsharp_handle font) { return Own(*Font(font)); }
void wxsharp_font_destroy(wxsharp_handle font) { delete Font(font); }
bool wxsharp_font_is_ok(wxsharp_handle font) { return Font(font)->IsOk(); }
bool wxsharp_font_equals(wxsharp_handle a, wxsharp_handle b) { return *Font(a) == *Font(b); }

// ---- Size -------------------------------------------------------------------------------------------

int    wxsharp_font_get_point_size(wxsharp_handle font) { return Font(font)->GetPointSize(); }
void   wxsharp_font_set_point_size(wxsharp_handle font, int size) { Font(font)->SetPointSize(size); }
double wxsharp_font_get_fractional_point_size(wxsharp_handle font) { return Font(font)->GetFractionalPointSize(); }
void   wxsharp_font_set_fractional_point_size(wxsharp_handle font, double size) { Font(font)->SetFractionalPointSize(size); }
bool   wxsharp_font_is_using_size_in_pixels(wxsharp_handle font) { return Font(font)->IsUsingSizeInPixels(); }

void wxsharp_font_get_pixel_size(wxsharp_handle font, int* width, int* height)
{
    const wxSize size = Font(font)->GetPixelSize();
    if (width) *width = size.x;
    if (height) *height = size.y;
}

void wxsharp_font_set_pixel_size(wxsharp_handle font, int width, int height)
{
    Font(font)->SetPixelSize(wxSize(width, height));
}

void wxsharp_font_set_symbolic_size(wxsharp_handle font, int size)
{
    Font(font)->SetSymbolicSize(static_cast<wxFontSymbolicSize>(size));
}

void wxsharp_font_set_symbolic_size_relative_to(wxsharp_handle font, int size, int base)
{
    Font(font)->SetSymbolicSizeRelativeTo(static_cast<wxFontSymbolicSize>(size), base);
}

// ---- Description ------------------------------------------------------------------------------------

int  wxsharp_font_get_family(wxsharp_handle font) { return static_cast<int>(Font(font)->GetFamily()); }
void wxsharp_font_set_family(wxsharp_handle font, int family) { Font(font)->SetFamily(static_cast<wxFontFamily>(family)); }
int  wxsharp_font_get_style(wxsharp_handle font) { return static_cast<int>(Font(font)->GetStyle()); }
void wxsharp_font_set_style(wxsharp_handle font, int style) { Font(font)->SetStyle(static_cast<wxFontStyle>(style)); }
int  wxsharp_font_get_numeric_weight(wxsharp_handle font) { return Font(font)->GetNumericWeight(); }
void wxsharp_font_set_numeric_weight(wxsharp_handle font, int weight) { Font(font)->SetNumericWeight(weight); }
int  wxsharp_font_get_weight(wxsharp_handle font) { return static_cast<int>(Font(font)->GetWeight()); }
void wxsharp_font_set_weight(wxsharp_handle font, int weight) { Font(font)->SetWeight(static_cast<wxFontWeight>(weight)); }
bool wxsharp_font_get_underlined(wxsharp_handle font) { return Font(font)->GetUnderlined(); }
void wxsharp_font_set_underlined(wxsharp_handle font, bool value) { Font(font)->SetUnderlined(value); }
bool wxsharp_font_get_strikethrough(wxsharp_handle font) { return Font(font)->GetStrikethrough(); }
void wxsharp_font_set_strikethrough(wxsharp_handle font, bool value) { Font(font)->SetStrikethrough(value); }
int  wxsharp_font_get_encoding(wxsharp_handle font) { return static_cast<int>(Font(font)->GetEncoding()); }
void wxsharp_font_set_encoding(wxsharp_handle font, int encoding) { Font(font)->SetEncoding(static_cast<wxFontEncoding>(encoding)); }
bool wxsharp_font_is_fixed_width(wxsharp_handle font) { return Font(font)->IsFixedWidth(); }

int wxsharp_font_get_face_name(wxsharp_handle font, char* buffer, int buffer_length)
{
    return CopyToBuffer(Font(font)->GetFaceName(), buffer, buffer_length);
}

bool wxsharp_font_set_face_name(wxsharp_handle font, const char* face)
{
    return Font(font)->SetFaceName(Str(face));
}

// The platform's own description of the font, which is what a settings file should store: it round-trips
// exactly, where a family-and-size description only approximates.
int wxsharp_font_get_native_info(wxsharp_handle font, char* buffer, int buffer_length)
{
    return CopyToBuffer(Font(font)->GetNativeFontInfoDesc(), buffer, buffer_length);
}

int wxsharp_font_get_native_info_user_desc(wxsharp_handle font, char* buffer, int buffer_length)
{
    return CopyToBuffer(Font(font)->GetNativeFontInfoUserDesc(), buffer, buffer_length);
}

bool wxsharp_font_set_native_info(wxsharp_handle font, const char* description)
{
    return Font(font)->SetNativeFontInfo(Str(description));
}

bool wxsharp_font_set_native_info_user_desc(wxsharp_handle font, const char* description)
{
    return Font(font)->SetNativeFontInfoUserDesc(Str(description));
}

int wxsharp_font_get_family_string(wxsharp_handle font, char* buffer, int buffer_length)
{
    return CopyToBuffer(Font(font)->GetFamilyString(), buffer, buffer_length);
}

int wxsharp_font_get_style_string(wxsharp_handle font, char* buffer, int buffer_length)
{
    return CopyToBuffer(Font(font)->GetStyleString(), buffer, buffer_length);
}

int wxsharp_font_get_weight_string(wxsharp_handle font, char* buffer, int buffer_length)
{
    return CopyToBuffer(Font(font)->GetWeightString(), buffer, buffer_length);
}

// ---- Derivations ------------------------------------------------------------------------------------
// wxWidgets has both an in-place MakeBold() and a copy-returning Bold(). Both are kept, because a caller
// adjusting a control's own font wants the copy and one building a font up wants the mutation.

wxsharp_handle wxsharp_font_bold(wxsharp_handle font) { return Own(Font(font)->Bold()); }
wxsharp_handle wxsharp_font_italic(wxsharp_handle font) { return Own(Font(font)->Italic()); }
wxsharp_handle wxsharp_font_underlined(wxsharp_handle font) { return Own(Font(font)->Underlined()); }
wxsharp_handle wxsharp_font_strikethrough(wxsharp_handle font) { return Own(Font(font)->Strikethrough()); }
wxsharp_handle wxsharp_font_larger(wxsharp_handle font) { return Own(Font(font)->Larger()); }
wxsharp_handle wxsharp_font_smaller(wxsharp_handle font) { return Own(Font(font)->Smaller()); }
wxsharp_handle wxsharp_font_scaled(wxsharp_handle font, float factor) { return Own(Font(font)->Scaled(factor)); }
wxsharp_handle wxsharp_font_base(wxsharp_handle font) { return Own(Font(font)->GetBaseFont()); }

void wxsharp_font_make_bold(wxsharp_handle font) { Font(font)->MakeBold(); }
void wxsharp_font_make_italic(wxsharp_handle font) { Font(font)->MakeItalic(); }
void wxsharp_font_make_underlined(wxsharp_handle font) { Font(font)->MakeUnderlined(); }
void wxsharp_font_make_strikethrough(wxsharp_handle font) { Font(font)->MakeStrikethrough(); }
void wxsharp_font_make_larger(wxsharp_handle font) { Font(font)->MakeLarger(); }
void wxsharp_font_make_smaller(wxsharp_handle font) { Font(font)->MakeSmaller(); }
void wxsharp_font_scale(wxsharp_handle font, float factor) { Font(font)->Scale(factor); }

// ---- Statics ----------------------------------------------------------------------------------------

int  wxsharp_font_get_default_encoding() { return static_cast<int>(wxFont::GetDefaultEncoding()); }
void wxsharp_font_set_default_encoding(int encoding) { wxFont::SetDefaultEncoding(static_cast<wxFontEncoding>(encoding)); }
int  wxsharp_font_numeric_weight_of(int weight) { return wxFont::GetNumericWeightOf(static_cast<wxFontWeight>(weight)); }
int  wxsharp_font_weight_closest_to(int numeric_weight) { return static_cast<int>(wxFontInfo::GetWeightClosestToNumericValue(numeric_weight)); }
int  wxsharp_font_adjust_to_symbolic_size(int size, int base) { return wxFont::AdjustToSymbolicSize(static_cast<wxFontSymbolicSize>(size), base); }

int wxsharp_font_add_private(const char* filename)
{
#if wxUSE_PRIVATE_FONTS
    return wxFont::AddPrivateFont(Str(filename)) ? 1 : 0;
#else
    (void)filename;
    return -1;
#endif
}

bool wxsharp_font_can_use_private()
{
#if wxUSE_PRIVATE_FONTS
    return true;
#else
    return false;
#endif
}

// The fonts the platform itself uses. A themed interface has to start from these rather than from a
// hard-coded family and size, or it stops following the user's chosen font.
wxsharp_handle wxsharp_font_from_system(int which)
{
    return Own(wxSystemSettings::GetFont(static_cast<wxSystemFont>(which)));
}

// ---- wxFontEnumerator ---------------------------------------------------------------------------------
// wxWidgets exposes this as a class to subclass, with OnFacename/OnFontEncoding callbacks, plus statics
// that collect the results into an array. The statics are the whole of what an application needs - listing
// faces for a picker, or checking one before using it - so only those are wrapped, which is also the shape
// wxPython recommends. Results are held until the next call so a caller can size its own buffer exactly,
// the same way the file dialog hands back a multiple selection.
namespace
{
    wxArrayString g_font_names;
}

int wxsharp_font_enumerate_facenames(int encoding, bool fixed_width_only)
{
    g_font_names = wxFontEnumerator::GetFacenames(static_cast<wxFontEncoding>(encoding), fixed_width_only);
    return static_cast<int>(g_font_names.GetCount());
}

int wxsharp_font_enumerate_encodings(const char* facename)
{
    g_font_names = wxFontEnumerator::GetEncodings(Str(facename));
    return static_cast<int>(g_font_names.GetCount());
}

int wxsharp_font_enumerated_name(int index, char* buffer, int buffer_length)
{
    if (index < 0 || static_cast<size_t>(index) >= g_font_names.GetCount())
        return 0;
    return CopyToBuffer(g_font_names[static_cast<size_t>(index)], buffer, buffer_length);
}

bool wxsharp_font_is_valid_facename(const char* facename)
{
    return wxFontEnumerator::IsValidFacename(Str(facename));
}

void wxsharp_font_invalidate_enumeration_cache() { wxFontEnumerator::InvalidateCache(); }

