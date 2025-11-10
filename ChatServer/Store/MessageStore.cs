using ChatServer.Models;

namespace ChatServer.Store;

public class MessageStore
{
  private readonly List<ChatMessage> messages = [];

  private int nextMessageId = 1;

  #region ADD MESSAGE
  /// <summary>
  /// Adds a new chat message to the store.
  /// </summary>
  /// <param name="user">The user sending the message. Must not be null.</param>
  /// <param name="messageContent">The text content of the message. Must not be null or whitespace.</param>
  /// <returns>
  /// True if the message was successfully added.
  /// False if <paramref name="user"/> is null or <paramref name="messageContent"/> is invalid.
  /// </returns>
  #endregion
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

  #region GET ALL MESSAGES
  /// <summary>
  /// Returns all stored chat messages in insertion order.
  /// </summary>
  /// <returns>
  /// A read-only list of <see cref="ChatMessage"/> objects.
  /// The caller can read and iterate through the messages without altering the internal storage.
  /// </returns>
  #endregion
  public IReadOnlyList<ChatMessage> GetAll()
  {
    return messages;
  }

  public void RemoveById(int messageId)
  {
    throw new NotImplementedException();
  }
}
