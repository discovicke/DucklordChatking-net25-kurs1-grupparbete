﻿using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;
using ChatClient.Core;
using ChatClient.UI.Components;

namespace ChatClient.UI.Rendering
{
    public class TextRenderer(Rectangle bounds, Color textColor, bool isPassword, bool allowMultiline)
    {
        private const int FontSize = 20;
        private const int Padding = 4;
        private const int LineSpacing = 2;

        private int scrollOffset;


        public void UpdateBounds(Rectangle newBounds)
        {
            bounds = newBounds;
        }

        public void Draw(string text, TextCursor cursor, bool isSelected)
        {
            float textX = bounds.X + Padding;
            float textY = bounds.Y + Padding;

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

        private void DrawSingleLineText(string text, float textX, float textY, TextCursor cursor)
        {
            string displayText = isPassword ? new string('*', text.Length) : text;
            AdjustScrollForCaret(displayText, cursor);
            Raylib.DrawTextEx(ResourceLoader.MediumFont, displayText, 
                new Vector2(textX - scrollOffset, textY), FontSize, 0.5f, textColor);
        }

        private void DrawMultilineText(string text, float textX, float textY)
        {
            float currentY = textY;
            var manualLines = text.Split('\n');

            foreach (var line in manualLines)
            {
                var wrappedLines = WrapText(line, (int)bounds.Width - Padding * 2);
                foreach (var wrappedLine in wrappedLines)
                {
                    Raylib.DrawTextEx(ResourceLoader.MediumFont, wrappedLine, 
                        new Vector2(textX, currentY), FontSize, 0.5f, textColor);
                    currentY += FontSize + LineSpacing;
                }
            }
        }

        private void DrawCaret(string text, float textX, float textY, TextCursor cursor)
        {
            var (cx, cy) = GetCaretPixelPosition(text, textX, textY, cursor);
            Raylib.DrawLine((int)cx, (int)cy, (int)cx, (int)(cy + FontSize), textColor);
        }

        private (float x, float y) GetCaretPixelPosition(string text, float textX, float textY, TextCursor cursor)
        {
            int pos = Math.Clamp(cursor.Position, 0, text.Length);

            string displayText = isPassword ? new string('*', text.Length) : text;

            if (!allowMultiline)
            {
                string left = pos > 0 ? displayText.Substring(0, pos) : "";
                float leftWidth = Raylib.MeasureTextEx(ResourceLoader.MediumFont, left, FontSize, 0.5f).X;
                return (textX - scrollOffset + leftWidth, textY);
            }

            int availableWidth = (int)bounds.Width - Padding * 2;
            string preText = pos > 0 ? displayText.Substring(0, pos) : "";
            var paras = preText.Split('\n');

            int linesBefore = 0;
            for (int i = 0; i < paras.Length - 1; i++)
            {
                linesBefore += WrapText(paras[i], availableWidth).Count;
            }

            string lastPara = paras.Length > 0 ? paras[^1] : "";
            var wrappedLast = WrapText(lastPara, availableWidth);
            string lastLine = wrappedLast.Count > 0 ? wrappedLast[^1] : "";
            float lastLineWidth = Raylib.MeasureTextEx(ResourceLoader.MediumFont, lastLine, FontSize, 0.5f).X;

            int caretLineIndex = linesBefore + Math.Max(0, wrappedLast.Count - 1);
            float caretX = textX + lastLineWidth;
            float caretY = textY + caretLineIndex * (FontSize + LineSpacing);

            return (caretX, caretY);
        }

        private void AdjustScrollForCaret(string text, TextCursor cursor)
        {
            if (allowMultiline) return;

            int availableWidth = (int)bounds.Width - Padding * 2;
            int textWidth = (int)Raylib.MeasureTextEx(ResourceLoader.MediumFont, text, FontSize, 0.5f).X;
            int maxScroll = Math.Max(0, textWidth - availableWidth);

            string left = cursor.Position > 0 ? text.Substring(0, cursor.Position) : "";
            string displayLeft = isPassword ? new string('*', left.Length) : left;
            int leftWidth = (int)Raylib.MeasureTextEx(ResourceLoader.MediumFont, displayLeft, FontSize, 0.5f).X;
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
                var testLine = string.IsNullOrEmpty(currentLine)
                    ? word
                    : currentLine + " " + word;
                int lineWidth = (int)Raylib.MeasureTextEx(ResourceLoader.MediumFont, testLine, FontSize, 0.5f).X;

                if (lineWidth > maxWidth)
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                        currentLine = word;
                    }

                    if (Raylib.MeasureTextEx(ResourceLoader.MediumFont, word, FontSize, 0.5f).X > maxWidth)
                    {
                        var splitWordLines = SplitLongWord(word, maxWidth);
                        lines.AddRange(splitWordLines.GetRange(0, splitWordLines.Count - 1));
                        currentLine = splitWordLines[^1];
                    }
                    else
                    {
                        currentLine = word;
                    }
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

        private List<string> SplitLongWord(string word, int maxWidth)
        {
            var lines = new List<string>();
            string currentLine = "";

            foreach (char c in word)
            {
                string test = currentLine + c;

                int testWidth = (int)Raylib.MeasureTextEx(ResourceLoader.MediumFont, test + "-", FontSize, 0.5f).X;

                if (testWidth > maxWidth)
                {
                    lines.Add(currentLine + "-");
                    currentLine = c.ToString();
                }
                else
                {
                    currentLine = test;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);

            return lines;
        }
    }
}