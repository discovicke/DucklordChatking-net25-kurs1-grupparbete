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
        
        private readonly TextField userField;
        private readonly TextField passwordField;
        private readonly Button loginButton;
        private readonly Button registerButton;
        private readonly IFeedbackService feedback;
        private readonly UserAuth userAuth;

        public FeedbackBox FeedbackBox { get; }

        public StartScreenLogic(
            TextField userField,
            TextField passwordField,
            Button loginButton,
            Button registerButton)
        {
            this.userField = userField;
            this.passwordField = passwordField;
            this.loginButton = loginButton;
            this.registerButton = registerButton;
            
            this.FeedbackBox = new FeedbackBox();
            this.feedback = new FeedbackService(FeedbackBox);
            this.userAuth = new UserAuth(ServerConfig.CreateHttpClient());

            // Register fields for automatic tab navigation
            RegisterField(userField);
            RegisterField(passwordField);
        }

        protected override void UpdateComponents()
        {
            base.UpdateComponents(); // Updates all registered fields with tab navigation
            feedback.Update();
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

            if (MouseInput.IsLeftClick(loginButton.Rect) || Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                Login();
            }

            if (MouseInput.IsLeftClick(registerButton.Rect))
            {
                NavigateToRegister();
            }
        }

        private void Login()
        {
            string username = userField.Text.Trim();
            string password = passwordField.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Raylib.PlaySound(ResourceLoader.FailedSound);
                feedback.ShowError("Please enter quackername and password!");
                Log.Info("[StartScreenLogic] Login failed - Username or password empty");
                return;
            }

            Log.Info($"[StartScreenLogic] Login attempt - Username: '{username}'");

            bool success = userAuth.Login(username, password);

            if (success)
            {
                Raylib.PlaySound(ResourceLoader.LoginSound);
                Log.Success($"[StartScreenLogic] Login successful for user '{username}'");
                AppState.LoggedInUsername = username;
                feedback.ShowSuccess($"Welcome duck, {username}!");

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
                feedback.ShowError("DUCK! Login failed, check your credentials.");
            }
        }

        private void DevLogin()
        {
            Log.Info("[StartScreenLogic] DEV MODE: Quack login activated (Ctrl+Shift+D)");
    
            bool success = userAuth.Login("Ducklord", "chatking");
    
            if (success)
            {
                Raylib.PlaySound(ResourceLoader.LoginSound);
                Log.Success("[StartScreenLogic] DEV MODE: Login successful for Ducklord");
                AppState.LoggedInUsername = "Ducklord";
                feedback.ShowSuccess("DEV MODE: Welcome back, Ducklord!");
        
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
                feedback.ShowError("DEV MODE: Login failed!");
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
