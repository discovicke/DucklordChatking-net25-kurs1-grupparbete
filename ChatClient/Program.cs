using ChatClient.Configurations;
using ChatClient.Data;
using ChatClient.Windows;
using Raylib_cs;

namespace ChatClient;

public class Program
{
    
    public static void Main()
    {
        Raylib.InitWindow(800, 600, "DuckLord 0.0.1");
        Console.WriteLine($"Log path: {AppDomain.CurrentDomain.BaseDirectory}log.txt");
        Log.Write("Program started.");
        
        while (!Raylib.WindowShouldClose())
        {
            // --- Mouse settings ---
            // Calls on class for mouse interact (always on top)
            MouseInput.Update();
            var mousePos = MouseInput.Position;
            bool mousePressed = Raylib.IsMouseButtonPressed(MouseButton.Left);
            
            // --- UI ---
            // StartScreen.Run();
            // RegisterScreen.Run();
             ChatScreen.Run();
        }

        Raylib.CloseWindow();
        Log.Write("Program closed.");
    }
}