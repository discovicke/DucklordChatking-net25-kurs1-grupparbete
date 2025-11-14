using ChatServer.Models;

namespace ChatServer.Store;

public class UserStore
{
  // Dictionaries for storing and looking up users
  private readonly Dictionary<string, User> usersByUsername =
      new(StringComparer.OrdinalIgnoreCase);  // Change stringcomparer setting to ignore case sensitivity
  private readonly Dictionary<int, User> usersById = [];
  private readonly Dictionary<string, User> usersBySessionAuthToken = [];

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

  #region UPDATE USER
  /// <summary>
  /// Updates the username and optionally the password of an existing user.
  /// The update keeps dictionary consistency by removing the old key and inserting the new one.
  /// </summary>
  /// <param name="oldUsername">
  /// The current username used to locate the existing user.
  /// </param>
  /// <param name="newUsername">
  /// The username that replaces the old one. Must be unused by other users.
  /// </param>
  /// <param name="newPassword">
  /// A new password to assign. When null or empty, the existing password remains.
  /// </param>
  /// <returns>
  /// True when the update succeeds. False when the old username is not found or the new one is already taken.
  /// </returns>
  #endregion
  public bool Update(string oldUsername, string newUsername, string? newPassword = null)
  {
    if (!usersByUsername.TryGetValue(oldUsername, out var user)) { return false; } // If the old username does not exist, the update cannot continue.
    if (oldUsername != newUsername && usersByUsername.ContainsKey(newUsername)) { return false; } // If the username changes, make sure the new username is not already taken.

    // Remove the old username key from the dictionary. (Note: the user object still exists in memory at this point)
    usersByUsername.Remove(oldUsername);

    // Update the properties on the existing User instance.
    user.Username = newUsername;
    if (!string.IsNullOrWhiteSpace(newPassword)) { user.Password = newPassword; } // Only update the password if a new one was provided.

    // Insert the updated user using the new username as key.
    usersByUsername.Add(newUsername, user);

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

  public User? GetBySessionAuthToken(string token)
  {
    usersBySessionAuthToken.TryGetValue(token, out var user);
    return user;
  }

  #region GET ALL USER[NAMES]
  /// <summary>
  /// Returns a collection of all usernames currently stored.
  /// The caller receives a read-only view of the keys from the username dictionary.
  /// </summary>
  /// <returns>
  /// An enumerable sequence of every username in the store.
  /// </returns>
  #endregion
  public IEnumerable<string> GetAllUsernames()
  {
    return usersByUsername.Keys;
  }

  public string AssignNewSessionAuthToken(User user)
  {
    // Remove old token if one exists
    if (!string.IsNullOrWhiteSpace(user.SessionAuthToken))
    {
      usersBySessionAuthToken.Remove(user.SessionAuthToken);
    }

    // Generate new token
    string newToken = Guid.NewGuid().ToString();
    user.SessionAuthToken = newToken;

    // Add mapping
    usersBySessionAuthToken[newToken] = user;

    return newToken;
  }


}
