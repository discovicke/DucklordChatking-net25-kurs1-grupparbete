using Shared;

namespace ChatClient.Data;

public class Message
{
    public string Sender { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    // Skapar en DTO från detta objekt (när vi skickar till servern)
    public MessageDTO SendToDTO()
    {
        return new MessageDTO
        {
            Sender = this.Sender,
            Content = this.Content,
            Timestamp = DateTime.UtcNow // klienten sätter ev. nuvarande tid
        };
    }

    // Skapar ett Message-objekt från en DTO (när vi tar emot från servern)
    public static Message GetFromDTO(MessageDTO dto)
    {
        return new Message
        {
            Sender = dto.Sender ?? "Unknown",
            Content = dto.Content ?? string.Empty,
            Timestamp = dto.Timestamp
        };
    }
}