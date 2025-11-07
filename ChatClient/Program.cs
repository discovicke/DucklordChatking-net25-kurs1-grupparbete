using Raylib_cs;

namespace ChatClient;

internal static class Program
{
    
    [System.STAThread]
    public static void Main()
    {
        Raylib.InitWindow(800, 480, "DiscLord 1.0.0");

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            Raylib.DrawText("Hej Mackan!", 12, 12, 20, Color.Black);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}