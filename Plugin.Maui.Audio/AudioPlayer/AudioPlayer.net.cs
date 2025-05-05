namespace Plugin.Maui.Audio;

partial class AudioPlayer : IAudioPlayer
{
	public AudioPlayer(BaseOptions options) { }

	protected virtual void Dispose(bool disposing) { }

	public double Duration { get; }

	public double CurrentPosition { get; }

	public double Volume { get; set; }

	public bool IsPlaying { get; }

	public void Play(byte[] data) { }

	public void Pause() { }

	public void Stop() { }

	public void Seek(double position) { }

	public double Speed { get; }

	public double MinimumSpeed { get; }

	public double MaximumSpeed { get; }
}