using System.Numerics;
using ChatClient.Configurations;
using Raylib_cs;

namespace ChatClient.Windows
{
    public class StartScreen
    {
        // Ladda logon direkt som statiskt fält
        private static Texture2D logo = Raylib.LoadTexture(@"Bilder/DuckLord1.0.png");

        // Skapa textfält och knappar som statiska fält
        private static TextField userField = new TextField(
            new Rectangle(300, 300, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor
        );

        private static TextField passwordField = new TextField(
            new Rectangle(300, 350, 150, 25),
            Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor
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

            // Buttons
           
            if (MouseInput.IsLeftClick(loginButton.Rect))
            {
                AppState.CurrentScreen = Screen.Chat;
                Log.Info("User logged in, switching to chat screen");
            }
            
            // Uppdatera och rita fälten
            userField.Update();
            userField.Draw();

            passwordField.Update();
            passwordField.Draw();

            registerButton.Draw();
            loginButton.Draw();

            // Logo
            Raylib.DrawTextureEx(logo, new Vector2(300, 50), 0, 0.15f, Color.White);

            Raylib.EndDrawing();
        }
    }
}


/*
public static void Run()
{

    Raylib.BeginDrawing();
    // LoginScreen-test
    Raylib.ClearBackground(Colors.BackgroundColor);
    int fontSize = 15;
    string userName = "Username:";
    string passWord = "Password";

    // Draw label
    Raylib.DrawText(userName, 220, 305, fontSize, Colors.TextFieldColor);
    Raylib.DrawText(passWord, 220, 355, fontSize, Colors.TextFieldColor);

    // Calculate the text width
    int textWidth = Raylib.MeasureText(userName, fontSize);

    // Place rectangle after text
    int rectX = 220 + textWidth + 10; // +10 for spaceing
    int rectY = 300;
    int rectWidth = 150;
    int rectHeight = fontSize + 10; // +10 for centering text vs rectangle

    // Rectangles
    Rectangle rectUser = new Rectangle(rectX, rectY, rectWidth, rectHeight);
    Rectangle rectPassword = new Rectangle(rectX, rectY + 50, rectWidth, rectHeight);
    Rectangle rectRegister = new Rectangle(rectX, rectY + 200, rectWidth, rectHeight);
    Rectangle rectLogin = new Rectangle(rectX + 300, rectY, rectWidth, rectHeight);

    // TextFields
    var userField = new TextField(rectUser, Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
    userField.Update();
    userField.Draw();

    var passwordField = new TextField(rectPassword, Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
    passwordField.Update();
    passwordField.Draw(); 

    // Buttons
    Button registerButton = new Button(rectRegister, "Register", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
    registerButton.Draw();

            Button loginButton = new Button(rectLogin, "Login", Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
            loginButton.Draw();
            if (MouseInput.IsLeftClick(rectLogin))
            {
                AppState.CurrentScreen = Screen.Chat;
                Log.Info("User logged in, switching to chat screen");
            }
           
            // Logo
            Raylib.DrawTextureEx(logo, new Vector2(300, 50), 0, 0.15f, Color.White);

    Raylib.EndDrawing();
}
}
}
*/