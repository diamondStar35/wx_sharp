using System;
using System.Text;

namespace WxSharp;

internal static class Utf8String
{
    internal static string Decode(byte[] buffer, int length)
        => length <= 0 ? string.Empty : Encoding.UTF8.GetString(buffer, 0, length);

    internal static string DecodeNullTerminated(byte[] buffer)
    {
        var end = Array.IndexOf(buffer, (byte)0);
        return Encoding.UTF8.GetString(buffer, 0, end < 0 ? buffer.Length : end);
    }
}
