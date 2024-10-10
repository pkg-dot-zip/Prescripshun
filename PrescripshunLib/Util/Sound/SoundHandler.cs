using NAudio.Wave;

namespace PrescripshunLib.Util.Sound;

public class SoundHandler
{
    #region Singleton

    private static SoundHandler? _instance = null;

    private SoundHandler()
    {
    }

    public static SoundHandler Get => _instance ??= new SoundHandler();

    #endregion

    private WaveOutEvent? _outputDevice = null;
    private AudioFileReader? _audioFile = null;

    public async Task PlaySoundFromUrlAsync(string url)
    {
        await Task.Run(() => PlaySoundFromUrl(url));
    }

    public void PlaySoundFromUrl(string url)
    {
        using (var mf = new MediaFoundationReader(url))
        using (var wo = new WasapiOut())
        {
            wo.Init(mf);
            wo.Play();
            while (wo.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(1000);
            }
        }
    }

    public void PlaySoundFromFile(string soundFilePath)
    {
        if (_outputDevice is null)
        {
            _outputDevice = new WaveOutEvent();
            _outputDevice.PlaybackStopped += OnPlaybackStopped;
        }
        if (_audioFile is null)
        {
            _audioFile = new AudioFileReader($"{soundFilePath}");
            _outputDevice.Init(_audioFile);
        }
        _outputDevice.Play();
    }

    // TODO: Implement.
    public async Task PlaySoundFromFileAsync(string soundFilePath)
    {
        await Task.Run(() => PlaySoundFromFile(soundFilePath));
    }

    private void OnPlaybackStopped(object? sender, StoppedEventArgs args)
    {
        _outputDevice?.Dispose();
        _outputDevice = null;
        _audioFile?.Dispose();
        _audioFile = null;
    }
}