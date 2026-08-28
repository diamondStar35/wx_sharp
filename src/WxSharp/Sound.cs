using System;

namespace WxSharp;

/// <summary>How a sound is played, following the <c>wxSOUND_</c> flags.</summary>
[Flags]
public enum SoundPlayback
{
    /// <summary>Blocks until the sound finishes. Freezes the interface, and anything assistive technology
    /// is reading from it, for as long as it plays.</summary>
    Synchronous = 0,
    /// <summary>Returns immediately and plays in the background. Almost always what you want.</summary>
    Asynchronous = 1,
    /// <summary>Repeats until <see cref="Sound.Stop"/> is called. Only with
    /// <see cref="Asynchronous"/>.</summary>
    Loop = 2,
}

/// <summary>Plays a WAV file, following <c>wxSound</c>.</summary>
///
/// <remarks>
/// This is deliberately small: wxWidgets reads one format and offers no position, volume or mixing, so it
/// suits short interface feedback and nothing more. Anything that needs to seek, control volume, or play a
/// compressed format wants a real audio library instead.
///
/// Construct a <see cref="Sound"/> once and replay it when the same clip is used repeatedly - the static
/// <see cref="Play(string, SoundPlayback)"/> re-reads the file on every call.
///
/// Do not treat a successful load or play as proof the sound exists. Windows hands the path to the system
/// without checking it, so both report success for a file that is not there and simply never plays; other
/// platforms do check. Verify the file yourself if it matters.
/// </remarks>
public sealed class Sound : IDisposable
{
    private nint _handle;

    private nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Sound));

    /// <summary>Loads a WAV file.</summary>
    /// <exception cref="ArgumentException">wxWidgets could not load the file. Note that Windows does not
    /// check the path here, so a missing file loads without complaint and fails silently on play.</exception>
    public Sound(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        _ = App.RequireCurrent();
        _handle = NativeMethods.wxsharp_sound_create(path);
        if (_handle == 0)
            throw new ArgumentException($"The sound could not be loaded: {path}", nameof(path));
    }

    /// <summary>Whether wxWidgets considers the sound loaded. On Windows this is true even for a file that
    /// does not exist, so it is not a way to check one.</summary>
    public bool IsOk => _handle != 0 && NativeMethods.wxsharp_sound_is_ok(_handle);

    /// <summary>Plays this sound. Returns false if the platform refused to play it.</summary>
    public bool Play(SoundPlayback playback = SoundPlayback.Asynchronous)
        => NativeMethods.wxsharp_sound_play(Handle, (uint)playback);

    /// <summary>Plays a file without keeping it loaded. Reads the file every time, so prefer a
    /// <see cref="Sound"/> for a clip played more than once.</summary>
    public static bool Play(string path, SoundPlayback playback = SoundPlayback.Asynchronous)
    {
        ArgumentNullException.ThrowIfNull(path);
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_sound_play_file(path, (uint)playback);
    }

    /// <summary>Stops whatever is playing. wxWidgets offers no way to ask what that is, only to stop
    /// it.</summary>
    public static void Stop()
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_sound_stop();
    }

    public void Dispose()
    {
        if (_handle != 0) NativeMethods.wxsharp_sound_destroy(_handle);
        _handle = 0;
    }
}
