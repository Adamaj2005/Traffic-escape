using Microsoft.Maui.Graphics;
using Microsoft.Maui.Dispatching;
using System;
using Microsoft.Maui.Graphics.Platform;

namespace CROSSPLATFORMPROJECT
{
    public partial class MainPage : ContentPage, IDrawable
    {
        DrawingRoad road = new();
        int lanes = 4;
        int playerLane = 1;
        int enemyLane = 2;
        float enemyY = -200;
        float roadSpeed = 6f;
        float enemySpeed = 10f;
        int score = 0;
        Random rnd = new();
        IDispatcherTimer timer;

        // Car images - use full namespace to avoid ambiguity
        Microsoft.Maui.Graphics.IImage sportsCarImage;
        Microsoft.Maui.Graphics.IImage taxiImage;
        Microsoft.Maui.Graphics.IImage truckImage;
        Microsoft.Maui.Graphics.IImage policeCarImage;
        Microsoft.Maui.Graphics.IImage currentEnemyImage;
        bool imagesLoaded = false;

        public MainPage()
        {
            InitializeComponent();
            GameCanvas.Drawable = this;
            GameCanvas.StartInteraction += OnTap;

            // Load car images
            LoadCarImages();

            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += GameLoop;
        }

        private async void LoadCarImages()
        {
            try
            {
                // Load player car
                var sportsStream = await FileSystem.OpenAppPackageFileAsync("sportscar.png");
                sportsCarImage = PlatformImage.FromStream(sportsStream);

                // Load enemy cars
                var taxiStream = await FileSystem.OpenAppPackageFileAsync("taxi.png");
                taxiImage = PlatformImage.FromStream(taxiStream);

                var truckStream = await FileSystem.OpenAppPackageFileAsync("truck.png");
                truckImage = PlatformImage.FromStream(truckStream);

                var policeStream = await FileSystem.OpenAppPackageFileAsync("policecar.png");
                policeCarImage = PlatformImage.FromStream(policeStream);

                // Set initial enemy car randomly
                PickRandomEnemyCar();

                imagesLoaded = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading images: {ex.Message}");
                imagesLoaded = false;
            }
        }

        private void PickRandomEnemyCar()
        {
            int carType = rnd.Next(0, 3);
            currentEnemyImage = carType switch
            {
                0 => taxiImage,
                1 => truckImage,
                2 => policeCarImage,
                _ => taxiImage
            };
        }

        private void OnTap(object s, TouchEventArgs e)
        {
            float x = (float)e.Touches[0].X;
            float laneWidth = (float)GameCanvas.Width / lanes;
            playerLane = (int)(x / laneWidth);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            road.Update(roadSpeed);
            enemyY += enemySpeed;

            if (enemyY > GameCanvas.Height)
            {
                enemyY = -200;
                enemyLane = rnd.Next(0, lanes);
                PickRandomEnemyCar(); // Pick a new random enemy car
                score++;
                ScoreLabel.Text = $"Score: {score}";
            }

            CheckCollision();
            GameCanvas.Invalidate();
        }

        private void CheckCollision()
        {
            if (playerLane == enemyLane &&
                enemyY > GameCanvas.Height - 250 &&
                enemyY < GameCanvas.Height - 100)
            {
                timer.Stop();
                ScoreLabel.IsVisible = false;
                GameOverPanel.IsVisible = true;
                FinalScoreLabel.Text = $"Score: {score}";
            }
        }

        private void ResetGame()
        {
            score = 0;
            enemyY = -200;
            playerLane = 1;
            PickRandomEnemyCar();
            ScoreLabel.Text = "Score: 0";
        }

        private void OnStartClicked(object s, EventArgs e)
        {
            ResetGame();
            StartPanel.IsVisible = false;
            GameOverPanel.IsVisible = false;
            ScoreLabel.IsVisible = true;
            timer.Start();
        }

        private void OnRestartClicked(object s, EventArgs e)
        {
            ResetGame();
            GameOverPanel.IsVisible = false;
            ScoreLabel.IsVisible = true;
            timer.Start();
        }

        public void Draw(ICanvas canvas, RectF rect)
        {
            float laneWidth = rect.Width / lanes;
            road.Draw(canvas, rect);

            // Player car (sports car)
            float px = playerLane * laneWidth + laneWidth / 2 - 40;
            float py = rect.Height - 150;

            if (imagesLoaded && sportsCarImage != null)
            {
                canvas.DrawImage(sportsCarImage, px, py, 80, 120);
            }
            else
            {
                canvas.FillColor = Colors.Red;
                canvas.FillRectangle(px, py, 80, 120);
            }

            // Enemy car (taxi, truck, or police car)
            float ex = enemyLane * laneWidth + laneWidth / 2 - 40;

            if (imagesLoaded && currentEnemyImage != null)
            {
                canvas.DrawImage(currentEnemyImage, ex, enemyY, 80, 120);
            }
            else
            {
                canvas.FillColor = Colors.Purple;
                canvas.FillRectangle(ex, enemyY, 80, 120);
            }
        }
    }
}