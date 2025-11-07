using Raylib_cs;

namespace ChatClient;

public class Program
{
    
    public static void Main()
    {
        Raylib.InitWindow(800, 480, "DuckLord 1.0.0");
        
        while (!Raylib.WindowShouldClose())
        {
            //MainMenu.Run();
            // UI
            UIkladd.Run();
        }

        Raylib.CloseWindow();
    }
}