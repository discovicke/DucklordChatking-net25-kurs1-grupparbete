using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.Core.Input;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Screens.Common;

namespace ChatClient.UI.Screens.Chat;

/// <summary>
/// Simplified logic: only handles back button navigation.
/// Message input/send is now handled by ChatToolbar.
/// </summary>
public class ChatScreenLogic(ChatScreen screen, BackButton backButton, OptionsButton optionsButton) : IScreenLogic
{
    public void HandleInput()
    {
        backButton.Update();
        if (backButton.IsClicked())
        {
            Log.Info("[ChatScreenLogic] Navigating back to start screen");
            screen.StopPolling();
            AppState.CurrentScreen = Screen.Start;
        }
        if (MouseInput.IsLeftClick(optionsButton.Rect))
        {
            NavigateToOptions();
        }
    }
    private void NavigateToOptions()
    {
        Log.Info("[StartScreenLogic] Navigating to options screen");
        AppState.CurrentScreen = Screen.Options;
    }


}
