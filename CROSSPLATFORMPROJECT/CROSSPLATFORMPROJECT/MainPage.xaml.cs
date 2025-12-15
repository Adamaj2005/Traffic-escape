using Microsoft.Maui.Dispatching;

namespace CROSSPLATFORMPROJECT;

public partial class MainPage : ContentPage
{
    DrawingRoad road = new();

    int lanes = 4;
    int playerLane = 1;
    int enemyLane = 2;

    float enemyY = -200;
    float enemySpeed = 10f;
    float roadSpeed = 6f;

    int score = 0;

    // Star variables
    int stars = 0;
    bool starActive = false;
    float starY = -200;
    int starLane = 0;

    Random rnd = new();
    IDispatcherTimer timer;

    public MainPage()
    {
        InitializeComponent();

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
        road.Update(roadSpeed);
        RoadView.Invalidate();

        enemyY += enemySpeed;

        // Enemy reset
        if (enemyY > Height)
        {
            enemyY = -200;
            enemyLane = rnd.Next(0, lanes);
            EnemyCar.Source = PickRandomEnemyCar();

            score++;
            ScoreLabel.Text = $"Score: {score}";
        }

        // Spawn star (not in enemy lane)
        if (!starActive && rnd.Next(0, 200) == 1)
        {
            starActive = true;
            starY = -100;

            // Make sure star is not in enemy lane
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

        UpdatePositions();
        CheckCollision();
    }

    void UpdatePositions()
    {
        if (Width <= 0 || Height <= 0) return;

        double laneWidth = Width / lanes;

        double px = playerLane * laneWidth + laneWidth / 2 - 40;
        double py = Height - 160;

        double ex = enemyLane * laneWidth + laneWidth / 2 - 40;

        AbsoluteLayout.SetLayoutBounds(PlayerCar, new Rect(px, py, 80, 120));
        AbsoluteLayout.SetLayoutBounds(EnemyCar, new Rect(ex, enemyY, 80, 120));

        // Star position
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
    }

    void CheckCollision()
    {
        // Crash
        if (playerLane == enemyLane &&
            enemyY > Height - 260 &&
            enemyY < Height - 120)
        {
            timer.Stop();
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
        }
    }

    string PickRandomEnemyCar()
    {
        int r = rnd.Next(0, 3);

        if (r == 0) return "taxi.png";
        if (r == 1) return "truck.png";
        return "policecar.png";
    }
}
