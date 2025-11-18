using ChatServer.Store;
using ChatServer.Auth;
using Shared;
using Scalar.AspNetCore;
using ChatServer.Logger;

namespace ChatServer.Endpoints;

public static class MessageEndpoints
{
  public static RouteGroupBuilder MapMessageEndpoints(
      this IEndpointRouteBuilder app,
      UserStore userStore,
      MessageStore messageStore)
  {
    var messages = app.MapGroup("/messages").WithTags("Messages");

    #region SEND MESSAGE
    messages.MapPost("/send", (HttpContext context, MessageDTO dto) =>
    {
      // 401: authentication required
      if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller) || caller == null)
      {
        ServerLog.Warning("Unauthorized attempt to send a message"); // TODO: could be improved to add more details, like token and IP address
        return Results.Unauthorized();
      }

      // 400: missing sender or content
      if (string.IsNullOrWhiteSpace(dto.Content) || string.IsNullOrWhiteSpace(dto.Sender))
      {
        ServerLog.Warning($"User '{caller.Username}' sent invalid message data (empty sender or content)");
        return Results.BadRequest();
      }

      // 403: sender must match authenticated user
      if (!AuthRules.IsSelf(caller, dto.Sender))
      {
        ServerLog.Warning($"User '{caller.Username}' attempted to send a message as '{dto.Sender}'");
        return Results.StatusCode(StatusCodes.Status403Forbidden);
      }

      // Attempt to store the message
      var added = messageStore.Add(dto.Sender, dto.Content);

      // 500: unexpected store failure
      if (!added)
      {
        ServerLog.Error(
            $"Message storage failed for user '{dto.Sender}'"
        );
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
      }

      ServerLog.Info($"Message stored from '{dto.Sender}'");

      // 204: success
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

    #region GET MESSAGE UPDATES (LONG POLLING)
    messages.MapGet("/updates", async (HttpContext context, int lastId) =>
    {
      // 401: authentication required
      if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller) || caller == null)
      {
        ServerLog.Warning("Unauthorized attempt to poll message updates");
        return Results.Unauthorized();
      }

      // 400: invalid lastId
      if (lastId < 0)
      {
        ServerLog.Warning($"User '{caller.Username}' sent invalid lastId value '{lastId}' in update request");
        return Results.BadRequest();
      }

      const int timeoutMs = 25000;     // total wait before returning empty
      const int sleepMs = 200;         // minimum wait between checks ( to avoid heavy load if server is busy )
      int waited = 0;

      // long polling loop
      while (waited < timeoutMs)
      {
        var updates = messageStore.GetMessagesAfter(lastId);

        if (updates.Count > 0)
        {
          // Map ChatMessage -> MessageDTO, including the ID
          var dtoUpdates = updates.Select(m => new MessageDTO
          {
            Id = m.Id,                     // copy the ID
            Sender = m.Sender,    // map sender username
            Content = m.Content,
            Timestamp = m.Timestamp
          }).ToList();

          // Optional lightweight info log:
          // ServerLog.Info($"Delivered {dtoUpdates.Count} updates to '{caller.Username}'");

          return Results.Ok(dtoUpdates); // 200: new messages available
        }

        await Task.Delay(sleepMs);
        waited += sleepMs;
      }

      // timeout reached, return empty list
      return Results.Ok(new List<MessageDTO>()); // 200: no new messages
    })
    .Produces<IEnumerable<MessageDTO>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .WithSummary("Get Message Updates (Long Polling)")
    .WithDescription(
        "Long-polling endpoint that waits for new messages after the given lastId. " +
        "Returns immediately if updates exist, otherwise waits up to 25 seconds before returning an empty list."
    )
    .WithBadge("Auth Required 🔐", BadgePosition.Before, "#ffec72");
    #endregion

    #region GET MESSAGE HISTORY (WITH OPTIONAL TAKE PARAMETER)
    messages.MapGet("/history", (HttpContext context, int? take) =>
    {
      // 401: authentication required
      if (!AuthUtils.TryAuthenticate(context.Request, userStore, out var caller) || caller == null)
      {
        ServerLog.Warning("Unauthorized attempt to fetch message history");
        return Results.Unauthorized();
      }

      // 400: invalid query parameter
      if (take.HasValue && take.Value <= 0)
      {
        ServerLog.Warning($"User '{caller.Username}' provided invalid take value '{take}'");
        return Results.BadRequest();
      }

      var messages = take.HasValue
          ? messageStore.GetLast(take.Value)
          : messageStore.GetAll();

      // 200: always OK, returns empty list if no messages exist
      ServerLog.Info($"Message history requested by '{caller.Username}' with {messages.Count} messages returned");
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
        ServerLog.Warning("Unauthorized attempt to clear message history");
        return Results.Unauthorized();
      }

      // 403: authorization, admin-only operation
      if (!caller.IsAdmin)
      {
        ServerLog.Warning($"User '{caller.Username}' attempted to clear message history without admin privileges");
        return Results.StatusCode(StatusCodes.Status403Forbidden);
      }

      // Attempt to clear all stored messages
      var cleared = messageStore.ClearAll();

      // 500: unexpected failure
      if (!cleared)
      {
        ServerLog.Error("Message history clear operation failed unexpectedly");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
      }

      // 204: success, no content
      ServerLog.Success($"Message history cleared by admin '{caller.Username}'");
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

    return messages;
  }
}
