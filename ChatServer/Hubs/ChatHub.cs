namespace ChatServer.Hubs;

using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
  public async Task SendMessage(string sender, string content)
  {
    // Broadcast to all connected clients
    await Clients.All.SendAsync("ReceiveMessage", sender, content);
  }
}
