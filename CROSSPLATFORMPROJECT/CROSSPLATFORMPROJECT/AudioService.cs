using Plugin.Maui.Audio;
using Microsoft.Maui.Storage;

namespace CROSSPLATFORMPROJECT;

public static class AudioService
{
    static IAudioManager audioManager = AudioManager.Current;
    static IAudioPlayer? musicPlayer;

    // Check if sound is enabled
    public static bool SoundEnabled =>
        Preferences.Get("SoundEnabled", true);

    // Play looping background music (ChatGPT helped)
    public static async Task PlayBackgroundMusic()
    {
        if (!SoundEnabled) return;

        musicPlayer?.Stop();
        musicPlayer?.Dispose();

        var stream = await FileSystem.OpenAppPackageFileAsync("background.mp3");
        musicPlayer = audioManager.CreatePlayer(stream);
        musicPlayer.Loop = true;
        musicPlayer.Play();
    }

    // Stop background music
    public static void StopMusic()
    {
        musicPlayer?.Stop();
    }

    // Play a single sound effect (ChatGPT helped)
    public static async Task PlaySound(string fileName)
    {
        if (!SoundEnabled) return;

        var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
        var player = audioManager.CreatePlayer(stream);
        player.Play();
    }
}
