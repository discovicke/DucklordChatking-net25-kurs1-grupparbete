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

        public TextField(Rectangle rect, Color backgroundColor, Color hoverColor, Color textColor,
            bool allowMultiline = false)
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
                // TODO add arrows left and right to follow the text array
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
            int availableWidth = (int)Rect.Width - Padding * 2;
            int textWidth = Raylib.MeasureText(Text, FontSize);
            int maxScroll = Math.Max(0, textWidth - availableWidth);
            scrollOffset = Math.Clamp(scrollOffset, 0, maxScroll);
            Raylib.DrawText(Text, textX - scrollOffset, textY, FontSize, TextColor);
        }
        private void ResetCaretBlink()
        {
            CreatBlinkTimer = 0f;
            CreatVisible = true;
        }

        private void MoveCursorLeft()
        {
            if (CursorPositon > 0)
            {
                CursorPositon--;
                ResetCaretBlink();
            }
        }
        private void MoveCursorRight()
        {
            if (CursorPositon < Text.Length)
            {
                CursorPositon++;
                ResetCaretBlink();
            }
        }


        //---
        //private void DrawSingleLineText(int textX, int textY)
        //{
        //    // Count text width and scroll if needed
        //    int textWidth = Raylib.MeasureText(Text, FontSize);
        //    int availableWidth = (int)Rect.Width - Padding * 2;
        //
        //    if (textWidth > availableWidth)
        //    {
        //        scrollOffset = textWidth - availableWidth;
        //    }
        //    else
        //    {
        //        scrollOffset = 0;
        //    }
        //
        //    Raylib.DrawText(Text, textX - scrollOffset, textY, FontSize, TextColor);
        //}
        //-----

        // Responsible for drawing multiline text with rowbreak
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
                    currentY += FontSize + 2; // line spacing
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
        // TODO bugg Fix for text sta in the chat box
        private (int caretX, int caretY) GetCaretPixelPosition(int textX, int textY)
        {
            int cursor = Math.Clamp(CursorPositon, 0, Text.Length);

            if (!AllowMultiline)
            {
                string left = cursor > 0 ? Text.Substring(0, cursor) : "";
                int leftWidth = Raylib.MeasureText(left, FontSize);
                int caretX = textX - scrollOffset + leftWidth;
                int caretY = textY;
                return (caretX, caretY);
            }

            int availableWidth = (int)Rect.Width - Padding * 2;

            // Text up to the cursor
            string preText = cursor > 0 ? Text.Substring(0, cursor) : "";

            // Split into manual paragraphs up to the cursor
            var paras = preText.Split('\n');

            // Count fully completed wrapped lines before the last paragraph
            int linesBefore = 0;
            for (int i = 0; i < paras.Length - 1; i++)
            {
                linesBefore += WrapText(paras[i], availableWidth).Count;
            }

            // Wrap the last partial paragraph and measure its last line
            string lastPara = paras.Length > 0 ? paras[^1] : "";
            var wrappedLast = WrapText(lastPara, availableWidth);
            string lastLine = wrappedLast.Count > 0 ? wrappedLast[^1] : "";
            int lastLineWidth = Raylib.MeasureText(lastLine, FontSize);

            int caretLineIndex = linesBefore + Math.Max(0, wrappedLast.Count - 1);
            int caretXPos = textX + lastLineWidth;
            int caretYPos = textY + caretLineIndex * (FontSize + 2);
            return (caretXPos, caretYPos);
        }
        // Draws caret at the end of the text
        // TODO: Change caret position with arrow keys and mouse click
        private void DrawCaret(int textX, int textY)
        {
            var (cx, cy) = GetCaretPixelPosition(textX, textY); // ADDED: use helper for clarity
            Raylib.DrawLine(cx, cy, cx, cy + FontSize, TextColor);
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
                    var s = char.ConvertFromUtf32(key);
                    CursorPositon = Math.Clamp(CursorPositon, 0, Text.Length);
                    Text = Text.Insert(CursorPositon, s);
                    CursorPositon += s.Length;
                    CreatBlinkTimer = 0f;
                    CreatVisible = true;
                }
                key = Raylib.GetCharPressed();

            }
            //  Enter adds newline only if multiline
            if (AllowMultiline
                 && (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                 && Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                CursorPositon = Math.Clamp(CursorPositon, 0, Text.Length);
                Text = Text.Insert(CursorPositon, "\n");
                CursorPositon++;
                CreatBlinkTimer = 0f;
                CreatVisible = true;
            }


            // Backspace
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && CursorPositon > 0 ||
                Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace) && CursorPositon > 0)
            {
                CursorPositon = Math.Clamp(CursorPositon, 0, Text.Length);
                Text = Text.Remove(CursorPositon - 1, 1);
                CreatBlinkTimer = 0f;
                CursorPositon--;
                CreatVisible = true;
            }

            // TODO: Text row break when hitting border

            // TODO: Scroll logicZ

            // TODO: Font?
        }

        public void Clear()
        {
            Text = string.Empty;
            CursorPositon = 0;
        }

    }
}