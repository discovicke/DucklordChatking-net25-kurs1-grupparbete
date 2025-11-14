using Scalar.AspNetCore;

namespace ChatServer.Endpoints;

public static class SystemEndpoints
{
  public static RouteGroupBuilder MapSystemEndpoints(
      this IEndpointRouteBuilder app)
  {
    var system = app.MapGroup("/system").WithTags("System");

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

    return system;
  }
}
