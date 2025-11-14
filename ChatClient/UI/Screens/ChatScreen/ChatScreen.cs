using System.Numerics;
using ChatClient.Core;
using ChatClient.Data;
using ChatClient.UI.Components;
using Raylib_cs;
using Shared;

namespace ChatClient.UI.Screens;

public class ChatScreen : ScreenBase<ChatScreenLayout.LayoutData>
{

    private readonly TextField inputField = new(new Rectangle(),
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor,
        true, false, "ChatScreen_MessageInput", "Quack a message... (Shift+Enter for new line)");
    private readonly Button sendButton = new(new Rectangle(), "Send",
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);
    private readonly BackButton backButton = new(new Rectangle(10, 10, 100, 30));

    private readonly MessageHandler messageHandler = new(ServerConfig.CreateHttpClient());
    private List<MessageDTO> messages = new();
    private List<ChatMessage> chatMessageBubbles = new();
    private double lastUpdateTime = 0;

    public ChatScreen()
    {
        logic = new ChatScreenLogic(inputField, sendButton, backButton, SendMessage);
    }


    protected override ChatScreenLayout.LayoutData CalculateLayout()
        => ChatScreenLayout.Calculate(ResourceLoader.LogoTexture.Width, ResourceLoader.LogoTexture.Height);

    protected override void ApplyLayout(ChatScreenLayout.LayoutData layout)
    {
        inputField.SetRect(layout.InputRect);
        sendButton.SetRect(layout.SendRect);
        backButton.SetRect(layout.BackRect);
    }

    public override void RenderContent()
    {
        // Logo
        Raylib.DrawTextureEx(ResourceLoader.LogoTexture,
            new Vector2(layout.LogoX, layout.LogoY),
            0f, layout.LogoScale, Color.White);

        // Chat window background
        Raylib.DrawRectangleRounded(layout.ChatRect, 0.08f, 10, Colors.TextFieldUnselected);
        Raylib.DrawRectangleRoundedLinesEx(layout.ChatRect, 0.08f, 10, 1, Colors.OutlineColor);

        // Pull history ~1/s
        double t = Raylib.GetTime();
        if (t - lastUpdateTime >= 1.0)
        {
            lastUpdateTime = t;
            var list = messageHandler?.ReceiveHistory();
            messageHandler.SendHeartbeat();
            if (list != null && list.Any())
            {
                messages = list.ToList();
            chatMessageBubbles = messages
                .Select(m => new ChatMessage(m, layout.ChatRect.Width - 20))
                .ToList();
            }
        }

        // TODO: Add scrollbar
        // TODO: Add Message Wrapping
        // Draw messages
        float startX = layout.ChatRect.X + 10;
        float currentY = layout.ChatRect.Y + 10;
        const float messagePadding = 8f;

        foreach (var chatMsg in chatMessageBubbles)
        {
            string sender = string.IsNullOrWhiteSpace(m.Sender) ? "Unknown Duck" : m.Sender;
            string text = $"{m.Timestamp}  -  {sender} :  {m.Content}";
            Raylib.DrawTextEx(ResourceLoader.RegularFont, text,
                new Vector2(startX, startY), 15, 0.5f, Colors.TextColor);
            startY += lineH;
        }

        // Input + send
        inputField.Draw();
        sendButton.Draw();

        // Back
        backButton.Draw();
    }

    private void SendMessage(string text)
    {
        if (messageHandler == null) return;

        // Use logged in username or default to "Anonymous Duck"
        string sender = !string.IsNullOrEmpty(AppState.LoggedInUsername)
            ? AppState.LoggedInUsername
            : "Anonymous Duck";

        Log.Info($"[ChatScreen] Sending message as '{sender}': {text}");

        bool ok = messageHandler.SendMessage(text);
        var list = messageHandler.ReceiveHistory();
        if (ok && list != null && list.Any())
        {
            messages = list.ToList();
        }
    }

    private void DrawUserList()
    {
        // Placeholder data
        var onlineUsers = new[] { "Ducklord", "QuackyMcQuack", "DaffyDev" };
        var offlineUsers = new[] { "SleepyDuck", "LazyFeathers" };

        float x = layout.UserListRect.X + 10;
        float y = layout.UserListRect.Y + 10;
        const float lineH = 22;
        const float fontSize = 14;

        // Online header
        Raylib.DrawTextEx(ResourceLoader.BoldFont, "ONLINE",
            new Vector2(x, y), fontSize, 0.5f, Colors.AccentColor);
        y += lineH;

        // Online users
        foreach (var user in onlineUsers)
        {
            Raylib.DrawCircle((int)x + 5, (int)y + 7, 4f, Colors.AccentColor);
            Raylib.DrawTextEx(ResourceLoader.RegularFont, user,
                new Vector2(x + 15, y), fontSize, 0.5f, Colors.UiText);
            y += lineH;
        }

        y += 10; // Extra spacing

        // Offline header
        Raylib.DrawTextEx(ResourceLoader.BoldFont, "OFFLINE",
            new Vector2(x, y), fontSize, 0.5f, Colors.SubtleText);
        y += lineH;

        // Offline users
        foreach (var user in offlineUsers)
        {
            Raylib.DrawCircle((int)x + 5, (int)y + 7, 4f, Colors.SubtleText);
            Raylib.DrawTextEx(ResourceLoader.RegularFont, user,
                new Vector2(x + 15, y), fontSize, 0.5f, Colors.SubtleText);
            y += lineH;
        }
    }
}
