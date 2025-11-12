using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens
{
    public class StartScreenLogic(
        TextField userField,
        TextField passwordField,
        Button loginButton,
        Button registerButton,
        OptionsButton optionsButton)
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
            // TODO: Add authentication logic here
            // if (string.IsNullOrWhiteSpace(userField.Text)) return;
            
            AppState.CurrentScreen = Screen.Chat;
            Log.Info("User logged in, switching to chat screen");
            ClearFields();
        }

        private void NavigateToRegister()
        {
            AppState.CurrentScreen = Screen.Register;
            Log.Info("User wants to register, switching to register screen");
            ClearFields();
        }

        private void NavigateToOptions()
        {
            AppState.CurrentScreen = Screen.Options;
            Log.Info("User pressed options / Ducktions screen");
            ClearFields();
        }

        private void ClearFields()
        {
            userField.Clear();
            passwordField.Clear();
        }
    }
}
