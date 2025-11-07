using Raylib_cs;

namespace ChatClient;

internal static class Program
{
    
    public static void Main()
    {
        Raylib.InitWindow(800, 480, "DiscLord 1.0.0");
        
        while (!Raylib.WindowShouldClose())
        {
            MainMenu.Run();
        }

        Raylib.CloseWindow();
    }
}