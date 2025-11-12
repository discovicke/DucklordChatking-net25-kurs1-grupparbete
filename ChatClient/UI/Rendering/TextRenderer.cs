using Raylib_cs;
using System.Collections.Generic;
using ChatClient.UI.Components;

namespace ChatClient.UI.Rendering
{
    public class TextRenderer
    {
        private const int FontSize = 20;
        private const int Padding = 4;
        private const int LineSpacing = 2;

        private int scrollOffset;
        private readonly Rectangle bounds;
        private readonly Color textColor;
        private readonly bool isPassword;
        private readonly bool allowMultiline;


        public TextRenderer(Rectangle bounds, Color textColor, bool isPassword, bool allowMultiline)
        {
            this.bounds = bounds;
            this.textColor = textColor;
            this.isPassword = isPassword;
            this.allowMultiline = allowMultiline;
        }

        public void Draw(string text, TextCursor cursor, bool isSelected)
        {
            int textX = (int)(bounds.X + Padding);
            int textY = (int)(bounds.Y + Padding);

            Raylib.BeginScissorMode((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);

            if (allowMultiline)
            {
                DrawMultilineText(text, textX, textY);
            }
            else
            {
                DrawSingleLineText(text, textX, textY, cursor);
            }

            Raylib.EndScissorMode();

            if (isSelected && cursor.IsVisible)
            {
                DrawCaret(text, textX, textY, cursor);
            }
        }

        private void DrawSingleLineText(string text, int textX, int textY, TextCursor cursor)
        {
            string displayText = isPassword ? new string('*', text.Length) : text;
            AdjustScrollForCaret(text, cursor);
            Raylib.DrawText(displayText, textX - scrollOffset, textY, FontSize, textColor);
        }

        private void DrawMultilineText(string text, int textX, int textY)
        {
            int currentY = textY;
            var manualLines = text.Split('\n');

            foreach (var line in manualLines)
            {
                var wrappedLines = WrapText(line, (int)bounds.Width - Padding * 2);
                foreach (var wrappedLine in wrappedLines)
                {
                    Raylib.DrawText(wrappedLine, textX, currentY, FontSize, textColor);
                    currentY += FontSize + LineSpacing;
                }
            }
        }

        private void DrawCaret(string text, int textX, int textY, TextCursor cursor)
        {
            var (cx, cy) = GetCaretPixelPosition(text, textX, textY, cursor);
            Raylib.DrawLine(cx, cy, cx, cy + FontSize, textColor);
        }

        private (int x, int y) GetCaretPixelPosition(string text, int textX, int textY, TextCursor cursor)
        {
            int pos = Math.Clamp(cursor.Position, 0, text.Length);

            if (!allowMultiline)
            {
                string left = pos > 0 ? text.Substring(0, pos) : "";
                int leftWidth = Raylib.MeasureText(left, FontSize);
                return (textX - scrollOffset + leftWidth, textY);
            }

            int availableWidth = (int)bounds.Width - Padding * 2;
            string preText = pos > 0 ? text.Substring(0, pos) : "";
            var paras = preText.Split('\n');

            int linesBefore = 0;
            for (int i = 0; i < paras.Length - 1; i++)
            {
                linesBefore += WrapText(paras[i], availableWidth).Count;
            }

            string lastPara = paras.Length > 0 ? paras[^1] : "";
            var wrappedLast = WrapText(lastPara, availableWidth);
            string lastLine = wrappedLast.Count > 0 ? wrappedLast[^1] : "";
            int lastLineWidth = Raylib.MeasureText(lastLine, FontSize);

            int caretLineIndex = linesBefore + Math.Max(0, wrappedLast.Count - 1);
            int caretX = textX + lastLineWidth;
            int caretY = textY + caretLineIndex * (FontSize + LineSpacing);

            return (caretX, caretY);
        }

        private void AdjustScrollForCaret(string text, TextCursor cursor)
        {
            if (allowMultiline) return;

            int availableWidth = (int)bounds.Width - Padding * 2;
            int textWidth = Raylib.MeasureText(text, FontSize);
            int maxScroll = Math.Max(0, textWidth - availableWidth);

            string left = cursor.Position > 0 ? text.Substring(0, cursor.Position) : "";
            int leftWidth = isPassword
                ? Raylib.MeasureText(new string('*', left.Length), FontSize)
                : Raylib.MeasureText(left, FontSize);
            int caretXLocal = leftWidth - scrollOffset;

            if (caretXLocal < 0)
            {
                scrollOffset = Math.Max(0, leftWidth - 8);
            }
            else if (caretXLocal > availableWidth)
            {
                scrollOffset = Math.Min(maxScroll, leftWidth - availableWidth + 8);
            }

            scrollOffset = Math.Clamp(scrollOffset, 0, maxScroll);
        }

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
    }
}