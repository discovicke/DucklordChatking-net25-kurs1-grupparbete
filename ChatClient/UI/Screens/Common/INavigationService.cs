using ChatClient.Core.Application;

namespace ChatClient.UI.Screens.Common;

/// <summary>
/// Handles navigation between screens
/// </summary>
public interface INavigationService
{
    void NavigateTo(Screen screen);
    void NavigateBack();
}

public class NavigationService : INavigationService
{
    public void NavigateTo(Screen screen)
    {
        AppState.CurrentScreen = screen;
    }

    public void NavigateBack()
    {
        AppState.GoBack();
    }
}

