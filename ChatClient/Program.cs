using ChatClient.Configurations;
using ChatClient.Data;
using ChatClient.Windows;
using Raylib_cs;

namespace ChatClient;

public class Program
{
    
    public static void Main()
    {
        Raylib.InitWindow(800, 600, "DuckLord 1.0.0");
        
        while (!Raylib.WindowShouldClose())
        {
            //MainMenu.Run();
            // calls on class for mouse interact
            MouseInput.Update();
            var mousePos = MouseInput.Position;
            bool mousePressed = Raylib.IsMouseButtonPressed(MouseButton.Left);
            //MainMenu.Run();
            // UI
            //UIkladd.Run();
            ChatScreen.Run();


        }
        
        Raylib.CloseWindow();
    }
}