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

        private float CreatBlinkTimer = 0f;
        private bool CreatVisible = true;

        public void Draw()
        {
            var fill = MouseInput.IsHovered(Rect) ? HoverColor : BackgroundColor;
            Raylib.DrawRectangleRounded(Rect, 0.3f, 10, fill);

            if (MouseInput.IsHovered(Rect))
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, 0.3f, 10, 2, TextColor);
            }

            if (IsSelected && CreatVisible)
            {
                int textWidth = Raylib.MeasureText(Text, 20); // same font size you draw with
                int caretX = (int)Rect.X + 10 + textWidth;    // same X offset as text
                int caretTop = (int)Rect.Y + 40;              // same Y as text
                int caretBottom = caretTop + 20;              // same font size as height
                Raylib.DrawLine(caretX, caretTop, caretX, caretBottom, TextColor);
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

            if (IsSelected)
            {
                CreatBlinkTimer += Raylib.GetFrameTime();
                if (CreatBlinkTimer >= 0.5f) // blink period in seconds
                {
                    CreatBlinkTimer = 0f;
                    CreatVisible = !CreatVisible;
                }
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
                    CreatBlinkTimer = 0f;
                    CreatVisible = true;
                }


                key = Raylib.GetCharPressed();
            }


            // Backspace
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && Text.Length > 0 ||
                Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace) && Text.Length >0)
            {
                Text = Text.Substring(0, Text.Length - 1);
                CreatBlinkTimer = 0f;
                CreatVisible = true;
            }


            // TODO: Add visible cursor

            // TODO: Text row break when hitting border

            // TODO: Scroll logicZ

            // TODO: Font?

        }

        public void Clear() => Text = string.Empty;
    }

}

