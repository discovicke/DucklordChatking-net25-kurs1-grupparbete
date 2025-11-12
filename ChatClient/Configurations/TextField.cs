using Raylib_cs;
using System;

namespace ChatClient.Configurations
{
    // Text field component handle draw, hover and textinput (incl. backspace).
    // Stores text between frames.
    public class TextField
        : UIComponent
    {
        private Rectangle Rect { get; }
        public string Text { get; private set; } = string.Empty;
        private Color BackgroundColor { get; set; }
        private Color HoverColor { get; set; }
        private Color TextColor { get; set; }
        private bool IsSelected { get; set; } = false;
        private bool AllowMultiline { get; set; } = false;

        private float CreatBlinkTimer = 0f;
        private bool CreatVisible = true;
        private int scrollOffset = 0; // For single line text
        private int CursorPositon { get; set; } = 0;
        private const int FontSize = 20;
        private const int Padding = 5;
        // private bool IsPassword { get; } = isPassword;
        // private string PasswordMask { get; } = string.IsNullOrEmpty(passwordMask) ? "•" : passwordMask;

        public TextField(Rectangle rect, Color backgroundColor, Color hoverColor, Color textColor, bool allowMultiline = false)
        {
            Rect = rect;
            BackgroundColor = backgroundColor;
            HoverColor = hoverColor;
            TextColor = textColor;
            AllowMultiline = allowMultiline;
        }
        // TODO: Ctrl+A, Ctr+C, Ctr+V
        // TODO: Ctrl+ArrowKey to move cursor over words and not characters
        public override void Draw()
        {
            var fill = MouseInput.IsHovered(Rect) ? HoverColor : BackgroundColor;
            Raylib.DrawRectangleRounded(Rect, 0.3f, 10, fill);

            if (MouseInput.IsHovered(Rect))
            {
                Raylib.DrawRectangleRoundedLinesEx(Rect, 0.3f, 10, 2, TextColor);
            }

            int textX = (int)(Rect.X + Padding);
            int textY = (int)(Rect.Y + Padding);

            Raylib.BeginScissorMode((int)Rect.X, (int)Rect.Y, (int)Rect.Width, (int)Rect.Height);
            // Checks if text is multiline or single line for text drawing
            if (AllowMultiline)
            {
                DrawMultilineText(textX, textY);
            }
            else
            {
                DrawSingleLineText(textX, textY);
            }
            Raylib.EndScissorMode();

            if (IsSelected && CreatVisible)
            {
                DrawCaret(textX, textY);
            }
        }

        private void DrawSingleLineText(int textX, int textY)
        {
            // Count text width and scroll if needed
            int textWidth = Raylib.MeasureText(Text, FontSize);
            int availableWidth = (int)Rect.Width - Padding * 2;

            if (textWidth > availableWidth)
            {
                scrollOffset = textWidth - availableWidth;
            }
            else
            {
                scrollOffset = 0;
            }

            Raylib.DrawText(Text, textX - scrollOffset, textY, FontSize, TextColor);
        }

        // Responsible for drawing multiline text with rowbreak
        // TODO: Rowbreak logic with Shift+Enter
        private void DrawMultilineText(int textX, int textY)
        {
            var lines = WrapText(Text, (int)Rect.Width - Padding * 2);
            int currentY = textY;
            var manualLines = Text.Split('\n');

            foreach (var i in manualLines)
            {
                var wrappedLines = WrapText(i, (int)Rect.Width - Padding * 2);
                foreach (var line in wrappedLines)
                {
                    Raylib.DrawText(line, textX, currentY, FontSize, TextColor);
                    currentY += FontSize + 2; // Radavstånd
                }
            }
        }

        // Wraps text with a string and splits it into lines.
        private List<string> WrapText(string text, int maxWidth)
        {
            var lines = new List<string>();
            var words = text.Split(' ');
            var currentLine = "";

            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                int lineWidth = Raylib.MeasureText(testLine, FontSize);

                if (lineWidth > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }

            return lines.Count > 0 ? lines : new List<string> { "" };
        }

        // Draws caret at the end of the text
        // TODO: Change caret position with arrow keys and mouse click
        private void DrawCaret(int textX, int textY)
        {
            int textWidth = Raylib.MeasureText(Text, FontSize);
            int caretX = AllowMultiline ? textX + textWidth : textX - scrollOffset + textWidth;

            if (AllowMultiline)
            {
                var lines = WrapText(Text, (int)Rect.Width - Padding * 2);
                int lineCount = lines.Count;
                textY += (lineCount - 1) * (FontSize + 2);

                if (lines.Count > 0)
                {
                    int lastLineWidth = Raylib.MeasureText(lines[^1], FontSize);
                    caretX = textX + lastLineWidth;
                }
            }

            int caretTop = textY;
            int caretBottom = caretTop + FontSize;
            Raylib.DrawLine(caretX, caretTop, caretX, caretBottom, TextColor);
        }

        // Get call every frame after MouseInput.Update() to handle click and text-input.
        public override void Update()
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

            if (!IsSelected)
            {
                return;
            }

            // Read text input
            int key = Raylib.GetCharPressed();

            while (key > 0)
            {
                // Accepts all Unicode-tecken (>= 32)
                if (key >= 32)
                {
                    // Converts to Unicode to accept all even åäö
                    Text += char.ConvertFromUtf32(key);
                    CursorPositon++;
                    CreatBlinkTimer = 0f;
                    CreatVisible = true;
                }
                key = Raylib.GetCharPressed();

            }
            //  Enter adds newline only if multiline
            if (AllowMultiline
                && (Raylib.IsKeyDown(KeyboardKey.LeftShift) && Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                && Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                Text = Text.Insert(CursorPositon, "\n");
                CreatBlinkTimer = 0f;
                CursorPositon++;
                CreatVisible = true;
            }


            // Backspace
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && CursorPositon > 0 ||
                Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace) && CursorPositon > 0)
            {
                Text = Text.Remove( CursorPositon - 1,1);
                CreatBlinkTimer = 0f;
                CursorPositon--;
                CreatVisible = true;
            }

            // TODO: Text row break when hitting border

            // TODO: Scroll logicZ

            // TODO: Font?
        }

        public void Clear() => Text = string.Empty;
    }
}