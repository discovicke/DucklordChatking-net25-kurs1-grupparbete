using ChatServer.Store;
using Shared;
using ChatServer.Logger;

namespace ChatServer.Endpoints;

public static class AuthEndpoints
{
  public static RouteGroupBuilder MapAuthEndpoints(
      this IEndpointRouteBuilder app,
      UserStore userStore)
  {
    var auth = app.MapGroup("/auth").WithTags("Authentication");

    #region LOGIN
    auth.MapPost("/login", (UserDTO dto) =>
    {
      ServerLog.Info($"Login attempt for '{dto.Username}'");

      // 400: invalid request shape
      if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
      {
        ServerLog.Warning("Login failed: missing username or password");
        return Results.BadRequest();
      }

      var user = userStore.GetByUsername(dto.Username);

      // 401: credentials invalid
      if (user == null || user.Password != dto.Password)
      {
        ServerLog.Warning($"Login failed (invalid credentials) for '{dto.Username}'");
        return Results.Unauthorized();
      }

      // Update the auth token
      var token = userStore.AssignNewSessionAuthToken(user);

      ServerLog.Success($"User '{dto.Username}' logged in");

      var response = new LoginResponseDTO
      {
        Username = user.Username,
        Token = token
      };

      // 200: success
      return Results.Ok(response);
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
      ServerLog.Info($"Registration attempt for '{dto.Username}'");

      // 400: invalid request shape
      if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
      {
        ServerLog.Warning("Registration failed: missing username or password");
        return Results.BadRequest();
      }

      // 409: username already exists
      if (!userStore.Add(dto.Username, dto.Password))
      {
        ServerLog.Warning($"Registration failed: username '{dto.Username}' already exists");
        return Results.Conflict();
      }

      // Just in case something unexpected happened
      var newUser = userStore.GetByUsername(dto.Username);
      if (newUser == null)
      {
        ServerLog.Error($"User '{dto.Username}' was added but could not be retrieved immediately after creation");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
      }

      ServerLog.Success($"User '{dto.Username}' registered");

      // 201: created, return the username
      return Results.Created("", newUser.Username);
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

    return auth;
  }
}
