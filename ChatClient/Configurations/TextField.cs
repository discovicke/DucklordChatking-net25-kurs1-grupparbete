using Raylib_cs;
using System;

namespace ChatClient.Configurations
{
    // Text field component handle draw, hover and textinput (incl. backspace).
    // Stores text between frames.
    public class TextField
    {
        public Rectangle Rect { get; }
        public string Text { get; private set; } = string.Empty;
        public Color BackgroundColor { get; set; }
        public Color HoverColor { get; set; }
        public Color TextColor { get; set; }
        public bool IsSelected { get; private set; } = false;

        public TextField(Rectangle rect, Color backgroundColor, Color hoverColor, Color textColor)
        {
            Rect = rect;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            TextColor = textColor;
        }

        public void Draw()
        {
            var fill = MouseInput.IsHovered(Rect) ? HoverColor : BackgroundColor;
            Raylib.DrawRectangleRounded(Rect, 0.3f, 10, fill);

            if (MouseInput.IsHovered(Rect))
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, 0.3f, 10, 2, TextColor);
            }

            Raylib.DrawText(Text, (int)Rect.X + 10, (int)Rect.Y + 40, 20, TextColor);
        }
        
        // Get call every frame after MouseInput.Update() to handle click and text-input.
        public void Update()
        {
            // Mouseclick to select field
            if (MouseInput.IsLeftClick(Rect))
            {
                IsSelected = true;
            }
            else if (Raylib.IsMouseButtonPressed(MouseButton.Left) && !MouseInput.IsHovered(Rect))
            {
                // Outside of field click unselects
                IsSelected = false;
            }

            if (!IsSelected) return;

            // Read text input
            int key = Raylib.GetCharPressed();
            while (key > 0)
            {
                if (key >= 32 && key <= 126) // synliga ASCII-tecken
                {
                    Text += (char)key;
                }
                key = Raylib.GetCharPressed();
            }

            // Backspace
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && Text.Length > 0)
            {
                Text = Text.Substring(0, Text.Length - 1);
            }
        }

        public void Clear() => Text = string.Empty;
    }
}

