using System;
using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class ChatScreenLogic(
    TextField inputField,
    Button sendButton,
    BackButton backButton,
    Action<string> onSend
) : IScreenLogic
{
    public void HandleInput()
    {
        inputField.Update();

        backButton.Update();
        if (backButton.IsClicked())
        {
            inputField.Clear();
            AppState.CurrentScreen = Screen.Start;
            return;
        }

        bool pressedEnter = Raylib.IsKeyPressed(KeyboardKey.Enter) && !Raylib.IsKeyDown(KeyboardKey.LeftShift);
        if (sendButton.IsClicked() || pressedEnter)
        {
            var text = inputField.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                onSend(text);
                inputField.Clear();
            }
        }
    }
}