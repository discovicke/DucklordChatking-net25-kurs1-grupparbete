using System.Net.Http.Json;
using Shared;

namespace ChatClient;

public class LoginScreen
{
    //TODO: Kolla så login fungerar med servern
    private static readonly HttpClient httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5000")
    };

    public static void LoginTest()
    {
        var loginScreen = new LoginScreen();
        bool loginSuccess = loginScreen.Login("Ducklord", "chatking"); //TODO: Läsa av input från användaren och skicka till Login-metoden
        if (!loginSuccess)
        {
            Console.WriteLine("Login failed");
        }
        else
        {
            Console.WriteLine($"Login successful: {loginSuccess}");
        }
    }
    
    public bool Login(string username, string password)
    {
        var loginDto = new LoginDTO
        {
            Username = username,
            Password = password
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