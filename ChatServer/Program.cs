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
  // add metadata to the OpenAPI document
  options.AddDocumentTransformer((document, context, ct) =>
  {
    document.Info = new()
    {
      Title = "Ducklord Chatking's Super Secure Server API Docs",
      Version = "v0.0.0.1",
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


app.MapGet("/users", () =>
{
  return Results.Ok(userStore.GetAllUsernames());
})
// API Docs through OpenAPI & ScalarUI
.WithSummary("List All Usernames")
.WithDescription("Returns every registered username as a simple list of strings. The response does not include passwords or any other account information.");
// TODO: Implement .Produces


app.MapPost("/user/update", (UpdateUserDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.OldUsername) || string.IsNullOrWhiteSpace(dto.NewUsername))
  {
    return Results.BadRequest(new { Message = "Old and new username are required." });
  }

  var updated = userStore.Update(dto.OldUsername, dto.NewUsername, dto.Password);
  if (!updated)
  {
    return Results.BadRequest(new { Message = "Failed to update user. Old username may not exist or new username already taken." }); // TODO: refine error messages in UserStore.Update to give more specific feedback, that is, if the old username does not exist, or if the new username is already taken specifically.
  }

  return Results.Ok(new { UpdatedUsername = dto.NewUsername, Message = "User updated successfully" });
})
// API Docs through OpenAPI & ScalarUI
.WithSummary("Update User Account")
.WithDescription("Changes a user's account information. The request must include the current `OldUsername` and the desired `NewUsername`. If a `Password` is provided, it replaces the existing password. If `Password` is omitted, the existing password stays the same.");
// TODO: Implement .Produces


app.MapPost("/user/delete", (UserDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username))
  {
    return Results.BadRequest(new { Message = "Username is required." });
  }
  if (string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new { Message = "Password is required." });
  }

  // Delete logic
  var deleted = userStore.Remove(dto.Username);
  if (!deleted)
  {
    return Results.BadRequest(new { Message = "User not found or could not be deleted." });
  }

  return Results.Ok(new { Message = "User deleted successfully." });

})
// API Docs through OpenAPI & ScalarUI
.WithSummary("Delete User Account")
.WithDescription("Deletes a user account based on the provided `username` and `password`. If the credentials match a stored account, the user is removed from the server and can no longer log in.");
// TODO: Implement .Produces


app.MapPost("/send-message", async (MessageDTO dto, IHubContext<ChatHub> hub) =>
{
  // Validate basic input
  if (string.IsNullOrWhiteSpace(dto.Content))
    return Results.BadRequest(new { Message = "Message content cannot be empty" });

  // Look up the user
  if (string.IsNullOrWhiteSpace(dto.Sender))
    return Results.BadRequest(new { Message = "Sender cannot be empty" });

  // Add message to store (history and lookup)
  var added = messageStore.Add(dto.Sender, dto.Content);
  if (!added)
    return Results.BadRequest(new { Message = "Failed to add message" }); // unlikely with current store, but safe

  // Broadcast to all SignalR Clients
  await hub.Clients.All.SendAsync("ReceiveMessage", dto.Sender, dto.Content);


  return Results.Ok(new { Message = "Message stored" });
})
// API Docs through OpenAPI & ScalarUI
.WithSummary("Send Message")
.WithDescription("Sends a chat message through HTTP and broadcasts it to all connected SignalR clients via the `ReceiveMessage` hub method. The message is saved to the server history and becomes available through `/messages/history`.");
// TODO: Implement .Produces


app.MapGet("/messages/history", (int? take) =>
{
  return take.HasValue
      ? Results.Ok(messageStore.GetLast(take.Value))
      : Results.Ok(messageStore.GetAll());
})
// API Docs through OpenAPI & ScalarUI
.WithSummary("Get Message History")
.WithDescription("Returns chat messages in chronological order (oldest to newest). If the optional `take` query parameter is used, the server selects the newest messages first and then returns them in chronological order. For example, `GET /messages/history?take=10` returns the 10 most recent messages, ordered from oldest to newest.");
// TODO: Implement .Produces




// Map the SignalR ChatHub to the /chat endpoint
app.MapHub<ChatHub>("/chat");

app.Run();

