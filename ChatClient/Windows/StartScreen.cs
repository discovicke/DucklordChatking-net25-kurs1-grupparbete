using System.Numerics;
using ChatClient.Configurations;
using Raylib_cs;

namespace ChatClient.Windows
{
    public class StartScreen
    {
        // Load logo
        private static Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.0.png");

        // Create text fields and buttons
        private static TextField userField = new TextField(
            new Rectangle(300, 300, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );

        private static TextField passwordField = new TextField(
            new Rectangle(300, 350, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );


        private static Button registerButton = new Button(
            new Rectangle(325, 450, 100, 25),
            "Register", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor
        );

        private static Button loginButton = new Button(new Rectangle(325, 400, 100, 25),
            "Login", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor
        );

        public static void Run()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Colors.BackgroundColor);

            Raylib.DrawText("Username:", 220, 305, 15, Colors.TextFieldColor);
            Raylib.DrawText("Password:", 220, 355, 15, Colors.TextFieldColor);

            // Button logics (change screens)
            if (MouseInput.IsLeftClick(loginButton.Rect) || Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                AppState.CurrentScreen = Screen.Chat;
                Log.Info("User logged in, switching to chat screen");
            }

            if (MouseInput.IsLeftClick(registerButton.Rect))
            {
                AppState.CurrentScreen = Screen.Register;
                Log.Info("User want to register, switching to register screen");
            }

            // Update and draw fields/buttons
            userField.Update();
            userField.Draw();

            passwordField.Update();
            passwordField.Draw();

            registerButton.Draw();
            loginButton.Draw();

            // Draw logo
            Raylib.DrawTextureEx(logo, new Vector2(300, 50), 0, 0.15f, Color.White);

            Raylib.EndDrawing();
        }
    }
}