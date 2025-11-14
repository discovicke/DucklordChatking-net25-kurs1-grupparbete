using ChatServer.Store;
using ChatServer.Auth;
using ChatServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Shared;
using Scalar.AspNetCore;

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

    return messages;
  }
}
