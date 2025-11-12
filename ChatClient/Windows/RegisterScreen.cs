using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ChatClient.Configurations;
using ChatClient.Data;
using Raylib_cs;



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
        
        // Back button
        private static BackButton backButton = new BackButton(new Rectangle(10, 10, 100, 30));

        public static void Run()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Colors.BackgroundColor);

            // Draw text 
            Raylib.DrawText("Register ID:", 160, 305, 15, Colors.TextFieldColor);
            Raylib.DrawText("New username:", 160, 355, 15, Colors.TextFieldColor);
            Raylib.DrawText("New password:", 160, 405, 15, Colors.TextFieldColor);
            Raylib.DrawText("Confirm password:", 160, 455, 15, Colors.TextFieldColor);

            // Button
            if (MouseInput.IsLeftClick(registerButton.Rect))
            {
                AppState.CurrentScreen = Screen.Start;
                Log.Info("New user registerd, switching to start screen");
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
            Raylib.DrawTextureEx(logo, new Vector2(300, 50), 0, 0.15f, Color.White);

            Raylib.EndDrawing();
        }
    }
}
