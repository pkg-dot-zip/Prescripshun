using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace PrescripshunGui.Util.Sound;

/// <summary>
/// Handles all audio and sound; no other classes are involved in audio and sound related functionality.
/// </summary>
public class SoundHandler
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    #region Singleton

    private static SoundHandler? _instance = null;

    private SoundHandler()
    {
    }

    public static SoundHandler Get => _instance ??= new SoundHandler();

    #endregion

    /// <summary>
    /// Asynchronously plays a sound from an <paramref name="url"/>.
    /// </summary>
    /// <param name="url">Url of sound to play. Needs to be absolute!</param>
    public async Task PlaySoundFromUrlAsync(string url)
    {
        await Task.Run(() => PlaySoundFromUrl(url));
    }

    /// <summary>
    /// Plays a sound from an <paramref name="url"/>.
    /// </summary>
    /// <param name="url">Url of sound to play. Needs to be absolute!</param>
    public void PlaySoundFromUrl(string url)
    {
        GuiEvents.Get.OnSoundPlay.Invoke(url);

        Logger.Trace("Started playback of {0}", url);
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
        Logger.Trace("Completed playback of {0}", url);

        GuiEvents.Get.OnSoundEnd.Invoke(url);
    }
}