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
}