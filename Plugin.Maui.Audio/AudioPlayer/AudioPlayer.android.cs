using Android.Media;
using System.Diagnostics;

namespace Plugin.Maui.Audio;

partial class AudioPlayer : IAudioPlayer
{
	readonly MediaPlayer player;
	double volume = 0.5;
	bool isDisposed = false;
	AudioStopwatch stopwatch = new(TimeSpan.Zero, 1.0);

	public double Duration => player.Duration <= -1 ? -1 : player.Duration / 1000.0;

	public double CurrentPosition => stopwatch.ElapsedMilliseconds / 1000.0;

	public double Volume
	{
		get => volume;
		set => SetVolume(volume = value, 0);
	}

	/// <summary>
	/// Internal state machine for isPlaying. This is needed because when Reset is called, the player is not playing anymore, but in Idle state IsPlaying is not available
	/// </summary>
	bool isPlaying = false;

	public double Speed => 1;

	public double MinimumSpeed => 0;

	public double MaximumSpeed => 2.5;

	public bool IsPlaying => isPlaying;

	public AudioPlayer(BaseOptions audioPlayerOptions)
	{
		player = new MediaPlayer();
		player.Completion += OnPlaybackEnded;
	}

	public void Play(byte[] data)
	{
		if (!(data?.Length > 0))
		{
			return;
		}

		player.Reset();
		player.SetDataSource(new StreamMediaDataSource(data));
		player.Prepare();
		player.Start();
		stopwatch.Restart();
		isPlaying = true;
	}

	public void Stop()
	{
		isPlaying = false;
		player.Stop();
		stopwatch.Reset();
	}

	public void Pause()
	{
		if (!IsPlaying)
		{
			return;
		}

		isPlaying = false;
		player.Pause();
		stopwatch.Stop();
	}

	public void Seek(double position)
	{
		player.SeekTo((int)(position * 1000D));
		stopwatch = new AudioStopwatch(TimeSpan.FromSeconds(position), Speed);
	}

	void SetVolume(double volume, double balance)
	{
		volume = Math.Clamp(volume, 0, 1);

		balance = Math.Clamp(balance, -1, 1);

		// Using the "constant power pan rule." See: http://www.rs-met.com/documents/tutorials/PanRules.pdf
		var left = Math.Cos((Math.PI * (balance + 1)) / 4) * volume;
		var right = Math.Sin((Math.PI * (balance + 1)) / 4) * volume;

		player.SetVolume((float)left, (float)right);
	}

	void OnPlaybackEnded(object? sender, EventArgs e)
	{
		PlaybackEnded?.Invoke(this, e);
		isPlaying = player.IsPlaying;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (isDisposed)
		{
			return;
		}

		if (disposing)
		{
			player.Completion -= OnPlaybackEnded;
			player.Reset();
			player.Release();
			player.Dispose();
		}

		isDisposed = true;
	}
}

public class AudioStopwatch(TimeSpan startOffset, double speed) : Stopwatch
{
    public TimeSpan StartOffset { get; private set; } = startOffset;
    readonly double currentSpeed = speed;

    public new void Restart()
    {
        StartOffset = TimeSpan.Zero;
        base.Restart();
    }

    public new long ElapsedMilliseconds
    {
        get
        {
            return (long)(StartOffset.TotalMilliseconds + (base.ElapsedMilliseconds * currentSpeed));
        }
    }

    public new long ElapsedTicks
    {
        get
        {
            return (long)(StartOffset.Ticks + (base.ElapsedTicks * currentSpeed));
        }
    }
}

internal class StreamMediaDataSource(byte[] data) : MediaDataSource
{
    public override long Size => data.Length;

    public override int ReadAt(long position, byte[]? buffer, int offset, int size)
    {
        if (position >= data.Length)
            return -1;

        var toRead = Math.Min(size, data.Length - (int)position);
        data.AsSpan((int)position, toRead).CopyTo(buffer.AsSpan(offset, toRead));
        return toRead;
    }

    public override void Close() { }
}