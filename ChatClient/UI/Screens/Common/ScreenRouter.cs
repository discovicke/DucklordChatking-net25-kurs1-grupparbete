using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Screens.Chat;
using ChatClient.UI.Screens.Options;
using ChatClient.UI.Screens.Register;
using ChatClient.UI.Screens.Start;

namespace ChatClient.UI.Screens.Common;

/// <summary>
/// Responsible for: routing between different screens and applying window size changes based on current screen.
/// Manages screen instances and delegates rendering to the active screen.
/// </summary>
public static class ScreenRouter
{
    private static readonly StartScreen start = new StartScreen();
    private static readonly RegisterScreen register = new RegisterScreen();
    private static readonly OptionsScreen options = new OptionsScreen();
    private static readonly ChatScreen chat = new ChatScreen();
    
    private static Screen? lastScreen = null;

    public static void RunCurrent()
    {
        // Apply window settings when screen changes
        if (lastScreen != AppState.CurrentScreen)
        {
            Log.Info($"[ScreenRouter] Screen changed from {lastScreen?.ToString() ?? "None"} to {AppState.CurrentScreen}");
            WindowSettings.ApplyCurrentMode(AppState.CurrentScreen);
            lastScreen = AppState.CurrentScreen;
        }
        
        switch (AppState.CurrentScreen)
        {
            case Screen.Start:    start.Run();    break;
            case Screen.Register: register.Run(); break;
            case Screen.Options:  options.Run();  break;
            case Screen.Chat:     chat.Run();     break;
        }
    }
}