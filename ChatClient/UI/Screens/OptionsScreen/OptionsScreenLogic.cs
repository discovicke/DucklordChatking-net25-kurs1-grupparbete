using ChatClient.Core;
using ChatClient.UI.Components;

namespace ChatClient.UI.Screens;

public class OptionsScreenLogic(
    TextField userField,
    TextField passField,
    TextField passConfirmField,
    Button confirmButton,
    BackButton backButton
) : IScreenLogic
{
    public void HandleInput()
    {
        userField.Update();
        passField.Update();
        passConfirmField.Update();

        confirmButton.Update();
        if (confirmButton.IsClicked())
        {
            Log.Info($"[OptionsScreenLogic] Settings confirmed - New username: '{userField.Text}'");
            // TODO: Save settings
            Clear();
            AppState.GoBack();
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
        Log.Info("[OptionsScreenLogic] Clearing all fields");
        userField.Clear();
        passField.Clear();
        passConfirmField.Clear();
    }
}