using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ChatClient.Configurations;


namespace ChatClient.Windows
{
    internal class UIkladd
    {
        private enum SelectedField { None, Username, Password }
        private static SelectedField selected = SelectedField.None;
        private static Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.0.png");
        public static void Run()
        {

            Raylib.BeginDrawing();
            // LoginScreen-test
            Raylib.ClearBackground(Colors.BackgroundColor);
            int fontSize = 15;
            string userName = "Username:";
            string passWord = "Password";

            // Rita label
            Raylib.DrawText(userName, 220, 305, fontSize, Colors.TextFieldColor);
            Raylib.DrawText(passWord, 220, 355, fontSize, Colors.TextFieldColor);

            // Beräkna textens bredd
            int textWidth = Raylib.MeasureText(userName, fontSize);

            // Placera rektangeln direkt efter texten
            int rectX = 220 + textWidth + 10; // +10 för lite mellanrum
            int rectY = 300;
            int rectWidth = 150;
            int rectHeight = fontSize + 10; // lite högre än texten

            // Textfält
            Rectangle rectUser = new Rectangle(rectX, rectY, rectWidth, rectHeight);
            Rectangle rectPassword = new Rectangle(rectX, rectY + 50, rectWidth, rectHeight);
            Rectangle rectLogIn = new Rectangle(rectX, rectY, rectWidth, rectHeight);

            bool hoverUser = MouseInput.IsHovered(rectUser);
            bool hoverPassword = MouseInput.IsHovered(rectPassword);
            bool leftPressed = Raylib.IsMouseButtonPressed(MouseButton.Left);

            if (MouseInput.IsLeftClick(rectUser))
            {
                selected = SelectedField.Username;
            }
            else if (MouseInput.IsLeftClick(rectPassword))
            {
                selected = SelectedField.Password;
            }
            else if (leftPressed && !hoverUser && !hoverPassword)
            {
                selected = SelectedField.None;
            }



            Raylib.DrawRectangleRounded(rectUser, 0.3f, 10, Colors.TextFieldColor);

            Raylib.DrawRectangleRounded(rectPassword, 0.3f, 10, Colors.TextFieldColor);
            
            
            
            // Visual hover feedback (outline)
            if (hoverUser)
            {
                Raylib.DrawRectangleRounded(rectUser, 0.3f, 10, Colors.HoverColor);
            }
            if (hoverPassword)
            { 
                Raylib.DrawRectangleRounded(rectPassword, 0.3f, 10, Colors.HoverColor);
            }

            // Logo
            Raylib.DrawTextureEx(logo, new Vector2(300, 50), 0, 0.15f, Color.White);


            Raylib.DrawText("DuckLord v.1.0.0", 10, 580, 10, Colors.TextColor);

            Raylib.EndDrawing();
        }
    }
}
