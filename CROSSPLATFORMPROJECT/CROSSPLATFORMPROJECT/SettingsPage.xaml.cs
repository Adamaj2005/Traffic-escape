using Microsoft.Maui.Storage;

namespace CROSSPLATFORMPROJECT;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    // Reset the saved high score back to zero
    void OnResetHighScoreClicked(object sender, EventArgs e)
    {
        Preferences.Set("HighScore", 0);
    }

    // Return to the previous page
    async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
