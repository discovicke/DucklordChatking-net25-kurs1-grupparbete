using ChatClient.Core;
using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.Core.Input;
using ChatClient.Data;
using ChatClient.Data.Services;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Components.Text;
using ChatClient.UI.Screens.Common;
using Raylib_cs;
using System.Security.AccessControl;

namespace ChatClient.UI.Screens.Register;

/// <summary>
/// Responsible for: handling user registration logic including validation and server communication.
/// Manages password confirmation matching, field validation, and feedback display for registration success/failure.
/// </summary>
public class RegisterScreenLogic(
    TextField userField,
    TextField passField,
    TextField passConfirmField,
    Button registerButton,
    BackButton backButton
) : IScreenLogic
{
    private readonly UserAuth userAuth = new UserAuth(ServerConfig.CreateHttpClient());
    public readonly FeedbackBox FeedbackBox = new();
    private readonly TabLogics tabs = new();
    private bool tabsInitialized;

    public void HandleInput()
    {
        tabs.Update();

        // Register fields once in desired tab order (username -> password)
        if (!tabsInitialized)
        {
            tabs.Register(userField);
            tabs.Register(passField);
            tabs.Register(passConfirmField);
            tabsInitialized = true;
        }
        FeedbackBox.Update();
        
        userField.Update();
        passField.Update();
        passConfirmField.Update();

        if (MouseInput.IsLeftClick(registerButton.Rect) || Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            TryRegister();
        }

        backButton.Update();
        if (backButton.IsClicked())
        {
            Clear();
            AppState.GoBack();
        }
    }

    private void TryRegister()
    {
        string username = userField.Text.Trim();
        string password = passField.Text;
        string passwordConfirm = passConfirmField.Text;

        if (string.IsNullOrWhiteSpace(username))
        {
            FeedbackBox.Show("Quackername cannot be empty!", false);
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            FeedbackBox.Show("Password cannot be empty!", false);
            return;
        }

        if (password != passwordConfirm)
        {
            FeedbackBox.Show("Passwords do not match!", false);
            return;
        }

        if (password.Length < 8)
        {
            FeedbackBox.Show("Password must be at least 8 characters!", false);
            return;
        }
        
        if (password.Any(char.IsWhiteSpace))
        {
            FeedbackBox.Show("Password can not contain blank spaces!", false);
            return;
        }

        bool success = userAuth.Register(username, password);

        if (success)
        {
            FeedbackBox.Show($"Duckount created! Welcome, {username}!", true);
            
            Task.Delay(3000).ContinueWith(_ =>
            {
                Clear();
                AppState.CurrentScreen = Screen.Start;
            });
        }
        else
        {
            FeedbackBox.Show("Registration failed! Quackername may be taken.", false);
        }
    }

    private void Clear()
    {
        userField.Clear();
        passField.Clear();
        passConfirmField.Clear();
    }
}
