using AVFoundation;

namespace Plugin.Maui.Audio;

partial class BaseOptions
{
    /// <summary>
    /// Gets or sets the category for the audio session.
    /// </summary>
    public AVAudioSessionCategory Category { get; } = AVAudioSessionCategory.Playback;

    /// <summary>
    /// Gets or sets the mode for the audio session.
    /// </summary>
    public AVAudioSessionMode Mode { get; } = AVAudioSessionMode.Default;

    /// <summary>
    /// Gets or sets the options for the audio session category.
    /// </summary>
    public AVAudioSessionCategoryOptions CategoryOptions { get; } = AVAudioSessionCategoryOptions.DuckOthers;
}