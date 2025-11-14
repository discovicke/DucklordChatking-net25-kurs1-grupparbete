using ChatServer.Store;
using ChatServer.Auth;
using Shared;

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
