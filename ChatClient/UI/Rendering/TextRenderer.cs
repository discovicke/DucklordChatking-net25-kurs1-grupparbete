using System.Numerics;
using ChatClient.Core.Infrastructure;
using Raylib_cs;

namespace ChatClient.UI.Components.Text
{
    public class TextRenderer
    {
        private Rectangle bounds;
        private readonly Color textColor;
        private readonly bool isPassword;
        private readonly bool allowMultiline;

        private const int FontSize = 20;
        private const int Padding = 8;
        private const int LineSpacing = 2;

        // Horizontal scrolling state for single-line
        private int scrollIndex = 0;

        public TextRenderer(Rectangle bounds, Color textColor, bool isPassword, bool allowMultiline)
        {
            this.bounds = bounds;
            this.textColor = textColor;
            this.isPassword = isPassword;
            this.allowMultiline = allowMultiline;
        }

        public void UpdateBounds(Rectangle newBounds) => bounds = newBounds;

        public void Draw(string text, TextCursor cursor, bool isSelected, bool showCursor)
        {
            if (string.IsNullOrEmpty(text) && !showCursor)
                return;

            float textX = bounds.X + Padding;
            float textY = allowMultiline
                ? bounds.Y + Padding
                : bounds.Y + (bounds.Height - FontSize) / 2f;

            Raylib.BeginScissorMode((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);

            if (allowMultiline)
                DrawMultilineText(text, textX, textY);
            else
                DrawSingleLineText(text, textX, textY, cursor);

            Raylib.EndScissorMode();

            if (isSelected && showCursor && cursor.IsVisible)
                DrawCaret(text, textX, textY, cursor);
        }

        private void DrawSingleLineText(string text, float x, float y, TextCursor cursor)
        {
            string full = isPassword ? new string('*', text.Length) : text;
            float maxWidth = bounds.Width - Padding * 2f;

            EnsureScrollForCaret(full, cursor.Position, maxWidth);

            string visible = SubstringThatFits(full, scrollIndex, maxWidth);
            Raylib.DrawTextEx(ResourceLoader.MediumFont, visible, new Vector2(x, y), FontSize, 0.5f, textColor);
        }

        private void DrawCaret(string text, float x, float y, TextCursor cursor)
        {
            if (allowMultiline)
            {
                var (cx, cy) = GetCaretPixelPosition(text, x, y, cursor);
                Raylib.DrawLine((int)cx, (int)cy, (int)cx, (int)(cy + FontSize), textColor);
                return;
            }

            string full = isPassword ? new string('*', text.Length) : text;
            float maxWidth = bounds.Width - Padding * 2f;

            EnsureScrollForCaret(full, cursor.Position, maxWidth);

            int caret = Math.Clamp(cursor.Position, 0, full.Length);
            int leftCount = Math.Max(0, caret - scrollIndex);
            string leftOfCaret = leftCount > 0 ? full.Substring(scrollIndex, leftCount) : "";
            float leftWidth = Raylib.MeasureTextEx(ResourceLoader.MediumFont, leftOfCaret, FontSize, 0.5f).X;

            float carX = x + leftWidth;
            Raylib.DrawLine((int)carX, (int)y, (int)carX, (int)(y + FontSize), textColor);
        }

        // Keep caret visible by adjusting scrollIndex
        private void EnsureScrollForCaret(string full, int caretIndex, float maxWidth)
        {
            caretIndex = Math.Clamp(caretIndex, 0, full.Length);
            scrollIndex = Math.Clamp(scrollIndex, 0, full.Length);

            // If caret moved left beyond view, snap start to caret
            if (caretIndex < scrollIndex)
            {
                scrollIndex = caretIndex;
                return;
            }

            // If caret is to the right and does not fit, advance start until it fits
            while (scrollIndex < caretIndex)
            {
                string span = full.Substring(scrollIndex, caretIndex - scrollIndex);
                float w = Raylib.MeasureTextEx(ResourceLoader.MediumFont, span, FontSize, 0.5f).X;
                if (w <= maxWidth)
                    break;
                scrollIndex++;
            }
        }

        // Returns the longest substring from startIndex that fits maxWidth
        private string SubstringThatFits(string full, int startIndex, float maxWidth)
        {
            if (startIndex >= full.Length)
                return "";

            int lo = 0;
            int hi = full.Length - startIndex;
            while (lo < hi)
            {
                int mid = (lo + hi + 1) / 2;
                string test = full.AsSpan(startIndex, mid).ToString();
                float w = Raylib.MeasureTextEx(ResourceLoader.MediumFont, test, FontSize, 0.5f).X;
                if (w <= maxWidth)
                    lo = mid;
                else
                    hi = mid - 1;
            }
            return lo <= 0 ? "" : full.Substring(startIndex, lo);
        }

        private void DrawMultilineText(string text, float x, float y)
        {
            float currentY = y;
            var manualLines = text.Split('\n');
            foreach (var line in manualLines)
            {
                var wrappedLines = WrapText(line, (int)bounds.Width - Padding * 2);
                foreach (var wrapped in wrappedLines)
                {
                    Raylib.DrawTextEx(ResourceLoader.MediumFont, wrapped, new Vector2(x, currentY), FontSize, 0.5f, textColor);
                    currentY += FontSize + LineSpacing;
                }
            }
        }

        private (float x, float y) GetCaretPixelPosition(string text, float x, float y, TextCursor cursor)
        {
            int pos = Math.Clamp(cursor.Position, 0, text.Length);
            string display = isPassword ? new string('*', text.Length) : text;
            int availableWidth = (int)bounds.Width - Padding * 2;
            string pre = pos > 0 ? display.Substring(0, pos) : "";
            var paras = pre.Split('\n');

            int linesBefore = 0;
            for (int i = 0; i < paras.Length - 1; i++)
                linesBefore += WrapText(paras[i], availableWidth).Count;

            string lastPara = paras.Length > 0 ? paras[^1] : "";
            var wrappedLast = WrapText(lastPara, availableWidth);
            string lastLine = wrappedLast.Count > 0 ? wrappedLast[^1] : "";
            float lastWidth = Raylib.MeasureTextEx(ResourceLoader.MediumFont, lastLine, FontSize, 0.5f).X;

            int caretLineIndex = linesBefore + Math.Max(0, wrappedLast.Count - 1);
            float caretX = x + lastWidth;
            float caretY = y + caretLineIndex * (FontSize + LineSpacing);
            return (caretX, caretY);
        }

        private List<string> WrapText(string text, int maxWidth)
        {
            var lines = new List<string>();
            var words = text.Split(' ');
            string current = "";
            foreach (var word in words)
            {
                string test = string.IsNullOrEmpty(current) ? word : current + " " + word;
                int w = (int)Raylib.MeasureTextEx(ResourceLoader.MediumFont, test, FontSize, 0.5f).X;
                if (w > maxWidth)
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        lines.Add(current);
                        current = word;
                    }
                    else
                    {
                        lines.AddRange(SplitLongWord(word, maxWidth));
                        current = "";
                    }
                }
                else
                {
                    current = test;
                }
            }
            if (!string.IsNullOrEmpty(current))
                lines.Add(current);
            return lines.Count > 0 ? lines : new List<string> { "" };
        }

        private List<string> SplitLongWord(string word, int maxWidth)
        {
            var parts = new List<string>();
            string current = "";
            foreach (char c in word)
            {
                string test = current + c;
                int w = (int)Raylib.MeasureTextEx(ResourceLoader.MediumFont, test + "-", FontSize, 0.5f).X;
                if (w > maxWidth)
                {
                    parts.Add(current + "-");
                    current = c.ToString();
                }
                else
                {
                    current = test;
                }
            }
            if (!string.IsNullOrEmpty(current))
                parts.Add(current);
            return parts;
        }
    }
}
