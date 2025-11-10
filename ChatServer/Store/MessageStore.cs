using ChatServer.Models;

namespace ChatServer.Store;

public class MessageStore
{
  private readonly List<ChatMessage> messages = [];

  private int nextMessageId = 1;


  public bool Add(User user, string messageContent)
  {
    if (user == null || string.IsNullOrWhiteSpace(messageContent)) // Validate message sender and content
    { return false; }

    var newMessage = new ChatMessage // Create new message
    {
      Id = nextMessageId++,
      SenderId = user.Id,
      Content = messageContent,
      Timestamp = DateTime.UtcNow
    };

    messages.Add(newMessage); // Store the new message
    return true;
  }

  public List<ChatMessage> GetAll()
  {
    throw new NotImplementedException();
  }

  public void Remove(int messageId)
  {
    throw new NotImplementedException();
  }
}
