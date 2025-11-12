using ChatClient.Core;
using ChatClient.Data;
using ChatClient.Screens;
using ChatClient.UI;
using ChatClient.UI.Components;
using ChatClient.UI.Screens;
using Raylib_cs;


namespace ChatClient;

public class Program
{
    // TODO: Add relative spacing to window size
    // TODO: What happens when connection to server is lost?
    public static void Main()
    {
        string appVersion = "DuckLord v.0.0.2";

        Raylib.InitWindow(800, 600, appVersion);
        Raylib.SetTargetFPS(60);
        Console.WriteLine($"Log path: {AppDomain.CurrentDomain.BaseDirectory}log.txt");
        Log.Write("Program started.");
        
        while (!Raylib.WindowShouldClose())
        {
            // --- Mouse settings ---
            // Calls on class for mouse interact (always on top)
            MouseInput.Update();
            
            // --- UI ---
            // Tracks current screen and runs it.
            switch (AppState.CurrentScreen)
            {
                case Screen.Start:
                    StartScreen.Run();
                    break;
                case Screen.Register:
                    RegisterScreen.Run();
                    break;
                case Screen.Chat:
                    ChatScreen.Run();
                    break;
                case Screen.Options:
                    OptionsWindow.Run();
                    break;
            }

            Raylib.DrawText($"{appVersion}", 10, 580, 10, Colors.TextColor);
        }

        Raylib.CloseWindow();
        Log.Write("Program closed.");
    }
}