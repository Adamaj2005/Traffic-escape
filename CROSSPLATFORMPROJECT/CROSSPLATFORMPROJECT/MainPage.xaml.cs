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
    Random rnd = new();
    IDispatcherTimer timer;

    public MainPage()
    {
        InitializeComponent();

        RoadView.Drawable = road;

        EnemyCar.Source = PickRandomEnemyCar();

        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16);
        timer.Tick += GameLoop;
        timer.Start();

        RoadView.StartInteraction += OnTap;
    }

    void GameLoop(object sender, EventArgs e)
    {
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
        }

        UpdatePositions();
        CheckCollision();
    }

    void UpdatePositions()
    {
        double laneWidth = Width / lanes;

        double px = playerLane * laneWidth + laneWidth / 2 - 40;
        double py = Height - 150;

        double ex = enemyLane * laneWidth + laneWidth / 2 - 40;

        AbsoluteLayout.SetLayoutBounds(PlayerCar, new Rect(px, py, 80, 120));
        AbsoluteLayout.SetLayoutBounds(EnemyCar, new Rect(ex, enemyY, 80, 120));
    }

    void OnTap(object sender, TouchEventArgs e)
    {
        double laneWidth = Width / lanes;
        playerLane = (int)(e.Touches[0].X / laneWidth);
    }

    void CheckCollision()
    {
        if (playerLane == enemyLane &&
            enemyY > Height - 250 &&
            enemyY < Height - 100)
        {
            timer.Stop();

            
            RestartButton.IsVisible = true;
        }
    }

    void OnRestartClicked(object sender, EventArgs e)
    {
        RestartButton.IsVisible = false;

        score = 0;
        enemyY = -200;
        playerLane = 1;
        enemyLane = rnd.Next(0, lanes);

        ScoreLabel.Text = "Score: 0";
        EnemyCar.Source = PickRandomEnemyCar();

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
