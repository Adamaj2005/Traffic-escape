using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;

namespace CROSSPLATFORMPROJECT
{
    public partial class MainPage : ContentPage
    {
        float backgroundY = 0;
        int score = 0;
        GameDrawer drawer;
        IDispatcherTimer timer;

        public MainPage()
        {
            InitializeComponent();

            drawer = new GameDrawer();
            GameCanvas.Drawable = drawer;

            GameCanvas.StartInteraction += HandleTouch;

            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += GameLoop;
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            backgroundY += 5f;
            if (backgroundY > GameCanvas.Height)
                backgroundY = 0f;

            score++;
            ScoreLabel.Text = "Score: " + score;

            GameCanvas.Invalidate();
        }

        private void HandleTouch(object? sender, TouchEventArgs e)
        {
            float laneWidth = (float)(GameCanvas.Width / 3);
            float touchX = (float)e.Touches[0].X;

            if (touchX < laneWidth)
                drawer.carLane = Math.Max(0, drawer.carLane - 1);
            else if (touchX > laneWidth * 2)
                drawer.carLane = Math.Min(2, drawer.carLane + 1);
        }

        private void OnStartClicked(object? sender, EventArgs e)
        {
            StartPanel.IsVisible = false;
            ScoreLabel.IsVisible = true;
            timer.Start();
        }

        private void OnRestartClicked(object? sender, EventArgs e) // FIXED SIGNATURE
        {
            GameOverPanel.IsVisible = false;
            ScoreLabel.IsVisible = true;
            score = 0;
            drawer.carLane = 1;
            backgroundY = 0;
            timer.Start();
        }
    }

    public class GameDrawer : IDrawable
    {
        public int carLane = 1;

        public void Draw(ICanvas canvas, RectF rect)
        {
            float laneWidth = rect.Width / 3f;

            // Road background
            canvas.FillColor = Colors.Gray;
            canvas.FillRectangle(0, 0, rect.Width, rect.Height);

            // Lane lines
            canvas.FillColor = Colors.White;
            for (float i = 0; i < rect.Height; i += 100)
            {
                canvas.FillRectangle(laneWidth - 5, i, 10, 50);
                canvas.FillRectangle(laneWidth * 2 - 5, i, 10, 50);
            }

            // Car rectangle (placeholder)
            float carX = (carLane * laneWidth) + (laneWidth / 2f) - 40f;
            float carY = rect.Height - 150f;

            canvas.FillColor = Colors.Red;
            canvas.FillRectangle(carX, carY, 80, 120);
        }
    }
}
