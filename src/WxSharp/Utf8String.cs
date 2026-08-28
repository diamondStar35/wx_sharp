using System;
using System.Runtime.InteropServices;
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

    /// <summary>Reads a null-terminated UTF-8 string out of a fixed-size field in a native struct.</summary>
    internal static unsafe string DecodeFixed(byte* buffer, int capacity)
    {
        if (buffer == null) return string.Empty;
        var length = 0;
        while (length < capacity && buffer[length] != 0) ++length;
        return length == 0 ? string.Empty : Encoding.UTF8.GetString(buffer, length);
    }

    /// <summary>Writes a UTF-8 string into a fixed-size field in a native struct, always null-terminated and
    /// never overrunning. A string too long for the field is dropped rather than cut mid-character.</summary>
    internal static unsafe void CopyInto(string? value, byte* buffer, int capacity)
    {
        if (buffer == null || capacity <= 0) return;
        buffer[0] = 0;
        if (string.IsNullOrEmpty(value)) return;

        var bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length >= capacity) return;
        for (var i = 0; i < bytes.Length; ++i) buffer[i] = bytes[i];
        buffer[bytes.Length] = 0;
    }

    internal static string DecodeNullTerminated(byte[] buffer)
    {
        var end = Array.IndexOf(buffer, (byte)0);
        return Encoding.UTF8.GetString(buffer, 0, end < 0 ? buffer.Length : end);
    }

    /// <summary>Decodes a null-terminated string the native side owns.</summary>
    internal static unsafe string DecodeNullTerminated(byte* buffer)
        => buffer is null ? string.Empty : Marshal.PtrToStringUTF8((nint)buffer) ?? string.Empty;
}
