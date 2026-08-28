using System;

namespace WxSharp;

/// <summary>A font family, following <c>wxFontFamily</c>. The values are wxWidgets' own, so nothing is
/// translated on the way across.</summary>
///
/// <remarks>
/// The names are wxWidgets': <see cref="Roman"/> is the serif family and <see cref="Swiss"/> the sans-serif
/// one. A family is what a font falls back to when no <see cref="Font.FaceName"/> is given, or when the one
/// asked for is not installed.
/// </remarks>
public enum FontFamily
{
    Default = 70,
    Decorative = 71,
    /// <summary>A serif family.</summary>
    Roman = 72,
    Script = 73,
    /// <summary>A sans-serif family.</summary>
    Swiss = 74,
    /// <summary>A fixed-pitch family suited to code and tabular text.</summary>
    Modern = 75,
    Teletype = 76,
    Unknown = 77,
}

/// <summary>How heavy a font is, following <c>wxFontWeight</c>. wxWidgets carries a numeric weight rather
/// than a handful of names, and so does this: any value in the range is valid, and the named ones are the
/// conventional stops.</summary>
public enum FontWeight
{
    Invalid = 0,
    Thin = 100,
    ExtraLight = 200,
    Light = 300,
    Normal = 400,
    Medium = 500,
    SemiBold = 600,
    Bold = 700,
    ExtraBold = 800,
    Heavy = 900,
    ExtraHeavy = 1000,
}

/// <summary>Whether a font is upright or sloped, following <c>wxFontStyle</c>.</summary>
public enum FontStyle
{
    Normal = 90,
    Italic = 93,
    /// <summary>Sloped rather than a true italic face. Most platforms treat it as italic.</summary>
    Slant = 94,
}

/// <summary>A relative size, following <c>wxFontSymbolicSize</c>. Each step is relative to the system
/// default, so a heading stays proportionate to whatever size the user has chosen.</summary>
public enum FontSymbolicSize
{
    ExtraExtraSmall = -3,
    ExtraSmall = -2,
    Small = -1,
    Medium = 0,
    Large = 1,
    ExtraLarge = 2,
    ExtraExtraLarge = 3,
}

/// <summary>The combined <c>wxFontFlag</c> bits accepted by <see cref="FontInfo.AllFlags(int)"/>.</summary>
[Flags]
public enum FontFlag
{
    Default = 0,
    Italic = 1 << 0,
    Slant = 1 << 1,
    Light = 1 << 2,
    Bold = 1 << 3,
    AntiAliased = 1 << 4,
    NotAntiAliased = 1 << 5,
    Underlined = 1 << 6,
    Strikethrough = 1 << 7,
    Mask = Italic | Slant | Light | Bold | AntiAliased | NotAntiAliased | Underlined | Strikethrough,
}

/// <summary>A character encoding, following <c>wxFontEncoding</c>. wxWidgets declares about a hundred; the
/// ones named here are those an application normally asks for, and any other value wxWidgets defines can be
/// cast to this enum and passed through unchanged.</summary>
public enum FontEncoding
{
    /// <summary>Whatever the system uses.</summary>
    System = -1,
    /// <summary>The current default, as set by <see cref="Font.DefaultEncoding"/>.</summary>
    Default = 0,
    /// <summary>West European (Latin-1), wxWidgets' <c>wxFONTENCODING_ISO8859_1</c>.</summary>
    Iso88591 = 1,
    Cp1252 = 33,
    Utf8 = 43,
}

/// <summary>Builds a <see cref="Font"/> by naming only the parts that matter, following
/// <c>wxFontInfo</c>. Every setter returns the same builder, so a font reads as one expression.</summary>
///
/// <example><code>
/// var heading = new Font(new FontInfo(14).Family(FontFamily.Swiss).Bold());
/// </code></example>
public sealed class FontInfo
{
    internal double PointSizeValue { get; private set; }
    internal Size PixelSizeValue { get; private set; } = new(-1, -1);
    internal bool UsePixels { get; private set; }
    internal FontFamily FamilyValue { get; private set; } = FontFamily.Default;
    internal FontStyle StyleValue => (FlagsValue & (int)FontFlag.Italic) != 0 ? FontStyle.Italic
        : (FlagsValue & (int)FontFlag.Slant) != 0 ? FontStyle.Slant : FontStyle.Normal;
    internal int WeightValue { get; private set; } = (int)FontWeight.Normal;
    internal bool UnderlinedValue => (FlagsValue & (int)FontFlag.Underlined) != 0;
    internal bool StrikethroughValue => (FlagsValue & (int)FontFlag.Strikethrough) != 0;
    internal string? FaceNameValue { get; private set; }
    internal FontEncoding EncodingValue { get; private set; } = FontEncoding.Default;
    internal int FlagsValue { get; private set; }

    /// <summary>A font of the system's default size, following the default <c>wxFontInfo</c> constructor.</summary>
    public FontInfo() => PointSizeValue = -1;

    /// <summary>A font of the given size in points. Fractional sizes are kept.</summary>
    public FontInfo(double pointSize) => PointSizeValue = pointSize;

    /// <summary>A font sized in pixels rather than points.</summary>
    public FontInfo(Size pixelSize) { PixelSizeValue = pixelSize; UsePixels = true; }

    public FontInfo(FontInfo other)
    {
        ArgumentNullException.ThrowIfNull(other);
        PointSizeValue = other.PointSizeValue;
        PixelSizeValue = other.PixelSizeValue;
        UsePixels = other.UsePixels;
        FamilyValue = other.FamilyValue;
        WeightValue = other.WeightValue;
        FaceNameValue = other.FaceNameValue;
        EncodingValue = other.EncodingValue;
        FlagsValue = other.FlagsValue;
    }

    public FontInfo Family(FontFamily family) { FamilyValue = family; return this; }
    public FontInfo FaceName(string faceName) { ArgumentNullException.ThrowIfNull(faceName); FaceNameValue = faceName; return this; }
    public FontInfo Style(FontStyle style)
    {
        if (style == FontStyle.Italic) Italic();
        else if (style == FontStyle.Slant) Slant();
        return this;
    }
    public FontInfo Encoding(FontEncoding encoding) { EncodingValue = encoding; return this; }

    /// <summary>Sets the numeric weight. Any value between <see cref="FontWeight.Thin"/> and
    /// <see cref="FontWeight.ExtraHeavy"/> is valid.</summary>
    public FontInfo Weight(int weight) { WeightValue = weight; return this; }

    public FontInfo Weight(FontWeight weight) { WeightValue = (int)weight; return this; }
    public FontInfo Bold(bool bold = true) { WeightValue = (int)(bold ? FontWeight.Bold : FontWeight.Normal); return this; }
    public FontInfo Light(bool light = true) { WeightValue = (int)(light ? FontWeight.Light : FontWeight.Normal); return this; }
    public FontInfo Italic(bool italic = true) { SetFlag(FontFlag.Italic, italic); return this; }
    public FontInfo Slant(bool slant = true) { SetFlag(FontFlag.Slant, slant); return this; }
    public FontInfo AntiAliased(bool antiAliased = true) { SetFlag(FontFlag.AntiAliased, antiAliased); return this; }
    public FontInfo Underlined(bool underlined = true) { SetFlag(FontFlag.Underlined, underlined); return this; }
    public FontInfo Strikethrough(bool strikethrough = true) { SetFlag(FontFlag.Strikethrough, strikethrough); return this; }

    public FontInfo AllFlags(int flags)
    {
        FlagsValue = flags;
        WeightValue = (flags & (int)FontFlag.Bold) != 0 ? (int)FontWeight.Bold
            : (flags & (int)FontFlag.Light) != 0 ? (int)FontWeight.Light : (int)FontWeight.Normal;
        return this;
    }

    public FontInfo AllFlags(FontFlag flags) => AllFlags((int)flags);

    public bool IsUsingSizeInPixels() => UsePixels;
    public double GetFractionalPointSize() => PointSizeValue;
    public int GetPointSize() => (int)Math.Round(PointSizeValue, MidpointRounding.AwayFromZero);
    public Size GetPixelSize() => PixelSizeValue;
    public bool HasFaceName() => !string.IsNullOrEmpty(FaceNameValue);
    public FontFamily GetFamily() => FamilyValue;
    public string GetFaceName() => FaceNameValue ?? string.Empty;
    public FontStyle GetStyle() => StyleValue;
    public int GetNumericWeight() => WeightValue;
    public FontWeight GetWeight() => GetWeightClosestToNumericValue(WeightValue);
    public bool IsAntiAliased() => (FlagsValue & (int)FontFlag.AntiAliased) != 0;
    public bool IsUnderlined() => UnderlinedValue;
    public bool IsStrikethrough() => StrikethroughValue;
    public FontEncoding GetEncoding() => EncodingValue;

    private void SetFlag(FontFlag flag, bool enabled)
    {
        if (enabled) FlagsValue |= (int)flag;
        else FlagsValue &= ~(int)flag;
    }

    /// <summary>The conventional weight nearest a numeric one, following
    /// <c>wxFontInfo.GetWeightClosestToNumericValue</c>.</summary>
    public static FontWeight GetWeightClosestToNumericValue(int numericWeight)
    {
        return (FontWeight)NativeMethods.wxsharp_font_weight_closest_to(numericWeight);
    }
}

/// <summary>A font, following <c>wxFont</c>.</summary>
///
/// <remarks>
/// A font is a real wxWidgets object rather than a description of one, so it carries everything wxWidgets
/// knows: fractional and pixel sizes, the numeric weight, the encoding, strikethrough, whether the face is
/// fixed-width, and the platform's own description of it. Copies share their storage, so passing one around
/// is cheap.
///
/// The derivations - <see cref="Bold()"/>, <see cref="Italic()"/>, <see cref="Larger"/>,
/// <see cref="Scaled"/> - return a new font and leave this one alone, while the <c>Make…</c> forms change
/// this one in place, exactly as wxWidgets splits them. Note that <see cref="Underlined()"/> is the
/// derivation, not a property; wxPython resolves the same collision the same way.
/// </remarks>
public sealed unsafe class Font : IDisposable
{
    private nint _handle;

    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Font));

    private Font(nint handle) => _handle = handle;

    /// <summary>Creates an invalid, uninitialised font, as Phoenix's default <c>wx.Font()</c> constructor
    /// does. Unlike every constructor that resolves a real font, this one does not require an App.</summary>
    public Font() => _handle = NativeMethods.wxsharp_font_create_empty();

    /// <summary>Adopts a font wxWidgets handed back. The caller owns it from here.</summary>
    internal static Font Attach(nint handle)
        => handle != 0 ? new(handle) : throw new InvalidOperationException("wxWidgets returned an invalid font.");

    /// <summary>Builds a font from a description.</summary>
    public Font(FontInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);
        _ = App.RequireCurrent();
        _handle = NativeMethods.wxsharp_font_create(info.PointSizeValue, info.PixelSizeValue.Width,
            info.PixelSizeValue.Height, info.UsePixels, (int)info.FamilyValue, (int)info.StyleValue,
            info.WeightValue, info.UnderlinedValue, info.StrikethroughValue, info.FaceNameValue ?? string.Empty,
            (int)info.EncodingValue, info.FlagsValue);
        if (_handle == 0) throw new InvalidOperationException("wxWidgets failed to create the font.");
    }

    /// <summary>Builds a font the way <c>wxFont</c>'s own constructor does. Note the order: style comes
    /// before weight, as it does in wxWidgets and wxPython.</summary>
    /// <param name="pointSize">Size in points.</param>
    public Font(int pointSize, FontFamily family = FontFamily.Default, FontStyle style = FontStyle.Normal,
        FontWeight weight = FontWeight.Normal, bool underlined = false, string faceName = "",
        FontEncoding encoding = FontEncoding.Default)
        : this(Describe(pointSize, family, style, weight, underlined, faceName, encoding))
    {
    }

    private static FontInfo Describe(int pointSize, FontFamily family, FontStyle style, FontWeight weight,
        bool underlined, string faceName, FontEncoding encoding)
    {
        ArgumentNullException.ThrowIfNull(faceName);
        var info = new FontInfo(pointSize).Family(family).Style(style).Weight(weight)
            .Underlined(underlined).Encoding(encoding);
        if (!string.IsNullOrEmpty(faceName)) info.FaceName(faceName);
        return info;
    }

    /// <summary>Rebuilds a font from the platform's own description, as produced by
    /// <see cref="NativeFontInfo"/>. Returns null when the description cannot be parsed - which is what a
    /// settings file written by a different platform gives you.</summary>
    public static Font? FromNativeInfo(string description)
    {
        ArgumentNullException.ThrowIfNull(description);
        _ = App.RequireCurrent();
        var handle = NativeMethods.wxsharp_font_create_from_native(description);
        return handle == 0 ? null : new Font(handle);
    }

    /// <summary>An independent copy.</summary>
    public Font Clone() => new(NativeMethods.wxsharp_font_copy(Handle));

    /// <summary>Whether the font was constructed successfully.</summary>
    public bool IsOk => _handle != 0 && NativeMethods.wxsharp_font_is_ok(_handle);

    // ---- Size -------------------------------------------------------------------------------------------

    /// <summary>The size in whole points.</summary>
    public int PointSize
    {
        get => NativeMethods.wxsharp_font_get_point_size(Handle);
        set => NativeMethods.wxsharp_font_set_point_size(Handle, value);
    }

    /// <summary>The size in points, keeping the fraction. Following
    /// <c>wxFont.GetFractionalPointSize</c>.</summary>
    public double FractionalPointSize
    {
        get => NativeMethods.wxsharp_font_get_fractional_point_size(Handle);
        set => NativeMethods.wxsharp_font_set_fractional_point_size(Handle, value);
    }

    /// <summary>The size in pixels. Setting it switches the font to pixel sizing.</summary>
    public Size PixelSize
    {
        get { NativeMethods.wxsharp_font_get_pixel_size(Handle, out var w, out var h); return new Size(w, h); }
        set => NativeMethods.wxsharp_font_set_pixel_size(Handle, value.Width, value.Height);
    }

    /// <summary>Whether this font was sized in pixels rather than points.</summary>
    public bool IsUsingSizeInPixels => NativeMethods.wxsharp_font_is_using_size_in_pixels(Handle);

    /// <summary>Sizes this font relative to the system default, so it stays proportionate to whatever size
    /// the user has chosen. Follows <c>wxFont.SetSymbolicSize</c>.</summary>
    public void SetSymbolicSize(FontSymbolicSize size)
        => NativeMethods.wxsharp_font_set_symbolic_size(Handle, (int)size);

    /// <summary>As <see cref="SetSymbolicSize"/>, but relative to a given base size in points.</summary>
    public void SetSymbolicSizeRelativeTo(FontSymbolicSize size, int basePointSize)
        => NativeMethods.wxsharp_font_set_symbolic_size_relative_to(Handle, (int)size, basePointSize);

    // ---- Description ------------------------------------------------------------------------------------

    public FontFamily Family
    {
        get => (FontFamily)NativeMethods.wxsharp_font_get_family(Handle);
        set => NativeMethods.wxsharp_font_set_family(Handle, (int)value);
    }

    public FontStyle Style
    {
        get => (FontStyle)NativeMethods.wxsharp_font_get_style(Handle);
        set => NativeMethods.wxsharp_font_set_style(Handle, (int)value);
    }

    /// <summary>The weight on wxWidgets' numeric scale, where 400 is normal and 700 is bold. Following
    /// <c>wxFont.GetNumericWeight</c>.</summary>
    public int NumericWeight
    {
        get => NativeMethods.wxsharp_font_get_numeric_weight(Handle);
        set => NativeMethods.wxsharp_font_set_numeric_weight(Handle, value);
    }

    /// <summary>The weight as one of the conventional stops, rounded to the nearest.</summary>
    public FontWeight Weight
    {
        get => (FontWeight)NativeMethods.wxsharp_font_get_weight(Handle);
        set => NativeMethods.wxsharp_font_set_weight(Handle, (int)value);
    }

    /// <summary>Whether the font is underlined. This is a property because there is a
    /// <see cref="Underlined()"/> derivation with the same name; wxPython keeps the same pair.</summary>
    public bool IsUnderlined
    {
        get => NativeMethods.wxsharp_font_get_underlined(Handle);
        set => NativeMethods.wxsharp_font_set_underlined(Handle, value);
    }

    /// <summary>Whether the font is struck through.</summary>
    public bool IsStrikethrough
    {
        get => NativeMethods.wxsharp_font_get_strikethrough(Handle);
        set => NativeMethods.wxsharp_font_set_strikethrough(Handle, value);
    }

    public FontEncoding Encoding
    {
        get => (FontEncoding)NativeMethods.wxsharp_font_get_encoding(Handle);
        set => NativeMethods.wxsharp_font_set_encoding(Handle, (int)value);
    }

    /// <summary>Whether every character in the face is the same width - what a control showing code or
    /// aligned columns needs to know.</summary>
    public bool IsFixedWidth => NativeMethods.wxsharp_font_is_fixed_width(Handle);

    /// <summary>The typeface name. Assigning one that is not installed leaves the font unchanged; use
    /// <see cref="TrySetFaceName"/> to find out which happened.</summary>
    public string FaceName
    {
        get => Read(NativeMethods.wxsharp_font_get_face_name);
        set => _ = TrySetFaceName(value);
    }

    /// <summary>Sets the typeface, reporting whether the platform has it. Following
    /// <c>wxFont.SetFaceName</c>, which is the only way to find out that a face is missing.</summary>
    public bool TrySetFaceName(string faceName)
    {
        ArgumentNullException.ThrowIfNull(faceName);
        return NativeMethods.wxsharp_font_set_face_name(Handle, faceName);
    }

    /// <summary>The platform's own complete description of this font. Store this rather than a family and
    /// size: it round-trips exactly through <see cref="FromNativeInfo"/>, where a description does not.
    /// The contents are the platform's and are not portable between them.</summary>
    public string NativeFontInfo
    {
        get => Read(NativeMethods.wxsharp_font_get_native_info);
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (!NativeMethods.wxsharp_font_set_native_info(Handle, value))
                throw new ArgumentException("The font description could not be parsed.", nameof(value));
        }
    }

    /// <summary>A human-readable form of <see cref="NativeFontInfo"/>, suitable for showing to a user.</summary>
    public string NativeFontInfoUserDesc
    {
        get => Read(NativeMethods.wxsharp_font_get_native_info_user_desc);
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (!NativeMethods.wxsharp_font_set_native_info_user_desc(Handle, value))
                throw new ArgumentException("The font description could not be parsed.", nameof(value));
        }
    }

    /// <summary>The family as a display string, e.g. for a font picker.</summary>
    public string FamilyString => Read(NativeMethods.wxsharp_font_get_family_string);
    /// <summary>The style as a display string.</summary>
    public string StyleString => Read(NativeMethods.wxsharp_font_get_style_string);
    /// <summary>The weight as a display string.</summary>
    public string WeightString => Read(NativeMethods.wxsharp_font_get_weight_string);

    // ---- Derivations, which leave this font alone -------------------------------------------------------

    public Font Bold() => new(NativeMethods.wxsharp_font_bold(Handle));
    public Font Italic() => new(NativeMethods.wxsharp_font_italic(Handle));
    public Font Underlined() => new(NativeMethods.wxsharp_font_underlined(Handle));
    public Font Strikethrough() => new(NativeMethods.wxsharp_font_strikethrough(Handle));

    /// <summary>A copy 20% larger, as <c>wxFont.Larger</c> is defined.</summary>
    public Font Larger() => new(NativeMethods.wxsharp_font_larger(Handle));

    /// <summary>A copy 20% smaller.</summary>
    public Font Smaller() => new(NativeMethods.wxsharp_font_smaller(Handle));

    public Font Scaled(float factor) => new(NativeMethods.wxsharp_font_scaled(Handle, factor));

    /// <summary>A copy with the weight, style and decoration cleared, keeping the family, size and face.</summary>
    public Font GetBaseFont() => new(NativeMethods.wxsharp_font_base(Handle));

    /// <summary>A copy at the given point size.</summary>
    public Font WithSize(int pointSize)
    {
        var copy = Clone();
        copy.PointSize = pointSize;
        return copy;
    }

    // ---- Derivations that change this font --------------------------------------------------------------

    public void MakeBold() => NativeMethods.wxsharp_font_make_bold(Handle);
    public void MakeItalic() => NativeMethods.wxsharp_font_make_italic(Handle);
    public void MakeUnderlined() => NativeMethods.wxsharp_font_make_underlined(Handle);
    public void MakeStrikethrough() => NativeMethods.wxsharp_font_make_strikethrough(Handle);
    public void MakeLarger() => NativeMethods.wxsharp_font_make_larger(Handle);
    public void MakeSmaller() => NativeMethods.wxsharp_font_make_smaller(Handle);
    public void Scale(float factor) => NativeMethods.wxsharp_font_scale(Handle, factor);

    // ---- Statics ----------------------------------------------------------------------------------------

    /// <summary>The encoding fonts use when none is given.</summary>
    public static FontEncoding DefaultEncoding
    {
        get { _ = App.RequireCurrent(); return (FontEncoding)NativeMethods.wxsharp_font_get_default_encoding(); }
        set { _ = App.RequireCurrent(); NativeMethods.wxsharp_font_set_default_encoding((int)value); }
    }

    // ---- Stock fonts ------------------------------------------------------------------------------------
    // wxWidgets exposes these as global objects that are only valid once the application exists, so they are
    // properties that build a fresh font rather than shared instances a caller could dispose out from under
    // everything else. wxPython does the same for the same reason.

    /// <summary>The font controls are normally drawn in.</summary>
    public static Font Normal => SystemSettings.GetFont(SystemFont.DefaultGui);

    /// <summary>A smaller version of the normal font.</summary>
    public static Font Small
    {
        get { var font = Normal; font.MakeSmaller(); return font; }
    }

    /// <summary>An italic version of the normal font.</summary>
    public static Font ItalicFont
    {
        get { var font = Normal; font.MakeItalic(); return font; }
    }

    /// <summary>A sans-serif font at the default size.</summary>
    public static Font Swiss => new(new FontInfo().Family(FontFamily.Swiss));

    /// <summary>The numeric weight a conventional one stands for.</summary>
    public static int GetNumericWeightOf(FontWeight weight)
    {
        return NativeMethods.wxsharp_font_numeric_weight_of((int)weight);
    }

    /// <summary>The point size a symbolic size works out to, relative to a base size.</summary>
    public static int AdjustToSymbolicSize(FontSymbolicSize size, int basePointSize)
    {
        return NativeMethods.wxsharp_font_adjust_to_symbolic_size((int)size, basePointSize);
    }

    /// <summary>Whether this wxWidgets build supports application-private fonts.</summary>
    public static bool CanUsePrivateFont => NativeMethods.wxsharp_font_can_use_private();

    /// <summary>Loads a font file for this application, following <c>wx.Font.AddPrivateFont</c>.</summary>
    public static bool AddPrivateFont(string filename)
    {
        ArgumentNullException.ThrowIfNull(filename);
        var result = NativeMethods.wxsharp_font_add_private(filename);
        if (result < 0) throw new NotImplementedException("wxWidgets was built without private-font support.");
        return result != 0;
    }

    // ---- Equality and lifetime --------------------------------------------------------------------------

    /// <summary>Whether two fonts describe the same font, as <c>wxFont</c>'s own comparison does.</summary>
    public bool Equals(Font? other)
        => other is not null && (ReferenceEquals(this, other) ||
            NativeMethods.wxsharp_font_equals(Handle, other.Handle));

    public override bool Equals(object? obj) => Equals(obj as Font);

    // wxFont has no hash of its own, and its description is what equality is defined on.
    public override int GetHashCode() => HashCode.Combine(PointSize, (int)Family, (int)Style, NumericWeight,
        IsUnderlined, IsStrikethrough, FaceName);

    public override string ToString() => NativeFontInfoUserDesc;

    public void Dispose()
    {
        if (_handle != 0) NativeMethods.wxsharp_font_destroy(_handle);
        _handle = 0;
        GC.SuppressFinalize(this);
    }

    ~Font()
    {
        // A finalizer must never let a late process/AssemblyLoadContext teardown failure escape.
        try { if (_handle != 0) NativeMethods.wxsharp_font_destroy(_handle); }
        catch (Exception) { }
    }

    private unsafe string Read(ReadString read)
    {
        var length = read(Handle, null, 0);
        if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1];
        fixed (byte* buffer = bytes) _ = read(Handle, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }

    private unsafe delegate int ReadString(nint font, byte* buffer, int bufferLength);
}
