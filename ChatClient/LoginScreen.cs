using System.Net.Http.Json;
using Shared;

namespace ChatClient;

public class LoginScreen
{
    //TODO: Kolla så login fungerar med servern
    private static readonly HttpClient httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5201")
    };

    public static void LoginTest()
    {
        var loginScreen = new LoginScreen();
        var user = new UserAccount();
        user.Username = "Ducklord";
        user.Password = "chatking";
        bool loginSuccess = loginScreen.Login(user); //TODO: Läsa av input från användaren och skicka till Login-metoden
        if (!loginSuccess)
        {
            Console.WriteLine("Login failed");
        }
        else
        {
            Console.WriteLine($"Login successful: {loginSuccess}");
        }
    }
    
    public bool Login(UserAccount user)
    {
        var loginDto = new LoginDTO
        {
            Username = user.Username,
            Password = user.Password
        };

        try
        {
            var response = httpClient.PostAsJsonAsync("/login", loginDto).Result;
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}