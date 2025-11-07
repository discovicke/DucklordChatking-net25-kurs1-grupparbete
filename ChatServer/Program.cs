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


// Login
app.MapPost("/login", (LoginDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new { Message = "Username and password are required" });
  }

  if (users.TryGetValue(dto.Username, out var user) && user.Password == dto.Password)
  {
    return Results.Ok(new { UserID = user.Id, Message = "Login successful" });
  }

  return Results.BadRequest(new { Message = "Invalid username or password" });
});

// Registration
app.MapPost("/register", (LoginDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new { Message = "Username and password are required" });
  }

  // Check if username already exists
  if (users.ContainsKey(dto.Username))
  {
    return Results.BadRequest(new { Message = "Username already exists" });
  }

  // Create new userID
  userID++;

  users[dto.Username] = new User
  {
    Id = userID,
    Password = dto.Password
  };

  return Results.Ok(new { UserID = userID, Message = "Registration successful" });
});


// TODO: POST for message to chat

// TODO: GET all messages
app.Run();
