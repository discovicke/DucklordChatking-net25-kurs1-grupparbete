using ChatClient.Core;
using ChatClient.Data;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

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

    public void HandleInput()
    {
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
