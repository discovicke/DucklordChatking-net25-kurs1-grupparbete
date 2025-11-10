using System.Net.Http.Json;
using Shared;

namespace ChatClient.Data;

public class LoginUser
{
    //TODO: Kolla så login fungerar med servern

    private readonly HttpClient httpClient;

    // Production construktor
    public LoginUser() : this(new HttpClient { BaseAddress = new Uri("http://localhost:5201") })
    {
    }

    // Test construktor
    public LoginUser(HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        if (this.httpClient.BaseAddress == null)
        {
            // set default base address
            this.httpClient.BaseAddress = new Uri("http://localhost:5201");
        }
    }

    public static void LoginTest()
    {
        var loginScreen = new LoginUser();
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