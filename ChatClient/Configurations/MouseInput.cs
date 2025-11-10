using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace ChatClient.Configurations
{
    internal static class MouseInput
    {
        private static Vector2 _mousePos;

        public static void Update() 
        {
            _mousePos = Raylib.GetMousePosition();
        }

        // mouse position
        public static Vector2 Position => _mousePos;

        // True if mouse curser are over rectangle
        public static bool IsHovered(Rectangle rect)
        {
            return Raylib.CheckCollisionPointRec(_mousePos, rect);
        }

        // True if mouse click is left when over rectangle 
        public static bool IsLeftClick(Rectangle rect)
        {
            return IsHovered(rect) && Raylib.IsMouseButtonPressed(MouseButton.Left);
        }

        // colore change if mouse hoovers over rectangle
        public static Color ButtonColor(Rectangle rect, Color hoverColor, Color normalColor)
        {
            return IsHovered(rect) ? hoverColor : normalColor;
        }

    }
}
