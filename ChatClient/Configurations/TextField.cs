using Raylib_cs;
using System;

namespace ChatClient.Configurations
{
    // Text field component handle draw, hover and textinput (incl. backspace).
    // Stores text between frames.
    public class TextField(Rectangle rect, Color backgroundColor, Color hoverColor, Color textColor)
    {
        public Rectangle Rect { get; } = rect;
        public string Text { get; private set; } = string.Empty;
        public Color BackgroundColor { get; set; } = backgroundColor;
        public Color HoverColor { get; set; } = hoverColor;
        public Color TextColor { get; set; } = textColor;
        public bool IsSelected { get; private set; } = false;

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
                // Accepts all Unicode-tecken (>= 32)
                if (key >= 32)
                {
                    // Converts to Unicode to accept all even åäö
                    Text += char.ConvertFromUtf32(key);
                }
                key = Raylib.GetCharPressed();
            }
                

                // Backspace
                // TODO: Backspace pressed = hold down for continuous delete
                if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && Text.Length > 0 || Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace))
                {
                    Text = Text.Substring(0, Text.Length - 1);
                }
                // TODO: Enter = isClicked

                // TODO: Add visible cursor

                // TODO: Text row break when hitting border

                // TODO: Scroll logicZ

                // TODO: Font?
            
        }
        
        public void Clear() => Text = string.Empty;
    }
    
}

