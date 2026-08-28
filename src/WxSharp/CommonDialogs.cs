using System;

namespace WxSharp;

/// <summary>How a <see cref="DirDialog"/> is built, following the <c>wxDD_</c> styles.</summary>
[Flags]
public enum DirDialogStyle
{
    /// <summary>wxWidgets' own default.</summary>
    Default = 0,
    /// <summary>Only allow a folder that already exists to be chosen.</summary>
    DirMustExist = 1,
    /// <summary>Change the process working directory to the chosen folder.</summary>
    ChangeDirectory = 2,
    /// <summary>Allow more than one folder to be chosen.</summary>
    Multiple = 4,
    /// <summary>Show hidden folders.</summary>
    ShowHidden = 8,
}

/// <summary>How a <see cref="TextEntryDialog"/> is built.</summary>
[Flags]
public enum TextEntryDialogStyle
{
    Default = 0,
    /// <summary>A box that takes more than one line.</summary>
    MultiLine = 1,
    /// <summary>Mask what is typed, for a password or key.</summary>
    Password = 2,
}

/// <summary>Asks for a file to open or save, following <c>wxFileDialog</c>.</summary>
///
/// <remarks>
/// A real dialog rather than a one-shot call: configure it, show it, and read back everything wxWidgets
/// knows - the path, the directory and file name separately, every path of a multiple selection, and which
/// wildcard filter the user picked. Dispose it when done, as with any window.
/// </remarks>
/// <example><code>
/// using var dialog = new FileDialog(frame, "Open audio", wildcard: "Audio files|*.mp3;*.wav");
/// if (dialog.ShowModal() == StandardId.Ok)
///     Load(dialog.Path);
/// </code></example>
public sealed unsafe class FileDialog : Dialog
{
    public FileDialog(Window? parent = null, string message = "Choose a file", string directory = "",
        string fileName = "", string wildcard = "", FileDialogStyle style = FileDialogStyle.Open)
        : base(parent, WindowId.Any, message, null, null, DialogStyle.Default, deferNativeCreation: true)
    {
        ArgumentNullException.ThrowIfNull(message);
        Initialize(NativeMethods.wxsharp_filedlg_create(parent?.Handle ?? 0, message, directory, fileName,
            wildcard, (int)style, Token));
    }

    /// <summary>The chosen path. With a multiple selection this is only the first; use
    /// <see cref="GetPaths"/>.</summary>
    public string Path
    {
        get => ReadDialogString(NativeMethods.wxsharp_filedlg_get_path);
        set { Verify(); NativeMethods.wxsharp_filedlg_set_path(Handle, value); }
    }

    /// <summary>The folder the chosen file is in - what the player used to have to derive from the
    /// path.</summary>
    public string Directory
    {
        get => ReadDialogString(NativeMethods.wxsharp_filedlg_get_directory);
        set { Verify(); NativeMethods.wxsharp_filedlg_set_directory(Handle, value); }
    }

    /// <summary>The chosen file's name, without its folder.</summary>
    public string FileName
    {
        get => ReadDialogString(NativeMethods.wxsharp_filedlg_get_filename);
        set { Verify(); NativeMethods.wxsharp_filedlg_set_filename(Handle, value); }
    }

    /// <summary>The filter string, in wxWidgets' own form: <c>"Audio|*.mp3;*.wav|All files|*.*"</c>.</summary>
    public string Wildcard
    {
        get => ReadDialogString(NativeMethods.wxsharp_filedlg_get_wildcard);
        set { Verify(); NativeMethods.wxsharp_filedlg_set_wildcard(Handle, value); }
    }

    /// <summary>The prompt shown to the user.</summary>
    public string Message
    {
        get => ReadDialogString(NativeMethods.wxsharp_filedlg_get_message);
        set { Verify(); NativeMethods.wxsharp_filedlg_set_message(Handle, value); }
    }

    /// <summary>Which wildcard filter is selected, counting from zero. Worth reading after the dialog
    /// closes: it is how a save dialog knows which extension the user chose.</summary>
    public int FilterIndex
    {
        get { Verify(); return NativeMethods.wxsharp_filedlg_get_filter_index(Handle); }
        set { Verify(); NativeMethods.wxsharp_filedlg_set_filter_index(Handle, value); }
    }

    /// <summary>Every chosen path. One entry unless the dialog was created with
    /// <see cref="FileDialogStyle.Multiple"/>.</summary>
    public string[] GetPaths()
    {
        Verify();
        var count = NativeMethods.wxsharp_filedlg_path_count(Handle);
        var paths = new string[count];
        for (var i = 0; i < count; i++)
        {
            var index = i;
            paths[i] = ReadDialogString((h, b, l) => NativeMethods.wxsharp_filedlg_path_at(h, index, b, l));
        }
        return paths;
    }

    /// <summary>Every chosen file name, without folders.</summary>
    public string[] GetFileNames()
    {
        Verify();
        var count = NativeMethods.wxsharp_filedlg_path_count(Handle);
        var names = new string[count];
        for (var i = 0; i < count; i++)
        {
            var index = i;
            names[i] = ReadDialogString((h, b, l) => NativeMethods.wxsharp_filedlg_filename_at(h, index, b, l));
        }
        return names;
    }
}

/// <summary>Asks for a folder, following <c>wxDirDialog</c>.</summary>
public sealed unsafe class DirDialog : Dialog
{
    public DirDialog(Window? parent = null, string message = "Choose a folder", string defaultPath = "",
        DirDialogStyle style = DirDialogStyle.Default)
        : base(parent, WindowId.Any, message, null, null, DialogStyle.Default, deferNativeCreation: true)
    {
        ArgumentNullException.ThrowIfNull(message);
        Initialize(NativeMethods.wxsharp_dirdlg_create(parent?.Handle ?? 0, message, defaultPath,
            (int)style, Token));
    }

    /// <summary>The chosen folder.</summary>
    public string Path
    {
        get => ReadDialogString(NativeMethods.wxsharp_dirdlg_get_path);
        set { Verify(); NativeMethods.wxsharp_dirdlg_set_path(Handle, value); }
    }

    /// <summary>The prompt shown to the user.</summary>
    public string Message
    {
        get => ReadDialogString(NativeMethods.wxsharp_dirdlg_get_message);
        set { Verify(); NativeMethods.wxsharp_dirdlg_set_message(Handle, value); }
    }

    /// <summary>Every chosen folder, for a dialog created with
    /// <see cref="DirDialogStyle.Multiple"/>.</summary>
    public string[] GetPaths()
    {
        Verify();
        var count = NativeMethods.wxsharp_dirdlg_path_count(Handle);
        var paths = new string[count];
        for (var i = 0; i < count; i++)
        {
            var index = i;
            paths[i] = ReadDialogString((h, b, l) => NativeMethods.wxsharp_dirdlg_path_at(h, index, b, l));
        }
        return paths;
    }
}

/// <summary>Asks for a line of text, following <c>wxTextEntryDialog</c>.</summary>
public sealed unsafe class TextEntryDialog : Dialog
{
    public TextEntryDialog(Window? parent = null, string message = "", string caption = "Input",
        string value = "", TextEntryDialogStyle style = TextEntryDialogStyle.Default)
        : base(parent, WindowId.Any, caption, null, null, DialogStyle.Default, deferNativeCreation: true)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(caption);
        Initialize(NativeMethods.wxsharp_textdlg_create(parent?.Handle ?? 0, message, caption, value,
            (int)style, Token));
    }

    /// <summary>What the user typed, or what the field starts with before the dialog is shown.</summary>
    public string Value
    {
        get => ReadDialogString(NativeMethods.wxsharp_textdlg_get_value);
        set { Verify(); NativeMethods.wxsharp_textdlg_set_value(Handle, value); }
    }

    /// <summary>Caps how much can be typed. Worth setting when the value has a limit of its own, so the
    /// user is stopped at the field rather than after they commit.</summary>
    public void SetMaxLength(ulong length) { Verify(); NativeMethods.wxsharp_textdlg_set_max_length(Handle, length); }

    /// <summary>Converts what is typed to upper case as it is entered.</summary>
    public void ForceUpper() { Verify(); NativeMethods.wxsharp_textdlg_force_upper(Handle); }
}

/// <summary>Asks for a number within a range, following <c>wxNumberEntryDialog</c>.</summary>
public sealed unsafe class NumberEntryDialog : Dialog
{
    public NumberEntryDialog(Window? parent = null, string message = "", string prompt = "",
        string caption = "Input", long value = 0, long minimum = 0, long maximum = 100)
        : base(parent, WindowId.Any, caption, null, null, DialogStyle.Default, deferNativeCreation: true)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(caption);
        Initialize(NativeMethods.wxsharp_numdlg_create(parent?.Handle ?? 0, message, prompt, caption,
            value, minimum, maximum, Token));
    }

    /// <summary>The number the user chose.</summary>
    public long Value { get { Verify(); return NativeMethods.wxsharp_numdlg_get_value(Handle); } }
}

/// <summary>Asks for a colour, following <c>wxColourDialog</c>.</summary>
public sealed unsafe class ColourDialog : Dialog
{
    /// <param name="showFull">Whether to open with the full colour picker rather than the basic palette.</param>
    public ColourDialog(Window? parent = null, Colour? initial = null, bool showFull = true)
        : base(parent, WindowId.Any, "", null, null, DialogStyle.Default, deferNativeCreation: true)
    {
        Initialize(NativeMethods.wxsharp_colourdlg_create(parent?.Handle ?? 0,
            (initial ?? Colour.Black).ToArgb(), showFull, Token));
    }

    /// <summary>The chosen colour.</summary>
    public Colour Colour
    {
        get { Verify(); return Colour.FromArgb(NativeMethods.wxsharp_colourdlg_get_colour(Handle)); }
        set { Verify(); NativeMethods.wxsharp_colourdlg_set_colour(Handle, value.ToArgb()); }
    }

    /// <summary>One of the sixteen custom colours the user can build up. Carry these between invocations,
    /// or the user's own colours are gone the next time the dialog opens.</summary>
    public Colour GetCustomColour(int index)
    {
        Verify();
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, 16);
        return Colour.FromArgb(NativeMethods.wxsharp_colourdlg_get_custom(Handle, index));
    }

    /// <summary>See <see cref="GetCustomColour"/>.</summary>
    public void SetCustomColour(int index, Colour colour)
    {
        Verify();
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, 16);
        NativeMethods.wxsharp_colourdlg_set_custom(Handle, index, colour.ToArgb());
    }
}

/// <summary>Asks for a font, following <c>wxFontDialog</c>.</summary>
public sealed unsafe class FontDialog : Dialog
{
    public FontDialog(Window? parent = null, Font? initial = null)
        : base(parent, WindowId.Any, "", null, null, DialogStyle.Default, deferNativeCreation: true)
    {
        Initialize(NativeMethods.wxsharp_fontdlg_create(parent?.Handle ?? 0, initial?.Handle ?? 0, Token));
    }

    /// <summary>The font the user chose. The caller owns it and should dispose it.</summary>
    public Font GetChosenFont()
    {
        Verify();
        return Font.Attach(NativeMethods.wxsharp_fontdlg_get_font(Handle));
    }

    /// <summary>The text colour shown alongside the font, where the platform offers one.</summary>
    public Colour Colour
    {
        get { Verify(); return Colour.FromArgb(NativeMethods.wxsharp_fontdlg_get_colour(Handle)); }
        set { Verify(); NativeMethods.wxsharp_fontdlg_set_colour(Handle, value.ToArgb()); }
    }

    /// <summary>Whether the dialog offers the colour and underline options.</summary>
    public void EnableEffects(bool enable) { Verify(); NativeMethods.wxsharp_fontdlg_enable_effects(Handle, enable); }

    /// <summary>Limits which point sizes may be chosen.</summary>
    public void SetSizeRange(int minimum, int maximum)
    {
        Verify();
        NativeMethods.wxsharp_fontdlg_set_range(Handle, minimum, maximum);
    }
}
