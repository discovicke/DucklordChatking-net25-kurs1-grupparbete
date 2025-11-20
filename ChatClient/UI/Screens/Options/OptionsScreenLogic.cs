﻿using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Screens.Common;
using Raylib_cs;

namespace ChatClient.UI.Screens.Options;

/// <summary>
/// Responsible for: calculating layout positions for all UI elements on the options/settings screen.
/// Determines field sizes for account updates and window mode toggle buttons (windowed/fullscreen).
/// </summary>
// TODO Save settings
public class OptionsScreenLogic : ScreenLogicBase
{
    private readonly TextField userField;
    private readonly TextField passField;
    private readonly TextField passConfirmField;
    private readonly Button confirmButton;
    private readonly BackButton backButton;
    private readonly ToggleBox toggleWindowed;
    private readonly ToggleBox toggleFullscreen;
    private readonly ToggleBox toggleMute;

    public OptionsScreenLogic(
        TextField userField,
        TextField passField,
        TextField passConfirmField,
        Button confirmButton,
        BackButton backButton,
        ToggleBox toggleWindowed,
        ToggleBox toggleFullscreen,
        ToggleBox toggleMute)
    {
        this.userField = userField;
        this.passField = passField;
        this.passConfirmField = passConfirmField;
        this.confirmButton = confirmButton;
        this.backButton = backButton;
        this.toggleWindowed = toggleWindowed;
        this.toggleFullscreen = toggleFullscreen;
        this.toggleMute = toggleMute;

        // Register fields for automatic tab navigation
        RegisterField(userField);
        RegisterField(passField);
        RegisterField(passConfirmField);
    }

    protected override void UpdateComponents()
    {
        base.UpdateComponents(); // Updates all registered fields with tab navigation
        
        // Use WindowSettings to handle window mode toggles
        WindowSettings.UpdateToggles(toggleWindowed, toggleFullscreen);
        
        toggleMute.Update();
        confirmButton.Update();
        backButton.Update();
        
        HandleMuteToggle();
    }

    protected override void HandleActions()
    {
        if (confirmButton.IsClicked())
        {
            SaveSettings();
        }

        if (backButton.IsClicked())
        {
            Cancel();
        }
    }

    private void HandleMuteToggle()
    {
        if (toggleMute.IsChecked)
        {
            Raylib.SetMasterVolume(0.0f);
        }
        else
        {
            Raylib.SetMasterVolume(1.0f);
        }
    }

    private void SaveSettings()
    {
        Log.Info($"[OptionsScreenLogic] Settings confirmed - New username: '{userField.Text}'");
        ClearFields();
        Navigation.NavigateBack();
    }

    private void Cancel()
    {
        Log.Info("[OptionsScreenLogic] Cancelling settings changes");
        ClearFields();
        Navigation.NavigateBack();
    }
}
