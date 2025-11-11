using System.Net.Http.Json;
using Shared;

namespace ChatClient.Data;

public class LoginUser
{
    //TODO: Kolla så login fungerar med servern
    
    //TODO: Change LoginUser to more abstract user class

    private readonly HttpClient httpClient;

    // Production construktor
    public LoginUser() : this(new HttpClient { BaseAddress = new Uri("http://localhost:5201") })
    { }

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