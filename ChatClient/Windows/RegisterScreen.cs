using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ChatClient.Configurations;
using ChatClient.Data;
using Raylib_cs;
using static Raylib_cs.Raylib;


namespace ChatClient.Windows
{
    public class RegisterScreen
    {
        // Load logo 
        private static Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.0.png");

        // Create text fields and button
        private static TextField idField = new TextField(
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
            allowMultiline: false
        );

        private static TextField passwordFieldC = new TextField(
            new Rectangle(300, 450, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );

        private static Button registerButton = new Button(
            new Rectangle(325, 500, 100, 25),
            "Register", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor
        );


        public static void Run()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Colors.BackgroundColor);

            // Draw text 
            DrawText("Register ID:", 160, 305, 15, Colors.TextFieldColor);
            DrawText("New username:", 160, 355, 15, Colors.TextFieldColor);
            DrawText("New password:", 160, 405, 15, Colors.TextFieldColor);
            DrawText("Confirm password:", 160, 455, 15, Colors.TextFieldColor);

            // Button
            if (MouseInput.IsLeftClick(registerButton.Rect))
            {
                AppState.CurrentScreen = Screen.Start;
                Log.Info("New user registerd, switching to start screen");
            }

            // Update and draw fields
            idField.Update();
            idField.Draw();

            userField.Update();
            userField.Draw();

            passwordField.Update();
            passwordField.Draw();

            passwordFieldC.Update();
            passwordFieldC.Draw();

            registerButton.Draw();

            // Logo
            DrawTextureEx(logo, new Vector2(300, 50), 0, 0.15f, Color.White);

            EndDrawing();
        }
    }
}
