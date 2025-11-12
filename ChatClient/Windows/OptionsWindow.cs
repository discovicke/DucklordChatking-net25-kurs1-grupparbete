using ChatClient.Configurations;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Windows
{
    public class OptionsWindow
    {
        // Loade Logo
        private static Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.0.png");

        private static TextField Ducktions= new TextField(
            new Rectangle(500, 300, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );
        private static TextField Kalle = new TextField(
            new Rectangle(300, 300, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );
        private static TextField userField = new TextField(
            new Rectangle(300, 350, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );

        private static TextField passwordField = new TextField(
            new Rectangle(300, 400, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false,
            isPassword: true
        );

        private static TextField passwordFieldC = new TextField(
            new Rectangle(300, 450, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false,
            isPassword: true
        );

        private static Button registerButton = new Button(
            new Rectangle(325, 500, 100, 25),
            "Register", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor
        );



        public static void Run() 
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Colors.BackgroundColor);
            Raylib.DrawText("Ducktions:", 160, 305, 15, Colors.TextFieldColor);

            Ducktions.Update();
            Ducktions.Draw();

            // Logo
            Raylib.DrawTextureEx(logo, new Vector2(300, 50), 0, 0.15f, Color.White);

            Raylib.EndDrawing();
        }
    }
}
