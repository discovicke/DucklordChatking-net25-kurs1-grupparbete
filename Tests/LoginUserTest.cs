using System;
using System.Net;
using System.Net.Http;
using ChatClient.Data;
using ChatClient.Tests.Helpers;
using ChatServer.Models;
using Shared;
using Xunit;

public class LoginUserTests
{
    [Fact]
    public void Login_ReturnsTrue_OnSuccessResponse()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5201") };
        var loginUser = new UserAuth(client);

        var user = new UserDTO { Username = "Ducklord", Password = "chatking" };

        bool result = loginUser.Login(user);

        Assert.True(result);
    }

    [Fact]
    public void Login_ReturnsFalse_OnBadRequest()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5201") };
        var loginUser = new UserAuth(client);

        var user = new UserDTO { Username = "Ducklord", Password = "wrong" };

        bool result = loginUser.Login(user);

        Assert.False(result);
    }

    [Fact]
    public void Login_ReturnsFalse_OnException()
    {
        var handler = new ThrowingHttpHandler();
        var client = new HttpClient(handler);
        var loginUser = new UserAuth(client);

        var user = new UserDTO { Username = "ErrorUser", Password = "nope" };

        bool result = loginUser.Login(user);

        Assert.False(result);
    }
}