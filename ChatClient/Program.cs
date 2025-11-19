using ChatClient.Core.Infrastructure;
using ChatClient.Core.Input;
using ChatClient.UI.Screens.Common;
using ChatClient.UI.Theme;
using Raylib_cs;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace ChatClient;

public class Program
{
    // TODO:
    //      - Add relative spacing to window size
    //      - Add chat bubble for own user message
    //      - Add feedback box / message when other users change status

    public static void Main()
    {
        string appVersion = "DuckLord v.0.5.1";
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);


        Raylib.InitWindow(500, 500, appVersion);
        Raylib.SetTargetFPS(60);

        // For ducksound
        Raylib.InitAudioDevice();

        // Load all resources (fonts, textures, etc.)
        ResourceLoader.LoadAll();

        Console.WriteLine($"Log path: {AppDomain.CurrentDomain.BaseDirectory}log.txt");
        Log.Write("Program started.");

        Raylib.SetWindowSize(500, 500);
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
            const int fontSize = 10; // Constant font size - does not scale with window
            const int padding = 10;

            Raylib.DrawTextEx(
                ResourceLoader.RegularFont,
                $"{appVersion}",
                new Vector2(padding, screenHeight - fontSize - padding),
                fontSize,
                0.5f,
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
