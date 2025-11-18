using ChatClient.Core;
using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Components.Text;
using ChatClient.UI.Screens.Common;

namespace ChatClient.UI.Screens.Options;

/// <summary>
/// Responsible for: handling settings changes including window mode toggles and account updates.
/// Manages windowed/fullscreen mode switching and saves user preferences.
/// </summary>
public class OptionsScreenLogic(
    TextField userField,
    TextField passField,
    TextField passConfirmField,
    Button confirmButton,
    BackButton backButton,
    Button btnWindowed,
    Button btnFullscreen
) : IScreenLogic
{
    private readonly TabLogics tabs = new();
    private bool tabsIniti;
    public void HandleInput()
    {
        tabs.Update();

        // Register fields once in desired tab order (username -> password)
        if (!tabsIniti)
        {
            tabs.Register(userField);
            tabs.Register(passField);
            tabs.Register(passConfirmField);
            tabsIniti = true;
        }

        userField.Update();
        passField.Update();
        passConfirmField.Update();

        confirmButton.Update();
        if (confirmButton.IsClicked())
        {
            Log.Info($"[OptionsScreenLogic] Settings confirmed - New username: '{userField.Text}'");
            // TODO: Save settings
            Clear();
            AppState.GoBack();
        }

        backButton.Update();
        if (backButton.IsClicked())
        {
            Clear();
            AppState.GoBack();
        }
        
        if (btnWindowed.IsClicked())
        {
            WindowSettings.SetMode(WindowMode.Windowed);
            Log.Info("[OptionsScreenLogic] Windowed mode selected");
        }
        
        if (btnFullscreen.IsClicked())
        {
            WindowSettings.SetMode(WindowMode.Fullscreen);
            Log.Info("[OptionsScreenLogic] Fullscreen mode selected");
        }
    }

    private void Clear()
    {
        Log.Info("[OptionsScreenLogic] Clearing all fields");
        userField.Clear();
        passField.Clear();
        passConfirmField.Clear();
    }
}