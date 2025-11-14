using System;
using System.Net.Http;
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
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

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

    }
}
