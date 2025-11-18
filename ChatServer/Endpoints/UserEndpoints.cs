using ChatServer.Store;
using ChatServer.Auth;
using Shared;
using Scalar.AspNetCore;
using ChatServer.Logger;

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
      {
        ServerLog.Warning("Unauthorized attempt to list users");
        return Results.Unauthorized();
      }

      var usernames = userStore.GetAllUsernames();

      // 500: unexpected storage failure
      if (usernames == null)
      {
        ServerLog.Error("User listing failed: storage returned null");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
      }

      // 204: no users
      if (!usernames.Any())
      {
        ServerLog.Info("User list requested, but store is empty");
        return Results.NoContent();
      }

      // 200: return list of usernames
      ServerLog.Info($"User list returned ({usernames.Count()} users)");
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
      {
        ServerLog.Warning("Unauthorized attempt to update user credentials");
        return Results.Unauthorized();
      }

      // 400: invalid input
      if (string.IsNullOrWhiteSpace(dto.OldUsername) || string.IsNullOrWhiteSpace(dto.NewUsername))
      {
        ServerLog.Warning("Credential update failed: missing username fields");
        return Results.BadRequest();
      }

      // 403: authorization (self or admin)
      if (!AuthRules.IsSelfOrAdmin(caller, dto.OldUsername))
      {
        ServerLog.Warning($"User '{caller.Username}' attempted unauthorized credential update on '{dto.OldUsername}'");
        return Results.StatusCode(StatusCodes.Status403Forbidden);
      }

      // Attempt update
      var updated = userStore.Update(dto.OldUsername, dto.NewUsername, dto.Password);


      // TODO: break this up so what the conflict issue is becomes clear
      // 409: conflict
      if (!updated)
      {
        ServerLog.Warning($"Credential update conflict for '{dto.OldUsername}' to '{dto.NewUsername}'");
        return Results.Conflict();
      }

      // 204: success
      ServerLog.Success($"User '{dto.OldUsername}' updated credentials to '{dto.NewUsername}'");
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
      {
        ServerLog.Warning("Unauthorized attempt to delete a user");
        return Results.Unauthorized();
      }

      // 400: invalid input
      if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
      {
        ServerLog.Warning("User deletion failed: missing username or password");
        return Results.BadRequest();
      }

      // Lookup target user
      var targetUser = userStore.GetByUsername(dto.Username);

      // 401: wrong credentials
      if (targetUser == null || targetUser.Password != dto.Password)
      {
        ServerLog.Warning($"User deletion failed for '{dto.Username}': invalid credentials");
        return Results.Unauthorized();
      }

      // 403: not self or admin
      if (!AuthRules.IsSelfOrAdmin(caller, dto.Username))
      {
        ServerLog.Warning($"User '{caller.Username}' attempted unauthorized deletion of '{dto.Username}'");
        return Results.StatusCode(StatusCodes.Status403Forbidden);
      }

      // Attempt deletion
      var deleted = userStore.Remove(dto.Username);

      if (!deleted)
      {
        ServerLog.Error($"User deletion for '{dto.Username}' failed unexpectedly");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
      }

      // 204: deletion successful, no content needed
      ServerLog.Success($"User '{dto.Username}' deleted");
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
      {
        ServerLog.Warning("Unauthorized attempt to list user statuses");
        return Results.Unauthorized();
      }

      var statuses = userStore.GetAllUserStatuses();

      // 204: empty status store
      if (!statuses.Any())
      {
        ServerLog.Info("User status list requested, but store is empty");
        return Results.NoContent();
      }

      ServerLog.Info($"User statuses returned ({statuses.Count()} entries)");

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

    #region USER HEARTBEAT CHECK
    users.MapPost("/heartbeat", (HttpContext context) =>
    {
      if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller) || caller == null)
      {
        ServerLog.Warning("Unauthorized heartbeat request");
        return Results.Unauthorized();
      }

      // This can be used to log heartbeat requests, but is commented out by default to reduce noise in logs.
      // ServerLog.Info($"Heartbeat received from '{caller.Username}'");

      caller.LastSeenUtc = DateTime.UtcNow;
      return Results.Ok();
    });
    return users;
  }
  #endregion
}
