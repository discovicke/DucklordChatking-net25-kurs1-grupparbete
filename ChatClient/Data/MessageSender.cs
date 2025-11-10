using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared;

namespace ChatClient.Data
{
    public class MessageSender
    {
        private readonly HttpClient _httpClient;

        public MessageSender(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public bool SendMessage(MessageDTO message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            
            if (string.IsNullOrWhiteSpace(message.Sender) || string.IsNullOrWhiteSpace(message.Content))
            {
                return false;
            }

            try
            {
                var response = _httpClient.PostAsJsonAsync("/messages", message).Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}