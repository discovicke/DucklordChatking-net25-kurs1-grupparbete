using Shared;
using ChatServer.Store;
using ChatServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Scalar.AspNetCore;
using ChatServer.Auth;


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

// Redirect root && /docs → /scalar/
app.MapGet("/", () => Results.Redirect("/scalar/", permanent: false)).ExcludeFromApiReference();
app.MapGet("/docs", () => Results.Redirect("/scalar/", permanent: false)).ExcludeFromApiReference();

// Configure the server to listen on all network interfaces on port 5201,
// so other devices on the local network can connect using the server machine's IP.
builder.WebHost.UseUrls("http://0.0.0.0:5201");

// Scalar and OpenAPI are open even in production now to make debugging easier
app.MapOpenApi();                // exposes /openapi/v1.json
app.MapScalarApiReference(opt => // exposes visual UI at /scalar
{
  opt.Title = "Ducklord's Server API Docs";
  opt.Theme = ScalarTheme.Default;
});

// Stores
UserStore userStore = new();
MessageStore messageStore = new(userStore);

// Add one user for testing
userStore.Add("Ducklord", "chatking", isAdmin: true);

// Endpoint grouping
var auth = app.MapGroup("/auth").WithTags("Authentication");
var users = app.MapGroup("/users").WithTags("Users");
var messages = app.MapGroup("/messages").WithTags("Messages");
var system = app.MapGroup("/system").WithTags("System");


// ENDPOINTS
#region LOGIN
auth.MapPost("/login", (UserDTO dto) =>
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

  // Update the auth token
  user.SessionAuthToken = Guid.NewGuid().ToString();

  // 200: success
  return Results.Ok(user.SessionAuthToken);
})
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized)
.WithSummary("User Login")
.WithDescription("Validates username and password. On success, creates a new session Auth-token for the user and returns it in the response body of status `200`. " +
"Returns `401` when the credentials are incorrect and `400` when the request is missing a username or password. " +
"The session token functions as proof that the caller has authenticated and must be included in subsequent requests that require access control.");
#endregion

#region REGISTER
auth.MapPost("/register", (UserDTO dto) =>
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
  return Results.Created("/auth/register", newUser.Username);
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
users.MapGet("", (HttpContext context) =>
{
  // 401: authentication required
  if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var user))
    return Results.Unauthorized();

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
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status500InternalServerError)
.WithSummary("List All Usernames")
.WithDescription(
    "Provides the complete list of registered usernames for authenticated callers. " +
    "A successful lookup yields a `200` response with the usernames, or `204` when the store is empty. " +
    "Unauthenticated requests receive `401`, and any internal retrieval failure results in `500`."
)
.WithBadge("🔐 Auth Required", BadgePosition.Before, "#ffd966");
#endregion

#region UPDATE USER CREDENTIALS
users.MapPost("/update", (HttpContext context, UpdateUserDTO dto) =>
{

  // 401: authentication required
  if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller) || caller == null)
    return Results.Unauthorized();

  // 400: invalid input
  if (string.IsNullOrWhiteSpace(dto.OldUsername) || string.IsNullOrWhiteSpace(dto.NewUsername))
  {
    return Results.BadRequest();
  }

  // 403: authorization (self or admin)
  if (!AuthRules.IsSelfOrAdmin(caller, dto.OldUsername))
    return Results.Forbid();

  // Attempt update
  var updated = userStore.Update(dto.OldUsername, dto.NewUsername, dto.Password);


  // TODO: break this up so what the conflict issue is becomes clear
  if (!updated)
  {
    return Results.Conflict(); // 409: conflict (old username missing or new one already taken)
  }

  // 204: update succeeded, no body needed
  return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status409Conflict)
.WithSummary("Update User Account")
.WithDescription(
    "Allows authenticated users to update their own account details. Administrators may update any account. " +
    "Successful updates return `204`. Conflicts in username availability yield `409`, " +
    "and callers without the right permissions receive `403`."
)
.WithBadge("🔐 Auth Required", BadgePosition.Before, "#ffd966");
#endregion

#region DELETE USER
users.MapPost("/delete", (UserDTO dto) =>
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
messages.MapPost("/send", async (MessageDTO dto, IHubContext<ChatHub> hub) =>
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
messages.MapGet("/history", (int? take) =>
{
  // 400: invalid query parameter
  if (take.HasValue && take.Value <= 0)
  {
    return Results.BadRequest();
  }

  var messages = take.HasValue
      ? messageStore.GetLast(take.Value)
      : messageStore.GetAll();

  // 200: always OK, returns empty list if no messages exist
  return Results.Ok(messages);
})
.Produces<IEnumerable<MessageDTO>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithSummary("Get Message History")
.WithDescription(
    "Returns `200` with the list of stored messages as content. " +
    "Returns `400` when the `take` query parameter is present but not greater than zero. " +
    "If `take` is omitted, the entire message history is returned. " +
    "If `take` is provided, the server selects the newest messages first and returns them in chronological order."
);
#endregion

#region CLEAR MESSAGE HISTORY
messages.MapPost("/clear", () =>
{
  // Attempt to clear all stored messages
  var cleared = messageStore.ClearAll();

  // 500: unexpected failure
  if (!cleared)
  {
    return Results.StatusCode(StatusCodes.Status500InternalServerError);
  }

  // 204: success, no content
  return Results.NoContent();
})
.WithBadge("Danger Zone 💣", BadgePosition.Before, "#ff3b30")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status500InternalServerError)
.WithSummary("Clear Message History")
.WithDescription(
    "Clears all stored chat messages. Returns `204` when the message history is successfully cleared. " +
    "Returns `500` when the server is unable to clear the message store."
);
#endregion

#region HEALTH CHECK
system.MapGet("/health", () => Results.Ok("OK"))
    .WithMetadata(new HttpMethodMetadata(["HEAD"]))
    .WithSummary("Health Check")
    .WithDescription(
        "Provides a simple server health indicator. Returns `200` with the content `OK` when the server is operational. " +
        "Supports both GET and HEAD requests for uptime monitoring."
    )
    .WithBadge("🩺💚", BadgePosition.After, "#e5e5e5")
    .WithBadge("⚙️ UptimeRobot", BadgePosition.After, "#51ff94");
#endregion

// Map the SignalR ChatHub to the /chat endpoint
app.MapHub<ChatHub>("/chat");

app.Run();

