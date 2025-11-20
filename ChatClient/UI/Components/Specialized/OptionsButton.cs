// csharp
using System.Numerics;
using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Theme;
using Raylib_cs;

namespace ChatClient.UI.Components.Specialized
{
    /// <summary>
    /// Settings button with a vector gear icon (no text).
    /// Draws a rounded background and a cog using Raylib primitives.
    /// </summary>
    public class OptionsButton(Rectangle rect)
        : Button(rect, "", Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor) // empty label
    {
        public override void Draw()
        {
            // Resolve hover state to pick background color
            var mouse = Raylib.GetMousePosition();
            bool hovered = Raylib.CheckCollisionPointRec(mouse, Rect);
            Color bg = hovered ? Colors.ButtonHovered : Colors.ButtonDefault;

            // Button background
            Raylib.DrawRectangleRounded(Rect, 0.12f, 12, bg);
            Raylib.DrawRectangleRoundedLinesEx(Rect, 0.12f, 12, 1f, Colors.OutlineColor);

            // --- TEST Gear Icon ---
            string gearIcon = "\uf013";
            float fontSize = Rect.Height * 0.6f;
            Vector2 textSize = Raylib.MeasureTextEx(ResourceLoader.SymbolFont, gearIcon, fontSize, 0);
            // Center icon in button
            Vector2 pos = new Vector2(
                Rect.X + (Rect.Width - textSize.X) * 0.5f,
                Rect.Y + (Rect.Height - textSize.Y) * 0.5f
            );
            Raylib.DrawTextEx(
            ResourceLoader.SymbolFont,
            gearIcon,
            pos,
            fontSize,
            0,
            Colors.TextColor
            );
        }

        public override void Update()
        {
            // Keep existing behavior (navigate back if stack allows)
            if (AppState.CanGoBack && IsClicked())
            {
                Log.Info($"[BackButton] Navigating back from {AppState.CurrentScreen}");
                AppState.GoBack();
                Log.Info($"[BackButton] Navigated to {AppState.CurrentScreen}");
            }
        }

        private static void DrawRing(Vector2 center, float inner, float outer, Color color)
        {
            // Use DrawRing when available, else emulate with two circles
            Raylib.DrawRing(center, inner, outer, 0f, 360f, 48, color);
        }
    }
}
