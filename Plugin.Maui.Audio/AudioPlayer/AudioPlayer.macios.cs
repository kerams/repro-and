namespace Plugin.Maui.Audio;

using System.Diagnostics;
using AVFoundation;

partial class AudioPlayer(BaseOptions audioPlayerOptions) : IAudioPlayer
{
	private readonly Lock _lock = new();
	private AVAudioPlayer? player = null;

    public double CurrentPosition => player?.CurrentTime ?? 0;

	public double Duration => player?.Duration ?? 0;

	public double Volume
	{
		get => player?.Volume ?? 0;
		set
		{
			if (player != null)
			{
				player.Volume = (float)Math.Clamp(value, 0, 1);
			}
		}
	}

	public double Speed => player?.Rate ?? 0;

	public void SetSpeed(double sp)
	{
		// Rate property supports values in the range of 0.5 for half-speed playback to 2.0 for double-speed playback.
		var speedValue = Math.Clamp((float)sp, 0.5f, 2.0f);

		if (float.IsNaN(speedValue))
		{
			speedValue = 1.0f;
		}

		if (player != null)
		{
			player.Rate = speedValue;
		}
	}

	public double MinimumSpeed => 0.5;

	public double MaximumSpeed => 2;

	public bool IsPlaying => player?.Playing == true;

    protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			Stop(true);
		}
	}

	public void Pause() => player?.Pause();

	public void Play(byte[] data)
	{
		if (!(data?.Length > 0))
		{
			return;
		}

		lock (_lock)
		{
            Stop(true);

            player =
				AVAudioPlayer.FromData(NSData.FromArray(data), out var e)
				?? throw new Exception($"Unable to create AVAudioPlayer from data: {e?.LocalizedDescription}");

			InitializeSession();

			player.FinishedPlaying += OnPlayerFinishedPlaying;
			player.EnableRate = true;
			player.PrepareToPlay();
			player.Play();
		}
	}

	public void Seek(double position)
	{
		if (player != null)
		{
			player.CurrentTime = position;
		}
	}

    private void Stop(bool dispose)
    {
        if (player != null)
        {
            player.FinishedPlaying -= OnPlayerFinishedPlaying;

            if (player.Playing)
                player.Stop();

            if (dispose)
            {
                player.Dispose();
                player = null;
            }
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            Stop(false);
        }
    }

	private void OnPlayerFinishedPlaying(object? sender, AVStatusEventArgs e)
	{
        try
        {
            var error = AVAudioSession.SharedInstance().SetActive(false);
            if (error != null)
            {
                Trace.TraceError("failed deactivate audio session");
                Trace.TraceError(error.ToString());
            }
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex.ToString());
        }

        Task.Run(() => PlaybackEnded?.Invoke(this, e));
    }

    private void InitializeSession()
	{
        try
        {
            var audioSession = AVAudioSession.SharedInstance();
            var error = audioSession.SetCategory(audioPlayerOptions.Category, audioPlayerOptions.Mode, audioPlayerOptions.CategoryOptions);
            if (error != null)
            {
                Trace.TraceError("failed to set category");
                Trace.TraceError(error.ToString());
            }

            error = audioSession.SetActive(true);
            if (error != null)
            {
                Trace.TraceError("failed activate audio session");
                Trace.TraceError(error.ToString());
            }
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }
    }
}