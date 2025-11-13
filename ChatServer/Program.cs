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
  // 400: invalid request shape
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest();
  }

  var user = userStore.GetByUsername(dto.Username);

  // 401: credentials invalid
  if (user == null || user.Password != dto.Password)
  {
    return Results.Unauthorized();
  }

  // 200: success
  return Results.Ok();
})
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized)
.WithSummary("User Login")
.WithDescription("Validates username and password and returns 401 if credentials are invalid, or 400 for invalid input.");
#endregion

#region REGISTER
app.MapPost("/register", (UserDTO dto) =>
{
  // 400: invalid request shape
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest();
  }

  // 409: username already exists
  if (!userStore.Add(dto.Username, dto.Password))
  {
    return Results.Conflict();
  }

  // Just in case something unexpected happened
  var newUser = userStore.GetByUsername(dto.Username);
  if (newUser == null)
  {
    return Results.StatusCode(StatusCodes.Status500InternalServerError);
  }

  // 201: created, return the username
  return Results.Created("/register", newUser.Username);
})
.Produces(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status409Conflict)
.Produces(StatusCodes.Status500InternalServerError)
.WithSummary("Register User Account")
.WithDescription(
    "Creates a new user account. Returns `201` with the username as content when the account is successfully created. " +
    "Returns `409` when the provided username already exists. Returns `400` when the request content is missing a username or password."
);
#endregion

#region LIST USERS
app.MapGet("/users", () =>
{
  var usernames = userStore.GetAllUsernames();

  // 500: unexpected storage failure
  if (usernames == null)
  {
    return Results.StatusCode(StatusCodes.Status500InternalServerError);
  }

  // 204: no users
  if (!usernames.Any())
  {
    return Results.NoContent();
  }

  // 200: return list of usernames
  return Results.Ok(usernames);
})
.Produces<IEnumerable<string>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status500InternalServerError)
.WithSummary("List All Usernames")
.WithDescription(
    "Returns `200` with a list of usernames when the list contains users. " +
    "Returns `204` when the list is empty. Returns `500` when the list of usernames cannot be retrieved (would be a server-side issue)."
);
#endregion

#region UPDATE USER CREDENTIALS
app.MapPost("/user/update", (UpdateUserDTO dto) =>
{
  // 400: invalid input
  if (string.IsNullOrWhiteSpace(dto.OldUsername) || string.IsNullOrWhiteSpace(dto.NewUsername))
  {
    return Results.BadRequest();
  }

  // Attempt update
  var updated = userStore.Update(dto.OldUsername, dto.NewUsername, dto.Password);

  // 409: conflict (username exists OR old username missing)
  // TODO: break this up so what the conflict issue is becomes clear, that is if it is "Username exists" or "Old username missing"
  if (!updated)
  {
    return Results.Conflict();
  }

  // 200: success: return the new username as content
  return Results.Ok(dto.NewUsername);
})
.Produces<string>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status409Conflict)
.WithSummary("Update User Account")
.WithDescription(
    "Updates a user's account. Returns `200` with the new username as content when the update succeeds. " +
    "Returns `409` when the old username does not exist or the new username is already taken. " +
    "Returns `400` when the request content is missing the old or new username."
);
#endregion

#region DELETE USER
app.MapPost("/user/delete", (UserDTO dto) =>
{
  // 400: invalid input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest();
  }

  // 401: Verify credentials before deletion
  var user = userStore.GetByUsername(dto.Username);
  if (user == null || user.Password != dto.Password)
  {
    return Results.Unauthorized();
  }

  // Attempt deletion
  var deleted = userStore.Remove(dto.Username);

  // 500: unexpected failure
  if (!deleted)
  {
    return Results.StatusCode(StatusCodes.Status500InternalServerError);
  }

  // 204: deletion successful, no content needed
  return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status500InternalServerError)
.WithSummary("Delete User Account")
.WithDescription(
    "Deletes a user account. Returns `204` when the account is successfully deleted. " +
    "Returns `401` when the provided credentials do not match any stored user. " +
    "Returns `400` when the request content is missing a username or password. " +
    "Returns `500` when the user could not be deleted after successful credential verification."
);
#endregion

#region SEND MESSAGE
app.MapPost("/send-message", async (MessageDTO dto, IHubContext<ChatHub> hub) =>
{
  // 400: missing sender or content
  if (string.IsNullOrWhiteSpace(dto.Content) || string.IsNullOrWhiteSpace(dto.Sender))
  {
    return Results.BadRequest();
  }

  // Attempt to store the message
  var added = messageStore.Add(dto.Sender, dto.Content);

  // 500: something unexpected went wrong storing the message
  if (!added)
  {
    return Results.StatusCode(StatusCodes.Status500InternalServerError);
  }

  // Broadcast to all SignalR clients
  await hub.Clients.All.SendAsync("ReceiveMessage", dto.Sender, dto.Content);

  // 204: message stored & broadcasted, no response body needed
  return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError)
.WithSummary("Send Message")
.WithDescription(
    "Stores a chat message and broadcasts it to all connected SignalR clients. " +
    "Returns `204` when the message is successfully stored and broadcasted. " +
    "Returns `400` when the request content lacks a sender or message text. " +
    "Returns `500` when the message cannot be stored."
);
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
.WithDescription("Simple endpoint for uptime monitoring. Returns `OK` if the server is running. Supports both HEAD and GET requests.")
.WithMetadata(new HttpMethodMetadata(["HEAD"]))
.WithBadge("👩🏻‍⚕️💚", BadgePosition.After, "#e5e5e5")
.WithBadge("⚙️ UptimeRobot", BadgePosition.After, "#51ff94");



#endregion

// Map the SignalR ChatHub to the /chat endpoint
app.MapHub<ChatHub>("/chat");

app.Run();

