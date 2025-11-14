using System.Net.Http.Json;
using ChatClient.Core;
using Shared;

namespace ChatClient.Data;

public class UserAuth
{
    private readonly HttpClient httpClient;

    // Production constructor - uses global server config
    public UserAuth() : this(ServerConfig.CreateHttpClient())
    {
    }

    // Test constructor - allows custom HttpClient for testing
    public UserAuth(HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public bool Login(string username, string password)
    {
        var userDto = new UserDTO { Username = username, Password = password };

        try
        {
            var response = httpClient.PostAsJsonAsync("/auth/login", userDto).Result;
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public bool Register(string username, string password)
    {
        var userDto = new UserDTO { Username = username, Password = password };

        try
        {
            var response = httpClient.PostAsJsonAsync("/auth/register", userDto).Result;
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