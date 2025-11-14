using System.Net.Http;

namespace ChatClient.Core;

// Centralized server configuration and HTTP client factory.
// Change the server URL in one place for the entire application.

public static class ServerConfig
{
    private const string SERVER_URL = "https://ducklord-server.onrender.com/";
    
    // Alternative URLs for easy switching if server down:
    // private const string SERVER_URL = "http://localhost:5201/";
    
    // Get the configured server base URL
    public static Uri BaseUrl => new Uri(SERVER_URL);
    
    // Create a new HttpClient configured with the server base URL
    public static HttpClient CreateHttpClient()
    {
        return new HttpClient { BaseAddress = BaseUrl };
    }
}

