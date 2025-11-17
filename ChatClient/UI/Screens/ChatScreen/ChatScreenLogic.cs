using System;
using ChatClient.Core;
using ChatClient.UI.Components;
using Raylib_cs;

namespace ChatClient.UI.Screens;

public class ChatScreenLogic(ChatScreen screen,
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
            screen.StopPolling();
            AppState.CurrentScreen = Screen.Start;
            return;
        }

        bool pressedEnter = Raylib.IsKeyPressed(KeyboardKey.Enter) && !Raylib.IsKeyDown(KeyboardKey.LeftShift);
        if (sendButton.IsClicked() || pressedEnter)
        {
            // Ducksound when sending message by pressedEnter
            Raylib.PlaySound(ResourceLoader.ButtonSound);

            var text = inputField.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                Log.Info($"[ChatScreenLogic] Sending message: '{text.Replace("\n", "\\n")}'");
                onSend(text);
                inputField.Clear();
            }
            else
            {
                Log.Info("[ChatScreenLogic] Send attempted with empty message");
            }
        }
    }
}
