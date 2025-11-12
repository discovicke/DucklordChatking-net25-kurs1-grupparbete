using Shared;
using ChatServer.Store;
using ChatServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Scalar.AspNetCore;
using static ChatServer.Models.Responses;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR(); // Register the SignalR service
builder.Services.AddOpenApi(options =>
{
  // add Scalar transformers
  options.AddScalarTransformers();

  // add metadata to the OpenAPI document
  options.AddDocumentTransformer((document, context, ct) =>
  {
    document.Info = new()
    {
      Title = "Ducklord Chatking's Super Secure Server API Docs",
      Version = "v0.0.2",
      Description = "Backend for a lightweight chat system. Supports account creation, login, updating and deleting users, " +
    "sending chat messages, retrieving message history, and real-time broadcasting through SignalR."
    };
    return Task.CompletedTask;
  });
});

var app = builder.Build();

// Configure the server to listen on all network interfaces on port 5201,
// so other devices on the local network can connect using the server machine's IP.
builder.WebHost.UseUrls("http://0.0.0.0:5201");

// Scalar and OpenAPI are only intended for development testing
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();                // exposes /openapi/v1.json
  app.MapScalarApiReference(opt => // exposes visual UI at /scalar
{
  opt.Title = "Ducklord's Server API Docs";
  opt.Theme = ScalarTheme.Default;
});
}

// Create a single shared UserStore instance.
// The store contains both dictionaries (by username and by id).
UserStore userStore = new();

// Create a single shared MessageStore instance
// The store contains both a list and dictionary (for ID lookup).
// It requires a reference to the UserStore to validate senders.
MessageStore messageStore = new(userStore);

// Add one user for testing
userStore.Add("Ducklord", "chatking");

#region LOGIN
app.MapPost("/login", (UserDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new ApiFailResponse("Username and password are required."));
  }

  var user = userStore.GetByUsername(dto.Username);

  if (user != null && user.Password == dto.Password)
  {
    return Results.Ok(new ApiSuccessResponseWithUsername(user.Username, "Login successful."));
  }

  return Results.BadRequest(new ApiFailResponse("Invalid username or password."));
})
// API Docs through OpenAPI & ScalarUI
.Produces<ApiSuccessResponseWithUsername>(StatusCodes.Status200OK)
.Produces<ApiFailResponse>(StatusCodes.Status400BadRequest)
.WithSummary("User Login")
.WithDescription("Validates username and password.");
#endregion

#region REGISTER
app.MapPost("/register", (UserDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new ApiFailResponse("Username and password are required"));
  }

  // Attempt to add user
  if (!userStore.Add(dto.Username, dto.Password))
  {
    return Results.BadRequest(new ApiFailResponse("Username already exists."));
  }

  var newUser = userStore.GetByUsername(dto.Username);
  if (newUser == null)
  {
    return Results.BadRequest(new ApiFailResponse("Failed to add user."));
  }

  return Results.Ok(new ApiSuccessResponseWithUsername(newUser.Username, "Registration successful."));
})
// API Docs through OpenAPI & ScalarUI
.Produces<ApiSuccessResponseWithUsername>(StatusCodes.Status200OK)
.Produces<ApiFailResponse>(StatusCodes.Status400BadRequest)
.WithSummary("Register User Account")
.WithDescription("Creates a new user account using the provided `username` and `password`. The server stores the account and returns the assigned user ID on success.");
#endregion

#region LIST USERS
app.MapGet("/users", () =>
{
  var usernames = userStore.GetAllUsernames();

  // Validation and error handling
  if (usernames == null)
  {
    return Results.BadRequest(
        new ApiFailResponse("List of usernames could not be retrieved.")
    );
  }

  if (!usernames.Any())
  {
    return Results.BadRequest(
        new ApiFailResponse("No users in list.")
    );
  }

  return Results.Ok(
      new ApiSuccessResponseWithUsernames(usernames)
  );
})
.Produces<ApiSuccessResponseWithUsernames>(StatusCodes.Status200OK)
.Produces<ApiFailResponse>(StatusCodes.Status400BadRequest)
.WithSummary("List All Usernames")
.WithDescription("Returns every registered username. If no users exist, returns an error.");
#endregion

#region UPDATE USER CREDENTIALS
app.MapPost("/user/update", (UpdateUserDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.OldUsername) || string.IsNullOrWhiteSpace(dto.NewUsername))
  {
    return Results.BadRequest(new ApiFailResponse("Old and new username are required."));
  }

  var updated = userStore.Update(dto.OldUsername, dto.NewUsername, dto.Password);

  if (!updated)
  {
    return Results.BadRequest(new ApiFailResponse(
        "Update failed. The old username might not exist or the new username is already taken."
    ));
  }

  return Results.Ok(new ApiSuccessResponseWithUsername(dto.NewUsername, "User updated successfully."));
})
// API Docs through OpenAPI & ScalarUI
.Produces<ApiSuccessResponseWithUsername>(StatusCodes.Status200OK)
.Produces<ApiFailResponse>(StatusCodes.Status400BadRequest)
.WithSummary("Update User Account")
.WithDescription("Changes a user's account information. The request must include the current `OldUsername` and the desired `NewUsername`. If a `Password` is provided, it replaces the existing password. If `Password` is omitted, the existing password stays the same.");
#endregion

#region DELETE USER
app.MapPost("/user/delete", (UserDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username))
  {
    return Results.BadRequest(new ApiFailResponse("Username is required."));
  }
  if (string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new ApiFailResponse("Password is required."));
  }

  // Delete logic
  var deleted = userStore.Remove(dto.Username);
  if (!deleted)
  {
    return Results.BadRequest(new ApiFailResponse("User not found or could not be deleted."));
  }

  return Results.Ok(new ApiSuccessResponse("User deleted successfully."));

})
// API Docs through OpenAPI & ScalarUI
.Produces<ApiSuccessResponse>(StatusCodes.Status200OK)
.Produces<ApiFailResponse>(StatusCodes.Status400BadRequest)
.WithSummary("Delete User Account")
.WithDescription("Deletes a user account based on the provided `username` and `password`. If the credentials match a stored account, the user is removed from the server and can no longer log in.");
#endregion

#region SEND MESSAGE
app.MapPost("/send-message", async (MessageDTO dto, IHubContext<ChatHub> hub) =>
{
  // Validate basic input
  if (string.IsNullOrWhiteSpace(dto.Content))
    return Results.BadRequest(new ApiFailResponse("Message content cannot be empty."));

  // Look up the user
  if (string.IsNullOrWhiteSpace(dto.Sender))
    return Results.BadRequest(new ApiFailResponse("Sender cannot be empty."));

  // Add message to store (history and lookup)
  var added = messageStore.Add(dto.Sender, dto.Content);
  if (!added)
    return Results.BadRequest(new ApiFailResponse("Failed to store message.")); // Unlikely to occur unless sender validation fails, but included for safety.

  // Broadcast to all SignalR Clients
  await hub.Clients.All.SendAsync("ReceiveMessage", dto.Sender, dto.Content);


  return Results.Ok(new ApiSuccessResponse("Message stored and broadcasted."));
})
// API Docs through OpenAPI & ScalarUI
.Produces<ApiSuccessResponse>(StatusCodes.Status200OK)
.Produces<ApiFailResponse>(StatusCodes.Status400BadRequest)
.WithSummary("Send Message")
.WithDescription("Sends a chat message through HTTP and broadcasts it to all connected SignalR clients via the `ReceiveMessage` hub method. The message is saved to the server history and becomes available through `/messages/history`.");
#endregion

#region GET MESSAGE HISTORY (WITH OPTIONAL TAKE PARAMETER)
app.MapGet("/messages/history", (int? take) =>
{

  // If take is provided, it must be a positive number
  if (take.HasValue && take.Value <= 0)
  {
    return Results.BadRequest(new ApiFailResponse("Query parameter 'take' must be greater than 0."));
  }

  var messages = take.HasValue
        ? messageStore.GetLast(take.Value)
        : messageStore.GetAll();

  return Results.Ok(messages);
})
// API Docs through OpenAPI & ScalarUI
.Produces<ApiSuccessResponseWithMessages>(StatusCodes.Status200OK)
.WithSummary("Get Message History")
.WithDescription("Returns chat messages in chronological order (oldest to newest). If the optional `take` query parameter is used, the server selects the newest messages first and then returns them in chronological order. For example, `GET /messages/history?take=10` returns the 10 most recent messages, ordered from oldest to newest.");
#endregion

#region CLEAR MESSAGE HISTORY
app.MapPost("/messages/clear", () =>
{
  // Attempt to clear all stored messages
  var cleared = messageStore.ClearAll();

  if (!cleared)
  {
    return Results.BadRequest(
      new ApiFailResponse("Message history could not be cleared. The store may be uninitialized.")
    );
  }

  return Results.Ok(
    new ApiSuccessResponse("All messages have been successfully cleared.")
  );
})
// API Docs through OpenAPI & ScalarUI
.WithBadge("Danger Zone", BadgePosition.Before, "#ff3b30")
.Produces<ApiSuccessResponse>(StatusCodes.Status200OK)
.Produces<ApiFailResponse>(StatusCodes.Status400BadRequest)
.WithSummary("Clear Message History")
.WithDescription("Deletes all stored chat messages from the server's history. This action cannot be undone and affects all users.");
#endregion

#region HEALTH CHECK
app.MapGet("health", () => Results.Ok("OK"))
.WithMetadata(new HttpMethodMetadata(["HEAD"]))
.WithSummary("Health Check")
.WithDescription("Simple endpoint for uptime monitoring. Returns `OK` if the server is running. Supports GET And HEAD requests as well.")
.WithMetadata(new HttpMethodMetadata(["HEAD"]));
#endregion

// Map the SignalR ChatHub to the /chat endpoint
app.MapHub<ChatHub>("/chat");

app.Run();

