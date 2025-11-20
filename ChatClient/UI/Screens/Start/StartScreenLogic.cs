using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.Core.Input;
using ChatClient.Data;
using ChatClient.Data.Services;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Screens.Common;
using Raylib_cs;

namespace ChatClient.UI.Screens.Start
{
    /// <summary>
    /// Responsible for: handling user input and navigation logic for the start/login screen.
    /// Manages login button clicks, field updates, tab navigation, and transitions to register/options/chat screens.
    /// </summary>
    public class StartScreenLogic : ScreenLogicBase
    {
        // DEV MODE: Set to false before production release
        private const bool DEV_MODE_ENABLED = true;
        
        private TextField UserField { get; set; }
        private TextField PasswordField { get; set; }
        private Button LoginButton { get; set; }
        private Button RegisterButton { get; set; }
        private IFeedbackService Feedback { get; set; }
        private UserAuth UserAuth { get; set; }
        
        private Action PostLoginAction { get; set; }

        public FeedbackBox FeedbackBox { get; }

        public StartScreenLogic(
            TextField userField,
            TextField passwordField,
            Button loginButton,
            Button registerButton)
        {
            UserField = userField;
            PasswordField = passwordField;
            LoginButton = loginButton;
            RegisterButton = registerButton;

            FeedbackBox = new FeedbackBox();
            Feedback = new FeedbackService(FeedbackBox);
            UserAuth = new UserAuth(ServerConfig.CreateHttpClient());

            PostLoginAction = () =>
            {
                Navigation.NavigateTo(Screen.Chat);
                ClearFields();
            };

            RegisterField(UserField);
            RegisterField(PasswordField);
        }

        protected override void UpdateComponents()
        {
            base.UpdateComponents(); // Updates all registered fields with tab navigation
            Feedback.Update();
        }

        protected override void HandleActions()
        {
            // DEV MODE: Ctrl+Shift+D for instant dev login
            if (DEV_MODE_ENABLED &&
                Raylib.IsKeyDown(KeyboardKey.LeftControl) &&
                Raylib.IsKeyDown(KeyboardKey.LeftShift) &&
                Raylib.IsKeyPressed(KeyboardKey.D))
            {
                DevLogin();
                return;
            }

            if (MouseInput.IsLeftClick(LoginButton.Rect) || Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                Login();
            }

            if (MouseInput.IsLeftClick(RegisterButton.Rect))
            {
                NavigateToRegister();
            }
        }

        private void Login()
        {
            string username = UserField.Text.Trim();
            string password = PasswordField.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Raylib.PlaySound(ResourceLoader.FailedSound);
                Feedback.ShowError("Please enter quackername and password!");
                Log.Info("[StartScreenLogic] Login failed - Username or password empty");
                return;
            }

            Log.Info($"[StartScreenLogic] Login attempt - Username: '{username}'");

            bool success = UserAuth.Login(username, password);

            if (success)
            {
                Raylib.PlaySound(ResourceLoader.LoginSound);
                Log.Success($"[StartScreenLogic] Login successful for user '{username}'");
                AppState.LoggedInUsername = username;
                Feedback.ShowSuccess($"Welcome duck, {username}!");

                Task.Delay(1000).ContinueWith(_ =>
                {
                    Navigation.NavigateTo(Screen.Chat);
                    ClearFields();
                });
            }
            else
            {
                Raylib.PlaySound(ResourceLoader.FailedSound);
                Log.Error($"[StartScreenLogic] Login failed for user '{username}' - Invalid credentials");
                Feedback.ShowError("DUCK! Login failed, check your credentials.");
            }
        }

        private void DevLogin()
        {
            Log.Info("[StartScreenLogic] DEV MODE: Quack login activated (Ctrl+Shift+D)");
    
            bool success = UserAuth.Login("Ducklord", "chatking");
    
            if (success)
            {
                Raylib.PlaySound(ResourceLoader.LoginSound);
                Log.Success("[StartScreenLogic] DEV MODE: Login successful for Ducklord");
                AppState.LoggedInUsername = "Ducklord";
                Feedback.ShowSuccess("DEV MODE: Welcome back, Ducklord!");
        
                Task.Delay(1000).ContinueWith(_ =>
                {
                    Navigation.NavigateTo(Screen.Chat);
                    ClearFields();
                });
            }
            else
            {
                Raylib.PlaySound(ResourceLoader.FailedSound);
                Log.Error("[StartScreenLogic] DEV MODE: Login failed for Ducklord - Invalid credentials");
                Feedback.ShowError("DEV MODE: Login failed!");
            }
        }

        private void NavigateToRegister()
        {
            Log.Info("[StartScreenLogic] Navigating to register screen");
            Navigation.NavigateTo(Screen.Register);
            ClearFields();
        }
    }
}
