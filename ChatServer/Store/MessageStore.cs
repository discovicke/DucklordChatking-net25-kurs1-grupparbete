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
  /// Retrieves every stored chat message and returns them in the order they were added.
  /// </summary>
  /// <remarks>
  /// Each stored message is converted into a <see cref="MessageDTO"/>. The sender identifier is resolved to a username
  /// by querying the <see cref="UserStore"/>. The returned list preserves the chronological order of insertion.
  /// </remarks>
  /// <returns>
  /// A read-only list of <see cref="MessageDTO"/> representing the full chat history.
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


  #region GET LAST MESSAGES
  /// <summary>
  /// Returns the most recently stored chat messages in chronological order up to the amount specified.
  /// </summary>
  /// <param name="count">
  /// The number of messages the caller wants to retrieve. If the value is larger than the number of stored messages,
  /// all available messages are returned.
  /// </param>
  /// <returns>
  /// A read-only list of <see cref="MessageDTO"/> representing the latest messages in the store. The list contains the
  /// newest messages based on insertion order, formatted for client use.
  /// </returns>
  #endregion
  public IReadOnlyList<MessageDTO> GetLast(int count)
  {
    if (count <= 0)
      return [];

    // Determine starting index
    int startIndex = Math.Max(0, messages.Count - count);

    var result = new List<MessageDTO>(count);

    for (int i = startIndex; i < messages.Count; i++)
    {
      var m = messages[i];
      var user = userStore.GetById(m.SenderId)
                 ?? throw new InvalidOperationException($"Message with ID {m.Id} references a missing user");

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
