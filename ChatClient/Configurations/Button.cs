using System.Numerics;
using Raylib_cs;

namespace ChatClient.Configurations
{
    // Simpel UI-button that handles drawing, hover color and click detection.
    // Uses Raylib for drawing and existing MouseInput for position/click.
    public class Button(Rectangle rect, string text, Color normalColor, Color hoverColor, Color textColor)
        : UIComponent
    {
        public Rectangle Rect { get; } = rect;
        private string Text { get; set; } = text;
        private Color NormalColor { get; set; } = normalColor;
        private Color HoverColor { get; set; } = hoverColor;
        private Color TextColor { get; set; } = textColor;
        private float Rounds { get; set; } = 0.3f;
        private int Segments { get; set; } = 10;

        // Draws button: fill color changes on hover and text is drawn centered.
        public override void Draw()
        {
            // Decide fill color based on hover
            var fill = MouseInput.IsHovered(Rect) ? HoverColor : NormalColor;
            Raylib.DrawRectangleRounded(Rect, Rounds, Segments, fill);

            // Draw border when hovered
            if (MouseInput.IsHovered(Rect))
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, Rounds, Segments, 2, TextColor);
            }

            // Draw text centered in rectangle (simple)
            int fontSize = 20;
            int textWidth = Raylib.MeasureText(Text, fontSize);
            int x = (int)(Rect.X + (Rect.Width - textWidth) / 2);
            int y = (int)(Rect.Y + (Rect.Height - fontSize) / 2);
            Raylib.DrawText(Text, x, y, fontSize, TextColor);
        }

        // Return true if mouse is hovering over button.
        public bool IsHovered() => MouseInput.IsHovered(Rect);

        // Returns true only on the frame when the left mouse button is pressed over the button.
        public bool IsClicked()
        {
            bool hovered = IsHovered();
            bool pressed = Raylib.IsMouseButtonPressed(MouseButton.Left);
            // Click happens when button is pressed in this frame while hovering over it.
            bool clicked = hovered && pressed;
            return clicked;
        }

        public override void Update()
        {
            // No internal state to update for button currently.
        }
    }
}