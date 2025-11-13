using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ChatClient.UI;
using ChatClient.UI.Components;

namespace ChatClient.Screens
{
    public class OptionsWindow
    {
        // Loade Logo
        private static Texture2D logo = Raylib.LoadTexture(@"Resources/DuckLord1.2.png");
        /*
        private static OptionsButton Ducktions = new OptionsButton(
            new Rectangle(300, 400, 150, 25)
        );
        */
        private static BackButton Back = new BackButton(new Rectangle(10, 10, 100, 25));

        private static Button confirmButton = new Button(
            new Rectangle(350, 500, 100, 25),
            "Confirm", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor
        );

        private static TextField newUsername = new TextField(
            new Rectangle(325, 350, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );

        /*
        private static TextField newPassword = new TextField(
            new Rectangle(300, 400, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );
        */

        private static TextField newPassword = new TextField(
            new Rectangle(325, 400, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false,
            isPassword: true
        );

        private static TextField passwordFieldC = new TextField(
            new Rectangle(325, 450, 150, 25),
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
            Raylib.DrawText("New username:", 180, 355, 15, Colors.TextFieldColor);
            Raylib.DrawText("New password:", 180, 405, 15, Colors.TextFieldColor);
            Raylib.DrawText("Confirm password:", 180, 455, 15, Colors.TextFieldColor);

            newUsername.Update();
            newUsername.Draw();

            newPassword.Update();
            newPassword.Draw();

            
            confirmButton.Update();
            confirmButton.Draw();
            
            
            Back.Update();
            Back.Draw();

            passwordFieldC.Update();
            passwordFieldC.Draw();

            // Logo
            Raylib.DrawTextureEx(logo, new Vector2(300, 0), 0, 0.15f, Color.White);

            Raylib.EndDrawing();
        }
    }
}