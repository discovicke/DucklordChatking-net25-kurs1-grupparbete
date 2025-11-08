namespace ChatServer.Store;

public class UserStore
{
  // Dictionaries for storing and looking up users
  private readonly Dictionary<string, User> usersByUsername = [];
  private readonly Dictionary<int, User> usersById = [];

  // User ID counter
  private int nextId = 1;

  // ADD USER
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

  // REMOVE USER
  public bool Remove(string username)
  {
    // TODO: Implement logic
    return false;
  }

  public bool Remove(int id)
  {
    // TODO: Implement logic
    return false;
  }

  // UPDATE USER
  public bool Update(string oldUsername, string newUsername, string? newPassword = null)
  {
    // TODO: Implement logic
    return false;
  }

  // GET USER
  public User? GetById(int id)
  {
    // TODO: Implement logic
    return null;
  }

  public User? GetByUsername(string username)
  {
    // TODO: Implement logic
    return null;
  }

  // GET ALL USERNAMES
  public IEnumerable<string> GetAllUsernames()
  {
    // TODO: Implement logic
    return [];
  }
}
