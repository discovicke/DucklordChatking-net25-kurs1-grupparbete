using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class RegisterScreenLogic(
    TextField idField,
    TextField userField,
    TextField passField,
    TextField passConfirmField,
    Button registerButton,
    BackButton backButton
) : IScreenLogic
{
    public void HandleInput()
    {
        idField.Update();
        userField.Update();
        passField.Update();
        passConfirmField.Update();

        if (MouseInput.IsLeftClick(registerButton.Rect) || Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            Log.Info($"[RegisterScreenLogic] Registration attempt - ID: '{idField.Text}', Username: '{userField.Text}'");
            // TODO: Validation & persistence
            Clear();
            AppState.CurrentScreen = Screen.Start;
        }

        backButton.Update();
        if (backButton.IsClicked())
        {
            Clear();
            AppState.GoBack();
        }
    }

    private void Clear()
    {
        Log.Info("[RegisterScreenLogic] Clearing all fields");
        idField.Clear();
        userField.Clear();
        passField.Clear();
        passConfirmField.Clear();
    }
}