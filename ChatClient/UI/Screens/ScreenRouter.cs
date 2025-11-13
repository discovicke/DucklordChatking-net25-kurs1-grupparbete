using ChatClient.Core;

namespace ChatClient.UI.Screens;

public static class ScreenRouter
{
    private static readonly StartScreen start = new StartScreen();
    private static readonly RegisterScreen register = new RegisterScreen();
    private static readonly OptionsScreen options = new OptionsScreen();
    private static readonly ChatScreen chat = new ChatScreen();
    
    private static Screen? lastScreen = null;

    public static void RunCurrent()
    {
        // Logga skärmbyte om det skett en förändring
        if (lastScreen != AppState.CurrentScreen)
        {
            Log.Info($"[ScreenRouter] Screen changed from {lastScreen?.ToString() ?? "None"} to {AppState.CurrentScreen}");
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