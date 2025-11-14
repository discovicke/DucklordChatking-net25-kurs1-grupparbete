using System.Numerics;
using ChatClient.Core;
using Raylib_cs;

namespace ChatClient.UI.Components
{
    public class Button : UIComponent
    {
        private string Text;
        private Color NormalColor;
        private Color HoverColorButton;
        private Color PressedColorButton;
        private Color TextColorButton;
        private float Rounds = 0.3f;
        private int Segments = 10;

        public Button(Rectangle rect, string text, Color normalColor, Color hoverColor, Color textColor)
        {
            Rect = rect;
            Text = text;
            NormalColor = normalColor;
            HoverColorButton = hoverColor;
            PressedColorButton = Colors.ButtonPressed; // Add pressed state
            TextColorButton = textColor;
        }

        // Simplified constructor using Colors.cs defaults
        public Button(Rectangle rect, string text)
        {
            Rect = rect;
            Text = text;
            NormalColor = Colors.ButtonDefault;
            HoverColorButton = Colors.ButtonHovered;
            PressedColorButton = Colors.ButtonPressed;
            TextColorButton = Colors.TextColor;
        }

        public override void Draw()
        {
            bool isHovered = MouseInput.IsHovered(Rect);
            bool isPressed = isHovered && Raylib.IsMouseButtonDown(MouseButton.Left);
            
            // Determine fill color based on state
            Color fill;
            if (isPressed)
                fill = PressedColorButton;
            else if (isHovered)
                fill = HoverColorButton;
            else
                fill = NormalColor;
            
            // Draw button background
            Raylib.DrawRectangleRounded(Rect, Rounds, Segments, fill);

            // Draw border/outline - accent color on hover, outline when pressed
            if (isPressed)
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, Rounds, Segments, 3, Colors.OutlineColor);
            }
            else if (isHovered)
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, Rounds, Segments, 3, Colors.AccentColor);
            }
            else
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, Rounds, Segments, 2, Colors.OutlineColor);
            }

            // Dynamic font size - scale down if text doesn't fit
            const int maxFontSize = 20;
            const int minFontSize = 8;
            const int padding = 10; // Padding inside button
            
            int fontSize = maxFontSize;
            int textWidth = Raylib.MeasureText(Text, fontSize);
            float availableWidth = Rect.Width - (padding * 2);
            float availableHeight = Rect.Height - (padding * 2);
            
            // Scale down font if text is too wide
            while ((fontSize > minFontSize && textWidth > availableWidth) || (fontSize > maxFontSize && textWidth > availableHeight))
            {
                fontSize--;
                textWidth = Raylib.MeasureText(Text, fontSize);
            }
            
            // Center text in button
            int x = (int)(Rect.X + (Rect.Width - textWidth) / 2);
            int y = (int)(Rect.Y + (Rect.Height - fontSize) / 2);
            Raylib.DrawText(Text, x, y, fontSize, TextColorButton);
        }

        public bool IsHovered() => MouseInput.IsHovered(Rect);

        public bool IsClicked()
        {
            bool hovered = IsHovered();
            bool pressed = Raylib.IsMouseButtonPressed(MouseButton.Left);
            bool clicked = hovered && pressed;
            
            if (clicked)
            {
                Log.Info($"[Button] '{Text}' clicked");

                // Duck sound
                Raylib.PlaySound(ResourceLoader.ButtonSound);

            }

            return clicked;
        }

        public override void Update() { }
    }


    // TODO: add button for exit application
}