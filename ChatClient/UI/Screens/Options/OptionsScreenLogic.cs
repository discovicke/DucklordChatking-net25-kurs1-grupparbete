﻿using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.Data.Services;
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
    private readonly FeedbackBox feedbackBox;
    private readonly IFeedbackService feedback;

    public FeedbackBox FeedbackBox => feedbackBox;


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

        RegisterField(userField);
        RegisterField(passField);
        RegisterField(passConfirmField);

        feedbackBox = new FeedbackBox();
        feedback = new FeedbackService(feedbackBox);
    }


    protected override void UpdateComponents()
    {
        base.UpdateComponents(); // Updates all registered fields with tab navigation

        // Use WindowSettings to handle window mode toggles
        WindowSettings.UpdateToggles(toggleWindowed, toggleFullscreen);

        toggleMute.Update();
        confirmButton.Update();
        backButton.Update();

        feedbackBox.Update();
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
        var oldUsername = AppState.LoggedInUsername;
        var newUsername = userField.Text;
        var newPassword = passField.Text;
        var confirmPassword = passConfirmField.Text;

        if (string.IsNullOrWhiteSpace(newUsername))
        {
            feedback.ShowError("Username cannot be empty!");
            return;
        }

        if (newPassword != confirmPassword)
        {
            feedback.ShowError("Passwords do not match!");
            return;
        }

        var auth = new UserAuth();
        var success = auth.UpdateCredentials(oldUsername, newUsername, newPassword);

        if (!success)
        {
            feedback.ShowError("Failed to update credentials!");
            return;
        }

        // Update client-side stored username
        AppState.LoggedInUsername = newUsername;

        feedback.ShowSuccess("Credentials updated!");
        Log.Info($"[OptionsScreenLogic] Settings confirmed - New username: '{userField.Text}'");
        ClearFields();
        Task.Delay(1000).ContinueWith(_ => Navigation.NavigateBack());
    }

    private void Cancel()
    {
        Log.Info("[OptionsScreenLogic] Cancelling settings changes");
        ClearFields();
        Navigation.NavigateBack();
    }
}
