namespace ChatClient.Data;

public class UserAccount
{
    public static string? Username { get; private set; } = "Ducklord";
    public static string? Password { get; private set; } = "Chatking";

    public static bool IsLoggedIn => !string.IsNullOrEmpty(Username);

    public static void SetUser(string username, string? password = null)
    {
        Username = username;
        Password = password;
    }

    public static void Clear()
    {
        Username = null;
        Password = null;
    }
}