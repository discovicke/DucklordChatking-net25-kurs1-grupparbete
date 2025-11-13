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
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor, true, false, "ChatScreen_MessageInput");
    private readonly Button sendButton = new(new Rectangle(), "Send", 
        Colors.TextFieldColor, Colors.HoverColor, Colors.TextColor);
    private readonly BackButton backButton = new(new Rectangle(10, 10, 100, 30));

    private readonly MessageHandler? messageSender = new
        (new HttpClient { BaseAddress = new System.Uri("https://ducklord-server.onrender.com/") });
    private List<MessageDTO> messages = new();
    private double lastUpdateTime = 0;

    public ChatScreen()
    {
        logic = new ChatScreenLogic(inputField, sendButton, backButton, SendMessage);
    }

    protected override ChatScreenLayout.LayoutData CalculateLayout() 
        => ChatScreenLayout.Calculate(ResourceLoader.LogoTexture.Width);

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

        // Chat window
        Raylib.DrawRectangleRounded(layout.ChatRect, 
            0.1f, 10, Colors.HoverColor);

        // Pull history ~1/s
        double t = Raylib.GetTime();
        if (t - lastUpdateTime >= 1.0)
        {
            lastUpdateTime = t;
            var list = messageSender?.ReceiveHistory();
            if (list != null && list.Any()) messages = list.ToList();
        }

        // TODO: Add scrollbar
        // TODO: Add Message Wrapping
        // Draw messages
        int startX = (int)layout.ChatRect.X + 10;
        int startY = (int)layout.ChatRect.Y + 10;
        int lineH = 20;
        foreach (var m in messages)
        {
            string sender = string.IsNullOrWhiteSpace(m.Sender) ? "Unknown" : m.Sender;
            string text = $"{m.Timestamp}  -  {sender} :  {m.Content}";
            Raylib.DrawText(text, startX, startY, 15, Colors.TextColor);
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
        if (messageSender == null) return;
        bool ok = messageSender.SendMessage(text);
        var list = messageSender.ReceiveHistory();
        if (ok && list != null && list.Any())
        {
            messages = list.ToList();
        }
    }
}
