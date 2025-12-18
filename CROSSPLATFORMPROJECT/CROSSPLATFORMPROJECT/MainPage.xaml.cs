using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Storage;

namespace CROSSPLATFORMPROJECT;

public partial class MainPage : ContentPage
{
    DrawingRoad road = new();

    const int lanes = 4;
    const double CarWidth = 80;
    const double CarHeight = 120;

    int playerLane = 1;
    int enemyLane = 2;

    float enemyY = -200;
    float enemySpeed = 10f;
    float roadSpeed = 6f;

    int score = 0;
    int stars = 0;
    int highScore = 0;

    bool starActive = false;
    float starY = -200;
    int starLane = 0;

    bool gameOver = false;

    Random rnd = new();
    IDispatcherTimer timer;

    public MainPage()
    {
        InitializeComponent();

        highScore = Preferences.Get("HighScore", 0);
        HighScoreLabel.Text = $"High Score: {highScore}";

        RoadView.Drawable = road;
        EnemyCar.Source = PickRandomEnemyCar();

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTap;
        RoadView.GestureRecognizers.Add(tap);

        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16);
        timer.Tick += GameLoop;
        timer.Start();
    }

    void GameLoop(object sender, EventArgs e)
    {
        if (gameOver) return;

        road.Update(roadSpeed);
        RoadView.Invalidate();

        enemyY += enemySpeed;

        if (enemyY > Height)
        {
            enemyY = -200;
            enemyLane = rnd.Next(0, lanes);
            EnemyCar.Source = PickRandomEnemyCar();

            score++;
            ScoreLabel.Text = $"Score: {score}";

            if (score % 5 == 0)
            {
                enemySpeed += 0.5f;
                roadSpeed += 0.3f;
            }
        }

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

        if (starActive)
        {
            starY += enemySpeed;

            if (starY > Height)
            {
                starActive = false;
                Star.IsVisible = false;
            }
        }

        UpdatePositions();
        CheckCollision();
    }

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
    }

    void OnTap(object sender, EventArgs e)
    {
        var tap = (TappedEventArgs)e;
        double laneWidth = Width / lanes;
        double x = tap.GetPosition(this)?.X ?? 0;

        playerLane = (int)(x / laneWidth);
        playerLane = Math.Clamp(playerLane, 0, lanes - 1);
    }

    void CheckCollision()
    {
        if (!gameOver &&
            playerLane == enemyLane &&
            enemyY > Height - 260 &&
            enemyY < Height - 120)
        {
            gameOver = true;
            timer.Stop();

            if (score > highScore)
            {
                highScore = score;
                Preferences.Set("HighScore", highScore);
                HighScoreLabel.Text = $"High Score: {highScore}";
            }

            ShowGameOverOptions();
        }
    }

    async void ShowGameOverOptions()
    {
        bool restart = await DisplayAlert(
            "Game Over",
            $"Score: {score}",
            "Restart",
            "Main Menu");

        if (restart)
        {
            ResetGame();
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(MainMenuPage));
        }
    }

    void ResetGame()
    {
        score = 0;
        stars = 0;
        enemySpeed = 10f;
        roadSpeed = 6f;

        ScoreLabel.Text = "Score: 0";
        StarsLabel.Text = "Stars: 0";

        enemyY = -200;
        enemyLane = rnd.Next(0, lanes);

        starActive = false;
        Star.IsVisible = false;

        playerLane = lanes / 2;

        gameOver = false;
        timer.Start();
    }

    string PickRandomEnemyCar()
    {
        int r = rnd.Next(0, 3);
        if (r == 0) return "taxi.png";
        if (r == 1) return "truck.png";
        return "policecar.png";
    }
}
