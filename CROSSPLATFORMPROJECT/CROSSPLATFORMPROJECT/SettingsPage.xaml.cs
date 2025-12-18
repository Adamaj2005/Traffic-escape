using Microsoft.Maui.Storage;

namespace CROSSPLATFORMPROJECT;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    async void OnResetHighScoreClicked(object sender, EventArgs e)
    {
        Preferences.Set("HighScore", 0);

        await DisplayAlert(
            "High Score Reset",
            "Your high score has been reset to 0.",
            "OK");
    }

    async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
