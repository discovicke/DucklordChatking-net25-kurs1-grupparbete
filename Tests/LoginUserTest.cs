using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ChatClient.Data;
using ChatClient.Tests.Helpers;
using Shared;
using Xunit;

public class LoginUserTests
{
    [Fact]
    public void Login_ReturnsTrue_OnSuccessResponse()
    {
        // Arrange
        var handler = new FakeHttpHandler(HttpStatusCode.OK);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5201") };
        var loginUser = new LoginUser(client);

        var user = new UserAccount { Username = "Ducklord", Password = "chatking" };

        // Act
        bool result = loginUser.Login(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Login_ReturnsFalse_OnBadRequest()
    {
        // Arrange
        var handler = new FakeHttpHandler(HttpStatusCode.BadRequest);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5201") };
        var loginUser = new LoginUser(client);

        var user = new UserAccount { Username = "Ducklord", Password = "wrong" };

        // Act
        bool result = loginUser.Login(user);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Login_ReturnsFalse_OnException()
    {
        // Arrange — handler som kastar exception
        var handler = new ThrowingHttpHandler();
        var client = new HttpClient(handler);
        var loginUser = new LoginUser(client);

        var user = new UserAccount { Username = "ErrorUser", Password = "nope" };

        // Act
        bool result = loginUser.Login(user);

        // Assert
        Assert.False(result);
    }
}
