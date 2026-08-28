namespace WxSharp;

/// <summary>Buttons and icon for <see cref="Wx.MessageBox"/>, and the button it returns. The values are
/// wxWidgets' own.</summary>
[System.Flags]
public enum MessageBoxStyle
{
    Yes = 0x00000002,
    Ok = 0x00000004,
    No = 0x00000008,
    YesNo = Yes | No,
    Cancel = 0x00000010,
    Apply = 0x00000020,
    Close = 0x00000040,
    /// <summary>With <see cref="YesNo"/>, makes No the default button.</summary>
    NoDefault = 0x00000080,
    Help = 0x00001000,

    IconWarning = 0x00000100,
    IconExclamation = IconWarning,
    IconError = 0x00000200,
    IconHand = IconError,
    IconQuestion = 0x00000400,
    IconInformation = 0x00000800,
    IconNone = 0x00040000,
    IconAuthNeeded = 0x00080000,

    /// <summary>Centre the box on its parent rather than on the screen.</summary>
    Centre = 0x00000001,
    StayOnTop = 0x00008000,
    /// <summary>With <see cref="Cancel"/>, makes Cancel the default button.</summary>
    CancelDefault = unchecked((int)0x80000000),
}
