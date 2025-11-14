using Shared;
using ChatServer.Store;
using ChatServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Scalar.AspNetCore;
using ChatServer.Auth;
using ChatServer.Configuration;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR(); // Register the SignalR service
builder.Services.AddCustomOpenApi(); // Register the OpenAPI custom configuration which is found in the OpenApiConfiguration.cs Class

var app = builder.Build();

// Stores
UserStore userStore = new();
MessageStore messageStore = new(userStore);

// Add one user for general testing
userStore.Add("Ducklord", "chatking", isAdmin: true);

// This user is used to allow Scalar UI to send test requests
userStore.Add("Scalar", "APIDOCS", isAdmin: true);
Console.WriteLine(userStore.GetByUsername("Scalar")?.SessionAuthToken ?? "Token retrieval error: user not found or no token assigned. Ask server admin to generate one"); // Debug token print

// Redirect root && /docs → /scalar/
app.MapGet("/", () => Results.Redirect("/scalar/", permanent: false)).ExcludeFromApiReference();
app.MapGet("/docs", () => Results.Redirect("/scalar/", permanent: false)).ExcludeFromApiReference();

// Endpoint grouping
var auth = app.MapGroup("/auth").WithTags("Authentication");
var users = app.MapGroup("/users").WithTags("Users");
var messages = app.MapGroup("/messages").WithTags("Messages");
var system = app.MapGroup("/system").WithTags("System");

// Configure the server to listen on all network interfaces on port 5201,
// so other devices on the local network can connect using the server machine's IP.
builder.WebHost.UseUrls("http://0.0.0.0:5201");

// Scalar and OpenAPI are open even in production now to make debugging easier
app.MapOpenApi();                // exposes /openapi/v1.json
app.MapScalarApiReference(opt => // exposes visual UI at /scalar
{
  opt.Title = "Ducklord's Server API Docs";
  opt.Theme = ScalarTheme.Default;

  // Automatically pick as the active scheme
  opt.AddPreferredSecuritySchemes("SessionAuth");
  opt.AddApiKeyAuthentication("SessionAuth", apiKey =>
  {
    apiKey.Name = "AuthSessionToken";
    apiKey.Value = userStore.GetByUsername("Scalar")?.SessionAuthToken
                   ?? "Token retrieval error: user not found or no token assigned. Ask server admin to generate one";
  });
});


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
  var token = userStore.AssignNewSessionAuthToken(user);

  // 200: success
  return Results.Ok(token);
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
.WithBadge("Auth Required 🔐", BadgePosition.Before, "#ffec72");
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
    return Results.StatusCode(StatusCodes.Status403Forbidden);

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
.WithBadge("Auth Required 🔐", BadgePosition.Before, "#ffec72");
#endregion

#region DELETE USER
users.MapPost("/delete", (HttpContext context, UserDTO dto) =>
{
  // 401: authentication required
  if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller) || caller == null)
    return Results.Unauthorized();

  // 400: invalid input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest();
  }

  // 401: Verify credentials before deletion
  // Lookup target user
  var targetUser = userStore.GetByUsername(dto.Username);
  if (targetUser == null || targetUser.Password != dto.Password)
  {
    return Results.Unauthorized(); // 401: wrong credentials or no such user
  }

  // 403: authorization (self or admin)
  if (!AuthRules.IsSelfOrAdmin(caller, dto.Username))
    return Results.StatusCode(StatusCodes.Status403Forbidden);

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
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status500InternalServerError)
.WithSummary("Delete User Account")
.WithDescription(
    "Authenticated users may delete their own account, while administrators may remove any account. " +
    "A successful deletion returns `204`. Invalid credentials result in `401`, insufficient permissions in `403`, " +
    "and unexpected storage failures in `500`."
)
.WithBadge("Auth Required 🔐", BadgePosition.Before, "#ffec72");
#endregion

#region SEND MESSAGE
messages.MapPost("/send", async (HttpContext context, MessageDTO dto, IHubContext<ChatHub> hub) =>
{
  // 401: authentication required
  if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller) || caller == null)
    return Results.Unauthorized();

  // 400: missing sender or content
  if (string.IsNullOrWhiteSpace(dto.Content) || string.IsNullOrWhiteSpace(dto.Sender))
  {
    return Results.BadRequest();
  }

  // 403: sender must match authenticated user
  if (!AuthRules.IsSelf(caller, dto.Sender))
  {
    return Results.StatusCode(StatusCodes.Status403Forbidden);
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
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status500InternalServerError)
.WithSummary("Send Message")
.WithDescription(
    "Handles the submission of a new chat message from an authenticated caller. " +
    "A valid request stores the message, broadcasts it to all connected clients, and concludes with `204`. " +
    "Missing fields lead to `400`, unauthenticated attempts receive `401`, and a sender mismatch produces `403`. " +
    "Unexpected storage issues return `500`."
)
.WithBadge("Auth Required 🔐", BadgePosition.Before, "#ffec72");
#endregion

#region GET MESSAGE HISTORY (WITH OPTIONAL TAKE PARAMETER)
messages.MapGet("/history", (HttpContext context, int? take) =>
{
  // 401: authentication required
  if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller) || caller == null)
  {
    return Results.Unauthorized();
  }

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
.Produces(StatusCodes.Status401Unauthorized)
.WithSummary("Get Message History")
.WithDescription(
   "Provides access to the stored chat history for authenticated callers. " +
   "A successful retrieval returns `200` with the messages, while invalid query values result in `400`. " +
   "Unauthenticated requests receive `401`."
)
.WithBadge("Auth Required 🔐", BadgePosition.Before, "#ffec72");
#endregion

#region CLEAR MESSAGE HISTORY
messages.MapPost("/clear", (HttpContext context) =>
{
  // 401: authentication required
  if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller) || caller == null)
  {
    return Results.Unauthorized();
  }

  // 403: authorization, admin-only operation
  if (!caller.IsAdmin)
    return Results.StatusCode(StatusCodes.Status403Forbidden);

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
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status500InternalServerError)
.WithSummary("Clear Message History")
.WithDescription(
    "Removes all stored chat messages. Available only to authenticated administrators. " +
    "A successful operation returns `204`. Unauthenticated attempts receive `401`, " +
    "while callers lacking administrative privileges receive `403`. " +
    "Unexpected storage failures result in `500`."
)
.WithBadge("Admin Only 🔐", BadgePosition.Before, "#707fff");
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

