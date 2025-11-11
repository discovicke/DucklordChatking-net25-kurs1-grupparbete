using System;
using System.Net;
using System.Net.Http;
using ChatClient.Data;
using ChatClient.Tests.Helpers;
using Xunit;

public class MessageSenderTests
{
    [Fact]
    public void SendMessage_ReturnsTrue_OnSuccessResponse()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5201") };
        var sender = new MessageHandler(client);

        UserAccount.SetUser("Ducklord");
        try
        {
            bool result = sender.SendMessage("Hello world!");
            Assert.True(result);
        }
        finally
        {
            UserAccount.Clear();
        }
    }

    [Fact]
    public void SendMessage_ReturnsFalse_OnEmptyMessage()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5201") };
        var sender = new MessageHandler(client);

        UserAccount.SetUser("Ducklord");
        try
        {
            bool result = sender.SendMessage("");
            Assert.False(result);
        }
        finally
        {
            UserAccount.Clear();
        }
    }

    [Fact]
    public void SendMessage_ReturnsFalse_OnException()
    {
        var handler = new ThrowingHttpHandler();
        var client = new HttpClient(handler);
        var sender = new MessageHandler(client);

        UserAccount.SetUser("Ducklord");
        try
        {
            bool result = sender.SendMessage("Oops");
            Assert.False(result);
        }
        finally
        {
            UserAccount.Clear();
        }
    }
}