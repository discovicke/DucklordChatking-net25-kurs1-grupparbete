using System.Net.Http.Json;
using ChatClient.Core;
using Shared;

namespace ChatClient.Data
{
    // Responsible for sending messages to server via HTTP
    public class MessageHandler(HttpClient httpClient)
    {
        private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        // Sends message to server. Returns true if message was sent, false otherwise.
        public bool SendMessage(string content)
        {
            ArgumentNullException.ThrowIfNull(content);

            //Converts current user and added content to a DTO for sending to server
            var messageDto = new MessageDTO
            {
                Sender = AppState.LoggedInUsername,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            if (string.IsNullOrWhiteSpace(messageDto.Sender) || string.IsNullOrWhiteSpace(messageDto.Content))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(AppState.LoggedInUsername))
            {
                throw new InvalidOperationException("Ingen duck inloggad!");
            }

            try
            {
                var response = httpClient.PostAsJsonAsync("/messages/send", messageDto).Result;
                if (response.IsSuccessStatusCode)
                {
                    Log.Success($"Message sent by {messageDto.Sender}");
                    Log.Info(messageDto.Content);
                    return true;
                }

                Log.Error($"Server returned {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"Exception sending message: {ex.Message}");
                return false;
            }
        }

        public void SendHeartbeat()
        {
            if (string.IsNullOrWhiteSpace(AppState.LoggedInUsername))
                return;

            try
            {
                var heartbeatDto = new { Username = AppState.LoggedInUsername };
                httpClient.PostAsJsonAsync("/users/heartbeat", heartbeatDto).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Log.Error($"Heartbeat failed: {ex.Message}");
            }
        }

        public List<MessageDTO>? ReceiveHistory(int? take = null)
        {
            try
            {
                var url = take.HasValue
                    ? $"/messages/history?take={take}"
                    : "/messages/history";

                var response = httpClient.GetAsync(url).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                    return null;

                var messages = response.Content
                    .ReadFromJsonAsync<List<MessageDTO>>()
                    .GetAwaiter()
                    .GetResult();

                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving history: {ex.Message}");
                return null;
            }
        }

        // ASYNC

        public async Task SendHeartbeatAsync()
        {
            if (string.IsNullOrWhiteSpace(AppState.LoggedInUsername))
                return;

            try
            {
                var heartbeatDto = new { Username = AppState.LoggedInUsername };
                var response = await httpClient.PostAsJsonAsync("/users/heartbeat", heartbeatDto);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Error($"Heartbeat failed: server returned {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Heartbeat failed: {ex.Message}");
            }
        }

        public async Task<bool> SendMessageAsync(string content)
        {
            ArgumentNullException.ThrowIfNull(content);

            if (string.IsNullOrWhiteSpace(AppState.LoggedInUsername))
                throw new InvalidOperationException("Ingen duck inloggad!");

            var messageDto = new MessageDTO
            {
                Sender = AppState.LoggedInUsername,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            if (string.IsNullOrWhiteSpace(messageDto.Sender) || string.IsNullOrWhiteSpace(messageDto.Content))
                return false;

            try
            {
                var response = await httpClient.PostAsJsonAsync("/messages/send", messageDto);
                if (response.IsSuccessStatusCode)
                {
                    Log.Success($"Message sent by {messageDto.Sender}");
                    Log.Info(messageDto.Content);
                    return true;
                }

                Log.Error($"Server returned {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"Exception sending message: {ex.Message}");
                return false;
            }
        }

        public async Task<List<MessageDTO>?> ReceiveHistoryAsync(int? take = null)
        {
            try
            {
                var url = take.HasValue
                    ? $"/messages/history?take={take}"
                    : "/messages/history";

                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var messages = await response.Content.ReadFromJsonAsync<List<MessageDTO>>();
                return messages ?? [];
            }
            catch (Exception ex)
            {
                Log.Error($"ReceiveHistoryAsync failed: {ex.Message}");
                return null;
            }
        }

        public async Task<List<MessageDTO>> ReceiveUpdatesAsync(int lastSeenId)
        {
            try
            {
                var response = await httpClient.GetAsync($"/messages/updates?lastId={lastSeenId}");
                if (!response.IsSuccessStatusCode)
                    return [];

                var messages = await response.Content.ReadFromJsonAsync<List<MessageDTO>>();
                return messages ?? [];
            }
            catch (Exception ex)
            {
                Log.Error($"ReceiveUpdatesAsync failed: {ex.Message}");
                return [];
            }
        }

    }
}
