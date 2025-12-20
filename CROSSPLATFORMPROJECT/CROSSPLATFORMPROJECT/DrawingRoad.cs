using Microsoft.Maui.Graphics;

namespace CROSSPLATFORMPROJECT
{
    public class DrawingRoad : IDrawable
    {
        // Number of lanes drawn on the road
        public int Lanes = 4;

        // Controls how the lane lines scroll
        float scroll = 0f;

        const float dashHeight = 70f;
        const float dashGap = 60f;

        // Updates the scrolling position based on speed (ChatGPT helped)
        public void Update(float speed)
        {
            scroll += speed;
            scroll %= (dashHeight + dashGap);
        }

        // Draws the road background and lane markings
        public void Draw(ICanvas canvas, RectF rect)
        {
            float laneWidth = rect.Width / Lanes;

            // Draw road surface
            canvas.FillColor = Color.FromArgb("#4A4A4A");
            canvas.FillRectangle(rect);

            // Draw yellow side lines
            canvas.FillColor = Colors.Yellow;
            canvas.FillRectangle(0, 0, 8, rect.Height);
            canvas.FillRectangle(rect.Width - 8, 0, 8, rect.Height);

            // Draw dashed lane separators (ChatGPT helped)
            canvas.FillColor = Colors.White;
            for (int lane = 1; lane < Lanes; lane++)
            {
                float x = lane * laneWidth;
                for (float y = -dashHeight; y < rect.Height + dashHeight; y += dashHeight + dashGap)
                {
                    canvas.FillRectangle(x - 5, y + scroll, 10, dashHeight);
                }
            }
        }
    }
}
