using System.Collections.Concurrent;
using System.Linq;
using ChatClient.Core.Application;
using ChatClient.Core.Infrastructure;
using ChatClient.Data.Services;
using Shared;

namespace ChatClient.UI.Screens.Chat.Services;

/// <summary>
/// Responsible for: async polling of message updates, sending messages, and managing background polling loop.
/// Uses async/await pattern with CancellationToken for proper lifecycle management.
/// </summary>
public class ChatDataService(MessageHandler handler)
{
    private List<MessageDTO> messages = new();
    private int latestReceivedMessageId;
    private bool hasLoadedInitialHistory;
    private bool isPolling;
    private CancellationTokenSource? pollingCts;
    private readonly ConcurrentQueue<MessageDTO> incomingMessages = new();
    private List<UserStatusDTO> lastStatuses = new();

    public event Action<IReadOnlyList<MessageDTO>>? MessagesChanged;
    public event Action<IReadOnlyList<UserStatusDTO>>? UsersStatusChanged;

    /// <summary>
    /// Loads initial chat history. Should be called once before starting polling.
    /// </summary>
    public async Task LoadChatHistoryAsync()
    {
        if (hasLoadedInitialHistory)
        {
            return;
        }
        hasLoadedInitialHistory = true;

        Log.Info("[ChatDataService] Loading initial chat history");
        var history = await handler.ReceiveHistoryAsync(AppState.HistoryFetchCount);
        messages = history ?? new List<MessageDTO>();

        latestReceivedMessageId = messages.Count != 0 ? messages.Max(m => m.Id) : 0;

        MessagesChanged?.Invoke(messages);
        Log.Info($"[ChatDataService] Loaded {messages.Count} messages, latest ID: {latestReceivedMessageId}");

        // Fetch initial user statuses so UI has data before polling loop kicks in
        try
        {
            var statuses = await handler.GetUserStatusesAsync();
            if (statuses.Count > 0)
            {
                lastStatuses = statuses;
                UsersStatusChanged?.Invoke(lastStatuses);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"[ChatDataService] Initial status fetch failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Starts background polling loop. Only starts once.
    /// </summary>
    public void StartPolling()
    {
        if (isPolling || !hasLoadedInitialHistory)
        {
            return;
        }

        isPolling = true;
        pollingCts = new CancellationTokenSource();

        var token = pollingCts.Token;

        // Start message long polling
        _ = Task.Run(() => PollMessagesAsync(token));

        // Start heartbeat loop
        _ = Task.Run(() => HeartbeatLoopAsync(token));

        Log.Info("[ChatDataService] Polling + Heartbeat started");
    }

    /// <summary>
    /// Stops the background polling loop.
    /// </summary>
    public void StopPolling()
    {
        if (!isPolling) return;

        pollingCts?.Cancel();
        isPolling = false;
        Log.Info("[ChatDataService] Polling stopped");
    }

    /// <summary>
    /// Sends a message asynchronously.
    /// </summary>
    public async void SendMessageAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        string sender = !string.IsNullOrEmpty(AppState.LoggedInUsername)
            ? AppState.LoggedInUsername
            : "Anonymous Duck";

        Log.Info($"[ChatDataService] Sending message as '{sender}': {text}");
        await handler.SendMessageAsync(text);

        // Message will be fetched by polling loop
    }

    /// <summary>
    /// Processes queued messages from background polling. Call from main render thread.
    /// </summary>
    public void ProcessIncomingMessages()
    {
        bool hasNewMessages = false;
        while (incomingMessages.TryDequeue(out var msg))
        {
            messages.Add(msg);
            latestReceivedMessageId = Math.Max(latestReceivedMessageId, msg.Id);
            hasNewMessages = true;
        }

        if (hasNewMessages)
        {
            MessagesChanged?.Invoke(messages);
        }
    }

    /// <summary>
    /// Background polling loop that continuously checks for new messages.
    /// </summary>
    private async Task PollMessagesAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                Log.Info($"[Poll] Requesting updates after ID {latestReceivedMessageId}");
                var updates = await handler.ReceiveUpdatesAsync(latestReceivedMessageId);

                if (updates != null && updates.Count != 0)
                {
                    Log.Info($"[Poll] Received {updates.Count} messages");
                    foreach (var msg in updates)
                    {
                        Log.Info($"[Poll] Message ID {msg.Id}: {msg.Content ?? "<no content>"}");
                        if (msg.Id > latestReceivedMessageId)
                        {
                            incomingMessages.Enqueue(msg);
                        }
                    }
                }
                else
                {
                    Log.Info("[Poll] No new messages");
                }



                await Task.Delay(150, token);
            }
            catch (OperationCanceledException)
            {
                Log.Info("[Poll] Polling cancelled");
                break;
            }
            catch (Exception ex)
            {
                Log.Error($"[Poll] Error: {ex.Message}");
                await Task.Delay(150, token);
            }
        }
    }

    private async Task HeartbeatLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                // send heartbeat
                await handler.SendHeartbeatAsync();

                // fetch statuses
                var statuses = await handler.GetUserStatusesAsync();

                if (statuses.Count > 0)
                {
                    bool changed =
                        statuses.Count != lastStatuses.Count ||
                        statuses.Any(s => !lastStatuses.Any(p =>
                            p.Username == s.Username && p.Online == s.Online));

                    if (changed)
                    {
                        lastStatuses = statuses;
                        UsersStatusChanged?.Invoke(lastStatuses);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Heartbeat] Failed: {ex.Message}");
            }

            await Task.Delay(3000, token); // every 3 seconds
        }
    }

    public IReadOnlyList<MessageDTO> GetCachedMessages() => messages;
    public IReadOnlyList<UserStatusDTO> GetCachedStatuses() => lastStatuses;
    public bool HasLoadedHistory => hasLoadedInitialHistory;
}
