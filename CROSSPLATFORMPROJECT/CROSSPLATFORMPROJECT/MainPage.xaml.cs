using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Storage;

namespace CROSSPLATFORMPROJECT;

public partial class MainPage : ContentPage
{
    // Draws and scrolls the road background
    DrawingRoad road = new();

    // Lane and car sizes
    const int lanes = 4;
    const double CarWidth = 80;
    const double CarHeight = 120;

    // Player and enemy positions
    int playerLane = 1;
    int enemyLane = 2;

    // Enemy movement and road speed
    float enemyY = -200;
    float enemySpeed = 10f;
    float roadSpeed = 6f;

    // Score tracking
    int score = 0;
    int stars = 0;
    int highScore = 0;

    // Star pickup state
    bool starActive = false;
    float starY = -200;
    int starLane = 0;

    // Shield power-up state (ChatGPT helped)
    bool shieldVisible = false;
    bool shieldActive = false;
    float shieldY = -200;
    int shieldLane = 0;
    int nextShieldScore = 15;

    // Shield countdown timer (ChatGPT helped)
    int shieldTimeLeft = 0;
    float shieldTimer = 0f;

    // Game state flag
    bool gameOver = false;

    Random rnd = new();
    IDispatcherTimer timer;

    public MainPage()
    {
        InitializeComponent();

        // Load saved high score
        highScore = Preferences.Get("HighScore", 0);
        HighScoreLabel.Text = $"High Score: {highScore}";
        ShieldTimerLabel.Text = "";

        // Set up road and enemy
        RoadView.Drawable = road;
        EnemyCar.Source = PickRandomEnemyCar();

        // Tap input for lane switching
        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTap;
        RoadView.GestureRecognizers.Add(tap);

        // Game loop timer running at ~60fps (ChatGPT helped)
        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16);
        timer.Tick += GameLoop;
        timer.Start();
    }

    // Start music when game screen opens (ChatGPT helped)
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = AudioService.PlayBackgroundMusic();
    }

    // Stop music when leaving the game
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        AudioService.StopMusic();
    }

    // Main game loop
    void GameLoop(object sender, EventArgs e)
    {
        if (gameOver) return;

        road.Update(roadSpeed);
        RoadView.Invalidate();

        enemyY += enemySpeed;

        // Enemy reset and score increase
        if (enemyY > Height)
        {
            enemyY = -200;
            enemyLane = rnd.Next(0, lanes);
            EnemyCar.Source = PickRandomEnemyCar();

            score++;
            ScoreLabel.Text = $"Score: {score}";

            // Increase difficulty over time (ChatGPT helped)
            if (score % 5 == 0)
            {
                enemySpeed += 0.5f;
                roadSpeed += 0.3f;
            }

            // Spawn shield at score intervals (ChatGPT helped)
            if (score >= nextShieldScore && !shieldVisible && !shieldActive)
            {
                shieldVisible = true;
                shieldY = 0;
                shieldLane = rnd.Next(0, lanes);
                Shield.IsVisible = true;
                nextShieldScore += 15;
            }
        }

        // Random star spawn
        if (!starActive && rnd.Next(0, 200) == 1)
        {
            starActive = true;
            starY = -100;

            do
            {
                starLane = rnd.Next(0, lanes);
            }
            while (starLane == enemyLane);

            Star.IsVisible = true;
        }

        // Move star
        if (starActive)
        {
            starY += enemySpeed;
            if (starY > Height)
            {
                starActive = false;
                Star.IsVisible = false;
            }
        }

        // Move shield
        if (shieldVisible)
        {
            shieldY += enemySpeed * 0.5f;
            if (shieldY > Height)
            {
                shieldVisible = false;
                Shield.IsVisible = false;
            }
        }

        // Shield countdown handling (ChatGPT helped)
        if (shieldActive)
        {
            shieldTimer += 0.016f;
            if (shieldTimer >= 1f)
            {
                shieldTimer = 0f;
                shieldTimeLeft--;
                ShieldTimerLabel.Text = $"Shield: {shieldTimeLeft}s";

                if (shieldTimeLeft <= 0)
                    DisableShield();
            }
        }

        UpdatePositions();
        CheckCollision();
    }

    // Update positions of all game objects
    void UpdatePositions()
    {
        if (Width <= 0 || Height <= 0) return;

        double laneWidth = Width / lanes;

        double px = playerLane * laneWidth + laneWidth / 2 - CarWidth / 2;
        double py = Height - CarHeight - 40;
        double ex = enemyLane * laneWidth + laneWidth / 2 - CarWidth / 2;

        AbsoluteLayout.SetLayoutBounds(PlayerCar, new Rect(px, py, CarWidth, CarHeight));
        AbsoluteLayout.SetLayoutBounds(EnemyCar, new Rect(ex, enemyY, CarWidth, CarHeight));

        if (starActive)
        {
            double sx = starLane * laneWidth + laneWidth / 2 - 20;
            AbsoluteLayout.SetLayoutBounds(Star, new Rect(sx, starY, 40, 40));
        }

        if (shieldVisible)
        {
            double sx = shieldLane * laneWidth + laneWidth / 2 - 20;
            AbsoluteLayout.SetLayoutBounds(Shield, new Rect(sx, shieldY, 40, 40));
        }
    }

    // Handle tap input
    void OnTap(object sender, EventArgs e)
    {
        var tap = (TappedEventArgs)e;
        double laneWidth = Width / lanes;
        double x = tap.GetPosition(this)?.X ?? 0;

        playerLane = (int)(x / laneWidth);
        playerLane = Math.Clamp(playerLane, 0, lanes - 1);
    }

    // Collision handling
    void CheckCollision()
    {
        // Shield pickup
        if (shieldVisible &&
            playerLane == shieldLane &&
            shieldY > Height - 260 &&
            shieldY < Height - 120)
        {
            shieldVisible = false;
            Shield.IsVisible = false;

            shieldActive = true;
            shieldTimeLeft = 5;
            shieldTimer = 0f;

            ShieldTimerLabel.Text = $"Shield: {shieldTimeLeft}s";
            PlayerCar.Opacity = 0.7;

            _ = AudioService.PlaySound("Shield.wav");
        }

        // Star pickup
        if (starActive &&
            playerLane == starLane &&
            starY > Height - 260 &&
            starY < Height - 120)
        {
            starActive = false;
            Star.IsVisible = false;

            stars++;
            StarsLabel.Text = $"Stars: {stars}";

            _ = AudioService.PlaySound("Star.wav");
        }

        // Crash handling with shield interaction (ChatGPT helped)
        if (!gameOver &&
            playerLane == enemyLane &&
            enemyY > Height - 260 &&
            enemyY < Height - 120)
        {
            if (shieldActive)
            {
                DisableShield();
                enemyY = -200;
                enemyLane = rnd.Next(0, lanes);
            }
            else
            {
                gameOver = true;
                timer.Stop();

                _ = AudioService.PlaySound("Crash.wav");

                if (score > highScore)
                {
                    highScore = score;
                    Preferences.Set("HighScore", highScore);
                    HighScoreLabel.Text = $"High Score: {highScore}";
                }

                ShowGameOverOptions();
            }
        }
    }

    // Disable shield effect
    void DisableShield()
    {
        shieldActive = false;
        shieldTimeLeft = 0;
        ShieldTimerLabel.Text = "";
        PlayerCar.Opacity = 1;
    }

    // Game over dialog and navigation (ChatGPT helped)
    async void ShowGameOverOptions()
    {
        bool restart = await DisplayAlert(
            "Game Over",
            $"Score: {score}",
            "Restart",
            "Main Menu");

        if (restart)
            ResetGame();
        else
            await Shell.Current.GoToAsync(nameof(MainMenuPage));
    }

    // Reset all gameplay values
    void ResetGame()
    {
        score = 0;
        stars = 0;

        ScoreLabel.Text = "Score: 0";
        StarsLabel.Text = "Stars: 0";
        ShieldTimerLabel.Text = "";

        enemySpeed = 10f;
        roadSpeed = 6f;

        enemyY = -200;
        enemyLane = rnd.Next(0, lanes);

        starActive = false;
        shieldActive = false;
        shieldVisible = false;
        nextShieldScore = 15;

        Star.IsVisible = false;
        Shield.IsVisible = false;
        PlayerCar.Opacity = 1;

        playerLane = lanes / 2;

        gameOver = false;
        timer.Start();
    }

    // Select a random enemy car image
    string PickRandomEnemyCar()
    {
        int r = rnd.Next(0, 3);
        if (r == 0) return "taxi.png";
        if (r == 1) return "truck.png";
        return "policecar.png";
    }
}
