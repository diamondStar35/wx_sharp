using System;
using System.Text;

namespace WxSharp;

internal static class Utf8String
{
    internal static string Decode(byte[] buffer, int length)
        => length <= 0 ? string.Empty : Encoding.UTF8.GetString(buffer, 0, length);

    /// <summary>Copies a UTF-8 string out of native memory. Used for event payloads, where the native buffer
    /// is only valid for the duration of the callback.</summary>
    internal static unsafe string Decode(nint buffer, int length)
        => buffer == 0 || length <= 0 ? string.Empty : Encoding.UTF8.GetString((byte*)buffer, length);

    internal static string DecodeNullTerminated(byte[] buffer)
    {
        var end = Array.IndexOf(buffer, (byte)0);
        return Encoding.UTF8.GetString(buffer, 0, end < 0 ? buffer.Length : end);
    }
}
