using ChatClient.Core.Infrastructure;
using ChatClient.UI.Components.Base;
using Raylib_cs;

namespace ChatClient.UI.Screens.Chat.Components;

/// <summary>
/// Responsible for: rendering and handling input for the message input field + send button.
/// Raises SendPressed event when user submits a message.
/// </summary>
public class ChatToolbar(TextField inputField, Button sendButton)
{
    public event Action<string>? SendPressed;

    public void SetBounds(Rectangle inputRect, Rectangle sendRect)
    {
        inputField.SetRect(inputRect);
        sendButton.SetRect(sendRect);
    }

    public void Update()
    {
        inputField.Update();

        // Send on button click or Enter (without Shift for multiline)
        bool sendTriggered = sendButton.IsClicked() ||
                             (Raylib.IsKeyPressed(KeyboardKey.Enter) &&
                              !Raylib.IsKeyDown(KeyboardKey.LeftShift));

        if (sendTriggered)
        {
            Raylib.PlaySound(ResourceLoader.ButtonSound);
            string text = inputField.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                Log.Info($"[ChatToolbar] Message submitted: '{text.Replace("\n", "\\n")}'");
                SendPressed?.Invoke(text);
                inputField.Clear();
            }
            else
            {
                Log.Info("[ChatToolbar] Send attempted with empty message");
            }
        }
    }

    public void Draw()
    {
        inputField.Draw();
        sendButton.Draw();
    }
    public void ClearInput()
    {
        inputField.Clear();
    }
}

