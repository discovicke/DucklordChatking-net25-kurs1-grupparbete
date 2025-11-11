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
    public class StartScreen
    {
        // private enum SelectedField { None, Username, Password }
        // private static SelectedField selected = SelectedField.None;
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

            // Rectangles
            Rectangle rectUser = new Rectangle(rectX, rectY, rectWidth, rectHeight);
            Rectangle rectPassword = new Rectangle(rectX, rectY + 50, rectWidth, rectHeight);
            Rectangle rectRegister = new Rectangle(rectX, rectY + 200, rectWidth, rectHeight);
            Rectangle rectLogin = new Rectangle(rectX + 300, rectY, rectWidth, rectHeight);

            // TextFields
            var userField = new TextField(rectUser, Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
            userField.Draw();

            var passwordField = new TextField(rectPassword, Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
            passwordField.Draw(); 
            
            // Buttons
            Button registerButton = new Button(rectRegister, "Register", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
            registerButton.Draw();

            Button loginButton = new Button(rectLogin, "Login", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
            loginButton.Draw();
           
            // Logo
            Raylib.DrawTextureEx(logo, new Vector2(300, 50), 0, 0.15f, Color.White);

            Raylib.EndDrawing();
        }
    }
}
