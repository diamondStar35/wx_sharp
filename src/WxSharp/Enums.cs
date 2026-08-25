namespace WxSharp;

/// <summary>Style flags for <see cref="Wx.MessageBox"/> (mirrors the wxWidgets values).</summary>
[System.Flags]
public enum MessageBoxStyle
{
    Ok = 0x00000004,
    Yes = 0x00000002,
    No = 0x00000008,
    Cancel = 0x00000010,
    YesNo = Yes | No,
    IconInformation = 0x00000800,
    IconWarning = 0x00000100,
    IconError = 0x00000200,
    IconQuestion = 0x00000400,
}

/// <summary>The outcome of <see cref="Dialog.ShowModal"/> (the wx id the dialog ended with). Closing or
/// escaping a dialog yields <see cref="Cancel"/>.</summary>
public enum DialogResult
{
    Ok = 5100,     // wxID_OK
    Cancel = 5101, // wxID_CANCEL
}
