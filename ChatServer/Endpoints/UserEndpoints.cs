using ChatServer.Store;
using ChatServer.Auth;
using Shared;
using Scalar.AspNetCore;

namespace ChatServer.Endpoints;

public static class UserEndpoints
{
  public static RouteGroupBuilder MapUserEndpoints(
      this IEndpointRouteBuilder app,
      UserStore userStore)
  {
    var users = app.MapGroup("/users").WithTags("Users");


    #region LIST USERS
    users.MapGet("", (HttpContext context) =>
    {
      // 401: authentication required
      if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var user))
        return Results.Unauthorized();

      var usernames = userStore.GetAllUsernames();

      // 500: unexpected storage failure
      if (usernames == null)
        return Results.StatusCode(StatusCodes.Status500InternalServerError);

      // 204: no users
      if (!usernames.Any())
        return Results.NoContent();

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
        return Results.BadRequest();

      // 403: authorization (self or admin)
      if (!AuthRules.IsSelfOrAdmin(caller, dto.OldUsername))
        return Results.StatusCode(StatusCodes.Status403Forbidden);

      // Attempt update
      var updated = userStore.Update(dto.OldUsername, dto.NewUsername, dto.Password);


      // TODO: break this up so what the conflict issue is becomes clear
      if (!updated)
        return Results.Conflict(); // 409: conflict (old username missing or new one already taken)

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

      // Lookup target user
      var targetUser = userStore.GetByUsername(dto.Username);

      // 401: Verify credentials before deletion
      if (targetUser == null || targetUser.Password != dto.Password)
        return Results.Unauthorized(); // 401: wrong credentials or no such user

      // 403: authorization (self or admin)
      if (!AuthRules.IsSelfOrAdmin(caller, dto.Username))
        return Results.StatusCode(StatusCodes.Status403Forbidden);

      // Attempt deletion
      var deleted = userStore.Remove(dto.Username);

      // 500: unexpected failure
      if (!deleted)
        return Results.StatusCode(StatusCodes.Status500InternalServerError);

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

    #region GET ALL USER STATUSES
    users.MapGet("/status", (HttpContext context) =>
    {
      // 401: authentication required
      if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller))
        return Results.Unauthorized();

      var statuses = userStore.GetAllUserStatuses();

      // 204: empty store
      if (!statuses.Any())
        return Results.NoContent();

      // Convert from tuple -> DTO
      var result = statuses.Select(s => new UserStatusDTO
      {
        Username = s.Username,
        Online = s.Online
      });

      return Results.Ok(result);
    })
    .Produces<IEnumerable<UserStatusDTO>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status401Unauthorized)
    .WithSummary("List All Users With Online/Offline Status")
    .WithDescription(
    "Returns all registered users along with their current online status. " +
    "Each entry includes the `Username` and a boolean `Online` value. " +
    "A user is considered online when their `LastActivityUtc` timestamp is within the configured `OnlineWindow`."
    )
    .WithBadge("Auth Required 🔐", BadgePosition.Before, "#ffec72");
    #endregion

    #region USER HEARTBEAT Check
    users.MapPost("/heartbeat", (HttpContext context) =>
    {
      if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller) || caller == null)
        return Results.Unauthorized();

      caller.LastSeenUtc = DateTime.UtcNow;
      return Results.Ok();
    });
    return users;
  }
  #endregion
}
