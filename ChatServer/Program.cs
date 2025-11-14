using ChatServer.Store;
using ChatServer.Hubs;
using Scalar.AspNetCore;
using ChatServer.Configuration;
using ChatServer.Endpoints;

// Stores (in-memory storage)
UserStore userStore = new();
MessageStore messageStore = new(userStore);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR(); // Register the SignalR service
builder.Services.AddCustomOpenApi(); // Register the OpenAPI custom configuration which is found in the OpenApiConfiguration.cs Class
var app = builder.Build();

// Seed initial users (for testing + Scalar UI authentication)
userStore.Add("Ducklord", "chatking", isAdmin: true);
userStore.Add("Scalar", "APIDOCS", isAdmin: true); // This user is used to allow Scalar UI to send test requests

// Print debug token
Console.WriteLine(userStore.GetByUsername("Scalar")?.SessionAuthToken ?? "Token retrieval error: user not found or no token assigned. Ask server admin to generate one");

// Run the server locally on port 5201
builder.WebHost.UseUrls("http://localhost:5201");

// Scalar and OpenAPI are open even in production now to make debugging easier
app.MapOpenApi();                // exposes /openapi/v1.json
app.MapScalarApiReference(opt => // exposes visual UI at /scalar
{
  opt.Title = "Ducklord's Server API Docs";
  opt.Theme = ScalarTheme.DeepSpace;
  opt.HideClientButton = true;
  opt.ExpandAllResponses = true;

  // Automatically pick as the active scheme
  opt.AddPreferredSecuritySchemes("SessionAuth");
  opt.AddApiKeyAuthentication("SessionAuth", apiKey =>
  {
    apiKey.Name = "AuthSessionToken";
    apiKey.Value = userStore.GetByUsername("Scalar")?.SessionAuthToken
                   ?? "Token retrieval error: user not found or no token assigned. Ask server admin to generate one";
  });
});

// Register all API endpoint modules
app.MapAuthEndpoints(userStore);
app.MapUserEndpoints(userStore);
app.MapMessageEndpoints(userStore, messageStore);
app.MapSystemEndpoints();

// Redirect root && /docs → /scalar/ (OpenAPI UI)
app.MapGet("/", () => Results.Redirect("/scalar/", permanent: false)).ExcludeFromApiReference();
app.MapGet("/docs", () => Results.Redirect("/scalar/", permanent: false)).ExcludeFromApiReference();

// Real-time chat endpoint (SignalR hub)
app.MapHub<ChatHub>("/chat");

app.Run();

