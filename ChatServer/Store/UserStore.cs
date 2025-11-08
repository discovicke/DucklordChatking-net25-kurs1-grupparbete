namespace ChatServer.Store;

public class UserStore
{
  // Dictionaries for storing and looking up users
  private readonly Dictionary<string, User> usersByUsername = [];
  private readonly Dictionary<int, User> usersById = [];

  // User ID counter
  private int nextId = 1;

  #region ADD USER
  /// <summary>
  /// Adds a new user to the store. The method assigns a unique ID and registers
  /// the user in both UserStore dictionaries (by username and by ID).
  /// </summary>
  /// <param name="username">
  /// The identifier the caller wants to use when referring to the user.
  /// </param>
  /// <param name="password">
  /// The secret credential stored for authentication.
  /// </param>
  /// <returns>
  /// True when creation succeeds. False when a user with the same username already exists.
  /// </returns>
  #endregion
  public bool Add(string username, string password)
  {
    if (usersByUsername.ContainsKey(username))
    {
      return false;
    }

    User newUser = new(username, password)
    {
      Id = nextId++
    };

    usersByUsername.Add(username, newUser);
    usersById.Add(newUser.Id, newUser);
    return true;
  }

  #region REMOVE USER (by USERNAME)
  /// <summary>
  /// Removes a user from the store using their username.
  /// Deletes the user from both dictionaries to maintain consistency.
  /// </summary>
  /// <param name="username">
  /// The username of the user that should be removed.
  /// </param>
  /// <returns>
  /// True when a user with the specified username existed and was removed. False when no such user was found.
  /// </returns>
  #endregion
  public bool Remove(string username)
  {
    if (!usersByUsername.TryGetValue(username, out var user))
      return false;

    usersByUsername.Remove(username);
    usersById.Remove(user.Id);

    return true;
  }

  #region REMOVE USER (by ID)
  /// <summary>
  /// Removes a user from the store using their unique ID.
  /// Deletes the user from both dictionaries to maintain consistency.
  /// </summary>
  /// <param name="id">
  /// The ID of the user that should be removed.
  /// </param>
  /// <returns>
  /// True when a user with the specified ID existed and was removed. False when no such user was found.
  /// </returns>
  #endregion
  public bool Remove(int id)
  {
    if (!usersById.TryGetValue(id, out var user))
      return false;

    usersById.Remove(id);
    usersByUsername.Remove(user.Username);

    return true;
  }

  // UPDATE USER
  public bool Update(string oldUsername, string newUsername, string? newPassword = null)
  {
    // TODO: Implement logic
    return false;
  }

  #region GET USER (by USERNAME)
  /// <summary>
  /// Retrieves a user based on their username.
  /// </summary>
  /// <param name="username">
  /// The username used as the lookup key.
  /// </param>
  /// <returns>
  /// The matching user when found, otherwise null.
  /// </returns>
  #endregion
  public User? GetByUsername(string username)
  {
    usersByUsername.TryGetValue(username, out var user);
    return user;
  }

  #region GET USER (by ID)
  /// <summary>
  /// Retrieves a user based on their unique ID.
  /// </summary>
  /// <param name="id">
  /// The numeric identifier assigned to the user when created.
  /// </param>
  /// <returns>
  /// The matching user when found, otherwise null.
  /// </returns>
  #endregion
  public User? GetById(int id)
  {
    usersById.TryGetValue(id, out var user);
    return user;
  }

  // GET ALL USERNAMES
  public IEnumerable<string> GetAllUsernames()
  {
    // TODO: Implement logic
    return [];
  }
}
