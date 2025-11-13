using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ChatClient.Core;
using ChatClient.Data;
using ChatClient.UI.Components;
using Raylib_cs;



namespace ChatClient.UI.Screens
{
    public class RegisterScreen
    {
        // Load logo 
        private static Texture2D logo = Raylib.LoadTexture(@"Resources/DuckLord1.2.png");

        // Create text fields and button
        private static TextField idField = new TextField(
            new Rectangle(325, 300, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );
        private static TextField userField = new TextField(
            new Rectangle(325, 350, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );

        private static TextField passwordField = new TextField(
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
            new Rectangle(350, 500, 100, 25),
            "Register", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor
        );
        
        // Back button
        private static BackButton backButton = new BackButton(new Rectangle(10, 10, 100, 30));

        public static void Run()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Colors.BackgroundColor);

            // Draw text 
            Raylib.DrawText("Register ID:", 180, 305, 15, Colors.TextFieldColor);
            Raylib.DrawText("New username:", 180, 355, 15, Colors.TextFieldColor);
            Raylib.DrawText("New password:", 180, 405, 15, Colors.TextFieldColor);
            Raylib.DrawText("Confirm password:", 180, 455, 15, Colors.TextFieldColor);

            // Button
            if (MouseInput.IsLeftClick(registerButton.Rect))
            {
                AppState.CurrentScreen = Screen.Start;
                Log.Info("New user registerd, switching to start screen");
                passwordField.Clear();
                passwordFieldC.Clear();
                userField.Clear();
                idField.Clear();
            }

            if (MouseInput.IsLeftClick(backButton.Rect))
            {
                passwordField.Clear();
                passwordFieldC.Clear();
                userField.Clear();
                idField.Clear();
            }

        //Back button
            backButton.Update();
            backButton.Draw();

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
            Raylib.DrawTextureEx(logo, new Vector2(300, 0), 0, 0.14f, Color.White);

            Raylib.EndDrawing();
        }
    }
}
