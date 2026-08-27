using System;
using System.Runtime.InteropServices;

namespace WxSharp;

/// <summary>Which parts of a <see cref="TextAttr"/> are actually set, following <c>wxTextAttrFlags</c>.
/// Anything left unset is inherited from the control rather than overridden.</summary>
[Flags]
public enum TextAttrFlags : uint
{
    None = 0,
    TextColour = 0x00000001,
    BackgroundColour = 0x00000002,
    FontFace = 0x00000004,
    FontPointSize = 0x00000008,
    FontWeight = 0x00000010,
    FontItalic = 0x00000020,
    FontUnderline = 0x00000040,
    Alignment = 0x00000080,
    LeftIndent = 0x00000100,
    RightIndent = 0x00000200,
    FontStrikethrough = 0x08000000,
    FontEncoding = 0x02000000,
    FontFamily = 0x04000000,
    FontPixelSize = 0x10000000,

    FontSize = FontPointSize | FontPixelSize,
    Font = FontFace | FontSize | FontWeight | FontItalic | FontUnderline | FontStrikethrough
         | FontEncoding | FontFamily,
}

/// <summary>How a paragraph is aligned, following <c>wxTextAttrAlignment</c>.</summary>
public enum TextAttrAlignment
{
    Default = 0,
    Left = 1,
    Centre = 2,
    Center = Centre,
    Right = 3,
    Justified = 4,
}

/// <summary>A character and paragraph style for a rich text control, following <c>wxTextAttr</c>.</summary>
///
/// <remarks>
/// A style only overrides what it explicitly sets. Assigning any property marks it as set; whatever is left
/// alone keeps coming from the control, which is what lets a style be applied to a range without disturbing
/// the rest of its appearance.
/// </remarks>
public sealed class TextAttr
{
    private Colour _textColour;
    private Colour _backgroundColour;
    private TextAttrAlignment _alignment;
    private int _leftIndent, _leftSubIndent, _rightIndent;
    private Font? _font;

    /// <summary>Which properties this style overrides.</summary>
    public TextAttrFlags Flags { get; private set; }

    /// <summary>Whether this style overrides nothing at all.</summary>
    public bool IsDefault => Flags == TextAttrFlags.None;

    public Colour TextColour
    {
        get => _textColour;
        set { _textColour = value; Flags |= TextAttrFlags.TextColour; }
    }

    public Colour BackgroundColour
    {
        get => _backgroundColour;
        set { _backgroundColour = value; Flags |= TextAttrFlags.BackgroundColour; }
    }

    public TextAttrAlignment Alignment
    {
        get => _alignment;
        set { _alignment = value; Flags |= TextAttrFlags.Alignment; }
    }

    /// <summary>The paragraph's left margin in tenths of a millimetre. The second value shifts the first
    /// line relative to the rest, which is how hanging indents are expressed.</summary>
    public int LeftIndent
    {
        get => _leftIndent;
        set { _leftIndent = value; Flags |= TextAttrFlags.LeftIndent; }
    }

    public int LeftSubIndent
    {
        get => _leftSubIndent;
        set { _leftSubIndent = value; Flags |= TextAttrFlags.LeftIndent; }
    }

    public int RightIndent
    {
        get => _rightIndent;
        set { _rightIndent = value; Flags |= TextAttrFlags.RightIndent; }
    }

    /// <summary>The typeface for the range. Setting it marks every font property as overridden; use
    /// <see cref="SetFont"/> to override only some of them.</summary>
    public Font? Font
    {
        get => _font;
        set => SetFont(value, TextAttrFlags.Font);
    }

    /// <summary>Applies a font but overrides only the properties named in <paramref name="which"/>, leaving
    /// the others to come from the control.</summary>
    public void SetFont(Font? font, TextAttrFlags which = TextAttrFlags.Font)
    {
        _font = font;
        if (font is null) Flags &= ~TextAttrFlags.Font;
        else Flags |= which & TextAttrFlags.Font;
    }

    public bool Has(TextAttrFlags flag) => (Flags & flag) == flag;

    internal unsafe NativeTextAttr ToNative()
    {
        var native = default(NativeTextAttr);
        native.Flags = (uint)Flags;
        native.TextColour = _textColour.ToArgb();
        native.BackgroundColour = _backgroundColour.ToArgb();
        native.Alignment = (int)_alignment;
        native.LeftIndent = _leftIndent;
        native.LeftSubIndent = _leftSubIndent;
        native.RightIndent = _rightIndent;
        if (_font is not null)
        {
            native.FontPointSize = _font.PointSize;
            native.FontFamily = (int)_font.Family;
            native.FontWeight = (int)_font.Weight;
            native.FontStyle = (int)_font.Style;
            native.FontUnderline = _font.Underline ? 1 : 0;
            Utf8String.CopyInto(_font.Face, native.FontFace, NativeTextAttr.FaceLength);
        }
        return native;
    }

    internal static unsafe TextAttr FromNative(in NativeTextAttr native)
    {
        var attr = new TextAttr
        {
            _textColour = Colour.FromArgb(native.TextColour),
            _backgroundColour = Colour.FromArgb(native.BackgroundColour),
            _alignment = (TextAttrAlignment)native.Alignment,
            _leftIndent = native.LeftIndent,
            _leftSubIndent = native.LeftSubIndent,
            _rightIndent = native.RightIndent,
            Flags = (TextAttrFlags)native.Flags,
        };
        if ((attr.Flags & TextAttrFlags.Font) != 0)
        {
            fixed (NativeTextAttr* p = &native)
            {
                attr._font = new Font(native.FontPointSize, (FontFamily)native.FontFamily,
                    (FontWeight)native.FontWeight, (FontStyle)native.FontStyle, native.FontUnderline != 0,
                    Utf8String.DecodeFixed(p->FontFace, NativeTextAttr.FaceLength));
            }
        }
        return attr;
    }
}

/// <summary>The flat form of <see cref="TextAttr"/> that crosses the native boundary.</summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeTextAttr
{
    internal const int FaceLength = 128;

    public uint Flags;
    public uint TextColour;
    public uint BackgroundColour;
    public int Alignment;
    public int LeftIndent;
    public int LeftSubIndent;
    public int RightIndent;
    public int FontPointSize;
    public int FontFamily;
    public int FontWeight;
    public int FontStyle;
    public int FontUnderline;
    public fixed byte FontFace[FaceLength];
}
