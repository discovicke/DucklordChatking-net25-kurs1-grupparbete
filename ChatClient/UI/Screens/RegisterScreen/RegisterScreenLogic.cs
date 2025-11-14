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
    
    // Feedback state
    public string FeedbackMessage { get; private set; } = "";
    public bool IsFeedbackSuccess { get; private set; } = false;
    private double feedbackStartTime = 0;
    private const double FeedbackDisplayDuration = 3.0; // Show feedback for 3 seconds

    public void HandleInput()
    {
        // Clear feedback after duration
        if (!string.IsNullOrEmpty(FeedbackMessage) && Raylib.GetTime() - feedbackStartTime > FeedbackDisplayDuration)
        {
            FeedbackMessage = "";
        }

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
            FeedbackMessage = ""; // Clear feedback on navigation
            AppState.GoBack();
        }
    }

    private void TryRegister()
    {
        string username = userField.Text.Trim();
        string password = passField.Text;
        string passwordConfirm = passConfirmField.Text;

        // Validation
        if (string.IsNullOrWhiteSpace(username))
        {
            ShowFeedback("Username cannot be empty!", false);
            Log.Info("[RegisterScreenLogic] Registration failed - Username empty");
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowFeedback("Password cannot be empty!", false);
            Log.Info("[RegisterScreenLogic] Registration failed - Password empty");
            return;
        }

        if (password != passwordConfirm)
        {
            ShowFeedback("Passwords do not match!", false);
            Log.Info("[RegisterScreenLogic] Registration failed - Passwords don't match");
            return;
        }

        if (password.Length < 8)
        {
            ShowFeedback("Password must be at least 3 characters!", false);
            Log.Info("[RegisterScreenLogic] Registration failed - Password too short");
            return;
        }
        
        if (password.Any(char.IsWhiteSpace))
        {
            ShowFeedback("Password can not contain blank spaces!", false);
            Log.Info("[RegisterScreenLogic] Registration failed - Password contains blank spaces");
            return;
        }

        Log.Info($"[RegisterScreenLogic] Registration attempt - Username: '{username}'");

        // Try to register with server
        bool success = userAuth.Register(username, password);

        if (success)
        {
            Log.Success($"[RegisterScreenLogic] Registration successful for user '{username}'");
            ShowFeedback($"Duckount created! Welcome, {username}!", true);
            
            // Navigate to start screen after a short delay
            Task.Delay(3000).ContinueWith(_ =>
            {
                Clear();
                FeedbackMessage = "";
                AppState.CurrentScreen = Screen.Start;
            });
        }
        else
        {
            Log.Error($"[RegisterScreenLogic] Registration failed for user '{username}' - Server error or username taken");
            ShowFeedback("✗ Registration failed! Username may be taken.", false);
        }
    }

    private void ShowFeedback(string message, bool isSuccess)
    {
        FeedbackMessage = message;
        IsFeedbackSuccess = isSuccess;
        feedbackStartTime = Raylib.GetTime();
    }

    private void Clear()
    {
        Log.Info("[RegisterScreenLogic] Clearing all fields");
        userField.Clear();
        passField.Clear();
        passConfirmField.Clear();
    }
}