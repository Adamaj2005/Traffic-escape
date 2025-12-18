using Microsoft.Maui.Storage;

namespace CROSSPLATFORMPROJECT;

public partial class MainMenuPage : ContentPage
{
    public MainMenuPage()
    {
        InitializeComponent();

        int highScore = Preferences.Get("HighScore", 0);
        HighScoreLabel.Text = $"High Score: {highScore}";
    }

    async void OnStartGameClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}
