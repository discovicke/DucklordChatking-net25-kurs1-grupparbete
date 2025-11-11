using System;
using System.Net.Http;
using System.Net.Http.Json;
using Shared;

namespace ChatClient.Data
{
    // Responsible for sending messages to server via HTTP
    public class MessageHandler
    {
        private readonly HttpClient _httpClient;

        public MessageHandler(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

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
                Sender = UserAccount.Username!,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            if (string.IsNullOrWhiteSpace(messageDto.Sender) || string.IsNullOrWhiteSpace(messageDto.Content))
            {
                return false;
            }

            if (!UserAccount.IsLoggedIn)
            {
                throw new InvalidOperationException("Ingen användare inloggad!");
            }

            try
            {
                var response = _httpClient.PostAsJsonAsync("/messages", messageDto).Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}