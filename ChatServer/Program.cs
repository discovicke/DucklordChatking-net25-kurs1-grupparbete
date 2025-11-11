using Shared;
using ChatServer.Store;
using ChatServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR(); // Register the SignalR service
builder.Services.AddOpenApi(); // Register OpenAPI services
var app = builder.Build();

// Configure the server to listen on all network interfaces on port 5201,
// so other devices on the local network can connect using the server machine's IP.
builder.WebHost.UseUrls("http://0.0.0.0:5201");

// Scalar and OpenAPI are only intended for development testing
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();                // exposes /openapi/v1.json
  app.MapScalarApiReference();     // exposes visual UI at /scalar
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


// Login endpoint
app.MapPost("/login", (UserDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new { Message = "Username and password are required" });
  }

  var user = userStore.GetByUsername(dto.Username);

  if (user != null && user.Password == dto.Password)
  {
    return Results.Ok(new { UserID = user.Id, Message = "Login successful" });
  }

  return Results.BadRequest(new { Message = "Invalid username or password" });
});

// Registration endpoint
app.MapPost("/register", (UserDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new { Message = "Username and password are required" });
  }

  // Attempt to add user
  if (!userStore.Add(dto.Username, dto.Password))
  {
    return Results.BadRequest(new { Message = "Username already exists" });
  }

  var newUser = userStore.GetByUsername(dto.Username);
  if (newUser == null)
  {
    return Results.BadRequest(new { Message = "Failed to add user" });
  }

  return Results.Ok(new { UserID = newUser.Id, Message = "Registration successful" }); // TODO: refactor the Add() in UserStore to return the User object, avoiding the issue here.
});

// Return a list of all usernames (Note: only return the usernames, not the full objects)
app.MapGet("/users", () =>
{
  return Results.Ok(userStore.GetAllUsernames());
});

// Update user endpoint
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
});

// Delete user endpoint
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
});

// Send and stream (SignalR broadcast) message endpoint
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
});

// Get history endpoint (with optional 'take' query parameter)
app.MapGet("/messages/history", (int? take) =>
{
  return take.HasValue
      ? Results.Ok(messageStore.GetLast(take.Value))
      : Results.Ok(messageStore.GetAll());
});

// Map the SignalR ChatHub to the /chat endpoint
app.MapHub<ChatHub>("/chat");

app.Run();
