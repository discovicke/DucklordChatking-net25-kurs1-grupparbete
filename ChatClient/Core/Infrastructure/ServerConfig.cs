using System.Net.Http;
using ChatClient.Core.Application;

namespace ChatClient.Core.Infrastructure;

/// <summary>
/// Responsible for: centralized server configuration and HTTP client factory.
/// Provides a single place to configure the server URL for the entire application.
/// </summary>
public static class ServerConfig
{
    private const string SERVER_URL = "https://ducklord-server.onrender.com/";

    // Alternative URLs for easy switching if server down:
    // private const string SERVER_URL = "http://localhost:5201/";

    private static readonly HttpClient sharedClient = new()
    {
        BaseAddress = new Uri(SERVER_URL)
    };

    public static HttpClient CreateHttpClient()
    {
        // If we already have a token stored, ensure it is attached
        if (!string.IsNullOrWhiteSpace(AppState.SessionAuthToken))
        {
            if (!sharedClient.DefaultRequestHeaders.Contains("SessionAuthToken"))
            {
                sharedClient.DefaultRequestHeaders.Add("SessionAuthToken", AppState.SessionAuthToken);
            }
        }

        return sharedClient;
    }
}

