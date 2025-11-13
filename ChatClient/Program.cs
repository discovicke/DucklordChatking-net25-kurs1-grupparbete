using ChatClient.Core;
using ChatClient.Data;
using ChatClient.UI;
using ChatClient.UI.Components;
using ChatClient.UI.Screens;
using Raylib_cs;
using System.Numerics;

namespace ChatClient;

public class Program
{
    // TODO: Add relative spacing to window size
    // TODO: What happens when connection to server is lost?
    // TODO: Tab & Ctrl + Tab for switching between fields
    public static void Main()
    {
        string appVersion = "2 girls, 1 duck v.0.1.1";
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);

        
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
            ScreenRouter.RunCurrent();

            Raylib.DrawTextEx(
                Fonts.regularFont,
                $"{appVersion}",
                new Vector2(10, 580),
                20,
                1,
                Colors.TextColor);
        }

        Raylib.CloseWindow();
        Log.Write("Program closed.");
    }
}