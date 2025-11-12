using System.Numerics;
using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens
{
    public class StartScreen
    {
        private static Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.0.png");
        
        // UI Components
        private static TextField userField = new TextField(
            new Rectangle(0, 0, 0, 0),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false
        );

        private static TextField passwordField = new TextField(
            new Rectangle(0, 0, 0, 0),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor,
            allowMultiline: false,
            isPassword: true
        );

        // Buttons
        private static Button registerButton = new Button(
            new Rectangle(0, 0, 0, 0),
            "Register", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor
        );

        private static Button loginButton = new Button(
            new Rectangle(0, 0, 0, 0),
            "Login", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor
        );

        private static OptionsButton ducktionButton = new OptionsButton(
            new Rectangle(0, 0, 0, 0)
        );
        
        // Imports logics data
        private static StartScreenLogic logic = new StartScreenLogic(
            userField, passwordField, loginButton, registerButton, ducktionButton
        );
        
        
        // Layout and state variables
        private static bool initialized = false;
        private static StartScreenLayout.LayoutData layout;

        // Main function
        public static void Run()
        {
            if (!initialized)
            {
                InitializeUI();
                initialized = true;
            }

            logic.HandleInput();
            Render();
        }

        // UI Initialization (calculate positions and sizes)
        // Updates when screen size changes
        private static void InitializeUI()
        {
            // Assigns the calculated rectangles to the UI components
            // Aligns logics and methods with their visual representation
            layout = StartScreenLayout.Calculate(logo.Width);

            userField.SetRect(layout.UserRect);
            passwordField.SetRect(layout.PassRect);
            loginButton.SetRect(layout.LoginRect);
            registerButton.SetRect(layout.RegisterRect);
            ducktionButton.SetRect(layout.OptionsRect);
        }
        
        // Update and draw UI elements each frame
        public static void Render()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Colors.BackgroundColor);
            
            // Draw labels for text fields
            int labelFont = 15;
            int labelYUser = (int)(layout.UserRect.Y + (layout.UserRect.Height - labelFont) / 2f);
            int labelYPass = (int)(layout.PassRect.Y + (layout.PassRect.Height - labelFont) / 2f);
            Raylib.DrawText("Username:", (int)(layout.UserRect.X - 110), labelYUser, labelFont, Colors.TextFieldColor);
            Raylib.DrawText("Password:", (int)(layout.UserRect.X - 110), labelYPass, labelFont, Colors.TextFieldColor);
            
            // Update and draw fields/buttons
            userField.Update();
            userField.Draw();

            passwordField.Update();
            passwordField.Draw();

            registerButton.Draw();
            loginButton.Draw();
            ducktionButton.Draw();

            // Draw logo & version text
            Raylib.DrawTextureEx(logo, new Vector2(layout.LogoX, layout.LogoY), 0, layout.LogoScale, Color.White);
            Raylib.DrawText("DuckLord v.0.0.2", 10, (int)(layout.ScreenHeight - 20), 10, Colors.TextColor);
            Raylib.EndDrawing();
        }
    }
}