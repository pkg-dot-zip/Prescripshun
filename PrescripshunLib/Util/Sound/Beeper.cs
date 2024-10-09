namespace PrescripshunLib.Util.Sound;

public static class Beeper
{
    /// <summary>
    /// Plays the sound of a beep through the console speaker using <c>Console.Beep()</c> if on the Windows platform.
    /// </summary>
    /// <param name="frequency">Frequency in <a href="https://nickfever.com/music/note-frequencies">hz</a>.</param>
    /// <param name="duration">Duration in ms.</param>
    /// <returns><see langword="true"/> if beeping was successful. <see langword="false"/> if not.</returns>
    private static bool Beep(int frequency, int duration = 200)
    {
        // Console.Beep() only works on Windows.
        if (!OperatingSystem.IsWindows()) return false;
        Console.Beep(frequency, duration);
        return true;
    }

    /// <summary>
    /// Beeps C4 -> E4 -> G4. Meant to be used for when the Server starts.
    /// </summary>
    public static void PlayServerBootSound()
    {
        Beep(262); // Approx C4.
        Beep(330); // Approx E4.
        Beep(392); // Approx G4.
    }

    /// <summary>
    /// Beeps C4 -> E4 -> G4 asynchronously. Meant to be used for when the Server starts.
    /// </summary>
    public static async Task PlayServerBootSoundAsync() => await Task.Run(PlayServerBootSound);
}