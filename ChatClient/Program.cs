using Raylib_cs;

namespace ChatClient;

internal static class Program
{
    
    [System.STAThread]
    public static void Main()
    {
        Raylib.InitWindow(800, 480, "DiscLord 1.0.0");

        int x = 200;
        int y = 200;
        int width = 150;
        int height = 80;

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            Raylib.DrawText("Hej Mackan!", 12, 12, 20, Color.Black);
            // Textruta test
            Raylib.DrawRectangle(x, y, width, height, Color.DarkGray);
            Raylib.DrawText("Logga in", x + 20, y + 25, 20, Color.White);


            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}