namespace WxSharp;

/// <summary>Buttons and icon for <see cref="Wx.MessageBox"/>, and the button it returns. The values are
/// wxWidgets' own.</summary>
[System.Flags]
public enum MessageBoxStyle
{
    /// <summary>A Yes button. Maps to <c>wxYES</c>.</summary>
    Yes = 0x00000002,
    /// <summary>An OK button. Maps to <c>wxOK</c>.</summary>
    Ok = 0x00000004,
    /// <summary>A No button. Maps to <c>wxNO</c>.</summary>
    No = 0x00000008,
    /// <summary>Yes and No buttons together. Maps to <c>wxYES_NO</c>.</summary>
    YesNo = Yes | No,
    /// <summary>A Cancel button. Maps to <c>wxCANCEL</c>.</summary>
    Cancel = 0x00000010,
    /// <summary>An Apply button. Maps to <c>wxAPPLY</c>.</summary>
    Apply = 0x00000020,
    /// <summary>A Close button. Maps to <c>wxCLOSE</c>.</summary>
    Close = 0x00000040,
    /// <summary>With <see cref="YesNo"/>, makes No the default button.</summary>
    NoDefault = 0x00000080,
    /// <summary>A Help button, which raises a help event instead of closing the box. Maps to <c>wxHELP</c>.</summary>
    Help = 0x00001000,

    /// <summary>Show the warning icon. Maps to <c>wxICON_WARNING</c>.</summary>
    IconWarning = 0x00000100,
    /// <summary>Show the exclamation icon. Same as <see cref="IconWarning"/>; maps to <c>wxICON_EXCLAMATION</c>.</summary>
    IconExclamation = IconWarning,
    /// <summary>Show the error icon. Maps to <c>wxICON_ERROR</c>.</summary>
    IconError = 0x00000200,
    /// <summary>Show the stop-hand icon. Same as <see cref="IconError"/>; maps to <c>wxICON_HAND</c>.</summary>
    IconHand = IconError,
    /// <summary>Show the question-mark icon. Maps to <c>wxICON_QUESTION</c>.</summary>
    IconQuestion = 0x00000400,
    /// <summary>Show the information icon. Maps to <c>wxICON_INFORMATION</c>.</summary>
    IconInformation = 0x00000800,
    /// <summary>Show no icon at all. Maps to <c>wxICON_NONE</c>.</summary>
    IconNone = 0x00040000,
    /// <summary>Show the authentication-needed icon (a shield on Windows). Maps to <c>wxICON_AUTH_NEEDED</c>.</summary>
    IconAuthNeeded = 0x00080000,

    /// <summary>Centre the box on its parent rather than on the screen.</summary>
    Centre = 0x00000001,
    /// <summary>Keep the box above all other windows. Maps to <c>wxSTAY_ON_TOP</c>.</summary>
    StayOnTop = 0x00008000,
    /// <summary>With <see cref="Cancel"/>, makes Cancel the default button.</summary>
    CancelDefault = unchecked((int)0x80000000),
}
