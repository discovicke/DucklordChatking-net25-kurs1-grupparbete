using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.Core.Input;
using ChatClient.Data;
using ChatClient.Data.Services;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Screens.Common;
using Raylib_cs;

namespace ChatClient.UI.Screens.Register;

/// <summary>
/// Responsible for: handling user registration logic including validation and server communication.
/// Manages password confirmation matching, field validation, and feedback display for registration success/failure.
/// </summary>
public class RegisterScreenLogic : ScreenLogicBase
{
    private readonly TextField UserField;
    private readonly TextField PassField;
    private readonly TextField PassConfirmField;
    private readonly Button RegisterButton;
    private readonly BackButton BackButton;
    private readonly IFeedbackService Feedback;
    private readonly UserAuth UserAuth;

    public FeedbackBox FeedbackBox { get; }

    public RegisterScreenLogic(
        TextField userField,
        TextField passField,
        TextField passConfirmField,
        Button registerButton,
        BackButton backButton)
    {
        UserField = userField;
        PassField = passField;
        PassConfirmField = passConfirmField;
        RegisterButton = registerButton;
        BackButton = backButton;
        
        FeedbackBox = new FeedbackBox();
        Feedback = new FeedbackService(FeedbackBox);
        UserAuth = new UserAuth(ServerConfig.CreateHttpClient());

        // Register fields for automatic tab navigation
        RegisterField(userField);
        RegisterField(passField);
        RegisterField(passConfirmField);
    }

    protected override void UpdateComponents()
    {
        base.UpdateComponents(); // Updates all registered fields with tab navigation
        Feedback.Update();
    }

    protected override void HandleActions()
    {
        if (MouseInput.IsLeftClick(RegisterButton.Rect) || Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            TryRegister();
        }

        BackButton.Update();
        if (BackButton.IsClicked())
        {
            ClearFields();
            Navigation.NavigateBack();
        }
    }

    private void TryRegister()
    {
        string username = UserField.Text.Trim();
        string password = PassField.Text;
        string passwordConfirm = PassConfirmField.Text;

        // Validate username
        var usernameValidation = InputValidator.ValidateUsername(username);
        if (!usernameValidation.IsValid)
        {
            Raylib.PlaySound(ResourceLoader.FailedSound);
            Feedback.ShowError(usernameValidation.ErrorMessage);
            return;
        }

        // Validate password
        var passwordValidation = InputValidator.ValidatePassword(password);
        if (!passwordValidation.IsValid)
        {
            Raylib.PlaySound(ResourceLoader.FailedSound);
            Feedback.ShowError(passwordValidation.ErrorMessage);
            return;
        }

        // Validate password match
        var matchValidation = InputValidator.ValidatePasswordMatch(password, passwordConfirm);
        if (!matchValidation.IsValid)
        {
            Raylib.PlaySound(ResourceLoader.FailedSound);
            Feedback.ShowError(matchValidation.ErrorMessage);
            return;
        }

        bool success = UserAuth.Register(username, password);

        if (success)
        {
            Raylib.PlaySound(ResourceLoader.LoginSound);
            Feedback.ShowSuccess($"Duckount created! Welcome, {username}!");
            
            Task.Delay(3000).ContinueWith(_ =>
            {
                ClearFields();
                Navigation.NavigateTo(Screen.Start);
            });
        }
        else
        {
            Raylib.PlaySound(ResourceLoader.FailedSound);
            Feedback.ShowError("Registration failed! Quackername may be taken.");
        }
    }
}
