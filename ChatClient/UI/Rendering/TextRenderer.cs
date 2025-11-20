using System.Numerics;
using ChatClient.Core.Infrastructure;
using Raylib_cs;

namespace ChatClient.UI.Components.Text
{
    public class TextRenderer(Rectangle bounds, Color textColor, bool isPassword, bool allowMultiline)
    {
        public Rectangle Bounds { get; private set; } = bounds;
        public Color TextColor { get; } = textColor;
        public bool IsPassword { get; } = isPassword;
        public bool AllowMultiline { get; } = allowMultiline;

        public int ScrollIndex { get; private set; }

        private const int FontSize = 20;
        private const int Padding = 8;
        private const int LineSpacing = 2;

        public void UpdateBounds(Rectangle newBounds) => Bounds = newBounds;

        public void Draw(string text, TextCursor cursor, bool isSelected, bool showCursor)
        {
            if (string.IsNullOrEmpty(text) && !showCursor)
                return;

            float textX = Bounds.X + Padding;
            float textY = AllowMultiline
                ? Bounds.Y + Padding
                : Bounds.Y + (Bounds.Height - FontSize) / 2f;

            Raylib.BeginScissorMode((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height);

            if (AllowMultiline)
                DrawMultilineText(text, textX, textY);
            else
                DrawSingleLineText(text, textX, textY, cursor);

            Raylib.EndScissorMode();

            if (isSelected && showCursor && cursor.IsVisible)
                DrawCaret(text, textX, textY, cursor);
        }

        private void DrawSingleLineText(string text, float x, float y, TextCursor cursor)
        {
            string full = IsPassword ? new string('*', text.Length) : text;
            float maxWidth = Bounds.Width - Padding * 2f;

            EnsureScrollForCaret(full, cursor.Position, maxWidth);

            string visible = SubstringThatFits(full, ScrollIndex, maxWidth);
            Raylib.DrawTextEx(ResourceLoader.MediumFont, visible, new Vector2(x, y), FontSize, 0.5f, TextColor);
        }

        private void DrawCaret(string text, float x, float y, TextCursor cursor)
        {
            if (AllowMultiline)
            {
                var (cx, cy) = GetCaretPixelPosition(text, x, y, cursor);
                Raylib.DrawLine((int)cx, (int)cy, (int)cx, (int)(cy + FontSize), TextColor);
                return;
            }

            string full = IsPassword ? new string('*', text.Length) : text;
            float maxWidth = Bounds.Width - Padding * 2f;

            EnsureScrollForCaret(full, cursor.Position, maxWidth);

            int caret = Math.Clamp(cursor.Position, 0, full.Length);
            int leftCount = Math.Max(0, caret - ScrollIndex);
            string leftOfCaret = leftCount > 0 ? full.Substring(ScrollIndex, leftCount) : "";
            float leftWidth = Raylib.MeasureTextEx(ResourceLoader.MediumFont, leftOfCaret, FontSize, 0.5f).X;

            float carX = x + leftWidth;
            Raylib.DrawLine((int)carX, (int)y, (int)carX, (int)(y + FontSize), TextColor);
        }

        // Keep caret visible by adjusting scrollIndex
        private void EnsureScrollForCaret(string full, int caretIndex, float maxWidth)
        {
            caretIndex = Math.Clamp(caretIndex, 0, full.Length);
            ScrollIndex = Math.Clamp(ScrollIndex, 0, full.Length);

            // If caret moved left beyond view, snap start to caret
            if (caretIndex < ScrollIndex)
            {
                ScrollIndex = caretIndex;
                return;
            }

            // If caret is to the right and does not fit, advance start until it fits
            while (ScrollIndex < caretIndex)
            {
                string span = full.Substring(ScrollIndex, caretIndex - ScrollIndex);
                float w = Raylib.MeasureTextEx(ResourceLoader.MediumFont, span, FontSize, 0.5f).X;
                if (w <= maxWidth)
                    break;
                ScrollIndex++;
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
                var wrappedLines = WrapText(line, (int)Bounds.Width - Padding * 2);
                foreach (var wrapped in wrappedLines)
                {
                    Raylib.DrawTextEx(ResourceLoader.MediumFont, wrapped, new Vector2(x, currentY), FontSize, 0.5f, TextColor);
                    currentY += FontSize + LineSpacing;
                }
            }
        }

        private (float x, float y) GetCaretPixelPosition(string text, float x, float y, TextCursor cursor)
        {
            int pos = Math.Clamp(cursor.Position, 0, text.Length);
            string display = IsPassword ? new string('*', text.Length) : text;
            int availableWidth = (int)Bounds.Width - Padding * 2;
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
