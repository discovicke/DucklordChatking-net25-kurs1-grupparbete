using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ChatClient.Data;
using Shared;
using Xunit;

public class MessageSenderTests
{
    [Fact]
    public void SendMessage_ReturnsTrue_OnSuccessResponse()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5201") };
        var sender = new MessageSender(client);

        var message = new MessageDTO
        {
            Sender = "Ducklord",
            Content = "Hello world!",
            Timestamp = DateTime.UtcNow
        };

        bool result = sender.SendMessage(message);

        Assert.True(result);
    }

    [Fact]
    public void SendMessage_ReturnsFalse_OnEmptyMessage()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5201") };
        var sender = new MessageSender(client);

        var message = new MessageDTO
        {
            Sender = "Ducklord",
            Content = "", // tomt meddelande
            Timestamp = DateTime.UtcNow
        };

        bool result = sender.SendMessage(message);

        Assert.False(result);
    }

    [Fact]
    public void SendMessage_ReturnsFalse_OnException()
    {
        var handler = new ThrowingHttpHandler();
        var client = new HttpClient(handler);
        var sender = new MessageSender(client);

        var message = new MessageDTO
        {
            Sender = "Ducklord",
            Content = "Oops",
            Timestamp = DateTime.UtcNow
        };

        bool result = sender.SendMessage(message);

        Assert.False(result);
    }
}