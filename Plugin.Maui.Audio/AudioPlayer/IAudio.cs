namespace Plugin.Maui.Audio;

/// <summary>
/// Exposes common audio related properties and methods.
/// </summary>
public interface IAudio : IDisposable
{
	///<Summary>
	/// Gets the length of audio in seconds.
	///</Summary>
	double Duration { get; }

	///<Summary>
	/// Gets the current position of audio playback in seconds.
	///</Summary>
	double CurrentPosition { get; }

	///<Summary>
	/// Gets or sets the playback volume 0 to 1 where 0 is no-sound and 1 is full volume.
	///</Summary>
	double Volume { get; set; }

	///<Summary>
	/// Gets the playback speed where 1 is normal speed. <see cref="MinimumSpeed"/> and <see cref="MaximumSpeed"/> can be used to determine the minumum and maximum value for each platform.
	///</Summary>
	double Speed { get; }

	/// <summary>
	/// Gets the minimum speed value that can be set for <see cref="Speed"/> on this platform.
	/// </summary>
	double MinimumSpeed { get; }

	/// <summary>
	/// Gets the maximum speed value that can be set for <see cref="Speed"/> on this platform.
	/// </summary>
	double MaximumSpeed { get; }

	///<Summary>
	/// Gets a value indicating whether the currently loaded audio file is playing.
	///</Summary>
	bool IsPlaying { get; }

	///<Summary>
	/// Set the current playback position (in seconds).
	///</Summary>
	void Seek(double position);
}
