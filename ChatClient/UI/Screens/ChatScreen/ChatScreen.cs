using System.Collections.Concurrent;
using System.Numerics;
using ChatClient.Core;
using ChatClient.Data;
using ChatClient.UI.Components;
using Raylib_cs;
using Shared;

namespace ChatClient.UI.Screens;

public class ChatScreen : ScreenBase<ChatScreenLayout.LayoutData>
{

    // TODO:- Last line spacing
    //      - Measure text height in NewLine multiline evironment
    //      - Add scroll to inputPanel if text too long
    //      - Improve user list with real data
    //      - Optimize message fetching (only new messages)
    //      - User bubble / received bubble difference
    //      - Visible scrollbar in chatwindow
    //      - Visible scrollbar IF input height exceeds chat height
    private readonly TextField inputField = new(new Rectangle(),
        Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor,
        true, false, "ChatScreen_MessageInput", "Quack a message... (Shift+Enter for new line)");
    private readonly Button sendButton = new(new Rectangle(), "Send",
        Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);
    private readonly BackButton backButton = new(new Rectangle(10, 10, 100, 30));

    private readonly MessageHandler messageHandler = new(ServerConfig.CreateHttpClient());
    private readonly ScrollablePanel chatPanel;
    private readonly ScrollablePanel userListPanel;
    private readonly ScrollablePanel inputPanel;
    private List<MessageDTO> messages = new();
    private List<ChatMessage> chatMessageBubbles = new();
    private int lastMessageId = 0;
    private readonly ConcurrentQueue<MessageDTO> incomingMessages = new();
    private bool pollingStarted = false;
    private CancellationTokenSource cts = new();
    private bool hasLoadedInitialMessageHistory = false;


    public ChatScreen()
    {
        logic = new ChatScreenLogic(inputField, sendButton, backButton, SendMessageAsync);
        chatPanel = new ScrollablePanel(new Rectangle(), scrollSpeed: 30f);
        userListPanel = new ScrollablePanel(new Rectangle(), scrollSpeed: 20f);
        inputPanel = new ScrollablePanel(new Rectangle(), scrollSpeed: 20f);
    }


    protected override ChatScreenLayout.LayoutData CalculateLayout()
        => ChatScreenLayout.Calculate(ResourceLoader.LogoTexture.Width, ResourceLoader.LogoTexture.Height);

    protected override void ApplyLayout(ChatScreenLayout.LayoutData layout)
    {
        inputField.SetRect(layout.InputRect);
        sendButton.SetRect(layout.SendRect);
        backButton.SetRect(layout.BackRect);
        chatPanel.SetBounds(layout.ChatRect);
        userListPanel.SetBounds(layout.UserListRect);
        inputPanel.SetBounds(layout.InputRect);
    }

    public override void RenderContent()
    {
        // 1. Make sure chat is initialized
        if (!hasLoadedInitialMessageHistory)
        {
            _ = LoadChatHistoryAsync();
            return;
        }

        // 2. Start polling only after initialization
        if (!pollingStarted && layout.ChatRect.Width > 0)
        {
            pollingStarted = true;
            cts = new CancellationTokenSource();
            _ = Task.Run(() => PollMessagesAsync(cts.Token));
        }

        // 3. Handle incoming queued messages (unchanged)
        while (incomingMessages.TryDequeue(out var msg))
        {
            messages.Add(msg);
            lastMessageId = Math.Max(lastMessageId, msg.Id);
            chatMessageBubbles.Add(new ChatMessage(msg, layout.ChatRect.Width - 20));
        }

        // Logo
        Raylib.DrawTextureEx(ResourceLoader.LogoTexture,
            new Vector2(layout.LogoX, layout.LogoY),
            0f, layout.LogoScale, Color.White);

        // Chat window background
        Raylib.DrawRectangleRounded(layout.ChatRect, 0.01f, 10, Colors.TextFieldUnselected);
        Raylib.DrawRectangleRoundedLinesEx(layout.ChatRect, 0.01f, 10, 1, Colors.OutlineColor);

        // Draw messages
        float totalChatHeight = chatMessageBubbles.Sum(m => m.Height + 8f);
        chatPanel.BeginScroll(totalChatHeight);

        float currentY = layout.ChatRect.Y + 10;
        foreach (var chatMsg in chatMessageBubbles)
        {
            float scrolledY = chatPanel.GetScrolledY(currentY);
            if (chatPanel.IsVisible(scrolledY, chatMsg.Height))
            {
                chatMsg.Draw(layout.ChatRect.X + 10, scrolledY);
            }
            currentY += chatMsg.Height + 8f;
        }

        chatPanel.EndScroll();


        // User list panel
        Raylib.DrawRectangleRounded(layout.UserListRect, 0.08f, 10, Colors.PanelColor);
        Raylib.DrawRectangleRoundedLinesEx(layout.UserListRect, 0.08f, 10, 1, Colors.OutlineColor);

        DrawUserList();

        // Input + send
        DrawInputWithScroll();
        sendButton.Draw();

        // Back
        backButton.Draw();
    }

    private async void SendMessageAsync(string text)
    {
        if (messageHandler == null) return;

        string sender = !string.IsNullOrEmpty(AppState.LoggedInUsername)
            ? AppState.LoggedInUsername
            : "Anonymous Duck";

        bool ok = await messageHandler.SendMessageAsync(text);

        // Don't assign ID locally — the polling loop will fetch it
    }

    private void DrawUserList()
    {
        // Placeholder data
        var onlineUsers = new[] {
            "Ducklord",
            "QuackyMcQuack",
            "DaffyDev",
            "DuckyDuck",
            "Goobert",
            "DuckyDuck"
        };
        var offlineUsers = new[] {
            "SleepyDuck",
            "LazyFeathers",
            "Bunny",
            "DuckyDuck",
            "DuckyDuck",
            "DuckyDuck",
            "Felhantering",
            "Varför",
            "Funkar",
            "inte",
            "detta?"
        };

        const float lineH = 22;
        const float fontSize = 14;

        float totalHeight = lineH + // "ONLINE" header
                            (onlineUsers.Length * lineH) +
                            10 + // spacing
                            lineH + // "OFFLINE" header
                            (offlineUsers.Length * lineH);

        userListPanel.BeginScroll(totalHeight);

        float x = layout.UserListRect.X + 10;
        float y = layout.UserListRect.Y + 10;

        // Online header
        float scrolledY = userListPanel.GetScrolledY(y);
        Raylib.DrawTextEx(ResourceLoader.BoldFont, "ONLINE",
            new Vector2(x, scrolledY), fontSize, 0.5f, Colors.AccentColor);
        y += lineH;

        // Online users
        foreach (var user in onlineUsers)
        {
            scrolledY = userListPanel.GetScrolledY(y);
            if (userListPanel.IsVisible(scrolledY, lineH))
            {
                Raylib.DrawCircle((int)x + 5, (int)scrolledY + 7, 4f, Colors.AccentColor);
                Raylib.DrawTextEx(ResourceLoader.RegularFont, user,
                    new Vector2(x + 15, scrolledY), fontSize, 0.5f, Colors.UiText);
            }
            y += lineH;
        }

        y += 10; // Extra spacing

        // Offline header
        scrolledY = userListPanel.GetScrolledY(y);
        Raylib.DrawTextEx(ResourceLoader.BoldFont, "OFFLINE",
            new Vector2(x, scrolledY), fontSize, 0.5f, Colors.SubtleText);
        y += lineH;

        // Offline users
        foreach (var user in offlineUsers)
        {
            scrolledY = userListPanel.GetScrolledY(y);
            if (userListPanel.IsVisible(scrolledY, lineH))
            {
                Raylib.DrawCircle((int)x + 5, (int)scrolledY + 7, 4f, Colors.SubtleText);
                Raylib.DrawTextEx(ResourceLoader.RegularFont, user,
                    new Vector2(x + 15, scrolledY), fontSize, 0.5f, Colors.SubtleText);
            }
            y += lineH;
        }

        userListPanel.EndScroll();
    }

    private void DrawInputWithScroll()
    {
        // TextField handles it's own rendering, but we can wrap it in scroll
        // if the text gets too long (multiline)

        // For now: draw TextField as normal
        // You can add scroll later if the content grows
        // inputPanel.BeginScroll(inputField.Height);
        inputField.Draw();
        // inputPanel.EndScroll();
        // Count text's height and use inputPanel.BeginScroll/EndScroll
    }

    /// <summary>
    /// Continuously polls the server for new chat messages and enqueues them for rendering.
    /// Runs in the background while the chat screen is active. UI rendering pulls messages
    /// from the <c>incomingMessages</c> queue.
    /// </summary>
    private async Task PollMessagesAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                Log.Info($"[Poll] Requesting updates after ID {lastMessageId}");
                var updates = await messageHandler.ReceiveUpdatesAsync(lastMessageId);

                if (updates != null && updates.Any())
                {
                    Log.Info($"[Poll] Received {updates.Count} messages");
                    foreach (var msg in updates)
                    {
                        Log.Info($"[Poll] Message ID {msg.Id}: {msg.Content ?? "<no content>"}");
                        if (msg.Id > lastMessageId)
                            incomingMessages.Enqueue(msg);
                    }
                }
                else
                {
                    Log.Info("[Poll] No new messages");
                }

                await Task.Delay(200, token); // Add delay between polls
            }
            catch (Exception ex)
            {
                Log.Error("[ChatScreen] Polling error: " + ex.Message);
                await Task.Delay(200, token);
            }
        }
    }
    private async Task LoadChatHistoryAsync()
    {
        if (hasLoadedInitialMessageHistory) return;
        hasLoadedInitialMessageHistory = true;

        // Load recent history (or full history if you prefer)
        var history = await messageHandler.ReceiveHistoryAsync(30);
        messages = history ?? new List<MessageDTO>();

        // Convert to bubbles
        chatMessageBubbles = messages
            .Select(m => new ChatMessage(m, layout.ChatRect.Width - 20))
            .ToList();

        // Set lastMessageId correctly
        lastMessageId = messages.Any() ? messages.Max(m => m.Id) : 0;
    }
}
