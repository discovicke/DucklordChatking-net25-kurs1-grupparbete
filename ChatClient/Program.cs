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
            Raylib.ClearBackground(Color.Black);

            Raylib.DrawText("Hej Mackan!", 12, 12, 20, Color.Black);
            // Textruta test
            Raylib.DrawRectangle(200, 200, 150, 120, Color.DarkGray);
            Raylib.DrawText("Logga in", 230, 250, 20, Color.White);
            
            Raylib.DrawRectangle(200, 70, 150, 120, Color.DarkGray);
            Raylib.DrawText("Creat User", 220, 125, 20, Color.White);

            


            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}