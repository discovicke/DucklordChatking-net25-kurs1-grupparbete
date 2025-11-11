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

    public bool Login(string username, string password)
    {
        var userDto = new UserDTO { Username = username, Password = password };

        try
        {
            var response = httpClient.PostAsJsonAsync("/login", userDto).Result;
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    // New overload to allow tests to pass a DTO instance directly
    public bool Login(UserDTO user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        return Login(user.Username ?? string.Empty, user.Password ?? string.Empty);
    }
}