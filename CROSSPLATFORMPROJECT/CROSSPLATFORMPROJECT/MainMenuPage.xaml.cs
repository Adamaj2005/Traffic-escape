using Microsoft.Maui.Storage;

namespace CROSSPLATFORMPROJECT;

public partial class MainMenuPage : ContentPage
{
    public MainMenuPage()
    {
        InitializeComponent();
        UpdateStats();
    }

    // Refresh values when the menu page appears
    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateStats();
    }

    // Load saved high score and total stars (ChatGPT helped)
    void UpdateStats()
    {
        int highScore = Preferences.Get("HighScore", 0);
        int totalStars = Preferences.Get("TotalStars", 0);

        HighScoreLabel.Text = $"High Score: {highScore}";
        TotalStarsLabel.Text = $"Total Stars: {totalStars}";
    }

    // Navigate to the game page
    async void OnStartGameClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    // Navigate to the settings page
    async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}
