using System;
using System.Net.Http;
using System.Net.Http.Json;
using Shared;

namespace ChatClient.Data
{
    // Responsible for sending messages to server via HTTP
    public class MessageSender
    {
        private readonly HttpClient _httpClient;

        public MessageSender(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // Sends message to server. Returns true if message was sent, false otherwise.
        public bool SendMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var dto = message.ToDTO();

            if (string.IsNullOrWhiteSpace(dto.Sender) || string.IsNullOrWhiteSpace(dto.Content))
                return false;

            try
            {
                var response = _httpClient.PostAsJsonAsync("/messages", dto).Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}