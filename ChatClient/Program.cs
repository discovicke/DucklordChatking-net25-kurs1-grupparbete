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
        
        // Load all resources (fonts, textures, etc.)
        ResourceLoader.LoadAll();
        
        Console.WriteLine($"Log path: {AppDomain.CurrentDomain.BaseDirectory}log.txt");
        Log.Write("Program started.");
        
        while (!Raylib.WindowShouldClose())
        {
            // --- Mouse settings ---
            // Calls on class for mouse interact (always on top)
            MouseInput.Update();
            
            // --- Begin Frame ---
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Colors.BackgroundColor);
            
            // --- UI ---
            // Tracks current screen and runs it.
            ScreenRouter.RunCurrent();

            // Version text in bottom-left corner (always on top, updates position on window resize)
            int screenHeight = Raylib.GetScreenHeight();
            const int fontSize = 13; // Constant font size - does not scale with window
            const int padding = 10;
            
            Raylib.DrawTextEx(
                ResourceLoader.RegularFont,
                $"{appVersion}",
                new Vector2(padding, screenHeight - fontSize - padding),
                fontSize,
                1,
                Colors.SubtleText);
            
            // --- End Frame ---
            Raylib.EndDrawing();
        }

        // Unload all resources before closing
        ResourceLoader.UnloadAll();
        Raylib.CloseWindow();
        Log.Write("Program closed.");
    }
}