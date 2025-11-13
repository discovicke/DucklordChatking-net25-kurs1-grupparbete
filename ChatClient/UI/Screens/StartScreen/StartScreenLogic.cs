using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens
{
    // "How should all fields and buttons behave?"
    public class StartScreenLogic(
        TextField userField,
        TextField passwordField,
        Button loginButton,
        Button registerButton,
        OptionsButton optionsButton) : IScreenLogic
    {
        public void HandleInput()
        {
            userField.Update();
            passwordField.Update();

            if (MouseInput.IsLeftClick(loginButton.Rect) || Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                Login();
            }

            if (MouseInput.IsLeftClick(registerButton.Rect))
            {
                NavigateToRegister();
            }

            if (MouseInput.IsLeftClick(optionsButton.Rect))
            {
                NavigateToOptions();
            }
        }

        private void Login()
        {
            Log.Info($"[StartScreenLogic] Login attempt - Username: '{userField.Text}'");
            AppState.CurrentScreen = Screen.Chat;
            Log.Info("[StartScreenLogic] User logged in, switching to chat screen");
            ClearFields();
        }

        private void NavigateToRegister()
        {
            Log.Info("[StartScreenLogic] Navigating to register screen");
            AppState.CurrentScreen = Screen.Register;
            ClearFields();
        }

        private void NavigateToOptions()
        {
            Log.Info("[StartScreenLogic] Navigating to options screen");
            AppState.CurrentScreen = Screen.Options;
            ClearFields();
        }

        private void ClearFields()
        {
            Log.Info("[StartScreenLogic] Clearing all fields");
            userField.Clear();
            passwordField.Clear();
        }
    }
}
