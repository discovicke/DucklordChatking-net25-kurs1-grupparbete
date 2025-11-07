using Shared;
using ChatServer.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

int userID = 0;

// Dictionary keyed by Username
var users = new Dictionary<string, User>
{
    { "TestUser", new User { Id = 1, Password = "Password123" } }
};

app.MapGet("/", () => "Hello world");

app.Run();
