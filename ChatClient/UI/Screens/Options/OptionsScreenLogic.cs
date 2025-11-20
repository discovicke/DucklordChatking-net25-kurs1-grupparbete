using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Components.Text;
using ChatClient.UI.Screens.Common;
using Raylib_cs;

namespace ChatClient.UI.Screens.Options;
/// <summary>
/// Responsible for: calculating layout positions for all UI elements on the options/settings screen.
/// Determines field sizes for account updates and window mode toggle buttons (windowed/fullscreen).
/// </summary>
// TODO Save settings
public class OptionsScreenLogic(
    TextField userField,
    TextField passField,
    TextField passConfirmField,
    Button confirmButton,
    BackButton backButton,
    ToggleBox toggleWindowed,
    ToggleBox toggleFullscreen,
    ToggleBox toggleMute
) : IScreenLogic
{
    private readonly TabLogics tabs = new();
    private bool tabsIniti;
    private bool togglesIniti;

    public void HandleInput()
    {
        tabs.Update();

        if (!tabsIniti)
        {
            tabs.Register(userField);
            tabs.Register(passField);
            tabs.Register(passConfirmField);
            tabsIniti = true;
        }

        // Initialize toggle state from current window mode once
        if (!togglesIniti)
        {
            if (WindowSettings.CurrentMode == WindowMode.Windowed)
            {
                toggleWindowed.SetChecked(true);
                toggleFullscreen.SetChecked(false);
            }
            else
            {
                toggleWindowed.SetChecked(false);
                toggleFullscreen.SetChecked(true);
            }
            togglesIniti = true;
        }

        userField.Update();
        passField.Update();
        passConfirmField.Update();

        toggleWindowed.Update();
        toggleFullscreen.Update();
        toggleMute.Update();

        // Enforce mutual exclusion and reflect active mode immediately
        if (toggleWindowed.IsClicked())
        {
            // Always force state: cannot uncheck without selecting the other
            toggleWindowed.SetChecked(true);
            toggleFullscreen.SetChecked(false);

            if (WindowSettings.CurrentMode != WindowMode.Windowed)
            {
                WindowSettings.SetMode(WindowMode.Windowed);
                Log.Info("[OptionsScreenLogic] Windowed mode selected");
            }
        }

        if (toggleFullscreen.IsClicked())
        {
            toggleFullscreen.SetChecked(true);
            toggleWindowed.SetChecked(false);

            if (WindowSettings.CurrentMode != WindowMode.Fullscreen)
            {
                WindowSettings.SetMode(WindowMode.Fullscreen);
                Log.Info("[OptionsScreenLogic] Fullscreen mode selected");
            }
        }

        // Mute application
        if (toggleMute.IsChecked)
        {
            Raylib.SetMasterVolume(0.0f);
        }
        else
        {
            Raylib.SetMasterVolume(1.0f);
        }

            



            confirmButton.Update();
            if (confirmButton.IsClicked())
            {
                Log.Info($"[OptionsScreenLogic] Settings confirmed - New username: '{userField.Text}'");
                Clear();
                AppState.GoBack();
            }

            backButton.Update();
            if (backButton.IsClicked())
            {
                Clear();
                AppState.GoBack();
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
