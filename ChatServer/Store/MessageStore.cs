using ChatServer.Models;
using Shared;

namespace ChatServer.Store;

public class MessageStore(UserStore userStore)
{
  private readonly UserStore userStore = userStore;
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
  public bool Add(string username, string messageContent)
  {
    if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(messageContent)) // Validate message sender and content
    { return false; }

    var newMessage = new ChatMessage // Create new message
    {
      Id = nextMessageId++,
      SenderId = userStore.GetByUsername(username)?.Id ?? 0,
      Content = messageContent,
      Timestamp = DateTime.UtcNow
    };

    if (newMessage.SenderId == 0) // Validate message sender
    { return false; }

    // Store message in both list and dictionary
    messages.Add(newMessage);
    messagesById.Add(newMessage.Id, newMessage);
    return true;
  }

  #region GET ALL MESSAGES
  /// <summary>
  /// Converts all stored chat messages into DTOs and returns them in insertion order.
  /// </summary>
  /// <remarks>
  /// Each returned message contains the sender's username resolved from the UserStore.
  /// If a message references a user that no longer exists, the method throws an exception.
  /// </remarks>
  /// <returns>
  /// A read-only list of <see cref="MessageDTO"/> objects formatted for client use.
  /// </returns>
  #endregion
  public IReadOnlyList<MessageDTO> GetAll()
  {
    var result = new List<MessageDTO>();

    foreach (var m in messages)
    {
      var user = userStore.GetById(m.SenderId) ?? throw new InvalidOperationException(
            $"Message with ID {m.Id} references a missing user"
        );
      result.Add(new MessageDTO
      {
        Sender = user.Username,
        Content = m.Content,
        Timestamp = m.Timestamp
      });
    }

    return result;
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
