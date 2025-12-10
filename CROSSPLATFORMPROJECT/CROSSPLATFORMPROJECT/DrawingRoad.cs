using Microsoft.Maui.Graphics;

namespace CROSSPLATFORMPROJECT
{
    public class DrawingRoad
    {
        public int Lanes = 4;
        float scroll = 0f;

        const float dashHeight = 70f;
        const float dashGap = 60f;

        public void Update(float roadSpeed)
        {
            scroll += roadSpeed;
            scroll %= (dashHeight + dashGap);
        }

        public void Draw(ICanvas canvas, RectF rect)
        {
            float laneWidth = rect.Width / Lanes;

            // Road
            canvas.FillColor = Color.FromArgb("#4A4A4A");
            canvas.FillRectangle(rect);

            // Yellow hard shoulders
            canvas.FillColor = Colors.Yellow;
            canvas.FillRectangle(0, 0, 8, rect.Height);
            canvas.FillRectangle(rect.Width - 8, 0, 8, rect.Height);

            // Broken white lane lines
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
