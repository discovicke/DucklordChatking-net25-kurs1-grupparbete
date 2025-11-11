using System;
using Shared;

namespace ChatClient.Data
{
    // Represents a chat message in the client application
    public class Message
    {
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        
        // Creates a DTO (when sending to server)
        public MessageDTO ToDTO()
        {
            return new MessageDTO
            {
                Sender = Sender,
                Content = Content,
                Timestamp = DateTime.UtcNow
            };
        }

        // Creates a message from a DTO (when receiving from server)
        public static Message FromDTO(MessageDTO dto)
        {
            return new Message
            {
                Sender = dto.Sender ?? "Unknown",
                Content = dto.Content ?? string.Empty,
                Timestamp = dto.Timestamp
            };
        }
    }
}