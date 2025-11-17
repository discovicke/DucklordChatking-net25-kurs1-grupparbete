using Raylib_cs;

namespace ChatClient.Core;

public enum WindowMode
{
    Windowed,
    Fullscreen
}

public static class WindowSettings
{
    public static WindowMode CurrentMode { get; private set; } = WindowMode.Windowed;
    
    public static void SetMode(WindowMode mode)
    {
        CurrentMode = mode;
        ApplyCurrentMode(AppState.CurrentScreen);
    }
    
    public static void ApplyCurrentMode(Screen screen)
    {
        if (CurrentMode == WindowMode.Fullscreen)
        {
            Raylib.SetWindowState(ConfigFlags.FullscreenMode);
        }
        else
        {
            Raylib.ClearWindowState(ConfigFlags.FullscreenMode);
            
            // Apply windowed size based on screen
            switch (screen)
            {
                case Screen.Start:
                case Screen.Register:
                    Raylib.SetWindowSize(576, 1024);
                    break;
                    
                case Screen.Chat:
                case Screen.Options:
                    Raylib.SetWindowSize(1200, 720);
                    break;
            }
        }
        
        Log.Info($"[WindowSettings] Applied {CurrentMode} mode for {screen} screen");
    }
}

