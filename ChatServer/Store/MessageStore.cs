using ChatServer.Models;

namespace ChatServer.Store;

public class MessageStore
{
  private readonly List<ChatMessage> messages = [];
  private readonly Dictionary<int, ChatMessage> messagesById = [];

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

    // Store message in both list and dictionary
    messages.Add(newMessage);
    messagesById.Add(newMessage.Id, newMessage);
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

  #region REMOVE MESSAGE BY ID
  /// <summary>
  /// Removes a chat message with the specified identifier from the store.
  /// </summary>
  /// <param name="messageId">The unique identifier of the message to remove.</param>
  /// <returns>
  /// True if the message was found and removed.
  /// False if no message with the given <paramref name="messageId"/> exists.
  /// </returns>
  #endregion
  public bool RemoveById(int messageId)
  {
    if (messagesById.TryGetValue(messageId, out var message))
    {
      messages.Remove(message);
      messagesById.Remove(messageId);
      return true;
    }

    return false;
  }

}
