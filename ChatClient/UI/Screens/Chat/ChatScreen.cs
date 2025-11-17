using System.Numerics;
using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.Data.Services;
using ChatClient.UI.Components;
using ChatClient.UI.Components.Base;
using ChatClient.UI.Components.Specialized;
using ChatClient.UI.Screens.Chat.Components;
using ChatClient.UI.Screens.Chat.Services;
using ChatClient.UI.Screens.Common;
using ChatClient.UI.Theme;
using Raylib_cs;
using Shared;

namespace ChatClient.UI.Screens.Chat;

/// <summary>
/// ChatScreen: Composition root for the chat interface.
/// Responsibilities:
/// - Wire up views (messages, user list, toolbar)
/// - Delegate async data fetching to ChatDataService
/// - Apply layout from ChatScreenLayout
/// - Coordinate rendering with async message updates
/// </summary>
public class ChatScreen : ScreenBase<ChatScreenLayout.LayoutData>
{
    #region Fields: UI Components
    private readonly ChatMessagesView messagesView;
    private readonly UserListView userListView;
    private readonly ChatToolbar toolbar;
    private readonly BackButton backButton;
    #endregion

    #region Fields: Data & Services
    private readonly ChatDataService dataService;
    private List<MessageDTO> currentMessages = new();
    private List<string> online = new();
    private List<string> offline = new();
    #endregion

    public ChatScreen()
    {
        // Create scroll panels
        var chatPanel = new ScrollablePanel(new Rectangle(), scrollSpeed: 30f);
        var userListPanel = new ScrollablePanel(new Rectangle(), scrollSpeed: 20f);

        // Create views
        messagesView = new ChatMessagesView(chatPanel);
        userListView = new UserListView(userListPanel);

        // Create toolbar components
        var inputField = new TextField(new Rectangle(),
            Colors.TextFieldUnselected, Colors.TextFieldHovered, Colors.TextColor,
            true, false, "ChatScreen_MessageInput", "Quack a message... (Shift+Enter for new line)");
        var sendButton = new Button(new Rectangle(), "Send",
            Colors.ButtonDefault, Colors.ButtonHovered, Colors.TextColor);
        toolbar = new ChatToolbar(inputField, sendButton);

        // Back button
        backButton = new BackButton(new Rectangle(10, 10, 100, 30));

        // Data service
        var messageHandler = new MessageHandler(ServerConfig.CreateHttpClient());
        dataService = new ChatDataService(messageHandler);

        // Wire up events
        dataService.MessagesChanged += OnMessagesChanged;
        dataService.UsersStatusChanged += OnUsersStatusChanged;
        toolbar.SendPressed += OnSendPressed;

        // Logic (simplified - now just handles back button)
        logic = new ChatScreenLogic(this, backButton);
    }

    protected override ChatScreenLayout.LayoutData CalculateLayout()
        => ChatScreenLayout.Calculate(ResourceLoader.LogoTexture.Width, ResourceLoader.LogoTexture.Height);

    protected override void ApplyLayout(ChatScreenLayout.LayoutData layout)
    {
        messagesView.SetBounds(layout.ChatRect);
        userListView.SetBounds(layout.UserListRect);
        toolbar.SetBounds(layout.InputRect, layout.SendRect);
        backButton.SetRect(layout.BackRect);
    }

    public override void RenderContent()
    {
        if (!CanRender()) return;

        // Ensure history is loaded before starting polling
        if (!EnsureHistoryLoaded()) return;

        // Start polling once layout is ready
        StartPollingIfNeeded();

        // Process any incoming messages from background polling
        dataService.ProcessIncomingMessages();

        // Logo
        Raylib.DrawTextureEx(ResourceLoader.LogoTexture,
            new Vector2(layout.LogoX, layout.LogoY),
            0f, layout.LogoScale, Color.White);

        // Chat window background
        Raylib.DrawRectangleRounded(layout.ChatRect, 0.01f, 10, Colors.TextFieldUnselected);
        Raylib.DrawRectangleRoundedLinesEx(layout.ChatRect, 0.01f, 10, 1, Colors.OutlineColor);

        // Render messages
        messagesView.Render();

        // User list background
        Raylib.DrawRectangleRounded(layout.UserListRect, 0.08f, 10, Colors.PanelColor);
        Raylib.DrawRectangleRoundedLinesEx(layout.UserListRect, 0.08f, 10, 1, Colors.OutlineColor);

        // Render user list
        userListView.Render(online, offline);

        // Toolbar (input + send button)
        toolbar.Update();
        toolbar.Draw();

        // Back button
        backButton.Draw();
    }

    #region Event Handlers
    private void OnMessagesChanged(IReadOnlyList<MessageDTO> messages)
    {
        currentMessages = messages.ToList();
        float usableWidth = layout.ChatRect.Width - 20; // 10px insets on both sides
        messagesView.UpdateMessages(currentMessages, usableWidth);
    }

    private void OnUsersStatusChanged(IReadOnlyList<UserStatusDTO> statuses)
    {
        // Split into online/offline and sort alphabetically
        online = statuses.Where(s => s.Online).Select(s => s.Username).OrderBy(n => n).ToList();
        offline = statuses.Where(s => !s.Online).Select(s => s.Username).OrderBy(n => n).ToList();
    }

    private void OnSendPressed(string text)
    {
        dataService.SendMessageAsync(text);
    }
    #endregion

    #region Public API for ChatScreenLogic
    /// <summary>
    /// Called by ChatScreenLogic when navigating away from chat.
    /// </summary>
    public void StopPolling()
    {
        dataService.StopPolling();
    }
    #endregion

    #region Render Pipeline Helpers
    /// <summary>
    /// Ensures RenderContent() only runs while the Chat screen is active.
    /// Prevents accidental rendering during or after a screen transition.
    /// </summary>
    private static bool CanRender() => AppState.CurrentScreen == Screen.Chat;

    /// <summary>
    /// Loads recent chat history the first time the screen renders.
    /// If history is not yet available, triggers an async load and halts rendering for this frame.
    /// </summary>
    private bool EnsureHistoryLoaded()
    {
        if (dataService.HasLoadedHistory)
            return true;

        _ = dataService.LoadChatHistoryAsync();
        return false;
    }

    /// <summary>
    /// Starts the background polling loop once the layout is ready and history has been loaded.
    /// Ensures polling is only started once.
    /// </summary>
    private void StartPollingIfNeeded()
    {
        if (layout.ChatRect.Width <= 0) return;

        dataService.StartPolling();
    }
    #endregion
}
